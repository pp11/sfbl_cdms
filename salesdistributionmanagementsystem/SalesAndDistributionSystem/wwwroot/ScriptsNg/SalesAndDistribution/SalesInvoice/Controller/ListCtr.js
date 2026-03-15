ngApp.controller('ngGridCtrl', ['$scope', 'InsertOrEditServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InsertOrEditServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { }
    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];
    $scope.InvoiceTypes = [];
    $scope.EntryType = [];
    $scope.model.ORDER_STATUS = 'Active';

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Sales Order Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'MST_ID', field: 'MST_ID', visible: false }
        
        , { name: 'INVOICE_TYPE_CODE', field: 'INVOICE_TYPE_CODE', visible: false }
        , { name: 'ORDER_MST_ID', field: 'ORDER_MST_ID', visible: false }

        , {
            name: 'INVOICE_NO', field: 'INVOICE_NO', displayName: 'Invoice No', enableFiltering: true, width: '10%'
        }
        , {
            name: 'INVOICE_DATE', field: 'INVOICE_DATE', displayName: 'Invoice Date', enableFiltering: true, width: '10%'
        }
        , {
            name: 'INVOICE_TYPE_NAME', field: 'INVOICE_TYPE_NAME', displayName: 'Invoice Type', enableFiltering: true, width: '10%'
        }
        , {
            name: 'CUSTOMER_CODE', field: 'CUSTOMER_CODE', displayName: 'Customer Code', enableFiltering: true, width: '10%'
        }
        , {
            name: 'CUSTOMER_NAME', field: 'CUSTOMER_NAME', displayName: 'Name', enableFiltering: true, width: '15%'
        }
        , {
            
            name: 'ORDER_NO', field: 'ORDER_NO', displayName: 'Order No', enableFiltering: true, width: '10%'
        }
        , {
            
            name: 'ORDER_AMOUNT', field: 'ORDER_AMOUNT', displayName: 'Order Amt', enableFiltering: true, width: '10%'
        },
        , {

            name: 'NET_INVOICE_AMOUNT', field: 'NET_INVOICE_AMOUNT', displayName: 'Invoice Amt', enableFiltering: true, width: '10%'
        },
        {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.DetailData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Details</button>' +
                //'<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.DELETE_PERMISSION == \'Active\'" ng-click="grid.appScope.DeleteInvoice(row.entity.INVOICE_NO)" type="button" class="btn btn-outline-danger mb-1">Delete</button>' +
                '<button style="margin-bottom: 5px;"/* ng-show="grid.appScope.model.DELETE_PERMISSION == \'Active\'"*/ ng-click="grid.appScope.GetPdfView(row.entity)" type="button" class="btn btn-outline-success mb-1">Print</button>' +

                '</div>'
        }

    ];

    //$scope.GetPdfView = function () {
    //    alert('this 1')
    //    var color = $scope.model.ReportColor;
    //    var IsLogoApplicable = $scope.model.IsLogoApplicable;
    //    var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
    //    var href = "/SalesAndDistribution/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&UNIT_ID=" + $scope.model.UNIT_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&REPORT_ID=" + $scope.model.REPORT_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&REPORT_EXTENSION=" + 'Pdf' + '&CUSTOMER_ID=' + $scope.model.CUSTOMER_ID + '&MST_ID=' + $scope.model.MST_ID;
    //    window.open(href, '_blank');
    //}

    $scope.DetailData = function (entity) {
        
        
        window.location = "/SalesAndDistribution/SalesInvoice/InvoiceDetails?q=" + entity.MST_ID_ENCRYPTED;

    }

    $scope.GetPdfView = function (entity) {
        if ($scope.model.CUSTOMER_NAME == null || $scope.model.CUSTOMER_NAME == '' || $scope.model.CUSTOMER_NAME == undefined) {
            $scope.model.CUSTOMER_ID = '';
            $scope.model.CUSTOMER_CODE = '';
        }
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/SalesAndDistribution/Report/GenerateReport?ReportId=eMjrZU/D8JBwUqyKyM3BDT/P050gXZqkNRhOq74blbdBl8kWJpdX4JNN0f41kXsg" + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&REPORT_ID=11" + "&INVOICE_DATE=" + entity.INVOICE_DATE + "&INVOICE_NO_FROM=" + entity.INVOICE_NO + "&INVOICE_NO_TO=" + entity.INVOICE_NO + "&DIVISION_ID=&REGION_ID=&AREA_ID=&TERRITORY_ID=&MARKET_ID=&CUSTOMER_ID=&UNIT_ID=" + entity.INVOICE_UNIT_ID+"&UNIT_NAME=" + $scope.model.UNIT_NAME + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&CUSTOMER_CODE=&REPORT_EXTENSION=" + 'Pdf' + "&PREVIEW=" + 'YES';
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

        $scope.showLoader = true;

        InsertOrEditServices.LoadInvoiceTypes($scope.model.COMPANY_ID).then(function (data) {

            $scope.InvoiceTypes = data.data;

            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.InvoiceTypeLoad();
    $scope.DataLoad = function () {
        

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

        InsertOrEditServices.Load_Sales_Invoice_Mst_data($scope.modeldata).then(function (data) {
            
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    $scope.DataInitialLoad = function () {
        
        const today = new Date();
        const yyyy = today.getFullYear();
        let mm = today.getMonth() + 1; // Months start at 0!
        let dd = today.getDate();

        if (dd < 10) dd = '0' + dd;
        if (mm < 10) mm = '0' + mm;

        $scope.model.DATE_FROM = dd + '/' + mm + '/' + yyyy;
        $scope.model.DATE_TO = dd + '/' + mm + '/' + yyyy;
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

        InsertOrEditServices.Load_Sales_Invoice_Mst_data($scope.modeldata).then(function (data) {

            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
        }, function (error) {

            alert(error);

            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        InsertOrEditServices.GetCompany().then(function (data) {
            
            $scope.model.COMPANY_ID = parseFloat(data.data);
          
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
    $scope.DeleteInvoice = function (invoice) {
        
        $scope.showLoader = true;

        InsertOrEditServices.DeleteInvoice(invoice).then(function (data) {
            
            $scope.DataLoad();

            notificationservice.Notification(1, 1, "Invoice " + invoice + " Deleted Successfully!!");
            $scope.showLoader = false;
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
   
    $scope.ClearForm = function () {
        window.location = "/SalesAndDistribution/SalesInvoice/List";
    }

    

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'SalesInvoice',
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

            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    $scope.DataInitialLoad();
    $scope.GetPermissionData();
    $scope.GetDivitionToMarketRelation();
  

}]);

