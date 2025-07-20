/**
 * appointment-unified.js - JavaScript thống nhất cho trang đặt lịch
 * Kết hợp từ appointment.js và appointment-new.js
 */

// ===================== JS Đặt lịch - Trang đặt lịch thống nhất (Appointment Common) =====================

// Thêm CSS toast hiện đại cho trang đặt lịch
if (document.querySelector('link[href*="toast-custom.css"]') === null) {
    const link = document.createElement('link');
    link.rel = 'stylesheet';
    link.href = '/cssjsAppointment/css/toast-custom.css';
    document.head.appendChild(link);
}

$(document).ready(function() {
    // ===== Quản lý trạng thái và bước =====
    let currentStep = 1;
    const totalSteps = 3;
    let selectedPets = new Map();
    let selectedServices = new Map();
    
    // ===== Hiển thị thông báo thành công nếu có =====
    if ($('#successMessage').length > 0) {
        showToast($('#successMessage').val(), 'success');
    }

    // ===== Xử lý chuyển bước đặt lịch =====
    function showStep(stepNumber) {
        if (stepNumber < 1 || stepNumber > totalSteps) return;
        
        // Nếu đang chuyển từ bước 1 sang bước 2, kiểm tra xem đã chọn thú cưng và dịch vụ chưa
        if (currentStep === 1 && stepNumber === 2) {
            if (selectedPets.size === 0) {
                showToast('Vui lòng chọn ít nhất một thú cưng', 'warning');
                return;
            }
            
            if (selectedServices.size === 0) {
                showToast('Vui lòng chọn ít nhất một dịch vụ', 'warning');
                return;
            }
        }
        
        // Nếu đang chuyển từ bước 2 sang bước 3, kiểm tra thông tin cá nhân (validate step 2)
        if (currentStep === 2 && stepNumber === 3) {
            if (!validateStep2()) {
                return;
            }
            // Cập nhật thông tin xác nhận ở bước 3
            updateConfirmationInfo();
        }
    // ===== Validate các trường ở bước 2 =====
    function validateStep2() {
        // Kiểm tra họ tên
        if (!$('#CustomerName').val().trim()) {
            showToast('Vui lòng nhập họ tên', 'warning');
            $('#CustomerName').focus();
            return false;
        }
        // Kiểm tra số điện thoại
        if (!$('#Phone').val().trim()) {
            showToast('Vui lòng nhập số điện thoại', 'warning');
            $('#Phone').focus();
            return false;
        }
        // Kiểm tra email
        const email = $('#Email').val().trim();
        if (!email) {
            showToast('Vui lòng nhập địa chỉ email', 'warning');
            $('#Email').focus();
            return false;
        }
        const emailRegex = /^[\w-]+@([\w-]+\.)+[\w-]{2,4}$/;
        if (!emailRegex.test(email)) {
            showToast('Địa chỉ email không hợp lệ', 'warning');
            $('#Email').focus();
            return false;
        }
        // Kiểm tra ngày hẹn
        const dateVal = $('#AppointmentDate').val();
        if (!dateVal) {
            showToast('Vui lòng chọn ngày hẹn', 'warning');
            $('#AppointmentDate').focus();
            return false;
        }
        // Không cho phép chọn ngày trong quá khứ
        const today = new Date();
        today.setHours(0,0,0,0);
        const selectedDate = new Date(dateVal);
        if (selectedDate < today) {
            showToast('Ngày hẹn không được là ngày trong quá khứ', 'warning');
            $('#AppointmentDate').focus();
            return false;
        }
        // Kiểm tra giờ hẹn
        const timeVal = $('#AppointmentTime').val();
        if (!timeVal) {
            showToast('Vui lòng chọn giờ hẹn', 'warning');
            $('#AppointmentTime').focus();
            return false;
        }
        // Giờ phải từ 08:00 đến 20:00
        const [hour, minute] = timeVal.split(":").map(Number);
        if (hour < 8 || hour > 20 || (hour === 20 && minute > 0)) {
            showToast('Giờ hẹn chỉ được từ 8h sáng đến 8h tối (08:00 - 20:00)', 'warning');
            $('#AppointmentTime').focus();
            return false;
        }
        return true;
    }
        
        // Cập nhật bước hiện tại
        currentStep = stepNumber;
        
        // Ẩn tất cả các bước
        $('.step-content').hide();
        
        // Hiển thị bước hiện tại
        $(`#step-${stepNumber}`).fadeIn(300);
        
        // Cập nhật trạng thái các step
        $('.step').removeClass('active completed');
        
        // Đánh dấu các bước đã hoàn thành và bước hiện tại
        for (let i = 1; i <= totalSteps; i++) {
            if (i < stepNumber) {
                $(`.step[data-step="${i}"]`).addClass('completed');
            } else if (i === stepNumber) {
                $(`.step[data-step="${i}"]`).addClass('active');
            }
        }
        
        // Cập nhật nút Next/Previous
        updateNavigationButtons();
        
        // Cuộn lên đầu trang
        $('html, body').animate({ scrollTop: $('.booking-container').offset().top - 20 }, 300);
    }

    // ===== Cập nhật trạng thái các nút điều hướng =====
    function updateNavigationButtons() {
        // Ẩn/hiện nút Previous
        if (currentStep === 1) {
            $('#prevBtn').hide();
        } else {
            $('#prevBtn').show();
        }
        
        // Cập nhật nút Next/Submit
        if (currentStep === totalSteps) {
            $('#nextBtn').hide();
            $('#submitBtn').show();
        } else {
            $('#nextBtn').show();
            $('#submitBtn').hide();
        }
    }
    
    // ===== Xác thực thông tin cá nhân =====
    function validatePersonalInfo() {
        // Kiểm tra họ tên
        if (!$('#CustomerName').val().trim()) {
            showToast('Vui lòng nhập họ tên', 'warning');
            $('#CustomerName').focus();
            return false;
        }
        
        // Kiểm tra số điện thoại
        if (!$('#Phone').val().trim()) {
            showToast('Vui lòng nhập số điện thoại', 'warning');
            $('#Phone').focus();
            return false;
        }
        
        // Kiểm tra email
        const email = $('#Email').val().trim();
        if (!email) {
            showToast('Vui lòng nhập địa chỉ email', 'warning');
            $('#Email').focus();
            return false;
        }
        
        const emailRegex = /^[\w-]+@([\w-]+\.)+[\w-]{2,4}$/;
        if (!emailRegex.test(email)) {
            showToast('Địa chỉ email không hợp lệ', 'warning');
            $('#Email').focus();
            return false;
        }
        
        // Kiểm tra ngày hẹn
        if (!$('#AppointmentDate').val()) {
            showToast('Vui lòng chọn ngày hẹn', 'warning');
            $('#AppointmentDate').focus();
            return false;
        }
        
        // Kiểm tra giờ hẹn
        if (!$('#AppointmentTime').val()) {
            showToast('Vui lòng chọn giờ hẹn', 'warning');
            $('#AppointmentTime').focus();
            return false;
        }
        
        return true;
    }
    
    // ===== Lấy tiêu đề cho toast =====
    function getToastTitle(type) {
        switch(type) {
            case 'error': return 'Lỗi';
            case 'warning': return 'Cảnh báo';
            case 'success': return 'Thành công';
            default: return 'Thông báo';
        }
    }
    
    // ===== Hiển thị thông báo toast nâng cao =====
    function showToast(message, type = 'info') {
        // Xử lý các loại toast
        let bgColor, iconClass, borderColor;
        
        switch(type) {
            case 'success':
                bgColor = 'bg-success bg-opacity-75';
                iconClass = 'fas fa-check-circle';
                borderColor = 'border-success';
                break;
            case 'warning':
                bgColor = 'bg-warning bg-opacity-75';
                iconClass = 'fas fa-exclamation-triangle';
                borderColor = 'border-warning';
                break;
            case 'error':
                bgColor = 'bg-danger bg-opacity-75';
                iconClass = 'fas fa-times-circle';
                borderColor = 'border-danger';
                break;
            default: // info
                bgColor = 'bg-info bg-opacity-75';
                iconClass = 'fas fa-info-circle';
                borderColor = 'border-info';
        }
        
        // Tạo container toast nếu chưa tồn tại
        if ($('#toastContainer').length === 0) {
            $('body').append('<div id="toastContainer" style="position: fixed; top: 20px; right: 20px; z-index: 9999; min-width: 300px; max-width: 350px;"></div>');
        }
        
        // Tạo toast theo Bootstrap 5
        const toastId = 'toast-' + new Date().getTime();
        const toast = $(`
            <div id="${toastId}" class="toast ${borderColor} border-2" role="alert" aria-live="assertive" aria-atomic="true" data-bs-autohide="true" data-bs-delay="5000">
                <div class="toast-header ${bgColor} text-white">
                    <i class="${iconClass} me-2"></i>
                    <strong class="me-auto">${getToastTitle(type)}</strong>
                    <small>${new Date().toLocaleTimeString()}</small>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
                <div class="toast-body">
                    ${message}
                </div>
            </div>
        `);
        
        // Thêm toast vào container
        $('#toastContainer').append(toast);
        
        // Khởi tạo và hiển thị toast
        const toastElement = new bootstrap.Toast(document.getElementById(toastId), {
            delay: 5000
        });
        toastElement.show();
        
        // Xóa toast sau khi ẩn
        $(`#${toastId}`).on('hidden.bs.toast', function() {
            $(this).remove();
        });
    }
    
    // ===== Cập nhật thông tin xác nhận ở bước 3 =====
    function updateConfirmationInfo() {
        // Thông tin khách hàng
        $('#conf-name').text($('#CustomerName').val());
        $('#conf-phone').text($('#Phone').val());
        $('#conf-email').text($('#Email').val());
        
        // Thông tin lịch hẹn
        $('#conf-date').text(formatDate($('#AppointmentDate').val()));
        $('#conf-time').text($('#AppointmentTime').val());
        $('#conf-notes').text($('#Notes').val() || '(Không có ghi chú)');
        
        // Hiển thị thú cưng được chọn
        $('#conf-pets').empty();
        selectedPets.forEach(function(pet) {
            $('#conf-pets').append(`
                <div class="conf-pet-item">
                    <div>
                        <i class="fa fa-paw"></i>
                        <strong>${pet.name}</strong>
                        ${pet.breed ? `<span> - ${pet.breed}</span>` : ''}
                    </div>
                </div>
            `);
        });
        
        // Hiển thị dịch vụ được chọn
        let totalPrice = 0;
        $('#conf-services').empty();
        
        selectedServices.forEach(function(service) {
            totalPrice += service.price;
            $('#conf-services').append(`
                <div class="conf-service-item">
                    <div>${service.name}</div>
                    <div class="service-price">${formatCurrency(service.price)}</div>
                </div>
            `);
        });
        
        // Hiển thị tổng tiền
        $('#conf-services').append(`
            <div class="total-price mt-3">
                Tổng cộng: <strong>${formatCurrency(totalPrice)}</strong>
            </div>
        `);
    }
    
    // ===== Format ngày tháng và tiền tệ =====
    function formatDate(dateString) {
        const date = new Date(dateString);
        const options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
        return date.toLocaleDateString('vi-VN', options);
    }
    
    function formatCurrency(amount) {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
    }
    
    // ===== Xử lý chọn thú cưng =====
    $('.pet-card').click(function() {
        const checkbox = $(this).find('.pet-checkbox');
        checkbox.prop('checked', !checkbox.prop('checked')).change();
    });
    
    // Ngăn sự kiện click lan ra ngoài khi click vào checkbox
    $('.pet-checkbox').click(function(e) {
        e.stopPropagation();
    });
    
    // Xử lý sự kiện khi thay đổi trạng thái checkbox thú cưng
    $('.pet-checkbox').change(function() {
        const petId = $(this).val();
        const petCard = $(this).closest('.pet-card');
        const petName = petCard.find('.pet-info h5').text();
        const petBreed = petCard.find('.pet-info small').text();
        
        if (this.checked) {
            petCard.addClass('selected');
            selectedPets.set(petId, { name: petName, breed: petBreed });
        } else {
            petCard.removeClass('selected');
            selectedPets.delete(petId);
        }
        
        updatePetsSummary();
    });
    
    // ===== Xử lý chọn dịch vụ =====
    $('.service-card').click(function() {
        const checkbox = $(this).find('.service-checkbox');
        checkbox.prop('checked', !checkbox.prop('checked')).change();
    });
    
    // Ngăn sự kiện click lan ra ngoài khi click vào checkbox
    $('.service-checkbox').click(function(e) {
        e.stopPropagation();
    });
    
    // Xử lý sự kiện khi thay đổi trạng thái checkbox dịch vụ
    $('.service-checkbox').change(function() {
        const serviceId = $(this).val();
        const serviceCard = $(this).closest('.service-card');
        const serviceName = serviceCard.find('.service-info h5').text();
        const servicePrice = parseFloat($(this).data('price'));
        
        if (this.checked) {
            serviceCard.addClass('selected');
            selectedServices.set(serviceId, { name: serviceName, price: servicePrice });
        } else {
            serviceCard.removeClass('selected');
            selectedServices.delete(serviceId);
        }
        
        updateServicesSummary();
        updateTotalPrice();
    });
    
    // ===== Cập nhật summary và tổng tiền =====
    function updatePetsSummary() {
        $('#selectedPetsList').empty();
        
        if (selectedPets.size === 0) {
            $('#selectedPetsList').html('<div class="text-muted">Chưa chọn thú cưng nào</div>');
            return;
        }
        
        selectedPets.forEach(function(pet, petId) {
            $('#selectedPetsList').append(`
                <div class="summary-item">
                    <div>
                        <i class="fa fa-paw"></i>
                        <strong>${pet.name}</strong>
                        ${pet.breed ? `<span class="text-muted"> - ${pet.breed}</span>` : ''}
                    </div>
                </div>
            `);
        });
    }
    
    function updateServicesSummary() {
        $('#selectedServicesList').empty();
        
        if (selectedServices.size === 0) {
            $('#selectedServicesList').html('<div class="text-muted">Chưa chọn dịch vụ nào</div>');
            return;
        }
        
        selectedServices.forEach(function(service, serviceId) {
            $('#selectedServicesList').append(`
                <div class="summary-item">
                    <div>${service.name}</div>
                    <div>${formatCurrency(service.price)}</div>
                </div>
            `);
        });
    }
    
    function updateTotalPrice() {
        let total = 0;
        selectedServices.forEach(function(service) {
            total += service.price;
        });
        
        $('#totalPrice').text(formatCurrency(total).replace('₫', ''));
    }
    
    // ===== Sự kiện cho nút chuyển bước =====
    $('#prevBtn').click(function() {
        showStep(currentStep - 1);
    });
    
    $('#nextBtn').click(function() {
        showStep(currentStep + 1);
    });
    
    // ===== Xử lý submit form =====
    $('#appointmentForm').submit(function(e) {
        e.preventDefault();
        
        // Chỉ xử lý submit khi ở bước cuối cùng
        if (currentStep !== totalSteps) {
            showStep(currentStep + 1);
            return;
        }
        
        // Tạo mảng các ID thú cưng và dịch vụ đã chọn
        const petIds = Array.from(selectedPets.keys());
        const serviceIds = Array.from(selectedServices.keys());
        
        // Tạo hidden inputs để gửi dữ liệu
        $('#appointmentForm').find('input[name="SelectedPetIds"]').remove();
        $('#appointmentForm').find('input[name="SelectedServiceIds"]').remove();
        
        petIds.forEach(function(id, index) {
            $('#appointmentForm').append(`<input type="hidden" name="SelectedPetIds[${index}]" value="${id}" />`);
        });
        
        serviceIds.forEach(function(id, index) {
            $('#appointmentForm').append(`<input type="hidden" name="SelectedServiceIds[${index}]" value="${id}" />`);
        });
        
        // Hiển thị thông báo đang xử lý
        showToast('Đang xử lý đặt lịch...', 'info');
        $('#submitBtn').prop('disabled', true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Đang xử lý...');
        
        // Gửi form bằng AJAX
        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            success: function(response) {
                if (response.success) {
                    showToast('Đặt lịch thành công!', 'success');
                    setTimeout(function() {
                        window.location.href = response.redirectUrl;
                    }, 1500);
                } else {
                    showToast(response.message || 'Có lỗi xảy ra khi đặt lịch', 'error');
                    $('#submitBtn').prop('disabled', false).html('<i class="fa fa-calendar-check"></i> Xác nhận đặt lịch');
                }
            },
            error: function() {
                showToast('Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại sau.', 'error');
                $('#submitBtn').prop('disabled', false).html('<i class="fa fa-calendar-check"></i> Xác nhận đặt lịch');
            }
        });
    });
    
    // ===== Khởi tạo giao diện và kiểm tra trạng thái ban đầu =====
    showStep(1);
    updatePetsSummary();
    updateServicesSummary();
    updateTotalPrice();
    
    // Kiểm tra xem các checkbox đã được chọn từ trước không (refresh page hoặc back browser)
    $('.pet-checkbox:checked').each(function() {
        const petId = $(this).val();
        const petCard = $(this).closest('.pet-card');
        const petName = petCard.find('.pet-info h5').text();
        const petBreed = petCard.find('.pet-info small').text();
        
        petCard.addClass('selected');
        selectedPets.set(petId, { name: petName, breed: petBreed });
    });
    
    $('.service-checkbox:checked').each(function() {
        const serviceId = $(this).val();
        const serviceCard = $(this).closest('.service-card');
        const serviceName = serviceCard.find('.service-info h5').text();
        const servicePrice = parseFloat($(this).data('price'));
        
        serviceCard.addClass('selected');
        selectedServices.set(serviceId, { name: serviceName, price: servicePrice });
    });
    
    // Cập nhật lại summary sau khi khởi tạo
    updatePetsSummary();
    updateServicesSummary();
    updateTotalPrice();
});
