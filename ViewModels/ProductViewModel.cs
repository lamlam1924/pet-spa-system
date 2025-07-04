using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; } = new();
        public List<Product> SuggestedProducts { get; set; } = new();
        public List<ProductCategory> Categories { get; set; } = new();
    }

}
