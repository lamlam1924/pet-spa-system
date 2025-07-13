using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using pet_spa_system1.Models;
using pet_spa_system1.ViewModel;
using RouteData = Microsoft.AspNetCore.Routing.RouteData;

namespace pet_spa_system1.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailService(
            IConfiguration config,
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public void SendBookingConfirmation(AppointmentViewModel viewModel)
        {
            // Lấy cấu hình email
            var smtpServer = _config["EmailSettings:SmtpServer"];
            var smtpPortStr = _config["EmailSettings:SmtpPort"];
            int smtpPort = int.TryParse(smtpPortStr, out int port) ? port : 587;

            var smtpUser = _config["EmailSettings:SmtpUser"];
            var smtpPass = _config["EmailSettings:SmtpPass"];
            var fromName = _config["EmailSettings:FromName"] ?? "SPA Thú Cưng";

            if (string.IsNullOrWhiteSpace(smtpUser))
            {
                throw new InvalidOperationException("EmailSettings:SmtpUser chưa được cấu hình hoặc bị null.");
            }

            try
            {
                // Render Razor view thành HTML string
                string viewPath = "/Views/Email/SuccessEmail.cshtml";
                string emailBody = RenderViewToString(viewPath, viewModel);

                var client = new SmtpClient(smtpServer, smtpPort)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(smtpUser, smtpPass)
                };
                var fromAddress = new MailAddress(smtpUser, fromName);
                var toAddress = new MailAddress(viewModel.Email, viewModel.CustomerName);
                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = "Xác nhận đặt lịch dịch vụ thành công - SPA Thú Cưng",
                    IsBodyHtml = true,
                    Body = emailBody
                };
                try
                {
                    client.Send(message);
                    Debug.WriteLine("[Email] Đã gửi email xác nhận thành công");
                }
                finally
                {
                    message.Dispose();
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Email Error] Lỗi gửi email: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"[Email Error] Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        // Render Razor view ra string (tích hợp logic từ RazorViewToStringRenderer)
        private string RenderViewToString(string viewPath, object model)
        {
            var actionContext = new ActionContext(
                _httpContextAccessor.HttpContext ?? new DefaultHttpContext { RequestServices = _serviceProvider },
                new RouteData(),
                new ActionDescriptor()
            );

            var viewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewPath, isMainPage: true);
            if (!viewResult.Success)
            {
                throw new FileNotFoundException($"View {viewPath} not found.");
            }

            var viewDictionary = new ViewDataDictionary(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary())
            {
                Model = model
            };

            using (var sw = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                var renderTask = viewResult.View.RenderAsync(viewContext);
                renderTask.GetAwaiter().GetResult();
                return sw.ToString();
            }
        }
    }
}
