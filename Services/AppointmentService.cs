using System.Collections;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Services
{
    public class AppointmentService : IAppointmentService
    {
        

        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPetRepository _petRepository;
        private readonly IEmailService _emailService;
        private readonly PetDataShopContext _context;
        private readonly IServiceService _serviceService;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IUserRepository userRepository,
            IPetRepository petRepository,
            IEmailService emailService,
            PetDataShopContext context,
            IServiceService serviceService)
        {
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
            _petRepository = petRepository;
            _emailService = emailService;
            _serviceService = serviceService;
            _context = context;
        }

        public List<StatusAppointment> GetValidNextStatuses(string currentStatus)
        {
            // Get all statuses from database
            var allStatuses = GetAllStatuses();

            // Get valid next status names from StatusAppointmentUtils
            var validNextStatusNames = Utils.StatusAppointmentUtils.GetValidNextStatuses(currentStatus);

            // Filter statuses that are valid transitions
            return allStatuses.Where(s => validNextStatusNames.Contains(s.StatusName)).ToList();
        }
        public List<User> GetEmployees()
        {
            // Stub: Trả về danh sách nhân viên
            return _appointmentRepository.GetEmployeeUsers();
        }

        public List<Pet> GetCustomerPets(int customerId)
        {
            // Trả về danh sách pet của khách hàng
            return _petRepository.GetPetsByUserId(customerId);
        }

        public bool RequestCancelAppointment(int appointmentId, int userId)
        {
            var appointment = _appointmentRepository.GetById(appointmentId);
            if (appointment == null) return false;

            var allStatuses = _appointmentRepository.GetAllStatuses();
            var currentStatus = allStatuses.FirstOrDefault(s => s.StatusId == appointment.StatusId);
            var pendingCancelStatus = allStatuses.FirstOrDefault(s => s.StatusName == "PendingCancel");

            if (currentStatus == null || pendingCancelStatus == null) return false;

            // Kiểm tra xem có thể chuyển sang trạng thái PendingCancel không
            if (!Utils.StatusAppointmentUtils.IsValidStatusTransition(currentStatus.StatusName, "PendingCancel"))
                return false;

            appointment.StatusId = pendingCancelStatus.StatusId;
            _appointmentRepository.SaveChanges();
            return true;
        }

        public AdminAppointmentDetailViewModel GetAdminAppointmentDetail(int appointmentId)

        {
            var appointment = _appointmentRepository.GetById(appointmentId);
            if (appointment == null) return new AdminAppointmentDetailViewModel();

            var selectedPets = MapAppointmentPets(appointment.AppointmentPets);
            var pets = MapPetInfos(appointment.AppointmentPets);
            var services = MapServiceInfos(appointment.AppointmentServices);

            decimal totalPrice = services.Sum(s => s.Price);
            int totalDuration = services.Sum(s => s.DurationMinutes ?? 0);

            DateTime appointmentDateTime = appointment.AppointmentDate.ToDateTime(appointment.StartTime);

            return new AdminAppointmentDetailViewModel
            {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointmentDateTime,
                Notes = appointment.Notes,
                StatusId = appointment.StatusId,
                StatusName = appointment.Status?.StatusName ?? string.Empty,
                TotalDurationMinutes = totalDuration,
                UserId = appointment.UserId,
                CustomerName = appointment.User?.FullName ?? string.Empty,
                CustomerPhone = appointment.User?.Phone ?? string.Empty,
                CustomerEmail = appointment.User?.Email ?? string.Empty,
                CustomerAddress = appointment.User?.Address ?? string.Empty,
                EmployeeName = appointment.AppointmentPets?.FirstOrDefault(ap => ap.Staff != null)?.Staff?.FullName ??
                               string.Empty,
                EmployeePhone = appointment.AppointmentPets?.FirstOrDefault(ap => ap.Staff != null)?.Staff?.Phone ??
                                string.Empty,
                EmployeeEmail = appointment.AppointmentPets?.FirstOrDefault(ap => ap.Staff != null)?.Staff?.Email ??
                                string.Empty,
                Pets = pets,
                Services = services,
                TotalPrice = totalPrice,
                CreatedAt = appointment.CreatedAt,
                SelectedPets = selectedPets,
            };
        }

        // Helper methods for mapping
        private List<AppointmentPetViewModel> MapAppointmentPets(IEnumerable<AppointmentPet>? appointmentPets)
        {
            return appointmentPets?.Select(ap => new AppointmentPetViewModel
            {
                PetId = ap.PetId,
                Name = ap.Pet?.Name ?? string.Empty,
                Breed = ap.Pet?.Breed ?? string.Empty,
                StaffName = ap.Staff?.FullName
            }).ToList() ?? new List<AppointmentPetViewModel>();
        }

        private List<PetInfo> MapPetInfos(IEnumerable<AppointmentPet>? appointmentPets)
        {
            return appointmentPets?.Select(p => new PetInfo
            {
                PetId = p.PetId,
                Name = p.Pet?.Name ?? string.Empty,
                SpeciesName = p.Pet?.Species?.SpeciesName ?? string.Empty,
                Breed = p.Pet?.Breed ?? string.Empty,
                Age = p.Pet?.Age ?? 0,
                Gender = p.Pet?.Gender
            }).ToList() ?? new List<PetInfo>();
        }

        private List<ServiceInfo> MapServiceInfos(IEnumerable<Models.AppointmentService>? appointmentServices)
        {
            return appointmentServices?.Select(s => new ServiceInfo
            {
                ServiceId = s.AppointmentServiceId,
                Name = s.Service?.Name ?? string.Empty,
                CategoryName = s.Service?.Category?.Name ?? string.Empty,
                Price = s.Service?.Price ?? 0,
                DurationMinutes = s.Service?.DurationMinutes ?? 0
            }).ToList() ?? new List<ServiceInfo>();
        }

        public object GetCalendarData()
        {
            // Truy vấn resources và events đã được tách sang repo
            var resources = _userRepository.GetStaffResources();
            
            // Lấy danh sách events và convert sang anonymous type
            var appointments = _appointmentRepository.GetAll()
                .Where(a => new[] { 2, 3, 4, 6 }.Contains(a.StatusId)) // Lọc theo status yêu cầu
                .Select(a => new
                {
                    id = a.AppointmentId,
                    resourceId = a.AppointmentPets?.FirstOrDefault()?.StaffId,
                    start = $"{a.AppointmentDate:yyyy-MM-dd}T{a.StartTime:HH:mm:ss}",
                    end = $"{a.AppointmentDate:yyyy-MM-dd}T{a.EndTime:HH:mm:ss}",
                    title = $"{a.User?.FullName ?? "Khách hàng"} - {string.Join(", ", a.AppointmentServices?.Select(s => s.Service?.Name) ?? Array.Empty<string>())}",
                    statusId = a.StatusId // Thêm statusId vào event
                })
                .ToList();

            return new { resources, events = appointments };
        }

        public ViewModel.CalendarViewModel GetCalendarViewModel()
        {
            var calendarViewModel = new CalendarViewModel
            {
                Resources = new List<StaffResourceViewModel>(),
                Events = new List<CalendarEventViewModel>()
            };

            // Get staff resources from repository
            var staffResources = _userRepository.GetStaffResources();
            if (staffResources != null)
            {
                foreach (var resource in staffResources)
                {
                    var resourceDict = resource.GetType().GetProperties()
                        .ToDictionary(prop => prop.Name, prop => prop.GetValue(resource));

                    calendarViewModel.Resources.Add(new StaffResourceViewModel
                    {
                        Id = resourceDict.ContainsKey("id")
                            ? resourceDict["id"]?.ToString() ?? string.Empty
                            : string.Empty,
                        Title = resourceDict.ContainsKey("title")
                            ? resourceDict["title"]?.ToString() ?? string.Empty
                            : string.Empty,
                        ImageUrl = resourceDict.ContainsKey("avatarUrl")
                            ? resourceDict["avatarUrl"]?.ToString() ?? string.Empty
                            : string.Empty,
                        BusinessHours = string.Empty // Default business hours can be added if needed
                    });
                }
            }

            // Get appointment events from repository
            var appointmentEvents = _appointmentRepository.GetCalendarEvents();
            if (appointmentEvents != null)
            {
                foreach (var evt in appointmentEvents)
                {
                    var evtDict = evt.GetType().GetProperties()
                        .ToDictionary(prop => prop.Name, prop => prop.GetValue(evt));

                    // Parse date strings
                    DateTime start = DateTime.Now, end = DateTime.Now;
                    if (evtDict.ContainsKey("start") && evtDict["start"] != null)
                    {
                        string? startStr = evtDict["start"]?.ToString();
                        if (!string.IsNullOrEmpty(startStr))
                        {
                            DateTime.TryParse(startStr, out start);
                        }
                    }

                    if (evtDict.ContainsKey("end") && evtDict["end"] != null)
                    {
                        string? endStr = evtDict["end"]?.ToString();
                        if (!string.IsNullOrEmpty(endStr))
                        {
                            DateTime.TryParse(endStr, out end);
                        }
                    }

                    // Get resource ID
                    string resourceId = string.Empty;
                    if (evtDict.ContainsKey("resourceIds") && evtDict["resourceIds"] != null)
                    {
                        if (evtDict["resourceIds"] is IEnumerable resourceIds)
                        {
                            // Convert each item to string
                            foreach (var id in resourceIds)
                            {
                                if (id != null)
                                {
                                    resourceId = id.ToString() ?? string.Empty;
                                    break;
                                }
                            }
                        }
                    }

                    // Create pet names list
                    var petNames = new List<string>();

                    // Create services list from services property
                    var servicesList = new List<string>();
                    if (evtDict.ContainsKey("services") && evtDict["services"] != null)
                    {
                        if (evtDict["services"] is IEnumerable services)
                        {
                            foreach (var service in services)
                            {
                                if (service != null)
                                {
                                    servicesList.Add(service.ToString() ?? string.Empty);
                                }
                            }
                        }
                    }

                    // Create event view model
                    calendarViewModel.Events.Add(new CalendarEventViewModel
                    {
                        Id = evtDict.ContainsKey("id") ? Convert.ToInt32(evtDict["id"] ?? 0) : 0,
                        Title = evtDict.ContainsKey("title")
                            ? evtDict["title"]?.ToString() ?? string.Empty
                            : string.Empty,
                        Start = start,
                        End = end,
                        Status = evtDict.ContainsKey("status")
                            ? evtDict["status"]?.ToString() ?? string.Empty
                            : string.Empty,
                        Customer = evtDict.ContainsKey("customer")
                            ? evtDict["customer"]?.ToString() ?? string.Empty
                            : string.Empty,
                        Phone = evtDict.ContainsKey("phone")
                            ? evtDict["phone"]?.ToString() ?? string.Empty
                            : string.Empty,
                        PetNames = petNames,
                        Services = servicesList,
                        ResourceId = resourceId,
                        Color = GetStatusColor(evtDict.ContainsKey("status")
                            ? evtDict["status"]?.ToString() ?? string.Empty
                            : string.Empty)
                    });
                }
            }

            return calendarViewModel;
        }

        // Helper method to get color based on appointment status
        private string GetStatusColor(string status)
        {
            return status?.ToLower() switch
            {
                "pending" => "#FFC107", // Yellow
                "approved" => "#4CAF50", // Green
                "completed" => "#2196F3", // Blue
                "cancelled" => "#F44336", // Red
                "rescheduled" => "#9C27B0", // Purple
                _ => "#757575" // Grey for unknown status
            };
        }

        public Appointment GetAppointmentById(int appointmentId)
        {
            var appointment = _appointmentRepository.GetById(appointmentId);
            return appointment ?? new Appointment();
        }

        public List<AdminAppointmentDetailViewModel> GetPendingApprovalAppointments()
        {
            return _appointmentRepository.GetPendingApprovalAppointments()
                .Select(a =>
                {
                    var detail = GetAdminAppointmentDetail(a.AppointmentId);
                    detail.AppointmentDate = a.AppointmentDate.ToDateTime(a.StartTime);
                    return detail;
                })
                .ToList();
        }

        public List<AdminAppointmentDetailViewModel> GetPendingAppointments()
        {
            return _appointmentRepository.GetPendingAppointments()
                .Select(a =>
                {
                    var detail = GetAdminAppointmentDetail(a.AppointmentId);
                    detail.AppointmentDate = a.AppointmentDate.ToDateTime(a.StartTime);
                    return detail;
                })
                .ToList();
        }

        public List<AdminAppointmentDetailViewModel> GetPendingCancelAppointments()
        {
            return _appointmentRepository.GetPendingCancelAppointments()
                .Select(a =>
                {
                    var detail = GetAdminAppointmentDetail(a.AppointmentId);
                    detail.AppointmentDate = a.AppointmentDate.ToDateTime(a.StartTime);
                    return detail;
                })
                .ToList();
        }

        public List<AppointmentViewModel> GetAppointmentsByStaffAndDate(int staffId, DateTime date)
        {
            var appointments = _appointmentRepository.GetAppointmentsByStaffAndDate(staffId, date);
            var result = appointments.Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                AppointmentDate = a.AppointmentDate,
                StartTime = a.StartTime,
                DurationMinutes = a.AppointmentServices.Sum(s => s.Service.DurationMinutes ?? 0),
                EndTime = TimeOnly.FromDateTime(a.AppointmentDate.ToDateTime(a.StartTime)
                    .AddMinutes(a.AppointmentServices.Sum(s => s.Service.DurationMinutes ?? 0))),
                StatusId = a.StatusId
            }).ToList();
            return result;
        }

        public RealtimeShiftViewModel GetManagementTimelineData(DateTime date)
        {
            try
            {
                Console.WriteLine($"[GetManagementTimelineData] Starting for date: {date}");

                // Khởi tạo ViewModel với giờ làm việc từ 8h đến 20h
                var viewModel = new RealtimeShiftViewModel
                {
                    Hours = Enumerable.Range(8, 13).ToList(),
                    StaffShifts = new List<StaffShift>()
                };

                Console.WriteLine(
                    $"[GetManagementTimelineData] Initialized hours: {string.Join(",", viewModel.Hours)}");

                // Lấy danh sách nhân viên
                var staffs = _userRepository.GetStaffList();
                Console.WriteLine($"[GetManagementTimelineData] Found {staffs.Count} staff members");

                if (staffs == null || staffs.Count == 0)
                {
                    Console.WriteLine(
                        "[GetManagementTimelineData] No staff members found. Check if RoleId is correct in database.");
                    return viewModel;
                }

                // Lấy tất cả cuộc hẹn trong ngày
                var dateOnly = DateOnly.FromDateTime(date);
                var appointments = _appointmentRepository.GetAppointmentsByDateAndStatus(
                    dateOnly,
                    new[] { 2, 3, 4, 6 } // Approved, InProgress, Completed, PendingCancel
                );
                Console.WriteLine(
                    $"[GetManagementTimelineData] Found {appointments.Count} appointments for date {dateOnly}");

                foreach (var staff in staffs)
                {
                    var staffShift = new StaffShift
                    {
                        UserId = staff.UserId,
                        StaffId = staff.UserId,
                        StaffName = staff.FullName ?? "Unknown",
                        AvatarUrl = staff.ProfilePictureUrl ?? "",
                        HourStatus = new Dictionary<int, ShiftStatus>(),
                        Appointments = new List<AppointmentViewModel>()
                    };

                    // Lấy các cuộc hẹn của nhân viên này
                    var staffAppointments = appointments
                        .Where(a => a.AppointmentPets.Any(ap => ap.StaffId == staff.UserId))
                        .ToList();

                    foreach (var appointment in staffAppointments)
                    {
                        var appointmentVm = new AppointmentViewModel
                        {
                            AppointmentId = appointment.AppointmentId,
                            AppointmentDate = appointment.AppointmentDate,
                            StartTime = appointment.StartTime,
                            EndTime = appointment.EndTime,
                            StatusId = appointment.StatusId,
                            PetStaffAssignments = appointment.AppointmentPets
                                .Where(ap => ap.StaffId == staff.UserId)
                                .Select(ap => new PetStaffAssignViewModel
                                {
                                    PetId = ap.PetId,
                                    PetName = ap.Pet?.Name ?? "(Chưa đặt tên)",
                                    StaffId = ap.StaffId,
                                    OwnerName = ap.Pet.User?.FullName ?? "(Chưa xác định)",
                                }).ToList()
                        };

                        staffShift.Appointments.Add(appointmentVm);
                    }

                    viewModel.StaffShifts.Add(staffShift);
                }

                return viewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetManagementTimelineData: {ex.Message}");
                throw;
            }
        }

        public bool IsTimeConflict(DateTime appointmentDate, int staffId, int durationMinutes)
        {
            // Chuẩn hóa kiểm tra trùng lịch cho từng pet/staff
            var appointmentIds = _appointmentRepository.GetAppointmentIdsByStaff(staffId);
            var appointments = _appointmentRepository.GetAppointmentsByIds(appointmentIds);
            var appointmentStart = appointmentDate;
            var appointmentEnd = appointmentDate.AddMinutes(durationMinutes);
            foreach (var appt in appointments)
            {
                var apptStart = appt.AppointmentDate.ToDateTime(appt.StartTime);
                var apptEnd = apptStart.AddMinutes(appt.AppointmentServices.Sum(s => s.Service?.DurationMinutes ?? 0));
                if (appointmentStart < apptEnd && appointmentEnd > apptStart)
                    return true;
            }

            return false;
        }

        public ApproveAppointmentsViewModel GetPendingAppointmentsViewModel(string customer = "", string pet = "",
            string service = "", string status = "")
        {
            var appointments = _appointmentRepository.GetPendingAppointments();

            // Lọc theo các trường
            if (!string.IsNullOrWhiteSpace(customer))
                appointments = appointments.Where(a => (a.User?.FullName ?? "").ToLower().Contains(customer.ToLower()))
                    .ToList();
            if (!string.IsNullOrWhiteSpace(pet))
                appointments = appointments.Where(a =>
                    a.AppointmentPets.Any(p => (p.Pet?.Name ?? "").ToLower().Contains(pet.ToLower()))).ToList();
            if (!string.IsNullOrWhiteSpace(service))
                appointments = appointments.Where(a =>
                        a.AppointmentServices.Any(s => (s.Service?.Name ?? "").ToLower().Contains(service.ToLower())))
                    .ToList();
            if (!string.IsNullOrWhiteSpace(status) && int.TryParse(status, out int statusId))
            {
                appointments = appointments.Where(a => a.StatusId == statusId).ToList();
            }

            // Không phân trang, trả về toàn bộ danh sách đã lọc
            var pendingAppointments = appointments.Select(appt => new AppointmentViewModel
            {
                AppointmentId = appt.AppointmentId,
                CustomerName = appt.User?.FullName ?? appt.User?.Username ?? "Khách hàng",
                AppointmentDate = appt.AppointmentDate,
                StartTime = appt.StartTime,
                EndTime = TimeOnly.FromDateTime(appt.AppointmentDate.ToDateTime(appt.StartTime).AddMinutes(
                    appt.AppointmentServices.Sum(s => s.Service?.DurationMinutes ?? 0))),
                SelectedServices = appt.AppointmentServices.Select(s => new AppointmentServiceInfo
                {
                    ServiceId = s.ServiceId,
                    Name = s.Service?.Name ?? string.Empty,
                    CategoryName = s.Service?.Category?.Name ?? string.Empty,
                    Price = s.Service?.Price ?? 0,
                    DurationMinutes = s.Service?.DurationMinutes ?? 0
                }).ToList(),
                SelectedPets = appt.AppointmentPets.Select(p => new AppointmentPetViewModel
                {
                    PetId = p.PetId,
                    Name = p.Pet?.Name ?? string.Empty,
                    Breed = p.Pet?.Breed ?? string.Empty
                }).ToList(),

                StatusId = appt.StatusId,
                StatusName = appt.Status?.StatusName ?? "",

                DurationMinutes = appt.AppointmentServices.Sum(s => s.Service?.DurationMinutes ?? 0),
            }).ToList();

            var staffList = _userRepository.GetStaffList();

            return new ApproveAppointmentsViewModel
            {
                PendingAppointments = pendingAppointments,
                StaffList = staffList,
                Statuses = _appointmentRepository.GetAllStatuses()
            };
        }


        public AppointmentViewModel PrepareEditViewModel(int id)
        {
            var appointment = _appointmentRepository.GetById(id);
            if (appointment == null) return new AppointmentViewModel();

            // Lấy danh sách nhân viên (RoleId = 3)
            var model = new AppointmentViewModel
            {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Notes = appointment.Notes,
                StatusId = appointment.StatusId,
                CustomerId = appointment.UserId,
                CustomerName = appointment.User?.FullName ?? string.Empty,
                SelectedPetIds = appointment.AppointmentPets?.Select(p => p.PetId).ToList() ?? new List<int>(),
                SelectedPets = appointment.AppointmentPets?.Select(p => new AppointmentPetViewModel
                {
                    PetId = p.PetId,
                    Name = p.Pet?.Name ?? string.Empty,
                    Breed = p.Pet?.Breed ?? string.Empty
                }).ToList() ?? new List<AppointmentPetViewModel>(),
                SelectedServiceIds = appointment.AppointmentServices?.Select(s => s.ServiceId).ToList() ??
                                     new List<int>(),
                SelectedServices = appointment.AppointmentServices?.Select(s => new AppointmentServiceInfo
                {
                    ServiceId = s.ServiceId,
                    Name = s.Service?.Name ?? string.Empty,
                    CategoryName = s.Service?.Category?.Name ?? string.Empty,
                    Price = s.Service?.Price ?? 0,
                    DurationMinutes = s.Service?.DurationMinutes ?? 0
                }).ToList() ?? new List<AppointmentServiceInfo>(),
                AllPets = _petRepository.GetPetsByUserId(appointment.UserId)
                    .Select(p => new PetInfo { PetId = p.PetId, Name = p.Name }).ToList(),
                AllServices = _appointmentRepository.GetAllServices(),
                Statuses = _appointmentRepository.GetAllStatuses(),
            };
            return model;
        }

        public T BuildAppointmentEmailModel<T>(Appointment appointment) where T : new()
        {
            var model = new T();
            var props = typeof(T).GetProperties();

            void Set<TValue>(string name, TValue value)
            {
                var prop = props.FirstOrDefault(p => p.Name == name);
                if (prop != null && prop.CanWrite)
                    prop.SetValue(model, value);
            }

            Set("ToEmail", appointment.User?.Email ?? string.Empty);
            Set("CustomerName", appointment.User?.FullName ?? "Khách hàng");
            Set("AppointmentDateTime", appointment.AppointmentDate);
            Set("EndTime", TimeOnly.FromDateTime(appointment.AppointmentDate.ToDateTime(appointment.StartTime)
                .AddMinutes(
                    appointment.AppointmentServices?.Sum(s => s.Service?.DurationMinutes ?? 0) ?? 0)));
            Set("StatusName", appointment.Status?.StatusName ?? string.Empty);
            Set("Notes", appointment.Notes ?? string.Empty);
            Set("SelectedPets", appointment.AppointmentPets?
                .Where(ap => ap.Pet != null)
                .Select(ap => ap.Pet)
                .ToList() ?? new List<Pet>());
            Set("SelectedServices", appointment.AppointmentServices?
                .Where(asv => asv.Service != null)
                .Select(asv => asv.Service)
                .ToList() ?? new List<Service>());

            // Nếu là AppointmentRejectedEmailModel thì có ProposedDateTime
            if (typeof(T).GetProperty("ProposedDateTime") != null)
                Set("ProposedDateTime", appointment.AppointmentDate);

            return model;
        }

        public AppointmentViewModel BuildAppointmentViewModel(Appointment appointment)
        {
            if (appointment == null) return new AppointmentViewModel();

            return new AppointmentViewModel
            {
                AppointmentId = appointment.AppointmentId,
                CustomerName = appointment.User?.FullName ?? appointment.User?.Username ?? "Khách hàng",
                Email = appointment.User?.Email ?? string.Empty,
                AppointmentDate = appointment.AppointmentDate,
                EndTime = TimeOnly.FromDateTime(appointment.AppointmentDate.ToDateTime(appointment.StartTime)
                    .AddMinutes(
                        appointment.AppointmentServices?.Sum(s => s.Service?.DurationMinutes ?? 0) ?? 0)),
                StatusId = appointment.StatusId,
                StatusName = appointment.Status?.StatusName ?? string.Empty,
                Notes = appointment.Notes,
                SelectedPets = appointment.AppointmentPets != null
                    ? appointment.AppointmentPets.Select(ap => new AppointmentPetViewModel
                    {
                        PetId = ap.PetId,
                        Name = ap.Pet?.Name ?? string.Empty,
                        Breed = ap.Pet?.Breed ?? string.Empty
                    }).ToList()
                    : new List<AppointmentPetViewModel>(),
                SelectedServices = appointment.AppointmentServices != null
                    ? appointment.AppointmentServices.Select(asv => new AppointmentServiceInfo
                    {
                        ServiceId = asv.ServiceId,
                        Name = asv.Service?.Name ?? string.Empty,
                        CategoryName = asv.Service?.Category?.Name ?? string.Empty,
                        Price = asv.Service?.Price ?? 0,
                        DurationMinutes = asv.Service?.DurationMinutes ?? 0
                    }).ToList()
                    : new List<AppointmentServiceInfo>(),
                // Không còn EmployeeIds ở appointment, chỉ lấy staff từ AppointmentPet nếu cần
                // Không còn EmployeeIds ở appointment, chỉ lấy staff từ AppointmentPet nếu cần
                // EmployeeIds = new List<int>()
            };
        }

        public List<User> GetCustomers()
        {
            return _appointmentRepository.GetCustomers();
        }

        public List<Pet> GetAllPets()
        {
            return _appointmentRepository.GetAllPets();
        }

        public List<Service> GetAllServices()
        {
            return _appointmentRepository.GetAllServices();
        }
        

        public List<User> GetAllCustomersAndStaffs()
        {
            var customers = _appointmentRepository.GetCustomers();
            var staffs = _appointmentRepository.GetEmployeeUsers();
            return customers.Concat(staffs).ToList();
        }

        public Pet GetPetById(int petId)
        {
            return _petRepository.GetById(petId);
        }

        public AppointmentPet GetAppointmentPet(int appointmentId, int petId)
        {
            return _appointmentRepository.GetAppointmentPet(appointmentId, petId);
        }

        public bool UpdateAppointmentWithPetStaff(AppointmentViewModel vm)
        {
            try
            {
                var appointment = _appointmentRepository.GetById(vm.AppointmentId);
                if (appointment == null)
                {
                    Console.WriteLine($"[UpdateAppointmentWithPetStaff] Không tìm thấy lịch hẹn với ID: {vm.AppointmentId}");
                    return false;
                }

                // Validate status change
                if (appointment.StatusId != vm.StatusId)
                {
                    var allStatuses = _appointmentRepository.GetAllStatuses();
                    var currentStatus = allStatuses.FirstOrDefault(s => s.StatusId == appointment.StatusId);
                    var newStatus = allStatuses.FirstOrDefault(s => s.StatusId == vm.StatusId);

                    if (currentStatus == null || newStatus == null)
                    {
                        Console.WriteLine($"[UpdateAppointmentWithPetStaff] Trạng thái không hợp lệ!");
                        return false;
                    }

                    // Kiểm tra xem việc chuyển trạng thái có hợp lệ không
                    if (!Utils.StatusAppointmentUtils.IsValidStatusTransition(currentStatus.StatusName, newStatus.StatusName))
                    {
                        Console.WriteLine($"[UpdateAppointmentWithPetStaff] Không thể thay đổi trạng thái từ '{currentStatus.StatusName}' sang '{newStatus.StatusName}'!");
                        return false;
                    }
                }

                // Cập nhật thông tin cơ bản
                appointment.AppointmentDate = vm.AppointmentDate;
                appointment.StartTime = vm.StartTime;
                appointment.EndTime = vm.EndTime;
                appointment.StatusId = vm.StatusId;
                appointment.Notes = vm.Notes;
                // Không cần gọi Update, chỉ cần SaveChanges vì entity đã được sửa trực tiếp

                // Ghi log thông tin debug
                Console.WriteLine($"[UpdateAppointmentWithPetStaff] Cập nhật lịch hẹn ID: {vm.AppointmentId}");
                Console.WriteLine($"[UpdateAppointmentWithPetStaff] Số lượng phân công: {vm.PetStaffAssignments?.Count ?? 0}");
                
                // Chuẩn hóa: mỗi pet gán 1 staff
                if (vm.PetStaffAssignments != null && vm.PetStaffAssignments.Any())
                {
                    foreach (var assign in vm.PetStaffAssignments)
                    {
                        if (assign.PetId <= 0)
                        {
                            Console.WriteLine($"[UpdateAppointmentWithPetStaff] Bỏ qua phân công có PetId không hợp lệ: {assign.PetId}");
                            continue;
                        }

                        int staffIdToAssign = assign.StaffId ?? 0;
                        Console.WriteLine($"[UpdateAppointmentWithPetStaff] Đang cập nhật: AppointmentID={vm.AppointmentId}, PetID={assign.PetId}, StaffID={staffIdToAssign}");
                        _appointmentRepository.UpdateAppointmentPetStaff(vm.AppointmentId, assign.PetId, staffIdToAssign);
                    }
                }
                else
                {
                    Console.WriteLine($"[UpdateAppointmentWithPetStaff] Không có phân công nhân viên nào được gửi lên");
                }

                // Lưu các thay đổi vào database
                _appointmentRepository.SaveChanges();
                Console.WriteLine($"[UpdateAppointmentWithPetStaff] Đã lưu thành công tất cả thay đổi");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateAppointmentWithPetStaff] Lỗi: {ex.Message}");
                Console.WriteLine($"[UpdateAppointmentWithPetStaff] Stack Trace: {ex.StackTrace}");
                return false;
            }
        }

        public bool RestoreAppointment(int id)
        {
            try
            {
                _appointmentRepository.RestoreAppointment(id);
                _appointmentRepository.RestoreAppointmentPets(id);
                _appointmentRepository.RestoreAppointmentServices(id);
                _appointmentRepository.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restoring appointment: {ex.Message}");
                return false;
            }
        }

        public void SendAppointmentNotificationMail(int appointmentId, string type, object? model = null)
        {
            //hãy triển khai body cho hàm logic gửi email
            var appointment = _appointmentRepository.GetById(appointmentId);
            if (appointment == null) return;
        }

        public (bool Success, int AppointmentId) SaveAppointment(AppointmentViewModel model, int userId)
        {   
            Console.WriteLine($"[SaveAppointment] Bắt đầu lưu lịch hẹn cho user {userId}");
            Console.WriteLine($"[SaveAppointment] PetIds: {string.Join(", ", model.SelectedPetIds ?? new List<int>())}");
            Console.WriteLine($"[SaveAppointment] ServiceIds: {string.Join(", ", model.SelectedServiceIds ?? new List<int>())}");
            Console.WriteLine($"[SaveAppointment] Ngày: {model.AppointmentDate}, Giờ bắt đầu: {model.StartTime}");
            
            try
            {
                // Tính toán EndTime trước khi lưu
                if (model.SelectedServices != null && model.SelectedServices.Count > 0)
                {
                    model.CalculateEndTime(model.SelectedServices);
                }
                else if (model.SelectedServiceIds != null && model.SelectedServiceIds.Count > 0)
                {
                    // Lấy thông tin services để tính EndTime
                    var services = _appointmentRepository.GetAllServices()
                        .Where(s => model.SelectedServiceIds.Contains(s.ServiceId))
                        .Select(s => new AppointmentServiceInfo
                        {
                            ServiceId = s.ServiceId,
                            DurationMinutes = s.DurationMinutes
                        }).ToList();
                    model.CalculateEndTime(services);
                }

                // Lấy danh sách petId
               
                // Kiểm tra trùng lịch
                var conflicts = CheckPetAppointment(
                    model.SelectedPetIds,
                    model.AppointmentDate,
                    model.StartTime,
                    model.EndTime
                );
                if (conflicts.Any())
                {
                    Console.WriteLine($"[CheckPetAppointment] Có {conflicts.Count} lịch trùng!");
                    foreach (var c in conflicts)
                    {
                        Console.WriteLine($"Pet: {c.PetName} | Lịch trùng: {c.ConflictingStartTime:dd/MM/yyyy HH:mm} - {c.ConflictingEndTime:HH:mm}");
                    }
                    // Có trùng lịch, không lưu và trả về false
                    return (false, 0);
                }

                // Tạo mới entity Appointment
                var appointment = new Appointment
                {
                    UserId = userId,
                    AppointmentDate = model.AppointmentDate,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime, // ✅ Thêm EndTime
                    StatusId = 1, // Pending
                    Notes = model.Notes
                };
                // Lưu vào DB
                int appointmentId = _appointmentRepository.AddAppointment(appointment);
                model.AppointmentId = appointmentId; // Cập nhật ID vào model

                // Lưu từng pet với staff
                if (model.PetStaffAssignments != null && model.PetStaffAssignments.Count > 0)
                {
                    foreach (var assign in model.PetStaffAssignments)
                    {
                        _appointmentRepository.AddAppointmentPet(appointmentId, assign.PetId, assign.StaffId);
                    }
                }
                else if (model.SelectedPetIds != null && model.SelectedPetIds.Count > 0)
                {
                    // Nếu chưa có phân staff, chỉ lưu pet
                    foreach (var petId in model.SelectedPetIds)
                    {
                        _appointmentRepository.AddAppointmentPet(appointmentId, petId, null);
                    }
                }

                // Lưu dịch vụ
                if (model.SelectedServiceIds != null && model.SelectedServiceIds.Count > 0)
                {
                    foreach (var serviceId in model.SelectedServiceIds)
                    {
                        _appointmentRepository.AddAppointmentService(appointmentId, serviceId);
                    }
                }

                _appointmentRepository.SaveChanges();
                Console.WriteLine($"[SaveAppointment] Đã lưu lịch hẹn thành công với ID: {appointmentId}");
                return (true, appointmentId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] SaveAppointment: {ex.Message}");
                return (false, 0);
            }
        }

        public AppointmentHistoryViewModel GetAppointmentHistory(int userId)
        {
            // Lấy tất cả lịch hẹn của user
            var appointments = _appointmentRepository.GetAppointments(new AppointmentFilter
                {
                    Customer = string.Empty,
                    Pet = string.Empty,
                    Service = string.Empty,
                    StaffId = null,
                    Date = null,
                    Page = 1,
                    PageSize = 1000 // lấy hết
                })
                .Where(a => a.UserId == userId && (a.IsActive == null || a.IsActive == true))
                .ToList();

            var items = appointments.Select(a => new AppointmentHistoryItemViewModel
            {
                AppointmentId = a.AppointmentId,
                AppointmentDate = a.AppointmentDate.ToDateTime(a.StartTime),
                StatusId = a.StatusId,
                StatusName = a.Status?.StatusName ?? string.Empty,
                PetNames = a.AppointmentPets?.Select(p => p.Pet?.Name ?? "").ToList() ?? new List<string>(),
                Notes = a.Notes ?? string.Empty,
                Services = a.AppointmentServices?.Select(s => new ServiceHistoryInfo
                {
                    ServiceId = s.ServiceId,
                    Name = s.Service?.Name ?? string.Empty,
                    Price = s.Service?.Price ?? 0
                }).ToList() ?? new List<ServiceHistoryInfo>()
            }).ToList();

            return new AppointmentHistoryViewModel
            {
                Appointments = items,
                Statuses = _appointmentRepository.GetAllStatuses()
            };
        }

        public bool IsTimeConflict(DateOnly appointmentDate, TimeOnly startTime, int staffId, int durationMinutes)
        {
            var appointmentStart = appointmentDate.ToDateTime(startTime);
            return IsTimeConflict(appointmentStart, staffId, durationMinutes);
        }

        public AppointmentDashboardViewModel GetDashboardViewModel()
        {
            var today = DateTime.Today;
            var model = new AppointmentDashboardViewModel();

            // Tổng số lịch hôm nay
            model.TodayAppointments = _appointmentRepository.GetAppointmentsByDateRange(today, today).Count;

            // Tổng số lịch sắp tới (ngày lớn hơn hôm nay)
            model.UpcomingAppointments = _appointmentRepository
                .GetAppointmentsByDateRange(today.AddDays(1), today.AddYears(1)).Count;

            // Lịch chờ duyệt
            model.PendingApprovalAppointments = _appointmentRepository.GetAppointmentsByStatus(1).Count; // 1: Pending
            // Lịch chờ duyệt hủy
            model.PendingCancelAppointments =
                _appointmentRepository.GetAppointmentsByStatus(3).Count; // 3: PendingCancel

            // Lấy 10 lịch gần nhất hôm nay
            model.RecentAppointments = _appointmentRepository.GetAppointmentsByDateRange(today, today)
                .OrderBy(a => a.StartTime)
                .Take(10)
                .Select(a => new AppointmentDashboardItemViewModel
                {
                    AppointmentId = a.AppointmentId,
                    CustomerName = a.User?.FullName,
                    AppointmentDate = a.AppointmentDate.ToDateTime(a.StartTime),
                    PetNames = string.Join(", ", a.AppointmentPets.Select(p => p.Pet.Name)),
                    ServiceNames = string.Join(", ", a.AppointmentServices.Select(s => s.Service.Name)),
                    StatusId = a.StatusId,
                    StatusName = a.Status?.StatusName
                })
                .ToList();

            // Thống kê theo tháng
            var year = today.Year;
            var monthlyStats = _appointmentRepository.GetMonthlyStats(year);
            model.MonthlyStats = new List<AppointmentDashboardMonthStat>();
            for (int m = 1; m <= 12; m++)
            {
                var stat = monthlyStats.FirstOrDefault(x => x.Month == m);
                model.MonthlyStats.Add(new AppointmentDashboardMonthStat
                {
                    Month = m,
                    MonthLabel = $"Tháng {m}",
                    AppointmentCount = stat?.TotalAppointments ?? 0
                });
            }

            return model;
        }

        // Lấy danh sách lịch hẹn với filter/phân trang
        public List<AppointmentViewModel> GetAppointments(AppointmentFilter filter)
        {
            var appointments = _appointmentRepository.GetAppointments(filter);
            return appointments.Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                CustomerName = a.User?.FullName ?? a.User?.Username ?? "Khách hàng",
                AppointmentDate = a.AppointmentDate,
                EndTime = TimeOnly.FromDateTime(a.AppointmentDate.ToDateTime(a.StartTime).AddMinutes(
                    a.AppointmentServices?.Sum(s => s.Service?.DurationMinutes ?? 0) ?? 0)),
                StatusId = a.StatusId,
                StatusName = a.Status?.StatusName ?? string.Empty,
                Notes = a.Notes,
                SelectedPets = a.AppointmentPets != null
                    ? a.AppointmentPets.Select(ap => new AppointmentPetViewModel
                    {
                        PetId = ap.PetId,
                        Name = ap.Pet?.Name ?? string.Empty,
                        Breed = ap.Pet?.Breed ?? string.Empty
                    }).ToList()
                    : new List<AppointmentPetViewModel>(),
                SelectedServices = a.AppointmentServices != null
                    ? a.AppointmentServices.Select(asv => new AppointmentServiceInfo
                    {
                        ServiceId = asv.ServiceId,
                        Name = asv.Service?.Name ?? string.Empty,
                        CategoryName = asv.Service?.Category?.Name ?? string.Empty,
                        Price = asv.Service?.Price ?? 0,
                        DurationMinutes = asv.Service?.DurationMinutes ?? 0
                    }).ToList()
                    : new List<AppointmentServiceInfo>(),
                DurationMinutes = a.AppointmentServices?.Sum(s => s.Service?.DurationMinutes ?? 0) ?? 0,

                // Bổ sung các property navigation/entity cho view
                User = a.User,
                AppointmentPets = a.AppointmentPets?.ToList(),
                AppointmentServices = a.AppointmentServices?.ToList(),
                IsActive = a.IsActive,
                Status = a.Status,
                // Lấy staff từ AppointmentPet thay vì Employee
                Employee = a.AppointmentPets?.FirstOrDefault(ap => ap.Staff != null)?.Staff,

                // Map DateTime for view formatting
                AppointmentDateTime = a.AppointmentDate.ToDateTime(a.StartTime)
            }).ToList();
        }

        public int CountAppointments(AppointmentFilter filter)
        {
            return _appointmentRepository.CountAppointments(filter);
        }

        public List<StatusAppointment> GetAllStatuses()
        {
            return _appointmentRepository.GetAllStatuses();
        }

        public void DeleteAppointment(int id)
        {
            // Xóa mềm lịch hẹn và các liên kết
            _appointmentRepository.Delete(id);
            _appointmentRepository.DeleteAppointmentPets(id);
            _appointmentRepository.DeleteAppointmentServices(id);
            _appointmentRepository.SaveChanges();
        }

        public ServiceResult ApproveAndAssignStaff(int appointmentId, int staffId)
        {
            var appointment = _appointmentRepository.GetById(appointmentId);
            if (appointment == null)
                return new ServiceResult { Success = false, Message = "Không tìm thấy lịch hẹn!" };

            // Cập nhật trạng thái thành Confirmed
            appointment.StatusId = 2;
            // Không cần gọi Update riêng vì entity đã được theo dõi bởi DbContext
            // và sẽ được cập nhật khi gọi SaveChanges()

            // Gán staff cho tất cả pets
            var appointmentPets = _appointmentRepository.GetAppointmentPets(appointmentId);
            foreach (var ap in appointmentPets)
            {
                _appointmentRepository.UpdateAppointmentPetStaff(appointmentId, ap.PetId, staffId);
            }

            _appointmentRepository.SaveChanges();
            return new ServiceResult { Success = true, Message = "Đã duyệt và gán nhân viên thành công!" };
        }

        public ServiceResult QuickUpdateStatus(int appointmentId, int statusId)
        {
            var appointment = _appointmentRepository.GetById(appointmentId);
            if (appointment == null)
                return new ServiceResult { Success = false, Message = "Không tìm thấy lịch hẹn!" };

            var allStatuses = _appointmentRepository.GetAllStatuses();
            var currentStatus = allStatuses.FirstOrDefault(s => s.StatusId == appointment.StatusId);
            var newStatus = allStatuses.FirstOrDefault(s => s.StatusId == statusId);

            if (currentStatus == null || newStatus == null)
                return new ServiceResult { Success = false, Message = "Trạng thái không hợp lệ!" };

            // Kiểm tra xem việc chuyển trạng thái có hợp lệ không
            if (!Utils.StatusAppointmentUtils.IsValidStatusTransition(currentStatus.StatusName, newStatus.StatusName))
                return new ServiceResult { Success = false, Message = $"Không thể thay đổi trạng thái từ '{currentStatus.StatusName}' sang '{newStatus.StatusName}'!" };

            appointment.StatusId = statusId;
            _appointmentRepository.SaveChanges();

            return new ServiceResult { Success = true, Message = "Cập nhật trạng thái thành công!" };
        }

        public AppointmentHistoryItemViewModel GetAppointmentDetailWithPetImages(int appointmentId, int userId)
        {
            // Lấy lịch hẹn theo id và user
            var appointment = _context.Appointments
                .FirstOrDefault(a => a.AppointmentId == appointmentId && a.UserId == userId);
            if (appointment == null) return null;

            // Lấy thú cưng trong lịch hẹn
            var pets = _context.AppointmentPets
                .Where(ap => ap.AppointmentId == appointmentId)
                .Select(ap => ap.Pet)
                .ToList();
            var petNames = pets.Select(p => p.Name).ToList();

            // Lấy danh sách AppointmentService kèm service và status
            var appointmentServices = _context.AppointmentServices
                .Where(asv => asv.AppointmentId == appointmentId)
                .Select(asv => new
                {
                    asv.Service,
                    asv.Status,
                    StatusName = asv.StatusNavigation.StatusName,
                    AppointmentServiceId = asv.AppointmentServiceId,
                    Images = asv.AppointmentServiceImages.Select(img => new
                    {
                        img.ImgUrl,
                        img.PhotoType
                    }).ToList()
                })
                .ToList();

            var serviceHistory = appointmentServices.Select(s => new ServiceHistoryInfo
            {
                ServiceId = s.Service.ServiceId,
                Name = s.Service.Name,
                Category = s.Service.Category?.Name ?? "",
                Price = s.Service.Price,
                Duration = s.Service.DurationMinutes ?? 0,
                StatusId = s.Status ?? 0,
                StatusName = s.StatusName ?? "",
                PetImagesBefore = s.Images.Where(i => i.PhotoType == "Before").Select(i => i.ImgUrl).ToList(),
                PetImagesAfter = s.Images.Where(i => i.PhotoType == "After").Select(i => i.ImgUrl).ToList()
            }).ToList();

            return new AppointmentHistoryItemViewModel
            {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate.ToDateTime(appointment.StartTime),
                StartTime = appointment.StartTime.ToTimeSpan(),
                EndTime = appointment.EndTime.ToTimeSpan(),
                StatusId = appointment.StatusId,
                StatusName = appointment.Status?.StatusName ?? string.Empty,
                PetNames = petNames,
                Notes = appointment.Notes,
                Services = serviceHistory
            };
        }


        public List<PetConflictInfo> CheckPetAppointment(List<int> petIds, DateTime startDateTime, DateTime endDateTime,
            int? excludeAppointmentId = null)
        {
            Console.WriteLine($"[CheckPetAppointment] Bắt đầu kiểm tra với DateTime");
            Console.WriteLine($"[CheckPetAppointment] StartDateTime: {startDateTime:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"[CheckPetAppointment] EndDateTime: {endDateTime:dd/MM/yyyy HH:mm}");
            
            if (petIds == null || !petIds.Any())
            {
                Console.WriteLine("[CheckPetAppointment] Không có petIds để kiểm tra");
                return new List<PetConflictInfo>();
            }

            var conflicts = new List<PetConflictInfo>();

            // Lấy tất cả lịch hẹn của các pet trong khoảng thời gian chỉ định
            var conflictingAppointments = _context.AppointmentPets
                .Where(ap => petIds.Contains(ap.PetId) && 
                            ap.IsActive != false && // Chỉ lấy lịch hẹn còn hoạt động
                            ap.Appointment.IsActive != false && // Chỉ lấy lịch hẹn còn hoạt động
                            ap.Appointment.StatusId != 5) // Loại trừ lịch đã từ chối (Rejected)
                .Select(ap => new
                {
                    ap.PetId,
                    ap.Pet.Name,
                    ap.Pet.Breed,
                    ap.AppointmentId,
                    ap.Appointment.AppointmentDate,
                    ap.Appointment.StartTime,
                    ap.Appointment.EndTime,
                    ap.Appointment.Status.StatusName,
                    ap.Appointment.User.FullName
                })
                .ToList();

            Console.WriteLine($"[CheckPetAppointment] Tìm thấy {conflictingAppointments.Count} lịch hẹn của các pet");

            // Loại trừ lịch hẹn hiện tại nếu đang cập nhật
            if (excludeAppointmentId.HasValue)
            {
                conflictingAppointments = conflictingAppointments
                    .Where(ca => ca.AppointmentId != excludeAppointmentId.Value)
                    .ToList();
                Console.WriteLine($"[CheckPetAppointment] Sau khi loại trừ appointment {excludeAppointmentId.Value}, còn {conflictingAppointments.Count} lịch hẹn");
            }

            // Kiểm tra từng lịch hẹn có trùng thời gian không
            foreach (var appointment in conflictingAppointments)
            {
                var appointmentStart = appointment.AppointmentDate.ToDateTime(appointment.StartTime);
                var appointmentEnd = appointment.AppointmentDate.ToDateTime(appointment.EndTime);

                Console.WriteLine($"[CheckPetAppointment] Kiểm tra lịch {appointment.AppointmentId}: {appointmentStart:dd/MM/yyyy HH:mm} - {appointmentEnd:HH:mm}");

                // Kiểm tra xem có trùng thời gian không
                if (startDateTime < appointmentEnd && endDateTime > appointmentStart)
                {
                    Console.WriteLine($"[CheckPetAppointment] PHÁT HIỆN TRÙNG LỊCH! Pet: {appointment.Name}");
                    conflicts.Add(new PetConflictInfo
                    {
                        PetId = appointment.PetId,
                        PetName = appointment.Name ?? string.Empty,
                        Breed = appointment.Breed ?? string.Empty,
                        ConflictingAppointmentId = appointment.AppointmentId,
                        ConflictingStartTime = appointmentStart,
                        ConflictingEndTime = appointmentEnd,
                        CustomerName = appointment.FullName ?? string.Empty,
                        StatusName = appointment.StatusName ?? string.Empty
                    });
                }
                else
                {
                    Console.WriteLine($"[CheckPetAppointment] Không trùng lịch");
                }
            }

            Console.WriteLine($"[CheckPetAppointment] Tổng số lịch trùng: {conflicts.Count}");
            return conflicts;
        }

        
        public List<PetConflictInfo> CheckPetAppointment(List<int> petIds, DateOnly appointmentDate, TimeOnly startTime, TimeOnly endTime, int? excludeAppointmentId = null)
        {
            Console.WriteLine($"[CheckPetAppointment] Bắt đầu kiểm tra trùng lịch với EndTime");
            Console.WriteLine($"[CheckPetAppointment] PetIds: {string.Join(", ", petIds)}");
            Console.WriteLine($"[CheckPetAppointment] Ngày: {appointmentDate}, Giờ bắt đầu: {startTime}, Giờ kết thúc: {endTime}");
            
            var startDateTime = appointmentDate.ToDateTime(startTime);
            var endDateTime = appointmentDate.ToDateTime(endTime);
            
            Console.WriteLine($"[CheckPetAppointment] Thời gian kiểm tra: {startDateTime:dd/MM/yyyy HH:mm} - {endDateTime:HH:mm}");
            
            return CheckPetAppointment(petIds, startDateTime, endDateTime, excludeAppointmentId);
        }

        
        public List<int> getBusyStaffIds(DateOnly appointmentDate, TimeOnly startTime, TimeOnly endTime, int? excludeAppointmentId = null)
        {
            try
            {
                var query = _context.AppointmentPets
                    .Where(ap => ap.Appointment.AppointmentDate == appointmentDate &&
                               ap.Appointment.IsActive == true &&
                               ap.Appointment.StatusId != 5 && // Loại trừ lịch đã từ chối
                               ap.StaffId.HasValue && // Chỉ lấy những pet đã được phân công nhân viên
                               ((ap.Appointment.StartTime <= startTime && ap.Appointment.EndTime > startTime) ||
                                (ap.Appointment.StartTime < endTime && ap.Appointment.EndTime >= endTime) ||
                                (ap.Appointment.StartTime >= startTime && ap.Appointment.EndTime <= endTime)));

                // Loại trừ lịch hẹn nếu có
                if (excludeAppointmentId.HasValue)
                {
                    query = query.Where(ap => ap.AppointmentId != excludeAppointmentId.Value);
                }

                var busyStaffIds = query
                    .Select(ap => ap.StaffId.Value)
                    .Distinct()
                    .ToList();

                Console.WriteLine($"[getBusyStaffIds] Ngày: {appointmentDate}, Giờ: {startTime} - {endTime}");
                Console.WriteLine($"[getBusyStaffIds] - Số nhân viên bận: {busyStaffIds.Count}");
                Console.WriteLine($"[getBusyStaffIds] - Danh sách StaffId bận: {string.Join(", ", busyStaffIds)}");

                return busyStaffIds;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[getBusyStaffIds] Lỗi: {ex.Message}");
                return new List<int>();
            }
        }

        public List<int> listStaffAvailable(DateOnly appointmentDate, TimeOnly startTime, TimeOnly endTime)
        {
            try
            {
                // Lấy tất cả nhân viên (RoleId == 3)
                var allStaff = _context.Users
                    .Where(u => u.RoleId == 3 && u.IsActive == true)
                    .Select(u => u.UserId)
                    .ToList();

                if (!allStaff.Any())
                {
                    return new List<int>();
                }

                // Sử dụng hàm helper để lấy nhân viên bận
                var busyStaffIds = getBusyStaffIds(appointmentDate, startTime, endTime);

                // Lấy danh sách nhân viên rảnh
                var availableStaffIds = allStaff.Except(busyStaffIds).ToList();

                if (!availableStaffIds.Any())
                {
                    return new List<int>();
                }

                // Đếm số lịch hẹn của mỗi nhân viên trong ngày
                var staffAppointmentCounts = _context.AppointmentPets
                    .Where(ap => ap.Appointment.AppointmentDate == appointmentDate &&
                               ap.Appointment.IsActive == true &&
                               ap.Appointment.StatusId != 5 && // Loại trừ lịch đã từ chối
                               ap.StaffId.HasValue &&
                               availableStaffIds.Contains(ap.StaffId.Value))
                    .GroupBy(ap => ap.StaffId.Value)
                    .Select(g => new { StaffId = g.Key, AppointmentCount = g.Count() })
                    .ToDictionary(x => x.StaffId, x => x.AppointmentCount);

                // Sắp xếp nhân viên rảnh theo số lịch hẹn từ ít đến nhiều
                var sortedAvailableStaff = availableStaffIds
                    .OrderBy(staffId => staffAppointmentCounts.ContainsKey(staffId) ? staffAppointmentCounts[staffId] : 0)
                    .ToList();

                Console.WriteLine($"[listStaffAvailable] Ngày: {appointmentDate}, Giờ: {startTime} - {endTime}");
                Console.WriteLine($"[listStaffAvailable] - Tổng số nhân viên: {allStaff.Count}");
                Console.WriteLine($"[listStaffAvailable] - Số nhân viên bận: {busyStaffIds.Count}");
                Console.WriteLine($"[listStaffAvailable] - Số nhân viên rảnh: {availableStaffIds.Count}");
                Console.WriteLine($"[listStaffAvailable] - Danh sách nhân viên rảnh (sắp xếp theo số lịch): {string.Join(", ", sortedAvailableStaff)}");

                return sortedAvailableStaff;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[listStaffAvailable] Lỗi: {ex.Message}");
                return new List<int>();
            }
        }

        
        public (bool IsEnoughStaff, int AvailableStaffCount, int RequiredStaffCount) checkNumStaffForAppointment(int appointmentId)
        {
            try
            {
                // Lấy thông tin lịch hẹn
                var appointment = _appointmentRepository.GetById(appointmentId);
                if (appointment == null)
                {
                    return (false, 0, 0);
                }

                // Lấy số lượng pet trong lịch hẹn (số nhân viên cần thiết)
                var appointmentPets = _appointmentRepository.GetAppointmentPets(appointmentId);
                int requiredStaffCount = appointmentPets.Count;

                if (requiredStaffCount == 0)
                {
                    return (true, 0, 0); // Không có pet nào thì không cần nhân viên
                }

                // Lấy tất cả nhân viên (RoleId == 3)
                var allStaff = _context.Users
                    .Where(u => u.RoleId == 3 && u.IsActive == true)
                    .ToList();

                if (!allStaff.Any())
                {
                    return (false, 0, requiredStaffCount);
                }

                // Sử dụng hàm helper để lấy nhân viên bận
                var busyStaffIds = getBusyStaffIds(appointment.AppointmentDate, appointment.StartTime, appointment.EndTime, appointmentId);

                // Tính số nhân viên rảnh
                int availableStaffCount = allStaff.Count - busyStaffIds.Count;

                // Kiểm tra xem có đủ nhân viên không
                bool isEnoughStaff = availableStaffCount >= requiredStaffCount;

                Console.WriteLine($"[checkNumStaffForAppointment] Appointment {appointmentId}:");
                Console.WriteLine($"[checkNumStaffForAppointment] - Số pet cần phục vụ: {requiredStaffCount}");
                Console.WriteLine($"[checkNumStaffForAppointment] - Tổng số nhân viên: {allStaff.Count}");
                Console.WriteLine($"[checkNumStaffForAppointment] - Số nhân viên bận (từ StaffId): {busyStaffIds.Count}");
                Console.WriteLine($"[checkNumStaffForAppointment] - Số nhân viên rảnh: {availableStaffCount}");
                Console.WriteLine($"[checkNumStaffForAppointment] - Có đủ nhân viên: {isEnoughStaff}");

                return (isEnoughStaff, availableStaffCount, requiredStaffCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[checkNumStaffForAppointment] Lỗi: {ex.Message}");
                return (false, 0, 0);
            }
        }

        public bool AutoAssignStaff(int appointmentId)
        {
            try
            {
                Console.WriteLine($"[AutoAssignStaff] 👉 Bắt đầu gán nhân viên cho AppointmentId: {appointmentId}");

                // Lấy lịch hẹn từ repository
                var appointment = _appointmentRepository.GetById(appointmentId);

                if (appointment == null)
                {
                    Console.WriteLine($"[AutoAssignStaff] ❌ Không tìm thấy lịch hẹn với ID: {appointmentId}");
                    return false;
                }

                if (appointment.IsActive != true || appointment.StatusId == 5)
                {
                    Console.WriteLine($"[AutoAssignStaff] ❌ Lịch hẹn không hợp lệ hoặc đã bị từ chối (IsActive: {appointment.IsActive}, StatusId: {appointment.StatusId})");
                    return false;
                }

                // Lấy danh sách AppointmentPet chưa được gán Staff
                var appointmentPets = _appointmentRepository
                    .GetAppointmentPets(appointmentId)
                    .Where(p => p.StaffId == null && p.IsActive == true)
                    .ToList();

                Console.WriteLine($"[AutoAssignStaff] 🔍 Số lượng AppointmentPet chưa gán Staff: {appointmentPets.Count}");

                if (!appointmentPets.Any())
                {
                    Console.WriteLine("[AutoAssignStaff] ✅ Tất cả pet đã được gán Staff.");
                    return false;
                }

                // Lấy danh sách nhân viên rảnh (chỉ gọi 1 lần)
                var availableStaff = listStaffAvailable(
                    appointment.AppointmentDate,
                    appointment.StartTime,
                    appointment.EndTime
                );

                Console.WriteLine($"[AutoAssignStaff] 👥 Danh sách nhân viên rảnh: {string.Join(", ", availableStaff)}");

                bool assigned = false;
                int staffIndex = 0;

                foreach (var ap in appointmentPets)
                {
                    if (staffIndex >= availableStaff.Count)
                    {
                        Console.WriteLine($"[AutoAssignStaff] ⚠️ Không đủ nhân viên để gán cho PetId: {ap.PetId}");
                        break;
                    }

                    int chosenStaffId = availableStaff[staffIndex];
                    ap.StaffId = chosenStaffId;
                    Console.WriteLine($"[AutoAssignStaff] ✅ Gán StaffId {chosenStaffId} cho PetId: {ap.PetId}");
                    assigned = true;
                    staffIndex++;
                }

                if (assigned)
                {
                    Console.WriteLine($"[AutoAssignStaff] ✅ Gán thành công ít nhất 1 nhân viên. Đang cập nhật trạng thái...");
                    ConfirmedAppointment(appointment); // Xác nhận trạng thái nếu cần
                    _appointmentRepository.SaveChanges(); // hoặc _unitOfWork.SaveChanges()
                    Console.WriteLine("[AutoAssignStaff] 💾 Đã lưu thay đổi vào database.");
                }
                else
                {
                    Console.WriteLine("[AutoAssignStaff] ❌ Không gán được nhân viên nào.");
                }

                return assigned;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AutoAssignStaff] ❌ Lỗi: {ex.Message}");
                return false;
            }
        }


        public bool ConfirmedAppointment(Appointment appointment)
        {
            try
            {
                // Kiểm tra lịch hẹn hợp lệ
                if (appointment == null || appointment.StatusId == 5)
                    return false;

                // Cập nhật trạng thái
                appointment.StatusId = 2; // 2 = "Đã xác nhận"
                appointment.IsActive = true;
                _appointmentRepository.Update(appointment); // Cập nhật đối tượng

                _appointmentRepository.SaveChanges(); // Lưu thay đổi

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ConfirmedAppointment] Lỗi: {ex.Message}");
                return false;
            }
        }

public List<StaffShift> GetRealtimeShiftViewModel()
        {
            var appointments = _appointmentRepository.GetActiveAppointmentsWithStaffAndStatus()
                .Include(a => a.Employee)
                .Include(a => a.AppointmentPets) // bảng ánh xạ appointment-pet
                .ToList()
                .Where(a => a.IsActive == true && new[] { 2, 3, 4, 6 }.Contains(a.StatusId))
                .ToList();

            var staffGroups = appointments
                .GroupBy(a => new
                {
                    StaffId = a.EmployeeId ?? 0,
                    UserId = a.Employee?.UserId ?? 0,
                    FullName = a.Employee?.FullName ?? "(Không xác định)",
                    AvatarUrl = a.Employee?.ProfilePictureUrl ?? ""
                })
                .Select(g => new StaffShift
                {
                    StaffId = g.Key.StaffId,
                    UserId = g.Key.UserId,
                    StaffName = g.Key.FullName,
                    AvatarUrl = g.Key.AvatarUrl,
                    Appointments = g.Select(a => new AppointmentViewModel
                    {
                        AppointmentId = a.AppointmentId,
                        AppointmentDate = a.AppointmentDate,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        StatusId = a.StatusId,
                        Notes = a.Notes ?? "",
                        // Build danh sách pet-staff assignments cho mỗi appointment
                        PetStaffAssignments = a.AppointmentPets.Select(ap => new PetStaffAssignViewModel
                        {
                            PetId = ap.PetId,
                            StaffId = a.EmployeeId ?? 0 // hoặc lấy đúng nhân viên cho pet nếu có gán khác
                        }).ToList()
                    }).ToList()
                })
                .ToList();

            return staffGroups;
        }


        public MoveResult AssignStaffToPet(int appointmentId, int petId, int newStaffId)
        {
            var appointment = _appointmentRepository.GetAppointmentWithPetAssignments(appointmentId);

            if (appointment == null)
                return new MoveResult { Success = false, Message = "Không tìm thấy lịch hẹn." };

            if (appointment.StatusId != 2)
                return new MoveResult { Success = false, Message = "Chỉ đổi nhân viên khi lịch đã được xác nhận." };

            var petAssignment = appointment.AppointmentPets.FirstOrDefault(p => p.PetId == petId);

            if (petAssignment == null)
                return new MoveResult { Success = false, Message = "Không tìm thấy pet trong lịch hẹn." };

            var isAvailable = _appointmentRepository.IsStaffAvailableForPet(
                newStaffId,
                appointment.AppointmentDate,
                appointment.StartTime,
                appointment.EndTime,
                appointmentId
            );

            if (!isAvailable)
                return new MoveResult { Success = false, Message = "Nhân viên mới bị trùng lịch." };

            var updated = _appointmentRepository.UpdateStaffForPet(appointmentId, petId, newStaffId);

            return updated
                ? new MoveResult { Success = true }
                : new MoveResult { Success = false, Message = "Cập nhật thất bại." };
        }
        
        public List<User> getAllStaffFreeByAppointmentId(int appointmentId)
        {
            try
            {
                // 🔁 Dùng repository để lấy appointment
                var appointment = _appointmentRepository.GetById(appointmentId);

                if (appointment == null || appointment.IsActive == false) // Fix: Explicitly compare nullable bool
                {
                    Console.WriteLine($"[getAllStaffFreeByAppointmentId] ❌ Không tìm thấy lịch hẹn hoặc đã bị vô hiệu hóa (ID: {appointmentId})");
                    return new List<User>();
                }

                // 🔍 Lấy thông tin ngày và giờ từ lịch hẹn
                var appointmentDate = appointment.AppointmentDate;
                var startTime = appointment.StartTime;
                var endTime = appointment.EndTime;

                // 📞 Gọi hàm có sẵn để lấy danh sách staffId rảnh
                var availableStaffIds = listStaffAvailable(appointmentDate, startTime, endTime);

                if (!availableStaffIds.Any())
                {
                    Console.WriteLine($"[getAllStaffFreeByAppointmentId] ⚠️ Không có nhân viên rảnh cho lịch hẹn #{appointmentId}");
                    return new List<User>();
                }

                // 👥 Lấy thông tin chi tiết của các staff từ User DbSet
                var staffList = _userRepository.GetUsersByIdsOrdered(availableStaffIds);


                Console.WriteLine($"[getAllStaffFreeByAppointmentId] ✅ Tìm thấy {staffList.Count} nhân viên rảnh.");
                return staffList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[getAllStaffFreeByAppointmentId] ❌ Lỗi: {ex.Message}");
                return new List<User>();
            }
        }

        public List<Appointment> GetActiveAppointmentsByService(int serviceId)
        {
            return _context.Appointments
                .Include(a => a.AppointmentServices)
                .Where(a =>
                        // Lọc các lịch hẹn có sử dụng dịch vụ này
                        a.AppointmentServices.Any(s => s.ServiceId == serviceId) &&
                        // Và có trạng thái đang hoạt động (không phải Completed hoặc Cancelled)
                        a.StatusId != 4 && // Completed
                        a.StatusId != 5 // Cancelled
                )
                .ToList();
        }
        public string GetStatusName(int statusId)
        {
            var status = GetAllStatuses().FirstOrDefault(s => s.StatusId == statusId);
            return status?.StatusName ?? "Unknown";
        }
    }
}