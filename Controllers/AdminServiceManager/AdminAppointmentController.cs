using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Repositories;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Controllers
{
    [Route("AdminAppointment")]
    public class AdminAppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IAppointmentRepository _appointmentRepository;

        public AdminAppointmentController(IAppointmentService appointmentService,
            IAppointmentRepository appointmentRepository)

        {
            _appointmentService = appointmentService;
            _appointmentRepository = appointmentRepository;
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


        // Hàm tiện ích gom lỗi ModelState
        // Removed unused GetModelErrors method
        [HttpGet]
        [Route("Detail")]
        public IActionResult Detail(int id)
        {
            if (!ModelState.IsValid) return Content("Invalid model state", "text/plain", Encoding.UTF8);
            var vm = _appointmentService.GetAdminAppointmentDetail(id);
            if (vm == null)
                return NotFound();
            return View("~/Views/Admin/ManageAppointment/AppointmentDetail.cshtml", vm);
        }


        [HttpGet]
        [Route("Edit")]
        public IActionResult Edit(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var vm = _appointmentService.PrepareEditViewModel(id); // Trả về AppointmentViewModel
            if (vm == null) return NotFound();
            // Bổ sung: load PetStaffAssignments từ các AppointmentPet
            if (vm.PetStaffAssignments == null || vm.PetStaffAssignments.Count == 0)
            {
                vm.PetStaffAssignments = vm.SelectedPetIds?.Select(pid =>
                {
                    // var pet = _appointmentService.GetPetById(pid);
                    var ap = _appointmentService.GetAppointmentPet(id, pid);
                    return new PetStaffAssignViewModel
                    {
                        PetId = pid,
                        StaffId = ap?.StaffId,
                        StaffName = ap?.Staff?.FullName
                    };
                }).ToList() ?? new List<PetStaffAssignViewModel>();
            }

            return View("~/Views/Admin/ManageAppointment/EditAppointment.cshtml", vm);
        }

        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AppointmentViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Đọc PetStaffAssignmentsJson từ form và chuyển đổi thành danh sách
            string? petStaffAssignmentsJson = Request.Form["PetStaffAssignmentsJson"].ToString();
            Console.WriteLine($"[AdminAppointmentController.Edit] PetStaffAssignmentsJson: {petStaffAssignmentsJson}");

            if (!string.IsNullOrEmpty(petStaffAssignmentsJson))
            {
                try
                {
                    var petStaffAssignments =
                        JsonSerializer.Deserialize<List<PetStaffAssignViewModel>>(
                            petStaffAssignmentsJson);
                    if (petStaffAssignments != null)
                    {
                        vm.PetStaffAssignments = petStaffAssignments;
                        Console.WriteLine(
                            $"[AdminAppointmentController.Edit] Deserialized {vm.PetStaffAssignments.Count} pet-staff assignments");
                    }
                    else
                    {
                        vm.PetStaffAssignments = new List<PetStaffAssignViewModel>();
                        Console.WriteLine("[AdminAppointmentController.Edit] Deserialized result was null");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"[AdminAppointmentController.Edit] Error deserializing PetStaffAssignmentsJson: {ex.Message}");
                    TempData["ErrorMessage"] = "Có lỗi khi xử lý dữ liệu phân công nhân viên: " + ex.Message;
                    vm.PetStaffAssignments = new List<PetStaffAssignViewModel>();
                }
            }
            else
            {
                Console.WriteLine("[AdminAppointmentController.Edit] PetStaffAssignmentsJson is null or empty");
                vm.PetStaffAssignments = new List<PetStaffAssignViewModel>();
            }

            // Lấy trạng thái cũ để kiểm tra thay đổi trạng thái
            int? oldStatusId = null;
            var oldAppointment = _appointmentService.GetAppointmentById(vm.AppointmentId);
            if (oldAppointment != null)
            {
                oldStatusId = oldAppointment.StatusId;
            }

            // Gọi service cập nhật staff từng pet
            bool updateSuccess = _appointmentService.UpdateAppointmentWithPetStaff(vm);

            if (!updateSuccess)
            {
                // Nếu cập nhật không thành công, báo lỗi và hiển thị lại form
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật lịch hẹn!";
                // Cập nhật danh sách cho dropdown
                vm.Statuses = _appointmentService.GetAllStatuses();
                vm.EmployeeList = _appointmentService.GetEmployees();
                return View("~/Views/Admin/ManageAppointment/EditAppointment.cshtml", vm);
            }

            // Chỉ gửi mail nếu cập nhật thành công và trạng thái thực sự thay đổi
            if (oldStatusId != null && oldStatusId != vm.StatusId)
            {
                try
                {
                    if (vm.StatusId == 2) // Confirmed
                    {
                        _appointmentService.SendAppointmentNotificationMail(vm.AppointmentId, "approved", null);
                    }
                    else if (vm.StatusId == 5) // Cancelled
                    {
                        _appointmentService.SendAppointmentNotificationMail(vm.AppointmentId, "cancelled", null);
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Lịch hẹn đã được cập nhật nhưng có lỗi khi gửi email: " + ex.Message;
                    Console.WriteLine($"[Edit] Lỗi gửi mail: {ex.Message}\n{ex.StackTrace}");
                }
            }

            // Đặt các biến TempData để hiện thông báo và modal CHỈ khi thành công
            TempData["SuccessMessage"] = "Cập nhật lịch hẹn thành công!";
            TempData["EditedAppointmentId"] = vm.AppointmentId;

            // Quay lại trang chỉnh sửa thay vì chuyển đến List
            return RedirectToAction("Edit", new { id = vm.AppointmentId });
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


        [HttpPost("CheckConflict")]
        public IActionResult CheckConflict([FromBody] AppointmentViewModel vm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (vm == null || vm.PetStaffAssignments == null || vm.PetStaffAssignments.Count == 0 ||
                vm.DurationMinutes <= 0)
                return BadRequest(new { conflict = false, message = "Dữ liệu không hợp lệ" });

            // Kiểm tra conflict cho từng pet-staff assignment
            var conflicts = vm.PetStaffAssignments.Select(assign => new
            {
                PetId = assign.PetId,
                StaffId = assign.StaffId,
                Conflict = _appointmentService.IsTimeConflict(vm.AppointmentDate.ToDateTime(vm.StartTime),
                    assign.StaffId ?? 0, vm.DurationMinutes)
            }).ToList();
            return Ok(new { conflicts });
        }


        [HttpGet]
        [Route("ManagementTimeline")]
        public IActionResult ManagementTimeline(DateTime? date)
        {
            var selectedDate = date ?? DateTime.Today;
            var viewModel = _appointmentService.GetManagementTimelineData(selectedDate);

            // Cập nhật thêm SelectedDate và giờ làm việc (nếu muốn)
            viewModel.SelectedDate = selectedDate;
            viewModel.Hours = Enumerable.Range(8, 13).ToList(); // hoặc 7, 13 tùy bạn

            return View("~/Views/Admin/ManageAppointment/ManagementTimeline.cshtml", viewModel);
        }

        [HttpGet]
        [Route("GetTimelineData")]
        public IActionResult GetTimelineData(DateTime date)
        {
            var data = _appointmentService.GetManagementTimelineData(date);

            // Dữ liệu resources (staff)
            var resources = data.StaffShifts.Select(s => new
            {
                id = s.StaffId,
                name = s.StaffName
            }).ToList();

            // Dữ liệu events (pet assignments)
            var events = data.StaffShifts.SelectMany(s => s.Appointments.SelectMany(a =>
                a.PetStaffAssignments.Select(p => new
                {
                    id = $"{a.AppointmentId}_{p.PetId}",
                    text = $"Pet {p.PetName} - Chủ: {p.OwnerName}",
                    start = a.AppointmentDate.ToString("yyyy-MM-dd") + "T" + a.StartTime.ToString(@"hh\\:mm\\:ss"),
                    end = a.AppointmentDate.ToString("yyyy-MM-dd") + "T" + a.EndTime.ToString(@"hh\\:mm\\:ss"),
                    resource = p.StaffId,
                    appointmentId = a.AppointmentId,
                    petId = p.PetId,
                    statusId = a.StatusId
                })
            )).ToList();

            return Json(new { resources, events });
        }



        [HttpGet]
        [Route("GetSchedulerEvents")]
        public IActionResult GetSchedulerEvents(DateTime date)
        {
            var data = _appointmentService.GetManagementTimelineData(date);

            var events = data.StaffShifts.SelectMany(s => s.Appointments.SelectMany(a =>
                a.PetStaffAssignments.Select(p => new
                {
                    id = $"{a.AppointmentId}_{p.PetId}",
                    text = $"Pet {p.PetName} - Chủ: {p.OwnerName}",
                    start = a.AppointmentDate.ToString("yyyy-MM-dd") + "T" + a.StartTime.ToString(@"hh\:mm\:ss"),
                    end = a.AppointmentDate.ToString("yyyy-MM-dd") + "T" + a.EndTime.ToString(@"hh\:mm\:ss"),
                    resource = p.StaffId
                })
            )).ToList();

            return Json(events);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStaffAndTime([FromBody] AppointmentViewModel vm)
        {
            if (vm == null || vm.AppointmentId <= 0)
                return BadRequest(new { success = false });

            if (vm.NewHour < 8 || vm.NewHour > 20)
                return BadRequest(new { success = false, message = "Giờ làm việc không hợp lệ." });

            var appointment = _appointmentService.GetAppointmentById(vm.AppointmentId);
            if (appointment == null)
                return NotFound(new { success = false });

            // Gán staff mới cho từng pet nếu có thông tin (ví dụ: vm.PetStaffAssignments)
            var petStaffAssignments = vm.PetStaffAssignments ?? (appointment.AppointmentPets?.Select(ap =>
                new PetStaffAssignViewModel
                {
                    PetId = ap.PetId,
                    StaffId = vm.NewStaffId ?? 0
                }).ToList() ?? new List<PetStaffAssignViewModel>());

            // Cập nhật giờ bắt đầu
            var date = appointment.AppointmentDate;
            appointment.AppointmentDate = DateOnly.FromDateTime(new DateTime(date.Year, date.Month, date.Day));
            appointment.StartTime = new TimeOnly(vm.NewHour ?? 8, 0);

            // Update via service
            var updateSuccess = _appointmentService.UpdateAppointmentWithPetStaff(
                new AppointmentViewModel
                {
                    AppointmentId = appointment.AppointmentId,
                    AppointmentDate = appointment.AppointmentDate,
                    CustomerId = appointment.UserId,
                    StatusId = appointment.StatusId,
                    Notes = appointment.Notes,
                    SelectedPetIds = appointment.AppointmentPets?.Select(p => p.PetId).ToList() ?? new List<int>(),
                    SelectedServiceIds = appointment.AppointmentServices?.Select(s => s.ServiceId).ToList() ??
                                         new List<int>(),
                    PetStaffAssignments = petStaffAssignments
                });

            if (updateSuccess)
                return Ok(new { success = true });
            else
                return BadRequest(new { success = false, message = "Có lỗi khi cập nhật lịch hẹn" });
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
                    text = $"{u.FullName} - {u.Phone} - {u.Email}",
                    fullName = u.FullName,
                    phone = u.Phone,
                    email = u.Email
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
            return Json(new
            {
                fullName = user.FullName,
                phone = user.Phone,
                email = user.Email,
                address = user.Address
            });
        }

        [HttpGet]
        [Route("Calendar")]
        public IActionResult Calendar()
        {
            var viewModel = _appointmentService.GetCalendarViewModel();
            return View("~/Views/Admin/ManageAppointment/AppointmentCalendar.cshtml", viewModel);
        }

        [HttpGet]
        [Route("GetCalendarData")]
        public IActionResult GetCalendarData()
        {
            var calendarData = _appointmentService.GetCalendarData();
            return Json(calendarData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAppointmentCalendar([FromBody] AppointmentViewModel vm)
        {
            if (!ModelState.IsValid || vm == null || vm.AppointmentId <= 0)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            var appointment = _appointmentService.GetAppointmentById(vm.AppointmentId);
            if (appointment == null)
                return Json(new { success = false, message = "Không tìm thấy lịch hẹn" });

            // Check for time conflict for the same staff
            var newStart = vm.NewStart ?? DateTime.Now;
            // Tính lại thời lượng và thời gian kết thúc dựa trên dịch vụ
            var durationMinutes = appointment.AppointmentServices?.Sum(s => (int?)s.Service.DurationMinutes ?? 0) ?? 0;
            if (durationMinutes <= 0) durationMinutes = 60;
            var newEnd = newStart.AddMinutes(durationMinutes);

            // Check conflict using IsTimeConflict trong service - kiểm tra conflict cho từng pet/staff
            var hasConflict = false;
            if (vm.PetStaffAssignments != null)
            {
                foreach (var assignment in vm.PetStaffAssignments)
                {
                    if (assignment.StaffId.HasValue)
                    {
                        // Sử dụng service để kiểm tra xung đột thời gian
                        if (_appointmentService.IsTimeConflict(newStart, assignment.StaffId.Value, durationMinutes) &&
                            assignment.StaffId.Value != 0) // Bỏ qua nếu staffId = 0 (chưa gán)
                        {
                            hasConflict = true;
                            break;
                        }
                    }
                }
            }

            if (hasConflict)
            {
                return Json(new { success = false, message = "Nhân viên đã bận trong khoảng thời gian này!" });
            }

            // Gán staff mới cho từng pet nếu có thông tin
            var petStaffAssignments = vm.PetStaffAssignments ?? new List<PetStaffAssignViewModel>();

            bool updateSuccess = _appointmentService.UpdateAppointmentWithPetStaff(
                new AppointmentViewModel
                {
                    AppointmentId = appointment.AppointmentId,
                    AppointmentDate = DateOnly.FromDateTime(newStart),
                    StartTime = TimeOnly.FromDateTime(newStart),
                    EndTime = TimeOnly.FromDateTime(newEnd),
                    StatusId = appointment.StatusId,
                    Notes = appointment.Notes,
                    CustomerId = appointment.UserId,
                    SelectedPetIds = appointment.AppointmentPets?.Select(p => p.PetId).ToList() ?? new List<int>(),
                    SelectedServiceIds = appointment.AppointmentServices?.Select(s => s.ServiceId).ToList() ??
                                         new List<int>(),
                    PetStaffAssignments = petStaffAssignments
                });

            return Json(new
            {
                success = updateSuccess,
                message = updateSuccess ? "Cập nhật thành công" : "Có lỗi khi cập nhật lịch hẹn"
            });
        }

        [HttpPost("RestoreAppointment")]
        [IgnoreAntiforgeryToken]
        public IActionResult RestoreAppointment([FromForm] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var success = _appointmentService.RestoreAppointment(id);
            if (success)
            {
                return Json(new { success = true, message = "Khôi phục lịch hẹn thành công!" });
            }
            else
            {
                return BadRequest(new { success = false, message = "Có lỗi xảy ra khi khôi phục lịch hẹn!" });
            }
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            var model = new AppointmentViewModel
            {
                AppointmentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                StartTime = new TimeOnly(9, 0), // 9:00 AM
                Statuses = _appointmentService.GetAllStatuses(),
                EmployeeList = _appointmentService.GetEmployees()
            };
            return View("~/Views/Admin/ManageAppointment/AddAppointment.cshtml", model);
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Statuses = _appointmentService.GetAllStatuses();
                model.EmployeeList = _appointmentService.GetEmployees();
                return View("~/Views/Admin/ManageAppointment/AddAppointment.cshtml", model);
            }

            try
            {
                // Parse PetStaffAssignments từ JSON
                if (!string.IsNullOrEmpty(Request.Form["PetStaffAssignmentsJson"]))
                {
                    var petStaffJson = Request.Form["PetStaffAssignmentsJson"].ToString();
                    model.PetStaffAssignments =
                        JsonSerializer.Deserialize<List<PetStaffAssignViewModel>>(petStaffJson) ??
                        new List<PetStaffAssignViewModel>();
                }

                // Tính toán EndTime
                if (model.SelectedServices != null && model.SelectedServices.Count > 0)
                {
                    model.CalculateEndTime(model.SelectedServices);
                }

                Console.WriteLine(
                    $"[AdminAppointmentController] Gọi SaveAppointment với CustomerId: {model.CustomerId}");
                Console.WriteLine(
                    $"[AdminAppointmentController] PetIds: {string.Join(", ", model.SelectedPetIds ?? new List<int>())}");
                Console.WriteLine(
                    $"[AdminAppointmentController] ServiceIds: {string.Join(", ", model.SelectedServiceIds ?? new List<int>())}");

                var result = _appointmentService.SaveAppointment(model, model.CustomerId);
                if (result.Success)
                {
                    Console.WriteLine("[AdminAppointmentController] SaveAppointment trả về thành công");
                    Console.WriteLine($"[AdminAppointmentController] AppointmentId: {result.AppointmentId}");
                    TempData["SuccessMessage"] = "Tạo lịch hẹn thành công!";
                    TempData["NewAppointmentId"] = result.AppointmentId;
                    return RedirectToAction("Create");
                }
                else
                {
                    Console.WriteLine("[AdminAppointmentController] SaveAppointment trả về false - thất bại");
                    ModelState.AddModelError("", "Có lịch trùng của Pet!");
                    model.Statuses = _appointmentService.GetAllStatuses();
                    model.EmployeeList = _appointmentService.GetEmployees();
                    return View("~/Views/Admin/ManageAppointment/AddAppointment.cshtml", model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                model.Statuses = _appointmentService.GetAllStatuses();
                model.EmployeeList = _appointmentService.GetEmployees();
                return View("~/Views/Admin/ManageAppointment/AddAppointment.cshtml", model);
            }
        }

        [HttpGet]
        [Route("Dashboard")]
        public IActionResult Dashboard()
        {
            var model = _appointmentService.GetDashboardViewModel();
            return View("~/Views/Admin/ManageAppointment/AppointmentDashboard.cshtml", model);
        }

        [HttpGet]
        [Route("List")]
        public IActionResult List(int page = 1, string search = "", int? status = null, DateTime? date = null,
            int? employee = null)
        {
            // Sử dụng các hàm service/repo đã có để lấy danh sách lịch hẹn, lọc, phân trang
            int pageSize = 20;
            var filter = new AppointmentFilter
            {
                Customer = search,
                Pet = search,
                Service = search,
                StaffId = employee,
                Date = date,
                StatusId = status,
                Page = page,
                PageSize = pageSize
            };
            var appointments = _appointmentService.GetAppointments(filter);
            var total = _appointmentService.CountAppointments(filter);
            var statusList = _appointmentService.GetAllStatuses();
            var employeeList = _appointmentService.GetEmployees();

            var model = new AppointmentListViewModel
            {
                Appointments = appointments,
                StatusList = statusList,
                EmployeeList = employeeList,
                Total = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)total / pageSize)
            };
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = model.TotalPages;
            return View("~/Views/Admin/ManageAppointment/AppointmentList.cshtml", model);
        }

        [HttpPost]
        [Route("ApproveAndAssignStaff")]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveAndAssignStaff([FromBody] ApproveRequestModel model)
        {
            try
            {
                var result = _appointmentService.ApproveAndAssignStaff(model.AppointmentId, model.StaffId);
                if (result.Success)
                    return Json(new { success = true, message = result.Message });
                else
                    return Json(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("AutoAssignStaff")]
        [ValidateAntiForgeryToken]
        public IActionResult AutoAssignStaff([FromBody] ApproveRequestModel model)
        {
            try
            {
                var (isEnoughStaff, availableStaffCount, requiredStaffCount) =
                    _appointmentService.checkNumStaffForAppointment(model.AppointmentId);

                if (isEnoughStaff)
                {
                    if (_appointmentService.AutoAssignStaff(model.AppointmentId))
                    {
                        return Json(new
                        {
                            success = true,
                            message = "Gán nhân viên và đã chấp nhận lịch thành công"
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Không thể tự động gán nhân viên cho lịch hẹn này."
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Thiếu nhân viên! Rảnh: {availableStaffCount}, Cần: {requiredStaffCount}",
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("QuickUpdateStatus")]
        [ValidateAntiForgeryToken]
        public IActionResult QuickUpdateStatus(int id, int statusId)
        {
            try
            {
                var result = _appointmentService.QuickUpdateStatus(id, statusId);
                if (result.Success)
                    return Json(new { success = true, message = result.Message });
                else
                    return Json(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("DeleteAppointment")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAppointment(int id)
        {
            if (!ModelState.IsValid) return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            try
            {
                // Xóa mềm lịch hẹn và các liên kết
                _appointmentService.DeleteAppointment(id);
                return Json(new { success = true, message = "Đã ẩn lịch hẹn thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi ẩn lịch hẹn!" });
            }
        }

        public IActionResult RealtimeShift()
        {
            var staffShifts = _appointmentService.GetRealtimeShiftViewModel();

            var vm = new RealtimeShiftViewModel
            {
                StaffShifts = staffShifts,
                Hours = Enumerable.Range(7, 14).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [Route("MoveAppointment")]
        public IActionResult MoveAppointment([FromBody] MoveAppointmentRequest request)
        {
            try
            {
                var appointment = _appointmentRepository.GetAppointmentWithPetAssignments(request.AppointmentId);
                if (appointment == null)
                    return BadRequest(new { success = false, message = "Không tìm thấy lịch hẹn." });

                var appointmentPet = appointment.AppointmentPets.FirstOrDefault(p => p.PetId == request.PetId);
                if (appointmentPet == null)
                    return BadRequest(new { success = false, message = "Không tìm thấy thú cưng trong lịch hẹn." });

                // Kiểm tra lại nhân viên có trùng không (phòng trường hợp race condition)
                bool isAvailable = _appointmentRepository.IsStaffAvailableForPet(
                    request.NewStaffId,
                    appointment.AppointmentDate,
                    appointment.StartTime,
                    appointment.EndTime,
                    appointmentPet.AppointmentPetId
                );

                if (!isAvailable)
                    return Ok(new { success = false, message = "Nhân viên bị trùng lịch." });

                // Cập nhật nhân viên mới cho thú cưng
                appointmentPet.StaffId = request.NewStaffId;

                _appointmentRepository.SaveChanges();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public IActionResult GetQuickInfo(int appointmentId, int petId)
        {
            var appointment = _appointmentRepository
                .GetAppointmentWithPetAssignments(appointmentId);

            var ap = appointment?.AppointmentPets
                .FirstOrDefault(p => p.PetId == petId);

            var pet = ap?.Pet;
            var customer = pet.User;

            if (appointment == null || pet == null || customer == null)
                return Content("<div style='padding:15px;'>Không tìm thấy dữ liệu.</div>", "text/html");

            string html = $@"
        <div style='padding:20px; font-size:14px;'>
            <h3 style='margin-top:0;'>Thông tin lịch hẹn</h3>
            <p><strong>Mã lịch hẹn:</strong> #{appointment.AppointmentId}</p>
            <p><strong>Ngày:</strong> {appointment.AppointmentDate:dd/MM/yyyy}</p>
            <p><strong>Giờ:</strong> {appointment.StartTime:hh\\:mm} - {appointment.EndTime:hh\\:mm}</p>
            <p><strong>Pet:</strong> {pet.Name} ({pet.Species})</p>
            <p><strong>Chủ:</strong> {customer.FullName} - {customer.Phone}</p>
            <p><strong>Ghi chú:</strong> {appointment.Notes ?? "(không có)"}</p>
        </div>
    ";

            return Content(html, "text/html");
        }

        [HttpPost]
        [Route("CheckStaffAvailable")]
        public IActionResult CheckStaffAvailable([FromBody] MoveAppointmentRequest request)
        {
            try
            {
                var appointment = _appointmentRepository.GetAppointmentWithPetAssignments(request.AppointmentId);
                if (appointment == null)
                    return BadRequest(new { success = false, message = "Không tìm thấy lịch hẹn." });

                var appointmentPet = appointment.AppointmentPets.FirstOrDefault(p => p.PetId == request.PetId);
                if (appointmentPet == null)
                    return BadRequest(new { success = false, message = "Không tìm thấy thú cưng trong lịch hẹn." });

                bool isAvailable = _appointmentRepository.IsStaffAvailableForPet(
                    request.NewStaffId,
                    appointment.AppointmentDate,
                    appointment.StartTime,
                    appointment.EndTime,
                    appointmentPet.AppointmentPetId // exclude chính thú cưng này để tránh false positive
                );

                if (!isAvailable)
                    return Ok(new { success = false, message = "Nhân viên bị trùng lịch." });

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}