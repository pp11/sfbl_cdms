ngApp.service("StorageInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/Storage/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Storage/GetCompany');
    }

    this.GenerateStorageCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/Storage/GenerateStorageCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/Storage/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchableStorage = function (comp_id, Storage) {
        return $http.post('/SalesAndDistribution/Storage/GetSearchableStorage', { COMPANY_ID: parseInt(comp_id), STORAGE_NAME: Storage });
    }
});