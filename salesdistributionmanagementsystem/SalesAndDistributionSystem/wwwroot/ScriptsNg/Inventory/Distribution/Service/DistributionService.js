ngApp.service("DistributionService", function ($http) {


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
            url: "/Inventory/Distribution/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }

    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetVehicleList = function (companyId) {
        return $http.post('/SalesAndDistribution/Vehicle/GetVehicleJsonList', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadUnitData = function (companyId) {
        return $http.get('/Security/Company/LoadUnitData', { COMPANY_ID: parseInt(companyId) });
    }


    this.LoadCustomerTypeData = function (companyId) {
        return $http.post('/SalesAndDistribution/CustomerType/LoadData', { COMPANY_ID: parseInt(companyId) });

    }
    this.LoadRequisitionData = function () {
        return $http.get('/Inventory/Distribution/GetPendingRequisition');
    }
 
    this.LoadStockData = function () {
        return $http.get('/Inventory/Distribution/GetPendingStock');
    }
    this.GetSearchableProduct = function (companyId, Sku_Name) {
        return $http.post('/SalesAndDistribution/Product/GetSearchableProduct', { COMPANY_ID: parseInt(companyId), SKU_NAME: Sku_Name });
    }
    //this.GetEditDataById = function (id) {
    //    return $http.post('/Inventory/Requisition/GetEditDataById', { q: id });
    //}

    this.LoadProductData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadProductData', { COMPANY_ID: parseInt(comp_id) });
    }

    this.LoadShipperData = function (comp_id, Mst_Id) {
        return $http.get('/Inventory/Distribution/LoadShipperDtlData?CompanyId=' + parseInt(comp_id) + '&MstId=' + parseInt(Mst_Id));
    }

    this.LoadBatchData = function (comp_id, Mst_Id) {
        return $http.get('/Inventory/Distribution/LoadDispatchBatchData?CompanyId=' + parseInt(comp_id) + '&MstId=' + parseInt(Mst_Id));
    }
    this.LoadExistingSkuData = function (companyId, customerId) {
        
        
        return $http.get('/SalesAndDistribution/PriceInfo/GetCustomerExistingSKUData?CompanyId=' + parseInt(companyId) + '&CustomerId=' + parseInt(customerId));
    }


    this.LoadData = function (companyId) {
        return $http.post('/Inventory/Distribution/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetEditDataById = function (id) {
        return $http.post('/Inventory/Distribution/GetEditDataById', { q: id });
    }
    this.GetRequisitionEditDataById = function (id) {
        return $http.post('/Inventory/Distribution/GetEditDataById', { q: id });
    }

    this.GetDistributionDetailDataById = function (id, dispatchType) {
        if (dispatchType == "Requisition") {
            return $http.get('/Inventory/Distribution/GetProductsByRequisition?id=' + id);
        }
        else {
            return $http.get('/Inventory/Distribution/GetProductsByStock?id=' + id);
        }
       
    }
  
});