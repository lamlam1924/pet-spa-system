using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModel;
//using pet_spa_system1.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace pet_spa_system1.Controllers
{
    public class LoginController : Controller
    {

        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public LoginController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginRegisterViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRegisterViewModel model)
        {
            var user = await _userService.AuthenticateAsync(model.Login.Email, model.Login.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                return View(model);
            }

            HttpContext.Session.SetInt32("CurrentUserId", user.UserId);
            HttpContext.Session.SetString("CurrentUserName", user.Username);
            HttpContext.Session.SetInt32("CurrentUserRoleId", user.RoleId);
            if(user.Address != null) { 
                HttpContext.Session.SetString("CurrentUserAddress", user.Address);
            }

            if (user.RoleId == 1)
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

           
        }


        [HttpPost]
        public async Task<IActionResult> Register(LoginRegisterViewModel model)
        {
            var newUser = await _userService.RegisterAsync(
                model.Register.Email,
                model.Register.Name,
                model.Register.Password
            );

            if (newUser == null)
            {
                ViewBag.ActiveTab = "register";
                ModelState.AddModelError("", "Email đã được đăng ký.");
                return View("Login", model);
            }

            HttpContext.Session.SetInt32("CurrentUserId", newUser.UserId);
            HttpContext.Session.SetString("CurrentUserName", newUser.Username);
            HttpContext.Session.SetInt32("CurrentUserRoleId", newUser.RoleId);
            if (newUser.Address != null)
            {
                HttpContext.Session.SetString("CurrentUserAddress", newUser.Address);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("login/google")]
        public IActionResult LoginWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Login");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded) return RedirectToAction("Login");

            var claims = result.Principal.Identities
                .FirstOrDefault()?.Claims.Select(claim =>
                    new
                    {
                        claim.Type,
                        claim.Value
                    });

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
            Console.WriteLine(email + " " + name + " khong lay duoc email");
            // TODO: Check user in database, auto-register or login
            // Example: Save session
            var user = await _userService.GetUserByEmail(email);
            if (user == null)
            {

                user = await _userService.RegisterByGoogle(email, name);

            }
            //var user = new User { Email = email, FullName = name };
            HttpContext.Session.SetInt32("CurrentUserId", user.UserId);
            HttpContext.Session.SetString("CurrentUserName", user.Username);
            // lưu role vào session nếu cần
            HttpContext.Session.SetInt32("CurrentUserRoleId", user.RoleId);
            if (user.Address != null)
            {
                HttpContext.Session.SetString("CurrentUserAddress", user.Address);
            }


            int? userId = HttpContext.Session.GetInt32("CurrentUserId");

            if (userId.HasValue)
            {
                Console.WriteLine($"User ID = {userId.Value}");
            }
            else
            {
                Console.WriteLine("Chưa đăng nhập hoặc session đã hết hạn");
            }



            if (user.RoleId == 1)
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            // Kiểm tra email có tồn tại không
            var user = await _userService.GetUserByEmail(Email);
            if (user == null)
            {
                ViewBag.ShowModal = "forgot";
                ModelState.AddModelError("", "Email không tồn tại trong hệ thống.");
                return View("Login", new LoginRegisterViewModel());
            }

            // Tạo OTP 6 số
            var random = new Random();
            var otp = random.Next(100000, 999999).ToString();

            // Lưu OTP và email vào TempData (hoặc Session nếu cần)
            TempData["ResetOtp"] = otp;
            TempData["ResetEmail"] = Email;
            TempData["ShowStep2"] = true;

            // Gửi email
            var title = "OTP resetPassword";
            var body = $"Mã OTP đặt lại mật khẩu của bạn là: <b>{otp}</b>";
            _emailService.SendEmailWithMessage(title, body, Email);

            // Hiển thị lại modal bước nhập OTP
            ViewBag.ShowModal = "forgot";
            ViewBag.ShowStep2 = true;
            ViewBag.ResetEmail = Email;
            return View("Login", new LoginRegisterViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtpAndResetPassword(string Email, string OtpCode, string NewPassword, string ConfirmPassword)
        {
            // Lấy OTP và email đã lưu
            var savedOtp = TempData["ResetOtp"] as string;
            var savedEmail = TempData["ResetEmail"] as string;

            // Đảm bảo giữ lại TempData cho lần render tiếp theo nếu cần
            TempData.Keep("ResetOtp");
            TempData.Keep("ResetEmail");

            if (string.IsNullOrEmpty(savedOtp) || string.IsNullOrEmpty(savedEmail) || !string.Equals(Email, savedEmail, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ShowModal = "forgot";
                ViewBag.ShowStep2 = true;
                ViewBag.ResetEmail = Email;
                ModelState.AddModelError("", "Thông tin xác thực không hợp lệ hoặc đã hết hạn. Vui lòng thử lại.");
                return View("Login", new LoginRegisterViewModel());
            }

            if (OtpCode != savedOtp)
            {
                ViewBag.ShowModal = "forgot";
                ViewBag.ShowStep2 = true;
                ViewBag.ResetEmail = Email;
                ModelState.AddModelError("", "Mã OTP không đúng. Vui lòng kiểm tra lại.");
                return View("Login", new LoginRegisterViewModel());
            }

            if (NewPassword != ConfirmPassword)
            {
                ViewBag.ShowModal = "forgot";
                ViewBag.ShowStep2 = true;
                ViewBag.ResetEmail = Email;
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp.");
                return View("Login", new LoginRegisterViewModel());
            }

            // Đặt lại mật khẩu
            var user = await _userService.GetUserByEmail(Email);
            if (user == null)
            {
                ViewBag.ShowModal = "forgot";
                ViewBag.ShowStep2 = true;
                ViewBag.ResetEmail = Email;
                ModelState.AddModelError("", "Không tìm thấy tài khoản.");
                return View("Login", new LoginRegisterViewModel());
            }
            var result = await _userService.UpdatePasswordWithUserIdAsync(user.UserId, NewPassword);
            if (!result.Success)
            {
                TempData.Remove("ResetOtp");
                TempData.Remove("ResetEmail");
                TempData.Remove("ShowStep2");
                TempData["ResetFail"] = result.Message ?? "Đặt lại mật khẩu thất bại.";
                return RedirectToAction("Login");
            }

            // Xóa OTP khỏi TempData
            TempData.Remove("ResetOtp");
            TempData.Remove("ResetEmail");
            TempData.Remove("ShowStep2");

            // Hiển thị thông báo thành công (có thể chuyển sang trang đăng nhập)
            TempData["ResetSuccess"] = "Đặt lại mật khẩu thành công.";
            return RedirectToAction("Login");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            // Xóa session nếu cần
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }

}


