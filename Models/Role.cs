﻿using System;
using System.Collections.Generic;

namespace pet_spa_system1.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
