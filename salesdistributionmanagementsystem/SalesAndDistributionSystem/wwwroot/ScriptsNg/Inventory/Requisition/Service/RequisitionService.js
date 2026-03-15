ngApp.service("RequisitionServices", function ($http) {


    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/ProductPrice/GetCompany');
    }

    this.GetUnit = function () {
        return $http.get('/Security/Company/GetUnit');
    }

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/Inventory/Requisition/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }

    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.LoadUnitData = function (companyId) {
        return $http.get('/Security/Company/LoadUnitData', { COMPANY_ID: parseInt(companyId) });
    }


    this.LoadCustomerTypeData = function (companyId) {
        return $http.post('/SalesAndDistribution/CustomerType/LoadData', { COMPANY_ID: parseInt(companyId) });

    }
    this.GetSearchableProduct = function (companyId, Sku_Name) {
        return $http.post('/SalesAndDistribution/Product/GetSearchableProduct', { COMPANY_ID: parseInt(companyId), SKU_NAME: Sku_Name });
    }
    this.LoadProductData = function () {
        return $http.get('/Inventory/Requisition/LoadProductData');
    }
    this.LoadProductsDataFiltered = function (comp_id, model) {
        return $http.post('/Inventory/Requisition/GetProductDataFiltered', { COMPANY_ID: parseInt(comp_id), BASE_PRODUCT_ID: model.BASE_PRODUCT_ID, BASE_PRODUCT_ID: model.BASE_PRODUCT_ID, GROUP_ID: model.GROUP_ID, BRAND_ID: model.BRAND_ID, CATEGORY_ID: model.CATEGORY_ID, SKU_ID: model.SKU_ID });
    }
    this.LoadExistingSkuData = function (companyId, customerId) {
        
        
        return $http.get('/SalesAndDistribution/PriceInfo/GetCustomerExistingSKUData?CompanyId=' + parseInt(companyId) + '&CustomerId=' + parseInt(customerId));
    }
    this.LoadGroupData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadGroupData', { COMPANY_ID: parseInt(comp_id) });
    }
    this.LoadBrandData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadBrandData', { COMPANY_ID: parseInt(comp_id) });
    }
    this.LoadData = function (companyId) {
        return $http.post('/Inventory/Requisition/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetEditDataById = function (id) {
        return $http.post('/Inventory/Requisition/GetEditDataById', { q: id });
    }
    this.LoadProductWeightData = function (Company_Id, Sku_Id, Req_Qty) {
        return $http.get('/Inventory/Requisition/LoadProductWeightData?CompanyId=' + parseInt(Company_Id) + '&Sku_ID=' + parseInt(Sku_Id) + '&ReqQty=' + parseInt(Req_Qty));
    }
    this.LoadCategoryData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadCategoryData', { COMPANY_ID: parseInt(comp_id) });
    }
    this.GetSkuPrice = function (sku_id, sku_code) {
        return $http.post('/SalesAndDistribution/ProductPrice/SkuPrice', { SKU_ID: parseInt(sku_id), SKU_CODE: sku_code })
    }

});