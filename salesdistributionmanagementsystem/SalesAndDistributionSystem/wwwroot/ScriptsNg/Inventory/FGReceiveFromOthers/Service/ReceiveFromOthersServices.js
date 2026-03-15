ngApp.service("ReceiveFromOthersServices", function ($http) {

    this.LoadData = function (companyId, date_from, date_to) {
        return $http.post('/Inventory/FGReceiveFromOthers/LoadData', { COMPANY_ID: parseInt(companyId), DATE_FROM: date_from, DATE_TO: date_to });
    }
    this.LoadUnchekckData = function (companyId) {
        return $http.post('/Inventory/FGReceiveFromOthers/LoadUnchekedData', { COMPANY_ID: parseInt(companyId) });
    }
    this.PendingList = function (companyId, date_from, date_to) {
        return $http.post('/Inventory/FGReceiveFromOthers/LoadUnchekedData', { COMPANY_ID: parseInt(companyId), DATE_FROM: date_from, DATE_TO: date_to });
    }
    this.GetApprovedList = function (companyId) {
        return $http.post('/Inventory/FGReceiveFromOthers/GetApprovedList', { COMPANY_ID: parseInt(companyId) })
    }
    this.LoadUsersByCompanyId = function (companyId) {
        return $http.post('/Security/User/LoadUsersByCompanyId', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/Inventory/FGReceiveFromOthers/GetCompany');
    }

    this.LoadUnitData = function (companyId) {
        return $http.get('/Security/Company/LoadUnitData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetSearchableProductPrice = function (companyId, Sku_Name) {
        return $http.post('/Inventory/ProductPrice/GetSearchableProductPrice', { COMPANY_ID: parseInt(companyId), SKU_NAME: Sku_Name });
    }
    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/Inventory/FGReceiveFromOthers/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }

    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    
    this.Get_Refurbishment_SKU = function (ref_code) {
        return $http.get('/Inventory/FGReceiveFromOthers/Get_Refurbishment_SKU?Ref_code=' + ref_code, {});
    }

    this.Get_Refurbishment_SKU_ALL = function (ref_code) {
        return $http.get('/Inventory/FGReceiveFromOthers/Get_Refurbishment_SKU_ALL?', {});
    }

    this.LoadFGTransferData = function (Company_ID, Unit_ID) {
        return $http.post('/Inventory/FGReceiveFromOthers/LoadFGTransferData', { COMPANY_ID: parseInt(Company_ID), UNIT_ID: parseInt(Unit_ID) });
    }
    this.LoadProductByProductCode = function (Company_ID, sku_code) {
        return $http.post('/Inventory/Product/LoadProductByProductCode', { COMPANY_ID: parseInt(Company_ID), SKU_CODE: sku_code });

    }
    this.GetEditDataById = function (id) {
        return $http.post('/Inventory/FGReceiveFromOthers/GetEditDataById', { RECEIVE_ID_ENCRYPTED: id });
    }

    this.GetSupplier = function () {
        return $http.post('/SalesAndDistribution/Supplier/LoadData', {});
    }

    this.GetSupplierByType = function (type) {
        return $http.post('/SalesAndDistribution/Supplier/GetSupplierByType', { SUPPLIER_TYPE: type });
    }

    this.GetProducts = function () {
        return $http.post('/SalesAndDistribution/Product/LoadDataFromView', {});
    }
    this.GetProductPrice = function (sku_code) {
        return $http.post('/SalesAndDistribution/ProductPrice/GetSearchableProductPrice', { SKU_NAME: sku_code })
    }
    this.GetUnitWiseSkuPrice = function (sku_id, sku_code) {
        return $http.post('/SalesAndDistribution/ProductPrice/UnitWiseSkuPrice', { SKU_ID: parseInt(sku_id), SKU_CODE: sku_code })
    }
    this.GetUnit = function () {
        return $http.get('/SalesAndDistribution/SalesOrder/GetUnit');
    }
    this.GetUnitList = function () {
        return $http.get('/Security/Company/LoadUnitData');
    }
    this.UpdateStatusToCancel = function (id) {
        return $http.get('/Inventory/FGReceiveFromOthers/UpdateStatusToCancel?id='+id);
    }
    
    

    this.GetBatches = function (id, RECEIVE_TYPE) {
        return $http.post('/Inventory/FGReceiveFromOthers/GetBatches', { SKU_ID: parseInt(id), RECEIVE_TYPE: RECEIVE_TYPE });
    }
    this.GetLastMrp = function (sku_code) {
        return $http.post('/Inventory/FGReceive/GetLastMrp', { SKU_CODE: sku_code })
    }
});