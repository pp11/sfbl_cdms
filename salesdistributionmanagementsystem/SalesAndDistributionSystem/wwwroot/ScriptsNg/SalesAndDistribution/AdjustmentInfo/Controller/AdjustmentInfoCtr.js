ngApp.controller('ngGridCtrl', ['$scope', 'AdjustmentInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, AdjustmentInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    //$scope.model = { COMPANY_ID: 0, ADJUSTMENT_ID: 0, ADJUSTMENT_NAME: '', ADJUSTMENT_CODE: '', REMARKS: '', ADJUSTMENT_STATUS: 'Active' }
    $scope.model = {}

    $scope.CompanyLoad = function () {
        $scope.showLoader = true;
        AdjustmentInfoServices.GetCompanyId().then(function (data) {
            $scope.model.COMPANY_ID = data.data;          
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }


    //$scope.GetCompanyId = function () {
    //    $scope.showLoader = true;
    //    AdjustmentInfoServices.GetCompanyId().then(function (data) {
    //        $scope.model.COMPANY_ID = data.data;
    //        $scope.showLoader = false;
    //    }, function (error) {
    
    //        $scope.showLoader = false;
    //    });
    //}

    //$scope.LoadStatus = function () {
    //    var Active = {
    //        ADJUSTMENT_STATUS: 'Active'
    //    }
    //    var InActive = {
    //        ADJUSTMENT_STATUS: 'InActive'
    //    }
    //    $scope.Status.push(Active);
    //    $scope.Status.push(InActive);

    //}
    $scope.ClearForm = function () {
        $scope.model.ADJUSTMENT_ID = 0;
        $scope.model.ADJUSTMENT_NAME = '';         
        $scope.model.ADJUSTMENT_STATUS = 'Active';
        $scope.model.REMARKS = '';

    }

    $scope.CompanyLoad();
    //$scope.LoadStatus();


    

}]);

