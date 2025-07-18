using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly PetDataShopContext _context;

    public ServiceRepository(PetDataShopContext context)
    {
        _context = context;
    }

    // ===== SERVICE CRUD =====
    public Service? GetServiceById(int id) => _context.Services.Find(id);
    public IEnumerable<Service> GetAll() => _context.Services.ToList();
    public IEnumerable<Service> GetActiveServices() => _context.Services.Where(s => s.IsActive == true).ToList();

    public void AddService(Service service) => _context.Services.Add(service);
    public void UpdateService(Service service) => _context.Services.Update(service);
    public void DeleteService(Service service) => _context.Services.Remove(service);

    public void SoftDeleteService(int id)
    {
        var service = _context.Services.Find(id);
        if (service != null)
        {
            service.IsActive = false;
            _context.Services.Update(service);
        }
    }

    public void RestoreService(int id)
    {
        var service = _context.Services.Find(id);
        if (service != null)
        {
            service.IsActive = true;
            _context.Services.Update(service);
        }
    }

    // ===== BUSINESS CHECKS =====
    public bool ServiceHasAppointments(int serviceId)
        => _context.AppointmentServices.Any(a => a.ServiceId == serviceId);

    public void Save() => _context.SaveChanges();

    public IEnumerable<Service> GetRecentServices(int count = 5)
    {
        return _context.Services
            .OrderByDescending(s => s.CreatedAt ?? DateTime.MinValue)
            .Take(count)
            .ToList();
    }
}
