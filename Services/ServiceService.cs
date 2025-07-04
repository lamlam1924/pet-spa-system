using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using System.Collections.Generic;
using pet_spa_system1.ViewModels;

namespace pet_spa_system1.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceService(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public void AddService(Service service)
        {
            _serviceRepository.AddService(service);
        }

        public List<Service> GetActiveServices()
        {
            return _serviceRepository.GetActiveServices();
        }

        public ServiceViewModel GetAllService()
        {
            return _serviceRepository.GetAllService();
        }

        public Service GetServiceById(int id)
        {
            return _serviceRepository.GetServiceById(id);
        }

        public void RestoreService(int id)
        {
            _serviceRepository.RestoreService(id);
        }

        public void Save()
        {
            _serviceRepository.Save();
        }

        public void SoftDeleteService(int id)
        {
            _serviceRepository.SoftDeleteService(id);
        }

        public void UpdateService(Service service)
        {
            _serviceRepository.UpdateService(service);
        }

        public List<Service> GetAll()
        {
            return _serviceRepository.GetAll();
        }
    }

}