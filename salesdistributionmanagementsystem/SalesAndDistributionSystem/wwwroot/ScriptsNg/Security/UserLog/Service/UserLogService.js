ngApp.service("UserLogServices", function ($http) {
    this.GetUsers = function () {
        return $http.get('/Security/User/LoadData');
    }
    this.Search = function (entity) {
        return $http.post('/Security/UserLog/Search', entity);
    }
});