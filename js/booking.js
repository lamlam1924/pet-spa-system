// Constants and configurations
const CONFIG = {
    CURRENCY: 'VND',
    LOCALE: 'vi-VN',
    NOTIFICATION_TIMEOUT: 5000
};

// const COMBOS = {
//     basic: {
//         name: 'Combo Cơ bản',
//         price: 300000,
//         services: ['bath', 'nailcut'],
//         savings
//     },
//     premium: {
//         name: 'Combo Cao cấp',
//         price: 660000,
//         services: ['bath', 'nailcut', 'grooming'],
//         savings: '25%'
//     },
//     vip: {
//         name: 'Combo VIP',
//         price: 1000000,
//         services: ['bath', 'nailcut', 'grooming', 'spa'],
//         savings: '30%'
//     }
// };

// const SERVICES = {
//     bath: {
//         name: 'Tắm spa',
//         price: 150000,
//         includedIn: ['basic', 'premium', 'vip']
//     },
//     nailcut: {
//         name: 'Cắt móng',
//         price: 50000,
//         includedIn: ['basic', 'premium', 'vip']
//     },
//     grooming: {
//         name: 'Cắt tỉa lông',
//         price: 200000,
//         includedIn: ['premium', 'vip']
//     },
//     spa: {
//         name: 'Massage',
//         price: 300000,
//         includedIn: ['vip']
//     }
// };

// Notification Manager
const notificationManager = {
    show(message, type) {
        const notification = document.createElement('div');
        notification.className = `notification ${type}`;
        notification.innerHTML = `
            <div class="notification-content">
                <i class="fa ${this.getIcon(type)}"></i>
                <span>${message}</span>
            </div>
        `;
        
        document.body.appendChild(notification);
        
        setTimeout(() => {
            notification.remove();
        }, CONFIG.NOTIFICATION_TIMEOUT);
    },

    getIcon(type) {
        switch(type) {
            case 'success': return 'fa-check-circle';
            case 'error': return 'fa-exclamation-circle';
            case 'warning': return 'fa-exclamation-triangle';
            default: return 'fa-info-circle';
        }
    },

    success(message) {
        this.show(message, 'success');
    },

    error(message) {
        this.show(message, 'error');
    },

    warning(message) {
        this.show(message, 'warning');
    },

    confirm(message, onConfirm, onCancel) {
        const modal = document.createElement('div');
        modal.className = 'confirmation-modal';
        modal.innerHTML = `
            <div class="modal-content">
                <p>${message}</p>
                <div class="modal-actions">
                    <button class="btn-confirm">Xác nhận</button>
                    <button class="btn-cancel">Hủy</button>
                </div>
            </div>
        `;

        document.body.appendChild(modal);

        modal.querySelector('.btn-confirm').addEventListener('click', () => {
            onConfirm();
            modal.remove();
        });

        modal.querySelector('.btn-cancel').addEventListener('click', () => {
            onCancel();
            modal.remove();
        });
    }
};

// Main Booking Manager
const BookingManager = {
    selectedServices: new Set(),

    init() {
        // Initialize state
        this.currentStep = 1;
        this.totalSteps = 3;
        this.selectedServices = new Set();
        this.selectedPets = new Set();

        // Cache DOM elements
        this.prevButton = document.querySelector('.btn-prev');
        this.nextButton = document.querySelector('.btn-next');
        this.steps = document.querySelectorAll('.step');
        this.stepContents = document.querySelectorAll('.booking-step');

        this.bindEvents();
        this.updateNavigationState();
        this.initPetSelection();

        // Load saved services from storage
        const savedServices = ServiceStorageManager.getServices();
        savedServices.forEach(service => {
            this.selectedServices.add(service.id);
            const button = document.querySelector(`[data-service="${service.id}"] .btn-select`);
            if (button) {
                button.classList.add('selected');
                button.innerHTML = '<i class="fa fa-check"></i> Đã chọn';
            }
        });

        this.updateSelectedServices();
    },

    bindEvents() {
        // Navigation buttons
        this.prevButton.addEventListener('click', () => this.navigate('prev'));
        this.nextButton.addEventListener('click', () => this.navigate('next'));

        // Service selection
        document.querySelectorAll('.btn-select').forEach(btn => {
            btn.addEventListener('click', (e) => {
                console.log('Service button clicked');
                this.toggleService(e.currentTarget);
            });
        });

        // Category filtering
        document.querySelectorAll('.filter-btn').forEach(btn => {
            btn.addEventListener('click', (e) => this.handleCategoryFilter(e.currentTarget));
        });
    },

    initPetSelection() {
        const checkboxes = document.querySelectorAll('.pet-checkbox');
        const counter = document.querySelector('.pet-counter .count');
        
        checkboxes.forEach(checkbox => {
            checkbox.addEventListener('change', (e) => {
                if (e.target.checked) {
                    this.selectedPets.add(e.target.value);
                } else {
                    this.selectedPets.delete(e.target.value);
                }
                
                // Update counter
                counter.textContent = this.selectedPets.size;
                
                // Add animation
                counter.classList.add('bounce');
                setTimeout(() => counter.classList.remove('bounce'), 300);
            });
        });
    },

    navigate(direction) {
        const currentStep = document.querySelector('.booking-step.active');
        const currentStepNumber = parseInt(currentStep.dataset.step);
        
        if (direction === 'next' && !this.validateCurrentStep()) {
            return;
        }

        const nextStepNumber = direction === 'next' ? currentStepNumber + 1 : currentStepNumber - 1;
        
        if (nextStepNumber < 1 || nextStepNumber > 3) {
            return;
        }

        // Cập nhật currentStep
        this.currentStep = nextStepNumber;

        // Cập nhật trạng thái active cho steps
        document.querySelectorAll('.booking-step').forEach(step => {
            step.classList.remove('active');
        });
        
        document.querySelector(`[data-step="${nextStepNumber}"]`).classList.add('active');
        
        // Cập nhật progress steps
        document.querySelectorAll('.progress-steps .step').forEach((step, index) => {
            if (index + 1 < nextStepNumber) {
                step.classList.add('completed');
                step.classList.remove('active');
            } else if (index + 1 === nextStepNumber) {
                step.classList.add('active');
                step.classList.remove('completed');
            } else {
                step.classList.remove('active', 'completed');
            }
        });

        this.updateNavigationState();
    },

    validateCurrentStep() {
        const currentStep = document.querySelector('.booking-step.active');
        const stepNumber = currentStep.dataset.step;

        switch(stepNumber) {
            case '1':
                if (this.selectedServices.size === 0) {
                    notificationManager.warning('Vui lòng chọn ít nhất một dịch vụ');
                    return false;
                }
                return true;

            case '2':
                // Kiểm tra form thông tin
                const form = currentStep.querySelector('.customer-form');
                const requiredFields = form.querySelectorAll('[required]');
                let isValid = true;
                
                requiredFields.forEach(field => {
                    if (!field.value.trim()) {
                        isValid = false;
                        field.classList.add('error');
                        notificationManager.error(`Vui lòng điền ${field.previousElementSibling.textContent.toLowerCase()}`);
                    } else {
                        field.classList.remove('error');
                    }
                });

                // Kiểm tra chọn thú cưng
                const selectedPets = currentStep.querySelectorAll('.pet-checkbox:checked');
                if (selectedPets.length === 0) {
                    notificationManager.warning('Vui lòng chọn ít nhất một thú cưng');
                    return false;
                }

                return isValid;

            case '3':
                return true;

            default:
                return false;
        }
    },

    updateNavigationState() {
        // Update progress steps
        this.steps.forEach((step, index) => {
            const stepNum = index + 1;
            step.classList.toggle('active', stepNum === this.currentStep);
            step.classList.toggle('completed', stepNum < this.currentStep);
        });

        // Update step content visibility
        this.stepContents.forEach((content, index) => {
            content.classList.toggle('active', index + 1 === this.currentStep);
        });

        // Update navigation buttons
        this.prevButton.disabled = this.currentStep === 1;
        
        // Update next button text
        if (this.currentStep === this.totalSteps) {
            this.nextButton.innerHTML = '<i class="fa fa-check"></i> Xác nhận đặt lịch';
            this.nextButton.classList.add('btn-confirm');
        } else {
            this.nextButton.innerHTML = 'Tiếp tục <i class="fa fa-arrow-right"></i>';
            this.nextButton.classList.remove('btn-confirm');
        }

        // Add animation
        const activeContent = document.querySelector('.booking-step.active');
        activeContent.classList.add('animate-in');
    },

    toggleService(button) {
        const serviceId = button.dataset.service;
        
        if (this.selectedServices.has(serviceId)) {
            this.selectedServices.delete(serviceId);
            button.classList.remove('selected');
            button.innerHTML = '<i class="fa fa-plus"></i> Chọn dịch vụ';
        } else {
            this.selectedServices.add(serviceId);
            button.classList.add('selected');
            button.innerHTML = '<i class="fa fa-check"></i> Đã chọn';
        }

        // Lưu vào storage
        ServiceStorageManager.saveServices(this.selectedServices);
        
        this.updateSelectedServices();
    },

    resetCategoryFilter() {
        const activeGroup = document.querySelector(`.category-group.active`);
        if (activeGroup) {
            const allButton = activeGroup.querySelector('[data-category="all"]');
            if (allButton) {
                this.filterByCategory(allButton);
            }
        }
    },

    handleCategoryFilter(button) {
        // Update active state
        const group = button.closest('.category-group');
        group.querySelectorAll('.category-btn').forEach(btn => {
            btn.classList.remove('active');
        });
        button.classList.add('active');

        // Filter services
        const category = button.dataset.category;
        const services = document.querySelectorAll('.service-card');
        
        services.forEach(service => {
            if (category === 'all' || service.dataset.category === category) {
                service.style.display = 'block';
                service.classList.add('animate-in');
            } else {
                service.style.display = 'none';
                service.classList.remove('animate-in');
            }
        });
    },

    updateServiceCount() {
        document.querySelectorAll('.filter-btn').forEach(btn => {
            const category = btn.dataset.category;
            const count = category === 'all' 
                ? document.querySelectorAll('.service-card').length
                : document.querySelectorAll(`.service-item[data-category="${category}"]`).length;
            
            btn.querySelector('.service-count').textContent = count;
        });
    },

    updateSelectedServices() {
        const summaryContainer = document.querySelector('.selected-services');
        if (!summaryContainer) return;

        let html = '';
        let total = 0;

        this.selectedServices.forEach(serviceId => {
            const serviceEl = document.querySelector(`[data-service="${serviceId}"], [data-category="${serviceId}"]`);
            if (serviceEl) {
                const name = serviceEl.querySelector('h4, h3').textContent;
                const price = serviceEl.querySelector('.price, .current').textContent;
                
                html += `
                    <div class="selected-service">
                        <div class="service-info">
                            <span class="name">${name}</span>
                            <span class="price">${price}</span>
                        </div>
                        <button class="remove-service" data-id="${serviceId}">
                            <i class="fa fa-times"></i>
                        </button>
                    </div>
                `;

                // Add to total
                const priceNum = parseInt(price.replace(/\D/g, ''));
                total += priceNum;
            }
        });

        // Add total if there are services selected
        if (this.selectedServices.size > 0) {
            html += `
                <div class="summary-total">
                    <span>Tổng cộng:</span>
                    <span class="total-amount">${total.toLocaleString()}đ</span>
                </div>
            `;
        }

        summaryContainer.innerHTML = html;

        // Bind remove buttons
        summaryContainer.querySelectorAll('.remove-service').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const serviceId = e.currentTarget.dataset.id;
                const serviceButton = document.querySelector(`[data-service="${serviceId}"] .btn-select, [data-category="${serviceId}"] .btn-select`);
                if (serviceButton) {
                    this.toggleService(serviceButton);
                }
            });
        });
    },

    reset() {
        this.selectedServices.clear();
        this.updateSelectedServices();
    }
};

const ServiceStorageManager = {
    STORAGE_KEY: 'selectedServices',

    // Lưu dịch vụ đã chọn
    saveServices(services) {
        const servicesData = Array.from(services).map(serviceId => {
            const serviceEl = document.querySelector(`[data-service="${serviceId}"]`);
            return {
                id: serviceId,
                name: serviceEl.querySelector('h4').textContent,
                price: serviceEl.querySelector('.price').textContent,
                duration: serviceEl.querySelector('.meta span:first-child').textContent
            };
        });
        localStorage.setItem(this.STORAGE_KEY, JSON.stringify(servicesData));
    },

    // Lấy dịch vụ đã lưu
    getServices() {
        const services = localStorage.getItem(this.STORAGE_KEY);
        return services ? JSON.parse(services) : [];
    },

    // Xóa dịch vụ đã lưu
    clearServices() {
        localStorage.removeItem(this.STORAGE_KEY);
    }
};

// Xử lý filter theo category
document.addEventListener('DOMContentLoaded', function() {
    const filterButtons = document.querySelectorAll('.filter-btn');
    const serviceCards = document.querySelectorAll('.service-card');

    filterButtons.forEach(button => {
        button.addEventListener('click', function() {
            // Xóa active class từ tất cả buttons
            filterButtons.forEach(btn => btn.classList.remove('active'));
            // Thêm active class vào button được click
            this.classList.add('active');

            const selectedCategory = this.getAttribute('data-category');

            // Hiển thị/ẩn service cards dựa trên category
            serviceCards.forEach(card => {
                if (selectedCategory === 'all') {
                    card.style.display = 'block';
                } else {
                    if (card.getAttribute('data-category') === selectedCategory) {
                        card.style.display = 'block';
                    } else {
                        card.style.display = 'none';
                    }
                }
            });
        });
    });

    // Xử lý chuyển đổi giữa dịch vụ lẻ và combo
    const typeButtons = document.querySelectorAll('.type-btn');
    const singleCategories = document.getElementById('singleCategories');
    const comboCategories = document.getElementById('comboCategories');

    typeButtons.forEach(button => {
        button.addEventListener('click', function() {
            // Xóa active class từ tất cả type buttons
            typeButtons.forEach(btn => btn.classList.remove('active'));
            // Thêm active class vào button được click
            this.classList.add('active');

            const serviceType = this.getAttribute('data-type');
            
            if (serviceType === 'single') {
                singleCategories.style.display = 'flex';
                comboCategories.style.display = 'none';
            } else {
                singleCategories.style.display = 'none';
                comboCategories.style.display = 'flex';
            }
        });
    });
});

// Main initialization
document.addEventListener('DOMContentLoaded', function() {
    // Initialize the booking manager
    BookingManager.init();

    
});