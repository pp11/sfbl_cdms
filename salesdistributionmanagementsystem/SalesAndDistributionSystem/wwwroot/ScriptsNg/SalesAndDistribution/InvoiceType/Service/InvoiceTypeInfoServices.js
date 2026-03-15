ngApp.service("InvoiceTypeInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/InvoiceType/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/InvoiceType/GetCompany');
    }

    this.GenerateInvoiceTypeCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/InvoiceType/GenerateInvoiceTypeCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/InvoiceType/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchableInvoiceType = function (comp_id, InvoiceType) {
        return $http.post('/SalesAndDistribution/InvoiceType/GetSearchableInvoiceType', { COMPANY_ID: parseInt(comp_id), INVOICE_TYPE_NAME: InvoiceType });
    }
});