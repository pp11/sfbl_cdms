ngApp.controller('ngGridCtrl', ['$scope', 'RolesServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', 'uiGridConstants', function ($scope, RolesServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q, uiGridConstants) {
    $scope.model = { COMPANY_ID: 0, ROLE_ID: 0, ROLE_NAME: '' }
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Roles Info"));
    $scope.gridOptionsList = {
        enableFiltering: true,
        enableGridMenu: true,
        showGridFooter: true,
        enableColumnMenus: false,
        showColumnFooter: false,
        enableHorizontalScrollbar: uiGridConstants.scrollbars.WHEN_NEEDED,
        enableVerticalScrollbar: uiGridConstants.scrollbars.WHEN_NEEDED,
        columnDefs: [
            { name: 'SL', field: 'ROW_NO', enableFiltering: false, width: '50' },
            { name: 'ROLE_ID', field: 'ROLE_ID', visible: false },
            { name: 'MENU_ID', field: 'MENU_ID', visible: false },
            { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false },
            { name: 'ROLE_CONFIG_ID', field: 'ROLE_CONFIG_ID', visible: false },
            { name: 'PARENT_MENU_ID', field: 'PARENT_MENU_ID', visible: false },
            { name: 'MODULE_ID', field: 'MODULE_ID', visible: false },
            { name: 'MODULE_NAME', field: 'MODULE_NAME', displayName: 'Module', enableFiltering: true, width: ' 8%', visible: false },
            { name: 'PARENT_MENU_NAME', field: 'PARENT_MENU_NAME', displayName: 'Parent', enableFiltering: true, width: ' 10%', visible: false },
            { name: 'MENU_NAME', field: 'MENU_NAME', displayName: 'Menu', enableFiltering: true, width: '14%' },
            { name: 'SEQUENCE', field: 'SEQUENCE', displayName: 'Sequence', enableFiltering: true, width: '24%' },
            checkboxColumn('ALL_PERMISSION_CHECK', 'ALL', 'ALL'),
            checkboxColumn('LIST_VIEW_CHECK', 'Entry(List)', 'LIST_VIEW_CHECK'),
            checkboxColumn('ADD_PERMISSION_CHECK', 'Add', 'ADD_PERMISSION_CHECK'),
            checkboxColumn('EDIT_PERMISSION_CHECK', 'Edit', 'EDIT_PERMISSION_CHECK'),
            checkboxColumn('DELETE_PERMISSION_CHECK', 'Delete', 'DELETE_PERMISSION_CHECK'),
            checkboxColumn('DETAIL_VIEW_CHECK', 'Detail', 'DETAIL_VIEW_CHECK'),
            checkboxColumn('DOWNLOAD_PERMISSION_CHECK', 'Download', 'DOWNLOAD_PERMISSION_CHECK'),
            checkboxColumn('CONFIRM_PERMISSION_CHECK', 'Confirm', 'CONFIRM_PERMISSION_CHECK')
        ],
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
            $scope.gridOptionsList.api = gridApi;
        }
    };
    function checkboxColumn(fieldName, displayName, ngModel) {
        return {
            name: fieldName,
            field: fieldName,
            displayName: displayName,
            enableFiltering: false,
            width: '7%',
            cellTemplate: `
            <input class="ngSelectionCheckbox" ng-click="grid.appScope.selectionOfPermission(row.entity, '${ngModel}')" 
            ng-model="row.entity.${fieldName}" type="checkbox" ng-checked="row.entity.${fieldName}" style="margin-top:0px !important" />
        `
        };
    }
    //$scope.gridOptionsList.columnDefs = [
    //    { name: 'SL', field: 'ROW_NO', enableFiltering: false, width: '50' }

    //    , { name: 'ROLE_ID', field: 'ROLE_ID', visible: false }
    //    , { name: 'MENU_ID', field: 'MENU_ID', visible: false }
    //    , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }

    //    , { name: 'ROLE_CONFIG_ID', field: 'ROLE_CONFIG_ID', visible: false }

    //    , { name: 'PARENT_MENU_ID', field: 'PARENT_MENU_ID', visible: false }
    //    , { name: 'MODULE_ID', field: 'MODULE_ID', visible: false }

    //    , { name: 'MODULE_NAME', field: 'MODULE_NAME', displayName: 'Module', enableFiltering: true, width: ' 8%', visible: false }
    //    , { name: 'PARENT_MENU_NAME', field: 'PARENT_MENU_NAME', displayName: 'Parent', enableFiltering: true, width: ' 10%', visible: false }

    //    , {
    //        name: 'MENU_NAME', field: 'MENU_NAME', displayName: 'Menu', enableFiltering: true, width: '15%'
    //    }
    //    , { name: 'SEQUENCE', field: 'SEQUENCE', displayName: 'Sequence', enableFiltering: true, width: '25%' }

    //    , {
    //        name: 'ALL_PERMISSION_CHECK', field: 'ALL_PERMISSION_CHECK', displayName: 'ALL', enableFiltering: false, width: '5%', cellTemplate:
    //            '<input class=\"ngSelectionCheckbox\" ng-click="grid.appScope.selectionOfPermission(row.entity,\'ALL\')"  ng-model="row.entity.ALL_PERMISSION_CHECK" type=\"checkbox\" ng-checked=\"row.entity.ALL_PERMISSION_CHECK\" style="margin-top:0px !important" />'
    //    }
    //    , { name: 'LIST_VIEW', field: 'LIST_VIEW', visible: false }

    //    , {
    //        name: 'LIST_VIEW_CHECK', field: 'LIST_VIEW_CHECK', displayName: 'List', enableFiltering: false, width: '5%', cellTemplate:
    //            '<input class=\"ngSelectionCheckbox\" ng-click="grid.appScope.selectionOfPermission(row.entity,\'LIST_VIEW_CHECK\')" ng-model="row.entity.LIST_VIEW_CHECK" type=\"checkbox\" ng-checked=\"row.entity.LIST_VIEW_CHECK\"  style="margin-top:0px !important" />'
    //    }
    //    , { name: 'ADD_PERMISSION', field: 'ADD_PERMISSION', visible: false }

    //    , {
    //        name: 'ADD_PERMISSION_CHECK', field: 'ADD_PERMISSION_CHECK', displayName: 'Add', enableFiltering: false, width: '7%', cellTemplate:
    //            '<input class=\"ngSelectionCheckbox\" ng-click="grid.appScope.selectionOfPermission(row.entity,\'ADD_PERMISSION_CHECK\')" ng-model="row.entity.ADD_PERMISSION_CHECK" type=\"checkbox\" ng-checked=\"row.entity.ADD_PERMISSION_CHECK\" style="margin-top:0px !important" />'
    //    }
    //    , { name: 'EDIT_PERMISSION', field: 'EDIT_PERMISSION', visible: false }

    //    , {
    //        name: 'EDIT_PERMISSION_CHECK', field: 'EDIT_PERMISSION_CHECK', displayName: 'Edit', enableFiltering: false, width: '7%', cellTemplate:
    //            '<input class=\"ngSelectionCheckbox\" ng-click="grid.appScope.selectionOfPermission(row.entity,\'EDIT_PERMISSION_CHECK\')" type=\"checkbox\" ng-model="row.entity.EDIT_PERMISSION_CHECK" ng-checked=\"row.entity.EDIT_PERMISSION_CHECK\"  style="margin-top:0px !important" />'
    //    }
    //    , { name: 'DELETE_PERMISSION', field: 'DELETE_PERMISSION', visible: false }

    //    , {
    //        name: 'DELETE_PERMISSION_CHECK', field: 'DELETE_PERMISSION_CHECK', displayName: 'Delete', enableFiltering: false, width: '7%', cellTemplate:
    //            '<input class=\"ngSelectionCheckbox\" ng-click="grid.appScope.selectionOfPermission(row.entity,\'DELETE_PERMISSION_CHECK\')" ng-model="row.entity.DELETE_PERMISSION_CHECK" type=\"checkbox\" ng-checked=\"row.entity.DELETE_PERMISSION_CHECK\"  style="margin-top:0px !important" />'
    //    }
    //    , { name: 'DETAIL_VIEW', field: 'DETAIL_VIEW', visible: false }

    //    , {
    //        name: 'DETAIL_VIEW_CHECK', field: 'DETAIL_VIEW_CHECK', displayName: 'Detail', enableFiltering: false, width: '7%', cellTemplate:
    //            '<input class=\"ngSelectionCheckbox\"  ng-click="grid.appScope.selectionOfPermission(row.entity,\'DETAIL_VIEW_CHECK\')" ng-model="row.entity.DETAIL_VIEW_CHECK" type=\"checkbox\" ng-checked=\"row.entity.DETAIL_VIEW_CHECK\" style="margin-top:0px !important" />'
    //    }
    //    , { name: 'DOWNLOAD_PERMISSION', field: 'DOWNLOAD_PERMISSION', visible: false }

    //    , {
    //        name: 'DOWNLOAD_PERMISSION_CHECK', field: 'DOWNLOAD_PERMISSION_CHECK', displayName: 'Download', enableFiltering: false, width: '7%', cellTemplate:
    //            '<input class=\"ngSelectionCheckbox\" ng-click="grid.appScope.selectionOfPermission(row.entity,\'DOWNLOAD_PERMISSION_CHECK\')" type=\"checkbox\" ng-model="row.entity.DOWNLOAD_PERMISSION_CHECK" ng-checked=\"row.entity.DOWNLOAD_PERMISSION_CHECK\"  style="margin-top:0px !important" />'
    //    }
    //    , { name: 'CONFIRM_PERMISSION', field: 'CONFIRM_PERMISSION', visible: false }
    //    , {
    //        name: 'CONFIRM_PERMISSION_CHECK', field: 'CONFIRM_PERMISSION_CHECK', displayName: 'Confirm', enableFiltering: false, width: '9%', cellTemplate:
    //            '<input class=\"ngSelectionCheckbox\" ng-click="grid.appScope.selectionOfPermission(row.entity,\'CONFIRM_PERMISSION_CHECK\')" type=\"checkbox\" ng-model="row.entity.CONFIRM_PERMISSION_CHECK" ng-checked=\"row.entity.CONFIRM_PERMISSION_CHECK\"  style="margin-top:0px !important" />'
    //    }

    //];



    $scope.ActivityID = [];
    $scope.RoleLst = [];
    $scope.Companies = []

    $scope.AutoCompleteDataLoadForRole = function (value) {
        if (value.length >= 3) {
            return RolesServices.GetSearchableRoles(value, $scope.model.COMPANY_ID).then(function (data) {
                $scope.RoleLst = data.data;

                return $scope.RoleLst;
            }, function (error) {
                alert(error);
            });
        }
    }

    $scope.typeaheadSelectedRole = function (entity, selectedItem) {
        $scope.model.ROLE_ID = selectedItem.ROLE_ID;
        $scope.model.ROLE_NAME = selectedItem.ROLE_NAME;
    };
    $scope.selectionOfPermission = function (entity, permissionfield) {
        $scope.ActivityID.push(entity.MENU_ID);
        if (permissionfield == "ALL") {
            if (!entity.ALL_PERMISSION_CHECK) {
                entity.LIST_VIEW = "Active";
                entity.LIST_VIEW_CHECK = true;

                entity.ADD_PERMISSION = "Active";
                entity.ADD_PERMISSION_CHECK = true;

                entity.EDIT_PERMISSION = "Active";
                entity.EDIT_PERMISSION_CHECK = true;

                entity.DELETE_PERMISSION = "Active";
                entity.DELETE_PERMISSION_CHECK = true;

                entity.DETAIL_VIEW = "Active";
                entity.DETAIL_VIEW_CHECK = true;

                entity.DOWNLOAD_PERMISSION = "Active";
                entity.DOWNLOAD_PERMISSION_CHECK = true;

                entity.CONFIRM_PERMISSION = "Active";
                entity.CONFIRM_PERMISSION_CHECK = true;
            }
            else {
                entity.LIST_VIEW = "InActive";
                entity.LIST_VIEW_CHECK = false;

                entity.ADD_PERMISSION = "InActive";
                entity.ADD_PERMISSION_CHECK = false;

                entity.EDIT_PERMISSION = "InActive";
                entity.EDIT_PERMISSION_CHECK = false;

                entity.DELETE_PERMISSION = "InActive";
                entity.DELETE_PERMISSION_CHECK = false;

                entity.DETAIL_VIEW = "InActive";
                entity.DETAIL_VIEW_CHECK = false;

                entity.DOWNLOAD_PERMISSION = "InActive";
                entity.DOWNLOAD_PERMISSION_CHECK = false;

                entity.CONFIRM_PERMISSION = "InActive";
                entity.CONFIRM_PERMISSION_CHECK = false;
            }
        }
        if (permissionfield == "LIST_VIEW_CHECK") {
            if (!entity.LIST_VIEW_CHECK) {
                entity.LIST_VIEW = "Active";
                entity.LIST_VIEW_CHECK = true;
            } else {
                entity.LIST_VIEW = "InActive";
                entity.LIST_VIEW_CHECK = false;
            }
        }
        if (permissionfield == "ADD_PERMISSION_CHECK") {
            if (!entity.ADD_PERMISSION_CHECK) {
                entity.ADD_PERMISSION = "Active";
                entity.ADD_PERMISSION_CHECK = true;
            } else {
                entity.ADD_PERMISSION = "InActive";
                entity.ADD_PERMISSION_CHECK = false;
            }
        }
        if (permissionfield == "EDIT_PERMISSION_CHECK") {
            if (!entity.EDIT_PERMISSION_CHECK) {
                entity.EDIT_PERMISSION = "Active";
                entity.EDIT_PERMISSION_CHECK = true;
            } else {
                entity.EDIT_PERMISSION = "InActive";
                entity.EDIT_PERMISSION_CHECK = false;
            }
        }
        if (permissionfield == "DELETE_PERMISSION_CHECK") {
            if (!entity.DELETE_PERMISSION_CHECK) {
                entity.DELETE_PERMISSION = "Active";
                entity.DELETE_PERMISSION_CHECK = true;
            } else {
                entity.DELETE_PERMISSION = "InActive";
                entity.DELETE_PERMISSION_CHECK = false;
            }
        }
        if (permissionfield == "DETAIL_VIEW_CHECK") {
            if (!entity.DETAIL_VIEW_CHECK) {
                entity.DETAIL_VIEW = "Active";
                entity.DETAIL_VIEW_CHECK = true;
            } else {
                entity.DETAIL_VIEW = "InActive";
                entity.DETAIL_VIEW_CHECK = false;
            }
        }
        if (permissionfield == "DOWNLOAD_PERMISSION_CHECK") {
            if (!entity.DOWNLOAD_PERMISSION_CHECK) {
                entity.DOWNLOAD_PERMISSION = "Active";
                entity.DOWNLOAD_PERMISSION_CHECK = true;
            } else {
                entity.DOWNLOAD_PERMISSION = "InActive";
                entity.DOWNLOAD_PERMISSION_CHECK = false;
            }
        }
        if (permissionfield == "CONFIRM_PERMISSION_CHECK") {
            if (!entity.CONFIRM_PERMISSION_CHECK) {
                entity.CONFIRM_PERMISSION = "Active";
                entity.CONFIRM_PERMISSION_CHECK = true;
            } else {
                entity.CONFIRM_PERMISSION = "InActive";
                entity.CONFIRM_PERMISSION_CHECK = false;
            }
        }
    };
    $scope.filterData = function (entity) {
        if (entity.LIST_VIEW == "Active" && entity.ADD_PERMISSION == "Active" && entity.EDIT_PERMISSION == "Active"
            && entity.DELETE_PERMISSION == "Active" && entity.DETAIL_VIEW == "Active" && entity.DOWNLOAD_PERMISSION == "Active") {
            entity.ALL_PERMISSION_CHECK = true;
        }
        else {
            entity.ALL_PERMISSION_CHECK = false;
        }

        if (entity.LIST_VIEW == "Active") {
            entity.LIST_VIEW_CHECK = true;
        } else {
            entity.LIST_VIEW_CHECK = false;
        }

        if (entity.ADD_PERMISSION == "Active") {
            entity.ADD_PERMISSION_CHECK = true;
        } else {
            entity.ADD_PERMISSION_CHECK = false;
        }

        if (entity.EDIT_PERMISSION == "Active") {
            entity.EDIT_PERMISSION_CHECK = true;
        } else {
            entity.EDIT_PERMISSION_CHECK = false;
        }

        if (entity.DELETE_PERMISSION == "Active") {
            entity.DELETE_PERMISSION_CHECK = true;
        } else {
            entity.DELETE_PERMISSION_CHECK = false;
        }

        if (entity.DETAIL_VIEW == "Active") {
            entity.DETAIL_VIEW_CHECK = true;
        } else {
            entity.DETAIL_VIEW_CHECK = false;
        }

        if (entity.DOWNLOAD_PERMISSION == "Active") {
            entity.DOWNLOAD_PERMISSION_CHECK = true;
        } else {
            entity.DOWNLOAD_PERMISSION_CHECK = false;
        }

        if (entity.CONFIRM_PERMISSION == "Active") {
            entity.CONFIRM_PERMISSION_CHECK = true;
        } else {
            entity.CONFIRM_PERMISSION_CHECK = false;
        }
    };
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;

        $scope.permissionReqModel = {
            Controller_Name: 'Role',
            Action_Name: 'RoleMenuConfig'
        }
        permissionProvider.GetPermission($scope.permissionReqModel).then(function (data) {
            $scope.getPermissions = data.data;
            $scope.model.ADD_PERMISSION = $scope.getPermissions.adD_PERMISSION;
            $scope.model.EDIT_PERMISSION = $scope.getPermissions.ediT_PERMISSION;
            $scope.model.DELETE_PERMISSION = $scope.getPermissions.deletE_PERMISSION;
            $scope.model.CONFIRM_PERMISSION = $scope.getPermissions.confirmE_PERMISSION;
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
    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;

        RolesServices.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;
        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        RolesServices.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);
            $scope.model.COMPANY_SEARCH_ID = parseInt(data.data);
            $interval(function () {
                $('#COMPANY_ID').trigger('change');
            }, 800, 4);

            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;
        });
    }

    $scope.CompaniesLoad();
    $scope.CompanyLoad();
    $scope.GetPermissionData();


    $scope.MenuRoleConfigData = [];

    $scope.bindMenuRoleConfigData = function (model) {
        $scope.MenuRoleConfigData = [];
        for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
            var activity = $scope.ActivityID.indexOf($scope.gridOptionsList.data[i].MENU_ID);
            if (activity != (-1) || ($scope.gridOptionsList.data[i].ROLE_CONFIG_ID != null && $scope.gridOptionsList.data[i].ROLE_CONFIG_ID > 0)) {
                $scope.loadData = {
                    ID: 0,
                    ROLE_ID: model.ROLE_ID,
                    MENU_ID: $scope.gridOptionsList.data[i].MENU_ID,
                    ADD_PERMISSION: $scope.gridOptionsList.data[i].ADD_PERMISSION,
                    EDIT_PERMISSION: $scope.gridOptionsList.data[i].EDIT_PERMISSION,
                    LIST_VIEW: $scope.gridOptionsList.data[i].LIST_VIEW,
                    DOWNLOAD_PERMISSION: $scope.gridOptionsList.data[i].DOWNLOAD_PERMISSION,
                    CONFIRM_PERMISSION: $scope.gridOptionsList.data[i].CONFIRM_PERMISSION,
                    DETAIL_VIEW: $scope.gridOptionsList.data[i].DETAIL_VIEW,
                    DELETE_PERMISSION: $scope.gridOptionsList.data[i].DELETE_PERMISSION,
                    ROLE_CONFIG_ID: $scope.gridOptionsList.data[i].ROLE_CONFIG_ID,
                    COMPANY_ID: parseInt($scope.model.COMPANY_ID)
                };

                $scope.MenuRoleConfigData.push($scope.loadData);
            }
        }
    };
    $scope.SaveData = function (model) {
        model.MenuRoleConfigData = $scope.gridOptionsList.data;
        $scope.bindMenuRoleConfigData(model);
        $scope.showLoader = true;

        RolesServices.SaveRoleMenuPermission($scope.MenuRoleConfigData).then(function (data) {
            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.RoleMenuConfigSelectionList();
                $scope.ClearForm();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

    $scope.RoleMenuConfigSelectionList = function () {
        var Id = $scope.model.ROLE_ID;
        var CompId = $scope.model.COMPANY_ID;

        $scope.showLoader = true;
        RolesServices.RoleMenuConfigSelectionList(Id, CompId).then(function (data) {
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