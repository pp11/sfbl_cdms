ngApp.service("StockTransferService", function ($http) {


    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/ProductPrice/GetCompany');
    }

    this.LoadProductWeightData = function (Company_Id, Sku_Id, Req_Qty) {
        return $http.get('/Inventory/Requisition/LoadProductWeightData?CompanyId=' + parseInt(Company_Id) + '&Sku_ID=' + parseInt(Sku_Id) + '&ReqQty=' + parseInt(Req_Qty));
    }

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/Inventory/StockTransfer/AddOrUpdate",
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
    this.GetSearchableProduct = function (companyId, Sku_Name) {
        return $http.post('/SalesAndDistribution/Product/GetSearchableProduct', { COMPANY_ID: parseInt(companyId), SKU_NAME: Sku_Name });
    }
    this.LoadProductData = function (comp_id) {
        return $http.post('/Inventory/StockTransfer/LoadProductData', { COMPANY_ID: parseInt(comp_id) });
    }
    this.LoadExistingSkuData = function (companyId, customerId) {
        
        
        return $http.get('/SalesAndDistribution/PriceInfo/GetCustomerExistingSKUData?CompanyId=' + parseInt(companyId) + '&CustomerId=' + parseInt(customerId));
    }

    this.LoadRcvUnitStock = function (rcvUnitId, skuId) {
        return $http.get('/Inventory/StockTransfer/LoadRcvUnitStock', {
            params: {
                rcvUnitId: parseInt(rcvUnitId),
                skuId: parseInt(skuId)
            }
        });
    };

    this.LoadData = function (companyId, date_from, date_to) {
        return $http.post('/Inventory/StockTransfer/LoadData', { COMPANY_ID: parseInt(companyId), DATE_FROM: date_from, DATE_TO: date_to });
    }
    this.GetEditDataById = function (id) {
        return $http.post('/Inventory/StockTransfer/GetEditDataById', { q: id });
    }
});