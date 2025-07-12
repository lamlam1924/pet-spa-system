$(document).ready(function() {
    // Xác nhận duyệt/từ chối bằng modal
    $(document).on('submit', '.approval-action-form', function(e) {
        var form = this;
        e.preventDefault();
        var action = $(form).find('button[type=submit]').text().trim();
        if (confirm('Bạn có chắc chắn muốn ' + action.toLowerCase() + ' lịch hẹn này?')) {
            form.submit();
        }
    });
});
