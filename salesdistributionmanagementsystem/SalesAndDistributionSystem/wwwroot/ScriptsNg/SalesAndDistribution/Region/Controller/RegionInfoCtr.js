ngApp.controller('ngGridCtrl', ['$scope', 'RegionInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, RegionInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    $scope.model = { COMPANY_ID: 0, REGION_ID: 0, REGION_NAME: '', REGION_CODE: '', REGION_ADDRESS: '', REMARKS: '', REGION_STATUS: 'Active' }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("RigionInfo"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false,  width: '50'
        }

        , { name: 'REGION_ID', field: 'REGION_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
     
        , {
            name: 'REGION_NAME', field: 'REGION_NAME', displayName: 'Name', enableFiltering: true, width: '20%'    
        }
        , {
            name: 'REGION_CODE', field: 'REGION_CODE', displayName: 'Code', enableFiltering: true, width: '12%'
        }
        , {
            name: 'REGION_ADDRESS', field: 'REGION_ADDRESS', displayName: 'Address', enableFiltering: true, width: '18%'
        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '15%'
        }
        , { name: 'REGION_STATUS', field: 'REGION_STATUS', displayName: 'Status', enableFiltering: true, width: '15%' }
        ,{
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];

    $scope.DataLoad = function (companyId) {
        
        $scope.showLoader = true;

        RegionInfoServices.LoadData(companyId).then(function (data) {
            
            
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = companyId;
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    $scope.LoadFormData = function () {
        
        $scope.showLoader = true;

        RegionInfoServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = $scope.model.COMPANY_ID;

            for (var i in $scope.gridOptionsList.data) {
                
                if ($scope.gridOptionsList.data[i].REGION_NAME == $scope.model.REGION_NAME) {
                    
                    $scope.model.REGION_ID = $scope.gridOptionsList.data[i].REGION_ID;
                    $scope.model.REGION_NAME = $scope.gridOptionsList.data[i].REGION_NAME;
                    $scope.model.REGION_CODE = $scope.gridOptionsList.data[i].REGION_CODE;
                    $scope.model.COMPANY_ID = $scope.gridOptionsList.data[i].COMPANY_ID;
                    $scope.model.REGION_ADDRESS = $scope.gridOptionsList.data[i].REGION_ADDRESS;
                    $scope.model.REGION_STATUS = $scope.gridOptionsList.data[i].REGION_STATUS;
                    $scope.model.REMARKS = $scope.gridOptionsList.data[i].REMARKS;
                }
            }
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        RegionInfoServices.GetCompany().then(function (data) {
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

        RegionInfoServices.GetCompanyList().then(function (data) {
            
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.LoadGeneratedRigionCode = function () {
        //$scope.showLoader = true;
        //
        //RegionInfoServices.GenerateRigionCode($scope.model.COMPANY_ID).then(function (data) {
        //    
        //    $scope.model.REGION_CODE = data.data;
        //    $scope.showLoader = false;
        //}, function (error) {
        //    alert(error);
        
        //    $scope.showLoader = false;

        //});
    }
    $scope.LoadStatus = function () {
        var Active = {
            REGION_STATUS: 'Active'
        }
        var InActive = {
            REGION_STATUS: 'InActive'
        }
        $scope.Status.push(Active);
        $scope.Status.push(InActive);

    }
    $scope.ClearForm = function () {
        $scope.model.REGION_ID = 0;
        $scope.model.REGION_NAME = '';
        $scope.model.REGION_CODE = '';
        $scope.model.REGION_ADDRESS = '';
        $scope.model.REGION_STATUS = 'Active';
        $scope.model.REMARKS = '';

    }

    $scope.EditData = function (entity) {
        
        $scope.model.REGION_ID = entity.REGION_ID;
        $scope.model.REGION_NAME = entity.REGION_NAME;
        $scope.model.REGION_CODE = entity.REGION_CODE;
        $scope.model.COMPANY_ID = entity.COMPANY_ID;
        $scope.model.REGION_ADDRESS = entity.REGION_ADDRESS;
        $scope.model.REGION_STATUS = entity.REGION_STATUS;
        $scope.model.REMARKS = entity.REMARKS;

    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'Region',
            Action_Name: 'RegionInfo'
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
    //$scope.LoadGeneratedRigionCode();


    $scope.SaveData = function (model) {
        
        $scope.showLoader = true;
        
        RegionInfoServices.AddOrUpdate(model).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.GetPermissionData();
                $scope.CompanyLoad();
                //$scope.LoadGeneratedRigionCode();
                $scope.LoadFormData();
            
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

}]);

