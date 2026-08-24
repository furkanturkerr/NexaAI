using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NexaAI.Application.Features.Auth.Commands;
using NexaAI.Application.Interfaces.Services;
using NexaAI.Infrastructure.Authentication;
using NexaAI.Infrastructure.Identity;
using NexaAI.Infrastructure.Persistence.Context;
using NexaAI.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});


// Identity
builder.Services
    .AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
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

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();


// MediatR
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(
        typeof(RegisterCommand).Assembly);
});


// Controllers
builder.Services.AddControllers();


// Application → Infrastructure servis eşleşmeleri
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();


// Sıra önemli
app.UseAuthentication();
app.UseAuthorization();


// Controller endpointlerini açar
app.MapControllers();

app.Run();