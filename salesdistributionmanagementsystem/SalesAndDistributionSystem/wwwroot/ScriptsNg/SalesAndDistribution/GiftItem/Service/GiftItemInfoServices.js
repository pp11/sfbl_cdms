ngApp.service("GiftItemInfoServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/GiftItem/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/GiftItem/GetCompany');
    }

    this.GenerateGiftItemCode = function (comp_id) {
        return $http.post('/SalesAndDistribution/GiftItem/GenerateGiftItemCode', { COMPANY_ID: parseInt(comp_id) });
    }
    

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/GiftItem/AddOrUpdate",
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