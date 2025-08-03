using pet_spa_system1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pet_spa_system1.Services
{
    public interface ICartService
    {
        // Lấy danh sách giỏ hàng theo UserId
        Task<List<Cart>> GetCartByUserIdAsync(int userId);

        // Thêm sản phẩm vào giỏ hàng
        Task AddToCartAsync(int userId, int productId, int quantity);

        // Xóa một mục khỏi giỏ hàng theo CartId
        Task RemoveFromCartAsync(int cartId);

        // Cập nhật số lượng theo CartId
        Task UpdateQuantityAsync(int cartId, int newQuantity);

        // Xóa toàn bộ giỏ hàng của người dùng
        Task ClearCartAsync(int userId);

        // Xóa sản phẩm khỏi giỏ hàng theo ProductId và UserId
        Task RemoveProductFromCart(int productId, int userId);

        // Tăng số lượng sản phẩm trong giỏ hàng
        Task<Cart> IncreaseQuantityAsync(int productId, int userId);

        // Giảm số lượng sản phẩm trong giỏ hàng
        Task<Cart> DecreaseQuantityAsync(int productId, int userId);

        // Tính tổng tiền của giỏ hàng theo UserId
        Task<decimal> GetTotalAmountAsync(int userId);

        Task<List<Cart>> GetCartsByProductIdAsync(int productId);
        Task<List<int>> GetUsersHavingProductExceedingStock(int productId);
    }
}