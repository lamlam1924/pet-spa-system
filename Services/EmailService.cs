using System.Net;
using System.Net.Mail;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer = "smtp.yourserver.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "your_email@domain.com";
        private readonly string _smtpPass = "your_password";
        private readonly string _fromEmail = "your_email@domain.com";

        public void SendOrderConfirmation(OrderConfirmationViewModel viewModel)
        {
            // TODO: Implement actual email logic
        }

        public void SendEmailWithMessage(string title, string description, string email)
        {
            // TODO: Implement actual email logic
        }

        public void SendAppointmentConfirmation(AppointmentConfirmationEmailModel model)
        {
            // TODO: Implement actual email logic
        }

        public void SendAppointmentRejected(AppointmentRejectedEmailModel model)
        {
            // TODO: Implement actual email logic
        }

        public void SendAppointmentCancelled(AppointmentCancelledEmailModel model)
        {
            // TODO: Implement actual email logic
        }

        public void SendAppointmentReminder(AppointmentReminderEmailModel model)
        {
            // TODO: Implement actual email logic
        }

        public void SendEmail(string to, string subject, string body)
        {
            var smtp = new SmtpClient(_smtpServer)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true
            };
            var mail = new MailMessage(_fromEmail, to, subject, body);
            smtp.Send(mail);
        }
    }
}
