ngApp.service("CategoryInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/Category/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Category/GetCompany');
    }

    this.GenerateCategoryCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/Category/GenerateCategoryCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/Category/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchableCategory = function (comp_id, Category) {
        return $http.post('/SalesAndDistribution/Category/GetSearchableCategory', { COMPANY_ID: parseInt(comp_id), CATEGORY_NAME: Category });
    }
});