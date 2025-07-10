using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using pet_spa_system1.Repository;
using pet_spa_system1.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register repositories and services
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISerCateRepository, SerCateRepository>();
builder.Services.AddScoped<ISerCateService, SerCateService>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAppointmentService, pet_spa_system1.Services.AppointmentService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
// Thêm Staff module
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IStaffService, StaffService>();

// Configure DbContext
builder.Services.AddDbContext<PetDataShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging() // Hữu ích để debug, chỉ dùng trong Development
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)); // Tối ưu hiệu suất

// Configure Session
builder.Services.AddSession();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Events.OnValidatePrincipal = context =>
    {
        // Xử lý validation session nếu cần
        return Task.CompletedTask;
    };
})
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
        if (string.IsNullOrEmpty(email))
        {
            Console.WriteLine("❌ Không lấy được email từ Google.");
            return Task.FromException(new Exception("Email không hợp lệ từ Google."));
        }
        identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, email));
        identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, name));
        Console.WriteLine($"🎯 Email nhận từ Google: {email}");
        Console.WriteLine($"🎯 Name nhận từ Google: {name}");
        return Task.CompletedTask;
    };

    options.CallbackPath = "/signin-google";
});

// Add other services
builder.Services.AddHttpContextAccessor();

// Add MVC services with validation
builder.Services.AddControllersWithViews()
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true;
    });

// Build the application
var app = builder.Build();

// Check Database connection
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
        Console.WriteLine($"❌ Error connecting to the database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Configure routing
app.MapControllerRoute(
    name: "Admin",
    pattern: "Admin/{action=Index}/{id?}",
    defaults: new { controller = "Admin" });

app.MapControllerRoute(
    name: "PetManagement",
    pattern: "Admin/Pets/{action}/{id?}",
    defaults: new { controller = "Admin", action = "Pets_List" });

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

app.MapControllerRoute(
    name: "Staff",
    pattern: "Staff/{action=Profile}/{id?}",
    defaults: new { controller = "Staff" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();