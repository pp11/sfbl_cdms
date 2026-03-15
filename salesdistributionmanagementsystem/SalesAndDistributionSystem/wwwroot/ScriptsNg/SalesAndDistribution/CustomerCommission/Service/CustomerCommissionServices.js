ngApp.service("CustomerCommissionServices", function ($http) {
    
    this.GetCompanyId = function () {
        return $http.get('/SalesAndDistribution/CustomerCommission/GetCompanyId');
    }

    this.GetCompanyName = function () {
        return $http.get('/SalesAndDistribution/CustomerCommission/GetCompanyName');
    }

    this.LoadCustomerData = function (companyId) {
       return $http.get('/SalesAndDistribution/Customer/LoadActiveCustomerData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadCustomerDropdownData = function (companyId) {      
        return $http.get('/SalesAndDistribution/Customer/LoadCustomerDropdownData', { COMPANY_ID: parseInt(companyId) });
    }

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/CustomerCommission/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }
    this.SearchData = function (companyId) {
        return $http.post('/SalesAndDistribution/CustomerCommission/SearchData', { COMPANY_ID: parseInt(companyId) });
    }
});