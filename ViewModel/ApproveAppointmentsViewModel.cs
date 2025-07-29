
using System.Collections.Generic;
using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModel
{
    public class ApproveAppointmentsViewModel
    {
        public List<AppointmentViewModel> PendingAppointments { get; set; } = new();
        public List<StaffViewModel> StaffList { get; set; } = new();
        public List<StatusAppointment> Statuses { get; set; } = new();
        public int? Page { get; set; } // Trang hiện tại cho phân trang
    }
}