ngApp.controller('ngGridCtrl', ['$scope', 'DistributionService', 'AreaInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, distributionService, AreaInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {
        COMPANY_ID: 0,
        MST_ID: 0,
        MST_ID_ENCRYPTED: "",
        REQUISITION_UNIT_ID: 0,
        ISSUE_UNIT_ID: 0,
        REQUISITION_NO: ""
        , REQUISITION_DATE: ""
        , ISSUE_NO: ""
        , ISSUE_DATE: ""
        , REQUISITION_AMOUNT: 0
        , ISSUE_AMOUNT: 0
        , ISSUE_BY: ""
        , STATUS: ""
        , REMAEKS: ""
        , requisitionIssueDtlList: []
    }
    $scope.gridDistributionList = (gridregistrationservice.GridRegistration("Requisition"));
    $scope.gridDistributionList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridDistributionList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }

        , { name: 'MST_ID', field: 'MST_ID', visible: false }

        , {
            name: 'REQUISITION_NO', field: 'REQUISITION_NO', displayName: 'Requisition No', enableFiltering: true, width: '20%'
        }
        , {
            name: 'REQUISITION_UNIT_NAME', field: 'REQUISITION_UNIT_NAME', displayName: 'Requisition Unit Name', enableFiltering: true, width: '30%'
        }
        ,
        {
            name: 'ISSUE_UNIT_NAME', field: 'ISSUE_UNIT_NAME', displayName: 'Issue Unit Name', enableFiltering: true, width: '30%'
        }
        ,{
            name: 'REQUISITION_DATE', field: 'REQUISITION_DATE', displayName: 'Requisition Date', enableFiltering: true, width: '20%'
        }
      
       

    ];

    $scope.EditData = function (entity) {

        window.location = "/Inventory/RequisitionIssue/RequisitionIssue?Id=" + entity.MST_ID_ENCRYPTED;
    }

    $scope.DataLoad = function (companyId, customerId) {

        $scope.showLoader = true;

        distributionService.LoadRequisitionData(companyId).then(function (data) {

            
            
            $scope.gridDistributionList.data = data.data[0];
            $scope.showLoader = false;
            //$scope.model.COMPANY_SEARCH_ID = companyId;
        }, function (error) {

            alert(error);

            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        distributionService.GetCompany().then(function (data) {

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



