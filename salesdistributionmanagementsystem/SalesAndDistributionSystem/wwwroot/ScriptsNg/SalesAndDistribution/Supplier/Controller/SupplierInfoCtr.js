ngApp.controller('ngGridCtrl', ['$scope', 'SupplierInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, SupplierInfoService, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { SUPPLIER_ID: 0, SUPPLIER_NAME: '', ADDRESS: '', EMAIL: '', PHONE_NO: '', MOBILE_NO: '', SUPPLIER_TYPE: "Supplier", STATUS: '', COMPANY_ID: 0, REMARKS: '' }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];
    $scope.Types = ["Supplier", "Toll"];
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration(" Supplier Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }

        , { name: 'SUPPLIER_ID', field: 'SUPPLIER_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }

        , {
            name: 'SUPPLIER_NAME', field: 'SUPPLIER_NAME', displayName: 'Name', enableFiltering: true, width: '20%'
        }
        , {
            name: 'ADDRESS', field: 'ADDRESS', displayName: 'Address', enableFiltering: true, width: '12%'
        }
        , {
            name: 'EMAIL', field: 'EMAIL', displayName: 'Email', enableFiltering: true
        }
        , {
            name: 'PHONE_NO', field: 'PHONE_NO', displayName: 'Phone', enableFiltering: true
        }
        , {
            name: 'MOBILE_NO', field: 'MOBILE_NO', displayName: 'Mobile', enableFiltering: true
        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true,
        }
        , {
            name: 'SUPPLIER_TYPE', field: 'SUPPLIER_TYPE', displayName: 'Type', enableFiltering: true
        }
        , { name: 'STATUS', field: 'STATUS', displayName: 'Status', enableFiltering: true }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];
    $scope.gridOptionsList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.DataLoad = function (companyId) {
        
        $scope.showLoader = true;

        SupplierInfoService.LoadData(companyId).then(function (data) {
            
            
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = companyId;
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {

        SupplierInfoService.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.CompanyNameLoad = function () {

        SupplierInfoService.GetCompanyName().then(function (data) {
            $scope.model.COMPANY_NAME = data.data;


            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }



    $scope.LoadFormData = function () {
        
        $scope.showLoader = true;

        SupplierInfoService.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = $scope.model.COMPANY_ID;

            for (var i in $scope.gridOptionsList.data) {
                
                if ($scope.gridOptionsList.data[i].SUPPLIER_NAME == $scope.model.SUPPLIER_NAME) {
                    
                    $scope.model.SUPPLIER_ID = $scope.gridOptionsList.data[i].SUPPLIER_ID;
                    $scope.model.SUPPLIER_NAME = $scope.gridOptionsList.data[i].SUPPLIER_NAME;
                    $scope.model.ADDRESS = $scope.gridOptionsList.data[i].ADDRESS;
                    $scope.model.COMPANY_ID = $scope.gridOptionsList.data[i].COMPANY_ID;
                    $scope.model.EMAIL = $scope.gridOptionsList.data[i].EMAIL;
                    $scope.model.PHONE_NO = $scope.gridOptionsList.data[i].PHONE_NO;
                    $scope.model.MOBILE_NO = $scope.gridOptionsList.data[i].MOBILE_NO;
                    $scope.model.STATUS = $scope.gridOptionsList.data[i].STATUS;
                    $scope.model.REMARKS = $scope.gridOptionsList.data[i].REMARKS;
                    $scope.model.SUPPLIER_TYPE = $scope.gridOptionsList.data[i].SUPPLIER_TYPE;
                }
            }
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
        $scope.model.SUPPLIER_ID = 0;
        $scope.model.SUPPLIER_NAME = '';
        $scope.model.ADDRESS = '';
        $scope.model.EMAIL = '';
        $scope.model.PHONE_NO = '';
        $scope.model.MOBILE_NO = '';
        $scope.model.STATUS = 'Active';
        $scope.model.REMARKS = '';
        $scope.model.SUPPLIER_TYPE = 'Supplier';
    }

    $scope.EditData = function (entity) {
        
        $scope.model.COMPANY_ID = entity.COMPANY_ID;
        $scope.model.SUPPLIER_ID = entity.SUPPLIER_ID;
        $scope.model.SUPPLIER_NAME = entity.SUPPLIER_NAME;
        $scope.model.ADDRESS = entity.ADDRESS;
        $scope.model.EMAIL = entity.EMAIL;
        $scope.model.PHONE_NO = entity.PHONE_NO;
        $scope.model.MOBILE_NO = entity.MOBILE_NO;
        $scope.model.STATUS = entity.STATUS;
        $scope.model.REMARKS = entity.REMARKS;
        $scope.model.SUPPLIER_TYPE = entity.SUPPLIER_TYPE;
        $interval(function () {
            $('#COMPANY_ID').trigger('change');
        }, 800, 4);

    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'Supplier',
            Action_Name: 'SupplierInfo'
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
    $scope.CompanyLoad();
    $scope.CompanyNameLoad();

    $scope.LoadStatus();


    $scope.SaveData = function (model) {
        

        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);

        $scope.showLoader = true;
        

        SupplierInfoService.AddOrUpdate(model).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.GetPermissionData();
                $scope.CompanyLoad();
                $scope.CompanyNameLoad();
                $scope.LoadFormData();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

}]);

