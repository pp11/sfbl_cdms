'use strict';

$(document).ready(function () {
    $.ajax({
        url: '/api/data/chart-sbu-data',
        method: 'GET',
        success: function (response) {
            const data = typeof response === 'string' ? JSON.parse(response) : response;
            const result = {
                labels: data.map(item => item.MONTH_LABEL),
                data: [
                    data.map(item => item.ORDER_MILLION),
                    data.map(item => item.SALES_MILLION),
                    data.map(item => item.COLLECTIONS_MILLION)
                ]
            };
            const labels = result.labels;
            const datasetsRaw = result.data;

            const ctx = document.getElementById("canvas").getContext("2d");

            // Create gradient for Order
            const orderGradient = ctx.createLinearGradient(0, 0, 0, 400);
            orderGradient.addColorStop(0, 'rgba(255, 99, 132, 0.8)');
            orderGradient.addColorStop(1, 'rgba(255, 159, 64, 0.6)');

            // Create gradient for Invoice
            const invoiceGradient = ctx.createLinearGradient(0, 0, 0, 400);
            invoiceGradient.addColorStop(0, 'rgba(75, 192, 192, 0.8)');
            invoiceGradient.addColorStop(1, 'rgba(153, 102, 255, 0.6)');

            const chartData = {
                labels: labels,
                datasets: [
                    {
                        type: 'line',
                        label: 'Collection',
                        borderColor: 'rgb(23, 20, 21)',
                        borderWidth: 2,
                        fill: false,
                        data: datasetsRaw[2],
                        tension: 0.3,
                        pointBackgroundColor: 'rgb(54, 162, 235)'
                    },
                    {
                        type: 'bar',
                        label: 'Order',
                        backgroundColor: orderGradient,
                        borderColor: 'rgba(255, 99, 132, 1)',
                        borderWidth: 1,
                        data: datasetsRaw[0]
                    },
                    {
                        type: 'bar',
                        label: 'Invoice',
                        backgroundColor: invoiceGradient,
                        borderColor: 'rgba(75, 192, 192, 1)',
                        borderWidth: 1,
                        data: datasetsRaw[1]
                    },
                ]
            };

            new Chart(ctx, {
                type: 'bar',
                data: chartData,
                options: {
                    responsive: true,
                    plugins: {
                        tooltip: {
                            mode: 'index',
                            intersect: false
                        },
                        legend: {
                            labels: {
                                color: '#333',
                                font: {
                                    size: 12
                                }
                            }
                        }
                    },
                    scales: {
                        x: {
                            ticks: {
                                color: '#555'
                            },
                            grid: {
                                display: false
                            }
                        },
                        y: {
                            ticks: {
                                color: '#555'
                            },
                            grid: {
                                color: 'rgba(200,200,200,0.2)'
                            }
                        }
                    }
                }
            });
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error:", error);
        }
    });
});
