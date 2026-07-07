using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using ReportCanvas.Application;
using ReportCanvas.Application.Common.Interfaces;
using ReportCanvas.Api.Services;
using ReportCanvas.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ─── Controllers ────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// ─── Application + Infrastructure ───────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ─── Current User ────────────────────────────────────────────────────────────
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// ─── JWT Authentication ───────────────────────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("Jwt:Secret is not configured.");

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

// ─── CORS (Angular dev server) ────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ─── Swagger / OpenAPI ────────────────────────────────────────────────────────
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info.Title = "ReportCanvas API";
        document.Info.Version = "v1";
        document.Info.Description = "Backend API for the ReportCanvas SaaS";
        return Task.CompletedTask;
    });
});

// ─── Build ────────────────────────────────────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "ReportCanvas API";
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseCors("Angular");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
