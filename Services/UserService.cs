using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using System.Globalization;

namespace pet_spa_system1.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return null;
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }

        public async Task<User?> RegisterAsync(string email, string userName, string plainPassword)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                return null;
            var newUser = new User
            {
                Email = email,
                Username = userName
            };
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, plainPassword);
            var role = await _userRepository.GetRoleByIdAsync(2);
            newUser.Role = role;
            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();
            return await AuthenticateAsync(email, plainPassword);
        }

        public async Task<User?> RegisterByGoogle(string email, string name)
        {
            var newUser = new User
            {
                Email = email,
                Username = name
            };
            var role = await _userRepository.GetRoleByIdAsync(2);
            newUser.Role = role;
            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();
            return await GetUserByEmail(email);
        }

        public User? GetUserInfo(int userId) => _userRepository.GetUserById(userId);
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        // Lấy danh sách user active
        public async Task<List<User>> GetActiveUsersAsync(string? search = null, string? sort = null)
        {
            return await _userRepository.GetActiveUsersAsync(search, sort);
        }

        // Lấy danh sách user đã xóa mềm
        public async Task<List<User>> GetDeletedUsersAsync()
        {
            return await _userRepository.GetDeletedUsersAsync();
        }

        // Lấy danh sách role active
        public async Task<List<Role>> GetActiveRolesAsync()
        {
            return await _userRepository.GetActiveRolesAsync();
        }

        // Tạo user mới
        public async Task<(bool Success, string? Message, string? DefaultPassword)> CreateUserAsync(User user)
        {
            if (user == null)
                return (false, "Invalid user data", null);
            if (string.IsNullOrWhiteSpace(user.Username))
                return (false, "Username is required", null);
            if (string.IsNullOrWhiteSpace(user.Email))
                return (false, "Email is required", null);
            if (string.IsNullOrWhiteSpace(user.FullName))
                return (false, "Full name is required", null);
            if (user.RoleId <= 0)
                return (false, "Role is required", null);
            var role = await _userRepository.GetRoleByIdAsync(user.RoleId);
            if (role == null || role.IsActive != true)
                return (false, "Selected role is not available", null);
            var activeUsers = await _userRepository.GetActiveUsersAsync();
            if (activeUsers.Any(u => u.Username == user.Username))
                return (false, "Username already exists", null);
            if (activeUsers.Any(u => u.Email == user.Email))
                return (false, "Email already exists", null);
            var defaultPassword = "123456";
            user.PasswordHash = _passwordHasher.HashPassword(user, defaultPassword);
            user.IsActive = true;
            user.CreatedAt = DateTime.Now;
            user.UserId = 0;
            await _userRepository.AddAsync(user);
            return (true, "User created successfully", defaultPassword);
        }

        // Sửa user
        public async Task<(bool Success, string? Message)> EditUserAsync(User updated)
        {
            if (updated == null)
                return (false, "Invalid data");
            if (updated.UserId == 0)
                return (false, "User ID is required for editing");
            var user = await _userRepository.GetByIdAsync(updated.UserId);
            if (user == null || user.IsActive != true)
                return (false, "User not found or inactive");
            var role = await _userRepository.GetRoleByIdAsync(updated.RoleId);
            if (role == null || role.IsActive != true)
                return (false, "Selected role is not available");
            var activeUsers = await _userRepository.GetActiveUsersAsync();
            if (activeUsers.Any(u => u.Username == updated.Username && u.UserId != updated.UserId))
                return (false, "Username already exists");
            if (activeUsers.Any(u => u.Email == updated.Email && u.UserId != updated.UserId))
                return (false, "Email already exists");
            user.Username = updated.Username;
            user.Email = updated.Email;
            user.FullName = updated.FullName;
            user.Phone = updated.Phone;
            user.Address = updated.Address;
            user.RoleId = updated.RoleId;
            user.UpdatedAt = DateTime.Now;
            await _userRepository.UpdateAsync(user);
            return (true, "User updated successfully");
        }

        // Xóa mềm user
        public async Task<(bool Success, string? Message)> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsActive != true)
                return (false, "User not found or already inactive");
            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;
            await _userRepository.UpdateAsync(user);
            return (true, "User deleted successfully (soft delete)");
        }

        // Khôi phục user đã xóa mềm
        public async Task<(bool Success, string? Message)> RestoreUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return (false, "User not found");
            var role = await _userRepository.GetRoleByIdAsync(user.RoleId);
            if (role == null || role.IsActive != true)
                return (false, "Cannot restore user: Associated role is inactive");
            var activeUsers = await _userRepository.GetActiveUsersAsync();
            if (activeUsers.Any(u => u.Username == user.Username && u.UserId != id))
                return (false, "Cannot restore: Username already exists in active users");
            if (activeUsers.Any(u => u.Email == user.Email && u.UserId != id))
                return (false, "Cannot restore: Email already exists in active users");
            user.IsActive = true;
            user.UpdatedAt = DateTime.Now;
            await _userRepository.UpdateAsync(user);
            return (true, "User restored successfully");
        }

        // Reset password
        public async Task<(bool Success, string? Message, string? NewPassword)> ResetPasswordAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsActive != true)
                return (false, "User not found or inactive", null);
            var defaultPassword = "123456";
            user.PasswordHash = _passwordHasher.HashPassword(user, defaultPassword);
            user.UpdatedAt = DateTime.Now;
            await _userRepository.UpdateAsync(user);
            return (true, "Password reset successfully", defaultPassword);
        }
    }
}