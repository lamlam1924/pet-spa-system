using pet_spa_system1.Models;
using pet_spa_system1.ViewModel;
using pet_spa_system1.Repositories;
using pet_spa_system1.ViewModel;
using System;
using System.Collections.Generic;
using pet_spa_system1.ViewModels;

namespace pet_spa_system1.Services
{
    public interface IAppointmentService
    {
        /// <summary>
        /// Gửi mail nhắc lịch cho các lịch hẹn sắp tới (trước 1 ngày)
        /// </summary>
        void SendUpcomingAppointmentReminders();
        /// <summary>
        /// Gửi mail thông báo cho khách hàng khi lịch hẹn được duyệt/gán hoặc bị từ chối/hủy
        /// </summary>
        /// <param name="appointmentId">Id lịch hẹn</param>
        /// <param name="type">Kiểu thông báo: approved, rejected</param>
        /// <param name="staffId">Id nhân viên được gán (nếu có)</param>
        void SendAppointmentNotificationMail(int appointmentId, string type, int? staffId);
        int CalculateDurationMinutes(int appointmentId);
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
        ApproveAppointmentsViewModel GetPendingAppointmentsViewModel(string customer = "", string pet = "", string service = "", string status = "");
        // Gửi mail khi duyệt lịch hoặc duyệt hủy
        // bool UpdateAppointmentStatusAndSendMail(int id, int statusId);

        public List<AppointmentViewModel> GetAppointmentsByStaffAndDate(int staffId, DateTime date);
        RealtimeShiftViewModel GetManagementTimelineData(DateTime date);
        bool TryUpdateAppointmentStaff(int appointmentId, int newStaffId);

        bool ApproveAndAssignStaff(int appointmentId, int staffId);
        ApproveAssignResult AdminApproveAndAssignStaff(ApproveAssignRequest request);

        int? AutoAssignStaffForAppointment(Appointment appointment);

        bool IsTimeConflict(DateTime appointmentDate, int staffId, int durationMinutes);


    }
}