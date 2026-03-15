
ngApp.controller('ngGridCtrl', ['$scope', 'RequisitionIssueServices', 'LiveNotificationServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, requisitionIssueServices, LiveNotificationServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {
        COMPANY_ID: 0,
        MST_ID: 0,
        MST_ID_ENCRYPTED: "",
        REQUISITION_UNIT_ID: "",
        ISSUE_UNIT_ID: "",
        ISSUE_NO: "",
        ISSUE_DATE: "",
        REQUISITION_NO: ""
        , REQUISITION_DATE: ""
        , REQUISITION_AMOUNT: 0
        , ISSUE_AMOUNT: 0
        , ISSUE_BY: ""
        , STATUS: "Active"
        , REMAEKS: ""
        , TOTAL_VOLUME:0
        , TOTAL_WEIGHT:0
        , requisitionIssueDtlList: []
    }
    var connection = new signalR.HubConnectionBuilder().withUrl("/notificationhub").build();
    connection.start();
    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 0,
            DTL_ID: 0,
            MST_ID: 0,
            COMPANY_ID: 0,
            SKU_ID: "",
            SKU_CODE: '',
            UNIT_TP: 0,
            REQUISITION_QTY: 0,
            ISSUE_QTY: 0,
            REQUISITION_AMOUNT: 0,
            ISSUE_AMOUNT: 0,
            STATUS: 'Active',
            COMPANY_ID: 0,
            REMARKS: '',

        }
    }

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

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Requisition Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.data = [];

    $scope.gridOptionsList = {
        data: [$scope.GridDefalutData()]
    }

    $scope.rowNumberGenerate = function () {
       
        $scope.model.ISSUE_AMOUNT = 0;
        for (let i = 0; i < $scope.gridOptionsList.data.length; i++) {
            $scope.gridOptionsList.data[i].ROW_NO = i;
            if ($scope.gridOptionsList.data[i].ISSUE_AMOUNT != undefined) {
                if ($scope.gridOptionsList.data[i].ISSUE_AMOUNT == '') {
                    $scope.gridOptionsList.data[i].ISSUE_AMOUNT = 0;
                }
                $scope.model.ISSUE_AMOUNT += ($scope.gridOptionsList.data[i].ISSUE_AMOUNT)
            }
        }
    }
  
    $scope.addNewRow = () => {

        if ($scope.model.REQUISITION_NO == '') {
            notificationservice.Notification("No requisition no has selected.", "", 'No requisition no has selected!!');
            return;
        }
        
        if ($scope.gridOptionsList.data.length > 0 && $scope.gridOptionsList.data[0].SKU_CODE != null && $scope.gridOptionsList.data[0].SKU_CODE != '' && $scope.gridOptionsList.data[0].SKU_CODE != 'undefined') {
          
            var newRow = {
                ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, MST_ID: $scope.gridOptionsList.data[0].MST_ID, DTL_ID: $scope.gridOptionsList.data[0].DTL_ID, SKU_ID: $scope.gridOptionsList.data[0].SKU_ID, SKU_CODE: $scope.gridOptionsList.data[0].SKU_CODE, STATUS: $scope.gridOptionsList.data[0].STATUS, REQUISITION_AMOUNT: $scope.gridOptionsList.data[0].REQUISITION_AMOUNT, ISSUE_AMOUNT: $scope.gridOptionsList.data[0].ISSUE_AMOUNT, REQUISITION_QTY: $scope.gridOptionsList.data[0].REQUISITION_QTY, ISSUE_QTY: $scope.gridOptionsList.data[0].ISSUE_QTY, STOCK_QTY: $scope.gridOptionsList.data[0].STOCK_QTY, ISSUE_STOCK_QTY: $scope.gridOptionsList.data[0].ISSUE_STOCK_QTY, UNIT_TP: $scope.gridOptionsList.data[0].UNIT_TP, STATUS: $scope.gridOptionsList.data[0].STATUS
                , NO_OF_SHIPPER: $scope.gridOptionsList.data[0].NO_OF_SHIPPER, LOOSE_QTY: $scope.gridOptionsList.data[0].LOOSE_QTY, SHIPPER_WEIGHT: $scope.gridOptionsList.data[0].SHIPPER_WEIGHT, SHIPPER_VOLUME: $scope.gridOptionsList.data[0].SHIPPER_VOLUME, PER_PACK_WEIGHT: $scope.gridOptionsList.data[0].PER_PACK_WEIGHT, PER_PACK_VOLUME: $scope.gridOptionsList.data[0].PER_PACK_VOLUME, SHIPPER_QTY: $scope.gridOptionsList.data[0].SHIPPER_QTY, TOTAL_SHIPPER_WEIGHT: $scope.gridOptionsList.data[0].TOTAL_SHIPPER_WEIGHT
                , TOTAL_SHIPPER_VOLUME: $scope.gridOptionsList.data[0].TOTAL_SHIPPER_VOLUME, LOOSE_WEIGHT: $scope.gridOptionsList.data[0].LOOSE_WEIGHT, LOOSE_VOLUME: $scope.gridOptionsList.data[0].LOOSE_VOLUME, TOTAL_WEIGHT: $scope.gridOptionsList.data[0].TOTAL_WEIGHT, TOTAL_VOLUME: $scope.gridOptionsList.data[0].TOTAL_VOLUME
            }
         
            //$scope.RegionsList.push(newRowSelected);
            $scope.gridOptionsList.data.push(newRow);

            $scope.gridOptionsList.data[0] = $scope.GridDefalutData();

        }
        //} else {
        //    notificationservice.Notification("Please Enter Valid Region First", "", 'Only Single Row Left!!');

        //}
        $scope.LoadSKUId();
        $scope.rowNumberGenerate();
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
                ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, MST_ID: entity.MST_ID, DTL_ID: entity.DTL_ID, REGION_ID: entity.REGION_ID, REGION_CODE: entity.REGION_CODE, SKU_ID: entity.SKU_ID, SKU_CODE: entity.SKU_CODE, STATUS: entity.STATUS, REQUISITION_AMOUNT: entity.REQUISITION_AMOUNT, ISSUE_AMOUNT: entity.ISSUE_AMOUNT, REQUISITION_QTY: entity.REQUISITION_QTY, STOCK_QTY: entity.STOCK_QTY, ISSUE_STOCK_QTY: entity.ISSUE_STOCK_QTY,UNIT_TP: entity.UNIT_TP, STATUS: entity.STATUS

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
        requisitionIssueServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
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
        
        window.location = "/Inventory/RequisitionIssue/RequisitionIssue?Id=" + MST_ID_ENCRYPTED;
    }
    $scope.GetEditDataById = function (value) {
        
        if (value != undefined && value.length > 0) {
            requisitionIssueServices.GetEditDataById(value).then(function (data) {
                

                if (data.data != null && data.data.requisitionIssueDtlList != null && data.data.requisitionIssueDtlList.length > 0) {
                    $scope.model.COMPANY_ID = data.data.COMPANY_ID;
                    $scope.model.MST_ID = data.data.MST_ID;
                    $scope.model.REQUISITION_DATE = data.data.REQUISITION_DATE;
                    $scope.model.ISSUE_DATE = data.data.ISSUE_DATE;
                    $scope.model.ISSUE_UNIT_ID = data.data.ISSUE_UNIT_ID;
                    $scope.model.REQUISITION_UNIT_ID = data.data.REQUISITION_UNIT_ID;
                    $scope.model.ISSUE_BY = data.data.ISSUE_BY;
                    $scope.model.ISSUE_AMOUNT = data.data.ISSUE_AMOUNT;
                    $scope.model.ISSUE_DATE = data.data.ISSUE_DATE;
                    $scope.model.REMARKS = data.data.REMARKS;
                    $scope.model.REQUISITION_AMOUNT = data.data.REQUISITION_AMOUNT;
                    $scope.model.STATUS = data.data.STATUS;
                    $scope.model.REQUISITION_NO = data.data.REQUISITION_NO;
                    $scope.model.ISSUE_NO = data.data.ISSUE_NO;
                    $scope.model.TOTAL_VOLUME = data.data.TOTAL_VOLUME;
                    $scope.model.TOTAL_WEIGHT = data.data.TOTAL_WEIGHT;
                    if (data.data.requisitionIssueDtlList != null) {
                        for (var i = 0; i < data.data.requisitionIssueDtlList.length; i++) {
                            if (data.data.requisitionIssueDtlList[i].STOCK_QTY > 0) {
                                data.data.requisitionIssueDtlList[i].STOCK_QTY = parseInt(data.data.requisitionIssueDtlList[i].STOCK_QTY) + parseInt(data.data.requisitionIssueDtlList[i].ISSUE_QTY)

                            }
                           
                        }
                        $scope.gridOptionsList.data = [$scope.GridDefalutData(), ...data.data.requisitionIssueDtlList]; 

                    }


                }
                $scope.LoadSKUId();
                //$scope.addNewRow();
                //$scope.rowNumberGenerate();
                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                
            });
        }
    }

    $scope.typeaheadSelectedSku = function (entity) {
        
        const searchIndex = $scope.Products.findIndex((x) => x.SKU_ID == entity.SKU_ID);
        entity.SKU_CODE = $scope.Products[searchIndex].SKU_CODE;
        entity.SKU_ID = $scope.Products[searchIndex].SKU_ID;
        entity.ISSUE_STOCK_QTY = parseInt($scope.Products[searchIndex].STOCK_QTY);

        requisitionIssueServices.GetUnitWiseSkuPrice(entity.SKU_ID, entity.SKU_CODE)
            .then(function (priceData) {
                if (priceData.data < 0 || Object.is(priceData.data, NaN)) {
                    entity.UNIT_TP = 0;
                }
                else {
                    entity.UNIT_TP = priceData.data;
                }
            });

        //$scope.LoadSKUId();


    };
    $scope.typeaheadSelectedQty = function (entity) {
        
        //const searchIndex = $scope.Products.findIndex((x) => x.SKU_ID == entity.SKU_ID);
        //entity.SKU_CODE = $scope.Products[searchIndex].SKU_CODE;
        //entity.UNIT_TP = $scope.Products[searchIndex].UNIT_TP;
        if (entity.ISSUE_QTY == '' || entity.ISSUE_QTY == null || entity.ISSUE_QTY <0 ) {
            entity.ISSUE_QTY = 0;
        }
        $scope.GetWeightVolumeCal(entity);
        entity.ISSUE_AMOUNT = parseFloat(entity.UNIT_TP) * parseInt(entity.ISSUE_QTY)
        $scope.rowNumberGenerate();

    };
    $scope.LoadSKUId = function () {
        setTimeout(function () {
            $('#SKU_CODE').trigger('change');
            $('#SKU_ID').trigger('change');
            $("#REQUISITION_UNIT_ID").trigger('change');
    
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
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '30%', cellTemplate:
                '<select class="select2-single form-control" ng-disabled="row.entity.ROW_NO != 0" data-select2-id="{{row.entity.SKU_CODE}}" id="SKU_ID"' +
                'name="SKU_ID" ng-model="row.entity.SKU_ID" style="width:100%" ng-change="grid.appScope.typeaheadSelectedSku(row.entity)">' +
                '<option ng-repeat="item in grid.appScope.Products" ng-selected="item.SKU_ID == row.entity.SKU_ID" value="{{item.SKU_ID}}">{{ item.SKU_NAME }} | Code: {{ item.SKU_CODE }}</option>' +
                '</select>'
         
        }
        
        , 
        {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="text" disabled  style="text-align: right;"  ng-model="row.entity.SKU_CODE"  class="pl-sm" />'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'UNIT TP', enableFiltering: true, width: '8%', cellTemplate:
                '<input type="number" style="text-align:right;" disabled  ng-model="row.entity.UNIT_TP"  class="pl-sm" />'
        }
        , {
            name: 'REQUISITION_QTY', field: 'REQUISITION_QTY', displayName: 'Req. Qty', enableFiltering: true, width: '9%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.REQUISITION_QTY"  class="pl-sm" />'
        },

        {
            name: 'STOCK_QTY', field: 'STOCK_QTY', displayName: 'Req. Stock Qty', enableFiltering: true, width: '12%', cellTemplate:
                '<input type="text"  disabled style="text-align:right;"  ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.STOCK_QTY"  class="pl-sm" />'
        },

        {
            name: 'ISSUE_QTY', field: 'ISSUE_QTY', displayName: 'Issue Qty', enableFiltering: true, width: '9%', cellTemplate:
                '<input type="number"  style="text-align:right;"  ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.ISSUE_QTY"  class="pl-sm" />'
        },
        {
            name: 'ISSUE_STOCK_QTY', field: 'ISSUE_STOCK_QTY', displayName: 'Issue Stock Qty', enableFiltering: true, width: '13%', cellTemplate:
                '<input type="text"  disabled style="text-align:right;"  ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.ISSUE_STOCK_QTY"  class="pl-sm" />'
        },

      
      
        {
            name: 'REQUISITION_AMOUNT', field: 'REQUISITION_AMOUNT', displayName: 'Requisition Amount', visible: false, enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-model="row.entity.REQUISITION_AMOUNT"  class="pl-sm" />'
        }
        ,
        {
            name: 'ISSUE_AMOUNT', field: 'ISSUE_AMOUNT', displayName: 'Issue Amount', visible: false, enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-model="row.entity.ISSUE_AMOUNT"  class="pl-sm" />'
        }
      
        , {
            name: 'Action', displayName: 'Action', width: '10%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                //'<button style="margin-bottom: 5px;" ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.EditItem(row.entity)" type="button" class="btn btn-outline-success mb-1"><img src="/img/edit-icon.png" height="15px" ></button>' +

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

    $scope.gridOptionsList.rowTemplate = "<div  ng-style='row.entity.ISSUE_QTY >row.entity.ISSUE_STOCK_QTY && grid.appScope.myObj' ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.isTrue = true;
    $scope.myObj = {
        "color": "red",
       
    }

    $scope.ClearForm = function () {
        window.location.href = "/Inventory/RequisitionIssue/RequisitionIssue";
    }
    //$scope.DataLoad = function (companyId) {
    //    $scope.showLoader = true;
    //    $scope.SkuList = "";
    //    $scope.GetExistingSku();
    
    //    setTimeout(function () {
    //        requisitionServices.LoadFilteredProduct($scope.model).then(function (data) {
    //            ;
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
    //            //    ;
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

        requisitionIssueServices.GetCompany().then(function (data) {
            
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

        requisitionIssueServices.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }


    $scope.LoadProductData = function () {
        $scope.showLoader = true;

        requisitionIssueServices.LoadProductData($scope.model.COMPANY_ID, $scope.model.ISSUE_UNIT_ID).then(function (data) {
            
            for (var i = 0; i < data.data.length; i++) {
                data.data[i].SKU_ID = data.data[i].SKU_ID.toString();
                if (data.data[i].STOCK_QTY == '' || data.data[i].STOCK_QTY == undefined) {
                    data.data[i].STOCK_QTY = 0;
                }
            }
         
            $scope.Products = data.data;
         
            $scope.showLoader = false;
            var _Products = {
                PRODUCT_ID: "0",
                PRODUCT_NAME: "All",
                PRODUCT_CODE: "ALL",
                STOCK_QTY:"0"

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
            Controller_Name: 'RequisitionIssue',
            Action_Name: 'RequisitionIssue'
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
    $scope.UnitLoad = function () {
        requisitionIssueServices.GetUnit().then(function (data) {

            $scope.model.UNIT_ID = parseInt(data.data);
            $scope.model.ORDER_UNIT_ID = parseInt(data.data);

            for (var i = 0; i < $scope.Unit.length; i++) {
                if ($scope.model.UNIT_ID == $scope.Unit[i].UNIT_ID) {
                    $scope.model.UNIT_NAME = $scope.Unit[i].UNIT_NAME;
                }
            }

            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }

    $scope.LoadUnitData = function () {
        $scope.showLoader = true;

        requisitionIssueServices.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Unit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });
            $scope.UnitLoad();
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.LoadRequisitionData = function () {
        $scope.showLoader = true;

        requisitionIssueServices.LoadRequisitionData($scope.model.COMPANY_ID).then(function (data) {
            
            
            $scope.RequisitionList = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.GetWeightVolumeCal = (entity) => {

        var count = 0;
        if (entity != null && entity != 'undefined') {

            requisitionIssueServices.LoadProductWeightData($scope.model.COMPANY_ID, entity.SKU_ID, entity.ISSUE_QTY).then(function (data) {

                
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
                    if ($scope.gridOptionsList.data[i].TOTAL_VOLUME > 0 || $scope.gridOptionsList.data[i].TOTAL_WEIGHT > 0) {
                        $scope.model.TOTAL_WEIGHT += $scope.gridOptionsList.data[i].TOTAL_VOLUME;
                        $scope.model.TOTAL_VOLUME += $scope.gridOptionsList.data[i].TOTAL_WEIGHT;
                    }

                }
                $scope.rowNumberGenerate();

            }, function (error) {

            });

        }
        else {
            notificationservice.Notification("No item has added!", "", 'No item has added!');

        }
    };
    $scope.LoadRequisitionDtlData = function () {
        
        $scope.showLoader = true;
        var value = "";
        for (var i = 0; i < $scope.RequisitionList.length; i++)
        {
            if ($scope.RequisitionList[i].REQUISITION_NO == $scope.model.REQUISITION_NO)
            {
                value = $scope.RequisitionList[i].MST_ID_ENCRYPTED;
                $scope.model.REQUISITION_AMOUNT = $scope.RequisitionList[i].REQUISITION_AMOUNT;
                $scope.model.REQUISITION_DATE = $scope.RequisitionList[i].REQUISITION_DATE;
                $scope.model.ISSUE_AMOUNT = $scope.RequisitionList[i].REQUISITION_AMOUNT;
                $scope.model.REQUISITION_UNIT_ID = $scope.RequisitionList[i].REQUISITION_UNIT_ID;
                $scope.model.TOTAL_VOLUME = $scope.RequisitionList[i].TOTAL_VOLUME;
                $scope.model.TOTAL_WEIGHT = $scope.RequisitionList[i].TOTAL_WEIGHT;
             
            }
        }
       
        requisitionIssueServices.GetRequisitionEditDataById(value).then(function (data) {
            
           
            $scope.gridOptionsList.data =[$scope.GridDefalutData(), ...data.data.requisitionDtlList];
            for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
                $scope.gridOptionsList.data[i].ISSUE_AMOUNT = $scope.gridOptionsList.data[i].REQUISITION_AMOUNT;
                $scope.gridOptionsList.data[i].ISSUE_QTY = $scope.gridOptionsList.data[i].REQUISITION_QTY;

            }
            $scope.showLoader = false;
            $scope.LoadSKUId();
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }

    $scope.SendNotification = function () {
        $scope.showLoader = true;

        LiveNotificationServices.Notification_Permitted_Users(4, $scope.model.COMPANY_ID, $scope.model.ISSUE_UNIT_ID).then(function (data) {
            
            $scope.showLoader = false;

            $scope.Users = data.data;
            $scope.Permitted_Users = [];
            if ($scope.Users != undefined && $scope.Users != null) {
                for (var i = 0; i < $scope.Users.length; i++) {
                    $scope.Permitted_Users.push(JSON.stringify(parseInt($scope.Users[i].USER_ID)));
                }
                connection.invoke("SendMessage", $scope.Permitted_Users, ": New Requisition has been Issued! Check out the notification for detail!!!!").catch(function (err) {
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
        
        $scope.showLoader = true;
        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);
        $scope.model.requisitionIssueDtlList = $scope.gridOptionsList.data;
        if ($scope.model.requisitionIssueDtlList != null) {
            if ($scope.model.requisitionIssueDtlList.length > 1 && $scope.model.requisitionIssueDtlList[0].SKU_CODE == "") {
                $scope.model.requisitionIssueDtlList.splice(0, 1);
            }
        }

        for (var i = 0; i < $scope.model.requisitionIssueDtlList.length; i++) {
            if ($scope.model.requisitionIssueDtlList[i].ISSUE_QTY <= 0) {
                notificationservice.Notification("Issue Qty should be greater then zero", "", 'Issue Qty should be greater then zero!');
                $scope.showLoader = false;
                return;
            }
 
            if ($scope.model.requisitionIssueDtlList[i].ISSUE_QTY > $scope.model.requisitionIssueDtlList[i].ISSUE_STOCK_QTY) {
                notificationservice.Notification("Issue Qty is greater then stock Qty", "", 'Issue Qty is greater then stock Qty!');
                $scope.showLoader = false;
                return;
            }
        }
     
        requisitionIssueServices.AddOrUpdate(model).then(function (data) {
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
    $scope.LoadRequisitionData();
    $scope.LoadProductData();
    $scope.LoadStatus();




}]);

