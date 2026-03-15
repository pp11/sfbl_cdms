ngApp.service("ProductReportServices", function ($http) {
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/Report/LoadData', { COMPANY_ID: parseInt(companyId) });
    }

    this.LoadBrand = function (companyId) {
        return $http.post('/SalesAndDistribution/Brand/LoadData', { COMPANY_ID: parseInt(companyId) });
    }

    this.LoadCategory = function (companyId) {
        return $http.post('/SalesAndDistribution/Category/LoadData', { COMPANY_ID: parseInt(companyId) });
    }

    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Report/GetCompany');
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
    this.LoadGroupData = function (companyId) {
        return $http.post('/SalesAndDistribution/Group/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadPackSizeData = function (companyId) {
        return $http.post('/SalesAndDistribution/PackSize/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadPrimaryProductData = function (companyId) {
        return $http.post('/SalesAndDistribution/PrimaryProduct/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadBaseProductData = function (companyId) {
        return $http.post('/SalesAndDistribution/BaseProduct/LoadData', { COMPANY_ID: parseInt(companyId) });
    }

    this.LoadProductSeasonData = function (companyId) {
        return $http.post('/SalesAndDistribution/ProductSeason/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadProductTypeData = function (companyId) {
        return $http.post('/SalesAndDistribution/ProductType/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadStorageData = function (companyId) {
        return $http.post('/SalesAndDistribution/Storage/LoadData', { COMPANY_ID: parseInt(companyId) });
    }

    this.IsReportPermitted = function (reportId) {
        return $http.post('/SalesAndDistribution/Report/IsReportPermitted', { REPORT_ID: parseInt(reportId) });
    }

    this.LoadSearchableInvoice = function (inv_no, inv_date, oreder_by, invoice_unit_id, company_id) {
        return $http.post('/SalesAndDistribution/SalesInvoice/LoadSearchableInvoiceInDate', { INVOICE_NO: inv_no, INVOICE_DATE: inv_date, q: oreder_by, INVOICE_UNIT_ID: invoice_unit_id, COMPANY_ID: company_id, });
    }
    this.LoadSearchableOrder = function (inv_no, inv_date, oreder_by, invoice_unit_id, company_id) {
        return $http.post('/SalesAndDistribution/SalesInvoice/LoadSearchableOrderInDate', { INVOICE_NO: inv_no, INVOICE_DATE: inv_date, q: oreder_by, INVOICE_UNIT_ID: invoice_unit_id, COMPANY_ID: company_id, });
    }
    this.LoadMaxMinInvoiceInDate = function (inv_date, invoice_unit_id, company_id) {
        return $http.post('/SalesAndDistribution/SalesInvoice/LoadMaxMinInvoiceInDate', { INVOICE_DATE: inv_date, INVOICE_UNIT_ID: invoice_unit_id, COMPANY_ID: company_id, });
    }
    this.LoadMaxMinInvoiceInDate = function (inv_date, invoice_unit_id, company_id) {
        return $http.post('/SalesAndDistribution/SalesInvoice/LoadMaxMinInvoiceInDate', { INVOICE_DATE: inv_date, INVOICE_UNIT_ID: invoice_unit_id, COMPANY_ID: company_id, });
    }
    this.LoadMaxMinOrderInDate = function (inv_date, invoice_unit_id, company_id) {
        return $http.post('/SalesAndDistribution/SalesInvoice/LoadMaxMinOrderInDate', { INVOICE_DATE: inv_date, INVOICE_UNIT_ID: invoice_unit_id, COMPANY_ID: company_id, });
    }

    this.GetSearchableCustomer = function (comp_id, Customer) {
        return $http.post('/SalesAndDistribution/Customer/GetSearchableCustomer', { COMPANY_ID: parseInt(comp_id), CUSTOMER_NAME: Customer });
    }

    this.GetDivisions = function (companyId) {
        return $http.post('/SalesAndDistribution/Division/LoadData', { COMPANY_ID: parseInt(companyId) });
    }

    this.GetRegions = function (companyId) {
        return $http.post('/SalesAndDistribution/Region/LoadData', { COMPANY_ID: parseInt(companyId) });
    }

    this.GetAreas = function (companyId) {
        return $http.post('/SalesAndDistribution/Area/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetTerritories = function (companyId) {
        return $http.post('/SalesAndDistribution/Territory/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetMarkets = function (companyId) {
        return $http.post('/SalesAndDistribution/Market/LoadData', { COMPANY_ID: parseInt(companyId) });
    }

    this.GetDivitionToMarketRelation = function (row_no) { //row_num : DIVISION_ID
        return $http.post('/SalesAndDistribution/Market/GetDivitionToMarketRelation', { ROW_NO: parseInt(row_no) });
    }
    this.GetDistributorId = function () { //row_num : DIVISION_ID
        return $http.get('/SalesAndDistribution/Report/GetDistributorId', {  });
    }
});