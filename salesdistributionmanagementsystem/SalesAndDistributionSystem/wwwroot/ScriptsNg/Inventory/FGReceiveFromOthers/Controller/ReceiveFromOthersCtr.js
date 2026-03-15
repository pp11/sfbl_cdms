ngApp.controller('ngGridCtrl', ['$scope', 'ReceiveFromOthersServices', 'permissionProvider', 'gridregistrationservice', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, ReceiveFromOthersServices, permissionProvider, gridregistrationservice, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'

    $scope.formatDate = function (date) {
        var d = new Date(date),
            month = '' + (d.getMonth() + 1),
            day = '' + d.getDate(),
            year = d.getFullYear();

        if (month.length < 2)
            month = '0' + month;
        if (day.length < 2)
            day = '0' + day;

        return [day, month, year].join('/');
    }


    const todate = $scope.formatDate(new Date());

    $scope.model = {
        COMPANY_ID: 0, UNIT_ID: 0, RECEIVE_ID: 0,
        UNIT_ID: 0, BATCH_ID: 0, BATCH_NO: '',
        //SKU_ID: 0,
        SKU_CODE: '', PACK_SIZE: '', UNIT_TP: '',
        RECEIVE_QTY: 0, SHIPPER_QTY: 0, RECEIVE_AMOUNT: 0,
        RECEIVE_STATUS: 'Complete', RECEIVE_STOCK_TYPE: 'P',
        RECEIVE_TYPE: '',
        RECEIVE_DATE: todate,
        CHALLAN_DATE: '',
        CHALLAN_NO: '',
        REMARKS: ''
    }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Units = [];
    $scope.suppliers = [];
    $scope.products = [];
    $scope.TransferTypes = [];
    $scope.ReceiveStockTypes = [];
    $scope.User_data = [];
    $scope.Status = [];
    $scope.batches = [];
    $scope.IsBatchActive = true;

    $scope.gridOptionsListUnchecked = (gridregistrationservice.GridRegistration("Unchecked Info"));
    $scope.gridOptionsListUnchecked.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }

    $scope.gridOptionsListUnchecked.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'UNIT_ID', field: 'UNIT_ID', visible: false }

        , { name: 'RECEIVE_ID', field: 'RECEIVE_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , {
            name: 'CHALLAN_NO', field: 'CHALLAN_NO', displayName: 'Challan No', enableFiltering: true, width: '10%'
        }
        , {
            name: 'CHALLAN_DATE', field: 'CHALLAN_DATE', displayName: 'Date', enableFiltering: true, width: '12%'
        }
        , {
            name: 'RECEIVE_DATE', field: 'RECEIVE_DATE', displayName: 'Receive Date', enableFiltering: true, width: '12%'
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: true, width: '10%'
        }
        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '10%'
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '20%'
        }
        , {
            name: 'RECEIVE_QTY', field: 'RECEIVE_QTY', displayName: 'Receive Qty', enableFiltering: true, width: '10%'
        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, visible: false, width: '10%'
        }
        , { name: 'DIVISION_REGION_MST_STATUS', field: 'DIVISION_REGION_MST_STATUS', visible: false, displayName: 'Status', enableFiltering: true, width: '15%' }
        , { name: 'CHECKED_BY_NAME', field: 'CHECKED_BY_NAME', visible: true, displayName: 'Checked By', enableFiltering: true, width: '13%' }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" data-dismiss="modal"  ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.UpdateStatusToCancel(row.entity)" type="button" class="btn btn-outline-primary mb-1">Cancel</button>' +
                '</div>'
        },
    ];

    $scope.gridOptionsListUnchecked.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    //$scope.gridOptionsListUnchecked.enableGridMenu = false;


    $scope.gridRefurbishment = (gridregistrationservice.GridRegistration("Ref Info"));
    $scope.gridRefurbishment.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridRefurbishment.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'SKU_ID', field: 'SKU_ID', visible: false }

        , {
            name: 'CHALLAN_NO', field: 'CHALLAN_NO', displayName: 'Challan No', enableFiltering: true, width: '30%'
        }
      
        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '10%'
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '20%'
        }
        , {
            name: 'PROD_QTY', field: 'PROD_QTY', displayName: 'Qty', enableFiltering: true, width: '10%'
        }
        , {
            name: 'AMOUNT', field: 'AMOUNT', displayName: 'Amount', enableFiltering: true, width: '10%'
        }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" data-dismiss="modal"  ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.Ref_SKU_Load(row.entity)" type="button" class="btn btn-outline-primary mb-1">Load</button>' +
                '</div>'
        },
    ];

    $scope.gridRefurbishment.rowTemplate = "<div ng-dblclick=\"grid.appScope.Ref_SKU_Load(row.entity)\" title=\"Please double click to load \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"


    

    $scope.EditData = function (entity) {
        $('#myModal1').modal('toggle');
        $scope.GetEditDataById(entity.RECEIVE_ID_ENCRYPTED, true);
    }

    $scope.DataLoadUnchecked = function () {
        $scope.showLoader = true;

        ReceiveFromOthersServices.LoadUnchekckData($scope.model.COMPANY_ID).then(function (data) {

            $scope.gridOptionsListUnchecked.data = data.data;
            $scope.showLoader = false;
        }, function (error) {

            alert(error);

            $scope.showLoader = false;

        });
    }
    
    $scope.Get_Refurbishment_SKU_ALL = function () {
        debugger

            $scope.showLoader = true;

        ReceiveFromOthersServices.Get_Refurbishment_SKU_ALL().then(function (data) {
            $scope.gridRefurbishment.data = data.data;
            $('#myModal2').modal('toggle');

                $scope.showLoader = false;
            }, function (error) {

                alert(error);

                $scope.showLoader = false;

            });
        

    }
    $scope.Ref_SKU_Load = function (entity) {
        $scope.model.CHALLAN_NO = entity.CHALLAN_NO;
        $scope.model.SKU_ID = entity.SKU_ID;
        $scope.model.SKU_NAME = entity.SKU_NAME;
        $scope.model.SKU_CODE = entity.SKU_CODE;

        $scope.model.RECEIVE_QTY = entity.PROD_QTY;
        $scope.SetProductInfo($scope.model.SKU_ID)
    }

    $scope.Get_Refurbishment_SKU = function () {
        debugger

        if ($scope.model.CHALLAN_NO.length > 10) {
            $scope.showLoader = true;

            ReceiveFromOthersServices.Get_Refurbishment_SKU($scope.model.CHALLAN_NO).then(function (data) {
                $scope.products = [];
                for (var i = 0; i < data.data.length; i++) {
                    var index = $scope.products_backup.findIndex(x => x.SKU_CODE == data.data[i].PRODUCT_CODE);
                    if (index > -1) {
                        $scope.products.push($scope.products_backup[index]);
                    } 
                }
                if ($scope.products.length == 0 && $scope.model.RECEIVE_TYPE == "Refubrishment") {
                    notificationservice.Notification('No Specific data found for refurbishment', 1, '');

                }
                if ($scope.products.length > 0 && $scope.model.RECEIVE_TYPE == "Refubrishment") {
                    notificationservice.Notification('1', 1, 'Refubrishment SKU Loaded!');

                }
                $scope.showLoader = false;
            }, function (error) {

                alert(error);

                $scope.showLoader = false;

            });
        }
        
    }
    
    $scope.OnReceiveTypeChange = function () {
        if ($scope.model.RECEIVE_TYPE == 'Toll' || $scope.model.RECEIVE_TYPE == 'Refubrishment') {
            $scope.IsBatchActive = false;
        } else {
            $scope.IsBatchActive = true;
            $scope.model.BATCH_NO = "";
        }
    }


    $scope.ApprovedList = function () {
        ReceiveFromOthersServices.GetApprovedList($scope.model.COMPANY_ID).then(response => {
            $scope.gridOptionsListUnchecked.data = response.data;
        })
    }

    $scope.LoadUsersByCompanyId = function (companyId) {


        ReceiveFromOthersServices.LoadUsersByCompanyId(companyId).then(function (data) {

            if (data.data.length > 0) {
                $scope.User_data = data.data.filter((x) => x.USER_ID == $scope.model.RECEIVED_BY_ID);
                $scope.model.RECEIVED_BY_NAME = $scope.User_data[0]?.USER_NAME;
                $scope.User_data = [];
                $scope.User_data = data.data.filter((x) => x.USER_ID == $scope.model.CHECKED_BY_ID);
                $scope.model.CHECKED_BY_NAME = $scope.User_data[0]?.USER_NAME;

            } else {
                $scope.model.RECEIVED_BY_NAME = "Not Received Yet";
                $scope.model.RECEIVE_AMOUNT = "Not Checked Yet";
            }
            $scope.showLoader = false;
        }, function (error) {

            alert(error);

            $scope.showLoader = false;

        });

    }

    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        ReceiveFromOthersServices.GetCompany().then(function (data) {
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

        ReceiveFromOthersServices.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }
    $scope.ReceiveStockType = function () {
        var Finished_Product = {
            RECEIVE_STOCK_TYPE: 'P'
        }


        $scope.Status.push(Finished_Product);

    }
    $scope.LoadSupplier = function (type) {
        $scope.showLoader = true;
        $scope.OnReceiveTypeChange();

        ReceiveFromOthersServices.GetSupplierByType(type).then(function (data) {
            $scope.suppliers = data.data;
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;
        })
    }

    $scope.CheckReceiveType = function () {

        if ($scope.model.RECEIVE_TYPE == '' || $scope.model.RECEIVE_TYPE == null) {
            notificationservice.Notification('Please select Receive From', 1, '');
        }
    }
    $scope.LoadProducts = function () {
        $scope.showLoader = true;
        ReceiveFromOthersServices.GetProducts().then(function (data) {
            $scope.products = data.data;
            $scope.products_backup = data.data;
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;
        })
    }

    $scope.SetProductInfo = function (id) {
        let product = $scope.products.find(e => e.SKU_ID == id);

        $scope.model.SKU_CODE = product.SKU_CODE;
        $scope.model.PACK_SIZE = product.PACK_SIZE;
        //$scope.model.UNIT_TP = product.UNIT_TP;
        $scope.model.SHIPPER_QTY = product.SHIPPER_QTY;

        //ReceiveFromOthersServices.GetUnitWiseSkuPrice(product.SKU_ID, product.SKU_CODE)
        //    .then(function (priceData) {
        //        $scope.model.UNIT_TP = priceData.data;
        //    });

        //ReceiveFromOthersServices.GetBatches($scope.model.SKU_ID, $scope.model.RECEIVE_TYPE).then(response => {
        //    $scope.batches = response.data;
        //    //$('#BATCH_NO').trigger();
        //})
    }

    $scope.ChangeReceiveAmount = function () {

        if ($scope.model.SKU_ID == undefined || $scope.model.SKU_ID == 0 || $scope.model.SKU_ID == null) {

            notificationservice.Notification('Please select SKU Name before quantity', 1, '');
            $scope.model.RECEIVE_QTY = 0;
        }
        else {
            $scope.model.RECEIVE_AMOUNT = $scope.model.UNIT_TP * $scope.model.RECEIVE_QTY;
        }
    }

    $scope.AutoCompleteDataLoadForBatch = function (value) {
        var batch = $scope.batches.filter(e => e.BATCH_NO.includes(value)).slice(0, 15);
        return batch;
    }

    $scope.ChangeBatchNo = function (batch) {
        var batch = $scope.batches.find(e => e.BATCH_NO == batch.BATCH_NO)
        $scope.model.BATCH_NO = batch.BATCH_NO;
        if (batch.UNIT_TP != null)
            $scope.model.UNIT_TP = batch?.UNIT_TP;
        if (batch?.MRP != null)
            $scope.model.MRP = batch?.MRP;
    }

    $scope.LoadStatus = function () {
        var Active = {
            RECEIVE_STATUS: 'Active'
        }
        var Complete = {
            RECEIVE_STATUS: 'Complete'
        }
        $scope.Status.push(Active);
        $scope.Status.push(Complete);
    }
    $scope.ClearForm = function () {
        window.location.href = "/Inventory/FGReceiveFromOthers/ReceiveFromOthers";
    }
    $scope.LoadListPage = function () {
        window.location.href = "/Inventory/FGReceiveFromOthers/ReceiveFromOthersList";
    }

    $scope.LoadUnitData = function () {
        $scope.showLoader = true;

        ReceiveFromOthersServices.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            //
            $scope.Unit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID && element.UNIT_TYPE == 'Factory' });;
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }

    $scope.GetEditDataById = function (value, ischecked = false) {
        if (value != undefined && value.length > 0 && value != "0") {
            ReceiveFromOthersServices.GetEditDataById(value).then(function (data) {
                if (data.data != null && data.data.length > 0) {
                    $scope.showLoader = true;
                    let uName = $scope.model.UNIT_NAME
                    $scope.model = data.data[0];
                    $scope.model.UNIT_NAME = uName;
                    var entity = data.data;
                    $scope.model.RECEIVE_ID = entity[0].RECEIVE_ID;
                    $scope.model.IsChecked = ischecked;
                    $scope.model.BATCH_NO = entity[0].BATCH_NO;
                    $scope.model.SKU_ID = entity[0].SKU_ID.toString();
                    $scope.model.COMPANY_ID = entity[0].COMPANY_ID;
                    $scope.model.SUPPLIER_ID = entity[0].SUPPLIER_ID;
                    $scope.model.SKU_NAME = entity[0].SKU_NAME;
                    $scope.model.RECEIVE_STOCK_TYPE = entity[0].RECEIVE_STOCK_TYPE;
                    $scope.model.RECEIVED_BY_ID = entity[0].RECEIVED_BY_ID;

                    $scope.model.SKU_CODE = entity[0].SKU_CODE;
                    $scope.model.PACK_SIZE = entity[0].PACK_SIZE;
                    $scope.model.RECEIVE_QTY = entity[0].RECEIVE_QTY;
                    $scope.model.SHIPPER_QTY = entity[0].SHIPPER_QTY;
                    $scope.model.EXPIRY_DATE = $scope.formatDate(entity[0].EXPIRY_DATE);
                    $scope.model.MFG_DATE = $scope.formatDate(entity[0].MFG_DATE);
                    $scope.model.RECEIVE_DATE = $scope.formatDate(entity[0].RECEIVE_DATE);
                    $scope.model.CHALLAN_DATE = $scope.formatDate(entity[0].CHALLAN_DATE);
                    $scope.model.REMARKS = entity[0].REMARKS;
                    $scope.model.SUPPLIER_ID = entity[0].SUPPLIER_ID;

                    $scope.LoadSupplier(entity[0].RECEIVE_TYPE);
                    $scope.TriggerSelects();

                    //$scope.LoadProductByProductCode($scope.model.COMPANY_ID, $scope.model.SKU_CODE);
                    //$scope.LoadProductPriceByProductCode($scope.model.COMPANY_ID, $scope.model.SKU_CODE);
                    $scope.LoadUsersByCompanyId($scope.model.COMPANY_ID);
                    if (ischecked) {
                        $scope.model.CHECKED_BY_DATE = $scope.formatDate(entity[0].CHECKED_BY_DATE);
                        $scope.LoadCheckedById();
                    }
                    ReceiveFromOthersServices.GetCompany().then(function (data) {
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
                $scope.showLoader = false;
            }, function (error) {
                alert(error);

            });
        }
    }
    $scope.TriggerSelects = function () {
        setTimeout(function () {
            $("#SKU_ID").trigger("change");
            //$("#UNIT_ID").trigger("change");
            $("#SUPPLIER_ID").trigger("change");
        }, 1000)
    }
    //$scope.GetEditDataById('5uRNG/8ZTv6IeJWdzuB5faB/IWjamuAGg9rzM82SlWbYJzc8QJa0xbd4+o6ocg66');

    $scope.LoadCheckedById = function () {
        var userName = $("#userName");
        $('#CHECKED_BY_NAME').val(userName.html());
    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        //
        $scope.permissionReqModel = {
            Controller_Name: 'FGReceiveFromOthers',
            Action_Name: 'ReceiveFromOthers'
        }
        permissionProvider.GetPermission($scope.permissionReqModel).then(function (data) {
            //
            $scope.getPermissions = data.data;
            $scope.model.ADD_PERMISSION = $scope.getPermissions.adD_PERMISSION;
            $scope.model.EDIT_PERMISSION = $scope.getPermissions.ediT_PERMISSION;
            $scope.model.DELETE_PERMISSION = $scope.getPermissions.deletE_PERMISSION;
            $scope.model.LIST_VIEW = $scope.getPermissions.lisT_VIEW;
            $scope.model.DETAIL_VIEW = $scope.getPermissions.detaiL_VIEW;
            $scope.model.DOWNLOAD_PERMISSION = $scope.getPermissions.downloaD_PERMISSION;
            $scope.model.USER_TYPE = $scope.getPermissions.useR_TYPE;
            $scope.model.CONFIRM_PERMISSION = $scope.getPermissions.confirM_PERMISSION;

            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }

    $scope.GetPermissionData();
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
    $scope.LoadUnitData();
    $scope.LoadStatus();
    //$scope.LoadSupplier();
    $scope.LoadProducts();
    // This Method work is Edit Data Loading

    $scope.UnitLoad = function () {
        ReceiveFromOthersServices.GetUnit().then(function (data) {

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
    $scope.UnitsLoad = function () {
        $scope.showLoader = true;

        ReceiveFromOthersServices.GetUnitList().then(function (data) {

            $scope.Units = data.data.filter(x => x.COMPANY_ID == $scope.model.COMPANY_ID);
            $scope.UnitLoad();
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }

    $scope.CheckDates = () => {
        let dateParts = $scope.model.MFG_DATE.split("/");
        let startDate = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
        dateParts = $scope.model.EXPIRY_DATE.split("/");
        let endDate = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
        if (startDate > endDate) {
            return false;
        }
        return true;
    }
    
    $scope.CheckMrp = function () {
        ReceiveFromOthersServices.GetLastMrp($scope.model.SKU_CODE).then(response => {
            var lastMrp = response.data[0];
            if (lastMrp?.MRP != null && lastMrp?.MRP != $scope.model.MRP) {
                alert("Previous MRP is not same! It was " + lastMrp?.MRP);
                $scope.model.BATCH_PRICE_REVIEW_STATUS = "Yes";
                if ($scope.model.UNIT_TP == null || $scope.model.UNIT_TP == 0 || $scope.model.UNIT_TP == '') {
                    $scope.model.UNIT_TP = lastMrp.UNIT_TP
                }
            }
            else {
                $scope.model.BATCH_PRICE_REVIEW_STATUS = "No";
            }
        })
    }
    $scope.UpdateStatusToCancel = function (entity) {
        ReceiveFromOthersServices.UpdateStatusToCancel(JSON.stringify(entity.RECEIVE_ID)).then(response => {
            notificationservice.Notification(1, 1, response.data.status);
        })
    }

    $scope.UnitsLoad();
    $scope.UnitLoad();
    $scope.SaveData = function (model) {
        $scope.showLoader = true;

        if ($scope.model.EXPIRY_DATE != undefined && $scope.model.EXPIRY_DATE != null && $scope.model.EXPIRY_DATE != "") {
            if (!$scope.CheckDates()) {
                notificationservice.Notification('Expiry date cannot be later than MFG date!!', 1, '');
                $scope.showLoader = false;
                return;
            }
        }
        else {
            if ($scope.model.RECEIVE_TYPE != 'Toll') {
                notificationservice.Notification('Expiry Date Required!!', 1, '');

            }
        }

        if (model.RECEIVE_TYPE != 'Supplier' && model.RECEIVE_TYPE != 'Refubrishment' && model.RECEIVE_TYPE != 'Toll' && (model.BATCH_NO == '' || model.BATCH_NO == null)) {
            notificationservice.Notification('Please Select Batch!!', 1, '');
            $scope.showLoader = false;
            return;
        }

        model.UNIT_ID = parseInt(model.UNIT_ID);
        model.CHECKED_BY_ID = model.CHECKED_BY_ID == null || model.CHECKED_BY_ID == NaN || model.CHECKED_BY_ID == undefined ? 0 : parseInt(model.CHECKED_BY_ID);
        model.SHIPPER_QTY = model.SHIPPER_QTY == null || model.SHIPPER_QTY == NaN || model.SHIPPER_QTY == undefined ? 0 : parseInt(model.SHIPPER_QTY);
        model.BATCH_ID = model.BATCH_ID == null || model.BATCH_ID == NaN || model.BATCH_ID == undefined ? 0 : parseInt(model.BATCH_ID);
        model.SKU_ID = model.SKU_ID == null || model.SKU_ID == NaN || model.SKU_ID == undefined ? 0 : parseInt(model.SKU_ID);
        model.RECEIVE_QTY = model.RECEIVE_QTY == null || model.RECEIVE_QTY == NaN || model.RECEIVE_QTY == undefined ? 0 : parseInt(model.RECEIVE_QTY);
        model.RECEIVE_AMOUNT = model.RECEIVE_AMOUNT == null || model.RECEIVE_AMOUNT == NaN || model.RECEIVE_AMOUNT == undefined ? 0 : parseInt(model.RECEIVE_AMOUNT);
        model.RECEIVED_BY_ID = model.RECEIVED_BY_ID == null || model.RECEIVED_BY_ID == NaN || model.RECEIVED_BY_ID == undefined ? 0 : parseInt(model.RECEIVED_BY_ID);
        model.SUPPLIER_ID = model.SUPPLIER_ID == null || model.SUPPLIER_ID == NaN || model.SUPPLIER_ID == undefined ? 0 : parseInt(model.SUPPLIER_ID);
        if ($scope.model.IsChecked) {
            if (!window.confirm("Are you Sure? This cannot be edited after confirm")) {
                $scope.showLoader = false;
                return;
            }
        }

        delete model.SKU_NAME;
        delete model.ROW_NO;

        

        ReceiveFromOthersServices.AddOrUpdate(model).then(function (data) {
            notificationservice.Notification(data.data.status, 1, 'Data Save Successfully !!');
            debugger
            if (data.data.status == 1) {
                //ReceiveFromOthersServices.GetEditDataById(data.data.key).then(function (response) {
                //    if ($scope.model.RECEIVE_TYPE == 'Supplier' || $scope.model.RECEIVE_TYPE == 'Toll') {
                //        $scope.model.BATCH_NO = response.data[0].BATCH_NO;
                //    }

                //    $scope.model.MRP = response.data[0].MRP;
                //    $scope.model.UNIT_TP = response.data[0].UNIT_TP;
                //    $scope.model.RECEIVE_ID = response.data[0].RECEIVE_ID;
                //    $scope.TriggerSelects();

                //})
                $scope.showLoader = false;
                setTimeout(function () {
                    $scope.ClearForm();
                }, 1000)
                
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

    $scope.formatRepoProduct = function (repo) {

        if (repo.loading) {
            return repo.text;
        }
        if (repo.text != "") {
            const textArray = repo.text.split("--");
            let text_title = textArray[0];
            let text_title_2 = textArray[1];
            let text_title_3 = textArray[2];

            var $container = $(
                "<div class='select2-result-repository clearfix'>" +
                "<div class='select2-result-repository__meta'>" +
                "<div class='select2-result-repository__title' style='font-size:14px;font-weight:700'></div>" +
                "<div class='select2-result-repository__watchers' style='font-size:12px;font-weight:700'> <span>Code: </span>  </div>" +
                "<div class='select2-result-repository__watchers_2' style='font-size:12px;font-weight:700'> <span>Pack Size: </span>  </div>" +
                "</div>" +
                "</div>"
            );

            $container.find(".select2-result-repository__title").text(text_title);
            $container.find(".select2-result-repository__watchers").append(text_title_2);
            $container.find(".select2-result-repository__watchers_2").append(text_title_3);


        }

        return $container;
    }

    $scope.formatRepoSelectionProduct = function (repo) {
        return repo.text.split("--")[0];
    }

    $(".select2-single-Product").select2({
        placeholder: "Select",
        templateResult: $scope.formatRepoProduct,
        templateSelection: $scope.formatRepoSelectionProduct
    });
}]);

