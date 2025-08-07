using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pet_spa_system1.Services
{
    public class AdminStaffScheduleService : IAdminStaffScheduleService
    {
        private readonly PetDataShopContext _context;
        public AdminStaffScheduleService(PetDataShopContext context)
        {
            _context = context;
        }
        public async Task<List<Appointment>> GetAppointmentsAsync(int? staffId = null, DateTime? date = null, int? statusId = null)
        {
            var query = _context.Appointments
                .Include(a => a.Employee)
                .Include(a => a.Status)
                .Include(a => a.User)
                .Include(a => a.AppointmentPets)
                    .ThenInclude(ap => ap.Pet)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(aps => aps.Service)
                .Where(a => a.IsActive == true)
                .AsQueryable();
            if (staffId.HasValue)
                query = query.Where(a => a.EmployeeId == staffId);
            if (date.HasValue)
                query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);
            if (statusId.HasValue)
                query = query.Where(a => a.StatusId == statusId);
            return await query.OrderByDescending(a => a.AppointmentDate).ToListAsync();
        }
        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Employee)
                .Include(a => a.Status)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);
        }
        public async Task<bool> CreateAppointmentAsync(Appointment appointment)
        {
            // Log các giá trị
            Console.WriteLine($"UserId: {appointment.UserId}, EmployeeId: {appointment.EmployeeId}, StatusId: {appointment.StatusId}, AppointmentDate: {appointment.AppointmentDate}, IsActive: {appointment.IsActive}");

            _context.Appointments.Add(appointment);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            var appt = await _context.Appointments.FindAsync(id);
            if (appt == null) return false;
            appt.IsActive = false;
            _context.Appointments.Update(appt);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> SoftDeleteAppointmentAsync(int id)
        {
            var appt = await _context.Appointments.FindAsync(id);
            if (appt == null) return false;
            appt.IsActive = false;
            _context.Appointments.Update(appt);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> RestoreAppointmentAsync(int id)
        {
            var appt = await _context.Appointments.FindAsync(id);
            if (appt == null) return false;
            appt.IsActive = true;
            _context.Appointments.Update(appt);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<List<Appointment>> GetDeletedAppointmentsAsync(int? staffId = null, DateTime? date = null, int? statusId = null)
        {
            var query = _context.Appointments
                .Include(a => a.Employee)
                .Include(a => a.Status)
                .Include(a => a.User)
                .Include(a => a.AppointmentPets)
                    .ThenInclude(ap => ap.Pet)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(aps => aps.Service)
                .Where(a => a.IsActive == false)
                .AsQueryable();
            if (staffId.HasValue)
                query = query.Where(a => a.EmployeeId == staffId);
            if (date.HasValue)
                query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);
            if (statusId.HasValue)
                query = query.Where(a => a.StatusId == statusId);
            return await query.OrderByDescending(a => a.AppointmentDate).ToListAsync();
        }
        public async Task<bool> DeleteAppointmentPermanentAsync(int id)
        {
            var appt = await _context.Appointments.FindAsync(id);
            if (appt == null) return false;
            _context.Appointments.Remove(appt);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, int statusId, string? reason = null)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(appointmentId);
                if (appointment == null) return false;

                appointment.StatusId = statusId;

                // Nếu có lý do (thường là khi hủy), có thể lưu vào Notes
                if (!string.IsNullOrEmpty(reason))
                {
                    appointment.Notes = string.IsNullOrEmpty(appointment.Notes)
                        ? reason
                        : $"{appointment.Notes}\n[Cập nhật]: {reason}";
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating appointment status: {ex.Message}");
                return false;
            }
        }
    }
}