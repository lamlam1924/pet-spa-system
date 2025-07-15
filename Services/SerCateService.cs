using System.Collections.Generic;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;

namespace pet_spa_system1.Services
{
    public class SerCateService : ISerCateService
    {
        private readonly ISerCateRepository _categoryRepository;

        public SerCateService(ISerCateRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // ===== FIX METHOD CALLS =====
        public List<SerCate> GetAll() => _categoryRepository.GetAllAsList(); // ? Fix
        public SerCate GetById(int id) => _categoryRepository.GetByIdNotNull(id); // ? Fix
        public void Add(SerCate category) => _categoryRepository.Add(category);
        public void Update(SerCate category) => _categoryRepository.Update(category);
        public void SoftDelete(int id) => _categoryRepository.SoftDelete(id); // ? Now available
        public void Restore(int id) => _categoryRepository.Restore(id); // ? Now available
        public void Save() => _categoryRepository.Save();
        public List<SerCate> GetActiveCategories() => _categoryRepository.GetActiveCategories(); // ? Now available
    }
}