using LoginPortal.Data;
using LoginPortal.Models;
using LoginPortal.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using System.Text;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    logger.Info("Application Starting...");

    // Create builder for WebApplication
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    // Add CORS in Startup/Program.cs
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigin",
            builder => builder.WithOrigins("https://localhost:8081") // Your UI address
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials());
    });



    // Registering custom services with Dependency Injection
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IPasswordService, PasswordService>();  // Assumed PasswordService implements IPasswordService
    builder.Services.AddScoped<TokenService>();  // TokenService can be used for generating JWT tokens
    builder.Services.AddScoped<EmailService>();  // EmailService for handling email functionality

    // Configure JWT Bearer Authentication
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // Set to true in production for secure communication
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)) // Ensure SecretKey is stored securely
            };
        });

    // Add Controllers
    builder.Services.AddControllers();

    // Set up Swagger for API documentation
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Configure NLog as the logging provider
    builder.Logging.ClearProviders(); // Clear other logging providers
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information); // Set log level
    builder.Host.UseNLog(); // Use NLog for logging

    var app = builder.Build();

    // Set up the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger(); // Enable Swagger in development environment
        app.UseSwaggerUI(); // Swagger UI
    }

    // Add CORS to the pipeline
    app.UseCors("AllowSpecificOrigin");
    app.UseHttpsRedirection(); // Enforce HTTPS
    app.UseAuthentication(); // Enable authentication
    app.UseAuthorization(); // Enable authorization
    app.MapControllers(); // Map API controllers
    app.Run(); // Start the application
}
catch (Exception ex)
{
    // Log error during application startup
    logger.Error(ex, "An error occurred during application startup.");
    throw;
}
finally
{
    NLog.LogManager.Shutdown(); // Ensure proper flushing and closing of log files
}
