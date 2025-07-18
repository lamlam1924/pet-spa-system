using System;
using System.Collections.Generic;

namespace pet_spa_system1.Models;

public partial class AppointmentPet
{
    public int AppointmentPetId { get; set; }

    public int AppointmentId { get; set; }

    public int PetId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Pet Pet { get; set; } = null!;
}