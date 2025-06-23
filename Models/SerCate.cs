using System;
using System.Collections.Generic;

namespace pet_spa_system1.Models;

public partial class SerCate
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? CateParent { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual SerCate? CateParentNavigation { get; set; }

    public virtual ICollection<SerCate> InverseCateParentNavigation { get; set; } = new List<SerCate>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
