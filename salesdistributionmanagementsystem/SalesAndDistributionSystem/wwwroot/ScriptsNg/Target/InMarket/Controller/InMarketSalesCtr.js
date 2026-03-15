ngApp.controller('ngGridCtrl', ['$scope', 'inmarketServices', 'LiveNotificationServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, inmarketServices, LiveNotificationServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

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
    $scope.ProductList = [];
    $scope.CustomerData = [];
    $scope.ProductList = [];
    $scope.Products = [];
    $scope.existingSKU = [];
    $scope.Products = [];
    $scope.Status = [];
    $scope.SaveButton = 'Save';
    $scope.model = {
        COMPANY_ID: 0
        , UNIT_ID: 0
        , MST_ID: 0
        , MARKET_ID: '0'
        , MARKET_CODE: ''
        , DELIVERY_DATE: ''
        , YEAR: ''
        , MST_ID_ENCRYPTED: ''
        , IN_MARKET_SALES_DTL: []
    }
    $scope.months = [{
        MONTH_CODE: '00',
        Name: '---SELECT MONTH---'
    }, {
        MONTH_CODE: '01',
        Name: 'JANUARY'
    }, {
        MONTH_CODE: '02',
        Name: 'FEBRUARY'
    }, {
        MONTH_CODE: '03',
        Name: 'MARCH'
    }, {
        MONTH_CODE: '04',
        Name: 'APRIL'
    }, {
        MONTH_CODE: '05',
        Name: 'MAY'
    }, {
        MONTH_CODE: '06',
        Name: 'JUNE'
    }, {
        MONTH_CODE: '07',
        Name: 'JULY'
    }, {
        MONTH_CODE: '08',
        Name: 'AUGUST'
    }, {
        MONTH_CODE: '09',
        Name: 'SEPTEMBER'
    }, {
        MONTH_CODE: '10',
        Name: 'OCTOBER'
    }, {
        MONTH_CODE: '11',
        Name: 'NOVEMBER'
    }, {
        MONTH_CODE: '12',
        Name: 'DECEMBER'
    }];
    $scope.years = [{
        Id: '',
        Name: 'YEAR'
    }];
    $scope.yearValidation = function (year) {
        if (year == undefined || year == null) {
            year = '';
        }
        var text = /^[0-9]+$/;
        if (year.length == 4) {
            if (year != 0) {
                if ((year != "") && (!text.test(year))) {
                    alert("Please Enter Numeric Values Only for year");
                    return false;
                }
                var current_year = new Date().getFullYear();
                if (year < current_year) {
                    alert("Year should be in range current year to Future");
                    return false;
                }
                return true;
            } else {
                alert("Year is not proper. Please check");
                return false;
            }
        }
        else {
            alert("Year is not proper. Please check");
            return false;
        }
    }

    //#region ------------user_company_information------------
    $scope.Companies = [];
    $scope.CompanyUnit = [];
    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;
        inmarketServices.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;
        inmarketServices.GetCompany().then(function (data) {
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
        inmarketServices.LoadCompanyUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.CompanyUnit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });
            $scope.CompanyUnit.unshift({ UNIT_ID: '', UNIT_NAME: 'All' })
            $scope.model.UNIT_ID = '';
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.CompanyUnitLoad = function () {
        $scope.showLoader = true;
        inmarketServices.GetUnitId().then(function (data) {
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

    //#region  ------------market load and manipulation functionality ------------
    $scope.Markets = [];
    $scope.LoadMarketDropDownDataData = function () {
        $scope.showLoader = true;
        inmarketServices.LoadMarketDropDownDataData().then(function (data) {
            $scope.Markets = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }
    $scope.typeaheadSelectedMarket = function () {
        const searchIndex = $scope.Markets.findIndex((x) => x.MARKET_ID == $scope.model.MARKET_ID);
        $scope.model.MARKET_CODE = $scope.Markets[searchIndex].MARKET_CODE;
    };
    //#endregion

    //#region ------------product load and manipulation_functionality ------------
    $scope.LoadProductData = function () {
        $scope.showLoader = true;
        inmarketServices.LoadProductData().then(function (data) {
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
    $scope.typeaheadSelectedSku = function (entity) {
        const searchIndex = $scope.Products.findIndex((x) => x.SKU_ID == entity.SKU_ID);
        entity.SKU_CODE = $scope.Products[searchIndex].SKU_CODE;
        entity.MRP = $scope.Products[searchIndex].MRP;
        entity.PACK_SIZE = $scope.Products[searchIndex].PACK_SIZE;
        $scope.Products[searchIndex].UNIT_TP = parseFloat($scope.Products[searchIndex].UNIT_TP);
        if ($scope.Products[searchIndex].UNIT_TP < 0 || Object.is($scope.Products[searchIndex].UNIT_TP, NaN)) {
            entity.UNIT_TP = 0;
        }
        else {
            entity.UNIT_TP = $scope.Products[searchIndex].UNIT_TP;
        }
        $scope.LoadSKUId();
    };
    $scope.typeaheadSelectedQty = function (entity) {
        entity.SALES_VALUE = Math.round(parseFloat(entity.UNIT_TP) * parseFloat(entity.SALES_QTY) * 100) / 100;
        $scope.rowNumberGenerate();
    
    };
    //#endregion

    //#region ------------Grid fileread ------------

    $scope.GridXlxsDefalutData = function () {
        return {
            ROW_NO: 0,
            ENTRY_DATE: '',
            YEAR: '',
            MONTH_CODE: '',
            MARKET_CODE: '',
            SKU_CODE: '',
            SKU_ID: '',
            PACK_SIZE: '',
            MRP: '',
            UNIT_TP: '',
            SALES_QTY: '',
            SALES_VALUE: ''
        }
    }
    $scope.process = function () {
        try {
            var sku_error = '';
            var market_error = '';
            let data = $scope.gridOptionsxlsx.data;
            $scope.gridOptionsxlsx.data = [];
            for (let i = 0; i < data.length; i++) {
                var result = $scope.Products.find((x) => x.SKU_CODE == data[i].SKU_CODE);
                var market = $scope.Markets.find((x) => x.MARKET_CODE == data[i].MARKET_CODE);
                if (result != undefined || result != null) {
                    if (market != undefined || market != null) {
                        $scope.gridOptionsxlsx.data.push({
                            //ROW_NO: parseInt(data[i].ROW_NO),
                            ENTRY_DATE: data[i].ENTRY_DATE,
                            YEAR: data[i].YEAR,
                            MONTH_CODE: data[i].MONTH_CODE,
                            MARKET_CODE: data[i].MARKET_CODE,
                            MARKET_ID: market.MARKET_ID.toString(),
                            SKU_CODE: data[i].SKU_CODE,
                            SKU_ID: result.SKU_ID.toString(),
                            PACK_SIZE: result.PACK_SIZE,
                            MRP: result.MRP == '' || result.MRP == null ? 0.0 : result.MRP,
                            UNIT_TP: result.UNIT_TP == '' || result.UNIT_TP == null ? 0.0 : result.UNIT_TP,
                            SALES_QTY: parseFloat(data[i].SALES_QTY),
                            //SALES_VALUE: parseFloat(data[i].SALES_QTY) * parseFloat(result.UNIT_TP)
                            SALES_VALUE: Math.round(parseFloat(result.UNIT_TP == '' || result.UNIT_TP == null ? 0.0 : result.UNIT_TP) * parseFloat(data[i].SALES_QTY) * 100) / 100

                        });
                    } else {
                        
                        market_error = market_error.concat(',', (i + 1).toString());
                        $scope.gridOptionsxlsx.data = [];
                    }
                } else {
                    
                    sku_error = sku_error.concat(',', (i + 1).toString());
                    $scope.gridOptionsxlsx.data = [];
                    
                }

                if ( market_error != '') {
                    alert('market Problem in (' + market_error + ' ) line')
                    $scope.gridOptionsxlsx.data = [];
                }
                
                if (sku_error != '') {
                    alert('Sku Problem in (' + sku_error + ' ) line')
                    $scope.gridOptionsxlsx.data = [];
                }
                if (!$scope.yearValidation(data[i].YEAR)) {
                    $scope.gridOptionsxlsx.data = [];
                    document.getElementById("hide").style.display = "block";
                    return;
                }               
            }
            for (let i = 0; i < $scope.gridOptionsxlsx.data.length; i++) {
                $scope.gridOptionsxlsx.data[i].ROW_NO = i + 1;
            }
            document.getElementById("hide").style.display = "none";
        }
        catch (error) {
            alert("Here is the error message\n" + error);
            $scope.gridOptionsxlsx.data = [];
            document.getElementById("hide").style.display = "block";

        }
    }
    $scope.gridOptionsxlsx = (gridregistrationservice.GridRegistration("In Market Sales xlx"));
    $scope.gridOptionsxlsx.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsxlsx.data = [];
    $scope.SaveDataXlsx = function (model) {
        $scope.showLoader = true;
        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);
        $scope.model.MST_ID=0;
        $scope.model.MARKET_ID= "0";
        $scope.model.MARKET_CODE= "";
        $scope.model.DELIVERY_DATE= "";
        $scope.model.YEAR= "";
        $scope.model.IN_MARKET_SALES_DTL = $scope.gridOptionsxlsx.data;
        if ($scope.model.IN_MARKET_SALES_DTL != null && $scope.model.IN_MARKET_SALES_DTL.length >= 1) {
            inmarketServices.AddOrUpdateXlsx(model).then(function (data) {
                if (data.data.Status == 1) {
                    $scope.showLoader = false;
                    $scope.model.MST_ID_ENCRYPTED = data.data.Key;
                    notificationservice.Notification(1, 1, 'Data save successfully!');
                    setTimeout(function () {
                        $scope.gridOptions.data = [];
                        $scope.gridOptionsxlsx.data = [];
                        $scope.gridOptions = {
                            data: [$scope.GridDefalutData()]
                        }
                        $scope.DataLoad('Search');
                        $scope.SaveButton = 'Save';
                        $scope.model.MONTH_CODE = null;
                        $scope.model.MARKET_ID = null;
                        $scope.model.MARKET_CODE = null;
                        $scope.model.MARKET_YEAR = null;
                        $scope.model.MST_ID = null;
                        window.history.pushState("create", "Title", "/Target/InMarketSales/Index");

                    }, 1000)



                }
                else {
                    $scope.showLoader = false;
                    notificationservice.Notification(data.data.Status, "", 'data not save successfully!');
                }
            });
        } else {
            $scope.showLoader = false;
            notificationservice.Notification("No data has added on  detail!", "", 'No data has added on  detail!');

        }


    }
    $scope.ExportXlsx = function (value) {
        var href = "/Target/InMarketSales/Export?MONTH_CODE=" + $scope.model.MONTH_CODE + "&MARKET_CODE=" + $scope.model.MARKET_CODE + "&YEAR=" + $scope.model.YEAR + "&ENTRY_DATE=" + $scope.model.ENTRY_DATE;
        window.open(href, '_blank');
    }

    $scope.reset = function () {
        $scope.gridOptionsxlsx.data = [];
        document.getElementById("hide").style.display = "block";
    }
    //#endregion

    //#region ------------gridOptions  operation ------------
    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 0,
            DTL_ID: '',
            MST_ID: 0,
            COMPANY_ID: 0,
            SKU_ID: "null",
            SKU_CODE: '',
            PACK_SIZE: '',
            UNIT_TP: 0,
            SALES_QTY: 0,
            SALES_VALUE: 0,
            STATUS: 'Active',
            REMARKS: ''
        }
    }
    $scope.addDefaultRow = (entity) => {
        var newRow = {
            ROW_NO: 1, DTL_ID: $scope.gridOptions.data[0].DTL_ID, SKU_ID: $scope.gridOptions.data[0].SKU_ID
            , SKU_CODE: $scope.gridOptions.data[0].SKU_CODE, PACK_SIZE: $scope.gridOptions.data[0].PACK_SIZE
            , UNIT_TP: $scope.gridOptions.data[0].UNIT_TP, SALES_QTY: $scope.gridOptions.data[0].SALES_QTY
            , SALES_VALUE: $scope.gridOptions.data[0].SALES_VALUE, MRP: $scope.gridOptions.data[0].MRP

        }
        $scope.gridOptions.data.push(newRow);
        $scope.gridOptions.data[0] = $scope.GridDefalutData();
        $scope.rowNumberGenerate();
    }
    $scope.ClearEntity = function (entity) {
        entity.ROW_NO = 0,
            entity.DTL_ID = '0',
            entity.MST_ID = 0,
            entity.COMPANY_ID = 0,
            entity.SKU_ID = "null",
            entity.SKU_CODE = '',
            entity.PACK_SIZE = 0,
            entity.UNIT_TP = 0,
            entity.SALES_QTY = 0,
            entity.SALES_VALUE = 'Active',
            entity.STATUS = 0,
            entity.REMARKS = ''

    };
    //#endregion

    //#region ------------ gridOptions Set Up ------------
    $scope.gridOptions = (gridregistrationservice.GridRegistration("In Market Sales"));
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptions.data = [];
    $scope.gridOptions = {
        data: [$scope.GridDefalutData()]
    }
    $scope.gridOptions.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        },

        {
            name: 'SKU_CODE', field: 'SKU_CODE', enableColumnMenus: true, displayName: 'SKU Code', enableFiltering: false, width: '10%', cellTemplate:
                '<input type="text" disabled  style="text-align: left;"  ng-model="row.entity.SKU_CODE"  class="pl-sm" />'
        }
        , {
            name: 'SKU_ID', field: 'SKU_ID', enableColumnMenus: false, displayName: 'SKU Name', enableFiltering: true, width: '27%', cellTemplate:
                '<select class="select2-single form-control" ng-disabled="row.entity.ROW_NO != 0" data-select2-id="{{row.entity.SKU_ID}}" id="SKU_ID"' +
                'name="SKU_ID" ng-model="row.entity.SKU_ID" style="width:100%" ng-change="grid.appScope.typeaheadSelectedSku(row.entity)">' +
                '<option ng-repeat="item in grid.appScope.Products" ng-selected="item.SKU_ID == row.entity.SKU_ID" value="{{item.SKU_ID}}">{{ item.SKU_NAME }} | Code: {{ item.SKU_CODE }}</option>' +
                '</select>'
        }
        , {
            name: 'PACK_SIZE', field: 'PACK_SIZE', enableColumnMenus: false, displayName: 'PACK SIZE', enableFiltering: false, width: '10%', cellTemplate:
                '<input type="text" disabled  style="text-align: left;"  ng-model="row.entity.PACK_SIZE"  class="pl-sm" />'
        }
        ,
        {
            name: 'MRP', field: 'MRP', enableColumnMenus: false, displayName: 'MRP', enableFiltering: false, width: '8%', cellTemplate:
                '<input type="text" disabled  style="text-align: left;"  ng-model="row.entity.MRP"  class="pl-sm" />'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', enableColumnMenus: false, displayName: 'Unit TP', enableFiltering: false, width: '8%', cellTemplate:
                '<input type="number" disabled style="text-align: right;"  ng-model="row.entity.UNIT_TP"  class="pl-sm" />'
        }
        , {
            name: 'SALES_QTY', field: 'SALES_QTY', enableColumnMenus: false, displayName: 'SALES QTY', enableFiltering: false, width: '11%', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #9ed4a0;color: White">Total:</div>', cellTemplate:
                '<input type="number" style="text-align: right;" min="0" ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.SALES_QTY" class="pl-sm" />'
        },


        {
            name: 'SALES_VALUE', field: 'SALES_VALUE', enableColumnMenus: false, displayName: 'SALES VALUE', enableFiltering: false, width: '13%', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #9ed4a0;color: White">{{grid.appScope.model.SALES_VALUE}}</div>', cellTemplate:
                '<input type="number" style="text-align: right;" disabled  ng-model="row.entity.SALES_VALUE"  class="pl-sm" />'
        }
        , {
            name: 'Action', displayName: 'Action', width: '11%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.EditItem(row.entity)" type="button" class="btn btn-outline-success mb-1"><img src="/img/edit-icon.png" height="15px" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO == 0" ng-click="grid.appScope.addNewRow(row.entity)" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                '</div>'
        },
        {
            name: 'MST_ID', field: 'MST_ID', enableFiltering: false, width: '50', visible: false
        }, {
            name: 'DTL_ID', field: 'DTL_ID', enableFiltering: false, width: '50', visible: false
        }

    ];
    $scope.gridOptions.showColumnFooter = true;
    //#endregion

    $scope.rowNumberGenerate = function () {
        $scope.model.SALES_VALUE = 0;
        for (let i = 0; i < $scope.gridOptions.data.length; i++) {
            $scope.gridOptions.data[i].ROW_NO = i;
            $scope.model.SALES_VALUE += ($scope.gridOptions.data[i].SALES_VALUE);
            $scope.gridOptions.data[i].DTL_ID = $scope.gridOptions.data[i].DTL_ID.toString();
        }
    }
    $scope.EditItem = (entity) => {
        if ($scope.gridOptions.data.length > 0) {
            var newRow = {
                ROW_NO: 1, DTL_ID: entity.DTL_ID, COMPANY_ID: $scope.model.COMPANY_ID, SKU_ID: entity.SKU_ID, SKU_CODE: entity.SKU_CODE, PACK_SIZE: entity.PACK_SIZE, SALES_VALUE: entity.SALES_VALUE, SALES_QTY: entity.SALES_QTY, UNIT_TP: entity.UNIT_TP, MRP: entity.MRP
            }
            $scope.gridOptions.data[0] = newRow;
        } else {
            notificationservice.Notification("Please Select Valid data", "", 'Only Single Row Left!!');
        }
        $scope.rowNumberGenerate();
        $scope.removeItem(entity);
    };
    $scope.addNewRow = (entity) => {
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
                if (entity.SALES_QTY <= 0) {
                    notificationservice.Notification("SALES quantity must be greater then zero!", "", 'SALES quantity must be greater then zero!!');
                    //$scope.ClearEntity(entity)
                    return;
                }
                var newRow = {
                    ROW_NO: 1, DTL_ID: $scope.gridOptions.data[0].DTL_ID, SKU_ID: $scope.gridOptions.data[0].SKU_ID, SKU_CODE: $scope.gridOptions.data[0].SKU_CODE, PACK_SIZE: $scope.gridOptions.data[0].PACK_SIZE, MRP: $scope.gridOptions.data[0].MRP, SALES_VALUE: $scope.gridOptions.data[0].SALES_VALUE, SALES_QTY: $scope.gridOptions.data[0].SALES_QTY, UNIT_TP: $scope.gridOptions.data[0].UNIT_TP
                }
                $scope.gridOptions.data.push(newRow);
                $scope.gridOptions.data[0] = $scope.GridDefalutData();

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
    $scope.GetEditDataById = function (value) {
        if (value != undefined && value.length > 0) {
            inmarketServices.GetEditDataById(value).then(function (data) {
                if (data.data != null && data.data.IN_MARKET_SALES_DTL != null && data.data.IN_MARKET_SALES_DTL.length > 0) {
                    $scope.model.UNIT_ID = data.data.UNIT_ID;
                    $scope.model.MST_ID = data.data.MST_ID;
                    $scope.model.ENTRY_DATE = data.data.ENTRY_DATE;
                    $scope.model.YEAR = data.data.YEAR;
                    $scope.model.MONTH_CODE = data.data.MONTH_CODE;
                    $scope.model.MARKET_ID = data.data.MARKET_ID;
                    $scope.model.MARKET_CODE = data.data.MARKET_CODE;
                    for (var i = 0; i < data.data.IN_MARKET_SALES_DTL.length; i++) {
                        $scope.gridOptions.data.push(data.data.IN_MARKET_SALES_DTL[i]);
                    }
                    $scope.SaveButton = 'Update';
                }
                $scope.rowNumberGenerate();
                $scope.showLoader = false;
            }, function (error) {
                alert(error);

            });
        }
    }
    $scope.removeItem = function (entity) {
        if ($scope.gridOptions.data.length > 1) {
            var index = $scope.gridOptions.data.indexOf(entity);
            if ($scope.gridOptions.data.length > 0) {
                $scope.gridOptions.data.splice(index, 1);
            }
            $scope.rowNumberGenerate();
        } else {
            notificationservice.Notification("Only Single Row Left!!", "", 'Only Single Row Left!!');
        }


    }
    $scope.ClearForm = function () {
        window.location.href = "/Target/InMarketSales/Index";
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
        $('.sku_Id').trigger('change');
    }
    $scope.LoadMONTH_CODE = function () {
        $('.MONTH_CODE').trigger('change');

    }
    //#endregion

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
    $scope.SaveData = function (model) {
        if ($scope.model.UNIT_ID == "") {
            notificationservice.Notification('Unit not selected!', "", 'Unit not selected!');
            return;
        }
        if ($scope.model.MARKET_ID == "") {
            notificationservice.Notification('Market not selected!', "", 'Market not selected!');
            return;
        }
        if ($scope.model.MONTH_CODE == undefined || $scope.model.MONTH_CODE == "" || $scope.model.MONTH_CODE == '00') {
            notificationservice.Notification('Month not selected!', "", 'Month not selected!');
            return;
        }
        if ($scope.model.ENTRY_DATE == undefined || $scope.model.ENTRY_DATE == "" || $scope.model.ENTRY_DATE == null) {
            notificationservice.Notification('Date not selected!', "", 'Date not selected!');
            return;
        }

        if (!$scope.yearValidation($scope.model.YEAR)) {
            return;
        }

        $scope.showLoader = true;
        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);
        $scope.model.IN_MARKET_SALES_DTL = $scope.gridOptions.data;
        if ($scope.model.IN_MARKET_SALES_DTL != null) {
            if ($scope.model.IN_MARKET_SALES_DTL.length > 1 && $scope.model.IN_MARKET_SALES_DTL[0].SKU_CODE == '') {
                $scope.model.IN_MARKET_SALES_DTL.splice(0, 1);
            }
        }
        if ($scope.model.IN_MARKET_SALES_DTL.length == 1 && $scope.model.IN_MARKET_SALES_DTL[0].SKU_CODE == "") {
            $scope.showLoader = false;
            notificationservice.Notification("No data has added on  detail!", "", 'No data has added on  detail!');
            return;
        }

        inmarketServices.AddOrUpdate(model).then(function (data) {
            if (data.data.Status == 1) {
                $scope.showLoader = false;
                $scope.model.MST_ID_ENCRYPTED = data.data.Key;
                notificationservice.Notification(1, 1, 'Data save successfully!');
                setTimeout(function () {
                    $scope.gridOptions.data = [];
                    $scope.gridOptions = {
                        data: [$scope.GridDefalutData()]
                    }
                    $scope.DataLoad('Search');
                    $scope.SaveButton = 'Save';
                    $scope.model.MONTH_CODE = null;
                    $scope.model.MARKET_ID = null;
                    $scope.model.MARKET_CODE = null;
                    $scope.model.MARKET_YEAR = null;
                    $scope.model.MST_ID = null;
                    window.history.pushState("create", "Title", "/Target/InMarketSales/Index");
                }, 1000)
            }
            else {
                $scope.showLoader = false;
                $scope.addDefaultRow($scope.GridDefalutData());
                notificationservice.Notification(data.data.Status, "", 'data not save successfully!');
            }
        });
    }
    $scope.DataLoad = function (value) {
        $scope.showLoader = true;
        if (value == 'Search') {
            document.getElementById("MarketSalesList").style.display = "block";
            document.getElementById("MarketSalesCreate").style.display = "none";
            document.getElementById("SaveData").style.display = "none";
            document.getElementById("Create").style.display = "block";
            let companyId = $scope.model.COMPANY_ID
            inmarketServices.LoadData(companyId).then(function (data) {
                $scope.gridOptionsList.data = data.data;
                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                $scope.showLoader = false;

            });
        } else {
            document.getElementById("MarketSalesList").style.display = "none";
            document.getElementById("MarketSalesCreate").style.display = "block";
            document.getElementById("SaveData").style.display = "block";
            document.getElementById("Create").style.display = "none";
            $scope.showLoader = false;

        }

    }
    $scope.SetAllAddComm3 = function () {
        let value = document.getElementById("MODEL_COL_FIELD3").value;
        for (var i = 0; i < $scope.gridOptionsxlsx.data.length; i++) {
            $scope.gridOptionsxlsx.data[i].SALES_QTY = parseFloat(value);
        }

    }

    $scope.LoadProductData();
    $scope.LoadStatus();
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
    $scope.LoadCompanyUnitData();
    $scope.GetPermissionData();
    $scope.CompanyUnitLoad();
    $scope.LoadMarketDropDownDataData();

    //---In Market Sales List grid configuration---
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("In Sales"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: 'ROW_NO', field: 'ROW_NO', displayName: '#', enableFiltering: false, width: '50'
        }

        , { name: 'MST_ID', field: 'MST_ID', visible: false }

        , {
            name: 'ENTRY_DATE', field: 'ENTRY_DATE', displayName: 'Entry Date', enableFiltering: true, width: '10%'
        }
        , {
            name: 'YEAR', field: 'YEAR', displayName: 'Year', enableFiltering: true, width: '10%'
        }
        , {
            name: 'MONTH_CODE', field: 'MONTH_CODE', displayName: 'Month Code', enableFiltering: true, width: '10%'
        }, {
            name: 'MONTH_NAME', field: 'MONTH_NAME', displayName: 'Month Name', enableFiltering: true, width: '10%'
        }
        , {
            name: 'MARKET_CODE', field: 'MARKET_CODE', displayName: 'Market Code', enableFiltering: true, width: '10%'

        }, {
            name: 'MARKET_NAME', field: 'MARKET_NAME', displayName: 'Market Name', enableFiltering: true, width: '20%'

        }
        , {
            name: 'ENTERED_DATE', field: 'ENTERED_DATE', displayName: 'Entered Date', enableFiltering: true, width: '15%'

        }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        }

    ];
    $scope.gridOptionsList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"
    $scope.EditData = function (entity) {
        window.location = "/Target/InMarketSales/Index?Id=" + entity.MST_ID_ENCRYPTED;
    }
}]);

ngApp.directive("fileread", [function () {
    return {
        scope: {
            opts: '='
        },
        link: function ($scope, $elm, $attrs) {

            $elm.on('change', function (changeEvent) {
                var reader = new FileReader();
                reader.onload = function (evt) {
                    $scope.$apply(function () {
                        var data = evt.target.result;
                        var workbook = XLSX.read(data, { type: 'binary' });
                        var headerNames = XLSX.utils.sheet_to_json(workbook.Sheets[workbook.SheetNames[0]], { header: 1 })[0];
                        var data = XLSX.utils.sheet_to_json(workbook.Sheets[workbook.SheetNames[0]]);
                        $scope.opts.columnDefs = [
                            {
                                name: 'ROW_NO', displayName: 'ROW_NO', field: 'ROW_NO', enableFiltering: false, width: '50'
                            },
                            {
                                name: 'ENTRY_DATE', field: 'ENTRY_DATE', displayName: 'Entry Date', enableFiltering: true, width: '5%'
                            }
                            , {
                                name: 'YEAR', field: 'YEAR', displayName: 'YEAR', enableFiltering: true, width: '5%'
                            }
                            , {
                                name: 'MONTH_CODE', field: 'MONTH_CODE', displayName: 'Month Code', enableFiltering: true, width: '5%'
                            }
                            , {
                                name: 'MARKET_CODE', field: 'MARKET_CODE', displayName: 'Market Code', enableFiltering: true, width: '5%'

                            },
                            {
                                name: 'SKU_CODE', field: 'SKU_CODE', enableColumnMenus: true, displayName: 'SKU Code', enableFiltering: false, width: '8%', cellTemplate:
                                    '<input type="text" disabled  style="text-align: left;"  ng-model="row.entity.SKU_CODE"  class="pl-sm" />'
                            }
                            , {
                                name: 'SKU_ID', field: 'SKU_ID', enableColumnMenus: false, displayName: 'SKU Name', enableFiltering: true, width: '20%', cellTemplate:
                                    '<select class="select2-single form-control" ng-disabled="row.entity.ROW_NO != 0" data-select2-id="{{row.entity.SKU_ID}}" id="SKU_ID"' +
                                    'name="SKU_ID" ng-model="row.entity.SKU_ID" style="width:100%" ng-change="grid.appScope.typeaheadSelectedSku(row.entity)">' +
                                    '<option ng-repeat="item in grid.appScope.Products" ng-selected="item.SKU_ID == row.entity.SKU_ID" value="{{item.SKU_ID}}">{{ item.SKU_NAME }} | Code: {{ item.SKU_CODE }}</option>' +
                                    '</select>'
                            }
                            , {
                                name: 'PACK_SIZE', field: 'PACK_SIZE', enableColumnMenus: false, displayName: 'PACK SIZE', enableFiltering: false, width: '5%', cellTemplate:
                                    '<input type="text" disabled  style="text-align: left;"  ng-model="row.entity.PACK_SIZE"  class="pl-sm" />'
                            }
                            ,
                            {
                                name: 'MRP', field: 'MRP', enableColumnMenus: false, displayName: 'MRP', enableFiltering: false, width: '5%', cellTemplate:
                                    '<input type="text" disabled  style="text-align: left;"  ng-model="row.entity.MRP"  class="pl-sm" />'
                            }
                            , {
                                name: 'UNIT_TP', field: 'UNIT_TP', enableColumnMenus: false, displayName: 'Unit TP', enableFiltering: false, width: '5%', cellTemplate:
                                    '<input type="number" disabled style="text-align: right;"  ng-model="row.entity.UNIT_TP"  class="pl-sm" />'
                            }
                            , {
                                name: 'SALES_QTY', field: 'SALES_QTY', enableColumnMenus: false, displayName: 'SALES QTY', enableFiltering: false, width: '10%', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #9ed4a0;color: White">Total:</div>', headerCellTemplate: '<div class="">SALES QTY<br></div><input id="MODEL_COL_FIELD3" ng-model="MODEL_COL_FIELD3" ng-change="grid.appScope.SetAllAddComm3()"  type="text" class="form-control" autocomplete="off" >', cellTemplate:
                                    '<input type="number" style="text-align: right;" min="0" ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"  ng-model="row.entity.SALES_QTY" class="pl-sm" />'
                            },
                            {
                                name: 'SALES_VALUE', field: 'SALES_VALUE', enableColumnMenus: false, displayName: 'SALES VALUE', enableFiltering: false, width: '13%', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #9ed4a0;color: White">{{grid.appScope.model.SALES_VALUE}}</div>', cellTemplate:
                                    '<input type="number" style="text-align: right;" disabled  ng-model="row.entity.SALES_VALUE"  class="pl-sm" />'
                            }
                            , {
                                name: 'Action', displayName: 'Action', width: '11%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                                    '<div style="margin:1px;">' +
                                    '<button style="margin-bottom: 5px;" ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.EditItem(row.entity)" type="button" class="btn btn-outline-success mb-1"><img src="/img/edit-icon.png" height="15px" ></button>' +
                                    '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO == 0" ng-click="grid.appScope.addNewRow(row.entity)" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                                    '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                                    '</div>'
                            }

                        ];
                        $scope.opts.data = data;
                        $elm.val(null);
                    });
                };
                reader.readAsBinaryString(changeEvent.target.files[0]);
            });

        }

    }
}]);