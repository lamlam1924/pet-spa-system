﻿using System;
using System.Collections.Generic;

namespace pet_spa_system1.Models;

public partial class Species
{
    public int SpeciesId { get; set; }

    public string SpeciesName { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
}
