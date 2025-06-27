using System.Collections.Generic;
using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public interface IPetRepository
    {
        List<Pet> GetPetsByUserId(int userId);
        void AddPet(Pet pet);
    }
}