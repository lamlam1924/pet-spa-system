namespace pet_spa_system1.ViewModel
{
    public class AppointmentFilter
    {
        public string Customer { get; set; } = string.Empty;
        public string Pet { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public List<int> StatusIds { get; set; } = new List<int>();
        public int? StatusId { get; set; } // Cho phép filter đơn lẻ
        public DateTime? Date { get; set; }
        public int? StaffId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
