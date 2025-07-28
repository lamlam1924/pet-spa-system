using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Controllers
{
[Route("AdminAppointment")]
public class AdminAppointmentController : Controller

    {
        private readonly IAppointmentService _appointmentService;
        private readonly IUserService _userService;

        [HttpGet]
        [Route("GetAppointmentDetail")]
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

        public IActionResult List(string customer = "", string pet = "", string service = "", int statusId = 0,
            DateTime? date = null, int employeeId = 0, int page = 1)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                Console.WriteLine("[AdminController] User not authenticated, redirecting or allowing anonymous access.");
                return RedirectToAction("AccessDenied", "Account");
            }

            const int pageSize = 10;

            var filter = new pet_spa_system1.ViewModel.AppointmentFilter
            {
                Customer = customer,
                Pet = pet,
                Service = service,
                StatusId = statusId,
                Date = date,
                EmployeeId = employeeId,
                Page = page,
                PageSize = pageSize
            };
            var appointments = _appointmentService.GetAppointments(filter);
            var totalItems = _appointmentService.CountAppointments(filter);

            var allEmployees = _appointmentService.GetEmployees();
            var employees = allEmployees?.Where(u => u.RoleId == 3).ToList() ?? new List<User>();
            var viewModel = new AppointmentListViewModel
            {
                Appointments = appointments,
                StatusList = _appointmentService.GetAllStatuses(),
                EmployeeList = employees, // truyền cho UI duyệt/gán
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
            var filter = new pet_spa_system1.ViewModel.AppointmentFilter();
            var appointments = _appointmentService.GetAppointments(filter);
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
            // Lọc lại EmployeeList nếu cần
            if (viewModel.EmployeeList != null)
                viewModel.EmployeeList = viewModel.EmployeeList.Where(u => u.RoleId == 3).ToList();
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
            // Lọc lại EmployeeList nếu cần
            if (vm.EmployeeList != null)
                vm.EmployeeList = vm.EmployeeList.Where(u => u.RoleId == 3).ToList();
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
        [Route("QuickUpdateStatus")]
        [ValidateAntiForgeryToken]
        public JsonResult QuickUpdateStatus(int id, int statusId)
        {
            var success = _appointmentService.UpdateAppointmentStatus(id, statusId);

            if (success)
                return Json(new { success = true, message = "Đã cập nhật trạng thái thành công" });
            else
                return Json(new { success = false, message = "Không tìm thấy lịch hẹn" });
        }


        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public IActionResult ApproveCancel(int id)
        // {
        //     // Chuyển trạng thái sang Đã hủy (5) và gửi mail
        //     _appointmentService.UpdateAppointmentStatusAndSendMail(id, 5);
        //     TempData["SuccessMessage"] = "Đã duyệt hủy lịch hẹn.";
        //     return RedirectToAction("ApprovalList");
        // }

        [HttpGet]
        [Route("ApproveAppointments")]
        public IActionResult ApproveAppointments(string customer = "", string pet = "", string service = "", string status = "")
        {
            // Trả về View kèm ViewModel, truyền tham số lọc cho service
            var model = _appointmentService.GetPendingAppointmentsViewModel(customer, pet, service, status);
            return View("~/Views/Admin/ManageAppointment/ApproveAppointments.cshtml", model);
        }

        [HttpPost("ApproveAndAssignStaff")]
        public IActionResult ApproveAndAssignStaff([FromBody] ApproveAssignRequest request)
        {
            if (request == null || request.AppointmentId <= 0 || request.StaffId <= 0)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });

            string mailWarning = null;
            ApproveAssignResult result = null;
            try
            {
                result = _appointmentService.AdminApproveAndAssignStaff(request);
            }
            catch (Exception ex)
            {
                // Nếu lỗi chỉ do gửi mail, vẫn trả về thành công, kèm cảnh báo
                mailWarning = "Lưu thành công nhưng gửi email thất bại: " + ex.Message;
                Console.WriteLine($"[ApproveAndAssignStaff] Gửi mail lỗi: {ex.Message}\n{ex.StackTrace}");
                // Nếu lỗi khác, vẫn trả về kết quả lưu DB
            }

            if (result != null && result.Success)
            {
                return Ok(new {
                    success = true,
                    message = result.Message,
                    updated = result.Updated,
                    mailWarning
                });
            }
            else
            {
                return Ok(new {
                    success = false,
                    message = result?.Message ?? "Có lỗi xảy ra khi lưu lịch hẹn!",
                    mailWarning
                });
            }
        }

        [HttpPost("AutoAssignStaff")]
        public IActionResult AutoAssignStaff([FromBody] AutoAssignRequest request)
        {
            if (request == null || request.AppointmentId <= 0)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });

            var appointment = _appointmentService.GetAppointmentById(request.AppointmentId);
            if (appointment == null)
                return Ok(new { success = false, message = "Không tìm thấy lịch hẹn" });

            var assignedStaffId = _appointmentService.AutoAssignStaffForAppointment(appointment);

            if (assignedStaffId == null)
                return Ok(new { success = false, message = "Không tìm được nhân viên phù hợp" });


            // Cập nhật lại EmployeeId cho appointment và lưu vào DB
            appointment.EmployeeId = assignedStaffId;
            _appointmentService.UpdateAppointment(new pet_spa_system1.ViewModel.AppointmentViewModel {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate,
                StatusId = appointment.StatusId,
                EmployeeIds = new List<int> { assignedStaffId.Value },
                CustomerId = appointment.UserId,
                Notes = appointment.Notes,
                SelectedPetIds = appointment.AppointmentPets?.Select(p => p.PetId).ToList() ?? new List<int>(),
                SelectedServiceIds = appointment.AppointmentServices?.Select(s => s.ServiceId).ToList() ?? new List<int>()
            });

            // Reload lại appointment từ DB để đảm bảo thông tin nhân viên đã cập nhật
            var updatedAppointment = _appointmentService.GetAppointmentById(request.AppointmentId);



            return Ok(new { success = true });
        }

        [HttpPost("CheckConflict")]
        public IActionResult CheckConflict([FromBody] CheckConflictRequest request)
        {
            if (request == null || request.StaffId <= 0 || request.DurationMinutes <= 0)
                return BadRequest(new { conflict = false, message = "Dữ liệu không hợp lệ" });

            bool conflict =
                _appointmentService.IsTimeConflict(request.AppointmentDate, request.StaffId, request.DurationMinutes);

            return Ok(new { conflict });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectAppointment(int id)
        {
            // Chuyển trạng thái sang Đã hủy
            _appointmentService.UpdateAppointmentStatus(id, (int)pet_spa_system1.Services.AppointmentStatus.Cancelled);
            // Gửi mail thông báo cho khách hàng
            try
            {
                _appointmentService.SendAppointmentNotificationMail(id, "rejected", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Mail] Lỗi gửi mail thông báo từ chối/hủy: {ex.Message}");
            }
            TempData["SuccessMessage"] = "Đã từ chối lịch hẹn.";
            return RedirectToAction("ApprovalList");
        }

        public IActionResult ManagementTimeline(DateTime? date)
        {
            var selectedDate = date ?? DateTime.Today;
            var viewModel = _appointmentService.GetManagementTimelineData(selectedDate);
            return View("~/Views/Admin/ManageAppointment/ManagementTimeline.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStaffAndTime([FromBody] ShiftUpdateRequest request)
        {
            if (request == null || request.AppointmentId <= 0)
                return BadRequest(new { success = false });

            if (request.NewHour < 8 || request.NewHour > 20)
                return BadRequest(new { success = false, message = "Giờ làm việc không hợp lệ." });

            var appointment = _appointmentService.GetAppointmentById(request.AppointmentId);
            if (appointment == null)
                return NotFound(new { success = false });

            // Cập nhật nhân viên
            appointment.EmployeeId = request.NewStaffId;

            // Cập nhật giờ bắt đầu
            var date = appointment.AppointmentDate.Date;
            appointment.AppointmentDate = new DateTime(date.Year, date.Month, date.Day, request.NewHour, 0, 0);

            // Cập nhật giờ kết thúc
            // Tính toán giờ kết thúc thông qua service
            int duration = _appointmentService.CalculateDurationMinutes(appointment.AppointmentId);
            var endTime = appointment.AppointmentDate.AddMinutes(duration);
            // Nếu cần lưu endTime vào ViewModel hoặc trả về cho client thì xử lý tại đây

            // Chuyển đổi sang ViewModel nếu cần
            var appointmentVm = new AppointmentViewModel
            {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate,
                CustomerId = appointment.UserId,
                EmployeeIds = appointment.EmployeeId.HasValue ? new List<int> { appointment.EmployeeId.Value } : new List<int>(),
                StatusId = appointment.StatusId,
                Notes = appointment.Notes,
                SelectedPetIds = appointment.AppointmentPets?.Select(p => p.PetId).ToList() ?? new List<int>(),
                SelectedServiceIds = appointment.AppointmentServices?.Select(s => s.ServiceId).ToList() ?? new List<int>()
            };
            _appointmentService.UpdateAppointment(appointmentVm);

            return Ok(new { success = true });
        }


    }
}