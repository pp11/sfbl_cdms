ngApp.controller('ngGridCtrl', ['$scope', 'RequisitionServices', 'AreaInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, requisitionServices, AreaInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'

    $scope.model = {
        COMPANY_ID: 0,
        MST_ID: 0,
        MST_ID_ENCRYPTED: "",
        REQUISITION_UNIT_ID: 0,
        ISSUE_UNIT_ID: 0,
        REQUISITION_NO: ""
        , REQUISITION_DATE: ""
        , REQUISITION_AMOUNT: 0
        , REQUISITION_RAISE_BY: ""
        , STATUS: ""
        , REMAEKS: ""
        , requisitionDtlList: []
    }
    $scope.gridRequisitionList = (gridregistrationservice.GridRegistration("Requisition"));
    $scope.gridRequisitionList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridRequisitionList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }

        , { name: 'MST_ID', field: 'MST_ID', visible: false }

        , {
            name: 'REQUISITION_NO', field: 'REQUISITION_NO', displayName: 'Requisition No', enableFiltering: true, width: '20%'
        }
        , {
            name: 'ISSUE_UNIT_NAME', field: 'ISSUE_UNIT_NAME', displayName: 'Issue Unit', enableFiltering: true, width: '25%'
        }
        , {
            name: 'REQUISITION_AMOUNT', field: 'REQUISITION_AMOUNT', displayName: 'Requisition Amount', enableFiltering: true, width: '15%'
        }
        //, {
        //    name: 'STATUS', field: 'STATUS', displayName: 'Status', enableFiltering: true, width: '10%'
        //}
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remarks', enableFiltering: true, width: '20%'
        }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        }

    ];
    $scope.gridRequisitionList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.EditData = function (entity) {
        window.location = "/Inventory/Requisition/RequisitionRaise?Id=" + entity.MST_ID_ENCRYPTED;
    }

    $scope.DataLoad = function (companyId, customerId) {
        $scope.showLoader = true;

        requisitionServices.LoadData(companyId).then(function (data) {
            $scope.gridRequisitionList.data = data.data;
            $scope.showLoader = false;
            //$scope.model.COMPANY_SEARCH_ID = companyId;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;
        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        requisitionServices.GetCompany().then(function (data) {
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