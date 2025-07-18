using System;
using System.Collections.Generic;

namespace pet_spa_system1.Models;

public partial class PetImage
{
    public int PetImageId { get; set; }

    public int PetId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Pet Pet { get; set; } = null!;
}
