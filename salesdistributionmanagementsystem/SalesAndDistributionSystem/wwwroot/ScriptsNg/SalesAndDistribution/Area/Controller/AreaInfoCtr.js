ngApp.controller('ngGridCtrl', ['$scope', 'AreaInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, AreaInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { COMPANY_ID: 0, AREA_ID: 0, AREA_NAME: '', AREA_CODE: '', AREA_ADDRESS: '', REMARKS: '', AREA_STATUS: 'Active' }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration(" Area Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false,  width: '50'
        }

        , { name: 'AREA_ID', field: 'AREA_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
     
        , {
            name: 'AREA_NAME', field: 'AREA_NAME', displayName: 'Name', enableFiltering: true, width: '20%'    
        }
        , {
            name: 'AREA_CODE', field: 'AREA_CODE', displayName: 'Code', enableFiltering: true, width: '12%'
        }
        , {
            name: 'AREA_ADDRESS', field: 'AREA_ADDRESS', displayName: 'Address', enableFiltering: true, width: '18%'
        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '15%'
        }
        , { name: 'AREA_STATUS', field: 'AREA_STATUS', displayName: 'Status', enableFiltering: true, width: '15%' }
        ,{
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];
    $scope.gridOptionsList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.DataLoad = function (companyId) {
        
        $scope.showLoader = true;

        AreaInfoServices.LoadData(companyId).then(function (data) {
            
            
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

        AreaInfoServices.GetCompany().then(function (data) {
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

        AreaInfoServices.GetCompanyList().then(function (data) {
            
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.LoadFormData = function () {
        
        $scope.showLoader = true;

        AreaInfoServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = $scope.model.COMPANY_ID;

            for (var i in $scope.gridOptionsList.data) {
                
                if ($scope.gridOptionsList.data[i].AREA_NAME == $scope.model.AREA_NAME) {
                    
                    $scope.model.AREA_ID = $scope.gridOptionsList.data[i].AREA_ID;
                    $scope.model.AREA_NAME = $scope.gridOptionsList.data[i].AREA_NAME;
                    $scope.model.AREA_CODE = $scope.gridOptionsList.data[i].AREA_CODE;
                    $scope.model.COMPANY_ID = $scope.gridOptionsList.data[i].COMPANY_ID;
                    $scope.model.AREA_ADDRESS = $scope.gridOptionsList.data[i].AREA_ADDRESS;
                    $scope.model.AREA_STATUS = $scope.gridOptionsList.data[i].AREA_STATUS;
                    $scope.model.REMARKS = $scope.gridOptionsList.data[i].REMARKS;
                }
            }
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.LoadGeneratedAreaCode = function () {
        //$scope.showLoader = true;
        //
        //AreaInfoServices.GenerateAreaCode($scope.model.COMPANY_ID).then(function (data) {
        //    $scope.showLoader = false;
        //}, function (error) {
        //    alert(error);
        
        //    $scope.showLoader = false;
        //});
    }
    $scope.LoadStatus = function () {
        var Active = {
            AREA_STATUS: 'Active'
        }
        var InActive = {
            AREA_STATUS: 'InActive'
        }
        $scope.Status.push(Active);
        $scope.Status.push(InActive);

    }
    $scope.ClearForm = function () {
        $scope.model.AREA_ID = 0;
        $scope.model.AREA_NAME = '';
        $scope.model.AREA_CODE = '';
        $scope.model.AREA_ADDRESS = '';
        $scope.model.AREA_STATUS = 'Active';
        $scope.model.REMARKS = '';

    }

    $scope.EditData = function (entity) {
        
        $scope.model.AREA_ID = entity.AREA_ID;
        $scope.model.AREA_NAME = entity.AREA_NAME;
        $scope.model.AREA_CODE = entity.AREA_CODE;
        $scope.model.COMPANY_ID = entity.COMPANY_ID;
        $scope.model.AREA_ADDRESS = entity.AREA_ADDRESS;
        $scope.model.AREA_STATUS = entity.AREA_STATUS;
        $scope.model.REMARKS = entity.REMARKS;
       

    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'Area',
            Action_Name: 'AreaInfo'
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
        
        AreaInfoServices.AddOrUpdate(model).then(function (data) {

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

