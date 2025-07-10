using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using pet_spa_system1.Services;
using pet_spa_system1.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pet_spa_system1.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffService _staffService;
        private readonly IStaffRepository _staffRepository;

        public StaffController(IStaffService staffService, IStaffRepository staffRepository)
        {
            _staffService = staffService ?? throw new ArgumentNullException(nameof(staffService));
            _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        }

        public async Task<IActionResult> Profile()
        {
            var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (currentUser.RoleId != 1 && currentUser.RoleId != 3)
            {
                return Unauthorized("Bạn không có quyền truy cập trang này.");
            }

            if (!await _staffRepository.StaffExistsAsync(currentUser.UserId))
            {
                return NotFound("Nhân viên không tồn tại.");
            }

            var user = await _staffService.GetCurrentStaffProfileAsync();
            ViewData["CurrentUser"] = currentUser;
            Console.WriteLine($"UserId đang đăng nhập: {currentUser.UserId}");
            return View("Staff_Profile", user);
        }

        [HttpGet]
        public async Task<IActionResult> EditStaffProfile()
        {
            var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (currentUser.RoleId != 1 && currentUser.RoleId != 3)
            {
                return Unauthorized("Bạn không có quyền chỉnh sửa hồ sơ.");
            }

            if (!await _staffRepository.StaffExistsAsync(currentUser.UserId))
            {
                return NotFound("Nhân viên không tồn tại.");
            }

            var user = await _staffService.GetCurrentStaffProfileAsync();
            ViewData["CurrentUser"] = currentUser;
            Console.WriteLine($"UserId đang đăng nhập: {currentUser.UserId}");
            return View("Edit_Staff_Profile", user);
        }

        [HttpPost]
        public async Task<IActionResult> EditStaffProfile(IFormCollection form)
        {
            var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (currentUser.RoleId != 1 && currentUser.RoleId != 3)
            {
                return Unauthorized("Bạn không có quyền chỉnh sửa hồ sơ.");
            }

            if (!await _staffRepository.StaffExistsAsync(currentUser.UserId))
            {
                return NotFound("Nhân viên không tồn tại.");
            }

            // Log dữ liệu từ form để debug
            Console.WriteLine($"UserId đang đăng nhập: {currentUser.UserId}");
            Console.WriteLine($"Form data: RoleId={form["RoleId"]}, Email={form["Email"]}, Username={form["Username"]}, " +
                             $"FullName={form["FullName"]}, Phone={form["Phone"]}, Address={form["Address"]}, " +
                             $"ProfilePictureUrl={form["ProfilePictureUrl"]}");

            // Lấy dữ liệu từ form
            var roleId = !string.IsNullOrEmpty(form["RoleId"]) ? int.Parse(form["RoleId"]) : currentUser.RoleId;
            var email = form["Email"].ToString() ?? currentUser.Email;
            var username = form["Username"].ToString() ?? currentUser.Username;
            var fullName = form["FullName"].ToString();
            var phone = form["Phone"].ToString();
            var address = form["Address"].ToString();
            var profilePictureUrl = form["ProfilePictureUrl"].ToString();

            // Kiểm tra ít nhất một trường tùy chọn được điền
            if (string.IsNullOrEmpty(fullName) && string.IsNullOrEmpty(phone) &&
                string.IsNullOrEmpty(address) && string.IsNullOrEmpty(profilePictureUrl))
            {
                ViewBag.ValidationErrors = new List<string> { "Vui lòng nhập ít nhất một trường để cập nhật." };
                ViewData["CurrentUser"] = currentUser;
                return View("Edit_Staff_Profile", currentUser);
            }

            try
            {
                var currentProfile = await _staffService.GetCurrentStaffProfileAsync();
                var updatedUser = new User
                {
                    UserId = currentProfile.UserId,
                    Username = username,
                    Email = email,
                    RoleId = roleId,
                    FullName = !string.IsNullOrEmpty(fullName) ? fullName : currentProfile.FullName,
                    Phone = !string.IsNullOrEmpty(phone) ? phone : currentProfile.Phone,
                    Address = !string.IsNullOrEmpty(address) ? address : currentProfile.Address,
                    ProfilePictureUrl = !string.IsNullOrEmpty(profilePictureUrl) ? profilePictureUrl : currentProfile.ProfilePictureUrl,
                    IsActive = currentProfile.IsActive,
                    CreatedAt = currentProfile.CreatedAt,
                    UpdatedAt = DateTime.Now,
                    PasswordHash = currentProfile.PasswordHash
                };

                await _staffService.SaveStaffProfileChangesAsync(updatedUser);
                TempData["SuccessMessage"] = "Hồ sơ đã được cập nhật thành công!";
                return RedirectToAction("Profile", "Staff");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving profile: {ex.Message}");
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi lưu hồ sơ. Vui lòng thử lại.";
                ViewData["CurrentUser"] = currentUser;
                return View("Edit_Staff_Profile", currentUser);
            }
        }
    }
}