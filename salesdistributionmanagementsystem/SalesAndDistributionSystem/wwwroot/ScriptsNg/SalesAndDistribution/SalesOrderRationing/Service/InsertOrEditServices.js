ngApp.service("InsertOrEditServices", function ($http) {
    this.LoadData = function (model) {
        return $http.post('/SalesAndDistribution/SalesOrderRationing/LoadRationingData', {
            DATE_FROM: model.DATE_FROM,
                DATE_TO: model.DATE_TO,
            DIVISION_ID: model.DIVISION_ID,
                REGION_ID: model.REGION_ID,
            AREA_ID: model.AREA_ID,
                TERRITORY_ID: model.TERRITORY_ID,
            CUSTOMER_ID: model.CUSTOMER_ID
        });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/SalesOrder/GetCompany');
    }
    this.GetUnit = function () {
        return $http.get('/SalesAndDistribution/SalesOrder/GetUnit');
    }
   
    this.GetCompanyName = function () {
        return $http.get('/Security/Company/GetCompanyName');
    }
 
    this.LoadInvoiceTypes = function (companyId) {

        return $http.post('/SalesAndDistribution/InvoiceType/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetEditDataById = function (id) {

        return $http.post('/SalesAndDistribution/SalesOrder/GetEditDataById', { q: id });
    }
    this.LoadCustomerData = function (companyId) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadCustomerData', { COMPANY_ID: parseInt(companyId) });
    }

    this.AddOrUpdate = function (model) {
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/SalesOrderRationing/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }

    this.GetDivisions = function (companyId) {
        return $http.post('/SalesAndDistribution/Division/LoadData', { COMPANY_ID: parseInt(companyId) });
    }

    this.GetDivitionToMarketRelation = function (row_no) { //row_num : DIVISION_ID 
        return $http.post('/SalesAndDistribution/Market/GetDivitionToMarketRelation', { ROW_NO: parseInt(row_no) });
    }
});