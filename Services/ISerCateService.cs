using System.Collections.Generic;
using pet_spa_system1.Models;

namespace pet_spa_system1.Services
{
    public interface ISerCateService
    {
        List<SerCate> GetAll();
        SerCate GetById(int id);
        void Add(SerCate category);
        void Update(SerCate category);
        void SoftDelete(int id);
        void Restore(int id);
        void Save();
        List<SerCate> GetActiveCategories();
    }
}