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

        public AdminAppointmentController(IAppointmentService appointmentService, IUserService userService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        [Route("GetAppointmentDetail")]
        public IActionResult GetAppointmentDetail(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var vm = _appointmentService.GetAdminAppointmentDetail(id);
            if (vm == null)
                return Content("<div class='text-danger'>Không tìm thấy lịch hẹn.</div>", "text/html");
            return PartialView("~/Views/Admin/ManageAppointment/AppointmentDetailPartial.cshtml", vm);
        }


        [HttpGet]
        [Route("Dashboard")]
        public IActionResult Dashboard()
        {
            var viewModel = _appointmentService.GetDashboardData();
            return View("~/Views/Admin/ManageAppointment/AppointmentDashboard.cshtml", viewModel);
        }

        [HttpGet]
        [Route("List")]
        public IActionResult List(string customer = "", string pet = "", string service = "", int statusId = 0,
            DateTime? date = null, int employeeId = 0, int page = 1)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                Console.WriteLine(
                    "[AdminController] User not authenticated, redirecting or allowing anonymous access.");
                return RedirectToAction("AccessDenied", "Account");
            }

            const int pageSize = 10;

            var filter = new pet_spa_system1.ViewModel.AppointmentFilter
            {
                Customer = customer,
                Pet = pet,
                Service = service,
                StatusIds = (statusId > 0) ? new List<int> { statusId } : new List<int>(),
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

        [HttpGet]
        [Route("Calendar")]
        public IActionResult Calendar()
        {
            // Lấy toàn bộ lịch hẹn hoặc theo range phù hợp cho calendar
            var filter = new pet_spa_system1.ViewModel.AppointmentFilter();
            var appointments = _appointmentService.GetAppointments(filter);
            return View("~/Views/Admin/ManageAppointment/AppointmentCalendar.cshtml", appointments);
        }

        [HttpGet]
        [Route("GetAppointmentsForCalendar")]
        public JsonResult GetAppointmentsForCalendar(DateTime start, DateTime end)
        {
            if (!ModelState.IsValid) return new JsonResult(new { error = "Invalid model state" }) { StatusCode = 400 };
            var events = _appointmentService.GetAppointmentsForCalendar(start, end);
            return Json(events);
        }

        // Hàm tiện ích gom lỗi ModelState
        // Removed unused GetModelErrors method
        [HttpGet]
        [Route("Detail")]
        public IActionResult Detail(int id)
        {
            if (!ModelState.IsValid) return Content("Invalid model state", "text/plain", System.Text.Encoding.UTF8);
            var vm = _appointmentService.GetAdminAppointmentDetail(id);
            if (vm == null)
                return NotFound();
            return View("~/Views/Admin/ManageAppointment/AppointmentDetail.cshtml", vm);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            var viewModel = _appointmentService.PrepareCreateViewModel();
            // Lọc lại EmployeeList nếu cần
            if (viewModel.EmployeeList != null)
                viewModel.EmployeeList = viewModel.EmployeeList.Where(u => u.RoleId == 3).ToList();
            return View("~/Views/Admin/ManageAppointment/AddAppointment.cshtml", viewModel);
        }

        [HttpPost]
        [Route("Create")]
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

            // Đặt statusId = 2 (admin tạo)
            model.StatusId = 2;
            int newAppointmentId = 0;
            var success = _appointmentService.CreateAppointment(model, out newAppointmentId);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã tạo lịch hẹn mới thành công!";
                TempData["NewAppointmentId"] = newAppointmentId;
                // Reset form về mặc định sau khi tạo thành công
                var newModel = _appointmentService.PrepareCreateViewModel();
                return View("~/Views/Admin/ManageAppointment/AddAppointment.cshtml", newModel);
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo lịch hẹn!";
                return View("~/Views/Admin/ManageAppointment/AddAppointment.cshtml", model);
            }
        }

        [HttpGet]
        [Route("Edit")]
        public IActionResult Edit(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var vm = _appointmentService.PrepareEditViewModel(id); // Trả về AppointmentViewModel
            if (vm == null) return NotFound();
            if (vm.EmployeeList != null)
                vm.EmployeeList = vm.EmployeeList.Where(u => u.RoleId == 3).ToList();
            return View("~/Views/Admin/ManageAppointment/EditAppointment.cshtml", vm);
        }

        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AppointmentViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            // Đảm bảo EmployeeIds có đúng nhân viên được chọn từ StaffId (vì UpdateAppointment dùng EmployeeIds)
            if (vm.EmployeeIds == null) vm.EmployeeIds = new List<int>();
            vm.EmployeeIds.Clear();
            if (vm.StaffId > 0) vm.EmployeeIds.Add(vm.StaffId);

            // Lấy trạng thái cũ để kiểm tra thay đổi trạng thái
            int? oldStatusId = null;
            var oldAppointment = _appointmentService.GetAppointmentById(vm.AppointmentId);
            if (oldAppointment != null)
            {
                oldStatusId = oldAppointment.StatusId;
            }

            _appointmentService.UpdateAppointment(vm);

            // Chỉ gửi mail nếu trạng thái thực sự thay đổi
            if (oldStatusId != null && oldStatusId != vm.StatusId)
            {
                try
                {
                    if (vm.StatusId == 2) // Confirmed
                    {
                        _appointmentService.SendAppointmentNotificationMail(vm.AppointmentId, "approved", vm.StaffId > 0 ? vm.StaffId : (int?)null);
                    }
                    else if (vm.StatusId == 5) // Cancelled
                    {
                        _appointmentService.SendAppointmentNotificationMail(vm.AppointmentId, "cancelled", null);
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Có lỗi khi gửi email: " + ex.Message;
                    Console.WriteLine($"[Edit] Lỗi gửi mail: {ex.Message}\n{ex.StackTrace}");
                }
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
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
        [Route("GetPetsByCustomer")]
        public JsonResult GetPetsByCustomer(int userId)
        {
            if (!ModelState.IsValid) return new JsonResult(new { error = "Invalid model state" }) { StatusCode = 400 };
            var pets = _appointmentService.GetCustomerPets(userId);
            return Json(pets.Select(p => new { p.PetId, p.Name, Type = p.Species?.SpeciesName ?? "Unknown" }));
        }
        // Removed stray code block after GetPetsByCustomer

        [HttpPost]
        [Route("QuickUpdateStatus")]
        [ValidateAntiForgeryToken]
        public JsonResult QuickUpdateStatus(int id, int statusId)
        {
            if (!ModelState.IsValid) return new JsonResult(new { error = "Invalid model state" }) { StatusCode = 400 };
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
        public IActionResult ApproveAppointments(string customer = "", string pet = "", string service = "",
            string status = "")
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
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = TryApproveAndAssignStaff(request, out string? mailWarning);
            if (result != null && result.Success)
                return Ok(new { success = true, message = result.Message, updated = result.Updated, mailWarning });
            else
                return Ok(new { success = false, message = result?.Message ?? "Có lỗi xảy ra khi lưu lịch hẹn!", mailWarning });
        }

        private ApproveAssignResult? TryApproveAndAssignStaff(ApproveAssignRequest request, out string? mailWarning)
        {
            mailWarning = null;
            try
            {
                return _appointmentService.AdminApproveAndAssignStaff(request);
            }
            catch (Exception ex)
            {
                mailWarning = "Lưu thành công nhưng gửi email thất bại: " + ex.Message;
                Console.WriteLine($"[ApproveAndAssignStaff] Gửi mail lỗi: {ex.Message}\n{ex.StackTrace}");
                return null;
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

            UpdateAppointmentWithAssignedStaff(appointment, assignedStaffId.Value);
            return Ok(new { success = true });
        }

        private void UpdateAppointmentWithAssignedStaff(Appointment appointment, int staffId)
        {
            appointment.EmployeeId = staffId;
            _appointmentService.UpdateAppointment(new pet_spa_system1.ViewModel.AppointmentViewModel
            {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate,
                StatusId = appointment.StatusId,
                EmployeeIds = new List<int> { staffId },
                CustomerId = appointment.UserId,
                Notes = appointment.Notes,
                SelectedPetIds = appointment.AppointmentPets?.Select(p => p.PetId).ToList() ?? new List<int>(),
                SelectedServiceIds = appointment.AppointmentServices?.Select(s => s.ServiceId).ToList() ?? new List<int>()
            });
        }

        [HttpPost("CheckConflict")]
        public IActionResult CheckConflict([FromBody] CheckConflictRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
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
            if (!ModelState.IsValid) return BadRequest(ModelState);
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

        [HttpGet]
        [Route("ManagementTimeline")]
        public IActionResult ManagementTimeline(DateTime? date)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
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
            // Tính toán giờ kết thúc thông qua service (nếu cần dùng thì trả về client)
            // (removed commented out code)

            // Chuyển đổi sang ViewModel nếu cần
            var appointmentVm = new AppointmentViewModel
            {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate,
                CustomerId = appointment.UserId,
                EmployeeIds = appointment.EmployeeId.HasValue
                    ? new List<int> { appointment.EmployeeId.Value }
                    : new List<int>(),
                StatusId = appointment.StatusId,
                Notes = appointment.Notes,
                SelectedPetIds = appointment.AppointmentPets?.Select(p => p.PetId).ToList() ?? new List<int>(),
                SelectedServiceIds = appointment.AppointmentServices?.Select(s => s.ServiceId).ToList() ??
                                     new List<int>()
            };
            _appointmentService.UpdateAppointment(appointmentVm);

            return Ok(new { success = true });
        }

        [HttpGet]
        [Route("SearchCustomers")]
        public JsonResult SearchCustomers(string term)
        {
            var customers = _appointmentService.GetCustomers();
            var results = customers
                .Where(u => string.IsNullOrEmpty(term) ||
                            (u.FullName != null && u.FullName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                            (u.Phone != null && u.Phone.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                            (u.Email != null && u.Email.Contains(term, StringComparison.OrdinalIgnoreCase)))
                .Select(u => new
                {
                    id = u.UserId,
                    text = $"{u.FullName} - {u.Phone} - {u.Email}"
                })
                .Take(20)
                .ToList();
            return Json(new { results });
        }

        [HttpGet]
        [Route("SearchStaffs")]
        public JsonResult SearchStaffs(string term)
        {
            var staffs = _appointmentService.GetEmployees();
            var results = staffs
                .Where(u => string.IsNullOrEmpty(term) ||
                            (u.FullName != null && u.FullName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                            (u.Phone != null && u.Phone.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                            (u.Email != null && u.Email.Contains(term, StringComparison.OrdinalIgnoreCase)))
                .Select(u => new
                {
                    id = u.UserId,
                    text = $"{u.FullName} - {u.Phone} - {u.Email}"
                })
                .Take(20)
                .ToList();
            return Json(new { results });
        }

        [HttpGet]
        [Route("SearchPets")]
        public JsonResult SearchPets(int? userId, string term)
        {
            var pets = userId.HasValue && userId.Value > 0
                ? _appointmentService.GetCustomerPets(userId.Value)
                : _appointmentService.GetAllPets();
            var results = pets
                .Where(p => string.IsNullOrEmpty(term) ||
                            (p.Name != null && p.Name.Contains(term, StringComparison.OrdinalIgnoreCase)))
                .Select(p => new { id = p.PetId, text = p.Name })
                .Take(20)
                .ToList();
            return Json(new { results });
        }

        [HttpGet]
        [Route("SearchServices")]
        public JsonResult SearchServices(string term)
        {
            var services = _appointmentService.GetAllServices();
            var results = services
                .Where(s => string.IsNullOrEmpty(term) ||
                            (s.Name != null && s.Name.Contains(term, StringComparison.OrdinalIgnoreCase)))
                .Select(s => new { id = s.ServiceId, text = s.Name })
                .Take(20)
                .ToList();
            return Json(new { results });
        }
        
        [HttpGet]
        [Route("GetUserInfo")]
        public JsonResult GetUserInfo(int id)
        {
            if (!ModelState.IsValid) return new JsonResult(new { error = "Invalid model state" }) { StatusCode = 400 };
            var user = _appointmentService.GetAllCustomersAndStaffs()?.FirstOrDefault(u => u.UserId == id);
            if (user == null)
                return Json(null);
            return Json(new {
                fullName = user.FullName,
                phone = user.Phone,
                email = user.Email,
                address = user.Address
            });
        }
    }
}