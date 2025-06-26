using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using System.Linq;

namespace pet_spa_system1.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly PetDataShopContext _context;

        public ServiceRepository(PetDataShopContext context)
        {
            _context = context;
        }

        public ServiceViewModel GetAllServiceAdmin()
        {
            return new ServiceViewModel
            {
                Services = _context.Services
                    .Include(s => s.Category)
                    .Where(s => s.IsActive)
                    .ToList(),
                Categories = _context.SerCates
                    .Where(sc => sc.IsActive)
                    .ToList(),
                Promotions = _context.Promotions
                    .Where(p => p.IsActive && p.ApplicableTo != "Product")
                    .ToList(),
                PromotionServices = _context.PromotionServices
                    .Include(ps => ps.Promotion)
                    .Include(ps => ps.Service)
                    .ToList(),
                Appointments = _context.Appointments
                    .Include(a => a.User)
                    .Include(a => a.Employee)
                    .Include(a => a.Status)
                    .Include(a => a.Promotion)
                    .Where(a => a.IsActive)
                    .ToList(),
                AppointmentServices = _context.AppointmentServices
                    .Include(aps => aps.Appointment)
                    .Include(aps => aps.Service)
                    .Where(aps => aps.IsActive)
                    .ToList()
            };
        }
        public ServiceViewModel GetAllService()
        {
            return new ServiceViewModel
            {
                Services = _context.Services
                    .Include(s => s.Category)
                    // .Where(s => s.IsActive)
                    .ToList(),
                Categories = _context.SerCates
                    // .Where(sc => sc.IsActive)
                    .ToList(),
                Promotions = _context.Promotions
                    .Where(p => p.IsActive && p.ApplicableTo != "Product")
                    .ToList(),
                PromotionServices = _context.PromotionServices
                    .Include(ps => ps.Promotion)
                    .Include(ps => ps.Service)
                    .ToList(),
                Appointments = _context.Appointments
                    .Include(a => a.User)
                    .Include(a => a.Employee)
                    .Include(a => a.Status)
                    .Include(a => a.Promotion)
                    .Where(a => a.IsActive)
                    .ToList(),
                AppointmentServices = _context.AppointmentServices
                    .Include(aps => aps.Appointment)
                    .Include(aps => aps.Service)
                    .Where(aps => aps.IsActive)
                    .ToList()
            };
        }

        public Service GetServiceById(int id)
        {
            return _context.Services.FirstOrDefault(s => s.ServiceId == id);
        }

        public void AddService(Service service)
        {
            _context.Services.Add(service);
        }

        public void UpdateService(Service service)
        {
            _context.Services.Update(service);
        }

        public void SoftDeleteService(int id)
        {
            var service = GetServiceById(id);
            if (service != null)
            {
                service.IsActive = false;
                UpdateService(service);
            }
        }

        public void RestoreService(int id)
        {
            var service = GetServiceById(id);
            if (service != null)
            {
                service.IsActive = true;
                UpdateService(service);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public List<Service> GetActiveServices() => _context.Services.Where(s => s.IsActive).ToList();
        public List<SerCate> GetActiveCategories() => _context.SerCates.Where(c => c.IsActive).ToList();
    }
}
