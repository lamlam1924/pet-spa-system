using System.Collections.Generic;
using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public interface ISerCateRepository
    {
        // ===== BASIC CRUD =====
        SerCate? GetById(int id);
        IEnumerable<SerCate> GetAll();
        List<SerCate> GetAllAsList(); // Cần thiết cho SerCateService
        SerCate GetByIdNotNull(int id); // Cần thiết cho SerCateService  
        List<SerCate> GetActiveCategories(); // Cần thiết cho SerCateService
        void Add(SerCate category);
        void Update(SerCate category);
        bool DeleteById(int id);
        
        // ===== BUSINESS CHECKS =====
        bool HasServices(int categoryId);
        Dictionary<int, int> GetServiceCountsByCategory();
        
        // ===== SOFT DELETE OPERATIONS =====
        void SoftDelete(int id);
        void Restore(int id);
        
        void Save();
    }
}