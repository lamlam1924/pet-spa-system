using System.Collections.Generic;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;

namespace pet_spa_system1.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;

        public PetService(IPetRepository petRepository)
        {
            _petRepository = petRepository;
        }

        public List<Pet> GetAllPets()
        {
            return _petRepository.GetAllPets();
        }

        public List<Pet> GetPetsByUserId(int userId)
        {
            return _petRepository.GetPetsByUserId(userId);
        }

        public Pet GetPetById(int id)
        {
            return _petRepository.GetPetById(id);
        }

        public void AddPet(Pet pet)
        {
            _petRepository.AddPet(pet);
        }

        public void UpdatePet(Pet pet)
        {
            _petRepository.UpdatePet(pet);
        }

        public void SoftDeletePet(int id)
        {
            _petRepository.SoftDeletePet(id);
        }

        public void RestorePet(int id)
        {
            _petRepository.RestorePet(id);
        }

        public void Save()
        {
            _petRepository.Save();
        }
    }
}