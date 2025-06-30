document.addEventListener('DOMContentLoaded', () => {

    // === State Variables ===
    let currentStep = 1;
    let selectedServices = [];
    const mainForm = document.querySelector('.booking-container form');

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

    // ---- Form Validation Helpers ----
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

    // --- Quản lý required theo step ---
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

    // Khởi tạo required ban đầu
    updateRequiredAttributes(currentStep); 

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

        // Kiểm tra thú cưng đã chọn bằng hàm chung
        if (!checkPetSelection(false)) {
            errors.push({message: 'Vui lòng chọn ít nhất một thú cưng.', field: null});
            if (!firstErrorField) firstErrorField = document.querySelector('.pet-list');
            
            // Log debug thêm
            const allPets = document.querySelectorAll('input[name="SelectedPetIds"]');
            console.log(`DEBUG: Tổng số thú cưng: ${allPets.length}`);
            allPets.forEach((pet, i) => {
                console.log(`  Pet #${i+1}: ID=${pet.id}, value=${pet.value}, checked=${pet.checked}`);
            });
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

    // === Kiểm tra thú cưng được chọn ===
    function checkPetSelection(showError = true) {
        // Using more specific selector to ensure we're getting all checked pet checkboxes
        const selectedPets = document.querySelectorAll('.pet-item input[name="SelectedPetIds"]:checked');
        const count = selectedPets.length;
        
        // Enhanced logging for debugging
        console.log(`Đang kiểm tra thú cưng: Đã chọn ${count} thú cưng`);
        console.log("Danh sách thú cưng đã chọn:", Array.from(selectedPets).map(pet => pet.id));
        
        // Check for nodes that exist but might have issues
        const allPetCheckboxes = document.querySelectorAll('.pet-item input[name="SelectedPetIds"]');
        console.log(`Tổng số checkbox thú cưng: ${allPetCheckboxes.length}`);
        
        if (count === 0) {
            if (showError) {
                showToast("Vui lòng chọn ít nhất một thú cưng", "error");
            }
            return false;
        }
        
        return count > 0;
    }
    
    // === Tạo input hidden cho dịch vụ ===
    // Được tích hợp trực tiếp vào nút Next và Confirm thay vì dùng function riêng
    // Function này được giữ lại để tham khảo và khả năng tương thích ngược
    function updateServiceInputs() {
        console.log("Bắt đầu cập nhật input hidden cho dịch vụ...");
        
        // === DỊCH VỤ ===
        // Xóa input hidden dịch vụ cũ để tránh trùng lặp
        mainForm.querySelectorAll('input[name="SelectedServiceIds"][type="hidden"]').forEach(e => e.remove());
        
        // Kiểm tra xem có dịch vụ nào được chọn không
        if (selectedServices.length === 0) {
            console.log("Không có dịch vụ nào được chọn!");
            showToast("Vui lòng chọn ít nhất một dịch vụ", "error");
            return "no-service"; // Trả về mã lỗi cụ thể thay vì false
        }
        
        // Thêm input hidden cho các dịch vụ đã chọn
        selectedServices.forEach(sv => {
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'SelectedServiceIds';
            input.value = sv.serviceId;
            mainForm.appendChild(input);
            console.log(`Đã thêm service: ${sv.name} (ID: ${sv.serviceId})`);
        });
        
        console.log("Hoàn tất cập nhật input hidden!");
        return true;
    }


    // === Confirmation Step ===
    function updateConfirmationStep() {
        const confirmServiceEl = document.getElementById('confirm-service');
        if (confirmServiceEl) confirmServiceEl.textContent = selectedServices.map(sv => sv.name).join(', ');

        const confirmPetsList = document.getElementById('confirm-pets-list');
        if (confirmPetsList) {
            console.log("Cập nhật danh sách pet ở bước xác nhận");
            
            // Sử dụng selector cụ thể hơn để đảm bảo lấy được đúng các checkbox
            // Selector: .pet-item input[name="SelectedPetIds"]:checked
            const petCheckboxes = document.querySelectorAll('.pet-item input[name="SelectedPetIds"]:checked');
            
            // Debug counts với nhiều selector khác nhau để tìm ra vấn đề
            console.log(`DEBUG: Số pet với 'input[name="SelectedPetIds"]:checked': ${document.querySelectorAll('input[name="SelectedPetIds"]:checked').length}`);
            console.log(`DEBUG: Số pet với '.pet-checkbox:checked': ${document.querySelectorAll('.pet-checkbox:checked').length}`);
            console.log(`DEBUG: Số pet với '.pet-item input:checked': ${document.querySelectorAll('.pet-item input:checked').length}`);
            console.log(`DEBUG: Số pet với '.pet-item input[name="SelectedPetIds"]:checked': ${petCheckboxes.length}`);
            
            // In chi tiết từng checkbox để debug
            document.querySelectorAll('.pet-item input[name="SelectedPetIds"]').forEach((cb, idx) => {
                console.log(`Pet checkbox ${idx+1}: id=${cb.id}, checked=${cb.checked}, value=${cb.value}`);
            });
            
            // Chuyển NodeList thành array để có thể dùng map
            const selectedPets = Array.from(petCheckboxes).map(petCheckbox => {
                const label = document.querySelector(`label[for="${petCheckbox.id}"]`);
                // Nếu không tìm thấy label với for=id thì thử nhận label là phần tử kế tiếp
                let name = '', breed = '';
                
                if (label) {
                    name = label.querySelector('h5')?.innerText.trim() || '';
                    breed = label.querySelector('.pet-breed')?.innerText.trim() || '';
                } else {
                    // Tìm trong parent của checkbox để handle TH không có label
                    const petItem = petCheckbox.closest('.pet-item');
                    name = petItem?.querySelector('h5')?.innerText.trim() || '';
                    breed = petItem?.querySelector('.pet-breed')?.innerText.trim() || '';
                }
                
                console.log(`Tìm được pet: ${name} (${breed}) từ checkbox ${petCheckbox.id}`);
                return name ? {name, breed} : null;
            }).filter(Boolean); // Lọc các giá trị null

            console.log(`Tổng số pet sau khi xử lý: ${selectedPets.length}`);
            
            confirmPetsList.innerHTML = '';
            selectedPets.forEach(pet => {
                const li = document.createElement('li');
                li.textContent = `${pet.name} (${pet.breed})`;
                confirmPetsList.appendChild(li);
            });
            
            // Nếu không tìm thấy pet nào, hiển thị thông báo
            if (selectedPets.length === 0) {
                const li = document.createElement('li');
                li.className = 'list-group-item text-danger';
                li.innerHTML = '<i class="fa fa-exclamation-triangle"></i> Chưa có thú cưng nào được chọn!';
                confirmPetsList.appendChild(li);
            }
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

    // === Scroll về đầu form ===
    function scrollToFormTop() {
        const formSection = document.querySelector('.appoint_ment_form');
        if (formSection) {
            formSection.scrollIntoView({behavior: 'smooth', block: 'start'});
        } else {
            window.scrollTo({top: 0, behavior: 'smooth'});
        }
    }

    // ---- Theo dõi thay đổi trên checkbox thú cưng ----
    document.addEventListener('change', function(e) {
        if (e.target.matches('input[name="SelectedPetIds"]')) {
            console.log(`Pet checkbox ${e.target.id} changed: checked=${e.target.checked}, value=${e.target.value}`);
            
            // Khi có thay đổi checkbox, cập nhật lại lỗi nếu có
            const petErrorDiv = document.querySelector('.pet-selection .pet-error');
            if (petErrorDiv && document.querySelectorAll('input[name="SelectedPetIds"]:checked').length > 0) {
                petErrorDiv.textContent = '';
                petErrorDiv.style.display = 'none';
            }
        }
    });

    // === EVENT LISTENERS ===

    // ---- Ngăn submit form nếu chưa phải bước 3 ----
    if (mainForm) {
        mainForm.addEventListener('submit', function (e) {
            console.log(`Form submit được kích hoạt (bước ${currentStep})`);
            
            if (currentStep !== 3) {
                console.log("Chặn submit form vì chưa phải ở bước 3");
                e.preventDefault();
                return false;
            }
            
            // Kiểm tra xem có pets được chọn không
            const selectedPetsCount = document.querySelectorAll('input[name="SelectedPetIds"]:checked').length;
            console.log(`Kiểm tra trước khi submit: Đã chọn ${selectedPetsCount} thú cưng`);
            // Kiểm tra xem có pets được chọn không với hàm chung
            if (!checkPetSelection(true)) {
                console.log("Chặn submit form vì chưa chọn thú cưng");
                e.preventDefault();
                moveToStep(2);
                return false;
            }
            return true;
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
            // STEP 1: Check services selection
            if (currentStep === 1 && selectedServices.length === 0) {
                showToast('Oops! Bạn chưa chọn dịch vụ nào, chọn một cái nha!', 'error');
                return;
            }
            
            // STEP 2: Validate customer info and pets
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
                
                // Double-check pet selection explicitly here - critical for step 3
                const selectedPetsCount = document.querySelectorAll('.pet-item input[name="SelectedPetIds"]:checked').length;
                console.log(`Double-check trước khi sang bước 3: Có ${selectedPetsCount} thú cưng đã chọn`);
                
                if (selectedPetsCount === 0) {
                    showToast('Vui lòng chọn ít nhất một thú cưng', 'error');
                    let petErrorDiv = document.querySelector('.pet-selection .pet-error');
                    if (petErrorDiv) {
                        petErrorDiv.textContent = 'Vui lòng chọn ít nhất một thú cưng.';
                        petErrorDiv.style.display = 'block';
                        petErrorDiv.scrollIntoView({behavior: 'smooth', block: 'center'});
                    }
                    return;
                }
                
                // Chuẩn bị input hidden cho dịch vụ
                mainForm.querySelectorAll('input[name="SelectedServiceIds"][type="hidden"]').forEach(e => e.remove());
                selectedServices.forEach(sv => {
                    const input = document.createElement('input');
                    input.type = 'hidden';
                    input.name = 'SelectedServiceIds';
                    input.value = sv.serviceId;
                    mainForm.appendChild(input);
                });
                
                // Cập nhật giao diện bước 3
                updateConfirmationStep();
            }
            
            // STEP 3: Just update confirmation
            if (currentStep === 3) {
                // Khi ở bước 3, chỉ cập nhật thông tin xác nhận, không cần validate lại
                updateConfirmationStep();
                return;
            }
            
            // Common navigation logic
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
        btnAddPet.addEventListener('click', function () {
            const petName = document.getElementById('petNameModal').value.trim();
            const petBreed = document.getElementById('petBreedModal').value;
            if (!petName || !petBreed) {
                showToast('Vui lòng nhập đầy đủ tên và giống loài.', 'error');
                return;
            }
            const tempPetId = "new_" + Date.now();
            const petList = document.querySelector('.pet-selection .pet-list');

            if (petList) {
                // Tạo element thay vì chỉ insertAdjacentHTML để có thể tương tác với checkbox
                const petItemDiv = document.createElement('div');
                petItemDiv.className = 'pet-item';
                petItemDiv.innerHTML = `
                    <input type="checkbox" id="pet-${tempPetId}" name="SelectedPetIds" value="${tempPetId}" class="pet-checkbox" checked>
                    <label for="pet-${tempPetId}" class="pet-card">
                        <div class="pet-avatar"><i class="fa fa-paw"></i></div>
                        <div class="pet-info">
                            <h5>${petName}</h5>
                            <span class="pet-breed">${petBreed}</span>
                        </div>
                    </label>
                `;
                
                petList.appendChild(petItemDiv);
                
                // Kiểm tra xem checkbox đã được thêm đúng và đã được check
                const newCheckbox = document.getElementById(`pet-${tempPetId}`);
                if (newCheckbox) {
                    console.log(`Đã thêm pet mới: ${petName} với ID=${newCheckbox.id}, checked=${newCheckbox.checked}`);
                    
                    // Đảm bảo checkbox được check
                    newCheckbox.checked = true;
                }
                
                // Xóa thông báo cảnh báo nếu có
                const warningAlert = petList.querySelector('.alert-warning');
                if (warningAlert) {
                    warningAlert.remove();
                }
                
                // Xóa lỗi thú cưng nếu có
                const petError = document.querySelector('.pet-selection .pet-error');
                if (petError) {
                    petError.textContent = '';
                    petError.style.display = 'none';
                }
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

    // Confirm Appointment
    const btnConfirmAppointment = document.getElementById('btnConfirmAppointment');
    if (btnConfirmAppointment) {
        btnConfirmAppointment.addEventListener('click', function (e) {
            e.preventDefault();
            console.log("BƯỚC 3: Nút xác nhận đặt lịch được nhấn");
            
            // Cập nhật form confirmation và kiểm tra dữ liệu
            updateConfirmationStep();
            
            // Debug hiện trạng các checkbox thú cưng với selector cụ thể
            console.log("Trạng thái checkbox thú cưng:");
            document.querySelectorAll('.pet-item input[name="SelectedPetIds"]').forEach((cb, i) => {
                console.log(`  ${i+1}. ID=${cb.id}, value=${cb.value}, checked=${cb.checked}`);
            });
            
            // Kiểm tra dịch vụ đã chọn 
            if (selectedServices.length === 0) {
                showToast("Vui lòng chọn ít nhất một dịch vụ", "error");
                moveToStep(1);
                return;
            }
            
            // Kiểm tra thú cưng đã chọn với selector cụ thể
            const selectedPetsCount = document.querySelectorAll('.pet-item input[name="SelectedPetIds"]:checked').length;
            if (selectedPetsCount === 0) {
                console.log("Không tìm thấy thú cưng nào được chọn (với selector .pet-item), quay lại bước 2");
                showToast("Vui lòng chọn ít nhất một thú cưng", "error");
                moveToStep(2);
                return;
            }
            
            // Log thông tin pet đã chọn để debug
            const selectedPets = document.querySelectorAll('.pet-item input[name="SelectedPetIds"]:checked');
            console.log(`Tìm thấy ${selectedPets.length} thú cưng được chọn (với selector .pet-item)`);
            selectedPets.forEach((pet, i) => {
                console.log(`  Pet #${i+1}: ID=${pet.id}, value=${pet.value}`);
            });
            
            // QUAN TRỌNG: Xóa hidden inputs cũ để tránh duplicate
            mainForm.querySelectorAll('input[name="SelectedServiceIds"][type="hidden"]').forEach(e => e.remove());
            
            // Thêm service IDs mới
            selectedServices.forEach(sv => {
                const input = document.createElement('input');
                input.type = 'hidden';
                input.name = 'SelectedServiceIds';
                input.value = sv.serviceId;
                mainForm.appendChild(input);
                console.log(`Thêm service: ${sv.name} (ID: ${sv.serviceId})`);
            });
            
            // Ensure all selected pets are properly checked before submission
            // This ensures the form will include the selected pet IDs
            selectedPets.forEach(pet => {
                if (!pet.checked) {
                    console.log(`Fixing unchecked pet: ${pet.id}`);
                    pet.checked = true;
                }
                console.log(`Đã chọn Pet ID: ${pet.value}`);
            });
                        
            // Hiển thị thông báo đang xử lý
            showToast("Đang xử lý đặt lịch...", "info");
            
            // Thêm delay nhỏ để người dùng thấy thông báo 
            setTimeout(() => {
                console.log("Đang submit form...");
                mainForm.submit();
            }, 500);
        });
    }

    function moveToStep(stepNumber) {
        // Ẩn step hiện tại
        const currentStepEl = document.querySelector('.booking-step.active');
        if (currentStepEl) {
            currentStepEl.style.display = 'none';
            currentStepEl.classList.remove('active');
        }
        
        // Cập nhật biến theo dõi bước hiện tại
        currentStep = stepNumber;
        updateRequiredAttributes(currentStep);
        
        // Hiển thị step mới
        const nextStep = document.querySelector(`.booking-step[data-step="${currentStep}"]`);
        if (nextStep) {
            nextStep.style.display = 'block';
            nextStep.classList.add('active');
            document.querySelectorAll('.step').forEach(stepEl => {
                stepEl.classList.toggle('active', parseInt(stepEl.dataset.step) === currentStep);
            });
            document.querySelector('.btn-prev').disabled = currentStep === 1;
            
            let stepName = "";
            if (currentStep === 1) stepName = "chọn dịch vụ";
            else if (currentStep === 2) stepName = "thông tin và thú cưng";
            else if (currentStep === 3) stepName = "xác nhận đặt lịch";
            
            showToast(`Quay lại bước ${currentStep}: ${stepName}`, 'info');
            scrollToFormTop();
        }
    }
});








