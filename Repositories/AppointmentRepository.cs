
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pet_spa_system1.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly PetDataShopContext _context;
        public AppointmentRepository(PetDataShopContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Cập nhật thông tin lịch hẹn (bao gồm trạng thái)
        /// </summary>
        /// <param name="appointment"></param>
        public void Update(Appointment appointment)
        {
            var tracked = _context.Appointments.Find(appointment.AppointmentId);
            if (tracked != null)
            {
                _context.Entry(tracked).CurrentValues.SetValues(appointment);
            }
        }

        // Lấy tất cả dịch vụ (bao gồm cả không active)
        public List<Service> GetAllServices()
        {
            return _context.Services.ToList();
        }

        public int AddAppointment(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            _context.SaveChanges();
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
                return;
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

        public void Save()
        {
            _context.SaveChanges();
        }

        public List<Appointment> GetByUserIdWithDetail(int userId)
        {
            var result = _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.AppointmentServices).ThenInclude(asv => asv.Service)
                .Include(a => a.AppointmentPets).ThenInclude(ap => ap.Pet)
                .Include(a => a.Status)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();

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
        
        public int CountAppointmentsByDate(DateTime date)
        {
            return _context.Appointments
                .Where(a => a.AppointmentDate.Date == date.Date)
                .Count();
        }

        public int CountUpcomingAppointments(DateTime fromDate)
        {
            return _context.Appointments
                .Where(a => a.AppointmentDate.Date > fromDate.Date && a.StatusId != 4)
                .Count();
        }

        public int CountAppointmentsByStatus(int statusId)
        {
            return _context.Appointments
                .Where(a => a.StatusId == statusId)
                .Count();
        }
        
        public List<Appointment> GetAppointments(
            string searchTerm = "", 
            int statusId = 0, 
            DateTime? date = null, 
            int employeeId = 0, 
            int page = 1, 
            int pageSize = 10)
        {
            var query = _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Status)
                .Include(a => a.Employee)
                .Include(a => a.AppointmentPets)
                    .ThenInclude(ap => ap.Pet)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(asr => asr.Service)
                .AsQueryable();
                
            query = ApplyFilters(query, searchTerm, statusId, date, employeeId);
            
            return query
                .OrderByDescending(a => a.AppointmentDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
        
        public int CountAppointments(
            string searchTerm = "", 
            int statusId = 0, 
            DateTime? date = null, 
            int employeeId = 0)
        {
            var query = _context.Appointments.AsQueryable();
            query = ApplyFilters(query, searchTerm, statusId, date, employeeId);
            return query.Count();
        }
        
        private IQueryable<Appointment> ApplyFilters(
            IQueryable<Appointment> query,
            string searchTerm = "",
            int statusId = 0,
            DateTime? date = null,
            int employeeId = 0)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => 
                    a.User.FullName.Contains(searchTerm) || 
                    a.User.Phone.Contains(searchTerm) ||
                    a.AppointmentPets.Any(ap => ap.Pet.Name.Contains(searchTerm)));
            }
            
            if (statusId > 0)
            {
                query = query.Where(a => a.StatusId == statusId);
            }
            
            if (date.HasValue)
            {
                query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);
            }
            
            if (employeeId > 0)
            {
                query = query.Where(a => a.EmployeeId == employeeId);
            }
            
            return query;
        }
        
        public List<Appointment> GetAppointmentsByDateRange(DateTime start, DateTime end)
        {
            return _context.Appointments
                .Include(a => a.Status)
                .Include(a => a.User)
                .Include(a => a.AppointmentPets)
                    .ThenInclude(ap => ap.Pet)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(as_ => as_.Service)
                .Where(a => a.AppointmentDate >= start && a.AppointmentDate <= end)
                .ToList();
        }
        
        public Appointment GetAppointmentWithDetails(int id)
        {
            return _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Status)
                .Include(a => a.Employee)
                .Include(a => a.AppointmentPets)
                    .ThenInclude(ap => ap.Pet)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(as_ => as_.Service)
                    .ThenInclude(s => s.Category)
                .FirstOrDefault(a => a.AppointmentId == id);
        }

        public void Add(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
        }



        public void Delete(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }
        }

        public void DeleteAppointmentPets(int appointmentId)
        {
            var appointmentPets = _context.AppointmentPets
                .Where(ap => ap.AppointmentId == appointmentId);
            _context.AppointmentPets.RemoveRange(appointmentPets);
        }

        public void DeleteAppointmentServices(int appointmentId)
        {
            var appointmentServices = _context.AppointmentServices
                .Where(ap => ap.AppointmentId == appointmentId);
            _context.AppointmentServices.RemoveRange(appointmentServices);
        }

        public List<MonthlyAppointmentStats> GetMonthlyStats(int year)
        {
            return _context.Appointments
                .Where(a => a.AppointmentDate.Year == year)
                .GroupBy(a => new { a.AppointmentDate.Year, a.AppointmentDate.Month })
                .Select(g => new MonthlyAppointmentStats
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalAppointments = g.Count(),
                    CompletedAppointments = g.Count(a => a.StatusId == 4),
                    CancelledAppointments = g.Count(a => a.StatusId == 5)
                })
                .OrderBy(s => s.Year)
                .ThenBy(s => s.Month)
                .ToList();
        }

        public List<Service> GetActiveServices()
        {
            return _context.Services
                .Where(s => s.IsActive.HasValue && s.IsActive.Value)
                .ToList();
        }

        public List<User> GetCustomers()
        {
            return _context.Users
                .Where(u => u.RoleId == 3)
                .ToList();
        }

        public List<User> GetEmployeeUsers()
        {
            return _context.Users
                .Where(u => u.RoleId == 2)
                .ToList();
        }

        public List<Pet> GetAllPets()
        {
            return _context.Pets.ToList();
        }
    }
}
