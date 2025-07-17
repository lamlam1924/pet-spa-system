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

/* ===== MESSAGE HANDLING ===== */
function showMessages() {
    // Success message
    if (typeof successMessage !== 'undefined' && successMessage && successMessage.trim() !== '') {
        Swal.fire({
            icon: 'success',
            title: 'Thành công',
            text: successMessage,
            timer: 3000,
            timerProgressBar: true,
            showConfirmButton: false
        });
    }
    
    // Error message
    if (typeof errorMessage !== 'undefined' && errorMessage && errorMessage.trim() !== '') {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: errorMessage,
            confirmButtonText: 'OK'
        });
    }
}

/* ===== STATUS CHANGE ===== */
function confirmStatusChange(serviceId, activateService) {
    const title = activateService ? 'Kích hoạt dịch vụ' : 'Tạm ngưng dịch vụ';
    const text = activateService 
        ? 'Bạn có chắc chắn muốn kích hoạt lại dịch vụ này?' 
        : 'Bạn có chắc chắn muốn tạm ngưng dịch vụ này?';
    const confirmButtonText = activateService ? 'Kích hoạt' : 'Tạm ngưng';
    const confirmButtonColor = activateService ? '#28a745' : '#dc3545';
    
    Swal.fire({
        title: title,
        text: text,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: confirmButtonColor,
        cancelButtonColor: '#6c757d',
        confirmButtonText: confirmButtonText,
        cancelButtonText: 'Hủy',
        reverseButtons: true
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
