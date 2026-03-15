ngApp.controller('ngGridCtrl', ['$scope', 'ActivityLogServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, ActivityLogServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

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
            name: 'LOG_DATE', field: 'LOG_DATE', displayName: 'Log Date', enableFiltering: true,
        }
        , {
            name: 'USER_TERMINAL', field: 'USER_TERMINAL', displayName: 'User Terminal', enableFiltering: true
        }
        , {
            name: 'ACTIVITY_TYPE', field: 'ACTIVITY_TYPE', displayName: 'Activity type', enableFiltering: true
        }
        , {
            name: 'ACTIVITY_TABLE', field: 'ACTIVITY_TABLE', displayName: 'Activity table', enableFiltering: true
        }
        , {
            name: 'PAGE_REF', field: 'PAGE_REF', displayName: 'Page_ref', enableFiltering: true
        }
        , {
            name: 'LOCATION', field: 'LOCATION', displayName: 'Location', enableFiltering: true
        }
        , {
            name: 'DTL', field: 'DTL', displayName: 'User DTL', enableFiltering: true
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
        ActivityLogServices.Search($scope.model).then(response => {
            $scope.gridOptionsList.data = response.data;
        })
    }

    $scope.SearchData = (entity) => {
        ActivityLogServices.Search(entity).then(response => {
            $scope.gridOptionsList.data = response.data;
        })
    }


    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'UserLog',
            Action_Name: 'ActivityLog'
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
    //$scope.GetPermissionData();
}]);