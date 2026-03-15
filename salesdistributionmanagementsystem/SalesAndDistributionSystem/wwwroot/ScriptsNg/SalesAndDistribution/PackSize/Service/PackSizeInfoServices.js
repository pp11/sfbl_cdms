ngApp.service("PackSizeInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/PackSize/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/PackSize/GetCompany');
    }

    this.GeneratePackSizeCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/PackSize/GeneratePackSizeCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/PackSize/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchablePackSize = function (comp_id, PackSize) {
        return $http.post('/SalesAndDistribution/PackSize/GetSearchablePackSize', { COMPANY_ID: parseInt(comp_id), PACK_SIZE_NAME: PackSize });
    }
});