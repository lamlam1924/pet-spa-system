using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public interface ISpeciesRepository
    {
        List<Species> GetAllActiveSpecies();
        Species GetSpeciesById(int id);
        void AddSpecies(Species species);
        void UpdateSpecies(Species species);
        void DeleteSpecies(int id); // Soft delete: cập nhật IsActive = false
        string GetSpeciesNameById(int id); // ✅ Hàm mới
    }
}
