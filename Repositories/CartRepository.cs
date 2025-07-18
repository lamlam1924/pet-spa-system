using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pet_spa_system1.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly PetDataShopContext _context;

        public CartRepository(PetDataShopContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
            if (quantity <= 0) throw new ArgumentException("Số lượng phải lớn hơn 0.", nameof(quantity));

            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
            }
            else
            {
                var newCartItem = new Cart
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    AddedAt = DateTime.Now
                };
                _context.Carts.Add(newCartItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int cartId)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem == null) throw new KeyNotFoundException($"Không tìm thấy mục giỏ hàng với ID {cartId}.");

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveProductFromCart(int productId, int userId)
        {
            var cartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem == null) throw new KeyNotFoundException($"Không tìm thấy sản phẩm {productId} trong giỏ hàng của người dùng {userId}.");

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int cartId, int newQuantity)
        {
            if (newQuantity < 0) throw new ArgumentException("Số lượng không thể âm.", nameof(newQuantity));

            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem == null) throw new KeyNotFoundException($"Không tìm thấy mục giỏ hàng với ID {cartId}.");

            cartItem.Quantity = newQuantity;
            await _context.SaveChangesAsync();
        }

        public async Task ClearCartAsync(int userId)
        {
            var cartItems = _context.Carts.Where(c => c.UserId == userId).ToList();
            if (!cartItems.Any()) throw new KeyNotFoundException($"Giỏ hàng của người dùng {userId} trống.");

            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        public async Task<Cart> IncreaseQuantityAsync(int productId, int userId)
        {
            var cartItem = await _context.Carts
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);

            if (cartItem == null) throw new KeyNotFoundException($"Không tìm thấy sản phẩm {productId} trong giỏ hàng của người dùng {userId}.");

            cartItem.Quantity++;
            await _context.SaveChangesAsync();

            return cartItem;
        }

        public async Task<Cart> DecreaseQuantityAsync(int productId, int userId)
        {
            var cartItem = await _context.Carts
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);

            if (cartItem == null) throw new KeyNotFoundException($"Không tìm thấy sản phẩm {productId} trong giỏ hàng của người dùng {userId}.");
            if (cartItem.Quantity <= 1) throw new InvalidOperationException("Số lượng không thể giảm dưới 1.");

            cartItem.Quantity--;
            await _context.SaveChangesAsync();

            return cartItem;
        }

        public async Task<decimal> GetTotalAmountAsync(int userId)
        {
            return await _context.Carts
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity * c.Product.Price );
        }
    }
}