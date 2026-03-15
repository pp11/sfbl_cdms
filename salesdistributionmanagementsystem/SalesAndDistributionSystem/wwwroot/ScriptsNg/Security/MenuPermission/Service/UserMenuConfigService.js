ngApp.service("UserMenuConfigService", function ($http) {
    this.GetSearchableUsers = function (user_name) {
        return $http.post("../MenuPermission/GetSearchableUsers", { USER_NAME: user_name });
    }
    this.UserMenuConfigSelectionList = function (Id) {
        return $http.post("../MenuPermission/UserMenuConfigSelectionList", { USER_ID: Id });
    }
    this.SaveUserMenuPermission = function (model) {
        return $http.post("../MenuPermission/SaveRoleMenuConfiguration", JSON.stringify(model));
    }

    //-------Role User Configuration----------------

    this.SaveRoleUserConfiguration = function (model) {
        return $http.post("../Role/SaveRoleUserConfiguration", JSON.stringify(model));
    }

    this.RoleUserConfigSelectionList = function (user_id) {
        return $http.post("../Role/RoleUserConfigSelectionList", { USER_ID: user_id });
    }
    //--------------- Central Role User Configuration---------------------------------------------
    this.RoleCentralUserConfigSelectionList = function (user_id) {
        return $http.post("../Role/RoleCentralUserConfigSelectionList", { USER_ID: user_id });
    }

    this.GetSearchableCentralUsers = function (user_name) {
        return $http.post("../MenuPermission/GetSearchableCentralUsers", { USER_NAME: user_name });
    }
    this.SaveCentralRoleUserConfiguration = function (model) {
        return $http.post("../Role/SaveCentralRoleUserConfiguration", JSON.stringify(model));
    }
});