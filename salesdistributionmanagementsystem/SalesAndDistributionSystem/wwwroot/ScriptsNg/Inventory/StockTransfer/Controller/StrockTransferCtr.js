ngApp.controller('ngGridCtrl', ['$scope', 'StockTransferService', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, stockTransferService, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

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
            COMPANY_ID: 0,
            MST_ID: 0,
            MST_ID_ENCRYPTED: "",
            REF_NO: "",
            REF_DATE:"",
            TRANS_RCV_UNIT_ID: "",

            TRANSFER_UNIT_ID: 0,
            TRANSFER_TYPE: "",
            TRANSFER_NO: "",
            TRANSFER_DATE: $scope.formatDate( new Date()),
            TRANSFER_AMOUNT: 0
  
            , TRANSFER_BY: ""
            , STATUS: "Active"
            , REMAEKS: ""
            , stockTransferDtlList: []
    }

    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 0,
            DTL_ID: 0,
            MST_ID: 0,
            COMPANY_ID: 0,
            SKU_ID: "",
            PACK_SIZE: "",
            SKU_CODE: '',
            UNIT_TP: 0,
            TRANSFER_QTY: 0,
            STOCK_QTY: 0,
            TRANSFER_AMOUNT: 0
            , TOTAL_VOLUME: 0
            , TOTAL_WEIGHT: 0

        }
    }
    $scope.ClearEntity = function (entity) {

        entity.ROW_NO = 0,
            entity.DTL_ID = 0,
            entity.MST_ID = 0,
            entity.COMPANY_ID = 0,
            entity.SKU_ID = "null",
            entity.PACK_SIZE = "",
            entity.SKU_CODE = '',
            entity.UNIT_TP = 0,
            TRANSFER_QTY = 0,
            TRANSFER_AMOUNT = 0,
            STATUS = 'Active',
            COMPANY_ID = 0,
            REMARKS = ''

    };
    $scope.getPermissions = [];
    $scope.ProductList = [];
    $scope.Companies = [];
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

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Stock Transfer Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.data = [];

    $scope.gridOptionsList = {
        data: [$scope.GridDefalutData()]
    }

    $scope.rowNumberGenerate = function () {
        
        $scope.model.TRANSFER_AMOUNT = 0;
        for (let i = 0; i < $scope.gridOptionsList.data.length; i++) {

            $scope.gridOptionsList.data[i].ROW_NO = i;
            $scope.model.TRANSFER_AMOUNT += ($scope.gridOptionsList.data[i].TRANSFER_AMOUNT)
          
        }
    }

    $scope.addDefaultRow = (entity) => {
        var newRow = {
            ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, MST_ID: $scope.gridOptionsList.data[0].MST_ID, DTL_ID: $scope.gridOptionsList.data[0].DTL_ID, SKU_ID: $scope.gridOptionsList.data[0].SKU_ID, SKU_CODE: $scope.gridOptionsList.data[0].SKU_CODE, STOCK_QTY: $scope.gridOptionsList.data[0].STOCK_QTY, TRANSFER_AMOUNT: $scope.gridOptionsList.data[0].TRANSFER_AMOUNT, TRANSFER_QTY: $scope.gridOptionsList.data[0].TRANSFER_QTY, UNIT_TP: $scope.gridOptionsList.data[0].UNIT_TP, PACK_SIZE: $scope.gridOptionsList.data[0].PACK_SIZE
        }
        $scope.gridOptionsList.data.push(newRow);
        $scope.gridOptionsList.data[0] = $scope.GridDefalutData();
        $scope.rowNumberGenerate();
    }
    $scope.addNewRow = (entity) => {
        
        var count = 0;
        if ($scope.gridOptionsList.data.length > 0 && $scope.gridOptionsList.data[0].SKU_CODE != null && $scope.gridOptionsList.data[0].SKU_CODE != '' && $scope.gridOptionsList.data[0].SKU_CODE != 'undefined') {
            for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
                if ($scope.gridOptionsList.data[i].SKU_CODE == entity.SKU_CODE) {
                    count++;
                }
            }
            //var result = $scope.CheckDateValidation($scope.gridOptions.data[0]);
            if (count == 1 || count == 0 || entity.SKU_CODE == "") {
                if (entity.TRANSFER_QTY <= 0) {
                    notificationservice.Notification("Transfer quantity must be greater then zero!", "", 'Transfer quantity must be greater then zero!!');
                    $scope.ClearEntity(entity)
                    return;
                }
                if (entity.TRANSFER_QTY > entity.STOCK_QTY) {
                    notificationservice.Notification("Transfer Qty is greater then stock Qty", "", 'Transfer Qty is greater then stock Qty!');
                    $scope.showLoader = false;
                    return;
                }

                var newRow = {
                    ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, MST_ID: $scope.gridOptionsList.data[0].MST_ID, DTL_ID: $scope.gridOptionsList.data[0].DTL_ID, SKU_ID: $scope.gridOptionsList.data[0].SKU_ID, SKU_CODE: $scope.gridOptionsList.data[0].SKU_CODE, STATUS: $scope.gridOptionsList.data[0].STATUS, TRANSFER_AMOUNT: $scope.gridOptionsList.data[0].TRANSFER_AMOUNT, TRANSFER_QTY: $scope.gridOptionsList.data[0].TRANSFER_QTY, STOCK_QTY: $scope.gridOptionsList.data[0].STOCK_QTY, PACK_SIZE: $scope.gridOptionsList.data[0].PACK_SIZE, UNIT_TP: $scope.gridOptionsList.data[0].UNIT_TP, STATUS: $scope.gridOptionsList.data[0].STATUS
                    , NO_OF_SHIPPER: $scope.gridOptionsList.data[0].NO_OF_SHIPPER, LOOSE_QTY: $scope.gridOptionsList.data[0].LOOSE_QTY, SHIPPER_WEIGHT: $scope.gridOptionsList.data[0].SHIPPER_WEIGHT, SHIPPER_VOLUME: $scope.gridOptionsList.data[0].SHIPPER_VOLUME, PER_PACK_WEIGHT: $scope.gridOptionsList.data[0].PER_PACK_WEIGHT, PER_PACK_VOLUME: $scope.gridOptionsList.data[0].PER_PACK_VOLUME, SHIPPER_QTY: $scope.gridOptionsList.data[0].SHIPPER_QTY, TOTAL_SHIPPER_WEIGHT: $scope.gridOptionsList.data[0].TOTAL_SHIPPER_WEIGHT
                    , TOTAL_SHIPPER_VOLUME: $scope.gridOptionsList.data[0].TOTAL_SHIPPER_VOLUME, LOOSE_WEIGHT: $scope.gridOptionsList.data[0].LOOSE_WEIGHT,
                    LOOSE_VOLUME: $scope.gridOptionsList.data[0].LOOSE_VOLUME, TOTAL_WEIGHT: $scope.gridOptionsList.data[0].TOTAL_WEIGHT,
                    TOTAL_VOLUME: $scope.gridOptionsList.data[0].TOTAL_VOLUME,
                    RU_STOCK_QTY: $scope.gridOptionsList.data[0].RU_STOCK_QTY,
                    BOX_QTY: $scope.gridOptionsList.data[0].BOX_QTY,

                }
                $scope.gridOptionsList.data.push(newRow);
                $scope.gridOptionsList.data[0] = $scope.GridDefalutData();

                $scope.rowNumberGenerate();

            }
            else {
                notificationservice.Notification("Product already exist!", "", 'Product already exist!');
                $scope.ClearEntity(entity)
            }
            $scope.LoadSKUId();
        }
        else {
            notificationservice.Notification("No item has added!", "", 'No item has added!');

        }
    };

    $scope.removeItem = function (entity) {
        
        if ($scope.gridOptionsList.data.length > 1) {
            var index = $scope.gridOptionsList.data.indexOf(entity);
            if ($scope.gridOptionsList.data.length > 0) {
                $scope.gridOptionsList.data.splice(index, 1);
            }
            $scope.rowNumberGenerate();


        } else {
            notificationservice.Notification("Only Single Row Left!!", "", 'Only Single Row Left!!');
        }


    }

    $scope.EditItem = (entity) => {
        
        if ($scope.gridOptionsList.data.length > 0) {

            var newRow = {
                ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, MST_ID: entity.MST_ID, DTL_ID: entity.DTL_ID, SKU_ID: entity.SKU_ID, SKU_CODE: entity.SKU_CODE, PACK_SIZE: entity.PACK_SIZE, STOCK_QTY: entity.STOCK_QTY, STATUS: entity.STATUS, TRANSFER_AMOUNT: entity.TRANSFER_AMOUNT, TRANSFER_QTY: entity.RECEIVE_QTY, UNIT_TP: entity.UNIT_TP, STATUS: entity.STATUS

            }
            $scope.gridOptionsList.data[0] = newRow;


        } else {
            notificationservice.Notification("Please Select Valid data", "", 'Only Single Row Left!!');

        }
        $scope.LoadSKUId();
        $scope.rowNumberGenerate();
        $scope.removeItem(entity);
    };

    $scope.LoadFormData = function () {
        
        $scope.showLoader = true;
        stockTransferService.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.relationdata = data.data;
            for (var i = 0; i < $scope.relationdata.length; i++) {
                ;
                if ($scope.relationdata[i].MST_ID == parseInt($scope.model.MST_ID)) {
                    $scope.model.MST_ID_ENCRYPTED = $scope.relationdata[i].MST_ID_ENCRYPTED
                }
            }
            $scope.EditData($scope.model.MST_ID_ENCRYPTED)


            $scope.showLoader = false;

        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.EditData = function (MST_ID_ENCRYPTED) {
        
        window.location = "/Inventory/StockTransfer/StockTransfer?Id=" + MST_ID_ENCRYPTED;
    }
    $scope.GetEditDataById = function (value) {
        
        if (value != undefined && value.length > 0) {
            stockTransferService.GetEditDataById(value).then(function (data) {
                

                if (data.data != null && data.data.stockTransferDtlList != null && data.data.stockTransferDtlList.length > 0) {
                    $scope.model.COMPANY_ID = data.data.COMPANY_ID;
                    $scope.model.MST_ID = data.data.MST_ID;

                    $scope.model.TRANSFER_UNIT_ID = data.data.TRANSFER_UNIT_ID;

                    $scope.model.TRANS_RCV_UNIT_ID = data.data.TRANS_RCV_UNIT_ID;
                    $scope.model.TRANSFER_BY = data.data.TRANSFER_BY;
                    $scope.model.TRANSFER_AMOUNT = data.data.TRANSFER_AMOUNT;
                    $scope.model.TRANSFER_DATE = data.data.TRANSFER_DATE;
            
       
                    $scope.model.REMARKS = data.data.REMARKS;
             
                    $scope.model.STATUS = data.data.STATUS;
                    $scope.model.TRANSFER_NO = data.data.TRANSFER_NO;

                    $scope.model.REF_NO = data.data.REF_NO;
                    $scope.model.REF_DATE = data.data.REF_DATE;
                    $scope.model.TRANSFER_TYPE = data.data.TRANSFER_TYPE;

                    $scope.gridOptionsList.data = data.data.stockTransferDtlList;
                    $scope.addDefaultRow($scope.GridDefalutData());
                }
                $scope.model.TRANSFER_UNIT_ID = data.data.TRANSFER_UNIT_ID;
                $scope.LoadSKUId();
        
                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                
            });
        }
    }
    $scope.typeaheadSelectedSku = function (entity) {
 
        
        const searchIndex = $scope.Products.findIndex((x) => x.SKU_ID == entity.SKU_ID);
        entity.SKU_CODE = $scope.Products[searchIndex].SKU_CODE;
        //entity.SKU_ID = $scope.Products[searchIndex].SKU_ID;
        $scope.Products[searchIndex].UNIT_TP = parseFloat($scope.Products[searchIndex].UNIT_TP);
        if ($scope.Products[searchIndex].UNIT_TP < 0 || Object.is($scope.Products[searchIndex].UNIT_TP, NaN)) {
            entity.UNIT_TP = 0;
        }
        else {
            entity.UNIT_TP = $scope.Products[searchIndex].UNIT_TP;
        }
        entity.PACK_SIZE = $scope.Products[searchIndex].PACK_SIZE;
        entity.STOCK_QTY = $scope.Products[searchIndex].STOCK_QTY;
        entity.SHIPPER_QTY = $scope.Products[searchIndex].SHIPPER_QTY;

        stockTransferService.LoadRcvUnitStock($scope.model.TRANS_RCV_UNIT_ID, entity.SKU_ID).then(function (data) {
            entity.RU_STOCK_QTY = data.data[0].STOCK_QTY;
        });

        $scope.LoadSKUId();


    };
 
    $scope.typeaheadSelectedSkuCode = function (entity) {
        
        const searchIndex = $scope.Products.findIndex((x) => x.SKU_CODE == entity.SKU_CODE);
        entity.SKU_ID = $scope.Products[searchIndex].SKU_ID;
        //entity.SKU_CODE = $scope.Products[searchIndex].SKU_CODE;
        entity.UNIT_TP = $scope.Products[searchIndex].UNIT_TP;
        $scope.LoadSKUId();

    };
    $scope.typeaheadSelectedQty = function (entity) {
        
        //const searchIndex = $scope.Products.findIndex((x) => x.SKU_ID == entity.SKU_ID);
        //entity.SKU_CODE = $scope.Products[searchIndex].SKU_CODE;
        //entity.UNIT_TP = $scope.Products[searchIndex].UNIT_TP;
        if (entity.RETURN_QTY == '' || entity.RETURN_QTY == null || entity.RETURN_QTY < 0) {
            entity.RETURN_QTY = 0;
        }
        $scope.GetWeightVolumeCal(entity);
        entity.TRANSFER_AMOUNT = parseFloat(entity.UNIT_TP) * parseInt(entity.TRANSFER_QTY)
        $scope.rowNumberGenerate();

    };
    $scope.LoadSKUId = function () {
        setTimeout(function () {
            $('#SKU_CODE').trigger('change');
            $('#SKU_ID').trigger('change');
            $('#SKU_IDRETURN_RCV_UNIT_ID').trigger('change');
            $('#TRANSFER_UNIT_ID').trigger('change');

            $('#TRANS_RCV_UNIT_ID').trigger('change');
            $('#RETURN_TYPE').trigger('change');

        }, 1000)

    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
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
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '35%', cellTemplate:
                '<select class="select2-single form-control" ng-disabled="grid.appScope.model.TRANS_RCV_UNIT_ID==0"   data-select2-id="{{row.entity.SKU_CODE}}" id="SKU_ID"' +
                'name="SKU_ID" ng-model="row.entity.SKU_ID" style="width:100%" ng-change="grid.appScope.typeaheadSelectedSku(row.entity)">' +
                '<option ng-repeat="item in grid.appScope.Products" ng-selected="item.SKU_ID == row.entity.SKU_ID" value="{{item.SKU_ID}}">{{ item.SKU_NAME }} | Code: {{ item.SKU_CODE }}</option>' +
                '</select>'
        }
        ,
        {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '8%', cellTemplate:
                '<input type="text" disabled  style="text-align: right;"  ng-model="row.entity.SKU_CODE"  class="pl-sm" />'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: true, width: '5%', cellTemplate:
                '<input type="number" style="text-align:right;" disabled  ng-model="row.entity.UNIT_TP"  class="pl-sm" />'
        }
        , {
            name: 'PACK_SIZE', field: 'PACK_SIZE', displayName: 'Pack Size', enableFiltering: true, width: '5%', cellTemplate:
                '<input type="text" style="text-align:right;" disabled  ng-model="row.entity.PACK_SIZE"  class="pl-sm" />'
        }
        , {
            name: 'STOCK_QTY', field: 'STOCK_QTY', displayName: 'Stock Qty', enableFiltering: true, width: '6%', cellTemplate:
                '<input type="number" style="text-align:right;" disabled  ng-model="row.entity.STOCK_QTY"  class="pl-sm" />'
        },
        , {
            name: 'RU_STOCK_QTY', field: 'RU_STOCK_QTY', displayName: 'Rcv Unit Stock', enableFiltering: true, width: '7%', cellTemplate:
                '<input type="number" style="text-align:right;" disabled  ng-model="row.entity.RU_STOCK_QTY"  class="pl-sm" />'
        }
        , {
            name: 'BOX_QTY', field: 'BOX_QTY', displayName: 'BOX', enableFiltering: false, width: '60', cellTemplate:
                '<input style="background:white;" type="number" min="0"  ng-model="row.entity.BOX_QTY"  ng-change="grid.appScope.OnProductBoxChange(row.entity)"  class="pl-sm text-right" />'

        }
        , {
            name: 'TRANSFER_QTY', field: 'TRANSFER_QTY', displayName: 'TRF Qty', enableFiltering: true, width: '7%', cellTemplate:
                '<input type="number"  style="background:white; text-align:right;"  ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.TRANSFER_QTY"  class="pl-sm" />'
        },
       
        {
            name: 'TRANSFER_AMOUNT', field: 'RECEIVE_AMOUNT', displayName: 'Receive Amount', enableFiltering: true, width: '8%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-model="row.entity.TRANSFER_AMOUNT"  class="pl-sm" />'
        }
       

        , {
            name: 'Action', displayName: 'Action', width: '7%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.EditItem(row.entity)" type="button" class="btn btn-outline-success mb-1"><img src="/img/edit-icon.png" height="15px" ></button>' +

                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO == 0" ng-click="grid.appScope.addNewRow(row.entity)" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                '</div>'
        }
        //, {
        //    name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
        //        '<div style="margin:1px;">' +
        //        '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Delete</button>' +
        //        '</div>'
        //},

    ];

    $scope.gridOptionsList.rowTemplate = "<div  ng-style='row.entity.TRANSFER_QTY >row.entity.STOCK_QTY && grid.appScope.myObj' ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.isTrue = true;
    $scope.myObj = {
        "color": "red",

    }



    $scope.OnProductBoxChange = (entity) => {
        if (entity.BOX_QTY < 1) {
            //alert('you cannot select fractional box quantity');
            entity.TRANSFER_QTY = 0;
            return;
        }
        entity.TRANSFER_QTY = entity.BOX_QTY * entity.SHIPPER_QTY;
        $scope.typeaheadSelectedQty(entity);
    }



    $scope.ClearForm = function () {
        window.location.href = "/Inventory/StockTransfer/StockTransfer";
    }

    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        stockTransferService.GetCompany().then(function (data) {
            
            $scope.model.COMPANY_ID = parseFloat(data.data);
            /*            $scope.DataLoad($scope.model.COMPANY_ID);*/
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

        stockTransferService.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }


    $scope.LoadProductData = function () {
        $scope.showLoader = true;

        stockTransferService.LoadProductData().then(function (data) {
            
            $scope.Products = data.data;
            $scope.showLoader = false;
            var _Products = {
                PRODUCT_ID: "0",
                PRODUCT_NAME: "All",
                PRODUCT_CODE: "ALL",
                PACK_SIZE:""

            }

            $scope.Products.push(_Products);
        }, function (error) {
            alert(error);
            

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
            Controller_Name: 'StockTransfer',
            Action_Name: 'StockTransfer'
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
    $scope.LoadUnitData = function () {
        $scope.showLoader = true;

        stockTransferService.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            
            
            $scope._Unit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });
            var _units = {
                UNIT_ID: "0",
                UNIT_NAME: "Select Unit Name",


            }
            $scope.Unit.push(_units);
            for (var i in $scope._Unit) {
                $scope.Unit.push($scope._Unit[i]);
            }

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }

    $scope.GetWeightVolumeCal = (entity) => {

        var count = 0;
        if (entity != null && entity != 'undefined') {

            stockTransferService.LoadProductWeightData($scope.model.COMPANY_ID, entity.SKU_ID, entity.TRANSFER_QTY).then(function (data) {

                
                entity.NO_OF_SHIPPER = data.data[0].NO_OF_SHIPPER;
                entity.LOOSE_QTY = data.data[0].LOOSE_QTY;
                entity.SHIPPER_WEIGHT = data.data[0].SHIPPER_WEIGHT;
                entity.SHIPPER_VOLUME = data.data[0].SHIPPER_VOLUME;
                entity.PER_PACK_WEIGHT = data.data[0].PER_PACK_WEIGHT;
                entity.PER_PACK_VOLUME = data.data[0].PER_PACK_VOLUME;
                entity.SHIPPER_QTY = data.data[0].SHIPPER_QTY;

                entity.TOTAL_SHIPPER_WEIGHT = parseFloat(((entity.NO_OF_SHIPPER * entity.SHIPPER_WEIGHT).toFixed(2)));
                entity.TOTAL_SHIPPER_VOLUME = parseFloat(((entity.SHIPPER_VOLUME * entity.NO_OF_SHIPPER).toFixed(2)));


                entity.LOOSE_WEIGHT = parseFloat((entity.PER_PACK_WEIGHT * entity.LOOSE_QTY).toFixed(2));
                entity.LOOSE_VOLUME = parseFloat((entity.PER_PACK_VOLUME * entity.LOOSE_QTY).toFixed(2));



                entity.TOTAL_WEIGHT = parseFloat(entity.TOTAL_SHIPPER_WEIGHT) + parseFloat(entity.LOOSE_WEIGHT);
                entity.TOTAL_VOLUME = parseFloat(entity.TOTAL_SHIPPER_VOLUME) + parseFloat(entity.LOOSE_VOLUME);
                //$scope.model.TOTAL_WEIGHT += entity.TOTAL_WEIGHT;
                //$scope.model.TOTAL_VOLUME += entity.TOTAL_VOLUME;
                
                $scope.model.TOTAL_WEIGHT = 0;
                $scope.model.TOTAL_VOLUME = 0;

                for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
                    $scope.model.TOTAL_VOLUME += $scope.gridOptionsList.data[i].TOTAL_VOLUME;
                    $scope.model.TOTAL_WEIGHT += $scope.gridOptionsList.data[i].TOTAL_WEIGHT;
                }
               
                $scope.rowNumberGenerate();

            }, function (error) {

            });

        }
        else {
            notificationservice.Notification("No item has added!", "", 'No item has added!');

        }
    };
    $scope.LoadRequisitionData = function () {
        $scope.showLoader = true;


        stockTransferService.LoadRequisitionData($scope.model.COMPANY_ID).then(function (data) {
            
            
            $scope.RequisitionList = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }



    $scope.SaveData = function (model) {
        debugger
        if ($scope.model.TRANS_RCV_UNIT_ID == "") {
            notificationservice.Notification('Receive unit not selected!', "", 'Receive unit not selected!');
            return;
        }
        if ($scope.model.TRANS_RCV_UNIT_ID == 0) {
            notificationservice.Notification('Receive unit not selected!', "", 'Receive unit not selected!');
            return;
        }
        if ($scope.model.TRANSFER_UNIT_ID == "") {
            notificationservice.Notification('Transfer unit not selected!', "", 'Transfer unit not selected!');
            return;
        }
        if ($scope.model.TRANSFER_UNIT_ID == 0) {
            notificationservice.Notification('Transfer unit not selected!', "", 'Transfer unit not selected!');
            return;
        }
        $scope.showLoader = true;
        if ($scope.model.TRANSFER_AMOUNT == NaN) {
            $scope.model.TRANSFER_AMOUNT = 0
        }

        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);



        $scope.model.stockTransferDtlList = $scope.gridOptionsList.data;
        for (var i = 0; i < $scope.model.stockTransferDtlList.length; i++) {
            if ($scope.model.stockTransferDtlList[i].TRANSFER_QTY > $scope.model.stockTransferDtlList[i].STOCK_QTY) {
                notificationservice.Notification("Transfer Qty is greater then stock Qty", "", 'Transfer Qty is greater then stock Qty!');
                $scope.showLoader = false;
                return;
            }
        }
        if ($scope.model.stockTransferDtlList != null) {
            if ($scope.model.stockTransferDtlList.length > 1 && $scope.model.stockTransferDtlList[0].SKU_CODE == "") {
                $scope.model.stockTransferDtlList.splice(0, 1);
            }
        }
        if ($scope.model.stockTransferDtlList.length == 1 && $scope.model.stockTransferDtlList[0].SKU_CODE == "") {

            $scope.showLoader = false;
            notificationservice.Notification("No data has added on requisition detail!", "", 'No data has added on requisition detail!');
            return;

        }

        if ($scope.model.TRANSFER_UNIT_ID == $scope.model.TRANS_RCV_UNIT_ID) {
            $scope.showLoader = false;
            notificationservice.Notification("Please select a diferent unit", "", 'Please select a diferent unit');
            return;
        }

        

        stockTransferService.AddOrUpdate(model).then(function (data) {
            //notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            ;
            if (parseInt(data.data) > 0) {
                $scope.showLoader = false;
                $scope.model.MST_ID = data.data;
                notificationservice.Notification(1, 1, 'data save successfully!');
                setTimeout(function () {
                    $scope.LoadFormData();
                }, 1000)



                //window.location.reload();

            }
            else {
                $scope.showLoader = false;
                $scope.addDefaultRow($scope.GridDefalutData());
                notificationservice.Notification('data not save successfully!', "", 'data not save successfully!');
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
    $scope.LoadUnitData();
    //$scope.LoadRequisitionData();
    $scope.LoadProductData();
    $scope.LoadStatus();




}]);