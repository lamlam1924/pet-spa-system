using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly PetDataShopContext _context;
        public AppointmentRepository(PetDataShopContext context)
        {
            _context = context;
        }

        public int AddAppointment(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            _context.SaveChanges(); // Cập nhật AppointmentId
            return appointment.AppointmentId;
        }

        public void AddAppointmentPet(int appointmentId, int petId)
        {
            _context.AppointmentPets.Add(new AppointmentPet
            {
                AppointmentId = appointmentId,
                PetId = petId
            });
        }

        public void AddAppointmentService(int appointmentId, int serviceId)
        {
            _context.AppointmentServices.Add(new AppointmentService
            {
                AppointmentId = appointmentId,
                ServiceId = serviceId
            });
        }

        public Appointment? GetById(int id)
        {
            return _context.Appointments
                .Include(a => a.AppointmentServices).ThenInclude(s => s.Service)
                .Include(a => a.AppointmentPets).ThenInclude(p => p.Pet)
                .Include(a => a.Status)
                .SingleOrDefault(a => a.AppointmentId == id);
        }



        public List<Appointment> GetByUserId(int userId)
        {
            return _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.AppointmentServices).ThenInclude(s => s.Service)
                .Include(a => a.AppointmentPets).ThenInclude(p => p.Pet)
                .Include(a => a.Status)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }

}

