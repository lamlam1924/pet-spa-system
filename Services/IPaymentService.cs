using System.Collections.Generic;
using pet_spa_system1.Models;
using pet_spa_system1.ViewModel;

public interface IPaymentService
{
    void AddPayment(Payment payment);
    Payment GetPaymentByOrderId(int orderId);
    Payment GetPaymentById(int paymentId);
    List<PaymentViewModel> GetAllPayments();
    void UpdatePayment(Payment payment);
    void DeletePayment(int paymentId);
}
