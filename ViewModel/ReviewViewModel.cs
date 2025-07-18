namespace pet_spa_system1.ViewModel
{
    public class ReviewViewModel
    {
        public string ReviewerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}