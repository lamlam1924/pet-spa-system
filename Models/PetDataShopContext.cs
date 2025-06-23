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
        => optionsBuilder.UseSqlServer("Server=localhost;Database=PetDataShop;User Id=sa;Password=123;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA2876D4698");

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
                .HasConstraintName("FK__Appointme__Emplo__03F0984C");

            entity.HasOne(d => d.Promotion).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PromotionId)
                .HasConstraintName("FK__Appointme__Promo__04E4BC85");

            entity.HasOne(d => d.Status).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Statu__05D8E0BE");

            entity.HasOne(d => d.User).WithMany(p => p.AppointmentUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__UserI__02FC7413");
        });

        modelBuilder.Entity<AppointmentPet>(entity =>
        {
            entity.HasKey(e => e.AppointmentPetId).HasName("PK__Appointm__C8A4B64CFAF72409");

            entity.HasIndex(e => new { e.AppointmentId, e.PetId }, "UQ_AppointmentPet").IsUnique();

            entity.Property(e => e.AppointmentPetId).HasColumnName("AppointmentPetID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PetId).HasColumnName("PetID");

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentPets)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Appoi__0A9D95DB");

            entity.HasOne(d => d.Pet).WithMany(p => p.AppointmentPets)
                .HasForeignKey(d => d.PetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__PetID__0B91BA14");
        });

        modelBuilder.Entity<AppointmentService>(entity =>
        {
            entity.HasKey(e => e.AppointmentServiceId).HasName("PK__Appointm__3B38F2760B6166EE");

            entity.HasIndex(e => new { e.AppointmentId, e.ServiceId }, "UQ_AppointmentService").IsUnique();

            entity.Property(e => e.AppointmentServiceId).HasColumnName("AppointmentServiceID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentServices)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Appoi__10566F31");

            entity.HasOne(d => d.Service).WithMany(p => p.AppointmentServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Servi__114A936A");
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blogs__54379E5046FA9547");

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
                .HasConstraintName("FK__Blogs__ApprovedB__4A8310C6");

            entity.HasOne(d => d.User).WithMany(p => p.BlogUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Blogs__UserID__498EEC8D");
        });

        modelBuilder.Entity<BlogImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Blog_Ima__7516F4ECCEF2E644");

            entity.ToTable("Blog_Images");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.BlogId).HasColumnName("BlogID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogImages)
                .HasForeignKey(d => d.BlogId)
                .HasConstraintName("FK__Blog_Imag__BlogI__503BEA1C");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Cart__51BCD7973B81DA89");

            entity.ToTable("Cart");

            entity.HasIndex(e => new { e.UserId, e.ProductId }, "UQ_Cart_User_Product").IsUnique();

            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Cart__ProductID__282DF8C2");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Cart__UserID__2739D489");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAFE07F64C7");

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
                .HasConstraintName("FK__Orders__StatusID__1CBC4616");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Orders__UserID__1BC821DD");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED06A1410CF407");

            entity.Property(e => e.OrderItemId).HasColumnName("OrderItemID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderItem__Order__208CD6FA");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Produ__2180FB33");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A5846E0455B");

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
                .HasConstraintName("FK__Payments__OrderI__3587F3E0");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Paymen__37703C52");

            entity.HasOne(d => d.PaymentStatus).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Paymen__3864608B");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__UserID__367C1819");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__DC31C1F36F0C2734");

            entity.HasIndex(e => e.MethodName, "UQ__PaymentM__218CFB17D5BBB0A3").IsUnique();

            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MethodName).HasMaxLength(20);
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.HasKey(e => e.PaymentStatusId).HasName("PK__PaymentS__34F8AC1F1A8BBAB7");

            entity.HasIndex(e => e.StatusName, "UQ__PaymentS__05E7698AAB508E6D").IsUnique();

            entity.Property(e => e.PaymentStatusId).HasColumnName("PaymentStatusID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.StatusName).HasMaxLength(20);
        });

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.HasKey(e => e.PetId).HasName("PK__Pets__48E5380242D74943");

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
                .HasConstraintName("FK__Pets__SpeciesID__5812160E");

            entity.HasOne(d => d.User).WithMany(p => p.Pets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pets__UserID__571DF1D5");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6ED7D630B09");

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
                .HasConstraintName("FK__Products__Catego__4D94879B");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__ProductC__19093A2B6AC43CAF");

            entity.HasIndex(e => new { e.Name, e.CateParent }, "UQ_ProductCategories_Name_CateParent").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CateParent).HasColumnName("Cate_parent");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.CateParentNavigation).WithMany(p => p.InverseCateParentNavigation)
                .HasForeignKey(d => d.CateParent)
                .HasConstraintName("FK__ProductCa__Cate___46E78A0C");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("PK__Promotio__52C42F2FCD778EC9");

            entity.HasIndex(e => e.Code, "UQ__Promotio__A25C5AA78C3BE73F").IsUnique();

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
            entity.HasKey(e => e.PromotionProductId).HasName("PK__Promotio__C7B85D3C78BA0F3A");

            entity.ToTable("Promotion_Products");

            entity.HasIndex(e => new { e.PromotionId, e.ProductId }, "UQ_PromotionProduct").IsUnique();

            entity.Property(e => e.PromotionProductId).HasColumnName("PromotionProductID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.PromotionId).HasColumnName("PromotionID");

            entity.HasOne(d => d.Product).WithMany(p => p.PromotionProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Promotion__Produ__72C60C4A");

            entity.HasOne(d => d.Promotion).WithMany(p => p.PromotionProducts)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Promotion__Promo__71D1E811");
        });

        modelBuilder.Entity<PromotionService>(entity =>
        {
            entity.HasKey(e => e.PromotionServiceId).HasName("PK__Promotio__1F3D298906F5FA5D");

            entity.ToTable("Promotion_Services");

            entity.HasIndex(e => new { e.PromotionId, e.ServiceId }, "UQ_PromotionService").IsUnique();

            entity.Property(e => e.PromotionServiceId).HasColumnName("PromotionServiceID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PromotionId).HasColumnName("PromotionID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Promotion).WithMany(p => p.PromotionServices)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Promotion__Promo__778AC167");

            entity.HasOne(d => d.Service).WithMany(p => p.PromotionServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Promotion__Servi__787EE5A0");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79AEAD3A37C1");

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
                .HasConstraintName("FK__Reviews__Product__40058253");

            entity.HasOne(d => d.Service).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Reviews__Service__40F9A68C");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Reviews__UserID__3F115E1A");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3AE4A1C1D4");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B61609BDFF5BA").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<SerCate>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Ser_cate__19093A2B0EC0A9A1");

            entity.ToTable("Ser_cate");

            entity.HasIndex(e => e.Name, "UQ__Ser_cate__737584F69A43B685").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CateParent).HasColumnName("Cate_parent");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.CateParentNavigation).WithMany(p => p.InverseCateParentNavigation)
                .HasForeignKey(d => d.CateParent)
                .HasConstraintName("FK__Ser_cate__Cate_p__5DCAEF64");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB0EA93BFB7D0");

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Services)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Services__Catego__6477ECF3");
        });

        modelBuilder.Entity<Species>(entity =>
        {
            entity.HasKey(e => e.SpeciesId).HasName("PK__Species__A938047FE23D0C46");

            entity.HasIndex(e => e.SpeciesName, "UQ__Species__304D4C0DC87AFF30").IsUnique();

            entity.Property(e => e.SpeciesId).HasColumnName("SpeciesID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SpeciesName).HasMaxLength(50);
        });

        modelBuilder.Entity<StatusAppointment>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Status_A__C8EE2043F5EC7EFA");

            entity.ToTable("Status_Appointment");

            entity.HasIndex(e => e.StatusName, "UQ__Status_A__05E7698AD1AA59F7").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.StatusName).HasMaxLength(20);
        });

        modelBuilder.Entity<StatusOrder>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__StatusOr__C8EE20435677C935");

            entity.ToTable("StatusOrder");

            entity.HasIndex(e => e.StatusName, "UQ__StatusOr__05E7698A00532D03").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.StatusName).HasMaxLength(20);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACD659248B");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E484E61D78").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534065E5C2A").IsUnique();

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
                .HasConstraintName("FK__Users__RoleID__412EB0B6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
