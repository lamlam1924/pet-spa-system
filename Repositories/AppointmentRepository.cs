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
                .Include(a => a.User)
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
            // Chỉ đếm các lịch đã xác nhận (StatusId == 2)
            return _context.Appointments
                .Where(a => a.AppointmentDate.Date == date.Date && a.StatusId == 2)
                .Count();
        }

        public int CountUpcomingAppointments(DateTime fromDate)
        {
            // Chỉ đếm các lịch đã xác nhận (StatusId == 2) và ngày lớn hơn hôm nay
            return _context.Appointments
                .Where(a => a.AppointmentDate.Date > fromDate.Date && a.StatusId == 2)
                .Count();
        }

        public int CountAppointmentsByStatus(int statusId)
        {
            return _context.Appointments
                .Where(a => a.StatusId == statusId)
                .Count();
        }

        public List<Appointment> GetAppointments(ViewModel.AppointmentFilter filter)
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

            query = ApplyFilters(query, filter);

            return query
                .OrderByDescending(a => a.AppointmentDate)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();
        }

        public int CountAppointments(ViewModel.AppointmentFilter filter)
        {
            var query = _context.Appointments.AsQueryable();
            query = ApplyFilters(query, filter);
            return query.Count();
        }

        private IQueryable<Appointment> ApplyFilters(IQueryable<Appointment> query, ViewModel.AppointmentFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Customer))
            {
                query = query.Where(a => a.User != null && a.User.FullName.Contains(filter.Customer));
            }

            if (!string.IsNullOrEmpty(filter.Pet))
            {
                query =
                    query.Where(a => a.AppointmentPets.Any(ap => ap.Pet != null && ap.Pet.Name.Contains(filter.Pet)));
            }

            if (!string.IsNullOrEmpty(filter.Service))
            {
                query = query.Where(a =>
                    a.AppointmentServices.Any(asv => asv.Service != null && asv.Service.Name.Contains(filter.Service)));
            }

            if (filter.StatusIds != null && filter.StatusIds.Count > 0)
            {
                query = query.Where(a => filter.StatusIds.Contains(a.StatusId));
            }

            if (filter.Date.HasValue)
            {
                query = query.Where(a => a.AppointmentDate.Date == filter.Date.Value.Date);
            }

            if (filter.EmployeeId > 0)
            {
                query = query.Where(a => a.EmployeeId == filter.EmployeeId);
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
                .Include(a => a.Employee)
                .Include(a => a.Status)
                .Include(a => a.Promotion)
                .Include(a => a.AppointmentPets).ThenInclude(ap => ap.Pet).ThenInclude(p => p.Species)
                .Include(a => a.AppointmentServices).ThenInclude(asv => asv.Service).ThenInclude(s => s.Category)
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
                // Xóa mềm: chuyển trạng thái sang inactive
                appointment.IsActive = false;
                _context.Entry(appointment).State = EntityState.Modified;
            }
        }

        public void DeleteAppointmentPets(int appointmentId)
        {
            var appointmentPets = _context.AppointmentPets
                .Where(ap => ap.AppointmentId == appointmentId);
            foreach (var ap in appointmentPets)
            {
                ap.IsActive = false;
                _context.Entry(ap).State = EntityState.Modified;
            }
        }

        public void DeleteAppointmentServices(int appointmentId)
        {
            var appointmentServices = _context.AppointmentServices
                .Where(ap => ap.AppointmentId == appointmentId);
            foreach (var ap in appointmentServices)
            {
                ap.IsActive = false;
                _context.Entry(ap).State = EntityState.Modified;
            }
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
                .Where(u => u.RoleId == 2)
                .ToList();
        }

        public List<User> GetEmployeeUsers()
        {
            // RoleId == 3 là nhân viên (sửa lại cho đúng hệ thống của bạn nếu khác)
            return _context.Users
                .Where(u => u.RoleId == 3)
                .ToList();
        }

        public List<Pet> GetAllPets()
        {
            return _context.Pets.ToList();
        }

        public List<Appointment> GetPendingApprovalAppointments()
        {
            return _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Status)
                .Include(a => a.Employee)
                .Include(a => a.AppointmentPets).ThenInclude(ap => ap.Pet)
                .Include(a => a.AppointmentServices).ThenInclude(asr => asr.Service)
                .Where(a => a.StatusId == (int)pet_spa_system1.Services.AppointmentStatus.PendingCancel ||
                            a.StatusId == 7)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();
        }

        public List<Appointment> GetPendingAppointments()
        {
            return _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Status)
                .Include(a => a.Employee)
                .Include(a => a.AppointmentPets).ThenInclude(ap => ap.Pet)
                .Include(a => a.AppointmentServices).ThenInclude(asr => asr.Service)
                .Where(a => a.StatusId == 1 ||
                            a.StatusId == (int)pet_spa_system1.Services.AppointmentStatus.PendingCancel)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();
        }

        public List<Appointment> GetPendingCancelAppointments()
        {
            return _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Status)
                .Include(a => a.Employee)
                .Include(a => a.AppointmentPets).ThenInclude(ap => ap.Pet)
                .Include(a => a.AppointmentServices).ThenInclude(asr => asr.Service)
                .Where(a => a.StatusId == (int)pet_spa_system1.Services.AppointmentStatus.PendingCancel)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();
        }

        public int CountPendingApprovalAppointments()
        {
            return _context.Appointments.Count(a => a.StatusId == 1);
        }

        public int CountPendingCancelAppointments()
        {
            return _context.Appointments.Count(a =>
                a.StatusId == (int)pet_spa_system1.Services.AppointmentStatus.PendingCancel);
        }

        // Repository trả entity hoặc entity list, không có EndTime
        public List<Appointment> GetAppointmentsByStaffAndDate(int staffId, DateTime date)
        {
            return _context.Appointments
                .Include(a => a.AppointmentServices)
                .ThenInclude(s => s.Service)
                .Where(a => a.EmployeeId == staffId && a.AppointmentDate.Date == date.Date)
                .ToList();
        }

        public bool HasTimeConflict(int staffId, DateTime newStart, DateTime newEnd, int? excludeAppointmentId = null)
        {
            return _context.Appointments
                .Where(a => a.EmployeeId == staffId)
                .Where(a => excludeAppointmentId == null || a.AppointmentId != excludeAppointmentId)
                .Any(a =>
                    newStart < a.AppointmentDate.AddMinutes(a.AppointmentServices.Sum(s =>
                        s.Service.DurationMinutes ?? 0))
                    && newEnd > a.AppointmentDate
                );
        }

        public List<Appointment> GetAppointmentsByStatus(int statusId)
        {
            return _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Status)
                .Include(a => a.AppointmentServices)
                .ThenInclude(asv => asv.Service)
                .Include(a => a.AppointmentPets)
                .ThenInclude(ap => ap.Pet)
                .Where(a => a.StatusId == statusId)
                .ToList();
        }

        public void RestoreAppointment(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment != null)
            {
                appointment.IsActive = true;
                _context.Entry(appointment).State = EntityState.Modified;
            }
        }

        public void RestoreAppointmentPets(int appointmentId)
        {
            var appointmentPets = _context.AppointmentPets
                .Where(ap => ap.AppointmentId == appointmentId);
            foreach (var ap in appointmentPets)
            {
                ap.IsActive = true;
                _context.Entry(ap).State = EntityState.Modified;
            }
        }

        public void RestoreAppointmentServices(int appointmentId)
        {
            var appointmentServices = _context.AppointmentServices
                .Where(ap => ap.AppointmentId == appointmentId);
            foreach (var ap in appointmentServices)
            {
                ap.IsActive = true;
                _context.Entry(ap).State = EntityState.Modified;
            }
        }
    }
}