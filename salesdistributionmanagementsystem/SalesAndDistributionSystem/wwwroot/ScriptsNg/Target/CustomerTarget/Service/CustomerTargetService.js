ngApp.service("CustomerTargetService", function ($http) {
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/ProductPrice/GetCompany');
    }

    this.GetProducts = function () {
        return $http.post('/SalesAndDistribution/Product/LoadDataFromView', {});
    }

    this.LoadMarketDropDownDataData = function (companyId) {
        return $http.post('/SalesAndDistribution/Market/MarketDropDownDataData', { COMPANY_ID: parseInt(companyId) });
    }

    this.loadCustomer = function () {
        return $http.get('/SalesAndDistribution/Customer/LoadActiveCustomerData', {});
    }

    this.AddOrUpdate = function (model) {
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/Target/CustomerTarget/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
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

    this.GetTargetList = function () {
        return $http.get('/Target/CustomerTarget/GetTargetList');
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
})
