ngApp.service("MenuCategoryServices", function ($http) {
    this.GetMenuCetagories = function (companyId) {
        return $http.post('../MenuCategory/LoadData', { COMPANY_ID: parseInt(companyId) });
    }



    this.AddOrUpdate = function (model) {

        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "../MenuCategory/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: { COMPANY_ID: parseInt(model.COMPANY_ID), MODULE_ID: model.MODULE_ID, MODULE_NAME: model.MODULE_NAME, ORDER_BY_NO: parseInt(model.ORDER_BY_NO) },
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
    this.ActivateMenuCategory = function (Id) {
        
        return $http.post("../MenuCategory/ActivateMenuCategory", { MODULE_ID: Id });
    }
    this.DeactivateMenuCategory = function (Id) {
        
        return $http.post("../MenuCategory/DectivateMenuCategory", { MODULE_ID: Id });
    }
    this.DeleteMenuCategory = function (Id) {
        
        return $http.post("../MenuCategory/Delete", { MODULE_ID: Id });
    }
    this.GetCompanyList = function () {
        return $http.get('../Company/LoadData');
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Market/GetCompany');
    }
  
});