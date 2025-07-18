using pet_spa_system1.Models;
public interface IPaymentService
{
    void AddPayment(Payment payment);
    Payment GetPaymentByOrderId(int orderId);
}
