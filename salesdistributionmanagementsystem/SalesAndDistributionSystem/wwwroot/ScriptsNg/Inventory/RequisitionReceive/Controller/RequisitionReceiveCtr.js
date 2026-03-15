
ngApp.controller('ngGridCtrl', ['$scope', 'RequisitionReceiveService', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, requisitionReceiveService, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {
        COMPANY_ID: 0,
        MST_ID: 0,
        MST_ID_ENCRYPTED: "",
        REQUISITION_UNIT_ID: "",
        REQUISITION_UNIT_NAME: "",
        ISSUE_UNIT_ID: "",
        ISSUE_UNIT_NAME: "",
        RECEIVE_NO: "",
        RECEIVE_DATE: "",
        ISSUE_NO: "",
        ISSUE_DATE: "",
        REQUISITION_NO: ""
        ,DISPATCH_NO: ""
        , DISPATCH_REQ_ID:0
        , RECEIVE_AMOUNT: 0
        , ISSUE_AMOUNT: 0
        , RECEIVE_BY: ""
        , STATUS: "Active"
        , REMAEKS: ""
        , requisitionRcvDtlList: []
    }

    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 0,
            DTL_ID: 0,
            MST_ID: 0,
            COMPANY_ID: 0,
            SKU_ID: "",
            SKU_CODE: '',
            UNIT_TP: 0,
            PACK_SIZE: 0,
            RECEIVE_QTY: 0,
            ISSUE_QTY: 0,
            RECEIVE_AMOUNT: 0,
         
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

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Requisition Receive Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.data = [];

    $scope.gridOptionsList = {
        data: [$scope.GridDefalutData()]
    }

    $scope.rowNumberGenerate = function () {

        $scope.model.RECEIVE_AMOUNT = 0;
        for (let i = 0; i < $scope.gridOptionsList.data.length; i++) {

            if ($scope.gridOptionsList.data[i].RECEIVE_AMOUNT != undefined) {
                if ($scope.gridOptionsList.data[i].RECEIVE_AMOUNT == '') {
                    $scope.gridOptionsList.data[i].RECEIVE_AMOUNT = 0;
                }
                $scope.model.RECEIVE_AMOUNT += ($scope.gridOptionsList.data[i].RECEIVE_AMOUNT)
            }
        }
    }

    $scope.addNewRow = () => {
        
        if ($scope.gridOptionsList.data.length > 0 && $scope.gridOptionsList.data[0].SKU_CODE != null && $scope.gridOptionsList.data[0].SKU_CODE != '' && $scope.gridOptionsList.data[0].SKU_CODE != 'undefined') {

            var newRow = {
                ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, MST_ID: $scope.gridOptionsList.data[0].MST_ID, DTL_ID: $scope.gridOptionsList.data[0].DTL_ID, SKU_ID: $scope.gridOptionsList.data[0].SKU_ID, SKU_CODE: $scope.gridOptionsList.data[0].SKU_CODE, RECEIVE_AMOUNT: $scope.gridOptionsList.data[0].RECEIVE_AMOUNT, ISSUE_QTY: $scope.gridOptionsList.data[0].ISSUE_QTY, RECEIVE_QTY: $scope.gridOptionsList.data[0].RECEIVE_QTY, UNIT_TP: $scope.gridOptionsList.data[0].UNIT_TP, STATUS: $scope.gridOptionsList.data[0].STATUS
            }

            //$scope.RegionsList.push(newRowSelected);
            $scope.gridOptionsList.data.push(newRow);
            $scope.gridOptionsList.data[0] = $scope.GridDefalutData();

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
            $scope.rowNumberGenerate();


        } else {
            notificationservice.Notification("Only Single Row Left!!", "", 'Only Single Row Left!!');
        }


    }


    $scope.LoadFormData = function () {
        
        $scope.showLoader = true;
        requisitionReceiveService.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
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
        
        window.location = "/Inventory/RequisitionReceive/RequisitionRcv?Id=" + MST_ID_ENCRYPTED;
    }
    $scope.GetEditDataById = function (value) {
        
        if (value != undefined && value!= "0" && value.length > 0) {
            requisitionReceiveService.GetEditDataById(value).then(function (data) {
                

                if (data.data != null && data.data.requisitionRcvDtlList != null && data.data.requisitionRcvDtlList.length > 0) {
                    $scope.model.COMPANY_ID = data.data.COMPANY_ID;
                    $scope.model.MST_ID = parseInt(data.data.MST_ID);
                    $scope.model.RECEIVE_DATE = data.data.RECEIVE_DATE;
                    $scope.model.ISSUE_DATE = data.data.ISSUE_DATE;
                    $scope.model.ISSUE_UNIT_ID = data.data.ISSUE_UNIT_ID;
                  
                    $scope.model.REQUISITION_UNIT_ID = data.data.REQUISITION_UNIT_ID;
                    $scope.model.ISSUE_BY = data.data.ISSUE_BY;
                    $scope.model.ISSUE_AMOUNT = data.data.ISSUE_AMOUNT;

                    $scope.model.REMARKS = data.data.REMARKS;
                    $scope.model.RECEIVE_AMOUNT = data.data.RECEIVE_AMOUNT;
                    $scope.model.STATUS = data.data.STATUS;
                    $scope.model.REQUISITION_NO = data.data.REQUISITION_NO;
                    $scope.model.ISSUE_NO = data.data.ISSUE_NO;
                    $scope.model.RECEIVE_NO = data.data.RECEIVE_NO;
                    if (data.data.requisitionRcvDtlList != null) {
                        //for (var i = 0; i < data.data.requisitionRcvDtlList.length; i++) {
                        //    if (data.data.requisitionRcvDtlList[i].STOCK_QTY > 0) {
                        //        data.data.requisitionRcvDtlList[i].STOCK_QTY = parseInt(data.data.requisitionRcvDtlList[i].STOCK_QTY) + parseInt(data.data.requisitionRcvDtlList[i].ISSUE_QTY)
                        //    }

                        //}
                        $scope.gridOptionsList.data = data.data.requisitionRcvDtlList;

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
        if (entity.RECEIVE_QTY == '' || entity.RECEIVE_QTY == null || entity.RECEIVE_QTY < 0) {
            entity.RECEIVE_QTY = 0;
        }
        entity.RECEIVE_QTY = parseFloat(entity.UNIT_TP) * parseInt(entity.RECEIVE_QTY)
        $scope.rowNumberGenerate();

    };
    $scope.LoadSKUId = function () {
        setTimeout(function () {
            $('#SKU_CODE').trigger('change');
            $('#SKU_ID').trigger('change');
         

        }, 1000)

    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '40%', cellTemplate:
                '<select class="select2-single form-control" disabled   data-select2-id="{{row.entity.SKU_ID}}" id="SKU_ID"' +
                'name="SKU_ID" ng-model="row.entity.SKU_ID" style="width:100%" ng-change="grid.appScope.typeaheadSelectedSku(row.entity)">' +
                '<option ng-repeat="item in grid.appScope.Products" ng-selected="item.SKU_ID == row.entity.SKU_ID" value="{{item.SKU_ID}}">{{ item.SKU_NAME }} | Code: {{ item.SKU_ID }}</option>' +
                '</select>'
        }
        ,
        {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="text" disabled  style="text-align: right;"  ng-model="row.entity.SKU_CODE"  class="pl-sm" />'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit Tp', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number" style="text-align:right;" disabled  ng-model="row.entity.UNIT_TP"  class="pl-sm" />'
        },
        
        {
            name: 'ISSUE_QTY', field: 'ISSUE_QTY', displayName: 'Issue Qty', enableFiltering: true, width: '15%', cellTemplate:
                '<input type="number"  style="text-align:right;"  disabled ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.ISSUE_QTY"  class="pl-sm" />'
        },
        ,{
            name: 'RECEIVE_QTY', field: 'RECEIVE_QTY', displayName: 'Receive Qty', enableFiltering: true, width: '20%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.RECEIVE_QTY"  class="pl-sm" />'
        },
        {
            name: 'RECEIVE_AMOUNT', field: 'RECEIVE_AMOUNT', displayName: 'Receive Amount', visible: false, enableFiltering: true, width: '20%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-model="row.entity.RECEIVE_AMOUNT"  class="pl-sm" />'
        }
        
    ];

    $scope.gridOptionsList.rowTemplate = "<div  ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.isTrue = true;
    $scope.myObj = {
        "color": "red",

    }

    $scope.ClearForm = function () {
        window.location.href = "/Inventory/RequisitionReceive/RequisitionRcv";
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

        requisitionReceiveService.GetCompany().then(function (data) {
            
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

        requisitionReceiveService.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }


    $scope.LoadProductData = function () {
        $scope.showLoader = true;

        requisitionReceiveService.LoadProductData().then(function (data) {
            
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
            Controller_Name: 'RequisitionReceive',
            Action_Name: 'RequisitionRcv'
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

        requisitionReceiveService.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            
            
            $scope.Unit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.LoadRequisitionData = function () {
        $scope.showLoader = true;

        requisitionReceiveService.LoadRequisitionData($scope.model.COMPANY_ID).then(function (data) {
            $scope.RequisitionList = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }

    $scope.LoadRequisitionDtlData = function () {
        
        $scope.showLoader = true;
        var value = "";
        for (var i = 0; i < $scope.RequisitionList.length; i++) {
            if ($scope.RequisitionList[i].DISPATCH_REQ_ID == $scope.model.DISPATCH_REQ_ID) {
                
                $scope.model.REQUISITION_NO = $scope.RequisitionList[i].REQUISITION_NO;
                $scope.model.DISPATCH_NO = $scope.RequisitionList[i].DISPATCH_NO;
                $scope.model.REQUISITION_UNIT_ID = $scope.RequisitionList[i].REQUISITION_UNIT_ID;
                $scope.model.REQUISITION_UNIT_NAME = $scope.RequisitionList[i].REQUISITION_UNIT_NAME;
                //$scope.model.ISSUE_AMOUNT = $scope.RequisitionList[i].ISSUE_AMOUNT;
                $scope.model.ISSUE_NO = $scope.RequisitionList[i].ISSUE_NO;
                $scope.model.ISSUE_DATE = $scope.RequisitionList[i].ISSUE_DATE;
                $scope.model.ISSUE_UNIT_ID = $scope.RequisitionList[i].ISSUE_UNIT_ID;
                $scope.model.ISSUE_UNIT_NAME = $scope.RequisitionList[i].ISSUE_UNIT_NAME;

            }
        }
        ;
        requisitionReceiveService.GetRequisitionEditDataById($scope.model.COMPANY_ID, $scope.model.DISPATCH_REQ_ID).then(function (data) {
            
            
            
            $scope.gridOptionsList.data = data.data;
            var Receive_Amount = 0;
            for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
                $scope.gridOptionsList.data[i].RECEIVE_AMOUNT = $scope.gridOptionsList.data[i].DISPATCH_AMOUNT;
                $scope.gridOptionsList.data[i].RECEIVE_QTY = $scope.gridOptionsList.data[i].DISPATCH_QTY;
                $scope.gridOptionsList.data[i].SKU_ID = $scope.gridOptionsList.data[i].SKU_ID+"";
                $scope.gridOptionsList.data[i].ENTERED_BY = "";
                $scope.gridOptionsList.data[i].UPDATED_BY = "";
                Receive_Amount += $scope.gridOptionsList.data[i].DISPATCH_AMOUNT;

            }
            $scope.model.RECEIVE_AMOUNT = Receive_Amount;
            $scope.showLoader = false;
            $scope.LoadSKUId();
        }, function (error) {
            
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
        $scope.model.requisitionRcvDtlList = $scope.gridOptionsList.data;
        
        requisitionReceiveService.AddOrUpdate(model).then(function (data) {

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


