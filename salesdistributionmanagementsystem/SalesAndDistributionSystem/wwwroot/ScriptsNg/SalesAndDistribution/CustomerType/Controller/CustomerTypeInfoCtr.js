ngApp.controller('ngGridCtrl', ['$scope', 'CustomerTypeInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, CustomerTypeInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { COMPANY_ID: 0, CUSTOMER_TYPE_ID: 0, CUSTOMER_TYPE_NAME: '', CUSTOMER_TYPE_CODE: '',  REMARKS: '', STATUS: 'Active' }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Customer Type Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false,  width: '50'
        }

        , { name: 'CUSTOMER_TYPE_ID', field: 'CUSTOMER_TYPE_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
     
        , {
            name: 'CUSTOMER_TYPE_NAME', field: 'CUSTOMER_TYPE_NAME', displayName: 'Name', enableFiltering: true, width: '22%'    
        }
        , {
            name: 'CUSTOMER_TYPE_CODE', field: 'CUSTOMER_TYPE_CODE', displayName: 'Code', enableFiltering: true, width: '15%'
        }
       
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '25%'
        }
        , { name: 'STATUS', field: 'STATUS', displayName: 'Status', enableFiltering: true, width: '18%' }
        ,{
            name: 'Action', displayName: 'Action', width: '18%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];

    $scope.DataLoad = function (companyId) {
        
        $scope.showLoader = true;

        CustomerTypeInfoServices.LoadData(companyId).then(function (data) {
            
            
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

        CustomerTypeInfoServices.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);
            for (let i = 0; i < $scope.Companies.length; i++) {
                if (parseInt($scope.Companies[i].COMPANY_ID) == $scope.model.COMPANY_ID) {
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

        CustomerTypeInfoServices.GetCompanyList().then(function (data) {
            
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.LoadGeneratedCustomerTypeCode = function () {
        
    }
    $scope.LoadStatus = function () {
        var Active = {
            STATUS: 'Active'
        }
        var InActive = {
            STATUS: 'InActive'
        }
        $scope.Status.push(Active);
        $scope.Status.push(InActive);

    }
    $scope.ClearForm = function () {
        $scope.model.CUSTOMER_TYPE_ID = 0;
        $scope.model.CUSTOMER_TYPE_NAME = '';
        $scope.model.CUSTOMER_TYPE_CODE = '';
        $scope.model.STATUS = 'Active';
        $scope.model.REMARKS = '';

    }
    $scope.LoadFormData = function () {
        
        $scope.showLoader = true;

        CustomerTypeInfoServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
            
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = $scope.model.COMPANY_ID;
            for (var i in $scope.gridOptionsList.data) {
                
                if ($scope.gridOptionsList.data[i].CUSTOMER_TYPE_NAME == $scope.model.CUSTOMER_TYPE_NAME) {
                    
                    $scope.model.CUSTOMER_TYPE_ID = $scope.gridOptionsList.data[i].CUSTOMER_TYPE_ID;
                    $scope.model.CUSTOMER_TYPE_NAME = $scope.gridOptionsList.data[i].CUSTOMER_TYPE_NAME;
                    $scope.model.CUSTOMER_TYPE_CODE = $scope.gridOptionsList.data[i].CUSTOMER_TYPE_CODE;
                    $scope.model.STATUS = $scope.gridOptionsList.data[i].STATUS;
                    $scope.model.REMARKS = $scope.gridOptionsList.data[i].REMARKS;


                }
            }
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.EditData = function (entity) {
        
        $scope.model.CUSTOMER_TYPE_ID = entity.CUSTOMER_TYPE_ID;
        $scope.model.CUSTOMER_TYPE_NAME = entity.CUSTOMER_TYPE_NAME;
        $scope.model.CUSTOMER_TYPE_CODE = entity.CUSTOMER_TYPE_CODE;
        $scope.model.COMPANY_ID = entity.COMPANY_ID;
        $scope.model.STATUS = entity.STATUS;
        $scope.model.REMARKS = entity.REMARKS;
      
    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'CustomerType',
            Action_Name: 'CustomerTypeInfo'
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
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
    $scope.LoadStatus();


    $scope.SaveData = function (model) {
        
        $scope.showLoader = true;
        
        CustomerTypeInfoServices.AddOrUpdate(model).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.GetPermissionData();
                $scope.CompanyLoad();
                $scope.LoadFormData();
            
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

}]);

