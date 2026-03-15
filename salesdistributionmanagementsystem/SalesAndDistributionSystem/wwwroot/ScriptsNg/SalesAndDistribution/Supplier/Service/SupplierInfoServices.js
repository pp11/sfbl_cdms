ngApp.service("SupplierInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/Supplier/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/Security/Company/GetCompany');
    }
    this.GetCompanyName = function () {
        return $http.get('/Security/Company/GetCompanyName');
    }

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/Supplier/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   

    this.GetSearchableSupplier = function (comp_id, supplier) {
        return $http.post('/SalesAndDistribution/Supplier/GetSearchableSupplier', { COMPANY_ID: parseInt(comp_id), SUPPLIER_NAME: supplier });
    }
});