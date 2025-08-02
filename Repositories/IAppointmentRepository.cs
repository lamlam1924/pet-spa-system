       

using pet_spa_system1.Models;
using System;
using System.Collections.Generic;

namespace pet_spa_system1.Repositories
{
    public interface IAppointmentRepository

    {
        List<object> GetCalendarEvents();
        List<Service> GetAllServices();
        List<Service> GetActiveServices();
        List<User> GetCustomers();
        List<User> GetEmployeeUsers();
        List<Pet> GetAllPets();
        int AddAppointment(Appointment appointment);
        void AddAppointmentPet(int appointmentId, int petId, int? staffId = null);
        void AddAppointmentService(int appointmentId, int serviceId);
        Appointment? GetById(int id);
        List<Appointment> GetAppointmentsByStaffAndDate(int staffId, DateTime date);
        List<int> GetAppointmentIdsByStaff(int staffId);
        List<Appointment> GetAppointmentsByIds(List<int> appointmentIds);
        AppointmentPet GetAppointmentPet(int appointmentId, int petId);
        void UpdateAppointmentPetStaff(int appointmentId, int petId, int staffId);
        void SaveChanges();
        List<StatusAppointment> GetAllStatuses();
        List<string> GetPetNamesByIds(List<int> petIds);
        List<string> GetServiceNamesByIds(List<int> serviceIds);
        Appointment GetAppointmentWithDetails(int id);
        List<Appointment> GetPendingApprovalAppointments();
        List<Appointment> GetPendingAppointments();
        List<Appointment> GetPendingCancelAppointments();
        List<Appointment> GetAppointments(pet_spa_system1.ViewModel.AppointmentFilter filter);
        int CountAppointments(pet_spa_system1.ViewModel.AppointmentFilter filter);

        // Dashboard helpers
        List<Appointment> GetAppointmentsByDateRange(DateTime start, DateTime end);
        List<Appointment> GetAppointmentsByStatus(int statusId);
        void RestoreAppointment(int id);
        void RestoreAppointmentPets(int appointmentId);
        void RestoreAppointmentServices(int appointmentId);
        List<MonthlyAppointmentStats> GetMonthlyStats(int year);
        void Delete(int id);
        void DeleteAppointmentPets(int appointmentId);
        void DeleteAppointmentServices(int appointmentId);
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

