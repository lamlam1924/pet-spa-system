# Order History Management System

## Tổng quan
Hệ thống quản lý lịch sử đơn hàng cho admin với các chức năng CRUD đầy đủ, sử dụng server-side rendering thay vì JavaScript.

## Các chức năng đã implement

### 1. View Orders (Xem đơn hàng)
- **Chức năng**: Hiển thị danh sách tất cả đơn hàng
- **Controller**: `AdminController.OrderHistory()`
- **Route**: `GET /Admin/OrderHistory`
- **Tính năng**:
  - Hiển thị thông tin đơn hàng: ID, khách hàng, sản phẩm, số lượng, tổng tiền, trạng thái, ngày đặt, địa chỉ giao hàng
  - Server-side rendering với Razor syntax
  - Phân trang và tìm kiếm client-side
  - Lọc theo trạng thái đơn hàng
  - Hiển thị tổng số sản phẩm thay vì chỉ số lượng của sản phẩm đầu tiên

### 2. View Order Details (Xem chi tiết đơn hàng)
- **Chức năng**: Xem thông tin chi tiết của một đơn hàng
- **Controller**: `AdminController.GetOrderDetail()`
- **Route**: `GET /Admin/GetOrderDetail`
- **Tính năng**:
  - Modal popup với thông tin chi tiết
  - **Hiển thị TẤT CẢ sản phẩm** trong đơn hàng (không chỉ sản phẩm đầu tiên)
  - AJAX call để lấy chi tiết đầy đủ
  - Nút chuyển sang chỉnh sửa
  - Hiển thị đầy đủ thông tin: tên sản phẩm, số lượng, đơn giá, tổng tiền cho từng sản phẩm

### 3. Edit Order Status (Chỉnh sửa trạng thái đơn hàng)
- **Chức năng**: Cập nhật trạng thái đơn hàng
- **Controller**: `AdminController.UpdateOrderStatus()`
- **Route**: `POST /Admin/UpdateOrderStatus`
- **Tính năng**:
  - Chỉ cho phép cập nhật trạng thái (không sửa thông tin khác)
  - Validation: Không cho phép thay đổi trạng thái đơn hàng đã giao hoặc đã hủy
  - Các trạng thái: Đang xử lí, Đang chuẩn bị, Đã giao vận chuyển, Đã giao, Đã hủy
  - Form submission với POST method

### 4. Delete Order (Xóa đơn hàng)
- **Chức năng**: Xóa đơn hàng
- **Controller**: `AdminController.DeleteOrder()`
- **Route**: `POST /Admin/DeleteOrder`
- **Tính năng**:
  - Validation: Không cho phép xóa đơn hàng đã giao vận chuyển hoặc đã giao
  - Xác nhận trước khi xóa với modal
  - Xóa các payment liên quan
  - Form submission với POST method

## Cấu trúc file

### Controllers
- `Controllers/AdminController.cs`: Controller chính xử lý tất cả actions
- `Controllers/AdminOrder/AdminOrderHistoryController.cs`: Controller phụ (đã rename để tránh conflict)

### Services
- `Services/OrderService.cs`: Business logic cho đơn hàng
- `Services/IOrderService.cs`: Interface cho OrderService

### Repositories
- `Repositories/OrderRepository.cs`: Data access layer cho đơn hàng
- `Repositories/IOrderRepository.cs`: Interface cho OrderRepository

### ViewModels
- `ViewModel/OrderViewModel.cs`: Model cho hiển thị đơn hàng với camelCase properties cho JavaScript

### Views
- `Views/Admin/OrderHistory.cshtml`: Giao diện quản lý đơn hàng với server-side rendering

## Thay đổi chính từ JavaScript sang Server-side

### Trước đây (JavaScript):
- Load dữ liệu bằng AJAX từ `/OrderHistory/GetAllOrders`
- Render table bằng JavaScript
- Xử lý CRUD bằng AJAX calls

### Hiện tại (Server-side):
- Load dữ liệu trực tiếp trong controller: `var orders = _orderService.GetAllOrders()`
- Render table bằng Razor syntax: `@foreach (var order in Model)`
- Xử lý CRUD bằng form submission với POST method
- Sử dụng TempData để hiển thị thông báo
- **Mới**: AJAX call để lấy chi tiết đơn hàng với tất cả sản phẩm

## Cải tiến mới cho Order Detail

### Hiển thị sản phẩm trong table:
- **Trước**: Chỉ hiển thị sản phẩm đầu tiên
- **Hiện tại**: 
  - Nếu 1 sản phẩm: Hiển thị tên sản phẩm
  - Nếu nhiều sản phẩm: Hiển thị sản phẩm đầu + "+X more"

### Hiển thị số lượng:
- **Trước**: Chỉ hiển thị số lượng sản phẩm đầu tiên
- **Hiện tại**: Hiển thị tổng số lượng tất cả sản phẩm

### Order Detail Modal:
- **Trước**: Chỉ hiển thị thông tin cơ bản
- **Hiện tại**: 
  - Hiển thị tất cả sản phẩm trong đơn hàng
  - Chi tiết từng sản phẩm: tên, số lượng, đơn giá, tổng tiền
  - AJAX call để lấy dữ liệu đầy đủ
  - Loading state và error handling

## Validation Rules

### Update Status
- Không thể thay đổi trạng thái đơn hàng đã giao (StatusId = 4)
- Không thể thay đổi trạng thái đơn hàng đã hủy (StatusId = 5)

### Delete Order
- Không thể xóa đơn hàng đã giao vận chuyển (StatusId = 3)
- Không thể xóa đơn hàng đã giao (StatusId = 4)

## Status Mapping
- 1: Đang xử lí
- 2: Đang chuẩn bị
- 3: Đã giao vận chuyển
- 4: Đã giao
- 5: Đã hủy

## UI Features
- **Server-side rendering**: Dữ liệu được render trực tiếp từ server
- **Alert messages**: Thông báo thành công/lỗi bằng TempData
- **Search & Filter**: Tìm kiếm và lọc đơn hàng (client-side)
- **Responsive design**: Giao diện responsive
- **Modal dialogs**: Popup cho xem chi tiết và chỉnh sửa
- **Disabled buttons**: Vô hiệu hóa nút không phù hợp với trạng thái
- **Form submission**: Sử dụng HTML forms thay vì AJAX
- **Multi-product display**: Hiển thị tất cả sản phẩm trong đơn hàng
- **AJAX for details**: Lấy chi tiết đầy đủ bằng AJAX call

## Error Handling
- Try-catch blocks trong tất cả controller methods
- User-friendly error messages qua TempData
- Proper HTTP status codes
- Client-side validation
- AJAX error handling cho order details

## Security
- Input validation
- SQL injection prevention (Entity Framework)
- XSS prevention
- CSRF protection với form submission

## Lợi ích của Server-side Rendering
1. **Hiệu suất tốt hơn**: Không cần AJAX calls cho danh sách
2. **SEO friendly**: Dữ liệu được render sẵn
3. **Đơn giản hơn**: Ít JavaScript code
4. **Bảo mật tốt hơn**: Validation ở server-side
5. **Tương thích tốt hơn**: Hoạt động ngay cả khi JavaScript bị tắt
6. **Multi-product support**: Hiển thị đầy đủ tất cả sản phẩm trong đơn hàng 