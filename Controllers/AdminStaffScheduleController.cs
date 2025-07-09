using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace pet_spa_system1.Controllers
{
    public class AdminStaffScheduleController : Controller
    {
        private readonly IAdminStaffScheduleService _service;
        private readonly PetDataShopContext _context;
        public AdminStaffScheduleController(IAdminStaffScheduleService service, PetDataShopContext context)
        {
            _service = service;
            _context = context;
        }
        public async Task<IActionResult> Index(int? staffId, DateTime? date, int? statusId)
        {
            var vm = new AdminStaffScheduleViewModel
            {
                Appointments = await _service.GetAppointmentsAsync(staffId, date, statusId),
                StaffList = await _context.Users.Where(u => u.RoleId != null && u.RoleId != 1 && u.IsActive == true).ToListAsync(),
                StatusList = await _context.StatusAppointments.ToListAsync(),
                FilterStaffId = staffId,
                FilterDate = date,
                FilterStatusId = statusId
            };
            return View("~/Views/Admin/AdminStaffSchedule/Index.cshtml", vm);
        }
        public async Task<IActionResult> Details(int id)
        {
            var appt = await _service.GetAppointmentByIdAsync(id);
            if (appt == null) return NotFound();
            return View("~/Views/Admin/AdminStaffSchedule/Details.cshtml", appt);
        }
        public async Task<IActionResult> Create()
        {
            var vm = new AdminStaffScheduleViewModel
            {
                Appointment = new Appointment { AppointmentDate = DateTime.Now },
                StaffList = await _context.Users.Where(u => u.RoleId != null && u.RoleId != 1 && u.IsActive == true).ToListAsync(),
                StatusList = await _context.StatusAppointments.ToListAsync()
            };
            return View("~/Views/Admin/AdminStaffSchedule/Create.cshtml", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminStaffScheduleViewModel vm)
        {
            if (ModelState.IsValid)
            {
                vm.Appointment.IsActive = true;
                vm.Appointment.CreatedAt = DateTime.Now;
                await _service.CreateAppointmentAsync(vm.Appointment);
                return RedirectToAction("Index");
            }
            vm.StaffList = await _context.Users.Where(u => u.RoleId != null && u.RoleId != 1 && u.IsActive == true).ToListAsync();
            vm.StatusList = await _context.StatusAppointments.ToListAsync();
            return View("~/Views/Admin/AdminStaffSchedule/Create.cshtml", vm);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var appt = await _service.GetAppointmentByIdAsync(id);
            if (appt == null) return NotFound();
            var vm = new AdminStaffScheduleViewModel
            {
                Appointment = appt,
                StaffList = await _context.Users.Where(u => u.RoleId != null && u.RoleId != 1 && u.IsActive == true).ToListAsync(),
                StatusList = await _context.StatusAppointments.ToListAsync()
            };
            return View("~/Views/Admin/AdminStaffSchedule/Edit.cshtml", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminStaffScheduleViewModel vm)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateAppointmentAsync(vm.Appointment);
                return RedirectToAction("Index");
            }
            vm.StaffList = await _context.Users.Where(u => u.RoleId != null && u.RoleId != 1 && u.IsActive == true).ToListAsync();
            vm.StatusList = await _context.StatusAppointments.ToListAsync();
            return View("~/Views/Admin/AdminStaffSchedule/Edit.cshtml", vm);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var appt = await _service.GetAppointmentByIdAsync(id);
            if (appt == null) return NotFound();
            return View("~/Views/Admin/AdminStaffSchedule/Delete.cshtml", appt);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.SoftDeleteAppointmentAsync(id);
            return RedirectToAction("Deleted");
        }
        [HttpGet]
        public async Task<IActionResult> Deleted(int? staffId = null, DateTime? date = null, int? statusId = null)
        {
            try
            {
                // Lấy danh sách lịch đã xóa
                var deletedAppointments = await _service.GetDeletedAppointmentsAsync(staffId, date, statusId);
                // Lấy danh sách nhân viên và trạng thái để hiển thị filter
                var staffList = await _context.Users.Where(u => u.RoleId == 3).ToListAsync();
                var statusList = await _context.StatusAppointments.ToListAsync();
                var model = new AdminStaffScheduleViewModel
                {
                    Appointments = deletedAppointments,
                    StaffList = staffList,
                    StatusList = statusList,
                    FilterStaffId = staffId,
                    FilterDate = date,
                    FilterStatusId = statusId
                };
                return View("~/Views/Admin/AdminStaffSchedule/Deleted.cshtml", model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi tải dữ liệu: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var result = await _service.RestoreAppointmentAsync(id);
                if (result)
                {
                    TempData["Success"] = "Khôi phục lịch thành công!";
                }
                else
                {
                    TempData["Error"] = "Không thể khôi phục lịch!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
            }
            return RedirectToAction("Deleted");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePermanent(int id)
        {
            try
            {
                var result = await _service.DeleteAppointmentPermanentAsync(id);
                if (result)
                {
                    TempData["Success"] = "Đã xóa vĩnh viễn lịch làm việc.";
                }
                else
                {
                    TempData["Error"] = "Không thể xóa vĩnh viễn lịch này.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa vĩnh viễn: " + ex.Message;
            }
            return RedirectToAction("Deleted");
        }
    }
} 