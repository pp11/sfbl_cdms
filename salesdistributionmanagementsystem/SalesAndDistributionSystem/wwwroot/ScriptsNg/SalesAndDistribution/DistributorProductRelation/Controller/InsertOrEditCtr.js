ngApp.controller('ngGridCtrl', ['$scope', 'InsertOrEditServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InsertOrEditServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { ROW_NO: 0, MST_ID: 0, distributor_Product_Dtls: [] }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];
    $scope.Distributor_Product_Mst = [];
    $scope.Products = [];
    $scope.ProductsListAll = [];
    $scope.Units = [];
    $scope.DefalutData = function () {
        return { MST_ID: 0, DISTRIBUTOR_PROPDUCT_TYPE: '' }
    }

    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 0, MST_ID: 0, DTL_ID: 0, SKU_ID: '', SKU_CODE: '', UNIT_ID: [], UNIT_NAME:''
        }
    }
    //Grid Register
    $scope.gridOptions = (gridregistrationservice.GridRegistration("Relation Info"));
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptions.data = [];
    $scope.gridOptions = {
        data: [$scope.GridDefalutData()]
    }


    //Generate New Row No
    $scope.rowNumberGenerate = function () {
        for (let i = 0; i < $scope.gridOptions.data.length; i++) {
            $scope.gridOptions.data[i].ROW_NO = i;
            if ($scope.gridOptions.data[i].SKU_CODE == '') {
                $scope.gridOptions.data[i].SKU_ID = '';
                $scope.gridOptions.data[i].DTL_ID = 0;
                $scope.gridOptions.data[i].MST_ID = 0;

            }
        }
    }


    //Add New Row
    $scope.addNewRow = () => {
        debugger
        var count = 0;
        for (var i = 0; i < $scope.gridOptions.data.length; i++) {
            if ($scope.gridOptions.data[i].SKU_CODE == $scope.gridOptions.data[0].SKU_CODE) {
                count++;
            }
        }
        if (count == 1 || count == 0) {
            if ($scope.gridOptions.data.length > 0 && $scope.gridOptions.data[0].SKU_CODE != null && $scope.gridOptions.data[0].SKU_CODE != '' && $scope.gridOptions.data[0].SKU_CODE != 'undefined') {
                var unit_names = "";
                if ($scope.gridOptions.data[0].UNIT_ID.length > 0) {
                    for (var i = 0; i < $scope.gridOptions.data[0].UNIT_ID.length; i++) {
                        if (i == 0) {
                            unit_names = unit_names + $scope.Units.filter(x => x.UNIT_ID == $scope.gridOptions.data[0].UNIT_ID[i])[0].UNIT_NAME;

                        } else {
                            unit_names = unit_names + " | " + $scope.Units.filter(x => x.UNIT_ID == $scope.gridOptions.data[0].UNIT_ID[i])[0].UNIT_NAME;

                        }
                    }
                }

                var newRow = {
                    ROW_NO: 1, MST_ID: $scope.gridOptions.data[0].MST_ID, DTL_ID: $scope.gridOptions.data[0].DTL_ID, SKU_ID: $scope.gridOptions.data[0].SKU_ID, SKU_CODE: $scope.gridOptions.data[0].SKU_CODE, SKU_NAME: $scope.gridOptions.data[0].SKU_NAME, UNIT_ID: $scope.gridOptions.data[0].UNIT_ID, UNIT_NAME: unit_names
                }
                $scope.gridOptions.data.push(newRow);
                $scope.gridOptions.data[0].SKU_ID = 0;
                $scope.gridOptions.data[0].SKU_CODE = '';
                $scope.gridOptions.data[0].SKU_NAME = '';

            } else {
                notificationservice.Notification("Please Enter Valid SKU First", "", 'Only Single Row Left!!');

            }
        } else {
            notificationservice.Notification("Product already exist !", "", 'Product already exist !!!');
        }
        $scope.rowNumberGenerate();

    };

    $scope.EditItem = (entity) => {
        if ($scope.gridOptions.data.length > 0) {
            var newRow = {
                ROW_NO: 1, MST_ID: entity.MST_ID, DTL_ID: entity.DTL_ID, SKU_ID: entity.SKU_ID, SKU_CODE: entity.SKU_CODE
            }
            $scope.gridOptions.data[0] = newRow;


        } else {
            notificationservice.Notification("Please Select Valid data", "", 'Only Single Row Left!!');

        }
        $scope.rowNumberGenerate();
        $scope.removeItem(entity);
    };

    // Grid one row remove if this mehtod is call
    $scope.removeItem = function (entity) {
        if ($scope.gridOptions.data.length > 1) {
            var index = $scope.gridOptions.data.indexOf(entity);
            if ($scope.gridOptions.data.length > 0) {
                $scope.gridOptions.data.splice(index, 1);
            }
            $scope.rowNumberGenerate();
        } else {
            notificationservice.Notification("Only Single Row Left!!", "", 'Only Single Row Left!!');
        }


    }

    $scope.gridOptions.columnDefs = [
        {
            name: 'ROW_NO', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , {
            name: 'MST_ID', field: 'MST_ID', visible: false
        }
        , {
            name: 'DTL_ID', field: 'DTL_ID', visible: false
        }
        , {
            name: 'SKU_ID', field: 'SKU_ID', displayName: 'SKU', enableFiltering: false, width: '35%', cellTemplate:
                '<select class="select2-single form-control" data-select2-id="{{row.entity.SKU_ID}}" id="SKU_ID" ng-disabled="row.entity.ROW_NO !=0"' +
                'name="SKU_ID" ng-model="row.entity.SKU_ID" style="width:100%" ng-change="grid.appScope.typeaheadSelectedSku(row.entity)">' +
                '<option ng-repeat="item in grid.appScope.Products" ng-selected="item.SKU_ID == row.entity.SKU_ID" value="{{item.SKU_ID}}">{{ item.SKU_NAME }} | Code: {{ item.SKU_CODE }}</option>' +
                '</select>'
        }
        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'Code', enableFiltering: false, width: '8%', cellTemplate:
                '<input type="text"  ng-model="row.entity.SKU_CODE" disabled="true"  class="pl-sm" />'
        }
        , {
            name: 'UNIT_ID', field: 'UNIT_ID', displayName: 'Units', enableFiltering: false, width: '45%', cellTemplate:
                '<div ng-show="row.entity.ROW_NO==0"><select class="form-control" id="select2-single1" ng-disabled="row.entity.ROW_NO !=0"' +
                'name="UNIT_ID" ng-model="row.entity.UNIT_ID" multiple style="width:100%" ng-change="grid.appScope.typeaheadSelectedSku(row.entity)">' +
                '<option ng-repeat="item in grid.appScope.Units" ng-selected="item.UNIT_ID == row.entity.UNIT_ID" value="{{item.UNIT_ID}}">{{ item.UNIT_NAME }} </option>' +
                '</select></div><div ng-show="row.entity.ROW_NO>0">{{row.entity.UNIT_NAME}}</div>'
        }

        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                //'<button style="margin-bottom: 5px;" ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.EditItem(row.entity)" type="button" class="btn btn-outline-success mb-1"><img src="/img/edit-icon.png" height="15px" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO == 0" ng-click="grid.appScope.addNewRow()" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                '</div>'
        },

    ];

    $scope.CompanyLoad = function () {
        InsertOrEditServices.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.CompanyNameLoad = function () {
        InsertOrEditServices.GetCompanyName().then(function (data) {
            $scope.model.COMPANY_NAME = data.data;
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }

    $scope.UnitsLoad = function () {
        $scope.showLoader = true;

        InsertOrEditServices.GetUnitList().then(function (data) {
            debugger
            $scope.Units = data.data.filter(x => x.COMPANY_ID == 1);


            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }
    $scope.UnitsLoad();

    $scope.typeaheadSelectedSku = function (entity) {
        const searchIndex = $scope.Products.findIndex((x) => x.SKU_ID == entity.SKU_ID);
        if (searchIndex != -1) {
            entity.SKU_CODE = $scope.Products[searchIndex].SKU_CODE;
        }

    };
    $scope.ProductsLoad = function () {
        $scope.showLoader = true;
        InsertOrEditServices.LoadProductData(0).then(function (data) {
            $scope.Products = data.data;
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }
    $scope.ClearForm = function () {
        window.location = "/SalesAndDistribution/DistributorProductRelation/InsertOrEdit";

    }
    $scope.GetCompanyDisableStatus = function (entity) {
        if ($scope.model.USER_TYPE == "SuperAdmin") {
            return false;
        }
        else {
            return true;
        }
    }
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'DistributorProductRelation',
            Action_Name: 'InsertOrEdit'
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
    $scope.GetPermissionData();
    $scope.ProductsLoad();


    // This Method work is Edit Data Loading
    $scope.GetEditDataById = function (value) {
        if (value != undefined && value.length > 0) {
            InsertOrEditServices.GetEditDataById(value).then(function (data) {
                if (data.data != null && data.data.distributor_Product_Dtls != null && data.data.distributor_Product_Dtls.length > 0) {
                    $scope.model = data.data;
                    $scope.CompanyLoad();
                    $scope.CompanyNameLoad();
                    $scope.gridOptions.data = data.data.distributor_Product_Dtls;
                   $scope.addNewRow();
                }
                $scope.rowNumberGenerate();
                $scope.showLoader = false;
            }, function (error) {
                alert(error);

            });
        }
    }
    $scope.SaveData = function (model) {
        $scope.showLoader = true;
        if ($scope.gridOptions.data.length > 1) {
            $scope.model.distributor_Product_Dtls = $scope.gridOptions.data.filter(function (el) {
                return el.SKU_CODE != '' && el.ROW_NO!=0;}
            );
            InsertOrEditServices.AddOrUpdate(model).then(function (data) {
                if (data.data == 1) {
                    debugger;
                    $scope.showLoader = false;
                    $scope.model.MST_ID = 0;
                    $scope.model.DISTRIBUTOR_PRODUCT_TYPE = '';
                    $scope.gridOptions.data = [];
                    $scope.gridOptions = {
                        data: [$scope.GridDefalutData()]
                    }
                    notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
                }
                else {
                    $scope.showLoader = false;
                }
            });
        }
        else {
            $scope.showLoader = false;
            notificationservice.Notification('Please Insert SKU and add to list!', 1, 'Data Save Successfully !!');

        }
    }

}]);

