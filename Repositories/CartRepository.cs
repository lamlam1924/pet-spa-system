using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly PetDataShopContext _context;

        public CartRepository(PetDataShopContext context)
        {
            _context = context;
        }

        public async Task<List<Cart>> GetCartByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
            var existing = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                var newCart = new Cart
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    AddedAt = DateTime.Now
                };
                _context.Carts.Add(newCart);
            }

            await _context.SaveChangesAsync();
        }

        //public async Task AddToCartAsync( int productId, int quantity)
        //{
        //    var existing = await _context.Carts
        //        .FirstOrDefaultAsync(c => c.UserId == 1 && c.ProductId == productId);

        //    if (existing != null)
        //    {
        //        existing.Quantity += quantity;
        //    }
        //    else
        //    {
        //        var newCart = new Cart
        //        {
        //            UserId = 1,
        //            ProductId = productId,
        //            Quantity = quantity,
        //            AddedAt = DateTime.Now
        //        };
        //        _context.Carts.Add(newCart);
        //    }

        //    await _context.SaveChangesAsync();
        //}


        public async Task RemoveFromCartAsync(int cartId)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveProductFromCart(int productId, int userId)
        {
           //thêm userid từ sesscion

            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }
       

        public async Task UpdateQuantityAsync(int cartId, int newQuantity)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart != null)
            {
                cart.Quantity = newQuantity;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            var carts = _context.Carts.Where(c => c.UserId == userId);
            _context.Carts.RemoveRange(carts);
            await _context.SaveChangesAsync();
        }

        public async Task<Cart> IncreaseQuantityAsync(int productId, int userId)
        {
            var cart = await _context.Carts.Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);

            if (cart != null)
            {
                cart.Quantity++;
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<Cart> DecreaseQuantityAsync(int productId, int userId)
        {
            var cart = await _context.Carts.Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);

            if (cart != null && cart.Quantity > 1)
            {
                cart.Quantity--;
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<decimal> GetTotalAmountAsync(int userId)
        {
            return await _context.Carts
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity * c.Product.Price);
        }

       
    }
}
