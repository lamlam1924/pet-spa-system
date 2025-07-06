using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace pet_spa_system1.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int UserId { get; set; }

    public int? EmployeeId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public int StatusId { get; set; }

    public string? Notes { get; set; }

    public int? PromotionId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<AppointmentPet> AppointmentPets { get; set; } = new List<AppointmentPet>();

    public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();

    [ValidateNever]
    public virtual User? Employee { get; set; }

    [ValidateNever]
    public virtual Promotion? Promotion { get; set; }

    [ValidateNever]
    public virtual StatusAppointment Status { get; set; } = null!;

    [ValidateNever]
    public virtual User User { get; set; } = null!;
}
