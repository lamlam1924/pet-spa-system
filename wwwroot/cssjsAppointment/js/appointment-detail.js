$(document).ready(function() {
    // Thay đổi trạng thái
    $('.status-change').on('click', function(e) {
        e.preventDefault();
        const statusId = $(this).data('status-id');
        const appointmentId = $(this).data('appointment-id');
        const token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: '/AdminAppointment/QuickUpdateStatus',
            type: 'POST',
            data: { id: appointmentId, statusId: statusId },
            headers: { 'RequestVerificationToken': token },
            success: function(response) {
                if (response.success) {
                    location.reload();
                } else {
                    alert(response.message || 'Có lỗi xảy ra khi cập nhật trạng thái.');
                }
            },
            error: function() {
                alert('Đã xảy ra lỗi khi kết nối với máy chủ.');
            }
        });
    });

    // Gửi email xác nhận
    $('#btnSendConfirmation').on('click', function() {
        $(this).html('<i class="fas fa-spinner fa-spin me-1"></i>Đang gửi...');
        $(this).attr('disabled', true);
        setTimeout(() => {
            $(this).html('<i class="fas fa-check-circle me-1"></i>Đã gửi email');
            $(this).removeClass('btn-outline-success').addClass('btn-success');
        }, 2000);
    });
});