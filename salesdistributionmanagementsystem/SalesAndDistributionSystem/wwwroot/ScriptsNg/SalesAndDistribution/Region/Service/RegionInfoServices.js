ngApp.service("RegionInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/Region/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Region/GetCompany');
    }

    this.GenerateRigionCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/Region/GenerateRegionCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/Region/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchableRegion = function (comp_id, region) {
        return $http.post('/SalesAndDistribution/Region/GetSearchableRegion', { COMPANY_ID: parseInt(comp_id), REGION_NAME: region });
    }
});