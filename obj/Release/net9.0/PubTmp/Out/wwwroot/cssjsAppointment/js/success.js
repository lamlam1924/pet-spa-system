/**
 * success.js - JavaScript cho trang thông báo đặt lịch thành công
 */

// ===================== JS Thông báo đặt lịch thành công (Success Page) =====================

$(document).ready(function() {
    // ===== Thêm hiệu ứng confetti khi trang tải xong =====
    const duration = 3 * 1000;
    const animationEnd = Date.now() + duration;
    
    function randomInRange(min, max) {
        return Math.random() * (max - min) + min;
    }

    // Chỉ thực hiện khi trang web hỗ trợ confetti
    if (window.confetti) {
        const interval = setInterval(function() {
            const timeLeft = animationEnd - Date.now();

            if (timeLeft <= 0) {
                return clearInterval(interval);
            }

            const particleCount = 50 * (timeLeft / duration);
            
            // Tạo confetti ở phía trên màn hình
            confetti({
                particleCount,
                spread: 70,
                origin: { y: 0 }
            });
            
            // Tạo confetti ở phía trái màn hình
            confetti({
                particleCount: particleCount / 2,
                angle: 60,
                spread: 55,
                origin: { x: 0 }
            });
            
            // Tạo confetti ở phía phải màn hình
            confetti({
                particleCount: particleCount / 2,
                angle: 120,
                spread: 55,
                origin: { x: 1 }
            });
        }, 250);
    }
});
