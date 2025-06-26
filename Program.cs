using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using pet_spa_system1.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// ✅ Dependency Injection
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// ✅ Cấu hình DbContext (chỉ giữ 1 dòng)
builder.Services.AddDbContext<PetDataShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Cấu hình Session (gộp lại 1 lần)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Cấu hình MVC
builder.Services.AddControllersWithViews();

// ✅ Cấu hình Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    options.CallbackPath = "/signin-google";
});

var app = builder.Build();

// ✅ Test kết nối database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PetDataShopContext>();
    try
    {
        if (db.Database.CanConnect())
        {
            Console.WriteLine("✅ Database connection successful.");
        }
        else
        {
            Console.WriteLine("❌ Database connection failed.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error connecting to the database: " + ex.Message);
    }
}

// ✅ Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ➕ Bổ sung StaticFiles để phục vụ JS/CSS
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Đảm bảo authentication + session + authorization đúng thứ tự
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// ✅ Cấu hình định tuyến Controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
