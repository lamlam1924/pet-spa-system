using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pet_spa_system1.Controllers
{
    public class AdminAppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;

        public AdminAppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
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
                
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.StatusList = _appointmentService.GetAllStatuses();
            ViewBag.EmployeeList = _appointmentService.GetEmployees();
            
            return View("~/Views/Admin/ManageAppointment/AppointmentList.cshtml", appointments);
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
            var appointment = _appointmentService.GetAppointmentDetails(id);
                
            if (appointment == null)
                return NotFound();
            
            ViewBag.Statuses = _appointmentService.GetAllStatuses();
                
            return View("~/Views/Admin/ManageAppointment/AppointmentDetail.cshtml", appointment);
        }
        
        public IActionResult Create()
        {
            var viewModel = _appointmentService.PrepareCreateViewModel();
            return View("~/Views/Admin/ManageAppointment/AddAppointment.cshtml", viewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AdminAppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var updatedModel = _appointmentService.PrepareCreateViewModel();
                updatedModel.AppointmentDate = model.AppointmentDate;
                updatedModel.CustomerId = model.CustomerId;
                updatedModel.EmployeeId = model.EmployeeId;
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
            var viewModel = _appointmentService.PrepareEditViewModel(id);
                
            if (viewModel == null)
                return NotFound();
            
            return View("~/Views/Admin/ManageAppointment/EditAppointment.cshtml", viewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AdminAppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var updatedModel = _appointmentService.PrepareEditViewModel(model.AppointmentId);
                
                if (updatedModel == null)
                    return NotFound();
                
                updatedModel.AppointmentDate = model.AppointmentDate;
                updatedModel.StatusId = model.StatusId;
                updatedModel.Notes = model.Notes;
                updatedModel.SelectedPetIds = model.SelectedPetIds;
                updatedModel.SelectedServiceIds = model.SelectedServiceIds;
                
                return View("~/Views/Admin/ManageAppointment/EditAppointment.cshtml", updatedModel);
            }
            
            var success = _appointmentService.UpdateAppointment(model);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã cập nhật lịch hẹn thành công!";
                return RedirectToAction("Detail", new { id = model.AppointmentId });
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật lịch hẹn!";
                return View("~/Views/Admin/ManageAppointment/EditAppointment.cshtml", model);
            }
        }
        
        [HttpPost]
        public JsonResult UpdateStatus(int id, int statusId)
        {
            var success = _appointmentService.UpdateAppointmentStatus(id, statusId);
            return Json(new { success });
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
            
            return Json(new { 
                monthlyStats = stats.MonthlyStats, 
                recentAppointments = stats.RecentAppointments 
            });
        }
        
        [HttpPost]
        public JsonResult QuickUpdateStatus(int id, int statusId)
        {
            var success = _appointmentService.UpdateAppointmentStatus(id, statusId);
            
            if (success)
                return Json(new { success = true, message = "Đã cập nhật trạng thái thành công" });
            else
                return Json(new { success = false, message = "Không tìm thấy lịch hẹn" });
        }
    }
} 