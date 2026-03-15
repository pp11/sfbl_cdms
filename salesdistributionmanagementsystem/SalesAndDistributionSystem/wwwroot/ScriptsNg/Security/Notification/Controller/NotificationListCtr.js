ngApp.controller('ngGridCtrl', ['$scope', 'InsertOrEditServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InsertOrEditServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = [];
    $scope.showloader = false;
    /* -------------------- -------------Search Filter Variables----------------------------------------------- */
    $scope.Status = [{ STATUS: 'Active' }, { STATUS: 'InActive' }, { STATUS: 'Complete' }]
    $scope.model.DATE_TO = localStorage.getItem("DATE_TO") == null ? $filter("date")(Date.now(), 'dd/MM/yyyy') : localStorage.getItem("DATE_TO");
    $scope.model.DATE_FROM = localStorage.getItem("DATE_FROM") == null ? $filter("date")(Date.now(), 'dd/MM/yyyy') : localStorage.getItem("DATE_FROM");
    $scope.ClearSearchForm = function () {
        $scope.model.DATE_TO = $filter("date")(Date.now(), 'dd/MM/yyyy');
        $scope.model.DATE_FROM = $filter("date")(Date.now(), 'dd/MM/yyyy');
        localStorage.removeItem("DATE_TO");
        localStorage.removeItem("DATE_FROM");
    };
    $scope.cacheInput = function ($event) {
        localStorage.setItem($event.target.name, $event.target.value);
    }
    /*-----------------------------------------------END----------------------------------------------------------*/


    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Notification List"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }


    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50', cellTemplate: '<div class="ui-grid-cell-contents">{{grid.renderContainers.body.visibleRowCache.indexOf(row)+1}}</div>'
        }, {
            name: 'NOTIFICATION_TITLE', field: 'NOTIFICATION_TITLE', displayName: 'NOTIFICATION TITLE', enableFiltering: true, width: '18%', enableColumnMenu: false,
        }, {
            name: 'NOTIFICATION_BODY', field: 'NOTIFICATION_BODY', displayName: 'NOTIFICATION BODY', width: '48%', enableFiltering: true, enableColumnMenu: false,
        }, {
            name: 'NOTIFICATION_DATE', field: 'NOTIFICATION_DATE', displayName: 'NOTIFICATION DATE', width: '15%', enableFiltering: true, enableColumnMenu: false,
        }, {
            name: 'STATUS', field: 'STATUS', displayName: 'STATUS', enableFiltering: true, width: '8%', enableColumnMenu: false,
        }, {
            name: 'Action', displayName: 'Action', width: '6%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                `<div style="margin:1px;">
                <button style="margin-bottom: 5px;" ng-show="row.entity.STATUS == \'InActive\'"  ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">View</button>
                </div>`
        },
    ];

    

    $scope.DataLoad = function (model) {
        $scope.showLoader = true;
        InsertOrEditServices.GetNotificationList(1, model.DATE_FROM, model.DATE_TO).then(function (data) {
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
        }, function (error) {

            alert(error);

            $scope.showLoader = false;

        });
    }

    $scope.EditData = (entity) => {
        window.location = "/Inventory/DistributionDelivery/Delivery?Id=" + entity.MST_ID_ENCRYPTED;
    }


    $scope.GetPermissionData = function () {
        $scope.showLoader = true;

        $scope.permissionReqModel = {
            Controller_Name: 'Notification',
            Action_Name: 'List'
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
    $scope.GetPermissionData();
    $scope.DataLoad($scope.model);
}]);

