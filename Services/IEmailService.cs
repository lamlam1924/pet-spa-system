using pet_spa_system1.ViewModel;
using pet_spa_system1.ViewModels;

namespace pet_spa_system1.Services
{
        public interface IEmailService
        {
                void SendBookingConfirmation(AppointmentViewModel viewModel);
                void SendOrderConfirmation(OrderConfirmationViewModel viewModel); // Thêm dòng này
                void SendEmailWithMessage(string title, string description, string email);
        }

}