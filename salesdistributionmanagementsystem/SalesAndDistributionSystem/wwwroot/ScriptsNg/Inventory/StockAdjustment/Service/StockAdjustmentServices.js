ngApp.service("StockAdjustmentServices", function ($http) {

    this.GetCompanyId = function () {
        return $http.get('/Inventory/StockAdjustment/GetCompanyId');
    }

    this.GetCompanyName = function () {
        return $http.get('/Inventory/StockAdjustment/GetCompanyName');
    }

    this.GetUnitId = function () {
        return $http.get('/Inventory/StockAdjustment/GetUnitId');
    }

    this.GetUnitName = function () {
        return $http.get('/Inventory/StockAdjustment/GetUnitName');
    }
    this.LoadProductData = function (comp_id) {

        return $http.post('/SalesAndDistribution/ProductBonus/LoadProductData', { COMPANY_ID: parseInt(comp_id) });
    }

    this.GetBatchList = function (company_id, unit_id, sku_id) {

        return $http.post('/Inventory/StockAdjustment/GetBatchList', { COMPANY_ID: parseInt(company_id), UNIT_ID: parseInt(unit_id), SKU_ID: parseInt(sku_id) });
    }


    this.GetSearchData = function (company_id, unit_id) {

        return $http.post('/Inventory/StockAdjustment/GetSearchData', { COMPANY_ID: parseInt(company_id), UNIT_ID: parseInt(unit_id) });
    }

    this.InsertOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/Inventory/StockAdjustment/InsertOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
    }



});