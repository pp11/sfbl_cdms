ngApp.service("BrandInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/Brand/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Brand/GetCompany');
    }

    this.GenerateBrandCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/Brand/GenerateBrandCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/Brand/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchableBrand = function (comp_id, brand) {
        return $http.post('/SalesAndDistribution/Brand/GetSearchableBrand', { COMPANY_ID: parseInt(comp_id), BRAND_NAME: brand });
    }
});