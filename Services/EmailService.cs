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
            string body =
                $@"<div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;padding:20px;background:#fff7f4;border-radius:12px;'>
            <h2 style='color:#ff6f61;'>Cảm ơn bạn đã đặt hàng tại SPA Thú Cưng!</h2>
            <p>Xin chào <b>{viewModel.CustomerName}</b>,</p>
            <p>Đơn hàng <b>#{viewModel.OrderId}</b> của bạn đã được ghi nhận.</p>
            <h3>Chi tiết đơn hàng:</h3>
            <table style='width:100%;border-collapse:collapse;'>
                <thead>
                    <tr style='background:#ffe5e0;'>
                        <th style='padding:8px;'>Sản phẩm</th>
                        <th style='padding:8px;'>Số lượng</th>
                        <th style='padding:8px;'>Đơn giá</th>
                    </tr>
                </thead>
                <tbody>
                    {string.Join("", viewModel.Items.Select(item =>
                        $"<tr>" +
                        $"<td style='padding:8px;'>{item.ProductName}</td>" +
                        $"<td style='padding:8px;text-align:center;'>{item.Quantity}</td>" +
                        $"<td style='padding:8px;'>{(item.UnitPrice.HasValue ? item.UnitPrice.Value.ToString("N0") : "0")} đ</td>" +
                        $"</tr>"))}
                </tbody>
            </table>
            <p style='margin-top:16px;'><b>Tổng tiền:</b> <span style='color:#43b581;font-size:1.2em;'>{viewModel.TotalAmount.ToString("N0")} đ</span></p>
            <p>Địa chỉ nhận hàng: <b>{viewModel.ShippingAddress}</b></p>
            <p style='color:#888;'>Nếu có thắc mắc, vui lòng liên hệ với chúng tôi.</p>
        </div>";
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
            var serviceNames = string.Join(", ", model.SelectedServices.Select(s => s.Name));
            var petNames = string.Join(", ", model.SelectedPets.Select(p => p.Name));
            string body = $@"
<div style='font-family:Segoe UI,Arial,sans-serif;max-width:600px;margin:auto;border:1px solid #eee;border-radius:8px;overflow:hidden;'>
    <div style='background:#f7b731;padding:24px 32px;'>
        <h2 style='color:#fff;margin:0;'>Pet Spa - Xác nhận lịch hẹn</h2>
    </div>
    <div style='padding:24px 32px;background:#fff;'>
        <p>Xin chào <b>{model.CustomerName}</b>,</p>
        <p>Lịch hẹn của bạn tại <b>Pet Spa</b> đã được <span style='color:#27ae60;font-weight:bold;'>xác nhận</span>.</p>
        <table style='width:100%;border-collapse:collapse;margin:16px 0;'>
            <tr>
                <td style='padding:8px 0;color:#888;'>Thời gian:</td>
                <td style='padding:8px 0;'><b>{model.AppointmentDateTime:dd/MM/yyyy HH:mm}</b></td>
            </tr>
            <tr>
                <td style='padding:8px 0;color:#888;'>Dịch vụ:</td>
                <td style='padding:8px 0;'>{serviceNames}</td>
            </tr>
            <tr>
                <td style='padding:8px 0;color:#888;'>Thú cưng:</td>
                <td style='padding:8px 0;'>{petNames}</td>
            </tr>
        </table>
        <p>Địa chỉ Spa: <b>123 Đường ABC, Quận 1, TP.HCM</b><br/>
        Hotline: <b>0123 456 789</b></p>
        <p style='color:#888;font-size:13px;'>Vui lòng đến đúng giờ để được phục vụ tốt nhất.<br/>Cảm ơn bạn đã tin tưởng Pet Spa!</p>
    </div>
    <div style='background:#f7b731;padding:12px 32px;text-align:center;color:#fff;font-size:13px;'>
        &copy; {DateTime.Now.Year} Pet Spa. All rights reserved.
    </div>
</div>
";
            SendEmail(model.ToEmail, subject, body);
        }

        public void SendAppointmentRejected(AppointmentRejectedEmailModel model)
        {
            string subject = "[Pet Spa] Lịch hẹn bị từ chối";
            var serviceNames = string.Join(", ", model.SelectedServices.Select(s => s.Name));
            var petNames = string.Join(", ", model.SelectedPets.Select(p => p.Name));
            string body = $@"
<div style='font-family:Segoe UI,Arial,sans-serif;max-width:600px;margin:auto;border:1px solid #eee;border-radius:8px;overflow:hidden;'>
    <div style='background:#f39c12;padding:24px 32px;'>
        <h2 style='color:#fff;margin:0;'>Pet Spa - Từ chối lịch hẹn</h2>
    </div>
    <div style='padding:24px 32px;background:#fff;'>
        <p>Xin chào <b>{model.CustomerName}</b>,</p>
        <p>Rất tiếc, lịch hẹn của bạn tại <b>Pet Spa</b> đã bị <span style='color:#e67e22;font-weight:bold;'>từ chối</span>.</p>
        <table style='width:100%;border-collapse:collapse;margin:16px 0;'>
            <tr>
                <td style='padding:8px 0;color:#888;'>Thời gian:</td>
                <td style='padding:8px 0;'><b>{model.ProposedDateTime:dd/MM/yyyy HH:mm}</b></td>
            </tr>
            <tr>
                <td style='padding:8px 0;color:#888;'>Dịch vụ:</td>
                <td style='padding:8px 0;'>{serviceNames}</td>
            </tr>
            <tr>
                <td style='padding:8px 0;color:#888;'>Thú cưng:</td>
                <td style='padding:8px 0;'>{petNames}</td>
            </tr>
        </table>
        <p style='color:#888;font-size:13px;'>Nếu cần hỗ trợ, vui lòng liên hệ hotline <b>0123 456 789</b>.</p>
    </div>
    <div style='background:#f39c12;padding:12px 32px;text-align:center;color:#fff;font-size:13px;'>
        &copy; {DateTime.Now.Year} Pet Spa. All rights reserved.
    </div>
</div>
";
            SendEmail(model.ToEmail, subject, body);
        }

        public void SendAppointmentCancelled(AppointmentCancelledEmailModel model)
        {
            string subject = "[Pet Spa] Lịch hẹn đã được hủy";
            var serviceNames = string.Join(", ", model.SelectedServices.Select(s => s.Name));
            var petNames = string.Join(", ", model.SelectedPets.Select(p => p.Name));
            string body = $@"
<div style='font-family:Segoe UI,Arial,sans-serif;max-width:600px;margin:auto;border:1px solid #eee;border-radius:8px;overflow:hidden;'>
    <div style='background:#e74c3c;padding:24px 32px;'>
        <h2 style='color:#fff;margin:0;'>Pet Spa - Hủy lịch hẹn</h2>
    </div>
    <div style='padding:24px 32px;background:#fff;'>
        <p>Xin chào <b>{model.CustomerName}</b>,</p>
        <p>Chúng tôi xin thông báo lịch hẹn của bạn tại <b>Pet Spa</b> đã được <span style='color:#e74c3c;font-weight:bold;'>hủy</span>.</p>
        <table style='width:100%;border-collapse:collapse;margin:16px 0;'>
            <tr>
                <td style='padding:8px 0;color:#888;'>Thời gian:</td>
                <td style='padding:8px 0;'><b>{model.AppointmentDateTime:dd/MM/yyyy HH:mm}</b></td>
            </tr>
            <tr>
                <td style='padding:8px 0;color:#888;'>Dịch vụ:</td>
                <td style='padding:8px 0;'>{serviceNames}</td>
            </tr>
            <tr>
                <td style='padding:8px 0;color:#888;'>Thú cưng:</td>
                <td style='padding:8px 0;'>{petNames}</td>
            </tr>
        </table>
        <p style='color:#888;font-size:13px;'>Nếu có thắc mắc, vui lòng liên hệ hotline <b>0123 456 789</b>.<br/>Cảm ơn bạn đã quan tâm Pet Spa!</p>
    </div>
    <div style='background:#e74c3c;padding:12px 32px;text-align:center;color:#fff;font-size:13px;'>
        &copy; {DateTime.Now.Year} Pet Spa. All rights reserved.
    </div>
</div>
";
            SendEmail(model.ToEmail, subject, body);
        }

        public void SendAppointmentReminder(AppointmentReminderEmailModel model)
        {
            string subject = "[Pet Spa] Nhắc lịch hẹn";
            var serviceNames = string.Join(", ", model.SelectedServices.Select(s => s.Name));
            var petNames = string.Join(", ", model.SelectedPets.Select(p => p.Name));
            string body = $@"
<div style='font-family:Segoe UI,Arial,sans-serif;max-width:600px;margin:auto;border:1px solid #eee;border-radius:8px;overflow:hidden;'>
    <div style='background:#2980b9;padding:24px 32px;'>
        <h2 style='color:#fff;margin:0;'>Pet Spa - Nhắc lịch hẹn</h2>
    </div>
    <div style='padding:24px 32px;background:#fff;'>
        <p>Xin chào <b>{model.CustomerName}</b>,</p>
        <p>Bạn có một lịch hẹn tại <b>Pet Spa</b> vào ngày <b>{model.AppointmentDateTime:dd/MM/yyyy HH:mm}</b>.</p>
        <table style='width:100%;border-collapse:collapse;margin:16px 0;'>
            <tr>
                <td style='padding:8px 0;color:#888;'>Dịch vụ:</td>
                <td style='padding:8px 0;'>{serviceNames}</td>
            </tr>
            <tr>
                <td style='padding:8px 0;color:#888;'>Thú cưng:</td>
                <td style='padding:8px 0;'>{petNames}</td>
            </tr>
        </table>
        <p>Địa chỉ Spa: <b>123 Đường ABC, Quận 1, TP.HCM</b><br/>
        Hotline: <b>0123 456 789</b></p>
        <p style='color:#888;font-size:13px;'>Vui lòng đến đúng giờ để được phục vụ tốt nhất.<br/>Cảm ơn bạn đã tin tưởng Pet Spa!</p>
    </div>
    <div style='background:#2980b9;padding:12px 32px;text-align:center;color:#fff;font-size:13px;'>
        &copy; {DateTime.Now.Year} Pet Spa. All rights reserved.
    </div>
</div>
";
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
                File.AppendAllText(logPath,
                    $"[ERROR] {DateTime.Now}: To={to}, Subject={subject}, Error={ex.Message}\nStackTrace={ex.StackTrace}\n\n");
                throw;
            }
        }
    }
}