ngApp.controller('ngGridCtrl', ['$scope', 'RequisitionServices', 'LiveNotificationServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, requisitionServices, LiveNotificationServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'
    var connection = new signalR.HubConnectionBuilder().withUrl("/notificationhub").build();
    connection.start();
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
        REQUISITION_UNIT_ID: 0,
        ISSUE_UNIT_ID: 0,
        REQUISITION_NO: ""
        , REQUISITION_DATE: $scope.formatDate(new Date())
        , REQUISITION_AMOUNT: 0
        , REQUISITION_RAISE_BY: ""
        , STATUS: "Active"
        , REMAEKS: ""
        , TOTAL_WEIGHT: 0
        , TOTAL_VOLUME: 0
        , requisitionDtlList: []
    }

    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 0,
            DTL_ID: 0,
            MST_ID: 0,
            COMPANY_ID: 0,
            SKU_ID: "null",
            SKU_CODE: '',
            UNIT_TP: 0,
            SUGGESTED_QTY: 0,
            STOCK_QTY: 0,
            REQUISITION_QTY: 0,
            REQUISITION_AMOUNT: 0,
            STATUS: 'Active',
            COMPANY_ID: 0,
            REMARKS: '',
        }
    }
    $scope.ClearEntity = function (entity) {
        entity.ROW_NO = 0,
            entity.DTL_ID = 0,
            entity.MST_ID = 0,
            entity.COMPANY_ID = 0,
            entity.SKU_ID = "null",
            entity.SKU_CODE = '',
            entity.UNIT_TP = 0,
            REQUISITION_QTY = 0,
            REQUISITION_AMOUNT = 0,
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

    $scope.BaseProducts = [];
    $scope.Categories = [];
    $scope.Brands = [];
    $scope.Groups = [];
    $scope.Products = [];
    $scope.existingSKU = [];
    $scope.Products = [];
    $scope.Status = [];

    $scope.Categories = [];
    $scope.Brands = [];
    $scope.Groups = [];

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Requisition Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.data = [];

    $scope.gridOptionsList = {
        data: [$scope.GridDefalutData()]
    }

    $scope.rowNumberGenerate = function () {
        $scope.model.REQUISITION_AMOUNT = 0;
        for (let i = 0; i < $scope.gridOptionsList.data.length; i++) {
            $scope.gridOptionsList.data[i].ROW_NO = i;
            $scope.model.REQUISITION_AMOUNT += ($scope.gridOptionsList.data[i].REQUISITION_AMOUNT)
            //if ($scope.gridOptionsList.data[i].SKU_CODE == '') {
            //    $scope.gridOptionsList.data[i].SKU_ID = 0;
            //    $scope.gridOptionsList.data[i].SKU_NAME = '';

            //}
        }
    }
    $scope.addDefaultRow = (entity) => {
        var newRow = {
            ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, MST_ID: $scope.gridOptionsList.data[0].MST_ID, DTL_ID: $scope.gridOptionsList.data[0].DTL_ID, SKU_ID: $scope.gridOptionsList.data[0].SKU_ID, SKU_CODE: $scope.gridOptionsList.data[0].SKU_CODE, STATUS: $scope.gridOptionsList.data[0].STATUS, REQUISITION_AMOUNT: $scope.gridOptionsList.data[0].REQUISITION_AMOUNT, REQUISITION_QTY: $scope.gridOptionsList.data[0].REQUISITION_QTY, STOCK_QTY: $scope.gridOptionsList.data[0].STOCK_QTY, SUGGESTED_QTY: $scope.gridOptionsList.data[0].SUGGESTED_QTY, UNIT_TP: $scope.gridOptionsList.data[0].UNIT_TP, STATUS: $scope.gridOptionsList.data[0].STATUS
            , NO_OF_SHIPPER: $scope.gridOptionsList.data[0].NO_OF_SHIPPER, LOOSE_QTY: $scope.gridOptionsList.data[0].LOOSE_QTY, SHIPPER_WEIGHT: $scope.gridOptionsList.data[0].SHIPPER_WEIGHT, SHIPPER_VOLUME: $scope.gridOptionsList.data[0].SHIPPER_VOLUME, PER_PACK_WEIGHT: $scope.gridOptionsList.data[0].PER_PACK_WEIGHT, PER_PACK_VOLUME: $scope.gridOptionsList.data[0].PER_PACK_VOLUME, SHIPPER_QTY: $scope.gridOptionsList.data[0].SHIPPER_QTY, TOTAL_SHIPPER_WEIGHT: $scope.gridOptionsList.data[0].TOTAL_SHIPPER_WEIGHT
            , TOTAL_SHIPPER_VOLUME: $scope.gridOptionsList.data[0].TOTAL_SHIPPER_VOLUME, LOOSE_WEIGHT: $scope.gridOptionsList.data[0].LOOSE_WEIGHT, LOOSE_VOLUME: $scope.gridOptionsList.data[0].LOOSE_VOLUME, TOTAL_WEIGHT: $scope.gridOptionsList.data[0].TOTAL_WEIGHT, TOTAL_VOLUME: $scope.gridOptionsList.data[0].TOTAL_VOLUME
        }
        $scope.gridOptionsList.data.push(newRow);
        $scope.gridOptionsList.data[0] = $scope.GridDefalutData();
        $scope.rowNumberGenerate();
    }

    $scope.GetWeightVolumeCal = (entity) => {
        var count = 0;
        if (entity != null && entity != 'undefined') {
            requisitionServices.LoadProductWeightData($scope.model.COMPANY_ID, entity.SKU_ID, entity.REQUISITION_QTY).then(function (data) {
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
                $scope.model.TOTAL_WEIGHT += entity.TOTAL_WEIGHT;
                $scope.model.TOTAL_VOLUME += entity.TOTAL_VOLUME;
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
                if (entity.REQUISITION_QTY <= 0) {
                    notificationservice.Notification("Requistion quantity must be greater then zero!", "", 'Requistion quantity must be greater then zero!!');
                    $scope.ClearEntity(entity)
                    return;
                }

                requisitionServices.LoadProductWeightData($scope.model.COMPANY_ID, entity.SKU_ID, entity.REQUISITION_QTY).then(function (data) {
                    var newRow = {
                        ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, MST_ID: $scope.gridOptionsList.data[0].MST_ID, DTL_ID: $scope.gridOptionsList.data[0].DTL_ID, SKU_ID: $scope.gridOptionsList.data[0].SKU_ID, SKU_CODE: $scope.gridOptionsList.data[0].SKU_CODE, STATUS: $scope.gridOptionsList.data[0].STATUS, REQUISITION_AMOUNT: $scope.gridOptionsList.data[0].REQUISITION_AMOUNT, SUGGESTED_QTY: $scope.gridOptionsList.data[0].SUGGESTED_QTY, REQUISITION_QTY: $scope.gridOptionsList.data[0].REQUISITION_QTY, STOCK_QTY: $scope.gridOptionsList.data[0].STOCK_QTY, UNIT_TP: $scope.gridOptionsList.data[0].UNIT_TP, STATUS: $scope.gridOptionsList.data[0].STATUS
                        , NO_OF_SHIPPER: $scope.gridOptionsList.data[0].NO_OF_SHIPPER, LOOSE_QTY: $scope.gridOptionsList.data[0].LOOSE_QTY, SHIPPER_WEIGHT: $scope.gridOptionsList.data[0].SHIPPER_WEIGHT, SHIPPER_VOLUME: $scope.gridOptionsList.data[0].SHIPPER_VOLUME, PER_PACK_WEIGHT: $scope.gridOptionsList.data[0].PER_PACK_WEIGHT, PER_PACK_VOLUME: $scope.gridOptionsList.data[0].PER_PACK_VOLUME, SHIPPER_QTY: $scope.gridOptionsList.data[0].SHIPPER_QTY, TOTAL_SHIPPER_WEIGHT: $scope.gridOptionsList.data[0].TOTAL_SHIPPER_WEIGHT
                        , TOTAL_SHIPPER_VOLUME: $scope.gridOptionsList.data[0].TOTAL_SHIPPER_VOLUME, LOOSE_WEIGHT: $scope.gridOptionsList.data[0].LOOSE_WEIGHT, LOOSE_VOLUME: $scope.gridOptionsList.data[0].LOOSE_VOLUME, TOTAL_WEIGHT: $scope.gridOptionsList.data[0].TOTAL_WEIGHT, TOTAL_VOLUME: $scope.gridOptionsList.data[0].TOTAL_VOLUME
                    }
                    $scope.gridOptionsList.data.push(newRow);
                    $scope.gridOptionsList.data[0] = $scope.GridDefalutData();

                    $scope.rowNumberGenerate();
                }, function (error) {
                });
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
            $scope.model.TOTAL_WEIGHT -= entity.TOTAL_WEIGHT;
            $scope.model.TOTAL_VOLUME -= entity.TOTAL_VOLUME;
            if ($scope.model.TOTAL_WEIGHT < 0) {
                $scope.model.TOTAL_WEIGHT = 0;
            }
            if ($scope.model.TOTAL_VOLUME < 0) {
                $scope.model.TOTAL_VOLUME = 0;
            }
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
                ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, MST_ID: entity.MST_ID, DTL_ID: entity.DTL_ID, REGION_ID: entity.REGION_ID, REGION_CODE: entity.REGION_CODE, SKU_ID: entity.SKU_ID, SKU_CODE: entity.SKU_CODE, STATUS: entity.STATUS, REQUISITION_AMOUNT: entity.REQUISITION_AMOUNT, REQUISITION_QTY: entity.REQUISITION_QTY, STOCK_QTY: entity.STOCK_QTY, UNIT_TP: entity.UNIT_TP, STATUS: entity.STATUS
            }
            $scope.gridOptionsList.data[0] = newRow;
        } else {
            notificationservice.Notification("Please Select Valid data", "", 'Only Single Row Left!!');
        }
        $scope.rowNumberGenerate();
        $scope.removeItem(entity);
    };

    $scope.LoadGroupData = function () {
        $scope.showLoader = true;

        requisitionServices.LoadGroupData().then(function (data) {
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
    $scope.LoadBrandData = function () {
        $scope.showLoader = true;

        requisitionServices.LoadBrandData().then(function (data) {
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
    $scope.LoadCategoryData = function () {
        $scope.showLoader = true;

        requisitionServices.LoadCategoryData().then(function (data) {
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
    $scope.LoadFormData = function () {
        $scope.showLoader = true;
        requisitionServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
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
        window.location = "/Inventory/Requisition/RequisitionRaise?Id=" + MST_ID_ENCRYPTED;
    }

    $scope.GetEditDataById = function (value) {
        if (value != undefined && value.length > 0) {
            requisitionServices.GetEditDataById(value).then(function (data) {

                if (data.data != null && data.data.requisitionDtlList != null && data.data.requisitionDtlList.length > 0) {
                    $scope.model.COMPANY_ID = data.data.COMPANY_ID;
                    $scope.model.MST_ID = data.data.MST_ID;
                    $scope.model.REQUISITION_DATE = data.data.REQUISITION_DATE;
                    $scope.model.ISSUE_UNIT_ID = data.data.ISSUE_UNIT_ID;
                    $scope.model.REQUISITION_RAISE_BY = data.data.REQUISITION_RAISE_BY;

                    $scope.model.REMARKS = data.data.REMARKS;
                    $scope.model.REQUISITION_AMOUNT = data.data.REQUISITION_AMOUNT;
                    $scope.model.TOTAL_VOLUME = data.data.TOTAL_VOLUME;
                    $scope.model.TOTAL_WEIGHT = data.data.TOTAL_WEIGHT;
                    $scope.model.STATUS = data.data.STATUS;
                    $scope.model.REQUISITION_NO = data.data.REQUISITION_NO;
                    if (data.data.requisitionDtlList != null) {
                        $scope.gridOptionsList.data = data.data.requisitionDtlList;
                    }
                    $scope.addDefaultRow($scope.GridDefalutData());
                }

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
        entity.STOCK_QTY = parseInt($scope.Products[searchIndex].STOCK_QTY);
        entity.SUGGESTED_QTY = parseInt($scope.Products[searchIndex].SUGGESTED_QTY);
        //entity.SKU_ID = $scope.Products[searchIndex].SKU_ID;

        requisitionServices.GetSkuPrice(entity.SKU_ID, entity.SKU_CODE)
            .then(function (priceData) {
                if (priceData.data < 0 || Object.is(priceData.data, NaN)) {
                    entity.UNIT_TP = 0;
                }
                else {
                    entity.UNIT_TP = priceData.data;
                }
            });

        $scope.LoadSKUCode();
    };

    $scope.typeaheadSelectedQty = function (entity) {
        //const searchIndex = $scope.Products.findIndex((x) => x.SKU_ID == entity.SKU_ID);
        //entity.SKU_CODE = $scope.Products[searchIndex].SKU_CODE;
        //entity.UNIT_TP = $scope.Products[searchIndex].UNIT_TP;
        if (entity.REQUISITION_QTY == '' || entity.REQUISITION_QTY == null || entity.REQUISITION_QTY < 0) {
            entity.REQUISITION_QTY = 0;
        }
        $scope.GetWeightVolumeCal(entity);
        entity.REQUISITION_AMOUNT = parseFloat(entity.UNIT_TP) * parseInt(entity.REQUISITION_QTY)
    };
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
                '<input type="text" disabled  style="text-align: left;"  ng-model="row.entity.SKU_CODE"  class="pl-sm" />'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: true, width: '8%', cellTemplate:
                '<input type="number" disabled style="text-align: right;"  ng-model="row.entity.UNIT_TP"  class="pl-sm" />'
        },
        {
            name: 'SUGGESTED_QTY', field: 'SUGGESTED_QTY', displayName: 'Suggested Qty', enableFiltering: true, width: '12%', cellTemplate:
                '<input type="text"  disabled style="text-align:right;" ng-model="row.entity.SUGGESTED_QTY"  class="pl-sm" />'
        }
        , {
            name: 'REQUISITION_QTY', field: 'REQUISITION_QTY', displayName: 'Req. Qty', enableFiltering: true, width: '9%', cellTemplate:
                '<input type="number" style="text-align: right;" min="0" ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.REQUISITION_QTY" class="pl-sm" />'
        },
        {
            name: 'STOCK_QTY', field: 'STOCK_QTY', displayName: 'Stock Qty', enableFiltering: true, width: '9%', cellTemplate:
                '<input type="text"  disabled style="text-align:right;" ng-model="row.entity.STOCK_QTY"  class="pl-sm" />'
        },
        {
            name: 'REQUISITION_AMOUNT', field: 'REQUISITION_AMOUNT', displayName: 'Requisition Amt', enableFiltering: true, width: '13%', cellTemplate:
                '<input type="number" style="text-align: right;" disabled  ng-model="row.entity.REQUISITION_AMOUNT"  class="pl-sm" />'
        }
        , {
            name: 'Action', displayName: 'Action', width: '10%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
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

    $scope.ClearForm = function () {
        window.location.href = "/Inventory/Requisition/RequisitionRaise";
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
    $scope.PrintRequisition = function () {
        var href = "/Inventory/Report/GenerateReport?ReportId=6" + "&UNIT_ID=" + $scope.model.REQUISITION_UNIT_ID + "&DATE_FROM=" + $scope.model.REQUISITION_DATE + "&DATE_TO=" + $scope.model.REQUISITION_DATE + "&MST_ID=" + $scope.model.MST_ID + "&REQUISITION_NO=" + $scope.model.REQUISITION_NO + "&REPORT_ID=6" + "&REPORT_EXTENSION=" + 'Pdf';
        window.open(href, '_blank');
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        requisitionServices.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);
            for (let i = 0; i < $scope.Companies.length; i++) {
                if (parseInt($scope.Companies[i].COMPANY_ID) == $scope.model.COMPANY_ID) {
                    $scope.model.COMPANY_NAME = $scope.Companies[i].COMPANY_NAME;
                }
            }
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;

        requisitionServices.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;

            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;
        });
    }

    $scope.LoadProductData = function () {
        $scope.showLoader = true;
        ;
        requisitionServices.LoadProductData().then(function (data) {
            $scope.Products = data.data;
            for (var i = 0; i < data.data.length; i++) {
                if (data.data[i].STOCK_QTY == '' || data.data[i].STOCK_QTY == undefined) {
                    data.data[i].STOCK_QTY = 0;
                }
                if (data.data[i].SUGGESTED_QTY == '' || data.data[i].SUGGESTED_QTY == undefined) {
                    data.data[i].SUGGESTED_QTY = 0;
                }
            }
            $scope.showLoader = false;
            var _Products = {
                PRODUCT_ID: "0",
                PRODUCT_NAME: "All",
                PRODUCT_CODE: "ALL",
                STOCK_QTY: 0,
                SUGGESTED_QTY: 0
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

    $scope.LoadSKUId = function () {
        setTimeout(function () {
            $('#SKU_CODE').trigger('change');
        }, 1000)
    }

    $scope.LoadSKUCode = function () {
        setTimeout(function () {
            $('#SKU_CODE').trigger('change');
        }, 1000)
    }
    $scope.LoadUNIT_ID = function () {
        $('#UNIT_ID').trigger('change');
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
    $scope.UnitLoad = function () {
        requisitionServices.GetUnit().then(function (data) {

            $scope.model.REQUISITION_UNIT_ID = parseInt(data.data);

            for (var i = 0; i < $scope.Unit.length; i++) {
                if ($scope.model.REQUISITION_UNIT_ID == $scope.Unit[i].UNIT_ID) {
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

        requisitionServices.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Unit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });
            $scope.Unit = [{ UNIT_ID: 0, UNIT_NAME: 'Select...' }, ...$scope.Unit];
            $scope.UnitLoad();
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

 
    $scope.SendNotification = function () {
        $scope.showLoader = true;

        LiveNotificationServices.Notification_Permitted_Users(3, $scope.model.COMPANY_ID, $scope.model.ISSUE_UNIT_ID).then(function (data) {
            $scope.showLoader = false;

            $scope.Users = data.data;
            $scope.Permitted_Users = [];
            if ($scope.Users != undefined && $scope.Users != null) {
                for (var i = 0; i < $scope.Users.length; i++) {
                    $scope.Permitted_Users.push(JSON.stringify(parseInt($scope.Users[i].USER_ID)));
                }
                connection.invoke("SendMessage", $scope.Permitted_Users, ": New Requisition has been raised and ready for issue.Check out the notification for detail!!!!").catch(function (err) {
                    return console.error(err.toString());
                });
            }
        }, function (error) {
            alert(error);

            $scope.showLoader = false;
        });
    }
    $scope.SaveData = function (model) {
        if ($scope.model.ISSUE_UNIT_ID == 0) {
            notificationservice.Notification('Issue unit not selected!', "", 'Issue unit not selected!');
            return;
        }

        $scope.showLoader = true;
        if ($scope.model.REQUISITION_AMOUNT == NaN) {
            $scope.model.REQUISITION_AMOUNT = 0
        }

        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);
        var reqList = [];
        for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
            if ($scope.gridOptionsList.data[i].REQUISITION_QTY > 0) {
                $scope.gridOptionsList.data[i].STOCK_QTY = parseInt($scope.gridOptionsList.data[i].STOCK_QTY);
                reqList.push($scope.gridOptionsList.data[i]);
            }
        }
        if (reqList.length <= 0) {
            $scope.showLoader = false;
            notificationservice.Notification("No data found at product detail", "", 'No data found at product detail!');
            return;
        }

        $scope.model.requisitionDtlList = reqList;
        if ($scope.model.requisitionDtlList != null) {
            if ($scope.model.requisitionDtlList.length > 1 && $scope.model.requisitionDtlList[0].SKU_CODE == "") {
                $scope.model.requisitionDtlList.splice(0, 1);
            }
        }
        if ($scope.model.requisitionDtlList.length == 1 && $scope.model.requisitionDtlList[0].SKU_CODE == "") {
            $scope.showLoader = false;
            notificationservice.Notification("No data has added on requisition detail!", "", 'No data has added on requisition detail!');
            return;
        }

        requisitionServices.AddOrUpdate(model).then(function (data) {
            //notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data.Status == 1) {
                $scope.showLoader = false;
                $scope.model.MST_ID_ENCRYPTED = data.data.Key;
                $scope.addDefaultRow($scope.GridDefalutData());
                $scope.SendNotification();

                notificationservice.Notification(1, 1, 'data save successfully!');
                setTimeout(function () {
                    $scope.LoadFormData();
                }, 1000)
            }
            else {
                $scope.showLoader = false;
                $scope.addDefaultRow($scope.GridDefalutData());
                notificationservice.Notification(data.data.Status, "", 'data not save successfully!');
            }
        });
    }

    $scope.LoadProductsDataFiltered = function () {
        $scope.showLoader = true;
        window.setTimeout(function () {
            $scope.gridOptionsList.data = [];
            requisitionServices.LoadProductsDataFiltered($scope.model.COMPANY_ID, $scope.model).then(function (data) {
                if (data.data.length > 0) {
                    for (var i = 0; i < data.data.length; i++) {
                        data.data[i].REQUISITION_QTY = 0;
                        data.data[i].REQUISITION_AMOUNT = 0;
                        data.data[i].SKU_ID = data.data[i].SKU_ID.toString();
                        if (data.data[i].STOCK_QTY == '' || data.data[i].STOCK_QTY == undefined) {
                            data.data[i].STOCK_QTY = 0;
                        }
                        if (data.data[i].UNIT_TP != '' && data.data[i].UNIT_TP != null && data.data[i].UNIT_TP != undefined) {
                            data.data[i].UNIT_TP = parseFloat(data.data[i].UNIT_TP);
                        }
                        else {
                            data.data[i].UNIT_TP = 0;
                        }
                    }

                    $scope.gridOptionsList.data = data.data;
                    $scope.addDefaultRow($scope.GridDefalutData());
                }
                else {
                    $scope.gridOptionsList = {
                        data: [$scope.GridDefalutData()]
                    }
                }
                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                console.log(error);
                $scope.showLoader = false;
            });
        }, 200)
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
    $scope.LoadProductData();
    $scope.LoadStatus();
    $scope.LoadCategoryData();
    $scope.LoadBrandData();
    $scope.LoadGroupData();
}]);