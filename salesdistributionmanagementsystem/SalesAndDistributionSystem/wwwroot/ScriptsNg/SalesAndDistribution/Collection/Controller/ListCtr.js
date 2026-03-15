ngApp.controller('ngGridCtrl', ['$scope', 'InsertOrEditServices',  'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InsertOrEditServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { COMPANY_ID: 0, REGION_ID: 0, REGION_CODE: '', REGION_AREA_MST_STATUS: 'Active', EFFECT_START_DATE: '', EFFECT_END_DATE: '', REMARKS: ''}
    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Region Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        
        , { name: 'COLLECTION_MST_ID', field: 'COLLECTION_MST_ID', visible: false }
        
        , { name: 'REGION_ID', field: 'REGION_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }

        , {

            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: true, width: '15%'

        }
        , {
            name: 'BATCH_DATE', field: 'BATCH_DATE', displayName: 'Batch Date', enableFiltering: true, width: '17%'
        }
        , {
            name: 'BATCH_POSTING_STATUS', field: 'BATCH_POSTING_STATUS', displayName: 'Posting Status', enableFiltering: true, width: '17%'
        }
        , {
            name: 'BATCH_STATUS', field: 'BATCH_STATUS', displayName: 'Batch Status', enableFiltering: true, width: '17%'
        }
         , {
            name: 'Action', displayName: 'Action', width: '35%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                 '<button style="margin-bottom: 5px;margin-left: 10px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1" ng-disabled="row.entity.BATCH_POSTING_STATUS == \'Approved\'">Update</button>' +
                 '<button style="margin-bottom: 5px;margin-left: 10px;"/* ng-show="grid.appScope.model.DELETE_PERMISSION == \'Active\'"*/ ng-click="grid.appScope.GetPdfView(row.entity)" type="button" class="btn btn btn-outline-info mb-1"><i class="fas fa-file-pdf"></i> Pdf</button>' +
                 '<button style="margin-bottom: 5px;margin-left: 10px;"/* ng-show="grid.appScope.model.DELETE_PERMISSION == \'Active\'"*/ ng-click="grid.appScope.GetExcelView(row.entity)" type="button" class="btn btn btn-outline-info mb-1"><i class="fas fa-file-excel"></i> Excel</button>' +
                 '<button style="margin-bottom: 5px;margin-left: 10px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'"  ng-click="grid.appScope.Update_Approval(row.entity)" ng-disabled="row.entity.BATCH_POSTING_STATUS == \'Approved\'" type="button" class="btn btn btn-success mb-1"> Post</button>' +

                 '</div>'

        },
        
    ];
    $scope.gridOptionsList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.EditData = function (entity) {
        
        window.location = "/SalesAndDistribution/Collection/InsertOrEdit?Id=" + entity.COLLECTION_MST_ID_ENCRYPTED;

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

        InsertOrEditServices.LoadData(companyId).then(function (data) {
            
            
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

         InsertOrEditServices.GetCompany().then(function (data) {
            
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

        InsertOrEditServices.Update_Approval(entity.COLLECTION_MST_ID).then(function (data) {
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
            Controller_Name: 'Collection',
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

