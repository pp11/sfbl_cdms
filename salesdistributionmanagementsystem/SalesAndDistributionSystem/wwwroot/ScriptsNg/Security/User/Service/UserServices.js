ngApp.service("UserServices", function ($http) {
    this.LoadData = function () {
        return $http.get('../User/LoadData');
    }

    this.AddOrUpdate = function (model) {
        console.log(model.STATUS);

        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "../User/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: {
                USER_ID: parseInt(model.USER_ID),
                USER_NAME: model.USER_NAME,
                USER_TYPE: model.USER_TYPE,
                EMAIL: model.EMAIL,
                USER_PASSWORD: model.USER_PASSWORD,
                EMPLOYEE_ID: model.EMPLOYEE_ID,
                UNIT_ID: parseInt(model.UNIT_ID),
                COMPANY_ID: parseInt(model.COMPANY_ID),
                STATUS: model.STATUS
            },
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }

    this.GetCompanyList = function () {
        return $http.get('../Company/LoadData');
    }

    this.LoadDefaultPages = function (companyId) {
        return $http.post('../User/LoadDefaultPages', { COMPANY_ID: parseInt(companyId)});
    }

    this.LoadSearchableDefaultPages = function (comp_id, value) {
        
        return $http.post('../User/GetSearchableDefaultPages', { COMPANY_ID: comp_id, MENU_NAME: value });
    }
    this.AddOrUpdateDefaultPage = function (model) {

        return $http.post('../User/AddOrUpdateDefaultPage', { ID: model.ID, COMPANY_ID: parseInt(model.COMPANY_ID), MENU_ID: model.MENU_ID, USER_ID: model.USER_ID });
    }

    this.GetSearchableUsers = function (comp_id,user_name) {
        
        return $http.post("../MenuPermission/GetSearchableUsers", { COMPANY_ID: comp_id, USER_NAME: user_name });
    }
    this.GetEmployees = function (comp_id) {
        
        return $http.post("../User/GetEmployeeWithoutAccount", { COMPANY_ID: comp_id });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Market/GetCompany');
    }

    //im//
    this.getAllCodeAndDropdownListData = function (id) {
        return $http.get('/Security/User/GetAllCodeAndDropdownListData');
    }
    //im//
    this.fetchLoggedInCompanyAndUnit = function () {
        return $http.get('/Security/Company/FetchLoggedInCompanyAndUnit');
    }


    this.ActivateUser = function (Id) {

        return $http.post("../USER/ActivateUser", { USER_ID: Id });
    }
    this.DeactivateUser = function (Id) {

        return $http.post("../USER/DeactivateUser", { USER_ID: Id });
    }
});