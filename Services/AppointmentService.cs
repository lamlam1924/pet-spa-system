using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using System.Linq;
using pet_spa_system1.ViewModels;


namespace pet_spa_system1.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;
        private readonly IEmailService _iEmailService;
        private readonly IPetRepository _petRepository;
        private readonly IServiceRepository _serviceRepository;
        
        public AppointmentService(
            IAppointmentRepository repo, 
            IEmailService iEmailService,
            IPetRepository petRepository,
            IServiceRepository serviceRepository)
        {
            _repo = repo;
            _iEmailService = iEmailService;
            _petRepository = petRepository;
            _serviceRepository = serviceRepository;
        }

        public bool SaveAppointment(AppointmentViewModel model, int userId)
        {
            try
            {
                // DEBUG
                Console.WriteLine($"SaveAppointment: Có {model.SelectedServiceIds?.Count ?? 0} dịch vụ");

                var appointment = new Appointment
                {
                    UserId = userId,
                    AppointmentDate = model.AppointmentDate.Date.Add(model.AppointmentTime),
                    StatusId = 1,
                    Notes = model.Notes,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                int appId = _repo.AddAppointment(appointment);

                // Thêm thú cưng vào lịch hẹn
                if (model.SelectedPetIds != null && model.SelectedPetIds.Any())
                {
                    foreach (var petId in model.SelectedPetIds)
                    {
                        Console.WriteLine($"Thêm thú cưng {petId} vào lịch hẹn {appId}");
                        _repo.AddAppointmentPet(appId, petId);
                    }
                }
                
                // Thêm dịch vụ vào lịch hẹn
                if (model.SelectedServiceIds != null && model.SelectedServiceIds.Any())
                {
                    foreach (var serviceId in model.SelectedServiceIds)
                    {
                        Console.WriteLine($"Thêm dịch vụ {serviceId} vào lịch hẹn {appId}");
                        _repo.AddAppointmentService(appId, serviceId);
                    }
                }

                _repo.Save(); // Lưu tất cả thay đổi
                
                // GỬI EMAIL XÁC NHẬN ở đây
                if (!string.IsNullOrWhiteSpace(model.Email))
                {
                    try
                    {
                        Console.WriteLine($"[Email] Bắt đầu gửi email xác nhận đến {model.Email}");
                        
                        // Lấy chi tiết về thú cưng và dịch vụ
                        if (model.SelectedPetIds != null && model.SelectedPetIds.Any())
                        {
                            model.SelectedPets = new List<Pet>();
                            foreach (var petId in model.SelectedPetIds)
                            {
                                var pet = _petRepository.GetById(petId);
                                if (pet != null)
                                {
                                    model.SelectedPets.Add(pet);
                                }
                            }
                        }
                        
                        if (model.SelectedServiceIds != null && model.SelectedServiceIds.Any())
                        {
                            model.SelectedServices = new List<Service>();
                            foreach (var serviceId in model.SelectedServiceIds)
                            {
                                var service = _serviceRepository.GetServiceById(serviceId);
                                if (service != null)
                                {
                                    model.SelectedServices.Add(service);
                                }
                            }
                        }
                            
                        var petNames = model.SelectedPets.Select(p => p.Name).ToList();
                        var serviceNames = model.SelectedServices.Select(s => s.Name).ToList();
                        
                        Console.WriteLine($"[Email] Danh sách thú cưng: {string.Join(", ", petNames)}");
                        Console.WriteLine($"[Email] Danh sách dịch vụ: {string.Join(", ", serviceNames)}");
                        
                        _iEmailService.SendBookingConfirmation(
                            model.Email,
                            model.CustomerName,
                            appointment.AppointmentDate,
                            model.Notes,
                            petNames: petNames,
                            serviceNames: serviceNames
                        );
                        
                        Console.WriteLine($"[Email] Đã gửi email thành công đến {model.Email}");
                    }
                    catch (Exception emailEx)
                    {
                        // Log chi tiết lỗi để dễ debug
                        Console.WriteLine($"[Email Error] Không gửi được email xác nhận cho {model.Email}: {emailEx.Message}");
                        Console.WriteLine($"[Email Error] Chi tiết: {emailEx.StackTrace}");
                        
                        if (emailEx.InnerException != null)
                        {
                            Console.WriteLine($"[Email Error] Inner Exception: {emailEx.InnerException.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("[Email Warning] Không có địa chỉ email để gửi xác nhận");
                }

                
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                return false;
            }
        }

        public AppointmentHistoryViewModel GetAppointmentHistory(int userId)
        {
            var appointments = _repo.GetByUserIdWithDetail(userId);
            var statuses = _repo.GetAllStatuses();
            
            
            // Debug để kiểm tra dữ liệu
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
                        Name = asv.Service?.Name ?? "",
                        IsActive = asv.Service?.IsActive ?? false
                    }).ToList() ?? new List<ServiceHistoryInfo>(),
                    PetNames = a.AppointmentPets?.Select(ap => ap.Pet?.Name ?? "").ToList() ?? new List<string>()
                }).OrderByDescending(x => x.AppointmentDate).ToList()
            };

            return vm;
        }


        public Appointment GetAppointmentById(int appointmentId)
        {
            return _repo.GetById(appointmentId);
        }

        public List<string> GetPetNames(List<int> petIds)
        {
            if (petIds == null || petIds.Count == 0) return new List<string>();
            // Nếu đã có repo Pet hoặc service, gọi lấy theo Id
            return _repo.GetPetNamesByIds(petIds); // Bạn cài repo này hoặc tự lấy từ context
        }

        public List<string> GetServiceNames(List<int> serviceIds)
        {
            if (serviceIds == null || serviceIds.Count == 0) return new List<string>();
            return _repo.GetServiceNamesByIds(serviceIds); // Tương tự như trên
        }

    }
}