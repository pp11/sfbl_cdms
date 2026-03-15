ngApp.service("MenuMasterServices", function ($http) {
    this.GetMenu = function (companyId) {
        return $http.post('../MenuMaster/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetMenuCetagories = function (companyId) {
        return $http.post('../MenuCategory/LoadData', { COMPANY_ID: parseInt(companyId)});
    }


    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "../MenuMaster/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: { COMPANY_ID: parseInt(model.COMPANY_ID), MENU_ID: model.MENU_ID, MENU_NAME: model.MENU_NAME, ORDER_BY_SLNO: parseInt(model.ORDER_BY_SLNO), PARENT_MENU_ID: parseInt(model.PARENT_MENU_ID), MODULE_ID: parseInt(model.MODULE_ID), CONTROLLER: model.CONTROLLER, ACTION: model.ACTION, AREA: model.AREA, HREF: model.AREA + "/" + model.CONTROLLER + "/" + model.ACTION, MENU_SHOW: model.MENU_SHOW },
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }
    this.ActivateMenu = function (Id) {
        
        return $http.post("../MenuMaster/ActivateMenu", { MENU_ID: Id });
    }
    this.DeactivateMenu = function (Id) {
        
        return $http.post("../MenuMaster/DeactivateMenu", { MENU_ID: Id });
    }
    this.DeleteMenu = function (Id) {
        
        return $http.post("../MenuMaster/Delete", { MENU_ID: Id });
    }
    this.GetCompanyList = function () {
        return $http.get('../Company/LoadData');
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Market/GetCompany');
    }

    //im//
    this.getAllCodeAndDropdownListData = function (id) {
        return $http.get('/Security/MenuMaster/GetAllCodeAndDropdownListData');
    }

    //im//
    this.fetchLoggedInCompanyAndUnit = function () {
        return $http.get('/Security/Company/FetchLoggedInCompanyAndUnit');
    }
});