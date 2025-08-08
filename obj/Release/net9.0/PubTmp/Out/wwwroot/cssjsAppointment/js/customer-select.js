$(document).ready(function () {
    // Khởi tạo Select2 cho dropdown khách hàng
    $('#CustomerIdSelect').select2({
        width: '100%',
        placeholder: 'Chọn khách hàng',
        allowClear: true,
        dropdownParent: $('#CustomerIdSelect').closest('.mb-3')
    });
    // Hàm fill thông tin khách hàng
    function fillCustomerInfo() {
        var selected = $('#CustomerIdSelect').find('option:selected');
        $('#CustomerPhone').val(selected.data('phone') || '');
        $('#CustomerEmail').val(selected.data('email') || '');
        $('#CustomerAddress').val(selected.data('address') || '');
    }
    // Fill thông tin khi chọn khách hàng
    $('#CustomerIdSelect').on('change', fillCustomerInfo);
    // Fill thông tin ngay khi load trang (lần đầu)
    fillCustomerInfo();
});
