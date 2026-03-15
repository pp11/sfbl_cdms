ngApp.controller('ngCollectionReverseCtrl', ['$scope', 'uiGridConstants', 'CollectionReverseServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, uiGridConstants, CollectionReverseServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'

    $scope.model = {
        REVERSE_DATE: $filter("date")(Date.now(), 'dd/MM/yyyy')
    }

    $scope.Batches = [];
    

    $scope.OpenModal = function () {
        CollectionReverseServices.GetTransactions($scope.model.BATCH_NO).then(response => {
            $scope.gridBatchList.data = response.data;
            $scope.gridBatchList.data.forEach((e, index) => {
                e.ROW_NUM = index + 1
            })
        })
        $('#BatchListModal').modal('show')
    }
    $scope.ResetData = function () {
        window.location = "/SalesAndDistribution/CollectionReverse/InsertOrEdit";

    }
    $scope.SearchData = function () {
        window.location = "/SalesAndDistribution/CollectionReverse/List";

    }
    $scope.gridBatchList = (gridregistrationservice.GridRegistration("Batch List"));
    $scope.gridBatchList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridBatchList.data = [];

    $scope.gridBatchList.columnDefs = [
        { field: 'ROW_NUM', displayName: 'Sl', width: '60' },
        { field: 'CUSTOMER_CODE', displayName: 'Cust. Code', enableFiltering: true, width: '10%' },
        { field: 'CUSTOMER_NAME', displayName: 'Customer Name', enableFiltering: true, width: '15%' },
        { field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: true, width: '10%' },
        { field: 'BATCH_DATE', displayName: 'Batch Date', enableFiltering: true, width: '10%' },
        { field: 'BATCH_STATUS', displayName: 'Batch Status', enableFiltering: true, width: '10%' },
        { field: 'VOUCHER_NO', displayName: 'Voucher No', enableFiltering: true, width: '10%' },
        { field: 'VOUCHER_DATE', displayName: 'Voucher Date', enableFiltering: true, width: '10%' },
        { field: 'INVOICE_NO', displayName: 'Invoice No', enableFiltering: true, width: '10%' },
        { field: 'COLLECTION_MODE', displayName: 'Collection Mode', enableFiltering: true, width: '10%' },
        { field: 'COLLECTION_AMT', displayName: 'Collection Amount', enableFiltering: true, width: '10%' },
        { field: 'TDS_AMT', displayName: 'TDS Amount', enableFiltering: true, width: '10%' },
        { field: 'MEMO_COST', displayName: 'Memo Cost', enableFiltering: true, width: '10%' },
        { field: 'NET_COLLECTION_AMT', displayName: 'Net Collection Amount', enableFiltering: true, width: '10%' },
        { field: 'UNIT_NAME', displayName: 'Unit Name', enableFiltering: true, width: '10%' },
        { field: 'BANK_NAME', displayName: 'Bank', enableFiltering: true, width: '10%' },
        { field: 'BRANCH_NAME', displayName: 'Branch', enableFiltering: true, width: '10%' },
        { field: 'REMARKS', displayName: 'Remarks', enableFiltering: true, width: '10%' }
    ]

    $scope.gridBatchList.rowTemplate = "<div ng-dblclick=\"grid.appScope.ViewData(row.entity)\" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.ViewData = function (entity) {
        $('#BatchListModal').modal('hide')
        $scope.model = {
            ...$scope.model,
            ...entity
        }
    }

    $scope.SaveData = function () {
        if ($scope.model.CUSTOMER_CODE == undefined || $scope.model.CUSTOMER_CODE == null || $scope.model.CUSTOMER_CODE == "") {
            notificationservice.Notification("Select a Transection!", 1, '');
            return
        }

        $scope.model.BRANCH_ID = parseInt($scope.model.BRANCH_ID)

        CollectionReverseServices.Save($scope.model).then(response => {
            notificationservice.Notification(response.data.status, "1", 'Data Save Successfully !!');
            if (response.data.status == "1") {
                $scope.model = {
                    REVERSE_DATE: $filter("date")(Date.now(), 'dd/MM/yyyy')
                };
                $scope.GetPermissionData();
            }
        })
    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;

        $scope.permissionReqModel = {
            Controller_Name: 'CollectionReverse',
            Action_Name: 'InsertOrEdit'
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

    //$scope.GetPermissionData();
}])