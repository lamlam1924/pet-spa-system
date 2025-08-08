using System.Collections.Generic;
using System.Threading.Tasks;
using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories;

public interface IUserRepository
{
    User FindById(int userId);
    Task<User?> GetByEmailAsync(string email);
    Task SaveChangesAsync();
    Task<Role?> GetRoleByIdAsync(int roleId);
    User? GetUserById(int userId);
    Task AddAsync(User user);
    // Task<List<User>> GetActiveUsersAsync(string? search = null, string? sort = null);
    Task<List<User>> GetActiveUsersAsync(string? search = null, string? sort = null, int page = 1, int pageSize = 10);
    Task<List<User>> GetDeletedUsersAsync();
    Task<List<Role>> GetActiveRolesAsync();

    Task UpdateAsync(User user);
    Task<User?> GetByIdAsync(int userId);
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
    List<pet_spa_system1.ViewModel.StaffViewModel> GetStaffList();
    List<object> GetStaffResources();
    List<User> GetUsersByIdsOrdered(List<int> userIds);

}

