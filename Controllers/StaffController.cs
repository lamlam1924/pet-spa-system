using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModel;
using pet_spa_system1.Models;

namespace pet_spa_system1.Controllers
{
    public class StaffController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;
        private readonly INotificationService _notificationService;
        private readonly IAdminStaffScheduleService _scheduleService;

        public StaffController(
            IUserService userService, 
            IAppointmentService appointmentService,
            INotificationService notificationService,
            IAdminStaffScheduleService scheduleService)
        {
            _userService = userService;
            _appointmentService = appointmentService;
            _notificationService = notificationService;
            _scheduleService = scheduleService;
        }

        // Staff Dashboard
        public async Task<IActionResult> StaffDashboard()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            int? roleId = HttpContext.Session.GetInt32("CurrentUserRoleId");

            if (userId == null || roleId != 3)
            {
                return RedirectToAction("Login", "Login");
            }

            var staff = await _userService.GetStaffDetailAsync(userId.Value);
            if (staff == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // Lấy thống kê cho staff
            var appointments = await _scheduleService.GetAppointmentsAsync(staffId: userId.Value);
            var now = DateTime.Now;

            // Debug: Log user information
            foreach (var appointment in appointments.Take(3))
            {
                Console.WriteLine($"Appointment {appointment.AppointmentId}: User ID = {appointment.UserId}, " +
                                $"FullName = '{appointment.User?.FullName}', " +
                                $"Username = '{appointment.User?.Username}', " +
                                $"Email = '{appointment.User?.Email}'");

                // Debug ModelUtils
                var displayName = Utils.ModelUtils.GetUserFullName(appointment.User);
                Console.WriteLine($"ModelUtils result: '{displayName}'");
            }
            
            var todayAppointments = appointments.Where(a => a.AppointmentDate.Date == now.Date).ToList();
            var thisWeekAppointments = appointments.Where(a => 
                a.AppointmentDate >= now.StartOfWeek() && 
                a.AppointmentDate <= now.StartOfWeek().AddDays(7)).ToList();
            var thisMonthAppointments = appointments.Where(a => 
                a.AppointmentDate.Month == now.Month && 
                a.AppointmentDate.Year == now.Year).ToList();

            // Lấy notifications
            var notifications = await _notificationService.GetByUserIdAsync(userId.Value);
            var unreadCount = notifications.Count(n => !n.IsRead);

            var viewModel = new StaffDashboardViewModel
            {
                Staff = staff,
                TodayAppointments = todayAppointments,
                ThisWeekAppointments = thisWeekAppointments,
                ThisMonthAppointments = thisMonthAppointments,
                TodayCount = todayAppointments.Count,
                WeekCount = thisWeekAppointments.Count,
                MonthCount = thisMonthAppointments.Count,
                UnreadNotificationCount = unreadCount,
                UpcomingAppointments = appointments.Where(a => a.AppointmentDate > now)
                    .OrderBy(a => a.AppointmentDate)
                    .Take(5)
                    .ToList()
            };

            return View(viewModel);
        }

        // Trang lịch hẹn của staff
        public IActionResult MyAppointments()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            int? roleId = HttpContext.Session.GetInt32("CurrentUserRoleId");

            if (userId == null || roleId != 3)
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }

        // Lấy lịch hẹn của staff (API)
        [HttpGet]
        public async Task<IActionResult> GetMyAppointments(DateTime? date, int? statusId)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var appointments = await _scheduleService.GetAppointmentsAsync(
                staffId: userId.Value,
                date: date,
                statusId: statusId);

            // Transform data to include proper user names
            var transformedAppointments = appointments.Select(a => new
            {
                appointmentId = a.AppointmentId,
                appointmentDate = a.AppointmentDate,
                notes = a.Notes,
                status = a.Status != null ? new { statusName = a.Status.StatusName } : null,
                user = a.User != null ? new
                {
                    fullName = !string.IsNullOrEmpty(a.User.FullName) ? a.User.FullName : a.User.Username,
                    username = a.User.Username,
                    email = a.User.Email,
                    phone = a.User.Phone
                } : null,
                employeeId = a.EmployeeId,
                appointmentPets = a.AppointmentPets?.Select(ap => new
                {
                    pet = new { name = ap.Pet?.Name }
                }).ToList(),
                appointmentServices = a.AppointmentServices?.Select(aps => new
                {
                    service = new { name = aps.Service?.Name }
                }).ToList()
            }).ToList();

            return Json(new { success = true, data = transformedAppointments });
        }

        // Cập nhật trạng thái appointment
        [HttpPost]
        public IActionResult UpdateAppointmentStatus(int appointmentId, int statusId)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var result = _appointmentService.UpdateAppointmentStatus(appointmentId, statusId, userId.Value);
                if (result)
                {
                    return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
                }
                return Json(new { success = false, message = "Không thể cập nhật trạng thái" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Xem chi tiết appointment
        [HttpGet]
        public IActionResult AppointmentDetail(int id)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var appointment = _appointmentService.GetAppointmentDetail(id);
            if (appointment == null)
            {
                return Json(new { success = false, message = "Không tìm thấy lịch hẹn" });
            }

            // Check if appointment belongs to this staff member
            var appointmentData = appointment as dynamic;
            if (appointmentData?.employeeId != userId.Value)
            {
                return Json(new { success = false, message = "Không có quyền truy cập lịch hẹn này" });
            }

            return Json(new { success = true, data = appointment });
        }

        // Lấy thống kê cá nhân
        [HttpGet]
        public async Task<IActionResult> GetPersonalStats()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var stats = await _userService.GetStaffStatsAsync(userId.Value);
            return Json(new { success = true, data = stats });
        }

        // Cập nhật profile
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User model, IFormFile profileImage)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                // Lấy thông tin user hiện tại
                var currentUser = await _userService.GetUserByIdAsync(userId.Value);
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // Cập nhật chỉ những trường được phép thay đổi từ form
                currentUser.FullName = model.FullName ?? currentUser.FullName;
                // Email không được phép thay đổi
                currentUser.Phone = model.Phone ?? currentUser.Phone;
                currentUser.Address = model.Address ?? currentUser.Address;

                // Xử lý upload ảnh nếu có
                if (profileImage != null && profileImage.Length > 0)
                {
                    try
                    {
                        string imageUrl = await _userService.UploadAvatarAsync(profileImage);
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            currentUser.ProfilePictureUrl = imageUrl;
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = $"Lỗi upload ảnh: {ex.Message}" });
                    }
                }

                var result = await _userService.EditUserAsync(currentUser);
                if (result.Success)
                {
                    return Json(new { success = true, message = "Cập nhật thông tin thành công" });
                }
                return Json(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Đánh dấu đã đọc notifications
        [HttpPost]
        public async Task<IActionResult> MarkNotificationsAsRead()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            await _notificationService.MarkAllAsReadAsync(userId.Value);
            return Json(new { success = true, message = "Đã đánh dấu tất cả thông báo là đã đọc" });
        }

        // Trang profile
        public IActionResult Profile()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            int? roleId = HttpContext.Session.GetInt32("CurrentUserRoleId");

            if (userId == null || roleId != 3)
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }

        // Lịch làm việc của staff
        [HttpGet]
        public async Task<IActionResult> MySchedule()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // Lấy lịch hẹn của staff
            var appointments = await _scheduleService.GetAppointmentsAsync(staffId: userId.Value);

            return View(appointments);
        }

        // Lấy thông tin profile
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            return Json(new {
                success = true,
                data = new {
                    fullName = user.FullName,
                    username = user.Username,
                    email = user.Email,
                    phone = user.Phone,
                    address = user.Address,
                    profilePictureUrl = user.ProfilePictureUrl
                }
            });
        }

        // Cập nhật trạng thái appointment
        [HttpPost]
        public async Task<IActionResult> UpdateAppointmentStatus(int appointmentId, int statusId, string? reason = null)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                // Kiểm tra appointment có thuộc về staff này không
                var appointment = await _scheduleService.GetAppointmentByIdAsync(appointmentId);
                if (appointment == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy lịch hẹn" });
                }

                if (appointment.EmployeeId != userId.Value)
                {
                    return Json(new { success = false, message = "Không có quyền cập nhật lịch hẹn này" });
                }

                // Cập nhật trạng thái
                var result = await _scheduleService.UpdateAppointmentStatusAsync(appointmentId, statusId, reason);
                if (result)
                {
                    return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể cập nhật trạng thái" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating appointment status: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái" });
            }
        }

        // Cập nhật trạng thái appointment với tên trạng thái
        [HttpPost]
        public async Task<IActionResult> UpdateAppointmentStatusByName(int appointmentId, string status, string? note = null)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                // Kiểm tra appointment có thuộc về staff này không
                var appointment = await _scheduleService.GetAppointmentByIdAsync(appointmentId);
                if (appointment == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy lịch hẹn" });
                }

                if (appointment.EmployeeId != userId.Value)
                {
                    return Json(new { success = false, message = "Không có quyền cập nhật lịch hẹn này" });
                }

                // Chuyển đổi tên trạng thái thành ID
                int statusId = status switch
                {
                    "Pending" => 1,        // Đang chờ xử lý
                    "Confirmed" => 2,      // Đã xác nhận
                    "InProgress" => 3,     // Đang thực hiện
                    "Completed" => 4,      // Hoàn thành
                    "Cancelled" => 5,      // Đã hủy
                    "PendingCancel" => 6,  // Chờ hủy
                    _ => 0
                };

                if (statusId == 0)
                {
                    return Json(new { success = false, message = "Trạng thái không hợp lệ" });
                }

                // Cập nhật trạng thái
                var result = await _scheduleService.UpdateAppointmentStatusAsync(appointmentId, statusId, note);
                if (result)
                {
                    return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể cập nhật trạng thái" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating appointment status: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái" });
            }
        }
    }

    // Extension method for DateTime
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
