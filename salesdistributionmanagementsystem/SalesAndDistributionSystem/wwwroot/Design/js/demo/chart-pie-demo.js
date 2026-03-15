// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';

// Pie Chart Example
$(document).ready(function () {
    $.ajax({
        url: '/api/data/chart-pie-data', // API endpoint
        method: 'GET',
        success: function (response) {
            // Extract labels and data from response
            //console.log(response)
            var labels = response.labels;
            var data = response.data;
            var ctx = document.getElementById("myPieChart");
            var myPieChart = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: labels,
                    datasets: [{
                        data: data,
                        backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc'],
                        hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf'],
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
                        display: true, // Display legend to identify small slices
                        position: 'left' // Position the legend to make room for small slices
                    },
                    cutoutPercentage: 80,
                },
            });
            // Dynamically create and populate list items
            var listGroup = $('#list-group');
            listGroup.empty(); // Clear existing items

            for (var i = 0; i < labels.length; i++) {
                var listItem = `
                    <div class="list-group-item d-flex align-items-center justify-content-between small px-0 py-2">
                        <div class="me-3">
                            ${labels[i]} <!-- Removed color circle -->
                        </div>
                        <div class="fw-500 text-dark">${data[i].toFixed(2)}%</div>
                    </div>
                `;
                listGroup.append(listItem);
            }
        },
        error: function (err) {
            console.error('Error fetching data', err);
        }
    });
});