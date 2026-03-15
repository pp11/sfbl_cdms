ngApp.controller('ngGridCtrl', ['$scope', 'DriverInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, DriverInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { DRIVER_ID: 0, DRIVER_NAME: '', CONTACT_NO: '', STATUS: '', COMPANY_ID: 0, REMARKS: '' }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];
    $scope.Units = [];
    $scope.buttonName = 'Save';
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration(" Driver Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }

        , { name: 'DRIVER_ID', field: 'DRIVER_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }

        , {
            name: 'DRIVER_NAME', field: 'DRIVER_NAME', displayName: 'Name', enableFiltering: true, width: '20%'
        }
        , {
            name: 'CONTACT_NO', field: 'CONTACT_NO', displayName: 'Contact No', enableFiltering: true, width: '18%'
        }
        , {
            name: 'UNIT_NAME', field: 'UNIT_NAME', displayName: 'Depot Name', enableFiltering: true, width: '18%'
        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '15%'
        }
        , { name: 'STATUS', field: 'STATUS', displayName: 'Status', enableFiltering: true, width: '15%' }
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

        DriverInfoServices.LoadData(companyId).then(function (data) {
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

        DriverInfoServices.GetCompany().then(function (data) {
            
            $scope.model.COMPANY_ID = parseFloat(data.data);
            $interval(function () {
                $('#COMPANY_ID').trigger('change');
            }, 800, 4);
            $scope.showLoader = false;

            $scope.LoadUnitData();

        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;

        DriverInfoServices.GetCompanyList().then(function (data) {
            
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    $scope.LoadFormData = function () {
        $scope.showLoader = true;
        DriverInfoServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = $scope.model.COMPANY_ID;

            //for (var i in $scope.gridOptionsList.data) {
                
            //    if ($scope.gridOptionsList.data[i].DRIVER_NAME == $scope.model.DRIVER_NAME) {
                    
            //        $scope.model.DRIVER_ID = $scope.gridOptionsList.data[i].DRIVER_ID;
            //        $scope.model.DRIVER_NAME = $scope.gridOptionsList.data[i].DRIVER_NAME;
            //        $scope.model.COMPANY_ID = $scope.gridOptionsList.data[i].COMPANY_ID;
            //        $scope.model.CONTACT_NO = $scope.gridOptionsList.data[i].CONTACT_NO;
            //        $scope.model.STATUS = $scope.gridOptionsList.data[i].STATUS;
            //        $scope.model.REMARKS = $scope.gridOptionsList.data[i].REMARKS;
            //    }
            //}
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    $scope.LoadUnitData = function () {
        $scope.showLoader = true;

        DriverInfoServices.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Units = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });

            DriverInfoServices.GetUnit().then(response => {
                $scope.model.UNIT_ID = response.data
            })
            $scope.showLoader = false;
        }, function (error) {
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
        $scope.model.DRIVER_ID = 0;
        $scope.model.DRIVER_NAME = '';
        $scope.model.CONTACT_NO = '';
        $scope.model.STATUS = 'Active';
        $scope.model.REMARKS = '';
        $scope.buttonName = 'Save';
    }

    $scope.EditData = function (entity) {
        $scope.buttonName = 'Update';
        $scope.model.COMPANY_ID = entity.COMPANY_ID;
        $scope.model.DRIVER_ID = entity.DRIVER_ID;
        $scope.model.DRIVER_NAME = entity.DRIVER_NAME;
        $scope.model.CONTACT_NO = entity.CONTACT_NO;
        $scope.model.STATUS = entity.STATUS;
        $scope.model.REMARKS = entity.REMARKS;
        $scope.model.UNIT_ID = entity.UNIT_ID.toString();
        $interval(function () {
            $('#COMPANY_ID').trigger('change');
        }, 800, 4);

    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'Driver',
            Action_Name: 'DriverInfo'
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

        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);

        $scope.showLoader = true;

        DriverInfoServices.AddOrUpdate(model).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                //$scope.GetPermissionData();
                $scope.CompanyLoad();
                $scope.LoadFormData();
                $scope.ClearForm();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

}]);

