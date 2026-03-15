ngApp.service("ProductTypeInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/ProductType/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/ProductType/GetCompany');
    }

    this.GenerateProductTypeCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/ProductType/GenerateProductTypeCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/ProductType/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchableProductType = function (comp_id, ProductType) {
        return $http.post('/SalesAndDistribution/ProductType/GetSearchableProductType', { COMPANY_ID: parseInt(comp_id), PRODUCT_TYPE_NAME: ProductType });
    }
});