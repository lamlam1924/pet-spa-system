using pet_spa_system1.Models;
using pet_spa_system1.Repositories;

namespace pet_spa_system1.Services
{
    public class SpeciesService : ISpeciesService
    {
        private readonly ISpeciesRepository _speciesRepo;

        public SpeciesService(ISpeciesRepository speciesRepo)
        {
            _speciesRepo = speciesRepo;
        }

        public List<Species> GetAllActiveSpecies()
            => _speciesRepo.GetAllActiveSpecies();

        public Species GetSpeciesById(int id)
            => _speciesRepo.GetSpeciesById(id);

        public string GetSpeciesNameById(int id)
            => _speciesRepo.GetSpeciesNameById(id);

        public void AddSpecies(Species species)
            => _speciesRepo.AddSpecies(species);

        public void UpdateSpecies(Species species)
            => _speciesRepo.UpdateSpecies(species);

        public void DeleteSpecies(int id)
            => _speciesRepo.DeleteSpecies(id);
    }
}
