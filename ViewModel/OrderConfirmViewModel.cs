using System.Collections.Generic;
using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModel
{
    public class OrderConfirmationViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string ShippingAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDetail> Items { get; set; }
    }

    public class OrderItemDetail
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string ImageUrl { get; set; }
    }
}