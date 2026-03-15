ngApp.service("AdjustmentInfoServices", function ($http) {


    this.GetCompanyName = function () {
        return $http.get('/SalesAndDistribution/AdjustmentInfo/GetCompanyName');
    }
    this.GetCompanyId = function () {
        return $http.get('/SalesAndDistribution/AdjustmentInfo/GetCompanyId');
    }

    this.InsertOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/AdjustmentInfo/InsertOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
    }

    this.SearchData = function (companyId) {
        return $http.post('/SalesAndDistribution/AdjustmentInfo/SearchData', { COMPANY_ID: parseInt(companyId) });
    }


});