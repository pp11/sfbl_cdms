ngApp.service("InsertOrEditServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/AreaTerritoryRelation/LoadData_Master', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/Security/Company/GetCompany');
    }
    this.GetCompanyName = function () {
        return $http.get('/Security/Company/GetCompanyName');
    }

    this.GenerateAreaCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/Area/GenerateAreaCode', { COMPANY_ID: parseInt(comp_id)});
    }
    this.GetEditDataById = function (id) {

        return $http.post('/SalesAndDistribution/AreaTerritoryRelation/GetEditDataById', { q: id });
    }
    this.GetExistingArea = function (id) {

        return $http.post('/SalesAndDistribution/AreaTerritoryRelation/Existing_Area_Load', { COMPANY_ID: id });
    }
    this.GetExistingTerritory = function (id) {

        return $http.post('/SalesAndDistribution/AreaTerritoryRelation/Existing_Territory_Load', { COMPANY_ID: id });
    }


    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/AreaTerritoryRelation/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
});