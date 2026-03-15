ngApp.service("EmployeeService", function ($http) {
    //im//
    this.GetSearchableMarketWithType = function (companyId, searchKey) {
        return $http.post('/SalesAndDistribution/SalesEmployee/GetSearchableMarketWithType', { COMPANY_ID: parseInt(companyId), SEARCH_KEY: searchKey });
    }
    //im//
    this.getCompanyAndItsUnit = function () {
        return $http.get('/Security/Company/GetCompanyAndItsUnit');
    }
    //im//
    this.fetchLoggedInCompanyAndUnit = function () {
        return $http.get('/Security/Company/FetchLoggedInCompanyAndUnit');
    }
    //im//
    this.getAllCodeAndDropdownListData = function (id) {
        return $http.get('/SalesAndDistribution/SalesEmployee/GetAllCodeAndDropdownListData');
    }
    //im//
    this.getEmployeeList = function (id) {
        return $http.get('/SalesAndDistribution/SalesEmployee/GetEmployeeList');
    }
    //im//
    this.getEmployeeDataById = function (id, code) {
        var url = '/SalesAndDistribution/SalesEmployee/GetEmployeeDataById?id=' + id + '&code=' + code;
        return $http.get(url);
    };
    //im//
    this.AddOrUpdate = function (model) {
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "../SalesEmployee/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
    }
});