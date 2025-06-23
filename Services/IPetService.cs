using System.Collections.Generic;
using pet_spa_system1.Models;

namespace pet_spa_system1.Services
{
    public interface IPetService
    {
        List<Pet> GetAllPets();
        List<Pet> GetPetsByUserId(int userId);
        Pet GetPetById(int id);
        void AddPet(Pet pet);
        void UpdatePet(Pet pet);
        void SoftDeletePet(int id);
        void RestorePet(int id);
        void Save();
    }
}