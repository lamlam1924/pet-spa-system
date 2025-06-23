using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using System.Globalization;

namespace pet_spa_system1.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        
        

        public UserService(UserRepository userRepository, IPasswordHasher<User> passwordHasher)
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
            Console.WriteLine($"Email in service: {email}, Password: {password}");
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }
        public async Task<User?> RegisterAsync(string email,string userName ,string plainPassword)
        {
            Console.WriteLine("[DEBUG] passwordHasher null? " + (_passwordHasher == null));
            Console.WriteLine("password" + plainPassword);
            // Kiểm tra xem email đã tồn tại chưa
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                Console.WriteLine("emaill existed!");
                return null; // hoặc throw exception
            }    

            // Tạo user mới
            var newUser = new User
            {
                Email = email,
                Username = userName
            };

            // Băm mật khẩu và lưu
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, plainPassword);
            var role = await _userRepository.GetRoleByIdAsync(2);
            newUser.Role = role;
            Console.WriteLine("register success!");
            // Lưu user vào database
            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            
            return await AuthenticateAsync(email,plainPassword);
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
    }
}
