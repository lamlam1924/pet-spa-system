using pet_spa_system1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pet_spa_system1.Services
{
    public interface IAdminStaffScheduleService
    {
        Task<List<Appointment>> GetAppointmentsAsync(int? staffId = null, DateTime? date = null, int? statusId = null);
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<bool> CreateAppointmentAsync(Appointment appointment);
        Task<bool> UpdateAppointmentAsync(Appointment appointment);
        Task<bool> DeleteAppointmentAsync(int id);
        Task<bool> SoftDeleteAppointmentAsync(int id);
        Task<bool> RestoreAppointmentAsync(int id);
        Task<List<Appointment>> GetDeletedAppointmentsAsync(int? staffId = null, DateTime? date = null, int? statusId = null);
        Task<bool> DeleteAppointmentPermanentAsync(int id);
        Task<bool> UpdateAppointmentStatusAsync(int appointmentId, int statusId, string? reason = null);
    }
}