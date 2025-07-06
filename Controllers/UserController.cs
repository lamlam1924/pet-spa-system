using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using pet_spa_system1.Services;

namespace pet_spa_system1.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> List(string search = "", string sort = "")
        {
            var users = await _userService.GetActiveUsersAsync(search, sort);
            return Json(users.Select(u => new
            {
                u.UserId,
                u.Username,
                u.Email,
                u.FullName,
                u.Phone,
                u.Address,
                Role = u.Role != null ? u.Role.RoleName : "N/A",
                RoleId = u.RoleId,
                Status = u.IsActive == true ? "On" : "Off",
                CreatedAt = u.CreatedAt.HasValue ? u.CreatedAt.Value.ToString("yyyy-MM-dd") : null
            }));
        }

        [HttpGet]
        [Route("User/GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _userService.GetActiveRolesAsync();
                return Json(roles.Select(r => new { r.RoleId, r.RoleName, r.Description }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching roles: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            var result = await _userService.CreateUserAsync(user);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(new { message = result.Message, defaultPassword = result.DefaultPassword });
        }

        [HttpPost]
        [Route("User/Edit")]
        public async Task<IActionResult> Edit([FromBody] User updated)
        {
            var result = await _userService.EditUserAsync(updated);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(new { message = result.Message });
        }

        [HttpPost]
        [Route("User/Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.Success)
                return NotFound(result.Message);
            return Ok(new { message = result.Message });
        }

        [HttpPost]
        [Route("User/Restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _userService.RestoreUserAsync(id);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(new { message = result.Message });
        }

        [HttpGet]
        [Route("User/Deleted")]
        public async Task<IActionResult> GetDeletedUsers()
        {
            try
            {
                var deletedUsers = await _userService.GetDeletedUsersAsync();
                var result = deletedUsers.Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.Email,
                    u.FullName,
                    u.Phone,
                    u.Address,
                    Role = u.Role != null ? u.Role.RoleName : "N/A",
                    RoleActive = u.Role != null ? u.Role.IsActive : false,
                    CreatedAt = u.CreatedAt.HasValue ? u.CreatedAt.Value.ToString("yyyy-MM-dd") : null,
                    UpdatedAt = u.UpdatedAt.HasValue ? u.UpdatedAt.Value.ToString("yyyy-MM-dd") : null
                }).OrderByDescending(u => u.UpdatedAt).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching deleted users: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("User/ResetPassword")]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var result = await _userService.ResetPasswordAsync(id);
            if (!result.Success)
                return NotFound(result.Message);
            return Ok(new { message = result.Message, newPassword = result.NewPassword });
        }
    }
}