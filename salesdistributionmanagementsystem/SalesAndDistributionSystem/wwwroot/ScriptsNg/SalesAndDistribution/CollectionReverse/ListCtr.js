ngApp.controller('ngGridCtrl', ['$scope', 'CollectionReverseServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, CollectionReverseServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { COMPANY_ID: 0, REGION_ID: 0, REGION_CODE: '', REGION_AREA_MST_STATUS: 'Active', EFFECT_START_DATE: '', EFFECT_END_DATE: '', REMARKS: '' }
    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Region Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: 'ROW_NO',
            field: 'ROW_NO',
            displayName: '#',
            enableFiltering: false,
            enableSorting: false,
            cellTemplate: '<div class="ui-grid-cell-contents">{{grid.renderContainers.body.visibleRowCache.indexOf(row) + 1}}</div>',
            width: '5%',
            pinnedLeft: true

        },
        { name: 'COLL_REVERSE_ID', field: 'COLL_REVERSE_ID', visible: false }, // Assuming COLL_REVERSE_ID is the primary key
        { name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: true, width: '10%' },
        { name: 'BATCH_DATE', field: 'BATCH_DATE', displayName: 'Batch Date', enableFiltering: true, width: '12%' },
        { name: 'CUSTOMER_ID', field: 'CUSTOMER_ID', visible: false }, // Assuming CUSTOMER_ID is a foreign key
        { name: 'CUSTOMER_CODE', field: 'CUSTOMER_CODE', displayName: 'Customer Code', enableFiltering: true, width: '10%' },
        { name: 'CUSTOMER_NAME', field: 'CUSTOMER_NAME', displayName: 'Customer Name', enableFiltering: true, width: '15%' },
        { name: 'CUSTOMER_ADDRESS', field: 'CUSTOMER_ADDRESS', displayName: 'Customer Address', enableFiltering: true, width: '20%' },
        { name: 'VOUCHER_NO', field: 'VOUCHER_NO', displayName: 'Voucher No', enableFiltering: true, width: '10%' },
        { name: 'VOUCHER_DATE', field: 'VOUCHER_DATE', displayName: 'Voucher Date', enableFiltering: true, width: '12%' },
        { name: 'BANK_ID', field: 'BANK_ID', visible: false }, // Assuming BANK_ID is a foreign key
        { name: 'BRANCH_ID', field: 'BRANCH_ID', visible: false }, // Assuming BRANCH_ID is a foreign key
        { name: 'COLLECTION_MODE', field: 'COLLECTION_MODE', displayName: 'Collection Mode', enableFiltering: true, width: '12%' },
        { name: 'COLLECTION_AMT', field: 'COLLECTION_AMT', displayName: 'Collection Amount', enableFiltering: true, width: '15%' },
        { name: 'TDS_AMT', field: 'TDS_AMT', displayName: 'TDS Amount', enableFiltering: true, width: '10%' },
        { name: 'MEMO_COST', field: 'MEMO_COST', displayName: 'Memo Cost', enableFiltering: true, width: '10%' },
        { name: 'NET_COLLECTION_AMT', field: 'NET_COLLECTION_AMT', displayName: 'Net Collection Amount', enableFiltering: true, width: '15%' },
        { name: 'ENTERED_BY', field: 'ENTERED_BY', displayName: 'Entered By', enableFiltering: true, width: '10%' },
        { name: 'ENTERED_DATE', field: 'ENTERED_DATE', displayName: 'Entered Date', enableFiltering: true, width: '12%' },
        { name: 'ENTERED_TERMINAL', field: 'ENTERED_TERMINAL', displayName: 'Entered Terminal', enableFiltering: true, width: '12%' },
        { name: 'REMARKS', field: 'REMARKS', displayName: 'Remarks', enableFiltering: true, width: '15%' },
        { name: 'REVERSE_DATE', field: 'REVERSE_DATE', displayName: 'Reverse Date', enableFiltering: true, width: '12%' },
        { name: 'COLLECTION_DTL_ID', field: 'COLLECTION_DTL_ID', visible: false } // Assuming COLLECTION_DTL_ID is a foreign key

        //, {
        //    name: 'Action', displayName: 'Action', width: '30%', enableFiltering: false, enableColumnMenu: false, pinnedRight: true, cellTemplate:
        //        '<div style="margin:1px;">' +
        //        '<button style="margin-bottom: 5px;margin-left: 10px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1" ng-disabled="row.entity.BATCH_POSTING_STATUS == \'Approved\'">Update</button>' +
        //        '<button style="margin-bottom: 5px;margin-left: 10px;"/* ng-show="grid.appScope.model.DELETE_PERMISSION == \'Active\'"*/ ng-click="grid.appScope.GetPdfView(row.entity)" type="button" class="btn btn btn-outline-info mb-1"><i class="fas fa-file-pdf"></i> Pdf</button>' +
        //        '<button style="margin-bottom: 5px;margin-left: 10px;"/* ng-show="grid.appScope.model.DELETE_PERMISSION == \'Active\'"*/ ng-click="grid.appScope.GetExcelView(row.entity)" type="button" class="btn btn btn-outline-info mb-1"><i class="fas fa-file-excel"></i> Excel</button>' +
        //        '<button style="margin-bottom: 5px;margin-left: 10px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'"  ng-click="grid.appScope.Update_Approval(row.entity)" ng-disabled="row.entity.BATCH_POSTING_STATUS == \'Approved\'" type="button" class="btn btn btn-success mb-1"> Post</button>' +

        //        '</div>'

        //},

    ];
    //$scope.gridOptionsList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.EditData = function (entity) {

        window.location = "/SalesAndDistribution/CollectionV2/InsertOrEdit?Id=" + entity.COLLECTION_MST_ID_ENCRYPTED;

    }



    $scope.GetPdfView = function (entity) {
        if ($scope.model.CUSTOMER_NAME == null || $scope.model.CUSTOMER_NAME == '' || $scope.model.CUSTOMER_NAME == undefined) {
            $scope.model.CUSTOMER_ID = '';
            $scope.model.CUSTOMER_CODE = '';

        }
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/SalesAndDistribution/Report/GenerateReport?ReportId=WKsARVzZCVznSIDeKNQSrCnHiA7PS9QFyzoI6wy2Frqhqtkx1s3AMY qqVhcH M4" + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&UNIT_ID=&DATE_FROM=&DATE_TO=&REPORT_ID=41&UNIT_NAME=&REPORT_EXTENSION=Pdf&CUSTOMER_ID=&MST_ID=" + entity.COLLECTION_MST_ID + "&REPORT_EXTENSION=" + 'Pdf' + " & PREVIEW=" + 'NO';

        window.open(href, '_blank');

    }

    $scope.GetExcelView = function (entity) {
        if ($scope.model.CUSTOMER_NAME == null || $scope.model.CUSTOMER_NAME == '' || $scope.model.CUSTOMER_NAME == undefined) {
            $scope.model.CUSTOMER_ID = '';
            $scope.model.CUSTOMER_CODE = '';

        }
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/SalesAndDistribution/Report/GenerateReport?ReportId=WKsARVzZCVznSIDeKNQSrCnHiA7PS9QFyzoI6wy2Frqhqtkx1s3AMY qqVhcH M4" + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&UNIT_ID=&DATE_FROM=&DATE_TO=&REPORT_ID=41&UNIT_NAME=&REPORT_EXTENSION=Excel&CUSTOMER_ID=&MST_ID=" + entity.COLLECTION_MST_ID + "&REPORT_EXTENSION=" + 'Excel' + " & PREVIEW=" + 'NO';

        window.open(href, '_blank');

    }

    $scope.DataLoad = function (companyId) {

        $scope.showLoader = true;

        CollectionReverseServices.LoadData(companyId).then(function (data) {


            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = companyId;
        }, function (error) {

            alert(error);

            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        CollectionReverseServices.GetCompany().then(function (data) {

            $scope.model.COMPANY_ID = parseFloat(data.data);
            $interval(function () {
                $('#COMPANY_ID').trigger('change');
            }, 800, 4);
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }


    $scope.Update_Approval = function (entity) {
        $scope.showLoader = true;
        if (confirm("Are you sure you want to confirm this batch? You cannot edit it after confirmation.")) {
            // Code to execute if user clicks "OK" in the confirmation dialog
        } else {
            $scope.showLoader = false;
            return;
        }
        CollectionReverseServices.Update_Approval(entity.COLLECTION_MST_ID).then(function (data) {
            $scope.DataLoad(0);
            notificationservice.Notification(data.data.Status, 1, 'Posted Successfully !!');
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }



    $scope.ClearForm = function () {
        window.location = "/SalesAndDistribution/RegionRegionRelation/List";
    }



    $scope.GetPermissionData = function () {
        $scope.showLoader = true;

        $scope.permissionReqModel = {
            Controller_Name: 'CollectionReverse',
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

    $scope.DataLoad(0);
    $scope.GetPermissionData();
    $scope.CompanyLoad();


}]);

