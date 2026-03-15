ngApp.service("CustomerTargetService", function ($http) {
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
    this.GetProducts = function () {
        return $http.post('/SalesAndDistribution/Product/LoadDataFromView', {});
    }

    this.LoadMarketDropDownDataData = function (companyId) {
        return $http.post('/SalesAndDistribution/Market/MarketDropDownDataData', { COMPANY_ID: parseInt(companyId) });
    }

    this.loadCustomer = function () {
        return $http.get('/SalesAndDistribution/Customer/LoadActiveCustomerData', {});
    }

    this.SaveFromExcel = function (model) {
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/Target/CustomerTarget/SaveFromExcel",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
    }
    this.GetEditDataById = function (id) {
        return $http.post('/Target/CustomerTarget/GetEditDataById', { MST_ID_ENCRYPTED: id });
    }

    this.ProcessExcel = function (file) {
        var fd = new FormData();
        fd.append('file', file);

        //var dataType = 'application/json; charset=utf-8';
        return $http.post("/Target/CustomerTarget/ProcessExcel", fd, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        })

        //return $http({
        //    type: 'POST',
        //    method: 'POST',
        //    url: "/Target/CustomerTarget/ProcessExcel",
        //    dataType: 'json',
        //    contentType: dataType,
        //    data: JSON.stringify(model),
        //    headers: { 'Content-Type': 'application/json; charset=utf-8' },
        //});
    }

    //im//
    this.GetTargetList = function (param) {
        return $http({
            method: 'GET',
            url: '/Target/CustomerTarget/GetCustWiseRemainingBnsRpt',
            params: param
        })
    }
    //im//
    this.lockLoadData = function (param) {
        return $http({
            method: 'GET',
            url: '/Target/CustomerTarget/LockCustWiseRemainingBnsRpt',
            params: param
        })
    }

    this.GetSearchableCustomer = function (comp_id, Customer) {
        return $http.post('/SalesAndDistribution/Customer/GetSearchableCustomer', { COMPANY_ID: parseInt(comp_id), CUSTOMER_NAME: Customer });
    }

    this.AddOrUpdate = function (model) {
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/Target/CustomerTarget/AddOrUpdateAdjustment",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
    }

})
