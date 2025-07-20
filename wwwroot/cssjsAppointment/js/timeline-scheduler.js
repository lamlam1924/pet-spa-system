// Slide-in detail panel logic
// ...existing code...
document.addEventListener('DOMContentLoaded', function () {
    // Set block height theo thời lượng
    document.querySelectorAll('.timeline-block[data-duration]').forEach(function(block) {
        var duration = parseInt(block.getAttribute('data-duration')) || 30;
        block.style.setProperty('--duration-minutes', duration);
    });

    // Fullscreen logic
    var btnFullscreen = document.getElementById('btnFullscreen');
    if (btnFullscreen) {
        btnFullscreen.addEventListener('click', function () {
            var url = window.location.pathname + window.location.search;
            if (url.indexOf('fullscreen=1') === -1) {
                if (url.indexOf('?') === -1) url += '?fullscreen=1';
                else url += '&fullscreen=1';
            }
            window.location.href = url;
        });
    }
    var btnExitFullscreen = document.getElementById('btnExitFullscreen');
    if (btnExitFullscreen) {
        btnExitFullscreen.addEventListener('click', function () {
            var url = window.location.pathname + window.location.search.replace(/([&?])fullscreen=1(&)?/, function (match, p1, p2) {
                if (p1 === '?' && !p2) return '';
                if (p1 === '&' && !p2) return '';
                return p1;
            });
            window.location.href = url;
        });
    }
    // Show detail panel when clicking on a timeline block
    document.querySelectorAll('.timeline-block').forEach(function (block) {
        block.addEventListener('click', function (e) {
            var apptId = this.getAttribute('data-appointment-id');
            console.log('Clicked block', apptId); // debug
            if (!apptId) return;
            var detailPanel = document.getElementById('detailPanel');
            var detailContent = document.getElementById('detailContent');
            var overlay = document.getElementById('timelineDetailOverlay');
            detailPanel.classList.add('open');
            if (overlay) overlay.style.display = 'block';
            detailPanel.style.right = '0';
            detailPanel.style.zIndex = '10000'; // đảm bảo panel nổi lên trên
            detailContent.innerHTML = '<div class="text-center text-muted py-4"><i class="fas fa-spinner fa-spin fa-2x"></i><br>Đang tải chi tiết...</div>';
            fetch('/AdminAppointment/GetAppointmentDetail?id=' + apptId)
                .then(function (res) { return res.text(); })
                .then(function (html) {
                    detailContent.innerHTML = html;
                })
                .catch(function () {
                    detailContent.innerHTML = '<div class="text-danger">Không thể tải chi tiết lịch hẹn.</div>';
                });
        });
    });
    // Close detail panel
    var btnCloseDetail = document.getElementById('btnCloseDetail');
    var overlay = document.getElementById('timelineDetailOverlay');
    function closeDetailPanel() {
        var detailPanel = document.getElementById('detailPanel');
        if (detailPanel) detailPanel.classList.remove('open');
        if (detailPanel) detailPanel.style.right = '-400px';
        if (overlay) overlay.style.display = 'none';
    }
    if (btnCloseDetail) {
        btnCloseDetail.addEventListener('click', closeDetailPanel);
    }
    if (overlay) {
        overlay.addEventListener('click', closeDetailPanel);
    }

    // Quick search logic (demo, filter block by text)
    document.getElementById('quickSearchForm').addEventListener('submit', function(e) {
        e.preventDefault();
        var keyword = document.getElementById('searchKeyword').value.trim().toLowerCase();
        var resultDiv = document.getElementById('quickSearchResult');
        var blocks = document.querySelectorAll('.timeline-block');
        var results = [];
        blocks.forEach(function(block) {
            if (block.textContent.toLowerCase().includes(keyword)) {
                results.push(`
                    <div class="card card-body mb-2 p-2" style="cursor:pointer;"
                        onclick="document.getElementById('detailPanel').style.right='0';
                                 document.getElementById('detailContent').innerHTML='<div class=&quot;font-weight-bold mb-2&quot;>${block.textContent}</div><div>Mã lịch: ${block.getAttribute('data-block-id')}</div><div class=&quot;mt-2&quot;>Chi tiết dịch vụ, thú cưng, khách hàng...</div>';">
                        ${block.textContent}
                    </div>
                `);
            }
        });
        resultDiv.innerHTML = results.length ? results.join('') : '<div class="text-muted">Không tìm thấy lịch hẹn phù hợp.</div>';
    });
});
