    
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Services;

// Đã có model riêng cho trạng thái AppointmentStatus, không cần enum nữa

public class AppointmentService : IAppointmentService


{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPetRepository _petRepository;
    private readonly IEmailService _emailService;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IUserRepository userRepository,
        IPetRepository petRepository,
        IEmailService emailService)
    {
        _appointmentRepository = appointmentRepository;
        _userRepository = userRepository;
        _petRepository = petRepository;
        _emailService = emailService;
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
        var pendingCancelStatus = _appointmentRepository.GetAllStatuses().FirstOrDefault(s => s.StatusName == "PendingCancel");
        if (pendingCancelStatus == null) return false;
        if (appointment.StatusId == pendingCancelStatus.StatusId) return false;
        appointment.StatusId = pendingCancelStatus.StatusId;
        _appointmentRepository.SaveChanges();
        return true;
    }

    public AdminAppointmentDetailViewModel GetAdminAppointmentDetail(int appointmentId)
    {
        // Trả về chi tiết lịch hẹn cho admin
        var appointment = _appointmentRepository.GetById(appointmentId);
        if (appointment == null) return new AdminAppointmentDetailViewModel();
        // Tùy chỉnh trả về ViewModel chi tiết
        return new AdminAppointmentDetailViewModel
        {
            AppointmentId = appointment.AppointmentId,
            CustomerName = appointment.User?.FullName ?? string.Empty,
            AppointmentDate = appointment.AppointmentDate.ToDateTime(appointment.StartTime),
            StatusId = appointment.StatusId,
            StatusName = appointment.Status?.StatusName ?? string.Empty,
            SelectedPets = appointment.AppointmentPets?.Select(p => new AppointmentPetViewModel
            {
                PetId = p.PetId,
                Name = p.Pet?.Name ?? string.Empty,
                Breed = p.Pet?.Breed ?? string.Empty
            }).ToList() ?? new List<AppointmentPetViewModel>(),
            SelectedServices = appointment.AppointmentServices?.Select(s => new AppointmentServiceInfo
            {
                ServiceId = s.ServiceId,
                Name = s.Service?.Name ?? string.Empty,
                CategoryName = s.Service?.Category?.Name ?? string.Empty,
                Price = s.Service?.Price ?? 0,
                DurationMinutes = s.Service?.DurationMinutes ?? 0
            }).ToList() ?? new List<AppointmentServiceInfo>(),
            Notes = appointment.Notes
        };
    }

    public object GetCalendarData()
    {
        // Truy vấn resources và events đã được tách sang repo
        var resources =
            _userRepository.GetStaffResources(); // Trả về List<object> hoặc List<StaffResourceViewModel>
        var events = _appointmentRepository.GetCalendarEvents(); // Trả về List<CalendarEventViewModel>
        return new { resources, events };
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
            EndTime = TimeOnly.FromDateTime(a.AppointmentDate.ToDateTime(a.StartTime).AddMinutes(a.AppointmentServices.Sum(s => s.Service.DurationMinutes ?? 0))),
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
        Set("EndTime", TimeOnly.FromDateTime(appointment.AppointmentDate.ToDateTime(appointment.StartTime).AddMinutes(
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
            EndTime = TimeOnly.FromDateTime(appointment.AppointmentDate.ToDateTime(appointment.StartTime).AddMinutes(
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

    public void UpdateAppointmentWithPetStaff(AppointmentViewModel vm)
    {
        var appointment = _appointmentRepository.GetById(vm.AppointmentId);
        if (appointment == null) return;
        appointment.AppointmentDate = vm.AppointmentDate;
        appointment.StatusId = vm.StatusId;
        appointment.Notes = vm.Notes;
        // Không cần gọi Update, chỉ cần SaveChanges vì entity đã được sửa trực tiếp

        // Chuẩn hóa: mỗi pet gán 1 staff
        if (vm.PetStaffAssignments != null)
        {
            foreach (var assign in vm.PetStaffAssignments)
            {
                _appointmentRepository.UpdateAppointmentPetStaff(vm.AppointmentId, assign.PetId,
                    assign.StaffId ?? 0);
            }
        }

        _appointmentRepository.SaveChanges();
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
    public bool SaveAppointment(AppointmentViewModel model, int userId)
    {
        try
        {
            // Tạo mới entity Appointment
            var appointment = new Appointment
            {
                UserId = userId,
                AppointmentDate = model.AppointmentDate,
                StartTime = model.StartTime,
                StatusId = 1, // Pending
                Notes = model.Notes
            };
            // Lưu vào DB
            int appointmentId = _appointmentRepository.AddAppointment(appointment);

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
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] SaveAppointment: {ex.Message}");
            return false;
        }
    }

    public AppointmentHistoryViewModel GetAppointmentHistory(int userId)
    {
        // Lấy tất cả lịch hẹn của user
        var appointments = _appointmentRepository.GetAppointments(new ViewModel.AppointmentFilter
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
}
    
