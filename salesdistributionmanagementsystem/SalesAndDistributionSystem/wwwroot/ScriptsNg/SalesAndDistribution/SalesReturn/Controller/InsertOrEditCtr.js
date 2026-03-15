ngApp.controller('ngGridCtrl', ['$scope', 'InsertOrEditServices', 'LiveNotificationServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InsertOrEditServices, LiveNotificationServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {}
    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Return_Types = [];
    $scope.modelPartial = [];
    $scope.invoices = [];

    $scope.model.RETURN_TYPE = 'Full';
    $scope.gridOptionsListComboGiftPartial = [];
    $scope.gridOptionsListComboBonusPartial = [];
    $scope.gridOptionsListGiftPartial = [];

    $scope.gridOptionsListBonusPartial = [];
    $scope.gridOptionsListIssuePartial = [];
    $scope.GetCurrentDate = function () {
        const today = new Date();
        const yyyy = today.getFullYear();
        let mm = today.getMonth() + 1; // Months start at 0!
        let dd = today.getDate();

        if (dd < 10) dd = '0' + dd;
        if (mm < 10) mm = '0' + mm;

        $scope.model.DATE_FROM = dd + '/' + mm + '/' + yyyy;
        $scope.model.DATE_TO = dd + '/' + mm + '/' + yyyy;

    }

    $scope.GetCurrentDate();



    var connection = new signalR.HubConnectionBuilder().withUrl("/notificationhub").build();
    connection.start();

    $scope.gridOptionsListDtlsPartial = (gridregistrationservice.GridRegistration("Sales Return Partial"));
    $scope.gridOptionsListDtlsPartial.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }

    $scope.gridOptionsListDtlsPartial.columnDefs = [
        {
            
            name: 'RETURN_CHECK', field: 'RETURN_CHECK', displayName: 'Select', enableFiltering: false, width: '9%', cellTemplate:
                '<input class=\"ngSelectionCheckbox\" ng-click="grid.appScope.selectionOfPermission(row.entity,\'LIST_VIEW_CHECK\')" ng-model="row.entity.RETURN_CHECK" type=\"checkbox\" ng-checked=\"row.entity.RETURN_CHECK\"  style="margin-top:0px !important" />'
     
        },
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'DTL_ID', field: 'DTL_ID', visible: false }

        , { name: 'INVOICE_UNIT_ID', field: 'INVOICE_UNIT_ID', visible: false }
        , { name: 'MST_ID', field: 'MST_ID', visible: false }
        , { name: 'SKU_ID', field: 'SKU_ID', visible: false }


        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: false, width: '15%'
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: false, width: '15%'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: false, width: '12%'
        }
        , {
            name: 'INVOICE_QTY', field: 'INVOICE_QTY', displayName: 'Return Qty', enableFiltering: false, width: '12%', cellTemplate:
                '<input type="number" style="text-align:right;" max="{{row.entity.RETURN_QTY_MAX}}"  ng-model="row.entity.INVOICE_QTY"  class="pl-sm" />'
        }
        , {
            name: 'TP_AMOUNT', field: 'TP_AMOUNT', displayName: 'TP Amt', enableFiltering: false, width: '12%'
        }
        , {

            name: 'TOTAL_AMOUNT', field: 'TOTAL_AMOUNT', displayName: 'Total Amt', enableFiltering: false, width: '15%'
        }
        , {

            name: 'NET_PRODUCT_AMOUNT', field: 'NET_PRODUCT_AMOUNT', displayName: 'Net Product Amt', enableFiltering: false, width: '15%'
        }



    ];





    $scope.gridOptionsListDtls = (gridregistrationservice.GridRegistration("Sales Order Info"));
    $scope.gridOptionsListDtls.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }

    $scope.gridOptionsListDtls.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'DTL_ID', field: 'DTL_ID', visible: false }

        , { name: 'INVOICE_UNIT_ID', field: 'INVOICE_UNIT_ID', visible: false }
        , { name: 'MST_ID', field: 'MST_ID', visible: false }
        , { name: 'SKU_ID', field: 'SKU_ID', visible: false }


        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: false, width: '15%'
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: false, width: '15%'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: false, width: '12%'
        }
        , {
            name: 'INVOICE_QTY', field: 'INVOICE_QTY', displayName: 'Return Qty', enableFiltering: false, width: '12%'
        }
        , {
            name: 'TP_AMOUNT', field: 'TP_AMOUNT', displayName: 'TP Amt', enableFiltering: false, width: '12%'
        }
        , {

            name: 'TOTAL_AMOUNT', field: 'TOTAL_AMOUNT', displayName: 'Total Amt', enableFiltering: false, width: '15%'
        }
        , {

            name: 'NET_PRODUCT_AMOUNT', field: 'NET_PRODUCT_AMOUNT', displayName: 'Net Product Amt', enableFiltering: false, width: '15%'
        }



    ];
    $scope.gridOptionsListBonus = (gridregistrationservice.GridRegistration("Sales Bonus Info"));
    $scope.gridOptionsListBonus.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsListBonus.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'BATCH_ID', field: 'BATCH_ID', visible: false }


        , { name: 'COMBO_BONUS_ID', field: 'COMBO_BONUS_ID', visible: false }
        , { name: 'MST_ID', field: 'MST_ID', visible: false }
        , { name: 'SKU_ID', field: 'SKU_ID', visible: false }


        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: false, width: '15%'
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: false, width: '15%'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: false, width: '12%'
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: false, width: '12%'
        }
        , {
            name: 'BONUS_QTY', field: 'BONUS_QTY', displayName: 'Batch Qty', enableFiltering: false, width: '12%'
        }
        , {

            name: 'BONUS_AMOUNT', field: 'BONUS_AMOUNT', displayName: 'Bonus Amt', enableFiltering: false, width: '15%'
        }
        , {

            name: 'NET_PRODUCT_AMOUNT', field: 'NET_PRODUCT_AMOUNT', displayName: 'Net Product Amt', enableFiltering: false, width: '15%'
        }



    ];
    $scope.gridOptionsListIssue = (gridregistrationservice.GridRegistration("Sales Issue Info"));
    $scope.gridOptionsListIssue.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsListIssue.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'DTL_ID', field: 'DTL_ID', visible: false }

        , { name: 'INVOICE_UNIT_ID', field: 'INVOICE_UNIT_ID', visible: false }
        , { name: 'ISSUE_ID', field: 'ISSUE_ID', visible: false }
        , { name: 'SKU_ID', field: 'SKU_ID', visible: false }


        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: false, width: '15%'
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: false, width: '25%'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: false, width: '15%'
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: false, width: '15%'
        }
        , {
            name: 'ISSUE_QTY', field: 'ISSUE_QTY', displayName: 'Issue Qty', enableFiltering: false, width: '12%'
        }
        , {
            name: 'ISSUE_AMOUNT', field: 'ISSUE_AMOUNT', displayName: 'Issue Amt', enableFiltering: false, width: '12%'
        }




    ];
    $scope.gridOptionsListComboGift = (gridregistrationservice.GridRegistration("Sales Combo Gift Info"));
    $scope.gridOptionsListComboGift.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }

    $scope.gridOptionsListComboGift.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'COMBO_GIFT_ID', field: 'COMBO_GIFT_ID', visible: false }

        , { name: 'INVOICE_UNIT_ID', field: 'INVOICE_UNIT_ID', visible: false }
        , { name: 'MST_ID', field: 'MST_ID', visible: false }
        , { name: 'GIFT_ITEM_ID', field: 'GIFT_ITEM_ID', visible: false }

        , {
            name: 'GIFT_ITEM_NAME', field: 'GIFT_ITEM_NAME', displayName: 'Gift Name', enableFiltering: false, width: '25%'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: false, width: '15%'
        }
        , {
            name: 'GIFT_QTY', field: 'GIFT_QTY', displayName: 'Gift Qty', enableFiltering: false, width: '15%'
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: false, width: '15%'
        }
        , {

            name: 'GIFT_AMOUNT', field: 'GIFT_AMOUNT', displayName: 'Gift Amt', enableFiltering: false, width: '20%'
        }




    ];
    $scope.gridOptionsListComboBonus = (gridregistrationservice.GridRegistration("Sales Combo Bonus Info"));
    $scope.gridOptionsListComboBonus.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }

    $scope.gridOptionsListComboBonus.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'COMBO_BONUS_ID', field: 'COMBO_BONUS_ID', visible: false }

        , { name: 'INVOICE_UNIT_ID', field: 'INVOICE_UNIT_ID', visible: false }
        , { name: 'MST_ID', field: 'MST_ID', visible: false }
        , { name: 'SKU_ID', field: 'SKU_ID', visible: false }


        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: false, width: '15%'
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: false, width: '25%'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: false, width: '15%'
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: false, width: '15%'
        }
        , {
            name: 'BONUS_QTY', field: 'BONUS_QTY', displayName: 'Bonus Qty', enableFiltering: false, width: '15%'
        }
        , {
            name: 'BONUS_AMOUNT', field: 'BONUS_AMOUNT', displayName: 'Bonus Amt', enableFiltering: false, width: '15%'
        }



    ];
    $scope.gridOptionsListGift = (gridregistrationservice.GridRegistration("Sales Gift Info"));
    $scope.gridOptionsListGift.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsListGift.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'DTL_ID', field: 'DTL_ID', visible: false }

        , { name: 'INVOICE_UNIT_ID', field: 'INVOICE_UNIT_ID', visible: false }
        , { name: 'GIFT_ID', field: 'GIFT_ID', visible: false }
        , { name: 'GIFT_ITEM_ID', field: 'GIFT_ITEM_ID', visible: false }


        , {
            name: 'GIFT_ITEM_NAME', field: 'GIFT_ITEM_NAME', displayName: 'Gift Item Name', enableFiltering: false, width: '15%'
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: false, width: '15%'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: false, width: '12%'
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: false, width: '12%'
        }
        , {
            name: 'GIFT_QTY', field: 'GIFT_QTY', displayName: 'Gift Qty', enableFiltering: false, width: '12%'
        }
        , {
            name: 'GIFT_AMOUNT', field: 'GIFT_AMOUNT', displayName: 'Gift Amt', enableFiltering: false, width: '12%'
        }



    ];
    $scope.DetailData = function (entity) {

        
        window.location = "/SalesAndDistribution/SalesInvoice/InvoiceDetails?q=" + entity.MST_ID_ENCRYPTED;

    }
    $scope.AutoCompleteDataLoadForInvoice = function (value) {
        
        if (value.length >= 1) {
            

            return InsertOrEditServices.LoadSearchableInvoice(value).then(function (data) {
                
                return data.data;
            }, function (error) {
                alert(error);
                

                
            });
        }
    }
    $scope.InvoicesLoad = function () {
        $scope.showLoader = true;

        InsertOrEditServices.InvoicesLoad($scope.model).then(function (data) {

            $scope.invoices = data.data;
         
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }

    $scope.typeaheadSelectedInvoice = function (entity, selectedItem) {
        $scope.model.MST_ID = selectedItem.MST_ID;
        $scope.model.MST_ID_ENCRYPTED = selectedItem.MST_ID_ENCRYPTED;

        $scope.model.INVOICE_NO = selectedItem.INVOICE_NO;

    };

    $scope.OnInvoiceSelect = function () {
       var data =   $scope.invoices.filter(x => x.INVOICE_NO == $scope.model.INVOICE_NO);
        $scope.model.MST_ID = data[0].MST_ID;
        $scope.model.MST_ID_ENCRYPTED = data[0].MST_ID_ENCRYPTED;

        $scope.model.INVOICE_NO = data[0].INVOICE_NO;

    };

    $scope.ReturnTypes = function () {
        var Full = {
            RETURN_TYPE: 'Full'
        }
        var Partial = {
            RETURN_TYPE: 'Partial'
        }
        $scope.Return_Types.push(Full);
        $scope.Return_Types.push(Partial);

    }
    $scope.ReturnTypes();

    $scope.GetInvoiceDetails = function (mst_id) {
        
        $scope.showLoader = true;
        

        InsertOrEditServices.LoadInvoiceDetailsData(mst_id).then(function (data) {
            
            if ($scope.model.RETURN_TYPE == 'Partial') {
                $scope.modelPartial.INVOICE_NO = data.data[0][0].INVOICE_NO;
                $scope.modelPartial.INVOICE_DATE = data.data[0][0].INVOICE_DATE;
                $scope.modelPartial.ORDER_NO = data.data[0][0].ORDER_NO;
                $scope.modelPartial.ORDER_DATE = data.data[0][0].ORDER_DATE;
                $scope.modelPartial.DELIVERY_DATE = data.data[0][0].DELIVERY_DATE;
                $scope.modelPartial.COMPANY_ID = data.data[0][0].COMPANY_ID;
                $scope.modelPartial.RETURN_UNIT_ID = data.data[0][0].INVOICE_UNIT_ID;
                $scope.modelPartial.CUSTOMER_ID = data.data[0][0].CUSTOMER_ID;
                $scope.modelPartial.CUSTOMER_NAME = data.data[0][0].CUSTOMER_NAME;
                $scope.modelPartial.CUSTOMER_CODE = data.data[0][0].CUSTOMER_CODE;
                $scope.modelPartial.CUSTOMER_NAME_CODE = data.data[0][0].CUSTOMER_NAME + " | Code: " + data.data[0][0].CUSTOMER_CODE;

                $scope.modelPartial.INVOICE_TYPE_NAME = data.data[0][0].INVOICE_TYPE_NAME;
                $scope.modelPartial.TP_AMOUNT = data.data[0][0].TP_AMOUNT;
                $scope.modelPartial.VAT_AMOUNT = data.data[0][0].VAT_AMOUNT;
                $scope.modelPartial.TOTAL_AMOUNT = data.data[0][0].TOTAL_AMOUNT;
                $scope.modelPartial.CUSTOMER_DISC_AMOUNT = data.data[0][0].CUSTOMER_DISC_AMOUNT;
                $scope.modelPartial.CUSTOMER_ADD1_DISC_AMOUNT = data.data[0][0].CUSTOMER_ADD1_DISC_AMOUNT;
                $scope.modelPartial.CUSTOMER_ADD2_DISC_AMOUNT = data.data[0][0].CUSTOMER_ADD2_DISC_AMOUNT;
                $scope.modelPartial.CUSTOMER_PRODUCT_DISC_AMOUNT = data.data[0][0].CUSTOMER_PRODUCT_DISC_AMOUNT;
                $scope.modelPartial.BONUS_PRICE_DISC_AMOUNT = data.data[0][0].BONUS_PRICE_DISC_AMOUNT;
                $scope.modelPartial.PROD_BONUS_PRICE_DISC_AMOUNT = data.data[0][0].PROD_BONUS_PRICE_DISC_AMOUNT;
                $scope.modelPartial.LOADING_CHARGE_AMOUNT = data.data[0][0].LOADING_CHARGE_AMOUNT;
                $scope.modelPartial.INVOICE_ADJUSTMENT_AMOUNT = data.data[0][0].INVOICE_ADJUSTMENT_AMOUNT;
                $scope.modelPartial.INVOICE_DISCOUNT_AMOUNT = data.data[0][0].INVOICE_DISCOUNT_AMOUNT;
                $scope.modelPartial.NET_INVOICE_AMOUNT = data.data[0][0].NET_INVOICE_AMOUNT;
                $scope.modelPartial.TDS_AMOUNT = data.data[0][0].TDS_AMOUNT;

                $scope.modelPartial.CUSTOMER_ADD_DISC_AMOUNT = $scope.modelPartial.CUSTOMER_ADD1_DISC_AMOUNT + $scope.modelPartial.CUSTOMER_ADD2_DISC_AMOUNT
                $scope.modelPartial.BONUS_DISC_AMOUNT = $scope.modelPartial.BONUS_PRICE_DISC_AMOUNT + $scope.modelPartial.PROD_BONUS_PRICE_DISC_AMOUNT



                $scope.gridOptionsListDtlsPartial.data = data.data[1];
                for (var i = 0; i < $scope.gridOptionsListDtlsPartial.data.length; i++) {
                    $scope.gridOptionsListDtlsPartial.data[i].RETURN_QTY_MAX = $scope.gridOptionsListDtlsPartial.data[i].INVOICE_QTY;
                }

                $scope.gridOptionsListComboGiftPartial= data.data[2];
                $scope.gridOptionsListComboBonusPartial= data.data[3];
                $scope.gridOptionsListGiftPartial= data.data[4];

                $scope.gridOptionsListBonusPartial= data.data[5];
                $scope.gridOptionsListIssuePartial = data.data[6];
                $('#myModal1').modal('toggle');

            } else {
                $scope.model.INVOICE_NO = data.data[0][0].INVOICE_NO;
                $scope.model.INVOICE_DATE = data.data[0][0].INVOICE_DATE;
                $scope.model.ORDER_NO = data.data[0][0].ORDER_NO;
                $scope.model.ORDER_DATE = data.data[0][0].ORDER_DATE;
                $scope.model.DELIVERY_DATE = data.data[0][0].DELIVERY_DATE;
                $scope.model.COMPANY_ID = data.data[0][0].COMPANY_ID;
                $scope.model.RETURN_UNIT_ID = data.data[0][0].INVOICE_UNIT_ID;
                $scope.model.CUSTOMER_ID = data.data[0][0].CUSTOMER_ID;
                $scope.model.CUSTOMER_NAME = data.data[0][0].CUSTOMER_NAME;
                $scope.model.CUSTOMER_CODE = data.data[0][0].CUSTOMER_CODE;
                $scope.model.CUSTOMER_NAME_CODE = data.data[0][0].CUSTOMER_NAME + " | Code: " + data.data[0][0].CUSTOMER_CODE;

                $scope.model.INVOICE_TYPE_NAME = data.data[0][0].INVOICE_TYPE_NAME;
                $scope.model.TP_AMOUNT = data.data[0][0].TP_AMOUNT;
                $scope.model.VAT_AMOUNT = data.data[0][0].VAT_AMOUNT;
                $scope.model.TOTAL_AMOUNT = data.data[0][0].TOTAL_AMOUNT;
                $scope.model.CUSTOMER_DISC_AMOUNT = data.data[0][0].CUSTOMER_DISC_AMOUNT;
                $scope.model.CUSTOMER_ADD1_DISC_AMOUNT = data.data[0][0].CUSTOMER_ADD1_DISC_AMOUNT;
                $scope.model.CUSTOMER_ADD2_DISC_AMOUNT = data.data[0][0].CUSTOMER_ADD2_DISC_AMOUNT;
                $scope.model.CUSTOMER_PRODUCT_DISC_AMOUNT = data.data[0][0].CUSTOMER_PRODUCT_DISC_AMOUNT;
                $scope.model.BONUS_PRICE_DISC_AMOUNT = data.data[0][0].BONUS_PRICE_DISC_AMOUNT;
                $scope.model.PROD_BONUS_PRICE_DISC_AMOUNT = data.data[0][0].PROD_BONUS_PRICE_DISC_AMOUNT;
                $scope.model.LOADING_CHARGE_AMOUNT = data.data[0][0].LOADING_CHARGE_AMOUNT;
                $scope.model.INVOICE_ADJUSTMENT_AMOUNT = data.data[0][0].INVOICE_ADJUSTMENT_AMOUNT;
                $scope.model.INVOICE_DISCOUNT_AMOUNT = data.data[0][0].INVOICE_DISCOUNT_AMOUNT;
                $scope.model.NET_INVOICE_AMOUNT = data.data[0][0].NET_INVOICE_AMOUNT;
                $scope.model.TDS_AMOUNT = data.data[0][0].TDS_AMOUNT;

                $scope.model.CUSTOMER_ADD_DISC_AMOUNT = $scope.model.CUSTOMER_ADD1_DISC_AMOUNT + $scope.model.CUSTOMER_ADD2_DISC_AMOUNT
                $scope.model.BONUS_DISC_AMOUNT = $scope.model.BONUS_PRICE_DISC_AMOUNT + $scope.model.PROD_BONUS_PRICE_DISC_AMOUNT



                $scope.gridOptionsListDtls.data = data.data[1];

                $scope.gridOptionsListComboGift.data = data.data[2];
                $scope.gridOptionsListComboBonus.data = data.data[3];
                $scope.gridOptionsListGift.data = data.data[4];

                $scope.gridOptionsListBonus.data = data.data[5];
                $scope.gridOptionsListIssue.data = data.data[6];
            }
            

            $scope.showLoader = false;
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    $scope.ProcessPartialReturnProduct = function () {
        $scope.showLoader = true;
        $scope.msg = "";
        $scope.Status = "1";
        $scope.model.INVOICE_NO = $scope.modelPartial.INVOICE_NO;
        $scope.gridOptionsListDtls.data = [];
        $scope.Return_partial = [];
        for (var i = 0; i < $scope.gridOptionsListDtlsPartial.data.length; i++) {
            

            if ($scope.gridOptionsListDtlsPartial.data[i].RETURN_CHECK == true) {
                
                $scope.showLoader = true;
                var full = {
                    SKU_ID: $scope.gridOptionsListDtlsPartial.data[i].SKU_ID,
                    SKU_CODE: $scope.gridOptionsListDtlsPartial.data[i].SKU_CODE,
                    RETURN_QTY: $scope.gridOptionsListDtlsPartial.data[i].INVOICE_QTY,
                    INVOICE_NO: $scope.model.INVOICE_NO
                }
                $scope.Return_partial.push(full);
               
            }
        }
        

        //InsertOrEditServices.ProcessPartialReturn($scope.Return_partial).then(function (data) {
        //    
           
        //    for (var m = 0; m < $scope.Return_partial.length; m++) {
        //        var indexxx = $scope.gridOptionsListDtlsPartial.data.findIndex(x => x.SKU_ID == $scope.Return_partial[m].SKU_ID);
        //        if (indexxx != -1) {
        //            $scope.gridOptionsListDtls.data.push($scope.gridOptionsListDtlsPartial.data[indexxx]);
        //            $scope.msg = $scope.msg == "" ? $scope.Return_partial[m].SKU_CODE + " is sucessfully processed! " : $scope.msg + " || " + $scope.model.SKU_CODE + " is sucessfully processed! "
        //        }
        //    }
        //    notificationservice.Notification($scope.Status, 1, $scope.msg);

        //    $scope.showLoader = false;


        //}, function (error) {

        //    $scope.Status = "2"
        //    $scope.msg = error;
        //});
        for (var m = 0; m < $scope.Return_partial.length; m++) {
                var indexxx = $scope.gridOptionsListDtlsPartial.data.findIndex(x => x.SKU_ID == $scope.Return_partial[m].SKU_ID);
                if (indexxx != -1) {
                    $scope.gridOptionsListDtls.data.push($scope.gridOptionsListDtlsPartial.data[indexxx]);
                    $scope.msg = $scope.msg == "" ? $scope.Return_partial[m].SKU_CODE + " is sucessfully processed! " : $scope.msg + " || " + $scope.model.SKU_CODE + " is sucessfully processed! "
                }
        }
        if ($scope.Status == "1") {

            $scope.model.INVOICE_NO = $scope.modelPartial.INVOICE_NO;
            $scope.model.INVOICE_DATE = $scope.modelPartial.INVOICE_DATE;
            $scope.model.ORDER_NO = $scope.modelPartial.ORDER_NO;
            $scope.model.ORDER_DATE = $scope.modelPartial.ORDER_DATE;
            $scope.model.DELIVERY_DATE = $scope.modelPartial.DELIVERY_DATE;
            $scope.model.COMPANY_ID = $scope.modelPartial.COMPANY_ID;
            $scope.model.RETURN_UNIT_ID = $scope.modelPartial.INVOICE_UNIT_ID;
            $scope.model.CUSTOMER_ID = $scope.modelPartial.CUSTOMER_ID;
            $scope.model.CUSTOMER_NAME = $scope.modelPartial.CUSTOMER_NAME;
            $scope.model.CUSTOMER_CODE = $scope.modelPartial.CUSTOMER_CODE;
            $scope.model.CUSTOMER_NAME_CODE = $scope.modelPartial.CUSTOMER_NAME + " | Code: " + $scope.modelPartial.CUSTOMER_CODE;

            $scope.model.INVOICE_TYPE_NAME = $scope.modelPartial.INVOICE_TYPE_NAME;



        }

            $scope.showLoader = false;
        

       
    }


    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        InsertOrEditServices.GetCompany().then(function (data) {
            
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
    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;

        InsertOrEditServices.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.SendNotification = function (msg) {
        $scope.showLoader = true;

        LiveNotificationServices.Notification_Permitted_Users(2, $scope.model.COMPANY_ID, $scope.model.UNIT_ID).then(function (data) {
            
            $scope.showLoader = false;

            $scope.Users = data.data;
            $scope.Permitted_Users = [];
            if ($scope.Users != undefined && $scope.Users != null) {
                for (var i = 0; i < $scope.Users.length; i++) {
                    $scope.Permitted_Users.push(JSON.stringify(parseInt($scope.Users[i].USER_ID)));
                }
                connection.invoke("SendMessage", $scope.Permitted_Users, ":" + msg).catch(function (err) {
                    return console.error(err.toString());
                });
            }
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });


        //connection.invoke("SendMessage", $scope.Permitted_Users, "Hey New Order Has been Created and Ready For Invoice!!!!").catch(function (err) {
        //       return console.error(err.toString());
        //   });
    }

    $scope.SaelsReturn = function () {
        
        $scope.showLoader = true;
        if ($scope.model.RETURN_TYPE == 'Full') {
            $scope.return_data = [];
            $scope.return_data.INVOICE_NO = $scope.model.INVOICE_NO;
            $scope.return_data.INVOICE_DATE = $scope.model.INVOICE_DATE;
            $scope.return_data.COMPANY_ID = $scope.model.COMPANY_ID;
            $scope.return_data.RETURN_UNIT_ID = $scope.model.RETURN_UNIT_ID;
            $scope.return_data.CUSTOMER_ID = $scope.model.CUSTOMER_ID;
            $scope.return_data.CUSTOMER_CODE = $scope.model.CUSTOMER_CODE;

            InsertOrEditServices.FullReturn($scope.return_data).then(function (data) {
                notificationservice.Notification(data.data, 1, 'Invoice ' + $scope.model.INVOICE_NO + ' is Returned Successfully !!');
                $scope.SendNotification('Invoice ' + $scope.model.INVOICE_NO + ' is Returned Successfully !!');

                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                
                $scope.showLoader = false;

            });
        } else {
            
            //$scope.return_data = [];
            //$scope.return_data = $scope.Return_partial;
            $scope.Process_data = [];
            $scope.Process_data = $scope.Return_partial;

            

            InsertOrEditServices.SalesReturnPartial($scope.Process_data).then(function (data) {
                if (data.data.Status == '1') {
                    notificationservice.Notification(1, 1, 'Invoice ' + $scope.model.INVOICE_NO + ' is Returned Successfully (Partialy) !!');
                    $scope.SendNotification('Invoice ' + $scope.model.INVOICE_NO + ' is Returned Partially !!');
                } else {
                    notificationservice.Notification(data.data.Status, 1, 'Invoice ' + $scope.model.INVOICE_NO + ' is Returned Successfully (Partialy) !!');

                }
               

                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                
                $scope.showLoader = false;

            });
        }
        
    }
    

    $scope.ClearForm = function () {
        window.location = "/SalesAndDistribution/SalesInvoice/List";
    }



    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'SalesReturn',
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

    $scope.GetPermissionData();


}]);

