        
using pet_spa_system1.Models;
using System;
using System.Collections.Generic;

namespace pet_spa_system1.Repositories
{
    public interface IAppointmentRepository

    {
        // Lấy tất cả dịch vụ (bao gồm cả không active)
        List<Service> GetAllServices();
        // Thêm các hàm cần thiết cho service và controller
        List<Service> GetActiveServices();
        List<User> GetCustomers();
        List<User> GetEmployeeUsers();
        List<Pet> GetAllPets();
        int AddAppointment(Appointment appointment);
        void AddAppointmentPet(int appointmentId, int petId);
        void AddAppointmentService(int appointmentId, int serviceId);
        Appointment? GetById(int id);
        void Save();
        List<Appointment> GetByUserIdWithDetail(int userId);
        List<StatusAppointment> GetAllStatuses();
        List<string> GetPetNamesByIds(List<int> petIds);
        List<string> GetServiceNamesByIds(List<int> serviceIds);

        int CountAppointmentsByDate(DateTime date);
        int CountUpcomingAppointments(DateTime fromDate);
        int CountAppointmentsByStatus(int statusId);
        List<Appointment> GetPendingAppointments();
        List<Appointment> GetPendingCancelAppointments();

        List<Appointment> GetPendingApprovalAppointments();

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

        List<Appointment> GetAppointmentsByDateRange(DateTime start, DateTime end);

        Appointment GetAppointmentWithDetails(int id);

        void Add(Appointment appointment);
        void Update(Appointment appointment);
        void Delete(int id);

        void DeleteAppointmentPets(int appointmentId);
        void DeleteAppointmentServices(int appointmentId);

        List<MonthlyAppointmentStats> GetMonthlyStats(int year);
        
    }

    public class MonthlyAppointmentStats
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }

    }
}
