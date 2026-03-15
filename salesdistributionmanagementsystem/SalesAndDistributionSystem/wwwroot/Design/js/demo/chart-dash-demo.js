

// Area Chart Example
$(document).ready(function () {
    $.ajax({
        url: '/api/data/chart-dash-data', // API endpoint
        method: 'GET',
        success: function (response) {
            // Extract labels and data from response
            const data = JSON.parse(response);
            // Update the HTML elements with the corresponding data
            document.getElementById('T_M').innerText = 'BDT '+ data[0].T_M+'M';
            document.getElementById('W_O').innerText = 'BDT ' + data[0].W_O + 'M';
            document.getElementById('W_O_P').innerText =  data[0].W_O_P+'%' ;
            document.getElementById('W_I').innerText = 'BDT ' + data[0].W_I + 'M';
            document.getElementById('W_I_P').innerText = data[0].W_I_P + '%';
            document.getElementById('W_R').innerText = 'BDT ' + data[0].W_R + 'M';
            document.getElementById('W_R_P').innerText = data[0].W_R_P + '%';
            document.getElementById('W_G').innerText = 'BDT ' + data[0].W_G + 'M'
            document.getElementById('W_G_P').innerText = data[0].W_G_P + '%';
            //console.log(data[0].T_A)
            //document.getElementById('T_A').classList.add('w-' + data[0].T_A);
            document.getElementById('T_A').style.width = data[0].T_A+'%';


        },
        error: function (err) {
            console.error('Error fetching data', err);
        }
    });
});