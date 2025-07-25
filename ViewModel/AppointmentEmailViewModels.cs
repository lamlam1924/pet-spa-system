using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModel
{
    public class AppointmentCancelledEmailModel
    {
        public string ToEmail { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime AppointmentDateTime { get; set; }
    }

    public class AppointmentReminderEmailModel
    {
        public string ToEmail { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime AppointmentDateTime { get; set; }
    }

    public class AppointmentConfirmationEmailModel
    {
        public string ToEmail { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime AppointmentDateTime { get; set; }
        public DateTime EndTime { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public List<Pet> SelectedPets { get; set; } = new List<Pet>();
        public List<Service> SelectedServices { get; set; } = new List<Service>();
    }

    public class AppointmentRejectedEmailModel
    {
        public string ToEmail { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime ProposedDateTime { get; set; }
    }
    

    public class ApproveAssignRequest
    {
        public int AppointmentId { get; set; }
        public int StaffId { get; set; }
    }

    public class AutoAssignRequest
    {
        public int AppointmentId { get; set; }
    }

    public class CheckConflictRequest
    {
        public DateTime AppointmentDate { get; set; }
        public int StaffId { get; set; }
        public int DurationMinutes { get; set; }
    }

}