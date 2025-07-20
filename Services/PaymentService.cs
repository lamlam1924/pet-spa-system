using System.Collections.Generic;
using System.Linq;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using pet_spa_system1.ViewModel;

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

    public Payment GetPaymentById(int paymentId)
    {
        return _paymentRepository.GetPaymentById(paymentId);
    }

    public List<PaymentViewModel> GetAllPayments()
    {
        var payments = _paymentRepository.GetAllPayments();
        return payments.Select(p => new PaymentViewModel
        {
            PaymentId = p.PaymentId,
            OrderId = p.OrderId,
            CustomerName = p.User?.FullName ?? "",
            Amount = p.Amount,
            PaymentMethod = p.PaymentMethod?.MethodName ?? "",
            Status = p.PaymentStatus?.StatusName ?? "",
            TransactionId = p.TransactionId,
            PaymentDate = p.PaymentDate
        }).ToList();
    }

    public void UpdatePayment(Payment payment)
    {
        _paymentRepository.UpdatePayment(payment);
    }

    public void DeletePayment(int paymentId)
    {
        _paymentRepository.DeletePayment(paymentId);
    }
}
