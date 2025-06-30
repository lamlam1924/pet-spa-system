using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.Utils;
using pet_spa_system1.ViewModel;
using System.Security.Claims;

namespace pet_spa_system1.Controllers
{
    public class LoginController : Controller
    {

        private readonly UserService _userService;

        public LoginController(UserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginRegisterViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRegisterViewModel model)
        {
            Console.WriteLine($"Email: {model.Login.Email}, Password: {model.Login.Password}");

            var user = await _userService.AuthenticateAsync(
                model.Login.Email,
                model.Login.Password
            );

            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                return View(model);
            }

            // 🔐 Lưu user vào session
            HttpContext.Session.SetObjectAsJson("CurrentUser", user);
            var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
            if (currentUser != null)
            {
                Console.WriteLine("✅ Session chứa object CurrentUser");
            }
            else
            {
                Console.WriteLine("❌ Session chưa được tạo hoặc đã bị xóa");
            }


            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> Register(LoginRegisterViewModel model)
        {
            //var name = HttpContext.Request.Form["Name"];
            //var email = HttpContext.Request.Form["Email"];
            //var password = HttpContext.Request.Form["Password"];
            Console.WriteLine($"[REGISTER] Name: {model.Register.Name}, Email: {model.Register.Email}, Password: {model.Register.Password}");
            Console.WriteLine("[DEBUG] Gọi tới RegisterAsync");

            var newUser = await _userService.RegisterAsync(
                model.Register.Name,
                model.Register.Email,
                model.Register.Password
                );

            if (newUser == null)
            {
                ViewBag.ActiveTab = "register";
                ModelState.AddModelError("", "Email đã được đăng ký.");
                return View("Login", model);
            }
            HttpContext.Session.SetObjectAsJson("CurrentUser", newUser);
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
            Console.WriteLine(email + " " + name +" khong lay duoc email");
            // TODO: Check user in database, auto-register or login
            // Example: Save session
            var user = await _userService.GetUserByEmail(email);
            if (user == null)
            {

                user = await _userService.RegisterByGoogle(email, name);

            }
            //var user = new User { Email = email, FullName = name };
            HttpContext.Session.SetObjectAsJson("CurrentUser", user);
            var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
            if (currentUser != null)
            {
                Console.WriteLine("✅ Session chứa object CurrentUser");
            }
            else
            {
                Console.WriteLine("❌ Session chưa được tạo hoặc đã bị xóa");
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpGet("/Login/Logout")]
        public IActionResult Logout()
        {
            // Xoá tất cả session
            HttpContext.Session.Clear();

            // (Nếu dùng cookie auth) SignOut
            // await HttpContext.SignOutAsync(); // nếu dùng Identity hoặc cookie auth

            // Chuyển về trang đăng nhập hoặc trang chủ
            return RedirectToAction("Index", "Home");
        }

    }

}

