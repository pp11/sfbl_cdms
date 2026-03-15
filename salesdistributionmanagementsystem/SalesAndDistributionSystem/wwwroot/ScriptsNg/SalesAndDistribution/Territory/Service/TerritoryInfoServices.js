ngApp.service("TerritoryInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/Territory/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Territory/GetCompany');
    }

    this.GenerateTerritoryCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/Territory/GenerateTerritoryCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/Territory/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchableTerritory = function (comp_id, territory) {
        return $http.post('/SalesAndDistribution/Territory/GetSearchableTerritory', { COMPANY_ID: parseInt(comp_id), TERRITORY_NAME: territory });
    }
});