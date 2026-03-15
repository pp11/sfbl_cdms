ngApp.service("CompanyService", function ($http) {
    this.GetCompanyList = function () {
        return $http.get('../Company/LoadData');
    }
  
    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "../Company/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }
    this.GetUnitList = function () {
        return $http.get('../Company/LoadUnitData');
    }

    this.AddOrUpdateUnit = function (model) {
        
        return $http.post("../Company/AddOrUpdateUnit", { ID : parseInt(model.ID), UNIT_ID: parseInt(model.UNIT_ID), COMPANY_ID: parseInt(model.COMPANY_ID), UNIT_NAME: model.UNIT_NAME, UNIT_SHORT_NAME: model.UNIT_SHORT_NAME, UNIT_TYPE: model.UNIT_TYPE, UNIT_ADDRESS1: model.UNIT_ADDRESS1, UNIT_ADDRESS2: model.UNIT_ADDRESS2 });
    }

    this.ActivateUnit = function (model) {
        
        return $http.post("../Company/ActivateUnit", { ID: parseInt(model)  });
    }
    this.DeactivateUnit = function (model) {
        
        return $http.post("../Company/DeactivateUnit", { ID: parseInt(model) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/Market/GetCompany');
    }
});