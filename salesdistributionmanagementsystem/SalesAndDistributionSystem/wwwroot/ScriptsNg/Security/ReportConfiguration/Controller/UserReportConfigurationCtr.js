ngApp.controller('ngGridCtrl', ['$scope', 'ReportConfigurationServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, ReportConfigurationServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    $scope.model = { USER_ID: 0, USER_NAME: '', EMAIL: '', EMPLOYEE_ID: '' }


    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("User Report Config"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.ActivityID = [];
    $scope.UserLst = [];
    $scope.AutoCompleteDataLoadForUser = function (value) {
        if (value.length >= 3) {
            
            return ReportConfigurationServices.GetSearchableUsers(value).then(function (data) {

                $scope.UserLst = data.data;
                

                return $scope.UserLst;
            }, function (error) {
                alert(error);
                

                
            });
        }
    }
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'ReportConfiguration',
            Action_Name: 'UserReportConfig'
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

    $scope.typeaheadSelectedUser = function (entity, selectedItem) {
        $scope.model.USER_ID = selectedItem.USER_ID;
        $scope.model.USER_NAME = selectedItem.USER_NAME;
        $scope.model.EMAIL = selectedItem.EMAIL;
        $scope.model.EMPLOYEE_ID = selectedItem.EMPLOYEE_ID;

    };
    $scope.selectionOfPermission = function (entity, permissionfield) {
        
        $scope.ActivityID.push(entity.REPORT_ID);
        if (permissionfield == "ALL") {
            if (!entity.ALL_PERMISSION_CHECK) {
               
                entity.PDF_PERMISSION = "Active";
                entity.PDF_PERMISSION_CHECK = true;

                entity.PREVIEW_PERMISSION = "Active";
                entity.PREVIEW_PERMISSION_CHECK = true;

                entity.CSV_PERMISSION = "Active";
                entity.CSV_PERMISSION_CHECK = true;

               
            }
            else {
                entity.PDF_PERMISSION = "InActive";
                entity.PDF_PERMISSION_CHECK = false;

                entity.PREVIEW_PERMISSION = "InActive";
                entity.PREVIEW_PERMISSION_CHECK = false;

                entity.EDIT_PERMISSION = "InActive";
                entity.EDIT_PERMISSION_CHECK = false;

                entity.CSV_PERMISSION = "InActive";
                entity.CSV_PERMISSION_CHECK = false;

            
            }



        }
        
        if (permissionfield == "PDF_PERMISSION_CHECK") {
            if (!entity.PDF_PERMISSION_CHECK) {
                entity.PDF_PERMISSION = "Active";
                entity.PDF_PERMISSION_CHECK = true;


            } else {
                entity.PDF_PERMISSION = "InActive";
                entity.PDF_PERMISSION_CHECK = false;
            }

        }
        if (permissionfield == "PREVIEW_PERMISSION_CHECK") {
            if (!entity.PREVIEW_PERMISSION_CHECK) {
                entity.PREVIEW_PERMISSION = "Active";
                entity.PREVIEW_PERMISSION_CHECK = true;


            } else {
                entity.PREVIEW_PERMISSION = "InActive";
                entity.PREVIEW_PERMISSION_CHECK = false;
            }

        }
        if (permissionfield == "CSV_PERMISSION_CHECK") {
            if (!entity.CSV_PERMISSION_CHECK) {
                entity.CSV_PERMISSION = "Active";
                entity.CSV_PERMISSION_CHECK = true;


            } else {
                entity.CSV_PERMISSION = "InActive";
                entity.CSV_PERMISSION_CHECK = false;
            }

        }
      
    };
    $scope.filterData = function (entity) {

        if ( entity.PDF_PERMISSION == "Active" && entity.PREVIEW_PERMISSION == "Active"
            && entity.CSV_PERMISSION == "Active") {
            entity.ALL_PERMISSION_CHECK = true;
        }
        else {

            entity.ALL_PERMISSION_CHECK = false;
        }


      
        
        if (entity.PDF_PERMISSION == "Active") {

            entity.PDF_PERMISSION_CHECK = true;

        } else {
            entity.PDF_PERMISSION_CHECK = false;
        }


        if (entity.PREVIEW_PERMISSION == "Active") {

            entity.PREVIEW_PERMISSION_CHECK = true;


        } else {
            entity.PREVIEW_PERMISSION_CHECK = false;
        }


        if (entity.CSV_PERMISSION == "Active") {
            entity.CSV_PERMISSION_CHECK = true;

        } else {
            entity.CSV_PERMISSION_CHECK = false;
        }

    };


    $scope.gridOptionsList.columnDefs = [
        { name: 'SL', field: 'ROW_NO', enableFiltering: false, width: '50' }

        , { name: 'USER_ID', field: 'USER_ID', visible: false }
        , { name: 'REPORT_ID', field: 'REPORT_ID', visible: false }
        , { name: 'ID', field: 'ID', visible: false }


        , { name: 'MENU_ID', field: 'MENU_ID', visible: false }
        , { name: 'REPORT_NAME', field: 'REPORT_NAME', displayName: 'Report', enableFiltering: true, width: ' 25%' }

        , { name: 'MENU_NAME', field: 'MENU_NAME', displayName: 'Menu', enableFiltering: true, width: ' 15%' }

        , {
            name: 'ALL_PERMISSION_CHECK', field: 'ALL_PERMISSION_CHECK', displayName: 'ALL', enableFiltering: false, width: '15%', cellTemplate:
                '<input class=\"ngSelectionCheckbox\" ng-click="grid.appScope.selectionOfPermission(row.entity,\'ALL\')"  ng-model="row.entity.ALL_PERMISSION_CHECK" type=\"checkbox\" ng-checked=\"row.entity.ALL_PERMISSION_CHECK\" style="margin-top:0px !important" />'
        }
       
        , { name: 'PDF_PERMISSION', field: 'PDF_PERMISSION', visible: false }

        , {
            name: 'PDF_PERMISSION_CHECK', field: 'PDF_PERMISSION_CHECK', displayName: 'PDF', enableFiltering: false, width: '15%', cellTemplate:
                '<input class=\"ngSelectionCheckbox\" ng-click="grid.appScope.selectionOfPermission(row.entity,\'ADD_PERMISSION_CHECK\')" ng-model="row.entity.PDF_PERMISSION_CHECK" type=\"checkbox\" ng-checked=\"row.entity.PDF_PERMISSION_CHECK\" style="margin-top:0px !important" />'
        }
        , { name: 'PREVIEW_PERMISSION', field: 'PREVIEW_PERMISSION', visible: false }

        , {
            name: 'PREVIEW_PERMISSION_CHECK', field: 'PREVIEW_PERMISSION_CHECK', displayName: 'Preview', enableFiltering: false, width: '15%', cellTemplate:
                '<input class=\"ngSelectionCheckbox\" ng-click="grid.appScope.selectionOfPermission(row.entity,\'PREVIEW_PERMISSION_CHECK\')" type=\"checkbox\" ng-model="row.entity.PREVIEW_PERMISSION_CHECK" ng-checked=\"row.entity.PREVIEW_PERMISSION_CHECK\"  style="margin-top:0px !important" />'
        }
        , { name: 'CSV_PERMISSION', field: 'CSV_PERMISSION', visible: false }

        , {
            name: 'CSV_PERMISSION_CHECK', field: 'CSV_PERMISSION_CHECK', displayName: 'CSV', enableFiltering: false, width: '15%', cellTemplate:
                '<input class=\"ngSelectionCheckbox\" ng-click="grid.appScope.selectionOfPermission(row.entity,\'CSV_PERMISSION_CHECK\')" ng-model="row.entity.CSV_PERMISSION_CHECK" type=\"checkbox\" ng-checked=\"row.entity.CSV_PERMISSION_CHECK\"  style="margin-top:0px !important" />'
        }
      
    ];

    $scope.ReportUserConfigData = [];

    $scope.bindReportUserConfigData = function (model) {
        
        for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
            var activity = $scope.ActivityID.indexOf($scope.gridOptionsList.data[i].REPORT_ID);
            if (activity != (-1) || ($scope.gridOptionsList.data[i].ID != null && $scope.gridOptionsList.data[i].ID > 0)) {

                $scope.loadData = {
                    ID: $scope.gridOptionsList.data[i].ID,
                    USER_ID: model.USER_ID,
                    REPORT_ID: $scope.gridOptionsList.data[i].REPORT_ID,
                    PDF_PERMISSION: $scope.gridOptionsList.data[i].PDF_PERMISSION,
                    PREVIEW_PERMISSION: $scope.gridOptionsList.data[i].PREVIEW_PERMISSION,
                    CSV_PERMISSION: $scope.gridOptionsList.data[i].CSV_PERMISSION,
                   
                };
                
                $scope.ReportUserConfigData.push($scope.loadData);
            }

        }


    };
    $scope.SaveData = function (model) {
        
        model.ReportUserConfigData = $scope.gridOptionsList.data;
        $scope.bindReportUserConfigData(model);
        $scope.showLoader = true;
        
        ReportConfigurationServices.SaveUserReportConfiguration($scope.ReportUserConfigData).then(function (data) {

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

    $scope.UserReportConfigSelectionList = function () {
        
        var Id = $scope.model.USER_ID;
        $scope.showLoader = true;
        ReportConfigurationServices.UserReportConfigSelectionList(Id).then(function (data) {

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

