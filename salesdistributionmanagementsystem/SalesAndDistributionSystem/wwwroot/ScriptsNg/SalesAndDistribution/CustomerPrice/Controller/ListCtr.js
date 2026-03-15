ngApp.controller('ngGridCtrl', ['$scope', 'CustomerPriceInfoServices', 'AreaInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, CustomerPriceInfoServices, AreaInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
  
    //$scope.getPermissions = [];
    //$scope.Companies = [];
    //$scope.Status = [];
    $scope.model = { COMPANY_ID: 0, REGION_ID: 0, REGION_CODE: '', REGION_AREA_MST_STATUS: 'Active', EFFECT_START_DATE: '', EFFECT_END_DATE: '', REMARKS: '' }
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Customer Price Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'CUSTOMER_ID', field: 'CUSTOMER_ID', visible: false }

        , { name: 'CUSTOMER_PRICE_MSTID', field: 'CUSTOMER_PRICE_MSTID', visible: false }

        , {
            name: 'CUSTOMER_NAME', field: 'CUSTOMER_NAME', displayName: 'Customer Name', enableFiltering: true, width: '20%'
        }
        , {
            name: 'CUSTOMER_CODE', field: 'CUSTOMER_CODE', displayName: 'Customer Code', enableFiltering: true, width: '10%'
        }

        , {
            name: 'EFFECT_START_DATE', field: 'EFFECT_START_DATE', displayName: 'EFFECT START DATE', enableFiltering: true, width: '20%'
        }
        , {
            name: 'EFFECT_END_DATE', field: 'EFFECT_END_DATE', displayName: 'EFFECT END DATE', enableFiltering: true, width: '20%'
        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'REMARKS', enableFiltering: true, width: '20%'
                               
        }
      
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '<button style="margin-bottom: 5px;" ng-click="grid.appScope.LikeEntry(row.entity)" type="button" class="btn btn-outline-primary mb-1">Like Entry</button>' +

                '</div>'
        }

    ];

    $scope.EditData = function (entity) {
        
        window.location = "/SalesAndDistribution/PriceInfo/CustomerPriceInfo?Id=" + entity.CUSTOMER_PRICE_MSTID_ENCRYPTED;

    }
    $scope.LikeEntry = function (entity) {

        window.location = "/SalesAndDistribution/PriceInfo/CustomerPriceInfo?Id=" + entity.CUSTOMER_PRICE_MSTID_ENCRYPTED+"&mode=like_entry";

    }
    

    $scope.DataLoad = function (companyId,customerId) {
        
        $scope.showLoader = true;

        CustomerPriceInfoServices.LoadData(companyId).then(function (data) {
            
            
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            //$scope.model.COMPANY_SEARCH_ID = companyId;
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        AreaInfoServices.GetCompany().then(function (data) {
            
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
    //$scope.CompaniesLoad = function () {
    //    $scope.showLoader = true;

    //    AreaInfoServices.GetCompanyList().then(function (data) {
    //        
    //        $scope.Companies = data.data;
    //        $scope.showLoader = false;
    //    }, function (error) {
    //        alert(error);
    
    //        $scope.showLoader = false;

    //    });
    //}


    //$scope.ClearForm = function () {
    //    window.location = "/SalesAndDistribution/RegionRegionRelation/List";
    //}



    //$scope.GetPermissionData = function () {
    //    $scope.showLoader = true;
    //    
    //    $scope.permissionReqModel = {
    //        Controller_Name: 'RegionAreaRelation',
    //        Action_Name: 'List'
    //    }
    //    permissionProvider.GetPermission($scope.permissionReqModel).then(function (data) {
    //        
    //        
    //        $scope.getPermissions = data.data;
    //        $scope.model.ADD_PERMISSION = $scope.getPermissions.adD_PERMISSION;
    //        $scope.model.EDIT_PERMISSION = $scope.getPermissions.ediT_PERMISSION;
    //        $scope.model.DELETE_PERMISSION = $scope.getPermissions.deletE_PERMISSION;
    //        $scope.model.LIST_VIEW = $scope.getPermissions.lisT_VIEW;
    //        $scope.model.DETAIL_VIEW = $scope.getPermissions.detaiL_VIEW;
    //        $scope.model.DOWNLOAD_PERMISSION = $scope.getPermissions.downloaD_PERMISSION;
    //        $scope.model.USER_TYPE = $scope.getPermissions.useR_TYPE;

    //        $scope.showLoader = false;
    //    }, function (error) {
    //        alert(error);
    
    //        $scope.showLoader = false;

    //    });
    //}

    //$scope.DataLoad(0);
    //$scope.GetPermissionData();
    //$scope.CompaniesLoad();
    $scope.CompanyLoad();


}]);

