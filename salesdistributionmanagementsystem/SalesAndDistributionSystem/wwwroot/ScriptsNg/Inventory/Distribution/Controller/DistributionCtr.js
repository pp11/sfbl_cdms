ngApp.controller('ngGridCtrl', ['$scope', 'DistributionService', 'LiveNotificationServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, distributionService, LiveNotificationServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {
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

    var connection = new signalR.HubConnectionBuilder().withUrl("/notificationhub").build();
    connection.start();

    $scope.model = {
        COMPANY_ID: 0,
        MST_ID: 0,
        MST_ID_ENCRYPTED: "",
        DISPATCH_NO: "",
        DISPATCH_DATE: $scope.formatDate(new Date()),
        VEHICLE_NO: ""
        , VEHICLE_DESCRIPTION: ""
        , VECHILE_VOLUME: 0
        , VECHILE_WEIGHT: 0
        , DISPATCH_VOLUME: 0
        , DISPATCH_WEIGHT: 0
        , DRIVER_ID: 0
        , DISPATCH_UNIT_ID: 0
        , DISPATCH_BY: ""
        , STATUS: "Active"
        , DISPATCH_TYPE: ""
        , REMAEKS: ""
        , requisitionIssueDtlList: []
    }

    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 0,
            DEPOT_REQ_ID: 0,
            MST_ID: 0,
            REQUISITION_NO: "",
            REQUISITION_DATE: "",
            REQUISITION_UNIT_ID: '',
            REQUISITION_UNIT_NAME: '',
            ISSUE_NO: "",
            ISSUE_DATE: "",
            ISSUE_UNIT_ID: "",
            ISSUE_UNIT_NAME: "",

            STATUS: 'Active',
            COMPANY_ID: 0,
            REMARKS: '',
        }
    }

    $scope.GridDefalutProductData = function () {
        return {
            ROW_NO: 0,
            DISPATCH_PRODUCT_ID: 0,
            DISPATCH_REQ_ID: 0,
            MST_ID: 0,
            SKU_ID: "",
            SKU_CODE: "",
            ISSUE_QTY: "",
            DISPATCH_QTY: 0,
            PENDING_DISPATCH_QTY: 0,
            UNIT_TP: "",
            DISPATCH_AMOUNT: "",
            DISPATCH_UNIT_ID: 0,
            COMPANY_ID: 0,
            DISPATCH_DATE: $scope.formatDate(new Date()),
            REMARKS: '',
        }
    }

    $scope.GridDefalutShipperData = function () {
        return {
            ROW_NO: 0,
            DISPATCH_SHIPPER_DTL_ID: 0,
            MST_ID: 0,
            SKU_ID: "",
            SKU_CODE: "",
            DISPATCH_QTY: 0,
            SHIPPER_QTY: 0,
            NO_OF_SHIPPER: 0,
            LOOSE_QTY: 0,
            SHIPPER_WEIGHT: 0,
            SHIPPER_VOLUME: 0,
            LOOSE_WEIGHT: 0,
            LOOSE_VOLUME: 0,
            TOTAL_WEIGHT: 0,
            TOTAL_VOLUME: 0,
            DISPATCH_UNIT_ID: 0,
            COMPANY_ID: 0,
            DISPATCH_DATE: $scope.formatDate(new Date()),
            REMARKS: '',
        }
    }

    $scope.GridDefalutBatchData = function () {
        return {
            ROW_NO: 0,
            DISPATCH_PROD_BATCH_ID: 0,
            DISPATCH_PRODUCT_ID: 0,
            MST_ID: 0,
            SKU_ID: "",
            SKU_CODE: "",
            REQUISITION_NO: "",
            DISPATCH_DATE: "",
            BATCH_ID: "",
            BATCH_NO: "",
            DISPATCH_UNIT_ID: 0,
            DISPATCH_QTY: 0,
            DISPATCH_AMOUNT: 0,
            COMPANY_ID: 0,
            DISPATCH_DATE: $scope.formatDate(new Date()),
            REMARKS: '',
        }
    }

    $scope.ClearEntity = function (entity) {
        entity.ROW_NO = 0,
            entity.DISPATCH_REQ_ID = 0,
            entity.MST_ID = 0,
            entity.COMPANY_ID = 0,
            entity.REQUISITION_NO = "",
            entity.REQUISITION_DATE = "",
            entity.REQUISITION_UNIT_ID = '',
            entity.ISSUE_NO = "",
            entity.ISSUE_DATE = "",
            entity.ISSUE_UNIT_ID = ''
    };

    $scope.getPermissions = [];
    $scope.ProductList = [];
    $scope.Companies = [];
    $scope.Vehicles = [];
    $scope.Unit = [];
    $scope.CustomerData = [];
    $scope.CustomerType = [];
    $scope.ProductList = [];
    $scope.RequisitionList = [];

    $scope.BaseProducts = [];
    $scope.Categories = [];
    $scope.Brands = [];
    $scope.Groups = [];
    $scope.Products = [];
    $scope.existingSKU = [];
    $scope.Products = [];
    $scope.Status = [];

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Distribution Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.data = [];

    $scope.gridOptionsList = {
        data: [$scope.GridDefalutData()]
    }

    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '45'
        }
        , { name: 'MST_ID', field: 'MST_ID', visible: false }
        , { name: 'Is_Selected', field: 'Is_Selected', visible: false }
        , { name: 'DEPOT_REQ_ID', field: 'DEPOT_REQ_ID', visible: false }
        , {
            name: 'REQUISITION_NO', field: 'REQUISITION_NO', displayName: 'Requisition No', enableFiltering: true, width: '25%', cellTemplate:
                '<select ng-disabled="row.entity.ROW_NO != 0" class="select2-single form-control"   data-select2-id="{{row.entity.REQUISITION_NO}}" class="REQUISITION_NO"' +
                'name="REQUISITION_NO" ng-model="row.entity.REQUISITION_NO" style="width:100%" ng-change="grid.appScope.typeaheadSelectedRequisitionNo(row.entity)">' +
                '<option ng-repeat="item in grid.appScope.RequisitionList" ng-selected="item.REQUISITION_NO == row.entity.REQUISITION_NO" value="{{item.REQUISITION_NO}}">{{ item.REQUISITION_NO  }}-{{item.ISSUE_UNIT_NAME}}-{{item.ISSUE_DATE}}</option>' +
                '</select>'
        }
        ,
        {
            name: 'REQUISITION_DATE', field: 'REQUISITION_DATE', displayName: 'Req. Date', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="text" disabled  style="text-align: right;"  ng-model="row.entity.REQUISITION_DATE"  class="pl-sm" />'
        }
        , {
            name: 'REQUISITION_UNIT_NAME', field: 'REQUISITION_UNIT_NAME', displayName: 'Requisition Unit', enableFiltering: true, width: '15%', cellTemplate:
                '<input type="text" style="text-align:right;" disabled  ng-model="row.entity.REQUISITION_UNIT_NAME"  class="pl-sm" />'
        }
        , {
            name: 'ISSUE_NO', field: 'ISSUE_NO', displayName: 'Issue No', enableFiltering: true, width: '13%', cellTemplate:
                '<input type="text"  style="text-align:right;" disabled  ng-model="row.entity.ISSUE_NO"  class="pl-sm" />'
        },
        {
            name: 'ISSUE_DATE', field: 'ISSUE_DATE', displayName: 'Issue Date', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="text" disabled  style="text-align:right;"    ng-model="row.entity.ISSUE_DATE"  class="pl-sm" />'
        },

        {
            name: 'ISSUE_UNIT_NAME', field: 'ISSUE_UNIT_NAME', displayName: 'Issue Unit', enableFiltering: true, width: '16%', cellTemplate:
                '<input type="text"  style="text-align:right;" disabled  ng-model="row.entity.ISSUE_UNIT_NAME"  class="pl-sm" />'
        },
        , {
            name: 'Action', displayName: 'Action', width: '60', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +

                '<button style="margin-bottom: 5px;" ng-show="row.entity.ROW_NO == 0"  ng-click="grid.appScope.addNewRow(row.entity)" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                '</div>'
        }

    ];

    $scope.gridOptionsList.rowTemplate = "<div ng-style='row.entity.Is_Selected == true && grid.appScope.myObj' ng-dblclick=\"grid.appScope.SelectInvoice(row.entity)\" title=\"Please double click to enter products \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"
    $scope.isTrue = false;
    $scope.myObj = {
        "background": "#999",
    }

    //--------------Distribution Product Grid-------------------------------------------------

    $scope.SelectInvoice = (entity) => {
        for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
            $scope.gridOptionsList.data[i].Is_Selected = false;
        }

        if (entity.ROW_NO != 0) {
            $scope.selectedInvoice = entity;
            $scope.selectedInvoice.Is_Selected = true;
            let invoice = $scope.gridOptionsList.data.find(e => e.REQUISITION_NO == entity.REQUISITION_NO);
            let shipper = $scope.gridOptionsList.data.find(e => e.REQUISITION_NO == entity.REQUISITION_NO);
            $scope.gridOptionsProductList.data = invoice.requisitionProductDtlList ?? [];
        }
    }

    $scope.gridOptionsProductList = (gridregistrationservice.GridRegistration("Distribution Info"));
    $scope.gridOptionsProductList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsProductList.data = [];

    $scope.gridOptionsProductList = {
        data: [$scope.GridDefalutProductData()]
    }

    $scope.gridOptionsProductList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '45'
        }
        , { name: 'ENTERED_BY', field: 'ENTERED_BY', visible: false }
        , { name: 'UPDATED_BY', field: 'UPDATED_BY', visible: false }
        , { name: 'DISPATCH_PRODUCT_ID', field: 'DISPATCH_PRODUCT_ID', visible: false }

        , { name: 'SHIPER_QTY', field: 'SHIPPER_QTY', visible: false }
        , { name: 'NO_OF_SHIPER', field: 'NO_OF_SHIPER', visible: false }
        , { name: 'LOOSE_QTY', field: 'LOOSE_QTY', visible: false }
        , { name: 'SHIPPER_WEIGHT', field: 'SHIPPER_WEIGHT', visible: false }
        , { name: 'SHIPPER_VOLUME', field: 'SHIPPER_VOLUME', visible: false }
        , { name: 'PER_PACK_WEIGHT', field: 'PER_PACK_WEIGHT', visible: false }
        , { name: 'PER_PACK_VOLUME', field: 'PER_PACK_VOLUME', visible: false }

        , { name: 'TOTAL_SHIPPER_WEIGHT', field: 'TOTAL_SHIPPER_WEIGHT', visible: false }
        , { name: 'TOTAL_LOOSE_WEIGHT', field: 'TOTAL_LOOSE_WEIGHT', visible: false }
        , { name: 'TOTAL_WEIGHT', field: 'TOTAL_WEIGHT', visible: false }

        , { name: 'TOTAL_SHIPPER_VOLUME', field: 'TOTAL_SHIPPER_VOLUME', visible: false }
        , { name: 'TOTAL_LOOSE_VOLUME', field: 'TOTAL_LOOSE_VOLUME', visible: false }
        , { name: 'TOTAL_VOLUME', field: 'TOTAL_VOLUME', visible: false }
        , { name: 'DISPATCH_PRODUCT_ID', field: 'DISPATCH_PRODUCT_ID', visible: false }

        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '25%', cellTemplate:
                '<input type="text" disabled  style="text-align: left;"  ng-model="row.entity.SKU_NAME"  class="pl-sm" />'
        }
        ,
        {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="text" disabled  style="text-align: right;"  ng-model="row.entity.SKU_CODE"  class="pl-sm" />'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: true, width: '8%', cellTemplate:
                '<input type="number" disabled style="text-align:right;" disabled  ng-model="row.entity.UNIT_TP"  class="pl-sm" />'
        }

        ,
        {
            name: 'ISSUE_QTY', field: 'ISSUE_QTY', displayName: 'Issue Qty', enableFiltering: true, width: '8%', cellTemplate:
                '<input type="number" disabled style="text-align:right;"    ng-model="row.entity.ISSUE_QTY"  class="pl-sm" />'
        },
        {
            name: 'PREV_DISTRIBUTION_QTY', displayName: 'Prev. Dis. Qty', enableFiltering: true, width: '8%', cellTemplate:
                '<input type="number"  style="text-align:right;"  disabled  ng-model="row.entity.DISPATCH_QTY"   class="pl-sm" />'
        },
        {
            name: 'TOTALDISQTY', displayName: 'Total Dis. Qty', enableFiltering: false, width: '9%', cellTemplate:
                '<input type="number"  style="text-align:right;"  disabled  ng-model="row.entity.TOTALDISQTY"   class="pl-sm" />'
        },
        {
            name: 'DISTRIBUTION_QTY', field: 'PENDING_DISPATCH_QTY', displayName: 'Dis. Qty', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;"   ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.PENDING_DISPATCH_QTY"  class="pl-sm" />'
        },
        {
            name: 'DISPATCH_AMOUNT', field: 'DISPATCH_AMOUNT', displayName: 'Dispatch Amount', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-model="row.entity.DISPATCH_AMOUNT"  class="pl-sm" />'
        },

        , {
            name: 'Action', displayName: 'Action', width: '60', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeProductItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                '</div>'
        }

    ];

    ////////Shipper Detail Grid///////////////////////

    $scope.gridOptionsShipperList = (gridregistrationservice.GridRegistration("Shipper Info"));
    $scope.gridOptionsShipperList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsShipperList.data = [];

    $scope.gridOptionsShipperList = {
        data: [$scope.GridDefalutShipperData()]
    }

    $scope.gridOptionsShipperList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'ENTERED_BY', field: 'ENTERED_BY', visible: false }
        , { name: 'UPDATED_BY', field: 'UPDATED_BY', visible: false }

        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '20%', cellTemplate:
                '<input type="text" disabled  style="text-align: left;"  ng-model="row.entity.SKU_NAME"  class="pl-sm" />'
        }
        ,
        {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '15%', cellTemplate:
                '<input type="text" disabled  style="text-align: right;"  ng-model="row.entity.SKU_CODE"  class="pl-sm" />'
        }
        ,

        {
            name: 'DISPATCH_QTY', displayName: 'Dis. Qty', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;"  disabled  ng-model="row.entity.DISPATCH_QTY"   class="pl-sm" />'
        },

        {
            name: 'SHIPPER_QTY', field: 'SHIPPER_QTY', displayName: 'Shipper Qty', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.SHIPPER_QTY"  class="pl-sm" />'
        },
        {
            name: 'NO_OF_SHIPPER', field: 'NO_OF_SHIPPER', displayName: 'No Of Shipper', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-model="row.entity.NO_OF_SHIPPER"  class="pl-sm" />'
        },
        {
            name: 'LOOSE_QTY', field: 'LOOSE_QTY', displayName: 'Loose Qty', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-model="row.entity.LOOSE_QTY"  class="pl-sm" />'
        },
        {
            name: 'SHIPPER_WEIGHT', field: 'SHIPPER_WEIGHT', displayName: 'Shipper Weight', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-model="row.entity.SHIPPER_WEIGHT"  class="pl-sm" />'
        },
        {
            name: 'SHIPPER_VOLUME', field: 'SHIPPER_VOLUME', displayName: 'Shipper Volume', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-model="row.entity.SHIPPER_VOLUME"  class="pl-sm" />'
        },
        {
            name: 'LOOSE_WEIGHT', field: 'LOOSE_WEIGHT', displayName: 'Loose Weight', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-model="row.entity.LOOSE_WEIGHT"  class="pl-sm" />'
        },
        {
            name: 'TOTAL_WEIGHT', field: 'TOTAL_WEIGHT', displayName: 'Total Weight', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-model="row.entity.TOTAL_WEIGHT"  class="pl-sm" />'
        },
        {
            name: 'TOTAL_VOLUME', field: 'TOTAL_VOLUME', displayName: 'Total Volume', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-model="row.entity.TOTAL_VOLUME"  class="pl-sm" />'
        },

    ];

    ////////Dispatch Batch Grid///////////////////////

    $scope.gridOptionsBatchList = (gridregistrationservice.GridRegistration("Shipper Info"));
    $scope.gridOptionsBatchList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsBatchList.data = [];

    $scope.gridOptionsBatchList = {
        data: [$scope.GridDefalutBatchData()]
    }

    $scope.gridOptionsBatchList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'ENTERED_BY', field: 'ENTERED_BY', visible: false }
        , { name: 'UPDATED_BY', field: 'UPDATED_BY', visible: false }

        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '20%', cellTemplate:
                '<input type="text" disabled  style="text-align: left;"  ng-model="row.entity.SKU_NAME"  class="pl-sm" />'
        }
        ,
        {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '15%', cellTemplate:
                '<input type="text" disabled  style="text-align: right;"  ng-model="row.entity.SKU_CODE"  class="pl-sm" />'
        }
        ,

        {
            name: 'REQUISITION_NO', displayName: 'Requisition No', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="text"  style="text-align:right;"  disabled  ng-model="row.entity.REQUISITION_NO"   class="pl-sm" />'
        },
        {
            name: 'BATCH_ID', displayName: 'Batch Id', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="text"  style="text-align:right;"  disabled  ng-model="row.entity.BATCH_ID"   class="pl-sm" />'
        },
        {
            name: 'BATCH_NO', displayName: 'Batch No', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="text"  style="text-align:right;"  disabled  ng-model="row.entity.BATCH_NO"   class="pl-sm" />'
        },
        {
            name: 'DISPATCH_UNIT_ID', field: 'DISPATCH_UNIT_ID', displayName: 'Dis. Unit Id', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.DISPATCH_UNIT_ID"  class="pl-sm" />'
        },
        {
            name: 'DISPATCH_QTY', field: 'DISPATCH_QTY', displayName: 'Dis Qty', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-model="row.entity.DISPATCH_QTY"  class="pl-sm" />'
        },
        {
            name: 'DISPATCH_AMOUNT', field: 'DISPATCH_AMOUNT', displayName: 'Dis. Amount', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-model="row.entity.DISPATCH_AMOUNT"  class="pl-sm" />'
        }

    ];
    $scope.rowNumberGenerate = function () {
        $scope.model.DISTRIBUTION_AMOUNT = 0;
        for (let i = 0; i < $scope.gridOptionsList.data.length; i++) {
            $scope.gridOptionsList.data[i].ROW_NO = i;
        }
    }

    $scope.typeaheadSelectedQty = function (entity) {
        if (entity.PENDING_DISPATCH_QTY == '' || entity.PENDING_DISPATCH_QTY == null || entity.PENDING_DISPATCH_QTY < 0) {
            entity.PENDING_DISPATCH_QTY = 0;
        }
        entity.DISPATCH_AMOUNT = parseFloat(entity.UNIT_TP) * parseInt(entity.PENDING_DISPATCH_QTY)

        entity.NO_OF_SHIPPER = parseInt(entity.PENDING_DISPATCH_QTY / entity.SHIPPER_QTY);
        entity.LOOSE_QTY = entity.PENDING_DISPATCH_QTY % entity.SHIPPER_QTY;

        entity.TOTAL_SHIPPER_WEIGHT = parseFloat(((entity.NO_OF_SHIPPER * entity.SHIPPER_WEIGHT).toFixed(2)));
        entity.TOTAL_SHIPPER_VOLUME = parseFloat(((entity.SHIPPER_VOLUME * entity.NO_OF_SHIPPER).toFixed(2)));

        entity.LOOSE_WEIGHT = parseFloat((entity.PER_PACK_WEIGHT * entity.LOOSE_QTY).toFixed(2));
        entity.LOOSE_VOLUME = parseFloat((entity.PER_PACK_VOLUME * entity.LOOSE_QTY).toFixed(2));

        entity.TOTAL_WEIGHT = parseFloat(entity.TOTAL_SHIPPER_WEIGHT) + parseFloat(entity.LOOSE_WEIGHT);
        entity.TOTAL_VOLUME = parseFloat(entity.TOTAL_SHIPPER_VOLUME) + parseFloat(entity.LOOSE_VOLUME);
        $scope.model.DISPATCH_VOLUME = 0;
        $scope.model.DISPATCH_WEIGHT = 0;
        for (var i = 0; i < $scope.gridOptionsProductList.data.length; i++) {
            $scope.model.DISPATCH_VOLUME += $scope.gridOptionsProductList.data[i].TOTAL_VOLUME;
            $scope.model.DISPATCH_WEIGHT += $scope.gridOptionsProductList.data[i].TOTAL_WEIGHT;
        }
        $scope.rowNumberGenerate();
    };
    //$scope.DeliveryWeightAndVolume = () => {
    //    $scope.TOTAL_DISPATCH_WEIGHT = 0;
    //    $scope.TOTAL_DISPATCH_VOLUME = 0;
    //    $scope.gridOptionsProductList.data.forEach(prod => {
    //        //if (inv.ROW_NO != 0) {
    //        prod.ProductList?.forEach(prod => {
    //                $scope.TOTAL_DELIVERY_WEIGHT += parseFloat(prod.TOTAL_WEIGHT)
    //                $scope.TOTAL_DELIVERY_VOLUME += parseFloat(prod.TOTAL_VOLUMN)
    //            })
    //       // }
    //    })

    //}

    $scope.GetEditDataById = function (value) {
        if (value != undefined && value.length > 0) {
            distributionService.GetEditDataById(value).then(function (data) {
                if (data.data != null && data.data.requisitionIssueDtlList != null && data.data.requisitionIssueDtlList.length > 0) {
                    for (var i = 0; i < data.data.requisitionIssueDtlList.length; i++) {
                        var flag = 0;
                        for (var j = 0; j < $scope.RequisitionList.length; j++) {
                            if (data.data.requisitionIssueDtlList[i].REQUISITION_NO == $scope.RequisitionList[j].REQUISITION_NO) {
                                flag = 1;
                            }
                        }
                        if (flag == 0) {
                            $scope.RequisitionList.push(data.data.requisitionIssueDtlList[i]);
                        }
                    }
                    //$scope.RequisitionList = [...$scope.RequisitionList, ...data.data.requisitionIssueDtlList];
                    $scope.model.COMPANY_ID = data.data.COMPANY_ID;
                    $scope.model.MST_ID = data.data.MST_ID;
                    $scope.model.DISPATCH_NO = data.data.DISPATCH_NO;
                    $scope.model.DISPATCH_DATE = data.data.DISPATCH_DATE;
                    $scope.model.VEHICLE_NO = data.data.VEHICLE_NO;
                    $scope.model.VECHILE_VOLUME = data.data.VECHILE_VOLUME;
                    $scope.model.VEHICLE_TOTAL_VOLUME = data.data.VEHICLE_TOTAL_VOLUME;
                    $scope.model.VECHILE_WEIGHT = data.data.VECHILE_WEIGHT;
                    $scope.model.DISPATCH_VOLUME = data.data.DISPATCH_VOLUME;
                    $scope.model.DISPATCH_WEIGHT = data.data.DISPATCH_WEIGHT;
                    $scope.model.DRIVER_ID = data.data.DRIVER_ID;
                    $scope.model.VEHICLE_DESCRIPTION = data.data.VEHICLE_DESCRIPTION;
                    $scope.model.STATUS = data.data.STATUS;
                    $scope.model.DISPATCH_BY = data.data.DISPATCH_BY;
                    $scope.model.DISPATCH_TYPE = data.data.DISPATCH_TYPE;
                    $scope.model.requisitionIssueDtlList = data.data.requisitionIssueDtlList;

                    $scope.model.DRIVER_NAME = $scope.Vehicles.find(e => e.VEHICLE_NO == data.data.VEHICLE_NO)?.DRIVER_NAME;

                    $scope.gridOptionsList.data = [$scope.GridDefalutData(), ...data.data.requisitionIssueDtlList];

                    for (var i = 0; i < data.data.requisitionIssueDtlList; i++) {
                        requisitionList.push(data.data.requisitionIssueDtlList[i]);
                    }
                    $scope.gridOptionsProductList.data = data.data.requisitionIssueDtlList[0].requisitionProductDtlList;
                    // $scope.addDefaultRow($scope.GridDefalutData());
                    //for (var i = 0; i < $scope.gridOptionsProductList.data.length; i++) {
                    //    $scope.gridOptionsProductList.data[i].PENDING_DISPATCH_QTY = $scope.gridOptionsProductList.data[i].ISSUE_QTY - $scope.gridOptionsProductList.data[i].DISPATCH_QTY;
                    //}

                    distributionService.LoadShipperData($scope.model.COMPANY_ID, $scope.model.MST_ID).then(function (data) {
                        $scope.gridOptionsShipperList.data = data.data;
                    });
                    distributionService.LoadBatchData($scope.model.COMPANY_ID, $scope.model.MST_ID).then(function (data) {
                        $scope.gridOptionsBatchList.data = data.data;
                    });
                    $("#VEHICLE_NO").trigger("change");
                }

                $scope.LoadSKUId();
                $scope.rowNumberGenerate();
                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                console.log(error);
            });
        }
    }

    $scope.addNewRow = (entity) => {
        var count = 0;

        if ($scope.gridOptionsList.data.length > 0 && $scope.gridOptionsList.data[0].REQUISITION_NO != null && $scope.gridOptionsList.data[0].REQUISITION_NO != '' && $scope.gridOptionsList.data[0].REQUISITION_NO != 'undefined') {
            for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
                if ($scope.gridOptionsList.data[i].REQUISITION_NO == entity.REQUISITION_NO) {
                    count++;
                }
            }
            if (count == 1 || count == 0 || entity.SKU_CODE == "") {
                if (entity.REQUISITION_QTY <= 0) {
                    notificationservice.Notification("Requistion quantity must be greater then zero!", "", 'Requistion quantity must be greater then zero!!');
                    $scope.ClearEntity(entity)
                    return;
                }

                $scope.showLoader = true;
                var value = "";

                distributionService.GetDistributionDetailDataById(entity.REQUISITION_NO, $scope.model.DISPATCH_TYPE).then(function (data) {
                    for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
                        $scope.gridOptionsList.data[i].Is_Selected = false;
                    }
                    var newRow = {
                        ROW_NO: 1, Is_Selected: true, COMPANY_ID: $scope.model.COMPANY_ID, MST_ID: $scope.gridOptionsList.data[0].MST_ID, REQUISITION_NO: $scope.gridOptionsList.data[0].REQUISITION_NO, REQUISITION_DATE: $scope.gridOptionsList.data[0].REQUISITION_DATE, REQUISITION_UNIT_NAME: $scope.gridOptionsList.data[0].REQUISITION_UNIT_NAME, REQUISITION_UNIT_ID: $scope.gridOptionsList.data[0].REQUISITION_UNIT_ID, ISSUE_NO: $scope.gridOptionsList.data[0].ISSUE_NO, ISSUE_DATE: $scope.gridOptionsList.data[0].ISSUE_DATE, ISSUE_UNIT_NAME: $scope.gridOptionsList.data[0].ISSUE_UNIT_NAME, ISSUE_UNIT_ID: $scope.gridOptionsList.data[0].ISSUE_UNIT_ID, STATUS: $scope.gridOptionsList.data[0].STATUS, requisitionProductDtlList: data.data[0]
                    }

                    for (var i = 0; i < newRow.requisitionProductDtlList.length; i++) {
                        newRow.requisitionProductDtlList[i].REQUISITION_NO = entity.REQUISITION_NO;
                        newRow.requisitionProductDtlList[i].DISPATCH_DATE = $scope.model.DISPATCH_DATE;
                        newRow.requisitionProductDtlList[i].ISSUE_NO = entity.ISSUE_NO;
                        $scope.typeaheadSelectedQty(newRow.requisitionProductDtlList[i]);
                    }

                    $scope.gridOptionsList.data.push(newRow);
                    $scope.gridOptionsProductList.data = newRow.requisitionProductDtlList;

                    $scope.model.DISPATCH_VOLUME = 0;
                    $scope.model.DISPATCH_WEIGHT = 0;
                    for (var i = 0; i < $scope.gridOptionsProductList.data.length; i++) {
                        $scope.model.DISPATCH_VOLUME += $scope.gridOptionsProductList.data[i].TOTAL_VOLUME;
                        $scope.model.DISPATCH_WEIGHT += $scope.gridOptionsProductList.data[i].TOTAL_WEIGHT;
                    }

                    $scope.gridOptionsList.data[0] = $scope.GridDefalutData();
                    $scope.rowNumberGenerate();

                    $scope.showLoader = false;
                    $scope.LoadSKUId();
                }, function (error) {
                    console.log(error);
                    $scope.showLoader = false;
                });
            }
            else {
                notificationservice.Notification("Requisition already exist!", "", 'Requisition already exist!');
                $scope.ClearEntity(entity)
            }
        }
        //} else {
        //    notificationservice.Notification("Please Enter Valid Region First", "", 'Only Single Row Left!!');

        //}
        $scope.rowNumberGenerate();
    };

    $scope.removeItem = function (entity) {
        if ($scope.gridOptionsList.data.length > 1) {
            var index = $scope.gridOptionsList.data.indexOf(entity);
            if ($scope.gridOptionsList.data.length > 0) {
                $scope.gridOptionsList.data.splice(index, 1);
            }

            $scope.model.DISPATCH_VOLUME = 0;
            $scope.model.DISPATCH_WEIGHT = 0;
            for (var i = 0; i < $scope.gridOptionsProductList.data.length; i++) {
                $scope.model.DISPATCH_VOLUME += $scope.gridOptionsProductList.data[i].TOTAL_VOLUME;
                $scope.model.DISPATCH_WEIGHT += $scope.gridOptionsProductList.data[i].TOTAL_WEIGHT;
            }
            $scope.rowNumberGenerate();
        } else {
            notificationservice.Notification("Only Single Row Left!!", "", 'Only Single Row Left!!');
        }
    }
    $scope.removeProductItem = function (entity) {
        if ($scope.gridOptionsProductList.data.length > 1) {
            var index = $scope.gridOptionsProductList.data.indexOf(entity);
            if ($scope.gridOptionsProductList.data.length > 0) {
                $scope.gridOptionsProductList.data.splice(index, 1);
            }
            $scope.rowNumberGenerate();
        } else {
            notificationservice.Notification("Only Single Row Left!!", "", 'Only Single Row Left!!');
        }
    }

    $scope.EditItem = (entity) => {
        if ($scope.gridOptionsList.data.length > 0) {
            var newRow = {
                ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, MST_ID: entity.MST_ID, DTL_ID: entity.DTL_ID, REGION_ID: entity.REGION_ID, REGION_CODE: entity.REGION_CODE, SKU_ID: entity.SKU_ID, SKU_CODE: entity.SKU_CODE, STATUS: entity.STATUS, REQUISITION_AMOUNT: entity.REQUISITION_AMOUNT, REQUISITION_QTY: entity.REQUISITION_QTY, UNIT_TP: entity.UNIT_TP, STATUS: entity.STATUS
            }
            $scope.gridOptionsList.data[0] = newRow;
        } else {
            notificationservice.Notification("Please Select Valid data", "", 'Only Single Row Left!!');
        }
        $scope.rowNumberGenerate();
        $scope.removeItem(entity);
    };

    $scope.LoadFormData = function () {
        $scope.showLoader = true;
        distributionService.LoadData($scope.model.COMPANY_ID).then(function (data) {
            $scope.relationdata = data.data;
            for (var i = 0; i < $scope.relationdata.length; i++) {
                if ($scope.relationdata[i].MST_ID == parseInt($scope.model.MST_ID)) {
                    $scope.model.MST_ID_ENCRYPTED = $scope.relationdata[i].MST_ID_ENCRYPTED
                }
            }
            $scope.EditData($scope.model.MST_ID_ENCRYPTED)

            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            console.log(error);
            $scope.showLoader = false;
        });
    }
    $scope.EditData = function (MST_ID_ENCRYPTED) {
        window.location = "/Inventory/Distribution/Distribution?Id=" + MST_ID_ENCRYPTED;
    }
    $scope.typeaheadSelectedRequisitionNo = function (entity) {
        const searchIndex = $scope.RequisitionList.findIndex((x) => x.REQUISITION_NO == entity.REQUISITION_NO);
        /* entity.SKU_ID = $scope.Products[searchIndex].SKU_ID;*/
        entity.REQUISITION_DATE = $scope.RequisitionList[searchIndex].REQUISITION_DATE;
        entity.MST_ID = $scope.RequisitionList[searchIndex].MST_ID;
        entity.REQUISITION_UNIT_ID = $scope.RequisitionList[searchIndex].REQUISITION_UNIT_ID;
        entity.REQUISITION_UNIT_NAME = $scope.RequisitionList[searchIndex].REQUISITION_UNIT_NAME;
        entity.ISSUE_NO = $scope.RequisitionList[searchIndex].ISSUE_NO;
        entity.ISSUE_DATE = $scope.RequisitionList[searchIndex].ISSUE_DATE;
        entity.ISSUE_UNIT_ID = $scope.RequisitionList[searchIndex].ISSUE_UNIT_ID;
        entity.ISSUE_UNIT_NAME = $scope.RequisitionList[searchIndex].ISSUE_UNIT_NAME;

        $scope.LoadRequisition();
    };

    $scope.typeaheadSelectedRequisition = function (entity) {
        const searchIndex = $scope.RequisitionList.findIndex((x) => x.REQUISITION_NO == entity.REQUISITION_NO);
        entity.REQUISITION_NO = $scope.RequisitionList[searchIndex].REQUISITION_NO;

        $scope.LoadRequisition();
    };

    $scope.LoadSKUId = function () {
        setTimeout(function () {
            $('#SKU_CODE').trigger('change');
            $('#SKU_ID').trigger('change');
            $("#REQUISITION_UNIT_ID").trigger('change');
        }, 1000)
    }
    $scope.LoadRequisition = function () {
        setTimeout(function () {
            $('#REQUISITION_NO').trigger('change');
        }, 1000)
    }

    $scope.ClearForm = function () {
        window.location.href = "/Inventory/Distribution/Distribution";
    }
    //$scope.DataLoad = function (companyId) {
    //    $scope.showLoader = true;
    //    $scope.SkuList = "";
    //    $scope.GetExistingSku();
    //    setTimeout(function () {
    //        requisitionServices.LoadFilteredProduct($scope.model).then(function (data) {
    //
    //            var dataList = [];
    //            var flag = 0
    //            for (var i = 0; i < data.data.length; i++) {
    //                for (var j = 0; j < $scope.existingSKU.length; j++) {
    //                    if (data.data[i].SKU_ID == $scope.existingSKU[j].SKU_ID) {
    //                        flag = 1;
    //                    }
    //                }
    //                if (flag == 0) {
    //                    dataList.push(data.data[i]);

    //                }
    //                else {
    //                    flag = 0;
    //                }
    //            }
    //            $scope.gridOptionsList.data = dataList;
    //            //if ($scope.existingSKU.length > 0) {
    //            //
    //            //    for (var j = 0; j < $scope.existingSKU.length; j++) {
    //            //        $scope.SkuList += $scope.existingSKU[j].SKU_ID + ",";
    //            //    }
    //            //    notificationservice.Notification(1, 1, $scope.SkuList + " already exist!");
    //            //}
    //            $scope.showLoader = false;

    //        }, function (error) {
    //            alert(error);
    //            $scope.showLoader = false;

    //        });
    //    }, 2000)

    //}
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        distributionService.GetCompany().then(function (data) {
            
            $scope.model.COMPANY_ID = parseFloat(data.data);
            $interval(function () {
                $('#COMPANY_ID').trigger('change');
            }, 800, 4);
            $scope.showLoader = false;
        }, function (error) {
            console.log(error);
            $scope.showLoader = false;
        });
    }
    $scope.UnitLoad = function () {
        $scope.showLoader = true;

        distributionService.GetUnit().then(function (data) {
            $scope.model.UNIT_ID = parseFloat(data.data);
            $scope.showLoader = false;
        }, function (error) {
            console.log(error);
            $scope.showLoader = false;
        });
    }

    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;

        distributionService.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            console.log(error);
            $scope.showLoader = false;
        });
    }
    $scope.VehicleLoad = function () {
        $scope.showLoader = true;
        distributionService.GetVehicleList().then(function (data) {
            $scope.Vehicles = data.data.filter(function (element) { return element.UNIT_ID == $scope.model.UNIT_ID });
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            console.log(error);
            $scope.showLoader = false;
        });
    }
    $scope.LoadVehicleData = function () {
        for (var i = 0; i < $scope.Vehicles.length; i++) {
            debugger;
            if ($scope.Vehicles[i].VEHICLE_NO == $scope.model.VEHICLE_NO) {
                $scope.model.VEHICLE_DESCRIPTION = $scope.Vehicles[i].VEHICLE_DESCRIPTION;
                $scope.model.VECHILE_VOLUME = $scope.Vehicles[i].VEHICLE_TOTAL_VOLUME;
                $scope.model.VECHILE_WEIGHT = $scope.Vehicles[i].VEHICLE_TOTAL_WEIGHT;
                $scope.model.DRIVER_ID = $scope.Vehicles[i].DRIVER_ID;
                $scope.model.DRIVER_NAME = $scope.Vehicles[i].DRIVER_NAME;
            }
        }
    }

    $scope.LoadProductData = function () {
        $scope.showLoader = true;

        distributionService.LoadProductData().then(function (data) {
            $scope.Products = data.data;
            $scope.showLoader = false;
            var _Products = {
                PRODUCT_ID: "0",
                PRODUCT_NAME: "All",
                PRODUCT_CODE: "ALL",
            }

            $scope.Products.push(_Products);
        }, function (error) {
            alert(error);
            console.log(error);

            $scope.showLoader = false;
        });
    }

    $scope.LoadStatus = function () {
        var Active = {
            STATUS: 'Active'
        }
        var InActive = {
            STATUS: 'InActive'
        }
        $scope.Status.push(Active);
        $scope.Status.push(InActive);
    }

    $scope.LoadUNIT_ID = function () {
        $('#UNIT_ID').trigger('change');
    }

    $scope.LoadCOMPANY_ID = function () {
        $('#COMPANY_ID').trigger('change');
    }
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;

        $scope.permissionReqModel = {
            Controller_Name: 'Distribution',
            Action_Name: 'Distribution'
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
            console.log(error);
            $scope.showLoader = false;
        });
    }
    $scope.LoadUnitData = function () {
        $scope.showLoader = true;

        distributionService.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.Unit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });;
            $scope.showLoader = false;
        }, function (error) {
            console.log(error);
            $scope.showLoader = false;
        });
    }

    $scope.LoadRequisitionData = function () {
        $scope.gridOptionsList = {
            data: [$scope.GridDefalutData()]
        }
        $scope.showLoader = true;
        if ($scope.model.DISPATCH_TYPE == "Requisition") {
            distributionService.LoadRequisitionData($scope.model.COMPANY_ID).then(function (data) {
                
                $scope.RequisitionList = data.data[0];
                $scope.showLoader = false;
            }, function (error) {
                console.log(error);
                $scope.showLoader = false;
            });
        }
        else {
            distributionService.LoadStockData($scope.model.COMPANY_ID).then(function (data) {
                
                $scope.RequisitionList = data.data[0];
                $scope.showLoader = false;
            }, function (error) {
                console.log(error);
                $scope.showLoader = false;
            });
        }
    }

    $scope.LoadRequisitionDtlData = function (entity) {
        $scope.showLoader = true;
        var value = "";

        distributionService.GetProductsByRequisitionNo(entity.REQUISITION_NO).then(function (data) {
            
            $scope.gridOptionsProductList.data = data.data;
            for (var i = 0; i < $scope.gridOptionsProductList.data.length; i++) {
                $scope.gridOptionsProductList.data[i].DISTRIBUTION_AMOUNT = $scope.gridOptionsProductList.data[i].ISSUE_AMOUNT;
                $scope.gridOptionsProductList.data[i].DISTRIBUTION_QTY = $scope.gridOptionsProductList.data[i].ISSUE_QTY;
            }
            $scope.gridOptionsList.data[0].requisitionProductDtlList = data.data;
            $scope.gridOptionsList.data[0] = $scope.GridDefalutData();
            $scope.showLoader = false;
            $scope.LoadSKUId();
        }, function (error) {
            console.log(error);
            $scope.showLoader = false;
        });
    }
    $scope.SendNotification = function () {
        $scope.showLoader = true;

        LiveNotificationServices.Notification_Permitted_Users(5, $scope.model.COMPANY_ID, $scope.model.REQUISITION_UNIT_ID).then(function (data) {
            $scope.showLoader = false;

            $scope.Users = data.data;
            $scope.Permitted_Users = [];
            if ($scope.Users != undefined && $scope.Users != null) {
                for (var i = 0; i < $scope.Users.length; i++) {
                    $scope.Permitted_Users.push(JSON.stringify(parseInt($scope.Users[i].USER_ID)));
                }
                connection.invoke("SendMessage", $scope.Permitted_Users, ": New Dispatch has been done and ready to receive. Check out the notification for detail!!!!").catch(function (err) {
                    return console.error(err.toString());
                });
            }
        }, function (error) {
            alert(error);

            $scope.showLoader = false;
        });
    }
    $scope.SaveData = function (model) {
        if ($scope.model.REQUISITION_NO == "") {
            notificationservice.Notification('Requisition no not selected!', "", 'Requisition no not selected!');
            return;
        }
        if ($scope.model.VEHICLE_NO == "") {
            notificationservice.Notification('Vehicle no not selected!', "", 'Vehicle no not selected!');
            return;
        }
        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);
        $scope.model.requisitionIssueDtlList = $scope.gridOptionsList.data;

        for (var i = 0; i < $scope.gridOptionsProductList.data.length; i++) {
            var dispatchQty = $scope.gridOptionsProductList.data[i].DISPATCH_QTY;
            if ($scope.gridOptionsProductList.data[i].PENDING_DISPATCH_QTY == 0) {
                notificationservice.Notification('Distribution Qty must be greater then zero!', "", 'Distribution Qty must be greater then Issue Qty!');
                return;
            }
            if (($scope.gridOptionsProductList.data[i].DISPATCH_PRODUCT_ID == 0 || $scope.gridOptionsProductList.data[i].DISPATCH_PRODUCT_ID == undefined)
                && $scope.gridOptionsProductList.data[i].ISSUE_QTY < ($scope.gridOptionsProductList.data[i].PENDING_DISPATCH_QTY + $scope.gridOptionsProductList.data[i].DISPATCH_QTY)) {
                notificationservice.Notification('Distribution Qty is greater then Issue Qty!', "", 'Distribution Qty is greater then Issue Qty!');

                return;
            }
        }

        if ($scope.model.requisitionIssueDtlList != null) {
            if ($scope.model.requisitionIssueDtlList.length > 1 && $scope.model.requisitionIssueDtlList[0].REQUISITION_NO == "") {
                $scope.model.requisitionIssueDtlList.splice(0, 1);
            }
        }

        $scope.showLoader = true;
        distributionService.AddOrUpdate(model).then(function (data) {
            if (parseInt(data.data) > 0) {
                $scope.showLoader = false;
                $scope.model.MST_ID = data.data;
                $scope.SendNotification();
                notificationservice.Notification(1, 1, 'data save successfully!');
                setTimeout(function () {
                    $scope.LoadFormData();
                }, 1000)

                //window.location.reload();
            }
            else {
                $scope.showLoader = false;
                notificationservice.Notification(data.data, "", data.data);
            }
        });
    }

    //$scope.typeaheadSelectedProduct = function (entity, selectedItem) {
    //    $scope.model.SKU_ID = selectedItem.SKU_ID;
    //    $scope.model.SKU_NAME = selectedItem.SKU_NAME;
    //    $scope.model.SKU_CODE = selectedItem.SKU_CODE;

    //};

    ///*    $scope.DataLoad(0);*/
    $scope.GetPermissionData();
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
    $scope.UnitLoad();
    $scope.LoadUnitData();

    $scope.LoadProductData();
    $scope.LoadStatus();
    $scope.VehicleLoad();
}]);