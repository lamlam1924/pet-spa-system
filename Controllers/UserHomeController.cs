using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.Utils;
using pet_spa_system1.ViewModel;
using System.Threading.Tasks;

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
        public async Task<IActionResult> ListOrderPartial(int? statusId)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
                return Unauthorized();

            var orders = _orderService.GetOrdersByUserId(userId.Value);

            // Lọc theo statusId nếu có
            if (statusId.HasValue)
                orders = orders.Where(o => o.StatusId == statusId.Value).ToList();

            var orderViewModels = new List<OrderViewModel>();

            foreach (var order in orders)
            {
                var orderItems = _orderItemService.GetOrderItemsByOrderId(order.OrderId);

                foreach (var item in orderItems)
                {
                    var product = await _productService.GetProductByIdAsync(item.ProductId);
                    var statusName = _orderStatusService.GetStatusNameById(order.StatusId);

                    orderViewModels.Add(new OrderViewModel
                    {
                        OrderID = "ORD" + order.OrderId.ToString("D3"),
                        ProductName = product?.Name ?? "Không rõ",
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        StatusName = statusName
                    });
                }
            }

            return PartialView("_ListOrderPartial", orderViewModels);
        }




        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile(UserViewModel model, IFormFile Avatar)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                foreach (var state in ModelState)
                {
                    var key = state.Key;
                    var errors = state.Value.Errors;

                    foreach (var error in errors)
                    {
                        Console.WriteLine($" - Field: {key}, Error: {error.ErrorMessage}");
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

            // Xử lý upload avatar nếu có
            if (Avatar != null && Avatar.Length > 0)
            {
                string imageUrl = await _userService.UploadAvatarAsync(Avatar);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    user.ProfilePictureUrl = imageUrl;
                }
            }
            // Đảm bảo luôn gán lại user.ProfilePictureUrl trước khi gọi EditUserAsync
            var result = await _userService.EditUserAsync(user);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? "Cập nhật thất bại.";
                return PartialView("_HosoPartial", model);
            }

            TempData["SuccessMessage"] = result.Message;

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
        [HttpPost]
        public IActionResult DeletePet(int id)
        {
            _petService.DeletePet(id);
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
