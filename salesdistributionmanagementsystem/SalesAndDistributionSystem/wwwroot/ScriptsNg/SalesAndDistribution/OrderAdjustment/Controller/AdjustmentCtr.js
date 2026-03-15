ngApp.controller('ngGridCtrl', ['$scope', 'AdjustmentServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, AdjustmentServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { COMPANY_ID: 0, ID: 0, ADJUSTMENT_ID: 0, ADJUSTMENT_NAME: '', Order_ID: 0, ORDER_NO: '', ORDER_UNIT_ID: 0, REMARKS: '', ADJUSTMENT_AMOUNT: 0 }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];
    $scope.Adjustments = [];

    $scope.gridOptionsListAdj = (gridregistrationservice.GridRegistration("Ajustment Info"));
    $scope.gridOptionsListAdj.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsListAdj.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false,  width: '50'
        }

        , { name: 'ID', field: 'ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , { name: 'ORDER_UNIT_ID', field: 'ORDER_UNIT_ID', visible: false }
        , { name: 'ORDER_MST_ID', field: 'ORDER_MST_ID', visible: false }
        , { name: 'ADJUSTMENT_ID', field: 'ADJUSTMENT_ID', visible: false }

        , {
            name: 'ORDER_NO', field: 'ORDER_NO', displayName: 'Order No', enableFiltering: true, width: '20%'
        }
        , {
            name: 'ADJUSTMENT_NAME', field: 'ADJUSTMENT_NAME', displayName: 'Adjustment Name', enableFiltering: true, width: '12%'
        }
        , {
            name: 'ADJUSTMENT_AMOUNT', field: 'ADJUSTMENT_AMOUNT', displayName: 'Adjustment Amount', enableFiltering: true, width: '18%'
        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '15%'
        }
        ,{
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];
    $scope.gridOptionsListAdj.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.DataLoad = function (companyId) {
        
        $scope.showLoader = true;

        AdjustmentServices.LoadData(companyId).then(function (data) {
            
            
            $scope.gridOptionsListAdj.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = companyId;
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        AdjustmentServices.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);
            for (let i = 0; i < $scope.Companies.length; i++) {
                if (parseInt($scope.Companies[i].COMPANY_ID ) == $scope.model.COMPANY_ID) {
                    $scope.model.COMPANY_NAME = $scope.Companies[i].COMPANY_NAME;
                }
            }
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;

        AdjustmentServices.GetCompanyList().then(function (data) {
            
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.GetAdjustmentList = function () {
        
        $scope.showLoader = true;

        AdjustmentServices.GetAdjustmentList($scope.model.COMPANY_ID).then(function (data) {
            
            
            $scope.Adjustments = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    
    $scope.LoadFormData = function () {
        
        $scope.showLoader = true;

        AdjustmentServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.gridOptionsListAdj.data = data.data;
            $scope.showLoader = false;

            
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
  
    
    $scope.ClearForm = function () {
        $scope.model.ID = 0;
        $scope.model.ADJUSTMENT_ID =0;
        $scope.model.ORDER_NO = '';
        $scope.model.ORDER_MST_ID = 0;
        $scope.model.ADJUSTMENT_NAME = '';
        $scope.model.ADJUSTMENT_AMOUNT = 0;
        $scope.model.ORDER_UNIT_ID =0;
        $scope.model.Order_ID = 0;

        $scope.model.REMARKS ='';
    }

    $scope.EditData = function (entity) {
        
        $scope.model.ID = entity.ID;
        $scope.model.ADJUSTMENT_ID = entity.ADJUSTMENT_ID;
        $scope.model.ORDER_NO = entity.ORDER_NO;
        $scope.model.ORDER_MST_ID = entity.ORDER_MST_ID;
        $scope.model.ADJUSTMENT_NAME = entity.ADJUSTMENT_NAME;
        $scope.model.ADJUSTMENT_AMOUNT = entity.ADJUSTMENT_AMOUNT;
        $scope.model.ORDER_UNIT_ID = entity.ORDER_UNIT_ID;

        $scope.model.REMARKS = entity.REMARKS;
       

    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'OrderAdjustment',
            Action_Name: 'Adjustment'
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
    $scope.GetOrderData = function (order_mst_id, order_no, order_unit_id) {
        
        $scope.model.ORDER_MST_ID = parseInt(order_mst_id);
        $scope.model.ORDER_NO = order_no;
        $scope.model.ORDER_UNIT_ID = parseInt(order_unit_id);


    }
    $scope.DataLoad(0);
    $scope.GetPermissionData();
    $scope.CompaniesLoad();
    $scope.CompanyLoad();

    $scope.GetAdjustmentList();
    $scope.SaveData = function (model) {
        
        $scope.showLoader = true;
        $scope.model.ADJUSTMENT_ID = parseInt($scope.model.ADJUSTMENT_ID);

        AdjustmentServices.AddOrUpdate(model).then(function (data) {
            

            notificationservice.Notification(data.data.Status, 1, 'Data Save Successfully !!');
            if (data.data.Status == 1) {
                $scope.showLoader = false;
                $scope.GetPermissionData();
                $scope.DataLoad(0);
                $scope.model.ID = parseInt(data.data.Key);
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

}]);

