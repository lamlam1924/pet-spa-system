using System.Collections.Generic;
using pet_spa_system1.Models;
namespace pet_spa_system1.Repositories
{
    public interface IPaymentRepository
    {
        void AddPayment(Payment payment);
        Payment GetPaymentByOrderId(int orderId);
        Payment GetPaymentById(int paymentId);
        List<Payment> GetAllPayments();
        void UpdatePayment(Payment payment);
        void DeletePayment(int paymentId);
        // Có thể bổ sung hàm tìm kiếm, lọc nếu cần
    }
}
