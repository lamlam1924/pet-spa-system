/**
 * services-list.js - JavaScript cho trang danh sách dịch vụ
 */

// ===================== JS Danh sách dịch vụ (Services List) =====================

$(document).ready(function() {
    // ===== Thêm animation vào các card khi trang load =====
    animateServiceCards();
    
    // ===== Xử lý khi thay đổi bộ lọc danh mục =====
    $('#category-filter').on('change', function() {
        // Hiển thị loader
        $('#service-loading').show();
        // Tự động submit form sau một chút delay để có hiệu ứng loading
        setTimeout(function() {
            $('#filter-form').submit();
        }, 300);
    });
    
    // ===== Tìm kiếm dịch vụ theo tên ngay trên trang =====
    $('#service-search').on('input', function() {
        const searchText = $(this).val().toLowerCase();
        // Lọc các card dựa vào từ khóa tìm kiếm
        $('.service-card').each(function() {
            const serviceName = $(this).find('.service-title').text().toLowerCase();
            const serviceDesc = $(this).find('.service-description').text().toLowerCase();
            if (serviceName.includes(searchText) || serviceDesc.includes(searchText)) {
                $(this).parent().fadeIn(300);
            } else {
                $(this).parent().fadeOut(300);
            }
        });
        // Hiển thị thông báo nếu không tìm thấy kết quả
        const visibleCards = $('.service-card').parent().filter(':visible').length;
        if (visibleCards === 0) {
            $('#no-results').fadeIn(300);
        } else {
            $('#no-results').fadeOut(300);
        }
    });
    
    // ===== Hiệu ứng khi hover vào card dịch vụ =====
    $('.service-card').hover(
        function() {
            $(this).find('.service-badge').addClass('animate__animated animate__heartBeat');
        },
        function() {
            $(this).find('.service-badge').removeClass('animate__animated animate__heartBeat');
        }
    );
    
    // ===== Sort dịch vụ theo giá =====
    $('#sort-price').on('click', function() {
        const isAscending = $(this).hasClass('asc');
        sortServices('price', !isAscending);
        // Toggle trạng thái sorting
        $(this).toggleClass('asc');
        $(this).html(isAscending ? 
            '<i class="fas fa-sort-amount-down"></i> Giá: Cao đến thấp' : 
            '<i class="fas fa-sort-amount-up"></i> Giá: Thấp đến cao'
        );
    });
    
    // ===== Sort dịch vụ theo thời lượng =====
    $('#sort-duration').on('click', function() {
        const isAscending = $(this).hasClass('asc');
        sortServices('duration', !isAscending);
        // Toggle trạng thái sorting
        $(this).toggleClass('asc');
        $(this).html(isAscending ? 
            '<i class="fas fa-sort-amount-down"></i> Thời lượng: Dài đến ngắn' : 
            '<i class="fas fa-sort-amount-up"></i> Thời lượng: Ngắn đến dài'
        );
    });
});

// ===== Hàm thêm animation vào các card =====
function animateServiceCards() {
    $('.service-card').each(function(index) {
        $(this).addClass('opacity-0').addClass('animate-fade-in');
        $(this).addClass('delay-' + (index % 5 + 1));
        setTimeout(() => {
            $(this).removeClass('opacity-0');
        }, 100);
    });
}

// ===== Hàm sắp xếp dịch vụ =====
function sortServices(sortBy, ascending) {
    const serviceContainer = $('#service-container');
    const serviceItems = serviceContainer.children('.col-md-6').get();
    serviceItems.sort(function(a, b) {
        let valueA, valueB;
        if (sortBy === 'price') {
            valueA = parseInt($(a).find('.service-price').attr('data-price'));
            valueB = parseInt($(b).find('.service-price').attr('data-price'));
        } else if (sortBy === 'duration') {
            valueA = parseInt($(a).find('.service-duration').attr('data-duration'));
            valueB = parseInt($(b).find('.service-duration').attr('data-duration'));
        }
        return ascending ? valueA - valueB : valueB - valueA;
    });
    // Xóa các phần tử hiện tại và thêm lại theo thứ tự mới
    $.each(serviceItems, function(index, item) {
        serviceContainer.append(item);
    });
    // Thêm hiệu ứng
    $('#service-container').addClass('fade-transition');
    setTimeout(function() {
        $('#service-container').removeClass('fade-transition');
    }, 500);
}
