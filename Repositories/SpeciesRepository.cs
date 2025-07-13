using System.Collections.Generic;
using System.Linq;
using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public class SpeciesRepository : ISpeciesRepository
    {
        private readonly PetDataShopContext _context;

        public SpeciesRepository(PetDataShopContext context)
        {
            _context = context;
        }

        public List<Species> GetAllActiveSpecies()
        {
            return _context.Species
                .Where(s => s.IsActive == true)
                .OrderBy(s => s.SpeciesName)
                .ToList();
        }
        public string GetSpeciesNameById(int id)
        {
            return _context.Species
                .Where(s => s.SpeciesId == id && s.IsActive == true)
                .Select(s => s.SpeciesName)
                .FirstOrDefault();
        }

        public Species GetSpeciesById(int id)
        {
            return _context.Species.FirstOrDefault(s => s.SpeciesId == id && s.IsActive == true);
        }

        public void AddSpecies(Species species)
        {
            _context.Species.Add(species);
            _context.SaveChanges();
        }

        public void UpdateSpecies(Species species)
        {
            _context.Species.Update(species);
            _context.SaveChanges();
        }

        public void DeleteSpecies(int id)
        {
            var species = _context.Species.FirstOrDefault(s => s.SpeciesId == id);
            if (species != null)
            {
                species.IsActive = false; // soft delete
                _context.SaveChanges();
            }
        }
    }
}
