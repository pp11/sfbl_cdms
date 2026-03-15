ngApp.service("MarketInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/Market/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Market/GetCompany');
    }

    this.GenerateMarketCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/Market/GenerateMarketCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/Market/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchableMarket = function (comp_id, market) {
        return $http.post('/SalesAndDistribution/Market/GetSearchableMarket', { COMPANY_ID: parseInt(comp_id), MARKET_NAME: market });
    }
});