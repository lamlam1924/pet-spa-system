using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Controllers
{
    public class AdminAppointmentController : Controller

    {
        private readonly IAppointmentService _appointmentService;
        private readonly IUserService _userService;

        [HttpGet]
        public IActionResult GetAppointmentDetail(int id)
        {
            var vm = _appointmentService.GetAdminAppointmentDetail(id);
            if (vm == null)
                return Content("<div class='text-danger'>Không tìm thấy lịch hẹn.</div>", "text/html");
            return PartialView("~/Views/Admin/ManageAppointment/AppointmentDetailPartial.cshtml", vm);
        }

        public AdminAppointmentController(IAppointmentService appointmentService, IUserService userService)
        {
            _appointmentService = appointmentService;
            _userService = userService;
        }

        public IActionResult Dashboard()
        {
            var viewModel = _appointmentService.GetDashboardData();
            return View("~/Views/Admin/ManageAppointment/AppointmentDashboard.cshtml", viewModel);
        }

        public IActionResult List(string searchTerm = "", int statusId = 0,
            DateTime? date = null, int employeeId = 0, int page = 1)
        {
            const int pageSize = 10;

            var appointments = _appointmentService.GetAppointments(
                searchTerm, statusId, date, employeeId, page, pageSize);

            var totalItems = _appointmentService.CountAppointments(
                searchTerm, statusId, date, employeeId);

            var allEmployees = _appointmentService.GetEmployees();
            var employees = allEmployees?.Where(u => u.RoleId == 3).ToList() ?? new List<User>();
            var viewModel = new AppointmentListViewModel
            {
                Appointments = appointments,
                StatusList = _appointmentService.GetAllStatuses(),
                Employees = employees,
                // Nếu cần filter khách hàng, dịch vụ, thú cưng thì bổ sung:
                // Customers = _appointmentService.GetCustomers(),
                // Services = _appointmentService.GetAllServices(),
                // Pets = _appointmentService.GetAllPets()
            };

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentPage = page;

            return View("~/Views/Admin/ManageAppointment/AppointmentList.cshtml", viewModel);
        }

        public IActionResult Calendar()
        {
            // Lấy toàn bộ lịch hẹn hoặc theo range phù hợp cho calendar
            var appointments = _appointmentService.GetAppointments();
            return View("~/Views/Admin/ManageAppointment/AppointmentCalendar.cshtml", appointments);
        }

        [HttpGet]
        public JsonResult GetAppointmentsForCalendar(DateTime start, DateTime end)
        {
            var events = _appointmentService.GetAppointmentsForCalendar(start, end);
            return Json(events);
        }

        public IActionResult Detail(int id)
        {
            var vm = _appointmentService.GetAdminAppointmentDetail(id);
            if (vm == null)
                return NotFound();

            return View("~/Views/Admin/ManageAppointment/AppointmentDetail.cshtml", vm);
        }

        public IActionResult Create()
        {
            var viewModel = _appointmentService.PrepareCreateViewModel();
            // Lọc lại Employees nếu cần
            if (viewModel.Employees != null)
                viewModel.Employees = viewModel.Employees.Where(u => u.RoleId == 3).ToList();
            return View("~/Views/Admin/ManageAppointment/AddAppointment.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var updatedModel = _appointmentService.PrepareCreateViewModel();
                updatedModel.AppointmentDate = model.AppointmentDate;
                updatedModel.CustomerId = model.CustomerId;
                updatedModel.EmployeeIds = model.EmployeeIds;
                updatedModel.StatusId = model.StatusId;
                updatedModel.Notes = model.Notes;
                updatedModel.SelectedPetIds = model.SelectedPetIds;
                updatedModel.SelectedServiceIds = model.SelectedServiceIds;
                return View("~/Views/Admin/ManageAppointment/AddAppointment.cshtml", updatedModel);
            }

            var success = _appointmentService.CreateAppointment(model);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã tạo lịch hẹn mới thành công!";
                return RedirectToAction("List");
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo lịch hẹn!";
                return View("~/Views/Admin/ManageAppointment/AddAppointment.cshtml", model);
            }
        }

        public IActionResult Edit(int id)
        {
            var vm = _appointmentService.PrepareEditViewModel(id);
            if (vm == null) return NotFound();
            // Lọc lại Employees nếu cần
            if (vm.Employees != null)
                vm.Employees = vm.Employees.Where(u => u.RoleId == 3).ToList();
            return View("~/Views/Admin/ManageAppointment/EditAppointment.cshtml", vm);
        }

        [HttpPost]
        public IActionResult Edit(AppointmentViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            _appointmentService.UpdateAppointment(vm);
            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var success = _appointmentService.DeleteAppointment(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Đã xóa lịch hẹn thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa lịch hẹn!";
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        public JsonResult GetPetsByCustomer(int userId)
        {
            var pets = _appointmentService.GetCustomerPets(userId);
            return Json(pets.Select(p => new { p.PetId, p.Name, Type = p.Species?.SpeciesName ?? "Unknown" }));
        }

        [HttpGet]
        public JsonResult GetDashboardStats()
        {
            var stats = _appointmentService.GetDashboardStats();

            return Json(new
            {
                monthlyStats = stats.MonthlyStats,
                recentAppointments = stats.RecentAppointments
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult QuickUpdateStatus(int id, int statusId)
        {
            var success = _appointmentService.UpdateAppointmentStatus(id, statusId);

            if (success)
                return Json(new { success = true, message = "Đã cập nhật trạng thái thành công" });
            else
                return Json(new { success = false, message = "Không tìm thấy lịch hẹn" });
        }

        // Trang danh sách lịch hẹn cần duyệt
        public IActionResult ApprovalList()
        {
            // Lấy 2 danh sách: 1 = Chờ xác nhận, 6 = Yêu cầu hủy
            var model = new ApprovalListTabsViewModel
            {
                Pending = _appointmentService.GetPendingAppointments(),
                PendingCancel = _appointmentService.GetPendingCancelAppointments()
            };
            return View("~/Views/Admin/ManageAppointment/ApprovalList.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveCancel(int id)
        {
            // Chuyển trạng thái sang Đã hủy (5) và gửi mail
            _appointmentService.UpdateAppointmentStatusAndSendMail(id, 5);
            TempData["SuccessMessage"] = "Đã duyệt hủy lịch hẹn.";
            return RedirectToAction("ApprovalList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectCancel(int id)
        {
            // Chuyển trạng thái về Đã xác nhận (2) hoặc trạng thái trước đó
            _appointmentService.UpdateAppointmentStatus(id, 2);
            TempData["SuccessMessage"] = "Đã từ chối yêu cầu hủy.";
            return RedirectToAction("ApprovalList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveAppointment(int id)
        {
            // Chuyển trạng thái sang Đã xác nhận (2) và gửi mail
            _appointmentService.UpdateAppointmentStatusAndSendMail(id, 2);
            TempData["SuccessMessage"] = "Đã duyệt lịch hẹn.";
            return RedirectToAction("ApprovalList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectAppointment(int id)
        {
            // Chuyển trạng thái sang Đã hủy (4)
            _appointmentService.UpdateAppointmentStatus(id, 4);
            TempData["SuccessMessage"] = "Đã từ chối lịch hẹn.";
            return RedirectToAction("ApprovalList");
        }

        // Timeline Scheduler View
        public async Task<IActionResult> TimelineScheduler(string keyword = "")
        {
            var today = DateTime.Today;
            var staffList = await _userService.GetStaffListAsync();
            var allAppointments = _appointmentService.GetAppointments("", 0, today, 0, 1, 100);
            var searchResults = string.IsNullOrWhiteSpace(keyword)
                ? new List<Appointment>()
                : _appointmentService.GetAppointments(keyword, 0, today, 0, 1, 100);

            List<TimelineAppointmentViewModel> MapAppointments(List<Appointment> appts)
            {
                return appts.Select(a => {
                    var serviceDurations = a.AppointmentServices?.Select(asr => asr.Service.DurationMinutes ?? 0).ToList() ?? new List<int>();
                    int totalDuration = serviceDurations.Sum();
                    if (serviceDurations.Count > 1)
                        totalDuration += (serviceDurations.Count - 1) * 5; // Cộng 5 phút giữa các dịch vụ
                    return new TimelineAppointmentViewModel
                    {
                        AppointmentId = a.AppointmentId,
                        AppointmentDate = a.AppointmentDate,
                        EmployeeId = a.EmployeeId,
                        EmployeeName = a.Employee?.FullName,
                        CustomerName = a.User?.FullName,
                        PetNames = a.AppointmentPets?.Select(ap => ap.Pet?.Name ?? "").ToList() ?? new List<string>(),
                        ServiceNames = a.AppointmentServices?.Select(asr => asr.Service?.Name ?? "").ToList() ?? new List<string>(),
                        TotalDurationMinutes = totalDuration,
                        StatusId = a.StatusId,
                        EndTime = a.AppointmentDate.AddMinutes(totalDuration)
                    };
                }).ToList();
            }

            var vm = new TimelineSchedulerViewModel
            {
                StaffList = staffList,
                Appointments = MapAppointments(allAppointments),
                SearchResults = MapAppointments(searchResults),
                Keyword = keyword
            };
            return View("~/Views/Admin/ManageAppointment/TimelineScheduler.cshtml", vm);
        }
    }
}