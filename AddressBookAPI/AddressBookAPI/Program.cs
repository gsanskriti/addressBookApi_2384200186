using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using BusinessLayer.Interface;
using BusinessLayer.Service;
using System.Text;
using AddressBookAPI.BusinessLayer.Interface;
using AddressBookAPI.BusinessLayer.Service;
using AddressBookAPI.RepositoryLayer.Interface;
using AddressBookAPI.RepositoryLayer.Service;
using AddressBookAPI.Middleware;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Register Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis");
    return ConnectionMultiplexer.Connect(configuration);
});


// Connection String
var connectionString = "Server=localhost;Database=AddressBook;User Id=sa;Password=Sanskriti_3009;TrustServerCertificate=True;";

// Add Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Controllers
builder.Services.AddControllers();

// Configure Swagger (API Documentation)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AddressBookAPI", Version = "v1" });

    // Enable JWT Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

//  Configure JWT Authentication
var jwtSecret = "Rp7vkNchIefMfVbKShT2KZnZ6PlSTyEjnIvrijmn27s=";
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero // Immediate token expiration handling
        };
    });

// Register Dependency Injection (DI)
builder.Services.AddScoped<IUserBL, UserBL>();
builder.Services.AddScoped<IUserRL, UserRL>();

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();


// Build the App
var app = builder.Build();

// Use Exception Middleware
app.UseMiddleware<ExceptionMiddleware>();

// Enable Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
