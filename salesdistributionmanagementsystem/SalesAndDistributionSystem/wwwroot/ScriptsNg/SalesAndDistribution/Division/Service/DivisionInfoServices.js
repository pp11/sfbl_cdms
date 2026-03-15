ngApp.service("DivisionInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/Division/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Division/GetCompany');
    }

    this.GenerateDivisionCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/Division/GenerateDivisionCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/Division/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchableDivision = function (comp_id, division) {
        return $http.post('/SalesAndDistribution/Division/GetSearchableDivision', { COMPANY_ID: parseInt(comp_id), DIVISION_NAME: division });
    }
});