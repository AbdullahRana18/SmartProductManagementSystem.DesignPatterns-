using Microsoft.EntityFrameworkCore;
using SmartProductManagementSystem.Data;
using SmartProductManagementSystem.DesignPatterns.Structural.Facade; // Facade Namespace Zaroori hai

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. Session Service Enable ki (Undo feature ke liye zaroori hai)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 30 min baad session expire
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 2. Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Register Facade for Dependency Injection (Professional Way - "new" nahi use karengy controller mai)
builder.Services.AddScoped<AdminDashboardFacade>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Static Assets fix

app.UseRouting();

app.UseAuthorization();

// 4. Use Session Middleware (Ye Routing ke baad ana chahiye)
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Dashboard}/{id?}");

app.Run();