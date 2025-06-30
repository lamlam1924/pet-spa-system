using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using pet_spa_system1.Services;
using System;

var builder = WebApplication.CreateBuilder(args);



// ✅ Dependency Injection
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// ✅ Cấu hình DbContext
builder.Services.AddDbContext<PetDataShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Cấu hình Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



// Add services to the container.
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
    options.Scope.Add("email");
    options.Scope.Add("profile"); // thêm profile để lấy tên đầy đủ
    options.SaveTokens = true;

    options.Events.OnCreatingTicket = context =>
    {
        var identity = context.Identity;

        var email = context.User.GetProperty("email").GetString();
        var name = context.User.GetProperty("name").GetString();

        identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, email));
        identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, name));

        Console.WriteLine("🎯 Email nhận từ Google: " + email);
        Console.WriteLine("🎯 Name nhận từ Google: " + name);
        return Task.CompletedTask;
    };

    options.CallbackPath = "/signin-google";
});

var app = builder.Build();

// ✅ Kiểm tra kết nối Database

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

app.UseHttpsRedirection();

// ✅ Phục vụ file tĩnh (CSS/JS/images...)
app.UseStaticFiles();

app.UseRouting();

// ✅ Đảm bảo thứ tự đúng
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// ✅ Định tuyến Controller

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
