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

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public List<User> GetAllStaff()
    {
        return _context.Users
            .Where(u => u.RoleId == 2 && u.IsActive == true)
            .ToList();
    }
}