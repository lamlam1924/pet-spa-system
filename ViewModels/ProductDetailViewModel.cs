using System.Collections.Generic;
using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModels
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public List<Product> SuggestedProducts { get; set; }
    }
}
