using pet_spa_system1.Models;

namespace pet_spa_system1.Services
{
    public interface ISpeciesService
    {
        List<Species> GetAllActiveSpecies();
        Species GetSpeciesById(int id);
        string GetSpeciesNameById(int id);
        void AddSpecies(Species species);
        void UpdateSpecies(Species species);
        void DeleteSpecies(int id);
    }
}
