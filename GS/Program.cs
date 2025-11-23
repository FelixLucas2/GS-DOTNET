// Program.cs
using GS.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===== Serilog =====
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Host.UseSerilog();

// ===== Services =====
builder.Services.AddControllers();

// ===== EF Core - Oracle =====
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ===== API Versioning =====
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

// ===== Health Checks =====
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("OK"));

// ===== Authentication (JWT) =====
var jwtKey = builder.Configuration["Jwt:Key"];
if (!string.IsNullOrEmpty(jwtKey))
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // ajustar para produção
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
}

builder.Services.AddAuthorization();

// ===== Read ApiKey from config =====
var apiKeyFromConfig = builder.Configuration["ApiKey"];

// ===== Build app =====
var app = builder.Build();

// Serve arquivos estáticos (wwwroot)
app.UseStaticFiles();

// Serilog request logging
app.UseSerilogRequestLogging();

// Simple API Key middleware:
// - Se header X-API-KEY presente e bate com config, injeta um ClaimsPrincipal para autorizar endpoints com [Authorize]
if (!string.IsNullOrEmpty(apiKeyFromConfig))
{
    app.Use(async (context, next) =>
    {
        if (context.Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
        {
            if (!string.IsNullOrEmpty(extractedApiKey) && extractedApiKey == apiKeyFromConfig)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "ApiKeyUser"), new Claim("auth_method", "ApiKey") };
                var identity = new ClaimsIdentity(claims, "ApiKey");
                context.User = new ClaimsPrincipal(identity);
            }
        }
        await next();
    });
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ===== Minimal endpoint to generate JWT for testing =====
app.MapPost("/auth/token", (IConfiguration config, TokenRequest req) =>
{
    var secret = config["Jwt:Key"];
    if (string.IsNullOrEmpty(secret))
        return Results.Problem("Jwt:Key not configured");

    // In real app validate user credentials here (req.Username / req.Password)
    // For demo purpose we issue token for any request that contains username
    var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, req.Username ?? "dev-user"),
        new Claim("role", "User")
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: null,
        audience: null,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(8),
        signingCredentials: creds
    );

    var jwt = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new { token = jwt, expires = DateTime.UtcNow.AddHours(8) });
})
.WithName("GenerateToken")
.Accepts<TokenRequest>("application/json")
.Produces(200);

// ===== Health endpoints =====
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");

// Map controllers
app.MapControllers();

app.Run();

// Helper minimal types
public partial class Program { }

public record TokenRequest(string Username, string Password);