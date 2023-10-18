

document.addEventListener("DOMContentLoaded", function () {

    fetch('/admin/order/TrendingProducts')
        .then(response => response.json())
        .then(trendingProductsData => {
            drawTrendingProductsChart(trendingProductsData);
        });

    fetch('/admin/order/TotalRevenue')
        .then(response => response.json())
        .then(totalRevenueData => {
            drawTotalRevenueChart(totalRevenueData);
        });

    fetch('/admin/order/TotalOrders')
        .then(response => response.json())
        .then(totalOrdersData => {
            drawTotalOrdersChart(totalOrdersData);
        });
});
function drawTrendingProductsChart(trendingProductsData) {
    var chartData = trendingProductsData.map(item => item.Count);

    var chart = new Chart(document.getElementById("trendingProductsChart"), {
        type: 'bar',
        data: {
            labels: trendingProductsData.map(item => item.ProductId),
            datasets: [{
                label: 'Trending Products',
                data: chartData,
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
}

function drawTotalRevenueChart(totalRevenueData) {
    var chart = new Chart(document.getElementById("totalRevenueChart"), {
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
}

function drawTotalOrdersChart(totalOrdersData) {
    var chart = new Chart(document.getElementById("totalOrdersChart"), {
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
}

