ngApp.controller('ngGridCtrl', ['$scope', 'DepotCustomerRelationServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, DepotCustomerRelationService, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'

    $scope.model = { MST_ID: 0, DEPOT_ID: 0, DEPOT_CODE: '', EFFECT_START_DATE: '', EFFECT_END_DATE: '', STATUS: '', COMPANY_ID: 0, REMARKS: '' }

    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 0, COMPANY_ID: 0, DTL_ID: 0, MST_ID: 0, CUSTOMER_ID: 0, CUSTOMER_CODE: '', STATUS: 'Active', EFFECT_START_DATE: '', EFFECT_END_DATE: '', REMARKS: ''
        }
    }

    $scope.Customers = [];
    $scope.CustomersList = [];
    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];
    $scope.Unit = [];

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration(" Depot Customer Relation Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }

    $scope.gridOptionsList.data = [];
    //Grid Register
    $scope.gridOptionsList = {
        data: [$scope.GridDefalutData()]
    }

    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }

        , { name: 'DTL_ID', field: 'DTL_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , { name: 'CUSTOMER_ID', field: 'CUSTOMER_ID', visible: false }
        , {
            name: 'CUSTOMER_CODE', field: 'CUSTOMER_CODE', displayName: 'Customer Code', enableFiltering: false, width: '20%', cellTemplate:
                '<select class="select2-single form-control" data-select2-id="{{row.entity.CUSTOMER_ID_GR}}" id="CUSTOMER_CODE" ng-disabled="row.entity.ROW_NO !=0"' +
                'name="CUSTOMER_CODE" ng-model="row.entity.CUSTOMER_CODE" style="width:100%" >' +
                '<option ng-repeat="item in grid.appScope.Customers" ng-selected="item.CUSTOMER_CODE == row.entity.CUSTOMER_CODE" value="{{item.CUSTOMER_ID}}">{{ item.CUSTOMER_CODE }} -- {{ item.CUSTOMER_NAME }}</option>' +
                '</select>'
        }
        , {
            name: 'EFFECT_START_DATE', field: 'EFFECT_START_DATE', displayName: 'Start', enableFiltering: false, width: '20%', cellTemplate:
                '<div class="simple-date2">'
                + '<div class="input-group date">'
                + '<div class="input-group-prepend">'
                + '</div>'
                + '<input  type="text" readonly datepicker class="form-control"  ng-disabled="row.entity.ROW_NO !=0" ng-model="row.entity.EFFECT_START_DATE" placeholder="dd/mm/yyyy" id="EFFECT_START_DATE">'
                + '</div>'
                + '</div>'
        }
        , {
            name: 'EFFECT_END_DATE', field: 'EFFECT_END_DATE', displayName: 'End', enableFiltering: false, width: '20%', cellTemplate:
                '<div class="simple-date2">'
                + '<div class="input-group date">'
                + '<div class="input-group-prepend">'
                + '</div>'
                + '<input  type="text" readonly datepicker class="form-control"  ng-disabled="row.entity.ROW_NO !=0" ng-model="row.entity.EFFECT_END_DATE" placeholder="dd/mm/yyyy"  id="EFFECT_END_DATE">'
                + '</div>'
                + '</div>'
        }
        , { name: 'STATUS', field: 'STATUS', displayName: 'Status', enableFiltering: true, width: '20%' }
        , {
            name: 'Action', displayName: 'Action', width: '20%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.EditItem(row.entity)" type="button" class="btn btn-outline-success mb-1"><img src="/img/edit-icon.png" height="15px" ></button>' +

                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO == 0" ng-click="grid.appScope.addNewRow()" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                '</div>'
        },
    ];

    //Add New Row
    $scope.addNewRow = () => {
        
        if ($scope.gridOptionsList.data.length > 0 && $scope.gridOptionsList.data[0].CUSTOMER_CODE != null && $scope.gridOptionsList.data[0].CUSTOMER_CODE != '' && $scope.gridOptionsList.data[0].CUSTOMER_CODE != 'undefined') {
            //var result = $scope.CheckDateValidation($scope.gridOptionsList.data[0]);
            //if (result == "true") {
                var newRow = {
                    ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, DTL_ID: $scope.gridOptionsList.data[0].DTL_ID, MST_ID: $scope.gridOptionsList.data[0].MST_ID, CUSTOMER_ID: $scope.gridOptionsList.data[0].CUSTOMER_ID, CUSTOMER_CODE: $scope.gridOptionsList.data[0].CUSTOMER_CODE, EFFECT_START_DATE: $scope.gridOptionsList.data[0].EFFECT_START_DATE, EFFECT_END_DATE: $scope.gridOptionsList.data[0].EFFECT_END_DATE, STATUS: $scope.gridOptionsList.data[0].STATUS, REMARKS: $scope.gridOptionsList.data[0].REMARKS
                }
                var newRowSelected = {
                    CUSTOMER_CODE: $scope.gridOptionsList.data[0].CUSTOMER_CODE
                }
                $scope.CustomersList.push(newRowSelected);
                $scope.gridOptionsList.data.push(newRow);
                $scope.gridOptionsList.data[0] = $scope.GridDefalutData();

           // }

        } else {
            notificationservice.Notification("Please Enter Valid Customer", "", 'Only Single Row Left!!');
        }
        $scope.rowNumberGenerate();
    };

    //Edit Row
    $scope.EditItem = (entity) => {
        
        if ($scope.gridOptionsList.data.length > 0) {

            var newRow = {
                ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, DTL_ID: entity.DTL_ID, MST_ID: entity.MST_ID, CUSTOMER_ID: entity.CUSTOMER_ID, CUSTOMER_CODE: entity.CUSTOMER_CODE, EFFECT_START_DATE: entity.EFFECT_START_DATE, EFFECT_END_DATE: entity.EFFECT_END_DATE, STATUS: entity.STATUS, REMARKS: entity.REMARKS
            }
            $scope.gridOptionsList.data[0] = newRow;
        } else {
            notificationservice.Notification("Please Select Valid data", "", 'Only Single Row Left!!');
        }
        $scope.rowNumberGenerate();
        $scope.removeItem(entity);
    };

    // Grid Remove Single Row
    $scope.removeItem = function (entity) {
        
        if ($scope.gridOptionsList.data.length > 1) {
            var index = $scope.gridOptionsList.data.indexOf(entity);
            if ($scope.gridOptionsList.data.length > 0) {
                $scope.gridOptionsList.data.splice(index, 1);
            }
            $scope.rowNumberGenerate();
        } else {
            notificationservice.Notification("Only Single Row Left!!", "", 'Only Single Row Left!!');
        }
    }

    //Generate New Row No
    $scope.rowNumberGenerate = function () {
        
        for (let i = 0; i < $scope.gridOptionsList.data.length; i++) {
            
            $scope.gridOptionsList.data[i].ROW_NO = i;
            if ($scope.gridOptionsList.data[i].CUSTOMER_CODE == '') {
                $scope.gridOptionsList.data[i].CUSTOMER_ID = 0;
                $scope.gridOptionsList.data[i].EFFECT_START_DATE = '';
                $scope.gridOptionsList.data[i].EFFECT_END_DATE = '';
            }
        }
    }

    $scope.CustomersLoad = function () {
        $scope.showLoader = true;
        DepotCustomerRelationService.GetCustomer($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.Customers = data.data;

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }

    $scope.DataLoad = function (companyId) {
        
        $scope.showLoader = true;

        DepotCustomerRelationService.LoadData(companyId).then(function (data) {
            
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

        DepotCustomerRelationService.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseFloat(data.data);
            
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.CompanyNameLoad = function () {
        $scope.showLoader = true;

        DepotCustomerRelationService.GetCompanyName().then(function (data) {
            $scope.model.COMPANY_NAME = data.data;
         
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
   
    $scope.LoadUnitData = function () {
        $scope.showLoader = true;

        DepotCustomerRelationService.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            
            
            $scope.Unit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }

    $scope.LoadFormData = function () {
        
        $scope.showLoader = true;

        DepotCustomerRelationService.LoadData($scope.model.COMPANY_ID).then(function (data) {
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = $scope.model.COMPANY_ID;

            for (var i in data.data) {
                
                if (data.data[i].DEPOT_CODE == $scope.model.DEPOT_CODE) {
                    
                    $scope.model.MST_ID_ENCRYPTED = data.data[i].MST_ID_ENCRYPTED;
                    $scope.GetEditDataById($scope.model.MST_ID_ENCRYPTED);
                    $scope.addNewRow();
                }
            }
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    // This Method work is Edit Data Loading
    $scope.GetEditDataById = function (value) {
        
        if (value != undefined && value.length > 0) {
            DepotCustomerRelationService.GetEditDataById(value).then(function (data) {
                
                if (data.data != null && data.data.Depot_Customer_Dtls != null && data.data.Depot_Customer_Dtls.length > 0) {
                    $scope.model = data.data;
                    $scope.CompanyLoad();
                    $scope.CompanyNameLoad();
                    $interval(function () {
                        $scope.LoadUnit_ID();
                    }, 800, 2);
                    if ($scope.model.STATUS == null) {
                        $scope.model.STATUS = 'Active';
                    }
                    if (data.data.Depot_Customer_Dtls != null) {
                        $scope.gridOptionsList.data = data.data.Depot_Customer_Dtls;

                    }
                    var CustomerData = {
                        CUSTOMER_CODE: $scope.model.CUSTOMER_CODE,
                        CUSTOMER_ID: $scope.model.CUSTOMER_ID,
                    }
                    $scope.Customers.push(CustomerData);
                    
                    $scope.addNewRow();
                }
                $scope.rowNumberGenerate();
                $scope.showLoader = false;
                
            }, function (error) {
                alert(error);
                
            });
        }
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
        window.location = "/SalesAndDistribution/DepotCustomerRelation/DepotCustomerRelation";

    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'DepotCustomerRelation',
            Action_Name: 'DepotCustomerRelation'
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

    //$scope.DataLoad(0);
    $scope.GetPermissionData();
    $scope.CompanyLoad();
    $scope.CompanyNameLoad();

    $scope.LoadStatus();
    $scope.CustomersLoad();
    $scope.LoadUnitData();

    $scope.LoadUnit_ID = function () {
        $('#DEPOT_ID').trigger('change');

    }
    $scope.SaveData = function (model) {
        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);
        $scope.model.DEPOT_ID = parseInt($scope.model.DEPOT_ID);
        $scope.gridOptionsList.data = $scope.gridOptionsList.data.filter((x) => x.ROW_NO !== 0);
        for (var i in $scope.gridOptionsList.data) {
            const searchIndex = $scope.Customers.findIndex((x) => x.CUSTOMER_ID == $scope.gridOptionsList.data[i].CUSTOMER_ID);
            if (searchIndex != -1) {
                $scope.gridOptionsList.data[i].CUSTOMER_ID = parseInt($scope.Customers[searchIndex].CUSTOMER_ID);
                $scope.gridOptionsList.data[i].CUSTOMER_CODE = $scope.Customers[searchIndex].CUSTOMER_CODE;
            }
        }
        $scope.model.Depot_Customer_Dtls = $scope.gridOptionsList.data;
        $scope.showLoader = true;
        DepotCustomerRelationService.AddOrUpdate(model).then(function (data) {
            debugger;
            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.GetPermissionData();
                $scope.CompanyLoad();
                $scope.LoadFormData();
                $scope.CustomersLoad();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

}]);

