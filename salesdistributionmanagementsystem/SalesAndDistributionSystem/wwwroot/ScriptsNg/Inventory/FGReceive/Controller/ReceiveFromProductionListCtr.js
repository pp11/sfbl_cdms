ngApp.controller('ngGridCtrl', ['$scope', 'ReceiveFromProductionServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, ReceiveFromProductionServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { COMPANY_ID: 0, DIVISION_ID: 0, DIVISION_CODE: '', DIVISION_REGION_MST_STATUS: 'Active', EFFECT_START_DATE: '', EFFECT_END_DATE: '', REMARKS: ''}



    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];
    /* -----------------------------------------------------------Search Filter Variables----------------------------------------------------------- */
    $scope.Status = [{ STATUS: 'Active' }, { STATUS: 'InActive' }, { STATUS: 'Complete' }]
    $scope.model.DATE_TO = localStorage.getItem("DATE_TO") == null ? $filter("date")(Date.now(), 'dd/MM/yyyy') : localStorage.getItem("DATE_TO");
    $scope.model.DATE_FROM = localStorage.getItem("DATE_FROM") == null ? $filter("date")(Date.now(), 'dd/MM/yyyy') : localStorage.getItem("DATE_FROM");
    $scope.ClearSearchForm = function () {
        $scope.model.DATE_TO = $filter("date")(Date.now(), 'dd/MM/yyyy');
        $scope.model.DATE_FROM = $filter("date")(Date.now(), 'dd/MM/yyyy');
        localStorage.removeItem("DATE_TO");
        localStorage.removeItem("DATE_FROM");
    };
    $scope.cacheInput = function ($event) {
        localStorage.setItem($event.target.name, $event.target.value);
    }
    /*-----------------------------------------------------------------END--------------------------------------------------------------------------*/
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Division Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'UNIT_ID', field: 'UNIT_ID', visible: false }
        
        , { name: 'RECEIVE_ID', field: 'RECEIVE_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }

        , {
            name: 'TRANSFER_NOTE_NO', field: 'TRANSFER_NOTE_NO', displayName: 'Transfer Note No', enableFiltering: true, width: '13%'
        }
        , {
            name: 'TRANSFER_DATE', field: 'TRANSFER_DATE', displayName: 'Date', enableFiltering: true, width: '12%'
        }
        , {
            name: 'RECEIVE_DATE', field: 'RECEIVE_DATE', displayName: 'Receive Date', enableFiltering: true, width: '12%'
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: true, width: '12%'
        }
        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '12%'
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '12%'
        }
        , {
            name: 'RECEIVE_QTY', field: 'RECEIVE_QTY', displayName: 'Receive Qty', enableFiltering: true, width: '12%'
        }
        //, {
        //    name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '13%'
        //}
        //, { name: 'RECEIVE_STATUS', field: 'RECEIVE_STATUS', displayName: 'Status', enableFiltering: true, width: '12%' }
        , {
            name: 'Action', displayName: 'Action', width: '10%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];
    $scope.gridOptionsList.rowTemplate = "<div  ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.EditData = function (entity) {
        
        window.location = "/Inventory/FGReceive/ReceiveFromProduction?Id=" + entity.RECEIVE_ID_ENCRYPTED;

    }
    
    $scope.DataLoad = function (model) {
        $scope.showLoader = true;
        ReceiveFromProductionServices.LoadData(model.COMPANY_ID, model.DATE_FROM, model.DATE_TO).then(function (data) {
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = model.COMPANY_ID;
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        ReceiveFromProductionServices.GetCompany().then(function (data) {
            
            $scope.model.COMPANY_ID = parseFloat(data.data);
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

        ReceiveFromProductionServices.GetCompanyList().then(function (data) {
            
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
   
   
    $scope.ClearForm = function () {
        window.location = "/Inventory/FGReceive/ReceiveFromProductionList";
    }

    

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'FGReceive',
            Action_Name: 'ReceiveFromProductionList'
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

    $scope.DataLoad($scope.model);
    $scope.GetPermissionData();
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
  

}]);

