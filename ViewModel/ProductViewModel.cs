using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModel
{
    public class ProductViewModel
    {
        public Product Product { get; set; } = new Product();
        public List<Product> SuggestedProducts { get; set; } = new List<Product>();
        public List<ProductCategory> Categories { get; set; } = new List<ProductCategory>();

    }


}
