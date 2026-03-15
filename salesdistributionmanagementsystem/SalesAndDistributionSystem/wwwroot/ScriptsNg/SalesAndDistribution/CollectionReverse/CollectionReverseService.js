ngApp.service("CollectionReverseServices", function ($http) {

    this.GetTransactions = function (code) {
        return $http.get(`/SalesAndDistribution/CollectionReverse/GetTransactions?batch_no=${code}`);
    };

    this.Save = function (model) {
        return $http.post('/SalesAndDistribution/CollectionReverse/Save', model)
    }

    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/CollectionReverse/LoadData');
    }
    this.GetCompany = function () {
        return $http.get('/Security/Company/GetCompany');
    }
})