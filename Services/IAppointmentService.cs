using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using pet_spa_system1.ViewModel;
using System;
using System.Collections.Generic;

namespace pet_spa_system1.Services
{

    public interface IAppointmentService
    {
        bool SaveAppointment(AppointmentViewModel vm, int userId);
        AppointmentHistoryViewModel GetAppointmentHistory(int userId);
        Appointment GetAppointmentById(int appointmentId);
        List<string> GetPetNames(List<int> petIds);
        List<string> GetServiceNames(List<int> serviceIds);

        AppointmentDashboardViewModel GetDashboardData();
        AppointmentDashboardViewModel GetDashboardStats();
        List<AdminAppointmentDetailViewModel> GetPendingAppointments();
        List<AdminAppointmentDetailViewModel> GetPendingCancelAppointments();

        List<Appointment> GetAppointments(
            string searchTerm = "",
            int statusId = 0,
            DateTime? date = null,
            int employeeId = 0,
            int page = 1,
            int pageSize = 10);

        int CountAppointments(
            string searchTerm = "",
            int statusId = 0,
            DateTime? date = null,
            int employeeId = 0);

        List<StatusAppointment> GetAllStatuses();
        List<User> GetEmployees();

        List<object> GetAppointmentsForCalendar(DateTime start, DateTime end);

        Appointment GetAppointmentDetails(int id);

        AppointmentViewModel PrepareCreateViewModel();
        bool CreateAppointment(AppointmentViewModel model);

        AppointmentViewModel PrepareEditViewModel(int id);
        bool UpdateAppointment(AppointmentViewModel model);

        bool UpdateAppointmentStatus(int id, int statusId);
        bool DeleteAppointment(int id);

        List<Pet> GetCustomerPets(int userId);

        /// <summary>
        /// User gửi yêu cầu hủy lịch hẹn (chuyển trạng thái sang PendingCancel)
        /// </summary>
        /// <param name="appointmentId"></param>
        /// <param name="userId"></param>
        /// <returns>true nếu thành công, false nếu thất bại</returns>
        bool RequestCancelAppointment(int appointmentId, int userId);

        AdminAppointmentDetailViewModel GetAdminAppointmentDetail(int id);

        /// <summary>
        /// Lấy danh sách lịch hẹn cần duyệt (status 6 hoặc 7)
        /// </summary>
        List<Appointment> GetPendingApprovalAppointments();
        // Gửi mail khi duyệt lịch hoặc duyệt hủy
        bool UpdateAppointmentStatusAndSendMail(int id, int statusId);
    }
}