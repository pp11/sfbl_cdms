ngApp.service("PrimaryProductInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/PrimaryProduct/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/PrimaryProduct/GetCompany');
    }

    this.GeneratePrimaryProductCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/PrimaryProduct/GeneratePrimaryProductCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/PrimaryProduct/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchablePrimaryProduct = function (comp_id, PrimaryProduct) {
        return $http.post('/SalesAndDistribution/PrimaryProduct/GetSearchablePrimaryProduct', { COMPANY_ID: parseInt(comp_id), PRIMARY_PRODUCT_NAME: PrimaryProduct });
    }
});