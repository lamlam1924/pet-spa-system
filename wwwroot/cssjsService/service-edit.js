// JavaScript for Service Edit
$(document).ready(function() {
    // Cập nhật xem trước khi nhập liệu
    function updatePreview() {
        const name = $('#serviceName').val();
        const categoryId = $('#serviceCategory').val();
        const categoryText = categoryId ? $('#serviceCategory option:selected').text() : 'Danh mục dịch vụ';
        const price = $('#servicePrice').val();
        const duration = $('#serviceDuration').val();
        const description = $('#serviceDescription').val() || 'Chưa có mô tả chi tiết cho dịch vụ này.';
        const isActive = $('#serviceStatus').prop('checked');
        
        // Cập nhật xem trước
        $('#previewName').text(name);
        $('#previewCategory').text(categoryText);
        $('#previewPrice').text(new Intl.NumberFormat('vi-VN').format(price) + ' đ');
        $('#previewDuration').text(duration + ' phút');
        $('#previewDescription').text(description);
        
        if (isActive) {
            $('#previewStatus').removeClass('badge-secondary').addClass('badge-success').text('Hoạt động');
        } else {
            $('#previewStatus').removeClass('badge-success').addClass('badge-secondary').text('Tạm ngưng');
        }
    }
    
    // Gọi hàm cập nhật xem trước khi thay đổi giá trị
    $('#serviceName, #serviceCategory, #servicePrice, #serviceDuration, #serviceDescription, #serviceStatus').on('input change', updatePreview);
    
    // Xử lý form submit
    $('#editServiceForm').on('submit', function(e) {
        // Kiểm tra form hợp lệ
        if (this.checkValidity() === false) {
            e.preventDefault();
            e.stopPropagation();
            $(this).addClass('was-validated');
            return false;
        }
        
        // Thông báo đang xử lý
        Swal.fire({
            title: 'Đang xử lý',
            text: 'Vui lòng chờ trong giây lát...',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
    });
    
    // Nút xóa dịch vụ
    $('#btnDeleteService').on('click', function() {
        $('#deleteServiceModal').modal('show');
    });
    
    // Hiển thị thông báo thành công (được gọi từ view khi cần)
    function showSuccessMessage(message) {
        Swal.fire({
            icon: 'success',
            title: 'Thành công!',
            text: message,
            confirmButtonText: 'OK'
        });
    }
    
    // Hiển thị thông báo lỗi (được gọi từ view khi cần)
    function showErrorMessage(message) {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi!',
            text: message
        });
    }
});

// Reset form về giá trị ban đầu
function resetForm(serviceData) {
    setTimeout(function() {
        $('#serviceName').val(serviceData.name);
        $('#serviceCategory').val(serviceData.categoryId);
        $('#servicePrice').val(serviceData.price);
        $('#serviceDuration').val(serviceData.duration);
        $('#serviceDescription').val(serviceData.description);
        $('#serviceStatus').prop('checked', serviceData.isActive);
        
        // Cập nhật lại preview
        $('#previewName').text(serviceData.name);
        $('#previewCategory').text(serviceData.categoryName);
        $('#previewPrice').text(serviceData.formattedPrice);
        $('#previewDuration').text(serviceData.duration + ' phút');
        $('#previewDescription').text(serviceData.description || 'Chưa có mô tả chi tiết cho dịch vụ này.');
        
        if (serviceData.isActive) {
            $('#previewStatus').removeClass('badge-secondary').addClass('badge-success').text('Hoạt động');
        } else {
            $('#previewStatus').removeClass('badge-success').addClass('badge-secondary').text('Tạm ngưng');
        }
    }, 10);
}
