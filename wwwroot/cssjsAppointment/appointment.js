document.addEventListener('DOMContentLoaded', () => {
    try {
        // === State Variables ===
        let currentStep = 1;
        let selectedServices = [];
        let toastQueue = [];

        // === Toast Notification ===
        function showToast(message, type = 'info') {
            const toastEl = document.getElementById('appToast');
            if (!toastEl) {
                console.error('Không tìm thấy #appToast');
                return;
            }

            const friendlyMessages = {
                success: {
                    addService: (name) => `Yay! Dịch vụ "${name}" đã được thêm!`,
                    removeService: (name) => `Dịch vụ "${name}" đã được bỏ nhé.`,
                    addPet: (name) => `Cưng xỉu! "${name}" đã vào danh sách!`,
                    confirm: `Đặt lịch thành công! Chúng mình sẽ liên hệ bạn sớm nha!`
                },
                error: {
                    noService: `Oops! Bạn chưa chọn dịch vụ nào, chọn một cái nha!`,
                    invalidInput: (msg) => `Ôi, ${msg.toLowerCase()[0] + msg.slice(1)}`,
                    addPet: `Hãy nhập đủ tên và giống loài cho bé cưng nha!`,
                    error: `Có lỗi rồi! Thử lại nhé!`
                },
                info: {
                    step: (step) => `Bước ${step} đây! Tiếp tục nào!`,
                    back: (step) => `Quay lại bước ${step} nè!`
                }
            };

            let displayMessage = message;
            if (type === 'success') {
                if (message.startsWith('Đã thêm dịch vụ')) displayMessage = friendlyMessages.success.addService(message.split(': ')[1]);
                else if (message.startsWith('Đã bỏ dịch vụ')) displayMessage = friendlyMessages.success.removeService(message.split(': ')[1]);
                else if (message.startsWith('Đã thêm thú cưng')) displayMessage = friendlyMessages.success.addPet(message.split(': ')[1]);
                else if (message === 'Đặt lịch thành công!') displayMessage = friendlyMessages.success.confirm;
            } else if (type === 'error') {
                if (message === 'Vui lòng chọn ít nhất một dịch vụ.') displayMessage = friendlyMessages.error.noService;
                else if (message === 'Vui lòng nhập đầy đủ tên và giống loài.') displayMessage = friendlyMessages.error.addPet;
                else if (message === 'Đã có lỗi xảy ra, vui lòng thử lại.') displayMessage = friendlyMessages.error.error;
                else displayMessage = friendlyMessages.error.invalidInput(message);
            } else if (type === 'info') {
                if (message.startsWith('Chuyển sang bước')) displayMessage = friendlyMessages.info.step(message.split(' ')[3]);
                else if (message.startsWith('Quay lại bước')) displayMessage = friendlyMessages.info.back(message.split(' ')[3]);
            }

            toastQueue.push({ message: displayMessage, type });
            if (toastQueue.length > 1) return;

            function displayNextToast() {
                if (toastQueue.length === 0) return;
                const { message, type } = toastQueue[0];

                const toastMessage = toastEl.querySelector('.toast-message');
                if (toastMessage) toastMessage.textContent = message;

                const toastIcon = toastEl.querySelector('.toast-icon');
                if (toastIcon) {
                    toastIcon.className = 'toast-icon';
                    toastIcon.classList.add(type);
                }

                toastEl.className = 'toast custom-toast';
                toastEl.classList.add(type);

                const toast = new bootstrap.Toast(toastEl);
                toastEl.addEventListener('hidden.bs.toast', () => {
                    toastQueue.shift();
                    displayNextToast();
                }, { once: true });
                toast.show();
            }

            displayNextToast();
        }

        // === Error Handling ===
        function showFieldError(field, message) {
            if (!field) return;
            field.classList.add('error');
            field.setAttribute('data-bs-toggle', 'tooltip');
            field.setAttribute('data-bs-title', message);
            new bootstrap.Tooltip(field).show();
            field.focus();
            field.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }

        function clearFieldError(field) {
            if (!field) return;
            field.classList.remove('error');
            const tooltip = bootstrap.Tooltip.getInstance(field);
            if (tooltip) tooltip.dispose();
        }

        function clearAllErrors() {
            document.querySelectorAll('input.error, select.error').forEach(input => {
                input.classList.remove('error');
                const tooltip = bootstrap.Tooltip.getInstance(input);
                if (tooltip) tooltip.dispose();
            });
        }

        // === Validation ===
        function validateStep2() {
            const errors = [];
            const nameInput = document.querySelector('.customer-form input[type="text"]');
            if (!nameInput || !nameInput.value.trim()) {
                errors.push({ message: 'Vui lòng nhập họ tên khách hàng.', field: nameInput });
            }

            const phoneInput = document.querySelector('.customer-form input[type="tel"]');
            if (!phoneInput || !phoneInput.value.trim()) {
                errors.push({ message: 'Vui lòng nhập số điện thoại.', field: phoneInput });
            } else if (!/^[0-9]{9,11}$/.test(phoneInput.value.trim())) {
                errors.push({ message: 'Số điện thoại không hợp lệ.', field: phoneInput });
            }

            const emailInput = document.querySelector('.customer-form input[type="email"]');
            if (!emailInput || !emailInput.value.trim()) {
                errors.push({ message: 'Vui lòng nhập email.', field: emailInput });
            } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(emailInput.value.trim())) {
                errors.push({ message: 'Email không hợp lệ.', field: emailInput });
            }

            const dateInput = document.querySelector('.customer-form input[type="date"]');
            if (!dateInput || !dateInput.value) {
                errors.push({ message: 'Vui lòng chọn ngày hẹn.', field: dateInput });
            } else {
                const selectedDate = new Date(dateInput.value);
                const today = new Date();
                today.setHours(0, 0, 0, 0);
                if (selectedDate < today) {
                    errors.push({ message: 'Ngày hẹn không được là ngày trong quá khứ.', field: dateInput });
                }
            }

            const timeInput = document.querySelector('.customer-form input[type="time"]');
            if (!timeInput || !timeInput.value) {
                errors.push({ message: 'Vui lòng chọn giờ hẹn.', field: timeInput });
            } else if (dateInput && dateInput.value) {
                const selectedDate = new Date(dateInput.value);
                const today = new Date();
                today.setHours(0, 0, 0, 0);
                if (selectedDate.getTime() === today.getTime()) {
                    const [hours, minutes] = timeInput.value.split(':').map(Number);
                    const currentHours = new Date().getHours();
                    const currentMinutes = new Date().getMinutes();
                    if (hours < currentHours || (hours === currentHours && minutes < currentMinutes)) {
                        errors.push({ message: 'Giờ hẹn không được là giờ trong quá khứ.', field: timeInput });
                    }
                }
            }

            const selectedPets = document.querySelectorAll('.pet-checkbox:checked');
            if (!selectedPets || selectedPets.length === 0) {
                errors.push({ message: 'Vui lòng chọn ít nhất một thú cưng.', field: null });
            }

            return errors.length ? { valid: false, errors } : { valid: true };
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
                        return { name, breed };
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
        }

        // === Event Listeners ===
        // Service Selection
        document.addEventListener('click', e => {
            const btn = e.target.closest('.btn-select');
            if (!btn) return;

            const { service: serviceId, name: serviceName, price: servicePrice } = btn.dataset;
            const price = parseFloat(servicePrice);
            const index = selectedServices.findIndex(sv => sv.serviceId === serviceId);

            if (index === -1) {
                selectedServices.push({ serviceId, name: serviceName, price });
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

        // Step Navigation: Next
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
                        validation.errors.forEach(({ message, field }, index) => {
                            if (field) {
                                showFieldError(field, message);
                                if (index === 0) {
                                    field.focus();
                                    field.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                }
                            }
                            showToast(friendlyMessages.error.invalidInput(message), 'error');
                        });
                        return;
                    }
                }

                if (currentStep === 3) {
                    updateConfirmationStep();
                    return;
                }

                const currentStepEl = document.querySelector('.booking-step.active');
                if (currentStepEl) {
                    currentStepEl.style.display = 'none';
                    currentStepEl.classList.remove('active');
                }

                currentStep++;
                const nextStep = document.querySelector(`.booking-step[data-step="${currentStep}"]`);
                if (nextStep) {
                    nextStep.style.display = 'block';
                    nextStep.classList.add('active');
                    document.querySelectorAll('.step').forEach(stepEl => {
                        stepEl.classList.toggle('active', parseInt(stepEl.dataset.step) === currentStep);
                    });
                    document.querySelector('.btn-prev').disabled = currentStep === 1;
                    showToast(`Bước ${currentStep} đây! Tiếp tục nào!`, 'info');
                }
            });
        }

        // Step Navigation: Previous
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
                const prevStep = document.querySelector(`.booking-step[data-step="${currentStep}"]`);
                if (prevStep) {
                    prevStep.style.display = 'block';
                    prevStep.classList.add('active');
                    document.querySelectorAll('.step').forEach(stepEl => {
                        stepEl.classList.toggle('active', parseInt(stepEl.dataset.step) === currentStep);
                    });
                    btnPrev.disabled = currentStep === 1;
                    showToast(`Quay lại bước ${currentStep} nè!`, 'info');
                }
            });
        }

        // Service Filtering
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

        // Add Pet
        const addPetForm = document.getElementById('addPetForm');
        if (addPetForm) {
            addPetForm.addEventListener('submit', e => {
                e.preventDefault();

                const petName = document.getElementById('petName')?.value.trim();
                const petBreed = document.getElementById('petBreed')?.value;

                if (!petName || !petBreed) {
                    showToast('Vui lòng nhập đầy đủ tên và giống loài.', 'error');
                    return;
                }

                const tempPetId = Date.now();
                const petHtml = `
                    <div class="pet-item">
                        <input type="checkbox" id="pet-${tempPetId}" name="pets[]" value="${tempPetId}" class="pet-checkbox" checked>
                        <label for="pet-${tempPetId}" class="pet-card">
                            <div class="pet-avatar"><i class="fa fa-paw"></i></div>
                            <div class="pet-info">
                                <h5>${petName}</h5>
                                <span class="pet-breed">${petBreed}</span>
                            </div>
                        </label>
                    </div>
                `;

                const petList = document.querySelector('.pet-list');
                if (petList) petList.insertAdjacentHTML('beforeend', petHtml);

                addPetForm.reset();

                const addPetModalEl = document.getElementById('addPetModal');
                if (addPetModalEl) {
                    const addPetModal = new bootstrap.Modal(addPetModalEl);
                    addPetModal.hide();
                    showToast(`Đã thêm thú cưng: ${petName}`, 'success');
                }
            });
        }

        // Confirm Appointment
        const btnConfirmAppointment = document.getElementById('btnConfirmAppointment');
        if (btnConfirmAppointment) {
            btnConfirmAppointment.addEventListener('click', () => {
                const successModalEl = document.getElementById('successModal');
                if (successModalEl) {
                    const successModal = new bootstrap.Modal(successModalEl);
                    successModal.show();
                    showToast('Đặt lịch thành công!', 'success');
                }
            });
        }

        // Modal Buttons
        const btnGoHome = document.getElementById('btnGoHome');
        if (btnGoHome) {
            btnGoHome.addEventListener('click', () => {
                window.location.href = '/';
            }, { once: true });
        }

        const btnNewBooking = document.getElementById('btnNewBooking');
        if (btnNewBooking) {
            btnNewBooking.addEventListener('click', () => {
                window.location.reload();
            }, { once: true });
        }

        // Input Event Listeners
        document.querySelectorAll('.customer-form input, .customer-form textarea')
            .forEach(input => input.addEventListener('input', () => clearFieldError(input)));

    } catch (error) {
        console.error('Lỗi trong appointment.js:', error);
        showToast('Đã có lỗi xảy ra, vui lòng thử lại.', 'error');
    }
});