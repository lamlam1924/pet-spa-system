using Microsoft.AspNetCore.Mvc;
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
        public UserHomeController(IUserService userService)
        {
            _userService = userService;
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
        public IActionResult NotificationsPartial()
        {
            var notifications = new List<NotificationViewModel>
    {
        new NotificationViewModel
        {
            Title = "Đơn hàng #1234",
            Message = "Đơn hàng của bạn đã được xác nhận.",
            CreatedAt = DateTime.Now.AddHours(-2),
            IsRead = false
        },
        new NotificationViewModel
        {
            Title = "Khuyến mãi mới",
            Message = "Giảm giá 20% cho dịch vụ spa tuần này!",
            CreatedAt = DateTime.Now.AddDays(-1),
            IsRead = true
        }
    };

            return PartialView("_NotificationPartial", notifications);
        }
        [HttpPost]
        public IActionResult MarkAllAsRead()
        {
            // TODO: cập nhật trạng thái tất cả là IsRead = true
            var updatedNotifications = GetFakeNotifications().Select(n => { n.IsRead = true; return n; }).ToList();
            return PartialView("_NotificationPartial", updatedNotifications);
        }

        [HttpPost]
        public IActionResult DeleteAllNotifications()
        {
            // TODO: xóa toàn bộ thông báo (ở đây giả lập trả về danh sách rỗng)
            return PartialView("_NotificationPartial", new List<NotificationViewModel>());
        }

        private List<NotificationViewModel> GetFakeNotifications()
        {
            return new List<NotificationViewModel>
    {
        new NotificationViewModel
        {
            Title = "Đơn hàng #1234",
            Message = "Đơn hàng của bạn đã được xác nhận.",
            CreatedAt = DateTime.Now.AddHours(-2),
            IsRead = false
        },
        new NotificationViewModel
        {
            Title = "Khuyến mãi mới",
            Message = "Giảm giá 20% cho dịch vụ spa tuần này!",
            CreatedAt = DateTime.Now.AddDays(-1),
            IsRead = true
        }
    };
        }
        public IActionResult ListPetPartial()
        {
            var pets = new List<PetViewModel>
    {
        new PetViewModel
        {
            Name = "Milo",
            Species = "Chó",
            Gender = "Đực",
            HealthCondition = "Khỏe mạnh",
            Note = "Rất thân thiện"
        },
        new PetViewModel
        {
            Name = "Kitty",
            Species = "Mèo",
            Gender = "Cái",
            HealthCondition = "Đang theo dõi bệnh da",
            Note = "Không thích người lạ"
        }
    };

            return PartialView("_ListPetPartial", pets);
        }
        public IActionResult ListOrderPartial()
        {
            var orders = new List<OrderViewModel>
    {
        new OrderViewModel
        {
            OrderID = "ORD001",
            ProductName = "Sữa tắm cho chó",
            Quantity = 2,
            TotalAmount = 180000,
            Status = "Chờ xử lý"
        },
        new OrderViewModel
        {
            OrderID = "ORD002",
            ProductName = "Thức ăn cho mèo",
            Quantity = 1,
            TotalAmount = 95000,
            Status = "Đang giao"
        },
        new OrderViewModel
        {
            OrderID = "ORD003",
            ProductName = "Dây dắt thú cưng",
            Quantity = 1,
            TotalAmount = 55000,
            Status = "Đã giao"
        },
        new OrderViewModel
        {
            OrderID = "ORD004",
            ProductName = "Lồng vận chuyển nhỏ",
            Quantity = 1,
            TotalAmount = 220000,
            Status = "Đã hủy"
        },
        new OrderViewModel
        {
            OrderID = "ORD005",
            ProductName = "Bát ăn đôi inox",
            Quantity = 3,
            TotalAmount = 150000,
            Status = "Chờ xử lý"
        }
    };

            return PartialView("_ListOrderPartial", orders);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile(UserViewModel model, IFormFile Avatar)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
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
                return RedirectToAction("Index");
            }

            HttpContext.Session.SetString("CurrentUserName", user.Username);
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }


    }
}
