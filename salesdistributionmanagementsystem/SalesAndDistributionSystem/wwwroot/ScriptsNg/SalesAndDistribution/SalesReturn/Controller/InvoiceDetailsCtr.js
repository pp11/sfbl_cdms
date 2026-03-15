ngApp.controller('ngGridCtrl', ['$scope', 'InsertOrEditServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InsertOrEditServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {}
    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];

    $scope.gridOptionsListDtls = (gridregistrationservice.GridRegistration("Sales Return Info"));
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
            name: 'INVOICE_QTY', field: 'INVOICE_QTY', displayName: 'Invoice Qty', enableFiltering: false, width: '12%'
        }
        , {
            name: 'RETURN_QTY', field: 'RETURN_QTY', displayName: 'Return Qty', enableFiltering: false, width: '12%'
        }
        , {
            name: 'TP_AMOUNT', field: 'TP_AMOUNT', displayName: 'TP Amt', enableFiltering: false, width: '12%'
        }
        , {

            name: 'TOTAL_AMOUNT', field: 'TOTAL_AMOUNT', displayName: 'Total Amt', enableFiltering: false, width: '15%'
        }
        , {

            name: 'RETURN_ADJUSTMENT_AMOUNT', field: 'RETURN_ADJUSTMENT_AMOUNT', displayName: 'Return Adjustment Amt', enableFiltering: false, width: '15%'
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

        , { name: 'RETURN_UNIT_ID', field: 'RETURN_UNIT_ID', visible: false }
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
            name: 'RETURN_QTY', field: 'RETURN_QTY', displayName: 'Return Qty', enableFiltering: false, width: '12%'
        }
        , {
            name: 'RETURN_AMOUNT', field: 'RETURN_AMOUNT', displayName: 'Return Amt', enableFiltering: false, width: '12%'
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

        , { name: 'RETURN_UNIT_ID', field: 'RETURN_UNIT_ID', visible: false }
        , { name: 'MST_ID', field: 'MST_ID', visible: false }
        , { name: 'GIFT_ITEM_ID', field: 'GIFT_ITEM_ID', visible: false }
        , {
            name: 'GIFT_ITEM_NAME', field: 'GIFT_ITEM_NAME', displayName: 'Gift Name', enableFiltering: false, width: '25%'
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: false, width: '15%'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: false, width: '15%'
        }
        , {
            name: 'GIFT_RETURN_QTY', field: 'GIFT_RETURN_QTY', displayName: 'Gift Return Qty', enableFiltering: false, width: '15%'
        }
     
        , {

            name: 'GIFT_RETURN_AMOUNT', field: 'GIFT_RETURN_AMOUNT', displayName: 'Gift Return Amt', enableFiltering: false, width: '20%'
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

        , { name: 'RETURN_UNIT_ID', field: 'RETURN_UNIT_ID', visible: false }
        , { name: 'MST_ID', field: 'MST_ID', visible: false }
        , { name: 'SKU_ID', field: 'SKU_ID', visible: false }


        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: false, width: '15%'
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: false, width: '25%'
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: false, width: '15%'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: false, width: '15%'
        }
        
        , {
            name: 'RETURN_BONUS_QTY', field: 'RETURN_BONUS_QTY', displayName: 'Bonus Return Qty', enableFiltering: false, width: '15%'
        }
        , {
            name: 'RETURN_BONUS_AMOUNT', field: 'RETURN_BONUS_AMOUNT', displayName: 'Bonus Return Amt', enableFiltering: false, width: '15%'
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

        , { name: 'RETURN_UNIT_ID', field: 'RETURN_UNIT_ID', visible: false }
        , { name: 'GIFT_ID', field: 'GIFT_ID', visible: false }
        , { name: 'GIFT_ITEM_ID', field: 'GIFT_ITEM_ID', visible: false }
        , {
            name: 'GIFT_ITEM_NAME', field: 'GIFT_ITEM_NAME', displayName: 'Gift Item Name', enableFiltering: false, width: '15%'
        }
       
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: false, width: '12%'
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: false, width: '12%'
        }
        , {
            name: 'GIFT_RETURN_QTY', field: 'GIFT_RETURN_QTY', displayName: 'Gift Return Qty', enableFiltering: false, width: '12%'
        }
        , {
            name: 'GIFT_RETURN_AMOUNT', field: 'GIFT_RETURN_AMOUNT', displayName: 'Gift Return Amt', enableFiltering: false, width: '12%'
        }



    ];
    $scope.DetailData = function (entity) {

        
        window.location = "/SalesAndDistribution/SalesInvoice/InvoiceDetails?q=" + entity.MST_ID_ENCRYPTED;

    }

    $scope.GetReturnDetails = function (mst_id) {
        
        $scope.showLoader = true;

        InsertOrEditServices.LoadReturnDetailsData(mst_id).then(function (data) {
            
            $scope.model = data.data[0][0];
            $scope.GetPermissionData();

           
            $scope.model.CUSTOMER_NAME_CODE = $scope.model.CUSTOMER_NAME + " | Code: " + $scope.model.CUSTOMER_CODE;
            $scope.model.CUSTOMER_ADD_DISC_AMOUNT = $scope.model.CUSTOMER_ADD1_DISC_AMOUNT + $scope.model.CUSTOMER_ADD2_DISC_AMOUNT
            $scope.model.BONUS_DISC_AMOUNT = $scope.model.BONUS_PRICE_DISC_AMOUNT + $scope.model.PROD_BONUS_PRICE_DISC_AMOUNT


            $scope.gridOptionsListDtls.data = data.data[1];

            $scope.gridOptionsListComboGift.data = data.data[2];
            $scope.gridOptionsListComboBonus.data = data.data[3];
            $scope.gridOptionsListGift.data = data.data[4];

            $scope.gridOptionsListBonus.data = data.data[5];
            $scope.gridOptionsListIssue.data = data.data[6];

            $scope.showLoader = false;
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
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


    $scope.ClearForm = function () {
        window.location = "/SalesAndDistribution/SalesInvoice/List";
    }



    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'SalesReturn',
            Action_Name: 'SalesReturnDetails'
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



}]);

