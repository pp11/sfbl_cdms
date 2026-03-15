ngApp.controller('ngGridCtrl', ['$scope', 'CustomerInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, CustomerInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'
    $scope.model = {
        BIN_NO: ''
        , COMPANY_ID: 0
        , CONTACT_PERSON_NAME: ''
        , CONTACT_PERSON_NO: ''
        , CUSTOMER_ADDRESS: ''
        , CUSTOMER_ADDRESS_BANGLA: ''
        , CUSTOMER_CODE: ''
        , CUSTOMER_CONTACT: ''
        , CUSTOMER_EMAIL: ''
        , CUSTOMER_ID: 0
        , CUSTOMER_NAME: ''
        , CUSTOMER_NAME_BANGLA: ''
        , CUSTOMER_REMARKS: ''
        , CUSTOMER_STATUS: 'Active'
        , CUSTOMER_TYPE_ID: 0
        , DB_LOCATION_NAME: ''
        , DELIVERY_ADDRESS: ''
        , DELIVERY_ADDRESS_BANGLA: ''
        , PROPRIETOR_NAME: ''
        , SECURITY_MONEY: 0
        , TDS_FLAG: ''
        , TIN_NO: ''
        , TRADE_LICENSE_NO: ''
        , UNIT_ID: 0
        , VAT_REG_NO: ''
        ,SERIAL_NO: 0
        ,SUGGEST_PERCENT: 0
        ,MAXIMUM_PERCENT: 0
        ,MINIMUM_PERCENT: 0

    }

  

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.PriceType = [];
    $scope.Unit = [];
    $scope.CustomerType = [];
    $scope.Status = [];
    $scope.Routedata = [];
    $scope.DistributorProductRelations = [];
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Customer Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }

    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }

        , { name: 'CUSTOMER_ID', field: 'CUSTOMER_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }

        , {
            name: 'CUSTOMER_NAME', field: 'CUSTOMER_NAME', displayName: 'Name', enableFiltering: true, width: '15%'
        }
        , {
            name: 'CUSTOMER_CODE', field: 'CUSTOMER_CODE', displayName: 'Code', enableFiltering: true, width: '12%'
        }
        , {
            name: 'DIST_ROUTE_NAME', field: 'DIST_ROUTE_NAME', displayName: 'Route', enableFiltering: true, width: '15%'
        }
        , {
            name: 'SERIAL_NO', field: 'SERIAL_NO', displayName: 'Serial', enableFiltering: true, width: '10%'
        }
        , {
            name: 'CUSTOMER_REMARKS', field: 'CUSTOMER_REMARKS', displayName: 'Remark', enableFiltering: true, width: '15%'
        }
        , { name: 'CUSTOMER_STATUS', field: 'CUSTOMER_STATUS', displayName: 'Status', enableFiltering: true, width: '10%' }
        , {
            name: 'Action', displayName: 'Action', width: '18%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        }

    ];

    $scope.routeGridList = (gridregistrationservice.GridRegistration("Route Info"));
    $scope.routeGridList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.routeGridList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , {
            name: 'ROUTE_ID', field: 'ROUTE_ID', displayName: 'Route', enableFiltering: true,
            cellTemplate:
                '<select ng-disabled="row.entity.ROW_NO != 0" class="select2-single form-control" data-select2-id="{{row.entity.ROUTE_ID}}"' +
                'name="ROUTE_ID" ng-model="row.entity.ROUTE_ID" style="width:100%" ng-change="grid.appScope.OnRouteSelection(row.entity)">' +
                '<option ng-repeat="item in grid.appScope.Routedata" ng-selected="item.DIST_ROUTE_ID == row.entity.ROUTE_ID" value="{{item.DIST_ROUTE_ID}}">{{ item.DIST_ROUTE_NAME  }}</option>' +
                '</select>'
        }
        , {
            name: 'SL_NO', field: 'SL_NO', displayName: 'Serial', enableFiltering: true, width: '20%', cellTemplate:
                '<input type="number" style="text-align:right;"  ng-model="row.entity.SL_NO"  class="pl-sm" />'
        }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                //'<button style="margin-bottom: 5px;" ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.EditItem(row.entity)" type="button" class="btn btn-outline-success mb-1"><img src="/img/edit-icon.png" height="15px" ></button>' +

                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO == 0" ng-click="grid.appScope.addNewRow(row.entity)" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                '</div>'
        }
    ]
    $scope.defaultRoute = () => {
        return {
            ID: 0,
            ROW_NO: 0,
            ROUTE_ID: "0",
            SL_NO: 0,
            CUSTOMER_ID: 0
        }
    }

    $scope.routeGridList.data = [$scope.defaultRoute()];

    $scope.addNewRow = function (entity) {
        $scope.routeGridList.data.push(entity);
        $scope.routeGridList.data[0] = $scope.defaultRoute();
        $scope.rowNumberGenerate($scope.routeGridList.data);
    }

    $scope.removeItem = function (entity) {
        var index = $scope.routeGridList.data.indexOf(entity);
        $scope.routeGridList.data.splice(index, 1);
        $scope.rowNumberGenerate($scope.routeGridList.data);
    }

    $scope.rowNumberGenerate = function (data) {
        for (let i = 0; i < data.length; i++) {
            data[i].ROW_NO = i;
        }
    }

    $scope.DataLoad = function (companyId) {
        $scope.showLoader = true;

        CustomerInfoServices.LoadData(companyId).then(function (data) {
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = companyId;
            $interval(function () {
                $('.select2-single').trigger('change');
            }, 800, 4);
        }, function (error) {
            alert(error);

            $scope.showLoader = false;
        });
    }

    $scope.LoadStatusFilteredDataAll = function () {
        if ($scope.AllCustomer == true) {
            $scope.DataLoad($scope.model.COMPANY_ID);
            $scope.ActiveCustomer = false;
            $scope.InActiveCustomer = false;
        }
    }
    $scope.LoadStatusFilteredDataActive = function () {
        if ($scope.ActiveCustomer == true) {
            $scope.showLoader = true;

            CustomerInfoServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
                $scope.gridOptionsList.data = data.data.filter(function (element) { return element.CUSTOMER_STATUS == 'Active' });;
                $scope.AllCustomer = false;
                $scope.InActiveCustomer = false;
                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                $scope.showLoader = false;
            });
        }
    }

    $scope.LoadStatusFilteredDataInActive = function () {
        if ($scope.InActiveCustomer == true) {
            $scope.showLoader = true;
            CustomerInfoServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
                $scope.gridOptionsList.data = data.data.filter(function (element) { return element.CUSTOMER_STATUS == 'InActive' });;
                $scope.AllCustomer = false;
                $scope.ActiveCustomer = false;
                $scope.showLoader = false;
            }, function (error) {
                alert(error);

                $scope.showLoader = false;
            });
        }
    }

    $scope.LoadRouteData = function () {
        $scope.showLoader = true;

        CustomerInfoServices.LoadRouteData().then(function (data) {
            $scope.Routedata = data.data[0];
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;
        });
    }
    $scope.LoadDistributorProductRelationData = function () {
        $scope.showLoader = true;

        CustomerInfoServices.LoadDistributorProductRelationData($scope.model.COMPANY_ID).then(function (data) {
            $scope.DistributorProductRelations = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }
    $scope.LoadRouteData();

    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        CustomerInfoServices.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);
            for (let i = 0; i < $scope.Companies.length; i++) {
                if (parseInt($scope.Companies[i].COMPANY_ID) == $scope.model.COMPANY_ID) {
                    $scope.model.COMPANY_NAME = $scope.Companies[i].COMPANY_NAME;
                }
            }
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

    $scope.LoadPriceTypeData = function () {
        $scope.showLoader = true;

        CustomerInfoServices.LoadPriceTypeData($scope.model.COMPANY_ID).then(function (data) {
            $scope.PriceType = data.data;

            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    //$scope.LoadData = function () {
    //    $scope.showLoader = true;

    //    CustomerInfoServices.LoadPackSizeData($scope.model.COMPANY_ID).then(function (data) {
    //
    //        $scope.PackSize = data.data;
    //        $scope.showLoader = false;
    //    }, function (error) {
    //        $scope.showLoader = false;

    //    });
    //}

    $scope.LoadCustomerTypeData = function () {
        $scope.showLoader = true;

        CustomerInfoServices.LoadCustomerTypeData($scope.model.COMPANY_ID).then(function (data) {
            $scope.CustomerType = data.data;
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.LoadUnitData = function () {
        $scope.showLoader = true;

        CustomerInfoServices.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Unit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });;
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.LoadGeneratedCustomerCode = function () {
    }
    $scope.LoadStatus = function () {
        var Active = {
            STATUS: 'Active'
        }
        var InActive = {
            STATUS: 'InActive'
        }
        var Hold = {
            STATUS: 'Hold'
        }
        $scope.Status.push(Active);
        $scope.Status.push(InActive);
        $scope.Status.push(Hold);
    }
    $scope.ClearForm = function () {
        window.location.href = "/SalesAndDistribution/Customer/CustomerInfo";
    }

    $scope.EditData = function (entity) {
        $scope.model.BIN_NO = entity.BIN_NO == null || entity.BIN_NO == undefined ? '' : entity.BIN_NO;
        $scope.model.CONTACT_PERSON_NAME = entity.CONTACT_PERSON_NAME == null || entity.CONTACT_PERSON_NAME == undefined ? '' : entity.CONTACT_PERSON_NAME;
        $scope.model.CONTACT_PERSON_NO = entity.CONTACT_PERSON_NO == null || entity.CONTACT_PERSON_NO == undefined ? '' : entity.CONTACT_PERSON_NO;
        $scope.model.CUSTOMER_ADDRESS = entity.CUSTOMER_ADDRESS == null || entity.CUSTOMER_ADDRESS == undefined ? '' : entity.CUSTOMER_ADDRESS;
        $scope.model.CUSTOMER_ADDRESS_BANGLA = entity.CUSTOMER_ADDRESS_BANGLA == null || entity.CUSTOMER_ADDRESS_BANGLA == undefined ? '' : entity.CUSTOMER_ADDRESS_BANGLA;
        $scope.model.CUSTOMER_CODE = entity.CUSTOMER_CODE == null || entity.CUSTOMER_CODE == undefined ? '' : entity.CUSTOMER_CODE;
        $scope.model.CUSTOMER_CONTACT = entity.CUSTOMER_CONTACT == null || entity.CUSTOMER_CONTACT == undefined ? '' : entity.CUSTOMER_CONTACT;
        $scope.model.CUSTOMER_EMAIL = entity.CUSTOMER_EMAIL == null || entity.CUSTOMER_EMAIL == undefined ? '' : entity.CUSTOMER_EMAIL;
        $scope.model.CUSTOMER_ID = entity.CUSTOMER_ID == null || entity.CUSTOMER_ID == undefined || entity.CUSTOMER_ID == NaN ? 0 : entity.CUSTOMER_ID;
        $scope.model.CUSTOMER_NAME = entity.CUSTOMER_NAME == null || entity.CUSTOMER_NAME == undefined ? '' : entity.CUSTOMER_NAME;
        $scope.model.CUSTOMER_NAME_BANGLA = entity.CUSTOMER_NAME_BANGLA == null || entity.CUSTOMER_NAME_BANGLA == undefined ? '' : entity.CUSTOMER_NAME_BANGLA;
        $scope.model.CUSTOMER_REMARKS = entity.CUSTOMER_REMARKS == null || entity.CUSTOMER_REMARKS == undefined ? '' : entity.CUSTOMER_REMARKS;
        $scope.model.CUSTOMER_STATUS = entity.CUSTOMER_STATUS == null || entity.CUSTOMER_STATUS == undefined ? '' : entity.CUSTOMER_STATUS;
        $scope.model.CUSTOMER_TYPE_ID = entity.CUSTOMER_TYPE_ID == null || entity.CUSTOMER_TYPE_ID == undefined || entity.CUSTOMER_TYPE_ID == NaN ? 0 : entity.CUSTOMER_TYPE_ID;
        $scope.model.DB_LOCATION_NAME = entity.DB_LOCATION_NAME == null || entity.DB_LOCATION_NAME == undefined ? '' : entity.DB_LOCATION_NAME;
        $scope.model.DELIVERY_ADDRESS = entity.DELIVERY_ADDRESS == null || entity.DELIVERY_ADDRESS == undefined ? '' : entity.DELIVERY_ADDRESS;
        $scope.model.DELIVERY_ADDRESS_BANGLA = entity.DELIVERY_ADDRESS_BANGLA == null || entity.DELIVERY_ADDRESS_BANGLA == undefined ? '' : entity.DELIVERY_ADDRESS_BANGLA;
        $scope.model.PRICE_TYPE_ID = entity.PRICE_TYPE_ID == null || entity.PRICE_TYPE_ID == undefined || entity.PRICE_TYPE_ID == NaN ? 0 : entity.PRICE_TYPE_ID;
        $scope.model.PROPRIETOR_NAME = entity.PROPRIETOR_NAME == null || entity.PROPRIETOR_NAME == undefined ? '' : entity.PROPRIETOR_NAME;
        $scope.model.SECURITY_MONEY = entity.SECURITY_MONEY == null || entity.SECURITY_MONEY == undefined || entity.SECURITY_MONEY == NaN ? 0 : entity.SECURITY_MONEY;
        $scope.model.TDS_FLAG = entity.TDS_FLAG == null || entity.TDS_FLAG == undefined ? '' : entity.TDS_FLAG;
        $scope.model.TIN_NO = entity.TIN_NO == null || entity.TIN_NO == undefined ? '' : entity.TIN_NO;
        $scope.model.TRADE_LICENSE_NO = entity.TRADE_LICENSE_NO == null || entity.TRADE_LICENSE_NO == undefined ? '' : entity.TRADE_LICENSE_NO;
        $scope.model.UNIT_ID = entity.UNIT_ID == null || entity.UNIT_ID == undefined || entity.UNIT_ID == NaN ? 0 : entity.UNIT_ID;
        $scope.model.VAT_REG_NO = entity.VAT_REG_NO == null || entity.VAT_REG_NO == undefined ? '' : entity.VAT_REG_NO;
        $scope.model.DISTRIBUTOR_PRODUCT_TYPE = entity.DISTRIBUTOR_PRODUCT_TYPE == null || entity.DISTRIBUTOR_PRODUCT_TYPE == undefined ? '' : entity.DISTRIBUTOR_PRODUCT_TYPE;
        //$scope.model.ROUTE_ID = entity.ROUTE_ID == null || entity.ROUTE_ID == undefined || entity.ROUTE_ID == NaN ? 0 : entity.ROUTE_ID;
        //$scope.model.SERIAL_NO = entity.SERIAL_NO == null || entity.SERIAL_NO == undefined || entity.SERIAL_NO == NaN ? 0 : entity.SERIAL_NO;
        $scope.model.SUGGEST_PERCENT = entity.SUGGEST_PERCENT == null || entity.SUGGEST_PERCENT == undefined ? 0 : entity.SUGGEST_PERCENT;
        $scope.model.MAXIMUM_PERCENT = entity.MAXIMUM_PERCENT == null || entity.MAXIMUM_PERCENT == undefined ? 0 : entity.MAXIMUM_PERCENT;
        $scope.model.MINIMUM_PERCENT = entity.MINIMUM_PERCENT == null || entity.MINIMUM_PERCENT == undefined ? 0 : entity.MINIMUM_PERCENT;


        CustomerInfoServices.GetCustomerRoute($scope.model.CUSTOMER_ID)
            .then(response => {
                var sl = 0;
                response.data.forEach(e => {
                    e.ROW_NO = ++sl;
                    e.ROUTE_ID = e.ROUTE_ID.toString()
                });
                $scope.routeGridList.data = [$scope.defaultRoute(), ...response.data];
            })

        $interval(function () {
            $scope.LoadCUSTOMER_TYPE_ID();
        }, 800, 2);

        $interval(function () {
            $scope.LoadUNIT_ID();
        }, 800, 2);

        $interval(function () {
            $scope.LoadCOMPANY_ID();
        }, 800, 2);
        $interval(function () {
            $scope.LoadPRICE_TYPE_ID();
        }, 800, 2);
        $interval(function () {
            $scope.LoadCUSTOMER_STATUS();
        }, 800, 2);
        $interval(function () {
            $scope.LoadTDS_FLAG();
        }, 800, 2);
    }
    $scope.LoadPRICE_TYPE_ID = function () {
        $('#PRICE_TYPE_ID').trigger('change');
    }

    $scope.LoadCUSTOMER_TYPE_ID = function () {
        $('#CUSTOMER_TYPE_ID').trigger('change');
    }

    $scope.LoadUNIT_ID = function () {
        $('#UNIT_ID').trigger('change');
    }

    $scope.LoadCOMPANY_ID = function () {
        $('#COMPANY_ID').trigger('change');
    }
    $scope.LoadTDS_FLAG = function () {
        $('#TDS_FLAG').trigger('change');
    }
    $scope.LoadCUSTOMER_STATUS = function () {
        $('#CUSTOMER_STATUS').trigger('change');
    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'Customer',
            Action_Name: 'CustomerInfo'
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
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
    $scope.LoadStatus();
    $scope.LoadPriceTypeData();
    $scope.LoadCustomerTypeData();
    $scope.LoadUnitData();
    $scope.DataLoad($scope.model.COMPANY_ID);
    $scope.LoadDistributorProductRelationData();

    var inputs = document.querySelectorAll(".form-control");
    inputs.forEach(input => {
        input.addEventListener('focusin', () => {
            input.style.border = '1px solid green';
        }, false);
    })

    $scope.stopPropagation = function () {
        if ($scope.model.isChecked) {
            $scope.model.DELIVERY_ADDRESS = "";
        } else {
            $scope.model.DELIVERY_ADDRESS = $scope.model.CUSTOMER_ADDRESS;
        }
    }
    
    $scope.AddOrUpdate_Dist_Prod_Type = function (model) {
        
        $scope.showLoader = true;
        model.PRICE_TYPE_ID = parseInt(model.PRICE_TYPE_ID);
        model.SECURITY_MONEY = model.SECURITY_MONEY == null || model.SECURITY_MONEY == NaN || model.SECURITY_MONEY == undefined ? 0 : parseFloat(model.SECURITY_MONEY);
        model.COMPANY_ID = parseInt(model.COMPANY_ID);
        model.CUSTOMER_TYPE_ID = parseInt(model.CUSTOMER_TYPE_ID);
        model.CUSTOMER_ID = parseInt(model.CUSTOMER_ID);
        model.UNIT_ID = parseInt(model.UNIT_ID);
        model.ROUTE_ID = parseInt(model.ROUTE_ID);
        var dpt = '';
        for (var i = 0; i < model.DISTRIBUTOR_PRODUCT_TYPE.length; i++) {
            if (i == 0) {
                dpt = model.DISTRIBUTOR_PRODUCT_TYPE[i];
            } else {
                dpt = dpt +','+ model.DISTRIBUTOR_PRODUCT_TYPE[i];

            }
        }
        model.DISTRIBUTOR_PRODUCT_TYPE = dpt;
        debugger
        CustomerInfoServices.AddOrUpdate_Dist_Prod_Type(model).then(function (data) {
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
    $scope.SaveData = function (model) {
        if ($scope.formMenuCategoryAdd.$invalid) {
            let err = $scope.formMenuCategoryAdd.$error.required;
            for (var i = 0; i < err.length; i++) {
                let input = document.getElementById(err[i].$$attr.id);
                input.style.border = '3px solid red';
            }
            alert("Please Select " + err[0].$$attr.id);
            return;
        }
        var dtl = $scope.routeGridList.data.filter(e => e.ROW_NO != 0);
        //.map(e => { e.ROUTE_ID = parseInt(e.ROUTE_ID); return e; })
        if (dtl.length > 0) {
            for (let i = 0; i < dtl.length; i++) {
                if (dtl[i].SL_NO == 0) {
                    alert('Please Enter Route Serial Properly');
                    return;
                }
            }
        } else {
            alert('Please Select Route')
            return;
        }

        model.Customer_Route_Relations = dtl;

        $scope.showLoader = true;
        model.PRICE_TYPE_ID = parseInt(model.PRICE_TYPE_ID);
        model.SECURITY_MONEY = model.SECURITY_MONEY == null || model.SECURITY_MONEY == NaN || model.SECURITY_MONEY == undefined ? 0 : parseFloat(model.SECURITY_MONEY);
        model.COMPANY_ID = parseInt(model.COMPANY_ID);
        model.CUSTOMER_TYPE_ID = parseInt(model.CUSTOMER_TYPE_ID);
        model.CUSTOMER_ID = parseInt(model.CUSTOMER_ID);
        model.UNIT_ID = parseInt(model.UNIT_ID);
        model.ROUTE_ID = parseInt(model.ROUTE_ID);



        CustomerInfoServices.AddOrUpdate(model).then(function (data) {
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
}]);