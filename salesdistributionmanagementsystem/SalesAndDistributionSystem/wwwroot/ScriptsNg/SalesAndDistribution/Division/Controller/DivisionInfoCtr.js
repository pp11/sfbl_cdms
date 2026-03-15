ngApp.controller('ngGridCtrl', ['$scope', 'DivisionInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, DivisionInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { COMPANY_ID: 0, DIVISION_ID: 0, DIVISION_NAME: '', DIVISION_CODE: '', DIVISION_ADDRESS: '', REMARKS: '', DIVISION_STATUS: 'Active' }

   


    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Division Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false,  width: '50'
        }

        , { name: 'DIVISION_ID', field: 'DIVISION_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
     
        , {
            name: 'DIVISION_NAME', field: 'DIVISION_NAME', displayName: 'Name', enableFiltering: true, width: '20%'    
        }
        , {
            name: 'DIVISION_CODE', field: 'DIVISION_CODE', displayName: 'Code', enableFiltering: true, width: '12%'
        }
        , {
            name: 'DIVISION_ADDRESS', field: 'DIVISION_ADDRESS', displayName: 'Address', enableFiltering: true, width: '18%'
        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '15%'
        }
        , { name: 'DIVISION_STATUS', field: 'DIVISION_STATUS', displayName: 'Status', enableFiltering: true, width: '15%' }
        ,{
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];

    $scope.DataLoad = function (companyId) {
        
        $scope.showLoader = true;

        DivisionInfoServices.LoadData(companyId).then(function (data) {
            
            
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

        DivisionInfoServices.GetCompany().then(function (data) {
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

        DivisionInfoServices.GetCompanyList().then(function (data) {
            
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.LoadGeneratedDivisionCode = function () {
        $scope.showLoader = true;
        
        DivisionInfoServices.GenerateDivisionCode($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.model.DIVISION_CODE = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.LoadStatus = function () {
        var Active = {
            DIVISION_STATUS: 'Active'
        }
        var InActive = {
            DIVISION_STATUS: 'InActive'
        }
        $scope.Status.push(Active);
        $scope.Status.push(InActive);

    }
    $scope.ClearForm = function () {
        $scope.model.DIVISION_ID = 0;
        $scope.model.DIVISION_NAME = '';
        $scope.model.DIVISION_CODE = '';
        $scope.model.DIVISION_ADDRESS = '';
        $scope.model.DIVISION_STATUS = 'Active';
        $scope.model.REMARKS = '';

    }
    $scope.LoadFormData = function () {
        
        $scope.showLoader = true;
        DivisionInfoServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = $scope.model.COMPANY_ID;
            for (var i in $scope.gridOptionsList.data) {
                
                if ($scope.gridOptionsList.data[i].DIVISION_NAME == $scope.model.DIVISION_NAME) {
                                    

                    $scope.model.DIVISION_ID = $scope.gridOptionsList.data[i].DIVISION_ID;
                    $scope.model.DIVISION_NAME = $scope.gridOptionsList.data[i].DIVISION_NAME;
                    $scope.model.DIVISION_CODE = $scope.gridOptionsList.data[i].DIVISION_CODE;
                    $scope.model.DIVISION_ADDRESS = $scope.gridOptionsList.data[i].DIVISION_ADDRESS;

                    $scope.model.DIVISION_STATUS = $scope.gridOptionsList.data[i].DIVISION_STATUS;
                    $scope.model.REMARKS = $scope.gridOptionsList.data[i].REMARKS;
                }
            }
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
        


    }

    $scope.EditData = function (entity) {
        
        $scope.model.DIVISION_ID = entity.DIVISION_ID;
        $scope.model.DIVISION_NAME = entity.DIVISION_NAME;
        $scope.model.DIVISION_CODE = entity.DIVISION_CODE;
        $scope.model.COMPANY_ID = entity.COMPANY_ID;
        $scope.model.DIVISION_ADDRESS = entity.DIVISION_ADDRESS;
        $scope.model.DIVISION_STATUS = entity.DIVISION_STATUS;
        $scope.model.REMARKS = entity.REMARKS;
        
    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'Division',
            Action_Name: 'DivisionInfo'
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
    //$scope.LoadGeneratedDivisionCode();  --as poer dicision to integret with previous data, DivisionCode will be given menually


    $scope.SaveData = function (model) {
        
        $scope.showLoader = true;
        
        DivisionInfoServices.AddOrUpdate(model).then(function (data) {

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

