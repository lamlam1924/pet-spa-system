using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModel
{
    public class CartViewModel
    {
        public List<Cart> Items { get; set; } = new List<Cart>();

        public decimal TotalAmount => Items.Sum(item => item.Product.Price * item.Quantity);
    }

    public class UpdateCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
