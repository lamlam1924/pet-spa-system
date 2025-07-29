namespace pet_spa_system1.ViewModel
{
    public class RealtimeShiftViewModel
    {
        public List<int> Hours { get; set; } = new();
        public List<StaffShift> StaffShifts { get; set; } = new();
    }

    public class StaffShift
    {
        public int UserId { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public Dictionary<int, ShiftStatus> HourStatus { get; set; } = new();
        public List<AppointmentViewModel> Appointments { get; set; }

    }

    public class ShiftStatus
    {
        public string StatusText { get; set; } = string.Empty;
        public string ColorClass { get; set; } = string.Empty;
        public string TimeRange { get; set; } = string.Empty;
        public int AppointmentId { get; set; }


    }

    public class AppointmentDragViewModel
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class UpdateStaffRequest
    {
        public int AppointmentId { get; set; }
        public int NewStaffId { get; set; }
    }
    public class ShiftUpdateRequest
    {
        public int AppointmentId { get; set; }
        public int NewStaffId { get; set; }
        public int NewHour { get; set; }
    }

}