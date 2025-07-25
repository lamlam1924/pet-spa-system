        
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;


namespace pet_spa_system1.Repositories;

public class UserRepository : IUserRepository

{
    private readonly PetDataShopContext _context;

    public UserRepository(PetDataShopContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Role?> GetRoleByIdAsync(int roleId)
    {
        return await _context.Roles.FindAsync(roleId);
    }

    public User? GetUserById(int userId)
        => _context.Users.FirstOrDefault(u => u.UserId == userId);
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
                             .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<List<User>> GetActiveUsersAsync(string? search = null, string? sort = null, int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        var users = _context.Users.Include(u => u.Role)
            .Where(u => u.IsActive == true && (u.Role == null || u.Role.IsActive == true));
        if (!string.IsNullOrEmpty(search))
        {
            users = users.Where(u => u.Username.Contains(search) || u.Email.Contains(search));
        }
        if (sort == "name")
            users = users.OrderBy(u => u.Username);
        else if (sort == "created")
            users = users.OrderByDescending(u => u.CreatedAt);
        return await users.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<List<User>> GetDeletedUsersAsync()
    {
        return await _context.Users.Include(u => u.Role)
            .Where(u => u.IsActive == false)
            .ToListAsync();
    }

    public async Task<List<Role>> GetActiveRolesAsync()
    {
        return await _context.Roles
            .Where(r => r.IsActive == true)
            .OrderBy(r => r.RoleName)
            .ToListAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<List<Pet>> GetPetsByUserIdAsync(int userId)
    {
        return await _context.Pets.Where(p => p.UserId == userId).ToListAsync();
    }

    public async Task<List<Appointment>> GetAppointmentsByUserIdAsync(int userId)
    {
        return await _context.Appointments
            .Include(a => a.Status)
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
    {
        return await _context.Orders.Where(o => o.UserId == userId).ToListAsync();
    }

    public async Task<List<Review>> GetReviewsByUserIdAsync(int userId)
    {
        return await _context.Reviews.Where(r => r.UserId == userId).ToListAsync();
    }

    public async Task<List<Payment>> GetPaymentsByUserIdAsync(int userId)
    {
        return await _context.Payments.Where(p => p.UserId == userId).ToListAsync();
    }

    public async Task<List<Appointment>> GetAppointmentsByStaffIdAsync(int staffId)
    {
        return await _context.Appointments
            .Include(a => a.User)
            .Include(a => a.Status)
            .Where(a => a.EmployeeId == staffId)
            .ToListAsync();
    }

    // TODO: Tri?n khai h�m l?y hi?u su?t l�m vi?c
    public async Task<StaffPerformanceStats> GetStaffPerformanceStatsAsync(int staffId)
    {
        // T�nh to�n hi?u su?t t? b?ng Appointment, Order, ...
        return new StaffPerformanceStats();
    }

    // TODO: Tri?n khai h�m l?y t�i li?u c?a nh�n vi�n
    public async Task<List<StaffDocument>> GetDocumentsByStaffIdAsync(int staffId)
    {
        return new List<StaffDocument>();
    }

    // TODO: Tri?n khai h�m upload t�i li?u
    public async Task AddStaffDocumentAsync(StaffDocument doc)
    {
        // Th�m doc v�o DB
    }

    // TODO: Tri?n khai h�m reset m?t kh?u nh�n vi�n
    public async Task<string> ResetStaffPasswordAsync(int staffId)
    {
        // Sinh m?t kh?u m?i, c?p nh?t DB, tr? v? m?t kh?u m?i
        return "newpassword123";
    }
    
    
}