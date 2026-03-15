ngApp.service("MiscellaneousIssueService", function ($http) {

    this.GetCompanyId = function () {
        return $http.get('/Inventory/MiscellaneousIssue/GetCompanyId');
    }

    this.GetCompanyName = function () {
        return $http.get('/Inventory/MiscellaneousIssue/GetCompanyName');
    }
    this.GetUnitId = function () {
        return $http.get('/Inventory/MiscellaneousIssue/GetUnitId');
    }
    this.GetUnitName = function () {
        return $http.get('/Inventory/MiscellaneousIssue/GetUnitName');
    }
    this.LoadProductData = function (comp_id) {
        return $http.post('/Inventory/MiscellaneousIssue/LoadProductData', { COMPANY_ID: parseInt(comp_id) });
    }
    //this.LoadSKUBatchData = function (sku_id) {
    //    return $http.post('/Inventory/MiscellaneousIssue/LoadSKUBatchData', { SKU_ID: parseInt(sku_id)  });
    //}

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/Inventory/MiscellaneousIssue/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
    }

    this.LoadData = function (companyId, date_from, date_to) {
        return $http.post('/Inventory/MiscellaneousIssue/LoadData', { COMPANY_ID: parseInt(companyId), DATE_FROM: date_from, DATE_TO: date_to });
    }

    this.GetEditDataById = function (id) {
        return $http.post('/Inventory/MiscellaneousIssue/GetEditDataById', { q: id });
    }

    this.GetProducts = function () {
        return $http.post('/Inventory/MiscellaneousIssue/LoadProductData', {});
    }

    this.GetUnitWiseSkuPrice = function (sku_id, sku_code) {
        return $http.post('/SalesAndDistribution/ProductPrice/UnitWiseSkuPrice', { SKU_ID: parseInt(sku_id), SKU_CODE: sku_code })
    }
});