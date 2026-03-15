ngApp.controller('ngGridCtrl', ['$scope', 'InsertOrEditServices', 'RegionInfoServices', 'AreaInfoServices', 'AdjustmentServices', 'LiveNotificationServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', 'uiGridConstants', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InsertOrEditServices, RegionInfoServices, AreaInfoServices, AdjustmentServices, LiveNotificationServices, permissionProvider, notificationservice, gridregistrationservice, uiGridConstants, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {
        COMPANY_ID: 0, UNIT_ID: 0, ORDER_MST_ID:0, CUSTOMER_ID: 0, CUSTOMER_CODE: '', MARKET_ID: 0, TERRITORY_ID: 0, AREA_ID: 0, REGION_ID: 0, DIVISION_ID: 0, PAYMENT_MODE: '', REPLACE_CLAIM_NO: '', BONUS_PROCESS_NO: '', BONUS_CLAIM_NO: '', INVOICE_STATUS: '', ORDER_AMOUNT: 0, ORDER_UNIT_ID: 0, INVOICE_UNIT_ID: 0, REMARKS: '', SPA_TOTAL_AMOUNT: 0, SPA_COMMISSION_PCT: 0, SPA_COMMISSION_AMOUNT: 0, SPA_NET_AMOUNT: 0, REMARKS: ''
      }
    //var connection = new signalR.HubConnectionBuilder().withUrl("/userhub").build();
    //connection.start();
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
    $scope.Products_all = [];
    $scope.Products_specific = [];
    $scope.model.SKU_ID_Specific = [];
    $scope.model.SKU_ID = [];
    $scope.PaymentMode = [];
    $scope.Users = [];
    $scope.model.ORDER_TYPE = 'I0001';
    $scope.model.ORDER_STATUS = 'Active';
    $scope.model.PAYMENT_MODE = 'Cash';
    $scope.BaseProducts = [];
    $scope.Categories = [];
    $scope.Brands = [];
    $scope.Groups = [];
    $scope.Replacements = [];
    $scope.Total_EST_AMOUNT = 0;
    $scope.Products_type = [];
    $scope.Products_specific_Stored = []; 
    $scope.Refurbishments = [];
    $scope.TDS_AMOUNT_TOTAL = 0;
    $scope.Payable_Total = 0;
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
    $scope.gridProductList = (gridregistrationservice.GridRegistration("Product Info"));
    $scope.gridProductList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridProductList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }

        , { name: 'BONUS_DECLARE_ID', field: 'BONUS_DECLARE_ID', visible: false }
        , {
            name: 'BONUS_MST_ID', field: 'BONUS_MST_ID', visible: false, width: '12%'
        }
        , {
            name: 'SKU_ID', field: 'SKU_ID', displayName: 'SKU_ID', enableFiltering: true, width: '20%', visible: false,
        }
        , {
            name: 'BRAND_ID', field: 'BRAND_ID', displayName: 'BRAND_ID', enableFiltering: true, width: '20%', visible: false,
        }
        , { name: 'BASE_PRODUCT_ID', field: 'BASE_PRODUCT_ID', visible: false }

        , {
            name: 'CATEGORY_ID', field: 'CATEGORY_ID', displayName: 'Name', enableFiltering: true, width: '20%', visible: false,
        }
        , {
            name: 'GROUP_ID', field: 'GROUP_ID', displayName: 'GROUP_ID', enableFiltering: true, width: '18%', visible: false,
        }

        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '12%'
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '12%'
        }
        , {
            name: 'PACK_SIZE', field: 'PACK_SIZE', displayName: 'Pack Size', enableFiltering: true, width: '10%'
        }
        , {
            name: 'GROUP_NAME', field: 'GROUP_NAME', displayName: 'Group', enableFiltering: true, width: '12%'
        }
        , {
            name: 'BRAND_NAME', field: 'BRAND_NAME', displayName: 'Brand', enableFiltering: true, width: '12%'
        }
        , {
            name: 'CATEGORY_NAME', field: 'CATEGORY_NAME', displayName: 'Category', enableFiltering: true, width: '12%'
        }
        , {
            name: 'BASE_PRODUCT_NAME', field: 'BASE_PRODUCT_NAME', displayName: 'Base Product', enableFiltering: true, width: '12%'
        }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-disabled="row.entity.BONUS_DECLARE_ID>0" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.removeItemDeclareProduct(row.entity)" type="button" class="btn btn-outline-primary mb-1">Delete</button>' +
                '</div>'
        },

    ];


    //Generate New Row No
    $scope.rowNumberGenerate = function () {
        
        for (let i = 0; i < $scope.gridOptions.data.length; i++) {
            
            $scope.gridOptions.data[i].ROW_NO = i;
                      
        }

    }
    $scope.Calculate_Total_Est_Amount = function () {
        $scope.model.ORDER_AMOUNT = 0;
        for (let i = 0; i < $scope.gridOptions.data.length; i++) {

            $scope.model.ORDER_AMOUNT = $scope.model.ORDER_AMOUNT +  $scope.gridOptions.data[i].ORDER_QTY * $scope.gridOptions.data[i].UNIT_TP;


        }

    }

    $scope.OnCustomerSelection = () => {
        //if ($scope.model.ORDER_MST_ID == 0) {
        //    $scope.gridOptions.data.splice(1, $scope.gridOptions.data.length);

        //}

        if ($scope.Customers.length > 0) {
            var index = $scope.Customers.findIndex(x => x.CUSTOMER_ID == $scope.model.CUSTOMER_ID);
            if (index != -1) {
                $scope.model.CUSTOMER_ADDRESS = $scope.Customers[index].CUSTOMER_ADDRESS;
                $scope.model.CUSTOMER_CODE = $scope.Customers[index].CUSTOMER_CODE;

                $scope.model.MARKET_ID = $scope.Customers[index].MARKET_ID;
                $scope.model.TERRITORY_ID = $scope.Customers[index].TERRITORY_ID;
                $scope.model.AREA_ID = $scope.Customers[index].AREA_ID;
                $scope.model.REGION_ID = $scope.Customers[index].REGION_ID;
                $scope.model.DIVISION_ID = $scope.Customers[index].DIVISION_ID;
                $scope.model.MARKET_NAME = $scope.Customers[index].MARKET_NAME;
            }
        }
        $scope.Get_Customer_Balance();
        if ($scope.model.ORDER_TYPE == 'I0002') {
            $scope.LoadReplacementDataFiltered();
        }
        if ($scope.model.ORDER_TYPE == 'I0009') {
            $scope.LoadRefurbishmentDataFiltered();
        }
        $scope.LoadProductType();

        
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
                    SKU_NAME: $scope.gridOptions.data[0].SKU_NAME,

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
                    SPA_AMOUNT: $scope.gridOptions.data[0].SPA_AMOUNT,
                    CURRENT_STOCK: $scope.gridOptions.data[0].CURRENT_STOCK,
                    DISTRIBUTOR_STOCK: $scope.gridOptions.data[0].DISTRIBUTOR_STOCK,
                    TRANSIT_STOCK: $scope.gridOptions.data[0].TRANSIT_STOCK,

                    SUGGESTED_QTY: $scope.gridOptions.data[0].SUGGESTED_QTY,
                    TARGET_QTY: $scope.gridOptions.data[0].TARGET_QTY,
                    MAXIMUM_QTY: $scope.gridOptions.data[0].MAXIMUM_QTY,
                    MINIMUM_QTY: $scope.gridOptions.data[0].MINIMUM_QTY,
                    REMAINING_QTY: $scope.gridOptions.data[0].REMAINING_QTY,
                    SHIPPER_QTY: $scope.gridOptions.data[0].SHIPPER_QTY,
                    NET_ORDER_AMOUNT: $scope.gridOptions.data[0].NET_ORDER_AMOUNT,
                    CUSTOMER_DISC_AMOUNT: $scope.gridOptions.data[0].CUSTOMER_DISC_AMOUNT,
                    CUSTOMER_ADD1_DISC_AMOUNT: $scope.gridOptions.data[0].CUSTOMER_ADD1_DISC_AMOUNT,
                    CUSTOMER_ADD2_DISC_AMOUNT: $scope.gridOptions.data[0].CUSTOMER_ADD2_DISC_AMOUNT,
                    CUSTOMER_PRODUCT_DISC_AMOUNT: $scope.gridOptions.data[0].CUSTOMER_PRODUCT_DISC_AMOUNT,
                    CUST_PROD_ADD1_DISC_AMOUNT: $scope.gridOptions.data[0].CUST_PROD_ADD1_DISC_AMOUNT,
                    CUST_PROD_ADD2_DISC_AMOUNT: $scope.gridOptions.data[0].CUST_PROD_ADD2_DISC_AMOUNT,
                    PROD_BONUS_PRICE_DISC_AMOUNT: $scope.gridOptions.data[0].PROD_BONUS_PRICE_DISC_AMOUNT,
                    LOADING_CHARGE_AMOUNT: $scope.gridOptions.data[0].LOADING_CHARGE_AMOUNT,
                    INVOICE_ADJUSTMENT_AMOUNT: $scope.gridOptions.data[0].INVOICE_ADJUSTMENT_AMOUNT,
                    BONUS_PRICE_DISC_AMOUNT: $scope.gridOptions.data[0].BONUS_PRICE_DISC_AMOUNT,
                    STOCK_AFTER_INVOICE: ($scope.gridOptions.data[0].CURRENT_STOCK - $scope.gridOptions.data[0].ORDER_QTY - $scope.gridOptions.data[0].BONUS_QTY),
                    SKU_SHIPPER_DETAILS: $scope.gridOptions.data[0].SKU_SHIPPER_DETAILS,
                    BONUS_QTY: $scope.gridOptions.data[0].BONUS_QTY,
                    SKU_TYPE: $scope.gridOptions.data[0].SKU_TYPE
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
                $scope.gridOptions.data[0] = $scope.GridDefalutData();

            }
        } else {
            notificationservice.Notification("Please Select Valid Product First", "", 'No Product Selected!!');

        }
        $scope.rowNumberGenerate();
    };
    $scope.CustomerLoad = function () {

        $scope.showLoader = true;
        $scope.Customers = [];

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
           
            if ($scope.model.CUSTOMER_ID > 0) {
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
                    }
                }
            } 

               if ($scope.model.USER_TYPE == 'Distributor' && $scope.model.ORDER_MST_ID == 0) {
                    $scope.GetDistributorCode();

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
            if ($scope.model.USER_TYPE == 'Distributor') {
                $scope.InvoiceTypes = data.data.filter(x => x.INVOICE_TYPE_CODE == 'I0001' || x.INVOICE_TYPE_CODE == 'I0005');
            }

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
   
    $scope.LoadBaseProductData = function () {
        $scope.showLoader = true;

        InsertOrEditServices.LoadBaseProductData().then(function (data) {
            $scope.BaseProducts = data.data;
            var _BaseProducts = {
                BASE_PRODUCT_ID: "0",
                BASE_PRODUCT_NAME: "All",
                BASE_PRODUCT_CODE: "ALL",

            }
            $scope.BaseProducts.push(_BaseProducts);
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            console.log(error);
            $scope.showLoader = false;

        });
    }
    $scope.LoadBrandData = function () {
        $scope.showLoader = true;

        InsertOrEditServices.LoadBrandData().then(function (data) {
            $scope.Brands = data.data;
            var _Brands = {
                BRAND_ID: "0",
                BRAND_NAME: "All",
                BRAND_CODE: "ALL",

            }
            $scope.Brands.push(_Brands);
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            console.log(error);
            $scope.showLoader = false;

        });
    }

    $scope.OnBrandSelection = function () {
        $scope.showLoader = true;

        $scope.Products = $scope.Products.filter(x => x.BRAND_ID == $scope.model.BRAND_ID);
        $scope.showLoader = false;

    }
    $scope.OnCategorySelection = function () {
        $scope.showLoader = true;

        $scope.Products = $scope.Products.filter(x => x.CATEGORY_ID == $scope.model.CATEGORY_ID);
        if ($scope.Products_specific_Stored.length > $scope.Products_specific.length) {
            $scope.Products_specific = $scope.Products_specific_Stored;
        }else{
            $scope.Products_specific_Stored = $scope.Products_specific;

        }
        $scope.Products_specific = $scope.Products_specific_Stored.filter(x => x.CATEGORY_ID == $scope.model.CATEGORY_ID); 

        $scope.showLoader = false;

    }
    $scope.OnUnitSelection = function () {
        $scope.showLoader = true;
        $scope.Products = $scope.Products.filter(x => x.CATEGORY_ID == $scope.model.CATEGORY_ID);
        if ($scope.Products_specific_Stored.length > $scope.Products_specific.length) {
            $scope.Products_specific = $scope.Products_specific_Stored;
        } else {
            $scope.Products_specific_Stored = $scope.Products_specific;

        }
        $scope.specific_products = [];
        for (var i = 0; i < $scope.Products_specific_Stored.length; i++) {
            if ($scope.Products_specific_Stored[i].SKU_ID == 0) {
                $scope.specific_products.push($scope.Products_specific_Stored[i]);

            } else {
                var kt = $scope.Products_specific_Stored[i].PRODUCT_UNIT_ID.split(',');
                var state = kt.filter(x => x == $scope.model.UNIT_ID);

                if (state.length > 0) {
                    $scope.specific_products.push($scope.Products_specific_Stored[i]);
                }
            }
           
        }
        $scope.Products_specific = $scope.specific_products;
        //$scope.Products_specific = $scope.Products_specific_Stored.filter(x => x.CATEGORY_ID == $scope.model.CATEGORY_ID);
        $scope.LoadProductsSpecific();
        //$scope.GridDefalutData();
        $scope.showLoader = false;

    }
    $scope.OnBaseProductSelection = function () {
        $scope.showLoader = true;

        $scope.Products = $scope.Products.filter(x => x.BASE_PRODUCT_ID == $scope.model.BASE_PRODUCT_ID);
        $scope.showLoader = false;

    }
    $scope.OnFilterSelection = function (selection) {
        if ($scope.model.CUSTOMER_ID > 0) {
            $scope.showLoader = true;
            if (selection == 'Group' && $scope.model.GROUP_ID.length > 0) {
                $scope.Products = [];
                for (var i = 0; i < $scope.model.GROUP_ID.length; i++) {
                    $scope.Products = [...$scope.Products, ...$scope.Products_all.filter(x => x.GROUP_ID == $scope.model.GROUP_ID[i])];

                }
            } else if (selection == 'Group') {
                $scope.Products = $scope.Products_all;
            }
            if (selection == 'Brand' && $scope.model.BRAND_ID.length > 0) {
                $scope.Products = [];

                if ($scope.model.GROUP_ID != undefined && $scope.model.GROUP_ID.length > 0) {

                    for (var i = 0; i < $scope.model.GROUP_ID.length; i++) {
                        $scope.Products = [...$scope.Products, ...$scope.Products_all.filter(x => x.GROUP_ID == $scope.model.GROUP_ID[i])];

                    }
                    if ($scope.model.BRAND_ID != undefined) {
                        for (var i = 0; i < $scope.model.BRAND_ID.length; i++) {
                            $scope.Products = [...$scope.Products, ...$scope.Products.filter(x => x.BRAND_ID == $scope.model.BRAND_ID[i])];
                        }
                    }

                } else {
                    for (var i = 0; i < $scope.model.BRAND_ID.length; i++) {
                        $scope.Products = [...$scope.Products, ...$scope.Products_all.filter(x => x.BRAND_ID == $scope.model.BRAND_ID[i])];
                    }
                }

            }
            else if (selection == 'Brand') {
                $scope.Products = $scope.Products_all;

            }
            if (selection == 'Category' && $scope.model.CATEGORY_ID.length > 0) {
                $scope.Products = [];
                if ($scope.model.BRAND_ID != undefined && $scope.model.BRAND_ID.length > 0) {
                    for (var i = 0; i < $scope.model.BRAND_ID.length; i++) {
                        $scope.Products = [...$scope.Products, ...$scope.Products_all.filter(x => x.BRAND_ID == $scope.model.BRAND_ID[i])];
                    }
                    if ($scope.model.CATEGORY_ID != undefined) {
                        for (var i = 0; i < $scope.model.CATEGORY_ID.length; i++) {
                            $scope.Products = [...$scope.Products, ...$scope.Products.filter(x => x.CATEGORY_ID == $scope.model.CATEGORY_ID[i])];
                        }
                    }
                }
                else {
                    for (var i = 0; i < $scope.model.CATEGORY_ID.length; i++) {
                        $scope.Products = [...$scope.Products, ...$scope.Products_all.filter(x => x.CATEGORY_ID == $scope.model.CATEGORY_ID[i])];

                        if ($scope.Products_specific_Stored.length == 0) {
                            $scope.Products_specific_Stored = $scope.Products_specific;
                        }
                        if (i == 0) {
                            $scope.Products_specific = $scope.Products_specific_Stored.filter(x => x.CATEGORY_ID == $scope.model.CATEGORY_ID[i]);
                        } else {
                            $scope.Products_specific = [...$scope.Products_specific, ...$scope.Products_specific_Stored.filter(x => x.CATEGORY_ID == $scope.model.CATEGORY_ID[i])];

                        }


                    }
                }

            }
            else if (selection == 'Category') {
                $scope.Products = $scope.Products_all;
                if ($scope.Products_specific_Stored.length > $scope.Products_specific.length) {
                    $scope.Products_specific = $scope.Products_specific_Stored;
                } else {
                    $scope.Products_specific_Stored = $scope.Products_specific;

                }
                $scope.Products_specific = $scope.Products_specific_Stored.filter(x => x.CATEGORY_ID == $scope.model.CATEGORY_ID); 

            }
            if (selection == 'BaseProduct' && $scope.model.BASE_PRODUCT_ID.length > 0) {
                $scope.Products = [];
                if ($scope.model.BRAND_ID != undefined && $scope.BRAND_ID.length > 0) {
                    for (var i = 0; i < $scope.model.BRAND_ID.length; i++) {
                        $scope.Products = [...$scope.Products, ...$scope.Products_all.filter(x => x.BRAND_ID == $scope.model.BRAND_ID[i])];
                    }

                    for (var i = 0; i < $scope.model.BASE_PRODUCT_ID.length; i++) {
                        $scope.Products = [...$scope.Products, ...$scope.Products.filter(x => x.BASE_PRODUCT_ID == $scope.model.BASE_PRODUCT_ID[i])];
                    }
                }
                else {
                    for (var i = 0; i < $scope.model.BASE_PRODUCT_ID.length; i++) {
                        $scope.Products = [...$scope.Products, ...$scope.Products_all.filter(x => x.BASE_PRODUCT_ID == $scope.model.BASE_PRODUCT_ID[i])];
                    }
                }

            }
            else if (selection == 'BaseProduct') {
                $scope.Products = $scope.Products_all;
            }
            $scope.showLoader = false;
        } else {
            notificationservice.Notification("Please Select Customer!!", "", '');

        }
      

    }
   
    $scope.LoadCategoryData = function () {
        $scope.showLoader = true;

        InsertOrEditServices.LoadCategoryData().then(function (data) {
            $scope.Categories = data.data;
            var _Categories = {
                CATEGORY_ID: "0",
                CATEGORY_NAME: "All",
                CATEGORY_CODE: "ALL",

            }
            $scope.Categories.push(_Categories);
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            console.log(error);
            $scope.showLoader = false;

        });
    }
    $scope.LoadGroupData = function () {
        $scope.showLoader = true;

        InsertOrEditServices.LoadGroupData().then(function (data) {
            $scope.Groups = data.data;
            var _Groups = {
                GROUP_ID: "0",
                GROUP_NAME: "All",
                GROUP_CODE: "ALL",

            }
            $scope.Groups.push(_Groups);
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            console.log(error);
            $scope.showLoader = false;

        });
    }

    $scope.LoadReplacementDataFiltered = function () {
        if ($scope.model.ORDER_TYPE == 'I0002') {
            $scope.showLoader = true;
            InsertOrEditServices.Replacement_Master($scope.model.CUSTOMER_ID, $scope.model.ORDER_MST_ID ?? 0).then(function (data) {
                $scope.Replacements = data.data;

                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                console.log(error);
                $scope.showLoader = false;

            });
        }
        

    }
    $scope.LoadRefurbishmentDataFiltered = function () {
        if ($scope.model.ORDER_TYPE == 'I0009') {
            $scope.showLoader = true;

            InsertOrEditServices.Refurbishment_Master($scope.model.CUSTOMER_ID).then(function (data) {
                $scope.Refurbishments = data.data;

                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                console.log(error);
                $scope.showLoader = false;

            });
        }


    }
    $scope.LoadRefurbishmentDtlDataFiltered = function () {
        $scope.showLoader = true;

        InsertOrEditServices.Refurbishment_Dtl($scope.model.REFURBISHMENT_CLAIM_NO).then(function (data) {
            $scope.OnProductSelectSelection(data.data);
            $scope.gridOptions.data = data.data;
            $scope.addNewRow();

            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            console.log(error);
            $scope.showLoader = false;

        });

    }
    $scope.LoadReplacementDtlDataFiltered = function () {
        $scope.showLoader = true;
        InsertOrEditServices.Replacement_Dtl($scope.model.REPLACE_CLAIM_NO, $scope.model.ORDER_UNIT_ID).then(function (data) {
                $scope.OnProductSelectSelection(data.data);
                $scope.gridOptions.data = data.data;
                $scope.addNewRow();

            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            console.log(error);
            $scope.showLoader = false;

        });

    }
    $scope.OnSpecificProductChange = function () {
        if ($scope.model.SKU_ID_Specific.length > 0) {
            $scope.model.SKU_ID = $scope.model.SKU_ID_Specific;

        } else {
            //for (var i = 0; i < $scope.Products_specific.length; i++) {
            //    if (i > 0) {
            //        $scope.model.SKU_ID.push(JSON.stringify(parseInt($scope.Products_specific[i].SKU_ID)));

            //    }

            //}
            notificationservice.Notification("No Products has been selected!!Select at least 1 product", 1, "No Products has been selected!!Select at least 1 product");

        }

        //$scope.model.SKU_ID = $scope.model.SKU_ID_Specific;
        $scope.LoadProductsDataFiltered();
    }
    $scope.OnSpecificProductTypeChange = function (type) {
        InsertOrEditServices.LoadProductsSpecificType($scope.model.COMPANY_ID, $scope.model.CUSTOMER_ID, type).then(function (data)
        {
            $scope.Products_specific_Typewise = [];
            if (data.data.length > 0) {
                for (var i = 0; i < data.data.length; i++) {
                    var kt = data.data[i].PRODUCT_UNIT_ID.split(',');
                    var state = kt.filter(x => x == $scope.model.UNIT_ID);

                    if (state.length > 0) {
                        $scope.Products_specific_Typewise.push(data.data[i]);
                    }
                }
                //$scope.Products_specific = data.data.filter(x => x.PRODUCT_UNIT_ID == $scope.model.UNIT_ID);
                for (var i = 0; i < $scope.Products_specific_Typewise.length; i++) {
                  
                    $scope.model.SKU_ID.push(JSON.stringify(parseInt($scope.Products_specific_Typewise[i].SKU_ID)));
                }
                $scope.LoadProductsDataFilteredSpecific();

            } else {
                notificationservice.Notification("No Products set for this type!!", 1, "Not Products set for this type!!");
            }
           
            //$scope.LoadProductsDataFiltered();

            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            $scope.showLoader = false;

        });

        

        //$scope.model.SKU_ID = $scope.model.SKU_ID_Specific;
    }
    $scope.LoadProductsDataFilteredSpecific = function () {
        $scope.showLoader = true;
        if ($scope.model.SKU_ID.length > 0) {
            InsertOrEditServices.LoadProductsDataFiltered($scope.model.COMPANY_ID, $scope.model).then(function (data) {
                for (var i = 0; i < data.data.length; i++) {
                    $scope.OnProductSelectSelection(data.data[i]);
                    $scope.gridOptions.data[0] = data.data[i];
                    $scope.addNewRow();
                    $scope.model.SKU_ID = [];
                    $scope.model.SKU_ID_Specific = [];

                    //$interval(function () {
                    //    $scope.LoadSKU_IDMASTER();
                    //}, 800, 4);
                    //$interval(function () {
                    //    $scope.LoadSKU_ID_Specific();
                    //}, 800, 4);
                }

                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                console.log(error);
                $scope.showLoader = false;

            });
        }
        else {
            notificationservice.Notification("Sorry, No product can be listed from this depo for this product relation type", "", 'Only Single Row Left!!');

        }

    }

    $scope.LoadProductsDataFiltered = function () {
        $scope.showLoader = true;
        if ($scope.model.SKU_ID.length == 0) {
            notificationservice.Notification("Please Select at least one product", "", "Please Select at least one product");
            $scope.showLoader = false;

        } else {
            InsertOrEditServices.LoadProductsDataFiltered($scope.model.COMPANY_ID, $scope.model).then(function (data) {


                for (var i = 0; i < data.data.length; i++) {
                    $scope.OnProductSelectSelection(data.data[i]);
                    $scope.gridOptions.data[0] = data.data[i];
                    $scope.addNewRow();
                    $scope.model.SKU_ID = [];
                    $scope.model.SKU_ID_Specific = [];

                    //$interval(function () {
                    //    $scope.LoadSKU_IDMASTER();
                    //}, 800, 4);
                    //$interval(function () {
                    //    $scope.LoadSKU_ID_Specific();
                    //}, 800, 4);
                }

                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                console.log(error);
                $scope.showLoader = false;

            });
        }
           

    }

    $scope.LoadBaseProductData();
    $scope.LoadBrandData();
    $scope.LoadCategoryData();
    $scope.LoadGroupData();
    $scope.ProductsLoad = function () {

        $scope.showLoader = true;
        if ($scope.Products_all != null && $scope.Products_all != undefined && $scope.Products_all.length > 0) {
            $scope.Products = $scope.Products_all;
            $scope.LoadProductsSpecific();

        } else {
            InsertOrEditServices.LoadProductData($scope.model.COMPANY_ID, $scope.model.CUSTOMER_ID).then(function (data) {
                $scope.Products = [];
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
                $scope.Products_all = $scope.Products;
                if ($scope.Products.length > 1) {
                    $scope.LoadProductsSpecific();
                }
              
                $scope.showLoader = false;
            }, function (error) {

                $scope.showLoader = false;

            });
        }
       
    }
    $scope.LoadProductsSpecific = function () {
        $scope.showLoader = true;
        
        InsertOrEditServices.LoadProductsSpecific($scope.model.COMPANY_ID, $scope.model.CUSTOMER_ID).then(function (data) {
            $scope.Products_data_specific = data.data;
                var _Products = {
                    SKU_ID: "0",
                    SKU_NAME: "None",
                    SKU_CODE: "None",
                    PACK_SIZE: "None",

            }
            $scope.Products_specific = [];
            $scope.Products_specific.push(_Products);
            for (var i = 0; i < $scope.Products_data_specific.length; i++) {
                var kt = $scope.Products_data_specific[i].PRODUCT_UNIT_ID.split(',');
                var state = kt.filter(x => x == $scope.model.UNIT_ID);

                if (state.length > 0) {
                    $scope.Products_specific.push($scope.Products_data_specific[i]);
                }
            }

            //for (var i in $scope.Products_data_specific.filter(x => x.PRODUCT_UNIT_ID == $scope.model.UNIT_ID)) {
            //        $scope.Products_specific.push($scope.Products_data_specific[i]);
            //    }
                

                $scope.showLoader = false;
            }, function (error) {

                $scope.showLoader = false;

            });
        
    }

    $scope.LoadProductType = function () {

        $scope.showLoader = true;

        InsertOrEditServices.LoadProductType($scope.model.COMPANY_ID, $scope.model.CUSTOMER_ID).then(function (data) {
            $scope.Products_type = data.data;
            
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });


    }

    $scope.OnProductSelectSelection = (entity) => {
        $scope.LoadProductUnitPrice(entity);
        if ($scope.model.ORDER_MST_ID > 0) {
            var index = $scope.Products.findIndex(x => x.SKU_ID == entity.SKU_ID);
            //var index = $scope.Products_specific.findIndex(x => x.SKU_ID == entity.SKU_ID);

            if (index != -1) {
                entity.SKU_ID = $scope.Products[index].SKU_ID;
                entity.SKU_NAME = $scope.Products[index].SKU_NAME;
                entity.SKU_CODE = $scope.Products[index].SKU_CODE;
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
                entity.ORDER_DTL_ID = 0;
                entity.ORDER_MST_ID = 0;
                entity.ORDER_AMOUNT = 0;
                entity.REMARKS = "";
                entity.NET_ORDER_AMOUNT = 0;
                entity.CUSTOMER_DISC_AMOUNT = 0;
                entity.CUSTOMER_ADD1_DISC_AMOUNT = 0;
                entity.CUSTOMER_ADD2_DISC_AMOUNT = 0;
                entity.CUSTOMER_PRODUCT_DISC_AMOUNT = 0;
                entity.PROD_BONUS_PRICE_DISC_AMOUNT = 0;
                entity.LOADING_CHARGE_AMOUNT = 0;
                entity.INVOICE_ADJUSTMENT_AMOUNT = 0;
                entity.BONUS_PRICE_DISC_AMOUNT = 0;
                entity.STOCK_AFTER_INVOICE = ($scope.gridOptions.data[0].CURRENT_STOCK - $scope.gridOptions.data[0].ORDER_QTY - $scope.gridOptions.data[0].BONUS_QTY)
                entity.SKU_SHIPPER_DETAILS = 0;
                entity.BONUS_QTY = 0;
                entity.SKU_TYPE = '';
                entity.TRANSIT_STOCK = 0;

            } else {
                //var index = $scope.Products.findIndex(x => x.SKU_ID == entity.SKU_ID);
                var index = $scope.Products_specific.findIndex(x => x.SKU_ID == entity.SKU_ID);

                if (index != -1) {
                    entity.SKU_ID = $scope.Products_specific[index].SKU_ID;
                    entity.SKU_NAME = $scope.Products_specific[index].SKU_NAME;
                    entity.SKU_CODE = $scope.Products_specific[index].SKU_CODE;
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
                    entity.ORDER_DTL_ID = 0;
                    entity.ORDER_MST_ID = 0;
                    entity.ORDER_AMOUNT = 0;
                    entity.REMARKS = "";
                    entity.NET_ORDER_AMOUNT = 0;
                    entity.CUSTOMER_DISC_AMOUNT = 0;
                    entity.CUSTOMER_ADD1_DISC_AMOUNT = 0;
                    entity.CUSTOMER_ADD2_DISC_AMOUNT = 0;
                    entity.CUSTOMER_PRODUCT_DISC_AMOUNT = 0;
                    entity.PROD_BONUS_PRICE_DISC_AMOUNT = 0;
                    entity.LOADING_CHARGE_AMOUNT = 0;
                    entity.INVOICE_ADJUSTMENT_AMOUNT = 0;
                    entity.BONUS_PRICE_DISC_AMOUNT = 0;
                    entity.STOCK_AFTER_INVOICE = ($scope.gridOptions.data[0].CURRENT_STOCK - $scope.gridOptions.data[0].ORDER_QTY - $scope.gridOptions.data[0].BONUS_QTY)
                    entity.SKU_SHIPPER_DETAILS = 0;
                    entity.BONUS_QTY = 0;
                    entity.SKU_TYPE = '';
                    entity.TRANSIT_STOCK = 0;

                }
            }
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

    $scope.OnProductBoxChange = (entity) => {
        if (entity.BOX_QTY < 1) {
            alert('you cannot select fractional box quantity');
            return;
        }

        let orderQuantiy = entity.BOX_QTY * entity.SHIPPER_QTY;
        if (entity.ROW_NO > 0) {
            if (entity.ORDER_QTY != '') {
                InsertOrEditServices.LoadProductPerfactOrderQty($scope.model.CUSTOMER_CODE, $scope.model.ORDER_TYPE, $scope.model.ORDER_UNIT_ID, entity.SKU_CODE, 0, orderQuantiy).then(function (data) {
                    if (data.data.length > 0) {
                        for (var i = 0; i < data.data.length; i++) {
                            if (data.data[i].SKU_CODE == entity.SKU_CODE && data.data[i].BONUS_SKU_CODE == entity.SKU_CODE) {
                                const resOrderAndBonus = calculateTotalOrderAndBonus(orderQuantiy, data.data[i].SLAB_QTY, data.data[i].DECLARE_BONUS_QTY);
                                entity.ORDER_QTY = resOrderAndBonus.total_order_qty;
                                //entity.BONUS_QTY = resOrderAndBonus.total_bonus_qty
                                $scope.OnProductQTYChange(entity)
                                break;
                            }
                        }
                    }
                    else {
                        entity.ORDER_QTY = orderQuantiy;
                        entity.BONUS_QTY = 0;

                        $scope.OnProductQTYChange(entity)

                    }
                    $scope.showLoader = false;
                }, function (error) {

                    $scope.showLoader = false;

                });

            }
        }
    }
    function calculateTotalOrderAndBonus(total_qty, slab_qty, bonus_qty) {
        let total_order_qty = 0;
        let total_bonus_qty = 0;
        let remaining_qty = total_qty;

        while (remaining_qty >= slab_qty) {
            total_order_qty += slab_qty;
            total_bonus_qty += bonus_qty;
            remaining_qty -= (slab_qty + bonus_qty);
        }

        if (remaining_qty > 0) {
            total_order_qty += remaining_qty;
        }

        return { total_order_qty, total_bonus_qty };
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
                SPA_AMOUNT: entity.SPA_AMOUNT,
                SKU_SHIPPER_DETAILS: entity.SKU_SHIPPER_DETAILS,
                BONUS_QTY: entity.BONUS_QTY,
                SKU_TYPE: entity.SKU_TYPE,
                TRANSIT_STOCK: entity.TRANSIT_STOCK
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
        //{
        //    name: '#', field: 'ROW_NO', enableFiltering: false, width: '30',pinnedLeft: true
        //}
        {
            name: '#',
            field: 'ROW_NO',
            enableFiltering: false,
            width: '30',
            pinnedLeft: true,
            cellTemplate:
                '<div ng-style="{\'background-color\': row.entity.ORDER_QTY > row.entity.CURRENT_STOCK ? \'red\' : \'white\', \'color\': row.entity.ORDER_QTY > row.entity.CURRENT_STOCK ? \'white\' : \'black\'}" class="ui-grid-cell-contents">{{row.entity.ROW_NO}}</div>'
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
        , { name: 'SKU_ID', field: 'SKU_ID', visible: false }
     
        , {
            name: 'SKU_ID', field: 'SKU_ID', displayName: 'Product (Name | Code | Packe Size)', enableFiltering: false, width: '24%', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #9ed4a0;color: Black">Disc Amt: {{grid.appScope.model.DISCOUNT_AMOUNT}}     <span style="margin-left:5px;margin-right:5px">|</span> Adj Amt: {{grid.appScope.model.ADJUSTMENT_AMOUNT}}</div>',  cellTemplate:
                '<div style="font-size: 12px;font-family: \'Bahnschrift Condensed\', sans-serif;" ng-style="{\'background-color\': row.entity.ORDER_QTY > row.entity.CURRENT_STOCK ? \'red\' : \'white\', \'color\': row.entity.ORDER_QTY > row.entity.CURRENT_STOCK ? \'white\' : \'black\'}" type="text"  ng-model="row.entity.SKU_CODE"><select ng-hide="row.entity.ROW_NO>0" class="select2-single sku_Id form-control" id="SKU_ID" ng-disabled="row.entity.ROW_NO !=0 "' +
                'name="SKU_ID" ng-model="row.entity.SKU_ID" style="width:100%;" ng-change="grid.appScope.OnProductSelectSelection(row.entity)" >' +
                '<option ng-repeat="item in grid.appScope.Products_specific | orderBy : \'SKU_NAME\'" ng-selected="item.SKU_ID == row.entity.SKU_ID" value="{{item.SKU_ID}}">{{ item.SKU_NAME }} | Code: {{ item.SKU_CODE }} | Pack Size: {{ item.PACK_SIZE }} </option>' +
                '</select>'
                + '<input ng-hide="row.entity.ROW_NO == 0" type="text"  ng-model="row.entity.SKU_NAME" disabled="true"  class="pl-sm" style="width: 100%; word-wrap: break-word; white-space: normal;" /></div>', pinnedLeft: true

        }
       
        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: false, width: '79', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #9ed4a0;color: Black">TDS: {{grid.appScope.numberToTwoDecimalPlaces(grid.appScope.TDS_AMOUNT_TOTAL)}}</div>', cellTemplate:
                '<input font-family: \'Bahnschrift Condensed\', sans-serif; ng-style="{\'background-color\': row.entity.ORDER_QTY > row.entity.CURRENT_STOCK ? \'red\' : \'white\', \'color\': row.entity.ORDER_QTY > row.entity.CURRENT_STOCK ? \'white\' : \'black\'}" type="text"  ng-model="row.entity.SKU_CODE" disabled="true"  class="pl-sm" />'
            ,pinnedLeft: true
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: false, enableColumnMenu: false, width: '40', cellTemplate: 
                '<input type="text"  ng-model="row.entity.UNIT_TP" disabled="true"  class="pl-sm text-right" />'
        },
        {
            name: 'DISTRIBUTOR_STOCK', field: 'DISTRIBUTOR_STOCK', displayName: 'Dist. Stock', enableFiltering: false, enableColumnMenu: false, width: '45', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #d8d8d8;color: Black; text-align: right;">Net Pay:</div>', cellTemplate: 
                '<input type="number"  ng-model="row.entity.DISTRIBUTOR_STOCK" disabled="false"  class="pl-sm text-right" />'

        },
        {
            name: 'TRANSIT_STOCK', field: 'TRANSIT_STOCK', displayName: 'Tran. Stock', enableFiltering: false, width: '50', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #d8d8d8;color: Black;text-align: left;">{{grid.appScope.numberToTwoDecimalPlaces(grid.appScope.Payable_Total)}}</div>', cellTemplate:
                '<input type="number"  ng-model="row.entity.TRANSIT_STOCK" disabled="false"  class="pl-sm text-right" />'

        }

        , {
            name: 'TARGET_QTY', visible: false, field: 'TARGET_QTY', displayName: 'Target Qty', enableFiltering: false, width: '58', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #d8d8d8;color: Black;  text-align: right;">Order Amt:</div>', cellTemplate:
                '<input type="number" min="0" ng-model="row.entity.TARGET_QTY" ng-disabled="true" class="pl-sm text-right" />'
        }
        , {
            name: 'TARGET_QTY_VIEW',
            field: 'TARGET_QTY_VIEW',
            displayName: 'Target Qty',
            enableFiltering: false,
            width: '50',
            footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #d8d8d8;color: Black;text-align: right;">Order Amt:</div>',
            cellTemplate:
                '<div class="pl-sm text-right">{{row.entity.TARGET_QTY | number:0}}</div>'
        }
        , {
            name: 'REMAINING_QTY', visible: false, field: 'REMAINING_QTY', displayName: 'Rem Qty', enableFiltering: false, width: '42', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #d8d8d8;color: Black;text-align: left;">{{grid.appScope.numberToTwoDecimalPlaces(grid.appScope.model.ORDER_AMOUNT)}}</div>', cellTemplate:
                '<input type="number" min="0" ng-model="row.entity.REMAINING_QTY" ng-disabled="true" class="pl-sm text-right" />'

        }
        , {
            name: 'REMAINING_QTY_VIEW',
            field: 'REMAINING_QTY_VIEW',
            displayName: 'Rem Qty',
            enableFiltering: false,
            width: '42',
            footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #d8d8d8;color: Black;text-align: left;">{{grid.appScope.model.ORDER_AMOUNT}}</div>',
            cellTemplate:
                '<div class="pl-sm text-right">{{row.entity.REMAINING_QTY | number:0}}</div>'
        }
        , {
            name: 'MAXIMUM_QTY', visible: false, field: 'MAXIMUM_QTY', displayName: 'Max Qty', enableFiltering: false, width: '35', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #d8d8d8;color: Black;text-align: right;">Combo:</div>', cellTemplate:
                '<input type="number" min="0" ng-model="row.entity.MAXIMUM_QTY" ng-disabled="true" class="pl-sm text-right" />'

        }
        ,{
            name: 'MAXIMUM_QTY_VIEW',
            field: 'MAXIMUM_QTY_VIEW',
            displayName: 'Max Qty',
            enableFiltering: false,
            width: '35',
            footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #d8d8d8;color: Black;text-align: right;">Combo:</div>',
            cellTemplate:
                '<div class="pl-sm text-right">{{row.entity.MAXIMUM_QTY | number:0}}</div>'
        }
        , {
            name: 'MINIMUM_QTY', visible: false, field: 'MINIMUM_QTY', displayName: 'Min Qty', enableFiltering: false, width: '35', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #d8d8d8;color: Black;text-align: left;">{{grid.appScope.model.COMBO_DISCOUNT }}</div>', cellTemplate:
                '<input type="number" min="0" ng-model="row.entity.MINIMUM_QTY" ng-disabled="true" class="pl-sm text-right" />'
        }
        , {
            name: 'MINIMUM_QTY_VIEW',
            field: 'MINIMUM_QTY_VIEW',
            displayName: 'Min Qty',
            enableFiltering: false,
            width: '35',
            footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #d8d8d8;color: Black;text-align: left;">{{grid.appScope.model.COMBO_DISCOUNT }}</div>',
            cellTemplate:
                '<div class="pl-sm text-right">{{row.entity.MINIMUM_QTY | number:0}}</div>'
        }
        , {
            name: 'SUGGESTED_QTY', visible: false, field: 'SUGGESTED_QTY', displayName: 'Sugg Qty', enableFiltering: false, enableColumnMenu: false, width: '45', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #d8d8d8;color: Black;text-align: right;">Qty:</div>', cellTemplate:
                '<input type="number" min="0" ng-model="row.entity.SUGGESTED_QTY" ng-disabled="true" class="pl-sm text-right" />'
        }
        , {
            name: 'SUGGESTED_QTY_VIEW',
            field: 'SUGGESTED_QTY_VIEW',
            displayName: 'Sugg Qty',
            enableFiltering: false,
            enableColumnMenu: false,
            width: '45',
            footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #d8d8d8;color: Black;text-align: right;">Qty:</div>',
            cellTemplate:
                '<div class="pl-sm text-right">{{row.entity.SUGGESTED_QTY | number:0}}</div>'
        }
        , {
            name: 'BOX_QTY', field: 'BOX_QTY', displayName: 'BOX', enableFiltering: false, width: '40', cellTemplate:
                '<input style="background:white;" type="number" min="0"  ng-model="row.entity.BOX_QTY" ng-disabled="grid.appScope.model.CUSTOMER_ID <1 || row.entity.SKU_TYPE == \'BONUS\'"  ng-change="grid.appScope.OnProductBoxChange(row.entity)"  class="pl-sm text-right" />'

        }
        , {
            name: 'ORDER_QTY', field: 'ORDER_QTY', displayName: 'QTY', enableFiltering: false, width: '5%', aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, cellTemplate:
                '<input style="background:white;" type="number" min="0" id="{{}}" ng-model="row.entity.ORDER_QTY" ng-disabled="grid.appScope.model.CUSTOMER_ID <1 || row.entity.SKU_TYPE == \'BONUS\'  " ng-change="grid.appScope.OnProductQTYChange(row.entity)"  class="pl-sm text-right" />'

        }
        , {
            name: 'BONUS_QTY', field: 'BONUS_QTY', displayName: 'Bonus Qty', enableFiltering: false, enableColumnMenu: false, width: '49', aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, cellTemplate:
                '<input type="number" min="0" ng-model="row.entity.BONUS_QTY" ng-disabled="true"  class="pl-sm text-right" />'

        }
        , {
            name: 'SKU_TYPE', field: 'SKU_TYPE', displayName: 'Type', enableFiltering: false, visible: false, width: '6%', cellTemplate:
                '<input type="text"  ng-model="row.entity.SKU_TYPE" disabled="true"  class="pl-sm " />'

        }
        ,
        , {
            name: 'REVISED_ORDER_QTY', field: 'REVISED_ORDER_QTY', displayName: 'Rev. QTY', enableFiltering: false, visible:false, width: '6%', cellTemplate:
                '<input type="text"  ng-model="row.entity.REVISED_ORDER_QTY" disabled="false"  class="pl-sm text-right" />'

        }
        ,
         {
             name: 'CURRENT_STOCK', field: 'CURRENT_STOCK', displayName: 'Stock', enableFiltering: false, width: '55', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #d8d8d8;color: Black;text-align: right;">Net Amt:</div>', cellTemplate:
                '<input type="number" min="0" ng-model="row.entity.CURRENT_STOCK" ng-hide="{{grid.appScope.model.USER_TYPE == \'Distributor\'}}" ng-disabled="true" class="pl-sm text-right" />'

        }
       
        ,

        {
            name: 'STOCK_AFTER_INVOICE', field: 'STOCK_AFTER_INVOICE', displayName: 'After Invoice Stock', enableFiltering: false,visible:false, width: '6%', cellTemplate:
                '<input type="number"  ng-model="row.entity.STOCK_AFTER_INVOICE" disabled="false"  class="pl-sm" />'

        }
        //, {
        //    name: 'BONUS_QTY', field: 'BONUS_QTY', displayName: 'Bonus Qty', enableFiltering: false, width: '6%', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #9ed4a0;color: Black">Net Amt:</div>',cellTemplate:
        //        '<input type="number"  ng-model="row.entity.BONUS_QTY" disabled="false"  class="pl-sm" />'

        //}
        , {
            name: 'NET_ORDER_AMOUNT', field: 'NET_ORDER_AMOUNT', displayName: 'Net Amt', enableFiltering: false, width: '6%', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #d8d8d8;color: Black;text-align: right;">{{grid.appScope.numberToTwoDecimalPlaces(grid.appScope.model.NET_ORDER_AMOUNT - grid.appScope.model.COMBO_LOADING_CHARGE -  grid.appScope.model.COMBO_DISCOUNT )}}</div>', cellTemplate:
                '<input type="number"  ng-model="row.entity.NET_ORDER_AMOUNT" disabled="false"  class="pl-sm text-right" />'

        }

        , {
            name: 'Action', displayName: 'Action', width: '50', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div  style="margin:1px;">' +
                //'<button style="margin-bottom: 5px;" ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.EditItem(row.entity)" type="button" class="btn btn-outline-success mb-1"><img src="/img/edit-icon.png" height="15px" ></button>' +

                '<button  style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO == 0" ng-click="grid.appScope.addNewRow()" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                '<button ng-style="{\'background-color\': row.entity.ORDER_QTY > row.entity.CURRENT_STOCK ? \'red\' : \'white\', \'color\': row.entity.ORDER_QTY > row.entity.CURRENT_STOCK ? \'white\' : \'black\'}" style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                '</div>', pinnedRight: true
        }
        , {
            name: 'SKU_NAME1', field: 'SKU_NAME', displayName: 'Sku Name', visible:false, enableFiltering: false, width: '17%'

        }
        , {
            name: 'SKU_SHIPPER_DETAILS', field: 'SKU_SHIPPER_DETAILS', displayName: 'Shipper Dtl', enableFiltering: false, width: '8%',  cellTemplate:
                '<input type="text"  ng-model="row.entity.SKU_SHIPPER_DETAILS" disabled="false"  class="pl-sm" />',

        }
        , {
            name: 'LOADING_CHARGE_AMOUNT', field: 'LOADING_CHARGE_AMOUNT', displayName: 'Loading Charge Amt.', enableFiltering: false, width: '10%', cellTemplate:
                '<input type="number"  ng-model="row.entity.LOADING_CHARGE_AMOUNT" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'BONUS_PRICE_DISC_AMOUNT', field: 'BONUS_PRICE_DISC_AMOUNT', displayName: 'Bonus Disc Amt.',visible:false, enableFiltering: false, width: '10%', cellTemplate:
                '<input type="number"  ng-model="row.entity.BONUS_PRICE_DISC_AMOUNT" disabled="false"  class="pl-sm" />'

        }
       
        , {
            name: 'CUSTOMER_PRODUCT_DISC_AMOUNT', field: 'CUSTOMER_PRODUCT_DISC_AMOUNT', displayName: 'Cust. SKU Comm. ', enableFiltering: false, width: '10%', cellTemplate:
                '<input type="number"  ng-model="row.entity.CUSTOMER_PRODUCT_DISC_AMOUNT" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'CUST_PROD_ADD1_DISC_AMOUNT', field: 'CUST_PROD_ADD1_DISC_AMOUNT', displayName: 'Cust. SKU Add1 Comm.', enableFiltering: false, width: '10%', cellTemplate:
                '<input type="number"  ng-model="row.entity.CUST_PROD_ADD1_DISC_AMOUNT" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'CUST_PROD_ADD2_DISC_AMOUNT', field: 'CUST_PROD_ADD2_DISC_AMOUNT', displayName: 'Cust. SKU Add2 Comm.', enableFiltering: false, width: '10%', cellTemplate:
                '<input type="number"  ng-model="row.entity.CUST_PROD_ADD2_DISC_AMOUNT" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'PROD_BONUS_PRICE_DISC_AMOUNT', field: 'PROD_BONUS_PRICE_DISC_AMOUNT', displayName: 'Price Disc Amt.', enableFiltering: false, width: '10%', cellTemplate:
                '<input type="number"  ng-model="row.entity.PROD_BONUS_PRICE_DISC_AMOUNT" disabled="false"  class="pl-sm" />'

        }

        , {
            name: 'CUSTOMER_DISC_AMOUNT', field: 'CUSTOMER_DISC_AMOUNT', displayName: 'Cust. Disc Amt.', enableFiltering: false, width: '10%', cellTemplate:
                '<input type="number"  ng-model="row.entity.CUSTOMER_DISC_AMOUNT" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'CUSTOMER_ADD1_DISC_AMOUNT', field: 'CUSTOMER_ADD1_DISC_AMOUNT', displayName: 'Cust. Disc Amt1.', enableFiltering: false, width: '10%', cellTemplate:
                '<input type="number"  ng-model="row.entity.CUSTOMER_ADD1_DISC_AMOUNT" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'CUSTOMER_ADD2_DISC_AMOUNT', field: 'CUSTOMER_ADD2_DISC_AMOUNT', displayName: 'Cust. Disc Amt2.', enableFiltering: false, width: '10%', cellTemplate:
                '<input type="number"  ng-model="row.entity.CUSTOMER_ADD2_DISC_AMOUNT" disabled="false"  class="pl-sm" />'

        }
 
        

    ];
    $scope.gridOptions.rowTemplate = `<div ng-style='row.entity.MAXIMUM_QTY<0 && grid.appScope.rowStyle'  ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>`
    $scope.rowStyle = {
        "background": "#ffc7bc"
    }
    $scope.gridOptions.showColumnFooter = !$scope.gridOptions.showColumnFooter;
    $scope.gridOptions.columnFooterHeight = 35;
   

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
            if ($scope.model.UNIT_ID == undefined || $scope.model.UNIT_ID == NaN || $scope.model.UNIT_ID == null || $scope.model.UNIT_ID == 0 || $scope.model.ORDER_MST_ID ==0) {
                $scope.model.UNIT_ID = parseInt(data.data);
                $scope.model.ORDER_UNIT_ID = parseInt(data.data);
            } 
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
            $scope.ProductsLoad();


            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.OnUnitSelect = function () {
        $scope.model.ORDER_UNIT_ID = parseInt($scope.model.UNIT_ID);
        $scope.model.INVOICE_UNIT_ID = parseInt($scope.model.UNIT_ID);
        $scope.gridOptions.data.splice(1, $scope.gridOptions.data.length);
        $scope.model.SKU_ID = [];
        $scope.OnUnitSelection()

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
        
        //$scope.showLoader = true;
        entity.UNIT_TP = 0;
        //InsertOrEditServices.LoadProductPrice($scope.model.COMPANY_ID, entity.SKU_ID, $scope.model).then(function (data) {
            
        //    entity.UNIT_TP = data.data;
        //    $scope.showLoader = false;
        //}, function (error) {
        //    alert(error);
            
        //    $scope.showLoader = false;

        //});
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
            STATUS: 'Active',
            STATUS_TEXT: 'Active'
        }
        var InActive = {
            STATUS: 'InActive',
            STATUS_TEXT: 'Active'

        }
        var Complete = {
            STATUS: 'Complete',
            STATUS_TEXT: 'Complete'
        }
        var On_DSM_Pending = {
            STATUS: 'On_DSM_Pending',
            STATUS_TEXT: 'Pending'
        }
        var On_SM_Pending = {
            STATUS: 'On_SM_Pending',
            STATUS_TEXT: 'Pending'
        }
        var On_HOS_Pending = {
            STATUS: 'On_HOS_Pending',
            STATUS_TEXT: 'Pending'
        }
        $scope.Status.push(Active);

        $scope.Status.push(InActive);
        $scope.Status.push(Complete);
        $scope.Status.push(On_DSM_Pending);
        $scope.Status.push(On_SM_Pending);
        $scope.Status.push(On_HOS_Pending);


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
    $scope.LoadStatus();
    $scope.UnitsLoad();

    $scope.CustomerLoad();
    $scope.InvoiceTypeLoad();
    $scope.LoadPaymentMode();
    

    

    $scope.GetDistributorCode = function () {
        $scope.showLoader = true;
        InsertOrEditServices.GetDistributorCode().then(function (data) {
            var dt = $scope.Customers.filter(x => x.CUSTOMER_CODE == data.data);
            if (dt.length > 0) {
                $scope.model.CUSTOMER_ID = JSON.stringify(parseInt(dt[0].CUSTOMER_ID));
                $scope.model.CUSTOMER_NAME = dt[0].CUSTOMER_NAME + ' | ' + dt[0].CUSTOMER_CODE;
               
                $scope.OnCustomerSelection();
            }
          
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }




    // This Method work is Edit Data Loading

    $scope.GetEditDataById = function (value) {
        
        $scope.showLoader = true;
        if (value != undefined && value.length > 0) {
            $scope.GetAdjustmentList();

            InsertOrEditServices.GetEditDataById(value).then(function (data) {
                if (data.data[0] != null && data.data[1] != null && data.data[1].length > 0) {
                    
                    $scope.model = data.data[0][0];
                    $scope.model.UNIT_ID = JSON.stringify($scope.model.INVOICE_UNIT_ID);
                    $scope.AdjustmentDataLoad(0);
                    var Unit_Zero_flag = false;
                   

                    $scope.model.CUSTOMER_ID = JSON.stringify(parseInt($scope.model.CUSTOMER_ID));

                    if (data.data[1] != null) {
                        
                        $scope.gridOptions.data = data.data[1];

                        var newRow = {
                            ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID,
                            ORDER_DTL_ID: $scope.gridOptions.data[0].ORDER_DTL_ID,
                            ORDER_MST_ID: $scope.gridOptions.data[0].ORDER_MST_ID,
                            ORDER_AMOUNT: $scope.gridOptions.data[0].ORDER_AMOUNT,
                            ORDER_QTY: $scope.gridOptions.data[0].ORDER_QTY,
                            REMARKS: $scope.gridOptions.data[0].REMARKS,
                            REVISED_ORDER_QTY: $scope.gridOptions.data[0].REVISED_ORDER_QTY,
                            SKU_NAME: $scope.gridOptions.data[0].SKU_NAME,

                            SKU_CODE: $scope.gridOptions.data[0].SKU_CODE,
                            SKU_ID: $scope.gridOptions.data[0].SKU_ID,
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
                            SPA_AMOUNT: $scope.gridOptions.data[0].SPA_AMOUNT,
                            NET_ORDER_AMOUNT: $scope.gridOptions.data[0].NET_ORDER_AMOUNT,
                            CUSTOMER_DISC_AMOUNT: $scope.gridOptions.data[0].CUSTOMER_DISC_AMOUNT,
                            CUSTOMER_ADD1_DISC_AMOUNT: $scope.gridOptions.data[0].CUSTOMER_ADD1_DISC_AMOUNT,
                            CUSTOMER_ADD2_DISC_AMOUNT: $scope.gridOptions.data[0].CUSTOMER_ADD2_DISC_AMOUNT,
                            CUSTOMER_PRODUCT_DISC_AMOUNT: $scope.gridOptions.data[0].CUSTOMER_PRODUCT_DISC_AMOUNT,
                            CUST_PROD_ADD1_DISC_AMOUNT: $scope.gridOptions.data[0].CUST_PROD_ADD1_DISC_AMOUNT,
                            CUST_PROD_ADD2_DISC_AMOUNT: $scope.gridOptions.data[0].CUST_PROD_ADD2_DISC_AMOUNT,
                            PROD_BONUS_PRICE_DISC_AMOUNT: $scope.gridOptions.data[0].PROD_BONUS_PRICE_DISC_AMOUNT,
                            LOADING_CHARGE_AMOUNT: $scope.gridOptions.data[0].LOADING_CHARGE_AMOUNT == null ? 0 : $scope.gridOptions.data[0].LOADING_CHARGE_AMOUNT,
                            INVOICE_ADJUSTMENT_AMOUNT: $scope.gridOptions.data[0].INVOICE_ADJUSTMENT_AMOUNT,
                            BONUS_PRICE_DISC_AMOUNT: $scope.gridOptions.data[0].BONUS_PRICE_DISC_AMOUNT,
                            STOCK_AFTER_INVOICE: ($scope.gridOptions.data[0].CURRENT_STOCK - $scope.gridOptions.data[0].ORDER_QTY - $scope.gridOptions.data[0].BONUS_QTY),
                            CURRENT_STOCK: $scope.gridOptions.data[0].CURRENT_STOCK,
                            SUGGESTED_QTY: $scope.gridOptions.data[0].SUGGESTED_QTY,
                            TARGET_QTY: $scope.gridOptions.data[0].TARGET_QTY,
                            MAXIMUM_QTY: $scope.gridOptions.data[0].MAXIMUM_QTY,
                            MINIMUM_QTY: $scope.gridOptions.data[0].MINIMUM_QTY,
                            REMAINING_QTY: $scope.gridOptions.data[0].REMAINING_QTY,
                            DISTRIBUTOR_STOCK: $scope.gridOptions.data[0].DISTRIBUTOR_STOCK,
                            TRANSIT_STOCK: $scope.gridOptions.data[0].TRANSIT_STOCK,
                            SHIPPER_QTY: $scope.gridOptions.data[0].SHIPPER_QTY,

                            SKU_SHIPPER_DETAILS: $scope.gridOptions.data[0].SKU_SHIPPER_DETAILS,
                            ORDER_QTY_PREVIOUS: $scope.gridOptions.data[0].ORDER_QTY,
                            BONUS_QTY: $scope.gridOptions.data[0].BONUS_QTY,
                            SKU_TYPE: $scope.gridOptions.data[0].SKU_TYPE

                        }

                        $scope.gridOptions.data.push(newRow);
                        $scope.CUSTOMER_PRODUCT_DISC_AMOUNT_TOTAL = 0;

                        for (var i = 0; i < $scope.gridOptions.data.length; i++) {

                            $scope.gridOptions.data[i].SKU_ID = JSON.stringify($scope.gridOptions.data[i].SKU_ID);
                            if ($scope.gridOptions.data[i].UNIT_TP == 0) {
                                Unit_Zero_flag = true;
                                $scope.gridOptions.data[i].ORDER_MST_ID = 0;
                                $scope.gridOptions.data[i].ORDER_DTL_ID = 0;
                                
                            }
                            if (i != 0) {
                                $scope.CUSTOMER_PRODUCT_DISC_AMOUNT_TOTAL = $scope.CUSTOMER_PRODUCT_DISC_AMOUNT_TOTAL + $scope.gridOptions.data[i].CUSTOMER_PRODUCT_DISC_AMOUNT;

                            }

                          
                        }
                        $scope.TDS_AMOUNT_TOTAL = $scope.CUSTOMER_PRODUCT_DISC_AMOUNT_TOTAL * (.10);
                        $scope.Payable_Total = $scope.TDS_AMOUNT_TOTAL + $scope.model.NET_ORDER_AMOUNT;


                        if (Unit_Zero_flag == true) {
                            $scope.model.ORDER_MST_ID = 0;
                            $scope.model.ORDER_NO = '';
                            notificationservice.Notification('Product price can not be "0"!!', 1, '');
                        }
                        $scope.gridOptions.data[0] = $scope.GridDefalutData();
                        //$scope.model.UNIT_ID = $scope.model.ORDER_UNIT_ID;

                        for (var i = 0; i < $scope.Units.length; i++) {
                            if ($scope.model.UNIT_ID == $scope.Units[i].UNIT_ID) {
                                $scope.model.UNIT_NAME = $scope.Units[i].UNIT_NAME;
                            }
                        }


                        $scope.gridOptionsListComboGift.data = data.data[2];
                        $scope.gridOptionsListComboBonus.data = data.data[3];
                        $scope.gridOptionsListGift.data = data.data[4];

                        $scope.gridOptionsListBonus.data = data.data[5];
                        $scope.OnCustomerSelection();

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


    //$scope.GetEditDataById = function (value) {
    //    
    //    $scope.showLoader = true;
    //    if (value != undefined && value.length > 0) {
    //        $scope.GetAdjustmentList();
    //        InsertOrEditServices.GetEditDataById(value).then(function (data) {
    //            

    //            if (data.data != null && data.data.Order_Dtls != null && data.data.Order_Dtls.length > 0) {
    //                $scope.model = data.data;
    //                $scope.OnCustomerSelection();
    //                $scope.AdjustmentDataLoad(0);

    //                $scope.model.CUSTOMER_ID = JSON.stringify(parseInt($scope.model.CUSTOMER_ID));

    //                if (data.data.Order_Dtls != null) {
                        
    //                    $scope.gridOptions.data = data.data.Order_Dtls;

    //                    var newRow = {
    //                        ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID,
    //                        ORDER_DTL_ID: $scope.gridOptions.data[0].ORDER_DTL_ID,
    //                        ORDER_MST_ID: $scope.gridOptions.data[0].ORDER_MST_ID,
    //                        ORDER_AMOUNT: $scope.gridOptions.data[0].ORDER_AMOUNT,
    //                        ORDER_QTY: $scope.gridOptions.data[0].ORDER_QTY,
    //                        REMARKS: $scope.gridOptions.data[0].REMARKS,
    //                        REVISED_ORDER_QTY: $scope.gridOptions.data[0].REVISED_ORDER_QTY,
    //                        SKU_CODE: $scope.gridOptions.data[0].SKU_CODE,
    //                        SKU_ID:$scope.gridOptions.data[0].SKU_ID,
    //                        UNIT_TP: $scope.gridOptions.data[0].UNIT_TP,
    //                        UNIT_ID: $scope.gridOptions.data[0].UNIT_ID,
    //                        STATUS: $scope.gridOptions.data[0].STATUS,
    //                        SPA_UNIT_TP: $scope.gridOptions.data[0].SPA_UNIT_TP,
    //                        SPA_TOTAL_AMOUNT: $scope.gridOptions.data[0].SPA_TOTAL_AMOUNT,
    //                        SPA_REQ_TIME_STOCK: $scope.gridOptions.data[0].SPA_REQ_TIME_STOCK,
    //                        SPA_DISCOUNT_VAL_PCT: $scope.gridOptions.data[0].SPA_DISCOUNT_VAL_PCT,
    //                        SPA_DISCOUNT_TYPE: $scope.gridOptions.data[0].SPA_DISCOUNT_TYPE,
    //                        SPA_DISCOUNT_AMOUNT: $scope.gridOptions.data[0].SPA_DISCOUNT_AMOUNT,
    //                        SPA_CUST_COM: $scope.gridOptions.data[0].SPA_CUST_COM,
    //                        SPA_AMOUNT: $scope.gridOptions.data[0].SPA_AMOUNT


    //                    }

    //                    $scope.gridOptions.data.push(newRow);

    //                    for (var i = 0; i < $scope.gridOptions.data.length; i++) {
                            
    //                        $scope.gridOptions.data[i].SKU_ID = JSON.stringify($scope.gridOptions.data[i].SKU_ID);
    //                    }
    //                    $scope.gridOptions.data[0] = $scope.GridDefalutData();
    //                    $scope.model.UNIT_ID = $scope.model.ORDER_UNIT_ID;

    //                    for (var i = 0; i < $scope.Units.length; i++) {
    //                        if ($scope.model.UNIT_ID == $scope.Units[i].UNIT_ID) {
    //                            $scope.model.UNIT_NAME = $scope.Units[i].UNIT_NAME;
    //                        }
    //                    }

    //                }
                   


    //            }
    //            $scope.rowNumberGenerate();
    //            $interval(function () {
    //                $scope.LoadSKU_ID();
    //            }, 800, 2);
    //            $interval(function () {
    //                $scope.LoadCUSTOMER_ID();
    //            }, 800, 2);
    //            $scope.showLoader = false;
    //        }, function (error) {
    //            alert(error);
                
    //            $scope.showLoader = false;
    //        });
    //    }
    //}
    $scope.LoadSKU_ID = function () {
        $('.sku_Id').trigger('change');

    }
    $scope.LoadCUSTOMER_ID = function () {
        $('#CUSTOMER_ID').trigger('change');

    }
    $scope.LoadSKU_IDMASTER = function () {
        $('#SKU_ID_MASTER_RRRR').trigger('change');
          //$("#SKU_ID_MASTER_RRRR").val("");
                    //$("#SKU_ID_MASTER_RRRR").trigger("change");
                    //$("#SKU_ID_Specific").val("");
                    //$("#SKU_ID_Specific").trigger("change");
    }
    $scope.LoadSKU_ID_Specific = function () {
        $('#SKU_ID_Specific').trigger('change');

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
    $scope.SavePostDataFinal = function () {

        $scope.showLoader = true;
        
        var st = false;

        
        for (var i in $scope.gridOptions.data) {
            if (i > 0 && $scope.gridOptions.data[i].MAXIMUM_QTY < $scope.gridOptions.data[i].ORDER_QTY) {
                st = true;
            }

        }

        if (st == true) {
            $scope.model.NOTIFY_TEXT = "Max QTY Exceded";

        } else if ($scope.model.CREDIT_LIMIT < $scope.model.NET_ORDER_AMOUNT)
        { 
            $scope.model.NOTIFY_TEXT = "Max Credit Exceded";
        } 
            InsertOrEditServices.Save_Post_Confirm_Order($scope.model).then(function (data) {

                
                if (data.data.status > 0) {
                    //$scope.LoadFormData(data.data.key);
                    notificationservice.Notification(1, 1, 'Order Posted Successfully!');


                } else {
                    notificationservice.Notification(data.data.status, 1, 'Order Generated Successfully! Please post order from "Search" list to submit for invoice');

                }
              
                $scope.showLoader = false;


            });
        
        $scope.showLoader = false;

    }
    $scope.CancelPostDataFinal = function () {

        $scope.showLoader = true;
        
        var st = false;


        for (var i in $scope.gridOptions.data) {
            if (i > 0 && $scope.gridOptions.data[i].MAXIMUM_QTY < $scope.gridOptions.data[i].ORDER_QTY) {
                st = true;
            }

        }

        if (st == true) {
            $scope.model.NOTIFY_TEXT = "Max QTY Exceded";

        } else if ($scope.model.CREDIT_LIMIT < $scope.model.NET_ORDER_AMOUNT) {
            $scope.model.NOTIFY_TEXT = "Max Credit Exceded";
        } else {
            $scope.model.NOTIFY_TEXT = "";

        }
        InsertOrEditServices.Save_Post_Confirm_Order($scope.model).then(function (data) {

            
            if (data.data.Status > 0) {
                $scope.LoadFormData(data.data.Key);
                notificationservice.Notification(1, 1, 'Order Posted Successfully!');


            } else {
                notificationservice.Notification(data.data.Status, 1, 'Order Generated Successfully! Please post order from "Search" list to submit for invoice');

            }

            $scope.showLoader = false;


        });

        $scope.showLoader = false;

    }


    $scope.SaveDataFinal = function (model) {

        $scope.showLoader = true;
        
        var st = false;

        for (var i in $scope.gridOptions.data) {
            if (i > 0 && $scope.gridOptions.data[i].ORDER_QTY_PREVIOUS != undefined && $scope.gridOptions.data[i].ORDER_QTY_PREVIOUS != $scope.gridOptions.data[i].ORDER_QTY) {
                st = true;
            }

        }

        if (st == true) {
            notificationservice.Notification('You have updated the Order Qty and did not process! Please Process the data, check and then save', 1, 'You have updated the Order Qty and did not process! Please Process the data, check and then save');

        } else
        {

            for (var i in $scope.gridOptions.data) {
                if (i > 0 && $scope.gridOptions.data[i].MAXIMUM_QTY < $scope.gridOptions.data[i].ORDER_QTY) {
                    st = true;
                }

            }

            if (st == true) {
                $scope.model.NOTIFY_TEXT = "Max QTY Exceded";

            } else if ($scope.model.CREDIT_LIMIT < $scope.model.NET_ORDER_AMOUNT) {
                $scope.model.NOTIFY_TEXT = "Max Credit Exceded";
            } else {
                $scope.model.NOTIFY_TEXT = "";

            }
            InsertOrEditServices.Save_Final_Order($scope.model).then(function (data) {

                
                if (data.data.Status > 0) {
                    $scope.LoadFormData(data.data.Key);
                    notificationservice.Notification(1, 1, 'Order Generated Successfully! Please post order from "Search" list to submit for invoice');


                } else {
                    notificationservice.Notification(data.data.Status, 1, 'Order Generated Successfully! Please post order from "Search" list to submit for invoice');

                }
                
                //$scope.LoadFormData($scope.model.ORDER_MST_ID_ENCRYPTED);
                ////$scope.SendNotification();
                //notificationservice.Notification(1, 1, 'Order Generated Successfully! Please post order from "Search" list to submit for invoice');

                $scope.showLoader = false;


            });
        }
        $scope.showLoader = false;

    }

    $scope.SaveData = function (model) {
        
        $scope.showLoader = true;
        $scope.model.BONUS_CLAIM_NO = $scope.model.BONUS_CLAIM_NO == null ? "" : $scope.model.BONUS_CLAIM_NO;
        $scope.model.BONUS_PROCESS_NO = $scope.model.BONUS_PROCESS_NO == null ? "" : $scope.model.BONUS_PROCESS_NO;
        $scope.model.REMARKS = $scope.model.REMARKS == null ? "" : $scope.model.REMARKS;
        $scope.model.REPLACE_CLAIM_NO = $scope.model.REPLACE_CLAIM_NO == null ? "" : $scope.model.REPLACE_CLAIM_NO
        $scope.model.UNIT_ID = parseInt($scope.model.UNIT_ID);

        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);
        $scope.model.CUSTOMER_ID = parseInt($scope.model.CUSTOMER_ID);
        $scope.model.INVOICE_STATUS = 'Incomplete';
        $scope.model.ORDER_ENTRY_TYPE = 'Manual';
        $scope.model.SKU_ID =0;

        const searchIndex = $scope.Customers.findIndex((x) => x.CUSTOMER_ID == $scope.model.CUSTOMER_ID);

        $scope.model.CUSTOMER_CODE = $scope.Customers[searchIndex].CUSTOMER_CODE;

        $scope.gridOptions.data = $scope.gridOptions.data.filter((x) => x.ROW_NO !== 0 && x.SKU_TYPE != 'BONUS');
        for (var i in $scope.gridOptions.data) {

                $scope.gridOptions.data[i].SKU_ID = parseInt($scope.gridOptions.data[i].SKU_ID);
                $scope.gridOptions.data[i].ORDER_QTY = parseFloat($scope.gridOptions.data[i].ORDER_QTY);
                $scope.gridOptions.data[i].ORDER_AMOUNT = parseFloat($scope.gridOptions.data[i].ORDER_AMOUNT);
                $scope.gridOptions.data[i].ORDER_UNIT_ID = $scope.model.UNIT_ID;
            $scope.gridOptions.data[i].STATUS = 'Manual';
            $scope.gridOptions.data[i].ORDER_DTL_ID = $scope.gridOptions.data[i].ORDER_DTL_ID == undefined ? 0 : $scope.gridOptions.data[i].ORDER_DTL_ID;
            $scope.gridOptions.data[i].ORDER_MST_ID = $scope.gridOptions.data[i].ORDER_MST_ID == undefined ? 0 : $scope.gridOptions.data[i].ORDER_MST_ID ;
            $scope.gridOptions.data[i].UNIT_ID = $scope.gridOptions.data[i].ORDER_UNIT_ID;
            $scope.gridOptions.data[i].REMARKS = $scope.gridOptions.data[i].REMARKS == null ? "" : $scope.gridOptions.data[i].REMARKS;
            $scope.gridOptions.data[i].REVISED_ORDER_QTY = $scope.gridOptions.data[i].REVISED_ORDER_QTY == null ? 0 : $scope.gridOptions.data[i].REVISED_ORDER_QTY;
            $scope.gridOptions.data[i].SPA_DISCOUNT_TYPE = $scope.gridOptions.data[i].SPA_DISCOUNT_TYPE == null ? "" : $scope.gridOptions.data[i].SPA_DISCOUNT_TYPE;
            $scope.gridOptions.data[i].LOADING_CHARGE_AMOUNT = $scope.gridOptions.data[i].LOADING_CHARGE_AMOUNT == null || $scope.gridOptions.data[i].LOADING_CHARGE_AMOUNT == undefined || $scope.gridOptions.data[i].LOADING_CHARGE_AMOUNT == NaN ? 0 : $scope.gridOptions.data[i].LOADING_CHARGE_AMOUNT;

            $scope.gridOptions.data[i].INVOICE_ADJUSTMENT_AMOUNT = $scope.gridOptions.data[i].INVOICE_ADJUSTMENT_AMOUNT == null ? 0 : $scope.gridOptions.data[i].INVOICE_ADJUSTMENT_AMOUNT;
            $scope.gridOptions.data[i].ORDER_QTY_PREVIOUS = $scope.gridOptions.data[i].ORDER_QTY;
            if (isNaN($scope.gridOptions.data[i].STOCK_AFTER_INVOICE)) {
                $scope.gridOptions.data[i].STOCK_AFTER_INVOICE = 0;
           }
            
            
        }


        $scope.model.Order_Dtls = $scope.gridOptions.data.filter(x => x.ORDER_QTY > 0);
        if (isNaN($scope.model.ORDER_AMOUNT)) {
            $scope.model.ORDER_AMOUNT = 0;
            $scope.model.NET_ORDER_AMOUNT = 0;

        }

        InsertOrEditServices.AddOrUpdate($scope.model).then(function (data) {


            if (data.data.key > 0) {
                
                $scope.model.ORDER_MST_ID = parseInt(data.data.key);
                $scope.model.ORDER_MST_ID_ENCRYPTED = data.data.status;

                //  $scope.SendNotification();
                $scope.Process_Order($scope.model);

                $scope.showLoader = false;
                //    $scope.LoadFormData(data.data.status);
            }
            else {
                notificationservice.Notification(data.data.status, 1, '');


                $scope.showLoader = false;
                $scope.addNewRow();

            }
        });
       
    }
    $scope.showportion = function () {
            $('hidea').css("display", "block");
            $('showa').css("display", "none");
            $('portion').css("portion", "block");

        
    }
    $scope.hideportion = function () {
        $('hidea').css("display", "none");
        $('showa').css("display", "block");
        $('portion').css("portion", "none");
    }
    $scope.Process_Order = function (model) {

        InsertOrEditServices.Process_Order(model).then(function (data) {

            notificationservice.Notification(data.data.status, 1, 'Data Processed Successfully !!');

            if (data.data.status == 1) {
                

                //$scope.SendNotification();

                $scope.LoadFormData($scope.model.ORDER_MST_ID_ENCRYPTED);
                $scope.showLoader = false;

            }
            else {
                $scope.showLoader = false;
                $scope.addNewRow();

            }
        });
    }

    $scope.Delete_Order_full_Query = function (model) {

        InsertOrEditServices.Delete_Order_full_Query(model).then(function (data) {

            notificationservice.Notification(data.data.status, 1, 'Data Deleted Successfully !!');

            if (data.data.status == 1) {
                
                $scope.model.ORDER_MST_ID = 0;

                //$scope.SendNotification();
                //$scope.addNewRow();
                $scope.showLoader = false;

            }
            else {
                $scope.showLoader = false;
                //$scope.addNewRow();

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

        $scope.model.MONTH_NUMBER = entity.MONTH_NUMBER;
        $scope.model.YEAR_NUMBER = entity.YEAR_NUMBER;
        $scope.onAdjustmentSelection();
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

        if (model.ADJUSTMENT_ID == 4) {
            if (!model.MONTH_NUMBER || !model.YEAR_NUMBER) {
                alert('For Bonus Adjustment Bonus And Month Must Be Required');
                $scope.showLoader = false;
                return;
            }
        }

        if (model.ADJUSTMENT_AMOUNT <= 0) {
            alert('The adjustment cannot be zero or a negative value.')
            $scope.showLoader = false;
            return;
        }
        
        AdjustmentServices.AddOrUpdate(model).then(function (data) {
            

            notificationservice.Notification(data.data.Status, 1, 'Data Save Successfully !!');
            if (data.data.Status == 1) {
                $scope.AdjustmentDataLoad(0);
                $scope.showLoader = false;
                $scope.model.ID = parseInt(data.data.Key);
                $scope.LoadFormData(data.data.Parent);

            }
            else {
                $scope.showLoader = false;
            }
        });
    }
    $scope.numberToTwoDecimalPlaces = function (input) {
        if (isNaN(input)) {
            return input;
        }
        return parseFloat(input).toFixed(2);
    };

    $scope.onAdjustmentSelection = function () {

        if ($scope.model.ADJUSTMENT_ID == 4 || $scope.model.ADJUSTMENT_ID == '4') {
            $scope.isRemBonusAdj = true;

        } else {
            $scope.isRemBonusAdj = false;
            $scope.model.YEAR_NUMBER = '';
            $scope.model.MONTH_NUMBER = '';
        }
    }

    var currentAdjYear = new Date().getFullYear();
    $scope.years = [currentAdjYear - 1, currentAdjYear];

}]);

