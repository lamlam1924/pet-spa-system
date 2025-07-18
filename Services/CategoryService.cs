using System.Collections.Generic;
using pet_spa_system1.Models;
using pet_spa_system1.Repository;

namespace pet_spa_system1.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public List<SerCate> GetAllCategories()
        {
            return _categoryRepository.GetActiveCategories();
        }

        public List<SerCate> GetActiveCategories()
        {
            return _categoryRepository.GetActiveCategories();
        }

        public SerCate GetCategoryById(int id)
        {
            return _categoryRepository.GetCategoryById(id);
        }

        public void AddCategory(SerCate category)
        {
            _categoryRepository.AddCategory(category);
        }

        public void UpdateCategory(SerCate category)
        {
            _categoryRepository.UpdateCategory(category);
        }

        public void SoftDeleteCategory(int id)
        {
            _categoryRepository.SoftDeleteCategory(id);
        }

        public void RestoreCategory(int id)
        {
            _categoryRepository.RestoreCategory(id);
        }

        public void Save()
        {
            _categoryRepository.Save();
        }
    }
}