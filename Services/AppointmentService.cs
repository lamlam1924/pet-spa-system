using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Services
{
    public class AppointmentService : IAppointmentService
    {
        /// <summary>
        /// Gửi mail thông báo cho khách hàng khi lịch hẹn được duyệt/gán hoặc bị từ chối/hủy
        /// </summary>
        /// <param name="appointmentId">Id lịch hẹn</param>
        /// <param name="type">Kiểu thông báo: approved, rejected</param>
        /// <param name="staffId">Id nhân viên được gán (nếu có)</param>
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IEmailService _emailService;
        private readonly IPetRepository _petRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUserRepository _userRepository;
        private readonly PetDataShopContext _context;
        private readonly IAppointmentLogService _appointmentLogService;

        // ...existing code...

        public void SendAppointmentNotificationMail(int appointmentId, string type, int? staffId)
        {
            var appointment = GetAppointmentById(appointmentId);
            if (appointment == null) return;
            var customer = appointment.User;
            if (customer == null || string.IsNullOrEmpty(customer.Email)) return;

            string subject = "[Pet Spa] Thông báo lịch hẹn của bạn";
            string spaAddress = "123 Đường Spa, Quận 1, TP.HCM";
            string spaHotline = "0123 456 789";
            string petNames = appointment.AppointmentPets != null && appointment.AppointmentPets.Count > 0
                ? string.Join(", ", appointment.AppointmentPets.Select(p => p.Pet.Name)) : "Không có";
            string serviceNames = appointment.AppointmentServices != null && appointment.AppointmentServices.Count > 0
                ? string.Join(", ", appointment.AppointmentServices.Select(s => s.Service.Name)) : "Không có";
            string staffInfo = "";
            if (staffId.HasValue)
            {
                var staff = GetEmployees().FirstOrDefault(e => e.UserId == staffId.Value);
                if (staff != null)
                    staffInfo = $"<tr><td>Nhân viên phục vụ:</td><td>{staff.FullName}</td></tr>";
            }
            string timeInfo = $"<tr><td>Thời gian:</td><td>{appointment.AppointmentDate:dd/MM/yyyy HH:mm}</td></tr>";
            string tableInfo = $@"
                <table style='border-collapse:collapse;'>
                    <tr><td>Khách hàng:</td><td>{customer.FullName}</td></tr>
                    {staffInfo}
                    {timeInfo}
                    <tr><td>Thú cưng:</td><td>{petNames}</td></tr>
                    <tr><td>Dịch vụ:</td><td>{serviceNames}</td></tr>
                </table>";
            string body = "";
            if (type == "approved")
            {
                body = $@"
                    <h3>Chào {customer.FullName},</h3>
                    <p>Lịch hẹn của bạn tại Pet Spa đã được <b>duyệt thành công</b>.</p>
                    {tableInfo}
                    <p>Địa chỉ Spa: {spaAddress}<br/>Hotline: {spaHotline}</p>
                    <p>Vui lòng đến đúng giờ để được phục vụ tốt nhất.<br/>Cảm ơn bạn đã tin tưởng Pet Spa!</p>
                ";
            }
            else if (type == "rejected")
            {
                body = $@"
                    <h3>Chào {customer.FullName},</h3>
                    <p>Rất tiếc, lịch hẹn của bạn tại Pet Spa đã bị <b>từ chối hoặc hủy</b>.</p>
                    {tableInfo}
                    <p>Địa chỉ Spa: {spaAddress}<br/>Hotline: {spaHotline}</p>
                    <p>Vui lòng liên hệ lại với chúng tôi để được hỗ trợ đặt lịch mới.<br/>Xin cảm ơn!</p>
                ";
            }
            else
            {
                body = $@"
                    <h3>Chào {customer.FullName},</h3>
                    <p>Thông báo về lịch hẹn của bạn tại Pet Spa.</p>
                    {tableInfo}
                    <p>Địa chỉ Spa: {spaAddress}<br/>Hotline: {spaHotline}</p>
                ";
            }

            try
            {
                this._emailService.SendEmail(customer.Email, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Mail] Lỗi gửi mail: {ex.Message}");
            }
        }
        public int CalculateDurationMinutes(int appointmentId)
        {
            var appointment = _appointmentRepository.GetById(appointmentId);
            if (appointment == null || appointment.AppointmentServices == null)
                return 0;
            return appointment.AppointmentServices.Sum(s => s.Service?.DurationMinutes ?? 0);
        }
    public ApproveAssignResult AdminApproveAndAssignStaff(ApproveAssignRequest request)
    {
        var appointment = _appointmentRepository.GetById(request.AppointmentId);
        if (appointment == null)
            return new ApproveAssignResult { Success = false, Message = "Không tìm thấy lịch hẹn", Updated = null };

        // Gán nhân viên
        appointment.EmployeeId = request.StaffId;
        appointment.StatusId = 2; // Đã xác nhận
        _appointmentRepository.Update(appointment);
        _appointmentRepository.Save();

        // Log lịch sử duyệt/gán
        _appointmentLogService?.LogAction(appointment.AppointmentId, "ApproveAndAssign", $"Admin duyệt và gán nhân viên {request.StaffId}", request.StaffId);

        // Gửi email xác nhận
        if (!string.IsNullOrEmpty(appointment.User?.Email))
        {
            var emailModel = BuildAppointmentConfirmationEmailModel(appointment);
            _emailService.SendAppointmentConfirmation(emailModel);
        }

        return new ApproveAssignResult
        {
            Success = true,
            Message = "Đã duyệt và gán nhân viên thành công, đã gửi email xác nhận.",
            Updated = BuildAppointmentViewModel(appointment)
        };
    }
    private User GetUserById(int userId)
    {
        // Sử dụng FindById thay cho GetById nếu có, kiểm tra null an toàn
        return _userRepository != null ? _userRepository.FindById(userId) : null;
    }
    private void SendConfirmationEmail(Appointment appointment)
    {
        var customer = GetUserById(appointment.UserId);
        if (customer == null || string.IsNullOrEmpty(customer.Email)) return;
        string subject = "Xác nhận lịch hẹn tại Pet Spa";
        string staffName = appointment.EmployeeId.HasValue ? (GetUserById(appointment.EmployeeId.Value)?.FullName ?? "Chưa có") : "Chưa có";
        string body = $"Xin chào {customer.FullName},\n\nLịch hẹn của bạn vào {appointment.AppointmentDate:dd/MM/yyyy HH:mm} đã được xác nhận.\nNhân viên phụ trách: {staffName}.\n\nCảm ơn bạn đã sử dụng dịch vụ!";
        _emailService?.SendEmail(customer.Email, subject, body);
    }

        public AppointmentService(
            IAppointmentRepository repo,
            IEmailService iEmailService,
            IPetRepository petRepository,
            IServiceRepository serviceRepository,
            IUserRepository userRepository,
            PetDataShopContext context,
            IAppointmentLogService appointmentLogService)
        {
            _appointmentRepository = repo;
            _emailService = iEmailService;
            _petRepository = petRepository;
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
            _context = context;
            _appointmentLogService = appointmentLogService;
        }

        public bool SaveAppointment(AppointmentViewModel model, int userId)
        {
            var appointment = new Appointment
            {
                UserId = userId,
                AppointmentDate = model.AppointmentDate.Date.Add(model.AppointmentTime),
                StatusId = 1,
                Notes = model.Notes,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            int appId = _appointmentRepository.AddAppointment(appointment);

            if (model.SelectedPetIds != null && model.SelectedPetIds.Any())
            {
                foreach (var petId in model.SelectedPetIds)
                {
                    _appointmentRepository.AddAppointmentPet(appId, petId);
                }
            }

            if (model.SelectedServiceIds != null && model.SelectedServiceIds.Any())
            {
                foreach (var serviceId in model.SelectedServiceIds)
                {
                    _appointmentRepository.AddAppointmentService(appId, serviceId);
                }
            }

            _appointmentRepository.Save();

            return true;
        }

        public AppointmentHistoryViewModel GetAppointmentHistory(int userId)
        {
            var appointments = _appointmentRepository.GetByUserIdWithDetail(userId);
            var statuses = _appointmentRepository.GetAllStatuses();

            foreach (var a in appointments)
            {
                Debug.WriteLine($"Mapping Appointment {a.AppointmentId}");
                var serviceCount = a.AppointmentServices?.Count ?? 0;
                Debug.WriteLine($"Service count: {serviceCount}");
            }

            var vm = new AppointmentHistoryViewModel
            {
                Statuses = statuses,
                Appointments = appointments.Select(a => new AppointmentHistoryItemViewModel
                {
                    AppointmentId = a.AppointmentId,
                    AppointmentDate = a.AppointmentDate,
                    StatusId = a.StatusId,
                    StatusName = a.Status?.StatusName ?? "",
                    Notes = a.Notes ?? "",
                    Services = a.AppointmentServices?.Select(asv => new ServiceHistoryInfo
                    {
                        ServiceId = asv.ServiceId,
                        Name = asv.Service?.Name ?? "Unknown",
                        Category = asv.Service?.Category?.Name ?? "Unknown",
                        Price = asv.Service?.Price ?? 0,
                        Duration = asv.Service?.DurationMinutes ?? 0
                    }).ToList() ?? new List<ServiceHistoryInfo>(),
                    PetNames = a.AppointmentPets?.Select(ap => ap.Pet?.Name ?? "").ToList() ?? new List<string>()
                }).OrderByDescending(x => x.AppointmentDate).ToList()
            };

            return vm;
        }

        public Appointment GetAppointmentById(int appointmentId)
        {
            return _appointmentRepository.GetById(appointmentId);
        }

        public List<string> GetPetNames(List<int> petIds)
        {
            if (petIds == null || petIds.Count == 0) return new List<string>();
            return _appointmentRepository.GetPetNamesByIds(petIds);
        }

        public List<string> GetServiceNames(List<int> serviceIds)
        {
            if (serviceIds == null || serviceIds.Count == 0) return new List<string>();
            return _appointmentRepository.GetServiceNamesByIds(serviceIds);
        }

        public AppointmentDashboardViewModel GetDashboardData()
        {
            return GetDashboardStats();
        }

        public AppointmentDashboardViewModel GetDashboardStats()
        {
            var today = DateTime.Today;
            var viewModel = new AppointmentDashboardViewModel
            {
                TodayAppointments = _appointmentRepository.CountAppointmentsByDate(today),
                UpcomingAppointments = _appointmentRepository.CountUpcomingAppointments(today.AddDays(1)),
                CompletedAppointments = _appointmentRepository.CountAppointmentsByStatus(3),
                CancelledAppointments = _appointmentRepository.CountAppointmentsByStatus(4),
                PendingApprovalAppointments = _appointmentRepository.CountPendingApprovalAppointments(),
                PendingCancelAppointments = _appointmentRepository.CountPendingCancelAppointments()
            };

            var monthlyStatsRaw = _appointmentRepository.GetMonthlyStats(today.Year);
            viewModel.MonthlyStats = monthlyStatsRaw.Select(s => new MonthlyAppointmentStatsViewModel
            {
                MonthLabel = $"Tháng {s.Month}",
                AppointmentCount = s.TotalAppointments
            }).ToList();

            var recentAppointments = _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Status)
                .Include(a => a.AppointmentPets).ThenInclude(ap => ap.Pet)
                .Include(a => a.AppointmentServices).ThenInclude(asr => asr.Service)
                .Where(a => a.AppointmentDate.Date == DateTime.Today)
                .OrderBy(a => a.AppointmentDate)
                .Take(5)
                .ToList();

            viewModel.RecentAppointments = recentAppointments.Select(a => new DailyAppointmentSummaryViewModel
            {
                AppointmentId = a.AppointmentId,
                CustomerName = a.User.FullName,
                AppointmentDate = a.AppointmentDate,
                StatusName = a.Status.StatusName,
                StatusId = a.StatusId,
                PetNames = string.Join(", ", a.AppointmentPets.Select(ap => ap.Pet.Name)),
                ServiceNames = string.Join(", ", a.AppointmentServices.Select(asr => asr.Service.Name))
            }).ToList();

            return viewModel;
        }

        public List<Appointment> GetAppointments(
            string searchTerm = "",
            int statusId = 0,
            DateTime? date = null,
            int employeeId = 0,
            int page = 1,
            int pageSize = 10)
        {
            return _appointmentRepository.GetAppointments(searchTerm, statusId, date, employeeId, page, pageSize);
        }

        public int CountAppointments(
            string searchTerm = "",
            int statusId = 0,
            DateTime? date = null,
            int employeeId = 0)
        {
            return _appointmentRepository.CountAppointments(searchTerm, statusId, date, employeeId);
        }

        public List<StatusAppointment> GetAllStatuses()
        {
            return _appointmentRepository.GetAllStatuses();
        }

        public List<User> GetEmployees()
        {
            return _appointmentRepository.GetEmployeeUsers();
        }

        public List<object> GetAppointmentsForCalendar(DateTime start, DateTime end)
        {
            var appointments = _appointmentRepository.GetAppointmentsByDateRange(start, end);

            return appointments.Select(a => new
            {
                id = a.AppointmentId,
                title = GetAppointmentTitle(a),
                start = a.AppointmentDate.ToString("yyyy-MM-dd HH:mm:ss"),
                end = a.AppointmentDate.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"),
                className = GetStatusClass(a.StatusId),
                description = a.Notes
            }).Cast<object>().ToList();
        }

        public Appointment GetAppointmentDetails(int id)
        {
            return _appointmentRepository.GetAppointmentWithDetails(id);
        }

        public AppointmentViewModel PrepareCreateViewModel()
        {
            var petInfos = _petRepository.GetAllPetsWithSpecies()
                .Select(p => new PetInfo
                {
                    PetId = p.PetId,
                    Name = p.Name,
                    SpeciesName = p.Species != null ? p.Species.SpeciesName : null
                }).ToList();

            var viewModel = new AppointmentViewModel
            {
                Statuses = GetAllStatuses(),
                Customers = _appointmentRepository.GetCustomers(),
                AllPets = petInfos,
                AllServices = _appointmentRepository.GetActiveServices().ToList()
            };
            return viewModel;
        }

        public bool CreateAppointment(AppointmentViewModel model)
        {
            try
            {
                var appointment = new Appointment
                {
                    UserId = model.CustomerId,
                    // Nếu chỉ lưu 1 nhân viên chính, lấy nhân viên đầu tiên
                    EmployeeId = model.EmployeeIds != null && model.EmployeeIds.Count > 0
                        ? model.EmployeeIds[0]
                        : (int?)null,
                    AppointmentDate = model.AppointmentDate,
                    StatusId = model.StatusId,
                    Notes = model.Notes,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _appointmentRepository.Add(appointment);
                _appointmentRepository.Save();

                if (model.SelectedPetIds != null && model.SelectedPetIds.Any())
                {
                    foreach (var petId in model.SelectedPetIds)
                    {
                        _appointmentRepository.AddAppointmentPet(appointment.AppointmentId, petId);
                    }
                }

                if (model.SelectedServiceIds != null && model.SelectedServiceIds.Any())
                {
                    foreach (var serviceId in model.SelectedServiceIds)
                    {
                        _appointmentRepository.AddAppointmentService(appointment.AppointmentId, serviceId);
                    }
                }

                _appointmentRepository.Save();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating appointment: {ex.Message}");
                return false;
            }
        }

        public AppointmentViewModel PrepareEditViewModel(int id)
        {
            var appointment = _appointmentRepository.GetAppointmentWithDetails(id);
            if (appointment == null) return null;

            // Lấy danh sách thú cưng của đúng user (CustomerId/UserId) kèm species từ repository

            var petInfos = _petRepository.GetAllPetsWithSpecies()
                .Where(p => p.UserId == appointment.UserId)
                .Select(p => new PetInfo
                {
                    PetId = p.PetId,
                    Name = p.Name,
                    SpeciesName = p.Species != null ? p.Species.SpeciesName : null
                }).ToList();

            return new AppointmentViewModel
            {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate,
                StatusId = appointment.StatusId,
                // Nếu chỉ lưu 1 nhân viên chính, đưa vào danh sách
                EmployeeIds = appointment.EmployeeId != null
                    ? new List<int> { appointment.EmployeeId.Value }
                    : new List<int>(),
                CustomerId = appointment.UserId,
                Notes = appointment.Notes,

                Statuses = _appointmentRepository.GetAllStatuses(),
                Customers = _appointmentRepository.GetCustomers(),
                AllPets = petInfos,
                AllServices = _serviceRepository.GetActiveServices().ToList(),

                SelectedPetIds = appointment.AppointmentPets != null
                    ? appointment.AppointmentPets.Select(ap => ap.PetId).ToList()
                    : new List<int>(),
                SelectedServiceIds = appointment.AppointmentServices != null
                    ? appointment.AppointmentServices.Select(asr => asr.ServiceId).ToList()
                    : new List<int>()
            };
        }

        public bool UpdateAppointment(AppointmentViewModel model)
        {
            var appointment = _appointmentRepository.GetById(model.AppointmentId);
            if (appointment == null) return false;

            // Map các trường từ model sang entity (chỉ update primitive, không update navigation property)
            appointment.AppointmentDate = model.AppointmentDate;
            appointment.StatusId = model.StatusId;
            appointment.Notes = model.Notes;
            appointment.EmployeeId = model.EmployeeIds != null && model.EmployeeIds.Any()
                ? model.EmployeeIds.First()
                : (int?)null;
            appointment.UserId = model.CustomerId;

            // Cập nhật các quan hệ (pet/service) - phải Save trước khi update entity để tránh lỗi tracking
            _appointmentRepository.DeleteAppointmentPets(model.AppointmentId);
            if (model.SelectedPetIds != null && model.SelectedPetIds.Any())
            {
                foreach (var petId in model.SelectedPetIds)
                {
                    _appointmentRepository.AddAppointmentPet(model.AppointmentId, petId);
                }
            }

            _appointmentRepository.DeleteAppointmentServices(model.AppointmentId);
            if (model.SelectedServiceIds != null && model.SelectedServiceIds.Any())
            {
                foreach (var serviceId in model.SelectedServiceIds)
                {
                    _appointmentRepository.AddAppointmentService(model.AppointmentId, serviceId);
                }
            }

            _appointmentRepository.Save(); // Save các thay đổi quan hệ trước

            _appointmentRepository.Update(appointment); // Update entity primitive fields
            _appointmentRepository.Save();
            return true;
        }

        public bool UpdateAppointmentStatus(int id, int statusId)
        {
            try
            {
                var appointment = _appointmentRepository.GetById(id);
                if (appointment == null) return false;

                appointment.StatusId = statusId;
                _appointmentRepository.Update(appointment);
                _appointmentRepository.Save();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating appointment status: {ex.Message}");
                return false;
            }
        }

        public bool DeleteAppointment(int id)
        {
            try
            {
                _appointmentRepository.DeleteAppointmentPets(id);
                _appointmentRepository.DeleteAppointmentServices(id);
                _appointmentRepository.Delete(id);
                _appointmentRepository.Save();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting appointment: {ex.Message}");
                return false;
            }
        }

        public List<Pet> GetCustomerPets(int userId)
        {
            // Nếu muốn tối ưu, có thể thêm hàm GetPetsByUserId vào repository
            return _appointmentRepository.GetAllPets().Where(p => p.UserId == userId).ToList();
        }

        private string GetAppointmentTitle(Appointment appointment)
        {
            var petNames = appointment.AppointmentPets?
                .Select(ap => ap.Pet?.Name)
                .Where(name => name != null)
                .Select(name => name!)
                .ToList() ?? new List<string>();

            var serviceNames = appointment.AppointmentServices?
                .Select(asr => asr.Service?.Name)
                .Where(name => name != null)
                .Select(name => name!)
                .ToList() ?? new List<string>();

            return $"{appointment.User?.FullName ?? "Unknown"}\n" +
                   $"Thú cưng: {string.Join(", ", petNames)}\n" +
                   $"Dịch vụ: {string.Join(", ", serviceNames)}";
        }

        private string GetStatusClass(int statusId)
        {
            return statusId switch
            {
                1 => "bg-warning", // Chờ xác nhận
                2 => "bg-info", // Đã xác nhận
                3 => "bg-success", // Hoàn thành
                4 => "bg-danger", // Đã hủy
                _ => "bg-secondary"
            };
        }

        /// <summary>
        /// User gửi yêu cầu hủy lịch hẹn (chuyển trạng thái sang PendingCancel)
        /// </summary>
        /// <param name="appointmentId"></param>
        /// <param name="userId"></param>
        /// <returns>true nếu thành công, false nếu thất bại</returns>
        public bool RequestCancelAppointment(int appointmentId, int userId)
        {
            try
            {
                Console.WriteLine($"[RequestCancelAppointment] appointmentId={appointmentId}, userId={userId}");
                var appointment = _appointmentRepository.GetById(appointmentId);
                if (appointment == null)
                {
                    Console.WriteLine($"[RequestCancelAppointment] appointment == null");
                    return false;
                }

                if (appointment.UserId != userId)
                {
                    Console.WriteLine(
                        $"[RequestCancelAppointment] appointment.UserId={appointment.UserId} != userId={userId}");
                    return false;
                }

                Console.WriteLine($"[RequestCancelAppointment] appointment.StatusId={appointment.StatusId}");
                // Chỉ cho phép hủy nếu trạng thái là pending/confirmed/inprogress
                if (appointment.StatusId == 4 || appointment.StatusId == 5 || appointment.StatusId == 6)
                {
                    Console.WriteLine($"[RequestCancelAppointment] StatusId={appointment.StatusId} không cho phép hủy");
                    return false;
                }

                // 6: PendingCancel (chờ duyệt hủy) - cần đồng bộ với DB
                appointment.StatusId = 6;
                _appointmentRepository.Update(appointment);
                _appointmentRepository.Save();

                Console.WriteLine(
                    $"[RequestCancelAppointment] Đã cập nhật StatusId=6 thành công cho appointmentId={appointmentId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RequestCancelAppointment: {ex.Message}");
                return false;
            }
        }

        public AdminAppointmentDetailViewModel GetAdminAppointmentDetail(int id)
        {
            var appointment = _appointmentRepository.GetAppointmentWithDetails(id);
            if (appointment == null) return null;

            var vm = new AdminAppointmentDetailViewModel
            {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate,
                Notes = appointment.Notes,
                StatusId = appointment.StatusId,
                StatusName = appointment.Status?.StatusName ?? "",
                // StatusColor = appointment.Status?.Color, // nếu có
                UserId = appointment.UserId,
                CustomerName = appointment.User?.FullName,
                CustomerPhone = appointment.User?.Phone,
                CustomerEmail = appointment.User?.Email,
                CustomerAddress = appointment.User?.Address,
                EmployeeId = appointment.EmployeeId,
                EmployeeName = appointment.Employee?.FullName,
                EmployeePhone = appointment.Employee?.Phone,
                EmployeeEmail = appointment.Employee?.Email,
                Pets = appointment.AppointmentPets.Select(ap => new PetInfo
                {
                    PetId = ap.Pet.PetId,
                    Name = ap.Pet.Name,
                    SpeciesName = ap.Pet.Species?.SpeciesName,
                    Breed = ap.Pet.Breed,
                    Age = ap.Pet.Age,
                    Gender = ap.Pet.Gender
                }).ToList(),
                Services = appointment.AppointmentServices.Select(asv => new ServiceInfo
                {
                    ServiceId = asv.Service.ServiceId,
                    Name = asv.Service.Name,
                    CategoryName = asv.Service.Category?.Name,
                    Price = asv.Service.Price,
                    DurationMinutes = asv.Service.DurationMinutes
                }).ToList(),
                TotalPrice = appointment.AppointmentServices.Sum(asv => asv.Service.Price),
                CreatedAt = appointment.CreatedAt,
                TotalDurationMinutes = appointment.AppointmentServices.Sum(asv => asv.Service.DurationMinutes ?? 0)
                // Các trường UpdatedAt, PromotionName, PromotionValue bị loại bỏ vì không có trong entity
            };

            return vm;
        }

        public List<Appointment> GetPendingApprovalAppointments()
        {
            return _appointmentRepository.GetPendingApprovalAppointments();
        }

        public List<AdminAppointmentDetailViewModel> GetPendingAppointments()
        {
            return _appointmentRepository.GetPendingAppointments()
                .Select(a => GetAdminAppointmentDetail(a.AppointmentId))
                .ToList();
        }

        public List<AdminAppointmentDetailViewModel> GetPendingCancelAppointments()
        {
            return _appointmentRepository.GetPendingCancelAppointments()
                .Select(a => GetAdminAppointmentDetail(a.AppointmentId))
                .ToList();
        }


        // Service map entity sang ViewModel + tính toán EndTime
        public List<AppointmentViewModel> GetAppointmentsByStaffAndDate(int staffId, DateTime date)
        {
            var appointments = _appointmentRepository.GetAppointmentsByStaffAndDate(staffId, date);
            var result = appointments.Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                AppointmentDate = a.AppointmentDate,
                DurationMinutes = a.AppointmentServices.Sum(s => s.Service.DurationMinutes ?? 0),
                EndTime = a.AppointmentDate.AddMinutes(a.AppointmentServices.Sum(s => s.Service.DurationMinutes ?? 0)),
                StatusId = a.StatusId
                // map thêm nếu cần
            }).ToList();

            return result;
        }

        public RealtimeShiftViewModel GetManagementTimelineData(DateTime date)
        {
            try
            {
                var hours = Enumerable.Range(8, 13).ToList(); // 8h đến 20h
                var viewModel = new RealtimeShiftViewModel
                {
                    Hours = hours,
                    StaffShifts = new List<StaffShift>()
                };

                var staffList = _userRepository.GetStaffList() ?? new List<pet_spa_system1.ViewModel.StaffViewModel>();

                foreach (var staff in staffList)
                {
                    var appointments = GetAppointmentsByStaffAndDate(staff.UserId, date); // lấy lịch hẹn của nhân viên

                    var hourStatus = new Dictionary<int, ShiftStatus>();

                    foreach (var appointment in appointments)
                    {
                        int hour = appointment.AppointmentDate.Hour;

                        // Nếu trong cùng 1 giờ có nhiều lịch thì chỉ ghi đè, bạn có thể tùy biến thêm
                        hourStatus[hour] = new ShiftStatus
                        {
                            AppointmentId = appointment.AppointmentId,
                            StatusText = "Đang làm",
                            ColorClass = "bg-inprogress",
                            TimeRange = $"{appointment.AppointmentDate:HH:mm} - {appointment.EndTime:HH:mm}"
                        };
                    }

                    var staffShift = new StaffShift
                    {
                        UserId = staff.UserId,
                        StaffName = staff.FullName,
                        AvatarUrl = string.IsNullOrEmpty(staff.ProfilePictureUrl)
                            ? "/images/default-avatar.png"
                            : staff.ProfilePictureUrl,
                        HourStatus = hourStatus,
                        Appointments = appointments.Select(a => new AppointmentViewModel
                        {
                            AppointmentId = a.AppointmentId,
                            AppointmentDate = a.AppointmentDate,
                            EndTime = a.EndTime
                        }).ToList()
                    };

                    viewModel.StaffShifts.Add(staffShift); // ❗ Đừng quên thêm vào danh sách!
                }

                return viewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetManagementTimelineData: {ex.Message}");
                throw;
            }
        }


        private string GetStatusText(int statusId) => statusId switch
        {
            1 => "Chờ xác nhận",
            2 => "Đang diễn ra",
            3 => "Hoàn tất",
            4 => "Đã hủy",
            5 => "Vắng mặt",
            6 => "Yêu cầu hủy",
            _ => "Không rõ"
        };

        private string GetColorClass(int statusId) => statusId switch
        {
            1 => "bg-warning text-dark",
            2 => "bg-info text-white",
            3 => "bg-success text-white",
            4 => "bg-danger text-white",
            5 => "bg-secondary text-white",
            6 => "bg-warning text-dark",
            _ => "bg-light"
        };

        public bool TryUpdateAppointmentStaff(int appointmentId, int newStaffId)
        {
            var appointment = _appointmentRepository.GetById(appointmentId);
            if (appointment == null)
                return false;

            var durationMinutes = appointment.AppointmentServices?.Sum(s => s.Service?.DurationMinutes ?? 0) ?? 0;
            var newStart = appointment.AppointmentDate;
            var newEnd = newStart.AddMinutes(durationMinutes);

            // Kiểm tra trùng lịch với nhân viên mới
            var conflictingAppointments = _appointmentRepository
                .GetAppointmentsByStaffAndDate(newStaffId, newStart.Date)
                .Where(a => a.AppointmentId != appointmentId) // ngoại trừ chính lịch này
                .Where(a =>
                {
                    var aStart = a.AppointmentDate;
                    var aEnd = aStart.AddMinutes(a.AppointmentServices?.Sum(s => s.Service?.DurationMinutes ?? 0) ?? 0);
                    // Kiểm tra chồng giờ: nếu bắt đầu trước kết thúc lịch khác và kết thúc sau bắt đầu lịch khác thì xung đột
                    return newStart < aEnd && newEnd > aStart;
                })
                .Any();

            if (conflictingAppointments)
                return false;

            // Cập nhật nhân viên
            appointment.EmployeeId = newStaffId;
            _appointmentRepository.Update(appointment);
            _appointmentRepository.Save();

            return true;
        }

        public bool IsTimeConflict(DateTime appointmentDate, int staffId, int durationMinutes)
        {
            var appointments = _appointmentRepository.GetAppointmentsByStaffAndDate(staffId, appointmentDate.Date);
            var appointmentStart = appointmentDate;
            var appointmentEnd = appointmentDate.AddMinutes(durationMinutes);

            foreach (var appt in appointments)
            {
                var apptStart = appt.AppointmentDate;
                var apptEnd = apptStart.AddMinutes(appt.AppointmentServices.Sum(s => s.Service?.DurationMinutes ?? 0));

                if (appointmentStart < apptEnd && appointmentEnd > apptStart)
                    return true;
            }

            return false;
        }

        public bool ApproveAndAssignStaff(int appointmentId, int staffId)
        {
            var appointment = _appointmentRepository.GetById(appointmentId);
            if (appointment == null) return false;

            int duration = appointment.AppointmentServices?.Sum(s => s.Service?.DurationMinutes ?? 0) ?? 0;

            if (IsTimeConflict(appointment.AppointmentDate, staffId, duration))
                return false;

            appointment.StatusId = 2; // Đã xác nhận
            appointment.EmployeeId = staffId;

            _appointmentRepository.Update(appointment);
            _appointmentRepository.Save();

            if (!string.IsNullOrEmpty(appointment.User?.Email))
            {
                var emailModel = BuildAppointmentConfirmationEmailModel(appointment);
                _emailService.SendAppointmentConfirmation(emailModel);
            }

            return true;
        }

        public ApproveAppointmentsViewModel GetPendingAppointmentsViewModel(string customer = "", string pet = "", string service = "", string status = "", int page = 1)
        {
            var appointments = _appointmentRepository.GetPendingAppointments();

            // Lọc theo các trường
            if (!string.IsNullOrWhiteSpace(customer))
                appointments = appointments.Where(a => (a.User?.FullName ?? "").ToLower().Contains(customer.ToLower())).ToList();
            if (!string.IsNullOrWhiteSpace(pet))
                appointments = appointments.Where(a => a.AppointmentPets.Any(p => (p.Pet?.Name ?? "").ToLower().Contains(pet.ToLower()))).ToList();
            if (!string.IsNullOrWhiteSpace(service))
                appointments = appointments.Where(a => a.AppointmentServices.Any(s => (s.Service?.Name ?? "").ToLower().Contains(service.ToLower()))).ToList();
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (int.TryParse(status, out int statusId))
                    appointments = appointments.Where(a => a.StatusId == statusId).ToList();
            }

            // Phân trang
            int pageSize = 12;
            var pagedAppointments = appointments.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var pendingAppointments = pagedAppointments.Select(appt => new AppointmentViewModel
            {
                AppointmentId = appt.AppointmentId,
                CustomerName = appt.User?.FullName ?? appt.User?.Username ?? "Khách hàng",
                AppointmentDate = appt.AppointmentDate,
                EndTime = appt.AppointmentDate.AddMinutes(
                    appt.AppointmentServices.Sum(s => s.Service?.DurationMinutes ?? 0)),
                SelectedServices = appt.AppointmentServices.Select(s => new Service
                {
                    Name = s.Service?.Name ?? ""
                }).ToList(),
                SelectedPets = appt.AppointmentPets.Select(p => new Pet
                {
                    Name = p.Pet?.Name ?? "",
                    Breed = p.Pet?.Breed ?? ""
                }).ToList(),

                StatusId = appt.StatusId,
                StatusName = appt.Status?.StatusName ?? "",

                EmployeeIds = (appt.EmployeeId ?? 0) > 0
                    ? new List<int> { appt.EmployeeId.GetValueOrDefault() }
                    : new List<int>(),

                DurationMinutes = appt.AppointmentServices.Sum(s => s.Service?.DurationMinutes ?? 0),

                CategoriesAdmin = new List<AppointmentViewModel.CategoryInfo>(),

                // Nếu có Staff gợi ý thì set ở đây (nếu cần)
                SuggestedStaffId = appt.EmployeeId > 0 ? appt.EmployeeId : (int?)null
            }).ToList();

            var staffList = _userRepository.GetStaffList();

            return new ApproveAppointmentsViewModel
            {
                PendingAppointments = pendingAppointments,
                StaffList = staffList,
                Statuses = GetAllStatuses()
            };
        }


        public int? AutoAssignStaffForAppointment(Appointment appointment)
        {
            var staffList = _userRepository.GetStaffList();
            int duration = appointment.AppointmentServices?.Sum(s => s.Service?.DurationMinutes ?? 0) ?? 0;

            foreach (var staff in staffList)
            {
                if (!IsTimeConflict(appointment.AppointmentDate, staff.UserId, duration))
                {
                    appointment.EmployeeId = staff.UserId;
                    appointment.StatusId = 2; // Đã xác nhận
                    _appointmentRepository.Update(appointment);
                    _appointmentRepository.Save();

                    if (!string.IsNullOrEmpty(appointment.User?.Email))
                    {
                        var emailModel = BuildAppointmentConfirmationEmailModel(appointment);
                        _emailService.SendAppointmentConfirmation(emailModel);
                    }

                    return staff.UserId;
                }
            }

            return null;
        }

        private void UpdateAppointmentAndSendEmail(Appointment appointment)
        {
            _appointmentRepository.Update(appointment);
            _appointmentRepository.Save();

            if (!string.IsNullOrEmpty(appointment.User?.Email))
            {
                var vm = BuildAppointmentConfirmationEmailModel(appointment);
                _emailService.SendAppointmentConfirmation(vm);
            }
        }

        public AppointmentConfirmationEmailModel BuildAppointmentConfirmationEmailModel(Appointment appointment)
        {
            return new AppointmentConfirmationEmailModel
            {
                ToEmail = appointment.User?.Email ?? string.Empty,
                CustomerName = appointment.User?.FullName ?? "Khách hàng",
                AppointmentDateTime = appointment.AppointmentDate
            };
        }

        public AppointmentViewModel BuildAppointmentViewModel(Appointment appointment)
        {
            if (appointment == null) return null;

            return new AppointmentViewModel
            {
                AppointmentId = appointment.AppointmentId,
                CustomerName = appointment.User?.FullName ?? appointment.User?.Username ?? "Khách hàng",
                Email = appointment.User?.Email ?? string.Empty,
                AppointmentDate = appointment.AppointmentDate,
                EndTime = appointment.AppointmentDate.AddMinutes(
                    appointment.AppointmentServices?.Sum(s => s.Service?.DurationMinutes ?? 0) ?? 0),
                StatusId = appointment.StatusId,
                StatusName = appointment.Status?.StatusName ?? string.Empty,
                Notes = appointment.Notes,
                SelectedPets =
                    appointment.AppointmentPets
                        ?.Select(ap => ap.Pet ?? new Pet { Name = string.Empty, Breed = string.Empty }).ToList() ??
                    new List<Pet>(),
                SelectedServices =
                    appointment.AppointmentServices
                        ?.Select(asv => asv.Service ?? new Service { Name = string.Empty, Price = 0 }).ToList() ??
                    new List<Service>(),
                EmployeeIds = appointment.EmployeeId > 0
                    ? new List<int> { appointment.EmployeeId.Value }
                    : new List<int>()
            };
        }
    }
}