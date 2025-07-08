/* Common JavaScript functions for all service management pages */

// Function to show alerts using SweetAlert2
function showAlert(title, message, type = 'success') {
    Swal.fire({
        icon: type,
        title: title,
        text: message,
        timer: type === 'success' ? 2000 : undefined,
        timerProgressBar: type === 'success',
    });
}

// Function to handle API errors
function handleApiError(error) {
    console.error('API Error:', error);
    showAlert('Lỗi', 'Có lỗi xảy ra trong quá trình xử lý. Vui lòng thử lại sau.', 'error');
}

// Function to format currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
}

// Function to format date
function formatDate(dateString) {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('vi-VN', { 
        day: '2-digit', 
        month: '2-digit', 
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    }).format(date);
}

// Function to initialize common components
function initCommonComponents() {
    // Initialize tooltips
    $('[data-toggle="tooltip"]').tooltip();
    
    // Initialize popovers
    $('[data-toggle="popover"]').popover();
    
    // Initialize select2 if available
    if($.fn.select2) {
        $('.select2').select2({
            theme: 'bootstrap4',
        });
    }
    
    // Initialize datepicker if available
    if($.fn.datepicker) {
        $('.datepicker').datepicker({
            format: 'dd/mm/yyyy',
            language: 'vi',
            autoclose: true
        });
    }
}

// Initialize common components when DOM is ready
$(document).ready(function() {
    initCommonComponents();
    
    // Handle any success/error messages from TempData
    if (typeof successMessage !== 'undefined' && successMessage) {
        showAlert('Thành công', successMessage);
    }
    
    if (typeof errorMessage !== 'undefined' && errorMessage) {
        showAlert('Lỗi', errorMessage, 'error');
    }
    
    // Navigation slider animation for active tab
    const activeNavItem = $('.nav-tabs-service .nav-link.active');
    if (activeNavItem.length) {
        const navSlider = $('#navSlider');
        const activeItemOffset = activeNavItem.position().left;
        const activeItemWidth = activeNavItem.outerWidth();
        
        navSlider.css({
            'left': activeItemOffset + 'px',
            'width': activeItemWidth + 'px'
        });
    }
});
