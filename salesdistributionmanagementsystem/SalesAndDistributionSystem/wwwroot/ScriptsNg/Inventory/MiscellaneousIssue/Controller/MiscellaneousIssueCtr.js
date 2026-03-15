ngApp.controller('ngGridCtrl', ['$scope', 'MiscellaneousIssueService', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, MiscellaneousIssueService, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'
    $scope.model = {
        COMPANY_ID: 0,
        MST_ID: 0,
        MST_ID_ENCRYPTED: "",
        UNIT_ID: 0,
        ISSUE_NO: ""
        , ISSUE_DATE: ""
        , SUBJECT: "Reprocess"
        , RAISED_FROM: "Production"
        , ISSUE_BY: ""
        , STATUS: "Active"
        , REMARKS: ""
        , MiscellaneousIssueDtlList: []
    }

    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 0,
            DTL_ID: 0,
            MST_ID: 0,
            COMPANY_ID: 0,
            SKU_ID: 0,
            SKU_CODE: '',

            PACK_SIZE: '',
            UNIT_TP: 0,
            STOCK_QTY: 0,
            ISSUE_QTY: 0,
            ISSUE_AMOUNT: 0,
            COMPANY_ID: 0,
            REMARKS: '',
            STATUS: 'Active'
        }
    }

    $scope.model.ISSUE_DATE = CurrentDate;
    $scope.Status = [];
    $scope.Products = [];
    $scope.Subject = [];
    $scope.RaisedFrom = [];
    $scope.MiscIssueList = [];

    $scope.LoadStatus = function () {
        var Active = {
            STATUS: 'Active'
        }
        var Inactive = {
            STATUS: 'Inactive'
        }
        $scope.Status.push(Active);
        $scope.Status.push(Inactive);
    }

    $scope.LoadRaisedFrom = function () {
        var Production = {
            RAISED_FROM: 'Production'
        }
        var Warehouse = {
            RAISED_FROM: 'Warehouse'
        }
        var QC = {
            RAISED_FROM: 'QC'
        }
        var Others = {
            RAISED_FROM: 'Others'
        }
        $scope.RaisedFrom.push(Production);
        $scope.RaisedFrom.push(Warehouse);
        $scope.RaisedFrom.push(QC);
        $scope.RaisedFrom.push(Others);
    }

    $scope.LoadSubject = function () {
        var Reprocess = {
            SUBJECT: 'Reprocess'
        }
        var Restamping_for_Price = {
            SUBJECT: 'Restamping for Price'
        }
        var Stability_Study = {
            SUBJECT: 'Stability Study'
        }
        var Export = {
            SUBJECT: 'Export'
        }
        var Physician_Sample = {
            SUBJECT: 'Physician Sample'
        }
        var Institutional = {
            SUBJECT: 'Institutional'
        }
        var Convert = {
            SUBJECT: 'Convert'
        }
        var Others = {
            SUBJECT: 'Others'
        }
        $scope.Subject.push(Reprocess);
        $scope.Subject.push(Restamping_for_Price);
        $scope.Subject.push(Stability_Study);
        $scope.Subject.push(Export);
        $scope.Subject.push(Physician_Sample);
        $scope.Subject.push(Institutional);
        $scope.Subject.push(Convert);
        $scope.Subject.push(Others);
    }

    $scope.CompanyNameLoad = function () {
        $scope.showLoader = true;
        MiscellaneousIssueService.GetCompanyName().then(function (data) {
            $scope.model.COMPANY_NAME = data.data;
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.UnitIdLoad = function () {
        $scope.showLoader = true;
        MiscellaneousIssueService.GetUnitId().then(function (data) {
            $scope.model.UNIT_ID = data.data;
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.UnitNameLoad = function () {
        $scope.showLoader = true;
        MiscellaneousIssueService.GetUnitName().then(function (data) {
            $scope.model.UNIT_NAME = data.data;
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Misc Issue Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.data = [];

    $scope.gridOptionsList = {
        data: [$scope.GridDefalutData()]
    }

    $scope.LoadSKU_ID = function () {
        //$('#SKU_ID').trigger('change');
    }

    $scope.LoadSKU_code = function () {
        $('#SKU_CODE').trigger('change');
    }

    $scope.changeProduct = function (entity) {
        const searchIndex = $scope.Products.findIndex((x) => x.SKU_ID == parseInt(entity.SKU_ID));
        if (searchIndex != -1) {
            entity.SKU_CODE = $scope.Products[searchIndex].SKU_CODE;
            entity.PACK_SIZE = $scope.Products[searchIndex].PACK_SIZE;
            entity.STOCK_QTY = $scope.Products[searchIndex].STOCK_QTY;
            //$scope.Products[searchIndex].UNIT_TP = parseFloat($scope.Products[searchIndex].UNIT_TP);

            MiscellaneousIssueService.GetUnitWiseSkuPrice(entity.SKU_ID, entity.SKU_CODE)
                .then(function (priceData) {
                    entity.UNIT_TP = priceData.data;
                }, error => {
                    console.log(error);
                });

            $scope.LoadSKU_ID();
            $scope.LoadSKU_code();
        }
    };

    $scope.LoadProductData = function () {
        $scope.showLoader = true;
        MiscellaneousIssueService.GetProducts().then(function (data) {
            $scope.Products_data = data.data;
            var _Products = {
                SKU_ID: 0,
                SKU_NAME: "",
                SKU_CODE: "",
                PACK_SIZE: ""
            }
            $scope.Products = [_Products, ...$scope.Products_data];

            //$scope.Products.push(_Products);
            //for (var i in $scope.Products_data) {
            //    $scope.Products.push($scope.Products_data[i]);
            //}

            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;
        });
    }

    $scope.gridOptionsList.columnDefs = [
        {
            name: 'ROW_NO', field: 'ROW_NO', enableFiltering: false, width: '50',
        },
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '25%', cellTemplate:
                '<select class="select2-single form-control" ng-disabled="row.entity.ROW_NO != 0" data-select2-id="{{row.entity.SKU_CODE}}" id="SKU_ID"' +
                'name="SKU_ID" ng-model="row.entity.SKU_ID" style="width:100%" ng-change="grid.appScope.changeProduct(row.entity)">' +
                '<option ng-repeat="item in grid.appScope.Products" ng-selected="item.SKU_ID == row.entity.SKU_ID" value="{{item.SKU_ID}}">{{ item.SKU_NAME }} | Code: {{ item.SKU_CODE }} | Pack Size: {{ item.PACK_SIZE }}</option>' +
                '</select>'
        },

        {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: false, enableColumnMenu: false, width: '10%', cellTemplate:
                '<input type="text" disabled  style="text-align: right;"  ng-model="row.entity.SKU_CODE"  class="pl-sm" />'
        },
        {
            name: 'PACK_SIZE', field: 'PACK_SIZE', displayName: 'Pack Size', enableFiltering: false, width: '10%', cellTemplate:
                '<input type="text" disabled  style="text-align: right;"  ng-model="row.entity.PACK_SIZE"  class="pl-sm" />'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: false, width: '10%', cellTemplate:
                '<input type="number" disabled  style="text-align: right;"  ng-model="row.entity.UNIT_TP"  class="pl-sm" />'
        },
        {
            name: 'STOCK_QTY', field: 'STOCK_QTY', displayName: 'Stock QTY', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number" disabled style="text-align: right;"   ng-model="row.entity.STOCK_QTY"  class="pl-sm" />'
        },
        {
            name: 'ISSUE_QTY', field: 'ISSUE_QTY', displayName: 'Issue QTY', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number" style="text-align: right;" min="0" ng-model="row.entity.ISSUE_QTY" ng-change="grid.appScope.changeIssueQty(row.entity)" class="pl-sm" />'
        },
        {
            name: 'ISSUE_AMOUNT', field: 'ISSUE_AMOUNT', displayName: 'Issue Amount', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number" style="text-align: right;" disabled  ng-model="row.entity.ISSUE_AMOUNT"  class="pl-sm" />'
        },
        {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                //'<button style="margin-bottom: 5px;" ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.EditItem(row.entity)" type="button" class="btn btn-outline-success mb-1"><img src="/img/edit-icon.png" height="15px" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO == 0" ng-click="grid.appScope.addNewRow(row.entity)" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                '</div>'
        }
    ];

    $scope.changeIssueQty = function (entity) {
        entity.ISSUE_AMOUNT = parseInt(entity.UNIT_TP) * parseInt(entity.ISSUE_QTY);
    };

    $scope.rowNumberGenerate = function () {
        $scope.model.ISSUE_AMOUNT = 0;
        for (let i = 0; i < $scope.gridOptionsList.data.length; i++) {
            $scope.gridOptionsList.data[i].ROW_NO = i;
            $scope.model.ISSUE_AMOUNT += ($scope.gridOptionsList.data[i].ISSUE_AMOUNT)
        }
    }

    $scope.addNewRow = (entity) => {
        var count = 0;
        if ($scope.gridOptionsList.data.length > 0 && $scope.gridOptionsList.data[0].SKU_CODE != null && $scope.gridOptionsList.data[0].SKU_CODE != '' && $scope.gridOptionsList.data[0].SKU_CODE != 'undefined') {
            for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
                if ($scope.gridOptionsList.data[i].SKU_CODE == entity.SKU_CODE) {
                    count++;
                }
            }

            if (count == 1 || count == 0 || entity.SKU_CODE == "") {
                if (entity.ISSUE_QTY <= 0) {
                    notificationservice.Notification("Issue quantity must be greater then zero!", "", 'Requistion quantity must be greater then zero!!');
                    $scope.ClearEntity(entity)
                    return;
                }
                var newRow = {
                    ROW_NO: 0, COMPANY_ID: $scope.model.COMPANY_ID,
                    MST_ID: $scope.gridOptionsList.data[0].MST_ID,
                    DTL_ID: $scope.gridOptionsList.data[0].DTL_ID,
                    SKU_ID: $scope.gridOptionsList.data[0].SKU_ID,
                    SKU_CODE: $scope.gridOptionsList.data[0].SKU_CODE,
                    PACK_SIZE: $scope.gridOptionsList.data[0].PACK_SIZE,
                    STATUS: $scope.gridOptionsList.data[0].STATUS,
                    ISSUE_AMOUNT: $scope.gridOptionsList.data[0].ISSUE_AMOUNT,
                    ISSUE_QTY: $scope.gridOptionsList.data[0].ISSUE_QTY,
                    STOCK_QTY: $scope.gridOptionsList.data[0].STOCK_QTY,
                    UNIT_TP: $scope.gridOptionsList.data[0].UNIT_TP,
                    STATUS: $scope.gridOptionsList.data[0].STATUS
                }
                $scope.gridOptionsList.data.push(newRow);
                $scope.gridOptionsList.data[0] = $scope.GridDefalutData();

                $scope.rowNumberGenerate();
            }
            else {
                notificationservice.Notification("Product already exist!", "", 'Product already exist!');
                $scope.ClearEntity(entity)
            }
        }
        else {
            notificationservice.Notification("No item has added!", "", 'No item has added!');
            $scope.ClearEntity(entity)
        }
    };

    $scope.addDefaultRow = (entity) => {
        var newRow = {
            ROW_NO: 0, COMPANY_ID: $scope.model.COMPANY_ID, MST_ID: $scope.gridOptionsList.data[0].MST_ID, DTL_ID: $scope.gridOptionsList.data[0].DTL_ID, SKU_ID: $scope.gridOptionsList.data[0].SKU_ID, SKU_CODE: $scope.gridOptionsList.data[0].SKU_CODE, PACK_SIZE: $scope.gridOptionsList.data[0].PACK_SIZE, STATUS: $scope.gridOptionsList.data[0].STATUS, ISSUE_AMOUNT: $scope.gridOptionsList.data[0].ISSUE_AMOUNT, ISSUE_QTY: $scope.gridOptionsList.data[0].ISSUE_QTY, STOCK_QTY: $scope.gridOptionsList.data[0].STOCK_QTY, UNIT_TP: $scope.gridOptionsList.data[0].UNIT_TP, STATUS: $scope.gridOptionsList.data[0].STATUS
        }
        $scope.gridOptionsList.data.push(newRow);
        $scope.gridOptionsList.data[0] = $scope.GridDefalutData();
        $scope.rowNumberGenerate();

        $interval(function () {
            $scope.LoadSKU_ID();
        }, 800, 2);
    }

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

    $scope.LoadProductData();
    $scope.CompanyNameLoad();
    $scope.LoadStatus();
    $scope.LoadSubject();
    $scope.LoadRaisedFrom();

    $scope.UnitIdLoad();
    $scope.UnitNameLoad();

    $scope.ClearEntity = function (entity) {
        entity.ROW_NO = 0,
            entity.DTL_ID = 0,
            entity.MST_ID = 0,
            entity.COMPANY_ID = 0,
            entity.SKU_ID = 0,
            entity.SKU_CODE = '',
            entity.STOCK_QTY = 0,
            entity.UNIT_TP = 0,
            entity.ISSUE_QTY = 0,
            entity.ISSUE_AMOUNT = 0,
            entity.STATUS = 'Active'
    };

    $scope.SaveData = function (model) {
        if ($scope.model.UNIT_ID == "") {
            notificationservice.Notification('Issue unit not selected!', "", 'Issue unit not selected!');
            return;
        }

        $scope.showLoader = true;
        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);
        $scope.model.UNIT_ID = parseInt($scope.model.UNIT_ID);

        $scope.model.MiscellaneousIssueDtlList = $scope.gridOptionsList.data;
        if ($scope.model.MiscellaneousIssueDtlList != null) {
            if ($scope.model.MiscellaneousIssueDtlList.length > 1 && $scope.model.MiscellaneousIssueDtlList[0].SKU_CODE == "") {
                $scope.model.MiscellaneousIssueDtlList.splice(0, 1);
            }
        }
        if ($scope.model.MiscellaneousIssueDtlList.length == 1
            && $scope.model.MiscellaneousIssueDtlList[0].SKU_CODE == ""
            || $scope.model.MiscellaneousIssueDtlList[0].ISSUE_QTY < 1
        ) {
            $scope.showLoader = false;
            notificationservice.Notification("No data/Quantity has added on requisition detail!", "", 'No data has added on requisition detail!');
            return;
        }

        for (var i = 0; i < $scope.model.MiscellaneousIssueDtlList.length; i++) {
            $scope.model.MiscellaneousIssueDtlList[i].SKU_ID = parseInt($scope.model.MiscellaneousIssueDtlList[i].SKU_ID);
            $scope.model.MiscellaneousIssueDtlList[i].ISSUE_AMOUNT = parseInt($scope.model.MiscellaneousIssueDtlList[i].ISSUE_AMOUNT);
        }
        if (window.confirm('Are you sure to save the current issue?')) {
            MiscellaneousIssueService.AddOrUpdate(model).then(function (data) {
                //notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
                if (parseInt(data.data) > 0) {
                    $scope.showLoader = false;
                    $scope.model.MST_ID = data.data;
                    notificationservice.Notification(1, 1, 'data save successfully!');

                    $scope.LoadFormData();
                }
                else {
                    $scope.showLoader = false;

                    notificationservice.Notification('data not save successfully!', "", 'data not save successfully!');
                }
            });
        } else {
            $scope.showLoader = false;

            notificationservice.Notification('Insertion Cancelled!', 1, 'Insertion Cancelled!');
            $scope.addNewRow($scope.gridOptionsList.data[0]);
        }
    }

    $scope.EditItem = (entity) => {
        if ($scope.gridOptionsList.data.length > 0) {
            var newRow = {
                ROW_NO: 0,
                COMPANY_ID: $scope.model.COMPANY_ID,
                MST_ID: $scope.gridOptionsList.data[0].MST_ID,
                DTL_ID: $scope.gridOptionsList.data[0].DTL_ID,
                SKU_ID: $scope.gridOptionsList.data[0].SKU_ID,
                SKU_CODE: $scope.gridOptionsList.data[0].SKU_CODE,
                PACK_SIZE: $scope.gridOptionsList.data[0].PACK_SIZE,
                STATUS: $scope.gridOptionsList.data[0].STATUS,
                ISSUE_AMOUNT: $scope.gridOptionsList.data[0].ISSUE_AMOUNT,
                ISSUE_QTY: $scope.gridOptionsList.data[0].ISSUE_QTY,
                STOCK_QTY: $scope.gridOptionsList.data[0].STOCK_QTY,
                UNIT_TP: $scope.gridOptionsList.data[0].UNIT_TP,
                STATUS: $scope.gridOptionsList.data[0].STATUS
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
        MiscellaneousIssueService.LoadData($scope.model.COMPANY_ID).then(function (data) {
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
        window.location = "/Inventory/MiscellaneousIssue/MiscellaneousIssue?Id=" + MST_ID_ENCRYPTED;
    }

    $scope.GetEditDataById = function (value) {
        if (value != undefined && value.length > 0) {
            MiscellaneousIssueService.GetEditDataById(value).then(function (data) {
                if (data.data != null && data.data.MiscellaneousIssueDtlList != null && data.data.MiscellaneousIssueDtlList.length > 0) {
                    $scope.model.COMPANY_ID = data.data.COMPANY_ID;
                    $scope.model.UNIT_ID = data.data.UNIT_ID;
                    $scope.model.MST_ID = data.data.MST_ID;
                    $scope.model.ISSUE_NO = data.data.ISSUE_NO;
                    $scope.model.ISSUE_DATE = data.data.ISSUE_DATE;
                    $scope.model.ISSUE_BY = data.data.ISSUE_BY;
                    $scope.model.Remarks = data.data.REMARKS;
                    $scope.model.STATUS = data.data.STATUS;
                    $scope.model.SUBJECT = data.data.SUBJECT;
                    $scope.model.RAISED_FROM = data.data.RAISED_FROM;
                    if (data.data.MiscellaneousIssueDtlList != null) {
                        $scope.gridOptionsList.data = [];
                        data.data.MiscellaneousIssueDtlList?.forEach(e => e.SKU_ID = e.SKU_ID.toString());
                        $scope.gridOptionsList.data = [$scope.GridDefalutData(), ...data.data.MiscellaneousIssueDtlList];
                    }
                }
                $scope.showLoader = false;
            }, function (error) {
                alert(error);
            });
        }
    }

    $scope.ClearForm = function () {
        window.location.href = "/Inventory/MiscellaneousIssue/MiscellaneousIssue";
    }
}]);