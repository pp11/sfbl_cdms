ngApp.service("MeasuringUnitInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/MeasuringUnit/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/MeasuringUnit/GetCompany');
    }

    this.GenerateMeasuringUnitCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/MeasuringUnit/GenerateMeasuringUnitCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/MeasuringUnit/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchableMeasuringUnit = function (comp_id, MeasuringUnit) {
        return $http.post('/SalesAndDistribution/MeasuringUnit/GetSearchableMeasuringUnit', { COMPANY_ID: parseInt(comp_id), MEASURING_UNIT_NAME: MeasuringUnit });
    }
});