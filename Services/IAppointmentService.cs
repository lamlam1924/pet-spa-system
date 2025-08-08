using pet_spa_system1.Models;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Services
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public interface IAppointmentService
    {
        ServiceResult ApproveAndAssignStaff(int appointmentId, int staffId);
        ServiceResult QuickUpdateStatus(int appointmentId, int statusId);
        List<User> GetEmployees();
        List<Pet> GetCustomerPets(int userId);
        bool RequestCancelAppointment(int appointmentId, int userId);
        Appointment GetAppointmentById(int appointmentId);
        AdminAppointmentDetailViewModel GetAdminAppointmentDetail(int id);
        List<AdminAppointmentDetailViewModel> GetPendingApprovalAppointments();
        List<AdminAppointmentDetailViewModel> GetPendingAppointments();
        List<AdminAppointmentDetailViewModel> GetPendingCancelAppointments();
        List<AppointmentViewModel> GetAppointmentsByStaffAndDate(int staffId, DateTime date);
        List<AppointmentViewModel> GetAppointments(AppointmentFilter filter);
        int CountAppointments(AppointmentFilter filter);
        RealtimeShiftViewModel GetManagementTimelineData(DateTime date);

        ApproveAppointmentsViewModel GetPendingAppointmentsViewModel(string customer = "", string pet = "",
            string service = "", string status = "");

        AppointmentViewModel PrepareEditViewModel(int id);
        T BuildAppointmentEmailModel<T>(Appointment appointment) where T : new();
        AppointmentViewModel BuildAppointmentViewModel(Appointment appointment);
        List<User> GetCustomers();
        List<Pet> GetAllPets();
        List<Service> GetAllServices();
        List<User> GetAllCustomersAndStaffs();
        Pet GetPetById(int petId);
        AppointmentPet GetAppointmentPet(int appointmentId, int petId);
        bool UpdateAppointmentWithPetStaff(AppointmentViewModel vm);
        bool RestoreAppointment(int id);
        void SendAppointmentNotificationMail(int appointmentId, string type, object? model = null);
        (bool Success, int AppointmentId, string message) SaveAppointment(AppointmentViewModel model, int userId);
        AppointmentHistoryViewModel GetAppointmentHistory(int userId);
        bool IsTimeConflict(DateTime appointmentDate, int staffId, int durationMinutes);
        bool IsTimeConflict(DateOnly appointmentDate, TimeOnly startTime, int staffId, int durationMinutes);
        AppointmentDashboardViewModel GetDashboardViewModel();
        List<StatusAppointment> GetAllStatuses();
        void DeleteAppointment(int id);
        List<StatusAppointment> GetValidNextStatuses(string currentStatus);
        object GetCalendarData();
        CalendarViewModel GetCalendarViewModel();
        AppointmentHistoryItemViewModel GetAppointmentDetailWithPetImages(int appointmentId, int userId);

        // Kiểm tra số lượng nhân viên rảnh cho lịch hẹn
        (bool IsEnoughStaff, int AvailableStaffCount, int RequiredStaffCount) checkNumStaffForAppointment(
            int appointmentId);

        List<int> listStaffAvailable(DateOnly appointmentDate, TimeOnly startTime, TimeOnly endTime);


        List<int> getBusyStaffIds(DateOnly appointmentDate, TimeOnly startTime, TimeOnly endTime,
            int? excludeAppointmentId = null);

        List<PetConflictInfo> CheckPetAppointment(List<int> petIds, DateTime startDateTime, DateTime endDateTime,
            int? excludeAppointmentId = null);

        List<PetConflictInfo> CheckPetAppointment(List<int> petIds, DateOnly appointmentDate, TimeOnly startTime,
            TimeOnly endTime, int? excludeAppointmentId = null);

        bool AutoAssignStaff(int appointmentId);
        bool ConfirmedAppointment(Appointment appointment);

        List<StaffShift> GetRealtimeShiftViewModel();
        MoveResult AssignStaffToPet(int appointmentId, int petId, int newStaffId);
        List<User> getAllStaffFreeByAppointmentId(int appointmentId);

        // Staff methods
        bool UpdateAppointmentStatus(int appointmentId, int statusId, int staffId);
        Appointment GetAppointmentDetail(int appointmentId);
    }
}
