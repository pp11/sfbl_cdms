ngApp.service("ActivityLogServices", function ($http) {
    this.Search = function (entity) {
        return $http.post('/Security/UserLog/Search', entity);
    }
});