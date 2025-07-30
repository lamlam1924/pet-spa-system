using System;
using System.Collections.Generic;

namespace pet_spa_system1.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public int RoleId { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Appointment> AppointmentEmployees { get; set; } = new List<Appointment>();

    public virtual ICollection<AppointmentPet> AppointmentPets { get; set; } = new List<AppointmentPet>();

    public virtual ICollection<Appointment> AppointmentUsers { get; set; } = new List<Appointment>();

    public virtual ICollection<Blog> BlogApprovedByNavigations { get; set; } = new List<Blog>();

    public virtual ICollection<BlogComment> BlogComments { get; set; } = new List<BlogComment>();

    public virtual ICollection<BlogLike> BlogLikes { get; set; } = new List<BlogLike>();

    public virtual ICollection<Blog> BlogUsers { get; set; } = new List<Blog>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role Role { get; set; } = null!;
}
