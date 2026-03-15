ngApp.service("BatchUnFreezingServices", function ($http) {

    this.GetCompanyId = function () {
        return $http.get('/Inventory/BatchUnFreezing/GetCompanyId');
    }

    this.GetCompanyName = function () {
        return $http.get('/Inventory/BatchUnFreezing/GetCompanyName');
    }

    this.GetUnitId = function () {
        return $http.get('/Inventory/BatchUnFreezing/GetUnitId');
    }

    this.GetUnitName = function () {
        return $http.get('/Inventory/BatchUnFreezing/GetUnitName');
    }

    this.GetSkuList = function (company_id, unit_id) {

        return $http.post('/Inventory/BatchUnFreezing/GetSkuList', { COMPANY_ID: parseInt(company_id), UNIT_ID: parseInt(unit_id) });
    }
    this.GetBatchList = function (company_id, unit_id, sku_id) {

        return $http.post('/Inventory/BatchUnFreezing/GetBatchList', { COMPANY_ID: parseInt(company_id), UNIT_ID: parseInt(unit_id), SKU_ID: parseInt(sku_id) });
    }


    this.GetDtlData = function (mst_id) {

        return $http.post('/Inventory/BatchUnFreezing/GetDtlData', { MST_ID: parseInt(mst_id)});
    }

    this.GetMstData = function (company_id, unit_id) {

        return $http.post('/Inventory/BatchUnFreezing/GetMstData', { COMPANY_ID: parseInt(company_id), UNIT_ID: parseInt(unit_id) });
    }


    this.InsertOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/Inventory/BatchUnFreezing/InsertOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }


});