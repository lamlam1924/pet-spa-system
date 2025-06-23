// Constants and configurations
const CONFIG = {
    CURRENCY: 'VND',
    LOCALE: 'vi-VN',
    NOTIFICATION_TIMEOUT: 5000
};

// Định nghĩa màu đỏ thương hiệu
const MAIN_RED = '#e53935';

// Đơn giản hóa notification
const notificationManager = {
    show(message, type = 'info') {
        const n = document.createElement('div');
        n.className = `notification ${type}`;
        n.style.background = type === 'error' ? MAIN_RED : '#fff';
        n.style.color = type === 'error' ? '#fff' : MAIN_RED;
        n.style.border = `1px solid ${MAIN_RED}`;
        n.style.position = 'fixed';
        n.style.top = '24px';
        n.style.right = '24px';
        n.style.padding = '12px 24px';
        n.style.borderRadius = '8px';
        n.style.zIndex = 9999;
        n.innerHTML = `<span>${message}</span>`;
        document.body.appendChild(n);
        setTimeout(() => n.remove(), 2500);
    },
    error(msg) { this.show(msg, 'error'); },
    warning(msg) { this.show(msg, 'warning'); }
};

// Main Booking Manager
const BookingManager = {
    step: 1,
    totalSteps: 3,
    selectedServices: new Set(),
    selectedPets: new Set(),

    init() {
        this.cacheDom();
        this.bindEvents();
        this.updateStep();
        this.updateSidebar();
    },

    cacheDom() {
        this.prevBtn = document.querySelector('.btn-prev');
        this.nextBtn = document.querySelector('.btn-next');
        this.steps = document.querySelectorAll('.progress-steps .step');
        this.stepContents = document.querySelectorAll('.booking-step');
        this.serviceBtns = document.querySelectorAll('.btn-select');
        this.filterBtns = document.querySelectorAll('.filter-btn');
        this.petCheckboxes = document.querySelectorAll('.pet-checkbox');
        this.petCounter = document.querySelector('.pet-counter .count');
        this.sidebarList = document.getElementById('selected-services-sidebar');
        this.sidebarTotal = document.getElementById('selected-services-total');
    },

    bindEvents() {
        if (this.prevBtn) this.prevBtn.onclick = () => this.gotoStep(this.step - 1);
        if (this.nextBtn) this.nextBtn.onclick = () => this.handleNext();

        this.serviceBtns.forEach(btn => {
            btn.onclick = () => this.toggleService(btn);
        });

        this.filterBtns.forEach(btn => {
            btn.onclick = () => this.filterCategory(btn);
        });

        this.petCheckboxes.forEach(cb => {
            cb.onchange = () => this.updatePetCounter();
        });
    },

    gotoStep(step) {
        if (step < 1 || step > this.totalSteps) return;
        this.step = step;
        this.updateStep();
    },

    handleNext() {
        if (this.step === 1 && this.selectedServices.size === 0) {
            notificationManager.warning('Vui lòng chọn ít nhất một dịch vụ');
            return;
        }
        if (this.step === 2) {
            let valid = true;
            document.querySelectorAll('.booking-step.active [required]').forEach(field => {
                if (!field.value.trim()) {
                    valid = false;
                    field.classList.add('error');
                } else {
                    field.classList.remove('error');
                }
            });
            if (!valid) {
                notificationManager.error('Vui lòng điền đầy đủ thông tin');
                return;
            }
            if (this.selectedPets.size === 0) {
                notificationManager.warning('Vui lòng chọn ít nhất một thú cưng');
                return;
            }
        }
        this.gotoStep(this.step + 1);
    },

    updateStep() {
        this.steps.forEach((step, idx) => {
            step.classList.toggle('active', idx + 1 === this.step);
            step.classList.toggle('completed', idx + 1 < this.step);
        });
        this.stepContents.forEach((content, idx) => {
            content.style.display = (idx + 1 === this.step) ? '' : 'none';
            content.classList.toggle('active', idx + 1 === this.step);
        });
        if (this.prevBtn) this.prevBtn.disabled = this.step === 1;
        if (this.nextBtn) {
            this.nextBtn.innerHTML = this.step === this.totalSteps
                ? '<i class="fa fa-check"></i> Xác nhận'
                : 'Tiếp tục <i class="fa fa-arrow-right"></i>';
        }
    },

    toggleService(btn) {
        const id = btn.dataset.service;
        const name = btn.dataset.name;
        const price = parseInt(btn.dataset.price);
        if (this.selectedServices.has(id)) {
            this.selectedServices.delete(id);
            btn.classList.remove('selected');
            btn.innerHTML = '<i class="fa fa-plus"></i> Chọn dịch vụ';
        } else {
            this.selectedServices.add(id);
            btn.classList.add('selected');
            btn.innerHTML = '<i class="fa fa-check"></i> Đã chọn';
        }
        this.updateSidebar();
    },

    filterCategory(btn) {
        this.filterBtns.forEach(b => b.classList.remove('active'));
        btn.classList.add('active');
        const cat = btn.dataset.category;
        document.querySelectorAll('.service-card').forEach(card => {
            card.style.display = (cat === 'all' || card.dataset.category === cat) ? '' : 'none';
        });
    },

    togglePet(cb) {
        if (cb.checked) this.selectedPets.add(cb.value);
        else this.selectedPets.delete(cb.value);
        if (this.petCounter) this.petCounter.textContent = this.selectedPets.size;
    },

    updateSidebar() {
        if (!this.sidebarList || !this.sidebarTotal) return;
        this.sidebarList.innerHTML = '';
        let total = 0;
        this.selectedServices.forEach(id => {
            const btn = document.querySelector(`[data-service="${id}"]`);
            if (btn) {
                const name = btn.dataset.name;
                const price = parseInt(btn.dataset.price);
                total += price;
                this.sidebarList.innerHTML += `
                    <li class="list-group-item d-flex justify-content-between align-items-center">
                        <span><i class="fa fa-check-circle" style="color:${MAIN_RED};margin-right:8px"></i>${name}</span>
                        <span class="badge rounded-pill" style="background:${MAIN_RED};color:#fff">${price.toLocaleString()} đ</span>
                    </li>`;
            }
        });
        this.sidebarTotal.textContent = total.toLocaleString() + ' đ';
    },

    updatePetCounter() {
        if (this.petCounter) {
            const count = Array.from(this.petCheckboxes).filter(cb => cb.checked).length;
            this.petCounter.textContent = count;
        }
    }
};

document.addEventListener('DOMContentLoaded', function () {
    // Đếm số thú cưng đã chọn
    function updatePetCounter() {
        var checked = document.querySelectorAll('.pet-checkbox:checked').length;
        var counter = document.querySelector('.pet-counter .count');
        if (counter) counter.textContent = checked;
    }

    // Lắng nghe sự kiện thay đổi checkbox thú cưng
    document.querySelectorAll('.pet-checkbox').forEach(function (cb) {
        cb.addEventListener('change', updatePetCounter);
    });

    // Gọi khi load trang để cập nhật số ban đầu
    updatePetCounter();
});

document.addEventListener('DOMContentLoaded', () => BookingManager.init());