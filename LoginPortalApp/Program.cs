using LoginPortalApp.Services;  // Service layer
using LoginPortal.Data;  // Database context from LoginPortal project
using Microsoft.EntityFrameworkCore;
using LoginPortal.Models;
using Microsoft.AspNetCore.Identity;  // For setting up DbContext
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the services with DI
builder.Services.AddScoped<LoginPortal.Services.IUserService, LoginPortal.Services.UserService>();
builder.Services.AddScoped<LoginPortal.Services.TokenService>();
builder.Services.AddScoped<LoginPortal.Services.IPasswordService, LoginPortal.Services.PasswordService>();
builder.Services.AddScoped<LoginPortal.Services.EmailService>();
builder.Services.AddScoped<UserService>();

// Register HttpClient for UserService (to be injected into the service)
builder.Services.AddHttpClient<UserService>(client =>
{
    // Specify the base address for HttpClient to be used in UserService
    client.BaseAddress = new Uri("http://localhost:8080");
});

// Register Identity services (UserManager, SignInManager, etc.)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container (MVC controllers and views)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

// Add authorization middleware
app.UseAuthorization();

// Map routes for controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Registration}/{action=Login}/{id?}");

// Custom route for Dashboard
app.MapControllerRoute(
    name: "dashboard",
    pattern: "Dashboard/{action=AdminDashboard}",
    defaults: new { controller = "Home" });

app.MapControllerRoute(
    name: "adminDashboard",
    pattern: "Dashboard/AdminDashboard",
    defaults: new { controller = "Dashboard", action = "AdminDashboard" });

app.MapControllerRoute(
    name: "userDashboard",
    pattern: "Dashboard/UserDashboard",
    defaults: new { controller = "Dashboard", action = "UserDashboard" });

// Run the app
app.Run();
