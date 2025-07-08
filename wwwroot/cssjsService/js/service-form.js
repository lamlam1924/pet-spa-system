/* Enhanced Service Form JavaScript with Better UX */

let hasUnsavedChanges = false;
let autoSaveTimer = null;
let originalFormData = {};
let isSubmitting = false;

$(document).ready(function() {
    initializeForm();
    setupFormWatchers();
    setupAutoSave();
    setupKeyboardShortcuts();
    setupFormSubmission();
    setupDeleteFunction();
});

// ===== FORM DATA MANAGEMENT =====
function getFormData() {
    let description = $('#serviceDescription').val() || '';
    
    // Get content from CKEditor if available
    if (window.descriptionEditor) {
        description = window.descriptionEditor.getData();
    }
    
    return {
        serviceId: $('input[name="Service.ServiceId"]').val(),
        name: $('#serviceName').val().trim(),
        categoryId: $('#serviceCategory').val(),
        price: parseFloat($('#servicePrice').val()) || 0,
        duration: parseInt($('#serviceDuration').val()) || 30,
        description: description,
        isActive: $('#serviceStatus').is(':checked')
    };
}

function getCompleteFormData() {
    return getFormData(); // Alias for consistency
}

function setFormData(data) {
    $('#serviceName').val(data.name || '');
    $('#serviceCategory').val(data.categoryId || '').trigger('change');
    $('#servicePrice').val(data.price || '');
    $('#serviceDuration').val(data.duration || '');
    $('#serviceDescription').val(data.description || '');
    $('#serviceStatus').prop('checked', data.isActive === true);
    
    // Update CKEditor if available
    if (window.descriptionEditor && data.description) {
        window.descriptionEditor.setData(data.description);
    }
    
    // Update hidden field
    $('input[name="Service.IsActive"]').val(data.isActive === true);
}

// ===== INITIALIZATION =====
function initializeForm() {
    // Store original form data - Use serviceData from script section if available
    if (typeof serviceData !== 'undefined') {
        originalFormData = {
            name: serviceData.name,
            categoryId: serviceData.categoryId,
            price: parseFloat(serviceData.price) || 0,
            duration: parseInt(serviceData.duration) || 30,
            description: serviceData.description,
            isActive: serviceData.isActive === 'true'
        };
    } else {
        originalFormData = getFormData();
    }
    
    // Check for auto-saved draft
    checkForDraft();
    
    // Initialize rich editor
    initializeEditor();
    
    // Enhanced select2
    if($.fn.select2) {
        $('#serviceCategory').select2({
            theme: 'bootstrap4',
            placeholder: 'Chọn danh mục dịch vụ...',
            allowClear: false,
            minimumResultsForSearch: 5
        });
    }
    
    // Initialize preview
    updatePreviewInstantly();
    
    // Show success/error messages
    showMessages();
}

function initializeEditor() {
    // Initialize CKEditor if available
    if(typeof ClassicEditor !== 'undefined') {
        ClassicEditor
            .create(document.querySelector('#serviceDescription'), {
                toolbar: ['heading', '|', 'bold', 'italic', 'link', 'bulletedList', 'numberedList', '|', 'undo', 'redo'],
                placeholder: 'Mô tả chi tiết về dịch vụ...'
            })
            .then(editor => {
                window.descriptionEditor = editor;
                // Watch for changes in editor
                editor.model.document.on('change:data', () => {
                    if (!isSubmitting) {
                        markAsChanged();
                        updatePreviewRealtime();
                    }
                });
            })
            .catch(error => console.error('CKEditor initialization error:', error));
    }
}

// ===== FORM WATCHERS =====
function setupFormWatchers() {
    // Watch all form inputs for changes
    $('#editServiceForm input, #editServiceForm select, #editServiceForm textarea').on('input change', function() {
        if (!isSubmitting) {
            markAsChanged();
            updatePreviewRealtime();
        }
    });
    
    // Special handling for checkbox
    $('#serviceStatus').on('change', function() {
        if (!isSubmitting) {
            markAsChanged();
            updateStatusPreview();
            // Update hidden field immediately
            $('input[name="Service.IsActive"]').val($(this).is(':checked'));
        }
    });
}

function markAsChanged() {
    if (!hasUnsavedChanges) {
        hasUnsavedChanges = true;
        updateSaveButtonState();
        
        // Add visual indicator
        if (!$('.unsaved-indicator').length) {
            $('.card-header h6').append(' <span class="unsaved-indicator text-warning"><i class="fas fa-circle"></i> Chưa lưu</span>');
        }
        
        // Add asterisk to page title
        if (document.title.indexOf('*') !== 0) {
            document.title = '*' + document.title;
        }
    }
}

// ===== PREVIEW FUNCTIONS =====
function updatePreviewRealtime() {
    // Debounced preview update for better performance
    clearTimeout(window.previewTimer);
    window.previewTimer = setTimeout(updatePreviewInstantly, 300);
}

function updatePreviewInstantly() {
    const formData = getFormData();
    const categoryText = $('#serviceCategory option:selected').text() || 'Chưa chọn danh mục';
    
    // Update with smooth animation
    $('#previewName').fadeOut(200, function() {
        $(this).text(formData.name || 'Tên dịch vụ').fadeIn(200);
    });
    
    $('#previewCategory').html(`<i class="fas fa-tag"></i> ${categoryText}`);
    $('#previewPrice').text((formData.price || 0).toLocaleString('vi-VN') + ' đ');
    $('#previewDuration').text((formData.duration || 30) + ' phút');
    $('#previewDescription').text(formData.description || 'Mô tả chi tiết về dịch vụ sẽ hiển thị tại đây...');
    
    updateStatusPreview();
}

function updateStatusPreview() {
    const isActive = $('#serviceStatus').is(':checked');
    const statusBadge = $('#previewStatus');
    
    statusBadge.removeClass('badge-success badge-secondary');
    
    if (isActive) {
        statusBadge.addClass('badge-success').text('Hoạt động');
    } else {
        statusBadge.addClass('badge-secondary').text('Tạm ngưng');
    }
}

// ===== FORM SUBMISSION =====
function setupFormSubmission() {
    // Handle form submission properly
    $('#editServiceForm').off('submit').on('submit', function(e) {
        e.preventDefault(); // Always prevent default first
        
        if (isSubmitting) {
            return false; // Prevent double submission
        }
        
        handleFormSubmit();
    });
    
    // Handle save button click
    $('.btn-save, .btn-primary').off('click').on('click', function(e) {
        if ($(this).attr('type') !== 'submit') {
            e.preventDefault();
            handleFormSubmit();
        }
    });
}

function handleFormSubmit() {
    if (isSubmitting) {
        return false;
    }
    
    const errors = validateForm();
    
    if (errors.length > 0) {
        showValidationErrors(errors);
        return false;
    }
    
    // Start submission process
    isSubmitting = true;
    showLoadingState();
    
    // Get form data including editor content
    const formData = getCompleteFormData();
    
    // Update form fields with latest data
    updateFormFields(formData);
    
    // Clear draft since we're saving to database
    clearDraft();
    
    // Submit form after brief delay (for UX)
    setTimeout(() => {
        const form = document.getElementById('editServiceForm');
        if (form) {
            // Create a new form element to avoid jQuery conflicts
            const nativeForm = form.cloneNode(true);
            form.parentNode.replaceChild(nativeForm, form);
            nativeForm.submit();
        }
    }, 500);
    
    return true;
}

function updateFormFields(formData) {
    // Ensure all form fields have the correct values
    $('input[name="Service.Name"]').val(formData.name);
    $('select[name="Service.CategoryId"]').val(formData.categoryId);
    $('input[name="Service.Price"]').val(formData.price);
    $('input[name="Service.DurationMinutes"]').val(formData.duration);
    $('textarea[name="Service.Description"]').val(formData.description);
    $('input[name="Service.IsActive"]').val(formData.isActive);
}

// ===== VALIDATION =====
function validateForm() {
    const errors = [];
    const formData = getCompleteFormData();
    
    // Remove previous validation classes
    $('#editServiceForm .is-invalid').removeClass('is-invalid');
    
    if (!formData.name) {
        errors.push('Tên dịch vụ không được để trống');
        $('#serviceName').addClass('is-invalid');
    } else if (formData.name.length > 100) {
        errors.push('Tên dịch vụ không được vượt quá 100 ký tự');
        $('#serviceName').addClass('is-invalid');
    }
    
    if (!formData.categoryId) {
        errors.push('Vui lòng chọn danh mục dịch vụ');
        $('#serviceCategory').addClass('is-invalid');
    }
    
    if (!formData.price || formData.price <= 0) {
        errors.push('Giá dịch vụ phải lớn hơn 0');
        $('#servicePrice').addClass('is-invalid');
    }
    
    if (!formData.duration || formData.duration < 5) {
        errors.push('Thời gian dịch vụ tối thiểu 5 phút');
        $('#serviceDuration').addClass('is-invalid');
    }
    
    return errors;
}

function showValidationErrors(errors) {
    Swal.fire({
        icon: 'error',
        title: 'Vui lòng kiểm tra lại',
        html: '<div style="text-align: left;"><ul>' + 
              errors.map(error => `<li>${error}</li>`).join('') + 
              '</ul></div>',
        confirmButtonText: 'Đã hiểu',
        confirmButtonColor: '#3085d6'
    });
}

// ===== UI STATE MANAGEMENT =====
function updateSaveButtonState() {
    const saveBtn = $('.btn-save, .btn-primary');
    
    if (isSubmitting) {
        return; // Don't update while submitting
    }
    
    if (hasUnsavedChanges) {
        saveBtn.removeClass('btn-outline-primary btn-success').addClass('btn-primary');
        saveBtn.html('<i class="fas fa-save"></i> Lưu thay đổi');
        saveBtn.prop('disabled', false);
        
        // Add pulsing effect for unsaved changes
        saveBtn.addClass('btn-pulse');
    } else {
        saveBtn.removeClass('btn-primary btn-pulse').addClass('btn-success');
        saveBtn.html('<i class="fas fa-check-circle"></i> Đã lưu');
        saveBtn.prop('disabled', false);
        
        // Remove unsaved indicator
        $('.unsaved-indicator').fadeOut(300, function() {
            $(this).remove();
        });
        
        // Remove asterisk from page title
        if (document.title.indexOf('*') === 0) {
            document.title = document.title.substring(1);
        }
    }
}

function showLoadingState() {
    const saveBtn = $('.btn-save, .btn-primary');
    saveBtn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Đang lưu...');
    
    // Also disable other form elements
    $('#editServiceForm input, #editServiceForm select, #editServiceForm textarea').prop('disabled', true);
}

function resetSubmissionState() {
    isSubmitting = false;
    const saveBtn = $('.btn-save, .btn-primary');
    saveBtn.prop('disabled', false);
    
    // Re-enable form elements
    $('#editServiceForm input, #editServiceForm select, #editServiceForm textarea').prop('disabled', false);
    
    updateSaveButtonState();
}

// ===== AUTO-SAVE & DRAFT =====
function setupAutoSave() {
    // Auto-save every 30 seconds if there are changes
    setInterval(function() {
        if (hasUnsavedChanges && !isSubmitting) {
            autoSaveForm();
        }
    }, 30000);
    
    // Auto-save when user idle (không typing) 5 giây
    let idleTimer;
    $('#editServiceForm input, #editServiceForm select, #editServiceForm textarea').on('input', function() {
        clearTimeout(idleTimer);
        idleTimer = setTimeout(function() {
            if (hasUnsavedChanges && !isSubmitting) {
                autoSaveForm();
            }
        }, 5000);
    });
}

function autoSaveForm() {
    const errors = validateForm();
    if (errors.length === 0) {
        const formData = getCompleteFormData();
        
        // Save to localStorage with timestamp
        localStorage.setItem('serviceFormBackup', JSON.stringify(formData));
        localStorage.setItem('serviceFormBackupTime', Date.now().toString());
        
        // Show subtle notification
        showToast('📝 Đã tự động lưu nháp', 'info');
    }
}

function checkForDraft() {
    const draftData = localStorage.getItem('serviceFormBackup');
    const draftTime = localStorage.getItem('serviceFormBackupTime');
    
    if (draftData && draftTime) {
        try {
            const draft = JSON.parse(draftData);
            const timeDiff = Date.now() - parseInt(draftTime);
            const minutesAgo = Math.floor(timeDiff / 60000);
            
            if (minutesAgo < 60) { // Show draft if less than 1 hour old
                Swal.fire({
                    title: 'Phát hiện bản nháp',
                    html: `Có bản nháp được lưu <strong>${minutesAgo} phút trước</strong>.<br>Bạn có muốn khôi phục không?`,
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonText: 'Khôi phục bản nháp',
                    cancelButtonText: 'Bỏ qua',
                    confirmButtonColor: '#3085d6'
                }).then((result) => {
                    if (result.isConfirmed) {
                        setFormData(draft);
                        markAsChanged();
                        updatePreviewInstantly();
                        showToast('Đã khôi phục bản nháp', 'success');
                    } else {
                        clearDraft();
                    }
                });
            } else {
                clearDraft(); // Clear old drafts
            }
        } catch (error) {
            console.error('Error parsing draft:', error);
            clearDraft();
        }
    }
}

function clearDraft() {
    localStorage.removeItem('serviceFormBackup');
    localStorage.removeItem('serviceFormBackupTime');
}

// ===== RESET FUNCTION =====
function resetForm() {
    if (!hasUnsavedChanges) {
        resetToOriginal();
        return;
    }
    
    Swal.fire({
        title: 'Khôi phục form?',
        text: 'Tất cả thay đổi chưa lưu sẽ bị mất. Bạn có chắc chắn?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Khôi phục',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            resetToOriginal();
        }
    });
}

function resetToOriginal() {
    // Reset to original data
    setFormData(originalFormData);
    
    hasUnsavedChanges = false;
    updateSaveButtonState();
    updatePreviewInstantly();
    
    // Remove unsaved indicator
    $('.unsaved-indicator').remove();
    
    showToast('Đã khôi phục về trạng thái ban đầu', 'info');
}

// ===== KEYBOARD SHORTCUTS =====
function setupKeyboardShortcuts() {
    $(document).on('keydown', function(e) {
        if (isSubmitting) {
            return; // Ignore shortcuts while submitting
        }
        
        // Ctrl+S to save
        if ((e.ctrlKey || e.metaKey) && e.which === 83) {
            e.preventDefault();
            handleFormSubmit();
        }
        
        // Ctrl+R to reset (with confirmation)
        if ((e.ctrlKey || e.metaKey) && e.which === 82) {
            e.preventDefault();
            resetForm();
        }
    });
}

// ===== DELETE FUNCTION =====
function setupDeleteFunction() {
    $('#btnDeleteService').on('click', function() {
        $('#deleteServiceModal').modal('show');
    });
    
    $('#deleteServiceForm').on('submit', function(e) {
        e.preventDefault();
        
        Swal.fire({
            title: 'Xác nhận xóa?',
            text: 'Hành động này không thể hoàn tác!',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Xóa',
            cancelButtonText: 'Hủy'
        }).then((result) => {
            if (result.isConfirmed) {
                this.submit();
            }
        });
    });
}

// ===== UTILITY FUNCTIONS =====
function showToast(message, type = 'info') {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer);
            toast.addEventListener('mouseleave', Swal.resumeTimer);
        }
    });
    
    Toast.fire({
        icon: type,
        title: message
    });
}

function showMessages() {
    if (typeof successMessage !== 'undefined' && successMessage) {
        Swal.fire({
            icon: 'success',
            title: 'Thành công!',
            text: successMessage,
            showCancelButton: true,
            confirmButtonText: '<i class="fas fa-list"></i> Về danh sách',
            cancelButtonText: '<i class="fas fa-edit"></i> Tiếp tục sửa',
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#28a745'
        }).then((result) => {
            if (result.isConfirmed) {
                window.location.href = '/Admin/ManageService/ServiceList';
            }
        });
    }
    
    if (typeof errorMessage !== 'undefined' && errorMessage) {
        Swal.fire({
            icon: 'error',
            title: 'Có lỗi xảy ra!',
            text: errorMessage
        }).then(() => {
            resetSubmissionState();
        });
    }
}

// ===== PREVENT ACCIDENTAL LEAVE =====
window.addEventListener('beforeunload', function(e) {
    if (hasUnsavedChanges && !isSubmitting) {
        e.preventDefault();
        e.returnValue = 'Bạn có thay đổi chưa được lưu. Bạn có chắc muốn rời khỏi trang?';
        return e.returnValue;
    }
});
