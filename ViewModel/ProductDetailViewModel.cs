using pet_spa_system1.ViewModel;

namespace pet_spa_system1.ViewModel
{
    public class ProductDetailViewModel
    {
        public ProductWithRatingViewModel ProductWithRating { get; set; } = null!;
        public List<ProductWithRatingViewModel> SuggestedProducts { get; set; } = new();
    }
}
