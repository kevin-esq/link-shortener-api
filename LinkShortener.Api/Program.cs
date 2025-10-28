using LinkShortener.Api.Middleware;
using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Abstractions.Security;
using LinkShortener.Application.Abstractions.Services;
using LinkShortener.Application.Features.Url.Handlers;
using LinkShortener.Infrastructure;
using LinkShortener.Infrastructure.Repositories;
using LinkShortener.Infrastructure.Security;
using LinkShortener.Infrastructure.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
    
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LinkShortener API",
        Version = "v1",
        Description = "API for shortening links and managing users."
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the JWT token in the format: **Bearer {token}**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownProxies.Clear();
    options.KnownNetworks.Clear();
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")!)
);

var jwtConfig = builder.Configuration.GetSection("Jwt");
var issuer = jwtConfig["Issuer"] ?? throw new InvalidOperationException("Missing Jwt:Issuer");
var audience = jwtConfig["Audience"] ?? throw new InvalidOperationException("Missing Jwt:Audience");
var keysFolderConfig = jwtConfig["KeysFolder"] ?? throw new InvalidOperationException("Missing Jwt:KeysFolder");

var keysFolder = Path.GetFullPath(
    Path.Combine(builder.Environment.ContentRootPath, keysFolderConfig)
);

Console.WriteLine($"🔑 Searching for RSA keys in: {keysFolder}");

var privatePemPath = Path.Combine(keysFolder, "private.pem");
var publicPemPath = Path.Combine(keysFolder, "public.pem");

if (!File.Exists(privatePemPath) || !File.Exists(publicPemPath))
    throw new FileNotFoundException($"❌ RSA keys not found in folder: {keysFolder}");

var privatePem = File.ReadAllText(privatePemPath);
var publicPem = File.ReadAllText(publicPemPath);

var privateKey = RsaPemLoader.CreateSecurityKeyFromPrivatePem(privatePem);
var publicKey = RsaPemLoader.CreateSecurityKeyFromPublicPem(publicPem);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = publicKey
        };
    });

builder.Services.AddScoped<IUrlRepository, UrlRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

builder.Services.AddSingleton<IJwtService>(new JwtService(
    privateKey,
    issuer: issuer,
    audience: audience
));

builder.Services.AddSingleton<IJwtValidator>(new JwtValidator(new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = issuer,
    ValidAudience = audience,
    IssuerSigningKey = publicKey
}));

builder.Services.AddMemoryCache();

var emailConfig = builder.Configuration.GetSection("Email");
builder.Services.AddSingleton<IEmailService>(new EmailService(
    host: emailConfig["Host"]!,
    port: int.Parse(emailConfig["Port"]!),
    username: emailConfig["Username"]!,
    password: emailConfig["Password"]!,
    fromAddress: emailConfig["FromAddress"]!
));

builder.Services.AddSingleton<IVerificationCodeStore, VerificationCodeStore>();

var googleClientId = builder.Configuration["Google:ClientId"]
    ?? throw new InvalidOperationException("Missing Google:ClientId configuration");
builder.Services.AddSingleton<IGoogleAuthService>(sp =>
    new GoogleAuthService(googleClientId, sp.GetRequiredService<ILogger<GoogleAuthService>>()));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ShortenUrlCommandHandler).Assembly)
);

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LinkShortener API v1");
        c.DocumentTitle = "LinkShortener API Docs";
    });

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
