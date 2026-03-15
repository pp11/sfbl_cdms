ngApp.service("CreditInfoServices", function ($http) {


    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/ProductPrice/GetCompany');
    }
    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/CreditInfo/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }
    this.LoadBaseProductData = function (companyId) {
        return $http.post('/SalesAndDistribution/Bonus/LoadBaseProductData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadProductData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadProductData', { COMPANY_ID: parseInt(comp_id) });
    }
    this.LoadBrandData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadBrandData', { COMPANY_ID: parseInt(comp_id) });
    }
    this.LoadCategoryData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadCategoryData', { COMPANY_ID: parseInt(comp_id) });
    }
    this.LoadGroupData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadGroupData', { COMPANY_ID: parseInt(comp_id) });
    }
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.LoadUnitData = function (companyId) {
        return $http.get('/Security/Company/LoadUnitData', { COMPANY_ID: parseInt(companyId) });
    }

    this.LoadCustomerData = function (companyId) {
        return $http.get('/SalesAndDistribution/Customer/LoadActiveCustomerData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadCustomerTypeData = function (companyId) {
        return $http.post('/SalesAndDistribution/CustomerType/LoadData', { COMPANY_ID: parseInt(companyId) });

    }
    this.GetSearchableProduct = function (companyId, Sku_Name) {
        return $http.post('/SalesAndDistribution/Product/GetSearchableProduct', { COMPANY_ID: parseInt(companyId), SKU_NAME: Sku_Name });
    }

    this.LoadExistingSkuData = function (companyId, customerId) {
        
        
        return $http.get('/SalesAndDistribution/PriceInfo/GetCustomerExistingSKUData?CompanyId=' + parseInt(companyId) + '&CustomerId=' + parseInt(customerId));
    }
    this.LoadFilteredProduct = function (model) {

        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/Product/LoadFilteredData",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
    }

    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/CreditInfo/LoadData_Master', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetEditDataById = function (id) {

        return $http.post('/SalesAndDistribution/PriceInfo/GetEditDataById', { q: id });
    }
});