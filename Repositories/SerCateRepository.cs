using System.Collections.Generic;
using System.Linq;
using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public class SerCateRepository : ISerCateRepository
    {
        private readonly PetDataShopContext _context;

        public SerCateRepository(PetDataShopContext context)
        {
            _context = context;
        }

        // ===== BASIC CRUD =====
        public SerCate? GetById(int id) => _context.SerCates.Find(id);
        public IEnumerable<SerCate> GetAll() => _context.SerCates.ToList();
        public List<SerCate> GetAllAsList() => _context.SerCates.ToList(); // Cần thiết cho SerCateService
        public SerCate GetByIdNotNull(int id) => _context.SerCates.Find(id) ?? new SerCate(); // Cần thiết cho SerCateService
        public List<SerCate> GetActiveCategories() => _context.SerCates.Where(c => c.IsActive == true).ToList(); // Cần thiết cho SerCateService
        public void Add(SerCate category) => _context.SerCates.Add(category);
        public void Update(SerCate category) => _context.SerCates.Update(category);

        public bool DeleteById(int id)
        {
            try
            {
                var category = _context.SerCates.Find(id);
                if (category != null)
                {
                    _context.SerCates.Remove(category);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // ===== BUSINESS CHECKS =====
        public bool HasServices(int categoryId)
            => _context.Services.Any(s => s.CategoryId == categoryId);

        public Dictionary<int, int> GetServiceCountsByCategory()
            => _context.Services.GroupBy(s => s.CategoryId).ToDictionary(g => g.Key, g => g.Count());

        // ===== SOFT DELETE OPERATIONS =====
        public void SoftDelete(int id)
        {
            var category = _context.SerCates.Find(id);
            if (category != null)
            {
                category.IsActive = false;
                _context.SerCates.Update(category);
            }
        }

        public void Restore(int id)
        {
            var category = _context.SerCates.Find(id);
            if (category != null)
            {
                category.IsActive = true;
                _context.SerCates.Update(category);
            }
        }

        public void Save() => _context.SaveChanges();
    }
}