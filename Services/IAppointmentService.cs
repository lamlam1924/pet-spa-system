using pet_spa_system1.Models;
using pet_spa_system1.ViewModels;

namespace pet_spa_system1.Services
{
    public interface IAppointmentService
    {
        bool SaveAppointment(AppointmentViewModel vm, int userId);
        AppointmentHistoryViewModel GetAppointmentHistory(int userId);
        Appointment GetAppointmentById(int appointmentId);
        List<string> GetPetNames(List<int> petIds);
        List<string> GetServiceNames(List<int> serviceIds);
    }
}