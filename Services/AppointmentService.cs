using pet_spa_system1.ViewModels;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IEmailService _iEmailService;
        private readonly IPetRepository _petRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly PetDataShopContext _context;

        public AppointmentService(
            IAppointmentRepository repo,
            IEmailService iEmailService,
            IPetRepository petRepository,
            IServiceRepository serviceRepository,
            PetDataShopContext context)
        {
            _appointmentRepository = repo;
            _iEmailService = iEmailService;
            _petRepository = petRepository;
            _serviceRepository = serviceRepository;
            _context = context;
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
                System.Diagnostics.Debug.WriteLine($"Mapping Appointment {a.AppointmentId}");
                var serviceCount = a.AppointmentServices?.Count ?? 0;
                System.Diagnostics.Debug.WriteLine($"Service count: {serviceCount}");
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
                        Duration = asv.Service?.DurationMinutes ?? 0,
                        StatusId = asv.Status ?? 0,
                        StatusName = asv.StatusNavigation?.StatusName ?? "Chờ xác nhận"
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
                Employees = GetEmployees(),
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
                    EmployeeId = model.EmployeeIds != null && model.EmployeeIds.Any() ? model.EmployeeIds.First() : (int?)null,
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
                EmployeeIds = appointment.EmployeeId != null ? new List<int> { appointment.EmployeeId.Value } : new List<int>(),
                CustomerId = appointment.UserId,
                Notes = appointment.Notes,

                Statuses = _appointmentRepository.GetAllStatuses(),
                Employees = GetEmployees(),
                Customers = _appointmentRepository.GetCustomers(),
                AllPets = petInfos,
                AllServices = _serviceRepository.GetActiveServices().ToList(),

                SelectedPetIds = appointment.AppointmentPets != null ? appointment.AppointmentPets.Select(ap => ap.PetId).ToList() : new List<int>(),
                SelectedServiceIds = appointment.AppointmentServices != null ? appointment.AppointmentServices.Select(asr => asr.ServiceId).ToList() : new List<int>()
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
            appointment.EmployeeId = model.EmployeeIds != null && model.EmployeeIds.Any() ? model.EmployeeIds.First() : (int?)null;
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
                2 => "bg-info",    // Đã xác nhận
                3 => "bg-success", // Hoàn thành
                4 => "bg-danger",  // Đã hủy
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
                    Console.WriteLine($"[RequestCancelAppointment] appointment.UserId={appointment.UserId} != userId={userId}");
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

                Console.WriteLine($"[RequestCancelAppointment] Đã cập nhật StatusId=6 thành công cho appointmentId={appointmentId}");
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
        
        // Gửi mail khi duyệt lịch hoặc duyệt hủy
        public bool UpdateAppointmentStatusAndSendMail(int id, int statusId)
        {
            try
            {
                var appointment = _appointmentRepository.GetById(id);
                if (appointment == null) return false;
                appointment.StatusId = statusId;
                _appointmentRepository.Update(appointment);
                _appointmentRepository.Save();

                // Lấy thông tin user và gửi mail nếu trạng thái là 2 (đã xác nhận) hoặc 5 (đã hủy)
                if (statusId == 2 || statusId == 5)
                {
                    var user = appointment.User;
                    if (!string.IsNullOrWhiteSpace(user?.Email))
                    {
                        // Gửi mail xác nhận hoặc hủy
                        var viewModel = new AppointmentViewModel
                        {
                            CustomerName = user.FullName ?? user.Username,
                            Email = user.Email,
                            AppointmentDate = appointment.AppointmentDate.Date,
                            AppointmentTime = appointment.AppointmentDate.TimeOfDay,
                            StatusId = appointment.StatusId,
                            StatusName = appointment.Status?.StatusName ?? string.Empty,
                            Notes = appointment.Notes,
                            SelectedPets = appointment.AppointmentPets?.Select(ap => ap.Pet != null ? ap.Pet : new Pet { Name = "", Breed = "" }).ToList() ?? new List<Pet>(),
                            SelectedServices = appointment.AppointmentServices?.Select(asv => asv.Service != null ? asv.Service : new Service { Name = "", Price = 0 }).ToList() ?? new List<Service>()
                        };
                        _iEmailService.SendBookingConfirmation(viewModel);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating appointment status/send mail: {ex.Message}");
                return false;
            }
        }

        // Sửa lại kiểu trả về và khởi tạo cho đúng với ViewModel thực tế
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
        AppointmentDate = appointment.AppointmentDate,
        StatusId = appointment.StatusId,
        StatusName = appointment.Status?.StatusName ?? string.Empty,
        PetNames = petNames,
        Notes = appointment.Notes,
        Services = serviceHistory
    };
        }

        // AppointmentServiceImage
        public IEnumerable<AppointmentServiceImage> GetImagesByAppointmentServiceId(int appointmentServiceId)
            => _context.AppointmentServiceImages.Where(i => i.AppointmentServiceId == appointmentServiceId).ToList();

        public AppointmentServiceImage? GetImageById(int imageId)
            => _context.AppointmentServiceImages.Find(imageId);

        public void AddImage(AppointmentServiceImage image)
        {
            _context.AppointmentServiceImages.Add(image);
                _context.SaveChanges();
            }

        public void DeleteImage(int imageId)
        {
            var img = _context.AppointmentServiceImages.Find(imageId);
            if (img != null)
            {
                _context.AppointmentServiceImages.Remove(img);
                _context.SaveChanges();
            }
        }

        // AppointmentServiceStatus
        public IEnumerable<AppointmentServiceStatus> GetAllServiceStatuses()
            => _context.AppointmentServiceStatus.ToList();

        public AppointmentServiceStatus? GetServiceStatusById(int statusId)
            => _context.AppointmentServiceStatus.Find(statusId);

        public void AddServiceStatus(AppointmentServiceStatus status)
        {
            _context.AppointmentServiceStatus.Add(status);
            _context.SaveChanges();
        }

        public void DeleteServiceStatus(int statusId)
        {
            var status = _context.AppointmentServiceStatus.Find(statusId);
            if (status != null)
            {
                _context.AppointmentServiceStatus.Remove(status);
                _context.SaveChanges();
            }
        }
    }
}