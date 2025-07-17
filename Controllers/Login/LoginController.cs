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

        public LoginController(IUserService userService)
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
            var user = await _userService.AuthenticateAsync(model.Login.Email, model.Login.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                return View(model);
            }

            HttpContext.Session.SetInt32("CurrentUserId", user.UserId);
            HttpContext.Session.SetString("CurrentUserName", user.Username);
            HttpContext.Session.SetInt32("CurrentUserRoleId", user.RoleId);
            HttpContext.Session.SetString("CurrentUserAddress", user.Address);

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

            HttpContext.Session.SetInt32("CurrentUserId", newUser.UserId);
            HttpContext.Session.SetString("CurrentUserName", newUser.Username);
            HttpContext.Session.SetInt32("CurrentUserRoleId", newUser.RoleId);
            HttpContext.Session.SetString("CurrentUserAddress", newUser.Address);

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
            HttpContext.Session.SetString("CurrentUserAddress", user.Address);


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
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            // Xóa session nếu cần
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }

}


