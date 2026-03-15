ngApp.controller('ngGridCtrl', ['$scope', 'MenuMasterServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', 'uiGridConstants', function ($scope, MenuMasterServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q, uiGridConstants) {

    $scope.modelDefault = { COMPANY_ID: 0, MENU_ID: 0, MENU_NAME: '', ORDER_BY_SLNO: 0, MODULE_ID: null, CONTROLLER: '', ACTION: '', HREF: '', STATUS: '', PARENT_MENU_ID: 0, MENU_SHOW: 'Active' }
    $scope.model = JSON.parse(JSON.stringify($scope.modelDefault));

    $scope.getPermissions = [];
    $scope.MenuCategories = [];
    $scope.Companies = [];
    $scope.IsReportValues = [];
    $scope.showLoader = true;
    $scope.ParantData = [];


    //+++++++++++++++++++++++Data Load Fucnction Defination and Call++++++++++++++++++++++++++++//
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'MenuMaster',
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
            $scope.model.CONFIRM_PERMISSION = $scope.getPermissions.confirM_PERMISSION;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }
    $scope.AutoCompleteDataLoadForMenuCategory = function () {
        return MenuMasterServices.GetMenuCetagories($scope.model.COMPANY_ID).then(function (data) {
            $scope.MenuCategories = data.data;
        }, function (error) {
            alert(error);
        });
    }
    $scope.DataLoad = function () {
        $scope.showLoader = true;
        let allCodeAndListData = MenuMasterServices.getAllCodeAndDropdownListData();
        let userCompanyAndUnitInfo = MenuMasterServices.fetchLoggedInCompanyAndUnit();
        $scope.combineResult = $q.all([
            allCodeAndListData,
            userCompanyAndUnitInfo
        ]).then(function (response) {
            //console.log(response)
            $scope.MenuCategories = response[0].data[0];
            $scope.ParantData = response[0].data[1];
            $scope.gridOptionsList.data = response[0].data[2];
           
            //unit and company
            $scope.model.COMPANY_ID = parseInt(response[1].data.companyId);
            $scope.model.COMPANY_NAME = response[1].data.companyName;




            $scope.gridApi.grid.refresh();
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
            alert(error);
            console.log(error);
        });
    }
    

    $scope.GetPermissionData();
    $scope.DataLoad();
    //$scope.GetFgRcvList();
    //+++++++++++++++++++++++Data Load Fucnction Defination and Call++++++++++++++++++++++++++++//



  
    
    $scope.Load_IsMenuShow = function () {
        var Active = {
            STATUS: 'Active'
        }
        var InActive = {
            STATUS: 'InActive'
        }
        
        $scope.IsReportValues.push(Active);
        $scope.IsReportValues.push(InActive);

    }
    $scope.Load_IsMenuShow();
    $scope.typeaheadSelectedMenuCategory = function (entity, selectedItem) {
        $scope.model.MODULE_ID = selectedItem.MODULE_ID;
        $scope.model.MODULE_NAME = selectedItem.MODULE_NAME;
    };


    $scope.onSelectParentMenu = function (parentMenuId) {
        let foundObject = $scope.ParantData.find(obj => obj.PARENT_MENU_ID == parentMenuId);
        $scope.model.MODULE_ID = foundObject.MODULE_ID;
        if ($scope.model.PARENT_MENU_ID == $scope.model.MENU_ID) {
            notificationservice.Notification('You cannot select own as a parent menu.', 1, 'Data Save Successfully !!');
        }
        $timeout(function () {
            $('.select2-single').trigger('change');
        });
    }

    
    $scope.ClearForm = function () {
        $scope.model.COMPANY_SEARCH_ID = 0;
        $scope.model.COMPANY_ID = 0;

        $scope.model.MENU_ID = 0;
        $scope.model.MENU_NAME = "";
        $scope.model.AREA = "";

        $scope.model.ORDER_BY_SLNO = 0;
        $scope.model.MODULE_ID = 0;
        $scope.model.MODULE = '';
        $scope.model.CONTROLLER = '';
        $scope.model.ACTION = '';
        $scope.model.HREF = '';
        $scope.model.STATUS = '';
        $scope.model.PARENT_MENU_ID = 0;
        $scope.model.PARENT_MENU = '';
        $scope.model.MENU_SHOW = 'Active';

    }

    
    




    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Manue List"));
    $scope.gridOptionsList = {
        enableSorting: true,
        enableFiltering: true,
        enableGridMenu: true,
        enableColumnMenus: false,
        showColumnFooter: false,
        enableHorizontalScrollbar: uiGridConstants.scrollbars.WHEN_NEEDED,
        enableVerticalScrollbar: uiGridConstants.scrollbars.WHEN_NEEDED,
        columnDefs: [
            {
                name: 'ROW_NO',
                field: 'ROW_NO',
                displayName: '#',
                enableFiltering: false,
                enableSorting: false,
                cellTemplate: '<div class="ui-grid-cell-contents">{{grid.renderContainers.body.visibleRowCache.indexOf(row) + 1}}</div>',
                width: '5%',
                pinnedLeft: true

            },
            { name: 'MENU_ID', field: 'MENU_ID', visible: false },
            { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false },
            {
                name: 'MENU_NAME',
                field: 'MENU_NAME',
                displayName: 'Menu Name',
                enableFiltering: true,
                width: '18%',
            },
            {
                name: 'SEQUENCE',
                field: 'SEQUENCE',
                displayName: 'Direction',
                enableFiltering: true,
                width: '26%',
            },
            {
                name: 'MODULE_NAME',
                field: 'MODULE_NAME',
                displayName: 'Module',
                enableFiltering: true,
                width: '9%',
            },
            {
                name: 'AREA',
                field: 'AREA',
                displayName: 'Area(SYS)',
                enableFiltering: true,
                visible:false,
                width: '10%',
            },
            {
                name: 'CONTROLLER',
                field: 'CONTROLLER',
                displayName: 'Controller(SYS)',
                visible: false,
                enableFiltering: true,
                width: '10%',
            },
            {
                name: 'ACTION',
                field: 'ACTION',
                displayName: 'Action(SYS)',
                visible: false,
                enableFiltering: true,
                width: '10%',
            },
            {
                name: 'ORDER_BY_SLNO',
                field: 'ORDER_BY_SLNO',
                displayName: 'Serial',
                enableFiltering: false,
                width: '6%',
            },
            {
                name: 'STATUS',
                field: 'STATUS',
                displayName: 'Status',
                enableFiltering: true,
                width: '70'
            },
            { name: 'HREF', field: 'HREF', displayName: 'URL', enableFiltering: true, width: '10%', visible: false},
            {
                name: 'MENU_SHOW',
                field: 'MENU_SHOW',
                displayName: 'Show Manu',
                enableFiltering: true,
                width: '80',
            }
            ,{
                name: 'Action', field: 'Action', displayName: 'Action', enableFiltering: false, width: '220', pinnedRight: true, cellTemplate:
                    '<button style="margin-bottom: 5px;margin-right: 10px"   ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" title="Edit Manu" class="btn btn-success mb-1"><i class="fa fa-edit"></i></button>'
                    + '<button style="margin-bottom: 5px;margin-right: 10px" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.ActivateMenu(row.entity.MENU_ID)" type="button" ng-disabled="row.entity.STATUS == \'Active\'" title="Active Manu" class="btn btn-primary mb-1">Active</i></button>'
                    + '<button style="margin-bottom: 5px;margin-right: 10px" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.DeactivateMenu(row.entity.MENU_ID)" type="button" ng-disabled="row.entity.STATUS == \'InActive\'" title="Deactive Manu" class="btn btn-danger mb-1">Inactive</i></i></button>'
            }
        ],
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
        }
    };


    $scope.SaveData = function (model) {
        $scope.showLoader = true;
        MenuMasterServices.AddOrUpdate(model).then(function (data) {
            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.model = { ...$scope.model, ...JSON.parse(JSON.stringify($scope.modelDefault)) };
                $scope.DataLoad();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }
    $scope.EditData = function (entity) {
    
        $scope.model = { ...$scope.model, ...entity };
        $scope.model.PARENT_MENU_ID = entity.PARENT_MENU_ID != null ? entity.PARENT_MENU_ID : 0;
        if ($scope.model.PARENT_MENU_ID>0) {
            $timeout(function () {
                $('.select2-single').trigger('change');
            });
        }
    }
    $scope.ActivateMenu = function (Id) {
        $scope.showLoader = true;
        MenuMasterServices.ActivateMenu(Id).then(function (data) {
            notificationservice.Notification(data.data, 1, 'Activated the selected Manu !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.DataLoad();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }
    $scope.DeactivateMenu = function (Id) {
        $scope.showLoader = true;
        MenuMasterServices.DeactivateMenu(Id).then(function (data) {
            notificationservice.Notification(data.data, 1, 'Deactivated the selected Manu!!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.DataLoad($scope.model.COMPANY_ID);
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
                    $scope.DataLoad($scope.model.COMPANY_ID);

                }
                else {
                    $scope.showLoader = false;
                }
            });
        }
    }

}]);

