ngApp.controller('ngGridCtrl', ['$scope', 'UserLogServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, UserLogServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'

    $scope.model = {
        FROM_DATE: $filter("date")(Date.now(), 'dd/MM/yyyy'),
        TO_DATE: $filter("date")(Date.now(), 'dd/MM/yyyy')
    };

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("User Ctr"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        { name: 'SL', field: 'ROW_NO', enableFiltering: false, width: 40 }
        , { name: 'USER_ID', field: 'USER_ID', visible: false }
        , {
            name: 'USER_NAME', field: 'USER_NAME', displayName: 'User Name', enableFiltering: true, width: '20%'
        }
        , {
            name: 'EMPLOYEE_ID', field: 'EMPLOYEE_ID', displayName: 'Employee Id', enableFiltering: true, width: ' 16%', cellTemplate:
                '<input required="required"  type="text"  ng-model="row.entity.EMPLOYEE_ID"  class="pl-sm" />'
        }
        , {
            name: 'USER_TYPE', field: 'USER_TYPE', displayName: 'User Type', enableFiltering: true, width: '8%'
        }
        , {
            name: 'LOG_DATE', field: 'LOG_DATE', displayName: 'Log Date', enableFiltering: true, width: '8%'
        }
        , {
            name: 'USER_TERMINAL', field: 'USER_TERMINAL', displayName: 'User Terminal', enableFiltering: true,  width: '8%'
        }
        , {
            name: 'ACTIVITY_TYPE', field: 'ACTIVITY_TYPE', displayName: 'Activity type', enableFiltering: true, width: '8%'
        }
        , {
            name: 'ACTIVITY_TABLE', field: 'ACTIVITY_TABLE', displayName: 'Activity table', enableFiltering: true, width: '8%'
        }
        , {
            name: 'PAGE_REF', field: 'PAGE_REF', displayName: 'Page_ref', enableFiltering: true, width: '8%'
        }
        , {
            name: 'LOCATION', field: 'LOCATION', displayName: 'Location', enableFiltering: true, width: '8%'
        }
        , {
            name: 'DTL', field: 'DTL', displayName: 'User DTL', enableFiltering: true, width: '8%'
        }
        //, {
        //    name: 'Actions', displayName: 'Actions', width: '20%', visible: false, enableFiltering: false, enableColumnMenu: false, cellTemplate:
        //        '<div style="margin:1px;">' +
        //        '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
        //        '</div>'
        //},

    ];

    $scope.Users = [];
    $scope.LoadData = () => {
        UserLogServices.GetUsers().then(response => {
            
            $scope.Users = response.data;
        })

        //UserLogServices.LoadData().then(response => {
        //    $scope.gridOptionsList.data = response.data;
        //})

        $scope.SearchData($scope.model);
    }

    $scope.SearchData = (entity) => {
        UserLogServices.Search(entity).then(response => {
            $scope.gridOptionsList.data = response.data;
        })
    }


    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'UserLog',
            Action_Name: 'ViewLogs'
        }
        permissionProvider.GetPermission($scope.permissionReqModel).then(function (data) {
            
            $scope.getPermissions = data.data;
            $scope.model.ADD_PERMISSION = $scope.getPermissions.adD_PERMISSION;
            $scope.model.EDIT_PERMISSION = $scope.getPermissions.ediT_PERMISSION;
            $scope.model.DELETE_PERMISSION = $scope.getPermissions.deletE_PERMISSION;
            $scope.model.LIST_VIEW = $scope.getPermissions.lisT_VIEW;
            $scope.model.DETAIL_VIEW = $scope.getPermissions.detaiL_VIEW;
            $scope.model.DOWNLOAD_PERMISSION = $scope.getPermissions.downloaD_PERMISSION;
            $scope.model.USER_TYPE = $scope.getPermissions.useR_TYPE;

            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    $scope.LoadData();
    $scope.GetPermissionData();
}]);