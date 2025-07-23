using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.Utils;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Controllers
{
    public class AdminController : Controller
    {
        private readonly PetDataShopContext _context;
        private readonly IProductService _productService;
        private readonly IServiceService _serviceService;
        private readonly IBlogService _blogService;
        private readonly IPetService _petService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;

        public AdminController(PetDataShopContext context, IProductService productService, IServiceService serviceService, IBlogService blogService, IPetService petService, IOrderService orderService, IPaymentService paymentService)
        {
            _context = context;
            _productService = productService;
            _serviceService = serviceService;
            _blogService = blogService;
            _petService = petService;
            _orderService = orderService;
            _paymentService = paymentService;
        }
        //=======================================================================================================================
        // SERVICE
        //public IActionResult ManageService(int? categoryId, string search)
        //{
        //    var model = _serviceService.GetAll();
        //    if (categoryId.HasValue)
        //    {
        //        model.Services = model.Services.Where(s => s.CategoryId == categoryId.Value).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        model.Services = model.Services.Where(s => s.Name.Contains(search)).ToList();
        //    }
        //    model.SelectedCategoryId = categoryId;
        //    return View(model);
        //}
        
        public IActionResult Index()
        {
            Console.WriteLine("[AdminController] Accessing Index...");
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                Console.WriteLine("[AdminController] User not authenticated, redirecting or allowing anonymous access.");
            }
            ViewBag.Title = "Admin Dashboard";
            return View();
        }

        public IActionResult Payment()
        {
            var payments = _paymentService.GetAllPayments();
            return View(payments);
        }

        public async Task<IActionResult> Pets_List(int page = 1, string searchName = "", string searchOwner = "", bool? isActive = null, string sortOrder = "name", string speciesName = "")
        {
            Console.WriteLine($"[AdminController] Pets_List called, page: {page}, searchName: {searchName}, searchOwner: {searchOwner}, isActive: {isActive}, sortOrder: {sortOrder}, speciesName: {speciesName}");
            const int pageSize = 10;

            IQueryable<Pet> query = _context.Pets
                .Include(p => p.User)
                .Include(p => p.Species)
                .Include(p => p.PetImages)
                .OrderBy(p => p.PetId);

            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(p => p.Name.Contains(searchName));
            }
            if (!string.IsNullOrEmpty(searchOwner))
            {
                query = query.Where(p => p.User.FullName != null && p.User.FullName.Contains(searchOwner));
            }
            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive);
            }
            if (!string.IsNullOrEmpty(speciesName))
            {
                query = query.Where(p => p.Species.SpeciesName == speciesName);
            }

            ViewBag.NameSortParam = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.OwnerSortParam = sortOrder == "owner" ? "owner_desc" : "owner";
            ViewBag.ActiveSortParam = sortOrder == "active" ? "active_desc" : "active";

            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(p => p.Name);
                    break;
                case "owner":
                    query = query.OrderBy(p => p.User.FullName);
                    break;
                case "owner_desc":
                    query = query.OrderByDescending(p => p.User.FullName);
                    break;
                case "active":
                    query = query.OrderBy(p => p.IsActive);
                    break;
                case "active_desc":
                    query = query.OrderByDescending(p => p.IsActive);
                    break;
                default:
                    query = query.OrderBy(p => p.Name);
                    break;
            }

            var totalPets = await query.CountAsync();
            var pets = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalPets / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.SearchName = searchName;
            ViewBag.SearchOwner = searchOwner;
            ViewBag.IsActive = isActive;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SpeciesName = speciesName;

            Console.WriteLine($"[AdminController] Retrieved {pets.Count} pets for page {page}");
            return View(pets);
        }
        [HttpPost]
        public async Task<IActionResult> DeletePetImage(int imageId, string imageUrl)
        {
            try
            {
                Console.WriteLine($"[AdminController] DeletePetImage called, imageId: {imageId}, imageUrl: {imageUrl}");
                if (imageId > 0)
                {
                    await _petService.DeletePetImageAsync(imageId);
                    Console.WriteLine("[AdminController] Image deleted successfully, imageId: {imageId}");
                    return Json(new { success = true, message = "Ảnh đã được xóa thành công." });
                }
                Console.WriteLine("[AdminController] Failed to delete image, imageId: {imageId}");
                return Json(new { success = false, message = "Không thể xóa ảnh." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AdminController] Error deleting image: {ex.Message} - StackTrace: {ex.StackTrace}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa ảnh." });
            }
        }
        public async Task<IActionResult> Pet_Detail(int petId)
        {
            var (pet, suggestedPets) = await _petService.GetPetDetailWithSuggestionsAsync(petId);
            if (pet == null)
            {
                return NotFound();
            }

            var viewModel = new PetDetailViewModel
            {
                Pet = pet,
                SuggestedPets = suggestedPets,
                LastSpaVisit = pet.LastSpaVisit,
                AppointmentCount = pet.AppointmentPets?.Count ?? 0,
                SpeciesName = pet.Species?.SpeciesName ?? "N/A",
                OwnerName = pet.User?.FullName ?? "N/A",
                IsActive = pet.IsActive ?? true,
                PetImages = await _petService.GetPetImagesAsync(petId) // Lấy danh sách ảnh
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Add_New_Pet()
        {
            Console.WriteLine("[AdminController] Add_New_Pet called");
            var species = await _petService.GetAllSpeciesAsync() ?? new List<Species>();
            var users = await _context.Users.ToListAsync();
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            var currentUser = currentUserId.HasValue ? await _context.Users.FindAsync(currentUserId.Value) : null;
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.CurrentUserName = currentUser?.FullName ?? "Chưa đăng nhập";
            ViewBag.Users = new SelectList(users, "UserId", "FullName", currentUserId);
            ViewBag.Species = species;
            return View(new PetDetailViewModel { Pet = new Pet { UserId = currentUserId }, SpeciesList = species });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add_New_Pet(PetDetailViewModel viewModel, List<IFormFile> Images)
        {
            Console.WriteLine("[AdminController] Add_New_Pet POST called");
            Console.WriteLine($"[AdminController] Received data: Name={viewModel.Pet.Name}, SpeciesId={viewModel.Pet.SpeciesId}, UserId={viewModel.Pet.UserId}");

            ModelState.Remove("OwnerName");
            ModelState.Remove("SpeciesName");
            ModelState.Remove("SuggestedPets");
            ModelState.Remove("LastSpaVisit");
            ModelState.Remove("AppointmentCount");
            ModelState.Remove("PetImages");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("[AdminController] ModelState is invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"❌ {error.ErrorMessage}");
                }
                PrepareViewData(viewModel);
                return View(viewModel);
            }

            var pet = viewModel.Pet;
            pet.UserId = pet.UserId ?? HttpContext.Session.GetInt32("CurrentUserId") ?? 1; // Mặc định session UserId
            if (pet.SpeciesId == null)
            {
                ModelState.AddModelError("Pet.SpeciesId", "Loài là trường bắt buộc.");
                PrepareViewData(viewModel);
                return View(viewModel);
            }

            pet.CreatedAt = DateTime.Now;
            pet.IsActive = true;

            try
            {
                Console.WriteLine("[AdminController] Attempting to create pet...");
                await _petService.CreatePetAsync(pet, Images);
                Console.WriteLine("[AdminController] Pet created successfully, PetId: {pet.PetId}");
                TempData["SuccessMessage"] = "Thêm thú cưng thành công!";
                return RedirectToAction("Pets_List");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AdminController] Error creating pet: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                PrepareViewData(viewModel);
                return View(viewModel);
            }
        }

        private void PrepareViewData(PetDetailViewModel viewModel)
        {
            viewModel.SpeciesList = _petService.GetAllSpeciesAsync().Result ?? new List<Species>();
            ViewBag.Users = new SelectList(_context.Users.ToListAsync().Result, "UserId", "FullName");
        }

        [HttpGet]
        public async Task<IActionResult> SearchOwners(string term)
        {
            var users = await _context.Users
                .Where(u => (u.Email != null && u.Email.Contains(term)) ||
                           (u.FullName != null && u.FullName.Contains(term)) ||
                           (u.Username != null && u.Username.Contains(term)))
                .Select(u => new { userId = u.UserId, fullName = u.FullName ?? "Chưa có tên", email = u.Email, username = u.Username })
                .ToListAsync();
            return Json(users);
        }
        public async Task<IActionResult> Edit_Pet(int id)
        {
            var pet = await _petService.GetPetByIdAsync(id);
            if (pet == null)
            {
                return NotFound();
            }

            var species = await _petService.GetAllSpeciesAsync();
            var users = await _context.Users.ToListAsync();
            var viewModel = new PetDetailViewModel
            {
                Pet = pet,
                SpeciesList = species,
                SuggestedPets = await _petService.GetSuggestedPetsAsync(pet.SpeciesId ?? 0, pet.PetId, 3),
                OwnerName = pet.User?.FullName ?? "Chưa có thông tin",
                SpeciesName = pet.Species?.SpeciesName ?? "Chưa có thông tin",
                PetImages = await _petService.GetPetImagesAsync(id) // Lấy danh sách ảnh hiện tại
            };
            ViewBag.Users = new SelectList(users, "UserId", "FullName", pet.UserId);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit_Pet(int id, PetDetailViewModel viewModel, List<IFormFile> Images)
        {
            Console.WriteLine("[AdminController] Edit_Pet POST called");
            Console.WriteLine($"Received data: PetId={id}, Name={viewModel.Pet.Name}, SpeciesId={viewModel.Pet.SpeciesId}, " +
                              $"Gender={viewModel.Pet.Gender}, UserId={viewModel.Pet.UserId}, IsActive={viewModel.Pet.IsActive}, " +
                              $"OwnerName={viewModel.OwnerName}, SpeciesName={viewModel.SpeciesName}");

            // Gán PetId từ route để đảm bảo không bị override
            viewModel.Pet.PetId = id;

            ModelState.Remove("OwnerName");
            ModelState.Remove("SpeciesName");
            ModelState.Remove("SuggestedPets");
            ModelState.Remove("LastSpaVisit");
            ModelState.Remove("AppointmentCount");
            ModelState.Remove("PetImages");

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    if (state.Value?.Errors.Count > 0)
                    {
                        Console.WriteLine($"❌ ERROR AT: {state.Key}");
                        foreach (var error in state.Value.Errors)
                        {
                            Console.WriteLine($"   ➤ {error.ErrorMessage}");
                        }
                    }
                }
                viewModel.SpeciesList = await _petService.GetAllSpeciesAsync();
                var users = await _context.Users.ToListAsync();
                ViewBag.Users = new SelectList(users, "UserId", "FullName", viewModel.Pet.UserId);
                return View(viewModel);
            }

            var pet = viewModel.Pet;
            Console.WriteLine($"[AdminController] Validated Pet data: Name={pet.Name}, SpeciesId={pet.SpeciesId}, Gender={pet.Gender}, UserId={pet.UserId}");

            if (pet.SpeciesId == null)
            {
                ModelState.AddModelError("Pet.SpeciesId", "Loài là trường bắt buộc.");
                viewModel.SpeciesList = await _petService.GetAllSpeciesAsync();
                var users = await _context.Users.ToListAsync();
                ViewBag.Users = new SelectList(users, "UserId", "FullName", viewModel.Pet.UserId);
                return View(viewModel);
            }

            if (pet.UserId == null || pet.UserId == 0)
            {
                Console.WriteLine("[AdminController] Warning: No valid UserId selected, using default 1.");
                pet.UserId = 1;
            }

            pet.CreatedAt = DateTime.Now;

            try
            {
                Console.WriteLine("[AdminController] Attempting to process pet...");
                await _petService.UpdatePetAsync(pet, Images);
                Console.WriteLine("[AdminController] Pet updated successfully, PetId: {pet.PetId}");
                TempData["SuccessMessage"] = "Cập nhật thú cưng thành công!";
                return RedirectToAction("Pets_List");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AdminController] Error processing pet: {ex.Message} - StackTrace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                viewModel.SpeciesList = await _petService.GetAllSpeciesAsync();
                var users = await _context.Users.ToListAsync();
                ViewBag.Users = new SelectList(users, "UserId", "FullName", viewModel.Pet.UserId);
                return View(viewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DisablePet(int id)
        {
            var pet = await _petService.GetPetByIdAsync(id);
            if (pet == null)
            {
                return NotFound();
            }

            Console.WriteLine($"[AdminController] Disabling pet, PetId: {id}");
            pet.IsActive = false;
            await _petService.UpdatePetAsync(pet);
            Console.WriteLine("[AdminController] Pet disabled successfully, PetId: {id}");
            TempData["SuccessMessage"] = "Thú cưng đã được vô hiệu hóa thành công!";
            return RedirectToAction("Pets_List");
        }

        [HttpPost]
        public async Task<IActionResult> RestorePet(int id)
        {
            var pet = await _petService.GetPetByIdAsync(id);
            if (pet == null)
            {
                return NotFound();
            }

            Console.WriteLine($"[AdminController] Restoring pet, PetId: {id}");
            pet.IsActive = true;
            await _petService.UpdatePetAsync(pet);
            Console.WriteLine("[AdminController] Pet restored successfully, PetId: {id}");
            TempData["SuccessMessage"] = "Thú cưng đã được khôi phục thành công!";
            return RedirectToAction("Pets_List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePet(int id)
        {
            try
            {
                await _petService.DeletePetAsync(id);
                TempData["SuccessMessage"] = "Xóa thú cưng thành công!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AdminController] Error deleting pet: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa thú cưng: " + ex.Message;
            }
            return RedirectToAction("Pets_List");
        }




        public IActionResult List_Customer()
        {
            return View();
        }

        //=======================================================================================================================
        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Product_Detail(int productID)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == productID);

            if (product == null)
            {
                return NotFound();
            }

            // Truyền dữ liệu sản phẩm vào View
            return View(product);
        }

        public async Task<IActionResult> Products_List(int page = 1)
        {

            const int pageSize = 15; // Số sản phẩm trên mỗi trang
            var totalProducts = await _context.Products.CountAsync();
            var products = await _context.Products
                .Include(p => p.Reviews)
                .Include(p => p.Category)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewBag.CurrentPage = page;

            return View(products);
        }
        //=======================================================================================================================
        // Hiển thị danh sách dịch vụ
        //public IActionResult ManageService(int? categoryId, string search)
        //{
        //    var model = _serviceService.GetAllService();
        //    if (categoryId.HasValue)
        //    {
        //        model.Services = model.Services.Where(s => s.CategoryId == categoryId.Value).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        model.Services = model.Services.Where(s => s.Name.Contains(search)).ToList();
        //    }
        //    model.SelectedCategoryId = categoryId;
        //    return View(model);
        //}

        [HttpPost]
        public IActionResult AddService(Service service)
        {
            _serviceService.AddService(service);
            _serviceService.Save();
            return RedirectToAction("ManageService");
        }

        //public IActionResult EditService(int id)
        //{
        //    var service = _serviceService.GetServiceById(id);
        //    var categories = _serviceService.GetAllService().Categories;
        //    ViewBag.Categories = categories;
        //    return View(service);
        //}

        [HttpPost]
        public IActionResult EditService(Service service)
        {
            _serviceService.UpdateService(service);
            _serviceService.Save();
            return RedirectToAction("ManageService");
        }

        public IActionResult SoftDeleteService(int id)
        {
            _serviceService.SoftDeleteService(id);
            _serviceService.Save();
            return RedirectToAction("ManageService");
        }

        public IActionResult RestoreService(int id)
        {
            _serviceService.RestoreService(id);
            _serviceService.Save();
            return RedirectToAction("ManageService");
        }
        //=======================================================================================================================


        //=======================================================================================================================

        // Add New Product

        public async Task<IActionResult> Add_New_Product()
        {
            var categories = await _productService.GetAllProductCategoriesAsync();
            var viewModel = new ProductViewModel
            {
                Product = new Product(),
                SuggestedProducts = new List<Product>(), // không dùng trong add
                Categories = categories
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add_New_Product(ProductViewModel viewModel, IFormFile Image)
        {

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    if (state.Value?.Errors.Count > 0)
                    {
                        Console.WriteLine($"❌ ERROR AT: {state.Key}");
                        foreach (var error in state.Value.Errors)
                        {
                            Console.WriteLine($"   ➤ {error.ErrorMessage}");
                        }
                    }
                }
                // Gửi lại categories nếu có lỗi
                viewModel.Categories = await _productService.GetAllProductCategoriesAsync();
                return View(viewModel);
            }

            var product = viewModel.Product;

            // Upload ảnh
            if (Image != null && Image.Length > 0)
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgProducts");
                if (!Directory.Exists(imagePath))
                    Directory.CreateDirectory(imagePath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                var filePath = Path.Combine(imagePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                product.ImageUrl = "/imgProducts/" + fileName;
            }

            product.CreatedAt = DateTime.Now;
            if (product.IsActive == null)
            {
                product.IsActive = true;
            }

            try
            {
                // thêm hoặc cập nhật
                await _productService.CreateProductAsync(product);
                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("Add_New_Product");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Add_New_Product");
            }

        }
        //=======================================================================================================================

        //Edit Products
        public async Task<IActionResult> Edit_Products(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _productService.GetAllProductCategoriesAsync();
            var suggestedProducts = await _productService.GetAllProductsAsync(1, 4); // Lấy 4 sản phẩm gợi ý
            var viewModel = new ProductViewModel
            {
                Product = product,
                SuggestedProducts = suggestedProducts.Where(p => p.ProductId != id).ToList(), // Loại bỏ sản phẩm hiện tại khỏi gợi ý
                Categories = categories
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit_Products(ProductViewModel viewModel, IFormFile? Image)
        {
            var product = await _productService.GetProductByIdAsync(viewModel.Product.ProductId);
            if (product == null) return NotFound();

            // Cập nhật các trường từ form
            product.Name = viewModel.Product.Name;
            product.Description = viewModel.Product.Description;
            product.Price = viewModel.Product.Price;
            product.Stock = viewModel.Product.Stock;
            product.CategoryId = viewModel.Product.CategoryId;
            product.IsActive = viewModel.Product.IsActive;

            // Nếu có ảnh mới
            if (Image != null && Image.Length > 0)
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgProducts");
                if (!Directory.Exists(imagePath))
                    Directory.CreateDirectory(imagePath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                var filePath = Path.Combine(imagePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                product.ImageUrl = "/imgProducts/" + fileName;
            }

            // Cập nhật vào CSDL
            await _productService.UpdateProductAsync(product);
            try
            {
                // thêm hoặc cập nhật
                await _productService.UpdateProductAsync(product);
                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Edit_Products", new { id = product.ProductId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Edit_Products", new { id = product.ProductId });
            }
        }


        //=======================================================================================================================

        // Disable Product

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DisableProductAsync(id);
            TempData["SuccessMessage"] = "Đã ngừng kích hoạt sản phẩm.";
            return RedirectToAction("Products_List");
        }

        //Enable Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableProduct(int id)
        {
            await _productService.EnableProductAsync(id);
            TempData["SuccessMessage"] = "Đã kích hoạt sản phẩm.";
            return RedirectToAction("Products_List");
        }
        //=======================================================================================================================

        // Delete Product
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteProduct(int id)
        //{
        //    await _productService.DeleteProductAsync(id);
        //    TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
        //    return RedirectToAction("Products_List");
        //}

        //=======================================================================================================================


        public IActionResult Refund()
        {
            return View();
        }

        public IActionResult Staff()
        {
            return View();
        }

        public IActionResult StaffSchedule()
        {
            return View("StaffSchedule");
        }

        //=======================================================================================================================
        // BLOG MANAGEMENT
        public async Task<IActionResult> ManageBlog(string status = "All", string? search = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            var currentUserName = HttpContext.Session.GetString("CurrentUserName") ?? "Unknown";
            var currentUserRoleId = HttpContext.Session.GetInt32("CurrentUserRoleId") ?? -1;
            Console.WriteLine($"[AdminController] ManageBlog - CurrentUserId: {currentUserId ?? -1}, CurrentUserName: {currentUserName}, CurrentUserRoleId: {currentUserRoleId}, IsAuthenticated: {User.Identity?.IsAuthenticated}");

            if (!currentUserId.HasValue || (currentUserRoleId != 1 && currentUserRoleId != 3)) // Admin or Staff
            {
                Console.WriteLine("[AdminController] Redirecting to Login due to insufficient permissions or null user ID.");
                return RedirectToAction("Login", "Login");
            }

            var model = await _blogService.GetAdminDashboardAsync();

            model.AllBlogs = await _blogService.GetAllBlogsForAdminAsync();

            if (status != "All")
            {
                model.AllBlogs = model.AllBlogs.Where(b => b.Status == status).ToList();
            }

            if (!string.IsNullOrEmpty(search))
            {
                model.AllBlogs = model.AllBlogs.Where(b =>
                    b.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    b.ShortContent.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    b.AuthorName.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (fromDate.HasValue)
            {
                model.AllBlogs = model.AllBlogs.Where(b => b.CreatedAt >= fromDate.Value).ToList();
            }

            if (toDate.HasValue)
            {
                model.AllBlogs = model.AllBlogs.Where(b => b.CreatedAt <= toDate.Value.AddDays(1)).ToList();
            }

            model.StatusFilter = status;
            model.SearchQuery = search;
            model.FromDate = fromDate;
            model.ToDate = toDate;

            if (!string.IsNullOrEmpty(search))
            {
                model.AllBlogs = model.AllBlogs.Where(b =>
                    b.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    b.AuthorName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.StatusFilter = status;
            ViewBag.SearchQuery = search;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveBlog(int blogId)
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            var currentUserRoleId = HttpContext.Session.GetInt32("CurrentUserRoleId") ?? -1;
            Console.WriteLine($"[AdminController] ApproveBlog - CurrentUserId: {currentUserId ?? -1}, CurrentUserRoleId: {currentUserRoleId}");

            if (!currentUserId.HasValue || (currentUserRoleId != 1 && currentUserRoleId != 3))
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            try
            {
                var success = await _blogService.ApproveBlogAsync(blogId, currentUserId.Value);
                if (success)
                {
                    return Json(new { success = true, message = "Blog đã được duyệt thành công." });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể duyệt blog này." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublishBlog(int blogId)
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            var currentUserRoleId = HttpContext.Session.GetInt32("CurrentUserRoleId") ?? -1;
            Console.WriteLine($"[AdminController] PublishBlog - CurrentUserId: {currentUserId ?? -1}, CurrentUserRoleId: {currentUserRoleId}");

            if (!currentUserId.HasValue || (currentUserRoleId != 1 && currentUserRoleId != 3))
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            try
            {
                var success = await _blogService.PublishBlogAsync(blogId, currentUserId.Value);
                if (success)
                {
                    return Json(new { success = true, message = "Blog đã được xuất bản." });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xuất bản blog này." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBlogAdmin(int blogId)
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            var currentUserRoleId = HttpContext.Session.GetInt32("CurrentUserRoleId") ?? -1;
            Console.WriteLine($"[AdminController] DeleteBlogAdmin - CurrentUserId: {currentUserId ?? -1}, CurrentUserRoleId: {currentUserRoleId}");

            if (!currentUserId.HasValue || (currentUserRoleId != 1 && currentUserRoleId != 3))
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            try
            {
                var success = await _blogService.DeleteBlogAsync(blogId, currentUserId.Value);
                if (success)
                {
                    return Json(new { success = true, message = "Blog đã được xóa." });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa blog này." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

//=======================================================================================================
// OrderHistory

public IActionResult OrderHistory(string status = "All", int page = 1)
{
    int pageSize = 10;
    int totalOrders;
    List<OrderViewModel> orders;

    if (status == "All")
    {
        orders = _orderService.GetOrdersPaged(page, pageSize, out totalOrders);
    }
    else
    {
        orders = _orderService.GetOrdersByStatusPaged(status, page, pageSize, out totalOrders);
    }

    ViewBag.CurrentPage = page;
    ViewBag.TotalPages = (int)Math.Ceiling((double)totalOrders / pageSize);
    ViewBag.StatusFilter = status;

    return View("~/Views/Admin/OrderHistory.cshtml", orders);
}

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, string newStatus)
        {
            try
            {
                var order = _orderService.GetOrderById(orderId);
                if (order == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng";
                    return RedirectToAction("OrderHistory");
                }

                // Kiểm tra trạng thái hiện tại
                if (order.StatusId == 4) // Đã giao
                {
                    TempData["ErrorMessage"] = "Không thể thay đổi trạng thái đơn hàng đã giao";
                    return RedirectToAction("OrderHistory");
                }

                if (order.StatusId == 5) // Đã hủy
                {
                    TempData["ErrorMessage"] = "Không thể thay đổi trạng thái đơn hàng đã hủy";
                    return RedirectToAction("OrderHistory");
                }

                // Cập nhật trạng thái
                order.StatusId = MapStatusNameToId(newStatus);
                _orderService.UpdateOrder(order);

                TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công";
                return RedirectToAction("OrderHistory");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi cập nhật trạng thái: " + ex.Message;
                return RedirectToAction("OrderHistory");
            }
        }

        [HttpPost]
        public IActionResult DeleteOrder(int orderId)
        {
            try
            {
                var order = _orderService.GetOrderById(orderId);
                if (order == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng";
                    return RedirectToAction("OrderHistory");
                }

                // Kiểm tra trạng thái đơn hàng trước khi xóa
                if (order.StatusId == 3 || order.StatusId == 4) // Đã giao vận chuyển hoặc Đã giao
                {
                    TempData["ErrorMessage"] = "Không thể xóa đơn hàng đã giao vận chuyển hoặc đã giao";
                    return RedirectToAction("OrderHistory");
                }

                _orderService.DeleteOrder(orderId);
                TempData["SuccessMessage"] = "Xóa đơn hàng thành công";
                return RedirectToAction("OrderHistory");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa đơn hàng: " + ex.Message;
                return RedirectToAction("OrderHistory");
            }
        }

            [HttpGet]
            public IActionResult GetOrderDetail(int orderId)
            {
                try
                {
                    Console.WriteLine($"[AdminController] GetOrderDetail called with orderId: {orderId}");
                
                    // Sử dụng GetOrderDetail từ OrderService
                    var order = _orderService.GetOrderDetail(orderId);
                    if (order == null)
                    {
                        Console.WriteLine($"[AdminController] Order not found for ID: {orderId}");
                        return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                    }

                
                    return Json(new { success = true, data = order });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AdminController] Error in GetOrderDetail: {ex.Message}");
                    return Json(new { success = false, message = "Lỗi khi tải chi tiết đơn hàng: " + ex.Message });
                }
            }

        // Helper: Map status name to id
        private int MapStatusNameToId(string statusName)
        {
            switch (statusName)
            {
                case "Đang chờ xử lý": return 1;
                case "Đang chuẩn bị": return 2;
                case "Đã giao vận chuyển": return 3;
                case "Đã giao": return 4;
                case "Đã hủy": return 5;
                default: return 1;
            }
        }

    }
}
