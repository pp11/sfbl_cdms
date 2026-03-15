ngApp.service("SalesAndCollectionReportServices", function ($http) {

    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/Report/LoadData', { COMPANY_ID: parseInt(companyId) });
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

    this.GetCustomers = function () {
        return $http.get('/SalesAndDistribution/Customer/LoadActiveCustomerData')
    }

    //this.LoadCustomerDropdownData = async function (companyId) {
    //    //const response = await fetch("/SalesAndDistribution/Customer/LoadCustomerDropdownData?COMPANY_ID=1")
    //    //const data = await response.json()
    //    //return data;
    //      return $http.get('/SalesAndDistribution/Customer/LoadCustomerDropdownData', { COMPANY_ID: parseInt(companyId) });
    //}

    this.LoadCustomerDropdownData = function (companyId) {
        return $http.get('/SalesAndDistribution/Customer/LoadCustomerDropdownData', { COMPANY_ID: parseInt(companyId) });
    }


    this.GetInvoiceNumbers = function (model) {
        model.UNIT_ID = model.UNIT_ID.toString();
        model.CUSTOMER_ID = model.CUSTOMER_ID.toString();
        return $http.post('/SalesAndDistribution/Report/GetReturnInvoiceNumbers', model)
    }
    this.GetUnitId = function () {
        return $http.get('/SalesAndDistribution/Report/GetUnitId');
    }

    this.GetDivitionToMarketRelation = function () {
        return $http.get('/Inventory/Report/GetDivitionToMarketRelation');
    }
});