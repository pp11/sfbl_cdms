ngApp.controller('ngGridCtrl', ['$scope', 'DistributionDeliveryServices', 'permissionProvider', 'gridregistrationservice', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, DistributionDeliveryServices, permissionProvider, gridregistrationservice, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'

    $scope.model = {
        DATE_FROM: $filter("date")(Date.now(), 'dd/MM/yyyy'),
        DATE_TO: $filter("date")(Date.now(), 'dd/MM/yyyy')
    };

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
    $scope.Customers = [];
    $scope.distRoutes = [];
    $scope.vehicles = [];

    $scope.GetCustomer = function (routeId) {
        DistributionDeliveryServices.GetCustomer(routeId).then(response => {
            $scope.Customers = response.data;
        })
    }

    $scope.GetRoutes = function () {
        DistributionDeliveryServices.GetDistributionRoutes().then(function (routeResponse) {
            $scope.distRoutes = routeResponse.data[0];
        }, error => {

        })
    }

    $scope.GetVehicle = function () {
        DistributionDeliveryServices.GetVehicles(0).then(function (vehicleRes) {
            $scope.vehicles = vehicleRes.data;
            //$scope.TriggerSelects();
        }, function (error) {

        })
    }
    $scope.GetRoutes();
    $scope.GetVehicle();


    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Delivery List"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }, {
            name: 'DISTRIBUTION_NO', field: 'DISTRIBUTION_NO', displayName: 'Distribution No', enableFiltering: true, width: '25%', enableColumnMenu: false,
        }, {
            name: 'DISTRIBUTION_DATE', field: 'DISTRIBUTION_DATE', displayName: 'Distribution Date', enableFiltering: true, enableColumnMenu: false,
        }, {
            name: 'DIST_ROUTE_NAME', field: 'DIST_ROUTE_NAME', displayName: 'Route', enableFiltering: true, enableColumnMenu: false,
        }, {
            name: 'VEHICLE_NO', field: 'VEHICLE_NO', displayName: 'Vehicle No', enableFiltering: true, enableColumnMenu: false,
        }, {
            name: 'CONFIRMED', field: 'CONFIRMED', displayName: 'CONFIRMED', enableFiltering: true, enableColumnMenu: false,
        },
        {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                `<div style="margin:1px;">
                <button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>
                </div>`
        },
    ]
    $scope.gridOptionsList.data = [];

    $scope.gridOptionsList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.gridOptionsList.enableGridMenu = false;

    $scope.LoadData = () => {
        DistributionDeliveryServices.GetDeliveryList($scope.model).then(response => {
            $scope.gridOptionsList.data = response.data[0];
            $scope.gridOptionsList.data.forEach(e => { e.DISTRIBUTION_DATE = $scope.formatDate(e.DISTRIBUTION_DATE)})
        });
    }


    $scope.LoadFilteredData = () => {
        DistributionDeliveryServices.GetDeliveryList($scope.model).then(response => {
            $scope.gridOptionsList.data = response.data[0];
            $scope.gridOptionsList.data.forEach(e => { e.DISTRIBUTION_DATE = $scope.formatDate(e.DISTRIBUTION_DATE) })
        });
    }

    $scope.EditData = (entity) => {
        window.location = "/Inventory/DistributionDelivery/Delivery?Id=" + entity.MST_ID_ENCRYPTED;
    }

    $scope.LoadData();

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'DistributionDelivery',
            Action_Name: 'DeliveryList'
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