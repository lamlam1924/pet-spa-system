using pet_spa_system1.Models;
using pet_spa_system1.Repositories;

namespace pet_spa_system1.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICheckoutRepository _checkoutRepository;

        public CheckoutService(ICheckoutRepository checkoutRepository)
        {
            _checkoutRepository = checkoutRepository;
        }

        // Lấy giỏ hàng theo userId
        public Task<List<Cart>> GetCartByUserIdAsync(int userId)
        {
            return _checkoutRepository.GetCartByUserIdAsync(userId);
        }

        // Lấy danh sách phương thức thanh toán
        public Task<List<PaymentMethod>> GetPaymentMethodsAsync()
        {
            return _checkoutRepository.GetPaymentMethodsAsync();
        }

        // Lấy thông tin người dùng theo ID
        public Task<User> GetUserByIdAsync(int userId)
        {
            return _checkoutRepository.GetUserByIdAsync(userId);
        }

        // Lấy thông tin người dùng hiện tại (có thể cần điều chỉnh để lấy từ session hoặc claims)
        public Task<User> GetUserByIdAsync()
        {
            return _checkoutRepository.GetUserByIdAsync();
        }

        public Task<bool> ProcessCheckoutAsync(int userId, int paymentMethodId, string discountCode = null)
        {
            throw new NotImplementedException();
        }
    }



}
