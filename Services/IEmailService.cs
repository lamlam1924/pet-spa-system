using pet_spa_system1.ViewModel;
using pet_spa_system1.ViewModels;

namespace pet_spa_system1.Services
{
public interface IEmailService
{
    void SendEmail(string to, string subject, string body);
    void SendOrderConfirmation(OrderConfirmationViewModel viewModel);
    void SendEmailWithMessage(string title, string description, string email);
    void SendAppointmentConfirmation(AppointmentConfirmationEmailModel model);
    void SendAppointmentRejected(AppointmentRejectedEmailModel model);
    void SendAppointmentCancelled(AppointmentCancelledEmailModel model);
    void SendAppointmentReminder(AppointmentReminderEmailModel model);
    void SendTestEmail(string to);
}
}