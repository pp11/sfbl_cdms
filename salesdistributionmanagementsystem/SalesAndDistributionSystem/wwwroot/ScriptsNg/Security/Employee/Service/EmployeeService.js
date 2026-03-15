ngApp.service("EmployeeService", function ($http) {
    this.GetEmployeeList = function () {
        return $http.get('../Employee/LoadData');
    }
    this.LoadCompanyUnitData = function (companyId) {
        return $http.get('/Security/Company/LoadUnitData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }

    this.GetCompany = function () {
        return $http.get('/Security/Employee/GetCompany');
    }
    this.AddOrUpdate = function (model) {
        var dataType = 'application/json; charset=utf-8';
        model.UNIT_ID = parseInt(model.UNIT_ID);
        return $http({
            type: 'POST',
            method: 'POST',
            url: "../Employee/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }
    this.GetSearchableDistributor = function (comp_id, Customer) {
        return $http.post('/Security/Employee/GetSearchableDistributor', { COMPANY_ID: parseInt(comp_id), CUSTOMER_NAME: Customer });
    }

    //im//
    this.getAllCodeAndDropdownListData = function (id) {
        return $http.get('/Security/Employee/GetAllCodeAndDropdownListData');
    }
    //im//
    this.fetchLoggedInCompanyAndUnit = function () {
        return $http.get('/Security/Company/FetchLoggedInCompanyAndUnit');
    }
});