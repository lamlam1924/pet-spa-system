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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {

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

            entity.HasOne(d => d.Promotion).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PromotionId)

            entity.HasOne(d => d.Status).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)

            entity.HasOne(d => d.User).WithMany(p => p.AppointmentUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
        });

        modelBuilder.Entity<AppointmentPet>(entity =>
        {

            entity.HasIndex(e => new { e.AppointmentId, e.PetId }, "UQ_AppointmentPet").IsUnique();

            entity.Property(e => e.AppointmentPetId).HasColumnName("AppointmentPetID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PetId).HasColumnName("PetID");

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentPets)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)

            entity.HasOne(d => d.Pet).WithMany(p => p.AppointmentPets)
                .HasForeignKey(d => d.PetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
        });

        modelBuilder.Entity<AppointmentService>(entity =>
        {

            entity.HasIndex(e => new { e.AppointmentId, e.ServiceId }, "UQ_AppointmentService").IsUnique();

            entity.Property(e => e.AppointmentServiceId).HasColumnName("AppointmentServiceID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentServices)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)

            entity.HasOne(d => d.Service).WithMany(p => p.AppointmentServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
        });

        modelBuilder.Entity<Blog>(entity =>
        {

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

            entity.HasOne(d => d.User).WithMany(p => p.BlogUsers)
                .HasForeignKey(d => d.UserId)
        });

        modelBuilder.Entity<BlogImage>(entity =>
        {

            entity.ToTable("Blog_Images");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.BlogId).HasColumnName("BlogID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogImages)
                .HasForeignKey(d => d.BlogId)
        });

        modelBuilder.Entity<Cart>(entity =>
        {

            entity.ToTable("Cart");

            entity.HasIndex(e => new { e.UserId, e.ProductId }, "UQ_Cart_User_Product").IsUnique();

            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
        });

        modelBuilder.Entity<Order>(entity =>
        {

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

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {

            entity.Property(e => e.OrderItemId).HasColumnName("OrderItemID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
        });

        modelBuilder.Entity<Payment>(entity =>
        {

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

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)

            entity.HasOne(d => d.PaymentStatus).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {


            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MethodName).HasMaxLength(20);
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {


            entity.Property(e => e.PaymentStatusId).HasColumnName("PaymentStatusID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.StatusName).HasMaxLength(20);
        });

        modelBuilder.Entity<Pet>(entity =>
        {

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

            entity.HasOne(d => d.User).WithMany(p => p.Pets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
        });

        modelBuilder.Entity<Product>(entity =>
        {

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
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {

            entity.HasIndex(e => new { e.Name, e.CateParent }, "UQ_ProductCategories_Name_CateParent").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CateParent).HasColumnName("Cate_parent");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.CateParentNavigation).WithMany(p => p.InverseCateParentNavigation)
                .HasForeignKey(d => d.CateParent)
        });

        modelBuilder.Entity<Promotion>(entity =>
        {


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

            entity.ToTable("Promotion_Products");

            entity.HasIndex(e => new { e.PromotionId, e.ProductId }, "UQ_PromotionProduct").IsUnique();

            entity.Property(e => e.PromotionProductId).HasColumnName("PromotionProductID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.PromotionId).HasColumnName("PromotionID");

            entity.HasOne(d => d.Product).WithMany(p => p.PromotionProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)

            entity.HasOne(d => d.Promotion).WithMany(p => p.PromotionProducts)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
        });

        modelBuilder.Entity<PromotionService>(entity =>
        {

            entity.ToTable("Promotion_Services");

            entity.HasIndex(e => new { e.PromotionId, e.ServiceId }, "UQ_PromotionService").IsUnique();

            entity.Property(e => e.PromotionServiceId).HasColumnName("PromotionServiceID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PromotionId).HasColumnName("PromotionID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Promotion).WithMany(p => p.PromotionServices)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.ClientSetNull)

            entity.HasOne(d => d.Service).WithMany(p => p.PromotionServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
        });

        modelBuilder.Entity<Review>(entity =>
        {

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

            entity.HasOne(d => d.Service).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.Cascade)

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
        });

        modelBuilder.Entity<Role>(entity =>
        {


            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<SerCate>(entity =>
        {

            entity.ToTable("Ser_cate");


            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CateParent).HasColumnName("Cate_parent");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.CateParentNavigation).WithMany(p => p.InverseCateParentNavigation)
                .HasForeignKey(d => d.CateParent)
        });

        modelBuilder.Entity<Service>(entity =>
        {

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Services)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
        });

        modelBuilder.Entity<Species>(entity =>
        {


            entity.Property(e => e.SpeciesId).HasColumnName("SpeciesID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SpeciesName).HasMaxLength(50);
        });

        modelBuilder.Entity<StatusAppointment>(entity =>
        {

            entity.ToTable("Status_Appointment");


            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.StatusName).HasMaxLength(20);
        });

        modelBuilder.Entity<StatusOrder>(entity =>
        {

            entity.ToTable("StatusOrder");


            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.StatusName).HasMaxLength(20);
        });

        modelBuilder.Entity<User>(entity =>
        {


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
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
