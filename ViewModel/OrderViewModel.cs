using System.Collections.Generic;

namespace pet_spa_system1.ViewModel
{
    public class OrderItemViewModel
    {   
        public int ProductId { get; set; } // Thêm trường ProductId
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string ImageUrl { get; set; }
    }

    public class OrderViewModel
    {
        public string OrderID { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime? OrderDate { get; set; } // Đổi từ string sang DateTime?
        public List<OrderItemViewModel> Items { get; set; }
        // Thêm các trường sau:
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public int StatusId { get; set; }
    }
}
