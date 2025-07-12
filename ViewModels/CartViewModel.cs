using System.Collections.Generic;
using System.Linq;
using pet_spa_system1.Models;
using pet_spa_system1.ViewModels;

namespace pet_spa_system1.ViewModel
{
    public class CartViewModel
    {
        public List<Cart> Items { get; set; } = new List<Cart>();

        public decimal TotalAmount => Items.Sum(item => item.Product.Price * item.Quantity);
    }
}
