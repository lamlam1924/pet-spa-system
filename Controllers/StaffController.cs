using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModel;

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

            var today = DateOnly.FromDateTime(now);
            var startOfWeek = DateOnly.FromDateTime(now.StartOfWeek());
            var endOfWeek = startOfWeek.AddDays(7);

            var todayAppointments = appointments.Where(a => a.AppointmentDate == today).ToList();
            var thisWeekAppointments = appointments.Where(a =>
                a.AppointmentDate >= startOfWeek &&
                a.AppointmentDate <= endOfWeek).ToList();
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
                UpcomingAppointments = appointments.Where(a => a.AppointmentDate > today)
                    .OrderBy(a => a.AppointmentDate)
                    .Take(5)
                    .ToList(),
                AverageRating = 0.0 // Default value, should be calculated from reviews
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
        public async Task<IActionResult> GetMyAppointments(DateTime? date, int? statusId, string? timeFilter, DateTime? startDate, DateTime? endDate)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            // Calculate date range based on timeFilter
            DateTime? filterStartDate = startDate;
            DateTime? filterEndDate = endDate;

            if (!string.IsNullOrEmpty(timeFilter))
            {
                var today = DateTime.Today;
                switch (timeFilter.ToLower())
                {
                    case "today":
                        filterStartDate = today;
                        filterEndDate = today;
                        break;
                    case "tomorrow":
                        var tomorrow = today.AddDays(1);
                        filterStartDate = tomorrow;
                        filterEndDate = tomorrow;
                        break;
                    case "week":
                        filterStartDate = today.StartOfWeek();
                        filterEndDate = filterStartDate.Value.AddDays(6);
                        break;
                    case "lastweek":
                        filterStartDate = today.StartOfWeek().AddDays(-7);
                        filterEndDate = filterStartDate.Value.AddDays(6);
                        break;
                    case "month":
                        filterStartDate = new DateTime(today.Year, today.Month, 1);
                        filterEndDate = filterStartDate.Value.AddMonths(1).AddDays(-1);
                        break;
                    case "lastmonth":
                        var lastMonth = today.AddMonths(-1);
                        filterStartDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                        filterEndDate = filterStartDate.Value.AddMonths(1).AddDays(-1);
                        break;
                    case "3months":
                        filterStartDate = today.AddMonths(-3);
                        filterEndDate = today;
                        break;
                    case "6months":
                        filterStartDate = today.AddMonths(-6);
                        filterEndDate = today;
                        break;
                }
            }
            else if (date.HasValue)
            {
                filterStartDate = date.Value.Date;
                filterEndDate = date.Value.Date;
            }

            // Get all appointments for this staff and then filter by date range
            var allAppointments = await _scheduleService.GetAppointmentsAsync(
                staffId: userId.Value,
                statusId: statusId);

            // Filter by date range if specified
            var appointments = allAppointments.AsQueryable();

            if (filterStartDate.HasValue && filterEndDate.HasValue)
            {
                var startDateOnly = DateOnly.FromDateTime(filterStartDate.Value);
                var endDateOnly = DateOnly.FromDateTime(filterEndDate.Value);
                appointments = appointments.Where(a => a.AppointmentDate >= startDateOnly && a.AppointmentDate <= endDateOnly);
            }
            else if (filterStartDate.HasValue)
            {
                var dateOnly = DateOnly.FromDateTime(filterStartDate.Value);
                appointments = appointments.Where(a => a.AppointmentDate == dateOnly);
            }

            var filteredAppointments = appointments.ToList();

            // Transform data to include proper user names
            var transformedAppointments = filteredAppointments.Select(a => new
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
                    appointmentServiceId = aps.AppointmentServiceId,
                    status = aps.Status ?? 1, // Default to 1 (Pending) if null
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
            if (appointment.EmployeeId != userId.Value)
            {
                return Json(new { success = false, message = "Không có quyền truy cập lịch hẹn này" });
            }

            return Json(new { success = true, data = appointment });
        }

        // Cập nhật trạng thái dịch vụ
        [HttpPost]
        public IActionResult UpdateServiceStatus(int appointmentServiceId, int status, string? note)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                using (var context = new PetDataShopContext())
                {
                    var appointmentService = context.AppointmentServices
                        .Include(aps => aps.Appointment)
                        .FirstOrDefault(aps => aps.AppointmentServiceId == appointmentServiceId);

                    if (appointmentService == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy dịch vụ" });
                    }

                    // Check if staff is assigned to this appointment
                    if (appointmentService.Appointment.EmployeeId != userId.Value)
                    {
                        return Json(new { success = false, message = "Không có quyền cập nhật dịch vụ này" });
                    }

                    // Check if appointment is confirmed or in progress (status = 2 or 3)
                    if (appointmentService.Appointment.StatusId != 2 && appointmentService.Appointment.StatusId != 3)
                    {
                        return Json(new { success = false, message = "Chỉ có thể cập nhật dịch vụ khi lịch hẹn đã được xác nhận hoặc đang thực hiện" });
                    }

                    // Check if service can be updated (not completed or cancelled)
                    if (appointmentService.Status == 3 || appointmentService.Status == 4)
                    {
                        return Json(new { success = false, message = "Không thể cập nhật dịch vụ đã hoàn thành hoặc đã hủy" });
                    }

                    // Update service status
                    appointmentService.Status = status;

                    // Auto-update appointment status based on service statuses
                    var appointment = appointmentService.Appointment;
                    var allServices = context.AppointmentServices
                        .Where(aps => aps.AppointmentId == appointment.AppointmentId)
                        .ToList();

                    // Update the current service status in memory for calculation
                    var currentService = allServices.FirstOrDefault(aps => aps.AppointmentServiceId == appointmentServiceId);
                    if (currentService != null)
                    {
                        currentService.Status = status;
                    }

                    // Check if all services are completed (status = 3)
                    bool allCompleted = allServices.All(aps => (aps.Status ?? 1) == 3);
                    // Check if any service is in progress (status = 2)
                    bool anyInProgress = allServices.Any(aps => (aps.Status ?? 1) == 2);

                    if (allCompleted)
                    {
                        // All services completed -> Appointment completed (status = 4)
                        appointment.StatusId = 4; // Completed
                    }
                    else if (anyInProgress)
                    {
                        // At least one service in progress -> Appointment in progress (status = 3)
                        appointment.StatusId = 3; // InProgress
                    }
                    // If appointment is still Confirmed (status = 2) and no services are in progress,
                    // keep it as Confirmed

                    context.SaveChanges();

                    return Json(new { success = true, message = "Cập nhật trạng thái dịch vụ thành công" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating service status: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái" });
            }
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

            return Json(new
            {
                success = true,
                data = new
                {
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
