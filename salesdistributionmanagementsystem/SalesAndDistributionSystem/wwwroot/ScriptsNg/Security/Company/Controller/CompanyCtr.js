ngApp.controller('ngGridCtrl', ['$scope', 'CompanyService', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, CompanyService, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    $scope.model = { ID: 0, COMPANY_ID: 0, COMPANY_NAME: '', COMPANY_SHORT_NAME: '', COMPANY_ADDRESS1: '', COMPANY_ADDRESS1: '' }
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Company List"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }



    

    $scope.DataLoad = function () {
        $scope.showLoader = true;
        CompanyService.GetCompanyList().then(function (data) {

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
    $scope.ClearForm = function () {
        $scope.model.ID = 0;
        $scope.model.COMPANY_ID = 0;
        $scope.model.COMPANY_NAME = '';
        $scope.model.COMPANY_SHORT_NAME = '';
        $scope.model.COMPANY_ADDRESS1 = '';
        $scope.model.COMPANY_ADDRESS2 = '';

    }

   
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'Company',
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
    $scope.DataLoad();
    $scope.GetPermissionData();

    $scope.gridOptionsList.columnDefs = [
        { name: 'SL', field: 'ROW_NO', enableFiltering: false, width: 40 }

        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , { name: 'ID', field: 'ID', visible: false }

        , {
            name: 'COMPANY_NAME', field: 'COMPANY_NAME', displayName: 'Company Name', enableFiltering: false, width: '25%', cellTemplate:
                '<input required="required"   ng-model="row.entity.COMPANY_NAME"  class="pl-sm" />'
        }
        , {
            name: 'COMPANY_SHORT_NAME', field: 'COMPANY_SHORT_NAME', displayName: 'Short Name', enableFiltering: false, width: ' 10%', cellTemplate:
                '<input required="required"  type="text"  ng-model="row.entity.COMPANY_SHORT_NAME"  class="pl-sm" />'
        }
        , {
            name: 'COMPANY_ADDRESS1', field: 'COMPANY_ADDRESS1', displayName: 'Address 1', enableFiltering: false, width: ' 25%', cellTemplate:
                '<input required="required"  type="text"  ng-model="row.entity.COMPANY_ADDRESS1"  class="pl-sm" />'
        }
        , {
            name: 'COMPANY_ADDRESS2', field: 'COMPANY_ADDRESS2', displayName: 'Address 2', enableFiltering: false, width: ' 25%', cellTemplate:
                '<input required="required"  type="text"  ng-model="row.entity.COMPANY_ADDRESS2"  class="pl-sm" />'
        }

        , {
            name: 'Actions', displayName: 'Actions', enableFiltering: false, enableColumnMenu: false, width: '10%', cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
             '</div>'
        },

    ];

    $scope.EditData = function (entity) {
        
        $scope.model.ID = entity.ID;
        $scope.model.COMPANY_ID = entity.COMPANY_ID;
        $scope.model.COMPANY_NAME = entity.COMPANY_NAME;
        $scope.model.COMPANY_SHORT_NAME = entity.COMPANY_SHORT_NAME;
        $scope.model.COMPANY_ADDRESS1 = entity.COMPANY_ADDRESS1;
        $scope.model.COMPANY_ADDRESS2 = entity.COMPANY_ADDRESS2;

        $scope.SaveData($scope.model);

    }


    $scope.SaveData = function (model) {
        

        $scope.showLoader = true;
        
        CompanyService.AddOrUpdate(model).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.DataLoad();
                $scope.model.Name = "";
                $scope.model.SerialNo = "";
                $scope.ClearForm();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }




   

}]);

