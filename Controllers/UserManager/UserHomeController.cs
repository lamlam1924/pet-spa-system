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
using Microsoft.EntityFrameworkCore.Migrations;

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
           Species = _speciesService.GetSpeciesNameById(p.SpeciesId), // tránh null
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

        public IActionResult AddPetPartial()
        {
            // Nếu cần truyền model, có thể truyền model rỗng hoặc dữ liệu mặc định
            return PartialView("AddPetPartial");
        }

        public IActionResult EditPetPartial(int id)
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
            // Chuyển sang ViewModel nếu cần
            var petViewModel = new pet_spa_system1.ViewModel.PetViewModel
            {
                Id = pet.PetId,
                Name = pet.Name,
                Species = _speciesService.GetSpeciesNameById(pet.SpeciesId),
                Gender = pet.Gender,
                HealthCondition = pet.HealthCondition,
                Note = pet.SpecialNotes
            };
            return PartialView("EditPetPartial", petViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile(UserViewModel model, IFormFile Avatar)
        {
            // Bỏ qua lỗi required cho Avatar nếu không upload file mới
            // if (ModelState.ContainsKey("Avatar") && Avatar == null)
            // {
            //     ModelState["Avatar"].Errors.Clear();
            // }
            ModelState.Remove("Avatar");
            Console.WriteLine("🔧 [UpdateUserProfile] Bắt đầu cập nhật hồ sơ...");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ [ModelState] Danh sách lỗi chi tiết:");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    if (errors.Count == 0)
                    {
                        Console.WriteLine($"    ⚪ Field: {key}, No error.");
                    }
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"    🔴 Field: {key}, Error: {error.ErrorMessage}");
                    }
                }
                Console.WriteLine($"Tổng số lỗi: {ModelState.Values.SelectMany(v => v.Errors).Count()}");
                Console.WriteLine("Các key trong ModelState:");
                foreach (var key in ModelState.Keys)
                {
                    Console.WriteLine($"- {key}");
                }
                return await Hoso(errorMessage: "Dữ liệu không hợp lệ.");
            }

            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            Console.WriteLine($"ℹ️ [Session] CurrentUserId: {userId}");

            if (userId == null)
            {
                Console.WriteLine("❌ [Session] Không tìm thấy CurrentUserId trong session.");
                return await Hoso(errorMessage: "Không tìm thấy người dùng.");
            }

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null)
            {
                Console.WriteLine($"❌ [DB] Không tìm thấy user với ID = {userId}");
                return await Hoso(errorMessage: "Không tìm thấy người dùng.");
            }

            Console.WriteLine($"✅ [User] Tìm thấy user: {user.Username} - Bắt đầu cập nhật thông tin...");

            user.FullName = model.FullName;
            user.Username = model.UserName;
            user.Address = model.Address;
            user.Phone = model.PhoneNumber;

            if (Avatar != null && Avatar.Length > 0)
            {
                Console.WriteLine($"📸 [Avatar] File name: {Avatar.FileName}, size: {Avatar.Length}");
                string imageUrl = await _userService.UploadAvatarAsync(Avatar);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    Console.WriteLine("✅ [Avatar] Upload thành công. URL: " + imageUrl);
                    user.ProfilePictureUrl = imageUrl;
                }
                else
                {
                    Console.WriteLine("⚠️ [Avatar] Upload thất bại hoặc URL rỗng.");
                }
            }
            else
            {
                Console.WriteLine("ℹ️ [Avatar] Không có avatar mới được upload.");
            }

            var result = await _userService.EditUserAsync(user);
            Console.WriteLine($"📝 [DB Update] Kết quả cập nhật: {(result.Success ? "Thành công" : "Thất bại")}. Message: {result.Message}");

            if (!result.Success)
            {
                return await Hoso(errorMessage: result.Message ?? "Cập nhật thất bại.");
            }

            HttpContext.Session.SetString("CurrentUserName", user.Username);
            Console.WriteLine("✅ [Session] Cập nhật CurrentUserName trong session.");

            return await Hoso(successMessage: result.Message ?? "Cập nhật thành công.");
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

        //public IActionResult AddPetPartial()
        //{
        //    // Nếu cần truyền model, có thể truyền model rỗng hoặc dữ liệu mặc định
        //    return PartialView("AddPetPartial");
        //}

    }
}