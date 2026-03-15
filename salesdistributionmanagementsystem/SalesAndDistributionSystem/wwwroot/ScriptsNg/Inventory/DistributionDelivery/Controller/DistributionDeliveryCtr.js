ngApp.controller('ngGridCtrl', ['$scope', 'DistributionDeliveryServices', 'uiGridConstants', 'permissionProvider', 'gridregistrationservice', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, DistributionDeliveryServices,uiGridConstants, permissionProvider, gridregistrationservice, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'

    $scope.showloader = false;
    $scope.buttonDisable = false;
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
    $scope.vehicles = [];
    $scope.TOTAL_DELIVERY_WEIGHT = 0;
    $scope.TOTAL_DELIVERY_VOLUME = 0;
    $scope.default = {
         MST_ID: 0
        , DISTRIBUTION_DATE: $scope.formatDate(new Date())
        , VEHICLE_DESCRIPTION: ''
        , CONFIRMED: 0
        , DISTRIBUTION_NO: ''
        , VEHICLE_NO: ''
        , DRIVER_NAME: ''
        , DRIVER_PHONE: ''
        , TOTAL_VOLUMN: 0
        , TOTAL_WEIGHT: 0
        , VEHICLE_TOTAL_WEIGHT: 0
        , VEHICLE_TOTAL_VOLUME: 0

    }

    $scope.model = {
        MST_ID: 0
        , DISTRIBUTION_DATE: $scope.formatDate(new Date())
        , VEHICLE_DESCRIPTION: ''
        , CONFIRMED: 0
    }
    $scope.PengingsInvocies = [];
    $scope.pendingInvoicesList = [];

    $scope.distRoutes = [];
    $scope.pendingInvoiceList = [];
    $scope.productBatches = [];
    $scope.Customers = [];
    $scope.PendingInvoices = [];
    $scope.PendingSKUs = [];
    $scope.TOTAL_WEIGHT = 0;
    $scope.TOTAL_VOLUME = 0;

    //invoice grid
    $scope.invoiceGridList = (gridregistrationservice.GridRegistration("Invoice"));
    $scope.invoiceGridList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.invoiceGridList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        },
        {
            name: 'is_selected', field: 'is_selected', enableFiltering: false, visible: false
        }
        , {
            name: 'INVOICE_NO', field: 'INVOICE_NO', displayName: 'Invoice No', enableFiltering: false, width: '20%', enableColumnMenu: false
            //,cellTemplate:
            //    `<select ng-disabled="row.entity.ROW_NO != 0" class="select2-single form-control" id="INVOICE_NO" style="width:100%"
            //            name="INVOICE_NO" ng-model="row.entity.INVOICE_NO" ng-change="grid.appScope.GetInvoiceDetails(row.entity)">
            //        <option ng-repeat="item in grid.appScope.pendingInvoiceList" value="{{item.INVOICE_NO}}"
            //                ng-selected="item.INVOICE_NO == row.entity.INVOICE_NO">
            //                                Invoice No: {{item.INVOICE_NO}}
            //                                Date:{{item.INVOICE_DATE}}
            //        </option>
            //    </select>`
        }
        , {
            name: 'CUSTOMER_CODE', field: 'CUSTOMER_CODE', displayName: 'Customer Code', enableFiltering: false, width: '15%', enableColumnMenu: false
        }
        , {
            name: 'CUSTOMER_NAME', field: 'CUSTOMER_NAME', displayName: 'Name', enableFiltering: false, enableColumnMenu: false
        }, {
            name: 'INVOICE_DATE', field: 'INVOICE_DATE', displayName: 'Date', enableFiltering: false, enableColumnMenu: false
        }
        , {
            name: 'ROUTE_NAME', field: 'ROUTE_NAME', displayName: 'Route Name', enableFiltering: false, enableColumnMenu: false, visible: false
        }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO >100000" ng-click="grid.appScope.addNewRowToInvoice(row.entity)" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +


                '<button style="margin-bottom: 5px;"   ng-click="grid.appScope.removeItem(row.entity, grid.appScope.invoiceGridList.data)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +
                '</div>'
        }
    ]
    $scope.invoiceGridList.rowTemplate = `<div ng-style='row.entity.is_selected == true && grid.appScope.rowStyle' ng-dblclick=\"grid.appScope.addNewRowToInvoice(row.entity)\" title=\"Double click to get products \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>`

    $scope.rowStyle = {
        "background": "#999"
    }

    $scope.invoiceGridList.enableGridMenu = false;
    $scope.DefaultInvoice = function () {
        return {
            ROW_NO: 0,
            INVOICE_NO: '',
            CUSTOMER_CODE: '',
            CUSTOMER_NAME: '',
            ROUTE_NAME: ''
        }
    }
    //$scope.invoiceGridList.data = [$scope.DefaultInvoice()];

    $scope.addNewRowToInvoice = function (entity) {
        var count = 0;
        if ($scope.invoiceGridList.data.length > 0 && entity.INVOICE_NO != null && entity.INVOICE_NO != '' && entity.INVOICE_NO != 'undefined') {
            var index =  $scope.invoiceGridList.data.findIndex(x => x.INVOICE_NO == entity.INVOICE_NO);
            if (index != -1) {

                for (var i = 0; i < $scope.invoiceGridList.data[index].Products.length; i++) {
                    $scope.invoiceGridList.data[index].Products[i].ROW_NO = i + 1;
                }
                $scope.productGridList.data = $scope.invoiceGridList.data[index].Products;
                
                let g = 0;

                for (var i = 0; i < $scope.invoiceGridList.data[index].Products.length; i++) {
                    $scope.giftGridList.data =
                        $scope.invoiceGridList.data[index].Gift_Batches?.forEach(e => {
                        e.ROW_NO = ++g;
                    })
                }
                $scope.giftGridList.data = $scope.invoiceGridList.data[index].Gift_Batches;

                $scope.SelectInvoice($scope.invoiceGridList.data[index]);
                $scope.DeliveryWeightAndVolume();
            }

        }
        else {
            notificationservice.Notification("No item has added!", "", 'No item has added!');
        }
    }

    $scope.selectedInvoice = null;
    $scope.SelectInvoice = (entity) => {
        if (entity.ROW_NO == undefined || entity?.ROW_NO != 0) {
            entity.is_selected = true;
            $scope.selectedInvoice = entity;
            let invoice = $scope.invoiceGridList.data.find(e => e.INVOICE_NO == entity.INVOICE_NO);
            $scope.productGridList.data = invoice.Products ?? [];
            $scope.SelectProduct(invoice.Products[0])
            $scope.giftGridList.data = invoice.Gift_Batches ?? [];
        }
        $scope.invoiceGridList.data.forEach(e => e.is_selected = e.INVOICE_NO == entity.INVOICE_NO)
    }

    $scope.GetInvoiceDetails = (entity) => {
        var invoice = $scope.pendingInvoiceList.find(e => e.INVOICE_NO == entity.INVOICE_NO);
        if (invoice != null) {
            entity.CUSTOMER_CODE = invoice.CUSTOMER_CODE ?? "";
            entity.CUSTOMER_NAME = invoice.CUSTOMER_NAME ?? "";
            entity.ROUTE_NAME = invoice.ROUTE_NAME ?? "";
            entity.CUSTOMER_ID = invoice.CUSTOMER_ID;
            entity.INVOICE_NO = invoice.INVOICE_NO;
            entity.MST_ID = invoice.MST_ID;
            entity.INVOICE_UNIT_ID = invoice.INVOICE_UNIT_ID;
            entity.INVOICE_DATE = invoice.INVOICE_DATE;
        }
    }

    //common functions
    $scope.rowNumberGenerate = function (data) {
        for (let i = 0; i < data.length; i++) {
            data[i].ROW_NO = i;
        }
    }
    $scope.rowNumberGenerateList = function (data) {
        for (let i = 0; i < data.length; i++) {
            data[i].ROW_NO = i+1;
        }
    }
    $scope.EditItem = (entity, data) => {
        $scope.removeItem(entity, data);
        if (data.length > 0) {
            data[0] = entity;
        } else {
            notificationservice.Notification("Please Select Valid data", "", 'Only Single Row Left!!');
        }
        $scope.rowNumberGenerate(data);
    };

    $scope.removeItem = function (entity, data) {
        if (data.length > 1) {
            if ($scope.selectedInvoice?.INVOICE_NO == entity.INVOICE_NO) {
                $scope.productGridList.data = []
                $scope.batchGridList.data = []
                $scope.giftGridList.data = []
            }

            var index = data.indexOf(entity);
            data.splice(index, 1);
            $scope.rowNumberGenerate(data);
            $scope.DeliveryWeightAndVolume();
        } else {
            notificationservice.Notification("Only Single Row Left!!", "", 'Only Single Row Left!!');
        }
    }
    // end common functions


    //invoice products grid
    $scope.productGridList = (gridregistrationservice.GridRegistration("Products"));
    $scope.productGridList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.productGridList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50', enableColumnMenu: false
        }, {
            name: 'is_selected', field: 'is_selected', enableFiltering: false, visible: false
        }
        , {
            name: 'SKU_ID', field: 'SKU_ID', displayName: 'SKU ID', enableFiltering: false, width: '100', enableColumnMenu: false
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'Name', enableFiltering: false, width: '200', enableColumnMenu: false
        }
        , {
            name: 'PACK_SIZE', field: 'PACK_SIZE', displayName: 'Pack Size', enableFiltering: false, width: '100', enableColumnMenu: false
        }
        , {
            name: 'SHIPPER_QTY', field: 'SHIPPER_QTY', displayName: 'Shipper Qty', enableFiltering: false, width: '100', enableColumnMenu: false
        }
        , {
            name: 'PENDING_INVOICE_DIST_QTY', field: 'PENDING_INVOICE_DIST_QTY', displayName: 'Pending Invoice Qty', enableFiltering: false, width: '100', enableColumnMenu: false
        }
        , {
            name: 'PENDING_BONUS_DIST_QTY', field: 'PENDING_BONUS_DIST_QTY', displayName: 'Pending Bonus Qty', enableFiltering: false, width: '100', enableColumnMenu: false
        }
        , {
            name: 'INVOICE_QTY', field: 'INVOICE_QTY', displayName: 'Invoice Qty', enableFiltering: false, width: '100', enableColumnMenu: false, visible: false
        }
        , {
            name: 'BONUS_QTY', field: 'BONUS_QTY', displayName: 'Bonus Qty', enableFiltering: false, width: '100', enableColumnMenu: false, visible:false
        }
        , {
            name: 'DISTRIBUTION_QTY', field: 'DISTRIBUTION_QTY', displayName: 'Distr. Inv Qty', enableFiltering: false, width: '100',
            cellTemplate:
                '<input type="number"  name="DISTRIBUTION_QTY" id=DISTRIBUTION_QTY" min="0" ng-model="row.entity.DISTRIBUTION_QTY" ng-change="grid.appScope.ChangeProductQty(row.entity)" />', enableColumnMenu: false
        }
        , {
            name: 'DISTRIBUTION_BONUS_QTY', field: 'DISTRIBUTION_BONUS_QTY', displayName: 'Dist. Bonus Qty', enableFiltering: false, width: '100', enableColumnMenu: false,
            cellTemplate:
                '<input type="number"  name="DISTRIBUTION_BONUS_QTY" id=DISTRIBUTION_BONUS_QTY" min="0" ng-model="row.entity.DISTRIBUTION_BONUS_QTY" ng-change="grid.appScope.ChangeProductQty(row.entity)" />'
        }
        , {
            name: 'TOTAL_DISTRIBUTION_QTY', field: 'TOTAL_DISTRIBUTION_QTY', displayName: 'Total Qty', enableFiltering: false, width: '100', enableColumnMenu: false,
        }, {
            name: 'NO_OF_SHIPPER', field: 'NO_OF_SHIPPER', displayName: 'No of Shipper', enableFiltering: false, width: '100', enableColumnMenu: false
        }, {
            name: 'LOOSE_QTY', field: 'LOOSE_QTY', displayName: 'Loose Qty', enableFiltering: false, width: '100', enableColumnMenu: false
        }, {
            name: 'PER_SHIPPER_WEIGHT', field: 'PER_SHIPPER_WEIGHT', displayName: 'Per Shipper Weight', enableFiltering: false, width: '100', enableColumnMenu: false
        }, {
            name: 'SHIPPER_WEIGHT', field: 'SHIPPER_WEIGHT', displayName: 'Shipper Weight', enableFiltering: false, width: '100', enableColumnMenu: false
        }, {
            name: 'PER_SHIPPER_VOLUME', field: 'PER_SHIPPER_VOLUME', displayName: 'Per Shipper Volume', enableFiltering: false, width: '100', enableColumnMenu: false
        }, {
            name: 'SHIPPER_VOLUMN', field: 'SHIPPER_VOLUMN', displayName: 'Shipper Volume', enableFiltering: false, width: '100', enableColumnMenu: false
        }, {
            name: 'LOOSE_WEIGHT', field: 'LOOSE_WEIGHT', displayName: 'Loose Weight', enableFiltering: false, width: '100', enableColumnMenu: false
        }, {
            name: 'LOOSE_VOLUMN', field: 'LOOSE_VOLUMN', displayName: 'Loose Volume', enableFiltering: false, width: '100', enableColumnMenu: false
        }, {
            name: 'SKU_TOTAL_WEIGHT', field: 'SKU_TOTAL_WEIGHT', displayName: 'Total Weight', enableFiltering: false, width: '100', enableColumnMenu: false
        }, {
            name: 'SKU_TOTAL_VOLUME', field: 'SKU_TOTAL_VOLUME', displayName: 'Total Volume', enableFiltering: false, width: '100', enableColumnMenu: false
        }
    ]

    $scope.productGridList.rowTemplate = `<div ng-style='row.entity.is_selected == true && grid.appScope.rowStyle' ng-dblclick=\"grid.appScope.SelectProduct(row.entity)\" title=\"Double click to get products \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>`

    $scope.productGridList.enableGridMenu = false;
    //$scope.DefaultProduct = function () {
    //    return {
    //        ROW_NO: 0,
    //        SKU_ID: '',
    //        SKU_NAME: '',
    //        UNIT_TP: '',
    //        INVOICE_QTY: '',
    //        BONUS_QTY: '',
    //        DISTRIBUTION_QTY: '',
    //        DISTRIBUTION_BONUS_QTY: '',
    //        TOTAL_DISTRIBUTION_QTY: ''
    //    }
    //}
    $scope.productGridList.data = [];
    $scope.ChangeProductQty_Global = (entity) => {

        if (entity.PENDING_INVOICE_DIST_QTY < entity.DISTRIBUTION_QTY
            || entity.PENDING_BONUS_DIST_QTY < entity.DISTRIBUTION_BONUS_QTY) {
            notificationservice.Notification('Distribution Qty and Bonus cannot be greater than Pending Qty and Bonus!!', 0, 'Vehicle No!!');
            entity.DISTRIBUTION_QTY = entity.PENDING_INVOICE_DIST_QTY;
            entity.DISTRIBUTION_BONUS_QTY = entity.PENDING_BONUS_DIST_QTY;
        }

        entity.TOTAL_DISTRIBUTION_QTY = parseFloat(entity.DISTRIBUTION_QTY) + parseFloat(entity.DISTRIBUTION_BONUS_QTY);
        entity.NO_OF_SHIPPER = parseInt(entity.TOTAL_DISTRIBUTION_QTY / entity.SHIPPER_QTY);
        entity.LOOSE_QTY = entity.TOTAL_DISTRIBUTION_QTY % entity.SHIPPER_QTY;

        entity.SHIPPER_WEIGHT = parseFloat((entity.NO_OF_SHIPPER * entity.PER_SHIPPER_WEIGHT).toFixed(2));
        entity.LOOSE_WEIGHT = parseFloat((entity.LOOSE_QTY * entity.WEIGHT_PER_PACK).toFixed(2));
        entity.TOTAL_WEIGHT = parseFloat(entity.SHIPPER_WEIGHT) + parseFloat(entity.LOOSE_WEIGHT);
        entity.SKU_TOTAL_WEIGHT = entity.TOTAL_WEIGHT + entity.SHIPPER_WEIGHT_UNIT ?? "-";

        entity.SHIPPER_VOLUMN = parseFloat((entity.PER_SHIPPER_VOLUME * entity.NO_OF_SHIPPER).toFixed(2));
        entity.LOOSE_VOLUMN = parseFloat((entity.PACK_VALUE * entity.LOOSE_QTY).toFixed(2));
        entity.TOTAL_VOLUMN = parseFloat(entity.SHIPPER_VOLUMN) + parseFloat(entity.LOOSE_VOLUMN);
        entity.SKU_TOTAL_VOLUME = entity.TOTAL_VOLUMN + entity.SHIPPER_VOLUME_UNIT ?? "-";

       
        $scope.DeliveryWeightAndVolume();
        //var invoice = $scope.invoiceGridList.data.find(e => e.MST_ID == entity.MST_ID);
        //;
        //invoice.Products.find(e => e.SKU_ID == entity.SKU_ID) = entity;
        //product = entity;
    }
    $scope.ChangeProductQty = (entity) => {
        if (entity.PENDING_INVOICE_DIST_QTY < entity.DISTRIBUTION_QTY
            || entity.PENDING_BONUS_DIST_QTY < entity.DISTRIBUTION_BONUS_QTY) {
            notificationservice.Notification('Distribution Qty and Bonus cannot be greater than Pending Qty and Bonus!!', 0, 'Vehicle No!!');
            entity.DISTRIBUTION_QTY = entity.PENDING_INVOICE_DIST_QTY;
            entity.DISTRIBUTION_BONUS_QTY = entity.PENDING_BONUS_DIST_QTY;
        }

        entity.TOTAL_DISTRIBUTION_QTY = parseFloat(entity.DISTRIBUTION_QTY) + parseFloat(entity.DISTRIBUTION_BONUS_QTY);
        entity.NO_OF_SHIPPER = parseInt(entity.TOTAL_DISTRIBUTION_QTY / entity.SHIPPER_QTY);
        entity.LOOSE_QTY = entity.TOTAL_DISTRIBUTION_QTY % entity.SHIPPER_QTY;

        entity.SHIPPER_WEIGHT = parseFloat((entity.NO_OF_SHIPPER * entity.PER_SHIPPER_WEIGHT).toFixed(2));
        entity.LOOSE_WEIGHT = parseFloat((entity.LOOSE_QTY * entity.WEIGHT_PER_PACK).toFixed(2));
        entity.TOTAL_WEIGHT = parseFloat(entity.SHIPPER_WEIGHT) + parseFloat(entity.LOOSE_WEIGHT);
        entity.SKU_TOTAL_WEIGHT = entity.TOTAL_WEIGHT + entity.SHIPPER_WEIGHT_UNIT ?? "-";

        entity.SHIPPER_VOLUMN = parseFloat((entity.PER_SHIPPER_VOLUME * entity.NO_OF_SHIPPER).toFixed(2));
        entity.LOOSE_VOLUMN = parseFloat((entity.PACK_VALUE * entity.LOOSE_QTY).toFixed(2));
        entity.TOTAL_VOLUMN = parseFloat(entity.SHIPPER_VOLUMN) + parseFloat(entity.LOOSE_VOLUMN);
        entity.SKU_TOTAL_VOLUME = entity.TOTAL_VOLUMN + entity.SHIPPER_VOLUME_UNIT ?? "-";

        var index = $scope.invoiceGridList.data.findIndex(x => x.INVOICE_NO == entity.INVOICE_NO);
        if (index != -1) {
            var ind = $scope.invoiceGridList.data[index].Products.findIndex(x => x.SKU_ID == entity.SKU_ID);
            if (ind != -1) {
                $scope.ChangeProductQty_Global($scope.invoiceGridList.data[index].Products[ind]);
            }

        }

        $scope.DeliveryWeightAndVolume();
        //var invoice = $scope.invoiceGridList.data.find(e => e.MST_ID == entity.MST_ID);
        //;
        //invoice.Products.find(e => e.SKU_ID == entity.SKU_ID) = entity;
        //product = entity;
    }

    $scope.DeliveryWeightAndVolume = () => {
        $scope.TOTAL_DELIVERY_WEIGHT = 0;
        $scope.TOTAL_DELIVERY_VOLUME = 0;
        $scope.invoiceGridList.data.forEach(inv => {
            if (inv.Products != undefined) {
                inv.Products?.forEach(prod => {
                    if (prod.TOTAL_WEIGHT != undefined && prod.TOTAL_WEIGHT != undefined) {
                        $scope.TOTAL_DELIVERY_WEIGHT += parseFloat(prod.TOTAL_WEIGHT)
                        $scope.TOTAL_DELIVERY_VOLUME += parseFloat(prod.TOTAL_VOLUMN)
                    }
                   
                  
                })
            }
        })
        //var products = $scope.invoiceGridList.data.reduce((a, inv) => a + inv.Products.reduce);

        //$scope.TOTAL_DELIVERY_WEIGHT = products?.reduce((a, b) => a + b.TOTAL_WEIGHT, 0);
        //$scope.TOTAL_DELIVERY_VOLUME = products?.reduce((a, b) => a + b.TOTAL_VOLUME, 0);

    }

    //batch grid
    $scope.selectedProduct = null;
    $scope.batchGridList = (gridregistrationservice.GridRegistration("Batch"));
    $scope.batchGridList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.batchGridList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , {
            name: 'SKU_ID', field: 'SKU_ID', displayName: 'SKU ID', enableFiltering: false
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'Name', enableFiltering: false
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'TP', enableFiltering: false
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: false
        }
        , {
            name: 'DISTRIBUTION_INVOICE_QTY', field: 'DISTRIBUTION_INVOICE_QTY', displayName: 'Dist. Invoice Qty', enableFiltering: false
        }
        , {
            name: 'DISTRIBUTION_BONUS_QTY', field: 'DISTRIBUTION_BONUS_QTY', displayName: 'Dist. Bonus Qty', enableFiltering: false
        }
    ]


    $scope.SelectProduct = (entity) => {
        if (entity != null && entity != undefined) {
            entity.is_selected = true;
            $scope.selectedProduct = entity;
            let batches = $scope.productBatches.filter(e => e.SKU_ID == entity.SKU_ID
                && e.INVOICE_NO == entity.INVOICE_NO);
            $scope.batchGridList.data = batches ?? [];

            $scope.productGridList.data.forEach(e => e.is_selected = e.SKU_ID == entity.SKU_ID)
        }
    }

    $scope.batchGridList.enableGridMenu = false;
    //$scope.DefaultBatch = function () {
    //    return {
    //        ROW_NO: 0,
    //        SKU_ID: '',
    //        SKU_NAME: '',
    //        UNIT_TP: '',
    //        BATCH_NO: '',
    //        DISTRIBUTION_QTY: '',
    //        DISTRIBUTION_BONUS_QTY: '',
    //    }
    //}

    $scope.batchGridList.data = [];


    //Gift
    $scope.giftGridList = (gridregistrationservice.GridRegistration("Gift"));
    $scope.giftGridList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.giftGridList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, enableColumnMenu: false, width: '50'
        }
        , {
            name: 'GIFT_ITEM_ID', field: 'GIFT_ITEM_ID', displayName: 'Gift ID', enableFiltering: false, enableColumnMenu: false
        }
        , {
            name: 'GIFT_ITEM_NAME', field: 'GIFT_ITEM_NAME', displayName: 'Name', enableFiltering: false, enableColumnMenu: false
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'TP', enableFiltering: false, enableColumnMenu: false
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: false, enableColumnMenu: false
        }
        , {
            name: 'GIFT_QTY', field: 'GIFT_QTY', displayName: 'Qty', enableFiltering: false, enableColumnMenu: false
        }
    ]

    $scope.giftGridList.enableGridMenu = false;
    //$scope.DefaultGift = function () {
    //    return {
    //        ROW_NO: 0,
    //        GIFT_ITEM_ID: '',
    //        GIFT_NAME: '',
    //        UNIT_TP: '',
    //        BATCH_NO: '',
    //        GIFT_QTY: ''
    //    }
    //}
    $scope.giftGridList.data = [];

    // end gift
    //Pending Customer Grid List
    $scope.pendingCustomerGridList = gridregistrationservice.GridRegistration("Pending Customer Grid");

    $scope.pendingCustomerGridList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    };

    $scope.pendingCustomerGridList.columnDefs = [
        {
            name: 'IS_SELECTED_ROW',
            field: 'IS_SELECTED_ROW',
            displayName: 'Select',
            enableFiltering: false,
            width: '5%',
            cellTemplate:
                '<input class="ngSelectionCheckbox" ' +
                'ng-change="grid.appScope.SumOfTotalWeightVolume()" ' +
                'ng-model="row.entity.IS_SELECTED_ROW" ' +
                'type="checkbox" ' +
                'ng-checked="row.entity.IS_SELECTED" ' +
                'style="margin-top:0px !important" />'
        },
        {
            name: '#',
            field: 'ROW_NO',
            displayName: '#',
            enableFiltering: false,
            width: '50'
        },
        {
            name: 'CUSTOMER_ID',
            field: 'CUSTOMER_ID',
            displayName: 'Customer ID',
            enableFiltering: false,
            visible: false
        },
        {
            name: 'CUSTOMER_CODE',
            field: 'CUSTOMER_CODE',
            displayName: 'Customer Code',
            enableFiltering: true,
            width: '10%',
            enableColumnMenu: false
        },
        {
            name: 'CUSTOMER_NAME',
            field: 'CUSTOMER_NAME',
            displayName: 'Name',
            enableFiltering: true,
            width: '20%',
            enableColumnMenu: false
        },
        {
            name: 'PENDING_INVOICE_COUNT',
            field: 'PENDING_INVOICE_COUNT',
            displayName: 'Left Invoices',
            enableFiltering: true,
            width: '12%',
            enableColumnMenu: false
        },
        {
            name: 'TOTAL_WEIGHT',
            field: 'TOTAL_WEIGHT',
            displayName: 'Total Weight (KG)',
            footerCellTemplate:
                '<div class="ui-grid-cell-contents" style="background-color: #9ed4a0;color: Black">' +
                '{{grid.appScope.TOTAL_WEIGHT}}</div>',
            aggregationHideLabel: true,
            enableFiltering: true,
            width: '12%',
            enableColumnMenu: false,
            visible: true
        },
        {
            name: 'TOTAL_VOLUME',
            field: 'TOTAL_VOLUME',
            displayName: 'Total Volume (CFT)',
            footerCellTemplate:
                '<div class="ui-grid-cell-contents" style="background-color: #9ed4a0;color: Black">' +
                '{{grid.appScope.TOTAL_VOLUME}}</div>',
            aggregationHideLabel: true,
            enableFiltering: true,
            width: '12%',
            enableColumnMenu: false,
            visible: true
        },
        {
            name: 'NET_INVOICE_AMOUNT',
            field: 'NET_INVOICE_AMOUNT',
            displayName: 'Invoice Value',
            aggregationHideLabel: true,
            enableFiltering: true,
            width: '12%',
            enableColumnMenu: false,
            visible: true
        },
        {
            name: 'NET_INVOICE_QTY',
            field: 'NET_INVOICE_QTY',
            displayName: 'Invoice Qty',
            aggregationHideLabel: true,
            enableFiltering: true,
            width: '12%',
            enableColumnMenu: false,
            visible: true
        },
        {
            name: 'SERIAL_NO',
            field: 'SERIAL_NO',
            displayName: 'Serial',
            enableFiltering: true,
            width: '10%',
            enableColumnMenu: false,
            visible: true
        }
    ];

    // Enable footer at the bottom of grid
    $scope.pendingCustomerGridList.showColumnFooter = true;


    
    // Pending Invoice Grid List
    $scope.pendingInvoiceGridList = (gridregistrationservice.GridRegistration("PendingInvoiceList"));
    $scope.pendingInvoiceGridList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.pendingInvoiceGridList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        },
        {
            name: 'is_selected', field: 'is_selected', enableFiltering: false, visible: false
        },
        {
            name: 'INVOICE_NO', field: 'INVOICE_NO', displayName: 'Invoice No', enableFiltering: true, width: '25%', enableColumnMenu: false
        }
        , {
            name: 'CUSTOMER_CODE', field: 'CUSTOMER_CODE', displayName: 'Customer Code', enableFiltering: true, width: '15%', enableColumnMenu: false
        }
        , {
            name: 'CUSTOMER_NAME', field: 'CUSTOMER_NAME', displayName: 'Name', enableFiltering: true, enableColumnMenu: false
        }, {
            name: 'INVOICE_DATE', field: 'INVOICE_DATE', displayName: 'Date', enableFiltering: true, enableColumnMenu: false
        }
        , {
            name: 'ROUTE_NAME', field: 'ROUTE_NAME', displayName: 'Route Name', enableFiltering: true, width: '25%', enableColumnMenu: false, visible: false
        }
    ]
   /* $scope.pendingInvoiceGridList.rowTemplate = `<div ng-style='row.entity.is_selected == true && grid.appScope.rowStyle' ng-dblclick=\"grid.appScope.pendingInvoiceSelect(row.entity)\" title=\"Double click to get products \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>`*/
    //$scope.pendingInvoiceSelect = function (entity) {
    //    $scope.GetInvoiceDetails(entity);
    //    $scope.addNewRowToInvoice(entity);
    //}
    // end pending Invoice grid list


    //im: intial//
    $scope.LoadData = function () {
        DistributionDeliveryServices.GetUnit().then(function (response) {
            let company = response.data[0];
            $scope.model.COMPANY_NAME = company.COMPANY_NAME
            $scope.model.UNIT_NAME = company.UNIT_NAME
            $scope.model.COMPANY_ID = company.COMPANY_ID
            $scope.model.INVOICE_UNIT_ID = company.UNIT_ID
       
            DistributionDeliveryServices.GetVehicles(company.COMPANY_ID).then(function (vehicleRes) {
                $scope.vehicles = vehicleRes.data.filter(function (element) { return element.UNIT_ID == $scope.model.INVOICE_UNIT_ID });
                //$scope.TriggerSelects();
            }, function (error) {

            })

            DistributionDeliveryServices.GetDistributionRoutes().then(function (routeResponse) {
                $scope.distRoutes = routeResponse.data[0];
            }, error => {

            })
        }, function (error) {

        });
    }
    $scope.LoadData();



    //$scope.GetCustomer = function (routeId) {
    //    if (routeId.length != 0) {
    //        DistributionDeliveryServices.GetCustomer(routeId).then(response => {
    //            $scope.Customers = response.data;
    //        })
    //    }
    //}


    //im//
    $scope.GetCustomer = function (routeId) {
        if (routeId == undefined) {
            notificationservice.Notification("No route Found!!", '1', 'Data Saved Successfully');
            return;
        }
        if (routeId.length != 0) {
            DistributionDeliveryServices.GetCustomer(routeId).then(response => {
                if (response.data == "No invoice Found!" || response.data == "No Route Found!") {
                    notificationservice.Notification(response.data, '1', 'Data Saved Successfully');
                    return;
                }
                $scope.Customers = [];
                $scope.PendingInvoices = response.data[0];
                $scope.PendingSKUs = response.data[1];
                for (var i = 0; i < response.data[0].length; i++) {
                    var _is_exist = $scope.Customers.filter(x => x.CUSTOMER_ID == response.data[0][i].CUSTOMER_ID);
                    if (_is_exist.length > 0) {
                        _is_exist[0].PENDING_INVOICE_COUNT++;
                        _is_exist[0].NET_INVOICE_AMOUNT = parseFloat(_is_exist[0].NET_INVOICE_AMOUNT + parseFloat(response.data[0][i].NET_INVOICE_AMOUNT)).toFixed(2);
                        _is_exist[0].NET_INVOICE_QTY = parseFloat(_is_exist[0].NET_INVOICE_QTY + parseFloat(response.data[0][i].NET_INVOICE_QTY)).toFixed(2);

                    } else {
                        var Customer_ = {
                            ROW_NO: 0,
                            CUSTOMER_ID: response.data[0][i].CUSTOMER_ID,
                            CUSTOMER_CODE: response.data[0][i].CUSTOMER_CODE,
                            CUSTOMER_NAME: response.data[0][i].CUSTOMER_NAME,
                            SERIAL_NO: parseInt(response.data[0][i].SERIAL_NO),
                            NET_INVOICE_AMOUNT: 0,
                            NET_INVOICE_QTY: 0,
                            PENDING_INVOICE_COUNT: 1,
                            TOTAL_WEIGHT: 0,
                            TOTAL_VOLUME: 0,
                            IS_SELECTED_ROW: false
                        }

                        $scope.Customers.push(Customer_);
                    }

                    //  01/12/2022 __
                }
                for (var i = 0; i < response.data[1].length; i++) {
                    var _is_exist = $scope.Customers.filter(x => x.CUSTOMER_ID == response.data[1][i].CUSTOMER_ID);
                    if (_is_exist.length > 0) {
                        _is_exist[0].TOTAL_WEIGHT = parseFloat(_is_exist[0].TOTAL_WEIGHT + (parseFloat(response.data[1][i].PER_SHIPPER_WEIGHT) * parseFloat(response.data[1][i].TOTAL_QTY)) / parseFloat(response.data[1][i].SHIPPER_QTY)).toFixed(2);
                        _is_exist[0].TOTAL_VOLUME = parseFloat(_is_exist[0].TOTAL_VOLUME + (parseFloat(response.data[1][i].PER_SHIPPER_VOLUME) * parseFloat(response.data[1][i].TOTAL_QTY)) / parseFloat(response.data[1][i].SHIPPER_QTY)).toFixed(2);

                    }

                }
                $scope.pendingCustomerGridList.data = $scope.Customers;
                $scope.SumOfTotalWeightVolumeTotal();
                $scope.rowNumberGenerateList($scope.pendingCustomerGridList.data);
            })
        } else {
            notificationservice.Notification("No route Found!!", '1', 'Data Saved Successfully');
            return;
        }

    }

    $scope.LoadInvoices = function () {
        var count = 1;
        $scope.pendingInvoiceGridList.data = [];
        $scope.invoiceGridList.data = [];
        $scope.pendingInvoicesList = [];
        $scope.pendinglist = [];
        for (var i = 0; i < $scope.Customers.length; i++) {
            if ($scope.Customers[i].IS_SELECTED_ROW == true) {
                var dt = $scope.PendingInvoices.filter(x => x.CUSTOMER_ID == $scope.Customers[i].CUSTOMER_ID)
                if (dt.length > 0) {
                    for (let j = 0; j < dt.length; j++) {
                        dt[j].ROW_NO = count;
                        $scope.pendingInvoicesList.push(dt[j].INVOICE_NO);
                        $scope.pendinglist.push(dt[j]);
                        count++;
                    }
                }

            }
        }
        if ($scope.pendingInvoicesList.length > 0) {
            $scope.Invoices = [];
            $scope.Invoices = $scope.pendingInvoicesList;
            DistributionDeliveryServices.GetProductsByInvoices($scope.Invoices).then(response => {
                for (var k = 0; k < response.data.length; k++) {
                    var index = $scope.pendinglist.findIndex(x => x.INVOICE_NO == response.data[k][0].INVOICE_NO);
                    if (index != -1) {
                        $scope.pendinglist[index].Products = response.data[k];
                        for (var i = 0; i < $scope.pendinglist[index].Products.length; i++) {
                            $scope.pendinglist[index].Products[i].DISTRIBUTION_BONUS_QTY = $scope.pendinglist[index].Products[i].PENDING_BONUS_DIST_QTY;
                            $scope.pendinglist[index].Products[i].DISTRIBUTION_QTY = $scope.pendinglist[index].Products[i].PENDING_INVOICE_DIST_QTY;
                            $scope.ChangeProductQty($scope.pendinglist[index].Products[i]);
                        }
                    }
                }
                $scope.DeliveryWeightAndVolume();

                DistributionDeliveryServices.GetProductsByGifts($scope.Invoices).then(response => {
                    for (var k = 0; k < response.data.length; k++) {
                        if (response.data[k].length > 0) {

                            var index = $scope.pendinglist.findIndex(x => x.INVOICE_NO == response.data[k][0].INVOICE_NO)
                            if (index != -1) {
                                $scope.pendinglist[index].Gift_Batches = response.data;
                            }
                        }
                    }
                    $scope.invoiceGridList.data = $scope.pendinglist;
                });
            });
        }
        //$scope.PendingInvoices.push(dt[j]);

       // $scope.pendingInvoiceList = $scope.pendingInvoiceGridList.data;
        //$scope.pendingInvoiceList.data = $scope.pendingInvoiceList.data.map((e, index) => ({ ...e, "ROW_NO": index + 1 }));


    }



    //sum of total volume and weight
    $scope.SumOfTotalWeightVolumeTotal = function () {
        $scope.TOTAL_WEIGHT = 0;
        $scope.TOTAL_VOLUME = 0;
        for (var i = 0; i < $scope.pendingCustomerGridList.data.length; i++) {
            $scope.TOTAL_WEIGHT = parseFloat(parseFloat($scope.TOTAL_WEIGHT) + parseFloat($scope.pendingCustomerGridList.data[i].TOTAL_WEIGHT)).toFixed(2);
            $scope.TOTAL_VOLUME = parseFloat(parseFloat($scope.TOTAL_VOLUME) + parseFloat($scope.pendingCustomerGridList.data[i].TOTAL_VOLUME)).toFixed(2);

        }
    }
    $scope.SumOfTotalWeightVolume = function () {
        $scope.TOTAL_WEIGHT = 0;
        $scope.TOTAL_VOLUME = 0;
        for (var i = 0; i < $scope.pendingCustomerGridList.data.length; i++) {
            if ($scope.pendingCustomerGridList.data[i].IS_SELECTED_ROW == true) {
                $scope.TOTAL_WEIGHT = parseFloat(parseFloat($scope.TOTAL_WEIGHT) + parseFloat($scope.pendingCustomerGridList.data[i].TOTAL_WEIGHT)).toFixed(2);
                $scope.TOTAL_VOLUME = parseFloat(parseFloat($scope.TOTAL_VOLUME) + parseFloat($scope.pendingCustomerGridList.data[i].TOTAL_VOLUME)).toFixed(2);

            }
          
        }
    }

    $scope.GetPendingInvoicesByCustomer = function () {
        //$scope.pendingInvoiceList = $scope.PendingInvoices.filter(e => e.CUSTOMER_ID == $scope.model.CUSTOMER_ID);
        $scope.pendingInvoiceList = $scope.PendingInvoices
            .filter(e => $scope.model.CUSTOMER_ID.includes(e.CUSTOMER_ID.toString()));
    }

    //$scope.GetPendingInvoices = function () {
    //    DistributionDeliveryServices.GetInvoices().then(invData => {
    //        $scope.PendingInvoices = invData.data[0];
    //        $scope.pendingInvoiceGridList.data = invData.data[0].map((e, index) => ({ ...e, "ROW_NO": index + 1}));
    //    })
    //}

    //$scope.GetPendingInvoices();
    $scope.GetEditDataById = (id) => {
        if (id != "" && id != null && id != 0) {
            $scope.showLoader = true;
            DistributionDeliveryServices.GetEditDataById(id).then(response => {
                //$scope.model = response.data;
                $scope.model.MST_ID = response.data.MST_ID;
                $scope.model.VEHICLE_NO = response.data.VEHICLE_NO;
                $scope.model.DISTRIBUTION_NO = response.data.DISTRIBUTION_NO;
                $scope.model.VEHICLE_DESCRIPTION = response.data.VEHICLE_DESCRIPTION;
                $scope.model.VEHICLE_TOTAL_VOLUME = response.data.VEHICLE_TOTAL_VOLUME;
                $scope.model.VEHICLE_TOTAL_WEIGHT = response.data.VEHICLE_TOTAL_WEIGHT;
                $scope.model.WEIGHT_UNIT_NAME = response.data.WEIGHT_UNIT;
                $scope.model.VOLUME_UNIT_NAME = response.data.VOLUME_UNIT;
                $scope.model.DRIVER_NAME = response.data.DRIVER_NAME;
                $scope.model.DRIVER_PHONE = response.data.DRIVER_PHONE;
                //$scope.model.DIST_ROUTE_ID = String(response.data.DIST_ROUTE_ID).split(',');
                $scope.model.DIST_ROUTE_ID = [...new Set(response.data.Invoices.map(invoice => invoice.ROUTE_ID))].map(String);

                $scope.model.CONFIRMED = response.data.CONFIRMED;
                $scope.TriggerRouteSelectMenu();
                $scope.model.REMARKS = response.data.REMARKS;
                $scope.model.DRIVER_ID = response.data.DRIVER_ID;
                $scope.model.DISTRIBUTION_DATE = $scope.formatDate(response.data.DISTRIBUTION_DATE)
                $scope.pendingInvoiceList = [...$scope.pendingInvoiceList, ...response.data.Invoices];
                $scope.invoiceGridList.data = response.data.Invoices;
                $scope.SelectInvoice(response.data.Invoices[0]);
                $scope.batchGridList.data = response.data.Invoices[0]?.Products[0]?.Batches;
                $scope.DeliveryWeightAndVolume();
                $scope.EditMode = true;
                $scope.showLoader = false;
            })
        }
    }

    $scope.SetVehicleDetails = function () {
        if ($scope.model.VEHICLE_NO != undefined && $scope.model.VEHICLE_NO != null
            && $scope.model.VEHICLE_NO != '') {
            let vehicle = $scope.vehicles.find(e => e.VEHICLE_NO == $scope.model.VEHICLE_NO);
            $scope.model.VEHICLE_DESCRIPTION = vehicle?.VEHICLE_DESCRIPTION
            $scope.model.DRIVER_NAME = vehicle?.DRIVER_NAME
            $scope.model.DRIVER_ID = vehicle?.DRIVER_ID
            $scope.model.DRIVER_PHONE = vehicle?.DRIVER_PHONE
            $scope.model.VEHICLE_TOTAL_VOLUME = vehicle?.VEHICLE_TOTAL_VOLUME
            $scope.model.VOLUME_UNIT_NAME = vehicle?.VOLUME_UNIT_NAME
            $scope.model.VEHICLE_TOTAL_WEIGHT = vehicle?.VEHICLE_TOTAL_WEIGHT
            $scope.model.WEIGHT_UNIT_NAME = vehicle?.WEIGHT_UNIT_NAME
        }
    }

    $scope.TriggerSelects = function () {
        setTimeout(function () {
            $("#VECHILE_NO").trigger("change");
            //$("#DIST_ROUTE_ID").trigger("change");
            //$("#SUPPLIER_ID").trigger("change");
        }, 500)
    }
    $scope.TriggerRouteSelectMenu = function () {
        setTimeout(function () {
            //$("#VECHILE_NO").trigger("change");
            $("#DIST_ROUTE_ID").trigger("change");
            //$("#SUPPLIER_ID").trigger("change");
        }, 500)
    }
    $scope.EditMode = false;

    $scope.SaveData = (model) => {
        $scope.buttonDisable = true;
        model.Invoices = $scope.invoiceGridList.data;
        model.TOTAL_VOLUMN = $scope.TOTAL_DELIVERY_VOLUME;
        model.TOTAL_WEIGHT = $scope.TOTAL_DELIVERY_WEIGHT;
        model.INVOICE_UNIT_ID = parseInt(model.INVOICE_UNIT_ID);
        model.ROW_ID = 0;

        model.DIST_ROUTE_ID = model.DIST_ROUTE_ID != undefined && model.DIST_ROUTE_ID != "" ? parseInt(model.DIST_ROUTE_ID) : null;
        if (model.VEHICLE_NO == null || model.VEHICLE_NO == "") {
            notificationservice.Notification('Please select Vehicle No!!', 0, 'Vehicle No!!');
            $scope.buttonDisable = false;
            return;
        }
        if (model.DIST_ROUTE_ID == null) {
            notificationservice.Notification('Please select Route First!!', 0, 'Route!!');
            $scope.buttonDisable = false
            return;
        }

        model.DRIVER_ID = JSON.stringify(parseInt(model.DRIVER_ID));
        model.Invoices.forEach(e => {
            e.CUSTOMER_ID = parseInt(e.CUSTOMER_ID);
            e.INVOICE_UNIT_ID = parseInt(e.INVOICE_UNIT_ID);
            e.COMPANY_ID = parseInt(e.COMPANY_ID);
            e.ROUTE_ID = parseInt(e.ROUTE_ID);
            //e.DIST_ROUTE_ID = parseInt(model.DIST_ROUTE_ID);
            e.DIST_ROUTE_ID = parseInt(e.ROUTE_ID);
            


            e.Products.forEach(prod => {
                if (prod.PENDING_INVOICE_DIST_QTY < prod.DISTRIBUTION_QTY
                    || prod.PENDING_BONUS_DIST_QTY < prod.DISTRIBUTION_BONUS_QTY) {
                    notificationservice.Notification('Distribution Qty and Bonus cannot be greater than Pending Qty and Bonus!!', 0, 'Vehicle No!!');
                    $scope.buttonDisable = false;
                    return;
                }

                prod.SHIPPER_VOLUMN = isNaN(prod.SHIPPER_VOLUMN) || prod.SHIPPER_VOLUMN == null ? 0 : prod.SHIPPER_VOLUMN;
                prod.SHIPPER_WEIGHT = isNaN(prod.SHIPPER_WEIGHT) || prod.SHIPPER_WEIGHT == null ? 0 : prod.SHIPPER_WEIGHT
                prod.TOTAL_WEIGHT = isNaN(prod.TOTAL_WEIGHT) || prod.TOTAL_WEIGHT == null ? 0 : prod.TOTAL_WEIGHT
                prod.PACK_UNIT = prod.PACK_UNIT ?? 0
                prod.SKU_TOTAL_VOLUME = prod.SKU_TOTAL_VOLUME.toString();
                prod.SKU_TOTAL_WEIGHT = prod.SKU_TOTAL_WEIGHT.toString();
                prod.PACK_VALUE = 0;
                prod.WEIGHT_PER_PACK = 0;


            })
        });
        
        if (!$scope.EditMode) {
            model.Invoices.forEach(e => {
                delete e.MST_ID
            });
        }
        DistributionDeliveryServices.AddOrUpdate(model).then(res => {

            if (res.data.status == "1") {
                notificationservice.Notification(res.data.status, '1', 'Data Saved Successfully with dispatch No: ' + res.data.codeNo);

                $scope.showLoader = false;
                $scope.model = { ...$scope.model, ...$scope.default }
                $scope.invoiceGridList.data = [];
                $scope.productGridList.data = [];
                $scope.batchGridList.data = [];
                $scope.giftGridList.data = [];
                $scope.model.Invoices = [];
                $scope.model.DIST_ROUTE_ID = String("0").split(',');
                $timeout(function () {
                    $('.select2-single-multi').trigger('change');
                });

                //$scope.GetEditDataById(res.data.parent);
                //let input = document.getElementById('DISTRIBUTION_NO');
                //input.style.border = '3px solid green';
                //window.location = "/Inventory/DistributionDelivery/Delivery?Id=" + res.data.parent;
                //window.location = "/Inventory/DistributionDelivery/Delivery";
                // Define the new URL you want to set
                // Get the current location's origin (base URL)
                const baseUrl = window.location.origin;

                // Define the path you want to navigate to
                const newPath = "Inventory/DistributionDelivery/Delivery";

                // Combine the base URL with the new path
                const newUrl = baseUrl + "/" + newPath;

                // Use history.replaceState to change the URL without reloading the page
                history.replaceState(null, null, newUrl);


                $scope.buttonDisable = false;
            }
            else {
                notificationservice.Notification(res.data.status, '1', 'Data Saved Successfully');

                $scope.showLoader = false;
                $scope.buttonDisable = false;
            }
        }, error => {
            $scope.buttonDisable = false;

        })
    }

    $scope.Confirm = () => {
        if (window.confirm("You cannot edit after confirm!")) {
            $scope.model.CONFIRMED = 1;
            $scope.SaveData($scope.model);
        }
    }

    $scope.LoadListPage = function () {
        window.location.href = "/Inventory/DistributionDelivery/DeliveryList";
    }

    $scope.LoadNonConfirmedListPage = function () {
        window.location.href = "/Inventory/DistributionDelivery/NonConfirmedList";
    }

    $scope.PrintReport = function () {
        var href = "/Inventory/Report/GenerateReport?ReportId=" +
            $scope.model.reportIdEncryptedSelected +
            "&Color=color" +
            '&DISPATCH_NO=' +
            $scope.model.DISTRIBUTION_NO +
            "&REPORT_ID=46" +
            "&REPORT_EXTENSION=pdf";
        window.open(href, '_blank');

    }
    $scope.ClearForm = () => {
        window.location = "/Inventory/DistributionDelivery/Delivery";
    }

    $scope.GetPermissionData = function () {
        //$scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'DistributionDelivery',
            Action_Name: 'Delivery'
        }
        permissionProvider.GetPermission($scope.permissionReqModel).then(function (data) {
            $scope.getPermissions = data.data;
            $scope.model.ADD_PERMISSION = $scope.getPermissions.adD_PERMISSION;
            $scope.model.EDIT_PERMISSION = $scope.getPermissions.ediT_PERMISSION;
            $scope.model.DELETE_PERMISSION = $scope.getPermissions.deletE_PERMISSION;
            $scope.model.CONFIRM_PERMISSION = $scope.getPermissions.confirM_PERMISSION;
            $scope.model.LIST_VIEW = $scope.getPermissions.lisT_VIEW;
            $scope.model.DETAIL_VIEW = $scope.getPermissions.detaiL_VIEW;
            $scope.model.DOWNLOAD_PERMISSION = $scope.getPermissions.downloaD_PERMISSION;
            $scope.model.USER_TYPE = $scope.getPermissions.useR_TYPE;

        //    $scope.showLoader = false;
        }, function (error) {
            alert(error);
            //$scope.showLoader = false;

        });
    }
    $scope.GetPermissionData();
}])