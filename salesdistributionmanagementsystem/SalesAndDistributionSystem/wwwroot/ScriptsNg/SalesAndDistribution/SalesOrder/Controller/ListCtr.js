ngApp.controller('ngGridCtrl', ['$scope', 'InsertOrEditServices', 'permissionProvider', 'notificationservice', 'LiveNotificationServices', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InsertOrEditServices, permissionProvider, notificationservice, LiveNotificationServices, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {}
    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];
    $scope.InvoiceTypes = [];
    $scope.EntryType = [];
    $scope.CHECK_DATA = [];
    $scope.state = 0;

    $scope.model.ORDER_STATUS = 'Active';
    var connection = new signalR.HubConnectionBuilder().withUrl("/notificationhub").build();
    connection.start();
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Sales Order Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        /*$scope.gridApi = gridApi;*/
        $scope.gridOptionsList.api = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        { name: '#', field: 'ROW_NO', enableFiltering: false, width: '50' }
        , { name: 'ORDER_DTL_ID', field: 'ORDER_DTL_ID', visible: false }
        , { name: 'ORDER_MST_ID', field: 'ORDER_MST_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , { name: 'ORDER_NO', field: 'ORDER_NO', displayName: 'Order No', enableFiltering: true, width: '140' }
        , {
            name: 'INVOICE_NO',
            field: 'INVOICE_NO',
            displayName: 'Invoice No',
            enableFiltering: true,
            width: '140',
            cellTemplate: '<a href="" ng-click="grid.appScope.GetInvoicePdfView(row.entity)">{{ COL_FIELD }}</a>'
        }
        , {
            name: 'INVOICE_TYPE_NAME', field: 'INVOICE_TYPE_NAME', displayName: 'Type', enableFiltering: true, width: '70'
        }
        , {
            name: 'CUSTOMER_NAME', field: 'CUSTOMER_NAME', displayName: 'Customer', enableFiltering: true, width: '15%'
        }
        , {
            name: 'CUSTOMER_CODE', field: 'CUSTOMER_CODE', displayName: 'Code', enableFiltering: true, width: '6%'
        }
        , {
            name: 'ORDER_DATE', field: 'ORDER_DATE', displayName: 'OrderDate', enableFiltering: true, width: '10%'
        }
        , {
            name: 'DELIVERY_DATE', field: 'DELIVERY_DATE', displayName: 'Delivery Date', enableFiltering: true, width: '10%'
        }
        , {
            name: 'MARKET_NAME', field: 'MARKET_NAME', displayName: 'Market', enableFiltering: true, width: '12%'
        }
        , {
            name: 'ORDER_ENTRY_TYPE', field: 'ORDER_ENTRY_TYPE', displayName: 'Entry Type', enableFiltering: true, width: '10%'
        }
        , {
            name: 'ORDER_STATUS', field: 'ORDER_STATUS', displayName: 'Order Status', enableFiltering: true, width: '10%'
        }
        , {
            name: 'ORDER_AMOUNT', field: 'ORDER_AMOUNT', displayName: 'Order Amount', enableFiltering: true, width: '10%'
        }

        , {
            name: 'Action', displayName: 'Action', width: '25%', enableFiltering: false, enableColumnMenu: false, pinnedRight: true, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;margin-left:5px"/* ng-show="grid.appScope.model.DELETE_PERMISSION == \'Active\'"*/ ng-click="grid.appScope.GetPdfView(row.entity)" type="button" class="btn btn-outline-success mb-1"><i class="fa fa-print"></i></button>' +
                '<button style="margin-bottom: 5px;margin-left:5px" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\' && (row.entity.FINAL_SUBMISSION_STATUS != \'Complete\' || (grid.appScope.model.USER_TYPE == \'DSM\' || grid.appScope.model.USER_TYPE == \'SM\' || grid.appScope.model.USER_TYPE == \'HOS\'))" ng-click="grid.appScope.SaveDataFinal(row.entity)" type="button" class="btn btn-success mb-1">Post</button>' +
                '<button style="margin-bottom: 5px;margin-left:5px" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '<button style="margin-bottom: 5px;margin-left:5px" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\' && row.entity.FINAL_SUBMISSION_STATUS != \'Complete\'" ng-click="grid.appScope.Delete_Order_full_Query(row.entity)" type="button" class="btn btn-outline-danger mb-1">Delete</button>' +
                '</div>'
        }

    ];
    //$scope.gridOptionsList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"
    $scope.gridOptionsProcessedList = (gridregistrationservice.GridRegistration("Sales Processed Info"));
    $scope.gridOptionsProcessedList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsProcessedList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }

        , { name: 'ORDER_DTL_ID', field: 'ORDER_DTL_ID', visible: false }

        , { name: 'ORDER_MST_ID', field: 'ORDER_MST_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }

        , {
            name: 'ORDER_NO', field: 'ORDER_NO', displayName: 'Order No', enableFiltering: true, width: '140'
        }, {
            name: 'INVOICE_TYPE_NAME', field: 'INVOICE_TYPE_NAME', displayName: 'Type', enableFiltering: true, width: '70'
        }
        , {
            name: 'CUSTOMER_NAME', field: 'CUSTOMER_NAME', displayName: 'Customer', enableFiltering: true, width: '15%'
        }
        , {
            name: 'ORDER_DATE', field: 'ORDER_DATE', displayName: 'OrderDate', enableFiltering: true, width: '10%'
        }
        , {
            name: 'DELIVERY_DATE', field: 'DELIVERY_DATE', displayName: 'Delivery Date', enableFiltering: true, width: '10%'
        }
        , {
            name: 'MARKET_NAME', field: 'MARKET_NAME', displayName: 'Market', enableFiltering: true, width: '12%'
        }
        , {
            name: 'ORDER_ENTRY_TYPE', field: 'ORDER_ENTRY_TYPE', displayName: 'Entry Type', enableFiltering: true, width: '10%'
        }
        , {
            name: 'ORDER_STATUS', field: 'ORDER_STATUS', displayName: 'Order Status', enableFiltering: true, width: '10%'
        }
        , {
            name: 'ORDER_AMOUNT', field: 'ORDER_AMOUNT', displayName: 'Order Amount', enableFiltering: true, width: '10%'
        }

        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;margin-left:5px" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.Delete_Order_full_Query(row.entity)" type="button" class="btn btn-outline-danger mb-1">Delete</button>' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];
    $scope.gridOptionsProcessedList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"
    $scope.EditData = function (entity) {
        window.location = "/SalesAndDistribution/SalesOrder/InsertOrEdit?Id=" + entity.ORDER_MST_ID_ENCRYPTED;
    }
    $scope.GetPdfView = function (entity) {
        if ($scope.model.CUSTOMER_NAME == null || $scope.model.CUSTOMER_NAME == '' || $scope.model.CUSTOMER_NAME == undefined) {
            $scope.model.CUSTOMER_ID = '';
            $scope.model.CUSTOMER_CODE = '';
        }
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/SalesAndDistribution/Report/GenerateReport?ReportId=" + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&REPORT_ID=" + "&INVOICE_DATE=" + entity.ORDER_DATE + "&INVOICE_NO_FROM=" + entity.ORDER_NO + "&INVOICE_NO_TO=" + entity.ORDER_NO + "&DIVISION_ID=&REGION_ID=&AREA_ID=&TERRITORY_ID=&MARKET_ID=&CUSTOMER_ID=&UNIT_ID=" + entity.ORDER_UNIT_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&CUSTOMER_CODE=&REPORT_EXTENSION=" + 'Pdf' + "&PREVIEW=" + 'NO';
        window.open(href, '_blank');
    }
    $scope.GetInvoicePdfView = function (entity) {
        debugger;
        if ($scope.model.CUSTOMER_NAME == null || $scope.model.CUSTOMER_NAME == '' || $scope.model.CUSTOMER_NAME == undefined) {
            $scope.model.CUSTOMER_ID = '';
            $scope.model.CUSTOMER_CODE = '';
        }
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/SalesAndDistribution/Report/GenerateReport?ReportId=eMjrZU/D8JBwUqyKyM3BDT/P050gXZqkNRhOq74blbdBl8kWJpdX4JNN0f41kXsg" + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&REPORT_ID=11" + "&INVOICE_DATE=" + entity.INVOICE_DATE + "&INVOICE_NO_FROM=" + entity.INVOICE_NO + "&INVOICE_NO_TO=" + entity.INVOICE_NO + "&DIVISION_ID=&REGION_ID=&AREA_ID=&TERRITORY_ID=&MARKET_ID=&CUSTOMER_ID=&UNIT_ID=" + entity.ORDER_UNIT_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&CUSTOMER_CODE=&REPORT_EXTENSION=" + 'Pdf' + "&PREVIEW=" + 'YES';
        window.open(href, '_blank');
    }
    //Filter Data Load----------------------
    $scope.GetDivitionToMarketRelation = function () {
        $scope.showLoader = true;
        InsertOrEditServices.GetDivitionToMarketRelation($scope.model.DIVISION_ID).then(function (data) {
            $scope.GetDivitionToMarketRelations = data.data;
            $scope.Divisions = [];
            $scope.Regions = [];
            $scope.Areas = [];
            $scope.Territories = [];
            $scope.Markets = [];

            const map = new Map();
            const mapREGION_ID = new Map();
            const mapAREA_ID = new Map();
            const mapTERRITORY_ID = new Map();
            const mapMARKET_ID = new Map();
            for (const item of data.data) {
                if (!map.has(item.DIVISION_ID)) {
                    map.set(item.DIVISION_ID, true);    // set any value to Map
                    $scope.Divisions.push({
                        DIVISION_ID: item.DIVISION_ID,
                        DIVISION_NAME: item.DIVISION_NAME
                    });
                }
                if (!mapREGION_ID.has(item.REGION_ID)) {
                    mapREGION_ID.set(item.REGION_ID, true);    // set any value to Map
                    $scope.Regions.push({
                        REGION_ID: item.REGION_ID,
                        REGION_NAME: item.REGION_NAME
                    });
                }
                if (!mapAREA_ID.has(item.AREA_ID)) {
                    mapAREA_ID.set(item.AREA_ID, true);    // set any value to Map
                    $scope.Areas.push({
                        AREA_ID: item.AREA_ID,
                        AREA_NAME: item.AREA_NAME
                    });
                }
                if (!mapTERRITORY_ID.has(item.TERRITORY_ID)) {
                    mapTERRITORY_ID.set(item.TERRITORY_ID, true);    // set any value to Map
                    $scope.Territories.push({
                        TERRITORY_ID: item.TERRITORY_ID,
                        TERRITORY_NAME: item.TERRITORY_NAME
                    });
                }
                if (!mapMARKET_ID.has(item.MARKET_ID)) {
                    mapMARKET_ID.set(item.MARKET_ID, true);    // set any value to Map
                    $scope.Markets.push({
                        MARKET_ID: item.MARKET_ID,
                        MARKET_NAME: item.MARKET_NAME
                    });
                }

            }
            $scope.Divisions.unshift({ DIVISION_ID: '', DIVISION_NAME: 'ALL' })
            $scope.Regions.unshift({ REGION_ID: '', REGION_NAME: 'ALL' })
            $scope.Areas.unshift({ AREA_ID: '', AREA_NAME: 'ALL' })
            $scope.Territories.unshift({ TERRITORY_ID: '', TERRITORY_NAME: 'ALL' })
            $scope.Markets.unshift({ MARKET_ID: '', MARKET_NAME: 'ALL' })
            $scope.showLoader = false;
        }, function (error) {
            console.log(error);
            $scope.showLoader = false;
        });
    }
    $scope.DivisionChange = function () {

        $scope.showLoader = true;
        let GetDivitionToMarketRelations = [];
        $scope.Regions = [];
        $scope.Areas = [];
        $scope.Territories = [];
        $scope.Markets = [];
        if ($scope.model.DIVISION_ID != '') {
            let DIVISION_ID = $scope.model.DIVISION_ID;
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations.filter((x) => { return x.DIVISION_ID == DIVISION_ID; });
        } else {

            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations;
        }
        const mapREGION_ID = new Map();
        const mapAREA_ID = new Map();
        const mapTERRITORY_ID = new Map();
        const mapMARKET_ID = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!mapREGION_ID.has(item.REGION_ID)) {
                mapREGION_ID.set(item.REGION_ID, true);    // set any value to Map
                $scope.Regions.push({
                    REGION_ID: item.REGION_ID,
                    REGION_NAME: item.REGION_NAME
                });
            }
            if (!mapAREA_ID.has(item.AREA_ID)) {
                mapAREA_ID.set(item.AREA_ID, true);    // set any value to Map
                $scope.Areas.push({
                    AREA_ID: item.AREA_ID,
                    AREA_NAME: item.AREA_NAME
                });
            }
            if (!mapTERRITORY_ID.has(item.TERRITORY_ID)) {
                mapTERRITORY_ID.set(item.TERRITORY_ID, true);    // set any value to Map
                $scope.Territories.push({
                    TERRITORY_ID: item.TERRITORY_ID,
                    TERRITORY_NAME: item.TERRITORY_NAME
                });
            }
            if (!mapMARKET_ID.has(item.MARKET_ID)) {
                mapMARKET_ID.set(item.MARKET_ID, true);    // set any value to Map
                $scope.Markets.push({
                    MARKET_ID: item.MARKET_ID,
                    MARKET_NAME: item.MARKET_NAME
                });
            }
        }

        $scope.Regions.unshift({ REGION_ID: '', REGION_NAME: 'ALL' })
        $scope.Areas.unshift({ AREA_ID: '', AREA_NAME: 'ALL' })
        $scope.Territories.unshift({ TERRITORY_ID: '', TERRITORY_NAME: 'ALL' })
        $scope.Markets.unshift({ MARKET_ID: '', MARKET_NAME: 'ALL' })
        $scope.showLoader = false;

    }
    $scope.RegionChange = function () {
        $scope.showLoader = true;
        let GetDivitionToMarketRelations = [];
        $scope.Areas = [];
        $scope.Territories = [];
        $scope.Markets = [];
        if ($scope.model.REGION_ID != '') {
            let REGION_ID = $scope.model.REGION_ID;
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations.filter((x) => { return x.REGION_ID == REGION_ID; });
        } else {
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations;
        }
        var map = new Map();

        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.AREA_ID)) {
                map.set(item.AREA_ID, true);    // set any value to Map
                $scope.Areas.push({
                    AREA_ID: item.AREA_ID,
                    AREA_NAME: item.AREA_NAME
                });
            }
        }
        map = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.TERRITORY_ID)) {
                map.set(item.TERRITORY_ID, true);    // set any value to Map
                $scope.Territories.push({
                    TERRITORY_ID: item.TERRITORY_ID,
                    TERRITORY_NAME: item.TERRITORY_NAME
                });
            }
        }
        map = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.MARKET_ID)) {
                map.set(item.MARKET_ID, true);    // set any value to Map
                $scope.Markets.push({
                    MARKET_ID: item.MARKET_ID,
                    MARKET_NAME: item.MARKET_NAME
                });
            }
        }
        $scope.Areas.unshift({ AREA_ID: '', AREA_NAME: 'ALL' })
        $scope.Territories.unshift({ TERRITORY_ID: '', TERRITORY_NAME: 'ALL' })
        $scope.Markets.unshift({ MARKET_ID: '', MARKET_NAME: 'ALL' })
        $scope.showLoader = false;

    }
    $scope.AreaChange = function () {
        $scope.showLoader = true;
        let GetDivitionToMarketRelations = [];
        $scope.Territories = [];
        $scope.Markets = [];
        if ($scope.model.AREA_ID != '') {
            let AREA_ID = $scope.model.AREA_ID;
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations.filter((x) => { return x.AREA_ID == AREA_ID; });
        } else {
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations;
        }
        var map = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.TERRITORY_ID)) {
                map.set(item.TERRITORY_ID, true);    // set any value to Map
                $scope.Territories.push({
                    TERRITORY_ID: item.TERRITORY_ID,
                    TERRITORY_NAME: item.TERRITORY_NAME
                });
            }
        }
        map = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.MARKET_ID)) {
                map.set(item.MARKET_ID, true);    // set any value to Map
                $scope.Markets.push({
                    MARKET_ID: item.MARKET_ID,
                    MARKET_NAME: item.MARKET_NAME
                });
            }
        }

        $scope.Territories.unshift({ TERRITORY_ID: '', TERRITORY_NAME: 'ALL' })
        $scope.Markets.unshift({ MARKET_ID: '', MARKET_NAME: 'ALL' })
        $scope.showLoader = false;

    }
    $scope.TerritoryChange = function () {
        $scope.showLoader = true;
        let GetDivitionToMarketRelations = [];

        $scope.Markets = [];
        if ($scope.model.TERRITORY_ID != '') {
            let TERRITORY_ID = $scope.model.TERRITORY_ID;
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations.filter((x) => { return x.TERRITORY_ID == TERRITORY_ID; });
        } else {
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations;
        }
        var map = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.MARKET_ID)) {
                map.set(item.MARKET_ID, true);    // set any value to Map
                $scope.Markets.push({
                    MARKET_ID: item.MARKET_ID,
                    MARKET_NAME: item.MARKET_NAME
                });
            }
        }

        $scope.Markets.unshift({ MARKET_ID: '', MARKET_NAME: 'ALL' })
        $scope.showLoader = false;

    }
    $scope.GetCurrentDate = function () {
        const today = new Date();
        const yyyy = today.getFullYear();
        let mm = today.getMonth() + 1; // Months start at 0!
        let dd = today.getDate();

        if (dd < 10) dd = '0' + dd;
        if (mm < 10) mm = '0' + mm;

        $scope.model.DATE_FROM = dd + '/' + mm + '/' + yyyy;
        $scope.model.DATE_TO = dd + '/' + mm + '/' + yyyy;

    }
    $scope.GetCurrentDate();
    //------------END----------------------
    $scope.DataLoad = function (companyId) {
        InsertOrEditServices.LoadData(companyId).then(function (data) {
            $scope.gridOptionsList.data = data.data;
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }
    $scope.DataLoad(0);
    $scope.LoadFilteredData = function () {
        $scope.showLoader = true;
        $scope.modeldata = [];
        $scope.modeldata.DATE_FROM = $scope.model.DATE_FROM;
        $scope.modeldata.DATE_TO = $scope.model.DATE_TO;
        $scope.modeldata.DIVISION_ID = $scope.model.DIVISION_ID == null || $scope.model.DIVISION_ID == '' ? '' : $scope.model.DIVISION_ID;
        $scope.modeldata.REGION_ID = $scope.model.REGION_ID == null || $scope.model.REGION_ID == '' ? '' : $scope.model.REGION_ID;
        $scope.modeldata.AREA_ID = $scope.model.AREA_ID == null || $scope.model.AREA_ID == '' ? '' : $scope.model.AREA_ID;
        $scope.modeldata.TERRITORY_ID = $scope.model.TERRITORY_ID == null || $scope.model.TERRITORY_ID == '' ? '' : $scope.model.TERRITORY_ID;
        $scope.modeldata.CUSTOMER_ID = $scope.model.CUSTOMER_ID == null || $scope.model.CUSTOMER_ID == '' ? '' : $scope.model.CUSTOMER_ID;
        $scope.modeldata.ORDER_ENTRY_TYPE = $scope.model.ORDER_ENTRY_TYPE == null || $scope.model.ORDER_ENTRY_TYPE == '' ? '' : $scope.model.ORDER_ENTRY_TYPE;
        $scope.modeldata.ORDER_STATUS = $scope.model.ORDER_STATUS == null || $scope.model.ORDER_STATUS == '' ? '' : $scope.model.ORDER_STATUS;
        $scope.modeldata.ORDER_TYPE = $scope.model.ORDER_TYPE == null || $scope.model.ORDER_TYPE == '' ? '' : $scope.model.ORDER_TYPE;
        InsertOrEditServices.LoadFilteredData($scope.modeldata).then(function (data) {

            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
        }, function (error) {

            alert(error);

            $scope.showLoader = false;

        });
    }
    $scope.LoadFilteredProcessedData = function () {
        document.getElementById("processedGrid").style.display = "block";

        $scope.showLoader = true;
        $scope.modeldata = [];
        $scope.modeldata.DATE_FROM = $scope.model.DATE_FROM;
        $scope.modeldata.DATE_TO = $scope.model.DATE_TO;
        $scope.modeldata.DIVISION_ID = $scope.model.DIVISION_ID == null || $scope.model.DIVISION_ID == '' ? '' : $scope.model.DIVISION_ID;
        $scope.modeldata.REGION_ID = $scope.model.REGION_ID == null || $scope.model.REGION_ID == '' ? '' : $scope.model.REGION_ID;
        $scope.modeldata.AREA_ID = $scope.model.AREA_ID == null || $scope.model.AREA_ID == '' ? '' : $scope.model.AREA_ID;
        $scope.modeldata.TERRITORY_ID = $scope.model.TERRITORY_ID == null || $scope.model.TERRITORY_ID == '' ? '' : $scope.model.TERRITORY_ID;
        $scope.modeldata.CUSTOMER_ID = $scope.model.CUSTOMER_ID == null || $scope.model.CUSTOMER_ID == '' ? '' : $scope.model.CUSTOMER_ID;
        $scope.modeldata.ORDER_ENTRY_TYPE = $scope.model.ORDER_ENTRY_TYPE == null || $scope.model.ORDER_ENTRY_TYPE == '' ? '' : $scope.model.ORDER_ENTRY_TYPE;
        $scope.modeldata.ORDER_STATUS = $scope.model.ORDER_STATUS == null || $scope.model.ORDER_STATUS == '' ? '' : $scope.model.ORDER_STATUS;
        $scope.modeldata.ORDER_TYPE = $scope.model.ORDER_TYPE == null || $scope.model.ORDER_TYPE == '' ? '' : $scope.model.ORDER_TYPE;
        InsertOrEditServices.LoadFilteredProcessedData($scope.modeldata).then(function (data) {
            $scope.gridOptionsProcessedList.data = data.data;
            $scope.showLoader = false;
        }, function (error) {

            alert(error);

            $scope.showLoader = false;

        });
    }
    $scope.LoadEntryType = function () {
        var Active = {
            STATUS: 'Manual'
        }
        var InActive = {
            STATUS: 'Auto'
        }

        $scope.EntryType.push(Active);

        $scope.EntryType.push(InActive);


    }
    $scope.LoadEntryType();

    $scope.LoadStatus = function () {
        var Active = {
            STATUS: 'Active'
        }
        var InActive = {
            STATUS: 'InActive'
        }
        var Complete = {
            STATUS: 'Complete'
        }
        $scope.Status.push(Active);

        $scope.Status.push(InActive);
        $scope.Status.push(Complete);


    }
    $scope.LoadStatus();
    $scope.InvoiceTypeLoad = function () {
        InsertOrEditServices.LoadInvoiceTypes($scope.model.COMPANY_ID).then(function (data) {

            $scope.InvoiceTypes = data.data;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.InvoiceTypeLoad();

    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        InsertOrEditServices.GetCompany().then(function (data) {

            $scope.model.COMPANY_ID = parseFloat(data.data);
            $scope.DataLoad($scope.model.COMPANY_ID);
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

        InsertOrEditServices.GetCompanyList().then(function (data) {

            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }
    $scope.SendNotification = function () {
        $scope.showLoader = true;

        LiveNotificationServices.Notification_Permitted_Users(1, $scope.model.COMPANY_ID, $scope.model.UNIT_ID).then(function (data) {

            $scope.showLoader = false;

            $scope.Users = data.data;
            $scope.Permitted_Users = [];
            if ($scope.Users != undefined && $scope.Users != null) {
                for (var i = 0; i < $scope.Users.length; i++) {
                    $scope.Permitted_Users.push(JSON.stringify(parseInt($scope.Users[i].USER_ID)));
                }
                connection.invoke("SendMessage", $scope.Permitted_Users, ": New Order Has been Added/Edited and Ready For Invoice!!!!").catch(function (err) {
                    return console.error(err.toString());
                });
            }
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });

    }
    $scope.ClearForm = function () {
        window.location = "/SalesAndDistribution/SalesOrder/List";
    }
    $scope.GetPermissionData = function () {

        $scope.permissionReqModel = {
            Controller_Name: 'SalesOrder',
            Action_Name: 'List'
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

            if ($scope.model.USER_TYPE != 'Distributor') {
                // Find the index of the column definition with name 'INVOICE_NO'
                const index = $scope.gridOptionsList.columnDefs.findIndex(column => column.name === 'INVOICE_NO');
                // If the column is found, remove it
                if (index !== -1) {
                    $scope.gridOptionsList.columnDefs.splice(index, 1);
                    // Notify the grid that the column definitions have changed
                    $scope.gridOptionsList.api.core.notifyDataChange('all');
                }
            }
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }
    $scope.GetPermissionData();
    $scope.Get_Customer_Balance = function (model) {
        $scope.showLoader = true;

        InsertOrEditServices.Get_Customer_Balance(model).then(function (data) {
            if (data.data[1].length > 0) {
                $scope.model.CREDIT_LIMIT = data.data[1][0].CREDIT_LIMIT;
                $scope.model.CREDIT_LIMIT_DAYS = data.data[1][0].CREDIT_DAYS;

                var st = false;

                for (var i = 0; i < $scope.CHECK_DATA[1].length;i++) {
                    if (i > 0 && $scope.CHECK_DATA[1][i].MAXIMUM_QTY != null & $scope.CHECK_DATA[1][i].MAXIMUM_QTY < $scope.CHECK_DATA[1][i].ORDER_QTY) {
                        st = true;
                    }

                }

                if (st == true) {
                    $scope.CHECK_DATA[0][0].NOTIFY_TEXT = "Max QTY Exceded";
                    $scope.state = 1;

                } else if ($scope.model.CREDIT_LIMIT < $scope.CHECK_DATA[0][0].NET_ORDER_AMOUNT) {
                    $scope.CHECK_DATA[0][0].NOTIFY_TEXT = "Max Credit Exceded";
                    $scope.state = 2;

                }
                $scope.showLoader = true;
                InsertOrEditServices.Save_Final_Post_Order($scope.CHECK_DATA[0][0]).then(function (data) {

                    $scope.DataLoad(0);
                    $scope.SendNotification();
                    if ($scope.state == 0) {
                        notificationservice.Notification(1, 1, 'Order Generated Successfully!');
                    } else if ($scope.state == 1) {
                        notificationservice.Notification("Max QTY Exceded! Order is forwarded to DSM to review!", 1, 'Order Generated Successfully!');

                    } else if ($scope.state == 2) {
                        notificationservice.Notification("Max Credit Exceded! Order is forwarded to DSM to review!", 1, 'Order Generated Successfully!');

                    }

                    $scope.showLoader = false;


                });
            } else {
                var st = false;

                for (var i = 0; i < $scope.CHECK_DATA[1].length; i++) {
                    if (i > 0 && $scope.CHECK_DATA[1][i].MAXIMUM_QTY != null && $scope.CHECK_DATA[1][i].MAXIMUM_QTY != undefined && $scope.CHECK_DATA[1][i].MAXIMUM_QTY < $scope.CHECK_DATA[1][i].ORDER_QTY) {
                        st = true;
                    }

                }

                if (st == true) {
                    $scope.CHECK_DATA[0][0].NOTIFY_TEXT = "Max QTY Exceded";

                }
                $scope.showLoader = true;
                InsertOrEditServices.Save_Final_Post_Order($scope.CHECK_DATA[0][0]).then(function (data) {

                    $scope.DataLoad(0);
                    $scope.SendNotification();
                    notificationservice.Notification(1, 1, 'Order Generated Successfully!');

                    $scope.showLoader = false;


                });
            }


            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.SaveDataFinal = function (model) {
        debugger
        InsertOrEditServices.GetEditDataById(model.ORDER_MST_ID_ENCRYPTED).then(function (data) {

            if (data.data != null) {
                $scope.CHECK_DATA = [];
                $scope.CHECK_DATA = data.data;
                $scope.Get_Customer_Balance($scope.CHECK_DATA[0][0].CUSTOMER_ID);
            }

            $scope.showLoader = false;
        }, function (error) {
            alert(error);

        });
    }
    $scope.Delete_Order_full_Query = function (model) {

        InsertOrEditServices.Delete_Order_full_Query(model).then(function (data) {

            notificationservice.Notification(data.data.status, 1, 'Data Deleted Successfully !!');

            if (data.data.status == 1) {

                $scope.model.ORDER_MST_ID = 0;

                //$scope.SendNotification();
                //$scope.addNewRow();
                $scope.showLoader = false;

            }
            else {
                $scope.showLoader = false;
                //$scope.addNewRow();

            }
        });
    }

    $scope.LoadOrderRationableSKUs = function () {

        if ($scope.model.USER_TYPE != 'Distributor' && $scope.GetDivitionToMarketRelations==undefined) {
            $scope.GetDivitionToMarketRelation();

        } 
    }

}]);

