using System.Collections.Generic;

namespace pet_spa_system1.Models;

public partial class AppointmentServiceStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
}
