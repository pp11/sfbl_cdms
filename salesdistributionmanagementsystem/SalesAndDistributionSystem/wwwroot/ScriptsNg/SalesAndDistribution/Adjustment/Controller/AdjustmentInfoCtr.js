ngApp.controller('ngAdjustmentInfoCtrl', ['$scope', 'uiGridConstants', 'AdjustmentInfoServices', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, uiGridConstants, AdjustmentInfoServices, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'

    $scope.model = { COMPANY_ID: 0, ADJUSTMENT_ID: 0, ADJUSTMENT_NAME: '', ADJUSTMENT_STATUS: 'Active', REMARKS: '' }


    $scope.isDisableCompanyName = true;
    $scope.isDisableCompanyId = true;
    $scope.isDisableAdjustmentId = true;

    var columnAdjustmentInfo = [

        { name: 'ROW_NO', field: 'ROW_NO', visible: false, displayName: '#', width: '50' },
        { name: 'COMPANY_ID', field: 'COMPANY_ID', displayName: 'Company ID', visible: false },
        { name: 'ADJUSTMENT_ID', field: 'ADJUSTMENT_ID', displayName: 'Adjustmet ID', visible: false, aggregationType: uiGridConstants.aggregationTypes.count, footerCellFilter: 'number:0' },
        { name: 'ADJUSTMENT_NAME', field: 'ADJUSTMENT_NAME', displayName: 'Adjustmet Name' },
        { name: 'ADJUSTMENT_STATUS', field: 'ADJUSTMENT_STATUS', displayName: 'Status' },
        { name: 'REMARKS', field: 'REMARKS', displayName: 'Remark' }
    ];

    $scope.gridAdjustmentInfo = {
        showColumnFooter: false,
        enableFiltering: true,
        enableSorting: true,
        columnDefs: columnAdjustmentInfo,
        enableGridMenu: true,
        enableSelectAll: false,
        exporterCsvFilename: 'Adjustment Info.csv',
        exporterMenuPdf: false,
        rowTemplate: rowTemplate(),
        exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),

        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
        }
    };


    function rowTemplate() {
        return '<div ng-dblclick="grid.appScope.rowDblClickComp(row)" >' +
            '  <div ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ng-class="{ \'ui-grid-row-header-cell\': col.isRowHeader }"  ui-grid-cell></div></div>';
    }
    $scope.rowDblClickComp = function (row) {

        $scope.model.ADJUSTMENT_ID = row.entity.ADJUSTMENT_ID;
        $scope.model.ADJUSTMENT_NAME = row.entity.ADJUSTMENT_NAME;
        $scope.model.ADJUSTMENT_STATUS = row.entity.ADJUSTMENT_STATUS;
        $scope.model.REMARKS = row.entity.REMARKS;
        //$scope.btnSaveValue = "Update";

    };

    $scope.GetCompanyId = function () {
        $scope.showLoader = true;
        AdjustmentInfoServices.GetCompanyId().then(function (data) {
            $scope.model.COMPANY_ID = data.data;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;
        });
    }
    $scope.GetCompanyName = function () {
        $scope.showLoader = true;
        AdjustmentInfoServices.GetCompanyName().then(function (data) {
            $scope.model.COMPANY_NAME = data.data;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;
        });
    }
    $scope.GetCompanyId();
    $scope.GetCompanyName();

    $scope.SaveData = function (model) {
        
        $scope.showLoader = true;
        
        model.COMPANY_ID = parseInt(model.COMPANY_ID);
        AdjustmentInfoServices.InsertOrUpdate(model).then(function (data) {
            
            $scope.model.ADJUSTMENT_ID = parseInt(data.data.Key);
            notificationservice.Notification(data.data.Status, 1, 'Data Save Successfully !!');
            $scope.SearchData(0);
        });
    }

    $scope.SearchData = function (companyId) {
        
        $scope.showLoader = true;
        AdjustmentInfoServices.SearchData(companyId).then(function (data) {
            
            
            $scope.gridAdjustmentInfo.data = data.data;
            $scope.showLoader = false;
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.SearchData(0);

    $scope.ResetData = function () {
        $scope.model.ADJUSTMENT_ID = 0;
        $scope.model.ADJUSTMENT_NAME = '';
        $scope.model.ADJUSTMENT_STATUS = 'Active';
        $scope.model.REMARKS = '';

    }

}]);

