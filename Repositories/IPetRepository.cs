using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public interface IPetRepository
    {
        Task<List<Pet>> GetAllPetsAsync(int page, int pageSize);
        Task<Pet> GetPetByIdAsync(int id);
        Task<List<Species>> GetAllSpeciesAsync();
        Task<List<Pet>> GetActivePetsAsync(int page, int pageSize);
        Task<int> CountActivePetsAsync();
        Task AddPetAsync(Pet pet);
        Task UpdatePetAsync(Pet pet);
        Task DeletePetAsync(int id);
        bool PetExists(int id);
        Task<int> GetTotalPetCountAsync();
        Task DisablePetAsync(int id);
        Task<List<Pet>> GetSuggestedPetsAsync(int speciesId, int excludePetId, int count);
        List<Pet> GetPetsByUserId(int userId);
        Task<List<PetImage>> GetPetImagesAsync(int petId);
        Task AddPetImageAsync(PetImage petImage);
        Task DeletePetImageAsync(int imageId);
        IEnumerable<Pet> GetAllPetsWithSpecies();

    }
}