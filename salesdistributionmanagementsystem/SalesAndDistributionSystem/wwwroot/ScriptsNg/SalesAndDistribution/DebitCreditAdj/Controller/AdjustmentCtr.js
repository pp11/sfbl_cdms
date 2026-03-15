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
        { name: 'serialNo', displayName: 'SL', width: 50, cellTemplate: '<div class="ui-grid-cell-contents">{{grid.renderContainers.body.visibleRowCache.indexOf(row) + 1}}</div>' },
        { name: 'ID', displayName: 'ID', width: 80, visible: false },
        { name: 'ADJUSTMENT_NO', displayName: 'Adj No', width: 80 },
        { name: 'CUSTOMER_ID', displayName: 'Customer ID', width: 80, visible: false },
        { name: 'CUSTOMER_CODE', displayName: 'Cus. Code', width: 100 },
        { name: 'CUSTOMER_NAME', displayName: 'Customer Name', width: 200 },
        { name: 'ADJUSTMENT_AMOUNT', displayName: 'Adj Amount', width: 100 },
        { name: 'COMPANY_ID', displayName: 'Company ID', width: 100, visible: false },
        { name: 'POSTING_STATUS', displayName: 'Posting Status', width: 120 },
        { name: 'ENTERED_BY', displayName: 'Entered By', width: 120, visible: false },
        { name: 'ENTERED_DATE', displayName: 'Entered Date', width: 110 },
        { name: 'ENTERED_TERMINAL', displayName: 'Entered Terminal', width: 150, visible: false },
        { name: 'POSTED_BY', displayName: 'Posted By', width: 120, visible: false },
        { name: 'POSTED_DATE', displayName: 'Posted Date', width: 105 },
        { name: 'POSTED_TERMINAL', displayName: 'Posted Terminal', width: 150, visible: false },
        {
            name: 'Action', displayName: 'Action', width: 110, enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\' && row.entity.POSTING_STATUS !=\'Approved\' " ng-click="grid.appScope.EditData(row.entity)"    type="button"   class="btn btn-outline-primary mb-1"><i class="fa fa-edit"></i></button>' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\' && row.entity.POSTING_STATUS !=\'Approved\' " ng-click="grid.appScope.PostData(row.entity)"    type="button"   class="btn btn-outline-info mb-1">Post</button>' +
                '</div>'
        },
        { name: 'REMARKS', displayName: 'Remarks', width: 200 }
        

    ];

    $scope.Reload = function () {
        window.location = "/SalesAndDistribution/OrderAdjustment/DebitCreditAdj";

    }

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
    $scope.GetCustomerList = function () {
        
        $scope.showLoader = true;

        AdjustmentServices.GetCustomerList($scope.model.COMPANY_ID).then(function (data) {
            
            
            $scope.Customers = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    $scope.onSelectCustomer = function () {
        let customer = $scope.Customers.find(x => x.CUSTOMER_ID == $scope.model.CUSTOMER_ID);
        if (customer) {
            $scope.model.CUSTOMER_CODE = customer.CUSTOMER_CODE;
        } else {
            $scope.model.CUSTOMER_CODE = null; // or some default value
        }    }

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
        $scope.model.ADJUSTMENT_NO ='';
        $scope.model.ADJUSTMENT_AMOUNT = 0;
        $scope.model.REMARKS = '';
        $scope.model.CUSTOMER_ID = 0;
        $scope.model.CUSTOMER_CODE = '';
    }

    $scope.EditData = function (entity) {
        $scope.ClearForm();
        $scope.model = { ...$scope.model, ...entity };
        $timeout(function () {
            $('.select2-single').trigger('change');
        });
    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'OrderAdjustment',
            Action_Name: 'DebitCreditAdj'
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

    $scope.GetCustomerList();
    $scope.SaveData = function (model) {
        $scope.showLoader = true;
        model.CUSTOMER_ID = parseInt(model.CUSTOMER_ID)
        AdjustmentServices.AddOrUpdate(model).then(function (data) {
            notificationservice.Notification(data.data.Status, 1, 'Data Save Successfully !!');
            if (data.data.Status == 1) {
                $scope.showLoader = false;
                $scope.DataLoad(0);
                $scope.ClearForm();
            }
            else {
                $scope.showLoader = false;
                model.CUSTOMER_ID = model.CUSTOMER_ID.toString();

            }
        });
    }

    $scope.PostData = function (entity) {
        $scope.showLoader = true;
        entity.CUSTOMER_ID = parseInt(entity.CUSTOMER_ID)
        AdjustmentServices.PostDebitCreditAdj(entity).then(function (data) {
            notificationservice.Notification(data.data.Status, 1, 'Data Save Successfully !!');
            if (data.data.Status == 1) {
                $scope.showLoader = false;
                $scope.DataLoad(0);
                $scope.ClearForm();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

}]);

