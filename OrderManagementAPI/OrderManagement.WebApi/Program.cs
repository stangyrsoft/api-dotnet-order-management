using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderManagement.Application;
using OrderManagement.Application.Security;
using OrderManagement.Domain.Models;
using OrderManagement.Infrastructure;
using OrderManagement.WebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT settings are missing.");
if (string.IsNullOrWhiteSpace(jwtOptions.Key) || string.IsNullOrWhiteSpace(jwtOptions.Issuer) ||
    string.IsNullOrWhiteSpace(jwtOptions.Audience) || jwtOptions.AccessTokenMinutes <= 0)
{
    throw new InvalidOperationException("JWT key, issuer, audience, and a positive token lifetime must be configured.");
}

byte[] jwtKeyBytes;
try
{
    jwtKeyBytes = Convert.FromBase64String(jwtOptions.Key);
}
catch (FormatException exception)
{
    throw new InvalidOperationException("JWT key must be a Base64-encoded random key.", exception);
}

if (jwtKeyBytes.Length < 32)
{
    throw new InvalidOperationException("JWT key must contain at least 256 bits.");
}

var signingKey = new SymmetricSecurityKey(jwtKeyBytes);
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddSingleton<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddScoped<IPasswordVerifier, AspNetIdentityPasswordVerifier>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });
builder.Services.AddAuthorization();
const string FrontendCorsPolicy = "Frontend";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order Management API",
        Version = "v1"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega el token JWT. Swagger agrega automáticamente el prefijo Bearer."
    });
    options.OperationFilter<AuthorizeCheckOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors(FrontendCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health/database");
app.MapControllers();

app.Run();
