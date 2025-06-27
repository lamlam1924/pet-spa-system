using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace pet_spa_system1.Models;

public partial class PetDataShopContext : DbContext
{
    public PetDataShopContext()
    {
    }

    public PetDataShopContext(DbContextOptions<PetDataShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentPet> AppointmentPets { get; set; }

    public virtual DbSet<AppointmentService> AppointmentServices { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<BlogImage> BlogImages { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }

    public virtual DbSet<Pet> Pets { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<PromotionProduct> PromotionProducts { get; set; }

    public virtual DbSet<PromotionService> PromotionServices { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SerCate> SerCates { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Species> Species { get; set; }

    public virtual DbSet<StatusAppointment> StatusAppointments { get; set; }

    public virtual DbSet<StatusOrder> StatusOrders { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
<<<<<<< HEAD
=======
        => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=PetDataShop;User Id=sa;Password=sa;TrustServerCertificate=true;");
>>>>>>> my-code

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
<<<<<<< HEAD
=======
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA22DA05403");
>>>>>>> my-code

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PromotionId).HasColumnName("PromotionID");
            entity.Property(e => e.StatusId)
                .HasDefaultValue(1)
                .HasColumnName("StatusID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Employee).WithMany(p => p.AppointmentEmployees)
                .HasForeignKey(d => d.EmployeeId)
<<<<<<< HEAD

            entity.HasOne(d => d.Promotion).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PromotionId)
=======
                .HasConstraintName("FK__Appointme__Emplo__03F0984C");

            entity.HasOne(d => d.Promotion).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PromotionId)
                .HasConstraintName("FK__Appointme__Promo__04E4BC85");
>>>>>>> my-code

            entity.HasOne(d => d.Status).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Appointme__Statu__05D8E0BE");
>>>>>>> my-code

            entity.HasOne(d => d.User).WithMany(p => p.AppointmentUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Appointme__UserI__02FC7413");
>>>>>>> my-code
        });

        modelBuilder.Entity<AppointmentPet>(entity =>
        {
<<<<<<< HEAD
=======
            entity.HasKey(e => e.AppointmentPetId).HasName("PK__Appointm__C8A4B64CA5161384");
>>>>>>> my-code

            entity.HasIndex(e => new { e.AppointmentId, e.PetId }, "UQ_AppointmentPet").IsUnique();

            entity.Property(e => e.AppointmentPetId).HasColumnName("AppointmentPetID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PetId).HasColumnName("PetID");

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentPets)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Appointme__Appoi__0A9D95DB");
>>>>>>> my-code

            entity.HasOne(d => d.Pet).WithMany(p => p.AppointmentPets)
                .HasForeignKey(d => d.PetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Appointme__PetID__0B91BA14");
>>>>>>> my-code
        });

        modelBuilder.Entity<AppointmentService>(entity =>
        {
<<<<<<< HEAD
=======
            entity.HasKey(e => e.AppointmentServiceId).HasName("PK__Appointm__3B38F27686CDAE1E");
>>>>>>> my-code

            entity.HasIndex(e => new { e.AppointmentId, e.ServiceId }, "UQ_AppointmentService").IsUnique();

            entity.Property(e => e.AppointmentServiceId).HasColumnName("AppointmentServiceID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentServices)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Appointme__Appoi__10566F31");
>>>>>>> my-code

            entity.HasOne(d => d.Service).WithMany(p => p.AppointmentServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Appointme__Servi__114A936A");
>>>>>>> my-code
        });

        modelBuilder.Entity<Blog>(entity =>
        {
<<<<<<< HEAD
=======
            entity.HasKey(e => e.BlogId).HasName("PK__Blogs__54379E50045C5929");
>>>>>>> my-code

            entity.Property(e => e.BlogId).HasColumnName("BlogID");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.ContentFormat)
                .HasMaxLength(20)
                .HasDefaultValue("Markdown");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.BlogApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
<<<<<<< HEAD

            entity.HasOne(d => d.User).WithMany(p => p.BlogUsers)
                .HasForeignKey(d => d.UserId)
=======
                .HasConstraintName("FK__Blogs__ApprovedB__4A8310C6");

            entity.HasOne(d => d.User).WithMany(p => p.BlogUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Blogs__UserID__498EEC8D");
>>>>>>> my-code
        });

        modelBuilder.Entity<BlogImage>(entity =>
        {
<<<<<<< HEAD
=======
            entity.HasKey(e => e.ImageId).HasName("PK__Blog_Ima__7516F4EC4ABC61C3");
>>>>>>> my-code

            entity.ToTable("Blog_Images");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.BlogId).HasColumnName("BlogID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogImages)
                .HasForeignKey(d => d.BlogId)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Blog_Imag__BlogI__503BEA1C");
>>>>>>> my-code
        });

        modelBuilder.Entity<Cart>(entity =>
        {
<<<<<<< HEAD
=======
            entity.HasKey(e => e.CartId).HasName("PK__Cart__51BCD797C23A3FBF");
>>>>>>> my-code

            entity.ToTable("Cart");

            entity.HasIndex(e => new { e.UserId, e.ProductId }, "UQ_Cart_User_Product").IsUnique();

            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
<<<<<<< HEAD

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
=======
                .HasConstraintName("FK__Cart__ProductID__282DF8C2");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Cart__UserID__2739D489");
>>>>>>> my-code
        });

        modelBuilder.Entity<Order>(entity =>
        {
<<<<<<< HEAD
=======
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF9C979A98");
>>>>>>> my-code

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.StatusId)
                .HasDefaultValue(1)
                .HasColumnName("StatusID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
=======
                .HasConstraintName("FK__Orders__StatusID__1CBC4616");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Orders__UserID__1BC821DD");
>>>>>>> my-code
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
<<<<<<< HEAD
=======
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED06A1F7864AC6");
>>>>>>> my-code

            entity.Property(e => e.OrderItemId).HasColumnName("OrderItemID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__OrderItem__Order__208CD6FA");
>>>>>>> my-code

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__OrderItem__Produ__2180FB33");
>>>>>>> my-code
        });

        modelBuilder.Entity<Payment>(entity =>
        {
<<<<<<< HEAD
=======
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A584387BC4F");
>>>>>>> my-code

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.PaymentStatusId)
                .HasDefaultValue(1)
                .HasColumnName("PaymentStatusID");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(100)
                .HasColumnName("TransactionID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Payments__OrderI__3587F3E0");
>>>>>>> my-code

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Payments__Paymen__37703C52");
>>>>>>> my-code

            entity.HasOne(d => d.PaymentStatus).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Payments__Paymen__3864608B");
>>>>>>> my-code

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Payments__UserID__367C1819");
>>>>>>> my-code
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
<<<<<<< HEAD

=======
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__DC31C1F30CECCE3B");

            entity.HasIndex(e => e.MethodName, "UQ__PaymentM__218CFB17FD4F2572").IsUnique();
>>>>>>> my-code

            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MethodName).HasMaxLength(20);
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
<<<<<<< HEAD

=======
            entity.HasKey(e => e.PaymentStatusId).HasName("PK__PaymentS__34F8AC1FCECE58A1");

            entity.HasIndex(e => e.StatusName, "UQ__PaymentS__05E7698A832691FF").IsUnique();
>>>>>>> my-code

            entity.Property(e => e.PaymentStatusId).HasColumnName("PaymentStatusID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.StatusName).HasMaxLength(20);
        });

        modelBuilder.Entity<Pet>(entity =>
        {
<<<<<<< HEAD
=======
            entity.HasKey(e => e.PetId).HasName("PK__Pets__48E53802587A1AF8");
>>>>>>> my-code

            entity.Property(e => e.PetId).HasColumnName("PetID");
            entity.Property(e => e.Breed).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.SpeciesId).HasColumnName("SpeciesID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Species).WithMany(p => p.Pets)
                .HasForeignKey(d => d.SpeciesId)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Pets__SpeciesID__5812160E");
>>>>>>> my-code

            entity.HasOne(d => d.User).WithMany(p => p.Pets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Pets__UserID__571DF1D5");
>>>>>>> my-code
        });

        modelBuilder.Entity<Product>(entity =>
        {
<<<<<<< HEAD
=======
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6ED27BA4023");
>>>>>>> my-code

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("ImageURL");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Products__Catego__4D94879B");
>>>>>>> my-code
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
<<<<<<< HEAD
=======
            entity.HasKey(e => e.CategoryId).HasName("PK__ProductC__19093A2BAA497997");
>>>>>>> my-code

            entity.HasIndex(e => new { e.Name, e.CateParent }, "UQ_ProductCategories_Name_CateParent").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CateParent).HasColumnName("Cate_parent");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.CateParentNavigation).WithMany(p => p.InverseCateParentNavigation)
                .HasForeignKey(d => d.CateParent)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__ProductCa__Cate___46E78A0C");
>>>>>>> my-code
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
<<<<<<< HEAD

=======
            entity.HasKey(e => e.PromotionId).HasName("PK__Promotio__52C42F2F0A55816F");

            entity.HasIndex(e => e.Code, "UQ__Promotio__A25C5AA76EBC4C02").IsUnique();
>>>>>>> my-code

            entity.Property(e => e.PromotionId).HasColumnName("PromotionID");
            entity.Property(e => e.ApplicableTo).HasMaxLength(20);
            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DiscountType).HasMaxLength(20);
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MinOrderValue).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<PromotionProduct>(entity =>
        {
<<<<<<< HEAD
=======
            entity.HasKey(e => e.PromotionProductId).HasName("PK__Promotio__C7B85D3CFD71DCA2");
>>>>>>> my-code

            entity.ToTable("Promotion_Products");

            entity.HasIndex(e => new { e.PromotionId, e.ProductId }, "UQ_PromotionProduct").IsUnique();

            entity.Property(e => e.PromotionProductId).HasColumnName("PromotionProductID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.PromotionId).HasColumnName("PromotionID");

            entity.HasOne(d => d.Product).WithMany(p => p.PromotionProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Promotion__Produ__72C60C4A");
>>>>>>> my-code

            entity.HasOne(d => d.Promotion).WithMany(p => p.PromotionProducts)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Promotion__Promo__71D1E811");
>>>>>>> my-code
        });

        modelBuilder.Entity<PromotionService>(entity =>
        {
<<<<<<< HEAD
=======
            entity.HasKey(e => e.PromotionServiceId).HasName("PK__Promotio__1F3D298900396B96");
>>>>>>> my-code

            entity.ToTable("Promotion_Services");

            entity.HasIndex(e => new { e.PromotionId, e.ServiceId }, "UQ_PromotionService").IsUnique();

            entity.Property(e => e.PromotionServiceId).HasColumnName("PromotionServiceID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PromotionId).HasColumnName("PromotionID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Promotion).WithMany(p => p.PromotionServices)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Promotion__Promo__778AC167");
>>>>>>> my-code

            entity.HasOne(d => d.Service).WithMany(p => p.PromotionServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Promotion__Servi__787EE5A0");
>>>>>>> my-code
        });

        modelBuilder.Entity<Review>(entity =>
        {
<<<<<<< HEAD
=======
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79AE6B2A7D40");
>>>>>>> my-code

            entity.Property(e => e.ReviewId).HasColumnName("ReviewID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Reviews__Product__40058253");
>>>>>>> my-code

            entity.HasOne(d => d.Service).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.Cascade)
<<<<<<< HEAD

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
=======
                .HasConstraintName("FK__Reviews__Service__40F9A68C");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Reviews__UserID__3F115E1A");
>>>>>>> my-code
        });

        modelBuilder.Entity<Role>(entity =>
        {
<<<<<<< HEAD

=======
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3AE6FA98A7");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B61603BB7A227").IsUnique();
>>>>>>> my-code

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<SerCate>(entity =>
        {
<<<<<<< HEAD

            entity.ToTable("Ser_cate");

=======
            entity.HasKey(e => e.CategoryId).HasName("PK__Ser_cate__19093A2BE1208FD3");

            entity.ToTable("Ser_cate");

            entity.HasIndex(e => e.Name, "UQ__Ser_cate__737584F69E113B95").IsUnique();
>>>>>>> my-code

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CateParent).HasColumnName("Cate_parent");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.CateParentNavigation).WithMany(p => p.InverseCateParentNavigation)
                .HasForeignKey(d => d.CateParent)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Ser_cate__Cate_p__5DCAEF64");
>>>>>>> my-code
        });

        modelBuilder.Entity<Service>(entity =>
        {
<<<<<<< HEAD
=======
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB0EAEA22CD20");
>>>>>>> my-code

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Services)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Services__Catego__6477ECF3");
>>>>>>> my-code
        });

        modelBuilder.Entity<Species>(entity =>
        {
<<<<<<< HEAD

=======
            entity.HasKey(e => e.SpeciesId).HasName("PK__Species__A938047F70A7F403");

            entity.HasIndex(e => e.SpeciesName, "UQ__Species__304D4C0D65B7148A").IsUnique();
>>>>>>> my-code

            entity.Property(e => e.SpeciesId).HasColumnName("SpeciesID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SpeciesName).HasMaxLength(50);
        });

        modelBuilder.Entity<StatusAppointment>(entity =>
        {
<<<<<<< HEAD

            entity.ToTable("Status_Appointment");

=======
            entity.HasKey(e => e.StatusId).HasName("PK__Status_A__C8EE2043B48AAE08");

            entity.ToTable("Status_Appointment");

            entity.HasIndex(e => e.StatusName, "UQ__Status_A__05E7698A7752840B").IsUnique();
>>>>>>> my-code

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.StatusName).HasMaxLength(20);
        });

        modelBuilder.Entity<StatusOrder>(entity =>
        {
<<<<<<< HEAD

            entity.ToTable("StatusOrder");

=======
            entity.HasKey(e => e.StatusId).HasName("PK__StatusOr__C8EE2043D6A07316");

            entity.ToTable("StatusOrder");

            entity.HasIndex(e => e.StatusName, "UQ__StatusOr__05E7698A22F3DC70").IsUnique();
>>>>>>> my-code

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.StatusName).HasMaxLength(20);
        });

        modelBuilder.Entity<User>(entity =>
        {
<<<<<<< HEAD

=======
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC05475DB9");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4F03942F9").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534DA24AF0C").IsUnique();
>>>>>>> my-code

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.ProfilePictureUrl).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
<<<<<<< HEAD
=======
                .HasConstraintName("FK__Users__RoleID__412EB0B6");
>>>>>>> my-code
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
