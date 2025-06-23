using pet_spa_system1.Models;
using System.Collections.Generic;

namespace pet_spa_system1.Repositories
{
    public interface IServiceRepository
    {
        ServicesViewModel GetAllService();
        Service GetServiceById(int id);
        void AddService(Service service);
        void UpdateService(Service service);
        void SoftDeleteService(int id);
        void RestoreService(int id);
        void Save();
        public List<Service> GetActiveServices();
        public List<SerCate> GetActiveCategories();
    }
}
