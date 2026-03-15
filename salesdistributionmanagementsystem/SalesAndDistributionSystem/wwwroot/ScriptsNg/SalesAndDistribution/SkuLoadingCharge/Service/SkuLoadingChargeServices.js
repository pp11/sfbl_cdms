ngApp.service("SkuLoadingChargeServices", function ($http) {

    this.GetCompanyId = function () {
        return $http.get('/SalesAndDistribution/SkuLoadingCharge/GetCompanyId');
    }

    this.GetCompanyName = function () {
        return $http.get('/SalesAndDistribution/SkuLoadingCharge/GetCompanyName');
    }
    this.LoadProductData = function (comp_id) {
        return $http.post('/SalesAndDistribution/ProductBonus/LoadProductData', { COMPANY_ID: parseInt(comp_id) });
    }

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/SkuLoadingCharge/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
    }

    this.SearchData = function (companyId) {
        return $http.post('/SalesAndDistribution/SkuLoadingCharge/SearchData', { COMPANY_ID: parseInt(companyId) });
    }
});