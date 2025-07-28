using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Services;

namespace pet_spa_system1.Controllers
{
    [Route("AdminAppointmentTest")]
    public class AdminAppointmentTestController : Controller
    {
        private readonly IEmailService _emailService;
        public AdminAppointmentTestController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet("SendTestEmail")]
        public IActionResult SendTestEmail(string to)
        {
            try
            {
                _emailService.SendTestEmail(to);
                return Ok(new { success = true, message = $"Đã gửi mail test tới {to}" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
