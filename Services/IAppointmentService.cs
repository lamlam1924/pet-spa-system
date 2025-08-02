        
using pet_spa_system1.Models;
using pet_spa_system1.ViewModel;


namespace pet_spa_system1.Services
{
    public interface IAppointmentService
    {
        void SendAppointmentNotificationMail(int appointmentId, string type, object? model = null);
        List<User> GetEmployees();
        List<Pet> GetCustomerPets(int userId);
        object GetCalendarData();
        Appointment GetAppointmentById(int appointmentId);
        bool RequestCancelAppointment(int appointmentId, int userId);
        AdminAppointmentDetailViewModel GetAdminAppointmentDetail(int id);
        List<AdminAppointmentDetailViewModel> GetPendingApprovalAppointments();
        List<AdminAppointmentDetailViewModel> GetPendingAppointments();
        List<AdminAppointmentDetailViewModel> GetPendingCancelAppointments();
        List<AppointmentViewModel> GetAppointmentsByStaffAndDate(int staffId, DateTime date);
        RealtimeShiftViewModel GetManagementTimelineData(DateTime date);
        bool IsTimeConflict(DateTime appointmentDate, int staffId, int durationMinutes);
        ApproveAppointmentsViewModel GetPendingAppointmentsViewModel(string customer = "", string pet = "", string service = "", string status = "");
        AppointmentViewModel PrepareEditViewModel(int id);
        T BuildAppointmentEmailModel<T>(Appointment appointment) where T : new();
        AppointmentViewModel BuildAppointmentViewModel(Appointment appointment);
        List<User> GetCustomers();
        List<Pet> GetAllPets();
        List<Service> GetAllServices();
        List<User> GetAllCustomersAndStaffs();
        Pet GetPetById(int petId);
        AppointmentPet GetAppointmentPet(int appointmentId, int petId);
        void UpdateAppointmentWithPetStaff(AppointmentViewModel vm);
        bool RestoreAppointment(int id);
        bool SaveAppointment(AppointmentViewModel model, int userId);
        AppointmentHistoryViewModel GetAppointmentHistory(int userId);
        
    }
    }
