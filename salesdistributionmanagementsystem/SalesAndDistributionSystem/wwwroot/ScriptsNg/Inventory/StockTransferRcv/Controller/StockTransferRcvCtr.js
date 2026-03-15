ngApp.controller('ngGridCtrl', ['$scope', 'StockTransferRcvService', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, stockTransferRcvService, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

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
        TRANS_RCV_NO: "",
        TRANS_RCV_DATE: $scope.formatDate(new Date()),
        TRANS_RCV_UNIT_ID: "",

        TRANSFER_UNIT_ID: 0,

        TRANSFER_NO: "",
        TRANSFER_DATE: "",
        TRANSFER_AMOUNT: 0,
        TRANS_RCV_AMOUNT: 0

        , TRANS_RCV_BY: ""
        , STATUS: "Active"
        , REMAEKS: ""
        , stockTransferDtlRcvList: []
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
            TRANSFER_AMOUNT: 0,
            TRANS_RCV_QTY:0,
            TRANS_RCV_AMOUNT:0

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
            TRANS_RCV_QTY = 0,
            TRANS_RCV_AMOUNT = 0,
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
    $scope.TransferList = [];

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
        
        $scope.model.TRANS_RCV_AMOUNT = 0;
        for (let i = 0; i < $scope.gridOptionsList.data.length; i++) {

            $scope.gridOptionsList.data[i].ROW_NO = i;
            $scope.model.TRANS_RCV_AMOUNT += ($scope.gridOptionsList.data[i].TRANS_RCV_AMOUNT)

        }
    }





    $scope.LoadFormData = function () {
        
        $scope.showLoader = true;
        stockTransferRcvService.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
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
        
        window.location = "/Inventory/StockTransferRcv/StockTransferRcv?Id=" + MST_ID_ENCRYPTED;
    }
    $scope.GetEditDataById = function (value) {
        
        if (value != undefined && value.length > 0) {
            stockTransferRcvService.GetEditDataById(value).then(function (data) {
                

                if (data.data != null && data.data.stockTransferDtlRcvList != null && data.data.stockTransferDtlRcvList.length > 0) {
                    $scope.model.COMPANY_ID = data.data.COMPANY_ID;
                    $scope.model.MST_ID = data.data.MST_ID;
                    $scope.model.SKU_ID = data.data.SKU_ID;

                    $scope.model.TRANSFER_UNIT_ID = data.data.TRANSFER_UNIT_ID;

                    $scope.model.TRANS_RCV_UNIT_ID = data.data.TRANS_RCV_UNIT_ID;
                    $scope.model.TRANS_RCV_BY = data.data.TRANSFER_BY;
                    $scope.model.TRANSFER_AMOUNT = data.data.TRANSFER_AMOUNT;
                    $scope.model.TRANS_RCV_AMOUNT = data.data.TRANS_RCV_AMOUNT;
                    $scope.model.TRANSFER_DATE = data.data.TRANSFER_DATE;
                    $scope.model.TRANS_RCV_DATE = data.data.TRANS_RCV_DATE;


                    $scope.model.REMARKS = data.data.REMARKS;

                    $scope.model.STATUS = data.data.STATUS;
                    $scope.model.TRANSFER_NO = data.data.TRANSFER_NO;
                    $scope.model.TRANS_RCV_NO = data.data.TRANS_RCV_NO;

    

                    $scope.gridOptionsList.data = data.data.stockTransferDtlRcvList;
    
                }
                $scope.model.TRANS_RCV_UNIT_ID = data.data.TRANS_RCV_UNIT_ID;
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
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '20%', cellTemplate:
                '<select class="select2-single form-control" disabled   data-select2-id="{{row.entity.SKU_CODE}}" id="SKU_ID"' +
                'name="SKU_ID" ng-model="row.entity.SKU_ID" style="width:100%" ng-change="grid.appScope.typeaheadSelectedSku(row.entity)">' +
                '<option ng-repeat="item in grid.appScope.Products" ng-selected="item.SKU_ID == row.entity.SKU_ID" value="{{item.SKU_ID}}">{{ item.SKU_NAME }} | Code: {{ item.SKU_CODE }}</option>' +
                '</select>'
        }
        ,
        {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '20%', cellTemplate:
                '<input type="text" disabled  style="text-align: right;"  ng-model="row.entity.SKU_CODE"  class="pl-sm" />'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number" disabled style="text-align:right;" disabled  ng-model="row.entity.UNIT_TP"  class="pl-sm" />'
        }
        , {
            name: 'PACK_SIZE', field: 'PACK_SIZE', displayName: 'PACK Size', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="text" disabled style="text-align:right;" disabled  ng-model="row.entity.PACK_SIZE"  class="pl-sm" />'
        }
     
        , {
            name: 'TRANSFER_QTY', field: 'TRANSFER_QTY', displayName: 'Transfer Qty', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number" disabled style="text-align:right;"  ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.TRANSFER_QTY"  class="pl-sm" />'
        },
        {
            name: 'TRANS_RCV_QTY', field: 'TRANS_RCV_QTY', displayName: 'Receive Qty', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number" disabled style="text-align:right;"  ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.TRANS_RCV_QTY"  class="pl-sm" />'
        },

        {
            name: 'TRANS_RCV_AMOUNT', field: 'TRANS_RCV_AMOUNT', displayName: 'Receive Amount', enableFiltering: true, width: '15%', cellTemplate:
                '<input type="number" disabled  style="text-align:right;" disabled  ng-model="row.entity.TRANS_RCV_AMOUNT"  class="pl-sm" />'
        }


    
        //, {
        //    name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
        //        '<div style="margin:1px;">' +
        //        '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Delete</button>' +
        //        '</div>'
        //},

    ];

   

    $scope.isTrue = true;
    $scope.myObj = {
        "color": "red",

    }

    $scope.ClearForm = function () {
        window.location.href = "/Inventory/StrockTransferRcv/StrockTransferRcv";
    }

    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        stockTransferRcvService.GetCompany().then(function (data) {
            
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

        stockTransferRcvService.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }


    $scope.LoadProductData = function () {
        $scope.showLoader = true;

        stockTransferRcvService.LoadProductData().then(function (data) {
            
            $scope.Products = data.data;
            $scope.showLoader = false;
            var _Products = {
                PRODUCT_ID: "0",
                PRODUCT_NAME: "All",
                PRODUCT_CODE: "ALL",
                PACK_SIZE: ""

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
            Controller_Name: 'Requisition',
            Action_Name: 'RequisitionRaise'
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

        stockTransferRcvService.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            
            
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
    $scope.LoadTransferData = function () {
        $scope.showLoader = true;


        stockTransferRcvService.LoadTransferData($scope.model.COMPANY_ID).then(function (data) {
            
            
            $scope.TransferList = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });;
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }
    $scope.LoadTransferDtlData = function () {
        if ($scope.model.RETURN_TYPE == "") {
            $scope.model.REQUISITION_NO = "";
            notificationservice.Notification('Please select requisition type!', "", 'Please select requisition type!');
            return;
        }

        $scope.showLoader = true;
        var value = "";
        for (var i = 0; i < $scope.TransferList.length; i++) {
            if ($scope.TransferList[i].TRANSFER_NO == $scope.model.TRANSFER_NO) {
                value = $scope.TransferList[i].MST_ID_ENCRYPTED;
                $scope.model.TRANSFER_AMOUNT = $scope.TransferList[i].TRANSFER_AMOUNT;
                $scope.model.TRANSFER_DATE = $scope.TransferList[i].TRANSFER_DATE;
       
        
                $scope.model.TRANSFER_UNIT_ID = $scope.TransferList[i].TRANSFER_UNIT_ID;
       

       

            }
        }

        stockTransferRcvService.GetTransferDtlDataById(value).then(function (data) {
            
            
            $scope.gridOptionsList.data = data.data.stockTransferDtlList;;
          
            for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {

                $scope.gridOptionsList.data[i].TRANS_RCV_AMOUNT = $scope.gridOptionsList.data[i].TRANSFER_AMOUNT;
                $scope.gridOptionsList.data[i].TRANS_RCV_QTY = $scope.gridOptionsList.data[i].TRANSFER_QTY;

            }
           
            $scope.showLoader = false;
            $scope.LoadSKUId();
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }


    $scope.SaveData = function (model) {
        
        if ($scope.model.TRANS_RCV_UNIT_ID == "") {
            notificationservice.Notification('Receive not selected!', "", 'Receive not selected!');
            return;
        }

        $scope.showLoader = true;
        if ($scope.model.TRANSFER_AMOUNT == NaN) {
            $scope.model.TRANSFER_AMOUNT = 0
        }

        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);



        $scope.model.stockTransferDtlRcvList = $scope.gridOptionsList.data;
    
   
        

        stockTransferRcvService.AddOrUpdate(model).then(function (data) {
            //notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            ;
            if (parseInt(data.data) > 0) {
                $scope.showLoader = false;
                $scope.model.MST_ID = data.data;
                notificationservice.Notification(1, 1, 'data save successfully!');
                setTimeout(function () {
                    $scope.LoadFormData();
                }, 3000)



                //window.location.reload();

            }
            else {
                $scope.showLoader = false;
         
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
    $scope.LoadTransferData();
    $scope.LoadProductData();
    $scope.LoadStatus();

}]);