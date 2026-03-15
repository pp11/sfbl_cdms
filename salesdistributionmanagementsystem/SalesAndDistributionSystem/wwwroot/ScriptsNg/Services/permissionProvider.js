ngApp.service("permissionProvider", function ($http) {
    this.GetPermission = function (model) {
        return $http.post("/Security/MenuPermission/GetPermissions", JSON.stringify(model));
    }
});