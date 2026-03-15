ngApp.controller('ngGridCtrl', ['$scope', 'SkuCommissionService', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, SkuCommissionService, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { SKU_ID: 0, UNIT_ID: 0, COMPANY_ID: 0 }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];
    $scope.Units = [];
    $scope.products = [];
    $scope.Market_Customer_List = [];
    //------Customer hierarchy
    $scope.Divisions = [];
    $scope.Regions = [];
    $scope.Areas = [];
    $scope.Territories = [];
    $scope.Markets = [];
    $scope.ComissionDoneCustomers = [];
    $scope.GetDivitionToMarketRelations = [];
    $scope.RemovedDist = [];
    $scope.CustomerTypes = [];

    //---------------------------------------------
    //#region Customer Info Report
    $scope.GetDivitionToMarketRelation = function () {
        $scope.showLoader = true;
        SkuCommissionService.GetDivitionToMarketRelation($scope.model.DIVISION_ID).then(function (data) {
            $scope.GetDivitionToMarketRelations = data.data;
            $scope.Divisions = [];
            $scope.Regions = [];
            $scope.Areas = [];
            $scope.Territories = [];
            $scope.Markets = [];

            const map = new Map();
            const mapREGION_ID = new Map();
            const mapAREA_ID = new Map();
            const mapTERRITORY_ID = new Map();
            const mapMARKET_ID = new Map();
            for (const item of data.data) {
                if (!map.has(item.DIVISION_ID)) {
                    map.set(item.DIVISION_ID, true);    // set any value to Map
                    $scope.Divisions.push({
                        DIVISION_ID: item.DIVISION_ID,
                        DIVISION_NAME: item.DIVISION_NAME
                    });
                }
                if (!mapREGION_ID.has(item.REGION_ID)) {
                    mapREGION_ID.set(item.REGION_ID, true);    // set any value to Map
                    $scope.Regions.push({
                        REGION_ID: item.REGION_ID,
                        REGION_NAME: item.REGION_NAME
                    });
                }
                if (!mapAREA_ID.has(item.AREA_ID)) {
                    mapAREA_ID.set(item.AREA_ID, true);    // set any value to Map
                    $scope.Areas.push({
                        AREA_ID: item.AREA_ID,
                        AREA_NAME: item.AREA_NAME
                    });
                }
                if (!mapTERRITORY_ID.has(item.TERRITORY_ID)) {
                    mapTERRITORY_ID.set(item.TERRITORY_ID, true);    // set any value to Map
                    $scope.Territories.push({
                        TERRITORY_ID: item.TERRITORY_ID,
                        TERRITORY_NAME: item.TERRITORY_NAME
                    });
                }
                if (!mapMARKET_ID.has(item.MARKET_ID)) {
                    mapMARKET_ID.set(item.MARKET_ID, true);    // set any value to Map
                    $scope.Markets.push({
                        MARKET_ID: item.MARKET_ID,
                        MARKET_NAME: item.MARKET_NAME
                    });
                }

            }

            $scope.Divisions.unshift({ DIVISION_ID: '', DIVISION_NAME: 'ALL' })
            $scope.Regions.unshift({ REGION_ID: '', REGION_NAME: 'ALL' })
            $scope.Areas.unshift({ AREA_ID: '', AREA_NAME: 'ALL' })
            $scope.Territories.unshift({ TERRITORY_ID: '', TERRITORY_NAME: 'ALL' })
            $scope.Markets.unshift({ MARKET_ID: '', MARKET_NAME: 'ALL' })


        }, function (error) {

            console.log(error);
            $scope.showLoader = false;

        });
    }
    $scope.DivisionChange = function () {
        $scope.showLoader = true;
        let GetDivitionToMarketRelations = [];
        $scope.Regions = [];
        $scope.Areas = [];
        $scope.Territories = [];
        $scope.Markets = [];
        if ($scope.model.DIVISION_ID != '') {
            let DIVISION_ID = $scope.model.DIVISION_ID;
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations.filter((x) => { return x.DIVISION_ID == DIVISION_ID; });
        } else {

            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations;
        }
        const mapREGION_ID = new Map();
        const mapAREA_ID = new Map();
        const mapTERRITORY_ID = new Map();
        const mapMARKET_ID = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!mapREGION_ID.has(item.REGION_ID)) {
                mapREGION_ID.set(item.REGION_ID, true);    // set any value to Map
                $scope.Regions.push({
                    REGION_ID: item.REGION_ID,
                    REGION_NAME: item.REGION_NAME
                });
            }
            if (!mapAREA_ID.has(item.AREA_ID)) {
                mapAREA_ID.set(item.AREA_ID, true);    // set any value to Map
                $scope.Areas.push({
                    AREA_ID: item.AREA_ID,
                    AREA_NAME: item.AREA_NAME
                });
            }
            if (!mapTERRITORY_ID.has(item.TERRITORY_ID)) {
                mapTERRITORY_ID.set(item.TERRITORY_ID, true);    // set any value to Map
                $scope.Territories.push({
                    TERRITORY_ID: item.TERRITORY_ID,
                    TERRITORY_NAME: item.TERRITORY_NAME
                });
            }
            if (!mapMARKET_ID.has(item.MARKET_ID)) {
                mapMARKET_ID.set(item.MARKET_ID, true);    // set any value to Map
                $scope.Markets.push({
                    MARKET_ID: item.MARKET_ID,
                    MARKET_NAME: item.MARKET_NAME
                });
            }
        }

        $scope.Regions.unshift({ REGION_ID: '', REGION_NAME: 'ALL' })
        $scope.Areas.unshift({ AREA_ID: '', AREA_NAME: 'ALL' })
        $scope.Territories.unshift({ TERRITORY_ID: '', TERRITORY_NAME: 'ALL' })
        $scope.Markets.unshift({ MARKET_ID: '', MARKET_NAME: 'ALL' })
        $scope.model.REGION_ID = '';
        $scope.model.AREA_ID = '';
        $scope.model.TERRITORY_ID = '';
        $scope.model.MARKET_ID = '';
        $scope.showLoader = false;

    }
    $scope.RegionChange = function () {
        $scope.showLoader = true;
        let GetDivitionToMarketRelations = [];
        $scope.Areas = [];
        $scope.Territories = [];
        $scope.Markets = [];
        if ($scope.model.REGION_ID != '') {
            let REGION_ID = $scope.model.REGION_ID;
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations.filter((x) => { return x.REGION_ID == REGION_ID; });
        } else {
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations;
        }
        var map = new Map();

        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.AREA_ID)) {
                map.set(item.AREA_ID, true);    // set any value to Map
                $scope.Areas.push({
                    AREA_ID: item.AREA_ID,
                    AREA_NAME: item.AREA_NAME
                });
            }
        }
        map = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.TERRITORY_ID)) {
                map.set(item.TERRITORY_ID, true);    // set any value to Map
                $scope.Territories.push({
                    TERRITORY_ID: item.TERRITORY_ID,
                    TERRITORY_NAME: item.TERRITORY_NAME
                });
            }
        }
        map = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.MARKET_ID)) {
                map.set(item.MARKET_ID, true);    // set any value to Map
                $scope.Markets.push({
                    MARKET_ID: item.MARKET_ID,
                    MARKET_NAME: item.MARKET_NAME
                });
            }
        }
        $scope.Areas.unshift({ AREA_ID: '', AREA_NAME: 'ALL' })
        $scope.Territories.unshift({ TERRITORY_ID: '', TERRITORY_NAME: 'ALL' })
        $scope.Markets.unshift({ MARKET_ID: '', MARKET_NAME: 'ALL' })
        $scope.model.AREA_ID = '';
        $scope.model.TERRITORY_ID = '';
        $scope.model.MARKET_ID = '';
        $scope.showLoader = false;

    }
    $scope.AreaChange = function () {
        $scope.showLoader = true;
        let GetDivitionToMarketRelations = [];
        $scope.Territories = [];
        $scope.Markets = [];
        if ($scope.model.AREA_ID != '') {
            let AREA_ID = $scope.model.AREA_ID;
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations.filter((x) => { return x.AREA_ID == AREA_ID; });
        } else {
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations;
        }
        var map = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.TERRITORY_ID)) {
                map.set(item.TERRITORY_ID, true);    // set any value to Map
                $scope.Territories.push({
                    TERRITORY_ID: item.TERRITORY_ID,
                    TERRITORY_NAME: item.TERRITORY_NAME
                });
            }
        }
        map = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.MARKET_ID)) {
                map.set(item.MARKET_ID, true);    // set any value to Map
                $scope.Markets.push({
                    MARKET_ID: item.MARKET_ID,
                    MARKET_NAME: item.MARKET_NAME
                });
            }
        }

        $scope.Territories.unshift({ TERRITORY_ID: '', TERRITORY_NAME: 'ALL' })
        $scope.Markets.unshift({ MARKET_ID: '', MARKET_NAME: 'ALL' })
        $scope.model.TERRITORY_ID = '';
        $scope.model.MARKET_ID = '';
        $scope.showLoader = false;

    }
    $scope.TerritoryChange = function () {
        $scope.showLoader = true;
        let GetDivitionToMarketRelations = [];

        $scope.Markets = [];
        if ($scope.model.TERRITORY_ID != '') {
            let TERRITORY_ID = $scope.model.TERRITORY_ID;
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations.filter((x) => { return x.TERRITORY_ID == TERRITORY_ID; });
        } else {
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations;
        }
        var map = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.MARKET_ID)) {
                map.set(item.MARKET_ID, true);    // set any value to Map
                $scope.Markets.push({
                    MARKET_ID: item.MARKET_ID,
                    MARKET_NAME: item.MARKET_NAME
                });
            }
        }

        $scope.Markets.unshift({ MARKET_ID: '', MARKET_NAME: 'ALL' })
        $scope.model.MARKET_ID = '';
        $scope.showLoader = false;

    }
    //#endregion


    $scope.LoadProducts = function () {
        $scope.showLoader = true;
        SkuCommissionService.GetProducts().then(function (data) {
            $scope.products = data.data;
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;
        })
    }
    $scope.LoadProducts();
    $scope.editMode = false;
    $scope.LoadCustomerTypeData = function () {
        $scope.showLoader = true;
        SkuCommissionService.LoadCustomerTypeData().then(function (data) {
            $scope.CustomerTypes = data.data;
            $scope.CustomerTypes.unshift({ CUSTOMER_TYPE_ID: '', CUSTOMER_TYPE_NAME: 'ALL' })

            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;
        })
    }
    $scope.LoadCustomerTypeData();
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Commission Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'CUSTOMER_CODE', field: 'CUSTOMER_CODE', displayName: 'Customer Code' }
        , { name: 'CUSTOMER_NAME', field: 'CUSTOMER_NAME', displayName: 'Customer Name' }
        , {
            name: 'PRICE_FLAG', field: 'PRICE_FLAG', displayName: 'Price Flag', enableFiltering: true, width: '10%'
            //, headerCellTemplate: '<div class="">Price<br> Flag<br></div><select id="PRICE_FLAG" ng-change="grid.appScope.SetAlPriceFlag()" class="form-control" ng-model="PRICE_FLAG" ><option  value="Yes">Yes</option><option value="No">No</option></select> '
            , cellTemplate:
                ` <select class="form-control"  ng-model="row.entity.PRICE_FLAG" ng-disabled="grid.appScope.model.IS_PROCESSED == \'Yes\'">
                                        <option  value="Yes">Yes</option>
                                        <option  value="No">No</option>
                                    </select>`
        },
        , {
            name: 'COMMISSION_FLAG', field: 'COMMISSION_FLAG', displayName: 'Commission Flag', enableFiltering: true, width: '10%'
            //, headerCellTemplate: '<div class="">Commission Flag<br></div><select id="COMMISSION_FLAG" ng-change="grid.appScope.SetAlCommissionFlag()" class="form-control" ng-model="COMMISSION_FLAG" ><option  value="Yes">Yes</option><option value="No">No</option></select> '
            , cellTemplate:
                ` <select class="form-control" ng-model="row.entity.COMMISSION_FLAG" ng-disabled="grid.appScope.model.IS_PROCESSED == \'Yes\'">
                                        <option  value="Yes">Yes</option>
                                        <option  value="No">No</option>
                                    </select>`
        }
        , {
            name: 'COMMISSION_TYPE', field: 'COMMISSION_TYPE', displayName: 'Commission Type', enableFiltering: true, width: '10%'
            //, headerCellTemplate: '<div class="">Commission Type<br></div><select id="COMMISSION_TYPE" ng-change="grid.appScope.SetAlCommissionType()" class="form-control" ng-model="COMMISSION_TYPE" ><option  value="PCT">PCT</option><option value="Value">Value</option></select> '
            , cellTemplate:
                ` <select class="form-control" ng-model="row.entity.COMMISSION_TYPE" ng-disabled="grid.appScope.model.IS_PROCESSED == \'Yes\'">
                                        <option  value="PCT">PCT</option>
                                        <option  value="Value">Value</option>
                                    </select>`
        }
        , {
            name: 'COMMISSION_VALUE', field: 'COMMISSION_VALUE', displayName: 'Commission Value'
            , headerCellTemplate: '<div class="">Commission Value<br></div><input id="MODEL_COL_FIELD3" ng-model="MODEL_COL_FIELD3" ng-change="grid.appScope.SetAllAddComm3()"  type="text" class="form-control" >'

            , cellTemplate:
                '<input ng-model="row.entity.COMMISSION_VALUE" min=0 class= "pl-sm" ng-disabled="grid.appScope.model.IS_PROCESSED == \'Yes\'" /> '
        }, {
            name: 'ADD_COMMISSION1', field: 'ADD_COMMISSION1', displayName: 'Additional Commission',
            headerCellTemplate: '<div class="">Additional Commission<br></div><input id="MODEL_COL_FIELD" ng-model="MODEL_COL_FIELD" ng-change="grid.appScope.SetAllAddComm()"  type="text" class="form-control" >'

            , cellTemplate:
                '<input ng-model="row.entity.ADD_COMMISSION1" min=0 class= "pl-sm" ng-disabled="grid.appScope.model.IS_PROCESSED == \'Yes\'" /> '
        }, {
            name: 'ADD_COMMISSION2', field: 'ADD_COMMISSION2', displayName: 'Additional Commission 2',
            headerCellTemplate: '<div class="">Additional Commission2<br></div><input id="MODEL_COL_FIELD2" ng-model="MODEL_COL_FIELD2" ng-change="grid.appScope.SetAllAddComm2()"  type="text" class="form-control" >', enableFiltering: true

            ,cellTemplate:
                '<input ng-model="row.entity.ADD_COMMISSION2" min=0 class= "pl-sm" ng-disabled="grid.appScope.model.IS_PROCESSED == \'Yes\'" /> '
        }
       ,{
           name: 'Action', displayName: 'Action', enableFiltering: false, enableColumnMenu: false, width: '145', cellTemplate:
                '<div style="margin:1px;">' +
               '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.DELETE_PERMISSION == \'Active\'" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-primary mb-1">Remove</button>' + 
               '</div>'
        }
    ];

    $scope.UpdateData = function (entity) {
        $scope.showLoader = true;
        entity.UNIT_ID = entity.UNIT_ID.toString();
        entity.SKU_ID = entity.SKU_ID.toString();
        entity.COMMISSION_VALUE = entity.COMMISSION_VALUE?.toString();
        entity.ADD_COMMISSION1 = entity.ADD_COMMISSION1?.toString();
        entity.ADD_COMMISSION2 = entity.ADD_COMMISSION2?.toString();
        SkuCommissionService.Update(entity).then(response => {
            $scope.showLoader = false;
            notificationservice.Notification(response.data, 1, 'Data Update Successfully !!');
        })
    }
    $scope.SetAllAddComm = function () {
        let value = document.getElementById("MODEL_COL_FIELD").value;
        for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
            $scope.gridOptionsList.data[i].ADD_COMMISSION1 = parseFloat(value);
        }

    }
    $scope.SetAllAddComm2 = function () {
        let value = document.getElementById("MODEL_COL_FIELD2").value;
        for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
            $scope.gridOptionsList.data[i].ADD_COMMISSION2 = parseFloat(value);
        }

    }
    $scope.SetAllAddComm3 = function () {
        let value = document.getElementById("MODEL_COL_FIELD3").value;
        for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
            $scope.gridOptionsList.data[i].COMMISSION_VALUE = parseFloat(value);
        }

    }
    $scope.removeItem = function (entity) {

        if ($scope.gridOptionsList.data.length > 1) {
            var index = $scope.gridOptionsList.data.indexOf(entity);
            if ($scope.gridOptionsList.data.length > 0) {
                $scope.gridOptionsList.data.splice(index, 1);
                var ind = $scope.RemovedDist.findIndex(x => x.CUSTOMER_ID == entity.CUSTOMER_ID);
                if (ind == -1) {
                    $scope.RemovedDist.push(entity);
                }
            }
            $scope.rowNumberGenerate();


        } else {
            notificationservice.Notification("Only Single Row Left!!", "", 'Only Single Row Left!!');
        }


    }
    $scope.AddItem = function () {
        debugger
        var ind = $scope.RemovedDist.findIndex(x => x.CUSTOMER_ID == $scope.model.VCUSTOMER_ID);
        if (ind > -1) {
            $scope.gridOptionsList.data.push($scope.RemovedDist[ind]);
            $scope.RemovedDist.splice(ind, 1);
        } 
        $scope.rowNumberGenerate();
    }
    $scope.DeleteDtlData = function (entity) {
        if (window.confirm("Are you sure?")) {
            SkuCommissionService.DeleteDtl(entity.DTL_ID).then(response => {
                if (response.data == "1") {
                    notificationservice.Notification(response.data, 1, 'Data Delete Successfully !!');
                    $scope.gridOptionsList.data.indexOf(e => e.DTL_ID == entity.DTL_ID);
                    debugger;
                    $scope.gridOptionsList.data.splice($scope.gridOptionsList.data.findIndex(e => e.DTL_ID == entity.DTL_ID), 1);
                }
                else {
                    notificationservice.Notification(response.data, 1, response.data);
                }
            })
        }
    }

    $scope.gridOptionsSkuPrice = (gridregistrationservice.GridRegistration("SkuPrice"));
    $scope.gridOptionsSkuPrice.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsSkuPrice.columnDefs = [
        { name: '#', field: 'ROW_NO', enableFiltering: false, width: '50' },
        { name: 'UNIT_NAME', field: 'UNIT_NAME', displayName: 'Unit', width: '60' },
        { name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code',width: '100' },
        { name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name' },
        { name: 'ENTERED_DATE', field: 'ENTERED_DATE', width: '150', displayName: 'Entered Date' },
        { name: 'ENTERED_BY', field: 'ENTERED_BY', width: '100', displayName: 'Entered By' },
        { name: 'IS_PROCESSED', field: 'IS_PROCESSED', width: '100', displayName: 'Is Processed?' },
        { name: 'TOTAL_CUST', field: 'TOTAL_CUST', width: '120', displayName: 'Total Customer' },
        {
            name: 'Action', displayName: 'Action', enableFiltering: false, enableColumnMenu: false, width:'180', cellTemplate: 
                '<div style="margin:1px;">' +
            '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">View</button>' +
                    '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.DELETE_PERMISSION == \'Active\'" ng-disabled="row.entity.IS_PROCESSED == \'Yes\'" ng-click="grid.appScope.DeleteMstData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Delete</button>' +
            '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-disabled="row.entity.IS_PROCESSED != \'Yes\'" ng-click="grid.appScope.LikeData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Like Entry</button>' +

             '</div>'
        }
    ];

    $scope.EditData = function (entity) {
        debugger
        $scope.editMode = true;
        $scope.model.MST_ID = entity.MST_ID;
        $scope.model.UNIT_ID = entity.UNIT_ID.toString();
        $scope.model.SKU_ID = entity.SKU_ID.toString();
        $scope.model.IS_PROCESSED = entity.IS_PROCESSED;

        SkuCommissionService.GetDetails(entity.MST_ID).then(response => {
            debugger
            $scope.gridOptionsList.data = response.data;
            $scope.model.SKU_PRICE = response.data[0]?.SKU_PRICE;
        })
        $('#exampleModalScrollable').modal('hide');
    }
    $scope.LikeData = function (entity) {
        debugger
        $scope.model.MST_ID = 0;
        $scope.model.UNIT_ID = entity.UNIT_ID.toString();
        $scope.model.SKU_ID = entity.SKU_ID.toString();
        $scope.model.IS_PROCESSED = "No";

        SkuCommissionService.GetDetails(entity.MST_ID).then(response => {
            debugger
            $scope.gridOptionsList.data = response.data;
            $scope.model.SKU_PRICE = response.data[0]?.SKU_PRICE;
            for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
                $scope.gridOptionsList.data[i].DTL_ID = 0;
                $scope.gridOptionsList.data[i].MST_ID = 0;

            }

        })
        $('#exampleModalScrollable').modal('hide');
    }

    $scope.DeleteMstData = function (entity) {
        if (window.confirm("Are you sure?")) {
            SkuCommissionService.DeleteMst(entity.MST_ID).then(response => {
                if (response.data == "1") {
                    notificationservice.Notification(response.data, 1, 'Data Delete Successfully !!');
                    $scope.LoadData();
                }
                else {
                    notificationservice.Notification(response.data, 1, response.data);
                }
            })
        }
    }

    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        SkuCommissionService.GetCompany().then(function (data) {

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

        SkuCommissionService.GetCompanyList().then(function (data) {

            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }

    $scope.onChangeProduct = function () {
        var product = $scope.products.find(e => e.SKU_ID == $scope.model.SKU_ID);
        $scope.SKU_ID_Back = product?.SKU_ID;
        $scope.model.SKU_PRICE = product?.UNIT_TP;
        $scope.Market_param = "";
        if ($scope.Markets.length > 0) {
            for (var i = 0; i < $scope.Markets.length; i++) {
                if ($scope.Market_param.length == 0) {
                    $scope.Market_param = $scope.Markets[i].MARKET_ID.toString();
                } else {
                    $scope.Market_param = $scope.Market_param + "," + $scope.Markets[i].MARKET_ID.toString();

                }
            }
        } else {
            $scope.Market_param = $scope.model.MARKET_ID;
        }
        $scope.model.Market_Code = $scope.Market_param;

        $scope.gridOptionsList.data = [];
        SkuCommissionService.GetCustomer($scope.model).then(response => {
            if ($scope.gridOptionsList.data.length == 0) {
                $scope.gridOptionsList.data = response.data;
                for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
                    $scope.gridOptionsList.data[i].ROW_NO = i;
                }
            }
            // else {
            //    $scope.Cust_Market_Backup = response.data;

            //    for (var i = 0; i < $scope.Cust_Market_Backup.length; i++) {
            //        var ind = $scope.gridOptionsList.data.findIndex(x => x.CUSTOMER_ID == $scope.Cust_Market_Backup[i].CUSTOMER_ID)
            //        if (ind == 0) {
            //            $scope.gridOptionsList.data.push($scope.Cust_Market_Backup[i]);
            //        }
            //    }
            //}
        //    $scope.LoadCustomer();
        })
    }
    $scope.LoadCustomer = function () {
        $scope.Market_param = "";
        if ($scope.Markets.length > 0) {
            for (var i = 0; i < $scope.Markets.length; i++) {
                if ($scope.Market_param.length == 0) {
                    $scope.Market_param = $scope.Markets[i].MARKET_ID.toString();
                } else {
                    $scope.Market_param = $scope.Market_param + "," + $scope.Markets[i].MARKET_ID.toString();

                }
            }
        } else {
            $scope.Market_param = $scope.model.MARKET_ID;
        }
       
        SkuCommissionService.GetMarketWiseCustomers($scope.Market_param).then(response => {
            $scope.Market_Customer_List = response.data;
            $scope.Cust_Market_Backup = $scope.gridOptionsList.data;
            $scope.gridOptionsList.data = [];

            for (var i = 0; i < $scope.Cust_Market_Backup.length; i++) {
                var ind = $scope.Market_Customer_List.findIndex(x => x.CUSTOMER_ID == $scope.Cust_Market_Backup[i].CUSTOMER_ID)
                if (ind > 0) {
                    $scope.gridOptionsList.data.push($scope.Cust_Market_Backup[i]);
                }
            }
            $scope.GetComissionDoneCustomers();
        })
    }
    $scope.GetComissionDoneCustomers = function () {
       
        SkuCommissionService.GetComissionDoneCustomers($scope.SKU_ID_Back).then(response => {
            $scope.ComissionDoneCustomers = response.data;
            $scope.Cust_Market_Backup = $scope.gridOptionsList.data;
            $scope.gridOptionsList.data = [];

            for (var i = 0; i < $scope.Cust_Market_Backup.length; i++) {
                var ind = $scope.ComissionDoneCustomers.findIndex(x => x.CUSTOMER_ID == $scope.Cust_Market_Backup[i].CUSTOMER_ID)
                if (ind == 0) {
                    $scope.gridOptionsList.data.push($scope.Cust_Market_Backup[i]);
                }
            }
        })
    }

    $scope.LoadUnitData = function () {
        $scope.showLoader = true;

        SkuCommissionService.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Units = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });

            SkuCommissionService.GetUnit().then(response => {
                $scope.model.UNIT_ID = response.data
            })
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.ClearForm = function () {
        $scope.model.DRIVER_ID = 0;
        $scope.model.DRIVER_NAME = '';
        $scope.model.CONTACT_NO = '';
        $scope.model.STATUS = 'Active';
        $scope.model.REMARKS = '';

    }

    $scope.LoadData = function () {
        SkuCommissionService.LoadData().then(response => {
            $scope.gridOptionsSkuPrice.data = response.data;
            $('#exampleModalScrollable').modal('show');
        })
    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;

        $scope.permissionReqModel = {
            Controller_Name: 'SkuCommission',
            Action_Name: 'SkuCommission'
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
    $scope.GetDivitionToMarketRelation();

    $scope.SaveData = function (model) {
        debugger
        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);

        $scope.showLoader = true;

        model.DETAILS = $scope.gridOptionsList.data
            .filter(e => e.COMMISSION_VALUE != undefined && e.COMMISSION_VALUE != null && e.COMMISSION_VALUE != 0);
        for (var i = 0; i < model.DETAILS.length; i++) {
            model.DETAILS[i].COMMISSION_VALUE = JSON.stringify(parseFloat(model.DETAILS[i].COMMISSION_VALUE));
            model.DETAILS[i].ADD_COMMISSION1 = JSON.stringify(parseFloat(model.DETAILS[i].ADD_COMMISSION1));
            model.DETAILS[i].ADD_COMMISSION2 = JSON.stringify(parseFloat(model.DETAILS[i].ADD_COMMISSION2));
            model.DETAILS[i].UNIT_ID = JSON.stringify(parseInt(model.DETAILS[i].UNIT_ID));
            model.DETAILS[i].SKU_ID = JSON.stringify(parseInt(model.DETAILS[i].SKU_ID));

        }
        SkuCommissionService.Add(model).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                //$scope.GetPermissionData();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

    $scope.Process = function () {
        if (window.confirm("Are you sure?")) {
            SkuCommissionService.Process($scope.model.MST_ID).then(response => {
                if (response.data == "1") {
                    $scope.model.IS_PROCESSED = 'Yes'
                    notificationservice.Notification(response.data, 1, 'Data Successfully Processed!!');
                }
            });
        }
    }

    $scope.ClearForm = function () {
        window.location = "/SalesAndDistribution/SkuCommission/SkuCommission";
    }
}]);
