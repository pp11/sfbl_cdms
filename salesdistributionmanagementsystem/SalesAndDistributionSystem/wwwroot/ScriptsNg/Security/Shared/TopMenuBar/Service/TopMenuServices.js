ngApp.service("TopMenuServices", function ($http) {
    this.GetUserInfo = function () {
        return $http.get('/Security/MenuPermission/GetUserInfo');
    }
  
    
    this.SearcableMenuLoad = function (value) {

        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/Security/MenuPermission/SearchableMenuLoad",
            dataType: 'json',
            contentType: dataType,
            data: { MENU_NAME: value },
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }

});