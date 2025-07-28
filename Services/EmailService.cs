
using System.Net;
using System.Net.Mail;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            var emailSection = configuration.GetSection("EmailSettings");
            _smtpServer = emailSection["SmtpServer"] ?? "";
            _smtpPort = int.TryParse(emailSection["SmtpPort"], out var port) ? port : 587;
            _smtpUser = emailSection["SmtpUser"] ?? "";
            _smtpPass = emailSection["SmtpPass"] ?? "";
            _fromName = emailSection["FromName"] ?? "Pet Spa";
            _fromEmail = _smtpUser;
        }

        public void SendTestEmail(string to)
        {
            string subject = "[Pet Spa] Test Email";
            string body = $"<h3>Đây là email test gửi từ hệ thống Pet Spa.</h3><p>Thời gian: {DateTime.Now}</p>";
            SendEmail(to, subject, body);
        }
        public void SendOrderConfirmation(OrderConfirmationViewModel viewModel)
        {
            string subject = $"[Pet Spa] Xác nhận đơn hàng #{viewModel.OrderId}";
            string body = $"<h3>Xin chào {viewModel.CustomerName},</h3><p>Đơn hàng của bạn đã được xác nhận.</p><p>Mã đơn: {viewModel.OrderId}</p>";
            SendEmail(viewModel.Email, subject, body);
        }

        public void SendEmailWithMessage(string title, string description, string email)
        {
            string subject = title;
            string body = $"<p>{description}</p>";
            SendEmail(email, subject, body);
        }

        public void SendAppointmentConfirmation(AppointmentConfirmationEmailModel model)
        {
            string subject = "[Pet Spa] Xác nhận lịch hẹn";
            string body = $"<h3>Xin chào {model.CustomerName},</h3><p>Lịch hẹn của bạn đã được xác nhận.</p><p>Thời gian: {model.AppointmentDateTime:dd/MM/yyyy HH:mm}</p>";
            SendEmail(model.ToEmail, subject, body);
        }

        public void SendAppointmentRejected(AppointmentRejectedEmailModel model)
        {
            string subject = "[Pet Spa] Lịch hẹn bị từ chối";
            string body = $"<h3>Xin chào {model.CustomerName},</h3><p>Rất tiếc, lịch hẹn của bạn đã bị từ chối.</p><p>Thời gian: {model.ProposedDateTime:dd/MM/yyyy HH:mm}</p>";
            SendEmail(model.ToEmail, subject, body);
        }

        public void SendAppointmentCancelled(AppointmentCancelledEmailModel model)
        {
            string subject = "[Pet Spa] Lịch hẹn bị hủy";
            string body = $"<h3>Xin chào {model.CustomerName},</h3><p>Lịch hẹn của bạn đã bị hủy.</p><p>Thời gian: {model.AppointmentDateTime:dd/MM/yyyy HH:mm}</p>";
            SendEmail(model.ToEmail, subject, body);
        }

        public void SendAppointmentReminder(AppointmentReminderEmailModel model)
        {
            string subject = "[Pet Spa] Nhắc lịch hẹn";
            string body = $"<h3>Xin chào {model.CustomerName},</h3><p>Đây là email nhắc lịch hẹn của bạn.</p><p>Thời gian: {model.AppointmentDateTime:dd/MM/yyyy HH:mm}</p>";
            SendEmail(model.ToEmail, subject, body);
        }

        public void SendEmail(string to, string subject, string body)
        {
            string logPath = "email_debug.log";
            try
            {
                var smtp = new SmtpClient(_smtpServer)
                {
                    Port = _smtpPort,
                    Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                    EnableSsl = true
                };
                var mail = new MailMessage()
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(to);
                File.AppendAllText(logPath, $"[SEND] {DateTime.Now}: To={to}, Subject={subject}\nBody={body}\n\n");
                smtp.Send(mail);
                File.AppendAllText(logPath, $"[SUCCESS] {DateTime.Now}: Sent to {to}\n");
            }
            catch (Exception ex)
            {
                File.AppendAllText(logPath, $"[ERROR] {DateTime.Now}: To={to}, Subject={subject}, Error={ex.Message}\nStackTrace={ex.StackTrace}\n\n");
                throw;
            }
        }
    }
}
