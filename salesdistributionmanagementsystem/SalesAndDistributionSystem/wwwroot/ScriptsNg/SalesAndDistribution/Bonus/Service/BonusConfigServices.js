ngApp.service("BonusConfigServices", function ($http) {
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/Bonus/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadBaseProductData = function (companyId) {
        return $http.post('/SalesAndDistribution/Bonus/LoadBaseProductData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadBrandData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadBrandData', { COMPANY_ID: parseInt(comp_id) });
    }
    this.LoadCategoryData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadCategoryData', { COMPANY_ID: parseInt(comp_id) });
    }
    this.LoadGroupData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadGroupData', { COMPANY_ID: parseInt(comp_id) });
    }
    this.GetCompany = function () {
        return $http.get('/Security/Company/GetCompany');
    }
    this.GetCompanyName = function () {
        return $http.get('/Security/Company/GetCompanyName');
    }


    this.LoadProductData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadProductData', { COMPANY_ID: parseInt(comp_id) });
    }
   
    this.LoadNewBonusNo = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadNewBonusNo', { COMPANY_ID: parseInt(comp_id) });
    }
    this.LoadLocationTypes = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadLocationTypes', { COMPANY_ID: parseInt(comp_id) });
    }
    this.LoadLocationByLocationTypes = function (comp_id, locationType) {
        return $http.post('/SalesAndDistribution/Bonus/LoadLocation_ByLocationType', { COMPANY_ID: parseInt(comp_id), LOCATION_TYPE: locationType });
    }
    this.LoadProductsDataFiltered = function (comp_id, model) {
        return $http.post('/SalesAndDistribution/Bonus/GetProductDataFiltered', { COMPANY_ID: parseInt(comp_id), BASE_PRODUCT_ID: model.BASE_PRODUCT_ID, BASE_PRODUCT_ID: model.BASE_PRODUCT_ID, GROUP_ID: model.GROUP_ID, BRAND_ID: model.BRAND_ID, CATEGORY_ID: model.CATEGORY_ID, SKU_ID: model.SKU_ID });
    }
    this.GetEditDataById = function (id) {

        return $http.post('/SalesAndDistribution/Bonus/GetEditDataById', { q: id });
    }
    this.LoadGiftData = function (companyId) {
        return $http.post('/SalesAndDistribution/GiftItem/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/Bonus/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }

    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }

});
