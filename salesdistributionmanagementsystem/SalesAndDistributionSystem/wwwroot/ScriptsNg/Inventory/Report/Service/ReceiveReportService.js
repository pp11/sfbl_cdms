ngApp.service("ReceiveReportService", function ($http) {
    this.LoadData = function (companyId) {
        return $http.post('/Inventory/Report/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Product/GetCompany');
    }

    this.GetUnitId = function () {
        return $http.get('/SalesAndDistribution/Report/GetUnitId');
    }

    this.LoadCompanyUnitData = function (companyId) {
        return $http.get('/Security/Company/LoadUnitData', { COMPANY_ID: parseInt(companyId) });
    }

    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }

    this.IsReportPermitted = function (reportId) {
        return $http.post('/Inventory/Report/IsReportPermitted', { REPORT_ID: parseInt(reportId) });
    }

    this.GetChallans = function (model) {
        return $http.post('/Inventory/Report/GetChallans', model);
    }

    this.GetGiftChallans = function (model) {
        return $http.post('/Inventory/Report/GetGiftChallans', model);
    }

    this.LoadProductData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Product/LoadData', { COMPANY_ID: parseInt(comp_id) });
    }

    this.GetBatches = function (model) {
        return $http.post('/Inventory/Report/ReceiveBatches', model);
    }

    this.GetGiftItems = function (model) {
        return $http.post('/SalesAndDistribution/GiftItem/LoadData', model);
    }
})