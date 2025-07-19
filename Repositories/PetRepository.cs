using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly PetDataShopContext _context;

        public PetRepository(PetDataShopContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Console.WriteLine("[PetRepository] Repository initialized. Context: " + (_context != null ? "OK" : "NULL"));
        }

        public async Task<List<Pet>> GetAllPetsAsync(int page, int pageSize)
        {
            Console.WriteLine($"[PetRepository] GetAllPetsAsync called, page: {page}, pageSize: {pageSize}, Connection State: {_context.Database.CanConnect()}");
            var query = _context.Pets
                .Include(p => p.User)
                .Include(p => p.Species)
                .Include(p => p.AppointmentPets)
                .Include(p => p.PetImages)
                .OrderBy(p => p.PetId);
            var result = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            Console.WriteLine($"[PetRepository] GetAllPetsAsync retrieved {result.Count} pets, IDs: {string.Join(", ", result.Select(p => p.PetId))}");
            return result;
        }

        public async Task<Pet> GetPetByIdAsync(int id)
        {
            Console.WriteLine($"[PetRepository] GetPetByIdAsync called for id: {id}, Connection State: {_context.Database.CanConnect()}");
            var pet = await _context.Pets
                .Include(p => p.User)
                .Include(p => p.Species)
                .Include(p => p.AppointmentPets)
                .Include(p => p.PetImages)
                .FirstOrDefaultAsync(p => p.PetId == id);
            if (pet == null)
            {
                Console.WriteLine($"[PetRepository] GetPetByIdAsync: Pet with id {id} not found.");
            }
            else
            {
                Console.WriteLine($"[PetRepository] GetPetByIdAsync: Found Pet - ID: {pet.PetId}, Name: {pet.Name}, UserId: {pet.UserId}, ImageUrls: {string.Join(", ", pet.PetImages.Select(pi => pi.ImageUrl))}");
            }
            return pet ?? throw new KeyNotFoundException("Pet not found.");
        }

        public async Task<List<Species>> GetAllSpeciesAsync()
        {
            Console.WriteLine("[PetRepository] GetAllSpeciesAsync called, Connection State: {_context.Database.CanConnect()}");
            var species = await _context.Species.ToListAsync();
            Console.WriteLine($"[PetRepository] GetAllSpeciesAsync retrieved {species.Count} species");
            return species;
        }

        public async Task<List<Pet>> GetActivePetsAsync(int page, int pageSize)
        {
            Console.WriteLine($"[PetRepository] GetActivePetsAsync called, page: {page}, pageSize: {pageSize}, Connection State: {_context.Database.CanConnect()}");
            var query = _context.Pets
                .Where(p => p.IsActive == true || p.IsActive == null)
                .Include(p => p.User)
                .Include(p => p.Species)
                .Include(p => p.AppointmentPets)
                .Include(p => p.PetImages)
                .OrderByDescending(p => p.CreatedAt);

            var count = await query.CountAsync();
            Console.WriteLine($"[PetRepository] Active Pets Count before pagination: {count}");

            var result = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Console.WriteLine($"[PetRepository] Retrieved Pets Count: {result.Count}, IDs: {string.Join(", ", result.Select(p => p.PetId))}");
            if (result.Count == 0)
            {
                Console.WriteLine("[PetRepository] Warning: No active pets retrieved, check IsActive or data in Pets table.");
            }
            return result;
        }

        public async Task<int> CountActivePetsAsync()
        {
            Console.WriteLine("[PetRepository] CountActivePetsAsync called, Connection State: {_context.Database.CanConnect()}");
            var count = await _context.Pets.CountAsync(p => p.IsActive == true || p.IsActive == null);
            Console.WriteLine($"[PetRepository] CountActivePetsAsync result: {count}");
            return count;
        }

        public async Task AddPetAsync(Pet pet)
        {
            Console.WriteLine("[PetRepository] AddPetAsync called, Connection State: {_context.Database.CanConnect()}");
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();
            Console.WriteLine("[PetRepository] AddPetAsync completed, PetId: {pet.PetId}");
        }

        public async Task UpdatePetAsync(Pet pet)
        {
            Console.WriteLine("[PetRepository] UpdatePetAsync called, Connection State: {_context.Database.CanConnect()}");
            _context.Pets.Update(pet);
            await _context.SaveChangesAsync();
            Console.WriteLine($"[PetRepository] UpdatePetAsync completed, PetId: {pet.PetId}");
        }

        public async Task DeletePetAsync(int id)
        {
            Console.WriteLine($"[PetRepository] DeletePetAsync called for id: {id}, Connection State: {_context.Database.CanConnect()}");
            var pet = await _context.Pets.FindAsync(id);
            if (pet != null)
            {
                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();
                Console.WriteLine($"[PetRepository] DeletePetAsync completed for id: {id}");
            }
            else
            {
                Console.WriteLine($"[PetRepository] DeletePetAsync: Pet with id {id} not found.");
            }
        }

        public bool PetExists(int id)
        {
            Console.WriteLine($"[PetRepository] PetExists called for id: {id}, Connection State: {_context.Database.CanConnect()}");
            var exists = _context.Pets.Any(e => e.PetId == id);
            Console.WriteLine($"[PetRepository] PetExists result for id {id}: {exists}");
            return exists;
        }

        public async Task<int> GetTotalPetCountAsync()
        {
            Console.WriteLine("[PetRepository] GetTotalPetCountAsync called, Connection State: {_context.Database.CanConnect()}");
            var count = await _context.Pets.CountAsync();
            Console.WriteLine($"[PetRepository] GetTotalPetCountAsync result: {count}");
            return count;
        }

        public async Task DisablePetAsync(int id)
        {
            Console.WriteLine($"[PetRepository] DisablePetAsync called for id: {id}, Connection State: {_context.Database.CanConnect()}");
            var pet = await _context.Pets.FindAsync(id);
            if (pet != null)
            {
                pet.IsActive = false;
                await _context.SaveChangesAsync();
                Console.WriteLine($"[PetRepository] DisablePetAsync completed for id: {id}");
            }
            else
            {
                Console.WriteLine($"[PetRepository] DisablePetAsync: Pet with id {id} not found.");
            }
        }

        public async Task<List<Pet>> GetSuggestedPetsAsync(int speciesId, int excludePetId, int count)
        {
            Console.WriteLine($"[PetRepository] GetSuggestedPetsAsync called, speciesId: {speciesId}, excludePetId: {excludePetId}, count: {count}, Connection State: {_context.Database.CanConnect()}");
            var result = await _context.Pets
                .Where(p => (p.SpeciesId == speciesId || speciesId == 0) && p.PetId != excludePetId && (p.IsActive ?? true))
                .Include(p => p.User)
                .Include(p => p.Species)
                .Include(p => p.AppointmentPets)
                .Include(p => p.PetImages)
                .OrderBy(p => Guid.NewGuid())
                .Take(count)
                .ToListAsync();
            Console.WriteLine($"[PetRepository] GetSuggestedPetsAsync retrieved {result.Count} pets, IDs: {string.Join(", ", result.Select(p => p.PetId))}");
            return result;
        }

        public List<Pet> GetPetsByUserId(int userId)
        {
            Console.WriteLine($"[PetRepository] GetPetsByUserId called for userId: {userId}, Connection State: {_context.Database.CanConnect()}");
            var pets = _context.Pets
                .Where(p => p.UserId == userId && (p.IsActive ?? true))
                .Include(p => p.Species)
                .Include(p => p.AppointmentPets)
                .Include(p => p.User)
                .Include(p => p.PetImages)
                .ToList();
            Console.WriteLine($"[PetRepository] GetPetsByUserId retrieved {pets.Count} pets, IDs: {string.Join(", ", pets.Select(p => p.PetId))}");
            return pets;
        }

        public async Task<List<PetImage>> GetPetImagesAsync(int petId)
        {
            Console.WriteLine($"[PetRepository] GetPetImagesAsync called for petId: {petId}, Connection State: {_context.Database.CanConnect()}");
            return await _context.PetImages
                .Where(pi => pi.PetId == petId)
                .OrderBy(pi => pi.DisplayOrder)
                .ToListAsync();
        }

        public async Task AddPetImageAsync(PetImage petImage)
        {
            Console.WriteLine($"[PetRepository] AddPetImageAsync called for petId: {petImage.PetId}, ImageUrl: {petImage.ImageUrl}, Connection State: {_context.Database.CanConnect()}");
            _context.PetImages.Add(petImage);
            await _context.SaveChangesAsync();
            Console.WriteLine("[PetRepository] AddPetImageAsync completed.");
        }

        public async Task DeletePetImageAsync(int imageId)
        {
            Console.WriteLine($"[PetRepository] DeletePetImageAsync called for imageId: {imageId}, Connection State: {_context.Database.CanConnect()}");
            var petImage = await _context.PetImages.FindAsync(imageId);
            if (petImage != null)
            {
                _context.PetImages.Remove(petImage);
                await _context.SaveChangesAsync();
                Console.WriteLine("[PetRepository] DeletePetImageAsync completed.");
            }
            else
            {
                Console.WriteLine("[PetRepository] DeletePetImageAsync: Image with id {imageId} not found.");
            }
        }

        public IEnumerable<Pet> GetAllPetsWithSpecies()
        {
            return _context.Pets.Include(p => p.Species);
        }
    }
}