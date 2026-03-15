ngApp.controller('ngGridCtrl', ['$scope', 'MenuCategoryServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, MenuCategoryServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    $scope.model = { COMPANY_ID: 0, MODULE_ID: 0, MODULE_NAME: '', ORDER_BY_NO: 0, COMPANY_SEARCH_ID: 0 }

    $scope.getPermissions = [];
    $scope.Companies = [];

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Menu Category"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, cellClass: 'red', width: '50'
        }

        , { name: 'MODULE_ID', field: 'MODULE_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        //, {
        //    name: 'COMPANY_NAME', field: 'COMPANY_NAME', displayName: 'Company', visible: true, enableFiltering: true, width: ' 17%'

        //}
        , {
            name: 'MODULE_NAME', field: 'MODULE_NAME', displayName: 'Name', enableFiltering: true, width: '20%', cellTemplate:
                '<input required="required"   ng-model="row.entity.MODULE_NAME"  class="pl-sm" />'
        }
        , {
            name: 'ORDER_BY_NO', field: 'ORDER_BY_NO', displayName: 'SL No', enableFiltering: true, width: '12%', cellTemplate:
                '<input type="number" required="required"   ng-model="row.entity.ORDER_BY_NO"  class="pl-sm" />'
        }
        , { name: 'STATUS', field: 'STATUS', displayName: 'Status', enableFiltering: true, width: '10%' }
        , { name: 'CREATENAME', field: 'CREATENAME', displayName: 'Entry By', enableFiltering: true, width: '10%' }
        , { name: 'ENTERED_DATE', field: 'ENTERED_DATE', displayName: 'Entry Date', enableFiltering: true, width: '10%' },
        {
            name: 'Action', displayName: 'Action', width: '30%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                //'<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '<button style="margin-bottom: 5px;"  ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'"  ng-click="grid.appScope.ActivateMenuCategory(row.entity.MODULE_ID)" type="button" class="btn btn-outline-success mb-1"  ng-disabled="row.entity.STATUS == \'Active\'">Activate</button>' +
                '<button style="margin-bottom: 5px;"  ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'"  type="button" class="btn btn-outline-secondary mb-1" ng-disabled="row.entity.STATUS == \'InActive\'" ng-click="grid.appScope.DeactivateMenuCategory(row.entity.MODULE_ID)">Deactive</button>' +
                //'<button style="margin-bottom: 5px;"  ng-show="grid.appScope.model.DELETE_PERMISSION == \'Active\'"  ng-click="grid.appScope.DeleteMenuCategory(row.entity.MODULE_ID)" type="button" class="btn btn-outline-danger mb-1">Delete</button>' +
                '</div>'
        },

    ];

    $scope.DataLoad = function (companyId) {

        $scope.showLoader = true;

        MenuCategoryServices.GetMenuCetagories(companyId).then(function (data) {

            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }
    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;

        MenuCategoryServices.GetCompanyList().then(function (data) {

            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        MenuCategoryServices.GetCompany().then(function (data) {


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

    $scope.ClearForm = function () {
        $scope.model.MODULE_ID = 0;
        $scope.model.MODULE_NAME = "";
        $scope.model.ORDER_BY_NO = '';
        $scope.model.COMPANY_ID = 0;

    }

    $scope.EditData = function (entity) {

        $scope.model.MODULE_ID = entity.MODULE_ID;
        $scope.model.MODULE_NAME = entity.MODULE_NAME;
        $scope.model.ORDER_BY_NO = entity.ORDER_BY_NO;
        $scope.SaveData($scope.model);

    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;

        $scope.permissionReqModel = {
            Controller_Name: 'MenuCategory',
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
            $scope.model.USER_TYPE = $scope.getPermissions.useR_TYPE;
            //$scope.HideCompanyColumn();

            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }
    //$scope.HideCompanyColumn = function () {
    //    
    //    if ($scope.model.USER_TYPE == 'SuperAdmin') {
    //        $scope.gridOptionsList.columnDefs[3].visible = true;

    //    }
    //    else {
    //        $scope.gridOptionsList.columnDefs[3].visible = false;

    //    }
    //    return false;
    //};
    $scope.DataLoad(0);
    $scope.GetPermissionData();
    $scope.CompaniesLoad();
    $scope.CompanyLoad();



    $scope.SaveData = function (model) {

        $scope.showLoader = true;

        MenuCategoryServices.AddOrUpdate(model).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.DataLoad($scope.model.COMPANY_ID);
                $scope.model.MODULE_NAME = "";
                $scope.model.ORDER_BY_NO = "";
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

    //$scope.EditData = function (entity) {
    //    $scope.SaveData(entity)
    //}


    $scope.ActivateMenuCategory = function (Id) {

        $scope.showLoader = true;
        MenuCategoryServices.ActivateMenuCategory(Id).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Activated the selected category !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.DataLoad($scope.model.COMPANY_ID);

            }
            else {
                $scope.showLoader = false;
            }
        });
    }

    $scope.DeactivateMenuCategory = function (Id) {

        $scope.showLoader = true;
        MenuCategoryServices.DeactivateMenuCategory(Id).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Deactivated the selected category !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.DataLoad($scope.model.COMPANY_ID);

            }
            else {
                $scope.showLoader = false;
            }
        });
    }
    $scope.DeleteMenuCategory = function (Id) {

        $scope.showLoader = true;
        if (window.confirm("Are you sure to delete this Menu Category?")) {
            MenuCategoryServices.DeleteMenuCategory(Id).then(function (data) {

                notificationservice.Notification(data.data, 1, 'Deleted the selected category !!');
                if (data.data == 1) {
                    $scope.showLoader = false;
                    $scope.DataLoad($scope.model.COMPANY_ID);

                }
                else {
                    $scope.showLoader = false;
                }
            });
        }


    }

}]);

