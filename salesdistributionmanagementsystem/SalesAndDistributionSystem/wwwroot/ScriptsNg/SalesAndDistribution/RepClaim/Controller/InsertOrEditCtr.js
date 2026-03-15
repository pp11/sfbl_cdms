ngApp.controller('ngGridCtrl', ['$scope', 'InsertOrEditServices', 'RegionInfoServices', 'AreaInfoServices', 'AdjustmentServices', 'LiveNotificationServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InsertOrEditServices, RegionInfoServices, AreaInfoServices, AdjustmentServices, LiveNotificationServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {
        COMPANY_ID: 0, UNIT_ID: 0, CUSTOMER_ID: 0, CUSTOMER_CODE: '', MARKET_ID: 0, TERRITORY_ID: 0, AREA_ID: 0, REGION_ID: 0, DIVISION_ID: 0, PAYMENT_MODE: '', REPLACE_CLAIM_NO: '', BONUS_PROCESS_NO: '', BONUS_CLAIM_NO: '', INVOICE_STATUS: '', ORDER_AMOUNT: 0, ORDER_UNIT_ID: 0, INVOICE_UNIT_ID: 0, REMARKS: '', SPA_TOTAL_AMOUNT: 0, SPA_COMMISSION_PCT: 0, SPA_COMMISSION_AMOUNT: 0, SPA_NET_AMOUNT: 0, REMARKS: ''
      }
    var connection = new signalR.HubConnectionBuilder().withUrl("/notificationhub").build();
    connection.start();
    //connection.on("ReceiveNotificationHandler", function (message) {
    //    
    //    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");

    //    alert(msg);
    //});
    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];
    $scope.Customers = [];
    $scope.InvoiceTypes = [];
    $scope.Units = [];
    $scope.Products = [];
    $scope.PaymentMode = [];
    $scope.Users = [];
    $scope.model.ORDER_TYPE = 'I0001';
    $scope.model.ORDER_STATUS = 'Active';
    $scope.model.PAYMENT_MODE = 'Cash';


    
    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 1, COMPANY_ID: 0, UNIT_ID: 0, ORDER_DTL_ID: 0, ORDER_MST_ID: 0, SKU_ID: 0, SKU_CODE: '', ORDER_QTY: 0, REVISED_ORDER_QTY: 0, UNIT_TP: 0, ORDER_AMOUNT: 0, STATUS: '', COMPANY_ID: 0, UNIT_ID: 0, REMARKS: '', SPA_UNIT_TP: 0, SPA_AMOUNT: 0, SPA_REQ_TIME_STOCK: 0, SPA_DISCOUNT_TYPE: 0, SPA_DISCOUNT_VAL_PCT: 0, SPA_CUST_COM: 0, SPA_DISCOUNT_AMOUNT: 0, SPA_TOTAL_AMOUNT: 0
        }
    }

    $scope.gridOptions = (gridregistrationservice.GridRegistration("Region Area Relation"));
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptions.data = [];
    //Grid Register
    $scope.gridOptions = {
        data: [$scope.GridDefalutData()]
    }

    //Generate New Row No
    $scope.rowNumberGenerate = function () {
        
        for (let i = 0; i < $scope.gridOptions.data.length; i++) {
            
            $scope.gridOptions.data[i].ROW_NO = i;
           
           
        }

    }


    $scope.OnCustomerSelection = () => {
        
        if ($scope.Customers.length > 0) {
            var index = $scope.Customers.findIndex(x => x.CUSTOMER_ID == $scope.model.CUSTOMER_ID);
            if (index != -1) {
                $scope.model.CUSTOMER_ADDRESS = $scope.Customers[index].CUSTOMER_ADDRESS;
                $scope.model.MARKET_ID = $scope.Customers[index].MARKET_ID;
                $scope.model.TERRITORY_ID = $scope.Customers[index].TERRITORY_ID;
                $scope.model.AREA_ID = $scope.Customers[index].AREA_ID;
                $scope.model.REGION_ID = $scope.Customers[index].REGION_ID;
                $scope.model.DIVISION_ID = $scope.Customers[index].DIVISION_ID;
                $scope.model.MARKET_NAME = $scope.Customers[index].MARKET_NAME;
                $scope.Get_Customer_Balance();
            }
        }
        else {
            $scope.showLoader = true;

            InsertOrEditServices.LoadCustomerData($scope.model.COMPANY_ID).then(function (data) {
                
                $scope.Customers_data = data.data;
                var _Customers = {
                    CUSTOMER_ID: "0",
                    CUSTOMER_NAME: "None",
                    CUSTOMER_CODE: "None",
                    CUSTOMER_ADDRESS: "None",

                }
                $scope.Customers.push(_Customers);
                for (var i in $scope.Customers_data) {
                    $scope.Customers.push($scope.Customers_data[i]);
                }
                var index = $scope.Customers.findIndex(x => x.CUSTOMER_ID == $scope.model.CUSTOMER_ID);
                if (index != -1) {
                    $scope.model.CUSTOMER_ADDRESS = $scope.Customers[index].CUSTOMER_ADDRESS;
                    $scope.model.MARKET_ID = $scope.Customers[index].MARKET_ID;
                    $scope.model.TERRITORY_ID = $scope.Customers[index].TERRITORY_ID;
                    $scope.model.AREA_ID = $scope.Customers[index].AREA_ID;
                    $scope.model.REGION_ID = $scope.Customers[index].REGION_ID;
                    $scope.model.DIVISION_ID = $scope.Customers[index].DIVISION_ID;
                    $scope.model.MARKET_NAME = $scope.Customers[index].MARKET_NAME;

                }

                $scope.showLoader = false;
            }, function (error) {
                
                $scope.showLoader = false;

            });
        }
        
    }


    



    $scope.addNewRow = () => {
        
        var result = "true"

        if ($scope.gridOptions.data.length > 0 && $scope.gridOptions.data[0].SKU_CODE != null && $scope.gridOptions.data[0].SKU_CODE != '' && $scope.gridOptions.data[0].SKU_CODE != 'undefined') {
            for (var i = 0; i < $scope.gridOptions.data.length; i++) {
                if (i > 0) {
                    if ($scope.gridOptions.data[i].SKU_ID == $scope.gridOptions.data[0].SKU_ID) {
                        result = "false";
                    }
                }
            }
            if (result == "true") {
                
                if ($scope.gridOptions.data[0].ORDER_QTY != '') {
                    $scope.gridOptions.data[0].ORDER_AMOUNT = parseFloat($scope.gridOptions.data[0].UNIT_TP) * parseFloat($scope.gridOptions.data[0].ORDER_QTY);
                    $scope.model.ORDER_AMOUNT = 0;
                    for (var i = 0; i < $scope.gridOptions.data.length; i++) {
                        $scope.model.ORDER_AMOUNT = parseFloat($scope.model.ORDER_AMOUNT) + parseFloat($scope.gridOptions.data[i].ORDER_AMOUNT);
                    }

                }
                var newRow = {
                    ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID,
                    ORDER_DTL_ID: $scope.gridOptions.data[0].ORDER_DTL_ID,
                    ORDER_MST_ID: $scope.gridOptions.data[0].ORDER_MST_ID,
                    ORDER_AMOUNT: $scope.gridOptions.data[0].ORDER_AMOUNT,
                    ORDER_QTY: $scope.gridOptions.data[0].ORDER_QTY,
                    REMARKS: $scope.gridOptions.data[0].REMARKS,
                    REVISED_ORDER_QTY: $scope.gridOptions.data[0].REVISED_ORDER_QTY,
                    SKU_CODE: $scope.gridOptions.data[0].SKU_CODE,
                    SKU_ID: JSON.stringify($scope.gridOptions.data[0].SKU_ID),
                    UNIT_TP: $scope.gridOptions.data[0].UNIT_TP,
                    UNIT_ID: $scope.gridOptions.data[0].UNIT_ID,
                    STATUS: $scope.gridOptions.data[0].STATUS,
                    SPA_UNIT_TP: $scope.gridOptions.data[0].SPA_UNIT_TP,
                    SPA_TOTAL_AMOUNT: $scope.gridOptions.data[0].SPA_TOTAL_AMOUNT,
                    SPA_REQ_TIME_STOCK: $scope.gridOptions.data[0].SPA_REQ_TIME_STOCK,
                    SPA_DISCOUNT_VAL_PCT: $scope.gridOptions.data[0].SPA_DISCOUNT_VAL_PCT,
                    SPA_DISCOUNT_TYPE: $scope.gridOptions.data[0].SPA_DISCOUNT_TYPE,
                    SPA_DISCOUNT_AMOUNT: $scope.gridOptions.data[0].SPA_DISCOUNT_AMOUNT,
                    SPA_CUST_COM: $scope.gridOptions.data[0].SPA_CUST_COM,
                    SPA_AMOUNT: $scope.gridOptions.data[0].SPA_AMOUNT


                }

                $scope.gridOptions.data.push(newRow);

                //for (var i = 0; i < $scope.gridOptions.data.length; i++) {
                //    
                //    $scope.gridOptions.data[i].SKU_ID = JSON.stringify($scope.gridOptions.data[i].SKU_ID);
                //}
                $scope.gridOptions.data[0] = $scope.GridDefalutData();
                $interval(function () {
                    $scope.LoadSKU_ID();
                }, 800, 2);
            } else {
                notificationservice.Notification("This Product is already added!!", "", 'Can not Select Twice!!');

            }
        } else {
            notificationservice.Notification("Please Select Valid Product First", "", 'No Product Selected!!');

        }
        $scope.rowNumberGenerate();
    };
    $scope.CustomerLoad = function () {

        $scope.showLoader = true;

        InsertOrEditServices.LoadCustomerData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.Customers_data = data.data;
            var _Customers = {
                CUSTOMER_ID: "0",
                CUSTOMER_NAME: "None",
                CUSTOMER_CODE: "None",
                CUSTOMER_ADDRESS: "None",

            }
            $scope.Customers.push(_Customers);
            for (var i in $scope.Customers_data) {
                $scope.Customers.push($scope.Customers_data[i]);
            }


            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }


    $scope.InvoiceTypeLoad = function () {

        $scope.showLoader = true;

        InsertOrEditServices.LoadInvoiceTypes($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.InvoiceTypes = data.data;

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
   

   
    $scope.ProductsLoad = function () {

        $scope.showLoader = true;

        InsertOrEditServices.LoadProductData($scope.model.COMPANY_ID).then(function (data) {
            
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
            
            $scope.showLoader = false;

        });
    }

    $scope.OnProductSelectSelection = (entity) => {
        
        $scope.LoadProductUnitPrice(entity);
        var index = $scope.Products.findIndex(x => x.SKU_ID == entity.SKU_ID);
        if (index != -1) {
            entity.SKU_CODE = $scope.Products[index].SKU_CODE;
            entity.SKU_ID = $scope.Products[index].SKU_ID;
            entity.SPA_AMOUNT = 0;
            entity.SPA_CUST_COM = 0;
            entity.SPA_DISCOUNT_AMOUNT = 0;
            entity.SPA_DISCOUNT_TYPE = '';
            entity.SPA_DISCOUNT_VAL_PCT = 0;
            entity.SPA_REQ_TIME_STOCK = 0;
            entity.SPA_TOTAL_AMOUNT = 0;
            entity.SPA_UNIT_TP = 0;
            entity.ORDER_QTY = 0;
            entity.REVISED_ORDER_QTY = 0;
            entity.ORDER_AMOUNT = 0;

            
        }
    }
    $scope.OnProductQTYChange = (entity) => {

        if (entity.ROW_NO > 0) {
            if (entity.ORDER_QTY != '') {
                entity.ORDER_AMOUNT = parseFloat(entity.UNIT_TP) * parseFloat(entity.ORDER_QTY);
                $scope.model.ORDER_AMOUNT = 0;
                for (var i = 0; i < $scope.gridOptions.data.length; i++) {
                    $scope.model.ORDER_AMOUNT = parseFloat($scope.model.ORDER_AMOUNT) + parseFloat($scope.gridOptions.data[i].ORDER_AMOUNT);
                }

            }
        }
       
    }


    $scope.EditItem = (entity) => {
        
        if ($scope.gridOptions.data.length > 0) {

            var newRow = {
                ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID,
                ORDER_DTL_ID: entity.ORDER_DTL_ID,
                ORDER_MST_ID: entity.ORDER_MST_ID,
                ORDER_AMOUNT: entity.ORDER_AMOUNT,
                ORDER_QTY: entity.ORDER_QTY,
                REMARKS: entity.REMARKS,
                REVISED_ORDER_QTY: entity.REVISED_ORDER_QTY,
                SKU_CODE: entity.SKU_CODE,
                SKU_ID: entity.SKU_ID,
                UNIT_TP: entity.UNIT_TP,
                UNIT_ID: entity.UNIT_ID,
                STATUS: entity.STATUS,
                SPA_UNIT_TP: entity.SPA_UNIT_TP,
                SPA_TOTAL_AMOUNT: entity.SPA_TOTAL_AMOUNT,
                SPA_REQ_TIME_STOCK: entity.SPA_REQ_TIME_STOCK,
                SPA_DISCOUNT_VAL_PCT: entity.SPA_DISCOUNT_VAL_PCT,
                SPA_DISCOUNT_TYPE: entity.SPA_DISCOUNT_TYPE,
                SPA_DISCOUNT_AMOUNT: entity.SPA_DISCOUNT_AMOUNT,
                SPA_CUST_COM: entity.SPA_CUST_COM,
                SPA_AMOUNT: entity.SPA_AMOUNT
            }
            $scope.gridOptions.data[0] = newRow;
         

        } else {
            notificationservice.Notification("Please Select Valid data", "", 'Only Single Row Left!!');

        }
        $scope.rowNumberGenerate();
        $scope.removeItem(entity);
        $interval(function () {
            $scope.LoadSKU_ID();
        }, 800,2);
    };
    // Grid one row remove if this mehtod is call
    $scope.removeItem = function (entity) {
        
        if ($scope.gridOptions.data.length > 1) {
            var index = $scope.gridOptions.data.indexOf(entity);
            if ($scope.gridOptions.data.length > 0) {
                $scope.gridOptions.data.splice(index, 1);
            }
            $scope.model.ORDER_AMOUNT = 0;

            for (var i = 0; i < $scope.gridOptions.data.length; i++) {
                if ($scope.gridOptions.data[i].ROW_NO > 0) {
                    $scope.model.ORDER_AMOUNT = parseFloat($scope.model.ORDER_AMOUNT) + (parseFloat($scope.gridOptions.data[i].UNIT_TP) * parseFloat($scope.gridOptions.data[i].ORDER_QTY));
                }

            }
            $scope.rowNumberGenerate();


        } else {
            notificationservice.Notification("Only Single Row Left!!", "", 'Only Single Row Left!!');
        }
       
        
    }
    $scope.LoadFormData = function (id) {

            $scope.model.ORDER_MST_ID_ENCRYPTED = id;
            $scope.GetEditDataById(id);
 
    }
    $scope.gridOptions.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false,  width: '50'
        }
        , { name: 'ORDER_DTL_ID', field: 'ORDER_DTL_ID', visible: false }
        , { name: 'ORDER_MST_ID', field: 'ORDER_MST_ID', visible: false }
        , { name: 'SKU_NAME', field: 'SKU_NAME', visible: false }
        , { name: 'STATUS', field: 'STATUS', visible: false }
        , { name: 'STSPA_UNIT_TPATUS', field: 'STSPA_UNIT_TPATUS', visible: false }
        , { name: 'SPA_AMOUNT', field: 'SPA_AMOUNT', visible: false }
        , { name: 'SPA_REQ_TIME_STOCK', field: 'SPA_REQ_TIME_STOCK', visible: false }
        , { name: 'SPA_DISCOUNT_TYPE', field: 'SPA_DISCOUNT_TYPE', visible: false }
        , { name: 'SPA_DISCOUNT_VAL_PCT', field: 'SPA_DISCOUNT_VAL_PCT', visible: false }
        , { name: 'SPA_CUST_COM', field: 'SPA_CUST_COM', visible: false }
        , { name: 'SPA_TOTAL_AMOUNT', field: 'SPA_TOTAL_AMOUNT', visible: false }
        , { name: 'SPA_CUST_COM', field: 'SPA_CUST_COM', visible: false }
        , { name: 'SPA_CUST_COM', field: 'SPA_CUST_COM', visible: false }
        , { name: 'SPA_CUST_COM', field: 'SPA_CUST_COM', visible: false }
        , { name: 'SPA_CUST_COM', field: 'SPA_CUST_COM', visible: false }

        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , { name: 'UNIT_ID', field: 'UNIT_ID', visible: false }
       
        , {
            name: 'SKU_ID', field: 'SKU_ID', displayName: 'Product (Name | Code | Packe Size)', enableFiltering: false, width: '48%', cellTemplate:
                '<select class="select2-single sku_Id form-control" id="SKU_ID" ng-disabled="row.entity.ROW_NO !=0 || grid.appScope.model.CUSTOMER_ID <1"' +
                'name="SKU_ID" ng-model="row.entity.SKU_ID" style="width:100%" ng-change="grid.appScope.OnProductSelectSelection(row.entity)" >' +
                '<option ng-repeat="item in grid.appScope.Products" ng-selected="item.SKU_ID == row.entity.SKU_ID" value="{{item.SKU_ID}}">{{ item.SKU_NAME }} | Code: {{ item.SKU_CODE }} | Pack Size: {{ item.PACK_SIZE }} </option>' +
                '</select>'
        }
        , { name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: false, width: '18%', cellTemplate:
            '<input type="SKU_CODE"  ng-model="row.entity.SKU_CODE" disabled="true"  class="pl-sm" />'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: false, width: '10%', cellTemplate:
                '<input type="text"  ng-model="row.entity.UNIT_TP" disabled="true"  class="pl-sm" />'
        }
      
        , {
            name: 'ORDER_QTY', field: 'ORDER_QTY', displayName: 'QTY', enableFiltering: false, width: '10%', cellTemplate:
                '<input type="number" min="0" ng-model="row.entity.ORDER_QTY" ng-disabled="grid.appScope.model.CUSTOMER_ID <1" ng-change="grid.appScope.OnProductQTYChange(row.entity)"  class="pl-sm" />'

        }, {
            name: 'REVISED_ORDER_QTY', field: 'REVISED_ORDER_QTY', displayName: 'Rev. QTY', enableFiltering: false, visible:false, width: '10%', cellTemplate:
                '<input type="text"  ng-model="row.entity.REVISED_ORDER_QTY" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'ORDER_AMOUNT', field: 'ORDER_AMOUNT', displayName: 'Total AMt', enableFiltering: false, visible: false, width: '10%', cellTemplate:
                '<input type="text"  ng-model="row.entity.ORDER_AMOUNT" disabled="false"  class="pl-sm" />'

        }

        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remarks', enableFiltering: false, width: '15%', visible: false,cellTemplate:
                '<input type="text"  ng-model="row.entity.REMARKS"  class="pl-sm" />'

        }
 
        ,{
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                //'<button style="margin-bottom: 5px;" ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.EditItem(row.entity)" type="button" class="btn btn-outline-success mb-1"><img src="/img/edit-icon.png" height="15px" ></button>' +

                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO == 0" ng-click="grid.appScope.addNewRow()" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                '</div>'
        },

    ];

    $scope.CompanyLoad = function () {

        InsertOrEditServices.GetCompany().then(function (data) {
            
            $scope.model.COMPANY_ID = parseFloat(data.data);

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.CompanyNameLoad = function () {
        
        InsertOrEditServices.GetCompanyName().then(function (data) {
            
            $scope.model.COMPANY_NAME = data.data;

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    
    $scope.GetAdjustmentList = function () {
        
        $scope.showLoader = true;

        AdjustmentServices.GetAdjustmentList($scope.model.COMPANY_ID).then(function (data) {
            
            
            $scope.Adjustments = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    $scope.UnitLoad = function () {

        InsertOrEditServices.GetUnit().then(function (data) {
            
            $scope.model.UNIT_ID = parseInt(data.data);
            $scope.model.ORDER_UNIT_ID = parseInt(data.data);

            for (var i = 0; i < $scope.Units.length; i++) {
                if ($scope.model.UNIT_ID == $scope.Units[i].UNIT_ID) {
                    $scope.model.UNIT_NAME = $scope.Units[i].UNIT_NAME;
                }
            }

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
   
    $scope.Get_Customer_Balance = function () {
        $scope.showLoader = true;

        InsertOrEditServices.Get_Customer_Balance($scope.model.CUSTOMER_ID).then(function (data) {
            
            if (data.data[0].length > 0) {
                $scope.model.CUSTOMER_BALANCE = data.data[0][0].INVOICE_BALANCE;
            }
            if (data.data[1].length > 0) {
                $scope.model.CREDIT_LIMIT = data.data[1][0].CREDIT_LIMIT;
                $scope.model.CREDIT_LIMIT_DAYS = data.data[1][0].CREDIT_DAYS;
            }
   

            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
   

    
    $scope.UnitsLoad = function () {
        $scope.showLoader = true;

        InsertOrEditServices.GetUnitList().then(function (data) {
            
            $scope.Units = data.data.filter(x => x.COMPANY_ID == $scope.model.COMPANY_ID);
            $scope.UnitLoad();
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.LoadProductUnitPrice = function (entity) {
        
        $scope.showLoader = true;

        InsertOrEditServices.LoadProductPrice($scope.model.COMPANY_ID, entity.SKU_ID, $scope.model).then(function (data) {
            
            entity.UNIT_TP = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.LoadPaymentMode = function () {
        var Cash = {
            PAYMENT_MODE: 'Cash'
        }
        var Credit = {
            PAYMENT_MODE: 'Credit'
        }
        $scope.PaymentMode.push(Cash);
        $scope.PaymentMode.push(Credit);

    }

    $scope.LoadStatus = function () {
        var Active = {
            STATUS: 'Active'
        }
        var InActive = {
            STATUS: 'InActive'
        }
        var Complete = {
            STATUS: 'Complete'
        }
        $scope.Status.push(Active);

        $scope.Status.push(InActive);
        $scope.Status.push(Complete);


    }

   
    

    $scope.ClearForm = function () {
        window.location = "/SalesAndDistribution/SalesOrder/InsertOrEdit";

    }
    $scope.GetCompanyDisableStatus = function (entity) {
        
        if ($scope.model.USER_TYPE == "SuperAdmin") {
            return false;
        }
        else {
            return true;
        }
    }
    
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'SalesOrder',
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

    $scope.GetOrderData = function (order_mst_id, order_no, order_unit_id) {
        
        $scope.model.ORDER_MST_ID = parseInt(order_mst_id);
        $scope.model.ORDER_NO = order_no;
        $scope.model.ORDER_UNIT_ID = parseInt(order_unit_id);


    }
    $scope.GetPermissionData();
    $scope.CompanyLoad();
    $scope.CompanyNameLoad();
    $scope.UnitsLoad();
    $scope.UnitLoad();
    $scope.LoadStatus();
    $scope.CustomerLoad();
    $scope.InvoiceTypeLoad();
    $scope.ProductsLoad();
    $scope.LoadPaymentMode();
    // This Method work is Edit Data Loading
    $scope.GetEditDataById = function (value) {
        
        $scope.showLoader = true;
        if (value != undefined && value.length > 0) {
            $scope.GetAdjustmentList();
            InsertOrEditServices.GetEditDataById(value).then(function (data) {
                

                if (data.data != null && data.data.Order_Dtls != null && data.data.Order_Dtls.length > 0) {
                    $scope.model = data.data;
                    $scope.OnCustomerSelection();
                    $scope.AdjustmentDataLoad(0);

                    $scope.model.CUSTOMER_ID = JSON.stringify(parseInt($scope.model.CUSTOMER_ID));

                    if (data.data.Order_Dtls != null) {
                        
                        $scope.gridOptions.data = data.data.Order_Dtls;

                        var newRow = {
                            ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID,
                            ORDER_DTL_ID: $scope.gridOptions.data[0].ORDER_DTL_ID,
                            ORDER_MST_ID: $scope.gridOptions.data[0].ORDER_MST_ID,
                            ORDER_AMOUNT: $scope.gridOptions.data[0].ORDER_AMOUNT,
                            ORDER_QTY: $scope.gridOptions.data[0].ORDER_QTY,
                            REMARKS: $scope.gridOptions.data[0].REMARKS,
                            REVISED_ORDER_QTY: $scope.gridOptions.data[0].REVISED_ORDER_QTY,
                            SKU_CODE: $scope.gridOptions.data[0].SKU_CODE,
                            SKU_ID:$scope.gridOptions.data[0].SKU_ID,
                            UNIT_TP: $scope.gridOptions.data[0].UNIT_TP,
                            UNIT_ID: $scope.gridOptions.data[0].UNIT_ID,
                            STATUS: $scope.gridOptions.data[0].STATUS,
                            SPA_UNIT_TP: $scope.gridOptions.data[0].SPA_UNIT_TP,
                            SPA_TOTAL_AMOUNT: $scope.gridOptions.data[0].SPA_TOTAL_AMOUNT,
                            SPA_REQ_TIME_STOCK: $scope.gridOptions.data[0].SPA_REQ_TIME_STOCK,
                            SPA_DISCOUNT_VAL_PCT: $scope.gridOptions.data[0].SPA_DISCOUNT_VAL_PCT,
                            SPA_DISCOUNT_TYPE: $scope.gridOptions.data[0].SPA_DISCOUNT_TYPE,
                            SPA_DISCOUNT_AMOUNT: $scope.gridOptions.data[0].SPA_DISCOUNT_AMOUNT,
                            SPA_CUST_COM: $scope.gridOptions.data[0].SPA_CUST_COM,
                            SPA_AMOUNT: $scope.gridOptions.data[0].SPA_AMOUNT


                        }

                        $scope.gridOptions.data.push(newRow);

                        for (var i = 0; i < $scope.gridOptions.data.length; i++) {
                            
                            $scope.gridOptions.data[i].SKU_ID = JSON.stringify($scope.gridOptions.data[i].SKU_ID);
                        }
                        $scope.gridOptions.data[0] = $scope.GridDefalutData();
                        $scope.model.UNIT_ID = $scope.model.ORDER_UNIT_ID;

                        for (var i = 0; i < $scope.Units.length; i++) {
                            if ($scope.model.UNIT_ID == $scope.Units[i].UNIT_ID) {
                                $scope.model.UNIT_NAME = $scope.Units[i].UNIT_NAME;
                            }
                        }

                    }
                   


                }
                $scope.rowNumberGenerate();
                $interval(function () {
                    $scope.LoadSKU_ID();
                }, 800, 2);
                $interval(function () {
                    $scope.LoadCUSTOMER_ID();
                }, 800, 2);
                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                
                $scope.showLoader = false;
            });
        }
    }
    $scope.LoadSKU_ID = function () {
        $('.sku_Id').trigger('change');

    }
    $scope.LoadCUSTOMER_ID = function () {
        $('#CUSTOMER_ID').trigger('change');

    }



    $scope.SendNotification = function () {
        $scope.showLoader = true;

        LiveNotificationServices.Notification_Permitted_Users(1, $scope.model.COMPANY_ID, $scope.model.UNIT_ID).then(function (data) {
            
            $scope.showLoader = false;

            $scope.Users =  data.data;
            $scope.Permitted_Users = [];
            if ($scope.Users != undefined && $scope.Users != null) {
                for (var i = 0; i < $scope.Users.length; i++) {
                    $scope.Permitted_Users.push(JSON.stringify(parseInt($scope.Users[i].USER_ID)));
                }
                connection.invoke("SendMessage", $scope.Permitted_Users, ": New Order Has been Added/Edited and Ready For Invoice!!!!").catch(function (err) {
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

    $scope.SaveData = function (model) {
        
        $scope.showLoader = true;

        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);
        $scope.model.CUSTOMER_ID = parseInt($scope.model.CUSTOMER_ID);
        $scope.model.INVOICE_STATUS = 'Incomplete';
        $scope.model.ORDER_ENTRY_TYPE = 'Manual';

        const searchIndex = $scope.Customers.findIndex((x) => x.CUSTOMER_ID == $scope.model.CUSTOMER_ID);

        $scope.model.CUSTOMER_CODE = $scope.Customers[searchIndex].CUSTOMER_CODE;

        $scope.gridOptions.data = $scope.gridOptions.data.filter((x) => x.ROW_NO !== 0);
        for (var i in $scope.gridOptions.data) {
                $scope.gridOptions.data[i].SKU_ID = parseInt($scope.gridOptions.data[i].SKU_ID);
                $scope.gridOptions.data[i].ORDER_QTY = parseFloat($scope.gridOptions.data[i].ORDER_QTY);
                $scope.gridOptions.data[i].ORDER_AMOUNT = parseFloat($scope.gridOptions.data[i].ORDER_AMOUNT);
                $scope.gridOptions.data[i].ORDER_UNIT_ID = $scope.model.UNIT_ID;
                $scope.gridOptions.data[i].STATUS = 'Manual';


        }

        $scope.model.Order_Dtls = $scope.gridOptions.data;

        
        InsertOrEditServices.AddOrUpdate(model).then(function (data) {
            
            notificationservice.Notification(data.data.status, 1, 'Data Save Successfully !!');
            
            if (data.data.status == 1) {
                
                $scope.SendNotification();
                
                $scope.showLoader = false;
                $scope.LoadFormData(data.data.key);
            }
            else {
                $scope.showLoader = false;
                $scope.addNewRow();

            }
        });
    }

    //-------------------Adjustment -------------------------------------------\




    $scope.gridOptionsListAdj = (gridregistrationservice.GridRegistration("Ajustment Info"));
    $scope.gridOptionsListAdj.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsListAdj.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }

        , { name: 'ID', field: 'ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , { name: 'ORDER_UNIT_ID', field: 'ORDER_UNIT_ID', visible: false }
        , { name: 'ORDER_MST_ID', field: 'ORDER_MST_ID', visible: false }
        , { name: 'ADJUSTMENT_ID', field: 'ADJUSTMENT_ID', visible: false }

        , {
            name: 'ORDER_NO', field: 'ORDER_NO', displayName: 'Order No', enableFiltering: true, width: '20%'
        }
        , {
            name: 'ADJUSTMENT_NAME', field: 'ADJUSTMENT_NAME', displayName: 'Adjustment Name', enableFiltering: true, width: '20%'
        }
        , {
            name: 'ADJUSTMENT_AMOUNT', field: 'ADJUSTMENT_AMOUNT', displayName: 'Adjustment Amount', enableFiltering: true, width: '20%'
        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '20%'
        }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;"  ng-click="grid.appScope.EditAdjustmentData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];
    $scope.gridOptionsListAdj.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditAdjustmentData(row.entity)\" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"
    $scope.EditAdjustmentData = function (entity) {
        
        $scope.model.ID = entity.ID;
        $scope.model.ADJUSTMENT_ID = entity.ADJUSTMENT_ID;
        $scope.model.ORDER_NO = entity.ORDER_NO;
        $scope.model.ORDER_MST_ID = entity.ORDER_MST_ID;
        $scope.model.ADJUSTMENT_NAME = entity.ADJUSTMENT_NAME;
        $scope.model.ADJUSTMENT_AMOUNT = entity.ADJUSTMENT_AMOUNT;
        $scope.model.ORDER_UNIT_ID = entity.ORDER_UNIT_ID;

        $scope.model.REMARKS = entity.REMARKS;


    }
    $scope.AdjustmentDataLoad = function (companyId) {
        
        $scope.showLoader = true;

        AdjustmentServices.LoadDataByOrderId(companyId, $scope.model.ORDER_MST_ID).then(function (data) {
            
            $scope.gridOptionsListAdj.data = data.data;
            $scope.showLoader = false;
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    

    $scope.SaveAdjustmentData = function (model) {
        
        $scope.showLoader = true;
        $scope.model.ADJUSTMENT_ID = parseInt($scope.model.ADJUSTMENT_ID);

        AdjustmentServices.AddOrUpdate(model).then(function (data) {
            

            notificationservice.Notification(data.data.Status, 1, 'Data Save Successfully !!');
            if (data.data.Status == 1) {
                $scope.AdjustmentDataLoad(0);
                $scope.showLoader = false;
                $scope.model.ID = parseInt(data.data.Key);
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

}]);

