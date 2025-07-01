using pet_spa_system1.Models;

namespace pet_spa_system1.Services
{
    public interface IAppointmentService
    {
        bool SaveAppointment(AppointmentViewModel vm, int userId);
        AppointmentHistoryViewModel GetAppointmentHistory(int userId);
        Appointment GetAppointmentById(int appointmentId);
    }
}

