/* Service Dashboard specific JavaScript */

$(document).ready(function() {
    // Initialize dashboard charts
    initServiceChart();
    initCategoryChart();
    
    // Initialize DataTables for dashboard tables
    $('#recentServicesTable').DataTable({
        paging: false,
        searching: false,
        info: false,
        responsive: true,
        order: [[0, 'desc']]
    });
    
    $('#upcomingAppointmentsTable').DataTable({
        paging: false,
        searching: false,
        info: false,
        responsive: true,
        order: [[1, 'asc']]
    });
    
    // Display success message if any
    if (typeof successMessage !== 'undefined' && successMessage) {
        Swal.fire({
            icon: 'success',
            title: 'Thành công',
            text: successMessage,
            timer: 2000,
            timerProgressBar: true
        });
    }
    
    // Display error message if any
    if (typeof errorMessage !== 'undefined' && errorMessage) {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: errorMessage
        });
    }
});

// Initialize the service booking chart
function initServiceChart() {
    // Check if the chart canvas exists
    const ctx = document.getElementById('serviceChart');
    if (!ctx) return;
    
    // Create the chart using Chart.js
    const myBarChart = new Chart(ctx.getContext('2d'), {
        type: 'bar',
        data: {
            labels: typeof topServiceLabels !== 'undefined' ? topServiceLabels : [],
            datasets: [{
                label: "Số lượt đặt",
                backgroundColor: "#4e73df",
                hoverBackgroundColor: "#2e59d9",
                borderColor: "#4e73df",
                data: typeof topServiceData !== 'undefined' ? topServiceData : [],
            }],
        },
        options: {
            maintainAspectRatio: false,
            layout: {
                padding: {
                    left: 10,
                    right: 25,
                    top: 25,
                    bottom: 0
                }
            },
            scales: {
                xAxes: [{
                    gridLines: {
                        display: false,
                        drawBorder: false
                    },
                    ticks: {
                        maxTicksLimit: 6
                    },
                    maxBarThickness: 25,
                }],
                yAxes: [{
                    ticks: {
                        min: 0,
                        maxTicksLimit: 5,
                        padding: 10,
                    },
                    gridLines: {
                        color: "rgb(234, 236, 244)",
                        zeroLineColor: "rgb(234, 236, 244)",
                        drawBorder: false,
                        borderDash: [2],
                        zeroLineBorderDash: [2]
                    }
                }],
            },
            legend: {
                display: false
            },
            tooltips: {
                titleMarginBottom: 10,
                titleFontColor: '#6e707e',
                titleFontSize: 14,
                backgroundColor: "rgb(255,255,255)",
                bodyFontColor: "#858796",
                borderColor: '#dddfeb',
                borderWidth: 1,
                xPadding: 15,
                yPadding: 15,
                displayColors: false,
                caretPadding: 10,
            },
        }
    });
    
    return myBarChart;
}

// Initialize the category distribution chart
function initCategoryChart() {
    // Check if the chart canvas exists
    const ctx = document.getElementById('categoryChart');
    if (!ctx) return;
    
    // Create the chart using Chart.js
    const myPieChart = new Chart(ctx.getContext('2d'), {
        type: 'doughnut',
        data: {
            labels: typeof categoryLabels !== 'undefined' ? categoryLabels : [],
            datasets: [{
                data: typeof categoryData !== 'undefined' ? categoryData : [],
                backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc', '#f6c23e', '#e74a3b', '#858796'],
                hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf', '#dda20a', '#be2617', '#6e707e'],
                hoverBorderColor: "rgba(234, 236, 244, 1)",
            }],
        },
        options: {
            maintainAspectRatio: false,
            tooltips: {
                backgroundColor: "rgb(255,255,255)",
                bodyFontColor: "#858796",
                borderColor: '#dddfeb',
                borderWidth: 1,
                xPadding: 15,
                yPadding: 15,
                displayColors: false,
                caretPadding: 10,
            },
            legend: {
                display: false
            },
            cutoutPercentage: 80,
        },
    });
    
    return myPieChart;
}
