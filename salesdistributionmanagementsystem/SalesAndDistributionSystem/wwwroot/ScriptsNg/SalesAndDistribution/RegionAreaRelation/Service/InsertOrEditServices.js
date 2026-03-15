ngApp.service("InsertOrEditServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/RegionAreaRelation/LoadData_Master', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/Security/Company/GetCompany');
    }
    this.GetCompanyName = function () {
        return $http.get('/Security/Company/GetCompanyName');
    }
    this.GetEditDataById = function (id) {

        return $http.post('/SalesAndDistribution/RegionAreaRelation/GetEditDataById', { q: id });
    }
    this.GetExistingRegion = function (id) {

        return $http.post('/SalesAndDistribution/RegionAreaRelation/Existing_Region_Load', { COMPANY_ID: id });
    }
    this.GetExistingArea = function (id) {

        return $http.post('/SalesAndDistribution/RegionAreaRelation/Existing_Area_Load', { COMPANY_ID: id });
    }


    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/RegionAreaRelation/AddOrUpdate",
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