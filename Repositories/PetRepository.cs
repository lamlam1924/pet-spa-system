using System.Collections.Generic;
using System.Linq;
using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly PetDataShopContext _context;
        public PetRepository(PetDataShopContext context) { _context = context; }

        public List<Pet> GetPetsByUserId(int userId)
            => _context.Pets.Where(p => p.UserId == userId && p.IsActive == true).ToList();

        public void AddPet(Pet pet)
        {
            _context.Pets.Add(pet);
            _context.SaveChanges();
        }
    }

}