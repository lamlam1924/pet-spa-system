using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.ViewModel;
using pet_spa_system1.Services;

namespace pet_spa_system1.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPetService _petService;
        private readonly IServiceService _serviceService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IAppointmentServiceImageService _appointmentServiceImageService;

        public AppointmentController(
            IAppointmentService appointmentService,
            IPetService petService,
            IServiceService serviceService,
            IUserService userService,
            INotificationService notificationService,
            IAppointmentServiceImageService appointmentServiceImageService)
        {
            _appointmentService = appointmentService;
            _petService = petService;
            _serviceService = serviceService;
            _userService = userService;
            _notificationService = notificationService;
            _appointmentServiceImageService = appointmentServiceImageService;
        }

        // GET: /Appointment
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                // Xử lý trường hợp chưa đăng nhập, ví dụ:
                return RedirectToAction("Login", "Login");
            }

            var userEntity = _userService.GetUserInfo(userId.Value);
            var viewModel = new AppointmentViewModel
            {
                Pets = _petService.GetPetsByUserId(userId.Value),
                Services = _serviceService.GetActiveServices().ToList(),
                Categories = _serviceService.GetAllCategories().ToList(),
                AppointmentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                StartTime = new TimeOnly(9, 0, 0), // Default to 9:00 AM
                User = userEntity
            };

            return View("Appointment", viewModel);
        }

        private void FillViewModel(AppointmentViewModel model, int? userId)
        {
            model.Pets = userId.HasValue ? _petService.GetPetsByUserId(userId.Value) : new List<Pet>();
            model.Services = _serviceService.GetActiveServices().ToList();
            model.Categories = _serviceService.GetAllCategories().ToList();
            model.User = userId.HasValue ? _userService.GetUserInfo(userId.Value) : null;
        }

        // POST: /Appointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AppointmentSync(AppointmentViewModel model)
        {
            // Parse StartTime từ StartTimeString nếu có
            if (!string.IsNullOrEmpty(model.StartTimeString))
            {
                if (TimeOnly.TryParse(model.StartTimeString, out var parsedTime))
                {
                    model.StartTime = parsedTime;
                }
                else
                {
                    ModelState.AddModelError("StartTimeString", "Giờ hẹn không hợp lệ. Định dạng phải là HH:mm.");
                }
            }

            int? userId = HttpContext.Session.GetInt32("CurrentUserId");

            if (!ModelState.IsValid)
            {
                FillViewModel(model, userId);
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { success = false, errors });
                }

                return View(model);
            }

            try
            {
                if (userId == null)
                {
                    // Xử lý trường hợp chưa đăng nhập, ví dụ:
                    return RedirectToAction("Login", "Login");
                }

                Console.WriteLine($"[AppointmentController] Gọi SaveAppointment với userId: {userId.Value}");
                Console.WriteLine($"[AppointmentController] PetIds: {string.Join(", ", model.SelectedPetIds ?? new List<int>())}");
                Console.WriteLine($"[AppointmentController] ServiceIds: {string.Join(", ", model.SelectedServiceIds ?? new List<int>())}");
                var result = _appointmentService.SaveAppointment(model, userId.Value);
                if (result.Success)
                {
                    Console.WriteLine("[AppointmentController] SaveAppointment trả về thành công");
                    // Tạo thông báo khi đặt lịch thành công
                    var notification = new Notification
                    {
                        UserId = userId.Value,
                        Title = "Đặt lịch thành công",
                        Message = $"Lịch hẹn của bạn đã được đặt thành công. Chúng tôi sẽ xác nhận sớm nhất.",
                        CreatedAt = DateTime.Now,
                        IsRead = false
                    };

                   await _notificationService.AddAsync(notification);

                    TempData["SuccessMessage"] = "Đặt lịch thành công!";
                    // Return JSON for AJAX requests
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, redirectUrl = Url.Action("Success") });
                    }

                    return RedirectToAction(nameof(Success));
                }
                else
                {
                    //ModelState.AddModelError("", "Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại.");
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = result.message });
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại.");
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại." });
                }
            }

            FillViewModel(model, userId);
            return View(model);
        }

        // GET: /Appointment/Success
        public IActionResult Success()
        {
            // Pass success message to view if it exists
            ViewBag.SuccessMessage = TempData["SuccessMessage"]?.ToString();
            return View();
        }

        // GET: /Appointment/History
        public IActionResult History()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                // Xử lý trường hợp chưa đăng nhập, ví dụ:
                return RedirectToAction("Login", "Login");
            }

            var model = _appointmentService.GetAppointmentHistory(userId.Value);
            return View("History", model);
        }

        // POST: /Appointment/RequestCancel
        [HttpPost]
        public IActionResult RequestCancel([FromBody] RequestCancelDto dto)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                // Xử lý trường hợp chưa đăng nhập, ví dụ:
                return RedirectToAction("Login", "Login");
            }

            Console.WriteLine($"[RequestCancel] appointmentId nhận được từ client: {dto?.appointmentId}");

            var result = _appointmentService.RequestCancelAppointment(dto?.appointmentId ?? 0, userId.Value);

            if (result)
            {
                // Tạo thông báo khi gửi yêu cầu hủy lịch thành công
                var notification = new Notification
                {
                    UserId = userId.Value,
                    Title = "Yêu cầu hủy lịch",
                    Message = $"Yêu cầu hủy lịch hẹn đã được gửi. Chúng tôi sẽ xử lý trong thời gian sớm nhất.",
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };

                _notificationService.AddAsync(notification);

                return Json(new { success = true, message = "Yêu cầu hủy lịch đã được gửi, chờ admin duyệt." });
            }
            else
            {
                return Json(new { success = false, message = "Không thể gửi yêu cầu hủy lịch. Vui lòng thử lại." });
            }
        }

        // GET: /Appointment/Detail/{id}
        [HttpGet]
        public IActionResult Detail(int id)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để xem chi tiết lịch hẹn." });
            }

            var detail = _appointmentService.GetAppointmentDetailWithPetImages(id, userId.Value);
            if (detail == null)
            {
                return Json(new { success = false, message = "Không tìm thấy lịch hẹn." });
            }

            return Json(new { success = true, data = detail });
        }

        public IActionResult AppointmentDetail()
        {
            return View();
        }

        /// <summary>
        /// Get service images for customer view
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetServiceImages(int appointmentServiceId)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                Console.WriteLine($"[DEBUG] GetServiceImages: No user session");
                return Json(new { success = false, message = "Unauthorized" });
            }

            Console.WriteLine($"[DEBUG] GetServiceImages: appointmentServiceId={appointmentServiceId}, userId={userId.Value}");

            var result = await _appointmentServiceImageService.GetImagesForCustomerAsync(
                appointmentServiceId,
                userId.Value);

            Console.WriteLine($"[DEBUG] GetServiceImages: Service result - Success={result.Success}, Message={result.Message}");
            Console.WriteLine($"[DEBUG] GetServiceImages: Images count={result.Data?.Count ?? 0}");

            if (result.Success)
            {
                var images = result.Data?.Select(img => new {
                    imageId = img.ImageId,
                    imageUrl = img.ImageUrl,
                    photoType = img.PhotoType,
                    petId = img.PetId, // Thêm petId để group ảnh theo pet
                    createdAt = img.FormattedCreatedAt
                }).ToList();

                Console.WriteLine($"[DEBUG] GetServiceImages: Returning {images?.Count ?? 0} images");
                return Json(new { success = true, images = images });
            }
            else
            {
                Console.WriteLine($"[DEBUG] GetServiceImages: Error - {result.Message}");
                return Json(new { success = false, message = result.Message });
            }
        }

        public async Task<IActionResult> AddPetPartial()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return PartialView("_ErrorPartial", "Vui lòng đăng nhập để thêm thú cưng.");
            }

            var species = await _petService.GetAllSpeciesAsync() ?? new List<Species>();
            var model = new PetDetailViewModel
            {
                SpeciesList = species
            };
            return PartialView("AddPetPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPet(PetDetailViewModel model, IFormFile ImageFile)
        {
            Console.WriteLine("[UserHomeController] AddPet POST called");
            Console.WriteLine($"Received data: Name={model.Pet?.Name}, SpeciesId={model.Pet?.SpeciesId}, " +
                              $"Breed={model.Pet?.Breed}, Age={model.Pet?.Age}, Gender={model.Pet?.Gender}, " +
                              $"SpecialNotes={model.Pet?.SpecialNotes}, ImageFile={ImageFile?.FileName}");


            ModelState.Remove("ImageFile");

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    if (state.Value?.Errors.Count > 0)
                    {
                        Console.WriteLine($"❌ ERROR AT: {state.Key}");
                        foreach (var error in state.Value.Errors)
                        {
                            Console.WriteLine($"   ➤ {error.ErrorMessage}");
                        }
                    }
                }

                model.SpeciesList = await _petService.GetAllSpeciesAsync() ?? new List<Species>();
                return Json(new
                {
                    success = false,
                    message = "Dữ liệu không hợp lệ",
                    errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                       .ToDictionary(
                           kv => kv.Key,
                           kv => kv.Value.Errors.Select(e => e.ErrorMessage).ToList()
                       )
                });

            }

            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                Console.WriteLine("[UserHomeController] UserId is null, returning ErrorPartial");
                return PartialView("_ErrorPartial", "Vui lòng đăng nhập để thêm thú cưng.");
            }

            var pet = model.Pet;
            pet.UserId = userId.Value;
            pet.CreatedAt = DateTime.Now;
            pet.IsActive = true;

            var images = ImageFile != null ? new List<IFormFile> { ImageFile } : new List<IFormFile>();
            try
            {
                Console.WriteLine("[UserHomeController] Attempting to add pet... Name: " + pet.Name);
                await _petService.CreatePetAsync(pet, images);
                Console.WriteLine("[UserHomeController] Pet added successfully, PetId: " + pet.PetId);

                // Tạo thông báo khi thêm thú cưng thành công
                var notification = new Notification
                {
                    UserId = userId.Value,
                    Title = "Thêm thú cưng thành công",
                    Message = $"Thú cưng '{pet.Name}' đã được thêm vào hồ sơ của bạn.",
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };

                await _notificationService.AddAsync(notification);

                return Json(new { success = true, message = "Thêm thú cưng thành công!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("[UserHomeController] Error adding pet: " + ex.Message + " - StackTrace: " +
                                  ex.StackTrace);
                return Json(new { success = false, message = $"Lỗi khi thêm thú cưng: {ex.Message}" });
            }
        }
    }

    public class RequestCancelDto
    {
        public int appointmentId { get; set; }
    }
}