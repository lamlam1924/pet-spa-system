namespace pet_spa_system1.ViewModel
{
    public class ReviewViewModel
    {
        public string ReviewerName { get; set; }
        public string UserAvatar { get; set; } // Đường dẫn avatar user
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public int Id { get; set; } // ReviewId thực tế
        public int? ParentReviewId { get; set; }
        public List<ReviewViewModel> Replies { get; set; } = new List<ReviewViewModel>();
    }
}