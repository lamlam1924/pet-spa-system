using System.Collections.Generic;
using System.Linq;
using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly PetDataShopContext _context;

        public PetRepository(PetDataShopContext context)
        {
            _context = context;
        }

        public List<Pet> GetAllPets()
        {
            return _context.Pets.ToList();
        }

        public List<Pet> GetPetsByUserId(int userId)
        {
            return _context.Pets.Where(p => p.UserId == userId).ToList();
        }

        public Pet GetPetById(int id)
        {
            return _context.Pets.FirstOrDefault(p => p.PetId == id);
        }

        public void AddPet(Pet pet)
        {
            _context.Pets.Add(pet);
            Save();
        }

        public void UpdatePet(Pet pet)
        {
            _context.Pets.Update(pet);
            Save();
        }

        public void SoftDeletePet(int id)
        {
            var pet = GetPetById(id);
            if (pet != null)
            {
                pet.IsActive = false;
                Save();
            }
        }

        public void RestorePet(int id)
        {
            var pet = GetPetById(id);
            if (pet != null)
            {
                pet.IsActive = true;
                Save();
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}