ngApp.service("ProductInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/Product/LoadData', { COMPANY_ID: parseInt(companyId) });
    }

    this.LoadProductPrimaryData = function (companyId) {
        return $http.post('/SalesAndDistribution/Product/LoadProductPrimaryData', { COMPANY_ID: parseInt(companyId) });
    }

    this.GetCompany = function () {
        return $http.get('/Security/Company/GetCompany');
    }
    this.GetCompanyName = function () {
        return $http.get('/Security/Company/GetCompanyName');
    }
    this.GenerateProductCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/Product/GenerateProductCode', { COMPANY_ID: parseInt(comp_id) });
    }
    
    this.LoadCompanyUnitData = function (companyId) {
        return $http.get('/Security/Company/LoadUnitData', { COMPANY_ID: parseInt(companyId) });
    }
    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/Product/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.LoadGroupData = function (companyId) {
        return $http.post('/SalesAndDistribution/Group/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
  
    this.LoadPackSizeData = function (companyId) {
        //return $http.post('/SalesAndDistribution/PackSize/LoadData', { COMPANY_ID: parseInt(companyId) });
        return $http.get('/SalesAndDistribution/Product/LoadPackSizeData');
    }

    this.LoadSkuCodeData = function (companyId) {
        return $http.get('/SalesAndDistribution/Product/LoadSkuCodeData');
    }
    this.LoadProductSegmentInfo = function () {
        return $http.get('/SalesAndDistribution/Product/LoadProductSegmentInfo');
    }
    this.LoadPrimaryProductData = function (companyId) {
        return $http.post('/SalesAndDistribution/PrimaryProduct/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadBaseProductData = function (companyId) {
        return $http.post('/SalesAndDistribution/BaseProduct/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadBrandData = function (companyId) {
        return $http.post('/SalesAndDistribution/Brand/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadCategoryData = function (companyId) {
        return $http.post('/SalesAndDistribution/Category/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadProductSeasonData = function (companyId) {
        return $http.post('/SalesAndDistribution/ProductSeason/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadProductTypeData = function (companyId) {
        return $http.post('/SalesAndDistribution/ProductType/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadStorageData = function (companyId) {
        return $http.post('/SalesAndDistribution/Storage/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadUnitData = function (companyId) {
        return $http.post('/SalesAndDistribution/MeasuringUnit/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetSearchableProduct = function (comp_id, Product) {
        return $http.post('/SalesAndDistribution/Product/GetSearchableProduct', { COMPANY_ID: parseInt(comp_id), SKU_NAME: Product });
    }
    this.LoadSKU_DEPOTData = function (comp_id, Product) {
        return $http.get('/SalesAndDistribution/Product/LoadSKU_DEPOTData', {});
    }
  
    this.AddSkuDepotRelation = function (model) {

        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/Product/AddSkuDepotRelation",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }
    this.DeleteSkuDepotRelation = function (sku_depo_id) {
        return $http.get('/SalesAndDistribution/Product/DeleteSkuDepotRelation', { sku_depo_id: parseInt(sku_depo_id) });
    }
});