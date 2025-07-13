using pet_spa_system1.ViewModels;

namespace pet_spa_system1.Services
{
    public interface IEmailService
    {
        void SendBookingConfirmation(AppointmentViewModel viewModel);
    }
}
