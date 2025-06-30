using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories;

public interface IAppointmentRepository
{
    int AddAppointment(Appointment appointment); // trả về id lịch mới
    void AddAppointmentPet(int appointmentId, int petId);
    void AddAppointmentService(int appointmentId, int serviceId);
    List<Appointment> GetByUserId(int userId);
    Appointment GetById(int id);
    void Save();
}