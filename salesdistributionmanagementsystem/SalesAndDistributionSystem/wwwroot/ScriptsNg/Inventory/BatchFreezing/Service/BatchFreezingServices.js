ngApp.service("BatchFreezingServices", function ($http) {

    this.GetCompanyId = function () {
        return $http.get('/Inventory/BatchFreezing/GetCompanyId');
    }

    this.GetCompanyName = function () {
        return $http.get('/Inventory/BatchFreezing/GetCompanyName');
    }

    this.GetUnitId = function () {
        return $http.get('/Inventory/BatchFreezing/GetUnitId');
    }

    this.GetUnitName = function () {
        return $http.get('/Inventory/BatchFreezing/GetUnitName');
    }

    this.GetSkuList = function (company_id, unit_id) {

        return $http.post('/Inventory/BatchFreezing/GetSkuList', { COMPANY_ID: parseInt(company_id), UNIT_ID: parseInt(unit_id) });
    }
    this.GetBatchList = function (company_id, unit_id, sku_id) {

        return $http.post('/Inventory/BatchFreezing/GetBatchList', { COMPANY_ID: parseInt(company_id), UNIT_ID: parseInt(unit_id), SKU_ID: parseInt(sku_id) });
    }


    this.GetDtlData = function (mst_id) {

        return $http.post('/Inventory/BatchFreezing/GetDtlData', { MST_ID: parseInt(mst_id)});
    }

    this.GetMstData = function (company_id, unit_id) {

        return $http.post('/Inventory/BatchFreezing/GetMstData', { COMPANY_ID: parseInt(company_id), UNIT_ID: parseInt(unit_id) });
    }


    this.InsertOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/Inventory/BatchFreezing/InsertOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }


});