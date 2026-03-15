ngApp.service("ProductPriceInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/ProductPrice/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/ProductPrice/GetCompany');
    }



    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/ProductPrice/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.LoadUnitData = function (companyId) {
        return $http.get('/Security/Company/LoadUnitData', { COMPANY_ID: parseInt(companyId) });
    }

    this.LoadCustomerData = function (companyId) {
        return $http.get('/Security/Company/LoadCustomerData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetSearchableProduct = function (companyId,Sku_Name) {
        return $http.post('/SalesAndDistribution/Product/GetSearchableProduct', { COMPANY_ID: parseInt(companyId), SKU_NAME: Sku_Name });
    }
    this.GetSearchableProductPrice = function (companyId, Sku_Name) {
        return $http.post('/SalesAndDistribution/ProductPrice/GetSearchableProductPrice', { COMPANY_ID: parseInt(companyId), SKU_NAME: Sku_Name });
    }
    this.LoadProductData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Product/LoadData', { COMPANY_ID: parseInt(comp_id) });
    }
    
});