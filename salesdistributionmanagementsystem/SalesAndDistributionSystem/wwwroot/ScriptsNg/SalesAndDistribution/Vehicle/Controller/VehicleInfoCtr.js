ngApp.controller('ngGridCtrl', ['$scope', 'VehicleInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, VehicleInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {
        VEHICLE_ID: 0, VEHICLE_NO: '', VEHICLE_DESCRIPTION: '', VEHICLE_TOTAL_VOLUME: '', VOLUME_UNIT: '', VEHICLE_TOTAL_WEIGHT: '', WEIGHT_UNIT: '', DRIVER_ID: 0, DRIVER_NAME: '', STATUS: '', COMPANY_ID: 0, REMARKS: ''
    }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Drivers = [];
    $scope.Status = [];
    $scope.Units = [];
    $scope.buttonName = 'Save';
    $scope.UnitWeight = [];
    $scope.UnitVolume = [];
    $scope.NumberPattern = "/^[0-9]+(\.[0-9]{1,9})?$/";

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Vehicle Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }

        , { name: 'VEHICLE_ID', field: 'VEHICLE_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }

        , {
            name: 'VEHICLE_NO', field: 'VEHICLE_NO', displayName: 'Vehicle No', enableFiltering: true, width: '20%'
        }
        , {
            name: 'VEHICLE_DESCRIPTION', field: 'VEHICLE_DESCRIPTION', displayName: 'Description', enableFiltering: true, width: '12%'
        }
        , {
            name: 'VEHICLE_TOTAL_VOLUME', field: 'VEHICLE_TOTAL_VOLUME', displayName: 'Total Volume', enableFiltering: true, width: '18%'
        }
        , {
            name: 'VOLUME_UNIT', field: 'VOLUME_UNIT', visible: false
        }
        , {
            name: 'VEHICLE_TOTAL_WEIGHT', field: 'VEHICLE_TOTAL_WEIGHT', displayName: 'Total Weight', enableFiltering: true, width: '18%'
        }
        , {
            name: 'WEIGHT_UNIT', field: 'WEIGHT_UNIT', visible: false
        }
        , {
            name: 'DRIVER_ID', field: 'DRIVER_ID', visible: false
        }
        , {
            name: 'DRIVER_NAME', field: 'DRIVER_NAME', displayName: 'Driver Name', enableFiltering: true, width: '18%'
        }
        , {
            name: 'UNIT_NAME', field: 'UNIT_NAME', displayName: 'Depot Name', enableFiltering: true, width: '18%'
        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '15%'
        }
        , { name: 'STATUS', field: 'STATUS', displayName: 'Status', enableFiltering: true, width: '15%' }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];

    $scope.LoadUnitData = function () {
        $scope.showLoader = true;
        VehicleInfoServices.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Units = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });
            VehicleInfoServices.GetUnit().then(response => {
                $scope.model.UNIT_ID = response.data;
                $scope.changeUnitId();
            })

            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }


    $scope.changeUnitId = function () {
        VehicleInfoServices.GetDriver($scope.model.COMPANY_ID).then(function (data) {
            let filteredItems = data.data.filter(item => item.UNIT_ID == $scope.model.UNIT_ID);
            $scope.Drivers = filteredItems;
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });

    }

    $scope.GetDriver = function () {
        $scope.showLoader = true;
        VehicleInfoServices.GetDriver($scope.model.COMPANY_ID).then(function (data) {
            $scope.Drivers = data.data;
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }

    $scope.WeightUnitLoad = function () {
        $scope.showLoader = true;

        VehicleInfoServices.GetWeightUnit($scope.model.COMPANY_ID).then(function (data) {
            $scope.UnitWeight = data.data;
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }


    $scope.VolumneUnitLoad = function () {
        $scope.showLoader = true;

        VehicleInfoServices.GetVolumeUnit($scope.model.COMPANY_ID).then(function (data) {
            $scope.UnitVolume = data.data;
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }

    $scope.DataLoad = function (companyId) {

        $scope.showLoader = true;

        VehicleInfoServices.LoadData(companyId).then(function (data) {


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
        VehicleInfoServices.GetCompany().then(function (data) {

            $scope.model.COMPANY_ID = parseFloat(data.data);
            $interval(function () {
                $('#COMPANY_ID').trigger('change');
            }, 800, 4);

            $scope.showLoader = false;
            $scope.LoadUnitData();

        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;

        VehicleInfoServices.GetCompanyList().then(function (data) {

            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }


    $scope.LoadFormData = function () {

        $scope.showLoader = true;

        VehicleInfoServices.LoadData($scope.model.COMPANY_ID).then(function (data) {

            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = $scope.model.COMPANY_ID;

            //for (var i in $scope.gridOptionsList.data) {

            //    if ($scope.gridOptionsList.data[i].VEHICLE_NO == $scope.model.VEHICLE_NO) {
            //        $scope.model.VEHICLE_ID = $scope.gridOptionsList.data[i].VEHICLE_ID;
            //        $scope.model.VEHICLE_DESCRIPTION = $scope.gridOptionsList.data[i].VEHICLE_DESCRIPTION;
            //        $scope.model.VEHICLE_TOTAL_VOLUME = $scope.gridOptionsList.data[i].VEHICLE_TOTAL_VOLUME;
            //        $scope.model.VOLUME_UNIT = $scope.gridOptionsList.data[i].VOLUME_UNIT;
            //        $scope.model.VEHICLE_TOTAL_WEIGHT = $scope.gridOptionsList.data[i].VEHICLE_TOTAL_WEIGHT;
            //        $scope.model.WEIGHT_UNIT = $scope.gridOptionsList.data[i].WEIGHT_UNIT;
            //        $scope.model.DRIVER_ID = $scope.gridOptionsList.data[i].DRIVER_ID;
            //        $scope.model.DRIVER_NAME = $scope.gridOptionsList.data[i].DRIVER_NAME;
            //        $scope.model.STATUS = $scope.gridOptionsList.data[i].STATUS;
            //        $scope.model.COMPANY_ID = $scope.gridOptionsList.data[i].COMPANY_ID;
            //        $scope.model.REMARKS = $scope.gridOptionsList.data[i].REMARKS;
            //    }
            //}
        }, function (error) {

            alert(error);

            $scope.showLoader = false;

        });
    }

    $scope.LoadStatus = function () {
        var Active = {
            STATUS: 'Active'
        }
        var InActive = {
            STATUS: 'InActive'
        }
        $scope.Status.push(Active);
        $scope.Status.push(InActive);

    }
    $scope.ClearForm = function () {
        $scope.buttonName = 'Save';
        $scope.model.VEHICLE_ID = 0;
        $scope.model.VEHICLE_NO = '';
        $scope.model.VEHICLE_DESCRIPTION = '';
        $scope.model.VEHICLE_TOTAL_VOLUME = '';
        $scope.model.VOLUME_UNIT = '';
        $scope.model.VEHICLE_TOTAL_WEIGHT = '';
        $scope.model.WEIGHT_UNIT = '';
        $scope.model.DRIVER_ID = '';
        $scope.model.DRIVER_NAME = '';
        $scope.model.STATUS = 'Active';
        $scope.model.REMARKS = '';
        $scope.model.UNIT_ID = '';
    }

    $scope.EditData = function (entity) {
        $scope.buttonName = 'Update';
        $scope.model.VEHICLE_ID = entity.VEHICLE_ID;
        $scope.model.VEHICLE_NO = entity.VEHICLE_NO;
        $scope.model.VEHICLE_DESCRIPTION = entity.VEHICLE_DESCRIPTION;
        $scope.model.VEHICLE_TOTAL_VOLUME = entity.VEHICLE_TOTAL_VOLUME;
        $scope.model.VOLUME_UNIT = entity.VOLUME_UNIT;
        $scope.model.VEHICLE_TOTAL_WEIGHT = entity.VEHICLE_TOTAL_WEIGHT;
        $scope.model.WEIGHT_UNIT = entity.WEIGHT_UNIT;
        $scope.model.STATUS = entity.STATUS;
        $scope.model.REMARKS = entity.REMARKS;
        $scope.model.UNIT_ID = entity.UNIT_ID.toString();
        $scope.changeUnitId();
        $scope.model.DRIVER_ID = entity.DRIVER_ID;
        $scope.model.DRIVER_NAME = entity.DRIVER_NAME;
        $interval(function () {
            $('#COMPANY_ID').trigger('change');
        }, 800, 4);

    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;

        $scope.permissionReqModel = {
            Controller_Name: 'Vehicle',
            Action_Name: 'VehicleInfo'
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
    $scope.LoadStatus();
    //$scope.GetDriver();
    $scope.WeightUnitLoad();
    $scope.VolumneUnitLoad();


    $scope.SaveData = function (model) {
        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);
        $scope.model.DRIVER_NAME = $("#DRIVER_ID option:selected").text();

        $scope.showLoader = true;


        VehicleInfoServices.AddOrUpdate(model).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.GetPermissionData();
                $scope.CompanyLoad();
                $scope.LoadFormData();
                $scope.ClearForm();
                $scope.GetDriver();

            }
            else {
                $scope.showLoader = false;
            }
        });
    }

}]);

