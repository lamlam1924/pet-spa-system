namespace pet_spa_system1.ViewModel
{
    public class OrderViewModel
    {
        public string OrderID { get; set; }           // Mã đơn hàng (sử dụng cho hiển thị và thao tác)
        public string ProductName { get; set; }       // Tên sản phẩm
        public int Quantity { get; set; }             // Số lượng
        public decimal UnitPrice { get; set; }      // Tổng tiền
        public string StatusName { get; set; }
    }
}
