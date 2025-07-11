using pet_spa_system1.Models;

namespace pet_spa_system1.Services;

public interface IUserService
{
    Task<User?> GetUserByEmail(string email);
    Task<User?> AuthenticateAsync(string email, string password);
    Task<User?> RegisterAsync(string email, string userName, string plainPassword);
    Task<User?> RegisterByGoogle(string email, string name);
    
    User? GetUserInfo(int userId);

    Task<List<User>> GetActiveUsersAsync(string? search = null, string? sort = null);
    Task<List<User>> GetDeletedUsersAsync();
    Task<List<Role>> GetActiveRolesAsync();
    Task<(bool Success, string? Message, string? DefaultPassword)> CreateUserAsync(User user);
    Task<(bool Success, string? Message)> EditUserAsync(User updated);
    Task<(bool Success, string? Message)> DeleteUserAsync(int id);
    Task<(bool Success, string? Message)> RestoreUserAsync(int id);
    Task<(bool Success, string? Message, string? NewPassword)> ResetPasswordAsync(int id);
    // Staff management
    Task<List<User>> GetStaffListAsync(string? search = null, string? sort = null);
    Task<User?> GetStaffDetailAsync(int id);
    Task<(bool Success, string? Message)> ToggleLockStaffAsync(int id);
    Task<object> GetStaffStatsAsync(int id);
    Task<(bool Success, string? Message)> SetUserActiveAsync(int id, bool isActive);
    // User detail
    Task<User?> GetUserByIdAsync(int userId);
    Task<List<Pet>> GetPetsByUserIdAsync(int userId);
    Task<List<Appointment>> GetAppointmentsByUserIdAsync(int userId);
    Task<List<Order>> GetOrdersByUserIdAsync(int userId);
    Task<List<Review>> GetReviewsByUserIdAsync(int userId);
    Task<List<Payment>> GetPaymentsByUserIdAsync(int userId);
    Task<List<Appointment>> GetAppointmentsByStaffIdAsync(int staffId);
    Task<StaffPerformanceStats> GetStaffPerformanceStatsAsync(int staffId);
    Task<List<StaffDocument>> GetDocumentsByStaffIdAsync(int staffId);
    Task AddStaffDocumentAsync(StaffDocument doc);
    Task<string> ResetStaffPasswordAsync(int staffId);
    Task<string> UploadAvatarAsync(Microsoft.AspNetCore.Http.IFormFile avatarFile);
}