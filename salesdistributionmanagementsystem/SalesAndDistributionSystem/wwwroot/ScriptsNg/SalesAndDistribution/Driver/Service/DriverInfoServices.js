ngApp.service("DriverInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/Driver/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Area/GetCompany');
    }

    this.GetUnit = function () {
        return $http.get('/SalesAndDistribution/SalesOrder/GetUnit');
    }
    this.LoadUnitData = function (companyId) {
        return $http.get('/Security/Company/LoadUnitData', { COMPANY_ID: parseInt(companyId) });
    }
    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/Driver/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchableDriver = function (comp_id, driver) {
        return $http.post('/SalesAndDistribution/Driver/GetSearchableDriver', { COMPANY_ID: parseInt(comp_id), DRIVER_NAME: driver });
    }
});