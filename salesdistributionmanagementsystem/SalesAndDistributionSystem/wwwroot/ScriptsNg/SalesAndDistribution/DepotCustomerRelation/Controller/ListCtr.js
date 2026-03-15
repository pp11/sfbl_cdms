ngApp.controller('ngGridCtrl', ['$scope', 'DepotCustomerRelationServices', 'CustomerInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, DepotCustomerRelationServices, CustomerInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    $scope.model = { MST_ID: 0, DEPOT_ID: 0, DEPOT_CODE: 0, EFFECT_START_DATE: '', EFFECT_END_DATE: '', STATUS: '', COMPANY_ID: 0, REMARKS: '' }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Customer Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        
        , { name: 'MST_ID', field: 'MST_ID', visible: false }
        
        , { name: 'DEPOT_ID', field: 'DEPOT_ID', visible: false }

        , {
            name: 'DEPOT_CODE', field: 'DEPOT_CODE', displayName: 'Depo Code', enableFiltering: true, width: '10%'
        }, {
            name: 'DEPOT_NAME', field: 'DEPOT_NAME', displayName: 'Depo Name', enableFiltering: true, width: '20%'
        }
        , {
            name: 'EFFECT_START_DATE', field: 'EFFECT_START_DATE', displayName: 'Start', enableFiltering: true, width: '10%'
        }
        , {
            name: 'EFFECT_END_DATE', field: 'EFFECT_END_DATE', displayName: 'End', enableFiltering: true, width: '10%'
        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '10%'
        }
        , { name: 'STATUS', field: 'STATUS', displayName: 'Status', enableFiltering: true, width: '10%' }
        , {
            name: 'Action', displayName: 'Action', width: '18%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];
    $scope.gridOptionsList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.EditData = function (entity) {
        
        window.location = "/SalesAndDistribution/DepotCustomerRelation/DepotCustomerRelation?Id=" + entity.MST_ID_ENCRYPTED;

    }
    
    $scope.DataLoad = function (companyId) {
        
        $scope.showLoader = true;

        DepotCustomerRelationServices.LoadData(companyId).then(function (data) {
            console.log(data.data);
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = companyId;
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        CustomerInfoServices.GetCompany().then(function (data) {
            
            $scope.model.COMPANY_ID = parseFloat(data.data);
            $scope.DataLoad($scope.model.COMPANY_ID);
            $interval(function () {
                $('#COMPANY_ID').trigger('change');
            }, 800, 4);
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;

        CustomerInfoServices.GetCompanyList().then(function (data) {
            
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
   
   
    $scope.ClearForm = function () {
        window.location = "/SalesAndDistribution/DepotCustomerRelation/List";
    }

    

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'DepotCustomerRelation',
            Action_Name: 'List'
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

    $scope.DataLoad(0);
    $scope.GetPermissionData();
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
  

}]);

