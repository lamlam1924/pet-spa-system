using pet_spa_system1.Models;
using pet_spa_system1.ViewModels;

namespace pet_spa_system1.Services
{
    public interface IServiceService
    {
        ServiceViewModel GetAllService();
        Service GetServiceById(int id);
        void AddService(Service service);
        void UpdateService(Service service);
        void SoftDeleteService(int id);
        void RestoreService(int id);
        void Save();
        public List<Service> GetActiveServices();
        List<Service> GetAll();
        
    }
}
