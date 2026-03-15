ngApp.service("ReportConfigurationServices", function ($http) {
    this.LoadData = function (companyId) {
        return $http.post('../ReportConfiguration/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetMenu = function (companyId) {
        return $http.post('../MenuMaster/LoadData', { COMPANY_ID: parseInt(companyId)});
    }


    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "../ReportConfiguration/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: { COMPANY_ID: parseInt(model.COMPANY_ID), REPORT_ID: model.REPORT_ID, REPORT_NAME: model.REPORT_NAME, ORDER_BY_SLNO: parseInt(model.ORDER_BY_SLNO), MENU_ID: parseInt(model.MENU_ID), HAS_PREVIEW: model.HAS_PREVIEW, HAS_PDF: model.HAS_PDF, HAS_CSV: model.HAS_CSV, STATUS: model.STATUS, REPORT_TITLE: model.REPORT_TITLE },
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }
    this.ActivateReport = function (Id) {
        
        return $http.post("../ReportConfiguration/ActivateReport", { REPORT_ID: Id });
    }
    this.DeActivateReport = function (Id) {
        
        return $http.post("../ReportConfiguration/DeactivateReport", { REPORT_ID: Id });
    }
  
    this.GetCompanyList = function () {
        return $http.get('../Company/LoadData');
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Market/GetCompany');
    }

    this.GetSearchableRoles = function (role_name, companyId) {
        
        return $http.post("../ReportConfiguration/GetSearchableRoles", { ROLE_NAME: role_name, COMPANY_ID: parseInt(companyId) });
    }
    this.RoleReportConfigSelectionList = function (Id, companyId) {
        
        return $http.post("../ReportConfiguration/RoleReportConfigSelectionList", { ROLE_ID: Id, COMPANY_ID: parseInt(companyId) });
    }
    this.SaveRoleReportPermission = function (model) {
        

        return $http.post("/Security/ReportConfiguration/SaveRoleReportConfiguration", JSON.stringify(model));
    }
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetUnitList = function (value) {
        
        return $http.post('/Security/Company/GetUnitListByCompanyId', { COMPANY_ID: parseInt(value) });
    }

    //User Report Configuration -------------------------------------
    this.GetSearchableUsers = function (user_name) {
        
        return $http.post("/Security/ReportConfiguration/GetSearchableUsers", { USER_NAME: user_name });
    }
    this.UserReportConfigSelectionList = function (Id) {
        
        return $http.post("/Security/ReportConfiguration/UserReportConfigSelectionList", { USER_ID: Id });
    }
    
    this.SaveUserReportConfiguration = function (model) {
        

        return $http.post("/Security/ReportConfiguration/SaveUserReportConfiguration", JSON.stringify(model));
    }

  
    
});