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

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IUserRepository userRepository,
            IPetRepository petRepository,
            IEmailService emailService,
            PetDataShopContext context)
        {
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
            _petRepository = petRepository;
            _emailService = emailService;
            _context = context;
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
            // Logic gửi yêu cầu hủy lịch hẹn
            // Ví dụ: cập nhật trạng thái appointment sang PendingCancel
            var appointment = _appointmentRepository.GetById(appointmentId);
            if (appointment == null) return false;
            // Chỉ cho phép yêu cầu hủy nếu trạng thái chưa phải PendingCancel
            // Sử dụng model StatusAppointment thay vì enum
            var pendingCancelStatus = _appointmentRepository.GetAllStatuses()
                .FirstOrDefault(s => s.StatusName == "PendingCancel");
            if (pendingCancelStatus == null) return false;
            if (appointment.StatusId == pendingCancelStatus.StatusId) return false;
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
            var calendarViewModel = new ViewModel.CalendarViewModel
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
                        if (evtDict["resourceIds"] is System.Collections.IEnumerable resourceIds)
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
                        if (evtDict["services"] is System.Collections.IEnumerable services)
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
                var hours = Enumerable.Range(8, 13).ToList(); // 8h đến 20h
                var viewModel = new RealtimeShiftViewModel
                {
                    Hours = hours,
                    StaffShifts = new List<StaffShift>()
                };
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

            appointment.StatusId = statusId;
            // Không cần gọi Update riêng vì entity đã được theo dõi bởi DbContext
            // và sẽ được cập nhật khi gọi SaveChanges()
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

        
        public List<PetConflictInfo> CheckPetAppointment(List<int> petIds, DateTime startDateTime, DateTime endDateTime, int? excludeAppointmentId = null)
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
    }
}