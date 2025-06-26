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
        => optionsBuilder.UseSqlServer("Server=DESKTOP-1TUBGEU\\SQLEXPRESS;Database=PetDataShop;User Id=sa;Password=123456;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA279B43F2A");

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
                .HasConstraintName("FK__Appointme__Emplo__160F4887");

            entity.HasOne(d => d.Promotion).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PromotionId)
                .HasConstraintName("FK__Appointme__Promo__17036CC0");

            entity.HasOne(d => d.Status).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Statu__17F790F9");

            entity.HasOne(d => d.User).WithMany(p => p.AppointmentUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__UserI__151B244E");
        });

        modelBuilder.Entity<AppointmentPet>(entity =>
        {
            entity.HasKey(e => e.AppointmentPetId).HasName("PK__Appointm__C8A4B64C84F2A401");

            entity.HasIndex(e => new { e.AppointmentId, e.PetId }, "UQ_AppointmentPet").IsUnique();

            entity.Property(e => e.AppointmentPetId).HasColumnName("AppointmentPetID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PetId).HasColumnName("PetID");

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentPets)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Appoi__1CBC4616");

            entity.HasOne(d => d.Pet).WithMany(p => p.AppointmentPets)
                .HasForeignKey(d => d.PetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__PetID__1DB06A4F");
        });

        modelBuilder.Entity<AppointmentService>(entity =>
        {
            entity.HasKey(e => e.AppointmentServiceId).HasName("PK__Appointm__3B38F2768F606EA9");

            entity.HasIndex(e => new { e.AppointmentId, e.ServiceId }, "UQ_AppointmentService").IsUnique();

            entity.Property(e => e.AppointmentServiceId).HasColumnName("AppointmentServiceID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentServices)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Appoi__22751F6C");

            entity.HasOne(d => d.Service).WithMany(p => p.AppointmentServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Servi__236943A5");
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blogs__54379E505924001A");

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
                .HasConstraintName("FK__Blogs__ApprovedB__5CA1C101");

            entity.HasOne(d => d.User).WithMany(p => p.BlogUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Blogs__UserID__5BAD9CC8");
        });

        modelBuilder.Entity<BlogImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Blog_Ima__7516F4EC378872E4");

            entity.ToTable("Blog_Images");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.BlogId).HasColumnName("BlogID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogImages)
                .HasForeignKey(d => d.BlogId)
                .HasConstraintName("FK__Blog_Imag__BlogI__625A9A57");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Cart__51BCD79799F15E64");

            entity.ToTable("Cart");

            entity.HasIndex(e => new { e.UserId, e.ProductId }, "UQ_Cart_User_Product").IsUnique();

            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Cart__ProductID__3A4CA8FD");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Cart__UserID__395884C4");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF418E10A0");

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
                .HasConstraintName("FK__Orders__StatusID__2EDAF651");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Orders__UserID__2DE6D218");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED06A19CDD2E07");

            entity.Property(e => e.OrderItemId).HasColumnName("OrderItemID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderItem__Order__32AB8735");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Produ__339FAB6E");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A5823F8EFF3");

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
                .HasConstraintName("FK__Payments__OrderI__47A6A41B");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Paymen__498EEC8D");

            entity.HasOne(d => d.PaymentStatus).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Paymen__4A8310C6");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__UserID__489AC854");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__DC31C1F3D52DE6D6");

            entity.HasIndex(e => e.MethodName, "UQ__PaymentM__218CFB174BF6DC27").IsUnique();

            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MethodName).HasMaxLength(20);
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.HasKey(e => e.PaymentStatusId).HasName("PK__PaymentS__34F8AC1F0032CB84");

            entity.HasIndex(e => e.StatusName, "UQ__PaymentS__05E7698A5558441E").IsUnique();

            entity.Property(e => e.PaymentStatusId).HasColumnName("PaymentStatusID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.StatusName).HasMaxLength(20);
        });

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.HasKey(e => e.PetId).HasName("PK__Pets__48E53802EE36633B");

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
                .HasConstraintName("FK__Pets__SpeciesID__6A30C649");

            entity.HasOne(d => d.User).WithMany(p => p.Pets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pets__UserID__693CA210");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6EDB8594779");

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
                .HasConstraintName("FK__Products__Catego__5FB337D6");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__ProductC__19093A2B4ADD39AB");

            entity.HasIndex(e => new { e.Name, e.CateParent }, "UQ_ProductCategories_Name_CateParent").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CateParent).HasColumnName("Cate_parent");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.CateParentNavigation).WithMany(p => p.InverseCateParentNavigation)
                .HasForeignKey(d => d.CateParent)
                .HasConstraintName("FK__ProductCa__Cate___59063A47");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("PK__Promotio__52C42F2F05C434EF");

            entity.HasIndex(e => e.Code, "UQ__Promotio__A25C5AA7D53801F3").IsUnique();

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
            entity.HasKey(e => e.PromotionProductId).HasName("PK__Promotio__C7B85D3CFB49D038");

            entity.ToTable("Promotion_Products");

            entity.HasIndex(e => new { e.PromotionId, e.ProductId }, "UQ_PromotionProduct").IsUnique();

            entity.Property(e => e.PromotionProductId).HasColumnName("PromotionProductID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.PromotionId).HasColumnName("PromotionID");

            entity.HasOne(d => d.Product).WithMany(p => p.PromotionProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Promotion__Produ__04E4BC85");

            entity.HasOne(d => d.Promotion).WithMany(p => p.PromotionProducts)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Promotion__Promo__03F0984C");
        });

        modelBuilder.Entity<PromotionService>(entity =>
        {
            entity.HasKey(e => e.PromotionServiceId).HasName("PK__Promotio__1F3D298998EA52F1");

            entity.ToTable("Promotion_Services");

            entity.HasIndex(e => new { e.PromotionId, e.ServiceId }, "UQ_PromotionService").IsUnique();

            entity.Property(e => e.PromotionServiceId).HasColumnName("PromotionServiceID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PromotionId).HasColumnName("PromotionID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Promotion).WithMany(p => p.PromotionServices)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Promotion__Promo__09A971A2");

            entity.HasOne(d => d.Service).WithMany(p => p.PromotionServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Promotion__Servi__0A9D95DB");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79AE5280E067");

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
                .HasConstraintName("FK__Reviews__Product__5224328E");

            entity.HasOne(d => d.Service).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Reviews__Service__531856C7");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Reviews__UserID__51300E55");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3AD5440245");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160E08B0E50").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<SerCate>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Ser_cate__19093A2B53D801C2");

            entity.ToTable("Ser_cate");

            entity.HasIndex(e => e.Name, "UQ__Ser_cate__737584F62407E36A").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CateParent).HasColumnName("Cate_parent");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.CateParentNavigation).WithMany(p => p.InverseCateParentNavigation)
                .HasForeignKey(d => d.CateParent)
                .HasConstraintName("FK__Ser_cate__Cate_p__6FE99F9F");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB0EA66339A24");

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Services)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Services__Catego__76969D2E");
        });

        modelBuilder.Entity<Species>(entity =>
        {
            entity.HasKey(e => e.SpeciesId).HasName("PK__Species__A938047FD22F1912");

            entity.HasIndex(e => e.SpeciesName, "UQ__Species__304D4C0D3AB68A6D").IsUnique();

            entity.Property(e => e.SpeciesId).HasColumnName("SpeciesID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SpeciesName).HasMaxLength(50);
        });

        modelBuilder.Entity<StatusAppointment>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Status_A__C8EE20437D8AC019");

            entity.ToTable("Status_Appointment");

            entity.HasIndex(e => e.StatusName, "UQ__Status_A__05E7698A5C3FD41F").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.StatusName).HasMaxLength(20);
        });

        modelBuilder.Entity<StatusOrder>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__StatusOr__C8EE2043A9C63E97");

            entity.ToTable("StatusOrder");

            entity.HasIndex(e => e.StatusName, "UQ__StatusOr__05E7698A286D32A1").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.StatusName).HasMaxLength(20);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC8E60288C");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E43B839247").IsUnique();
            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534838E41F3").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.ProfilePictureUrl).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleID__534D60F1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}