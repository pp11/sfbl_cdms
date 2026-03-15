ngApp.service("RequisitionRetRcvService", function ($http) {


    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/ProductPrice/GetCompany');
    }



    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/Inventory/RequisitionReturnReceive/AddOrUpdate",
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


    this.LoadCustomerTypeData = function (companyId) {
        return $http.post('/SalesAndDistribution/CustomerType/LoadData', { COMPANY_ID: parseInt(companyId) });

    }
    this.LoadRequisitionData = function (companyId) {
        return $http.post('/Inventory/RequisitionReturn/LoadDataForRcv', { COMPANY_ID: parseInt(companyId) });
    }

    this.GetSearchableProduct = function (companyId, Sku_Name) {
        return $http.post('/SalesAndDistribution/Product/GetSearchableProduct', { COMPANY_ID: parseInt(companyId), SKU_NAME: Sku_Name });
    }
    this.GetEditDataById = function (id) {
        return $http.post('/Inventory/RequisitionReturn/GetEditDataById', { q: id });
    }

    this.LoadProductData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadProductData', { COMPANY_ID: parseInt(comp_id) });
    }
    this.LoadExistingSkuData = function (companyId, customerId) {
        
        
        return $http.get('/SalesAndDistribution/PriceInfo/GetCustomerExistingSKUData?CompanyId=' + parseInt(companyId) + '&CustomerId=' + parseInt(customerId));
    }


    this.LoadData = function (companyId) {
        return $http.post('/Inventory/RequisitionReturnReceive/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetEditDataById = function (id) {
        return $http.post('/Inventory/RequisitionReturnReceive/GetEditDataById', { q: id });
    }
    this.GetRequisitionEditDataById = function (id) {
        return $http.post('/Inventory/RequisitionReturn/GetEditDataById', { q: id });
    }
});