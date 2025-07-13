using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task AddToCartAsync(Cart cart)
        {
            var existing = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == cart.UserId && c.ProductId == cart.ProductId);

            if (existing != null)
            {
                existing.Quantity += cart.Quantity;
            }
            else
            {
                cart.AddedAt = DateTime.Now;
                _context.Carts.Add(cart);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int cartId)
        {
            var cart = await _context.Carts.FindAsync(cartId);
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
    }
}
