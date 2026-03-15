ngApp.controller('ngGridCtrl', ['$scope', 'CustomerTargetService', 'permissionProvider', 'gridregistrationservice', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, CustomerTargetService, permissionProvider, gridregistrationservice, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'

    $scope.showloader = false;
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
    var year = new Date().getFullYear();
    $scope.years = [0, 1, 2, 3, 4, 5].map(e => e + year)

    $scope.model = { MONTH_CODE: '00', YEAR: year }
    $scope.products = [];

    $scope.months = [{
        Id: '00',
        Name: 'MONTH'
    }, {
        Id: '01',
        Name: 'JANUARY'
    }, {
        Id: '02',
        Name: 'FEBRUARY'
    }, {
        Id: '03',
        Name: 'MARCH'
    }, {
        Id: '04',
        Name: 'APRIL'
    }, {
        Id: '05',
        Name: 'MAY'
    }, {
        Id: '06',
        Name: 'JUNE'
    }, {
        Id: '07',
        Name: 'JULY'
    }, {
        Id: '08',
        Name: 'AUGUST'
    }, {
        Id: '09',
        Name: 'SEPTEMBER'
    }, {
        Id: '10',
        Name: 'OCTOBER'
    }, {
        Id: '11',
        Name: 'NOVEMBER'
    }, {
        Id: '12',
        Name: 'DECEMBER'
    }];

    $scope.TriggerSelects = function () {
        setTimeout(function () {
            $("#YEAR").trigger("change");
        }, 500)
    }
    $scope.TriggerSelects();

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Invoice"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , {
            name: 'CUSTOMER_CODE', field: 'CUSTOMER_CODE', displayName: 'Customer Code', enableFiltering: true, enableColumnMenu: false
        }
        , {
            name: 'CUSTOMER_NAME', field: 'CUSTOMER_NAME', displayName: 'Customer Name', enableFiltering: true, enableColumnMenu: false
        }
        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, enableColumnMenu: false,width: '90'
        }
        , {
            name: 'SKU_ID', field: 'SKU_ID', displayName: 'SKU Name', enableFiltering: true, width: '25%', enableColumnMenu: false,
            cellTemplate:
                `<select class="select2-single sku_Id form-control" id="SKU_ID" 
                'name="SKU_ID" ng-model="row.entity.SKU_ID" style="width:100%" ng-change="grid.appScope.OnProductSelection(row.entity)" >'
                '<option ng-repeat="item in grid.appScope.products" ng-selected="item.SKU_ID == row.entity.SKU_ID" value="{{item.SKU_ID}}">{{ item.SKU_NAME }} | Code: {{ item.SKU_CODE }} | Pack Size: {{ item.PACK_SIZE }} </option>'
                '</select>'`
        }
        , {
            name: 'PACK_SIZE', field: 'PACK_SIZE', displayName: 'Pack Size', enableFiltering: true, enableColumnMenu: false
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: true, enableColumnMenu: false
        }, {
            name: 'MRP', field: 'MRP', displayName: 'MRP', enableFiltering: true, enableColumnMenu: false
        }
        , {
            name: 'PREVIOUS_TARGET_QTY', field: 'PREVIOUS_TARGET_QTY', displayName: 'Previous Target Qty', enableFiltering: true, enableColumnMenu: false, visible: true
        }
        , {
            name: 'TARGET_QTY', field: 'TARGET_QTY', displayName: 'Target Qty', enableFiltering: false, enableColumnMenu: true, cellTemplate: '<input type="number"  style="text-align: left;"  ng-model="row.entity.TARGET_QTY" min="0" class="pl-sm" ng-change="grid.appScope.CalculateValue(row.entity)" />'
        }
        , {
            name: 'TARGET_VALUE', field: 'TARGET_VALUE', displayName: 'Target Value', enableFiltering: true, enableColumnMenu: false
        }
        , {
            name: 'NET_VALUE', field: 'NET_VALUE', displayName: 'Net Value', enableFiltering: true, enableColumnMenu: false
        }
        , {
            name: 'Action', displayName: 'Action', enableFiltering: true, enableColumnMenu: true, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO == 0" ng-click="grid.appScope.addNewRow(row.entity)" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +

                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity, grid.appScope.invoiceGridList.data)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +
                '</div>'
        }
    ]


    //$scope.gridOptionsList.enableGridMenu = true;
    $scope.DefaultData = () => {
        return {
            ROW_NO: 0,
            SKU_ID: '',
            SKU_NAME: '',
            SKU_CODE: '',
            UNIT_TP: '',
            PACK_SIZE: '',
            TARGET_QTY: ''
        }
    }

    $scope.gridOptionsList.data = [$scope.DefaultData()];

    $scope.OnProductSelection = function (entity) {
        if (entity != null) {
            var product = $scope.products.find(e => e.SKU_ID == entity.SKU_ID);
            entity.SKU_CODE = product.SKU_CODE;
            entity.UNIT_TP = product.UNIT_TP;
            entity.PACK_SIZE = product.PACK_SIZE;
            entity.MRP = product.MRP;
        }
    }

    $scope.ClearForm = () => {
        window.location = "/Target/CustomerTarget/TargetProcess";
    }

    $scope.CalculateValue = function (entity) {
        if (!isNaN(entity.TARGET_QTY)) {
            entity.TARGET_VALUE = entity.TARGET_QTY * entity.UNIT_TP;
            entity.NET_VALUE = entity.TARGET_VALUE - (entity.TARGET_VALUE * isNaN(entity.DISCOUNT_VALUE) ? 0 : entity.DISCOUNT_VALUE / 100);
        }
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

    $scope.rowNumberGenerate = function () {
        $scope.model.REQUISITION_AMOUNT = 0;
        for (let i = 0; i < $scope.gridOptionsList.data.length; i++) {
            $scope.gridOptionsList.data[i].ROW_NO = i;
            $scope.model.REQUISITION_AMOUNT += ($scope.gridOptionsList.data[i].REQUISITION_AMOUNT)
        }
    }

    $scope.addNewRow = function (entity) {
        var count = 0;
        if ($scope.gridOptionsList.data.length > 0 && $scope.gridOptionsList.data[0].SKU_CODE != null && $scope.gridOptionsList.data[0].SKU_CODE != '' && $scope.gridOptionsList.data[0].SKU_CODE != 'undefined') {
            for (var i = 1; i < $scope.gridOptionsList.data.length; i++) {
                if ($scope.gridOptionsList.data[i].SKU_CODE == entity.SKU_CODE) {
                    count++;
                    break;
                }
            }

            if (count > 0) {
                notificationservice.Notification("SKU Name already exist!", "", 'SKU Name already exist!');
                return 0;
            }
            else {
                var newRow = {
                    ...$scope.gridOptionsList.data[0],
                    COMPANY_ID: $scope.model.COMPANY_ID,
                    ROW_NO: 1
                }
                $scope.gridOptionsList.data.push(newRow);
                $scope.gridOptionsList.data[0] = $scope.DefaultData();
                $scope.rowNumberGenerate();
            }
        }
    }

    $scope.SaveData = function (model) {
        $scope.showLoader = true;

        model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);

        model.TARGET_DTLs = $scope.gridOptionsList.data;
        if (model.TARGET_DTLs != null) {
            if (model.TARGET_DTLs.length > 1 && model.TARGET_DTLs[0].SKU_CODE == "") {
                model.TARGET_DTLs.splice(0, 1);
            }
        }
        if (model.TARGET_DTLs.length == 1 && model.TARGET_DTLs[0].SKU_CODE == "") {

            $scope.showLoader = false;
            notificationservice.Notification("No data has added on target detail!", "", 'No data has added on target detail!');
            return;
        }
        if (model.YEAR == null && model.YEAR == '') {
            $scope.showLoader = false;
            notificationservice.Notification("Select Year", "", 'Enter Year');
            return;
        }

        if (model.MONTH_CODE == "00" || model.MONTH_CODE == null) {
            $scope.showLoader = false;
            notificationservice.Notification("Select Month", "", 'Enter Month');
            return;
        }


        CustomerTargetService.SaveFromExcel(model).then(function (data) {
            if (data.data?.status == 1) {
                $scope.model.MST_ID_ENCRYPTED = data.data?.key;
                //$scope.addDefaultRow($scope.GridDefalutData());
                $scope.gridOptionsList.data = [$scope.DefaultData(), ...$scope.gridOptionsList.data];
                $scope.showLoader = false;
                notificationservice.Notification(1, 1, 'data save successfully!');

            }
            else {
                //$scope.addDefaultRow($scope.GridDefalutData());
                $scope.showLoader = false;
                notificationservice.Notification(data.data.status, "", 'data not save successfully!');
            }
        });
    }

    $scope.UploadFile = function () {
        var file = document.getElementById("ATTACHMENT")?.files[0];
        $scope.showLoader = true;
        CustomerTargetService.ProcessExcel(file).then(response => {
            if (response.data.Status == "0") {
                $scope.gridOptionsList.data = [$scope.DefaultData(), ...JSON.parse(response.data.Key)];
            }
            else {
                notificationservice.Notification(response.data.Key, "", 'Enter Year');
            }
            $scope.showLoader = false;
        })
    }


    $scope.LoadProducts = function () {
        $scope.showLoader = true;
        CustomerTargetService.GetProducts().then(function (data) {
            $scope.products = data.data;
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;
        })
    }

    $scope.LoadProducts();

    $scope.CompanyLoad = function () {
        $scope.showLoader = true;
        CustomerTargetService.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseFloat(data.data);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.CompanyLoad();

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'CustomerTarget',
            Action_Name: 'TargetProcess'
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

    $scope.GetPermissionData();

}])