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

        public List<SerCate> GetAll() => _context.SerCates.ToList();

        public SerCate GetById(int id) => _context.SerCates.FirstOrDefault(c => c.CategoryId == id);

        public void Add(SerCate category)
        {
            _context.SerCates.Add(category);
        }

        public void Update(SerCate category)
        {
            _context.SerCates.Update(category);
        }

        public void SoftDelete(int id)
        {
            var cate = GetById(id);
            if (cate != null)
            {
                cate.IsActive = false;
                Update(cate);
            }
        }

        public void Restore(int id)
        {
            var cate = GetById(id);
            if (cate != null)
            {
                cate.IsActive = true;
                Update(cate);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public List<SerCate> GetActiveCategories() => _context.SerCates.Where(c => c.IsActive).ToList();
    }
}