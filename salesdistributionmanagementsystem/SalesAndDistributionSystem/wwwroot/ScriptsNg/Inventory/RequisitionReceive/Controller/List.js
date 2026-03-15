ngApp.controller('ngGridCtrl', ['$scope', 'RequisitionReceiveService', 'AreaInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, requisitionReceiveService, AreaInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'
    $scope.model = {
        COMPANY_ID: 0,
        MST_ID: 0,
        MST_ID_ENCRYPTED: "",
        REQUISITION_UNIT_ID: 0,
        RECEIVE_UNIT_ID: 0,
        ISSUE_UNIT_ID: 0,
        REQUISITION_NO: ""
        , RECEIVE_DATE: ""
        , ISSUE_NO: ""
        , RECEIVE_NO: ""
        , ISSUE_DATE: ""
        , REQUISITION_AMOUNT: 0
        , RECEIVE_AMOUNT: 0
        , ISSUE_AMOUNT: 0
        , RECEIVE_BY: ""
        , STATUS: ""
        , REMAEKS: ""
        , requisitionRcvDtlList: []
    }
    $scope.gridRequisitionList = (gridregistrationservice.GridRegistration("Requisition"));
    $scope.gridRequisitionList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridRequisitionList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }

        , { name: 'MST_ID', field: 'MST_ID', visible: false }

        , {
            name: 'REQUISITION_NO', field: 'REQUISITION_NO', displayName: 'Requisition No', enableFiltering: true, width: '15%'
        }
        , {
            name: 'RECEIVE_UNIT_NAME', field: 'RECEIVE_UNIT_NAME', displayName: 'Receive Unit Name', enableFiltering: true, width: '23%'
        }
        , {
            name: 'RECEIVE_DATE', field: 'RECEIVE_DATE', displayName: 'Receive Date', enableFiltering: true, width: '20%'
        }
        , {
            name: 'ISSUE_AMOUNT', field: 'ISSUE_AMOUNT', displayName: 'Issue Amount', enableFiltering: true, width: '12%'
        }
        , {
            name: 'RECEIVE_AMOUNT', field: 'RECEIVE_AMOUNT', displayName: 'Receive Amount', enableFiltering: true, width: '15%'
        }

        , {
            name: 'Action', displayName: 'Action', width: '10%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        }

    ];

    $scope.EditData = function (entity) {
        window.location = "/Inventory/RequisitionReceive/RequisitionRcv?Id=" + entity.MST_ID_ENCRYPTED;
    }

    $scope.DataLoad = function (companyId, customerId) {
        $scope.showLoader = true;

        requisitionReceiveService.LoadData(companyId).then(function (data) {
            $scope.gridRequisitionList.data = data.data;
            $scope.showLoader = false;
            //$scope.model.COMPANY_SEARCH_ID = companyId;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;
        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        requisitionReceiveService.GetCompany().then(function (data) {
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