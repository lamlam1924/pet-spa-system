// Khởi tạo biến để theo dõi trạng thái
let $statusModal;
let lastValidStatus;

function initStatusValidation() {
    console.log('Initializing status validation...');

    // Khởi tạo modal
    $statusModal = $('#statusCheckModal');
    window.canSubmitForm = true;

    // Lưu giá trị trạng thái ban đầu
    var $statusSelect = $('#StatusId');
    var $form = $('#editAppointmentForm');

    // Lưu trạng thái ban đầu (trạng thái khi form được load)
    window.initialStatus = {
        id: $statusSelect.val(),
        name: $statusSelect.data('current-status') || $statusSelect.find('option:selected').data('status-name')
    };

    console.log('Initial status when form loaded:', window.initialStatus);

    // Đặt lastValidStatus bằng trạng thái ban đầu
    lastValidStatus = { ...initialStatus };

    console.log('Initial status:', initialStatus);

    console.log('Initial status:', lastValidStatus);

    // Xử lý sự kiện thay đổi trạng thái
    $statusSelect.on('change', function () {
        var $select = $(this);
        var currentStatusName = lastValidStatus.name;
        var newStatusName = $select.find('option:selected').data('status-name');
        var newStatusId = $select.val();

        console.log('Status change detected:', {
            currentName: currentStatusName,
            newName: newStatusName,
            newId: newStatusId,
            selectedOption: $select.find('option:selected').text()
        });

        // Lưu trạng thái vừa chọn
        let selectedStatus = {
            id: newStatusId,
            name: newStatusName
        };

        // Log thông tin debug
        console.log('Status selection:', {
            initial: initialStatus,
            current: lastValidStatus,
            selected: selectedStatus
        });

        // Nếu chọn lại trạng thái ban đầu, cho phép
        if (selectedStatus.id === initialStatus.id) {
            lastValidStatus = { ...initialStatus };
            return;
        }

        // Nếu không thay đổi so với trạng thái hợp lệ cuối, không cần kiểm tra
        if (selectedStatus.id === lastValidStatus.id) {
            return;
        }

        // Gọi API để kiểm tra trạng thái hợp lệ
        $.ajax({
            url: '/AdminAppointment/GetValidNextStatuses',
            type: 'GET',
            data: { currentStatus: currentStatusName },
            success: function (validStatuses) {
                console.log('Valid next statuses:', validStatuses);
                var isValid = validStatuses.some(s => s.statusName === newStatusName);
                console.log('Status transition is valid:', isValid);

                if (!isValid) {
                    showStatusValidationModal(currentStatusName, newStatusName, validStatuses);
                    // Quay về trạng thái trước đó
                    setTimeout(() => {
                        $select.val(lastValidStatus.id);
                    }, 0);
                    window.canSubmitForm = false;
                } else {
                    // Cập nhật trạng thái hợp lệ mới nhất
                    lastValidStatus = {
                        id: newStatusId,
                        name: newStatusName
                    };
                    window.canSubmitForm = true;
                    console.log('Updated last valid status:', lastValidStatus);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error checking status validity:', error);
                // Quay về trạng thái trước đó nếu có lỗi
                $select.val(lastValidStatus.id);
            }
        });
    });
}

function showStatusValidationModal(currentStatus, selectedStatus, validStatuses) {
    console.log('Showing status validation modal');

    // Cập nhật nội dung modal
    var $modal = $statusModal;

    // Hiển thị trạng thái hiện tại
    $modal.find('.status-current-text').text(currentStatus);

    // Hiển thị trạng thái đã chọn
    $modal.find('.status-selected-text').text(selectedStatus);

    // Hiển thị danh sách trạng thái hợp lệ
    var $validList = $modal.find('.valid-status-list');
    $validList.empty();

    if (validStatuses && validStatuses.length > 0) {
        validStatuses.forEach(function (status) {
            var $statusOption = $('<div class="status-option mb-2">')
                .append('<i class="fas fa-arrow-right text-success mr-2"></i>' + status.statusName)
                .on('click', function () {
                    console.log('Selecting valid status:', status.statusName);
                    var $select = $('#StatusId');
                    var targetOption = $select.find('option[data-status-name="' + status.statusName + '"]');
                    var targetStatusId = targetOption.val();

                    // Cập nhật trạng thái hợp lệ mới
                    lastValidStatus = {
                        id: targetStatusId,
                        name: status.statusName
                    };

                    // Cập nhật select và đóng modal
                    $select.val(targetStatusId);
                    $modal.modal('hide');
                });

            $validList.append($statusOption);
        });
    } else {
        $validList.html(`
            <div class="text-muted">
                <i class="fas fa-info-circle mr-2"></i>Đây là trạng thái cuối cùng và không thể chuyển sang trạng thái khác
            </div>
        `);
    }

    // Xử lý nút "Giữ trạng thái hiện tại"
    $modal.find('.keep-current-status').off('click').on('click', function () {
        console.log('Keeping original status:', window.initialStatus);
        var $select = $('#StatusId');

        // Quay về trạng thái ban đầu của form
        $select.val(window.initialStatus.id);
        lastValidStatus = { ...window.initialStatus };  // Cập nhật lastValidStatus
        window.canSubmitForm = true;

        // Đảm bảo sự kiện change được kích hoạt
        $select.trigger('change');

        // Đóng modal (sử dụng Bootstrap 4)
        $modal.modal('hide');
    });

    // Xử lý sự kiện đóng modal
    $modal.off('hidden.bs.modal').on('hidden.bs.modal', function () {
        console.log('Modal closed, reverting to original status:', window.initialStatus);
        var $select = $('#StatusId');

        // Quay về trạng thái ban đầu của form
        $select.val(window.initialStatus.id);
        lastValidStatus = { ...window.initialStatus };  // Cập nhật lastValidStatus
        window.canSubmitForm = true;

        // Đảm bảo sự kiện change được kích hoạt
        $select.trigger('change');
    });

    // Hiển thị modal
    $modal.modal('show');
}