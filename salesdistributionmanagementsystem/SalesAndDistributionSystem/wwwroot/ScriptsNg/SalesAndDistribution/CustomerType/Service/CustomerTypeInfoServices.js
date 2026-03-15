ngApp.service("CustomerTypeInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/CustomerType/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/CustomerType/GetCompany');
    }

    this.GenerateCustomerTypeCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/CustomerType/GenerateCustomerTypeCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/CustomerType/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchableCustomerType = function (comp_id, CustomerType) {
        return $http.post('/SalesAndDistribution/CustomerType/GetSearchableCustomerType', { COMPANY_ID: parseInt(comp_id), CUSTOMER_TYPE_NAME: CustomerType });
    }
});