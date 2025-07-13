using System.Collections.Generic;
using pet_spa_system1.Models;

namespace pet_spa_system1.Services
{
    public interface IPetService
    {
        List<Pet> GetPetsByUserId(int userId);
        Pet GetPetById(int id);
        void AddPet(Pet pet);
        void DeletePet(int petId);
    }
}