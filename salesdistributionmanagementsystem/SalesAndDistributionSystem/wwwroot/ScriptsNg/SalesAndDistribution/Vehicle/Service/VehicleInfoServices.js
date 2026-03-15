ngApp.service("VehicleInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/Vehicle/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Area/GetCompany');
    }
    this.GetUnit = function () {
        return $http.get('/SalesAndDistribution/SalesOrder/GetUnit');
    }
    this.LoadUnitData = function (companyId) {
        return $http.get('/Security/Company/LoadUnitData', { COMPANY_ID: parseInt(companyId) });
    }
    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/Vehicle/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }

    this.GetDriver = function (companyId) {
        return $http.post('/SalesAndDistribution/Driver/LoadData', { COMPANY_ID: parseInt(companyId) });
    }

    this.GetWeightUnit = function (companyId) {
        return $http.post('/SalesAndDistribution/Vehicle/GetMeasuringUnit', { COMPANY_ID: parseInt(companyId), Measuring_Unit_Type: "Weight"});
    }

    this.GetVolumeUnit = function (companyId) {
        return $http.post('/SalesAndDistribution/Vehicle/GetMeasuringUnit', { COMPANY_ID: parseInt(companyId), Measuring_Unit_Type: "Volume" });
    }
    //this.GetSearchableSupplier = function (comp_id, supplier) {
    //    return $http.post('/SalesAndDistribution/Supplier/GetSearchableSupplier', { COMPANY_ID: parseInt(comp_id), SUPPLIER_NAME: supplier });
    //}
});