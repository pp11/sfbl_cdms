ngApp.service("DepotCustomerRelationServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/DepotCustomerRelation/LoadData_Master', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/Security/Company/GetCompany');
    }
    this.GetCompanyName  = function () {
        return $http.get('/Security/Company/GetCompanyName');
    }
  
    this.GetCustomer = function (id) {
        return $http.get('/SalesAndDistribution/Customer/LoadActiveCustomerData', { COMPANY_ID: id });
    }


    this.GetUnit = function (id) {
        return $http.post('/SalesAndDistribution/DepotCustomerRelation/LoadUnitData', { COMPANY_ID: id });
    }

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/DepotCustomerRelation/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });   
    }

    this.LoadUnitData = function (companyId) {
        return $http.get('/SalesAndDistribution/DepotCustomerRelation/GetUnitByCompanyId', { COMPANY_ID: parseInt(companyId) });
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }

    this.GetEditDataById = function (id) {
        return $http.post('/SalesAndDistribution/DepotCustomerRelation/GetEditDataById', { q: id });
    }

});