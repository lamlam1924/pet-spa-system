using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using pet_spa_system1.Services;
using System;
using AppointmentService = pet_spa_system1.Services.AppointmentService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add database context
builder.Services.AddDbContext<PetDataShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<ISerCateRepository, SerCateRepository>();
builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

// Register services
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<ISerCateService, SerCateService>();
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
// builder.Services.AddScoped<UserService>();
// builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Authentication
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
    options.Scope.Add("profile");
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


// Check DB connection
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PetDataShopContext>();
    try
    {
        if (db.Database.CanConnect())
            Console.WriteLine("✅ Database connection successful.");
        else
            Console.WriteLine("❌ Database connection failed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error connecting to the database: " + ex.Message);
    }
}

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Route config
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "ProductManagement",
    pattern: "Products/{action}/{id?}",
    defaults: new { controller = "Products", action = "Shop" });

app.MapControllerRoute(
    name: "productDetail",
    pattern: "Admin/Product_Detail/{productID}",
    defaults: new { controller = "Products", action = "Product_Detail" });

app.MapControllerRoute(
    name: "Detail",
    pattern: "Products/Detail/{productID}",
    defaults: new { controller = "Products", action = "Detail" });

app.Run();
