ngApp.controller('ngGridCtrl', ['$scope', 'StockTransferRcvService', 'AreaInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, stockTransferRcvService, AreaInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {
        COMPANY_ID: 0,
        MST_ID: 0,
        MST_ID_ENCRYPTED: "",
        TRANS_RCV_NO: "",

        TRANS_RCV_UNIT_ID: "",

        TRANSFER_UNIT_ID: 0,

        TRANSFER_NO: "",
        TRANSFER_DATE: "",
        TRANSFER_AMOUNT: 0,
        TRANS_RCV_AMOUNT: 0

        , TRANS_RCV_BY: ""
        , STATUS: "Active"
        , REMAEKS: ""
        , stockTransferDtlRcvList: []
    }

    /*Search Filter Variables*/
    $scope.Status = [{ STATUS: 'Active' }, { STATUS: 'InActive' }, { STATUS: 'Complete' }]
    $scope.model.DATE_TO = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.model.DATE_FROM = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.ClearSearchForm = function () {
        $scope.model.DATE_TO = $filter("date")(Date.now(), 'dd/MM/yyyy');
        $scope.model.DATE_FROM = $filter("date")(Date.now(), 'dd/MM/yyyy');
    };

    $scope.gridRequisitionList = (gridregistrationservice.GridRegistration("Stock Transfer Receive"));
    $scope.gridRequisitionList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridRequisitionList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }

        , { name: 'MST_ID', field: 'MST_ID', visible: false }

        , {
            name: 'TRANSFER_NO', field: 'TRANSFER_NO', displayName: 'TRANSFER No', enableFiltering: true, width: '20%'
        }
        , {
            name: 'TRANS_RCV_UNIT_NAME', field: 'TRANS_RCV_UNIT_NAME', displayName: 'TRANSFER Unit Name', enableFiltering: true, width: '20%'
        }
        , {
            name: 'TRANS_RCV_DATE', field: 'TRANS_RCV_DATE', displayName: 'Transfer Date', enableFiltering: true, width: '20%'
        }
        , {
            name: 'TRANS_RCV_AMOUNT', field: 'TRANS_RCV_AMOUNT', displayName: 'Receive Amount', enableFiltering: true, width: '20%'
        }


        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        }

    ];

    $scope.EditData = function (entity) {

        window.location = "/Inventory/StockTransferRcv/StockTransferRcv?Id=" + entity.MST_ID_ENCRYPTED;
    }

    $scope.DataLoad = function (model) {
        $scope.showLoader = true;
        stockTransferRcvService.LoadData(model.COMPANY_ID, model.DATE_FROM, model.DATE_TO).then(function (data) {
            $scope.gridRequisitionList.data = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;
        stockTransferRcvService.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseFloat(data.data);
            $interval(function () {
                $('#COMPANY_ID').trigger('change');
            }, 800, 4);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.CompanyLoad();
    $scope.DataLoad($scope.model);
}]);



