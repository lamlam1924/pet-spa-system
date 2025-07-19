using pet_spa_system1.Models;
using pet_spa_system1.Repositories;

namespace pet_spa_system1.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _repository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly PetDataShopContext _context; // Inject PetDataShopContext

        public PetService(IPetRepository repository, ICloudinaryService cloudinaryService, PetDataShopContext context)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _cloudinaryService = cloudinaryService ?? throw new ArgumentNullException(nameof(cloudinaryService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
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

        public async Task CreatePetAsync(Pet pet, List<IFormFile> images = null)
        {
            Console.WriteLine($"[PetService] CreatePetAsync called, Pet: {pet.PetId}, Name: {pet.Name}");
            pet.CreatedAt = DateTime.Now;
            pet.IsActive = true;
            await _repository.AddPetAsync(pet);

            if (images != null && images.Any())
            {
                int displayOrder = 0;
                foreach (var image in images)
                {
                    if (image.Length > 0)
                    {
                        var imageUrl = await _cloudinaryService.UploadImageAsync(image, "pets");
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            await AddPetImageAsync(new PetImage
                            {
                                PetId = pet.PetId,
                                ImageUrl = imageUrl,
                                DisplayOrder = displayOrder++,
                                CreatedAt = DateTime.Now
                            });
                        }
                    }
                }
            }
        }

        public async Task UpdatePetAsync(Pet pet, List<IFormFile> images = null)
        {
            Console.WriteLine($"[PetService] UpdatePetAsync called, Pet: {pet.PetId}, Name: {pet.Name}");
            pet.CreatedAt = DateTime.Now; // Cập nhật thời gian, tùy chỉnh nếu cần
            await _repository.UpdatePetAsync(pet);

            if (images != null && images.Any())
            {
                int displayOrder = (await _repository.GetPetImagesAsync(pet.PetId)).Count;
                foreach (var image in images)
                {
                    if (image.Length > 0)
                    {
                        var imageUrl = await _cloudinaryService.UploadImageAsync(image, "pets");
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            await AddPetImageAsync(new PetImage
                            {
                                PetId = pet.PetId,
                                ImageUrl = imageUrl,
                                DisplayOrder = displayOrder++,
                                CreatedAt = DateTime.Now
                            });
                        }
                    }
                }
            }
        }

        public async Task DeletePetAsync(int id)
        {
            Console.WriteLine($"[PetService] DeletePetAsync called for id: {id}");
            await _repository.DeletePetAsync(id);
        }

        public async Task DisablePetAsync(int id)
        {
            Console.WriteLine($"[PetService] DisablePetAsync called for id: {id}");
            var pet = await _repository.GetPetByIdAsync(id);
            if (pet != null)
            {
                pet.IsActive = false;
                await _repository.UpdatePetAsync(pet);
            }
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

            var suggestedPets = await _repository.GetSuggestedPetsAsync(pet.SpeciesId ?? 0, pet.PetId, 3);
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

        public async Task<List<PetImage>> GetPetImagesAsync(int petId)
        {
            Console.WriteLine($"[PetService] GetPetImagesAsync called for petId: {petId}");
            return await _repository.GetPetImagesAsync(petId);
        }

        public async Task AddPetImageAsync(PetImage petImage)
        {
            Console.WriteLine($"[PetService] AddPetImageAsync called for petId: {petImage.PetId}, ImageUrl: {petImage.ImageUrl}");
            await _repository.AddPetImageAsync(petImage);
        }

        public async Task DeletePetImageAsync(int imageId)
        {
            Console.WriteLine($"[PetService] DeletePetImageAsync called for imageId: {imageId}");
            var petImage = await _context.PetImages.FindAsync(imageId); // Lấy bản ghi cụ thể bằng imageId
            if (petImage != null)
            {
                // Xóa ảnh trên Cloudinary
                var deleteResult = await _cloudinaryService.DeleteImageAsync(petImage.ImageUrl);
                if (deleteResult)
                {
                    Console.WriteLine($"[PetService] Image deleted from Cloudinary, ImageUrl: {petImage.ImageUrl}");
                    // Xóa bản ghi trong DB
                    await _repository.DeletePetImageAsync(imageId);
                    Console.WriteLine("[PetService] DeletePetImageAsync completed.");
                }
                else
                {
                    Console.WriteLine("[PetService] Failed to delete image from Cloudinary.");
                }
            }
            else
            {
                Console.WriteLine("[PetService] DeletePetImageAsync: Image with id {imageId} not found.");
            }
        }
    }
}