ngApp.service("GiftItemReceiveServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/GiftItemReceiving/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/Security/Company/GetCompany');
    }
    this.GetCompanyName = function () {
        return $http.get('/Security/Company/GetCompanyName');
    }
    this.LoadSupplierData = function (companyId) {
        return $http.post('/SalesAndDistribution/Supplier/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadGiftItemData = function (companyId) {
        return $http.post('/SalesAndDistribution/GiftItem/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.AddOrUpdate = function (model) {
       
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/GiftItemReceiving/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetSearchableGiftItem = function (comp_id, GiftItem) {
        return $http.post('/SalesAndDistribution/GiftItem/GetSearchableGiftItem', { COMPANY_ID: parseInt(comp_id), GIFT_ITEM_NAME: GiftItem });
    }
});