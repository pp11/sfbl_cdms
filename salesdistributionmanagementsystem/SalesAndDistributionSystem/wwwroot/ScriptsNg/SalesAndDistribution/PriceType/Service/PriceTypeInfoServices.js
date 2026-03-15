ngApp.service("PriceTypeInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/PriceType/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/PriceType/GetCompany');
    }

    this.GeneratePriceTypeCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/PriceType/GeneratePriceTypeCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/PriceType/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchablePriceType = function (comp_id, PriceType) {
        return $http.post('/SalesAndDistribution/PriceType/GetSearchablePriceType', { COMPANY_ID: parseInt(comp_id), PRICE_TYPE_NAME: PriceType });
    }
});