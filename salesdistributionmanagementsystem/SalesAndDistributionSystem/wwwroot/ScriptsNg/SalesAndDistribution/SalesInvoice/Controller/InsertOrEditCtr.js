ngApp.controller('ngGridCtrl', ['$scope', 'InsertOrEditServices', 'RegionInfoServices', 'AreaInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', 'uiGridConstants', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InsertOrEditServices, RegionInfoServices, AreaInfoServices, permissionProvider, notificationservice, gridregistrationservice, uiGridConstants, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {
        COMPANY_ID: 0, UNIT_ID: 0, CUSTOMER_ID: 0, CUSTOMER_CODE: '', MARKET_ID: 0, TERRITORY_ID: 0, AREA_ID: 0, REGION_ID: 0, DIVISION_ID: 0, PAYMENT_MODE: '', REPLACE_CLAIM_NO: '', BONUS_PROCESS_NO: '', BONUS_CLAIM_NO: '', INVOICE_STATUS: '', ORDER_AMOUNT: 0, ORDER_UNIT_ID: 0, INVOICE_UNIT_ID: 0, REMARKS: '', SPA_TOTAL_AMOUNT: 0, SPA_COMMISSION_PCT: 0, SPA_COMMISSION_AMOUNT: 0, SPA_NET_AMOUNT: 0, REMARKS: ''
      }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];
    $scope.Markets = [];
    $scope.InvoiceTypes = [];
    $scope.Units = [];
    $scope.Products = [];
    $scope.PaymentMode = [];
    $scope.LastPreInvoiceEntity = [];
    $scope.EntryType = [];
    $scope.model.ORDER_STATUS = 'Active';
    $scope.COMBO_LOADING_DISCOUNT = 0;
    $scope.MST_COMBO_DISCOUNT = 0;
    $scope.PRE_NET_AMOUNT = 0;

    //Filter Data Load----------------------
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


    $scope.GetCurrentDate = function () {
        const today = new Date();
        const yyyy = today.getFullYear();
        let mm = today.getMonth() + 1; // Months start at 0!
        let dd = today.getDate();

        if (dd < 10) dd = '0' + dd;
        if (mm < 10) mm = '0' + mm;

        $scope.model.DATE_FROM = dd + '/' + mm + '/' + yyyy;
        $scope.model.DATE_TO = dd + '/' + mm + '/' + yyyy;

    }

    $scope.GetCurrentDate();
    //------------END----------------------










    $scope.gridOptions = (gridregistrationservice.GridRegistration("Region Area Relation"));
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptions.data = [];
    $scope.gridOptionsPreInvoice = (gridregistrationservice.GridRegistration("Pre Invoice"));
    $scope.gridOptionsPreInvoice.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsPreInvoice.data = [];
    



  
    $scope.LoadActiveDivisions = function () {

        $scope.showLoader = true;

        InsertOrEditServices.LoadActiveDivisions($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.Markets_data = data.data;
            var _Markets = {
                DIVISION_ID: "0",
                DIVISION_NAME: "None",
               
            }
            $scope.Markets.push(_Markets);
            for (var i in $scope.Markets_data) {
                $scope.Markets.push($scope.Markets_data[i]);
            }


            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }

    $scope.LoadPreInvoiceData = function (entity) {
        
        $scope.showLoader = true;
        
        InsertOrEditServices.LoadPreInvoiceData(entity).then(function (data) {
            
            $scope.LastPreInvoiceEntity = entity;
            for (var i = 0; i < data.data.length; i++) {
                data.data[i].ORDER_NO = entity.ORDER_NO;
                $scope.COMBO_LOADING_DISCOUNT = data.data[0].COMBO_LOADING_DISCOUNT;
                $scope.MST_COMBO_DISCOUNT = data.data[0].MST_COMBO_DISCOUNT;
                $scope.PRE_NET_AMOUNT = $scope.PRE_NET_AMOUNT  + data.data[i].NET_AMOUNT;
            }

            $scope.PRE_NET_AMOUNT = $scope.PRE_NET_AMOUNT - $scope.COMBO_LOADING_DISCOUNT - $scope.MST_COMBO_DISCOUNT;
            $scope.gridOptionsPreInvoice.data = data.data;

            
            $('#exampleModalScrollable').modal('show');

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }


    $scope.LoadEntryType = function () {
        var Active = {
            STATUS: 'Manual'
        }
        var InActive = {
            STATUS: 'Auto'
        }

        $scope.EntryType.push(Active);

        $scope.EntryType.push(InActive);


    }
    $scope.LoadEntryType();

    $scope.LoadStatus = function () {
        var Active = {
            STATUS: 'Active'
        }
        var InActive = {
            STATUS: 'InActive'
        }
        var Complete = {
            STATUS: 'Complete'
        }
        $scope.Status.push(Active);

        $scope.Status.push(InActive);
        $scope.Status.push(Complete);


    }
    $scope.LoadStatus();

    $scope.InvoiceTypeLoad = function () {

        $scope.showLoader = true;

        InsertOrEditServices.LoadInvoiceTypes($scope.model.COMPANY_ID).then(function (data) {

            $scope.InvoiceTypes = data.data;

            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.InvoiceTypeLoad();
   

   

    $scope.ShowDetails = (entity) => {
        
        window.open("/SalesAndDistribution/SalesOrder/OrderDetails?Id=" + entity.ORDER_MST_ID_ENCRYPTED + "&st=" + $scope.model.edt, '_blank');


    };

    $scope.EditItem = (entity) => {
        
        window.open("/SalesAndDistribution/SalesOrder/InsertOrEdit?Id=" + entity.ORDER_MST_ID_ENCRYPTED + "&st=" + $scope.model.edt, '_blank');


    };
    // Grid one row remove if this mehtod is call
   
    $scope.LoadPostableData = function () {
        
        $scope.showLoader = true;
        $scope.modeldata = [];
        $scope.modeldata.DATE_FROM = $scope.model.DATE_FROM;
        $scope.modeldata.DATE_TO = $scope.model.DATE_TO;
        $scope.modeldata.DIVISION_ID = $scope.model.DIVISION_ID == null || $scope.model.DIVISION_ID == '' ? '' : $scope.model.DIVISION_ID;
        $scope.modeldata.REGION_ID = $scope.model.REGION_ID == null || $scope.model.REGION_ID == '' ? '' : $scope.model.REGION_ID;
        $scope.modeldata.AREA_ID = $scope.model.AREA_ID == null || $scope.model.AREA_ID == '' ? '' : $scope.model.AREA_ID;
        $scope.modeldata.TERRITORY_ID = $scope.model.TERRITORY_ID == null || $scope.model.TERRITORY_ID == '' ? '' : $scope.model.TERRITORY_ID;
        $scope.modeldata.CUSTOMER_ID = $scope.model.CUSTOMER_ID == null || $scope.model.CUSTOMER_ID == '' ? '' : $scope.model.CUSTOMER_ID;
        $scope.modeldata.ORDER_ENTRY_TYPE = $scope.model.ORDER_ENTRY_TYPE == null || $scope.model.ORDER_ENTRY_TYPE == '' ? '' : $scope.model.ORDER_ENTRY_TYPE;
        $scope.modeldata.ORDER_STATUS = $scope.model.ORDER_STATUS == null || $scope.model.ORDER_STATUS == '' ? '' : $scope.model.ORDER_STATUS;
        $scope.modeldata.ORDER_TYPE = $scope.model.ORDER_TYPE == null || $scope.model.ORDER_TYPE == '' ? '' : $scope.model.ORDER_TYPE;

        InsertOrEditServices.LoadPostableData($scope.modeldata).then(function (data) {
            $scope.gridOptions.data = data.data;
            $scope.showLoader = false;

        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    
    $scope.CancelOrder = function (entity) {
        
        $scope.showLoader = true;

        InsertOrEditServices.CancelOrder(0, entity.ORDER_MST_ID).then(function (data) {
            $scope.LoadPostableData();
            notificationservice.Notification(1, 1, "Data Deleted Successfully!!");
            $scope.showLoader = false;

        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.SinglePostOrder = function (entity) {
        
        $scope.showLoader = true;

        InsertOrEditServices.SinglePostOrder(0, entity.ORDER_MST_ID).then(function (data) {
            if (data.data.Key == "1") {
                notificationservice.Notification(data.data.Key, "1", data.data.Status);
            } else {
                notificationservice.Notification(data.data.Status, "1", data.data.Key);

            }
            $scope.LoadPostableData();

            $scope.showLoader = false;

        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    $scope.AllPostOrder = function () {
        
        $scope.showLoader = true;
        $scope.ORDER_NO_LIST = [];
        for (var i = 0; i < $scope.gridOptions.data.length; i++) {
            $scope.ORDER_NO_LIST.push(JSON.stringify($scope.gridOptions.data[i].ORDER_MST_ID));
        }
        InsertOrEditServices.AllPostOrder(0, $scope.ORDER_NO_LIST).then(function (data) {
            if (data.data.Key == "1") {
                notificationservice.Notification(data.data.Key, "1", data.data.Status);
            } else {
                notificationservice.Notification(data.data.Status, "1", data.data.Key);

            }
            $scope.LoadPostableData();

            $scope.showLoader = false;

        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    $scope.AddAdjustment = function (entity) {
        window.open("/SalesAndDistribution/OrderAdjustment/Adjustment?ORDER_MST_ID=" + entity.ORDER_MST_ID + "&ORDER_NO=" + entity.ORDER_NO + "&ORDER_UNIT_ID=" + entity.ORDER_UNIT_ID, "_blank")
    }
    $scope.LoadToDate = function ()
    {
        $scope.model.ORDER_DATE = new Date();
    }
    $scope.gridOptions.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false,  width: '50'
        }
       
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , { name: 'UNIT_ID', field: 'UNIT_ID', visible: false }

        , {
            name: 'ORDER_NO', field: 'ORDER_NO', displayName: 'Order No', enableFiltering: true, width: '130', cellTemplate:
                '<input type="text"  ng-model="row.entity.ORDER_NO" disabled="true"  class="pl-sm" />'

        }, {
            name: 'INVOICE_TYPE_NAME', field: 'INVOICE_TYPE_NAME', displayName: 'Type', enableFiltering: true, width: '70'
        }
       
        , {
            name: 'ORDER_DATE', field: 'ORDER_DATE', displayName: 'Date', enableFiltering: true, width: '8%', cellTemplate:
                '<input type="text"  ng-model="row.entity.ORDER_DATE" disabled="true"  class="pl-sm" />'

        }

        , {
            name: 'CUSTOMER_NAME', field: 'CUSTOMER_NAME', displayName: 'Customer', enableFiltering: true, width: '12%', cellTemplate:
                '<input type="text"  ng-model="row.entity.CUSTOMER_NAME" ng-disabled="grid.appScope.model.CUSTOMER_ID <1" ng-change="grid.appScope.OnProductQTYChange(row.entity)"  class="pl-sm" />'

        }, {
            name: 'MARKET', field: 'MARKET', displayName: 'Market', enableFiltering: true, width: '8%', cellTemplate:
                '<input type="text"  ng-model="row.entity.MARKET_NAME" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'ORDER_AMOUNT', field: 'ORDER_AMOUNT', displayName: 'Order Amt', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="text"  ng-model="row.entity.ORDER_AMOUNT" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'NET_ORDER_AMOUNT', field: 'NET_ORDER_AMOUNT', displayName: 'Net Amt', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="text"  ng-model="row.entity.NET_ORDER_AMOUNT" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'Action', field: 'Action', displayName: 'Action', enableFiltering: false, width: '45%', cellTemplate:
                //'<button style="margin-bottom: 5px;margin-right: 10px"  ng-click="grid.appScope.ShowDetails(row.entity)" type="button" class="btn btn-success mb-1">Details</button>' + 

                '<button style="margin-bottom: 5px;margin-right: 10px"  ng-click="grid.appScope.EditItem(row.entity)" type="button" ng-disabled="row.entity.ORDER_STATUS != \'Active\'" class="btn btn-success mb-1">Edit/Adjustment</button>'
                //+  '<button style="margin-bottom: 5px;margin-right: 10px"  ng-click="grid.appScope.AddAdjustment(row.entity)" type="button" ng-disabled="row.entity.ORDER_STATUS != \'Active\'" class="btn btn-success mb-1">Add Adjustment</button>'

                + '<button style="margin-bottom: 5px;margin-right: 10px"  ng-click="grid.appScope.LoadPreInvoiceData(row.entity)" type="button" ng-disabled="row.entity.ORDER_STATUS != \'Active\'" class="btn btn-primary mb-1">Pre Invoice</button>'
                + '<button style="margin-bottom: 5px;margin-right: 10px" ng-click="grid.appScope.CancelOrder(row.entity)" type="button" ng-disabled="row.entity.ORDER_STATUS != \'Active\'" class="btn btn-danger mb-1">Cancel</button>'
                +  '<button style="margin-bottom: 5px;margin-right: 10px" ng-click="grid.appScope.SinglePostOrder(row.entity)"  ng-disabled="row.entity.ORDER_STATUS != \'Active\'"   type="button" class="btn  btn-secondary  mb-1">Posting</button>'

        }
       

    ];

    $scope.gridOptionsPreInvoice.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'ORDER_MST_ID', field: 'ORDER_MST_ID', visible: false }

        , { name: 'SKU_ID', field: 'SKU_ID', visible: false }
        , {
            name: 'ORDER_NO', field: 'ORDER_NO', displayName: 'Order No', enableFiltering: false, width: '6%', cellTemplate:
                '<input type="text"  ng-model="row.entity.ORDER_NO" disabled="true"  class="pl-sm" />'

        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'Name', enableFiltering: false, width: '18%', cellTemplate:
                '<input type="text"  ng-model="row.entity.SKU_NAME" disabled="true"  class="pl-sm" />'

        }

        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'Code', enableFiltering: false, width: '6%', cellTemplate:
                '<input type="text"  ng-model="row.entity.SKU_CODE" disabled="true"  class="pl-sm" />'

        }

        , {
            name: 'PACK_SIZE', field: 'PACK_SIZE', displayName: 'Pack Size', enableFiltering: false, width: '6%', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #9ed4a0;color: Black">Total:</div>', cellTemplate:
                '<input type="text"  ng-model="row.entity.PACK_SIZE" ng-disabled="grid.appScope.model.CUSTOMER_ID <1" ng-change="grid.appScope.OnProductQTYChange(row.entity)"  class="pl-sm" />'

        },
         {
            name: 'TOTAL_AMOUNT', field: 'TOTAL_AMOUNT', displayName: 'Total Amt', enableFiltering: false, width: '6%', aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, cellTemplate:
                '<input type="number"  ng-model="row.entity.TOTAL_AMOUNT" disabled="false"  class="pl-sm" />'

        },
        {
            name: 'CURRENT_STOCK', field: 'CURRENT_STOCK', displayName: 'Current Stock', enableFiltering: false, width: '6%',   cellTemplate:
                '<input type="number"  ng-model="row.entity.CURRENT_STOCK" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'ORDER_QTY', field: 'ORDER_QTY', displayName: 'Order Qty', enableFiltering: false, width: '8%', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #9ed4a0;color: Black">Combo Load Disc:</div>', cellTemplate:
                '<input type="number"  ng-model="row.entity.ORDER_QTY" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'REVISED_ORDER_QTY', field: 'REVISED_ORDER_QTY', displayName: 'Revised Qty', footerCellTemplate: '<div class="ui-grid-cell-contents" style="color: Black"> {{grid.appScope.COMBO_LOADING_DISCOUNT}}</div>', enableFiltering: false, width: '8%', cellTemplate:
                '<input type="number"  ng-model="row.entity.REVISED_ORDER_QTY"  style="background-color: white;"  class="pl-sm" />'

        }
        , {
            name: 'BONUS_QTY', field: 'BONUS_QTY', displayName: 'Bonus Qty', enableFiltering: false, width: '10%', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: #9ed4a0;color: Black">Combo Disc:</div>',  cellTemplate:
                '<input type="number"  ng-model="row.entity.BONUS_QTY" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'TOTAL_QTY', field: 'TOTAL_QTY', displayName: 'Total Qty', enableFiltering: false, width: '6%', footerCellTemplate: '<div class="ui-grid-cell-contents" style="color: Black"> {{grid.appScope.MST_COMBO_DISCOUNT}}</div>',  cellTemplate:
                '<input type="number"  ng-model="row.entity.TOTAL_QTY" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'STOCK_AFTER_INVOICE', field: 'STOCK_AFTER_INVOICE', displayName: 'After Invoice Stock', enableFiltering: false, width: '6%', cellTemplate:
                '<input type="number"  ng-model="row.entity.STOCK_AFTER_INVOICE" disabled="false"  class="pl-sm" />'

        }

        , {
            name: 'SKU_PRICE', field: 'SKU_PRICE', displayName: 'SKU Price', enableFiltering: false, width: '6%',  cellTemplate:
                '<input type="number"  ng-model="row.entity.SKU_PRICE" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'CUSTOMER_DISC_AMOUNT', field: 'CUSTOMER_DISC_AMOUNT', displayName: 'Customer Disc Amt', enableFiltering: false, width: '6%', aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, cellTemplate:
                '<input type="number"  ng-model="row.entity.CUSTOMER_DISC_AMOUNT" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'CUSTOMER_PROD_DISC_AMOUNT', field: 'CUSTOMER_PROD_DISC_AMOUNT', displayName: 'Customer Prod Disc Amt', enableFiltering: false, width: '6%', aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true,  cellTemplate:
                '<input type="number"  ng-model="row.entity.CUSTOMER_PROD_DISC_AMOUNT" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'BONUS_DISC_AMOUNT', field: 'BONUS_DISC_AMOUNT', displayName: 'Bonus Disc Amt', enableFiltering: false, width: '6%', aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true,  cellTemplate:
                '<input type="number"  ng-model="row.entity.BONUS_DISC_AMOUNT" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'NET_AMOUNT', field: 'NET_AMOUNT', displayName: 'Net Amt', enableFiltering: false, width: '6%', footerCellTemplate: '<div class="ui-grid-cell-contents" style="color: Black"> {{grid.appScope.PRE_NET_AMOUNT}}</div>', aggregationHideLabel: true,  cellTemplate:
                '<input type="number"  ng-model="row.entity.NET_AMOUNT" disabled="false"  class="pl-sm" />'

        }
      

        
        , {
            name: 'PREV_IMS_QTY', field: 'PREV_IMS_QTY', displayName: 'Prev IMS Qty', enableFiltering: false, width: '6%',   cellTemplate:
                '<input type="number"  ng-model="row.entity.PREV_IMS_QTY" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'SUGG_LIFTING_QTY', field: 'SUGG_LIFTING_QTY', displayName: 'SUGG Lifting Qty', enableFiltering: false, width: '6%', cellTemplate:
                '<input type="number"  ng-model="row.entity.SUGG_LIFTING_QTY" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'DIST_CURRENT_STOCK', field: 'DIST_CURRENT_STOCK', displayName: 'Dist Current Stock', enableFiltering: false, width: '6%',  cellTemplate:
                '<input type="number"  ng-model="row.entity.DIST_CURRENT_STOCK" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'LOADING_DISCOUNT', field: 'LOADING_DISCOUNT', displayName: 'Loading Disc', enableFiltering: false, width: '6%', aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true,  cellTemplate:
                '<input type="number"  ng-model="row.entity.LOADING_DISCOUNT" disabled="false"  class="pl-sm" />'

        }
        , {
            name: 'ADJUSTMENT_DISCOUNT', field: 'ADJUSTMENT_DISCOUNT', displayName: 'Adj Disc', enableFiltering: false, width: '6%', aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true,  cellTemplate:
                '<input type="number"  ng-model="row.entity.ADJUSTMENT_DISCOUNT" disabled="false"  class="pl-sm" />'

        }
       


    ];

    $scope.gridOptionsPreInvoice.showColumnFooter = !$scope.gridOptions.showColumnFooter;
    //$scope.gridOptionsPreInvoice.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);


    $scope.CompanyLoad = function () {

        InsertOrEditServices.GetCompany().then(function (data) {
            
            $scope.model.COMPANY_ID = parseFloat(data.data);
            
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
   


    $scope.UnitLoad = function () {

        InsertOrEditServices.GetUnit().then(function (data) {
            
            $scope.model.UNIT_ID = parseInt(data.data);
            $scope.model.ORDER_UNIT_ID = parseInt(data.data);

            for (var i = 0; i < $scope.Units.length; i++) {
                if ($scope.model.UNIT_ID == $scope.Units[i].UNIT_ID) {
                    $scope.model.UNIT_NAME = $scope.Units[i].UNIT_NAME;
                }
            }

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
   
    
   

    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;

        InsertOrEditServices.GetCompanyList().then(function (data) {
            
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    $scope.UnitsLoad = function () {
        $scope.showLoader = true;

        InsertOrEditServices.GetUnitList().then(function (data) {
            
            $scope.Units = data.data.filter(x => x.COMPANY_ID == $scope.model.COMPANY_ID);
            $scope.UnitLoad();
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }


 
    

    $scope.ClearForm = function () {
        window.location = "/SalesAndDistribution/SalesInvoice/InsertOrEdit";

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
            Controller_Name: 'SalesInvoice',
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
            $scope.model.CONFIRM_PERMISSION = $scope.getPermissions.confirM_PERMISSION;

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
    $scope.UnitsLoad();
    $scope.UnitLoad();
    $scope.InvoiceTypeLoad();
    $scope.GetDivitionToMarketRelation();
    //$scope.LoadActiveDivisions(0);

    // This Method work is Edit Data Loading
   
    $scope.SaveRevisedOrderQty = function (model) {
        
        $scope.showLoader = true;
        model.Order_Dtls = [];
        for (var i = 0; i < $scope.gridOptionsPreInvoice.data.length; i++) {

                model.Order_Dtls.push($scope.gridOptionsPreInvoice.data[i]);
        }
        InsertOrEditServices.UpdateOrderRevisedQty(model).then(function (data) {
            
            $scope.LoadPostableData();
            $scope.LoadPreInvoiceData($scope.LastPreInvoiceEntity);
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }

}]);

