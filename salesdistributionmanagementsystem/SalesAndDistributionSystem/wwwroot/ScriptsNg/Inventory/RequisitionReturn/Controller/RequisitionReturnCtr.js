ngApp.controller('ngGridCtrl', ['$scope', 'RequisitionReturnService', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, requisitionReturnService, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {
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
        RETURN_RCV_UNIT_ID: "",
        RETURN_RCV_UNIT_NAME: "",
        RETURN_UNIT_ID: "",
        RETURN_TYPE: "",
        RETURN_NO: "",
        RECEIVE_NO: "",
        RETURN_DATE: $scope.formatDate(new Date()),
        REQUISITION_NO: ""
        , RECEIVE_AMOUNT: 0
        , RETURN_AMOUNT: 0
        , RETURN_BY: ""
        , STATUS: "Active"
        , REMAEKS: ""
        , requisitionReturnDtlList: []
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
            RECEIVE_QTY: 0,
            RETURN_QTY: 0,
            RETURN_AMOUNT: 0,
            RECEIVE_AMOUNT: 0
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
        $scope.model.RETURN_AMOUNT = 0;
        for (let i = 0; i < $scope.gridOptionsList.data.length; i++) {
            if ($scope.gridOptionsList.data[i].RETURN_AMOUNT != undefined) {
                if ($scope.gridOptionsList.data[i].RETURN_AMOUNT == '') {
                    $scope.gridOptionsList.data[i].RETURN_AMOUNT = 0;
                }
                $scope.model.RETURN_AMOUNT += ($scope.gridOptionsList.data[i].RETURN_AMOUNT)
            }
        }
    }

    $scope.addNewRow = () => {
        if ($scope.gridOptionsList.data.length > 0 && $scope.gridOptionsList.data[0].SKU_CODE != null && $scope.gridOptionsList.data[0].SKU_CODE != '' && $scope.gridOptionsList.data[0].SKU_CODE != 'undefined') {
            var newRow = {
                ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, MST_ID: $scope.gridOptionsList.data[0].MST_ID, DTL_ID: $scope.gridOptionsList.data[0].DTL_ID, SKU_ID: $scope.gridOptionsList.data[0].SKU_ID, SKU_CODE: $scope.gridOptionsList.data[0].SKU_CODE, STATUS: $scope.gridOptionsList.data[0].STATUS, RECEIVE_AMOUNT: $scope.gridOptionsList.data[0].RECEIVE_AMOUNT, RETURN_AMOUNT: $scope.gridOptionsList.data[0].RETURN_AMOUNT, RECEIVE_QTY: $scope.gridOptionsList.data[0].RECEIVE_QTY, RETURN_QTY: $scope.gridOptionsList.data[0].RETURN_QTY, UNIT_TP: $scope.gridOptionsList.data[0].UNIT_TP
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

    $scope.EditItem = (entity) => {
        if ($scope.gridOptionsList.data.length > 0) {
            var newRow = {
                ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, MST_ID: entity.MST_ID, DTL_ID: entity.DTL_ID, SKU_ID: entity.SKU_ID, SKU_CODE: entity.SKU_CODE, STATUS: entity.STATUS, RECEIVE_AMOUNT: entity.RECEIVE_AMOUNT, RETURN_AMOUNT: entity.RETURN_AMOUNT, RECEIVE_QTY: entity.RECEIVE_QTY, RETURN_QTY: entity.RETURN_QTY, UNIT_TP: entity.UNIT_TP, STATUS: entity.STATUS
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
        requisitionReturnService.LoadData($scope.model.COMPANY_ID).then(function (data) {
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
        window.location = "/Inventory/RequisitionReturn/RequisitionReturn?Id=" + MST_ID_ENCRYPTED;
    }
    $scope.GetEditDataById = function (value) {
        if (value != undefined && value.length > 0) {
            requisitionReturnService.GetEditDataById(value).then(function (data) {
                if (data.data != null && data.data.requisitionReturnDtlList != null && data.data.requisitionReturnDtlList.length > 0) {
                    $scope.model.COMPANY_ID = data.data.COMPANY_ID;
                    $scope.model.MST_ID = data.data.MST_ID;

                    $scope.model.RETURN_RCV_UNIT_ID = data.data.ISSUE_UNIT_ID;

                    $scope.model.RETURN_UNIT_ID = data.data.RETURN_UNIT_ID;
                    $scope.model.RETURN_BY = data.data.RETURN_BY;
                    $scope.model.RETURN_AMOUNT = data.data.RETURN_AMOUNT;
                    $scope.model.RETURN_DATE = data.data.RETURN_DATE;
                    $scope.model.RECEIVE_DATE = data.data.RECEIVE_DATE;
                    $scope.model.REMARKS = data.data.REMARKS;
                    $scope.model.RECEIVE_AMOUNT = data.data.RECEIVE_AMOUNT;
                    $scope.model.STATUS = data.data.STATUS;
                    $scope.model.RECEIVE_NO = data.data.RECEIVE_NO;
                    $scope.model.REQUISITION_NO = data.data.REQUISITION_NO;
                    $scope.model.RETURN_NO = data.data.RETURN_NO;
                    $scope.model.RETURN_TYPE = data.data.RETURN_TYPE;

                    $scope.gridOptionsList.data = data.data.requisitionReturnDtlList;
                }
                $scope.LoadSKUId();
                //$scope.addNewRow();
                //$scope.rowNumberGenerate();
                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                console.log(error);
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
        if (entity.RETURN_QTY == '' || entity.RETURN_QTY == null || entity.RETURN_QTY < 0) {
            entity.RETURN_QTY = 0;
        }
        entity.RETURN_AMOUNT = parseFloat(entity.UNIT_TP) * parseInt(entity.RETURN_QTY)
        $scope.rowNumberGenerate();
    };
    $scope.LoadSKUId = function () {
        setTimeout(function () {
            $('#SKU_CODE').trigger('change');
            $('#SKU_IDRETURN_RCV_UNIT_ID').trigger('change');
            $('#RETURN_RCV_UNIT_ID').trigger('change');

            $('#RETURN_UNIT_ID').trigger('change');
            $('#RETURN_TYPE').trigger('change');
        }, 1000)
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '20%', cellTemplate:
                '<input type="text" disabled  style="text-align: right;"  ng-model="row.entity.SKU_NAME"  class="pl-sm" />'
        }
        ,
        {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="text" disabled  style="text-align: right;"  ng-model="row.entity.SKU_CODE"  class="pl-sm" />'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number" style="text-align:right;" disabled  ng-model="row.entity.UNIT_TP"  class="pl-sm" />'
        }
        , {
            name: 'RECEIVE_QTY', field: 'RECEIVE_QTY', displayName: 'Receive Qty', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.RECEIVE_QTY"  class="pl-sm" />'
        },
        {
            name: 'STOCK_QTY', field: 'STOCK_QTY', displayName: 'Stock Qty', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.STOCK_QTY"  class="pl-sm" />'
        },
        {
            name: 'RETURN_QTY', field: 'RETURN_QTY', displayName: 'Return Qty', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" ng-disabled="grid.appScope.model.RETURN_TYPE==\'Full\'"  ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.RETURN_QTY"  class="pl-sm" />'
        },
        {
            name: 'RECEIVE_AMOUNT', field: 'RECEIVE_AMOUNT', displayName: 'Receive Amount', enableFiltering: true, width: '15%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-model="row.entity.RECEIVE_AMOUNT"  class="pl-sm" />'
        }
        ,
        {
            name: 'RETURN_AMOUNT', field: 'RETURN_AMOUNT', displayName: 'Return Amount', enableFiltering: true, width: '15%', cellTemplate:
                '<input type="number" disabled  style="text-align:right;"   ng-model="row.entity.RETURN_AMOUNT"  class="pl-sm" />'
        }

        //, {
        //    name: 'Action', displayName: 'Action', width: '10%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
        //        '<div style="margin:1px;">' +
        //        '<button style="margin-bottom: 5px;" ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.EditItem(row.entity)" type="button" class="btn btn-outline-success mb-1"><img src="/img/edit-icon.png" height="15px" ></button>' +

        //        '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

        //        '</div>'
        //}
        //, {
        //    name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
        //        '<div style="margin:1px;">' +
        //        '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Delete</button>' +
        //        '</div>'
        //},

    ];

    $scope.gridOptionsList.rowTemplate = "<div  ng-style='row.entity.RETURN_QTY >row.entity.STOCK_QTY && grid.appScope.myObj' ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.isTrue = true;
    $scope.myObj = {
        "color": "red",
    }

    $scope.ClearForm = function () {
        window.location.href = "/Inventory/RequisitionReturn/RequisitionReturn";
    }

    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        requisitionReturnService.GetCompany().then(function (data) {
            
            $scope.model.COMPANY_ID = parseFloat(data.data);
            /*            $scope.DataLoad($scope.model.COMPANY_ID);*/
            $interval(function () {
                $('#COMPANY_ID').trigger('change');
            }, 800, 4);
            $scope.showLoader = false;
        }, function (error) {
            console.log(error);
            $scope.showLoader = false;
        });
    }
    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;

        requisitionReturnService.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            console.log(error);
            $scope.showLoader = false;
        });
    }

    $scope.LoadProductData = function () {
        $scope.showLoader = true;

        requisitionReturnService.LoadProductData().then(function (data) {
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
            Controller_Name: 'RequisitionReturn',
            Action_Name: 'RequisitionReturn'
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

        requisitionReturnService.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.Unit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });;
            $scope.showLoader = false;
        }, function (error) {
            console.log(error);
            $scope.showLoader = false;
        });
    }
    $scope.LoadRequisitionData = function () {
        $scope.showLoader = true;

        requisitionReturnService.LoadRequisitionData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.RequisitionList = data.data;
            $scope.showLoader = false;
        }, function (error) {
            console.log(error);
            $scope.showLoader = false;
        });
    }

    $scope.LoadRequisitionDtlData = function () {
        if ($scope.model.RETURN_TYPE == "") {
            $scope.model.REQUISITION_NO = "";
            notificationservice.Notification('Please select requisition type!', "", 'Please select requisition type!');
            return;
        }

        $scope.showLoader = true;
        var value = "";
        
        for (var i = 0; i < $scope.RequisitionList.length; i++) {
            if ($scope.RequisitionList[i].REQUISITION_NO == $scope.model.REQUISITION_NO) {
                value = $scope.RequisitionList[i].MST_ID_ENCRYPTED;
                $scope.model.RECEIVE_AMOUNT = $scope.RequisitionList[i].RECEIVE_AMOUNT;
                $scope.model.RECEIVE_DATE = $scope.RequisitionList[i].RECEIVE_DATE;
                $scope.model.ISSUE_AMOUNT = $scope.RequisitionList[i].ISSUE_AMOUNT;
                $scope.model.RETURN_RCV_UNIT_ID = $scope.RequisitionList[i].ISSUE_UNIT_ID;
                $scope.model.RETURN_RCV_UNIT_NAME = $scope.RequisitionList[i].ISSUE_UNIT_NAME;

                $scope.model.RECEIVE_NO = $scope.RequisitionList[i].RECEIVE_NO;
            }
        }
        
        requisitionReturnService.GetRcvDtlDataById(value).then(function (data) {
            
            $scope.gridOptionsList.data = data.data.requisitionRcvDtlList;;
            if ($scope.model.RETURN_TYPE == "Partial") {
                for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
                    $scope.gridOptionsList.data[i].RETURN_AMOUNT = 0;
                    $scope.gridOptionsList.data[i].RETURN_QTY = 0;
                }
            }
            else {
                for (var i = 0; i < $scope.gridOptionsList.data?.length; i++) {
                    $scope.gridOptionsList.data[i].RETURN_AMOUNT = $scope.gridOptionsList.data[i].RECEIVE_AMOUNT;
                    $scope.gridOptionsList.data[i].RETURN_QTY = $scope.gridOptionsList.data[i].RECEIVE_QTY;
                }
            }

            $scope.showLoader = false;
            $scope.LoadSKUId();
        }, function (error) {
            console.log(error);
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
        $scope.model.requisitionReturnDtlList = $scope.gridOptionsList.data;
        for (var i = 0; i < $scope.model.requisitionReturnDtlList.length; i++) {
            if ($scope.model.requisitionReturnDtlList[i].RETURN_QTY > $scope.model.requisitionReturnDtlList[i].STOCK_QTY) {
                notificationservice.Notification("Return Qty is greater then stock Qty", "", 'Return Qty is greater then stock Qty!');
                $scope.showLoader = false;
                return;
            }
        }
        var Rturn_Amount = 0;
        for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
            $scope.gridOptionsList.data[i].RETURN_AMOUNT = $scope.gridOptionsList.data[i].RETURN_AMOUNT;
            Rturn_Amount += $scope.gridOptionsList.data[i].RETURN_AMOUNT;
        }
        $scope.model.RETURN_AMOUNT = Rturn_Amount;
        
        requisitionReturnService.AddOrUpdate(model).then(function (data) {
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