using System.Collections.Generic;
using System.Linq;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using Microsoft.EntityFrameworkCore;

public class PaymentRepository : IPaymentRepository
{
    private readonly PetDataShopContext _context;
    public PaymentRepository(PetDataShopContext context)
    {
        _context = context;
    }

    public void AddPayment(Payment payment)
    {
        _context.Payments.Add(payment);
        _context.SaveChanges();
    }

    public Payment GetPaymentByOrderId(int orderId)
    {
        return _context.Payments.FirstOrDefault(p => p.OrderId == orderId);
    }

    public Payment GetPaymentById(int paymentId)
    {
        return _context.Payments.FirstOrDefault(p => p.PaymentId == paymentId);
    }

    public List<Payment> GetAllPayments()
    {
        return _context.Payments
            .Include(p => p.Order)
            .Include(p => p.User)
            .Include(p => p.PaymentMethod)
            .Include(p => p.PaymentStatus)
            .ToList();
    }

    public void UpdatePayment(Payment payment)
    {
        _context.Payments.Update(payment);
        _context.SaveChanges();
    }

    public void DeletePayment(int paymentId)
    {
        var payment = _context.Payments.FirstOrDefault(p => p.PaymentId == paymentId);
        if (payment != null)
        {
            _context.Payments.Remove(payment);
            _context.SaveChanges();
        }
    }
}
