using pet_spa_system1.Models;
namespace pet_spa_system1.Repositories
{
    public interface IPaymentRepository
    {
        void AddPayment(Payment payment);
        Payment GetPaymentByOrderId(int orderId);
        // ... các hàm khác nếu cần
    }
}
