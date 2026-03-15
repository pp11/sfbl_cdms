ngApp.service("CustomerPriceInfoServices", function ($http) {
    this.GetCompany = function () {
        return $http.get('/Security/Company/GetCompany');
    }
    this.GetCompanyName = function () {
        return $http.get('/Security/Company/GetCompanyName');
    }
    this.AddOrUpdate = function (model) {
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/PriceInfo/AddOrUpdate",
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
    this.LoadProductdropdownData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Product/LoadProductdropdownData', { COMPANY_ID: parseInt(comp_id) });
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
    this.LoadCustomerDataByType = function (companyId, customer_type_id) {
        return $http.post('/SalesAndDistribution/Customer/LoadCustomerDataByType', { COMPANY_ID: parseInt(companyId), CUSTOMER_TYPE_ID: parseInt(customer_type_id) });
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

    this.LoadSKUPriceDtlDataRestrict = function (cust_id, sku_ids, start_date, cust_ids) {
        if (cust_id == "") {
            cust_id = "0"; 
        }
        if (cust_ids == "") {
            cust_ids = [];
        }
        return $http.post('/SalesAndDistribution/PriceInfo/LoadSKUPriceDtlDataRestrict?', { cust_id: parseInt(cust_id), sku_id: sku_ids, start_date: start_date, cust_ids });
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
        return $http.post('/SalesAndDistribution/PriceInfo/LoadData_Master', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetEditDataById = function (id) {

        return $http.post('/SalesAndDistribution/PriceInfo/GetEditDataById', { q: id });
    }
});