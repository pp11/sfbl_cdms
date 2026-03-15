ngApp.service("GroupInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/Group/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Group/GetCompany');
    }

    this.GenerateGroupCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/Group/GenerateGroupCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/Group/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchableGroup = function (comp_id, Group) {
        return $http.post('/SalesAndDistribution/Group/GetSearchableGroup', { COMPANY_ID: parseInt(comp_id), GROUP_NAME: Group });
    }
});