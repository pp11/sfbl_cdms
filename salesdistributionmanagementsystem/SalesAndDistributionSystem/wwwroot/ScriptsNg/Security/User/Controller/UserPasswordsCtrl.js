ngApp.controller('ngGridCtrl', ['$scope', 'UserPasswordsService', 'permissionProvider', 'CompanyService', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, UserPasswordsService, permissionProvider, CompanyService, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    $scope.model = {
        PASSWORD: '',
    }
    $scope.show = false;

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("User Ctr"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }

    $scope.onChangePassword = function (event) {
        if (event.key == "Enter") {
            $scope.SaveData();
        }
    }

    $scope.gridOptionsList.columnDefs = [
        {
            name: 'SL', field: 'ROW_NO', enableFiltering: false, width: 50
        },
        {
            name: 'useR_NAME', field: 'useR_NAME', displayName: 'User Name', enableFiltering: true
        }, {
            name: 'email', field: 'email', displayName: 'email', enableFiltering: true
        },
        {
            name: 'useR_PASSWORD', field: 'useR_PASSWORD', displayName: 'Password', enableFiltering: true
        }
        
    ];

    $scope.SaveData = function () {
        if ($scope.model.PASSWORD) {
            UserPasswordsService.GetPasswords($scope.model).then(function (response) {
                console.log(response)
                if (response.data.length > 0) {
                    $scope.gridOptionsList.data = response.data
                    var sl = 0;
                    $scope.gridOptionsList.data.forEach(e => e.ROW_NO = ++sl)
                    $scope.show = true;
                } else {
                    alert("Please Enter Valid Security Password");
                }
            });
        }
        else {
            alert("Please Enter Security Password");
        }
    }

}]);