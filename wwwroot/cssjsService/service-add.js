// Script for Add Service page
$(document).ready(function() {
    // Cập nhật xem trước khi nhập liệu
    function updatePreview() {
        const name = $('#serviceName').val() || 'Tên dịch vụ';
        const categoryId = $('#serviceCategory').val();
        const categoryText = categoryId ? $('#serviceCategory option:selected').text() : 'Danh mục dịch vụ';
        const price = $('#servicePrice').val() || 0;
        const duration = $('#serviceDuration').val() || 0;
        const description = $('#serviceDescription').val() || 'Mô tả chi tiết dịch vụ sẽ hiển thị ở đây...';
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
    
    // Reset form và xem trước
    $('button[type="reset"]').on('click', function() {
        setTimeout(updatePreview, 10);
    });
    
    // Khởi tạo xem trước
    updatePreview();
});
