ngApp.service("InsertOrEditServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/UserAccountRelation/LoadData_Master', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/Security/Company/GetCompany');
    }
    this.GetCompanyName = function () {
        return $http.get('/Security/Company/GetCompanyName');
    }
    this.GenerateUserCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/User/GenerateUserCode', { COMPANY_ID: parseInt(comp_id)});
    }
    this.GetEditDataById = function (id) {

        return $http.post('/SalesAndDistribution/UserAccountRelation/GetEditDataById', { q: id });
    }
    this.GetExistingUser = function (id) {

        return $http.post('/SalesAndDistribution/UserAccountRelation/Existing_User_Load', { COMPANY_ID: id });
    }
    this.GetExistingAccount = function (id) {

        return $http.post('/SalesAndDistribution/UserAccountRelation/Existing_Account_Load', { COMPANY_ID: id });
    }

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/UserAccountRelation/AddOrUpdate",
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