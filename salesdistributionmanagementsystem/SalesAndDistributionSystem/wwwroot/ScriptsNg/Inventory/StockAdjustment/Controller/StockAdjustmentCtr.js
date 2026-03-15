ngApp.controller('ngStockAdjustmentCtrl', ['$scope', 'uiGridConstants', 'StockAdjustmentServices', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, uiGridConstants, StockAdjustmentServices, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'

    $scope.model = { ADJUSTMENT_ID: 0 }

    $scope.Products = [];

    $scope.isDisableCompanyId = true;
    $scope.isDisableCompanyName = true;

    $scope.isDisableUnitId = true;
    $scope.isDisableUnitName = true;

    $scope.isDisableAdjustmentId = true;
    $scope.isDisableAdjustmentDate = true;

    $scope.isDisableSkuCode = true;
    $scope.isDisableSkuId = true;
    $scope.isDisablePackSize = true;
    $scope.isDisableUnitTp = true;

    $scope.isDisableSBatchId = true;

    $scope.isDisableBatchStockQty = true;

    $scope.model.STOCK_TYPE = 'P';

    $scope.GetCurrentDate = function () {
        const today = new Date();
        const yyyy = today.getFullYear();
        let mm = today.getMonth() + 1;
        let dd = today.getDate();

        if (dd < 10) dd = '0' + dd;
        if (mm < 10) mm = '0' + mm;

        $scope.model.ADJUSTMENT_DATE = dd + '/' + mm + '/' + yyyy;
    }

    $scope.GetCurrentDate();

    $scope.GetCompanyId = function () {
        $scope.showLoader = true;
        StockAdjustmentServices.GetCompanyId().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;
        });
    }

    $scope.GetCompanyName = function () {
        $scope.showLoader = true;
        StockAdjustmentServices.GetCompanyName().then(function (data) {
            $scope.model.COMPANY_NAME = data.data;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;
        });
    }

    $scope.GetUnitId = function () {
        $scope.showLoader = true;
        StockAdjustmentServices.GetUnitId().then(function (data) {
            $scope.model.UNIT_ID = parseInt(data.data);
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;
        });
    }

    $scope.GetUnitName = function () {
        $scope.showLoader = true;
        StockAdjustmentServices.GetUnitName().then(function (data) {
            $scope.model.UNIT_NAME = data.data;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;
        });
    }


    $scope.LoadProductData = function () {
        $scope.showLoader = true;

        StockAdjustmentServices.LoadProductData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.Products_data = data.data;
            var _Products = {
                SKU_ID: "0",
                SKU_NAME: "None",
                SKU_CODE: "None",
                PACK_SIZE: "None",

            }
            $scope.Products.push(_Products);
            for (var i in $scope.Products_data) {
                $scope.Products.push($scope.Products_data[i]);
            }

            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    $scope.GetCompanyId();

    $scope.GetCompanyName();

    $scope.GetUnitId();
    $scope.GetUnitName();

    $scope.LoadProductData();
    $scope.OnProductSelect = function () {
        

        var _product_index = $scope.Products.findIndex(x => x.SKU_ID == $scope.model.SKU_ID);
        if (_product_index != -1) {
            $scope.model.SKU_CODE = $scope.Products[_product_index].SKU_CODE;
            $scope.model.SKU_ID = $scope.Products[_product_index].SKU_ID;
            $scope.model.PACK_SIZE = $scope.Products[_product_index].PACK_SIZE;
        }
    }

    $scope.formatRepoProduct = function (repo) {
        
        if (repo.loading) {
            return repo.text;
        }
        if (repo.text != "") {
            const textArray = repo.text.split("--");
            let text_title = textArray[0];
            let text_title_2 = textArray[1];
            let text_title_3 = textArray[2];

            var $container = $(
                "<div class='select2-result-repository clearfix'>" +
                "<div class='select2-result-repository__meta'>" +
                "<div class='select2-result-repository__title' style='font-size:14px;font-weight:700'></div>" +
                "<div class='select2-result-repository__watchers' style='font-size:12px;font-weight:700'> <span>Code: </span>  </div>" +
                "<div class='select2-result-repository__watchers_2' style='font-size:12px;font-weight:700'> <span>Pack Size: </span>  </div>" +
                "</div>" +
                "</div>"
            );

            $container.find(".select2-result-repository__title").text(text_title);
            $container.find(".select2-result-repository__watchers").append(text_title_2);
            $container.find(".select2-result-repository__watchers_2").append(text_title_3);


        }

        return $container;
    }

    $scope.formatRepoSelectionProduct = function (repo) {
        return repo.text.split("--")[0];
    }


    $(".select2-single-Product").select2({
        placeholder: "Select",
        templateResult: $scope.formatRepoProduct,
        templateSelection: $scope.formatRepoSelectionProduct
    });




    var columnBatchList = [

        { name: 'SKU_ID', field: 'SKU_ID', displayName: 'SKU ID', visible: false },
        { name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', visible: true, aggregationType: uiGridConstants.aggregationTypes.count, footerCellFilter: 'number:0' },
        { name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', visible: true },
        { name: 'PACK_SIZE', field: 'PACK_SIZE', displayName: 'Pack Size', visible: true },
        { name: 'BATCH_ID', field: 'BATCH_ID', displayName: 'Batch ID', visible: false },
        { name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No' },
        { name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit Tp' },
        { name: 'STOCK_QTY', field: 'STOCK_QTY', displayName: 'Stock Qty' }
    ];

    $scope.gridBatchList = {
        showGridFooter: true,
        enableFiltering: true,
        enableSorting: true,
        columnDefs: columnBatchList,
        enableGridMenu: false,
        enableSelectAll: false,
        exporterCsvFilename: 'Stock Adjustment.csv',
        exporterMenuPdf: false,
        rowTemplate: rowTemplate1(),
        exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),

        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
        }
    };

    function rowTemplate1() {
        return '<div ng-dblclick="grid.appScope.rowDblClickComp1(row)" >' +
            '  <div ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ng-class="{ \'ui-grid-row-header-cell\': col.isRowHeader }"  ui-grid-cell></div></div>';
    }
    $scope.rowDblClickComp1 = function (row) {

        $scope.model.BATCH_ID = row.entity.BATCH_ID;
        $scope.model.BATCH_NO = row.entity.BATCH_NO;
        $scope.model.UNIT_TP = row.entity.UNIT_TP;
        $scope.model.BATCH_STOCK_QTY = row.entity.STOCK_QTY;
        $('#BatchListModal').modal('toggle');

    };


    $scope.GetBatchList = function () {
        
        $scope.showLoader = true;

        if ($scope.model.COMPANY_ID > 0) {

            if ($scope.model.UNIT_ID > 0) {

                if ($scope.model.SKU_ID > 0) {

                    StockAdjustmentServices.GetBatchList($scope.model.COMPANY_ID, $scope.model.UNIT_ID, $scope.model.SKU_ID).then(function (data) {
                        
                        $scope.gridBatchList.data = data.data;
                        $('#BatchListModal').modal('toggle');
                        $scope.showLoader = false;
                    }, function (error) {
                        
                        $scope.showLoader = false;

                    });
                }
                else {
                    $scope.showLoader = false;
                    notificationservice.Notification('Please select Product', 1, '');
                }
            }
            else {
                $scope.showLoader = false;
                notificationservice.Notification('Unit can not be empty', 1, '');
            }
        }
        else {
            $scope.showLoader = false;
            notificationservice.Notification('Company can not be empty', 1, '');
        }

    }

    $scope.OnChangeAdjustmentType = function () {


        $scope.model.ADJUSTMENT_QTY = "";

    }

    $scope.OnChangeAdjustmentQty = function () {

        

        if ($scope.model.COMPANY_ID > 0) {

            if ($scope.model.UNIT_ID > 0) {

                if ($scope.model.SKU_ID > 0) {

                    if ($scope.model.BATCH_ID > 0) {

                        if ($scope.model.ADJUSTMENT_TYPE != null) {

                            if ($scope.model.ADJUSTMENT_TYPE == 'G') {

                                if (parseInt($scope.model.ADJUSTMENT_QTY) <= 0) {
                                    $scope.model.ADJUSTMENT_QTY = "";
                                    notificationservice.Notification('Adjustment qty must be greater than 0!', 1, '');
                                    return false;
                                }
                            }

                            if ($scope.model.ADJUSTMENT_TYPE == 'L') {

                                if (parseInt($scope.model.ADJUSTMENT_QTY) <= 0) {
                                    $scope.model.ADJUSTMENT_QTY = "";
                                    notificationservice.Notification('Adjustment qty must be greater than 0!', 1, '');
                                    return false;
                                }

                                if (parseInt($scope.model.ADJUSTMENT_QTY) > parseInt($scope.model.BATCH_STOCK_QTY)) {
                                    $scope.model.ADJUSTMENT_QTY = "";
                                    notificationservice.Notification('Adjustment qty can not be greater than stock qty!', 1, '');
                                    return false;
                                }

                            }

                        } else {
                            $scope.showLoader = false;
                            $scope.model.ADJUSTMENT_QTY = "";
                            notificationservice.Notification('Please select adjustment type', 1, '');
                        }

                    } else {
                        $scope.showLoader = false;
                        $scope.model.ADJUSTMENT_QTY = "";
                        notificationservice.Notification('Please select batch', 1, '');
                    }
                }
                else {
                    $scope.showLoader = false;
                    $scope.model.ADJUSTMENT_QTY = "";
                    notificationservice.Notification('Please select Product', 1, '');
                }
            }
            else {
                $scope.showLoader = false;
                $scope.model.ADJUSTMENT_QTY = "";
                notificationservice.Notification('Unit can not be empty', 1, '');
            }
        }
        else {
            $scope.showLoader = false;
            $scope.model.ADJUSTMENT_QTY = "";
            notificationservice.Notification('Company can not be empty', 1, '');
        }

    }



    $scope.SaveData = function (model) {
        
        $scope.showLoader = true;
        
        model.ADJUSTMENT_ID = parseInt(model.ADJUSTMENT_ID);
        model.COMPANY_ID = parseInt(model.COMPANY_ID);
        model.UNIT_ID = parseInt(model.UNIT_ID);
        model.SKU_ID = parseInt(model.SKU_ID);
        model.UNIT_TP = parseInt(model.UNIT_TP);
        model.ADJUSTMENT_AMOUNT = parseInt(model.UNIT_TP) * parseInt(model.ADJUSTMENT_QTY);


        StockAdjustmentServices.InsertOrUpdate(model).then(function (data) {
            
            $scope.model.ADJUSTMENT_ID = parseInt(data.data);

            $interval(function () {
                $scope.LoadSKU_ID();
            }, 800, 2);

            notificationservice.Notification(1, 1, 'Data Save Successfully !!');
            //$scope.SearchData(0);
        });
    }


    $scope.LoadSKU_ID = function () {
        $('#SKU_ID').trigger('change');

    }






    var columnSearchData = [

        { name: 'ADJUSTMENT_ID', field: 'ADJUSTMENT_ID', displayName: 'Adjustment ID', visible: true },
        { name: 'ADJUSTMENT_DATE', field: 'ADJUSTMENT_DATE', displayName: 'Adjustment Date', visible: true },

        { name: 'STOCK_TYPE', field: 'STOCK_TYPE', displayName: 'Stock Type', visible: false },
        { name: 'STOCK_TYPE_NAME', field: 'STOCK_TYPE_NAME', displayName: 'Stock Type', visible: true },

        { name: 'ADJUSTMENT_TYPE', field: 'ADJUSTMENT_TYPE', displayName: 'Adjustment Type', visible: false },
        { name: 'ADJUSTMENT_TYPE_NAME', field: 'ADJUSTMENT_TYPE_NAME', displayName: 'Adjustment Type', visible: true },

        { name: 'SKU_ID', field: 'SKU_ID', displayName: 'SKU ID', visible: false },
        { name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', visible: true, aggregationType: uiGridConstants.aggregationTypes.count, footerCellFilter: 'number:0' },
        { name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', visible: true },
        { name: 'PACK_SIZE', field: 'PACK_SIZE', displayName: 'Pack Size', visible: true },
        { name: 'BATCH_ID', field: 'BATCH_ID', displayName: 'Batch ID', visible: false },
        { name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No' },
        { name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit Tp' },
        { name: 'STOCK_QTY', field: 'STOCK_QTY', displayName: 'Stock Qty' }
    ];

    $scope.gridSearchData = {
        showGridFooter: true,
        enableFiltering: true,
        enableSorting: true,
        columnDefs: columnSearchData,
        enableGridMenu: false,
        enableSelectAll: false,
        exporterCsvFilename: 'Stock Adjustment.csv',
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
        $scope.model.ADJUSTMENT_DATE = row.entity.ADJUSTMENT_DATE;
        $scope.model.ADJUSTMENT_TYPE = row.entity.ADJUSTMENT_TYPE;
        $scope.model.STOCK_TYPE = row.entity.STOCK_TYPE;

        $scope.model.SKU_ID = row.entity.SKU_ID;
        $scope.model.SKU_CODE = row.entity.SKU_CODE;
        $scope.model.PACK_SIZE = row.entity.PACK_SIZE;
        $scope.model.UNIT_TP = row.entity.UNIT_TP;
        $scope.model.BATCH_ID = row.entity.BATCH_ID;
        $scope.model.BATCH_NO = row.entity.BATCH_NO;
        $scope.model.ADJUSTMENT_QTY = row.entity.ADJUSTMENT_QTY;
        $scope.model.BATCH_STOCK_QTY = row.entity.STOCK_QTY;
        $scope.model.REMARKS = row.entity.REMARKS;

        $(SearchDataModal).modal('toggle');

    };





    $scope.GetSearchData = function () {
        
        $scope.showLoader = true;

        if ($scope.model.COMPANY_ID > 0) {

            if ($scope.model.UNIT_ID > 0) {

                StockAdjustmentServices.GetSearchData($scope.model.COMPANY_ID, $scope.model.UNIT_ID).then(function (data) {
                    
                    $scope.gridSearchData.data = data.data;
                    $(SearchDataModal).modal('toggle');
                    $scope.showLoader = false;
                }, function (error) {
                    
                    $scope.showLoader = false;
                });
            }
            else {
                $scope.showLoader = false;
                notificationservice.Notification('Unit can not be empty', 1, '');
            }
        }
        else {
            $scope.showLoader = false;
            notificationservice.Notification('Company can not be empty', 1, '');
        }

    }





    $scope.ResetData = function () {

        window.location.href = "/Inventory/StockAdjustment/StockAdjustment"
    }



}]);

