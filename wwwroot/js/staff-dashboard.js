// Staff Dashboard JavaScript

$(document).ready(function() {
    initializeDashboard();
    loadPersonalStats();
    setupEventHandlers();
});

function initializeDashboard() {
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Auto-refresh appointments every 5 minutes
    setInterval(function() {
        refreshAppointments();
    }, 300000);
}

function loadPersonalStats() {
    $.get('/Staff/GetPersonalStats', function(response) {
        if (response.success && response.data) {
            updateStatsDisplay(response.data);
        }
    }).fail(function() {
        console.error('Failed to load personal stats');
    });
}

function updateStatsDisplay(stats) {
    // Update completion rate if available
    if (stats.completionRate !== undefined) {
        $('.completion-rate').text(stats.completionRate.toFixed(1) + '%');
    }
    
    // Update revenue if available
    if (stats.revenue !== undefined) {
        $('.revenue-amount').text(formatCurrency(stats.revenue));
    }
    
    // Update customer count
    if (stats.uniqueCustomers !== undefined) {
        $('.unique-customers').text(stats.uniqueCustomers);
    }
}

function setupEventHandlers() {
    // Appointment status update
    $(document).on('click', '.btn-complete-appointment', function() {
        const appointmentId = $(this).data('appointment-id');
        completeAppointment(appointmentId);
    });

    // Appointment detail view
    $(document).on('click', '.btn-view-appointment', function() {
        const appointmentId = $(this).data('appointment-id');
        viewAppointmentDetail(appointmentId);
    });

    // Quick action buttons
    $('.btn-quick-action').on('click', function() {
        const action = $(this).data('action');
        handleQuickAction(action);
    });
}

function refreshAppointments() {
    const today = new Date().toISOString().split('T')[0];
    
    $.get('/Staff/MyAppointments', { date: today }, function(response) {
        if (response.success) {
            updateAppointmentsTable(response.data);
        }
    }).fail(function() {
        showNotification('Không thể tải lại lịch hẹn', 'error');
    });
}

function updateAppointmentsTable(appointments) {
    const tbody = $('.appointments-table tbody');
    tbody.empty();
    
    if (appointments.length === 0) {
        tbody.append(`
            <tr>
                <td colspan="6" class="text-center py-4">
                    <i class="fas fa-calendar-times fa-2x text-muted mb-2"></i>
                    <p class="text-muted mb-0">Không có lịch hẹn nào hôm nay</p>
                </td>
            </tr>
        `);
        return;
    }
    
    appointments.forEach(function(appointment) {
        const row = createAppointmentRow(appointment);
        tbody.append(row);
    });
}

function createAppointmentRow(appointment) {
    const statusColor = getStatusColor(appointment.status?.statusName);
    const time = new Date(appointment.appointmentDate).toLocaleTimeString('vi-VN', { 
        hour: '2-digit', 
        minute: '2-digit' 
    });
    
    return `
        <tr>
            <td>${time}</td>
            <td>${appointment.user?.fullName || 'N/A'}</td>
            <td>${getPetNames(appointment.appointmentPets)}</td>
            <td>${getServiceNames(appointment.appointmentServices)}</td>
            <td>
                <span class="badge bg-${statusColor}">
                    ${appointment.status?.statusName || 'N/A'}
                </span>
            </td>
            <td>
                <button class="btn btn-sm btn-outline-primary btn-view-appointment" 
                        data-appointment-id="${appointment.appointmentId}">
                    <i class="fas fa-eye"></i>
                </button>
                ${(appointment.status?.statusName === 'Confirmed' || appointment.status?.statusName === 'Đã xác nhận') ?
                    `<button class="btn btn-sm btn-success btn-complete-appointment"
                             data-appointment-id="${appointment.appointmentId}">
                        <i class="fas fa-check"></i>
                    </button>` : ''
                }
            </td>
        </tr>
    `;
}

function getPetNames(appointmentPets) {
    if (!appointmentPets || appointmentPets.length === 0) return 'N/A';
    return appointmentPets.map(ap => ap.pet?.name || 'N/A').join(', ');
}

function getServiceNames(appointmentServices) {
    if (!appointmentServices || appointmentServices.length === 0) return 'N/A';
    return appointmentServices.map(as => as.service?.serviceName || 'N/A').join(', ');
}

function getStatusColor(status) {
    switch (status) {
        case 'Pending':
        case 'Đang chờ xử lý': return 'warning';
        case 'Confirmed':
        case 'Đã xác nhận': return 'info';
        case 'In Progress':
        case 'Đang thực hiện': return 'primary';
        case 'Completed':
        case 'Hoàn thành': return 'success';
        case 'Cancelled':
        case 'Đã hủy': return 'danger';
        default: return 'secondary';
    }
}

function completeAppointment(appointmentId) {
    if (!confirm('Xác nhận hoàn thành lịch hẹn này?')) {
        return;
    }

    updateAppointmentStatus(appointmentId, 4);
}

function updateAppointmentStatus(appointmentId, statusId) {
    let confirmMessage = '';
    switch(statusId) {
        case 3: confirmMessage = 'Xác nhận bắt đầu thực hiện lịch hẹn này?'; break;
        case 4: confirmMessage = 'Xác nhận hoàn thành lịch hẹn này?'; break;
        case 5: confirmMessage = 'Xác nhận hủy lịch hẹn này?'; break;
        default: confirmMessage = 'Xác nhận cập nhật trạng thái?'; break;
    }

    if (!confirm(confirmMessage)) {
        return;
    }

    $.post('/Staff/UpdateAppointmentStatus', {
        appointmentId: appointmentId,
        statusId: statusId
    }, function(response) {
        if (response.success) {
            showNotification('Cập nhật trạng thái thành công', 'success');
            // Reload trang để cập nhật toàn bộ dữ liệu
            setTimeout(function() {
                location.reload();
            }, 1000);
        } else {
            showNotification(response.message || 'Có lỗi xảy ra', 'error');
        }
    }).fail(function() {
        showNotification('Không thể cập nhật trạng thái', 'error');
    });
}

function viewAppointmentDetail(appointmentId) {
    $.get('/Staff/AppointmentDetail', { id: appointmentId }, function(response) {
        if (response.success) {
            showAppointmentDetailModal(response.data);
        } else {
            showNotification(response.message || 'Không thể tải chi tiết lịch hẹn', 'error');
        }
    }).fail(function() {
        showNotification('Không thể tải chi tiết lịch hẹn', 'error');
    });
}

function showAppointmentDetailModal(appointment) {
    // Create and show modal with appointment details
    const modalHtml = createAppointmentDetailModal(appointment);
    $('body').append(modalHtml);
    $('#appointmentDetailModal').modal('show');
    
    // Remove modal from DOM when hidden
    $('#appointmentDetailModal').on('hidden.bs.modal', function() {
        $(this).remove();
    });
}

function createAppointmentDetailModal(appointment) {
    return `
        <div class="modal fade" id="appointmentDetailModal" tabindex="-1">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Chi tiết lịch hẹn #${appointment.appointmentId}</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-md-6">
                                <h6>Thông tin khách hàng</h6>
                                <p><strong>Tên:</strong> ${appointment.user?.fullName || 'N/A'}</p>
                                <p><strong>Email:</strong> ${appointment.user?.email || 'N/A'}</p>
                                <p><strong>Điện thoại:</strong> ${appointment.user?.phone || 'N/A'}</p>
                            </div>
                            <div class="col-md-6">
                                <h6>Thông tin lịch hẹn</h6>
                                <p><strong>Ngày giờ:</strong> ${new Date(appointment.appointmentDate).toLocaleString('vi-VN')}</p>
                                <p><strong>Trạng thái:</strong> 
                                    <span class="badge bg-${getStatusColor(appointment.status?.statusName)}">
                                        ${appointment.status?.statusName || 'N/A'}
                                    </span>
                                </p>
                            </div>
                        </div>
                        <hr>
                        <h6>Thú cưng</h6>
                        <p>${getPetNames(appointment.appointmentPets)}</p>
                        <h6>Dịch vụ</h6>
                        <p>${getServiceNames(appointment.appointmentServices)}</p>
                        ${appointment.notes ? `<h6>Ghi chú</h6><p>${appointment.notes}</p>` : ''}
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
                        ${(appointment.status?.statusName === 'Confirmed' || appointment.status?.statusName === 'Đã xác nhận') ?
                            `<button type="button" class="btn btn-primary me-2" onclick="updateAppointmentStatus(${appointment.appointmentId}, 3); $('#appointmentDetailModal').modal('hide');">
                                <i class="fas fa-play me-1"></i>Bắt đầu thực hiện
                            </button>
                            <button type="button" class="btn btn-warning" onclick="updateAppointmentStatus(${appointment.appointmentId}, 5); $('#appointmentDetailModal').modal('hide');">
                                <i class="fas fa-times me-1"></i>Hủy lịch
                            </button>` : ''
                        }
                    </div>
                </div>
            </div>
        </div>
    `;
}

function handleQuickAction(action) {
    switch (action) {
        case 'view-all-appointments':
            window.location.href = '/Staff/MyAppointments';
            break;
        case 'update-profile':
            window.location.href = '/Staff/Profile';
            break;
        case 'mark-notifications-read':
            markNotificationsRead();
            break;
        default:
            console.log('Unknown action:', action);
    }
}

function markNotificationsRead() {
    $.post('/Staff/MarkNotificationsAsRead', function(response) {
        if (response.success) {
            showNotification('Đã đánh dấu tất cả thông báo là đã đọc', 'success');
            // Update notification count in navbar
            $('#notification-count').hide();
        } else {
            showNotification('Không thể cập nhật thông báo', 'error');
        }
    }).fail(function() {
        showNotification('Không thể cập nhật thông báo', 'error');
    });
}

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

function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
}
