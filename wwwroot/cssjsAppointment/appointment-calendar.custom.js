// appointment-calendar.custom.js
document.addEventListener('DOMContentLoaded', function() {
    // Lấy dữ liệu calendar từ thẻ script JSON
    let calendarData = [];
    const dataScript = document.getElementById('calendar-data-script');
    if (dataScript) {
        try {
            calendarData = JSON.parse(dataScript.textContent);
        } catch (e) {
            calendarData = [];
        }
    }
    const calendarEl = document.getElementById('calendar');
    const calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        headerToolbar: false,
        dayMaxEvents: true,
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
        },
        dateClick: function(info) {
            window.location.href = `/AdminAppointment/Create?date=${info.dateStr}`;
        }
    });

    calendar.render();

    function updateTitle() {
        const view = calendar.view;
        const title = new Intl.DateTimeFormat('vi-VN', {
            month: 'long',
            year: 'numeric'
        }).format(view.currentStart);
        document.getElementById('calendar-title').textContent = title;
    }

    updateTitle();

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

    function resetOtherButtons(exceptButton) {
        const buttons = ['btn-today', 'btn-tomorrow', 'btn-week', 'btn-month'];
        buttons.forEach(btnId => {
            if (document.getElementById(btnId) !== exceptButton) {
                document.getElementById(btnId).classList.remove('btn-primary');
                document.getElementById(btnId).classList.add('btn-outline-primary');
            }
        });
    }

    function getStatusClass(status) {
        switch (status) {
            case 'Chờ xác nhận': return 'warning text-dark';
            case 'Đã xác nhận': return 'success';
            case 'Đã hoàn thành': return 'secondary';
            case 'Đã hủy': return 'danger';
            default: return 'light';
        }
    }
});
