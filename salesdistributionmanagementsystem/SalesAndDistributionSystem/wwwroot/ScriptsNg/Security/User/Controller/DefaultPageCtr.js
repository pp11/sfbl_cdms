ngApp.controller('ngGridCtrl', ['$scope', 'UserServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, UserServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    $scope.model = { COMPANY_ID: 0, USER_ID: 0, MENU_ID: 0, ID:0 }


    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("User Default"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.Companies = []

    $scope.AutoCompleteDataLoadForDefaultPage = function (value) {
        if (value.length >= 3) {
            
            return UserServices.LoadSearchableDefaultPages(parseInt($scope.model.COMPANY_ID), value).then(function (data) {
                $scope.DefaultPage = data.data;
                

                return $scope.DefaultPage;
            }, function (error) {
                alert(error);
                

                
            });
        }
    }


    $scope.typeaheadSelectedDefaultPage = function (entity, selectedItem) {
        
        entity.MENU_ID = selectedItem.MENU_ID;
        entity.MENU_NAME = selectedItem.DEFAULTPAGE;

    };

    $scope.AutoCompleteDataLoadForUsers = function (value) {
        if (value.length >= 3) {
            
            return UserServices.GetSearchableUsers(parseInt($scope.model.COMPANY_ID), value).then(function (data) {
                $scope.UserList = data.data;
                

                return $scope.UserList;
            }, function (error) {
                alert(error);
                

                
            });
        }
    }


    $scope.typeaheadSelectedUsers = function (entity, selectedItem) {
        $scope.model.USER_ID = selectedItem.USER_ID;
        $scope.model.USER_NAME = selectedItem.USER_NAME;

    };
    $scope.LoadDefaultPageslist = function (model) {
        

        $scope.showLoader = true;
        UserServices.LoadDefaultPages(model.COMPANY_ID).then(function (data) {

            $scope.gridOptionsList.data = data.data;
            for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
                $scope.gridOptionsList.data[i].ROW_NO = i + 1;
            }
            $scope.showLoader = false;

        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        

        $scope.permissionReqModel = {
            Controller_Name: 'User',
            Action_Name: 'DefaultPage'
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
    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;

        UserServices.GetCompanyList().then(function (data) {
            
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        UserServices.GetCompany().then(function (data) {
            
            
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
    $scope.GetPermissionData();
    $scope.CompanyLoad();

    $scope.LoadDefaultPageslist($scope.model.COMPANY_ID);
    $scope.gridOptionsList.columnDefs = [
        { name: 'SL', field: 'ROW_NO', enableFiltering: false, width: '50' }

        , { name: 'USER_ID', field: 'USER_ID', visible: false }
        , { name: 'MENU_ID', field: 'MENU_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }

        , { name: 'ID', field: 'ID', visible: false }


        , { name: 'USER_NAME', field: 'USER_NAME', displayName: 'User Name', enableFiltering: false, width: ' 18%'}
        , { name: 'EMPLOYEE_ID', field: 'EMPLOYEE_ID', displayName: 'Employee Id', enableFiltering: true, width: '20%'}
        , {
            
            name: 'MENU_NAME', field: 'MENU_NAME', displayName: 'Default Page', enableFiltering: true, width: '30%'}
        , {
            name: 'ENTERED_DATE', field: 'ENTERED_DATE', displayName: 'Date', enableFiltering: false, width: '20%'
        }
        


    ];

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
        
        $scope.showLoader = true;
        
        UserServices.AddOrUpdateDefaultPage(model).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.LoadDefaultPageslist($scope.model.COMPANY_ID);
                $scope.ClearForm();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

    



}]);

