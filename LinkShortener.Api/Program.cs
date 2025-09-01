using LinkShortener.Infrastructure;
using LinkShortener.Infrastructure.Services;
using LinkShortener.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!));

builder.Services.AddScoped<IUrlShorteningAppService, UrlShorteningAppService>();
builder.Services.AddScoped<UrlShorteningService>();

builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
