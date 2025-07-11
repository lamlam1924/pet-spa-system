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

    public async Task<List<User>> GetActiveUsersAsync(string? search = null, string? sort = null)
    {
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
        return await users.ToListAsync();
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
        return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);
    }
}