using System;
using System.Collections.Generic;

namespace PetShopDemo_1.Models;

public partial class PromotionService
{
    public int PromotionServiceId { get; set; }

    public int PromotionId { get; set; }

    public int ServiceId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Promotion Promotion { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
