ngApp.service("DistributionRouteService", function ($http) {
    this.LoadData = function () {
        return $http.post('/SalesAndDistribution/DistributionRoute/LoadData', { });
    }
    this.GetUnit = function () {
        return $http.get('/Security/Company/GetUserUnit');
    }
    this.AddOrUpdate = function (entity) {
        return $http.post(`/SalesAndDistribution/DistributionRoute/AddOrUpdate`, entity)
    }
});