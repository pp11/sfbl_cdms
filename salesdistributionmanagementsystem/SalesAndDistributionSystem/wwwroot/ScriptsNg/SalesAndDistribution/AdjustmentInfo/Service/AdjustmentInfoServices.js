ngApp.service("AdjustmentInfoServices", function ($http) {

    //this.LoadData = function (companyId) {
    //    return $http.post('/SalesAndDistribution/Area/LoadData', { COMPANY_ID: parseInt(companyId) });
    //}
    this.GetCompanyId = function () {
        return $http.get('/SalesAndDistribution/AdjustmentInfo/GetCompanyId');
    }

    //this.GenerateAreaCode = function (comp_id) {
    //    return $http.post('/SalesAndDistribution/Area/GenerateAreaCode', { COMPANY_ID: parseInt(comp_id) });
    //}


    //this.AddOrUpdate = function (model) {
    //    
    //    var dataType = 'application/json; charset=utf-8';
    //    return $http({
    //        type: 'POST',
    //        method: 'POST',
    //        url: "/SalesAndDistribution/Area/AddOrUpdate",
    //        dataType: 'json',
    //        contentType: dataType,
    //        data: JSON.stringify(model),
    //        headers: { 'Content-Type': 'application/json; charset=utf-8' },
    //    });

    //}

    //this.GetCompanyList = function () {
    //    return $http.get('/Security/Company/LoadData');
    //}
    //this.GetSearchableArea = function (comp_id, area) {
    //    return $http.post('/SalesAndDistribution/Area/GetSearchableArea', { COMPANY_ID: parseInt(comp_id), AREA_NAME: area });
    //}
});