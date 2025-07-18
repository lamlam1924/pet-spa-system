using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.Utils;
using pet_spa_system1.ViewModel;
using System.Threading.Tasks;
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
        public UserHomeController(
            IUserService userService,
            IPetService petService,
            ISpeciesService speciesService,
            IOrderService orderService,
            IOrderItemService orderItemService,
            IProductService productService,
            IOrderStatusService orderStatusService,
            INotificationService notificationService)
        {
            _userService = userService;
            _petService = petService;
            _speciesService = speciesService;
            _orderService = orderService;
            _orderItemService = orderItemService;
            _productService = productService;
            _orderStatusService = orderStatusService;
            _notificationService = notificationService;
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
            return View(userModel); // truyền model vào view
        }
        public async Task<IActionResult> Hoso()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            var currentUser = await _userService.GetUserByIdAsync(userId.Value);
            var userModel = new UserViewModel
            {
                UserName = currentUser.Username,
                Email = currentUser.Email,
                PhoneNumber = currentUser.Phone,
                Address = currentUser.Address,
                FullName = currentUser.FullName
            };

            return PartialView("_HosoPartial", userModel);
        }
        public IActionResult ChangePasswordPartial()
        {
            var model = new ChangePasswordViewModel(); // model trống ban đầu
            return PartialView("_ChangePasswordPartial", model);
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

            return await NotificationsPartial(); // reload lại view với thông báo đã đọc
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");

            await _notificationService.DeleteAllAsync(userId.Value);

            return await NotificationsPartial(); // reload lại view với danh sách rỗng
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
           Species =  _speciesService.GetSpeciesNameById(p.SpeciesId), // tránh null
           Gender = p.Gender,
           HealthCondition = p.HealthCondition,
           Note = p.SpecialNotes
       })
       .ToList();

            return PartialView("_ListPetPartial", pets);
        }
        public async Task<IActionResult> ListOrderPartial()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return PartialView("_ListOrderPartial", new List<pet_spa_system1.ViewModel.OrderViewModel>());
            }
            // Lấy danh sách đơn hàng động từ service
            var orders = await Task.Run(() => _orderService.GetOrdersByUserId(userId));
            return PartialView("_ListOrderPartial", orders);
        }



         [HttpPost]
        public async Task<IActionResult> UpdateUserProfile(UserViewModel model, IFormFile Avatar)
        {
            ModelState.Remove("Avatar"); // Bỏ qua lỗi required cho Avatar nếu có
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ: " + string.Join("; ", errors);
                Console.WriteLine("[UpdateUserProfile] ModelState errors: " + string.Join(" | ", errors));
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
                return RedirectToAction("Index");
            }

            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null)
                return NotFound("User not found");

            user.FullName = model.FullName;
            user.Username = model.UserName;
            user.Address = model.Address;
            user.Phone = model.PhoneNumber;
            // KHÔNG gán user.RoleId, giữ nguyên giá trị hiện tại

            // Xử lý upload avatar nếu có
            if (Avatar != null && Avatar.Length > 0)
            {
                string imageUrl = await _userService.UploadAvatarAsync(Avatar);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    user.ProfilePictureUrl = imageUrl;
                }
            }
            else
            {
                // Không upload ảnh mới, giữ lại ảnh cũ
                user.ProfilePictureUrl = model.ProfilePictureUrl;
            }

            var result = await _userService.EditUserAsync(user);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? "Cập nhật thất bại.";
                return RedirectToAction("Index");
            }

            HttpContext.Session.SetString("CurrentUserName", user.Username);
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult DeletePet(int id)
        {
            _petService.DisablePetAsync(id);
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            var pets = _petService.GetPetsByUserId(userId.Value)
                .Select(p => new PetViewModel
                {
                    Id = p.PetId,
                    Name = p.Name,
                    Species = _speciesService.GetSpeciesNameById(p.SpeciesId),
                    Gender = p.Gender,
                    HealthCondition = p.HealthCondition,
                    Note = p.SpecialNotes
                }).ToList();

            return PartialView("_ListPetPartial", pets); // hoặc đổi tên tương ứng nếu bạn dùng tên khác
        }

    }
}
