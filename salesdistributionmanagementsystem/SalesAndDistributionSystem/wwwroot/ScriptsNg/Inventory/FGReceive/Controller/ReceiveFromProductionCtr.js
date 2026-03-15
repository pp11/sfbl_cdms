ngApp.controller('ngGridCtrl', ['$scope', 'ReceiveFromProductionServices', 'permissionProvider', 'gridregistrationservice', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, ReceiveFromProductionServices, permissionProvider, gridregistrationservice, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'

    const todate = new Date();

    $scope.model = {
        COMPANY_ID: 0, UNIT_ID: 0, RECEIVE_ID: 0,
        UNIT_ID: 0,
        TRANSFER_TYPE: '', BATCH_ID: 0, BATCH_NO: '',
        SKU_ID: 0,
        SKU_CODE: '', PACK_SIZE: '', UNIT_TP: '',
        RECEIVE_QTY: 0, SHIPPER_QTY: 0, RECEIVE_AMOUNT: 0,
        RECEIVE_STATUS: '', RECEIVE_STOCK_TYPE: '',
        REMARKS: ''
    }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Units = [];
    $scope.TransferTypes = [];
    $scope.ReceiveStockTypes = [];
    $scope.User_data = [];
    $scope.Status = [];
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Customer Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }

    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'TRANSFER_ID', field: 'TRANSFER_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , { name: 'UNIT_ID', field: 'UNIT_ID', visible: false }
        , { name: 'TRANSFER_DATE', field: 'TRANSFER_DATE', visible: false }
        , { name: 'PRD_PRC_MST_SLNO', field: 'PRD_PRC_MST_SLNO', visible: false }
        , { name: 'P_PRODUCT_CODE', field: 'P_PRODUCT_CODE', visible: false }
        , { name: 'MFG_DATE', field: 'MFG_DATE', visible: false }
        , { name: 'EXPIRE_DATE', field: 'EXPIRE_DATE', visible: false }
        , { name: 'PACK_SIZE', field: 'PACK_SIZE', visible: false }
        , { name: 'BATCH_STATUS', field: 'BATCH_STATUS', visible: false }
        , { name: 'TRANSFER_STATUS', field: 'TRANSFER_STATUS', visible: false }
        , { name: 'ISSUED_BY', field: 'ISSUED_BY', visible: false }
        , { name: 'EQUIVALENT_QTY', field: 'EQUIVALENT_QTY', visible: false }
        , { name: 'NO_SHIPPER_CTN', field: 'NO_SHIPPER_CTN', visible: false }
        , { name: 'QTY_PER_SHIPPER', field: 'QTY_PER_SHIPPER', visible: false }
        , { name: 'LOOSE_QTY', field: 'LOOSE_QTY', visible: false }
        , { name: 'REMARKS', field: 'REMARKS', visible: false }
        , { name: 'REPACK_FLAG', field: 'REPACK_FLAG', visible: false }
        , { name: 'REMARKS', field: 'REMARKS', visible: false }
        , { name: 'UNIT_CODE', field: 'UNIT_CODE', visible: false }
        , { name: 'TEST_REQUEST_NO', field: 'TEST_REQUEST_NO', visible: false }
        , {
            name: 'TRANSFER_NOTE_NO', field: 'TRANSFER_NOTE_NO', displayName: 'Transfer Note No', enableFiltering: true, width: '10%'
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: true, width: '10%'
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '25%'
        }
        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '10%'
        }
        , {
            name: 'MRP', field: 'MRP', displayName: 'MRP', enableFiltering: true, width: '10%'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: true, width: '10%'
        }
        , {
            name: 'TRANSFER_QTY', field: 'TRANSFER_QTY', displayName: 'Transfer Qty', enableFiltering: true, width: '10%'
        }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;"  data-dismiss="modal" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.SelectTransferData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Select</button>' +
                '</div>'
        },

    ];

    $scope.gridOptionsList.rowTemplate = "<div ng-dblclick=\"grid.appScope.SelectTransferData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

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
            name: 'TRANSFER_NOTE_NO', field: 'TRANSFER_NOTE_NO', displayName: 'Transfer Note No', enableFiltering: true, width: '20%'
        }
        , {
            name: 'TRANSFER_DATE', field: 'TRANSFER_DATE', displayName: 'Date', enableFiltering: true, width: '12%'
        }
        , {
            name: 'RECEIVE_DATE', field: 'RECEIVE_DATE', displayName: 'Receive Date', enableFiltering: true, width: '13%'
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: true, width: '13%'
        }
        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '13%'
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '13%'
        }
        , {
            name: 'RECEIVE_QTY', field: 'RECEIVE_QTY', displayName: 'Receive Qty', enableFiltering: true, width: '13%'
        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, visible: false, width: '13%'
        }
        , { name: 'DIVISION_REGION_MST_STATUS', field: 'DIVISION_REGION_MST_STATUS', visible: false, displayName: 'Status', enableFiltering: true, width: '15%' }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;"   ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];

    $scope.gridOptionsListUnchecked.rowTemplate = "<div  ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.EditData = function (entity) {
        $scope.GetEditDataById(entity.RECEIVE_ID_ENCRYPTED, "True");
        $scope.CompanyLoad();
        $scope.CompanyNameLoad();
        $('#myModal1').modal('toggle');
    }

    $scope.DataLoadUnchecked = function () {
        $scope.showLoader = true;

        ReceiveFromProductionServices.LoadUnchekckData($scope.model.COMPANY_ID).then(function (data) {
            $scope.gridOptionsListUnchecked.data = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;
        });
    }
    $scope.SelectTransferData = function (entity) {
        $scope.showLoader = true;
        $scope.model.TRANSFER_NOTE_NO = entity.TRANSFER_NOTE_NO;
        $scope.model.TRANSFER_ID = entity.TRANSFER_ID;

        $scope.model.BATCH_NO = entity.BATCH_NO;
        $scope.model.SKU_NAME = entity.SKU_NAME;
        $scope.model.RECEIVE_STOCK_TYPE = 'Production';
        $scope.model.TRANSFER_TYPE = 'Production';

        $scope.model.SKU_CODE = entity.SKU_CODE;
        $scope.model.PACK_SIZE = entity.PACK_SIZE;
        $scope.model.RECEIVE_QTY = entity.TRANSFER_QTY;
        $scope.model.SHIPPER_QTY = entity.SHIPPER_QTY;
        $scope.model.EXPIRY_DATE = $scope.formatDate(entity.EXPIRE_DATE);
        $scope.model.MFG_DATE = $scope.formatDate(entity.MFG_DATE);
        $scope.model.RECEIVE_DATE = $scope.formatDate(new Date());
        $scope.model.PACK_SIZE = entity.PACK_SIZE;
        $scope.model.TRANSFER_DATE = $scope.formatDate(entity.TRANSFER_DATE);
        $scope.model.REMARKS = "";
        $scope.model.MRP = entity.MRP;
        $scope.model.UNIT_TP = entity.UNIT_TP;

        $scope.LoadProductByProductCode($scope.model.COMPANY_ID, $scope.model.SKU_CODE);

        //$scope.LoadProductPriceByProductCode($scope.model.COMPANY_ID, $scope.model.SKU_CODE);
        $('#exampleModalScrollable').modal('toggle');

        ReceiveFromProductionServices.GetLastMrp(entity.SKU_CODE).then(response => {
            var lastMrp = response.data[0];
            
            if (lastMrp?.MRP != null && lastMrp?.MRP != entity.MRP) {
                alert("Previous MRP is not same!");
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
    $scope.LoadUsersByCompanyId = function (companyId) {
        ReceiveFromProductionServices.LoadUsersByCompanyId(companyId).then(function (data) {
            if (data.data.length > 0) {
                $scope.User_data = data.data.filter((x) => x.USER_ID == $scope.model.RECEIVED_BY_ID);
                $scope.model.RECEIVED_BY_NAME = $scope.User_data[0].USER_NAME;
                $scope.User_data = [];
                $scope.User_data = data.data.filter((x) => x.USER_ID == $scope.model.CHECKED_BY_ID);
                if ($scope.User_data.length > 0) {
                    $scope.model.CHECKED_BY_NAME = $scope.User_data[0].USER_NAME;
                }
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
    $scope.LoadProductPriceByProductCode = function (companyId, side_code) {
        ReceiveFromProductionServices.GetSearchableProductPrice(companyId, side_code).then(function (data) {
            if (data.data.length > 0) {
                $scope.model.UNIT_TP = data.data[0].UNIT_TP;
                $scope.model.RECEIVE_AMOUNT = parseFloat($scope.model.UNIT_TP) * parseFloat($scope.model.RECEIVE_QTY);
            } else {
                $scope.model.UNIT_TP = 0;
                $scope.model.RECEIVE_AMOUNT = 0;
            }
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;
        });
    }
    $scope.LoadProductByProductCode = function (companyId, side_code) {
        ReceiveFromProductionServices.LoadProductByProductCode(companyId, side_code).then(function (data) {
            $scope.model.SKU_ID = data.data[0].SKU_ID;
            $scope.model.SKU_NAME = data.data[0].SKU_NAME;
            $scope.model.SHIPPER_QTY = data.data[0].SHIPPER_QTY;

            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;
        });
    }

    $scope.DataLoad = function (companyId) {
        $scope.showLoader = true;

        ReceiveFromProductionServices.LoadData(companyId).then(function (data) {
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = companyId;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;
        });
    }
    $scope.CompanyLoad = function () {
        ReceiveFromProductionServices.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);

            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.CompanyNameLoad = function () {
        ReceiveFromProductionServices.GetCompanyName().then(function (data) {
            $scope.model.COMPANY_NAME = data.data;

            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.ReceiveStockType = function () {
        var Finished_Product = {
            RECEIVE_STOCK_TYPE: 'Production'
        }

        $scope.Status.push(Finished_Product);
    }

    $scope.UnitLoad = function () {
        ReceiveFromProductionServices.GetUnit().then(function (data) {
            $scope.model.UNIT_ID = data.data.toString();
            //$scope.model.ORDER_UNIT_ID = parseInt(data.data);

            for (var i = 0; i < $scope.Units.length; i++) {
                if ($scope.model.UNIT_ID == $scope.Units[i].UNIT_ID) {
                    $scope.model.UNIT_NAME = $scope.Units[i].UNIT_NAME;
                }
            }
            $interval(function () {
                $('#UNIT_ID').trigger('change');
            }, 800, 4);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.UnitLoad();
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
        window.location.href = "/Inventory/FGReceive/ReceiveFromProduction";
    }
    $scope.LoadListPage = function () {
        window.location.href = "/Inventory/FGReceive/ReceiveFromProductionList";
    }
    $scope.LoadFormData = function () {
        $scope.showLoader = true;
        ReceiveFromProductionServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = $scope.model.COMPANY_ID;
            for (var i in $scope.gridOptionsList.data) {
                if ($scope.gridOptionsList.data[i].DIVISION_NAME == $scope.model.DIVISION_NAME) {
                    $scope.model.DIVISION_ID = $scope.gridOptionsList.data[i].DIVISION_ID;
                    $scope.model.DIVISION_NAME = $scope.gridOptionsList.data[i].DIVISION_NAME;
                    $scope.model.DIVISION_CODE = $scope.gridOptionsList.data[i].DIVISION_CODE;
                    $scope.model.DIVISION_ADDRESS = $scope.gridOptionsList.data[i].DIVISION_ADDRESS;

                    $scope.model.DIVISION_STATUS = $scope.gridOptionsList.data[i].DIVISION_STATUS;
                    $scope.model.REMARKS = $scope.gridOptionsList.data[i].REMARKS;
                }
            }
        }, function (error) {
            alert(error);

            $scope.showLoader = false;
        });
    }
    $scope.LoadUnitData = function () {
        $scope.showLoader = true;
       
        $scope.Units = [{
            UNIT_NAME: 'Select One',
            UNIT_ID: 0
        }];
        ReceiveFromProductionServices.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            var units = data.data.filter(function (element) {
                return element.COMPANY_ID == $scope.model.COMPANY_ID && element.UNIT_TYPE == 'Factory'
            });
            $scope.Units = [...$scope.Units, ...units];

            if (!$scope.Units.some(e => e.UNIT_ID == $scope.model.UNIT_ID)) {
                $scope.model.UNIT_ID = ($scope.Units[0]?.UNIT_ID)?.toString();
            }

            $interval(function () {
                $('#UNIT_ID').trigger('change');
            }, 800, 4);

            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.LoadFGTransferData = function () {
        $scope.showLoader = true;
        if ($scope.model.COMPANY_ID > 0 && $scope.model.UNIT_ID > 0 && $scope.Units.some(e => e.UNIT_ID == $scope.model.UNIT_ID)) {
            ReceiveFromProductionServices.LoadFGTransferData($scope.model.COMPANY_ID, $scope.model.UNIT_ID).then(function (data) {
                $scope.gridOptionsList.data = data.data;
                $('#exampleModalScrollable').modal('toggle');
                $scope.showLoader = false;
            }, function (error) {
                $scope.showLoader = false;
            });
        }
        else {
            $scope.showLoader = false;

            notificationservice.Notification('Please select Unit/Factory First to receive finished goods', 1, '');
        }
    }
    $scope.GetEditDataById = function (value, ischecked = "False") {
        if (value != undefined && value.length > 0) {
            ReceiveFromProductionServices.GetEditDataById(value).then(function (data) {
                if (data.data != null && data.data.length > 0) {
                    //var entity = data.data;
                    $scope.showLoader = true;
                    $scope.model = data.data[0];
                    var entity = data.data;
                    $scope.CompanyLoad();
                    $scope.CompanyNameLoad();
                    $scope.model.TRANSFER_NOTE_NO = entity[0].TRANSFER_NOTE_NO;
                    $scope.model.TRANSFER_ID = entity[0].TRANSFER_ID;
                    $scope.model.RECEIVE_ID = entity[0].RECEIVE_ID;
                    $scope.model.IsChecked = ischecked;

                    $scope.model.BATCH_NO = entity[0].BATCH_NO;
                    $scope.model.SKU_NAME = entity[0].SKU_NAME;
                    $scope.model.RECEIVE_STOCK_TYPE = entity[0].RECEIVE_STOCK_TYPE;
                    $scope.model.TRANSFER_TYPE = entity[0].TRANSFER_TYPE;
                    $scope.model.RECEIVED_BY_ID = entity[0].RECEIVED_BY_ID;

                    $scope.model.SKU_CODE = entity[0].SKU_CODE;
                    $scope.model.PACK_SIZE = entity[0].PACK_SIZE;
                    $scope.model.RECEIVE_QTY = entity[0].RECEIVE_QTY;
                    $scope.model.SHIPPER_QTY = entity[0].SHIPPER_QTY;
                    $scope.model.EXPIRY_DATE = $scope.formatDate(entity[0].EXPIRY_DATE);
                    $scope.model.MFG_DATE = $scope.formatDate(entity[0].MFG_DATE);
                    $scope.model.RECEIVE_DATE = $scope.formatDate(entity[0].RECEIVE_DATE);
                    $scope.model.TRANSFER_DATE = $scope.formatDate(entity[0].TRANSFER_DATE);
                    $scope.model.REMARKS = entity[0].REMARKS;
                    $scope.model.MRP = entity[0].MRP;
                    $scope.model.UNIT_TP = entity[0].UNIT_TP;
                   
                    $scope.model.CHECKED_BY_DATE = $scope.formatDate(entity[0].CHECKED_BY_DATE);
                    if (ischecked) {
                        $scope.LoadCheckedById();
                    }
                    $scope.LoadProductByProductCode($scope.model.COMPANY_ID, $scope.model.SKU_CODE);

                    //$scope.LoadProductPriceByProductCode($scope.model.COMPANY_ID, $scope.model.SKU_CODE);
                    $scope.LoadUsersByCompanyId($scope.model.COMPANY_ID);
                }
                $scope.showLoader = false;
            }, function (error) {
                alert(error);
            });
        }
    }
    //$scope.GetEditDataById('5uRNG/8ZTv6IeJWdzuB5faB/IWjamuAGg9rzM82SlWbYJzc8QJa0xbd4+o6ocg66');
    $scope.LoadCheckedById = function () {
        var userName = $("#userName");
        $('#CHECKED_BY_NAME').val(userName.html());
    }
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;

        $scope.permissionReqModel = {
            Controller_Name: 'FGReceive',
            Action_Name: 'ReceiveFromProduction'
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
    $scope.CompanyLoad();
    $scope.CompanyNameLoad();

    $scope.LoadUnitData();
    $scope.LoadStatus();

    // This Method work is Edit Data Loading

    $scope.SaveData = function (model) {
        $scope.showLoader = true;
        model.UNIT_ID = parseInt(model.UNIT_ID);
        model.CHECKED_BY_ID = model.CHECKED_BY_ID == null || model.CHECKED_BY_ID == NaN || model.CHECKED_BY_ID == undefined ? 0 : parseInt(model.CHECKED_BY_ID);
        model.TRANSFER_ID = model.TRANSFER_ID == null || model.TRANSFER_ID == NaN || model.TRANSFER_ID == undefined ? 0 : parseInt(model.TRANSFER_ID);
        model.SHIPPER_QTY = model.SHIPPER_QTY == null || model.SHIPPER_QTY == NaN || model.SHIPPER_QTY == undefined ? 0 : parseInt(model.SHIPPER_QTY);
        model.BATCH_ID = model.BATCH_ID == null || model.BATCH_ID == NaN || model.BATCH_ID == undefined ? 0 : parseInt(model.BATCH_ID);
        model.SKU_ID = model.SKU_ID == null || model.SKU_ID == NaN || model.SKU_ID == undefined ? 0 : parseInt(model.SKU_ID);
        model.RECEIVE_QTY = model.RECEIVE_QTY == null || model.RECEIVE_QTY == NaN || model.RECEIVE_QTY == undefined ? 0 : parseInt(model.RECEIVE_QTY);
        model.RECEIVED_BY_ID = model.RECEIVED_BY_ID == null || model.RECEIVED_BY_ID == NaN || model.RECEIVED_BY_ID == undefined ? 0 : parseInt(model.RECEIVED_BY_ID);
        model.REMARKS = model.REMARKS == null || model.REMARKS == NaN || model.REMARKS == undefined ? '' : model.REMARKS;
        model.TRANSFER_QTY = model.RECEIVE_QTY;
        model.TRANSFER_NOTE_NO = JSON.stringify(parseInt(model.TRANSFER_NOTE_NO));
        ReceiveFromProductionServices.AddOrUpdate(model).then(function (data) {
            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.GetPermissionData();
                $scope.CompanyLoad();
                $scope.CompanyNameLoad();
                //    $scope.LoadFormData();
                $scope.model.UNIT_ID = JSON.stringify(parseInt($scope.model.UNIT_ID));
            }
            else {
                $scope.showLoader = false;
            }
        });
    }
}]);