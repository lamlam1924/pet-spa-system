# Toast Notifications - Hướng dẫn sử dụng

## Tổng quan
Hệ thống toast notifications đã được tích hợp để thay thế các alert JavaScript thông thường, cung cấp trải nghiệm người dùng tốt hơn với các thông báo đẹp mắt và có thể tùy chỉnh.

## Cách sử dụng

### 1. Hiển thị toast cơ bản
```javascript
// Hiển thị toast thành công
showToast('Thao tác thành công!', 'success');

// Hiển thị toast lỗi
showToast('Có lỗi xảy ra!', 'error');

// Hiển thị toast cảnh báo
showToast('Vui lòng kiểm tra lại thông tin!', 'warning');

// Hiển thị toast thông tin
showToast('Đây là thông tin quan trọng!', 'info');
```

### 2. Sử dụng ToastUtils trực tiếp
```javascript
// Hiển thị toast với thời gian tùy chỉnh (5 giây)
ToastUtils.show('Thông báo tùy chỉnh', 'success', 5000);

// Hiển thị toast thành công
ToastUtils.success('Thao tác thành công!');

// Hiển thị toast lỗi
ToastUtils.error('Có lỗi xảy ra!');

// Hiển thị toast cảnh báo
ToastUtils.warning('Cảnh báo!');

// Hiển thị toast thông tin
ToastUtils.info('Thông tin!');
```

### 3. Hiển thị dialog xác nhận
```javascript
showConfirm('Bạn có chắc chắn muốn xóa?', 
    function() {
        // Hành động khi người dùng xác nhận
        console.log('Đã xác nhận');
    }, 
    function() {
        // Hành động khi người dùng hủy
        console.log('Đã hủy');
    }
);
```

### 4. Sử dụng ToastUtils.confirm
```javascript
ToastUtils.confirm('Bạn có chắc chắn muốn xóa?', 
    function() {
        // Hành động khi xác nhận
        console.log('Đã xác nhận');
    }, 
    function() {
        // Hành động khi hủy
        console.log('Đã hủy');
    }
);
```

## Các loại toast

### 1. Success (Thành công)
- Màu: Xanh lá (#28a745)
- Icon: fa-check-circle
- Tự động ẩn sau 2 giây

### 2. Error (Lỗi)
- Màu: Đỏ (#dc3545)
- Icon: fa-exclamation-circle
- Không tự động ẩn

### 3. Warning (Cảnh báo)
- Màu: Vàng (#ffc107)
- Icon: fa-exclamation-triangle
- Không tự động ẩn

### 4. Info (Thông tin)
- Màu: Xanh dương (#17a2b8)
- Icon: fa-info-circle
- Không tự động ẩn

## Tùy chỉnh

### Thay đổi vị trí hiển thị
```css
#toast-container {
    position: fixed;
    top: 20px;          /* Khoảng cách từ trên */
    right: 20px;        /* Khoảng cách từ phải */
    z-index: 9999;
    max-width: 350px;
}
```

### Thay đổi thời gian hiển thị
```javascript
// Toast thành công tự động ẩn sau 2 giây
ToastUtils.success('Thành công!', 2000);

// Toast lỗi không tự động ẩn
ToastUtils.error('Lỗi!', 0);
```

### Tùy chỉnh style
```css
.toast-notification {
    background: #fff;
    color: #333;
    padding: 15px 20px;
    margin-bottom: 10px;
    border-radius: 8px;
    box-shadow: 0 4px 12px rgba(0,0,0,0.15);
    /* Thêm các style tùy chỉnh khác */
}
```

## Migration từ alert

### Trước (alert cũ)
```javascript
alert('Có lỗi xảy ra!');
```

### Sau (toast mới)
```javascript
showToast('Có lỗi xảy ra!', 'error');
```

## Lưu ý quan trọng

1. **File CSS và JS cần thiết:**
   - `toast-notifications.css` - Styling cho toast
   - `toast-utils.js` - Logic xử lý toast

2. **Dependencies:**
   - Font Awesome (cho icons)
   - jQuery (cho một số tính năng)

3. **Responsive:**
   - Toast tự động điều chỉnh trên mobile
   - Vị trí và kích thước thay đổi theo màn hình

4. **Accessibility:**
   - Toast có thể đóng bằng nút X
   - Hỗ trợ keyboard navigation
   - Screen reader friendly

## Ví dụ thực tế

### Trong AJAX call
```javascript
$.ajax({
    url: '/api/data',
    success: function(response) {
        if (response.success) {
            showToast('Dữ liệu đã được cập nhật!', 'success');
        } else {
            showToast('Có lỗi xảy ra: ' + response.message, 'error');
        }
    },
    error: function() {
        showToast('Không thể kết nối đến máy chủ!', 'error');
    }
});
```

### Trong form validation
```javascript
if (!email || !password) {
    showToast('Vui lòng điền đầy đủ thông tin!', 'warning');
    return;
}
```

### Trong delete confirmation
```javascript
showConfirm('Bạn có chắc muốn xóa?', 
    function() {
        // Thực hiện xóa
        deleteItem();
    }
);
``` 