ngApp.controller('ngGridCtrl', ['$scope', 'InsertOrEditServices', 'AdjustmentServices', 'permissionProvider', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InsertOrEditServices, AdjustmentServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {
        COMPANY_ID: 0, UNIT_ID: 0, CUSTOMER_ID: 0, CUSTOMER_CODE: '', MARKET_ID: 0, TERRITORY_ID: 0, AREA_ID: 0, REGION_ID: 0, DIVISION_ID: 0, PAYMENT_MODE: '', REPLACE_CLAIM_NO: '', BONUS_PROCESS_NO: '', BONUS_CLAIM_NO: '', INVOICE_STATUS: '', ORDER_AMOUNT: 0, ORDER_UNIT_ID: 0, INVOICE_UNIT_ID: 0, REMARKS: '', SPA_TOTAL_AMOUNT: 0, SPA_COMMISSION_PCT: 0, SPA_COMMISSION_AMOUNT: 0, SPA_NET_AMOUNT: 0, REMARKS: ''
    }

    $scope.getPermissions = [];
    $scope.Customers = [];
    $scope.Customer_Order = [];
    $scope.SKU_Order = [];
    $scope.Ratioanable_Rows = [];
    $scope.Order_SKU_Data = [];
    $scope.GetDivitionToMarketRelation = function () {
        $scope.showLoader = true;
        InsertOrEditServices.GetDivitionToMarketRelation($scope.model.DIVISION_ID).then(function (data) {
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
        $scope.showLoader = false;

    }

    $scope.totalOrder = function (value) {
        return value.reduce(function (sum, current) {
            return sum + current.ORDER_QTY;
        }, 0);

    }

    $scope.totalStock = function (arr) {
        for (let index = 0; index < arr.length; index++) {
            if (arr[index].STOCK_QTY != 0) {
                return arr[index].STOCK_QTY;
            }
        }
    }

    $scope.Calculate = function (curr_index, pre_index) {

        var list = document.getElementsByClassName(pre_index);

        let total = 0
        //for (let index = 0; index < $scope.Ratioanable_Rows[pre_index].row_data.length; index++) {
        //    if ($scope.Ratioanable_Rows[pre_index].row_data[index].REVISED_QTY != undefined && $scope.Ratioanable_Rows[pre_index].row_data[index].REVISED_QTY != null && $scope.Ratioanable_Rows[pre_index].row_data[index].REVISED_QTY != '')
        //        total += parseInt($scope.Ratioanable_Rows[pre_index].row_data[index].REVISED_QTY)
        //}
        for (let n = 0; n < list.length; ++n) {
            if (list[n].value != undefined && list[n].value != null && list[n].value != '')
                total += parseInt(list[n].value);
        }

        document.getElementById(pre_index).value = total;

    }
    $scope.GetAdjustmentList = function () {
        $scope.showLoader = true;
        AdjustmentServices.GetAdjustmentList($scope.model.COMPANY_ID).then(function (data) {
            $scope.Adjustments = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;

        $scope.permissionReqModel = {
            Controller_Name: 'SalesOrder',
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
    $scope.LoadRationableData = function () {
        $scope.showLoader = true;
        $scope.modeldata = [];
        $scope.modeldata.DATE_FROM = $scope.model.DATE_FROM;
        $scope.modeldata.DATE_TO = $scope.model.DATE_TO;
        $scope.modeldata.DIVISION_ID = $scope.model.DIVISION_ID == null || $scope.model.DIVISION_ID == '' ? 'ALL' : $scope.model.DIVISION_ID;
        $scope.modeldata.REGION_ID = $scope.model.REGION_ID == null || $scope.model.REGION_ID == '' ? 'ALL' : $scope.model.REGION_ID;
        $scope.modeldata.AREA_ID = $scope.model.AREA_ID == null || $scope.model.AREA_ID == '' ? 'ALL' : $scope.model.AREA_ID;
        $scope.modeldata.TERRITORY_ID = $scope.model.TERRITORY_ID == null || $scope.model.TERRITORY_ID == '' ? 'ALL' : $scope.model.TERRITORY_ID;
        $scope.modeldata.CUSTOMER_ID = $scope.model.CUSTOMER_ID == null || $scope.model.CUSTOMER_ID == '' ? 'ALL' : $scope.model.CUSTOMER_ID;

        InsertOrEditServices.LoadData($scope.modeldata).then(function (data) {

            var flags_Order = [];
            var flags_SKU = [];
            var flags_Mst = [];
            $scope.Order_SKU_Data = data.data;
            $scope.Customer_Order = [];
            $scope.Mst_Order = [];
            $scope.SKU_Order = [];

            var l = data.data.length;

            for (var i = 0; i < l; i++) {
                if (flags_Order[data.data[i].CUSTOMER_ID]) {

                } else {
                    flags_Order[data.data[i].CUSTOMER_ID] = true;
                    $scope.Customer_Order.push(data.data[i]);
                }
                if (flags_SKU[data.data[i].SKU_ID]) {

                } else {
                    flags_SKU[data.data[i].SKU_ID] = true;
                    $scope.SKU_Order.push(data.data[i]);
                }
                if (flags_Mst[data.data[i].ORDER_MST_ID]) {

                } else {
                    flags_Mst[data.data[i].ORDER_MST_ID] = true;
                    $scope.Mst_Order.push(data.data[i]);
                }
            }
            $scope.Ratioanable_Rows = [];
            for (var i = 0; i < $scope.SKU_Order.length; i++) {
                $scope.Ratioanable_Rows.push({ SKU_ID: $scope.SKU_Order[i].SKU_ID, SKU_NAME: $scope.SKU_Order[i].SKU_NAME, SKU_CODE: $scope.SKU_Order[i].SKU_CODE, ORDER_MST_ID: $scope.SKU_Order[i].ORDER_MST_ID })
            }

            for (var j = 0; j < $scope.Ratioanable_Rows.length; j++) {

                $scope.Ratioanable_Rows[j].row_data = [];


                for (var k = 0; k < $scope.Mst_Order.length; k++) {

                    var sku_Raw = data.data.filter(x => x.SKU_ID == $scope.Ratioanable_Rows[j].SKU_ID && x.ORDER_MST_ID == $scope.Mst_Order[k].ORDER_MST_ID);
                    if (sku_Raw != null && sku_Raw.length > 0) {
                        sku_Raw[0].active = false;
                        $scope.Ratioanable_Rows[j].row_data.push(sku_Raw[0]);

                    } else {

                        $scope.Ratioanable_Rows[j].row_data.push({ ORDER_NO: 0, ORDER_QTY: 0, STOCK_QTY: 0, SUGGESTED_LIFTING: 0, REVISED_QTY: '', Active: true });

                    }

                }


            }
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.ClearForm = function () {
        $scope.model = [];
        $scope.GetPermissionData();

    }
    $scope.LoadGIFT_ITEM_ID = function () {
        $('#GIFT_ITEM_ID').trigger('change');

    }
    $scope.LoadGIFT_ITEM_ID = function () {
        $('#GIFT_ITEM_ID').trigger('change');

    }
    $scope.LoadGIFT_ITEM_ID = function () {
        $('#GIFT_ITEM_ID').trigger('change');

    }
    $scope.LoadGIFT_ITEM_ID = function () {
        $('#GIFT_ITEM_ID').trigger('change');

    }
    $scope.LoadGIFT_ITEM_ID = function () {
        $('#GIFT_ITEM_ID').trigger('change');

    }
    $scope.GetPermissionData();
    $scope.GetAdjustmentList();
    $scope.GetDivitionToMarketRelation();
    // This Method work is Edit Data Loading

    $scope.SaveData = function () {
        $scope.showLoader = true;
        for (var i = 0; i < $scope.Order_SKU_Data.length; i++) {
            var id_name = '#' + $scope.Order_SKU_Data[i].ORDER_NO + '_' + $scope.Order_SKU_Data[i].SKU_ID;
            var id_adj_type = '#' + 'ADJUSTMENT_TYPE_' + $scope.Order_SKU_Data[i].ORDER_NO;
            var id_adj_amt = '#' + 'ADJUSTMENT_AMOUNT_' + $scope.Order_SKU_Data[i].ORDER_NO;

            var d_revised = angular.element(id_name).val();
            //Adjustment 
            var d_adj_type = angular.element(id_adj_type).val();
            var d_adj_amt = angular.element(id_adj_amt).val();

            $scope.Order_SKU_Data[i].REVISED_QTY = parseInt(d_revised);
            if (d_revised != null && d_revised != '' && d_revised != undefined) {
                $scope.Order_SKU_Data[i].REVISED_QTY = parseInt(d_revised);

            }
            $scope.Order_SKU_Data[i].ADJ_TYPE = d_adj_type != '? undefined:undefined ?' &&  d_adj_type != null && d_adj_type != '' && d_adj_type != undefined ? parseFloat(d_adj_type) : 0.0;
            $scope.Order_SKU_Data[i].ADJ_AMT = d_adj_amt != null && d_adj_amt != '' && d_adj_amt != undefined ? parseInt(d_adj_amt) : 0;
        }
        let saveObj = $scope.Order_SKU_Data.filter(x => x.REVISED_QTY != undefined && x.REVISED_QTY != null && !isNaN(x.REVISED_QTY));

        InsertOrEditServices.AddOrUpdate(saveObj).then(function (data) {
            
            if (data.data.status == '1') {
                $scope.showLoader = false;
                notificationservice.Notification(data.data.status, '1', 'data save successfully!');
                setTimeout(function () {
                }, 1000)
            }
            else {
                $scope.showLoader = false;
                notificationservice.Notification(data.data.status, "", 'data not save successfully!');
            }
        });


    }
   

}]);

