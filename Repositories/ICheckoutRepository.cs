using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public interface ICheckoutRepository
    {
        //get payment methods
        Task<List<PaymentMethod>> GetPaymentMethodsAsync();

        //get cart by user id
        Task<List<Cart>> GetCartByUserIdAsync(int userId);
        // get discount code by code
        //Task<DiscountCode> GetDiscountCodeByCodeAsync(string code);
        //get user by id
        Task<User> GetUserByIdAsync(int userId);

        Task<User> GetUserByIdAsync();



    }
}
