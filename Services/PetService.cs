using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pet_spa_system1.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _repository;

        public PetService(IPetRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Console.WriteLine("[PetService] Service initialized.");
        }

        public async Task<Pet> GetPetByIdAsync(int id)
        {
            Console.WriteLine($"[PetService] GetPetByIdAsync called for id: {id}");
            return await _repository.GetPetByIdAsync(id);
        }

        public async Task<List<Pet>> GetAllPetsAsync(int page, int pageSize)
        {
            Console.WriteLine($"[PetService] GetAllPetsAsync called, page: {page}, pageSize: {pageSize}");
            return await _repository.GetAllPetsAsync(page, pageSize);
        }

        public async Task<List<Species>> GetAllSpeciesAsync()
        {
            Console.WriteLine("[PetService] GetAllSpeciesAsync called");
            return await _repository.GetAllSpeciesAsync();
        }

        public async Task<List<Pet>> GetActivePetsAsync(int page, int pageSize)
        {
            Console.WriteLine($"[PetService] GetActivePetsAsync called, page: {page}, pageSize: {pageSize}");
            var pets = await _repository.GetActivePetsAsync(page, pageSize) ?? new List<Pet>();
            Console.WriteLine($"[PetService] GetActivePetsAsync retrieved {pets.Count} pets, IDs: {string.Join(", ", pets.Select(p => p.PetId))}");
            return pets;
        }

        public async Task<int> CountActivePetsAsync()
        {
            Console.WriteLine("[PetService] CountActivePetsAsync called");
            return await _repository.CountActivePetsAsync();
        }

        public async Task CreatePetAsync(Pet pet)
        {
            Console.WriteLine("[PetService] CreatePetAsync called");
            await _repository.AddPetAsync(pet);
        }

        public async Task UpdatePetAsync(Pet pet)
        {
            Console.WriteLine("[PetService] UpdatePetAsync called");
            await _repository.UpdatePetAsync(pet);
        }

        public async Task DeletePetAsync(int id)
        {
            Console.WriteLine($"[PetService] DeletePetAsync called for id: {id}");
            await _repository.DeletePetAsync(id);
        }

        public async Task DisablePetAsync(int id)
        {
            Console.WriteLine($"[PetService] DisablePetAsync called for id: {id}");
            await _repository.DisablePetAsync(id);
        }

        public bool PetExists(int id)
        {
            Console.WriteLine($"[PetService] PetExists called for id: {id}");
            return _repository.PetExists(id);
        }

        public async Task<int> GetTotalPetCountAsync()
        {
            Console.WriteLine("[PetService] GetTotalPetCountAsync called");
            return await _repository.GetTotalPetCountAsync();
        }

        public async Task<(Pet Pet, List<Pet> SuggestedPets)> GetPetDetailWithSuggestionsAsync(int petId)
        {
            Console.WriteLine($"[PetService] GetPetDetailWithSuggestionsAsync called for petId: {petId}");
            var pet = await _repository.GetPetByIdAsync(petId);
            if (pet == null)
            {
                Console.WriteLine("[PetService] Pet not found for petId: {petId}");
                return (null, new List<Pet>());
            }

            var suggestedPets = await _repository.GetSuggestedPetsAsync(pet.SpeciesId ?? 0, pet.PetId, 3); // Lấy 3 thú cưng gợi ý
            Console.WriteLine($"[PetService] Suggested Pets Count: {suggestedPets.Count}");
            return (pet, suggestedPets);
        }

        public List<Pet> GetPetsByUserId(int userId)
        {
            Console.WriteLine($"[PetService] GetPetsByUserId called for userId: {userId}");
            var pets = _repository.GetPetsByUserId(userId);
            Console.WriteLine($"[PetService] GetPetsByUserId retrieved {pets.Count} pets, IDs: {string.Join(", ", pets.Select(p => p.PetId))}");
            return pets;
        }

        public async Task<List<Pet>> GetSuggestedPetsAsync(int speciesId, int excludePetId, int count)
        {
            Console.WriteLine($"[PetService] GetSuggestedPetsAsync called, speciesId: {speciesId}, excludePetId: {excludePetId}, count: {count}");
            return await _repository.GetSuggestedPetsAsync(speciesId, excludePetId, count);
        }
    }
}