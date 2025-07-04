// Common utility functions for service management

/**
 * Shows a success message using SweetAlert2
 * @param {string} message - The success message to display
 */
function showSuccessMessage(message) {
    Swal.fire({
        icon: 'success',
        title: 'Thành công',
        text: message,
        timer: 3000,
        timerProgressBar: true
    });
}

/**
 * Shows an error message using SweetAlert2
 * @param {string} message - The error message to display
 */
function showErrorMessage(message) {
    Swal.fire({
        icon: 'error',
        title: 'Lỗi',
        text: message
    });
}

/**
 * Formats a number as Vietnamese currency
 * @param {number} amount - The amount to format
 * @returns {string} - Formatted currency string
 */
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN').format(amount) + ' đ';
}

/**
 * Initializes common components (tooltips, etc)
 */
function initializeComponents() {
    // Initialize tooltips if Bootstrap is available
    if (typeof $().tooltip === 'function') {
        $('[data-toggle="tooltip"]').tooltip();
    }
}

// Initialize components when the document is ready
$(document).ready(function() {
    initializeComponents();
});
