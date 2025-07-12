using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModels;

namespace pet_spa_system1.Controllers.ProductManager
{
    public class ProductsController : Controller
    {
       
            private readonly IProductService _productService;
            private readonly ICartService _cartService;

        public ProductsController(IProductService productService, ICartService cartService)
            {
                _productService = productService;
                _cartService = cartService;
        }

        public async Task<IActionResult> Shop(int page = 1)
        {
            const int pageSize = 15;

            var products = await _productService.GetActiveProductsAsync(page, pageSize);
            var totalActiveProducts = await _productService.CountActiveProductsAsync();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalActiveProducts / pageSize);
            ViewBag.CurrentPage = page;

            return View(products);
        }

        

            //public async Task<IActionResult> Product_List(int id)
            //{
            //    var product = await _productService.GetProductByIdAsync(id);
            //    if (product == null)
            //    {
            //        return NotFound();
            //    }
            //    ViewBag.Categories = await _productService.GetAllProductCategoriesAsync();
            //    return View(product);
            //}

            //[HttpPost]
            //[ValidateAntiForgeryToken]
            //public async Task<IActionResult> Product_List(int id, [Bind("ProductId,CategoryId,Name,Description,Price,ImageUrl,Stock,IsActive,CreatedAt")] Product product)
            //{
            //    if (id != product.ProductId)
            //    {
            //        return NotFound();
            //    }

            //    if (ModelState.IsValid)
            //    {
            //        try
            //        {
            //            await _productService.UpdateProductAsync(product);
            //        }
            //        catch (DbUpdateConcurrencyException)
            //        {
            //            if (!await Task.Run(() => _productService.ProductExists(id)))
            //            {
            //                return NotFound();
            //            }
            //            else
            //            {
            //                throw;
            //            }
            //        }
            //        return RedirectToAction(nameof(Shop));
            //    }
            //    ViewBag.Categories = await _productService.GetAllProductCategoriesAsync();
            //    return View(product);
            //}

            //public async Task<IActionResult> Delete(int id)
            //{
            //    var product = await _productService.GetProductByIdAsync(id);
            //    if (product == null)
            //    {
            //        return NotFound();
            //    }
            //    return View(product);
            //}

            //[HttpPost, ActionName("Delete")]
            //[ValidateAntiForgeryToken]
            //public async Task<IActionResult> DeleteConfirmed(int id)
            //{
            //    await _productService.DeleteProductAsync(id);
            //    return RedirectToAction(nameof(Shop));
            //}

        public async Task<IActionResult> Detail(int productID)
        {
            var (product, suggestedProducts) = await _productService.GetProductDetailWithSuggestionsAsync(productID);
            if (product == null) return NotFound();

            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                SuggestedProducts = suggestedProducts
            };

            return View(viewModel);
        }

        [HttpGet]
        [Route("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _productService.GetAllProductCategoriesAsync(); // Giả định phương thức này tồn tại
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi lấy danh sách danh mục: " + ex.Message);
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> Cart()
        //{
        //    int userId = GetCurrentUserId(); // Viết hàm này hoặc lấy từ session

        //    var cartItems = await _cartService.GetCartByUserIdAsync(userId);

        //    var viewModel = new CartViewModel
        //    {
        //        Items = cartItems
        //    };

        //    return View(viewModel);
        //}
        [HttpPost]
        public async Task<IActionResult> Cart(int id)
        {
            var userId = GetCurrentUserId(); // Hàm tự viết để lấy user hiện tại
            await _cartService.AddToCartAsync(new Cart
            {
                UserId = userId,
                ProductId = id,
                Quantity = 1, // Hoặc lấy từ input nếu có
                AddedAt = DateTime.Now
            });
            return RedirectToAction("Cart");
        }

        private int GetCurrentUserId()
    {
        return int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
    }
        }
    }
   