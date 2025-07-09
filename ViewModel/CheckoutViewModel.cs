using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModel
{
    public class CheckoutViewModel
    {
         public List<Cart> CartItems { get; set; }
    public User User { get; set; }
    public List<PaymentMethod> PaymentMethods { get; set; }
    public decimal TotalAmount { get; set; }
    }
}
