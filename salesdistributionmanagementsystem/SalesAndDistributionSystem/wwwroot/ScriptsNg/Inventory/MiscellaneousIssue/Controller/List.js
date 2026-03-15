ngApp.controller('ngGridCtrl', ['$scope', 'MiscellaneousIssueService', 'AreaInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, MiscellaneousIssueService, AreaInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {
        COMPANY_ID: 0,
        MST_ID: 0,
        MST_ID_ENCRYPTED: "",
        UNIT_ID: 0,
        ISSUE_NO: ""
        , ISSUE_DATE: ""
        , SUBJECT: ""
        , RAISED_FROM: ""
        , ISSUE_BY: ""
        , STATUS: "Active"
        , REMARKS: ""
        , MiscellaneousIssueDtlList: []
    }
    /* Search Filter Variables */
    $scope.Status = [{ STATUS: 'Active' }, { STATUS: 'InActive' }, { STATUS: 'Complete' }]
    $scope.model.DATE_TO = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.model.DATE_FROM = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.ClearSearchForm = function () {
        $scope.model.DATE_TO = $filter("date")(Date.now(), 'dd/MM/yyyy');
        $scope.model.DATE_FROM = $filter("date")(Date.now(), 'dd/MM/yyyy');
    };

    $scope.gridMiscIssueList = (gridregistrationservice.GridRegistration("Misc. Issue"));
    $scope.gridMiscIssueList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridMiscIssueList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'MST_ID', field: 'MST_ID', visible: false }
        , { name: 'UNIT_ID', field: 'UNIT_ID', visible: false }
        , {
            name: 'ISSUE_NO', field: 'ISSUE_NO', displayName: 'Issue No', enableFiltering: true, width: '8%'
        }        
        ,{
            name: 'ISSUE_DATE', field: 'ISSUE_DATE', displayName: 'Issue Date', enableFiltering: true, width: '8%'
        }
        ,{
            name: 'SUBJECT', field: 'SUBJECT', displayName: 'SUBJECT', enableFiltering: true, width: '20%'
        }
        ,
        {
            name: 'RAISED_FROM', field: 'RAISED_FROM', displayName: 'RAISED_FROM', enableFiltering: true, width: '20%'
        }
        
        ,
        {
            name: 'ISSUE_BY', field: 'ISSUE_BY', displayName: 'ISSUE_BY', visible:false,  enableFiltering: true, width: '10%'
        }

        , {
            name: 'ISSUE_AMOUNT', field: 'ISSUE_AMOUNT', displayName: 'Issue Amount', enableFiltering: true, width: '10%'
        }
        , {
            name: 'COMPANY_ID', field: 'COMPANY_ID', displayName: 'COMPANY_ID', visible: false, enableFiltering: true, width: '10%'
        }

        , {
            name: 'STATUS', field: 'STATUS', displayName: 'Status', enableFiltering: true, width: '10%'

        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'REMARKS', enableFiltering: true, width: '10%'

        }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        }

    ];

    $scope.EditData = function (entity) {
        
        window.location = "/Inventory/MiscellaneousIssue/MiscellaneousIssue?Id=" + entity.MST_ID_ENCRYPTED;
    }

    $scope.DataLoad = function (model) {
        
        $scope.showLoader = true;

        MiscellaneousIssueService.LoadData(model.COMPANY_ID, model.DATE_FROM, model.DATE_TO).then(function (data) {
            
            
            $scope.gridMiscIssueList.data = data.data;
            $scope.showLoader = false;
            //$scope.model.COMPANY_SEARCH_ID = companyId;
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;
        MiscellaneousIssueService.GetCompanyId().then(function (data) {
            $scope.model.COMPANY_ID = data.data;
            $scope.DataLoad($scope.model);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });

    }


    //$scope.GetPermissionData = function () {
    //    $scope.showLoader = true;
    //    
    //    $scope.permissionReqModel = {
    //        Controller_Name: 'RegionAreaRelation',
    //        Action_Name: 'List'
    //    }
    //    permissionProvider.GetPermission($scope.permissionReqModel).then(function (data) {
    //        
    //        
    //        $scope.getPermissions = data.data;
    //        $scope.model.ADD_PERMISSION = $scope.getPermissions.adD_PERMISSION;
    //        $scope.model.EDIT_PERMISSION = $scope.getPermissions.ediT_PERMISSION;
    //        $scope.model.DELETE_PERMISSION = $scope.getPermissions.deletE_PERMISSION;
    //        $scope.model.LIST_VIEW = $scope.getPermissions.lisT_VIEW;
    //        $scope.model.DETAIL_VIEW = $scope.getPermissions.detaiL_VIEW;
    //        $scope.model.DOWNLOAD_PERMISSION = $scope.getPermissions.downloaD_PERMISSION;
    //        $scope.model.USER_TYPE = $scope.getPermissions.useR_TYPE;

    //        $scope.showLoader = false;
    //    }, function (error) {
    //        alert(error);
    
    //        $scope.showLoader = false;

    //    });
    //}

    //$scope.DataLoad(0);
    //$scope.GetPermissionData();
    //$scope.CompaniesLoad();
    $scope.CompanyLoad();
}]);



