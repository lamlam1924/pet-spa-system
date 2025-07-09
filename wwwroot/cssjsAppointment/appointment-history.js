/**
 * appointment-history-grouped.js - Logic xử lý cho trang lịch sử đặt lịch với nhóm thời gian
 */

// Debug helper
console.log("Grouped Timeline JS loaded");

document.addEventListener('DOMContentLoaded', function() {
    // Sự kiện gửi yêu cầu hủy lịch
    document.addEventListener('click', function(e) {
        if (e.target.classList.contains('btn-cancel-request') || e.target.closest('.btn-cancel-request')) {
            const button = e.target.classList.contains('btn-cancel-request') ? e.target : e.target.closest('.btn-cancel-request');
            const appointmentId = button.getAttribute('data-id');
            if (!appointmentId) return;
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
    // Các biến cần thiết
    const timelineContainer = document.querySelector('.history-timeline');
    const searchInput = document.querySelector('.search-box input');
    const sortSelect = document.querySelector('.sort-select');
    const statusFilters = document.querySelectorAll('.filter-badge');
    let appointmentData = []; // Dữ liệu lịch hẹn từ server
    let currentPage = 1;
    const itemsPerPage = 6;
    
    // Khởi tạo dữ liệu (thay thế bằng dữ liệu thực từ server)
    fetchAppointmentData();
    
    // Xử lý sự kiện tìm kiếm
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            currentPage = 1;
            renderAppointments();
        });
    }
    
    // Xử lý sự kiện sắp xếp
    if (sortSelect) {
        sortSelect.addEventListener('change', function() {
            currentPage = 1;
            renderAppointments();
        });
    }
    
    // Xử lý sự kiện lọc theo trạng thái
    if (statusFilters) {
        statusFilters.forEach(filter => {
            filter.addEventListener('click', function() {
                // Bỏ active của tất cả các filter khác
                statusFilters.forEach(f => f.classList.remove('active'));
                // Thêm active cho filter hiện tại
                this.classList.add('active');
                currentPage = 1;
                renderAppointments();
            });
        });
    }
    
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
    
    // Hiển thị các lịch hẹn theo nhóm thời gian
    function renderAppointments() {
        if (!timelineContainer) {
            console.error("Timeline container not found!");
            return;
        }
        
        console.log("Rendering appointments with data:", appointmentData);
        
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
                return new Date(a.date) - new Date(b.date);
            } else if (sortBy === 'newest') {
                return new Date(b.date) - new Date(a.date);
            } else if (sortBy === 'status') {
                return a.status - b.status;
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
            const allowCancel = [1,2,5].includes(appointment.statusId) && !appointment.isPendingCancel;
            let cancelBadge = '';
            if (appointment.isPendingCancel) {
                cancelBadge = '<span class="badge bg-warning text-dark ms-2">Chờ duyệt hủy</span>';
            }
            html += `
            <div class="appointment-card ${position} status-${appointment.statusId} ${animateClass}" data-status="${appointment.statusId}">
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
                            `<span class="service-tag${!service.isActive ? ' inactive' : ''}">${service.name}</span>`
                        ).join('')}
                    </div>
                    <div class="appointment-pets">
                        <h5>Thú cưng</h5>
                        ${appointment.petNames.map(pet => `<span class="pet-tag">${pet}</span>`).join('')}
                    </div>
                    ${appointment.notes ? `<div class="appointment-notes">${appointment.notes}</div>` : ''}
                    <div class="appointment-actions">
                        ${appointment.statusId === 3 ? 
                            `<button class="btn-review" data-id="${appointment.appointmentId}">
                                <i class="fas fa-star"></i> Đánh giá
                            </button>` : ''}
                        ${allowCancel ? `<button class="btn btn-outline-danger btn-cancel-request" data-id="${appointment.appointmentId}"><i class="fas fa-times-circle"></i> Yêu cầu hủy lịch</button>` : ''}
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
    
    // Khởi tạo sự kiện thu gọn/mở rộng nhóm
    function initCollapseEvents() {
        const collapseButtons = document.querySelectorAll('.time-group-collapse');
        
        collapseButtons.forEach(button => {
            button.addEventListener('click', function() {
                const monthYear = this.getAttribute('data-month');
                const content = document.querySelector(`.time-group-content[data-month="${monthYear}"]`);
                
                this.classList.toggle('collapsed');
                content.classList.toggle('collapsed');
            });
        });
    }
    
    // Animation cho các card
    function animateCards() {
        const cards = document.querySelectorAll('.appointment-card');
        cards.forEach((card, index) => {
            setTimeout(() => {
                card.style.opacity = 1;
            }, index * 100);
        });
    }
    
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
    
    // Không cần getStatusText vì đã có statusName từ model
    
    function getStatusClass(status) {
        switch (status) {
            case 1: return 'status-pending';
            case 2: return 'status-confirmed';
            case 3: return 'status-inprogress';
            case 4: return 'status-completed';
            case 5: return 'status-pendingcancel';
            case 6: return 'status-cancelled';
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
    
    // Không cần generateMockData() nữa vì đã có dữ liệu thực từ server
    
    // Khởi tạo Modal Review
    initReviewModal();
    
    function initReviewModal() {
        // Lắng nghe sự kiện click nút đánh giá
        document.addEventListener('click', function(e) {
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
            reviewForm.addEventListener('submit', function(e) {
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
        console.log(`Đã gửi đánh giá cho lịch hẹn #${appointmentId}: ${rating} sao, "${comment}"`);
        
        // Thay thế bằng API call thực tế khi có API
        // Ví dụ:
        /*
        fetch('/api/reviews/submit', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                appointmentId: appointmentId,
                rating: rating,
                comment: comment
            })
        })
        .then(response => response.json())
        .then(data => {
            console.log('Success:', data);
        })
        .catch((error) => {
            console.error('Error:', error);
        });
        */
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
        toastElement.addEventListener('hidden.bs.toast', function() {
            toastElement.remove();
        });
    }
});
