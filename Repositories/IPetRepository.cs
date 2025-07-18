using System.Collections.Generic;
using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public interface IPetRepository
    {
        List<Pet> GetPetsByUserId(int userId);
        List<Pet> GetAllPets();
        void AddPet(Pet pet);
        Pet GetById(int id);

        /// <summary>
        /// Lấy tất cả thú cưng kèm thông tin loài (Species)
        /// </summary>
        /// <returns>Danh sách Pet có include Species</returns>
        IEnumerable<Pet> GetAllPetsWithSpecies();
    }
}