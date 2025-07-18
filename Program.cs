using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.Repo;
using pet_spa_system1.Repositories;
using pet_spa_system1.Repository;
using pet_spa_system1.Services;

var builder = WebApplication.CreateBuilder(args);

// Đọc cấu hình từ appsettings.json

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true); // thêm dòng này

var smtpPass = builder.Configuration["Smtp:Pass"];

// ======= SERVICES =======
builder.Services.AddControllersWithViews();
// Đăng ký IHttpContextAccessor cho DI container
builder.Services.AddHttpContextAccessor();

// Kết nối DB từ appsettings.json
builder.Services.AddDbContext<PetDataShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI cho Repository và Service
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
builder.Services.AddScoped<IAppointmentServiceRepository, AppointmentServiceRepository>();

builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<ICheckoutRepository, CheckoutRepository>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<ISpeciesService, SpeciesService>();
builder.Services.AddScoped<ISpeciesRepository, SpeciesRepository>();
// Blog services
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IOrderStatusRepository, OrderStatusRepository>();
builder.Services.AddScoped<IOrderStatusService, OrderStatusService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAdminStaffScheduleService, AdminStaffScheduleService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Đăng ký ICloudinaryService
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// Cấu hình Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cấu hình Authentication Google + Cookie
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

        Console.WriteLine("🎯 Email From Google: " + email);
        Console.WriteLine("🎯 Name From Google: " + name);

        return Task.CompletedTask;
    };

    options.CallbackPath = "/signin-google";
});

var app = builder.Build();

// ======= MIDDLEWARE =======

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 💡 Thứ tự quan trọng
app.UseSession();             // ✅ Session phải trước Authentication
app.UseAuthentication();      // ✅ Dùng xác thực
app.UseAuthorization();       // ✅ Dùng phân quyền

// ✅ Kiểm tra kết nối DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PetDataShopContext>();
    try
    {
        if (db.Database.CanConnect())
        {
            Console.WriteLine("✅ Database connected.");
        }
        else
        {
            Console.WriteLine("❌ Database connection failed.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error: " + ex.Message);
    }
}

// ======= ROUTING =======
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

app.MapControllerRoute(
    name: "Admin",
    pattern: "Admin/{action=Index}/{id?}",
    defaults: new { controller = "Admin" });
// Blog routes
app.MapControllerRoute(
    name: "BlogDetail",
    pattern: "Blogs/Detail/{id:int}",
    defaults: new { controller = "Blogs", action = "Detail" });

app.MapControllerRoute(
    name: "BlogCreate",
    pattern: "Blogs/Create",
    defaults: new { controller = "Blogs", action = "Create" });

app.Run();