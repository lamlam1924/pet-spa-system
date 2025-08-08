// Staff Appointments JavaScript

// Global variables
let currentAppointments = [];

// Utility functions
function getPetNames(appointmentPets) {
    if (!appointmentPets || appointmentPets.length === 0) return 'N/A';
    return appointmentPets.map(ap => ap.pet?.name || 'Không rõ tên').join(', ');
}

function getServiceNames(appointmentServices) {
    if (!appointmentServices || appointmentServices.length === 0) return 'N/A';
    return appointmentServices.map(as => {
        const statusName = getServiceStatusName(as.status);
        const statusBadge = `<span class="badge bg-${getServiceStatusColor(statusName)} ms-2">${statusName}</span>`;
        return `${as.service?.name || 'Không rõ dịch vụ'} ${statusBadge}`;
    }).join('<br>');
}

function getServiceStatusName(statusId) {
    switch (statusId) {
        case 1: return 'Đang chờ xử lý';
        case 2: return 'Đã xác nhận';
        case 3: return 'Đang thực hiện';
        case 4: return 'Đã hủy';
        case 5: return 'Hoàn thành';
        default: return 'Không rõ';
    }
}

function translateStatus(englishStatus) {
    switch (englishStatus) {
        case 'Pending': return 'Đang chờ xử lý';
        case 'Confirmed': return 'Đã xác nhận';
        case 'InProgress': return 'Đang thực hiện';
        case 'Completed': return 'Hoàn thành';
        case 'Cancelled': return 'Đã hủy';
        default: return englishStatus;
    }
}

function getStatusColor(status) {
    switch (status) {
        case 'Đang chờ xử lý': return 'warning';
        case 'Đã xác nhận': return 'info';
        case 'Đang thực hiện': return 'primary';
        case 'Hoàn thành': return 'success';
        case 'Đã hủy': return 'danger';
        default: return 'secondary';
    }
}

function getServiceStatusColor(status) {
    switch (status) {
        case 'Pending': return 'warning';
        case 'In Progress': return 'primary';
        case 'Completed': return 'success';
        case 'Cancelled': return 'danger';
        default: return 'secondary';
    }
}

function formatDateTime(dateString) {
    const date = new Date(dateString);
    return date.toLocaleString('vi-VN', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
    });
}

// Appointment detail functions
function viewAppointmentDetail(appointmentId) {
    console.log('viewAppointmentDetail called with ID:', appointmentId);
    $.get('/Staff/GetMyAppointments', function(response) {
        console.log('Response received:', response);
        if (response.success && response.data) {
            const appointment = response.data.find(a => a.appointmentId === appointmentId);
            console.log('Appointment found:', appointment);
            if (appointment) {
                showAppointmentDetailModal(appointment);
            } else {
                showNotification('Không tìm thấy lịch hẹn', 'error');
            }
        } else {
            showNotification(response.message || 'Không thể tải chi tiết lịch hẹn', 'error');
        }
    }).fail(function(xhr, status, error) {
        console.error('AJAX failed:', status, error);
        showNotification('Không thể tải chi tiết lịch hẹn', 'error');
    });
}

function showAppointmentDetailModal(appointment) {
    console.log('showAppointmentDetailModal called with:', appointment);

    const modalContent = createAppointmentDetailContent(appointment);
    $('#appointmentDetailContent').html(modalContent);

    // Add action buttons
    const actions = createAppointmentActions(appointment);
    $('#appointmentActions').html(actions);

    $('#appointmentDetailModal').modal('show');
}

function createAppointmentDetailContent(appointment) {
    return `
        <div class="row">
            <div class="col-md-6">
                <h6 class="text-primary">
                    <i class="fas fa-user me-2"></i>Thông tin khách hàng
                </h6>
                <table class="table table-sm">
                    <tr>
                        <td><strong>Tên:</strong></td>
                        <td>${appointment.user?.fullName || 'N/A'}</td>
                    </tr>
                    <tr>
                        <td><strong>Email:</strong></td>
                        <td>${appointment.user?.email || 'N/A'}</td>
                    </tr>
                    <tr>
                        <td><strong>Điện thoại:</strong></td>
                        <td>${appointment.user?.phone || 'N/A'}</td>
                    </tr>
                    <tr>
                        <td><strong>Mã lịch hẹn:</strong></td>
                        <td>#${appointment.appointmentId}</td>
                    </tr>
                </table>
            </div>
            <div class="col-md-6">
                <h6 class="text-primary">
                    <i class="fas fa-calendar me-2"></i>Thông tin lịch hẹn
                </h6>
                <table class="table table-sm">
                    <tr>
                        <td><strong>Ngày giờ:</strong></td>
                        <td>${formatDate(appointment.appointmentDate)} ${appointment.startTime} - ${appointment.endTime}</td>
                    </tr>
                    <tr>
                        <td><strong>Trạng thái:</strong></td>
                        <td>
                            <span class="badge bg-${getStatusColor(translateStatus(appointment.status?.statusName))}">
                                ${translateStatus(appointment.status?.statusName) || 'N/A'}
                            </span>
                        </td>
                    </tr>
                    ${appointment.notes ? `
                    <tr>
                        <td><strong>Ghi chú:</strong></td>
                        <td class="text-danger">${appointment.notes}</td>
                    </tr>
                    ` : ''}
                </table>
            </div>
        </div>
        <hr>
        <div class="row">
            <div class="col-md-6">
                <h6 class="text-primary">
                    <i class="fas fa-paw me-2"></i>Thú cưng
                </h6>
                <div class="border rounded p-2 bg-light">
                    ${getPetNames(appointment.appointmentPets)}
                </div>
            </div>
            <div class="col-md-6">
                <h6 class="text-primary">
                    <i class="fas fa-spa me-2"></i>Dịch vụ
                </h6>
                <div class="border rounded p-2 bg-light">
                    ${getServiceNames(appointment.appointmentServices)}
                </div>
            </div>
        </div>
        ${appointment.notes ? `
            <hr>
            <h6 class="text-primary">
                <i class="fas fa-sticky-note me-2"></i>Ghi chú
            </h6>
            <div class="border rounded p-2 bg-light">
                ${appointment.notes}
            </div>
        ` : ''}
    `;
}

function createAppointmentActions(appointment) {
    const status = translateStatus(appointment.status?.statusName);
    let actions = '';
    
    if (status === 'Đã xác nhận') {
        actions += `
            <button type="button" class="btn btn-primary me-2" onclick="quickUpdateStatus(${appointment.appointmentId}, 3)">
                <i class="fas fa-play me-1"></i>Bắt đầu thực hiện
            </button>
            <button type="button" class="btn btn-warning me-2" onclick="quickUpdateStatus(${appointment.appointmentId}, 5)">
                <i class="fas fa-times me-1"></i>Hủy lịch
            </button>
        `;
    } else if (status === 'Đang thực hiện') {
        actions += `
            <button type="button" class="btn btn-success me-2" onclick="quickUpdateStatus(${appointment.appointmentId}, 4)">
                <i class="fas fa-check me-1"></i>Hoàn thành
            </button>
        `;
    }

    if (['Confirmed', 'In Progress'].includes(status)) {
        actions += `
            <button type="button" class="btn btn-outline-secondary" onclick="showStatusUpdateModal(${appointment.appointmentId})">
                <i class="fas fa-edit me-1"></i>Cập nhật trạng thái
            </button>
        `;
    }
    
    return actions;
}

// Status update functions
function showStatusUpdateModal(appointmentId) {
    $('#updateAppointmentId').val(appointmentId);
    $('#newStatus').val('');
    $('#cancelReason').val('');
    $('#cancelReasonGroup').hide();
    $('#appointmentDetailModal').modal('hide');
    $('#statusUpdateModal').modal('show');
}

function quickUpdateStatus(appointmentId, statusId) {
    if (statusId === 4 && !confirm('Xác nhận hoàn thành lịch hẹn này?')) {
        return;
    }
    
    updateStatus(appointmentId, statusId);
}

function updateAppointmentStatus() {
    const appointmentId = $('#updateAppointmentId').val();
    const statusId = $('#newStatus').val();
    const cancelReason = $('#cancelReason').val();
    
    if (!statusId) {
        showNotification('Vui lòng chọn trạng thái', 'warning');
        return;
    }
    
    if (statusId === '5' && !cancelReason.trim()) {
        showNotification('Vui lòng nhập lý do hủy', 'warning');
        return;
    }
    
    updateStatus(appointmentId, parseInt(statusId), cancelReason);
}

function updateStatus(appointmentId, statusId, reason = '') {
    const data = {
        appointmentId: appointmentId,
        statusId: statusId
    };
    
    if (reason) {
        data.reason = reason;
    }
    
    $.post('/Staff/UpdateAppointmentStatus', data, function(response) {
        if (response.success) {
            showNotification('Cập nhật trạng thái thành công', 'success');
            $('#statusUpdateModal').modal('hide');
            $('#appointmentDetailModal').modal('hide');
            loadAppointments(); // Reload the appointments list
        } else {
            showNotification(response.message || 'Có lỗi xảy ra', 'error');
        }
    }).fail(function() {
        showNotification('Không thể cập nhật trạng thái', 'error');
    });
}

// Notification function
function showNotification(message, type = 'info') {
    const alertClass = type === 'error' ? 'alert-danger' : 
                      type === 'success' ? 'alert-success' : 
                      type === 'warning' ? 'alert-warning' : 'alert-info';
    
    const notification = $(`
        <div class="alert ${alertClass} alert-dismissible fade show position-fixed" 
             style="top: 20px; right: 20px; z-index: 9999; min-width: 300px;">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `);
    
    $('body').append(notification);
    
    // Auto-remove after 5 seconds
    setTimeout(function() {
        notification.alert('close');
    }, 5000);
}

// Export functions for global use
window.viewAppointmentDetail = viewAppointmentDetail;
window.showStatusUpdateModal = showStatusUpdateModal;
window.updateAppointmentStatus = updateAppointmentStatus;
window.quickUpdateStatus = quickUpdateStatus;
