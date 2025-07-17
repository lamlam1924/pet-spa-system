using pet_spa_system1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pet_spa_system1.Services
{
    public interface IPetService
    {
        Task<Pet> GetPetByIdAsync(int id);
        Task<List<Pet>> GetAllPetsAsync(int page, int pageSize);
        Task<List<Species>> GetAllSpeciesAsync();
        Task<List<Pet>> GetActivePetsAsync(int page, int pageSize);
        Task<int> CountActivePetsAsync();
        Task CreatePetAsync(Pet pet, List<IFormFile> images = null);
        Task UpdatePetAsync(Pet pet, List<IFormFile> images = null);
        Task DeletePetAsync(int id);
        Task DisablePetAsync(int id);
        bool PetExists(int id);
        Task<int> GetTotalPetCountAsync();
        Task<(Pet Pet, List<Pet> SuggestedPets)> GetPetDetailWithSuggestionsAsync(int petId);
        List<Pet> GetPetsByUserId(int userId);
        Task<List<Pet>> GetSuggestedPetsAsync(int speciesId, int excludePetId, int count);
        Task<List<PetImage>> GetPetImagesAsync(int petId);
        Task AddPetImageAsync(PetImage petImage);
        Task DeletePetImageAsync(int imageId);
        List<Pet> GetAllPets();
    }
}