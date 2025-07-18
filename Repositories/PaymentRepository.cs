using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
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
}
