using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModel;
using Microsoft.AspNetCore.Http;

namespace pet_spa_system1.Controllers
{
    public class UserHomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPetService _petService;
        private readonly ISpeciesService _speciesService;
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        private readonly IProductService _productService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly INotificationService _notificationService;
        public readonly IAppointmentService _appointmentService;

        public UserHomeController(
            IUserService userService,
            IPetService petService,
            ISpeciesService speciesService,
            IOrderService orderService,
            IOrderItemService orderItemService,
            IProductService productService,
            IOrderStatusService orderStatusService,
            INotificationService notificationService,
            IAppointmentService appointmentService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _petService = petService ?? throw new ArgumentNullException(nameof(petService));
            _speciesService = speciesService ?? throw new ArgumentNullException(nameof(speciesService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _orderStatusService = orderStatusService ?? throw new ArgumentNullException(nameof(orderStatusService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            Console.WriteLine("[UserHomeController] Services injected successfully.");
            _appointmentService = appointmentService;
        }

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var currentUser = await _userService.GetUserByIdAsync(userId.Value);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var userModel = new UserViewModel
            {
                UserName = currentUser.Username,
                Email = currentUser.Email,
                PhoneNumber = currentUser.Phone,
                Address = currentUser.Address,
                FullName = currentUser.FullName,
                ProfilePictureUrl = currentUser.ProfilePictureUrl
            };
            return View(userModel);
        }

        public async Task<IActionResult> Hoso(string successMessage = null, string errorMessage = null)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            var currentUser = await _userService.GetUserByIdAsync(userId.Value);

            var userModel = new UserViewModel
            {
                UserName = currentUser.Username,
                Email = currentUser.Email,
                PhoneNumber = currentUser.Phone,
                Address = currentUser.Address,
                FullName = currentUser.FullName,
                ProfilePictureUrl = currentUser.ProfilePictureUrl,

            };

            ViewBag.SuccessMessage = successMessage;
            ViewBag.ErrorMessage = errorMessage;
            return PartialView("_HosoPartial", userModel);
        }

        public IActionResult ChangePasswordPartial()
        {
            var model = new ChangePasswordViewModel();
            return PartialView("_ChangePasswordPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thay đổi mật khẩu." });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            try
            {
                // Get current user to verify current password
                var currentUser = await _userService.GetUserByIdAsync(userId.Value);
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin người dùng." });
                }

                // Verify current password (allow empty if user has no password set)
                if (!string.IsNullOrEmpty(model.CurrentPassword))
                {
                    var isCurrentPasswordValid = await _userService.AuthenticateAsync(currentUser.Email, model.CurrentPassword);
                    if (isCurrentPasswordValid == null)
                    {
                        return Json(new { success = false, message = "Mật khẩu hiện tại không đúng." });
                    }
                }
                else
                {
                    // Check if user actually has a password set
                    if (!string.IsNullOrEmpty(currentUser.PasswordHash))
                    {
                        return Json(new { success = false, message = "Vui lòng nhập mật khẩu hiện tại." });
                    }
                    // If user has no password set, allow empty current password
                }

                // Check if new password is different from current password
                if (model.CurrentPassword == model.NewPassword)
                {
                    return Json(new { success = false, message = "Mật khẩu mới phải khác mật khẩu hiện tại." });
                }

                // Update password
                var result = await _userService.UpdatePasswordWithUserIdAsync(userId.Value, model.NewPassword);
                if (result.Success)
                {
                    // Create notification for successful password change
                    var notification = new Notification
                    {
                        UserId = userId.Value,
                        Title = "Đổi mật khẩu thành công",
                        Message = "Mật khẩu của bạn đã được thay đổi thành công.",
                        CreatedAt = DateTime.Now,
                        IsRead = false
                    };
                    
                    await _notificationService.AddAsync(notification);
                    
                    return Json(new { success = true, message = "Đổi mật khẩu thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[UserHomeController] Error changing password: " + ex.Message);
                return Json(new { success = false, message = "Có lỗi xảy ra khi thay đổi mật khẩu: " + ex.Message });
            }
        }

        public async Task<IActionResult> NotificationsPartial()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");

            var notifications = await _notificationService.GetByUserIdAsync(userId.Value);

            var viewModel = notifications.Select(n => new NotificationViewModel
            {
                Title = n.Title,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            }).ToList();

            return PartialView("_NotificationPartial", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");

            await _notificationService.MarkAllAsReadAsync(userId.Value);

            return await NotificationsPartial();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");

            await _notificationService.DeleteAllAsync(userId.Value);

            return await NotificationsPartial();
        }

        public IActionResult ListPetPartial()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            var pets = _petService.GetPetsByUserId(userId.Value)
       .Where(p => p.IsActive == true) // Không cần check lại UserID vì repo đã lọc rồi
       .Select(p => new PetViewModel
       {
           Id = p.PetId,
           Name = p.Name,
           Species = _speciesService.GetSpeciesNameById(p.SpeciesId), // tránh null
           Gender = p.Gender,
           HealthCondition = p.HealthCondition,
           Note = p.SpecialNotes
       })
       .ToList();

            return PartialView("_ListPetPartial", pets);
        }

        public async Task<IActionResult> DeletePetPartial(int id)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return PartialView("_ErrorPartial", "Vui lòng đăng nhập để xóa thú cưng.");
            }

            var pet = _petService.GetPetsByUserId(userId.Value).FirstOrDefault(p => p.PetId == id);
            if (pet == null)
            {
                return PartialView("_ErrorPartial", "Không tìm thấy thú cưng để xóa.");
            }

            return PartialView("_DeletePetPartial", new { PetId = id, PetName = pet.Name });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePet(int id)
        {
            Console.WriteLine("[UserHomeController] DeletePet POST called for PetId: " + id);

            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                Console.WriteLine("[UserHomeController] UserId is null");
                return Json(new { success = false, message = "Vui lòng đăng nhập để xóa thú cưng." });
            }

            var pet = _petService.GetPetsByUserId(userId.Value).FirstOrDefault(p => p.PetId == id);
            if (pet == null)
            {
                Console.WriteLine("[UserHomeController] Pet not found for PetId: " + id);
                return Json(new { success = false, message = "Không tìm thấy thú cưng để xóa." });
            }

            try
            {
                Console.WriteLine("[UserHomeController] Attempting to delete pet... PetId: " + id + ", UserId: " + userId);
                await _petService.DeletePetAsync(id);
                Console.WriteLine("[UserHomeController] Pet deleted successfully, PetId: " + id);
                
                // Tạo thông báo khi xóa thú cưng thành công
                var notification = new Notification
                {
                    UserId = userId.Value,
                    Title = "Xóa thú cưng thành công",
                    Message = $"Thú cưng '{pet.Name}' đã được xóa khỏi hồ sơ của bạn.",
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };
                
                await _notificationService.AddAsync(notification);
                
                return Json(new { success = true, message = "Xóa thú cưng thành công!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("[UserHomeController] Error deleting pet: " + ex.Message + " - StackTrace: " + ex.StackTrace);
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa thú cưng: " + ex.Message });
            }
        }

        public IActionResult ListOrderPartial()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return PartialView("_ListOrderPartial", new List<pet_spa_system1.ViewModel.OrderViewModel>());
            }
            var orders = _orderService.GetOrdersByUserId(userId);
            return PartialView("_ListOrderPartial", orders);
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

        public async Task<IActionResult> EditPetPartial(int id)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return PartialView("_ErrorPartial", "Không tìm thấy người dùng.");
            }
            var pet = _petService.GetPetsByUserId(userId.Value).FirstOrDefault(p => p.PetId == id);
            if (pet == null)
            {
                return PartialView("_ErrorPartial", "Không tìm thấy thú cưng.");
            }
            var speciesList = await _petService.GetAllSpeciesAsync() ?? new List<Species>();
            var model = new PetDetailViewModel
            {
                Pet = pet,
                SpeciesList = speciesList
            };
            return PartialView("EditPetPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePet(int id, PetDetailViewModel model, IFormFile ImageFile)
        {
            Console.WriteLine("[UserHomeController] UpdatePet POST called");
            Console.WriteLine($"Received data: PetId={id}, Name={model.Pet?.Name}, SpeciesId={model.Pet?.SpeciesId}, " +
                              $"Gender={model.Pet?.Gender}, UserId={model.Pet?.UserId}, IsActive={model.Pet?.IsActive}");

            // Gán PetId từ route để đảm bảo không bị override
            model.Pet.PetId = id;

            // Loại bỏ validation cho ImageFile nếu không bắt buộc
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
                return PartialView("EditPetPartial", model);
            }

            var pet = model.Pet;
            if (pet.SpeciesId == null)
            {
                Console.WriteLine("[UserHomeController] Warning: SpeciesId is null");
                model.SpeciesList = await _petService.GetAllSpeciesAsync() ?? new List<Species>();
                return PartialView("EditPetPartial", model);
            }

            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                Console.WriteLine("[UserHomeController] UserId is null, returning ErrorPartial");
                return PartialView("_ErrorPartial", "Vui lòng đăng nhập để chỉnh sửa thú cưng.");
            }

            // Lấy pet hiện tại để giữ nguyên IsActive
            var existingPet = await _petService.GetPetByIdAsync(pet.PetId);
            if (existingPet == null)
            {
                Console.WriteLine("[UserHomeController] Existing pet not found for PetId: " + pet.PetId);
                return Json(new { success = false, message = "Không tìm thấy thú cưng để cập nhật" });
            }

            // Cập nhật các thuộc tính từ form, giữ nguyên IsActive
            existingPet.Name = pet.Name;
            existingPet.SpeciesId = pet.SpeciesId;
            existingPet.Breed = pet.Breed;
            existingPet.Age = pet.Age;
            existingPet.Gender = pet.Gender;
            existingPet.SpecialNotes = pet.SpecialNotes;
            existingPet.UserId = userId.Value; // Gán UserId từ session

            var images = ImageFile != null ? new List<IFormFile> { ImageFile } : new List<IFormFile>();
            try
            {
                Console.WriteLine("[UserHomeController] Attempting to update pet... IsActive before: " + existingPet.IsActive);
                await _petService.UpdatePetAsync(existingPet, images);
                Console.WriteLine("[UserHomeController] Pet updated successfully, PetId: " + existingPet.PetId + ", IsActive after: " + existingPet.IsActive);
                
                // Tạo thông báo khi cập nhật thú cưng thành công
                var notification = new Notification
                {
                    UserId = userId.Value,
                    Title = "Cập nhật thú cưng thành công",
                    Message = $"Thông tin thú cưng '{existingPet.Name}' đã được cập nhật.",
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };
                
                await _notificationService.AddAsync(notification);
                
                return Json(new { success = true, message = "Cập nhật thú cưng thành công!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("[UserHomeController] Error processing pet: " + ex.Message + " - StackTrace: " + ex.StackTrace);
                return Json(new { success = false, message = $"Lỗi khi cập nhật thú cưng: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile(UserViewModel model, IFormFile Avatar)
        {
            ModelState.Remove("Avatar");

            if (!ModelState.IsValid)
            {
                return await Hoso(errorMessage: "Dữ liệu không hợp lệ.");
            }

            int? userId = HttpContext.Session.GetInt32("CurrentUserId");

            if (userId == null)
            {
                return await Hoso(errorMessage: "Không tìm thấy người dùng.");
            }

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null)
            {
                return await Hoso(errorMessage: "Không tìm thấy người dùng.");
            }

            user.FullName = model.FullName;
            user.Username = model.UserName;
            user.Address = model.Address;
            user.Phone = model.PhoneNumber;

            if (Avatar != null && Avatar.Length > 0)
            {
                string imageUrl = await _userService.UploadAvatarAsync(Avatar);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    user.ProfilePictureUrl = imageUrl;
                }
            }

            var result = await _userService.EditUserAsync(user);

            if (!result.Success)
            {
                return await Hoso(errorMessage: result.Message ?? "Cập nhật thất bại.");
            }

            // Tạo thông báo khi cập nhật hồ sơ thành công
            var notification = new Notification
            {
                UserId = userId.Value,
                Title = "Cập nhật hồ sơ thành công",
                Message = $"Thông tin hồ sơ của bạn đã được cập nhật thành công.",
                CreatedAt = DateTime.Now,
                IsRead = false
            };
            
            await _notificationService.AddAsync(notification);

            HttpContext.Session.SetString("CurrentUserName", user.Username);
            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                HttpContext.Session.SetString("CurrentUserAvatar", user.ProfilePictureUrl);
            }
            else
            {
                HttpContext.Session.Remove("CurrentUserAvatar");
            }
            //Console.WriteLine("✅ [Session] Cập nhật CurrentUserName trong session.");

            // Clear any existing TempData to prevent persistence
            TempData.Remove("SuccessMessage");
            TempData.Remove("ErrorMessage");

            return await Hoso(successMessage: result.Message ?? "Cập nhật thành công.");
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
                return PartialView("AddPetPartial", model);
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
                Console.WriteLine("[UserHomeController] Error adding pet: " + ex.Message + " - StackTrace: " + ex.StackTrace);
                return Json(new { success = false, message = $"Lỗi khi thêm thú cưng: {ex.Message}" });
            }
        }
        public IActionResult EmployeeSchedulePartial()
        {
            int? employeeId = HttpContext.Session.GetInt32("CurrentUserId");
            if (employeeId == null) return PartialView("_ErrorPartial", "Vui lòng đăng nhập.");

            var appointments = _appointmentService.GetUpcomingScheduleByEmployee(employeeId.Value);
            return PartialView("_ScheduleEmployeePartial", appointments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearTempData()
        {
            TempData.Remove("SuccessMessage");
            TempData.Remove("ErrorMessage");
            return Json(new { success = true });
        }




    }
}