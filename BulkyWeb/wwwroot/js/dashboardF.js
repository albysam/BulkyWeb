

document.addEventListener("DOMContentLoaded", function () {
    fetch('/admin/order/TrendingProducts')
        .then(response => response.json())
        .then(data => {
            // Process the data and update your charts here
            // For example:
            var trendingProductsData = [10, 20, 15, 30, 25];
  
    var totalRevenueData = [1000, 2000, 1500, 3000, 2500];
    var totalOrdersData = [50, 60, 70, 80, 90];

    // Create Charts
    var trendingProductsChart = new Chart(document.getElementById("trendingProductsChart"), {
        type: 'bar',
        data: {
            labels: ['Product A', 'Product B', 'Product C', 'Product D', 'Product E'],
            datasets: [{
                label: 'Trending Products',
                data: trendingProductsData,
                backgroundColor: 'rgba(75, 192, 192, 0.2)',
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });

    var totalRevenueChart = new Chart(document.getElementById("totalRevenueChart"), {
        type: 'line',
        data: {
            labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May'],
            datasets: [{
                label: 'Total Revenue',
                data: totalRevenueData,
                fill: false,
                borderColor: 'rgb(75, 192, 192)',
                tension: 0.1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });

    var totalOrdersChart = new Chart(document.getElementById("totalOrdersChart"), {
        type: 'doughnut',
        data: {
            labels: ['Pending', 'Processing', 'Completed'],
            datasets: [{
                label: 'Total Orders',
                data: totalOrdersData,
                backgroundColor: ['rgba(255, 99, 132, 0.2)', 'rgba(54, 162, 235, 0.2)', 'rgba(75, 192, 192, 0.2)'],
                borderColor: ['rgba(255, 99, 132, 1)', 'rgba(54, 162, 235, 1)', 'rgba(75, 192, 192, 1)'],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true
        }
    });
});
});