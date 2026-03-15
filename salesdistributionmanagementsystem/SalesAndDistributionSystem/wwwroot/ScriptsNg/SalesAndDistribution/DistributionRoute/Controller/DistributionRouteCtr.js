ngApp.controller('ngGridCtrl', ['$scope', 'DistributionRouteService', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, DistributionRouteService, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'
    var defaultData = { COMPANY_ID: 0, DIST_ROUTE_NAME: '', STATUS: 'Active', REMARKS: '' }
    $scope.model = { ...defaultData };
    $scope.showLoader = false;
    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.statusList = ["Active", "Inactive"];
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration(" Area Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        //, { name: 'DIST_ROUTE_ID', field: 'DIST_ROUTE_ID', visible: false }
        , { name: 'COMPANY_NAME', field: 'COMPANY_NAME', displayName: "Company", visible: true }
        , { name: 'DIST_ROUTE_NAME', field: 'DIST_ROUTE_NAME', displayName: "Route Name", visible: true }
        , { name: 'STATUS', field: 'STATUS', displayName: "Status", visible: true }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        }
    ]


    $scope.LoadData = function () {
        DistributionRouteService.GetUnit().then(function (response) {
            let company = response.data[0];
            $scope.model.COMPANY_NAME = company.COMPANY_NAME
            $scope.model.COMPANY_ID = company.COMPANY_ID
        }, function (error) {
            
        });

        DistributionRouteService.LoadData().then((response) => {
            
            $scope.gridOptionsList.data = response.data[0];
        }, error => {
            
        })
    }

    $scope.LoadData();

    $scope.SaveData = function () {
        if ($scope.model.DIST_ROUTE_NAME != null && $scope.model.DIST_ROUTE_NAME != "") {
            DistributionRouteService.AddOrUpdate($scope.model).then((response) => {
                notificationservice.Notification(response.data.status, 1, 'Data Save Successfully !!');
                if (response.data.status == "1") {
                    $scope.LoadData();
                }
            })
        }
    }

    $scope.EditData = function (entity) {
        $scope.model.DIST_ROUTE_ID = entity.DIST_ROUTE_ID;
        $scope.model.DIST_ROUTE_NAME = entity.DIST_ROUTE_NAME;
        $scope.model.STATUS = entity.STATUS;
        $scope.model.REMARKS = entity.REMARKS;
    }

    $scope.ClearForm = function () {
        
        $scope.model = { ...$scope.model, ...defaultData };
    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'DistributionRoute',
            Action_Name: 'Route'
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