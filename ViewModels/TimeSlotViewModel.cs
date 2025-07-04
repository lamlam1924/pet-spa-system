namespace pet_spa_system1.ViewModels
{
    public class TimeSlotViewModel
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public int AvailableSpots { get; set; }
    }
} 