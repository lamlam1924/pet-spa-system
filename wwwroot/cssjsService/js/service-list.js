/* ===== SERVICE LIST JAVASCRIPT - CLEAN VERSION ===== */

$(document).ready(function() {
    console.log('Document ready - starting initialization');
    
    // Initialize tooltips - SAFE
    try {
        if (typeof $.fn.tooltip === 'function') {
            $('[data-toggle="tooltip"]').tooltip();
            console.log('Tooltips initialized');
        }
    } catch (e) {
        console.warn('Tooltip error:', e);
    }
    
    // Setup all functionality
    setupSearch();
    setupFilters();
    showMessages();
    setupPriceInputFormat();
    
    console.log('All functions initialized');
});

/* ===== SEARCH FUNCTIONALITY ===== */
function setupSearch() {
    const searchInput = $('#searchInput');
    const clearBtn = $('#clearSearch');
    
    if (searchInput.length) {
        let searchTimeout;
        
        // Search with debounce
        searchInput.on('input', function() {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                if (this.value.length >= 3 || this.value.length === 0) {
                    $('#filterForm').submit();
                }
            }, 500);
        });
        
        // Enter key submit
        searchInput.on('keypress', function(e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                $('#filterForm').submit();
            }
        });
    }
    
    // Clear search
    if (clearBtn.length) {
        clearBtn.on('click', function() {
            searchInput.val('');
            $('#filterForm').submit();
        });
    }
}

/* ===== FILTER FUNCTIONALITY ===== */
function setupFilters() {
    $('#categoryFilter, #statusFilter, #sortOrder').on('change', function() {
        $('#filterForm').submit();
    });
}

/* ===== STATUS TOGGLE - SIMPLE BUT BEAUTIFUL ===== */
$(document).on('click', '.btn-status-toggle', function(e) {
    e.preventDefault();
    e.stopPropagation();
    
    const $this = $(this);
    const serviceId = $this.data('service-id');
    const currentStatus = $this.data('current-status');
    const isActive = currentStatus === 'true' || currentStatus === true;
    
    // Validation
    if (!serviceId) {
        showToast('error', 'Lỗi', 'Không tìm thấy ID dịch vụ');
        return;
    }
    
    // Disable button
    $this.prop('disabled', true);
    
    // Simple but beautiful confirmation
    Swal.fire({
        title: isActive ? 'Tạm ngưng dịch vụ?' : 'Kích hoạt dịch vụ?',
        text: isActive ? 'Dịch vụ sẽ không hiển thị với khách hàng' : 'Dịch vụ sẽ hoạt động trở lại',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: 'transparent',
        cancelButtonColor: 'transparent',
        confirmButtonText: isActive ? 'Tạm ngưng' : 'Kích hoạt',
        cancelButtonText: 'Hủy',
        reverseButtons: true,
        customClass: {
            popup: 'simple-confirmation-popup',
            confirmButton: isActive ? 'btn-outline-warning-custom' : 'btn-outline-success-custom',
            cancelButton: 'btn-outline-gray-custom'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            submitStatusChange(serviceId, isActive);
        } else {
            $this.prop('disabled', false);
        }
    }).catch(() => {
        $this.prop('disabled', false);
    });
});

/* ===== FORM SUBMISSION ===== */
function submitStatusChange(serviceId, isActive) {
    const form = $('#statusToggleForm');
    const serviceIdInput = $('#statusServiceId');
    
    // Set form values
    serviceIdInput.val(serviceId);
    form.attr('action', isActive ? deactivateUrl : activateUrl);
    
    // Submit form
    form.submit();
}

/* ===== MESSAGE HANDLING ===== */
function showMessages() {
    console.log('showMessages called');
    console.log('successMessage:', typeof successMessage, successMessage);
    console.log('errorMessage:', typeof errorMessage, errorMessage);
    
    if (typeof successMessage !== 'undefined' && successMessage && successMessage.trim() !== '') {
        console.log('Showing success confirmation');
        showSuccessConfirmation(decodeHtmlEntities(successMessage));
    }
    
    if (typeof errorMessage !== 'undefined' && errorMessage && errorMessage.trim() !== '') {
        console.log('Showing error dialog');
        showErrorDialog(decodeHtmlEntities(errorMessage));
    }
}

function showSuccessConfirmation(message) {
    console.log('showSuccessConfirmation called with:', message);
    
    Swal.fire({
        title: 'Thành công!',
        text: message,
        icon: 'success',
        confirmButtonColor: '#10b981',
        confirmButtonText: 'Tuyệt vời!',
        timer: 4000,
        timerProgressBar: true,
        allowOutsideClick: true,
        allowEscapeKey: true,
        customClass: {
            popup: 'success-center-popup',
            confirmButton: 'btn-success-styled'
        }
    }).then((result) => {
        console.log('Success dialog closed:', result);
    });
}

function showErrorDialog(message) {
    Swal.fire({
        title: 'Lỗi!',
        text: message,
        icon: 'error',
        confirmButtonText: 'OK',
        confirmButtonColor: '#ef4444'
    });
}

function showToast(type, title, message) {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 2000,
        timerProgressBar: true
    });
    
    Toast.fire({
        icon: type,
        title: title,
        text: message
    });
}

/* ===== PRICE INPUT FORMAT FUNCTIONALITY ===== */
function setupPriceInputFormat() {
    const priceInputs = $('input.price-input');
    priceInputs.on('input', function () {
        // Lưu vị trí con trỏ cũ
        const oldValue = this.value;
        const oldCursor = this.selectionStart;

        // Lấy số thuần
        let val = oldValue.replace(/\D/g, '');
        if (val) {
            // Format lại với dấu chấm
            let formatted = val.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
            this.value = formatted;

            // Tính lại vị trí con trỏ
            let diff = formatted.length - oldValue.length;
            let newCursor = oldCursor + diff;
            // Nếu xóa hết thì về đầu
            if (formatted === '') newCursor = 0;
            // Đặt lại vị trí con trỏ
            this.setSelectionRange(newCursor, newCursor);
        } else {
            this.value = '';
        }
    });

    // Khi submit form, bỏ dấu chấm để gửi số thuần
    $('#filterForm').on('submit', function () {
        priceInputs.each(function () {
            this.value = this.value.replace(/\./g, '');
        });
    });
}

/* ===== UTILITY FUNCTIONS ===== */
function decodeHtmlEntities(text) {
    const textArea = document.createElement('textarea');
    textArea.innerHTML = text;
    return textArea.value;
}

function formatCurrency(amount) {
    if (amount >= 1000000) {
        return (amount / 1000000).toFixed(1) + 'M đ';
    }
    return new Intl.NumberFormat('vi-VN').format(amount) + ' đ';
}
