using System;
using System.Collections.Generic;

namespace PetShopDemo_1.Models;

public partial class ProductCategory
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public int? CateParent { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ProductCategory? CateParentNavigation { get; set; }

    public virtual ICollection<ProductCategory> InverseCateParentNavigation { get; set; } = new List<ProductCategory>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
