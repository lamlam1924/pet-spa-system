// ===================== JS Lịch hẹn - Trang lịch (Appointment Calendar) =====================

document.addEventListener('DOMContentLoaded', function() {
    // ===== Lấy dữ liệu lịch từ thẻ script JSON =====
    let calendarData = [];
    const dataScript = document.getElementById('calendar-data-script');
    if (dataScript) {
        try {
            calendarData = JSON.parse(dataScript.textContent);
        } catch (e) {
            calendarData = [];
        }
    }

    // ===== Khởi tạo FullCalendar =====
    const calendarEl = document.getElementById('calendar');
    const calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        headerToolbar: false,
        dayMaxEvents: true,
        moreLinkClick: 'none', // <-- Thêm dòng này
        locale: 'vi',
        firstDay: 1, // Bắt đầu từ thứ 2
        navLinks: true,
        editable: false,
        selectable: true,
        businessHours: {
            daysOfWeek: [0, 1, 2, 3, 4, 5, 6],
            startTime: '08:00',
            endTime: '18:00',
        },
        events: calendarData,
        // ===== Xử lý khi click vào sự kiện lịch =====
        eventClick: function(info) {
            const event = info.event;
            const props = event.extendedProps;
            const content = `
                <h5 class="mb-1">${event.title}</h5>
                <p class="text-muted mb-3">${new Date(event.start).toLocaleString('vi-VN')}</p>
                <div class="mb-3">
                    <span class="badge bg-${getStatusClass(props.status)}">${props.status}</span>
                </div>
                <div class="mb-2">
                    <strong>Khách hàng:</strong>
                    <span>${props.customer}</span>
                </div>
                <div class="mb-2">
                    <strong>Số điện thoại:</strong>
                    <span>${props.phone}</span>
                </div>
                <div class="mb-2">
                    <strong>Thú cưng:</strong>
                    <span>${(props.petNames||[]).join(', ')}</span>
                </div>
                <div class="mb-2">
                    <strong>Dịch vụ:</strong>
                    <span>${(props.services||[]).join(', ')}</span>
                </div>
            `;
            document.getElementById('quickViewContent').innerHTML = content;
            document.getElementById('viewDetailBtn').href = `/AdminAppointment/Detail/${event.id}`;
            const modal = new bootstrap.Modal(document.getElementById('quickViewModal'));
            modal.show();

            // Đảm bảo chỉ có 1 modal backdrop
            $('#quickViewModal').modal('hide');
            setTimeout(function() {
                $('#quickViewModal').modal('show');
            }, 200);
        },
        // ===== Xử lý khi click vào ngày trên lịch =====
        dateClick: function(info) {
            showMiniDayView(info.dateStr);
        },
        // ===== Chỉ cho phép chọn ngày hôm nay hoặc tương lai =====
        selectAllow: function(selectInfo) {
            const today = new Date();
            today.setHours(0,0,0,0);
            return selectInfo.start >= today;
        },
        // ===== Xử lý khi chọn khoảng thời gian trên lịch =====
        select: function(info) {
            const today = new Date();
            today.setHours(0,0,0,0);
            if (info.start < today) {
                // Show quick view hoặc chi tiết
                showQuickView(info.startStr);
                return;
            }
            // ...logic thêm mới như bình thường...
        }
    });

    calendar.render();

    // Sau khi render lịch, thêm class cho ngày tương lai
    setTimeout(() => {
        document.querySelectorAll('.fc-daygrid-day').forEach(cell => {
            const date = cell.getAttribute('data-date');
            if (date) {
                const today = new Date();
                today.setHours(0,0,0,0);
                const cellDate = new Date(date);
                cellDate.setHours(0,0,0,0);
                if (cellDate > today) {
                    cell.classList.add('fc-day-future');
                }
            }
        });
    }, 100);

    // ===== Cập nhật tiêu đề tháng/năm =====
    function updateTitle() {
        const view = calendar.view;
        const title = new Intl.DateTimeFormat('vi-VN', {
            month: 'long',
            year: 'numeric'
        }).format(view.currentStart);
        document.getElementById('calendar-title').textContent = title;
    }

    updateTitle();

    // ===== Xử lý các nút điều hướng lịch =====
    document.getElementById('btn-prev').addEventListener('click', function() {
        calendar.prev();
        updateTitle();
    });
    document.getElementById('btn-next').addEventListener('click', function() {
        calendar.next();
        updateTitle();
    });
    document.getElementById('btn-today').addEventListener('click', function() {
        calendar.today();
        calendar.changeView('dayGridDay');
        updateTitle();
        this.classList.add('btn-primary');
        this.classList.remove('btn-outline-primary');
        resetOtherButtons(this);
    });
    document.getElementById('btn-tomorrow').addEventListener('click', function() {
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        calendar.gotoDate(tomorrow);
        calendar.changeView('dayGridDay');
        updateTitle();
        this.classList.add('btn-primary');
        this.classList.remove('btn-outline-primary');
        resetOtherButtons(this);
    });
    document.getElementById('btn-week').addEventListener('click', function() {
        calendar.changeView('dayGridWeek');
        updateTitle();
        this.classList.add('btn-primary');
        this.classList.remove('btn-outline-primary');
        resetOtherButtons(this);
    });
    document.getElementById('btn-month').addEventListener('click', function() {
        calendar.changeView('dayGridMonth');
        updateTitle();
        this.classList.add('btn-primary');
        this.classList.remove('btn-outline-primary');
        resetOtherButtons(this);
    });

    // ===== Hỗ trợ đổi trạng thái nút =====
    function resetOtherButtons(exceptButton) {
        const buttons = ['btn-today', 'btn-tomorrow', 'btn-week', 'btn-month'];
        buttons.forEach(btnId => {
            if (document.getElementById(btnId) !== exceptButton) {
                document.getElementById(btnId).classList.remove('btn-primary');
                document.getElementById(btnId).classList.add('btn-outline-primary');
            }
        });
    }

    // ===== Đổi màu badge theo trạng thái =====
    function getStatusClass(status) {
        switch (status) {
            case 'Chờ xác nhận': return 'warning text-dark';
            case 'Đã xác nhận': return 'success';
            case 'Đã hoàn thành': return 'secondary';
            case 'Đã hủy': return 'danger';
            default: return 'light';
        }
    }

    function showMiniDayView(dateStr) {
        const events = calendar.getEvents().filter(ev => {
            const evDate = ev.start;
            return evDate && evDate.toISOString().slice(0,10) === dateStr;
        });

        let html = `<div class="mb-3">
            <span class="fw-bold text-primary"><i class="far fa-calendar-alt me-1"></i> ${new Date(dateStr).toLocaleDateString('vi-VN')}</span>
            <span class="badge bg-info ms-2">${events.length} lịch hẹn</span>
        </div>`;

        if (events.length === 0) {
            html += `<div class="alert alert-light text-muted mb-0">Không có lịch hẹn nào.</div>`;
        } else {
            html += `<div class="appointment-list">`;
            events.forEach(ev => {
                html += `
                <div class="appointment-item status-${ev.extendedProps.statusKey || ''}">
                    <div class="appointment-time">
                        <i class="far fa-clock me-1"></i>
                        ${ev.start.toLocaleTimeString('vi-VN', {hour: '2-digit', minute:'2-digit'})}
                    </div>
                    <div class="appointment-title">
                        ${ev.title}
                        <span class="appointment-status status-${ev.extendedProps.statusKey || ''}">
                            ${ev.extendedProps.status || ''}
                        </span>
                    </div>
                    <a href="/AdminAppointment/Detail/${ev.id}" class="btn btn-sm btn-outline-primary ms-2" title="Xem chi tiết">
                        <i class="fas fa-eye"></i>
                    </a>
                </div>`;
            });
            html += `</div>`;
        }

        // Xử lý nút "Thêm lịch mới"
        const today = new Date();
        today.setHours(0,0,0,0);
        const selected = new Date(dateStr);
        selected.setHours(0,0,0,0);
        const addBtn = document.getElementById('addAppointmentBtn');
        if (selected >= today) {
            addBtn.style.display = '';
            addBtn.href = `/AdminAppointment/Create?date=${dateStr}`;
        } else {
            addBtn.style.display = 'none';
        }

        document.getElementById('quickViewContent').innerHTML = html;
        $('#quickViewModal').modal('show'); // Bootstrap 4
    }

    // Vô hiệu hóa click vào "more" (x more) trên lịch
    document.addEventListener('click', function(e) {
        if (e.target.closest('.fc-more')) {
            e.preventDefault();
            e.stopPropagation();
            // Không làm gì cả, không mở modal!
            return false;
        }
    }, true);
});
