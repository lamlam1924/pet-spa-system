/**
 * appointment-history-grouped.js - Logic xử lý cho trang lịch sử đặt lịch với nhóm thời gian
 */

// ===================== JS Lịch sử đặt lịch (Appointment History) =====================

document.addEventListener('DOMContentLoaded', function () {
    // ===== Sự kiện gửi yêu cầu hủy lịch =====
    document.addEventListener('click', function (e) {
        if (e.target.classList.contains('btn-cancel-request') || e.target.closest('.btn-cancel-request')) {
            const button = e.target.classList.contains('btn-cancel-request') ? e.target : e.target.closest('.btn-cancel-request');

            const appointmentId = button.getAttribute('data-id');
            if (!appointmentId) {
                showToast('Không lấy được mã lịch hẹn!', 'error');
                return;
            }
            if (!confirm('Bạn chắc chắn muốn gửi yêu cầu hủy lịch này?')) return;

            button.disabled = true;
            button.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Đang gửi...';

            fetch('/Appointment/RequestCancel', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                },
                body: JSON.stringify({ appointmentId: appointmentId })
            })
                .then(res => res.json())
                .then(data => {
                    if (data.success) {
                        showToast(data.message, 'success');
                        // Cập nhật UI: đổi badge trạng thái, disable nút
                        button.classList.add('disabled');
                        button.innerHTML = '<i class="fas fa-clock"></i> Đã gửi yêu cầu hủy';
                        // Đổi badge trạng thái nếu có
                        const card = button.closest('.appointment-card');
                        if (card) {
                            const statusBadge = card.querySelector('.appointment-status');
                            if (statusBadge) {
                                statusBadge.textContent = 'Chờ duyệt hủy';
                                statusBadge.className = 'appointment-status status-pendingcancel';
                            }
                        }
                    } else {
                        showToast(data.message || 'Gửi yêu cầu thất bại', 'error');
                        button.disabled = false;
                        button.innerHTML = '<i class="fas fa-times-circle"></i> Yêu cầu hủy lịch';
                    }
                })
                .catch(err => {
                    showToast('Có lỗi xảy ra, vui lòng thử lại!', 'error');
                    button.disabled = false;
                    button.innerHTML = '<i class="fas fa-times-circle"></i> Yêu cầu hủy lịch';
                });
        }
    });

    // ===== Khởi tạo biến và dữ liệu =====
    const timelineContainer = document.querySelector('.history-timeline');
    const searchInput = document.querySelector('.search-box input');
    const sortSelect = document.querySelector('.sort-select');
    const statusFilters = document.querySelectorAll('.filter-badge');
    let appointmentData = []; // Dữ liệu lịch hẹn từ server
    let currentPage = 1;
    const itemsPerPage = 6;

    // Khởi tạo dữ liệu (thay thế bằng dữ liệu thực từ server)
    fetchAppointmentData();

    // ===== Xử lý sự kiện tìm kiếm, sắp xếp, lọc =====
    // Xử lý sự kiện tìm kiếm
    if (searchInput) {
        searchInput.addEventListener('input', function () {
            currentPage = 1;
            renderAppointments();
        });
    }

    // Xử lý sự kiện sắp xếp
    if (sortSelect) {
        sortSelect.addEventListener('change', function () {
            currentPage = 1;
            renderAppointments();
        });
    }

    // Xử lý sự kiện lọc theo trạng thái
    if (statusFilters) {
        statusFilters.forEach(filter => {
            filter.addEventListener('click', function () {
                // Bỏ active của tất cả các filter khác
                statusFilters.forEach(f => f.classList.remove('active'));
                // Thêm active cho filter hiện tại
                this.classList.add('active');
                currentPage = 1;
                renderAppointments();
            });
        });
    }

    // ===== Lấy dữ liệu lịch hẹn từ server =====
    // Sử dụng dữ liệu từ window.appointmentData được thiết lập trong view
    function fetchAppointmentData() {
        // Sử dụng dữ liệu từ server đã render vào trang
        setTimeout(() => {
            if (window.appointmentData) {
                appointmentData = window.appointmentData;
            } else {
                appointmentData = [];
            }
            renderAppointments();
        }, 300);
    }

    // ===== Hiển thị các lịch hẹn theo nhóm thời gian =====
    function renderAppointments() {
        if (!timelineContainer) {
            return;
        }

        // Lấy trạng thái đang active
        const activeFilterElement = document.querySelector('.filter-badge.active');
        const activeFilter = activeFilterElement ? activeFilterElement.getAttribute('data-status') : 'all';

        // Lấy từ khóa tìm kiếm
        const searchTerm = searchInput ? searchInput.value.toLowerCase().trim() : '';

        // Lấy kiểu sắp xếp
        const sortBy = sortSelect ? sortSelect.value : 'newest';

        // Lọc dữ liệu
        let filteredData = appointmentData.filter(appointment => {
            // Lọc theo trạng thái nếu có
            if (activeFilter !== 'all' && appointment.statusId.toString() !== activeFilter) {
                return false;
            }

            // Tìm kiếm
            if (searchTerm) {
                // Tạo chuỗi tìm kiếm từ các dữ liệu của lịch hẹn
                const serviceNames = appointment.services ? appointment.services.map(s => s.name).join(' ') : '';
                const petNames = appointment.petNames ? appointment.petNames.join(' ') : '';
                const notes = appointment.notes || '';
                const statusName = appointment.statusName || '';
                const date = new Date(appointment.appointmentDate).toLocaleDateString();

                const searchIn = `${serviceNames} ${petNames} ${notes} ${statusName} ${date}`.toLowerCase();

                if (!searchIn.includes(searchTerm)) {
                    return false;
                }
            }

            return true;
        });

        // Sắp xếp dữ liệu
        filteredData.sort((a, b) => {
            if (sortBy === 'oldest') {
                return new Date(a.appointmentDate) - new Date(b.appointmentDate);
            } else if (sortBy === 'newest') {
                return new Date(b.appointmentDate) - new Date(a.appointmentDate);
            } else if (sortBy === 'status') {
                return a.statusId - b.statusId;
            }
            return 0;
        });

        // Nhóm lịch hẹn theo tháng/năm
        const groupedAppointments = groupAppointmentsByMonth(filteredData);

        // Hiển thị kết quả
        if (filteredData.length === 0) {
            timelineContainer.innerHTML = generateNoResultsHTML();
        } else {
            // Tạo HTML cho các nhóm thời gian
            let html = '';
            for (const [month, appointments] of Object.entries(groupedAppointments)) {
                html += generateTimeGroupHTML(month, appointments);
            }
            timelineContainer.innerHTML = html;

            // Khởi tạo sự kiện thu gọn/mở rộng nhóm
            initCollapseEvents();

            // Animation
            animateCards();
        }
    }

    // ===== Nhóm lịch hẹn theo tháng/năm =====
    // Nhóm lịch hẹn theo tháng/năm
    function groupAppointmentsByMonth(appointments) {
        const groups = {};

        appointments.forEach(appointment => {
            const date = new Date(appointment.appointmentDate);
            const monthYear = `${getMonthName(date.getMonth())} ${date.getFullYear()}`;

            if (!groups[monthYear]) {
                groups[monthYear] = [];
            }

            groups[monthYear].push(appointment);
        });

        return groups;
    }

    // ===== Tạo HTML cho nhóm thời gian =====
    // Tạo HTML cho nhóm thời gian
    function generateTimeGroupHTML(monthYear, appointments) {
        let html = `
        <div class="time-group">
            <div class="time-group-header">
                <span class="time-group-title">${monthYear}</span>
                <div class="time-group-collapse" data-month="${monthYear}">
                    <i class="fas fa-chevron-down"></i>
                </div>
            </div>
            <div class="time-group-content" data-month="${monthYear}">
        `;

        appointments.forEach((appointment, index) => {
            const position = index % 2 === 0 ? 'left' : 'right';
            const animateClass = position === 'left' ? 'animate-left' : 'animate-right';
            const allowCancel = [1, 2].includes(appointment.statusId) && !appointment.isPendingCancel;
            let cancelBadge = '';
            if (appointment.isPendingCancel) {
                cancelBadge = '<span class="badge bg-warning text-dark ms-2">Chờ duyệt hủy</span>';
            }
            html += `
            <div class="appointment-card ${position} status-${appointment.statusId} ${animateClass}" data-status="${appointment.statusId}" data-appointment-id="${appointment.appointmentId}">
    <div class="appointment-content">
        <span class="appointment-status ${getStatusClass(appointment.statusId)}">${appointment.statusName}</span>
        ${cancelBadge}
        <div class="appointment-date">
            <span class="appointment-date-badge">
                <i class="far fa-calendar-alt"></i> ${formatDate(appointment.appointmentDate)}
            </span>
        </div>
        <div class="appointment-services">
            <h5>Dịch vụ</h5>
            ${appointment.services.map(service =>
                `<span class="service-tag">${service.name}</span>`
            ).join('')}
        </div>
        <div class="appointment-pets">
            <h5>Thú cưng</h5>
            ${appointment.petNames.map(pet => `<span class="pet-tag">${pet}</span>`).join('')}
        </div>
        ${appointment.notes ? `<div class="appointment-notes">${appointment.notes}</div>` : ''}
        <div class="appointment-actions">
           

            ${appointment.statusId === 4 ?
                    `<button class="btn-review" data-id="${appointment.appointmentId}">
                    <i class="fas fa-star"></i> Đánh giá
                </button>` : ''}
                
            ${allowCancel ?
                    `<button class="btn btn-outline-danger btn-cancel-request" data-id="${appointment.appointmentId}">
                    <i class="fas fa-times-circle"></i> Yêu cầu hủy lịch
                </button>` : ''}
        </div>
    </div>
</div>

            `;
        });

        html += `
            </div>
        </div>`;

        return html;
    }

    // ===== Khởi tạo sự kiện thu gọn/mở rộng nhóm =====
    // Khởi tạo sự kiện thu gọn/mở rộng nhóm
    function initCollapseEvents() {
        const collapseButtons = document.querySelectorAll('.time-group-collapse');

        collapseButtons.forEach(button => {
            button.addEventListener('click', function () {
                const monthYear = this.getAttribute('data-month');
                const content = document.querySelector(`.time-group-content[data-month="${monthYear}"]`);

                this.classList.toggle('collapsed');
                content.classList.toggle('collapsed');
            });
        });
    }

    // ===== Animation cho các card =====
    // Animation cho các card
    function animateCards() {
        const cards = document.querySelectorAll('.appointment-card');
        cards.forEach((card, index) => {
            setTimeout(() => {
                card.style.opacity = 1;
            }, index * 100);
        });
    }

    // ===== Hàm hỗ trợ =====
    // Hàm hỗ trợ
    function getMonthName(monthIndex) {
        const months = ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
            'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'];
        return months[monthIndex];
    }

    function formatDate(dateStr) {
        const date = new Date(dateStr);
        return `${date.getDate()}/${date.getMonth() + 1}/${date.getFullYear()}`;
    }

    function getStatusClass(status) {
        switch (status) {
            case 1: return 'status-pending'; // Chờ xác nhận
            case 2: return 'status-confirmed'; // Đã xác nhận
            case 3: return 'status-inprogress'; // Đang thực hiện
            case 4: return 'status-completed'; // Hoàn thành
            case 5: return 'status-cancelled'; // Đã hủy
            case 6: return 'status-pendingcancel'; // Chờ duyệt hủy
            default: return '';
        }
    }

    function generateNoResultsHTML() {
        return `
        <div class="no-results">
            <i class="fas fa-search"></i>
            <h4>Không tìm thấy kết quả</h4>
            <p>Vui lòng thử lại với các bộ lọc khác hoặc từ khóa khác.</p>
        </div>`;
    }

    // ===== Khởi tạo Modal Review =====
    // Khởi tạo Modal Review
    initReviewModal();

    function initReviewModal() {
        // Lắng nghe sự kiện click nút đánh giá
        document.addEventListener('click', function (e) {
            if (e.target.classList.contains('btn-review') || e.target.closest('.btn-review')) {
                const button = e.target.classList.contains('btn-review') ? e.target : e.target.closest('.btn-review');
                const appointmentId = button.getAttribute('data-id');

                // Hiển thị modal và lưu ID lịch hẹn
                const modal = document.getElementById('reviewModal');
                if (modal) {
                    modal.setAttribute('data-appointment-id', appointmentId);
                    // Hiển thị modal bằng Bootstrap
                    new bootstrap.Modal(modal).show();
                }
            }
        });

        // Xử lý sự kiện gửi đánh giá
        const reviewForm = document.getElementById('reviewForm');
        if (reviewForm) {
            reviewForm.addEventListener('submit', function (e) {
                e.preventDefault();

                const modal = document.getElementById('reviewModal');
                const appointmentId = modal.getAttribute('data-appointment-id');
                const rating = document.querySelector('input[name="rating"]:checked')?.value || 5;
                const comment = document.getElementById('reviewComment')?.value || '';

                // Gửi đánh giá đến server (thay thế bằng API call thực tế)
                submitReview(appointmentId, rating, comment);

                // Đóng modal
                bootstrap.Modal.getInstance(modal).hide();

                // Hiển thị thông báo thành công
                showToast('Cảm ơn bạn đã gửi đánh giá!', 'success');

                // Cập nhật UI để hiển thị đã đánh giá
                const reviewButton = document.querySelector(`.btn-review[data-id="${appointmentId}"]`);
                if (reviewButton) {
                    const parent = reviewButton.parentElement;
                    parent.innerHTML = '<span class="reviewed-badge"><i class="fas fa-check-circle"></i> Đã đánh giá</span>';
                }
            });
        }
    }

    function submitReview(appointmentId, rating, comment) {
        // Gửi đánh giá (nếu có API thực tế thì thay thế đoạn này)
    }

    function showToast(message, type = 'info') {
        // Kiểm tra nếu container toast đã tồn tại
        let toastContainer = document.querySelector('.toast-container');

        if (!toastContainer) {
            // Tạo container nếu chưa có
            toastContainer = document.createElement('div');
            toastContainer.className = 'toast-container position-fixed bottom-0 end-0 p-3';
            document.body.appendChild(toastContainer);
        }

        // Tạo ID duy nhất cho toast
        const toastId = 'toast-' + Date.now();

        // Xác định class theo loại thông báo
        let bgClass = 'bg-info text-white';
        if (type === 'success') bgClass = 'bg-success text-white';
        if (type === 'warning') bgClass = 'bg-warning text-dark';
        if (type === 'error') bgClass = 'bg-danger text-white';

        // Tạo HTML cho toast
        const toastHTML = `
        <div id="${toastId}" class="toast align-items-center ${bgClass} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
        `;

        // Thêm toast vào container
        toastContainer.insertAdjacentHTML('beforeend', toastHTML);

        // Khởi tạo và hiển thị toast
        const toastElement = document.getElementById(toastId);
        const toast = new bootstrap.Toast(toastElement, {
            autohide: true,
            delay: 3000
        });

        toast.show();

        // Xóa toast khỏi DOM sau khi ẩn
        toastElement.addEventListener('hidden.bs.toast', function () {
            toastElement.remove();
        });
    }

    // ===== Khởi tạo Modal Chi tiết Lịch hẹn =====
    initAppointmentDetailModal();
    function initAppointmentDetailModal() {
        document.addEventListener('click', function (e) {
            console.log('Click target:', e.target);
            const card = e.target.closest('.appointment-card');
            console.log('Found card:', card);
            if (card && !e.target.classList.contains('btn-review') && !e.target.classList.contains('btn-cancel-request')) {
                const appointmentId = card.getAttribute('data-appointment-id') || card.querySelector('[data-id]')?.getAttribute('data-id');
                console.log('Appointment ID:', appointmentId);
                if (!appointmentId) return;
                // Gọi API lấy chi tiết lịch hẹn
                fetch(`/Appointment/Detail/${appointmentId}`)
                    .then(res => res.json())
                    .then(result => {
                        console.log("API result:", result);
                        if (result.success && result.data) {
                            showAppointmentDetailModal(result.data);
                        } else {
                            showToast(result.message || 'Không lấy được chi tiết lịch hẹn', 'error');
                        }
                    })
                    .catch(() => showToast('Có lỗi khi lấy chi tiết lịch hẹn', 'error'));
            }
        });
    }


    function showAppointmentDetailModal(appointment) {
        const modal = document.getElementById('appointmentDetailModal');
        const content = document.getElementById('appointmentDetailContent');
        if (!modal || !content) return;

        // Generate appointment header info
        const appointmentDate = new Date(appointment.appointmentDate);
        const startTime = formatTimeSpan(appointment.startTime);
        const endTime = formatTimeSpan(appointment.endTime);

        const appointmentInfo = `
            <div class="appointment-detail-header">
                <div class="container-fluid">
                    <div class="row">
                        <div class="col-md-8">
                            <h4 class="mb-2">
                                <i class="fas fa-calendar-check me-2"></i>
                                Lịch hẹn #${appointment.appointmentId}
                            </h4>
                            <div class="d-flex flex-wrap gap-3">
                                <span><i class="fas fa-clock me-1"></i> ${formatDate(appointmentDate)} | ${startTime} - ${endTime}</span>
                                <span><i class="fas fa-paw me-1"></i> ${appointment.petNames.join(', ')}</span>
                            </div>
                        </div>
                        <div class="col-md-4 text-end">
                            <span class="badge badge-lg bg-${getServiceStatusColor(appointment.statusId)} fs-6">
                                ${appointment.statusName}
                            </span>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Group services by pet using Pet-Staff assignments
        const petServiceMap = {};

        if (appointment.petStaffAssignments && appointment.services) {
            // Initialize pets from petStaffAssignments
            appointment.petStaffAssignments.forEach(petStaff => {
                petServiceMap[petStaff.petId] = {
                    petName: petStaff.petName,
                    staffName: petStaff.staffName || 'Chưa phân công',
                    services: []
                };
            });

            // Add all services to all pets (since services apply to all pets in appointment)
            appointment.services.forEach(service => {
                Object.keys(petServiceMap).forEach(petId => {
                    // Find images for this specific pet
                    const petImages = service.petImages?.find(img => img.petId == petId) || { before: [], after: [] };

                    petServiceMap[petId].services.push({
                        appointmentServiceId: service.appointmentServiceId, // Use actual appointmentServiceId from API
                        serviceName: service.name,
                        status: service.statusId,
                        petImages: petImages
                    });
                });
            });
        }

        let petSectionsHtml = '';

        if (Object.keys(petServiceMap).length > 0) {
            Object.entries(petServiceMap).forEach(([petId, petData]) => {
                petSectionsHtml += `
                    <div class="pet-service-section">
                        <div class="pet-header">
                            <div class="d-flex justify-content-between align-items-center">
                                <div>
                                    <i class="fas fa-paw me-2"></i>
                                    <strong>${petData.petName}</strong>
                                </div>
                                <div class="text-end">
                                    <small>Nhân viên phụ trách: <strong>${petData.staffName}</strong></small>
                                </div>
                            </div>
                        </div>
                        <div class="service-timeline">
                `;

                if (petData.services && petData.services.length > 0) {
                    petData.services.forEach((service, idx) => {
                        const statusClass = getServiceStatusClass(service.status);
                        const statusName = getServiceStatusName(service.status);

                        petSectionsHtml += `
                            <div class="service-item ${statusClass}">
                                <div class="service-header">
                                    <h5 class="service-name">${service.serviceName}</h5>
                                    <span class="badge status-${statusClass} service-status">${statusName}</span>
                                </div>

                                <div class="service-meta">
                                    <div class="staff-info">
                                        <i class="fas fa-user-tie me-1"></i>
                                        Nhân viên thực hiện: <strong>${petData.staffName}</strong>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="image-section">
                                            <h6><i class="fas fa-camera me-1"></i> Ảnh trước khi làm</h6>
                                            <div class="image-grid" id="before-images-${service.appointmentServiceId}">
                                                <div class="no-images">
                                                    <i class="fas fa-image fa-2x mb-2"></i>
                                                    <div>Chưa có ảnh</div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="image-section">
                                            <h6><i class="fas fa-camera me-1"></i> Ảnh sau khi làm</h6>
                                            <div class="image-grid" id="after-images-${service.appointmentServiceId}">
                                                <div class="no-images">
                                                    <i class="fas fa-image fa-2x mb-2"></i>
                                                    <div>Chưa có ảnh</div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        `;
                    });
                } else {
                    petSectionsHtml += `
                        <div class="text-center text-muted py-4">
                            <i class="fas fa-info-circle fa-2x mb-2"></i>
                            <div>Chưa có dịch vụ nào được đặt cho thú cưng này</div>
                        </div>
                    `;
                }

                petSectionsHtml += `
                        </div>
                    </div>
                `;
            });
        } else {
            petSectionsHtml = `
                <div class="text-center text-muted py-5">
                    <i class="fas fa-exclamation-triangle fa-3x mb-3"></i>
                    <h5>Không có thông tin chi tiết</h5>
                    <p>Lịch hẹn này chưa có thông tin về thú cưng và dịch vụ.</p>
                </div>
            `;
        }

        // Notes section
        const notesSection = appointment.notes ? `
            <div class="appointment-info-card">
                <h6><i class="fas fa-sticky-note me-2"></i>Ghi chú</h6>
                <p class="mb-0">${appointment.notes}</p>
            </div>
        ` : '';

        const finalHtml = `
            ${appointmentInfo}
            <div class="container-fluid py-3">
                ${notesSection}
                ${petSectionsHtml}
            </div>
        `;

        content.innerHTML = finalHtml;

        // Debug: List all image containers in the modal
        console.log('🔍 [DEBUG] Modal HTML rendered. Looking for all image containers...');
        const allImageContainers = document.querySelectorAll('[id*="before-images"], [id*="after-images"]');
        console.log('🔍 [DEBUG] Found image containers:', allImageContainers.length);
        allImageContainers.forEach(container => {
            console.log('🔍 [DEBUG] Container ID:', container.id);
        });

        // Load images for each service (using the working logic from before)
        if (appointment.services) {
            appointment.services.forEach(service => {
                if (appointment.petStaffAssignments) {
                    appointment.petStaffAssignments.forEach(petStaff => {
                        const serviceId = service.appointmentServiceId; // Use actual appointmentServiceId
                        console.log('🔍 [DEBUG] Loading images for appointmentServiceId:', serviceId);
                        loadServiceImagesForModal(serviceId);
                    });
                }
            });
        }

        new bootstrap.Modal(modal).show();
    }

    // Function to load images for a specific service in modal
    function loadServiceImagesForModal(appointmentServiceId) {
        console.log('🔍 [DEBUG] Loading images for appointmentServiceId:', appointmentServiceId);

        // Try customer endpoint first, then staff endpoint as fallback
        fetch(`/Appointment/GetServiceImages?appointmentServiceId=${appointmentServiceId}`)
            .then(response => response.json())
            .then(result => {
                console.log('🔍 [DEBUG] API response:', result);

                if (result.success && result.images && result.images.length > 0) {
                    console.log('🔍 [DEBUG] Raw images from API:', result.images);

                    // Group images by petId, handle null/empty petId
                    const imagesByPet = {};
                    result.images.forEach(img => {
                        // Use 'default' for null/empty petId
                        const petKey = img.petId || 'default';

                        if (!imagesByPet[petKey]) {
                            imagesByPet[petKey] = { before: [], after: [] };
                        }

                        if (img.photoType === 'Before') {
                            imagesByPet[petKey].before.push(img);
                        } else if (img.photoType === 'After') {
                            imagesByPet[petKey].after.push(img);
                        }
                    });

                    console.log('🔍 [DEBUG] Images grouped by pet:', imagesByPet);

                    // Display images for each pet - try all possible container IDs
                    Object.keys(imagesByPet).forEach(petKey => {
                        const petImages = imagesByPet[petKey];

                        // Try multiple container ID formats
                        const possibleServiceIds = [
                            `${appointmentServiceId}`, // Simple format: before-images-56
                            `${appointmentServiceId}_${petKey}`,
                            `${appointmentServiceId}_0`,
                            `${appointmentServiceId}_default`
                        ];

                        // Also try to find containers for all pets in this appointment
                        const allContainers = document.querySelectorAll(`[id^="before-images-${appointmentServiceId}_"], [id^="after-images-${appointmentServiceId}_"]`);
                        console.log('🔍 [DEBUG] Found containers with prefix:', allContainers.length);

                        let foundContainer = false;

                        // Try each possible service ID
                        possibleServiceIds.forEach(serviceId => {
                            console.log('🔍 [DEBUG] Trying serviceId:', serviceId);
                            const beforeContainer = document.getElementById(`before-images-${serviceId}`);
                            const afterContainer = document.getElementById(`after-images-${serviceId}`);

                            if (beforeContainer && afterContainer) {
                                console.log('🔍 [DEBUG] Found containers for serviceId:', serviceId);
                                foundContainer = true;

                                // Render before images
                                if (petImages.before.length > 0) {
                                    beforeContainer.innerHTML = petImages.before.map(img =>
                                        `<img src="${img.imageUrl}" class="service-image" alt="Before" onclick="viewImageFullSize('${img.imageUrl}')" title="Xem ảnh lớn">`
                                    ).join('');
                                }

                                // Render after images
                                if (petImages.after.length > 0) {
                                    afterContainer.innerHTML = petImages.after.map(img =>
                                        `<img src="${img.imageUrl}" class="service-image" alt="After" onclick="viewImageFullSize('${img.imageUrl}')" title="Xem ảnh lớn">`
                                    ).join('');
                                }
                            }
                        });

                        if (!foundContainer) {
                            console.log('🔍 [DEBUG] No containers found for petKey:', petKey, 'appointmentServiceId:', appointmentServiceId);
                        }
                    });
                } else {
                    console.log('🔍 [DEBUG] No images returned from API');
                }
            })
            .catch(error => {
                console.log('🔍 [DEBUG] Error loading images:', error);
            });
    }

    // Function to view image in full size
    function viewImageFullSize(imageUrl) {
        window.open(imageUrl, '_blank');
    }

    // Function to display images in modal containers
    function displayImagesInModal(serviceId, beforeImages, afterImages) {
        const beforeContainer = document.getElementById(`before-images-${serviceId}`);
        const afterContainer = document.getElementById(`after-images-${serviceId}`);

        if (beforeContainer && beforeImages && beforeImages.length > 0) {
            beforeContainer.innerHTML = beforeImages.map(imageUrl =>
                `<img src="${imageUrl}" class="service-image" alt="Before" onclick="viewImageFullSize('${imageUrl}')" title="Xem ảnh lớn">`
            ).join('');
        }

        if (afterContainer && afterImages && afterImages.length > 0) {
            afterContainer.innerHTML = afterImages.map(imageUrl =>
                `<img src="${imageUrl}" class="service-image" alt="After" onclick="viewImageFullSize('${imageUrl}')" title="Xem ảnh lớn">`
            ).join('');
        }
    }

    // Make function global so it can be called from HTML
    window.viewImageFullSize = viewImageFullSize;

    // Helper function to get service status class
    function getServiceStatusClass(status) {
        switch(status) {
            case 1: return 'pending';
            case 2: return 'in-progress';
            case 3: return 'completed';
            case 4: return 'cancelled';
            default: return 'pending';
        }
    }

    // Helper function to get service status name
    function getServiceStatusName(status) {
        switch(status) {
            case 1: return 'Chờ thực hiện';
            case 2: return 'Đang thực hiện';
            case 3: return 'Hoàn thành';
            case 4: return 'Đã hủy';
            default: return 'Không xác định';
        }
    }

    // Helper function to format TimeSpan to HH:mm
    function formatTimeSpan(timeSpan) {
        if (!timeSpan) return '00:00';

        // TimeSpan format: "HH:mm:ss" or "H:mm:ss"
        const parts = timeSpan.toString().split(':');
        if (parts.length >= 2) {
            const hours = parts[0].padStart(2, '0');
            const minutes = parts[1].padStart(2, '0');
            return `${hours}:${minutes}`;
        }
        return timeSpan;
    }

    // Helper function to get service status color (reuse existing function)
    function getServiceStatusColor(statusId) {
        switch(statusId) {
            case 1: return 'secondary'; // Pending
            case 2: return 'warning';   // In Progress
            case 3: return 'success';   // Completed
            case 4: return 'danger';    // Cancelled
            default: return 'secondary';
        }
    }

});