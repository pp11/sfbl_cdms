ngApp.service("UserPasswordsService", function ($http) {
    this.GetPasswords = function (model) {
        return $http.post('/security/user/GetPasswords?password='+ model.PASSWORD);
    }
});