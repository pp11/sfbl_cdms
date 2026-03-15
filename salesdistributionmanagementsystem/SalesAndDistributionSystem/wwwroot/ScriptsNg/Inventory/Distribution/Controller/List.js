ngApp.controller('ngGridCtrl', ['$scope', 'DistributionService', 'AreaInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, distributionService, AreaInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'

    $scope.model = {
        COMPANY_ID: 0,
        MST_ID: 0,
        MST_ID_ENCRYPTED: "",
        DISTRIBUTION_NO: "",
        DISTRIBUTION_DATE: "",
        VEHICLE_NO: ""
        , VEHICLE_DESCRIPTION: ""
        , VECHILE_VOLUME: 0
        , VECHILE_WEIGHT: 0
        , DRIVER_ID: 0
        , DISTRIBUTION_BY: ""
        , STATUS: "Active"
        , REMAEKS: ""
        , requisitionIssueDtlList: []
    }
    $scope.gridDistributionList = (gridregistrationservice.GridRegistration("Requisition"));
    $scope.gridDistributionList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridDistributionList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }

        , { name: 'MST_ID', field: 'MST_ID', visible: false }

        , {
            name: 'DISPATCH_NO', field: 'DISPATCH_NO', displayName: 'Distribution No', enableFiltering: true, width: '12%'
        }, {
            name: 'DISPATCH_DATE', field: 'DISPATCH_DATE', displayName: 'Distribution Date', enableFiltering: true, width: '12%'
        }
        , {
            name: 'VEHICLE_NO', field: 'VEHICLE_NO', displayName: 'Vechicle No', enableFiltering: true, width: '11%'
        }
        , {
            name: 'VEHICLE_DESCRIPTION', field: 'VEHICLE_DESCRIPTION', displayName: 'Vehicle Description', enableFiltering: true, width: '10%'
        }, {
            name: 'DRIVER_NAME', field: 'DRIVER_NAME', displayName: 'Driver Name', enableFiltering: true, width: '10%'
        }, {
            name: 'DISPATCH_VOLUME', field: 'DISPATCH_VOLUME', displayName: 'Dispatch Volume', enableFiltering: true, width: '12%'
        }, {
            name: 'DISPATCH_WEIGHT', field: 'DISPATCH_WEIGHT', displayName: 'Dispatch Weight', enableFiltering: true, width: '12%'
        }
        , {
            name: 'DISPATCH_TYPE', field: 'DISPATCH_TYPE', displayName: 'Dispatch By', enableFiltering: true, width: '10%'
        }
        , {
            name: 'Action', displayName: 'Action', width: '10%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">View</button>' +
                '</div>'
        }
    ];

    $scope.EditData = function (entity) {
        window.location = "/Inventory/Distribution/Distribution?Id=" + entity.MST_ID_ENCRYPTED;
    }

    $scope.DataLoad = function (companyId, customerId) {
        $scope.showLoader = true;

        distributionService.LoadData(companyId).then(function (data) {
            $scope.gridDistributionList.data = data.data;
            $scope.showLoader = false;
            //$scope.model.COMPANY_SEARCH_ID = companyId;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;
        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        distributionService.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseFloat(data.data);
            $scope.DataLoad($scope.model.COMPANY_ID);
            $interval(function () {
                $('#COMPANY_ID').trigger('change');
            }, 800, 4);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    //$scope.GetPermissionData = function () {
    //    $scope.showLoader = true;
    //
    //    $scope.permissionReqModel = {
    //        Controller_Name: 'RegionAreaRelation',
    //        Action_Name: 'List'
    //    }
    //    permissionProvider.GetPermission($scope.permissionReqModel).then(function (data) {
    //
    //
    //        $scope.getPermissions = data.data;
    //        $scope.model.ADD_PERMISSION = $scope.getPermissions.adD_PERMISSION;
    //        $scope.model.EDIT_PERMISSION = $scope.getPermissions.ediT_PERMISSION;
    //        $scope.model.DELETE_PERMISSION = $scope.getPermissions.deletE_PERMISSION;
    //        $scope.model.LIST_VIEW = $scope.getPermissions.lisT_VIEW;
    //        $scope.model.DETAIL_VIEW = $scope.getPermissions.detaiL_VIEW;
    //        $scope.model.DOWNLOAD_PERMISSION = $scope.getPermissions.downloaD_PERMISSION;
    //        $scope.model.USER_TYPE = $scope.getPermissions.useR_TYPE;

    //        $scope.showLoader = false;
    //    }, function (error) {
    //        alert(error);

    //        $scope.showLoader = false;

    //    });
    //}

    //$scope.DataLoad(0);
    //$scope.GetPermissionData();
    //$scope.CompaniesLoad();
    $scope.CompanyLoad();
}]);