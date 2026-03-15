ngApp.controller('ngGridCtrl', ['$scope', 'UserMenuConfigService', 'RolesServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, UserMenuConfigService, RolesServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    $scope.model = { USER_ID: 0, USER_NAME: '', EMAIL: '', EMPLOYEE_ID : ''}

    $scope.showLoader = true;

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("UserMenu Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.ActivityID = [];
    $scope.UserLst = [];
    $scope.AutoCompleteDataLoadForUser = function (value) {
        if (value.length >= 3) {
            
            return UserMenuConfigService.GetSearchableUsers(value).then(function (data) {
               
                $scope.UserLst = data.data;
                

                return $scope.UserLst;
            }, function (error) {
                alert(error);
                

                
            });
        }
    }
    $scope.AutoCompleteDataLoadForUserCentral = function (value) {
        if (value.length >= 3) {
            
            return UserMenuConfigService.GetSearchableCentralUsers(value).then(function (data) {

                $scope.UserLst = data.data;
                

                return $scope.UserLst;
            }, function (error) {
                alert(error);
                

                
            });
        }
    }

    $scope.typeaheadSelectedUser = function (entity, selectedItem) {
        $scope.model.USER_ID = selectedItem.USER_ID;
        $scope.model.USER_NAME_BACKUP = selectedItem.USER_NAME;
        $scope.model.EMAIL = selectedItem.EMAIL;
        $scope.model.EMPLOYEE_ID = selectedItem.EMPLOYEE_ID;

    };
    $scope.typeaheadBlurUser = function () {
        alert();
        $scope.model.USER_NAME = $scope.model.USER_NAME_BACKUP;
    };

    
    $scope.filterDataSelectedList = function (entity) {



        if (entity.IS_PERMITTED_CHECK == true) {

            entity.IS_PERMITTED = "Active";

        } else {
            entity.IS_PERMITTED_CHECK = false;
            entity.IS_PERMITTED = "InActive";

        }
        entity.USER_ID = $scope.model.USER_ID;
         


    };
    $scope.filterData = function (entity) {
        
        

        if (entity.IS_PERMITTED == "Active") {

            entity.IS_PERMITTED_CHECK = true;

        } else {
            entity.IS_PERMITTED_CHECK = false;
        }
       

    };
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'Role',
            Action_Name: 'RoleUserConfig'
        }
        permissionProvider.GetPermission($scope.permissionReqModel).then(function (data) {
            
            
            $scope.getPermissions = data.data;
            $scope.model.ADD_PERMISSION = $scope.getPermissions.adD_PERMISSION;
            $scope.model.EDIT_PERMISSION = $scope.getPermissions.ediT_PERMISSION;
            $scope.model.DELETE_PERMISSION = $scope.getPermissions.deletE_PERMISSION;
            $scope.model.LIST_VIEW = $scope.getPermissions.lisT_VIEW;
            $scope.model.DETAIL_VIEW = $scope.getPermissions.detaiL_VIEW;
            $scope.model.DOWNLOAD_PERMISSION = $scope.getPermissions.downloaD_PERMISSION;

            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    $scope.GetPermissionData();

    $scope.gridOptionsList.columnDefs = [
        { name: 'SL', field: 'ROW_NO', enableFiltering: false, width: '50' }

        , { name: 'USER_ID', field: 'USER_ID', visible: false }
        , { name: 'ROLE_ID', field: 'ROLE_ID', visible: false }
        , { name: 'USER_CONFIG_ID', field: 'USER_CONFIG_ID', visible: false }


        , { name: 'ROLE_NAME', field: 'ROLE_NAME', displayName: 'Role',  enableFiltering: false, width: '30%'}
        

        , { name: 'IS_PERMITTED', field: 'IS_PERMITTED', visible: false }

        , {
            name: 'IS_PERMITTED_CHECK', field: 'IS_PERMITTED_CHECK', displayName: 'List', enableFiltering: false, width: '20%', cellTemplate:
                '<input class=\"ngSelectionCheckbox\" ng-click="grid.appScope.selectionOfPermission(row.entity,\'IS_PERMITTED_CHECK\')" ng-model="row.entity.IS_PERMITTED_CHECK" type=\"checkbox\" ng-checked=\"row.entity.IS_PERMITTED_CHECK\"  style="margin-top:0px !important" />'
        }
        , {
            name: 'ENTERED_DATE', field: 'ENTERED_DATE', displayName: 'Date', enableFiltering: false, width: '40%'
        }



    ];

    $scope.RoleUserConfigData = [];


    $scope.SaveData = function (model) {
        
        model.MenuUserConfigData = $scope.gridOptionsList.data;
        $scope.showLoader = true;
        
        for (var i = 0; i < model.MenuUserConfigData.length; i++) {
            $scope.filterDataSelectedList(model.MenuUserConfigData[i])
        }
        UserMenuConfigService.SaveRoleUserConfiguration(model.MenuUserConfigData).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.UserMenuConfigSelectionList();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }
    $scope.SaveDataCentral = function (model) {
        
        model.MenuUserConfigData = $scope.gridOptionsList.data;
        $scope.showLoader = true;
        
        for (var i = 0; i < model.MenuUserConfigData.length; i++) {
            $scope.filterDataSelectedList(model.MenuUserConfigData[i])
        }
        UserMenuConfigService.SaveCentralRoleUserConfiguration(model.MenuUserConfigData).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.CentralUserMenuConfigSelectionList();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }
    $scope.UserMenuConfigSelectionList = function () {
        
        var Id = $scope.model.USER_ID;
        $scope.showLoader = true;
        UserMenuConfigService.RoleUserConfigSelectionList(Id).then(function (data) {
            $scope.gridOptionsList.data = data.data;
            for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
                $scope.filterData($scope.gridOptionsList.data[i])
            }
            $scope.showLoader = false;

        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.CentralUserMenuConfigSelectionList = function () {
        
        var Id = $scope.model.USER_ID;
        $scope.showLoader = true;
        UserMenuConfigService.RoleCentralUserConfigSelectionList(Id).then(function (data) {

            $scope.gridOptionsList.data = data.data;
            
            for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
                $scope.filterData($scope.gridOptionsList.data[i])
            }
            
            $scope.showLoader = false;

        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }


}]);

