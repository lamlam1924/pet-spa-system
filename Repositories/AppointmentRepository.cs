using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly PetDataShopContext _context;

        public AppointmentRepository(PetDataShopContext context)
        {
            _context = context;
        }

        public List<object> GetCalendarEvents()
        {
            return GetAppointments(new AppointmentFilter { })
                .Where(a =>
                {
                    var cancelledStatus = _context.StatusAppointments.FirstOrDefault(s => s.StatusName == "Cancelled");
                    return cancelledStatus != null && a.StatusId != cancelledStatus.StatusId;
                })
                .Select(a => new
                {
                    id = a.AppointmentId,
                    userFullName = a.User?.FullName,
                    appointmentDate = a.AppointmentDate,
                    startTime = a.StartTime,
                    staffIds = a.AppointmentPets?.Select(ap => ap.StaffId).ToList(),
                    status = a.Status?.StatusName,
                    phone = a.User?.Phone,
                    serviceNames = a.AppointmentServices?.Select(s => s.Service?.Name),
                    duration = a.AppointmentServices?.Sum(s => s.Service?.DurationMinutes ?? 0) ?? 0
                })
                .Where(a => a.appointmentDate.Year >= 1900)
                .Select(a => new
                {
                    id = a.id,
                    title = a.userFullName + " - " + string.Join(", ", a.serviceNames ?? new List<string>()),
                    start = a.appointmentDate.ToDateTime(a.startTime).ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = a.appointmentDate.ToDateTime(a.startTime).AddMinutes(a.duration)
                        .ToString("yyyy-MM-ddTHH:mm:ss"),
                    resourceIds = a.staffIds,
                    customer = a.userFullName,
                    phone = a.phone,
                    services = (a.serviceNames ?? new List<string>()).ToList(),
                    status = a.status
                })
                .Cast<object>().ToList();
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

        public int CountAppointmentsByDate(DateTime date)
        {
            // Chỉ đếm các lịch đã xác nhận (StatusId == 2)
            return _context.Appointments
                .Where(a => a.AppointmentDate == DateOnly.FromDateTime(date) && a.StatusId == 2)
                .Count();
        }

        public int CountUpcomingAppointments(DateTime fromDate)
        {
            // Chỉ đếm các lịch đã xác nhận (StatusId == 2) và ngày lớn hơn hôm nay
            return _context.Appointments
                .Where(a => a.AppointmentDate > DateOnly.FromDateTime(fromDate) && a.StatusId == 2)
                .Count();
        }

        public int CountAppointmentsByStatus(int statusId)
        {
            return _context.Appointments
                .Where(a => a.StatusId == statusId)
                .Count();
        }

        public List<Appointment> GetAppointments(AppointmentFilter filter)
        {
            var query = _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Status)
                .Include(a => a.AppointmentPets).ThenInclude(ap => ap.Pet)
                .Include(a => a.AppointmentPets).ThenInclude(ap => ap.Staff) // Nếu có navigation Staff
                .Include(a => a.AppointmentServices).ThenInclude(asr => asr.Service)
                .AsQueryable();

            query = ApplyFilters(query, filter);

            return query
                .OrderByDescending(a => a.AppointmentDate)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();
        }

        public int CountAppointments(pet_spa_system1.ViewModel.AppointmentFilter filter)
        {
            var query = _context.Appointments.AsQueryable();
            query = ApplyFilters(query, filter);
            return query.Count();
        }

        private IQueryable<Appointment> ApplyFilters(IQueryable<Appointment> query,
            pet_spa_system1.ViewModel.AppointmentFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Customer))
            {
                query = query.Where(a =>
                    a.User != null && a.User.FullName != null && a.User.FullName.Contains(filter.Customer));
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

            if (filter.StaffId.HasValue)
            {
                query = query.Where(a => a.AppointmentPets.Any(ap => ap.StaffId == filter.StaffId.Value));
            }

            if (filter.Date.HasValue)
            {
                query = query.Where(a => a.AppointmentDate == DateOnly.FromDateTime(filter.Date.Value));
            }

            // Đã loại bỏ EmployeeId, chỉ lọc theo staff qua AppointmentPet nếu cần

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
                .Where(a => a.AppointmentDate >= DateOnly.FromDateTime(start) &&
                            a.AppointmentDate <= DateOnly.FromDateTime(end))
                .ToList();
        }

        public Appointment GetAppointmentWithDetails(int id)
        {
            return _context.Appointments
                .Include(a => a.User)
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
                .Where(a => a.StatusId == _context.StatusAppointments
                                .FirstOrDefault(s => s.StatusName == "PendingCancel").StatusId ||
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
                            a.StatusId == _context.StatusAppointments
                                .FirstOrDefault(s => s.StatusName == "PendingCancel").StatusId)
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
                .Where(a => a.StatusId == _context.StatusAppointments
                    .FirstOrDefault(s => s.StatusName == "PendingCancel").StatusId)
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
                a.StatusId == _context.StatusAppointments.FirstOrDefault(s => s.StatusName == "PendingCancel")
                    .StatusId);
        }

        // Repository trả entity hoặc entity list, không có EndTime
        public List<Appointment> GetAppointmentsByStaffAndDate(int staffId, DateTime date)
        {
            // Lấy các lịch hẹn mà có ít nhất 1 AppointmentPet được gán staffId này
            return _context.Appointments
                .Include(a => a.AppointmentServices).ThenInclude(s => s.Service)
                .Include(a => a.AppointmentPets).ThenInclude(ap => ap.Pet)
                .Include(a => a.AppointmentPets).ThenInclude(ap => ap.Staff) // Nếu có navigation Staff
                .Where(a => a.AppointmentPets.Any(ap => ap.StaffId == staffId) &&
                            a.AppointmentDate == DateOnly.FromDateTime(date))
                .ToList();
        }

        public bool HasTimeConflict(int staffId, DateTime newStart, DateTime newEnd, int? excludeAppointmentId = null)
        {
            // Kiểm tra trùng lịch cho từng pet-staff
            return _context.AppointmentPets
                .Where(ap =>
                    ap.StaffId == staffId && (excludeAppointmentId == null || ap.AppointmentId != excludeAppointmentId))
                .Join(_context.Appointments,
                    ap => ap.AppointmentId,
                    a => a.AppointmentId,
                    (ap, a) => new { ap, a })
                .Any(joined =>
                    newStart < joined.a.AppointmentDate.ToDateTime(joined.a.StartTime)
                        .AddMinutes(joined.a.AppointmentServices.Sum(s => s.Service.DurationMinutes ?? 0))
                    && newEnd > joined.a.AppointmentDate.ToDateTime(joined.a.StartTime)
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

        public List<int> GetAppointmentIdsByStaff(int staffId)
        {
            return _context.AppointmentPets.Where(ap => ap.StaffId == staffId).Select(ap => ap.AppointmentId).Distinct()
                .ToList();
        }

        public List<Appointment> GetAppointmentsByIds(List<int> appointmentIds)
        {
            return _context.Appointments.Where(a => appointmentIds.Contains(a.AppointmentId)).ToList();
        }

        public AppointmentPet GetAppointmentPet(int appointmentId, int petId)
        {
            return _context.AppointmentPets.FirstOrDefault(ap =>
                ap.AppointmentId == appointmentId && ap.PetId == petId) ?? new AppointmentPet();
        }

        public void UpdateAppointmentPetStaff(int appointmentId, int petId, int staffId)
        {
            var ap = _context.AppointmentPets.FirstOrDefault(x => x.AppointmentId == appointmentId && x.PetId == petId);
            if (ap != null)
            {
                ap.StaffId = staffId;
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
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

        public List<StatusAppointment> GetAllStatuses()
        {
            return _context.StatusAppointments.ToList();
        }

        public Appointment? GetById(int id)
        {
            return _context.Appointments
                .Include(a => a.AppointmentServices).ThenInclude(s => s.Service)
                .Include(a => a.AppointmentPets).ThenInclude(p => p.Pet)
                .Include(a => a.AppointmentPets).ThenInclude(ap => ap.Staff)
                .Include(a => a.Status)
                .Include(a => a.User)
                .SingleOrDefault(a => a.AppointmentId == id);
        }

        public void AddAppointmentPet(int appointmentId, int petId, int? staffId = null)
        {
            _context.AppointmentPets.Add(new AppointmentPet
            {
                AppointmentId = appointmentId,
                PetId = petId,
                StaffId = staffId
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
    }
}