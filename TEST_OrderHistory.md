# Test OrderHistory Functionality

## Các chức năng cần test:

### 1. View Orders List
- [ ] Hiển thị danh sách tất cả đơn hàng
- [ ] Hiển thị đúng thông tin: Order ID, Customer, Products, Total Items, Total Amount, Status, Order Date, Shipping Address
- [ ] Hiển thị tổng số sản phẩm thay vì chỉ số lượng sản phẩm đầu tiên
- [ ] Hiển thị "+X more" cho đơn hàng có nhiều sản phẩm

### 2. Search & Filter
- [ ] Tìm kiếm theo customer name
- [ ] Tìm kiếm theo order ID
- [ ] Tìm kiếm theo product name
- [ ] Lọc theo status: All, Đang chờ xử lý, Đang chuẩn bị, Đã giao vận chuyển, Đã giao, Đã hủy

### 3. View Order Details
- [ ] Click nút "View" hiển thị modal
- [ ] Hiển thị thông tin cơ bản: Order ID, Customer, Total Amount, Total Items, Status, Order Date, Shipping Address
- [ ] **QUAN TRỌNG**: Hiển thị TẤT CẢ sản phẩm trong đơn hàng (không chỉ sản phẩm đầu tiên)
- [ ] AJAX call thành công đến `/Admin/GetOrderDetail` với parameter `orderId`
- [ ] Sử dụng `GetOrderDetail` từ OrderService để lấy chi tiết đơn hàng
- [ ] Hiển thị chi tiết từng sản phẩm: tên, số lượng, đơn giá, tổng tiền
- [ ] Loading state và error handling
- [ ] **FIXED**: JSON property conflict đã được giải quyết với JsonPropertyName và JsonIgnore

### 4. Edit Order Status
- [ ] Click nút "Edit" mở modal
- [ ] Chỉ cho phép edit status (các field khác readonly)
- [ ] Validation: Không cho phép thay đổi trạng thái đơn hàng đã giao (StatusId = 4)
- [ ] Validation: Không cho phép thay đổi trạng thái đơn hàng đã hủy (StatusId = 5)
- [ ] Form submission thành công đến `/Admin/UpdateOrderStatus`
- [ ] Hiển thị thông báo thành công/lỗi

### 5. Delete Order
- [ ] Click nút "Delete" hiển thị confirmation modal
- [ ] Validation: Không cho phép xóa đơn hàng đã giao vận chuyển (StatusId = 3)
- [ ] Validation: Không cho phép xóa đơn hàng đã giao (StatusId = 4)
- [ ] Form submission thành công đến `/Admin/DeleteOrder`
- [ ] Hiển thị thông báo thành công/lỗi

### 6. Disabled Buttons
- [ ] Edit button disabled cho đơn hàng đã giao (StatusId = 4)
- [ ] Edit button disabled cho đơn hàng đã hủy (StatusId = 5)
- [ ] Delete button disabled cho đơn hàng đã giao vận chuyển (StatusId = 3)
- [ ] Delete button disabled cho đơn hàng đã giao (StatusId = 4)

## Test Cases:

### Test Case 1: View Order with Multiple Products
1. Tìm đơn hàng có nhiều sản phẩm
2. Click "View" button
3. Kiểm tra modal hiển thị tất cả sản phẩm
4. Kiểm tra thông tin chi tiết từng sản phẩm
5. **Kiểm tra AJAX call**: `/Admin/GetOrderDetail?orderId=X`
6. **Kiểm tra JSON response**: Không có conflict giữa camelCase và PascalCase

### Test Case 2: Edit Order Status
1. Tìm đơn hàng có status "Đang chờ xử lý"
2. Click "Edit" button
3. Thay đổi status thành "Đang chuẩn bị"
4. Submit form
5. Kiểm tra thông báo thành công

### Test Case 3: Delete Order
1. Tìm đơn hàng có status "Đang chờ xử lý"
2. Click "Delete" button
3. Confirm trong modal
4. Kiểm tra thông báo thành công

### Test Case 4: Validation Tests
1. Thử edit đơn hàng đã giao → Kiểm tra validation
2. Thử delete đơn hàng đã giao vận chuyển → Kiểm tra validation
3. Kiểm tra disabled buttons

## Expected Results:

### Success Cases:
- ✅ Hiển thị tất cả sản phẩm trong order detail
- ✅ AJAX call thành công đến `/Admin/GetOrderDetail`
- ✅ Sử dụng `GetOrderDetail` từ OrderService
- ✅ **FIXED**: JSON serialization không có conflict
- ✅ Form submission thành công
- ✅ Validation hoạt động đúng
- ✅ Disabled buttons hiển thị đúng

### Error Cases:
- ✅ Error handling cho AJAX calls
- ✅ Validation messages hiển thị đúng
- ✅ Loading states hoạt động đúng

## Technical Details:

### AdminController.GetOrderDetail:
```csharp
[HttpGet]
public IActionResult GetOrderDetail(int orderId)
{
    try
    {
        Console.WriteLine($"[AdminController] GetOrderDetail called with orderId: {orderId}");
        
        // Sử dụng GetOrderDetail từ OrderService
        var order = _orderService.GetOrderDetail(orderId);
        if (order == null)
        {
            Console.WriteLine($"[AdminController] Order not found for ID: {orderId}");
            return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
        }

        Console.WriteLine($"[AdminController] Order found with {order.items?.Count ?? 0} items");
        return Json(new { success = true, data = order });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[AdminController] Error in GetOrderDetail: {ex.Message}");
        return Json(new { success = false, message = "Lỗi khi tải chi tiết đơn hàng: " + ex.Message });
    }
}
```

### JavaScript AJAX Call:
```javascript
$.get('/Admin/GetOrderDetail', { orderId: orderId }, function(response) {
    if (response.success && response.data) {
        const order = response.data;
        // Render products...
    }
});
```

### OrderViewModel JSON Properties:
```csharp
[JsonPropertyName("orderID")]
public string orderID { get; set; }

[JsonPropertyName("items")]
public List<OrderItemViewModel> items { get; set; }

[JsonIgnore]
public string OrderID { get => orderID; set => orderID = value; }
```

## Notes:
- ✅ Sử dụng đúng parameter `orderId` thay vì `userId`
- ✅ Sử dụng `GetOrderDetail` từ OrderService thay vì `GetOrdersByUserId`
- ✅ Status mapping đúng: "Đang chờ xử lý" = 1, "Đang chuẩn bị" = 2, etc.
- ✅ camelCase properties trong JavaScript
- ✅ Error handling cho AJAX calls
- ✅ **FIXED**: JSON property conflict với JsonPropertyName và JsonIgnore 