ngApp.controller('ngGridCtrl', ['$scope', 'MarketInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, MarketInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { COMPANY_ID: 0, MARKET_ID: 0, MARKET_NAME: '', MARKET_CODE: '', MARKET_ADDRESS: '', REMARKS: '', MARKET_STATUS: 'Active' }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Market Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false,  width: '50'
        }

        , { name: 'MARKET_ID', field: 'MARKET_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
     
        , {
            name: 'MARKET_NAME', field: 'MARKET_NAME', displayName: 'Name', enableFiltering: true, width: '20%'    
        }
        , {
            name: 'MARKET_CODE', field: 'MARKET_CODE', displayName: 'Code', enableFiltering: true, width: '12%'
        }
        , {
            name: 'MARKET_ADDRESS', field: 'MARKET_ADDRESS', displayName: 'Address', enableFiltering: true, width: '18%'
        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '15%'
        }
        , { name: 'MARKET_STATUS', field: 'MARKET_STATUS', displayName: 'Status', enableFiltering: true, width: '15%' }
        ,{
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];

    $scope.DataLoad = function (companyId) {
        
        $scope.showLoader = true;

        MarketInfoServices.LoadData(companyId).then(function (data) {
            
            
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

        MarketInfoServices.GetCompany().then(function (data) {
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

        MarketInfoServices.GetCompanyList().then(function (data) {
            
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.LoadGeneratedMarketCode = function () {
       
    }
    $scope.LoadStatus = function () {
        var Active = {
            MARKET_STATUS: 'Active'
        }
        var InActive = {
            MARKET_STATUS: 'InActive'
        }
        $scope.Status.push(Active);
        $scope.Status.push(InActive);

    }
    $scope.ClearForm = function () {
        $scope.model.MARKET_ID = 0;
        $scope.model.MARKET_NAME = '';
        $scope.model.MARKET_CODE = '';
        $scope.model.MARKET_ADDRESS = '';
        $scope.model.MARKET_STATUS = 'Active';
        $scope.model.REMARKS = '';

    }

    $scope.EditData = function (entity) {
        
        $scope.model.MARKET_ID = entity.MARKET_ID;
        $scope.model.MARKET_NAME = entity.MARKET_NAME;
        $scope.model.MARKET_CODE = entity.MARKET_CODE;
        $scope.model.COMPANY_ID = entity.COMPANY_ID;
        $scope.model.MARKET_ADDRESS = entity.MARKET_ADDRESS;
        $scope.model.MARKET_STATUS = entity.MARKET_STATUS;
        $scope.model.REMARKS = entity.REMARKS;

    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'Market',
            Action_Name: 'MarketInfo'
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

    $scope.LoadFormData = function () {
        
        $scope.showLoader = true;

        MarketInfoServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = $scope.model.COMPANY_ID;
            for (var i in $scope.gridOptionsList.data) {
                
                if ($scope.gridOptionsList.data[i].MARKET_NAME == $scope.model.MARKET_NAME) {
                    $scope.model.MARKET_ID = $scope.gridOptionsList.data[i].MARKET_ID;
                    $scope.model.MARKET_NAME = $scope.gridOptionsList.data[i].MARKET_NAME;
                    $scope.model.MARKET_CODE = $scope.gridOptionsList.data[i].MARKET_CODE;
                    $scope.model.COMPANY_ID = $scope.gridOptionsList.data[i].COMPANY_ID;
                    $scope.model.MARKET_ADDRESS = $scope.gridOptionsList.data[i].MARKET_ADDRESS;
                    $scope.model.MARKET_STATUS = $scope.gridOptionsList.data[i].MARKET_STATUS;
                    $scope.model.REMARKS = $scope.gridOptionsList.data[i].REMARKS;
                }
            }
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.SaveData = function (model) {
        
        $scope.showLoader = true;
        
        MarketInfoServices.AddOrUpdate(model).then(function (data) {

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

