ngApp.controller('ngGridCtrl', ['$scope', 'UserServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', 'uiGridConstants', function ($scope, UserServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q, uiGridConstants) {
    $scope.model = { USER_ID: 0, USER_NAME: '', COMPANY_ID: 0, UNIT_ID: 0, USER_TYPE: 'General', STATUS: 'Active' }
    $scope.UnitData = [];
    $scope.UserTypeData = [];
    $scope.EmployeeData = [];
    $scope.UserTypeData = [
        { Type: 'General' },
        { Type: 'Admin' },
        { Type: 'SuperAdmin' },
        { Type: 'Distributor' },
        { Type: 'CreditController' },
        { Type: 'DSM' },
        { Type: 'SM' },
        { Type: 'HOS' },
        { Type: 'OSM' }
    ];
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("UserMenu Info"));
    $scope.gridOptionsList = {
        enableFiltering: true,
        enableGridMenu: true,
        showGridFooter: true,
        enableColumnMenus: false,
        showColumnFooter: false,
        enableHorizontalScrollbar: uiGridConstants.scrollbars.WHEN_NEEDED,
        enableVerticalScrollbar: uiGridConstants.scrollbars.WHEN_NEEDED,
        columnDefs: [
            {
                name: 'ITEM_NO',
                field: 'ITEM_NO',
                displayName: '#',
                enableFiltering: false,
                enableSorting: false,
                cellTemplate: '<div class="ui-grid-cell-contents">{{grid.renderContainers.body.visibleRowCache.indexOf(row) + 1}}</div>',
                width: '5%',
                pinnedLeft: true
            },
            { name: 'COMPANY_NAME', field: 'COMPANY_NAME', displayName: 'Company Name', width: '15%' },
            { name: 'UNIT_NAME', field: 'UNIT_NAME', displayName: 'Unit Name', width: '8%' },
            { name: 'USER_ID', field: 'USER_ID', displayName: 'User ID', visible: false, width: '5%' },
            { name: 'USER_NAME', field: 'USER_NAME', displayName: 'Employee Name', width: '12%' },
            { name: 'EMPLOYEE_ID', field: 'EMPLOYEE_ID', displayName: 'Emp. ID', width: '7%' },
            { name: 'USER_TYPE', field: 'USER_TYPE', displayName: 'User Type', width: '8%' },
            {
                name: 'EMAIL',
                field: 'EMAIL',
                displayName: 'Email',
                width: '20%',
                cellTemplate: '<input type="email" ng-model="row.entity.EMAIL" class="pl-sm" required />'
            },
            { name: 'USER_PASSWORD', field: 'USER_PASSWORD', displayName: 'Password', visible: false, width: '5%' },
            { name: 'UNIQUEACCESSKEY', field: 'UNIQUEACCESSKEY', displayName: 'Unique Access Key', visible: false, width: '5%' },
            { name: 'STATUS', field: 'STATUS', displayName: 'Status', width: '5%' },
            {
                name: 'Action',
                field: 'Action',
                displayName: 'Action',
                enableFiltering: false,
                width: 220,
                pinnedRight: true,
                cellTemplate:
                    '<button style="margin-bottom: 5px; margin-right: 10px" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-success mb-1">Update</button>' +
                    '<button style="margin-bottom: 5px; margin-right: 10px" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.ActivateUser(row.entity.USER_ID)" type="button" ng-disabled="row.entity.STATUS == \'Active\'" class="btn btn-primary mb-1">Active</button>' +
                    '<button style="margin-bottom: 5px; margin-right: 10px" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.DeactivateUser(row.entity.USER_ID)" type="button" ng-disabled="row.entity.STATUS == \'InActive\'" class="btn btn-danger mb-1">Inactive</button>'
            }
        ],
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
            $scope.gridOptionsList.api = gridApi;
        }
    };
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'User',
            Action_Name: 'Index'
        }
        permissionProvider.GetPermission($scope.permissionReqModel).then(function (data) {
            $scope.getPermissions = data.data;
            $scope.model.ADD_PERMISSION = $scope.getPermissions.adD_PERMISSION;
            $scope.model.EDIT_PERMISSION = $scope.getPermissions.ediT_PERMISSION;
            $scope.model.DELETE_PERMISSION = $scope.getPermissions.deletE_PERMISSION;
            $scope.model.LIST_VIEW = $scope.getPermissions.lisT_VIEW;
            $scope.model.DETAIL_VIEW = $scope.getPermissions.detaiL_VIEW;
            $scope.model.DOWNLOAD_PERMISSION = $scope.getPermissions.downloaD_PERMISSION;
            //$scope.model.USER_TYPE = $scope.getPermissions.useR_TYPE;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }
    $scope.GetUserList = function () {
        $scope.showLoader = true;
        UserServices.LoadData().then(function (data) {
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;

        }, function (error) {
            console.log(error);
            $scope.showLoader = false;
        });
    }
    $scope.DataLoad = function () {
        $scope.showLoader = true;
        let allCodeAndListData = UserServices.getAllCodeAndDropdownListData();
        let userCompanyAndUnitInfo = UserServices.fetchLoggedInCompanyAndUnit();
        $scope.combineResult = $q.all([
            allCodeAndListData,
            userCompanyAndUnitInfo
        ]).then(function (response) {
            $scope.EmployeeStatus = response[0].data[0];
            $scope.CompanyUnits = response[0].data[1];
            $scope.EmployeeData = response[0].data[2];

            $scope.model.COMPANY_ID = parseInt(response[1].data.companyId);
            $scope.model.COMPANY_NAME = response[1].data.companyName;
            $scope.model.UNIT_ID = parseInt(response[1].data.unitId);

            $scope.gridApi.grid.refresh();
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = true;
            alert(error);
            console.log(error);
        });
    }
    $scope.onemployeeSelect = function (employeeId) {
        let index = $scope.EmployeeData.findIndex(x => x.EMPLOYEE_ID == employeeId);
        if (index !== -1) {
            $scope.model.UNIT_ID = $scope.EmployeeData[index].UNIT_ID;
            $scope.model.STATUS = $scope.EmployeeData[index].EMPLOYEE_STATUS;

        } else {
            // Handle the case where the supplier is not found
            console.log('employee not found');
            alert('employee not found');
        }
    }
    $scope.ClearForm = function () {
        $scope.model.USER_ID = 0;
        $scope.model.USER_NAME = "";
        $scope.model.COMPANY_ID = 0;
        $scope.model.USER_TYPE = '';
        $scope.model.EMPLOYEE_ID = 0;
        $scope.model.EMAIL = '';
        $scope.model.USER_TYPE = 'General';
        $scope.model.STATUS = 'Active';
    }
    $scope.EditData = function (entity) {
        $scope.SaveData(entity);
    }
    $scope.DataLoad();
    $scope.GetPermissionData();
    $scope.GetUserList()
    $scope.SaveData = function (model) {
        $scope.showLoader = true;
        model.EMPLOYEE_ID = model.EMPLOYEE_ID.toString();
        UserServices.AddOrUpdate(model).then(function (data) {
            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.GetUserList();
                $scope.ClearForm();
                $scope.DataLoad();
            }
            else {
                model.EMPLOYEE_ID = parseInt(model.EMPLOYEE_ID);
                $scope.showLoader = false;
                $scope.GetUserList();
            }
        });
    }
    $scope.ActivateUser = function (Id) {
        $scope.showLoader = true;
        UserServices.ActivateUser(Id).then(function (data) {
            notificationservice.Notification(data.data, 1, 'Activated the selected user !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.GetUserList();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }
    $scope.DeactivateUser = function (Id) {
        $scope.showLoader = true;
        UserServices.DeactivateUser(Id).then(function (data) {
            notificationservice.Notification(data.data, 1, 'Deactivated the selected user !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.GetUserList();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }
    $scope.DeleteMenu = function (Id) {
        $scope.showLoader = true;
        if (window.confirm("Are you sure to delete this Menu Category?")) {
            MenuMasterServices.DeleteMenu(Id).then(function (data) {
                notificationservice.Notification(data.data, 1, 'Deleted the selected category !!');
                if (data.data == 1) {
                    $scope.showLoader = false;
                    $scope.GetUserList();
                }
                else {
                    $scope.showLoader = false;
                }
            });
        }
    }
}]);

