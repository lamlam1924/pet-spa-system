using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace pet_spa_system1.Controllers
{
    public class UserController : Controller
    {
        private readonly PetDataShopContext _context;
        public UserController(PetDataShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult List(string search = "", string sort = "")
        {
            // Chỉ hiển thị user chưa bị xóa mềm và role active
            var users = _context.Users.Include(u => u.Role)
                .Where(u => u.IsActive == true && (u.Role == null || u.Role.IsActive == true)) // Chỉ lấy user active và role active
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.Username.Contains(search) || u.Email.Contains(search));
            }

            if (sort == "name")
                users = users.OrderBy(u => u.Username);
            else if (sort == "created")
                users = users.OrderByDescending(u => u.CreatedAt);

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

        // API để lấy danh sách roles active cho dropdown
        [HttpGet]
        [Route("User/GetRoles")]
        public IActionResult GetRoles()
        {
            try
            {
                var roles = _context.Roles
                    .Where(r => r.IsActive == true)
                    .Select(r => new
                    {
                        r.RoleId,
                        r.RoleName,
                        r.Description
                    })
                    .OrderBy(r => r.RoleName)
                    .ToList();

                return Json(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching roles: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] User user)
        {
            if (user == null)
                return BadRequest("Invalid user data");

            // Validation
            if (string.IsNullOrWhiteSpace(user.Username))
                return BadRequest("Username is required");
            if (string.IsNullOrWhiteSpace(user.Email))
                return BadRequest("Email is required");
            if (string.IsNullOrWhiteSpace(user.FullName))
                return BadRequest("Full name is required");
            if (user.RoleId <= 0)
                return BadRequest("Role is required");

            // Kiểm tra role có tồn tại và active không
            var role = _context.Roles.Find(user.RoleId);
            if (role == null || role.IsActive != true)
                return BadRequest("Selected role is not available");

            // Kiểm tra username và email đã tồn tại chưa (chỉ trong active users)
            if (_context.Users.Any(u => u.Username == user.Username && u.IsActive == true))
                return BadRequest("Username already exists");
            if (_context.Users.Any(u => u.Email == user.Email && u.IsActive == true))
                return BadRequest("Email already exists");

            // Tạo mật khẩu mặc định và hash
            var defaultPassword = "123456";
            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, defaultPassword);

            // Set default values
            user.IsActive = true;
            user.CreatedAt = DateTime.Now;
            user.UserId = 0; // Reset để EF tự generate

            _context.Users.Add(user);

            try
            {
                _context.SaveChanges();
                return Ok(new { message = "User created successfully", defaultPassword = defaultPassword });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving user: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("User/Edit")]
        public IActionResult Edit([FromBody] User updated)
        {
            if (updated == null)
                return BadRequest("Invalid data");

            if (updated.UserId == 0)
            {
                return BadRequest("User ID is required for editing");
            }

            var user = _context.Users.Find(updated.UserId);
            if (user == null || user.IsActive != true)
                return NotFound("User not found or inactive");

            // Kiểm tra role có tồn tại và active không
            var role = _context.Roles.Find(updated.RoleId);
            if (role == null || role.IsActive != true)
                return BadRequest("Selected role is not available");

            // Kiểm tra username và email trùng với user khác (chỉ active users)
            if (_context.Users.Any(u => u.Username == updated.Username && u.UserId != updated.UserId && u.IsActive == true))
                return BadRequest("Username already exists");
            if (_context.Users.Any(u => u.Email == updated.Email && u.UserId != updated.UserId && u.IsActive == true))
                return BadRequest("Email already exists");

            // Cập nhật thông tin (không cập nhật password)
            user.Username = updated.Username;
            user.Email = updated.Email;
            user.FullName = updated.FullName;
            user.Phone = updated.Phone;
            user.Address = updated.Address;
            user.RoleId = updated.RoleId;
            user.UpdatedAt = DateTime.Now;

            try
            {
                _context.SaveChanges();
                return Ok(new { message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("User/Delete")]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null || user.IsActive != true)
                return NotFound("User not found or already inactive");

            // Xóa mềm - chỉ set IsActive = false
            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;

            try
            {
                _context.SaveChanges();
                return Ok(new { message = "User deleted successfully (soft delete)" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        // Thêm method để khôi phục user đã xóa mềm
        [HttpPost]
        [Route("User/Restore")]
        public IActionResult Restore(int id)
        {
            var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.UserId == id);
            if (user == null)
                return NotFound("User not found");

            // Kiểm tra role của user có còn active không
            if (user.Role == null || user.Role.IsActive != true)
                return BadRequest("Cannot restore user: Associated role is inactive");

            // Kiểm tra username và email có bị trùng với active users không
            if (_context.Users.Any(u => u.Username == user.Username && u.UserId != id && u.IsActive == true))
                return BadRequest("Cannot restore: Username already exists in active users");
            if (_context.Users.Any(u => u.Email == user.Email && u.UserId != id && u.IsActive == true))
                return BadRequest("Cannot restore: Email already exists in active users");

            user.IsActive = true;
            user.UpdatedAt = DateTime.Now;

            try
            {
                _context.SaveChanges();
                return Ok(new { message = "User restored successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        // Method để xem danh sách user đã bị xóa mềm
        [HttpGet]
        [Route("User/Deleted")]
        public IActionResult GetDeletedUsers()
        {
            try
            {
                var deletedUsers = _context.Users
                    .Include(u => u.Role)
                    .Where(u => u.IsActive == false)
                    .ToList() // Lấy data về memory trước
                    .Select(u => new
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
                    })
                    .OrderByDescending(u => u.UpdatedAt)
                    .ToList();

                return Json(deletedUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching deleted users: {ex.Message}");
            }
        }

        // Method để reset password
        [HttpPost]
        [Route("User/ResetPassword")]
        public IActionResult ResetPassword(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null || user.IsActive != true)
                return NotFound("User not found or inactive");

            // Reset về mật khẩu mặc định
            var defaultPassword = "123456";
            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, defaultPassword);
            user.UpdatedAt = DateTime.Now;

            try
            {
                _context.SaveChanges();
                return Ok(new { message = "Password reset successfully", newPassword = defaultPassword });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }
    }
}