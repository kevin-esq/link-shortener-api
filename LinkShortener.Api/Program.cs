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

// Redis connection (optional - for analytics caching)
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnectionString))
{
    try
    {
        builder.Services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(
            StackExchange.Redis.ConnectionMultiplexer.Connect(redisConnectionString));
        Console.WriteLine("✅ Redis connected successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Redis connection failed: {ex.Message}. Analytics caching will be disabled.");
        builder.Services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(sp => null!);
    }
}
else
{
    Console.WriteLine("ℹ️ Redis not configured. Analytics caching will be disabled.");
    builder.Services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(sp => null!);
}

var jwtConfig = builder.Configuration.GetSection("Jwt");
var issuer = jwtConfig["Issuer"] ?? throw new InvalidOperationException("Missing Jwt:Issuer");
var audience = jwtConfig["Audience"] ?? throw new InvalidOperationException("Missing Jwt:Audience");
var keysFolderConfig = jwtConfig["KeysFolder"] ?? throw new InvalidOperationException("Missing Jwt:KeysFolder");

var keysFolder = Path.IsPathRooted(keysFolderConfig)
    ? keysFolderConfig
    : Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, keysFolderConfig));

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

// Repository registrations
builder.Services.AddScoped<IUrlRepository, UrlRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
builder.Services.AddScoped<ILinkStatsRepository, LinkStatsRepository>();

// Security and hashing
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

// Analytics services (LinkPulse)
builder.Services.AddSingleton<IUserAgentParser, UserAgentParserService>();
builder.Services.AddScoped<IGeolocationService, GeolocationService>();
builder.Services.AddSingleton<IAnalyticsCacheService, AnalyticsCacheService>();

// Audit and Metrics services
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IClickEventService, ClickEventService>();
builder.Services.AddSingleton<IQrCodeService, QrCodeService>();

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

// Background services
builder.Services.AddHostedService<AnalyticsBackgroundService>();
builder.Services.AddHostedService<MetricsAggregationService>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<AuditMiddleware>();

// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LinkShortener API v1");
    c.DocumentTitle = "LinkShortener API Docs";
});

// Auto-migrate database in development
if (app.Environment.IsDevelopment())
{
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
