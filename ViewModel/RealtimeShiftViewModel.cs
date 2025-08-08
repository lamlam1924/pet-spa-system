namespace pet_spa_system1.ViewModel
{
    public class RealtimeShiftViewModel
    {
        public List<int> Hours { get; set; } = new();
        public List<StaffShift> StaffShifts { get; set; } = new();
        public DateTime SelectedDate { get; set; }
    }

    public class StaffShift
    {
        public int UserId { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public Dictionary<int, ShiftStatus> HourStatus { get; set; } = new();
        public List<AppointmentViewModel> Appointments { get; set; } = new();
    }

    public class ShiftStatus
    {
        public string StatusText { get; set; } = string.Empty;
        public string ColorClass { get; set; } = string.Empty;
        public string TimeRange { get; set; } = string.Empty;
        public int AppointmentId { get; set; }
    }

    public class MoveAppointmentRequest
    {
        public int AppointmentId { get; set; }
        public int PetId { get; set; }
        public int NewStaffId { get; set; }
        public bool CheckOnly { get; set; }
    }

    public class MoveResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
    }
}