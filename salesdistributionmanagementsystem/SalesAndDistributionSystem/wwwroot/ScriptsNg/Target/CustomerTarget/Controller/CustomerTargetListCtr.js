ngApp.controller('ngGridCtrl', ['$scope', 'CustomerTargetService', 'permissionProvider', 'gridregistrationservice', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, CustomerTargetService, permissionProvider, gridregistrationservice, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'
    $scope.model = [];
    $scope.showloader = false;
    $scope.formatDate = function (date) {
        var d = new Date(date),
            month = '' + (d.getMonth() + 1),
            day = '' + d.getDate(),
            year = d.getFullYear();

        if (month.length < 2)
            month = '0' + month;
        if (day.length < 2)
            day = '0' + day;

        return [day, month, year].join('/');
    }
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Delivery List"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }, {
            name: 'MONTH', field: 'MONTH', displayName: 'Month', enableFiltering: true, width: '15%', enableColumnMenu: false
        }, {
            name: 'YEAR', field: 'YEAR', displayName: 'Year', enableFiltering: true, enableColumnMenu: false, width: '15%'
        }, {
            name: 'CUSTOMER_NAME', field: 'CUSTOMER_NAME', displayName: 'Customer', enableFiltering: true, enableColumnMenu: false,
        },
        , {
            name: 'ENTERED_DATE', field: 'ENTERED_DATE', displayName: 'Entry Date', enableFiltering: true, enableColumnMenu: false, width: '15%'
        }, {
            name: 'Action', displayName: 'Action', width: '10%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                `<div style="margin:1px;">
                <button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>
                </div>`
        },
    ]
    $scope.gridOptionsList.data = [];

    $scope.gridOptionsList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.gridOptionsList.enableGridMenu = false;

    $scope.LoadData = () => {
        CustomerTargetService.GetTargetList().then(response => {
            $scope.gridOptionsList.data = response.data[0];
            $scope.gridOptionsList.data.forEach(e => { e.ENTERED_DATE = $scope.formatDate(e.ENTERED_DATE) })
        });
    }

    $scope.EditData = (entity) => {
        window.location = "/Target/CustomerTarget/CustomerNationalTarget?Id=" + entity.MST_ID_ENCRYPTED;
    }

    $scope.LoadData();

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;

        $scope.permissionReqModel = {
            Controller_Name: 'CustomerTarget',
            Action_Name: 'CustomerNationalTargetList'
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
}])