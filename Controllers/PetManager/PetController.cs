using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;

namespace pet_spa_system1.Controllers
{
    public class PetsController : Controller
    {
        private readonly IPetService _petService;

        public PetsController(IPetService petService)
        {
            _petService = petService;
        }

        #region Index
        public async Task<IActionResult> Index(int page = 1)
        {
            #region GetPetsList
            Console.WriteLine($"[PetsController] Index called, page: {page}");
            const int pageSize = 10;
            var pets = await _petService.GetActivePetsAsync(page, pageSize);
            var totalPets = await _petService.CountActivePetsAsync();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalPets / pageSize);
            ViewBag.CurrentPage = page;

            Console.WriteLine($"[PetsController] Retrieved {pets.Count} pets for page {page}");
            return View("~/Views/Admin/Index.cshtml", pets); // Sử dụng view trong Views/Admin/
            #endregion
        }
        #endregion
    }
}