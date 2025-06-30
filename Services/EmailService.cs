using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace pet_spa_system1.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public void SendBookingConfirmation(
        string toEmail,
        string customerName,
        DateTime appointmentDate,
        string? notes = null,
        List<string>? petNames = null,
        List<string>? serviceNames = null
    )
    {
        // Kiểm tra cấu hình email
        var smtpServer = _config["EmailSettings:SmtpServer"];
        if (string.IsNullOrEmpty(smtpServer))
        {
            throw new InvalidOperationException("SmtpServer không được cấu hình trong appsettings.json");
        }
        
        var smtpPortStr = _config["EmailSettings:SmtpPort"];
        if (string.IsNullOrEmpty(smtpPortStr) || !int.TryParse(smtpPortStr, out int smtpPort))
        {
            throw new InvalidOperationException($"SmtpPort không hợp lệ: {smtpPortStr}");
        }
        
        var smtpUser = _config["EmailSettings:SmtpUser"];
        if (string.IsNullOrEmpty(smtpUser) || smtpUser == "yourmail@gmail.com")
        {
            throw new InvalidOperationException($"SmtpUser không hợp lệ hoặc chưa được cấu hình: {smtpUser}");
        }
        
        var smtpPass = _config["EmailSettings:SmtpPass"];
        if (string.IsNullOrEmpty(smtpPass) || smtpPass == "yourapppassword")
        {
            throw new InvalidOperationException("SmtpPass chưa được cấu hình hoặc là giá trị mặc định");
        }
        
        var fromName = _config["EmailSettings:FromName"] ?? "SPA Thú Cưng";
        
        Console.WriteLine($"[Email Debug] Chuẩn bị gửi email từ {smtpUser} đến {toEmail}");
        Console.WriteLine($"[Email Debug] SMTP: {smtpServer}:{smtpPort}, SSL enabled");

        var fromAddress = new MailAddress(smtpUser, fromName);
        var toAddress = new MailAddress(toEmail, customerName);

        string pets = (petNames != null && petNames.Count > 0)
            ? $"<ul>{string.Join("", petNames.Select(name => $"<li>{name}</li>"))}</ul>"
            : "Không có";

        string services = (serviceNames != null && serviceNames.Count > 0)
            ? $"<ul>{string.Join("", serviceNames.Select(name => $"<li>{name}</li>"))}</ul>"
            : "Không có";

        string body = $@"
        <div style='font-family:sans-serif; border:1px solid #eee; padding:20px; border-radius:8px;'>
            <h2 style='color:#4CAF50;'>Xác nhận đặt lịch spa thú cưng thành công</h2>
            <p>Xin chào <b>{customerName}</b>,</p>
            <p>Bạn đã đặt lịch thành công cho thú cưng tại <b>SPA Thú Cưng</b>!</p>
            <p><b>Thời gian:</b> {appointmentDate:dd/MM/yyyy HH:mm}</p>
            {(string.IsNullOrWhiteSpace(notes) ? "" : $"<p><b>Ghi chú:</b> {notes}</p>")}
            <p><b>Thú cưng:</b> {pets}</p>
            <p><b>Dịch vụ đã chọn:</b> {services}</p>
            <hr>
            <p style='font-size:small;color:#888;'>Nếu có thắc mắc, liên hệ hotline: 0123 456 789</p>
        </div>
    ";

        var smtp = new SmtpClient
        {
            Host = smtpServer,
            Port = smtpPort,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            Timeout = 20000
        };

        try
        {
            Console.WriteLine("[Email Debug] Tạo message và chuẩn bị gửi");
            using (var message = new MailMessage(fromAddress, toAddress)
                  {
                      Subject = "Xác nhận đặt lịch spa thú cưng thành công",
                      Body = body,
                      IsBodyHtml = true
                  })
            {
                // Nếu gặp lỗi về SSL certificate validation
                // System.Net.ServicePointManager.ServerCertificateValidationCallback = 
                //    (sender, certificate, chain, sslPolicyErrors) => true;
                
                Console.WriteLine("[Email Debug] Bắt đầu gửi email...");
                smtp.Send(message);
                Console.WriteLine("[Email Debug] Đã gửi thành công!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Email Error] Lỗi khi gửi email: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[Email Error] Inner Exception: {ex.InnerException.Message}");
            }
            throw; // Re-throw để xử lý ở lớp gọi
        }
    }
}