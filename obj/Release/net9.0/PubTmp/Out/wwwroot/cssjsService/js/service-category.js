/* Service Category specific JavaScript */

$(document).ready(function() {
    // Initialize DataTables
    $('#categoriesTable').DataTable({
        "paging": true,
        "pageLength": 10,
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.25/i18n/Vietnamese.json"
        },
        "responsive": true,
        "ordering": true,
        "order": [[0, 'asc']],
        "columnDefs": [
            { "orderable": false, "targets": -1 } // Disable ordering for action column
        ],
        "drawCallback": function() {
            // Reattach event handlers after table redraw
            $('.btn-edit-category').on('click', handleEditCategoryClick);
            $('.btn-delete-category').on('click', handleDeleteCategoryClick);
        }
    });
    
    // Initialize form validation
    $('#categoryForm').validate({
        rules: {
            'Name': {
                required: true,
                minlength: 2,
                maxlength: 100
            },
            'Description': {
                maxlength: 500
            }
        },
        messages: {
            'Name': {
                required: 'Vui lòng nhập tên danh mục',
                minlength: 'Tên danh mục phải có ít nhất {0} ký tự',
                maxlength: 'Tên danh mục không được vượt quá {0} ký tự'
            },
            'Description': {
                maxlength: 'Mô tả không được vượt quá {0} ký tự'
            }
        },
        errorElement: 'div',
        errorClass: 'invalid-feedback',
        highlight: function(element) {
            $(element).addClass('is-invalid').removeClass('is-valid');
        },
        unhighlight: function(element) {
            $(element).addClass('is-valid').removeClass('is-invalid');
        },
        errorPlacement: function(error, element) {
            error.insertAfter(element);
        }
    });
    
    // Initialize tooltips
    $('[data-toggle="tooltip"]').tooltip();
    
    // Initialize Sortable for category ordering
    const sortable = new Sortable(document.getElementById('sortableCategoryList'), {
        animation: 150,
        ghostClass: 'sortable-ghost',
        dragClass: 'sortable-drag',
        handle: '.sortable-handle'
    });
    
    // Handle save order button
    $('#btnSaveOrder').on('click', function() {
        const categories = [];
        $('.category-card').each(function(index) {
            categories.push({
                CategoryId: parseInt($(this).data('id')),
                DisplayOrder: index + 1
            });
        });
        
        // Send AJAX request to update order
        $.ajax({
            url: updateCategoryOrderUrl,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(categories),
            beforeSend: function(xhr) {
                // Add anti-forgery token
                xhr.setRequestHeader("RequestVerificationToken", 
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function(response) {
                if (response.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Thành công',
                        text: response.message,
                        timer: 2000,
                        timerProgressBar: true
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: response.message
                    });
                }
            },
            error: function() {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi',
                    text: 'Đã xảy ra lỗi khi cập nhật thứ tự danh mục.'
                });
            }
        });
    });
    
    // Initialize category distribution chart
    const initCategoryChart = () => {
        const ctx = document.getElementById('categoryDistributionChart');
        if (!ctx) return;
        
        new Chart(ctx.getContext('2d'), {
            type: 'doughnut',
            data: {
                labels: typeof chartLabels !== 'undefined' ? chartLabels : [],
                datasets: [{
                    data: typeof chartData !== 'undefined' ? chartData : [],
                    backgroundColor: [
                        '#4e73df', '#1cc88a', '#36b9cc', '#f6c23e', '#e74a3b', '#858796',
                        '#6f42c1', '#20c9a6', '#5a5c69', '#2e59d9', '#17a673', '#2c9faf'
                    ],
                    hoverBackgroundColor: [
                        '#2e59d9', '#17a673', '#2c9faf', '#dda20a', '#be2617', '#6e707e',
                        '#5a33a0', '#169b80', '#444550', '#2246b1', '#128f60', '#25808e'
                    ],
                    hoverBorderColor: "rgba(234, 236, 244, 1)",
                }]
            },
            options: {
                maintainAspectRatio: false,
                legend: {
                    display: false
                },
                tooltips: {
                    callbacks: {
                        label: function(tooltipItem, data) {
                            const dataset = data.datasets[tooltipItem.datasetIndex];
                            const currentValue = dataset.data[tooltipItem.index];
                            const total = dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = Math.round((currentValue / total) * 100);
                            return data.labels[tooltipItem.index] + ': ' + currentValue + ' dịch vụ (' + percentage + '%)';
                        }
                    }
                },
                cutoutPercentage: 60
            }
        });
    };
    
    // Initialize the chart
    initCategoryChart();
    
    // Display success/error messages from TempData if available
    if (typeof successMessage !== 'undefined' && successMessage) {
        Swal.fire({
            icon: 'success',
            title: 'Thành công',
            text: successMessage,
            timer: 3000,
            timerProgressBar: true
        });
    }
    
    if (typeof errorMessage !== 'undefined' && errorMessage) {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: errorMessage
        });
    }
});

// Helper function to get a stable random color based on ID
function GetRandomColor(id) {
    const colors = [
        '#4e73df', '#1cc88a', '#36b9cc', '#f6c23e', '#e74a3b', '#858796',
        '#6f42c1', '#20c9a6', '#5a5c69', '#2e59d9', '#17a673', '#2c9faf'
    ];
    return colors[id % colors.length];
}

// Handle edit category button click
function handleEditCategoryClick() {
    const id = $(this).data('id');
    const name = $(this).data('name');
    const description = $(this).data('description');
    const isActive = $(this).data('is-active');
    
    $('#editCategoryId').val(id);
    $('#editCategoryName').val(name);
    $('#editCategoryDescription').val(description);
    $('#editCategoryIsActive').prop('checked', isActive === 'True');
    
    $('#editCategoryModal').modal('show');
}

// Handle delete category button click
function handleDeleteCategoryClick() {
    const id = $(this).data('id');
    const name = $(this).data('name');
    const count = parseInt($(this).data('count'));
    
    $('#deleteCategoryId').val(id);
    $('#deleteCategoryName').text(name);
    
    if (count > 0) {
        $('#serviceCount').text(count);
        $('#categoryHasServices').removeClass('d-none').addClass('d-block');
        $('#btnConfirmDelete').prop('disabled', true);
    } else {
        $('#categoryHasServices').removeClass('d-block').addClass('d-none');
        $('#btnConfirmDelete').prop('disabled', false);
    }
    
    $('#deleteCategoryModal').modal('show');
}
