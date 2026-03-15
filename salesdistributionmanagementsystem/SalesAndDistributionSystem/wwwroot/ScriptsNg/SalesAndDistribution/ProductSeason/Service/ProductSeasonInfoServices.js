ngApp.service("ProductSeasonInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/ProductSeason/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/ProductSeason/GetCompany');
    }

    this.GenerateProductSeasonCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/ProductSeason/GenerateProductSeasonCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/ProductSeason/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchableProductSeason = function (comp_id, ProductSeason) {
        return $http.post('/SalesAndDistribution/ProductSeason/GetSearchableProductSeason', { COMPANY_ID: parseInt(comp_id), PRODUCT_SEASON_NAME: ProductSeason });
    }
});