ngApp.controller('ngGridCtrl', ['$scope', 'PackSizeInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, PackSizeInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { COMPANY_ID: 0, PACK_SIZE_ID: 0, PACK_SIZE_NAME: '',  REMARKS: '', STATUS: 'Active' }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Pack Size Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false,  width: '50'
        }

        , { name: 'PACK_SIZE_ID', field: 'PACK_SIZE_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
     
        , {
            name: 'PACK_SIZE_NAME', field: 'PACK_SIZE_NAME', displayName: 'Name', enableFiltering: true, width: '26%'    
        }

       
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '30%'
        }
        , { name: 'STATUS', field: 'STATUS', displayName: 'Status', enableFiltering: true, width: '20%' }
        ,{
            name: 'Action', displayName: 'Action', width: '18%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];

    $scope.DataLoad = function (companyId) {
        
        $scope.showLoader = true;

        PackSizeInfoServices.LoadData(companyId).then(function (data) {
            
            
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

        PackSizeInfoServices.GetCompany().then(function (data) {
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

        PackSizeInfoServices.GetCompanyList().then(function (data) {
            
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
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
        $scope.model.PACK_SIZE_ID = 0;
        $scope.model.PACK_SIZE_NAME = '';
        $scope.model.STATUS = 'Active';
        $scope.model.REMARKS = '';

    }

    $scope.EditData = function (entity) {
        
        $scope.model.PACK_SIZE_ID = entity.PACK_SIZE_ID;
        $scope.model.PACK_SIZE_NAME = entity.PACK_SIZE_NAME;
        $scope.model.COMPANY_ID = entity.COMPANY_ID;
        $scope.model.STATUS = entity.STATUS;
        $scope.model.REMARKS = entity.REMARKS;
       
    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'PackSize',
            Action_Name: 'PackSizeInfo'
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
        
        PackSizeInfoServices.AddOrUpdate(model).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.DataLoad(0);
                $scope.GetPermissionData();
                $scope.CompaniesLoad();
                $scope.CompanyLoad();
                $scope.LoadStatus();
                $scope.ClearForm();
                window.setTimeout(function () {
                    window.location.href = "/SalesAndDistribution/PackSize/PackSizeInfo";
                }, 2000)
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

}]);

