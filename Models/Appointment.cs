using System;
using System.Collections.Generic;

namespace pet_spa_system1.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int UserId { get; set; }

    public int? EmployeeId { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public int StatusId { get; set; }
    

    public string? Notes { get; set; }

    public int? PromotionId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual ICollection<AppointmentPet> AppointmentPets { get; set; } = new List<AppointmentPet>();

    public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();

    public virtual User? Employee { get; set; }

    public virtual Promotion? Promotion { get; set; }

    public virtual StatusAppointment Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
