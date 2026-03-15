ngApp.service("SkuCommissionService", function ($http) {

    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Area/GetCompany');
    }

    this.GetUnit = function () {
        return $http.get('/SalesAndDistribution/SalesOrder/GetUnit');
    }
    this.LoadUnitData = function (companyId) {
        return $http.get('/Security/Company/LoadUnitData', { COMPANY_ID: parseInt(companyId) });
    }

    this.GetProducts = function () {
        return $http.post('/SalesAndDistribution/Product/LoadDataFromView', {});
    }

    //this.GetCustomer = function (model) {
    //    return $http.post('/SalesAndDistribution/SkuCommission/GetCustomer', model)
    //}

    this.GetCustomer = function (model) {
        return $http.post('/SalesAndDistribution/SkuCommission/GetCustomerMarketwise', model)
    }

    this.LoadData = function () {
        return $http.get('/SalesAndDistribution/SkuCommission/LoadData');
    }

    this.GetDetails = function (id) {
        return $http.get('/SalesAndDistribution/SkuCommission/GetDetails?id=' + id);
    }

    this.Process = function (id) {
        return $http.get('/SalesAndDistribution/SkuCommission/Process?id=' + id);
    }

    this.Add = function (model) {
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/SkuCommission/Add",
            dataType: 'json',
            contentType: dataType,
            data: model,
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
    }

    this.Update = function (model) {
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/SkuCommission/Update",
            dataType: 'json',
            contentType: dataType,
            data: model,
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
    }

    this.DeleteMst = function (id) {
        return $http.get('/SalesAndDistribution/SkuCommission/DeleteMst?id=' + id);
    }

    this.DeleteDtl = function (id) {
        return $http.get('/SalesAndDistribution/SkuCommission/DeleteDtl?id=' + id);
    }

    //----------------------
    //--------------- Customer Info Report ---------------
    this.GetDivitionToMarketRelation = function (row_no) { //row_num : DIVISION_ID 
        return $http.post('/SalesAndDistribution/Market/GetDivitionToMarketRelation', { ROW_NO: parseInt(row_no) });
    }
    this.GetSearchableCustomer = function (comp_id, Customer) {
        return $http.post('/SalesAndDistribution/Customer/GetSearchableCustomer', { COMPANY_ID: parseInt(comp_id), CUSTOMER_NAME: Customer });
    }
    this.GetMarketWiseCustomers = function (Market_Codes) {
        return $http.post('/SalesAndDistribution/SkuCommission/GetMarketWiseCustomers', { Market_Code: Market_Codes });
    }
    this.GetComissionDoneCustomers = function (Sku_Code) {
        
        return $http.get('/SalesAndDistribution/SkuCommission/GetComissionDoneCustomers?Sku_Code=' + Sku_Code, {});
    }

    this.LoadCustomerTypeData = function (companyId) {
        return $http.post('/SalesAndDistribution/CustomerType/LoadData', { COMPANY_ID: parseInt(companyId) });

    }
    
});