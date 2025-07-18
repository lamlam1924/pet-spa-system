namespace pet_spa_system1.ViewModel
{
    public class ProductWithRatingViewModel
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string? CategoryName { get; set; }

        // ⭐ Đánh giá
        public int AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();
    }
}