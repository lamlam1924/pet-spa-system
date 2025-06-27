document.addEventListener('DOMContentLoaded', () => {

    // === State Variables ===
    let currentStep = 1;
    let selectedServices = [];

    // === Toast Notification ===
    function showToast(message, type = 'info') {
        const container = document.getElementById('toastContainer');
        if (!container) {
            console.warn('Không tìm thấy #toastContainer');
            return;
        }

        const toast = document.createElement('div');
        toast.className = `custom-toast ${type} show mb-2 d-flex align-items-center justify-content-between`;
        toast.style.animation = 'slideInTop 0.3s ease forwards';
        toast.setAttribute('role', 'alert');

        const iconMap = {
            success: '<i class="fas fa-check-circle text-success me-2"></i>',
            error: '<i class="fas fa-times-circle text-danger me-2"></i>',
            info: '<i class="fas fa-info-circle text-info me-2"></i>'
        };

        toast.innerHTML = `
        <div class="toast-content d-flex align-items-center p-2">
            <span class="toast-icon">${iconMap[type] || ''}</span>
            <div class="toast-message">${message}</div>
        </div>
        <button type="button" class="btn-close m-2" aria-label="Đóng"></button>
    `;

        toast.querySelector('.btn-close').addEventListener('click', () => {
            toast.classList.add('hiding');
            setTimeout(() => toast.remove(), 300);
        });

        setTimeout(() => {
            toast.classList.add('hiding');
            setTimeout(() => toast.remove(), 300);
        }, 2500);

        container.appendChild(toast);
    }


    // ---- Toast Notification ----
    function showFieldError(field, message) {
        if (!field) return;
        field.classList.add('error');
        let err = field.closest('.form-group, .form-section')?.querySelector('.field-error');
        if (err) {
            err.textContent = message;
            err.style.display = 'block';
        }
    }

    function clearFieldError(field) {
        if (!field) return;
        field.classList.remove('error');
        let err = field.closest('.form-group, .form-section')?.querySelector('.field-error');
        if (err) {
            err.textContent = '';
            err.style.display = 'none';
        }
    }

    function clearAllErrors() {
        document.querySelectorAll('.field-error').forEach(div => {
            div.textContent = '';
            div.style.display = 'none';
        });
        document.querySelectorAll('.form-control.error').forEach(input => input.classList.remove('error'));
    }

    // --- Hàm này quản lý required theo step ---
    function updateRequiredAttributes(currentStep) {
        document.querySelectorAll('.booking-step').forEach(step => {
            const isActive = parseInt(step.dataset.step) === currentStep;
            step.querySelectorAll('input, select, textarea').forEach(input => {
                // Lưu trạng thái required gốc (chỉ lưu 1 lần)
                if (input.dataset.initialRequired === undefined) {
                    input.dataset.initialRequired = input.hasAttribute('required') ? "true" : "false";
                }
                if (isActive && input.dataset.initialRequired === "true") {
                    input.setAttribute('required', 'required');
                } else {
                    input.removeAttribute('required');
                }
            });
        });
    }

    updateRequiredAttributes(currentStep); // Đảm bảo chỉ input step đầu có required


    // === Validation cho Step 2 ===
    function validateStep2() {
        clearAllErrors();

        const errors = [];
        let firstErrorField = null;

        const nameInput = document.querySelector('.customer-form input[type="text"]');
        if (!nameInput || !nameInput.value.trim()) {
            errors.push({message: 'Vui lòng nhập họ tên khách hàng.', field: nameInput});
            if (!firstErrorField) firstErrorField = nameInput;
        }

        const phoneInput = document.querySelector('.customer-form input[type="tel"]');
        if (!phoneInput || !phoneInput.value.trim()) {
            errors.push({message: 'Vui lòng nhập số điện thoại.', field: phoneInput});
            if (!firstErrorField) firstErrorField = phoneInput;
        } else if (!/^[0-9]{9,11}$/.test(phoneInput.value.trim())) {
            errors.push({message: 'Số điện thoại không hợp lệ.', field: phoneInput});
            if (!firstErrorField) firstErrorField = phoneInput;
        }

        const emailInput = document.querySelector('.customer-form input[type="email"]');
        if (!emailInput || !emailInput.value.trim()) {
            errors.push({message: 'Vui lòng nhập email.', field: emailInput});
            if (!firstErrorField) firstErrorField = emailInput;
        } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(emailInput.value.trim())) {
            errors.push({message: 'Email không hợp lệ.', field: emailInput});
            if (!firstErrorField) firstErrorField = emailInput;
        }

        const dateInput = document.querySelector('.customer-form input[type="date"]');
        if (!dateInput || !dateInput.value) {
            errors.push({message: 'Vui lòng chọn ngày hẹn.', field: dateInput});
            if (!firstErrorField) firstErrorField = dateInput;
        } else {
            const selectedDate = new Date(dateInput.value);
            const today = new Date();
            today.setHours(0, 0, 0, 0);
            if (selectedDate < today) {
                errors.push({message: 'Ngày hẹn không được là ngày trong quá khứ.', field: dateInput});
                if (!firstErrorField) firstErrorField = dateInput;
            }
        }

        const timeInput = document.querySelector('.customer-form input[type="time"]');
        if (!timeInput || !timeInput.value) {
            errors.push({message: 'Vui lòng chọn giờ hẹn.', field: timeInput});
            if (!firstErrorField) firstErrorField = timeInput;
        } else if (dateInput && dateInput.value) {
            const selectedDate = new Date(dateInput.value);
            const today = new Date();
            today.setHours(0, 0, 0, 0);
            if (selectedDate.getTime() === today.getTime()) {
                const [hours, minutes] = timeInput.value.split(':').map(Number);
                const currentHours = new Date().getHours();
                const currentMinutes = new Date().getMinutes();
                if (hours < currentHours || (hours === currentHours && minutes < currentMinutes)) {
                    errors.push({message: 'Giờ hẹn không được là giờ trong quá khứ.', field: timeInput});
                    if (!firstErrorField) firstErrorField = timeInput;
                }
            }
        }

        const selectedPets = document.querySelectorAll('.pet-checkbox:checked');
        if (!selectedPets || selectedPets.length === 0) {
            errors.push({message: 'Vui lòng chọn ít nhất một thú cưng.', field: null});
            if (!firstErrorField) firstErrorField = document.querySelector('.pet-list');
        }

        // Hiển thị lỗi cho từng trường
        errors.forEach(e => {
            if (e.field) {
                showFieldError(e.field, e.message);
            } else {
                // Báo lỗi cho pet
                let petErrorDiv = document.querySelector('.pet-selection .pet-error');
                if (petErrorDiv) {
                    petErrorDiv.textContent = e.message;
                    petErrorDiv.style.display = 'block';
                }
            }
        });

        // Tự động focus và scroll tới lỗi đầu tiên
        if (firstErrorField) {
            firstErrorField.scrollIntoView({behavior: 'smooth', block: 'center'});
            if (firstErrorField.focus) firstErrorField.focus();
        }

        return errors.length ? {valid: false, errors} : {valid: true};
    }


    // === Sidebar Update ===
    function updateSelectedServicesSidebar() {
        const list = document.getElementById('selected-services-sidebar');
        if (!list) {
            console.error('Không tìm thấy #selected-services-sidebar');
            return;
        }

        let total = 0;
        list.innerHTML = '';
        selectedServices.forEach(sv => {
            const li = document.createElement('li');
            li.className = 'list-group-item d-flex justify-content-between align-items-center';
            li.innerHTML = `
                    <span><i class="fa fa-check-circle text-success me-2"></i>${sv.name}</span>
                    <span class="badge bg-primary rounded-pill">${sv.price.toLocaleString()} đ</span>
                `;
            list.appendChild(li);
            total += sv.price;
        });

        const totalEl = document.getElementById('selected-services-total');
        if (totalEl) totalEl.textContent = total.toLocaleString() + ' đ';
    }

    // === Confirmation Step ===
    function updateConfirmationStep() {
        const confirmServiceEl = document.getElementById('confirm-service');
        if (confirmServiceEl) confirmServiceEl.textContent = selectedServices.map(sv => sv.name).join(', ');

        const confirmPetsList = document.getElementById('confirm-pets-list');
        if (confirmPetsList) {
            const selectedPets = [...document.querySelectorAll('.pet-checkbox:checked')]
                .map(petCheckbox => {
                    const label = document.querySelector(`label[for="${petCheckbox.id}"]`);
                    if (!label) return null;
                    const name = label.querySelector('h5')?.innerText.trim() || '';
                    const breed = label.querySelector('.pet-breed')?.innerText.trim() || '';
                    return {name, breed};
                })
                .filter(p => p && p.name);

            confirmPetsList.innerHTML = '';
            selectedPets.forEach(pet => {
                const li = document.createElement('li');
                li.textContent = `${pet.name} (${pet.breed})`;
                confirmPetsList.appendChild(li);
            });
        }

        const confirmCustomerName = document.getElementById('confirm-customer-name');
        if (confirmCustomerName) {
            const nameInput = document.querySelector('.customer-form input[type="text"]');
            confirmCustomerName.textContent = nameInput?.value.trim() || '';
        }

        const confirmCustomerPhone = document.getElementById('confirm-customer-phone');
        if (confirmCustomerPhone) {
            const phoneInput = document.querySelector('.customer-form input[type="tel"]');
            confirmCustomerPhone.textContent = phoneInput?.value.trim() || '';
        }

        const confirmCustomerEmail = document.getElementById('confirm-customer-email');
        if (confirmCustomerEmail) {
            const emailInput = document.querySelector('.customer-form input[type="email"]');
            confirmCustomerEmail.textContent = emailInput?.value.trim() || '';
        }

        const confirmTimeEl = document.getElementById('confirm-time');
        if (confirmTimeEl) {
            const dateInput = document.querySelector('.customer-form input[type="date"]');
            const timeInput = document.querySelector('.customer-form input[type="time"]');
            const dateVal = dateInput?.value || '';
            const timeVal = timeInput?.value || '';
            confirmTimeEl.textContent = dateVal && timeVal ? `${dateVal} lúc ${timeVal}` : 'Chưa chọn thời gian';
        }

        const confirmNoteEl = document.getElementById('confirm-note');
        if (confirmNoteEl) {
            const noteInput = document.querySelector('.customer-form textarea');
            confirmNoteEl.textContent = noteInput?.value.trim() || '(Không có)';
        }
        const selectedIds = selectedServices.map(sv => sv.serviceId).join(',');
        document.getElementById('SelectedServiceIds').value = selectedIds;
    }

    // ---- Ngăn submit form nếu chưa phải bước 3 ----
    const mainForm = document.querySelector('.booking-container form');
    if (mainForm) {
        mainForm.addEventListener('submit', function (e) {
            if (currentStep !== 3) {
                e.preventDefault();
                return false;
            }
            // Đảm bảo cập nhật input hidden (dự phòng)
            updateConfirmationStep();
        });
    }

    // ---- Ngăn Enter ở input/textarea khi chưa phải bước 3 ----
    document.querySelectorAll('.customer-form input, .customer-form textarea').forEach(input => {
        input.addEventListener('keydown', function (e) {
            if (e.key === 'Enter' && currentStep !== 3) {
                e.preventDefault();
            }
        });
    });


    // === Event Listeners ===
    // Service Selection
    document.addEventListener('click', e => {
        const btn = e.target.closest('.btn-select');
        if (!btn) return;

        const {service: serviceId, name: serviceName, price: servicePrice} = btn.dataset;
        const price = parseFloat(servicePrice);
        const index = selectedServices.findIndex(sv => sv.serviceId === serviceId);

        if (index === -1) {
            selectedServices.push({serviceId, name: serviceName, price});
            btn.classList.add('selected');
            btn.innerHTML = '<i class="fa fa-check"></i> Đã chọn';
            showToast(`Đã thêm dịch vụ: ${serviceName}`, 'success');
        } else {
            selectedServices.splice(index, 1);
            btn.classList.remove('selected');
            btn.innerHTML = '<i class="fa fa-plus"></i> Chọn';
            showToast(`Đã bỏ dịch vụ: ${serviceName}`, 'info');
        }

        updateSelectedServicesSidebar();
    });


    // ---- Step Navigation: Next ----
    const btnNext = document.querySelector('.btn-next');
    if (btnNext) {
        btnNext.addEventListener('click', () => {
            if (currentStep === 1 && selectedServices.length === 0) {
                showToast('Oops! Bạn chưa chọn dịch vụ nào, chọn một cái nha!', 'error');
                return;
            }
            if (currentStep === 2) {
                clearAllErrors();
                const validation = validateStep2();
                if (!validation.valid) {
                    validation.errors.forEach(({message, field}, index) => {
                        if (field) {
                            showFieldError(field, message);
                            if (index === 0) {
                                field.focus();
                                field.scrollIntoView({behavior: 'smooth', block: 'center'});
                            }
                        }
                        showToast(message, 'error');
                    });
                    return;
                }
                updateConfirmationStep();
            }
            if (currentStep === 3) {
                updateConfirmationStep();
                return;
            }
            // Ẩn step cũ
            const currentStepEl = document.querySelector('.booking-step.active');
            if (currentStepEl) {
                currentStepEl.style.display = 'none';
                currentStepEl.classList.remove('active');
            }
            currentStep++;
            updateRequiredAttributes(currentStep);
            const nextStep = document.querySelector(`.booking-step[data-step="${currentStep}"]`);
            if (nextStep) {
                nextStep.style.display = 'block';
                nextStep.classList.add('active');
                document.querySelectorAll('.step').forEach(stepEl => {
                    stepEl.classList.toggle('active', parseInt(stepEl.dataset.step) === currentStep);
                });
                document.querySelector('.btn-prev').disabled = currentStep === 1;
                showToast(`Bước ${currentStep} đây! Tiếp tục nào!`, 'info');
                scrollToFormTop();
            }
        });
    }

    // ---- Step Navigation: Previous ----
    const btnPrev = document.querySelector('.btn-prev');
    if (btnPrev) {
        btnPrev.addEventListener('click', () => {
            if (currentStep <= 1) return;
            const currentStepEl = document.querySelector('.booking-step.active');
            if (currentStepEl) {
                currentStepEl.style.display = 'none';
                currentStepEl.classList.remove('active');
            }
            currentStep--;
            updateRequiredAttributes(currentStep);
            const prevStep = document.querySelector(`.booking-step[data-step="${currentStep}"]`);
            if (prevStep) {
                prevStep.style.display = 'block';
                prevStep.classList.add('active');
                document.querySelectorAll('.step').forEach(stepEl => {
                    stepEl.classList.toggle('active', parseInt(stepEl.dataset.step) === currentStep);
                });
                btnPrev.disabled = currentStep === 1;
                showToast(`Quay lại bước ${currentStep} nè!`, 'info');
                scrollToFormTop();
            }
        });
    }

    // ---- Service Filtering ----
    document.querySelectorAll('.filter-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            const selectedCategory = btn.dataset.category;

            document.querySelectorAll('.filter-btn').forEach(b => b.classList.remove('active'));
            btn.classList.add('active');

            document.querySelectorAll('.service-card').forEach(card => {
                const cardCategory = card.dataset.category;
                card.style.display = selectedCategory === 'all' || cardCategory === selectedCategory ? 'block' : 'none';
            });
        });
    });

    // Modal Thêm thú cưng mới
    const btnAddPet = document.getElementById('btnAddPet');
    if (btnAddPet) {
        console.log("Đang gắn event cho btnAddPet:", document.getElementById('btnAddPet'));

        btnAddPet.addEventListener('click', function () {
            console.log("Button Thêm thú cưng được bấm!"); // Debug
            const petName = document.getElementById('petNameModal').value.trim();
            const petBreed = document.getElementById('petBreedModal').value;
            if (!petName || !petBreed) {
                showToast('Vui lòng nhập đầy đủ tên và giống loài.', 'error');
                return;
            }
            const tempPetId = "new_" + Date.now();
            console.log("Button Thêm thú cưng được bấm!"); // phải thấy log này khi click
            const petList = document.querySelector('.pet-selection .pet-list');
            console.log('petList:', petList);

            if (petList) {
                petList.insertAdjacentHTML('beforeend', `
                    <div class="pet-item">
                        <input type="checkbox" id="pet-${tempPetId}" name="SelectedPetIds[]" value="${tempPetId}" class="pet-checkbox" checked>
                        <label for="pet-${tempPetId}" class="pet-card">
                            <div class="pet-avatar"><i class="fa fa-paw"></i></div>
                            <div class="pet-info">
                                <h5>${petName}</h5>
                                <span class="pet-breed">${petBreed}</span>
                            </div>
                        </label>
                    </div>
                `);
            } else {
                showToast('Không tìm thấy danh sách thú cưng!', 'error');
            }
            bootstrap.Modal.getOrCreateInstance(document.getElementById('addPetModal')).hide();
            document.getElementById('petNameModal').value = '';
            document.getElementById('petBreedModal').value = '';
            showToast(`Đã thêm thú cưng: ${petName}`, 'success');
        });
    }

    // ---- Xóa thú cưng (event delegation) ----
    const petList = document.querySelector('.pet-list');
    if (petList) {
        petList.addEventListener('click', function (e) {
            const removeBtn = e.target.closest('.btn-pet-remove');
            if (removeBtn) {
                const petItem = removeBtn.closest('.pet-item');
                if (petItem) {
                    petItem.remove();
                    showToast('Đã xóa thú cưng khỏi danh sách.', 'info');
                }
            }
        });
    }

    // ---- Clear lỗi khi nhập lại ----
    document.querySelectorAll('.customer-form input, .customer-form textarea')
        .forEach(input => input.addEventListener('input', () => clearFieldError(input)));

    // ---- Nút trang chủ & đặt lịch mới ----
    const btnGoHome = document.getElementById('btnGoHome');
    if (btnGoHome) {
        btnGoHome.addEventListener('click', () => {
            window.location.href = '/';
        }, {once: true});
    }
    const btnNewBooking = document.getElementById('btnNewBooking');
    if (btnNewBooking) {
        btnNewBooking.addEventListener('click', () => {
            window.location.reload();
        }, {once: true});
    }

    // ---- Scroll về đầu form ----
    function scrollToFormTop() {
        const formSection = document.querySelector('.appoint_ment_form');
        if (formSection) {
            formSection.scrollIntoView({behavior: 'smooth', block: 'start'});
        } else {
            window.scrollTo({top: 0, behavior: 'smooth'});
        }
    }

    // Confirm Appointment
    const btnConfirmAppointment = document.getElementById('btnConfirmAppointment');
    if (btnConfirmAppointment) {
        btnConfirmAppointment.addEventListener('click', function (e) {
            // Trước khi submit, đảm bảo update input ẩn (dự phòng)
            updateConfirmationStep();
            // Submit form
            this.form.submit();
            // Không show modal nữa, vì submit là redirect, không cần show JS modal!
        });
    }


    function scrollToFormTop() {
        const formSection = document.querySelector('.appoint_ment_form');
        if (formSection) {
            formSection.scrollIntoView({behavior: 'smooth', block: 'start'});
        } else {
            window.scrollTo({top: 0, behavior: 'smooth'});
        }
    }

});
    // Đảm bảo SelectedServiceIds và SelectedPetIds là nhiều input hidden cho đúng ASP.NET MVC
    
    function createHiddenInputs() {
        // Xóa input cũ
        document.querySelectorAll('input[name="SelectedServiceIds"]').forEach(el => el.remove());
        document.querySelectorAll('input[name="SelectedPetIds"]').forEach(el => el.remove());
    
        // Tạo input dịch vụ
        selectedServices.forEach(sv => {
            let input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'SelectedServiceIds';
            input.value = sv.serviceId;
            mainForm.appendChild(input);
        });
    
        // Tạo input thú cưng
        document.querySelectorAll('.pet-checkbox:checked').forEach(petCb => {
            let input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'SelectedPetIds';
            input.value = petCb.value;
            mainForm.appendChild(input);
        });
    }
    
    // Trong event submit form hoặc click nút xác nhận:
    btnConfirmAppointment.addEventListener('click', function (e) {
        createHiddenInputs(); // <-- THÊM DÒNG NÀY
        updateConfirmationStep();
        this.form.submit();
    });

function updateHiddenInputs() {
    // Remove old
    document.querySelectorAll('input[name="SelectedServiceIds"]').forEach(el => el.remove());
    document.querySelectorAll('input[name="SelectedPetIds"]').forEach(el => el.remove());
    // Add services
    selectedServices.forEach(sv => {
        let input = document.createElement('input');
        input.type = 'hidden';
        input.name = 'SelectedServiceIds';
        input.value = sv.serviceId;
        mainForm.appendChild(input);
    });
    // Add pets
    document.querySelectorAll('.pet-checkbox:checked').forEach(cb => {
        let input = document.createElement('input');
        input.type = 'hidden';
        input.name = 'SelectedPetIds';
        input.value = cb.value;
        mainForm.appendChild(input);
    });
}
// Và gọi hàm này NGAY TRƯỚC khi submit form








