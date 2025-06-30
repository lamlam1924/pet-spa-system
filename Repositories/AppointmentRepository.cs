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
            var existingService = _context.Services.Find(serviceId);
            if (existingService == null)
            {
                Console.WriteLine($"WARNING: Không tìm thấy dịch vụ ID {serviceId}");
                return; // Không thêm nếu không tìm thấy dịch vụ
            }
            
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



        // public List<Appointment> GetByUserId(int userId)
        // {
        //     return _context.Appointments
        //         .Where(a => a.UserId == userId)
        //         .Include(a => a.AppointmentServices).ThenInclude(s => s.Service)
        //         .Include(a => a.AppointmentPets).ThenInclude(p => p.Pet)
        //         .Include(a => a.Status)
        //         .OrderByDescending(a => a.AppointmentDate)
        //         .ToList();
        // }

        public void Save()
        {
            _context.SaveChanges();
        }

        public List<Appointment> GetByUserIdWithDetail(int userId)
        {
            var result= _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.AppointmentServices).ThenInclude(asv => asv.Service)
                .Include(a => a.AppointmentPets).ThenInclude(ap => ap.Pet)
                .Include(a => a.Status)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();

                // DEBUG: In ra số lượng service và pet của mỗi appointment
                foreach (var appointment in result)
                {
                    Console.WriteLine($"AppointmentId: {appointment.AppointmentId}");
                    Console.WriteLine($"Services count: {appointment.AppointmentServices?.Count ?? 0}");
                    Console.WriteLine($"Pets count: {appointment.AppointmentPets?.Count ?? 0}");
                    
                    if (appointment.AppointmentServices != null)
                    {
                        foreach (var svc in appointment.AppointmentServices)
                        {
                            Console.WriteLine($"  Service Id: {svc.ServiceId}, Name: {svc.Service?.Name ?? "NULL"}");
                        }
                    }
                }
                
                return result;
                
        }

        public List<StatusAppointment> GetAllStatuses()
        {
            return _context.StatusAppointments.ToList();
        }
        
        // Trong AppointmentRepository
        public List<string> GetPetNamesByIds(List<int> petIds)
        {
            if (petIds == null || petIds.Count == 0) return new List<string>();
            return _context.Pets
                .Where(p => petIds.Contains(p.PetId))
                .Select(p => p.Name)
                .ToList();
        }
        public List<string> GetServiceNamesByIds(List<int> serviceIds)
        {
            if (serviceIds == null || serviceIds.Count == 0) return new List<string>();
            return _context.Services
                .Where(s => serviceIds.Contains(s.ServiceId))
                .Select(s => s.Name)
                .ToList();
        }

    }

}

