ngApp.service("AdjustmentServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/OrderAdjustment/LoadDebitCreditAdjData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadDataByOrderId = function (companyId, OrderId) {
        return $http.post('/SalesAndDistribution/OrderAdjustment/LoadDataByOrderId', { COMPANY_ID: parseInt(companyId), ORDER_MST_ID: parseInt(OrderId) });
    }
    
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/OrderAdjustment/GetCompany');
    }
  
    this.GetCustomerList = function (companyId) {
        return $http.get('/SalesAndDistribution/Customer/LoadCustomerDropdownData', { COMPANY_ID: parseInt(companyId) });
    }

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/OrderAdjustment/AddOrUpdateDebitCreditAdj",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
    this.PostDebitCreditAdj = function (model) {

        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/OrderAdjustment/PostDebitCreditAdj",
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