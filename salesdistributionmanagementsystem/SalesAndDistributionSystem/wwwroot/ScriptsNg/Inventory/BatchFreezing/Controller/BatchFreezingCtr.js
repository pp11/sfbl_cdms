ngApp.controller('ngBatchFreezingCtrl', ['$scope', 'uiGridConstants', 'BatchFreezingServices', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, uiGridConstants, BatchFreezingServices, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'

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

    $scope.model = {
       
        MST_ID: 0,
        ENTRY_DATE: $scope.formatDate(new Date()),
        COMPANY_ID: 0,
        UNIT_ID: 0,
        SKU_ID: "",
        SKU_CODE: "",
        REMAEKS: "",
        batchFreezingDtlList: []
    }


    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 0,
            DTL_ID: 0,
            MST_ID: 0,
            COMPANY_ID: 0,
            UNIT_ID: 0,
            SKU_ID: 0,
            SKU_CODE: '',
            SKU_NAME: '',
            PACK_SIZE: '',
            BATCH_ID: 0,
            BATCH_NO: "",
            UNIT_TP: 0,
            //BATCH_UNIT_TP: 0,
            //STOCK_QTY: 0,
            BATCH_QTY: 0,
            FREEZE_QTY: 0,
            //BATCH_UN_FREEZE_QTY: 0,
            //STATUS: ''
        }
    }

    $scope.ClearEntity = function () {

        $scope.gridBatchFreezing.data[0].ROW_NO = 0
        $scope.gridBatchFreezing.data[0].DTL_ID = 0;
        $scope.gridBatchFreezing.data[0].MST_ID = 0;
        $scope.gridBatchFreezing.data[0].COMPANY_ID = 0;
        $scope.gridBatchFreezing.data[0].UNIT_ID = 0;
        $scope.gridBatchFreezing.data[0].SKU_ID = 0;
        $scope.gridBatchFreezing.data[0].SKU_CODE = "",
        $scope.gridBatchFreezing.data[0].SKU_NAME = "",
        $scope.gridBatchFreezing.data[0].PACK_SIZE = '';
        $scope.gridBatchFreezing.data[0].BATCH_ID = 0;
        $scope.gridBatchFreezing.data[0].BATCH_NO = '';
        $scope.gridBatchFreezing.data[0].UNIT_TP = 0;
        //$scope.gridBatchFreezing.data[0].BATCH_UNIT_TP = 0;
        $scope.gridBatchFreezing.data[0].BATCH_QTY = 0;
        $scope.gridBatchFreezing.data[0].FREEZE_QTY = 0;
        //$scope.gridBatchFreezing.data[0].BATCH_UN_FREEZE_QTY = 0;
        //$scope.gridBatchFreezing.data[0].STATUS = '';

    };


    $scope.Products = [];
    $scope.Batches = [];
    $scope.Status = [];

    $scope.isDisableEntryDate = true;
    $scope.isDisableMstId = true;
    $scope.isDisableUnitId = true;
    $scope.isDisableUnitName = true;
    $scope.isDisableSkuCode = true;

  


    $scope.GetCompanyId = function () {
        $scope.showLoader = true;
        BatchFreezingServices.GetCompanyId().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;
        });
    }

    $scope.GetCompanyName = function () {
        $scope.showLoader = true;
        BatchFreezingServices.GetCompanyName().then(function (data) {
            $scope.model.COMPANY_NAME = data.data;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;
        });
    }

    $scope.GetUnitId = function () {
        $scope.showLoader = true;
        BatchFreezingServices.GetUnitId().then(function (data) {
            $scope.model.UNIT_ID = parseInt(data.data);
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;
        });
    }

    $scope.GetUnitName = function () {
        $scope.showLoader = true;
        BatchFreezingServices.GetUnitName().then(function (data) {
            $scope.model.UNIT_NAME = data.data;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;
        });
    }

    $scope.GetCompanyId();
    $scope.GetCompanyName();

    $scope.GetUnitId();
    $scope.GetUnitName();



    $scope.GetSkuList = function () {
        $scope.showLoader = true;

        BatchFreezingServices.GetSkuList($scope.model.COMPANY_ID, $scope.model.UNIT_ID).then(function (data) {
            
            $scope.Products_data = data.data;

            for (var i in $scope.Products_data) {

                $scope.Products.push($scope.Products_data[i]);
            }
            
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.GetSkuList();


    
    $scope.OnSelectSkuName = function () {

        //Generate batch list
        $scope.Batches = [];
        $scope.showLoader = true;

        ;
        
        var _product_index = $scope.Products.findIndex(x => x.SKU_ID == $scope.model.SKU_ID);

        if (_product_index != -1) {

            $scope.model.SKU_CODE = $scope.Products[_product_index].SKU_CODE;

  
        }

        BatchFreezingServices.GetBatchList($scope.model.COMPANY_ID, $scope.model.UNIT_ID, $scope.model.SKU_ID).then(function (data) {

            

            $scope.Batch_data = data.data;

            var _Batches = {

                SKU_ID: "0",
                SKU_CODE: "",
                SKU_NAME: "",
                PACK_SIZE: "",
                BATCH_ID: 0,
                BATCH_NO: "",
                UNIT_TP: 0,
                //BATCH_UNIT_TP: 0,
                STOCK_QTY: 0,
                BATCH_QTY: 0,
                FREEZE_QTY: 0,
                //BATCH_UN_FREEZE_QTY: 0,
                //STATUS: ""

            }

            $scope.Batches.push(_Batches);

            for (var i in $scope.Batch_data) {
                $scope.Batches.push($scope.Batch_data[i]);
            }

            $scope.showLoader = false;

        }, function (error) {

            alert(error);

            

            $scope.showLoader = false;

        });
    }



    var BatchFreezingcolDefs = [

        {
            name: '#', field: 'ROW_NO', visible: true, width: '4%', cellTemplate:
                '<input type="number" disabled style="text-align: right;"  ng-model="row.entity.ROW_NO"  class="pl-sm" />'
        },

        {
            name: 'DTL_ID', field: 'DTL_ID', displayName: 'Dtl ID', visible: false, width: '10%', cellTemplate:
                '<input type="number" disabled style="text-align: right;"  ng-model="row.entity.DTL_ID"  class="pl-sm" />'
        },

        {
            name: 'MST_ID', field: 'MST_ID', displayName: 'Mst ID', visible: false, width: '10%', cellTemplate:
                '<input type="number" disabled style="text-align: right;"  ng-model="row.entity.MST_ID"  class="pl-sm" />'
        },

        ,

        {
            name: 'COMPANY_ID', field: 'COMPANY_ID', displayName: 'Company ID', visible: false, width: '10%', cellTemplate:
                '<input type="number" disabled style="text-align: right;"  ng-model="row.entity.COMPANY_ID"  class="pl-sm" />'
        },

        {
            name: 'UNIT_ID', field: 'UNIT_ID', displayName: 'Unit ID', visible: false, width: '10%', cellTemplate:
                '<input type="number" disabled style="text-align: right;"  ng-model="row.entity.UNIT_ID"  class="pl-sm" />'
        },

        {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: true, width: '15%', cellTemplate:
                '<select class="select2-single form-control batch-no"  ng-style="row.entity.ROW_NO != 0 && grid.appScope.myObj" ng-disabled="row.entity.ROW_NO !=0"   data-select2-id="{{row.entity.BATCH_NO}}" id="BATCH_NO"' +
                'name="BATCH_NO" ng-model="row.entity.BATCH_NO" style="width:100%" ng-change="grid.appScope.OnSelectBatchNo(row.entity)">' +
                '<option ng-repeat="item in grid.appScope.Batches" ng-selected="item.BATCH_NO == row.entity.BATCH_NO" value="{{item.BATCH_NO}}"> {{ item.BATCH_NO }}</option>' +
                '</select>'
        },

        {
            name: 'BATCH_ID', field: 'BATCH_ID', displayName: 'Batch ID', visible: false, width: '10%', cellTemplate:
                '<input type="number" disabled style="text-align: right;"  ng-model="row.entity.BATCH_ID"  class="pl-sm" />'
        },


        {
            name: 'SKU_ID', field: 'SKU_ID', displayName: 'SKU ID', visible: false, width: '10', cellTemplate:
                '<input type="number" disabled style="text-align: right;"  ng-model="row.entity.SKU_ID"  class="pl-sm" />'
        },


        {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', visible: true, width: '12%', cellTemplate:
                '<input type="text" disabled  style="text-align: left;"  ng-model="row.entity.SKU_CODE"  class="pl-sm" />'
        },


        {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', visible: true, width: '35%', cellTemplate:
                '<input type="text" disabled  style="text-align: left;"  ng-model="row.entity.SKU_NAME"  class="pl-sm" />'
        },

        {
            name: 'PACK_SIZE', field: 'PACK_SIZE', displayName: 'Pack Size', visible: false, width: '10%', cellTemplate:
                '<input type="text" disabled  style="text-align: left;"  ng-model="row.entity.PACK_SIZE"  class="pl-sm" />'
        },


        {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Trade Price', visible: true, width: '10%', cellTemplate:
                '<input type="number" disabled style="text-align: right;"  ng-model="row.entity.UNIT_TP"  class="pl-sm" />'
        },

        //{
        //    name: 'BATCH_UNIT_TP', field: 'BATCH_UNIT_TP', displayName: 'Batch Price', visible: true, width: '10%', cellTemplate:
        //        '<input type="number" disabled style="text-align: right;" ng-model="row.entity.BATCH_UNIT_TP"  class="pl-sm" />'
        //}
        ,
        {
            name: 'STOCK_QTY', field: 'STOCK_QTY', displayName: 'Stock Qty', visible: true, width: '9%', cellTemplate:
                '<input type="number" disabled style="text-align: right;" ng-model="row.entity.STOCK_QTY"  class="pl-sm" />'
        },
        {
            name: 'BATCH_QTY', field: 'BATCH_QTY', displayName: 'Batch Qty', visible: true, width: '10%', cellTemplate:
                '<input type="number" disabled style="text-align: right;" ng-model="row.entity.BATCH_QTY"  class="pl-sm" />'
        },

        {
            name: 'FREEZE_QTY', field: 'FREEZE_QTY', displayName: 'Freeze Qty', enableFiltering: true, visible: true, width: '10%', cellTemplate:
                '<input type="number" style="text-align: right;" ng-model="row.entity.FREEZE_QTY"  class="pl-sm" />'              
        },    


        //{
        //    name: 'BATCH_UN_FREEZE_QTY', field: 'BATCH_UN_FREEZE_QTY', displayName: 'Unfreeze Qty', enableFiltering: true, visible: true, width: '11%', cellTemplate:
        //        '<input type="number" style="text-align: right;" ng-model="row.entity.BATCH_UN_FREEZE_QTY"  class="pl-sm" />'
        //},  


        //{
        //    name: 'STATUS', field: 'STATUS', displayName: 'Status', enableFiltering: true, visible: true, width: '9%', cellTemplate:
        //        '<input type="text" style="text-align: right;" ng-model="row.entity.STATUS"  class="pl-sm" />'
        //},      


        //{
        //    name: 'STATUS', field: 'STATUS', displayName: 'Status', visible: true, width: '7%', cellTemplate:
        //        '<select class="form-control" ng-style="row.entity.ROW_NO != 0 && grid.appScope.myObj"  id="STATUS"'
        //        + 'name="STATUS" ng-model="row.entity.STATUS" style="width:100%">'
        //        + '<option ng-repeat="item in grid.appScope.Status" value="{{item.STATUS}}" ng-selected="item.STATUS == row.entity.STATUS" >{{ item.STATUS }}</option>'
        //        + '</select>'
        //},

        {
            name: 'Action', displayName: 'Action', width: '7%', visible: true, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.EditItem(row.entity)" type="button" class="btn btn-outline-success mb-1"><img src="/img/edit-icon.png" height="15px" ></button>' +

                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO == 0" ng-click="grid.appScope.addNewRow(row.entity)" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                '</div>'
        }


    ];
  

    $scope.gridBatchFreezing = {
        showGridFooter: false,
        enableFiltering: false,
        enableSorting: false,
        columnDefs: BatchFreezingcolDefs,
        enableGridMenu: false,
        enableSelectAll: false,
        exporterCsvFilename: 'Adjustment Info.csv',
        data: [$scope.GridDefalutData()],
        exporterMenuPdf: false,
        exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
        }
    };


    $scope.gridBatchFreezing.rowTemplate = "<div  ng-style='row.entity.FREEZE_QTY >row.entity.STOCK_QTY && grid.appScope.myObj' ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.isTrue = true;
    $scope.myObj = {
        "color": "red",

    }

    $scope.rowNumberGenerate = function () {

        

        for (let i = 0; i < $scope.gridBatchFreezing.data.length; i++) {

            $scope.gridBatchFreezing.data[i].ROW_NO = i;

        }
    }


    $scope.OnSelectBatchNo = function (entity) {

        

        const searchIndex = $scope.Batches.findIndex((x) => x.BATCH_NO == entity.BATCH_NO);
        if (searchIndex != -1) {
            entity.BATCH_ID = $scope.Batches[searchIndex].BATCH_ID;
            entity.SKU_ID = $scope.Batches[searchIndex].SKU_ID;

            entity.SKU_CODE = $scope.Batches[searchIndex].SKU_CODE;
            entity.SKU_NAME = $scope.Batches[searchIndex].SKU_NAME;
            entity.PACK_SIZE = $scope.Batches[searchIndex].PACK_SIZE;

            entity.STOCK_QTY = $scope.Batches[searchIndex].STOCK_QTY;

            entity.BATCH_QTY = $scope.Batches[searchIndex].BATCH_QTY;
            entity.FREEZE_QTY = $scope.Batches[searchIndex].FREEZE_QTY;
            //entity.BATCH_UN_FREEZE_QTY = $scope.Batches[searchIndex].BATCH_UN_FREEZE_QTY;

            entity.UNIT_TP = parseInt($scope.Batches[searchIndex].UNIT_TP);
            //entity.BATCH_UNIT_TP = parseInt($scope.Batches[searchIndex].BATCH_UNIT_TP);


            //entity.STATUS = $scope.Batches[searchIndex].STATUS;
        }

       

        //$scope.LoadBatchNo();


    };

    $scope.LoadBatchNo = function () {

        setTimeout(function () {
            $(".batch-no").trigger('change');
        }, 1000)
    }



    $scope.addDefaultRow = (entity) => {

        var newRow = {

            ROW_NO: 1,
            DTL_ID: $scope.gridBatchFreezing.data[0].DTL_ID,
            MST_ID: $scope.gridBatchFreezing.data[0].MST_ID,
            BATCH_ID: $scope.gridBatchFreezing.data[0].BATCH_ID,
            BATCH_NO: $scope.gridBatchFreezing.data[0].BATCH_NO,
            SKU_ID: $scope.gridBatchFreezing.data[0].SKU_ID,
            SKU_CODE: $scope.gridBatchFreezing.data[0].SKU_CODE,
            SKU_NAME: $scope.gridBatchFreezing.data[0].SKU_NAME,
            PACK_SIZE: $scope.gridBatchFreezing.data[0].PACK_SIZE,
            //STOCK_QTY: $scope.gridBatchFreezing.data[0].STOCK_QTY,
            BATCH_QTY: $scope.gridBatchFreezing.data[0].BATCH_QTY,
            FREEZE_QTY: $scope.gridBatchFreezing.data[0].FREEZE_QTY,
            //BATCH_UN_FREEZE_QTY: $scope.gridBatchFreezing.data[0].BATCH_UN_FREEZE_QTY,
            UNIT_TP: parseInt($scope.gridBatchFreezing.data[0].UNIT_TP),
            //BATCH_UNIT_TP: parseInt($scope.gridBatchFreezing.data[0].BATCH_UNIT_TP),
            //STATUS: $scope.model.STATUS,
            COMPANY_ID: $scope.model.COMPANY_ID,
            UNIT_ID: $scope.model.UNIT_ID

        }

        $scope.gridBatchFreezing.data.push(newRow);

        $scope.gridBatchFreezing.data[0] = $scope.GridDefalutData();

        $scope.rowNumberGenerate();
    }

    $scope.addNewRow = (entity) => {

        if (entity.FREEZE_QTY > entity.STOCK_QTY) {
            notificationservice.Notification("Freez qty is greater then stock qty", "", 'Freez qty is greater then stock qty');
            return;
        }

        var count = 0;

        if ($scope.gridBatchFreezing.data.length > 0 && $scope.gridBatchFreezing.data[0].BATCH_NO != null && $scope.gridBatchFreezing.data[0].BATCH_NO != '' && $scope.gridBatchFreezing.data[0].BATCH_NO != 'undefined') {

            for (var i = 0; i < $scope.gridBatchFreezing.data.length; i++) {

                if ($scope.gridBatchFreezing.data[i].BATCH_NO == entity.BATCH_NO) {

                    count++;

                }
            }


            if (count == 1 || count == 0 || entity.BATCH_NO == "") {

                var newRow = {

                    ROW_NO: 1,
                    DTL_ID: $scope.gridBatchFreezing.data[0].DTL_ID,
                    MST_ID: $scope.gridBatchFreezing.data[0].MST_ID,
                    BATCH_ID: $scope.gridBatchFreezing.data[0].BATCH_ID,
                    BATCH_NO: $scope.gridBatchFreezing.data[0].BATCH_NO,
                    SKU_ID: $scope.gridBatchFreezing.data[0].SKU_ID,
                    SKU_CODE: $scope.gridBatchFreezing.data[0].SKU_CODE,
                    SKU_NAME: $scope.gridBatchFreezing.data[0].SKU_NAME,
                    PACK_SIZE: $scope.gridBatchFreezing.data[0].PACK_SIZE,
                    //STOCK_QTY: $scope.gridBatchFreezing.data[0].STOCK_QTY,
                    BATCH_QTY: $scope.gridBatchFreezing.data[0].BATCH_QTY,
                    FREEZE_QTY: $scope.gridBatchFreezing.data[0].FREEZE_QTY,
                    //BATCH_UN_FREEZE_QTY: $scope.gridBatchFreezing.data[0].BATCH_UN_FREEZE_QTY,
                    UNIT_TP: parseInt($scope.gridBatchFreezing.data[0].UNIT_TP),
                    //BATCH_UNIT_TP: parseInt($scope.gridBatchFreezing.data[0].BATCH_UNIT_TP),
                    //STATUS: $scope.gridBatchFreezing.data[0].STATUS,
                    COMPANY_ID: $scope.model.COMPANY_ID,
                    UNIT_ID: $scope.model.UNIT_ID

                }

                $scope.gridBatchFreezing.data.push(newRow);

                $scope.gridBatchFreezing.data[0] = $scope.GridDefalutData();

                $scope.rowNumberGenerate();

            }
            else {

                notificationservice.Notification("Batch no already exist!", "", 'Batch already exist!');
                $scope.ClearEntity();
            }
        }
        else {

            notificationservice.Notification("No item has added!", "", 'No item has added!');

        }
    };

    $scope.removeItem = function (entity) {

        

        if ($scope.gridBatchFreezing.data.length > 1) {

            var index = $scope.gridBatchFreezing.data.indexOf(entity);

            if ($scope.gridBatchFreezing.data.length > 0) {

                $scope.gridBatchFreezing.data.splice(index, 1);

            }

            $scope.rowNumberGenerate();


        } else {

            notificationservice.Notification("Only Single Row Left!!", "", 'Only Single Row Left!!');
        }


    }


    $scope.EditItem = (entity) => {

        

        if ($scope.gridBatchFreezing.data.length > 0) {

            var newRow = {

                ROW_NO: 1,
                DTL_ID: entity.DTL_ID,
                MST_ID: entity.MST_ID,
                BATCH_ID: entity.BATCH_ID,
                BATCH_NO: entity.BATCH_NO,
                SKU_ID: entity.SKU_ID,
                SKU_CODE: entity.SKU_CODE,
                SKU_NAME: entity.SKU_NAME,
                PACK_SIZE: entity.PACK_SIZE,
                //STOCK_QTY: entity.STOCK_QTY,
                BATCH_QTY: entity.BATCH_QTY,
                FREEZE_QTY: entity.FREEZE_QTY,
                //BATCH_UN_FREEZE_QTY: entity.BATCH_UN_FREEZE_QTY,
                //UNIT_TP: parseInt(entity.UNIT_TP),
                BATCH_UNIT_TP: parseInt(entity.BATCH_UNIT_TP),
                //STATUS: entity.STATUS,
                COMPANY_ID: entity.COMPANY_ID,
                UNIT_ID: entity.UNIT_ID

            }

            $scope.gridBatchFreezing.data[0] = newRow;

        } else {

            notificationservice.Notification("Please Select Valid data", "", 'Only Single Row Left!!');

        }

        $scope.rowNumberGenerate();

        $scope.removeItem(entity);
    };


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


    //$scope.LoadStatus = function () {

    //    var Active = {
    //        STATUS: 'Yes'
    //    }
    //    var InActive = {
    //        STATUS: 'No'
    //    }
    //    $scope.Status.push(Active);
    //    $scope.Status.push(InActive);

    //}
    //$scope.LoadStatus();

    $(".select2-single-Product").select2({
        placeholder: "Select",
        templateResult: $scope.formatRepoProduct,
        templateSelection: $scope.formatRepoSelectionProduct
    });








    $scope.SaveData = function (model) {

        

        $scope.model.SKU_ID = parseInt($scope.model.SKU_ID);

        if ($scope.model.SKU_CODE == "") {

            notificationservice.Notification('Product not selected!', "", 'Product not selected!');

            return;
        }

        $scope.model.batchFreezingDtlList = $scope.gridBatchFreezing.data;
   
        for (var i = 0; i < $scope.model.batchFreezingDtlList.length; i++) {
            if ($scope.model.batchFreezingDtlList[i].FREEZE_QTY > $scope.model.batchFreezingDtlList[i].STOCK_QTY && $scope.model.batchFreezingDtlList[i].STATUS == "Yes") {
                notificationservice.Notification("Freeze Qty is greater then stock Qty", "", 'Freeze Qty is greater then stock Qty!');
                $scope.showLoader = false;
                return;
            }
        }
        if ($scope.model.batchFreezingDtlList.length == 1) {

            $scope.showLoader = false;
            notificationservice.Notification("No data has been added on batch freezing detail!", "", 'No data has been added on batch freezing detail!');
            return;

        }

        if ($scope.model.batchFreezingDtlList != null) {

            if ($scope.model.batchFreezingDtlList.length > 1 && $scope.model.batchFreezingDtlList[0].BATCH_NO == "") {

                $scope.model.batchFreezingDtlList.splice(0, 1);

            }
        }

        $scope.showLoader = true;

        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID)
        $scope.model.UNIT_ID = parseInt($scope.model.UNIT_ID);

        

        BatchFreezingServices.InsertOrUpdate(model).then(function (data) {

            ;

            if (parseInt(data.data) > 0) {

                $scope.showLoader = false;
                $scope.model.SKU_ID = JSON.stringify($scope.model.SKU_ID);
                $scope.model.MST_ID = parseInt(data.data);

                $interval(function () {
                    $('#SKU_ID').trigger('change');
                }, 800, 2);

                
                $scope.GetDtlData();

                notificationservice.Notification(1, 1, 'data save successfully!');



            }
            else {

                $scope.showLoader = false;

                $scope.addDefaultRow($scope.GridDefalutData());

                notificationservice.Notification(data.data, "", data.data);
            }
        });



    }


    var columnSearchData = [

        { name: 'MST_ID', field: 'MST_ID', displayName: 'ID', visible: true },
        { name: 'ENTRY_DATE', field: 'ENTRY_DATE', displayName: 'Entry Date', visible: true },

        { name: 'COMPANY_ID', field: 'COMPANY_ID', displayName: 'Company ID', visible: false },
        { name: 'COMPANY_NAME', field: 'COMPANY_NAME', displayName: 'Company Name', visible: false },

        { name: 'UNIT_ID', field: 'UNIT_ID', displayName: 'Unit ID', visible: false },
        { name: 'UNIT_NAME', field: 'UNIT_NAME', displayName: 'Unit Name', visible: true },

        { name: 'SKU_ID', field: 'SKU_ID', displayName: 'SKU ID', visible: false },
        { name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', visible: true, aggregationType: uiGridConstants.aggregationTypes.count, footerCellFilter: 'number:0' },
        { name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', visible: true },
        { name: 'PACK_SIZE', field: 'PACK_SIZE', displayName: 'Pack Size', visible: true },

        { name: 'REMARKS', field: 'REMARKS', displayName: 'Remarks', visible: true }
 
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
        return '<div ng-dblclick="grid.appScope.rowDblClick(row)" >' +
            '  <div ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ng-class="{ \'ui-grid-row-header-cell\': col.isRowHeader }"  ui-grid-cell></div></div>';
    }
    $scope.rowDblClick = function (row) {

        $scope.model.MST_ID = row.entity.MST_ID;
        $scope.model.ENTRY_DATE = row.entity.ENTRY_DATE;
        $scope.model.SKU_ID = row.entity.SKU_ID;
        $scope.model.SKU_CODE = row.entity.SKU_CODE;        
        $scope.model.REMARKS = row.entity.REMARKS;
        $scope.OnSelectSkuName();

        $scope.GetDtlData();
        $(SearchDataModal).modal('toggle');

    };





    $scope.GeMstData = function () {
        
        $scope.showLoader = true;

        BatchFreezingServices.GetMstData($scope.model.COMPANY_ID, $scope.model.UNIT_ID).then(function (data) {
            

            if (data.data.length > 0) {

                
                $scope.gridSearchData.data = data.data;

                $interval(function () {
                    $('#SKU_ID').trigger('change');
                }, 800, 2);

                $(SearchDataModal).modal('toggle');
                $scope.showLoader = false;


            }

        }, function (error) {
            
            $scope.showLoader = false;
        }); 
    }


    $scope.GetDtlData = function () {
        
        $scope.showLoader = true;

        if ($scope.model.MST_ID > 0) {

            BatchFreezingServices.GetDtlData($scope.model.MST_ID).then(function (data) {

                

                if (data.data.length > 0) {

                    $scope.gridBatchFreezing.data = [
                        $scope.GridDefalutData(),
                        ...data.data
                    ];


                    $scope.showLoader = false;



                }
                $scope.Refresh(); 

            }, function (error) {
                
                $scope.showLoader = false;
            });

        }
        else {
            $scope.showLoader = false;
            notificationservice.Notification('MST_ID not found', 1, '');
        }

    }

    $scope.Refresh = function () {
        $("#BATCH_NO").trigger("change");
        $("#STATUS").trigger("change");
    }
    $scope.ResetData = function () {

        $scope.model.MST_ID = 0;

        $scope.model.SKU_ID = 0;

 
        $scope.model.SKU_CODE = '';

        $scope.model.SKU_NAME = '';

        $scope.model.REMARKS = '';
        $scope.Batches = [];
        $scope.gridBatchFreezing.data = [];
        $scope.gridBatchFreezing.data[0] = $scope.GridDefalutData();
    }


    $scope.ResetDtlData = function () {

        $scope.Batches = [];
        $scope.gridBatchFreezing.data = [];
        $scope.gridBatchFreezing.data[0] = $scope.GridDefalutData();
    }




}]);

