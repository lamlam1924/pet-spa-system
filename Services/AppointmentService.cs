using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using System.Linq;


namespace pet_spa_system1.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;
        
        public AppointmentService(IAppointmentRepository repo)
        {
            _repo = repo;
        }

        public bool SaveAppointment(AppointmentViewModel vm, int userId)
        {
            try
            {
                var appointment = new Appointment
                {
                    UserId = userId,
                    AppointmentDate = vm.AppointmentDate.Date.Add(vm.AppointmentTime),
                    StatusId = 1,
                    Notes = vm.Notes,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                int appId = _repo.AddAppointment(appointment);
                foreach (var petId in vm.SelectedPetIds)
                    _repo.AddAppointmentPet(appId, petId);
                foreach (var serviceId in vm.SelectedServiceIds)
                    _repo.AddAppointmentService(appId, serviceId);

                _repo.Save(); // Lưu tất cả thay đổi
                return true;
            }
            catch
            {
                // Có thể log lỗi chi tiết tại đây
                return false;
            }
        }

        public AppointmentHistoryViewModel GetAppointmentHistory(int userId)
        {
            var appointments = _repo.GetByUserId(userId);

            var items = appointments.Select(a => new AppointmentHistoryItemViewModel()
            {
                AppointmentId = a.AppointmentId,
                AppointmentDate = a.AppointmentDate,
                StatusId = a.StatusId,
                StatusName = a.Status?.StatusName ?? "Chưa có trạng thái",
                Notes = a.Notes,
                ServiceNames = a.AppointmentServices?.Select(s => s.Service?.Name ?? "Chưa có dữ liệu").ToList() ?? new List<string> { "Chưa có dữ liệu" },
                PetNames = a.AppointmentPets?.Select(p => p.Pet?.Name ?? "Chưa có dữ liệu").ToList() ?? new List<string> { "Chưa có dữ liệu" }
            }).ToList();

            return new AppointmentHistoryViewModel
            {
                Appointments = items
            };
        }


        public Appointment GetAppointmentById(int appointmentId)
        {
            return _repo.GetById(appointmentId);
        }
    }
}