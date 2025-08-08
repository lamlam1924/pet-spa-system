// ===================== JS Danh sách lịch hẹn (Appointment List) =====================

$(document).ready(function() {
    // ===== Xử lý khi mở modal xóa =====
    $('#deleteModal').on('show.bs.modal', function (event) {
        const button = $(event.relatedTarget);
        const id = button.data('id');
        $('#deleteAppointmentId').val(id);
    });

    // ===== Xử lý tìm kiếm theo từ khóa =====
    $('#search-input').on('keyup', function() {
        const value = $(this).val().toLowerCase();
        $("#appointment-table tbody tr").filter(function() {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    // ===== Xử lý lọc theo trạng thái =====
    $('#status-filter').on('change', function() {
        const value = $(this).val();
        if (value === '') {
            $("#appointment-table tbody tr").show();
        } else {
            $("#appointment-table tbody tr").filter(function() {
                const hasStatus = $(this).find('td.status-col .badge-status').data('statusid') == value;
                $(this).toggle(hasStatus);
            });
        }
    });

    // ===== Xử lý lọc theo ngày =====
    $('#date-filter').on('change', function() {
        const value = $(this).val();
        if (value === '') {
            $("#appointment-table tbody tr").show();
        } else {
            const selectedDate = new Date(value);
            $("#appointment-table tbody tr").filter(function() {
                const dateText = $(this).find('td.time-col span').first().text().trim();
                const parts = dateText.split('/');
                const rowDate = new Date(`${parts[1]}/${parts[0]}/${parts[2]}`);
                const isSameDate = rowDate.getDate() === selectedDate.getDate() &&
                                  rowDate.getMonth() === selectedDate.getMonth() &&
                                  rowDate.getFullYear() === selectedDate.getFullYear();
                $(this).toggle(isSameDate);
            });
        }
    });

    // ===== Reset bộ lọc =====
    $('#btn-reset-filter').on('click', function() {
        $('#search-input').val('');
        $('#status-filter').val('');
        $('#date-filter').val('');
        $('#employee-filter').val('');
        $("#appointment-table tbody tr").show();
    });

    // ===== Tooltip cho badge dịch vụ/thú cưng =====
    $(document).on('mouseenter', '.badge-service, .badge-pet', function() {
        $(this).tooltip('show');
    });
    $(document).on('mouseleave', '.badge-service, .badge-pet', function() {
        $(this).tooltip('hide');
    });

    // ===== Spinner loading khi lọc (demo, có thể mở rộng khi dùng ajax) =====
    $(document).ajaxStart(function() {
        $('#loading-spinner').show();
    }).ajaxStop(function() {
        $('#loading-spinner').hide();
    });
});
