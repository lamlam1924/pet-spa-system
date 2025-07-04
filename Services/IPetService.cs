using System.Collections.Generic;
using pet_spa_system1.Models;

namespace pet_spa_system1.Services
{
    public interface IPetService
    {
        List<Pet> GetPetsByUserId(int userId);
        List<Pet> GetAllPets();
        void AddPet(Pet pet);
    }
}