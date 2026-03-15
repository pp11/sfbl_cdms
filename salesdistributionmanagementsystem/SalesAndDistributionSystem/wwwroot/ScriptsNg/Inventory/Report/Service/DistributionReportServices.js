ngApp.service("DistributionReportServices", function ($http) {

    this.LoadData = function (companyId) {
        return $http.post('/Inventory/Report/LoadData', { COMPANY_ID: parseInt(companyId) });
    }

    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }

    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Product/GetCompany');
    }

    this.LoadCompanyUnitData = function (companyId) {
        return $http.get('/Security/Company/LoadUnitData', { COMPANY_ID: parseInt(companyId) });
    }

    this.IsReportPermitted = function (reportId) {
        return $http.post('/Inventory/Report/IsReportPermitted', { REPORT_ID: parseInt(reportId) });
    }

    this.GetDistributionNumbers = function (model) {
        model.UNIT_ID = model.UNIT_ID.toString();
        return $http.post('/Inventory/Report/GetDistributionNumbers', model);
    }

    this.GetCustomers = function (model) {
        return $http.post('/Inventory/Report/GetCustomers', model);
    }

    this.LoadCustomerDropdownData = function (companyId) {
        return $http.get('/SalesAndDistribution/Customer/LoadCustomerDropdownData', { COMPANY_ID: parseInt(companyId) });
    }

    this.GetDistributionDeliveryNumbers = function (model) {
        model.UNIT_ID = model.UNIT_ID.toString();
        return $http.post('/Inventory/Report/GetDistributionDeliveryNumbers', model);
    }

    this.GetInvoices = function () {
        return $http.get('/Inventory/DistributionDelivery/GetPendingInvoices');
    }
    this.GetUnitWiseInvoices = function (unit_id) {
        return $http.get('/Inventory/DistributionDelivery/GetPendingInvoicesByUnit?unitId='+ unit_id );
    }
    this.GetUnitId = function () {
        return $http.get('/SalesAndDistribution/Report/GetUnitId');
    }
    this.GetSearchableCustomer = function (comp_id, Customer) {
        return $http.post('/SalesAndDistribution/Customer/GetSearchableCustomer', { COMPANY_ID: parseInt(comp_id), CUSTOMER_NAME: Customer });
    }
});