using System.Collections.Generic;
using pet_spa_system1.Models;

namespace pet_spa_system1.Services
{
    public interface ICategoryService
    {
        List<SerCate> GetAllCategories();
        List<SerCate> GetActiveCategories();
        SerCate GetCategoryById(int id);
        void AddCategory(SerCate category);
        void UpdateCategory(SerCate category);
        void SoftDeleteCategory(int id);
        void RestoreCategory(int id);
        void Save();
    }
}