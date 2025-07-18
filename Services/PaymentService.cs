using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    public PaymentService(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public void AddPayment(Payment payment)
    {
        _paymentRepository.AddPayment(payment);
    }

    public Payment GetPaymentByOrderId(int orderId)
    {
        return _paymentRepository.GetPaymentByOrderId(orderId);
    }
}
