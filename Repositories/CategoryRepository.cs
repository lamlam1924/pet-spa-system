using System.Collections.Generic;
using System.Linq;
using pet_spa_system1.Models;

namespace pet_spa_system1.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly PetDataShopContext _context;

        public CategoryRepository(PetDataShopContext context)
        {
            _context = context;
        }

        public List<SerCate> GetAllCategories()
        {
            return _context.SerCates.ToList();
        }

        public List<SerCate> GetActiveCategories()
        {
            return _context.SerCates.Where(c => c.IsActive).ToList();
        }

        public SerCate GetCategoryById(int id)
        {
            return _context.SerCates.FirstOrDefault(c => c.CategoryId == id);
        }

        public void AddCategory(SerCate category)
        {
            _context.SerCates.Add(category);
            Save();
        }

        public void UpdateCategory(SerCate category)
        {
            _context.SerCates.Update(category);
            Save();
        }

        public void SoftDeleteCategory(int id)
        {
            var category = GetCategoryById(id);
            if (category != null)
            {
                category.IsActive = false;
                Save();
            }
        }

        public void RestoreCategory(int id)
        {
            var category = GetCategoryById(id);
            if (category != null)
            {
                category.IsActive = true;
                Save();
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}