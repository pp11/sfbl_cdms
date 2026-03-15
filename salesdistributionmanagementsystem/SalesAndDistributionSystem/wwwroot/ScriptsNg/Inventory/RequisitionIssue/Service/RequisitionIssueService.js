ngApp.service("RequisitionIssueServices", function ($http) {


    this.GetCompany = function () {
        return $http.get('/Security/Company/GetCompany');
    }

    this.GetUnit = function () {
        return $http.get('/Security/Company/GetUnit');
    }


    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/Inventory/RequisitionIssue/AddOrUpdate",
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
    this.LoadProductWeightData = function (Company_Id, Sku_Id, Req_Qty) {
        return $http.get('/Inventory/Requisition/LoadProductWeightData?CompanyId=' + parseInt(Company_Id) + '&Sku_ID=' + parseInt(Sku_Id) + '&ReqQty=' + parseInt(Req_Qty));
    }
    this.LoadRequisitionData = function (companyId) {
        return $http.post('/Inventory/Requisition/LoadDataForIssue', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetSearchableProduct = function (companyId, Sku_Name) {
        return $http.post('/SalesAndDistribution/Product/GetSearchableProduct', { COMPANY_ID: parseInt(companyId), SKU_NAME: Sku_Name });
    }
    this.GetEditDataById = function (id) {
        return $http.post('/Inventory/Requisition/GetEditDataById', { q: id });
    }

    this.LoadProductData = function (comp_id, unit_id) {
        return $http.get('/Inventory/RequisitionIssue/LoadProductData?CompanyId=' + parseInt(comp_id) + '&UnitId=' + parseInt(unit_id));
    }
    this.LoadExistingSkuData = function (companyId, customerId) {
        
        
        return $http.get('/SalesAndDistribution/PriceInfo/GetCustomerExistingSKUData?CompanyId=' + parseInt(companyId) + '&CustomerId=' + parseInt(customerId));
    }


    this.LoadData = function (companyId) {
        return $http.post('/Inventory/RequisitionIssue/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetEditDataById = function (id) {
        return $http.post('/Inventory/RequisitionIssue/GetEditDataById', { q: id });
    }
    this.GetRequisitionEditDataById = function (id) {
        return $http.post('/Inventory/Requisition/GetEditDataById', { q: id });
    }

    this.GetUnitWiseSkuPrice = function (sku_id, sku_code) {
        return $http.post('/SalesAndDistribution/ProductPrice/UnitWiseSkuPrice', { SKU_ID: parseInt(sku_id), SKU_CODE: sku_code })
    }
});