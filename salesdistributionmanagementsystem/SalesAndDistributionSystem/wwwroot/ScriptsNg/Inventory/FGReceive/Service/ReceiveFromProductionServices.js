ngApp.service("ReceiveFromProductionServices", function ($http) {
    
    this.LoadData = function (companyId, date_from, date_to) {
        return $http.post('/Inventory/FGReceive/LoadData', { COMPANY_ID: parseInt(companyId), DATE_FROM: date_from, DATE_TO: date_to });
    }
    this.LoadUnchekckData = function (companyId) {
        return $http.post('/Inventory/FGReceive/LoadUnchekedData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadUsersByCompanyId = function (companyId) {
        return $http.post('/Security/User/LoadUsersByCompanyId', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/Security/Company/GetCompany');
    }
    this.GetCompanyName = function () {
        return $http.get('/Security/Company/GetCompanyName');
    }

    this.LoadUnitData = function (companyId) {
        return $http.get('/Security/Company/LoadUnitData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetSearchableProductPrice = function (companyId, Sku_Name) {
        return $http.post('/SalesAndDistribution/ProductPrice/GetSearchableProductPrice', { COMPANY_ID: parseInt(companyId), SKU_NAME: Sku_Name });
    }
    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/Inventory/FGReceive/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.LoadFGTransferData = function (Company_ID, Unit_ID) {
        return $http.post('/Inventory/FGReceive/LoadFGTransferData', { COMPANY_ID: parseInt(Company_ID), UNIT_ID: parseInt(Unit_ID) });
    }
    this.LoadProductByProductCode = function (Company_ID, sku_code) {
        return $http.post('/SalesAndDistribution/Product/LoadProductByProductCode', { COMPANY_ID: parseInt(Company_ID), SKU_CODE: sku_code });

    }
    this.GetEditDataById = function (id) {
        return $http.post('/Inventory/FGReceive/GetEditDataById', { RECEIVE_ID_ENCRYPTED : id });
    }
    this.GetUnit = function () {
        return $http.get('/SalesAndDistribution/SalesOrder/GetUnit');
    }
    this.GetLastMrp = function (sku_code) {
        return $http.post('/Inventory/FGReceive/GetLastMrp', { SKU_CODE: sku_code })
    }
});