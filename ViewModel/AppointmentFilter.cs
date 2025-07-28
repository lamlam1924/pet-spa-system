namespace pet_spa_system1.ViewModel
{
    public class AppointmentFilter
    {
        public string Customer { get; set; } = string.Empty;
        public string Pet { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public int StatusId { get; set; } = 0;
        public DateTime? Date { get; set; }
        public int EmployeeId { get; set; } = 0;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
