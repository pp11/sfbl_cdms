ngApp.controller('ngGridCtrl', ['$scope', 'CompanyService', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, CompanyService, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    $scope.model = { ID: 0, COMPANY_ID: 0, UNIT_ID: 0 }

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Unit List"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.UnitData = [];
    $scope.CompanyData = [];
    $scope.LoadUnitData = function () {

        $scope.unidataload = {
            UNIT_TYPE: "Factory"
        }
        $scope.UnitData.push($scope.unidataload);
        $scope.unidataload1 = {
            UNIT_TYPE: "Depot"

        }
        $scope.UnitData.push($scope.unidataload1);
        $scope.unidataload2 = {
            UNIT_TYPE: "HeadOffice"

        }
        $scope.UnitData.push($scope.unidataload2);
    }


    $scope.CompDataLoad = function () {
        $scope.showLoader = true;
        CompanyService.GetCompanyList().then(function (data) {

            $scope.CompanyData = data.data;
            $scope.showLoader = false;

        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }

    $scope.DataLoad = function () {
        $scope.showLoader = true;
        CompanyService.GetUnitList().then(function (data) {
            $scope.gridOptionsList.data = [];
            for (var i = 0; i < data.data.length; i++) {
                if (data.data[i].COMPANY_ID == $scope.model.COMPANY_ID) {
                    $scope.gridOptionsList.data.push(data.data[i]);
                }
            }
            for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
                $scope.gridOptionsList.data[i].ROW_NO = i + 1;
            }
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }
    $scope.ClearForm = function () {
        $scope.model.ID = 0;
        $scope.model.UNIT_NAME = '';
        $scope.model.UNIT_SHORT_NAME = '';
        $scope.model.UNIT_TYPE = '';
        $scope.model.UNIT_ADDRESS1 = '';
        $scope.model.UNIT_ADDRESS2 = '';
    }


    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'Company',
            Action_Name: 'Unit'
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

    $scope.CompanyLoad = function () {
        $scope.showLoader = true;
        CompanyService.GetCompany().then(function (data) {

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
    $scope.CompDataLoad()
    $scope.CompanyLoad();

    $scope.DataLoad();
    $scope.GetPermissionData();
    $scope.LoadUnitData();

    $scope.gridOptionsList.columnDefs = [
        { name: 'SL', field: 'ROW_NO', enableFiltering: false, width: 40 }

        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , { name: 'ID', field: 'ID', visible: false }

        , { name: 'COMPANY_NAME', field: 'COMPANY_NAME', displayName: 'Company Name', enableFiltering: true, width: '20%', }
        , { name: 'UNIT_ID', field: 'UNIT_ID', visible: false }
        , {
            name: 'UNIT_NAME', field: 'UNIT_NAME', displayName: 'Unit Name', enableFiltering: true, width: '12%', cellTemplate:
                '<input required="required"   ng-model="row.entity.UNIT_NAME"  class="pl-sm" />'
        }
        , {
            name: 'UNIT_SHORT_NAME', field: 'UNIT_SHORT_NAME', displayName: 'Short Name', enableFiltering: true, width: ' 10%', cellTemplate:
                '<input required="required"  type="text"  ng-model="row.entity.UNIT_SHORT_NAME"  class="pl-sm" />'
        }
        , {
            name: 'UNIT_TYPE', field: 'UNIT_TYPE', displayName: 'Unit Type', enableFiltering: true, width: '10%'

        }
        , { name: 'STATUS', field: 'STATUS', displayName: 'Status', enableFiltering: true, width: ' 6%' }

        , {
            name: 'UNIT_ADDRESS1', field: 'UNIT_ADDRESS1', displayName: 'Unit Address 1', enableFiltering: true, width: ' 24%', cellTemplate:
                '<input required="required"  type="text"  ng-model="row.entity.UNIT_ADDRESS1"  class="pl-sm" />'
        }
        , {
            name: 'UNIT_ADDRESS2', field: 'UNIT_ADDRESS2', displayName: 'Unit Address 2', enableFiltering: true, width: ' 24%', cellTemplate:
                '<input required="required"  type="text"  ng-model="row.entity.UNIT_ADDRESS2"  class="pl-sm" />'
        }

        , {
            name: 'Actions', displayName: 'Actions', pinnedRight: true, enableFiltering: false, enableColumnMenu: false, width: '25%', cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditUnitData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.ActivateUnit(row.entity.ID)" type="button" class="btn btn-outline-success mb-1"  ng-disabled="row.entity.STATUS == \'Active\'">Activate</button>' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" type="button" class="btn btn-outline-secondary mb-1" ng-disabled="row.entity.STATUS == \'InActive\'" ng-click="grid.appScope.DeactivateUnit(row.entity.ID)">Deactive</button>' +
                '</div>'
        },

    ];

    $scope.EditUnitData = function (entity) {
        $scope.model = { ...$scope.model, ...entity };
        //$scope.model.ID = entity.ID;
        //$scope.model.COMPANY_ID = entity.COMPANY_ID;
        //$scope.model.COMPANY_NAME = entity.COMPANY_NAME;
        //$scope.model.UNIT_NAME = entity.UNIT_NAME;
        //$scope.model.UNIT_SHORT_NAME = entity.UNIT_SHORT_NAME;
        //$scope.model.UNIT_TYPE = entity.UNIT_TYPE;
        //$scope.model.UNIT_ADDRESS1 = entity.UNIT_ADDRESS1;
        //$scope.model.UNIT_ADDRESS2 = entity.UNIT_ADDRESS2;
        //$scope.model.COMPANY_ADDRESS2 = entity.COMPANY_ADDRESS2;
        //$scope.model.UNIT_ID = entity.UNIT_ID;
        //$scope.SaveData($scope.model);

    }


    $scope.SaveData = function (model) {
        $scope.showLoader = true;
        CompanyService.AddOrUpdateUnit(model).then(function (data) {
            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.ClearForm();
                $scope.DataLoad();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }


    $scope.ActivateUnit = function (Id) {

        $scope.showLoader = true;
        CompanyService.ActivateUnit(Id).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Activated the selected Unit !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.DataLoad();

            }
            else {
                $scope.showLoader = false;
            }
        });
    }

    $scope.DeactivateUnit = function (Id) {

        $scope.showLoader = true;
        CompanyService.DeactivateUnit(Id).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Deactivated the selected Unit !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.CompanyLoad();

                $scope.DataLoad();

            }
            else {
                $scope.showLoader = false;
            }
        });
    }




}]);

