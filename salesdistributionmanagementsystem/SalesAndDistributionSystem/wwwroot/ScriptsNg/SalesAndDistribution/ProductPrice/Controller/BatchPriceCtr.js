ngApp.controller('ngGridCtrl', ['$scope', 'batchPriceServices', 'LiveNotificationServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, batchPriceServices, LiveNotificationServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

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
    $scope.flag = false;
    $scope.getPermissions = [];
    $scope.CustomerData = [];
    $scope.Products = [];
    $scope.Status = [];
    $scope.SaveButton = 'Save';
    $scope.model = {
        COMPANY_ID: 0, UNIT_ID: '', MST_ID: 0, BATCH_PRICE_DTL_LIST: []
    }
    $scope.model.ENTRY_DATE = $filter("date")(Date.now(), 'dd/MM/yyyy');


    //#region ------------user_company_information----------------------**
    $scope.Companies = [];
    $scope.CompanyUnit = [];
    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;
        batchPriceServices.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;
        batchPriceServices.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseFloat(data.data);
            $interval(function () {
                $scope.LoadCOMPANY_ID();
            }, 800, 2);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }
    $scope.LoadCompanyUnitData = function () {
        $scope.showLoader = true;
        batchPriceServices.LoadCompanyUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.CompanyUnit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });
            $scope.CompanyUnit.unshift({ UNIT_ID: 'ALL', UNIT_NAME: '---ALL---' })
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.CompanyUnitLoad = function () {
        $scope.showLoader = true;
        batchPriceServices.GetUnitId().then(function (data) {
            $scope.model.UNIT_ID = parseInt(data.data);
            $interval(function () {

                $scope.LoadUNIT_ID();
            }, 800, 2);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }
    $scope.LoadUNIT_ID = function () {
        $('#UNIT_ID').trigger('change');
    }
    $scope.LoadCOMPANY_ID = function () {
        $('#COMPANY_ID').trigger('change');
    }
    //#endregion
    //#region ------------product load and manipulation_functionality ------------**
    $scope.LoadProductData = function () {
        $scope.showLoader = true;
        batchPriceServices.LoadProductData().then(function (data) {
            $scope.Products = data.data;
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }

    $scope.typeaheadSelectedSku = function () {
        let searchIndex = $scope.Products.findIndex((x) => x.SKU_ID == $scope.model.SKU_ID);
        $scope.model.SKU_CODE = $scope.Products[searchIndex].SKU_CODE;
        $scope.model.PACK_SIZE = $scope.Products[searchIndex].PACK_SIZE;
        $scope.LoadSKUId();
    };
    $scope.loadBatchWiseStock = function (model) {
        if (model.SKU_ID == undefined || model.SKU_ID == 0 || model.SKU_ID == null || model.SKU_ID == '') {
            alert('Please Select SKU ')
            return;
        }
        if (model.UNIT_ID_MULTIPLE == undefined || model.UNIT_ID_MULTIPLE.length <= 0) {
            alert('Please Select Unit/Depot')
            return;
        }
        batchPriceServices.loadBatchWiseStock(model).then(function (data) {
            $scope.gridOptions.data = data.data;
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }
    $scope.DataLoad = function (value) {
        $scope.showLoader = true;
        if (value == 'Search') {
            document.getElementById("BatchPriceList").style.display = "block";
            document.getElementById("MarketSalesCreate").style.display = "none";
            document.getElementById("SaveData").style.display = "none";
            document.getElementById("Create").style.display = "block";
            let companyId = $scope.model.COMPANY_ID
            batchPriceServices.LoadData(companyId).then(function (data) {
                $scope.gridOptionsList.data = data.data;
                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                $scope.showLoader = false;

            });
        } else {
            document.getElementById("BatchPriceList").style.display = "none";
            document.getElementById("MarketSalesCreate").style.display = "block";
            document.getElementById("SaveData").style.display = "block";
            document.getElementById("Create").style.display = "none";
            $scope.showLoader = false;

        }

    }
    //#endregion
    //#region ------------ gridOptions Set Up ------------
    $scope.gridOptions = (gridregistrationservice.GridRegistration("In Market Sales"));
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptions.data = [];
    $scope.gridOptions = {
        data: []
    }
    $scope.gridOptions.columnDefs = [

        { name: '#', field: 'ROW_NO', enableFiltering: false, width: '50', displayName: '#', cellTemplate: '<span>{{rowRenderIndex+1}}</span>' },

        {
            name: 'UNIT_ID', field: 'UNIT_ID', enableColumnMenus: true, visible: false, displayName: 'UNIT', enableFiltering: false, width: '20%', cellTemplate:
                '<input type="text" disabled  style="text-align: left;"  ng-model="row.entity.UNIT_ID"  class="pl-sm" />'
        },

        {
            name: 'UNIT_NAME', field: 'UNIT_NAME', enableColumnMenus: true, displayName: 'Unit Name', enableFiltering: false, width: '20%', cellTemplate:
                '<input type="text" disabled  style="text-align: left;"  ng-model="row.entity.UNIT_NAME"  class="pl-sm" />'
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', enableColumnMenus: false, displayName: 'Batch No', enableFiltering: false, width: '20%', cellTemplate:
                '<input type="text"  disabled style="text-align: left;"  ng-model="row.entity.BATCH_NO"  class="pl-sm" />'
        }, {
            name: 'BATCH_ID', field: 'BATCH_ID', enableColumnMenus: false, displayName: 'Batch Id', enableFiltering: false, width: '20%', visible: false, cellTemplate:
                '<input type="text"  disabled style="text-align: left;"  ng-model="row.entity.BATCH_ID"  class="pl-sm" />'
        },
        {
            name: 'MRP', field: 'MRP', enableColumnMenus: false, displayName: 'MRP', enableFiltering: false, width: '20%', cellTemplate:
                '<input type="number"   style="text-align: left;"  ng-model="row.entity.MRP"  class="pl-sm" />'
        }

        , {
            name: 'UNIT_TP', field: 'UNIT_TP', enableColumnMenus: false, displayName: 'Unit TP', enableFiltering: false, width: '20%', cellTemplate:
                '<input type="number"  style="text-align: right;"  ng-model="row.entity.UNIT_TP"  class="pl-sm" />'
        }

    ];
    $scope.gridOptions.showColumnFooter = true;
    //#endregion
    $scope.rowNumberGenerate = function () {
        for (let i = 0; i < $scope.gridOptions.data.length; i++) {
            $scope.gridOptions.data[i].ROW_NO = i;
        }
    }

    $scope.GetEditDataById = function (value) {
        if (value != undefined && value.length > 0 && value != '0') {
            batchPriceServices.GetEditDataById(value).then(function (data) {
                if (data.data != null && data.data.BATCH_PRICE_DTL_LIST != null && data.data.BATCH_PRICE_DTL_LIST.length > 0) {
                    $scope.model.UNIT_ID = data.data.UNIT_ID;
                    $scope.model.MST_ID = data.data.MST_ID;
                    $scope.model.SKU_CODE = data.data.SKU_CODE;
                    $scope.model.SKU_ID = data.data.SKU_ID;
                    $scope.model.ENTRY_DATE = data.data.ENTRY_DATE;
                    let searchIndex = $scope.Products.findIndex((x) => x.SKU_ID == $scope.model.SKU_ID);
                    $scope.model.PACK_SIZE = $scope.Products[searchIndex].PACK_SIZE;
                    for (var i = 0; i < data.data.BATCH_PRICE_DTL_LIST.length; i++) {
                        $scope.gridOptions.data.push(data.data.BATCH_PRICE_DTL_LIST[i]);
                    }
                    $scope.SaveButton = 'Update';
                    document.getElementById("SaveButton").setAttribute('disabled', '');
                }
                $scope.rowNumberGenerate();
                $interval(function () {
                    $scope.LoadSKU_ID();
                }, 800, 2);

                $scope.showLoader = false;
            }, function (error) {
                alert(error);

            });
        }
    }
    $scope.ClearForm = function () {
        window.location.href = "/SalesAndDistribution/ProductPrice/BatchPrice";
    }
    //#region ------------ dropdown trigger ------------
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
    $scope.LoadSKU_ID = function () {
        $('#SKU_ID').trigger('change');
    }
    //#endregion

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'ProductPrice',
            Action_Name: 'BatchPrice'
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
    $scope.SaveData = function (model) {
        $scope.showLoader = true;
        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);
        $scope.model.BATCH_PRICE_DTL_LIST = $scope.gridOptions.data;

        batchPriceServices.AddOrUpdate(model).then(function (data) {
            if (data.data == '1') {
                $scope.showLoader = false;
                $scope.model.MST_ID_ENCRYPTED = data.data.Key;
                notificationservice.Notification(1, 1, 'Data save successfully!');
                setTimeout(function () {
                    $scope.gridOptions.data = [];
                    $scope.model.ENTRY_DATE = '';
                    $scope.model.SKU_ID = '';
                    $scope.model.SKU_CODE = '';
                    $scope.DataLoad('Search');
                    $scope.SaveButton = 'Save';
                    window.history.pushState("create", "Title", "/SalesAndDistribution/ProductPrice/BatchPrice");
                }, 1000)
            }
            else {
                $scope.showLoader = false;
                notificationservice.Notification(data.data, "", 'data not save successfully!');
            }
        });
    }
    $scope.LoadProductData();
    $scope.LoadStatus();
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
    $scope.LoadCompanyUnitData();
    $scope.GetPermissionData();
    $scope.CompanyUnitLoad();

    //---Batch Price List grid configuration---
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("In Sales"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        { name: '#', field: 'ROW_NO', enableFiltering: false, width: '50', displayName: 'Row Number', cellTemplate: '<span>{{rowRenderIndex+1}}</span>' },

        , { name: 'MST_ID', field: 'MST_ID', visible: false }

        , {
            name: 'ENTRY_DATE', field: 'ENTRY_DATE', displayName: 'Entry Date', enableFiltering: true, width: '15%'
        }
        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '10%'
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '20%'
        }
        , {
            name: 'ENTERED_DATE', field: 'ENTERED_DATE', displayName: 'Entered Date', enableFiltering: true, width: '15%'

        }
        , {
            name: 'UPDATED_DATE', field: 'UPDATED_DATE', displayName: 'Updated Date', enableFiltering: true, width: '15%'

        }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">View</button>' +
                '</div>'
        }

    ];
    $scope.gridOptionsList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"
    $scope.EditData = function (entity) {
        window.location = "/SalesAndDistribution/ProductPrice/BatchPrice?Id=" + entity.MST_ID;
    }
}]);

