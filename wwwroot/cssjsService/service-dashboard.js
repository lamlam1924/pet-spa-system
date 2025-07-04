// JavaScript for Service Dashboard
document.addEventListener("DOMContentLoaded", function() {
    // Chart dịch vụ nổi bật
    if (document.getElementById("serviceChart")) {
        var ctx = document.getElementById("serviceChart").getContext("2d");
        var serviceData = serviceBookingData; // This will be passed from the view
        
        new Chart(ctx, {
            type: "bar",
            data: {
                labels: serviceData.labels,
                datasets: [{
                    label: "Số lần đặt",
                    backgroundColor: window.theme.primary,
                    borderColor: window.theme.primary,
                    hoverBackgroundColor: window.theme.primary,
                    hoverBorderColor: window.theme.primary,
                    data: serviceData.data,
                    barPercentage: 0.75,
                    categoryPercentage: 0.5
                }]
            },
            options: {
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: {
                    y: {
                        grid: {
                            color: "rgba(0, 0, 0, 0.05)"
                        },
                        beginAtZero: true
                    },
                    x: {
                        grid: {
                            display: false
                        }
                    }
                }
            }
        });
    }

    // Chart phân bố danh mục
    if (document.getElementById("categoryChart")) {
        var ctx = document.getElementById("categoryChart").getContext("2d");
        var categoryData = categoryDistributionData; // This will be passed from the view
        
        new Chart(ctx, {
            type: 'pie',
            data: {
                labels: categoryData.labels,
                datasets: [{
                    data: categoryData.data,
                    backgroundColor: [
                        window.theme.primary,
                        window.theme.warning,
                        window.theme.danger,
                        window.theme.info,
                        window.theme.success,
                        window.theme.secondary,
                        "#d2f5fe", "#feeaa5", "#fecda5", "#d9f5d6"
                    ],
                    borderColor: "transparent"
                }]
            },
            options: {
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: true
                    }
                }
            }
        });
    }
});
