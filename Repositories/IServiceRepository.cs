using System.Collections.Generic;
using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public interface IServiceRepository
    {
        // ===== SERVICE CRUD OPERATIONS =====
        Service? GetServiceById(int id);
        IEnumerable<Service> GetAll();
        IEnumerable<Service> GetActiveServices();
        void AddService(Service service);
        void UpdateService(Service service);
        void DeleteService(Service service);
        void SoftDeleteService(int id);
        void RestoreService(int id);

        // ===== SERVICE BUSINESS CHECKS =====
        bool ServiceHasAppointments(int serviceId);

        void Save();

        public IEnumerable<Service> GetRecentServices(int count);
    }
}
