using System;
using System.Collections.Generic;

namespace PetShopDemo_1.Models;

public partial class BlogImage
{
    public int ImageId { get; set; }

    public int BlogId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? Caption { get; set; }

    public int? DisplayOrder { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Blog Blog { get; set; } = null!;
}
