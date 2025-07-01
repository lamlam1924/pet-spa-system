using System.Collections.Generic;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;

namespace pet_spa_system1.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepo;
        public PetService(IPetRepository repo) { _petRepo = repo; }
        public List<Pet> GetPetsByUserId(int userId) => _petRepo.GetPetsByUserId(userId);
        public void AddPet(Pet pet) => _petRepo.AddPet(pet);
    }
}