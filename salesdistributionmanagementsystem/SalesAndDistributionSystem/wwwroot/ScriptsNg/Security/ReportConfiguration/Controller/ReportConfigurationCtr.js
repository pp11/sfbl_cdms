ngApp.controller('ngGridCtrl', ['$scope', 'ReportConfigurationServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, ReportConfigurationServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    $scope.model = { COMPANY_ID: 0, MENU_ID: 0, MENU_NAME: '', ORDER_BY_SLNO: 0, MENU_ID: 0, CONTROLLER: '', ACTION: '', HREF: '', STATUS: '', PARENT_MENU_ID: 0}
    $scope.ReportConfiguration = [];
    $scope.Companies = [];
    $scope.Status = [];
    $scope.showLoader = true;
    $scope.MenuData = [];
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Report Configuration"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }


  
    $scope.LoadMenuData = function () {
        
        return ReportConfigurationServices.GetMenu($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.MenuData = data.data;
            }, function (error) {
                
                alert(error);
                
            });
        
    }

    $scope.LoadMenuData();
    $scope.Load_Status = function () {
        var Active = {
            STATUS: 'InActive'
        }
        var InActive = {
            STATUS: 'Active'
        }
        
        $scope.Status.push(Active);
        $scope.Status.push(InActive);

    }
    $scope.Load_Status();
    $scope.typeaheadSelectedReportConfiguration = function (entity, selectedItem) {
        $scope.model.MENU_ID = selectedItem.MENU_ID;
        $scope.model.MENU_NAME = selectedItem.MENU_NAME;
    };


    $scope.DataLoad = function (companyId) {
        $scope.showLoader = true;
        ReportConfigurationServices.LoadData(companyId).then(function (data) {
            
            
            $scope.gridOptionsList.data = data.data;
            $scope.ParantData = data.data;
            $scope.model.COMPANY_SEARCH_ID = companyId;
            $scope.showLoader = false;
            
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.ClearForm = function () {
        $scope.model.COMPANY_SEARCH_ID = 0;
        $scope.model.COMPANY_ID = 0;

        $scope.model.MENU_ID = 0;
        $scope.model.REPORT_ID = "";
        $scope.model.REPORT_NAME = "";

        $scope.model.HAS_PDF = "";

        $scope.model.ORDER_BY_SLNO = 0;
        $scope.model.HAS_CSV = 0;
        $scope.model.HAS_PREVIEW = '';
        $scope.model.STATUS = '';

    }

    $scope.EditData = function (entity) {
        

        $scope.model.REPORT_ID = entity.REPORT_ID;
        $scope.model.REPORT_NAME = entity.REPORT_NAME;
        $scope.model.REPORT_TITLE = entity.REPORT_TITLE;

        $scope.model.ORDER_BY_SLNO = entity.ORDER_BY_SLNO;
        $scope.model.MENU_ID = entity.MENU_ID;
        $scope.model.HAS_PREVIEW = entity.HAS_PREVIEW;
        $scope.model.HAS_PDF = entity.HAS_PDF;
        $scope.model.STATUS = entity.STATUS;

        $scope.model.HAS_CSV = entity.HAS_CSV;
        $scope.SaveData($scope.model);

    }
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'ReportConfiguration',
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

            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;

        ReportConfigurationServices.GetCompanyList().then(function (data) {
            
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        ReportConfigurationServices.GetCompany().then(function (data) {
            
            
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

    $scope.DataLoad(0);
    $scope.GetPermissionData();
    $scope.CompaniesLoad();
    $scope.CompanyLoad();

    $scope.gridOptionsList.columnDefs = [
        { name: 'SL', field: 'ROW_NO', enableFiltering: false, width: '60' }

        , { name: 'REPORT_ID', field: 'REPORT_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }


        , {
            name: 'REPORT_NAME', field: 'REPORT_NAME', displayName: 'Report Name', enableFiltering: true, width: '18%', cellTemplate:
                '<input required="required"   ng-model="row.entity.REPORT_NAME"  class="pl-sm" />'
        }
        , {
            name: 'REPORT_TITLE', field: 'REPORT_TITLE', displayName: 'Report Title', enableFiltering: true, width: '18%', cellTemplate:
                '<input required="required"   ng-model="row.entity.REPORT_TITLE"  class="pl-sm" />'
        }
        , {
            name: 'ORDER_BY_SLNO', field: 'ORDER_BY_SLNO', displayName: 'Order', enableFiltering: true, width: '8%', cellTemplate:
                '<input required="required"  type="number"  ng-model="row.entity.ORDER_BY_SLNO"  class="pl-sm" />'
        }
        , { name: 'STATUS', field: 'STATUS', displayName: 'Status', enableFiltering: true, width: '10%' }
        , {
            name: 'HAS_PREVIEW', field: 'HAS_PREVIEW', displayName: 'Preview', enableFiltering: true, width: '10%', cellTemplate:
                '<select  class="select2-single form-control" id="HAS_PREVIEW" required name="HAS_PREVIEW" ng-model="row.entity.HAS_PREVIEW" style = "width:100%" ><option ng-repeat="item in grid.appScope.Status"  ng-selected="row.entity.HAS_PREVIEW == item.STATUS"  value="{{item.STATUS}}">{{ item.STATUS }}</option> </select ></div >'
        }
      
        , {
            name: 'HAS_PDF', field: 'HAS_PDF', displayName: 'PDF', enableFiltering: true, width: '10%', cellTemplate:
                '<select  class="select2-single form-control" id="HAS_PDF" required name="HAS_PDF" ng-model="row.entity.HAS_PDF" style = "width:100%" ><option ng-repeat="item in grid.appScope.Status"  ng-selected="row.entity.HAS_PDF == item.STATUS"  value="{{item.STATUS}}">{{ item.STATUS }}</option> </select ></div >'
        }
        , {
            name: 'HAS_CSV', field: 'HAS_CSV', displayName: 'Excel', enableFiltering: true, width: '12%', cellTemplate:
                '<select  class="select2-single form-control" id="HAS_CSV" required name="HAS_CSV" ng-model="row.entity.HAS_CSV" style = "width:100%" ><option ng-repeat="item in grid.appScope.Status"  ng-selected="row.entity.HAS_CSV == item.STATUS"  value="{{item.STATUS}}">{{ item.STATUS }}</option> </select ></div >'
        }
       
        , {
            name: 'MENU_ID', field: 'MENU_ID', displayName: 'Module', enableFiltering: true, width: '12%', cellTemplate:
                '<select  class="select2-single form-control pl-sm" id="MENU_ID"   ng-model="row.entity.MENU_ID" style = "width:100%"> <option ng-repeat="item in grid.appScope.MenuData" ng-value="{{item.MENU_ID}}" ng-selected="row.entity.MENU_ID == item.MENU_ID">{{ item.MENU_NAME }}</option> </select > '
        }
       
       , {
            name: 'Actions', displayName: 'Actions', width: '35%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
               '<div style="margin:1px;">' +
               '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
               '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.ActivateReport(row.entity.REPORT_ID)" type="button" class="btn btn-outline-success mb-1"  ng-disabled="row.entity.STATUS == \'Active\'">Activate</button>' +
               '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" type="button" class="btn btn-outline-secondary mb-1" ng-disabled="row.entity.STATUS == \'InActive\'" ng-click="grid.appScope.DeActivateReport(row.entity.REPORT_ID)">Deactive</button>' +
               '</div>'
        },

    ];

   


    $scope.SaveData = function (model) {
        
       
        $scope.showLoader = true;
        
        ReportConfigurationServices.AddOrUpdate(model).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.DataLoad($scope.model.COMPANY_ID);
              
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

    


    $scope.ActivateReport = function (Id) {
        
        $scope.showLoader = true;
        ReportConfigurationServices.ActivateReport(Id).then(function (data) {

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

    $scope.DeActivateReport = function (Id) {
        
        $scope.showLoader = true;
        ReportConfigurationServices.DeActivateReport(Id).then(function (data) {

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

}]);

