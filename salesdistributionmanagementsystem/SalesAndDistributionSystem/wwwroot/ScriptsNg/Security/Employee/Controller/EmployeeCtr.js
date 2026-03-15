ngApp.controller('ngGridCtrl', ['$scope', 'EmployeeService', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, EmployeeService, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    $scope.model = { ID: 0, COMPANY_ID: 0, COMPANY_NAME: '', EMPLOYEE_STATUS: 'Active', USER_TYPE:'General' }


    $scope.Companies = [];
    $scope.CompanyUnit = [];
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Company List"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'Employee',
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
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }
    $scope.GetEmployeelist = function () {
        $scope.showLoader = true;
        EmployeeService.GetEmployeeList().then(function (data) {
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }
    $scope.DataLoad = function () {
        $scope.showLoader = true;
        let allCodeAndListData = EmployeeService.getAllCodeAndDropdownListData();
        let userCompanyAndUnitInfo = EmployeeService.fetchLoggedInCompanyAndUnit();
        $scope.combineResult = $q.all([
            allCodeAndListData,
            userCompanyAndUnitInfo
        ]).then(function (response) {
            $scope.EmployeeStatus = response[0].data[0];
            $scope.CompanyUnits = response[0].data[1];

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


    $scope.GetEmployeelist();
    $scope.DataLoad();
    $scope.GetPermissionData();


    $scope.ClearForm = function () {
        $scope.model.ID = 0;
        $scope.model.EMPLOYEE_ID = 0;
        $scope.model.EMPLOYEE_CODE = '';
        $scope.model.EMPLOYEE_NAME = '';
        $scope.model.EMPLOYEE_STATUS = 'Active';
        $scope.model.USER_TYPE= 'General'

    }

 

    $scope.gridOptionsList.columnDefs = [
        {
            name: 'ITEM_NO',
            field: 'ITEM_NO',
            displayName: '#',
            enableFiltering: false,
            enableSorting: false,
            cellTemplate: '<div class="ui-grid-cell-contents">{{grid.renderContainers.body.visibleRowCache.indexOf(row) + 1}}</div>',
            width: '5%',
            pinnedLeft: true

        }
        ,{
            name: 'UNIT_NAME', field: 'UNIT_NAME', displayName: 'Unit Name', enableFiltering: true, width: '15%'
        }

        , { name: 'ID', field: 'ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , { name: 'COMPANY_NAME', field: 'COMPANY_NAME', visible: false }

        , {
            name: 'EMPLOYEE_ID', field: 'EMPLOYEE_ID', displayName: 'Employee Id', enableFiltering: true, width: '12%', cellClass:'text-right'
        }
        , {
            name: 'EMPLOYEE_CODE', field: 'EMPLOYEE_CODE', displayName: 'Employee Code', enableFiltering: true, width: ' 12%'
        }
        , {
            name: 'EMPLOYEE_NAME', field: 'EMPLOYEE_NAME', displayName: 'Employee Name', enableFiltering: true, width: ' 24%'
        }
        , {
            name: 'EMPLOYEE_STATUS', field: 'EMPLOYEE_STATUS', displayName: 'Employee Status', enableFiltering: true, width: ' 14%'
        }
        , {
            name: 'Actions', displayName: 'Actions', enableFiltering: true, enableColumnMenu: false, width: '10%', cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];

    $scope.AutoCompleteDataLoadForCustomer = function (value) {
        return EmployeeService.GetSearchableDistributor($scope.model.COMPANY_ID, value).then(function (data) {
            $scope.CustomersList = [];
            for (var i = 0; i < data.data.length; i++) {
                var _customer = {
                    CUSTOMER_CODE: data.data[i].CUSTOMER_CODE,
                    CUSTOMER_NAME: data.data[i].CUSTOMER_NAME,
                    CUSTOMER_ID: data.data[i].CUSTOMER_ID,
                    CUSTOMER_NAME_CODE: data.data[i].CUSTOMER_NAME + ' | CODE:' + data.data[i].CUSTOMER_CODE
                }
                $scope.CustomersList.push(_customer);
            }

            return $scope.CustomersList;
        }, function (error) {
            alert(error);
        });
    }

    $scope.loadDistributor = function () {
        if ($scope.model.USER_TYPE == 'Distributor') {
            document.getElementById("company_customer_name_param").style.display = "block";
            document.getElementById("company_customer_name_param_alter").style.display = "none";

        } else {
            document.getElementById("company_customer_name_param").style.display = "none";
            document.getElementById("company_customer_name_param_alter").style.display = "block";

        }
    }

    $scope.typeaheadSelectedCustomer = function (entity, selectedItem) {
        $scope.model.EMPLOYEE_ID = parseInt(selectedItem.CUSTOMER_CODE);
        $scope.model.EMPLOYEE_CODE = selectedItem.CUSTOMER_CODE;
        $scope.model.EMPLOYEE_NAME = selectedItem.CUSTOMER_NAME;
    };
    $scope.EditData = function (entity) {
        //document.getElementById("company_customer_name_param").style.display = "none";
        //document.getElementById("company_customer_name_param_alter").style.display = "block";
        $scope.model.ID = entity.ID;
        $scope.model.COMPANY_ID = entity.COMPANY_ID;
        $scope.model.COMPANY_NAME = entity.COMPANY_NAME;
        $scope.model.UNIT_ID = entity.UNIT_ID;
        $scope.model.EMPLOYEE_ID = entity.EMPLOYEE_ID;
        $scope.model.EMPLOYEE_CODE = entity.EMPLOYEE_CODE;
        $scope.model.EMPLOYEE_NAME = entity.EMPLOYEE_NAME;
        $scope.model.EMPLOYEE_CODE = entity.EMPLOYEE_CODE;
        $scope.model.EMPLOYEE_STATUS = entity.EMPLOYEE_STATUS;
    }
    $scope.SaveData = function (model) {
        $scope.showLoader = true;
        $scope.model.EMPLOYEE_CODE = $scope.model.EMPLOYEE_CODE.toString();
        EmployeeService.AddOrUpdate(model).then(function (data) {
            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.GetEmployeelist();
                $scope.ClearForm();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

    $scope.updateEmployeeCode = function () {
        // Check if EMPLOYEE_ID is not empty
        if ($scope.model.EMPLOYEE_ID !== null && $scope.model.EMPLOYEE_ID !== undefined) {
            // Convert EMPLOYEE_ID to string
            var employeeIdString = $scope.model.EMPLOYEE_ID.toString();
            // Ensure EMPLOYEE_ID is not longer than 8 characters
            if (employeeIdString.length <= 8) {
                // Pad EMPLOYEE_ID with leading zeros to make it 8 characters long
                $scope.model.EMPLOYEE_CODE = employeeIdString.padStart(8, '0');
            } else {
                // If EMPLOYEE_ID is longer than 8 characters, truncate it to 8 characters
                $scope.model.EMPLOYEE_CODE = employeeIdString.slice(0, 8);
            }
        } else {
            // If EMPLOYEE_ID is empty, reset EMPLOYEE_CODE
            $scope.model.EMPLOYEE_CODE = '';
        }
    };



}]);

