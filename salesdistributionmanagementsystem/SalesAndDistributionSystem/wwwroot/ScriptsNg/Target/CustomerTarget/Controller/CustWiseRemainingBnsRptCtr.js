ngApp.controller('ngGridCtrl', ['$scope', 'CustomerTargetService', 'permissionProvider', 'gridregistrationservice', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', 'uiGridConstants', function ($scope, CustomerTargetService, permissionProvider, gridregistrationservice, notificationservice, $http, $log, $filter, $timeout, $interval, $q, uiGridConstants) {
    'use strict'
    $scope.model = {};
    $scope.showloader = false;
    $scope.model.DATE_FROM = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.model.DATE_TO = $filter("date")(Date.now(), 'dd/MM/yyyy');
    var year = new Date().getFullYear();
    $scope.years = [-1,0, 1, 2, 3, 4, 5].map(e => e + year)
    $scope.months = [
    {
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
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Delivery List"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridOptionsList.gridApi = gridApi;
    }
    $scope.gridOptionsList.showColumnFooter = true;
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#',
            field: 'ROW_NO',
            enableFiltering: false,
            enableSorting: false,
            cellTemplate: '<div class="ui-grid-cell-contents">{{grid.renderContainers.body.visibleRowCache.indexOf(row) + 1}}</div>',
            width: '40'
        },
        { name: 'INVOICE_UNIT_ID', displayName: 'Invoice Unit Id', visible: false },
        { name: 'INVOICE_NO', displayName: 'Invoice No' },
        {
            name: 'INVOICE_DATE',
            displayName: 'Invoice Date',
            cellTemplate: '<div>{{grid.getCellValue(row, col) | date:"dd/MM/yyyy"}}</div>',
        },
        { name: 'CUSTOMER_CODE', displayName: 'Customer Code' },
        { name: 'CUSTOMER_NAME', displayName: 'Customer Name' },
        { name: 'SKU_ID', displayName: 'SKU Id', visible: false },
        { name: 'SKU_CODE', displayName: 'SKU Code' },
        { name: 'SKU_NAME', displayName: 'SKU Name' },
        { name: 'PACK_SIZE', displayName: 'Pack Size' },
        { name: 'ISSUED_QTY', displayName: 'Issued Qty', cellClass: 'text-right' },
        { name: 'BONUS_QTY', displayName: 'Bonus Qty', cellClass: 'text-right' },
        { name: 'DECLARE_BONUS_QTY', displayName: 'Decl. Bonus Qty', cellClass: 'text-right' },
        { name: 'SLAB_QTY', displayName: 'Slab Qty', cellClass: 'text-right' },
        { name: 'REMAINING_QTY', displayName: 'Rem. Qty', cellClass: 'text-right' },
        {
            name: 'UNIT_TP',
            displayName: 'Unit TP',
            cellClass: 'text-right',

        },
        {
            name: 'REMAINING_VALUE', displayName: 'Rem. Value',
            cellClass: 'text-right',
            aggregationType: uiGridConstants.aggregationTypes.sum,
            footerCellTemplate: '<div class="text-right">{{ col.getAggregationValue() | number:2 }}</div>' // Format to 2 decimal places

        }
    ]
    $scope.gridOptionsList.data = [];
    //$scope.gridOptionsList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"
    $scope.gridOptionsList.enableColumnMenus = false;

    $scope.gridCollection = (gridregistrationservice.GridRegistration("Collection List"));
    $scope.gridCollection.onRegisterApi = function (gridApi) {
        $scope.gridCollectionApi = gridApi;
    }
    $scope.gridCollection.showColumnFooter = true;
    $scope.gridCollection.columnDefs = [
        {
            name: '#',
            field: 'ROW_NO',
            enableFiltering: false,
            enableSorting: false,
            cellTemplate: '<div class="ui-grid-cell-contents">{{grid.renderContainers.body.visibleRowCache.indexOf(row) + 1}}</div>',
            width: '40'
        }
        , {
            name: 'NAME_WITH_CODE', field: 'NAME_WITH_CODE', pinnedLeft: true, displayName: 'Customer', enableFiltering: false, width: '22%', cellTemplate:
                '<div class="typeaheadcontainer">' +
                '<input type="text" autocomplete="off" style="width:100%;" class="form-control" name="ROLE_NAME"' +
                ' ng-model="row.entity.NAME_WITH_CODE"' +
                ' ng-blur="grid.appScope.typeaheadBlurCustomer(row.entity)"' +
                ' uib-typeahead="Customer.CUSTOMER_NAME as Customer.CUSTOMER_NAME_CODE for Customer in grid.appScope.AutoCompleteDataLoadForCustomer($viewValue)"' +
                ' typeahead-append-to-body="true"' +
                ' placeholder="Customer Name Min. Two Character"' +
                ' typeahead-editable="false"' +
                ' typeahead-on-select="grid.appScope.typeaheadSelectedCustomer(row.entity, $item)" />' +
                '</div>' 
        },
        { name: 'CUSTOMER_CODE', field: 'CUSTOMER_CODE', displayName: 'Code', enableFiltering: false, width: '5%' }

        , {
            name: 'EFFECT_START_DATE', field: 'EFFECT_START_DATE', displayName: 'Start', enableFiltering: false, width: '20%', cellTemplate:
                '<div class="simple-date2">'
                + '<div class="input-group date">'
                + '<div class="input-group-prepend">'
                + '</div>'
                + '<input  type="text"  datepicker class="form-control"   ng-model="row.entity.EFFECT_START_DATE" placeholder="dd/mm/yyyy" id="EFFECT_START_DATE">'
                + '</div>'
                + '</div>'
        }
        , {
            name: 'EFFECT_END_DATE', field: 'EFFECT_END_DATE', displayName: 'End', enableFiltering: false, width: '20%', cellTemplate:
                '<div class="simple-date2">'
                + '<div class="input-group date">'
                + '<div class="input-group-prepend">'
                + '</div>'
                + '<input  type="text"  datepicker class="form-control"  ng-model="row.entity.EFFECT_END_DATE" placeholder="dd/mm/yyyy"  id="EFFECT_END_DATE">'
                + '</div>'
                + '</div>'
        },
        {
            name: 'AMOUNT',
            field: 'AMOUNT',
            displayName: 'Amount',
            headerCellClass: 'text-center',
            aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true,
            footerCellTemplate: '<div class="ui-grid-cell-contents text-right">{{col.getAggregationValue() | number:2}}</div>',
            enableFiltering: false,
            type: 'number',
            width: '8%',
            cellTemplate:
                '<input type="number" min="0"  ng-model="row.entity.AMOUNT" ng-change="grid.appScope.calculateTotalAmount(row.entity)" class="pl-sm" style="text-align: right;"/>'
        },
        {
            name: 'REMARKS',
            field: 'REMARKS',
            displayName: 'Remarks',
            headerCellClass: 'text-center',
            enableFiltering: false,
            width: '15%',
            cellTemplate:
                '<input type="text"   ng-model="row.entity.REMARKS"  class="pl-sm" />'
        }
        , {
            name: 'Action',
            headerCellTemplate:
                '<div class="ui-grid-cell-contents" style="text-align: center;">' +
                '<button ng-click="grid.appScope.addRow()" ng-disabled="!grid.appScope.model.UNIT_ID" type="button" class="btn btn-outline-danger mb-1"><img src="/img/plus-icon.png" height="15px" ></button>' +
                '</div>',
            headerCellClass: 'text-center',
            width: '5%',
            enableFiltering: false,
            enableColumnMenu: false,
            pinnedRight: true,
            cellTemplate:
                '<div style="margin: 1px;"' +
                '<button style="margin-bottom: 5px;" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px"></button>' +
                '</div>'
        }
    ]
    $scope.gridCollection.enableColumnMenus = false;

    $scope.LoadData = () => {
        $scope.showLoader = true;

        if (!$scope.model.CUSTOMER_CODE) {
            $scope.model.CUSTOMER_CODE = '';
        }
        let param = {
            DATE_FROM: $scope.model.DATE_FROM,
            DATE_TO: $scope.model.DATE_TO,
            CUSTOMER_CODE: $scope.model.CUSTOMER_CODE,
            UNIT_ID: $scope.model.UNIT_ID
        }
        return CustomerTargetService.GetTargetList(param).then(function (res) {
            $scope.gridOptionsList.data = res.data;
            $scope.showLoader = false;
            $scope.gridOptionsList.gridApi.core.notifyDataChange(uiGridConstants.dataChange.ALL);
        }, function (error) {
            $scope.showLoader = false;
            alert(error);
        });
    }
    $scope.lockLoadData = () => {
        $scope.showLoader = true;

        if (!$scope.model.CUSTOMER_CODE) {
            $scope.model.CUSTOMER_CODE = '';
        }
        let param = {
            DATE_FROM: $scope.model.DATE_FROM,
            DATE_TO: $scope.model.DATE_TO,
            CUSTOMER_CODE: $scope.model.CUSTOMER_CODE,
            UNIT_ID: $scope.model.UNIT_ID
        }
        return CustomerTargetService.lockLoadData(param).then(function (res) {
            notificationservice.Notification(res.data[0], "Success", 'Process Data Lock Successfully!');
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
            alert(error);
        });
    }
    $scope.onChangeYear = () => {
        $scope.gridOptionsList.data = [];
        if (!$scope.model.YEAR) {
            $scope.model.MONTH_CODE = '';
        }
    }
    $scope.onChangeMonth = () => {
        $scope.gridOptionsList.data = [];
        if ($scope.model.MONTH_CODE) {
            var year = parseInt($scope.model.YEAR);
            var month = parseInt($scope.model.MONTH_CODE) - 1;

            // Set DATE_FROM to the first day of the month
            var dateFrom = new Date(year, month, 1);
            $scope.model.DATE_FROM = formatDate(dateFrom); // Format as 'dd/mm/yyyy'

            // Set DATE_TO to the last day of the month
            var dateTo = new Date(year, month + 1, 0);
            $scope.model.DATE_TO = formatDate(dateTo); // Format as 'dd/mm/yyyy'
        }
    };
    $scope.onChangeToDate = () => {
        $scope.gridOptionsList.data = [];
    };
    $scope.onChangeFromDate = () => {
        $scope.gridOptionsList.data = [];
    };
    $scope.onChangeUnit = () => {
        $scope.gridOptionsList.data = [];
        $scope.gridCollection.data = [];

    };
    $scope.onChangeCustomer = () => {
        $scope.gridOptionsList.data = [];
    };
    function formatDate(date) {
        var day = ('0' + date.getDate()).slice(-2); // Get day with leading zero
        var month = ('0' + (date.getMonth() + 1)).slice(-2); // Get month with leading zero
        var year = date.getFullYear();
        return day + '/' + month + '/' + year; // Return formatted date
    }
    $scope.LoadCustomerDropDownData = function () {
        $scope.showLoader = true;
        CustomerTargetService.loadCustomer().then(function (data) {
            $scope.Customers = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }
    $scope.LoadCustomerDropDownData();
    //$scope.LoadData();
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;

        $scope.permissionReqModel = {
            Controller_Name: 'CustomerTarget',
            Action_Name: 'CustWiseRemainingBnsRpt'
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
    $scope.GetPermissionData();
    //#region ------------user_company_information------------
    $scope.Companies = [];
    $scope.CompanyUnit = [];
    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;
        CustomerTargetService.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
            $scope.CompanyLoad();
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;
        CustomerTargetService.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseFloat(data.data);
            $scope.LoadCompanyUnitData();
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }
    $scope.LoadCompanyUnitData = function () {
        $scope.showLoader = true;
        CustomerTargetService.LoadCompanyUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.CompanyUnit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });
            $scope.CompanyUnitLoad();
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.CompanyUnitLoad = function () {
        $scope.showLoader = true;
        CustomerTargetService.GetUnitId().then(function (data) {
            $scope.model.UNIT_ID = (data.data).toString();
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
    $scope.CompaniesLoad();
    //#endregion
    $scope.StockDetails = function (REPORT_EXTENSION) {
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        if ($scope.model.DATE_FROM != '' || $scope.model.DATE_TO != null) {
            let href = "/SalesAndDistribution/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&REPORT_ID=102&DATE_FROM=" + $scope.model.DATE_FROM + '&DATE_TO=' + $scope.model.DATE_TO + "&CUSTOMER_CODE=" + $scope.model.CUSTOMER_CODE + "&UNIT_ID=" + $scope.model.UNIT_ID + "&REPORT_EXTENSION=" + REPORT_EXTENSION + "&PREVIEW=" + 'NO' + "&DOT_PRINTER=" + 'NO';
            window.open(href, '_blank');
        } else {
            alert('Please Select an Date Range');
        }
    }
    $scope.StockSummary = function (REPORT_EXTENSION) {
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        if ($scope.model.DATE_FROM != '' || $scope.model.DATE_TO != null) {
            let href = "/SalesAndDistribution/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&REPORT_ID=103&DATE_FROM=" + $scope.model.DATE_FROM + '&DATE_TO=' + $scope.model.DATE_TO + "&CUSTOMER_CODE=" + $scope.model.CUSTOMER_CODE + "&UNIT_ID=" + $scope.model.UNIT_ID + "&REPORT_EXTENSION=" + REPORT_EXTENSION + "&PREVIEW=" + 'NO' + "&DOT_PRINTER=" + 'NO';
            window.open(href, '_blank');
        } else {
            alert('Please Select an Date Range');
        }

    }

    $scope.addRow = function () {
        $scope.gridCollection.data.push({
            UNIT_ID: $scope.model.UNIT_ID
        });
    };

    $scope.removeItem = function (entity) {
        let index = $scope.gridCollection.data.findIndex(item => item === entity);
        if (index !== -1) {
            $scope.gridCollection.data.splice(index, 1);
        }
    };

    $scope.AutoCompleteDataLoadForCustomer = function (value) {
        return CustomerTargetService.GetSearchableCustomer($scope.model.COMPANY_ID, value).then(function (data) {
            $scope.CustomersList = [];
            for (var i = 0; i < data.data.length; i++) {
                var _customer = {
                    CUSTOMER_CODE: data.data[i].CUSTOMER_CODE,
                    CUSTOMER_NAME: data.data[i].CUSTOMER_NAME,
                    CUSTOMER_ID: data.data[i].CUSTOMER_ID,
                    CUSTOMER_NAME_CODE: data.data[i].CUSTOMER_NAME + ' | CODE:' + data.data[i].CUSTOMER_CODE
                }
                $scope.CustomersList.push(_customer);
            }

            return $scope.CustomersList;
        }, function (error) {
            alert(error);
        });
    }
    $scope.typeaheadSelectedCustomer = function (entity, selectedItem) {
        entity.CUSTOMER_ID = selectedItem.CUSTOMER_ID;
        entity.CUSTOMER_CODE = selectedItem.CUSTOMER_CODE;
        entity.CUSTOMER_NAME = selectedItem.CUSTOMER_NAME;
    };
    $scope.typeaheadBlurCustomer = function (entity) {
        if (!entity.NAME_WITH_CODE && entity.CUSTOMER_ID > 0) {
            entity.NAME_WITH_CODE = entity.CUSTOMER_NAME;
        }
    };

    $scope.calculateTotalAmount = function (rowEntity) {
        $scope.gridCollectionApi.core.notifyDataChange(uiGridConstants.dataChange.COLUMN);
    };

    $scope.SaveData = function () {
        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);

        $scope.model.Adjustment = $scope.gridCollection.data;
        CustomerTargetService.AddOrUpdate($scope.model).then(function (data) {
            if (data.data.key == '1') {
                $scope.showLoader = false;
                notificationservice.Notification(data.data.key, '1', data.data.status);
                $scope.gridCollection.data = [];

            }
            else {
                notificationservice.Notification(data.data.status, '1', data.data.status);
                $scope.showLoader = false;
                console.log(model)
            }
        });
    }


}])