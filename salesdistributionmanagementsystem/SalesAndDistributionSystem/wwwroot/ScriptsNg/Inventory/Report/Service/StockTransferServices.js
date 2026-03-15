ngApp.service("StockTransferServices", function ($http) {
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
    this.GetTransferNo = function (model) {
        return $http.post('/Inventory/Report/GetTransferNo', model);
    }
    this.GetTransferRcvNo = function (model) {
        return $http.post('/Inventory/Report/GetTransferRcvNo', model);
    }
})