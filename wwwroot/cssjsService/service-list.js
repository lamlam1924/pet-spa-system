// JavaScript for Service List
$(document).ready(function() {
    // Khởi tạo tooltip
    $('[data-toggle="tooltip"]').tooltip();
    
    // Xử lý nút in danh sách
    $('#btnPrintList').on('click', function(e) {
        e.preventDefault();
        window.print();
    });
    
    // Hiển thị thông báo thành công (được gọi từ view khi cần)
    function showSuccessMessage(message) {
        Swal.fire({
            icon: 'success',
            title: 'Thành công',
            text: message,
            timer: 3000,
            timerProgressBar: true
        });
    }
    
    // Hiển thị thông báo lỗi (được gọi từ view khi cần)
    function showErrorMessage(message) {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: message
        });
    }
});

// Hàm xác nhận thay đổi trạng thái
function confirmStatusChange(serviceId, activateService, restoreUrl, softDeleteUrl) {
    const action = activateService ? 'kích hoạt' : 'tạm ngưng';
    const actionUrl = activateService ? restoreUrl : softDeleteUrl;
        
    Swal.fire({
        title: `Xác nhận ${action}`,
        text: `Bạn có chắc chắn muốn ${action} dịch vụ này không?`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: activateService ? '#28a745' : '#dc3545',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Xác nhận',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = `${actionUrl}?id=${serviceId}`;
        }
    });
}
