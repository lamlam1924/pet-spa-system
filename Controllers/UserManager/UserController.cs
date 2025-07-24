using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> List(string search = "", string sort = "", int? roleId = null, int page = 1, int pageSize = 10)
        {
            var users = await _userService.GetActiveUsersAsync(search, sort, page, pageSize);
            // Lấy tất cả user để đếm tổng số (không phân trang)
            var allUsers = await _userService.GetActiveUsersAsync(search, sort, 1, int.MaxValue);
            if (roleId.HasValue)
            {
                users = users.Where(u => u.RoleId == roleId.Value).ToList();
                allUsers = allUsers.Where(u => u.RoleId == roleId.Value).ToList();
            }
            var totalCount = allUsers.Count;
            return Json(new {
                data = users.Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.Email,
                    u.FullName,
                    u.Phone,
                    u.Address,
                    Role = u.Role != null ? u.Role.RoleName : "N/A",
                    RoleId = u.RoleId,
                    Status = u.IsActive == true ? "On" : "Off",
                    CreatedAt = u.CreatedAt.HasValue ? u.CreatedAt.Value.ToString("yyyy-MM-dd") : null,
                    IsActive = u.IsActive
                }),
                totalCount
            });
        }

        [HttpGet]
        [Route("User/GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _userService.GetActiveRolesAsync();
                return Json(roles.Select(r => new { r.RoleId, r.RoleName, r.Description }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching roles: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            var result = await _userService.CreateUserAsync(user);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(new { message = result.Message, defaultPassword = result.DefaultPassword });
        }

        [HttpPost]
        [Route("User/Edit")]
        public async Task<IActionResult> Edit([FromBody] User updated)
        {
            var result = await _userService.EditUserAsync(updated);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(new { message = result.Message });
        }

        [HttpPost]
        [Route("User/Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.Success)
                return NotFound(result.Message);
            return Ok(new { message = result.Message });
        }

        [HttpPost]
        [Route("User/Restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _userService.RestoreUserAsync(id);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(new { message = result.Message });
        }

        [HttpGet]
        [Route("User/Deleted")]
        public async Task<IActionResult> GetDeletedUsers(int? roleId = null)
        {
            try
            {
                var deletedUsers = await _userService.GetDeletedUsersAsync();
                if (roleId.HasValue)
                    deletedUsers = deletedUsers.Where(u => u.RoleId == roleId.Value).ToList();
                var result = deletedUsers.Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.Email,
                    u.FullName,
                    u.Phone,
                    u.Address,
                    Role = u.Role != null ? u.Role.RoleName : "N/A",
                    RoleActive = u.Role != null ? u.Role.IsActive : false,
                    CreatedAt = u.CreatedAt.HasValue ? u.CreatedAt.Value.ToString("yyyy-MM-dd") : null,
                    UpdatedAt = u.UpdatedAt.HasValue ? u.UpdatedAt.Value.ToString("yyyy-MM-dd") : null
                }).OrderByDescending(u => u.UpdatedAt).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching deleted users: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("User/ResetPassword")]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var result = await _userService.ResetPasswordAsync(id);
            if (!result.Success)
                return NotFound(result.Message);
            return Ok(new { message = result.Message, newPassword = result.NewPassword });
        }

        // ========== STAFF MANAGEMENT ========== //

        [HttpGet]
        [Route("User/StaffList")]
        public async Task<IActionResult> StaffList(string search = "", string sort = "")
        {
            var staff = await _userService.GetStaffListAsync(search, sort);
            return Json(staff.Select(u => new
            {
                u.UserId,
                u.Username,
                u.Email,
                u.FullName,
                u.Phone,
                u.Address,
                u.ProfilePictureUrl,
                u.IsActive,
                u.CreatedAt,
                u.UpdatedAt,
                Role = u.Role != null ? u.Role.RoleName : "N/A",
                RoleId = u.RoleId
            }));
        }

        [HttpPost]
        [Route("User/StaffCreate")]
        public async Task<IActionResult> StaffCreate([FromBody] User user)
        {
            user.RoleId = 3; // Đảm bảo là staff
            var result = await _userService.CreateUserAsync(user);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(new { message = result.Message, defaultPassword = result.DefaultPassword });
        }

        [HttpPost]
        [Route("User/StaffEdit")]
        public async Task<IActionResult> StaffEdit([FromBody] User updated)
        {
            updated.RoleId = 3; // Đảm bảo là staff
            var result = await _userService.EditUserAsync(updated);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(new { message = result.Message });
        }

        [HttpPost]
        [Route("User/StaffDelete")]
        public async Task<IActionResult> StaffDelete(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.Success)
                return NotFound(result.Message);
            return Ok(new { message = result.Message });
        }

        [HttpPost]
        [Route("User/StaffRestore")]
        public async Task<IActionResult> StaffRestore(int id)
        {
            var result = await _userService.RestoreUserAsync(id);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(new { message = result.Message });
        }

        [HttpPost]
        [Route("User/StaffToggleLock")]
        public async Task<IActionResult> StaffToggleLock(int id)
        {
            var result = await _userService.ToggleLockStaffAsync(id);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(new { message = result.Message });
        }

        [HttpGet]
        [Route("Admin/StaffDetail/{id}")]
        public async Task<IActionResult> StaffDetail(int id, [FromServices] IAdminStaffScheduleService scheduleService)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                Console.WriteLine("[AdminController] User not authenticated, redirecting or allowing anonymous access.");
                return RedirectToAction("AccessDenied", "Account");
            }
            var staff = await _userService.GetStaffDetailAsync(id);
            if (staff == null) return NotFound();
            var appointments = await scheduleService.GetAppointmentsAsync(staffId: id);
            var now = DateTime.Now;
            var todayCount = appointments.Count(a => a.AppointmentDate.Date == now.Date);
            var monthCount = appointments.Count(a => a.AppointmentDate.Month == now.Month && a.AppointmentDate.Year == now.Year);
            var allAppointments = await _userService.GetAppointmentsByStaffIdAsync(id);
            // Tính hiệu suất làm việc
            int totalAppointments = allAppointments.Count;
            int completedAppointments = allAppointments.Count(a => a.Status?.StatusName == "Hoàn thành" || a.Status?.StatusName == "Completed");
            int cancelledAppointments = allAppointments.Count(a => a.Status?.StatusName == "Đã hủy" || a.Status?.StatusName == "Cancelled");
            int uniqueCustomers = allAppointments.Select(a => a.UserId).Distinct().Count();
            var vm = new StaffDetailViewModel
            {
                UserId = staff.UserId,
                FullName = staff.FullName,
                Email = staff.Email,
                Phone = staff.Phone,
                Address = staff.Address,
                IsActive = staff.IsActive ?? false,
                TodayCount = todayCount,
                MonthCount = monthCount,
                ProfilePictureUrl = staff.ProfilePictureUrl,
                AllAppointments = allAppointments,
                // Hiệu suất làm việc
                PerformanceStats = new StaffPerformanceStats
                {
                    TotalAppointments = totalAppointments,
                    CompletedAppointments = completedAppointments,
                    CancelledAppointments = cancelledAppointments,
                    UniqueCustomers = uniqueCustomers,
                    TotalRevenue = 0 // Nếu muốn tính doanh thu, cần join thêm bảng Order
                }
            };
            return View("~/Views/Admin/StaffDetail.cshtml", vm);
        }

        [HttpPost]
        public async Task<IActionResult> StaffDetail(StaffDetailViewModel model, IFormFile imageFile, [FromServices] IAdminStaffScheduleService scheduleService)
        {
            var staff = await _userService.GetStaffDetailAsync(model.UserId);
            if (staff == null) return NotFound();
            // Upload ảnh nếu có
            if (imageFile != null && imageFile.Length > 0)
            {
                var account = new Account(
                    "dprp1jbd9", // cloud_name
                    "584135338254938", // api_key
                    "QbUYngPIdZcXEn_mipYn8RE5dlo" // api_secret
                );
                var cloudinary = new Cloudinary(account);
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream())
                };
                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                string imageUrl = uploadResult.SecureUrl.ToString();
                if (staff.ProfilePictureUrl != imageUrl)
                {
                    staff.ProfilePictureUrl = imageUrl;
                }
            }
            // Cập nhật thông tin nếu có thay đổi
            if (staff.FullName != model.FullName)
                staff.FullName = model.FullName;
            if (staff.Email != model.Email)
                staff.Email = model.Email;
            if (staff.Phone != model.Phone)
                staff.Phone = model.Phone;
            if (staff.Address != model.Address)
                staff.Address = model.Address;
            await _userService.EditUserAsync(staff);
            // Sau khi lưu, redirect lại chính trang StaffDetail
            return RedirectToAction("StaffDetail", new { id = model.UserId });
        }

        [HttpGet]
        [Route("User/StaffStats")]
        public async Task<IActionResult> StaffStats(int id)
        {
            var stats = await _userService.GetStaffStatsAsync(id);
            return Json(stats);
        }

        [HttpPost]
        [Route("User/StaffSetActive")]
        public async Task<IActionResult> StaffSetActive(int id, bool isActive)
        {
            var result = await _userService.SetUserActiveAsync(id, isActive);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(new { message = result.Message });
        }

        [HttpGet]
        public IActionResult StaffUploadImage(int userId)
        {
            return View("~/Views/Admin/StaffUploadImage.cshtml", userId);
        }

        [HttpPost]
        public async Task<IActionResult> StaffUploadImage(int userId, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var account = new Account(
                    "dprp1jbd9", // cloud_name
                    "584135338254938", // api_key
                    "QbUYngPIdZcXEn_mipYn8RE5dlo" // api_secret
                );
                var cloudinary = new Cloudinary(account);
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream())
                };
                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                string imageUrl = uploadResult.SecureUrl.ToString();
                // Lưu imageUrl vào DB
                var user = await _userService.GetStaffDetailAsync(userId);
                if (user != null)
                {
                    user.ProfilePictureUrl = imageUrl;
                    await _userService.EditUserAsync(user);
                }
                return RedirectToAction("StaffDetail", new { id = userId });
            }
            return View("~/Views/Admin/StaffUploadImage.cshtml", userId);
        }

        [HttpGet]
        [Route("Admin/UserDetail/{id}")]
        public async Task<IActionResult> UserDetail(int id, [FromServices] IUserService userService)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                Console.WriteLine("[AdminController] User not authenticated, redirecting or allowing anonymous access.");
                return RedirectToAction("AccessDenied", "Account");
            }
            var user = await userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            var pets = await userService.GetPetsByUserIdAsync(id);
            var appointments = await userService.GetAppointmentsByUserIdAsync(id);
            var orders = await userService.GetOrdersByUserIdAsync(id);
            var reviews = await userService.GetReviewsByUserIdAsync(id);
            var payments = await userService.GetPaymentsByUserIdAsync(id);
            var vm = new UserDetailViewModel
            {
                User = user,
                Pets = pets,
                Appointments = appointments,
                Orders = orders,
                Reviews = reviews,
                Payments = payments
            };
            return View("~/Views/Admin/UserDetail.cshtml", vm);
        }

        [HttpPost]
        [Route("Admin/UserDetail/{id}")]
        public async Task<IActionResult> UserDetail(int id, [FromForm] string FullName, [FromForm] string Email, [FromForm] string Phone, [FromForm] string Address, IFormFile AvatarFile, [FromServices] IUserService userService)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                Console.WriteLine("[AdminController] User not authenticated, redirecting or allowing anonymous access.");
                return RedirectToAction("AccessDenied", "Account");
            }
            var user = await userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            // Upload avatar if provided
            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                string imageUrl = await userService.UploadAvatarAsync(AvatarFile);
                if (!string.IsNullOrEmpty(imageUrl) && user.ProfilePictureUrl != imageUrl)
                {
                    user.ProfilePictureUrl = imageUrl;
                }
            }
            // Update info
            if (user.FullName != FullName) user.FullName = FullName;
            if (user.Email != Email) user.Email = Email;
            if (user.Phone != Phone) user.Phone = Phone;
            if (user.Address != Address) user.Address = Address;
            await userService.EditUserAsync(user);
            return RedirectToAction("UserDetail", new { id = user.UserId });
        }

        [HttpGet]
        [Route("Admin/List_Customer")]
        public async Task<IActionResult> List_Customer()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                Console.WriteLine("[AdminController] User not authenticated, redirecting or allowing anonymous access.");
                return RedirectToAction("AccessDenied", "Account");
            }
            var users = await _userService.GetActiveUsersAsync();
            var customers = users.Where(u => u.RoleId == 2).ToList();
            // Chỉ định rõ đường dẫn view
            return View("~/Views/Admin/List_Customer.cshtml", customers);
        }

        // ========== USER HOME/PROFILE ========== //
        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction("Index", "UserHome");
            }

            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null)
                return NotFound("User not found");

            user.FullName = model.FullName;
            user.Username = model.UserName;
            user.Address = model.Address;
            user.Phone = model.PhoneNumber;

            var result = await _userService.EditUserAsync(user);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? "Cập nhật thất bại.";
                return RedirectToAction("Index", "UserHome");
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index", "UserHome");
        }
    }
}