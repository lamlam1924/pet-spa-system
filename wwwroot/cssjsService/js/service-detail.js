/* filepath: d:\Code\VSCode\C#\pet-spa-system\wwwroot\cssjsService\js\service-detail.js */

/* ===== SERVICE DETAIL JAVASCRIPT ===== */

$(document).ready(function() {
    console.log('Service Detail page initialized');
    
    // Initialize components
    initializeComponents();
    
    // Show messages
    showMessages();
    
    // Add animations
    addAnimations();
});

/* ===== INITIALIZATION ===== */
function initializeComponents() {
    // Enhanced tab switching
    setupEnhancedTabs();
    
    // Counter animations
    animateCounters();
    
    // Scroll effects
    setupScrollEffects();
}

/* ===== ENHANCED TABS ===== */
function setupEnhancedTabs() {
    $('.nav-enhanced .nav-link').on('click', function(e) {
        e.preventDefault();
        
        const target = $(this).attr('href');
        const $targetPane = $(target);
        
        // Add loading state
        $targetPane.addClass('loading-skeleton');
        
        // Simulate content loading
        setTimeout(() => {
            $(this).tab('show');
            $targetPane.removeClass('loading-skeleton');
        }, 300);
    });
}

/* ===== ANIMATIONS ===== */
function animateCounters() {
    $('.stat-number').each(function() {
        const $this = $(this);
        const finalValue = parseInt($this.text().replace(/,/g, ''));
        
        if (!isNaN(finalValue)) {
            $this.text('0');
            
            $({ value: 0 }).animate({ value: finalValue }, {
                duration: 1500,
                easing: 'swing',
                step: function() {
                    $this.text(Math.floor(this.value).toLocaleString());
                },
                complete: function() {
                    $this.text(finalValue.toLocaleString());
                }
            });
        }
    });
}

function addAnimations() {
    // Stat cards hover effects
    $('.stat-card').on('mouseenter', function() {
        $(this).find('.stat-card-icon').addClass('animate__animated animate__pulse');
    }).on('mouseleave', function() {
        $(this).find('.stat-card-icon').removeClass('animate__animated animate__pulse');
    });
}

/* ===== SCROLL EFFECTS ===== */
function setupScrollEffects() {
    $(window).on('scroll', function() {
        const scrolled = $(window).scrollTop();
        const heroHeight = $('.service-hero-section').outerHeight();
        
        // Parallax effect for hero
        $('.service-hero-section').css('transform', `translateY(${scrolled * 0.3}px)`);
        
        // Sticky actions behavior
        if (scrolled > heroHeight) {
            $('.service-actions-sticky').addClass('floating');
        } else {
            $('.service-actions-sticky').removeClass('floating');
        }
    });
}


// ===== SWEETALERT2 MESSAGE HANDLING (ĐỒNG BỘ VỚI SERVICE LIST) =====
function showMessages() {
    if (typeof successMessage !== 'undefined' && successMessage && successMessage.trim() !== '') {
        showSuccessConfirmation(decodeHtmlEntities(successMessage));
    }
    if (typeof errorMessage !== 'undefined' && errorMessage && errorMessage.trim() !== '') {
        showErrorDialog(decodeHtmlEntities(errorMessage));
    }
}

function showSuccessConfirmation(message) {
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

// ===== STATUS CHANGE CONFIRMATION (ĐỒNG BỘ VỚI SERVICE LIST) =====
function confirmStatusChange(serviceId, activateService) {
    const isRestore = !!activateService;
    const title = isRestore ? 'Kích hoạt dịch vụ này?' : 'Tạm ngưng dịch vụ này?';
    const html = isRestore
        ? `<div class="swal2-confirm-text"> Dịch vụ sẽ được <b>kích hoạt trở lại</b> và hiển thị cho khách hàng.</div>`
        : `<div class="swal2-confirm-text"> Dịch vụ sẽ <b>tạm ngưng</b> và không hiển thị với khách hàng.</div>`;
    const confirmButtonText = isRestore ? 'Kích hoạt' : 'Tạm ngưng';
    const confirmBtnClass = isRestore ? 'btn-outline-success-custom' : 'btn-outline-warning-custom';
    Swal.fire({
        title: title,
        html: html,
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: confirmButtonText,
        cancelButtonText: 'Hủy',
        reverseButtons: true,
        customClass: {
            popup: 'simple-confirmation-popup',
            confirmButton: confirmBtnClass,
            cancelButton: 'btn-outline-gray-custom'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            submitStatusChange(serviceId, activateService);
        }
    });
}

function submitStatusChange(serviceId, activateService) {
    const form = document.getElementById('serviceActionForm');
    form.action = activateService ? serviceRestoreUrl : serviceSoftDeleteUrl;
    document.getElementById('serviceIdInput').value = serviceId;
    form.submit();
}

// ===== UTILITY =====
function decodeHtmlEntities(text) {
    const textArea = document.createElement('textarea');
    textArea.innerHTML = text;
    return textArea.value;
}
