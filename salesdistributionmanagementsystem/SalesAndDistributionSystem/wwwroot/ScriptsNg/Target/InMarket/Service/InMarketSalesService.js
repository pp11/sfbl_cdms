ngApp.service("inmarketServices", function ($http) {
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/ProductPrice/GetCompany');
    }

    this.GetUnit = function () {
        return $http.get('/Security/Company/GetUnit');
    }

    this.AddOrUpdate = function (model) {
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/Target/InMarketSales/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }
    this.AddOrUpdateXlsx = function (model) {
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/Target/InMarketSales/AddOrUpdateXlsx",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

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


    this.LoadProductWeightData = function (Company_Id, Sku_Id, Req_Qty) {
        return $http.get('/Inventory/Requisition/LoadProductWeightData?CompanyId=' + parseInt(Company_Id) + '&Sku_ID=' + parseInt(Sku_Id) + '&ReqQty=' + parseInt(Req_Qty));
    }

    //#region company_information
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Product/GetCompany');
    }
    this.LoadCompanyUnitData = function (companyId) {
        return $http.get('/Security/Company/LoadUnitData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetUnitId = function () {
        return $http.get('/SalesAndDistribution/Report/GetUnitId');
    }
    //#endregion
    this.LoadMarketDropDownDataData = function (companyId) {
        return $http.post('/SalesAndDistribution/Market/MarketDropDownDataData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadProductData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Product/LoadDropDownDataFromView', { COMPANY_ID: parseInt(comp_id) });
    }

    this.LoadData = function (companyId) {
        return $http.post('/Target/InMarketSales/LoadData', { COMPANY_ID: parseInt(companyId) });
    }

    this.GetEditDataById = function (id) {
        return $http.post('/Target/InMarketSales/GetEditDataById', { q: id });
    }
});

