ngApp.service("batchPriceServices", function ($http) {
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
            url: "/SalesAndDistribution/ProductPrice/AddOrUpdateBatchPrice",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }
    this.LoadProductData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Product/LoadProductdropdownData', { COMPANY_ID: parseInt(comp_id) });
    }
    this.loadBatchWiseStock = function (model) {
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/ProductPrice/loadBatchWiseStock",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
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
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/ProductPrice/BatchPriceLoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetEditDataById = function (id) {
        return $http.post('/SalesAndDistribution/ProductPrice/GetEditDataById', { MST_ID_ENCRYPTED: id });
    }
});

