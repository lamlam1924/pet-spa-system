using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.IO;
using pet_spa_system1.ViewModels;
using pet_spa_system1.Models;

namespace pet_spa_system1.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public EmailService(IConfiguration config, IWebHostEnvironment webHostEnvironment)
    {
        _config = config;
        _webHostEnvironment = webHostEnvironment;
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

        try 
        {
            // Tạo ViewModel cho email template
            var viewModel = new AppointmentViewModel
            {
                CustomerName = customerName,
                Email = toEmail,
                AppointmentDate = appointmentDate.Date,
                AppointmentTime = appointmentDate.TimeOfDay,
                Notes = notes,
                SelectedPets = petNames?.Select(name => new Pet { Name = name, Breed = "" }).ToList() ?? new List<Pet>(),
                SelectedServices = serviceNames?.Select((name, index) => 
                    new Service { 
                        Name = name, 
                        Price = 0, // Giá thực tế nên lấy từ database
                        ServiceId = index + 1
                    }).ToList() ?? new List<Service>()
            };

            // Đọc CSS file cho email
            string cssFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "cssjsAppointment", "email-confirmation.css");
            string cssContent = "";
            
            if (File.Exists(cssFilePath))
            {
                cssContent = File.ReadAllText(cssFilePath);
                Console.WriteLine("[Email CSS] Đã tải CSS cho email template");
            }
            else
            {
                Console.WriteLine("[Email CSS Warning] Không tìm thấy file CSS cho email");
            }

            // Gửi email sử dụng template với CSS inline
            var client = new SmtpClient(smtpServer, smtpPort)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtpUser, smtpPass)
            };

            var fromAddress = new MailAddress(smtpUser, fromName);
            var toAddress = new MailAddress(toEmail, customerName);

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "Xác nhận đặt lịch dịch vụ thành công - SPA Thú Cưng",
                IsBodyHtml = true,
                Body = GenerateEmailHtmlWithInlineCss(viewModel, cssContent)
            };

            client.Send(message);
            Console.WriteLine("[Email] Đã gửi email xác nhận thành công");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Email Error] Lỗi gửi email: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[Email Error] Inner Exception: {ex.InnerException.Message}");
            }
            throw;
        }
    }
    
    private string GenerateEmailHtmlWithInlineCss(AppointmentViewModel model, string cssContent)
    {
        try
        {
            // Chuẩn bị HTML của email template với CSS nhúng trong thẻ style
            string emailHtmlTemplate = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Xác nhận đặt lịch thành công</title>
    <style type='text/css'>
    {cssContent}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='email-header'>
            <h1>SPA THÚ CƯNG</h1>
            <p>Chăm sóc chuyên nghiệp - Tình cảm trọn vẹn</p>
        </div>
        
        <div class='email-body'>
            <div class='greeting'>
                Xin chào {model.CustomerName},
            </div>
            
            <div class='message'>
                Cảm ơn bạn đã đặt lịch tại SPA Thú Cưng. Chúng tôi xác nhận thông tin đặt lịch của bạn như sau:
            </div>
            
            <div class='appointment-details'>
                <div class='section-title'>Thông tin lịch hẹn</div>
                
                <div class='detail-item'>
                    <div class='detail-label'>Ngày hẹn:</div>
                    <div class='detail-value'>{model.AppointmentDate.ToString("dddd, dd/MM/yyyy")} (VN)</div>
                </div>
                
                <div class='detail-item'>
                    <div class='detail-label'>Giờ hẹn:</div>
                    <div class='detail-value'>{model.AppointmentTime.ToString(@"hh\:mm")}</div>
                </div>
                
                <div class='detail-item'>
                    <div class='detail-label'>Khách hàng:</div>
                    <div class='detail-value'>{model.CustomerName}</div>
                </div>
                
                <div class='detail-item'>
                    <div class='detail-label'>Điện thoại:</div>
                    <div class='detail-value'>{model.Phone}</div>
                </div>
                
                <div class='detail-item'>
                    <div class='detail-label'>Email:</div>
                    <div class='detail-value'>{model.Email}</div>
                </div>";
            
            // Thêm ghi chú nếu có
            if (!string.IsNullOrEmpty(model.Notes))
            {
                emailHtmlTemplate += $@"
                <div class='notes-section'>
                    <strong>Ghi chú:</strong>
                    <p>{model.Notes}</p>
                </div>";
            }
            
            // Thú cưng
            emailHtmlTemplate += $@"
                <div class='pets-section'>
                    <div class='section-title'>Thú cưng được chọn</div>";
            
            if (model.SelectedPets != null && model.SelectedPets.Any())
            {
                foreach (var pet in model.SelectedPets)
                {
                    emailHtmlTemplate += $@"
                    <div class='pet-item'>
                        <div>
                            <i class='fa fa-paw'></i>
                            <strong>{pet.Name}</strong>
                            <span> - {pet.Breed}</span>
                        </div>
                    </div>";
                }
            }
            else
            {
                emailHtmlTemplate += "<p class='note'>Không có thông tin thú cưng</p>";
            }
            
            // Dịch vụ
            emailHtmlTemplate += $@"
                </div>
                
                <div class='services-section'>
                    <div class='section-title'>Dịch vụ được chọn</div>";
            
            decimal totalPrice = 0;
            
            if (model.SelectedServices != null && model.SelectedServices.Any())
            {
                foreach (var service in model.SelectedServices)
                {
                    totalPrice += service.Price;
                    emailHtmlTemplate += $@"
                    <div class='service-item'>
                        <div>{service.Name}</div>
                        <div class='service-price'>{service.Price.ToString("N0")} đ</div>
                    </div>";
                }
                
                emailHtmlTemplate += $@"
                    <div class='total-price'>
                        Tổng cộng: {totalPrice.ToString("N0")} đ
                    </div>";
            }
            else
            {
                emailHtmlTemplate += "<p class='note'>Không có thông tin dịch vụ</p>";
            }
            
            // Hoàn thiện phần còn lại của template
            emailHtmlTemplate += $@"
                </div>
            </div>
            
            <div class='message'>
                Xin vui lòng đến đúng giờ hẹn. Nếu bạn cần thay đổi hoặc hủy lịch hẹn, vui lòng liên hệ với chúng tôi trước 24 giờ để được hỗ trợ.
            </div>
            
            <div class='call-to-action'>
                <a href='tel:+84123456789' class='cta-button'>
                    Gọi Hỗ Trợ
                </a>
            </div>
        </div>
        
        <div class='email-footer'>
            <div class='footer-logo'>SPA THÚ CƯNG</div>
            
            <div class='footer-contact'>
                <p>123 Đường Lê Lợi, Quận 1, TP. Hồ Chí Minh</p>
                <p>Email: contact@spathucung.com | Điện thoại: 0123 456 789</p>
            </div>
            
            <div class='social-links'>
                <a href='#' class='social-link'><i class='fab fa-facebook-f'></i></a>
                <a href='#' class='social-link'><i class='fab fa-instagram'></i></a>
                <a href='#' class='social-link'><i class='fab fa-youtube'></i></a>
                <a href='#' class='social-link'><i class='fab fa-tiktok'></i></a>
            </div>
            
            <div class='copyright'>
                &copy; 2025 SPA Thú Cưng. Tất cả quyền được bảo lưu.
            </div>
        </div>
    </div>
</body>
</html>";
            
            return emailHtmlTemplate;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Template Error] Không thể tạo HTML email: {ex.Message}");
            throw;
        }
    }
}