using System.Text.Json;
using System.Text.Json.Serialization;
using CareerPath.Data;
using CareerPath.Data.Repository;
using CareerPath.DataAccess;
using CareerPath.DataAccess.Repository;
using CareerPath.DataAccess.Repository.IRepository;
using CareerPath.Models;
using CareerPath.Services;
using CareerPath.Services.IServices;
using CareerPath.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------
// DATABASE
// ---------------------------------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention()
);

// ---------------------------------------------------------------
// IDENTITY — uses Razor Pages for Login/Register/Logout UI
// ---------------------------------------------------------------
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// tell Identity where the Razor Pages login/logout pages live
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath       = "/Identity/Account/Login";
    options.LogoutPath      = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan  = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;

    
});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromMinutes(30);
});

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        // ✅ Prevents circular reference crashes globally
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// ---------------------------------------------------------------
// MVC + RAZOR PAGES
// both needed — MVC for your controllers, Razor Pages for Identity
// ---------------------------------------------------------------
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // handles snake_case JSON from Python API responses
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    });

builder.Services.AddRazorPages(); // required for Identity scaffolded UI

// ---------------------------------------------------------------
// HTTP CLIENT — Python FastAPI backend
// ---------------------------------------------------------------
builder.Services.AddHttpClient("PythonApi", client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["PythonApi:BaseUrl"]
                                     ?? "https://lakshya-api.onrender.com");
        client.Timeout = TimeSpan.FromSeconds(120);
    })
    .AddTransientHttpErrorPolicy(p => 
        p.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(5)));


// ---------------------------------------------------------------
// APPLICATION SERVICES
// ---------------------------------------------------------------
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<CareerDetailService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IPythonApiServices, PythonApiServices>();
builder.Services.AddScoped<ICareerDetailServices, CareerDetailService>();

// add more services here as you build them:
// builder.Services.AddScoped<IPythonApiService, PythonApiService>();

// ---------------------------------------------------------------
// PIPELINE
// ---------------------------------------------------------------
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

// order matters — Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

// maps Identity Razor Pages (/Identity/Account/Login etc)
app.MapRazorPages();

// maps your MVC controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{area=student}/{controller=Home}/{action=Index}/{id?}"
);

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Migration failed: {Message}", ex.Message);
        throw;
    }
}

app.Run();