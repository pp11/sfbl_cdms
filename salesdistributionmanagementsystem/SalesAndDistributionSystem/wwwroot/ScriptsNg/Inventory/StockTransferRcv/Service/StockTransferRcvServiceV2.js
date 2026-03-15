ngApp.service("StockTransferRcvService", function ($http) {


    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/ProductPrice/GetCompany');
    }



    this.AddOrUpdate = function (model) {
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/Inventory/StockTransferRcv/AddOrUpdate",
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

    this.LoadTransferData = function (companyId) {
        return $http.post('/Inventory/StockTransfer/LoadReceivableTransferdData', { COMPANY_ID: parseInt(companyId) });
    }

    this.LoadData = function (companyId, date_from, date_to) {
        return $http.post('/Inventory/StockTransferRcv/LoadData', { COMPANY_ID: parseInt(companyId), DATE_FROM: date_from, DATE_TO: date_to });
    }

    this.GetEditDataById = function (id) {
        return $http.post('/Inventory/StockTransferRcv/GetEditDataById', { q: id });
    }
    this.GetTransferDtlDataById = function (id) {
        return $http.post('/Inventory/StockTransfer/GetEditDataById', { q: id });
    }
   
    this.GetTransferDtlDataByTransferId = function (id,unit_id) {
        return $http.get('/Inventory/Distribution/GetTransferedProduct?id=' + id + '&unitid=' + unit_id);
    }

    this.LoadDispatchedTransferDtl = function (company_id, trans_rcv_unit_id, dispatch_no, transfer_no) {
        return $http.post('/Inventory/StockTransferRcv/LoadDispatchedTransferDtl', { COMPANY_ID: parseInt(company_id), TRANS_RCV_UNIT_ID: trans_rcv_unit_id.toString(), DISPATCH_NO: dispatch_no, TRANSFER_NO: transfer_no });
    }
    this.LoadDispatchedTransferData = function (companyId) {
        return $http.post('/Inventory/StockTransferRcv/LoadDispatchedTransferData', { COMPANY_ID: parseInt(companyId) });
    }

});