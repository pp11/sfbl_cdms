ngApp.service("BaseProductInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/BaseProduct/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/BaseProduct/GetCompany');
    }

    this.GenerateBaseProductCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/BaseProduct/GenerateBaseProductCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/BaseProduct/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchableBaseProduct = function (comp_id, BaseProduct) {
        return $http.post('/SalesAndDistribution/BaseProduct/GetSearchableBaseProduct', { COMPANY_ID: parseInt(comp_id), BASE_PRODUCT_NAME: BaseProduct });
    }
});