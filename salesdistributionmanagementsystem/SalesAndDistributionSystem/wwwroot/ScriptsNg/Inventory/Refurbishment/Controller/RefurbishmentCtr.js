ngApp.controller('ngGridCtrl', ['$scope', 'refurbishmentServices', 'uiGridConstants', 'permissionProvider', 'gridregistrationservice', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, refurbishmentServices, uiGridConstants, permissionProvider, gridregistrationservice, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'

    //-------variaable Declare------------------
    $scope.showloader = false;
    $scope.saveBtnName = "Save";
    $scope.isSaveBtnVisible = true;
    $scope.isAutomatic = true;
    $scope.DATE_FROM = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.DATE_TO = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.model = { MST_SLNO: 0, CUSTOMER_TYPE: 'Automation', APPROVED_STATUS: 'P', Details: [] }
    $scope.model.RECEIVE_DATE = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.distList = [];
    //$scope.manualDistList = [];

    //-----------grid operation------------------
    $scope.gridOptions = (gridregistrationservice.GridRegistration("refurbishmentGrid"));
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptions.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50',
            cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                if (typeof row.entity.PARENT === 'string') {
                    return 'red';
                }
            }
        }
        , {
            name: 'DTL_SLNO', field: 'DTL_SLNO', displayName: 'DTL_SLNO', enableFiltering: false, width: '10%', enableColumnMenu: false, visible: false
        }
        , {
            name: 'MST_SLNO', field: 'MST_SLNO', displayName: 'MST_SLNO', enableFiltering: false, width: '10%', enableColumnMenu: false, visible: false
        }
        , {
            name: 'REFURBISHMENT_PRODUCT_STATUS', field: 'REFURBISHMENT_PRODUCT_STATUS', enableColumnMenus: false, displayName: 'Product Status*', enableFiltering: false, width: '13%', cellTemplate:
                '<select name="REFURBISHMENT_PRODUCT_STATUS" class="pl-sm form-control" ng-change="grid.appScope.onChangeGridRow(row.entity)"  ng-model="row.entity.REFURBISHMENT_PRODUCT_STATUS" required> <option value="Refurbishment">Refurbishment</option><option value="Cancel">Cancel</option></select > '
        }
        , {
            name: 'PRODUCT_CODE', field: 'PRODUCT_CODE', displayName: 'Product Code', enableFiltering: false, width: '12%', enableColumnMenu: false
            , cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                if (typeof row.entity.PARENT === 'string') {
                    return 'red';
                }
            }
        }
        , {
            name: 'PRODUCT_NAME', field: 'PRODUCT_NAME', displayName: 'Product Name', enableFiltering: false, width: '12%', enableColumnMenu: false
            , cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                if (typeof row.entity.PARENT === 'string') {
                    return 'red';
                }
            }
        }
        , {
            name: 'PACK_SIZE', field: 'PACK_SIZE', displayName: 'Pack Size', enableFiltering: false, width: '10%', enableColumnMenu: false
            , cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                if (typeof row.entity.PARENT === 'string') {
                    return 'red';
                }
            }
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: false, width: '10%', enableColumnMenu: false, cellTemplate:
                '<input type="text"  ng-model="row.entity.BATCH_NO"   class="pl-sm text-left" ng-readonly="grid.appScope.isAutomatic"/>'
        }
        , {
            name: 'CLAIM_QTY', field: 'CLAIM_QTY', displayName: 'Claim Qty', enableFiltering: false, width: '10%', cellTemplate:
                '<input type="number"  ng-model="row.entity.CLAIM_QTY"  min="0" step="any"   class="pl-sm text-right" ng-readonly="grid.appScope.isAutomatic"/>'
        }
        , {
            name: 'RECEIVED_QTY', field: 'RECEIVED_QTY', displayName: 'Received Qty', enableFiltering: false, width: '12%', cellTemplate:
                '<input type="number" ng-change="grid.appScope.onChangeGridRow(row.entity)"  ng-model="row.entity.RECEIVED_QTY"  min="0" step="any"   class="pl-sm text-right" />'
        }
        , {
            name: 'DISPUTE_QTY', field: 'DISPUTE_QTY', displayName: 'Dispute Qty', enableFiltering: false, width: '12%', cellTemplate:
                '<input type="number"  ng-model="row.entity.DISPUTE_QTY"  min="0" step="any"   class="pl-sm text-right" />'
        }
        , {
            name: 'TRADE_PRICE', field: 'TRADE_PRICE', displayName: 'Tread Price', enableFiltering: false, width: '12%', cellTemplate:
                '<input type="number"  ng-model="row.entity.TRADE_PRICE"  min="0" step="any"   class="pl-sm text-right"  ng-readonly="grid.appScope.isAutomatic"/>'
        }
        , {
            name: 'REVISED_PRICE', field: 'REVISED_PRICE', displayName: 'Revised Price', enableFiltering: false, width: '12%', cellTemplate:
                '<input type="number" ng-change="grid.appScope.onChangeGridRow(row.entity)" ng-model="row.entity.REVISED_PRICE"  min="0" step="any"   class="pl-sm text-right" />'
        }
        , {
            name: 'EXPIRY_DATE', field: 'EXPIRY_DATE', displayName: 'Expiry Date', enableFiltering: false, width: '12%', cellTemplate:
                '<div class="simple-date2">'
                + '<div class="input-group date">'
                + '<div class="input-group-prepend">'
                + '</div>'
                + '<input  type="text" datepicker class="form-control"  readonly  ng-model="row.entity.EXPIRY_DATE" placeholder="dd/mm/yyyy" id="EXPIRY_DATE">'
                + '</div>'
                + '</div>'
        }
        , {
            name: 'VALUE', field: 'VALUE', displayName: 'Value', enableFiltering: false, enableColumnMenu: false, width: '10%'
            , cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                if (typeof row.entity.PARENT === 'string') {
                    return 'red';
                }
            }
        }
        , {
            name: 'PARENT', field: 'PARENT', displayName: 'PARENT', visible: false, width: '10%'
        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'REMARKS', enableFiltering: false, enableColumnMenu: false, width: '10%', cellTemplate:
                '<input type="text"  ng-model="row.entity.REMARKS"   class="pl-sm" />'
        }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO >100000" ng-click="grid.appScope.addNewRowToInvoice(row.entity)" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-click="grid.appScope.removeItem(row.entity, grid.appScope.invoiceGridList.data)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +
                '</div>'
        }
    ]
    $scope.gridOptions.rowTemplate = `<div ng-style='row.entity.is_selected == true && grid.appScope.rowStyle' ng-dblclick=\"grid.appScope.addCancelRow(row.entity,col.field)\" title=\"Double click to Add Row \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>`;
    $scope.rowNumberGenerate = function () {
        for (let i = 0; i < $scope.gridOptions.data.length; i++) {
            $scope.gridOptions.data[i].ROW_NO = i + 1;
        }
    }
    $scope.removeItem = function (rowEntity) {
        if (confirm('Do you really want to remove this row? It cannot be undone !') == true) {
            var index = $scope.gridOptions.data.indexOf(rowEntity);
            if (index !== -1) {
                $scope.gridOptions.data.splice(index, 1);
            }
        }
        $scope.calculateTotal();
    };
    //----------Master Data Grid operation--------
    $scope.gridList = (gridregistrationservice.GridRegistration("refurbishment Mst Data"));
    $scope.gridList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridList.columnDefs = [
        { name: '#', field: 'ROW_NO', enableFiltering: false, width: '2%' },
        {
            name: 'Print', displayName: 'Print', width: '73', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;margin-left:5px" ng-click="grid.appScope.GetPdfView(row.entity)" type="button" class="btn btn-outline-success mb-1"><i class="fa fa-print"></i>  Print</button>' +
                '</div>'
        },
        { name: 'MST_SLNO', displayName: "MST_SLNO", visible: false, width: '50' },
        { name: 'APPROVED_MGS', displayName: "Approved Status", visible: true, width: '120' },
        { name: 'CLAIM_NO', displayName: "Claim No", visible: true, width: '12%' },
        { name: 'RECEIVE_DATE', displayName: "Receive Date", visible: true, width: '110' },
        { name: 'RECEIVE_SHIFT', displayName: "Receive Shift", visible: true, width: '110' },
        { name: 'RECEIVE_CATEGORY', displayName: "Receive Category", visible: true, width: '140' },
        { name: 'CUSTOMER_CODE', displayName: "Dist. Code", visible: true, width: '100' },
        { name: 'CUSTOMER_NAME', displayName: "Dist. Name", visible: true, width: '150' },
        { name: 'CUSTOMER_TYPE', displayName: "Dist. Type", visible: true, width: '100' },
        { name: 'CHALLAN_NUMBER', displayName: "Challan No", visible: true, width: '100' },
        { name: 'CHALLAN_DATE', displayName: "Challan Date", visible: true, width: '110' },
        //{ name: 'SENDING_CARTON_QTY', displayName: "Entered By", visible: true, width: '90' },
        //{ name: 'SENDING_BAG_QTY', displayName: "Entered Date", visible: true, width: '90' },
        //{ name: 'SENDING_TOTAL_AMOUNT', displayName: "Notification Status", visible: true, width: '10%' },
        //{ name: 'RECEIVE_CARTON_QTY', displayName: "Entry Date", visible: true, width: '90' },
        //{ name: 'RECEIVE_BAG_QTY', displayName: "Entered By", visible: true, width: '90' },
        //{ name: 'RECEIVE_TOTAL_AMOUNT', displayName: "Entered Date", visible: true, width: '90' },
        //{ name: 'DRIVER_CODE', displayName: "Entry Date", visible: true, width: '90' },
        //{ name: 'DRIVER_NAME', displayName: "Entered By", visible: true, width: '90' },
        //{ name: 'VEHICLE_NO', displayName: "Entered Date", visible: true, width: '90' },
        //{ name: 'RECEIVED_BY', displayName: "Entry Date", visible: true, width: '90' },
        { name: 'REMARKS', displayName: "Remarks", visible: true, width: '100' },
        { name: 'ENTERED_BY', displayName: "Entered By", visible: true, width: '110' },
        { name: 'ENTERED_DATE', displayName: "Entered Date", visible: true, width: '200' }
    ];
    $scope.gridList.rowTemplate = `<div ng-style='row.entity.is_selected == true && grid.appScope.rowStyle' ng-dblclick=\"grid.appScope.editMst(row.entity)\" title=\"Double click to get Edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>`;
    $scope.rowNoGenOfGridList = function () {
        for (let i = 0; i < $scope.gridList.data.length; i++) {
            $scope.gridList.data[i].ROW_NO = i + 1;
        }
    }
    //--------------Data Load----------------------
    $scope.getDistList = function () {
        $scope.showLoader = true;
        refurbishmentServices.getDistList().then(function (res) {
            $scope.distList = res.data;
            $scope.showLoader = false;

        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }
    $scope.onSelectDistributor = function () {
        let selectedOption = JSON.parse($scope.model.CUSTOMER);
        $scope.model.CHALLAN_NUMBER = selectedOption.CHALLAN_NO;
        $scope.model.CHALLAN_DATE = selectedOption.CHALLAN_DATE;
        $scope.model.CUSTOMER_CODE = selectedOption.CUSTOMER_CODE;
        $scope.model.CUSTOMER_NAME = selectedOption.CUSTOMER_NAME;
        $scope.model.CUSTOMER_ADDRESS = selectedOption.CUSTOMER_ADDRESS;
        $scope.model.FACTORY_CODE = selectedOption.FACTORY_CODE;
        $scope.model.FACTORY_NAME = selectedOption.FACTORY_NAME;
        $scope.model.SENDING_CARTON_QTY = selectedOption.SEND_CARTON_QTY;
        $scope.model.SENDING_BAG_QTY = selectedOption.SEND_BAG_QTY;
        $scope.model.SENDING_TOTAL_AMOUNT = selectedOption.TOTAL_AMOUNT;
        $scope.model.DRIVER_CODE = selectedOption.DRIVER_CODE;
        $scope.model.DRIVER_NAME = selectedOption.DRIVER_NAME;
        $scope.model.DRIVER_CONTACT_NO = selectedOption.DRIVER_CONTACT_NO;
        $scope.model.VEHICLE_NO = selectedOption.VEHICLE_NO;

        refurbishmentServices.getProductsByChallan({ challanNo: $scope.model.CHALLAN_NUMBER }).then(function (res) {
            $scope.gridOptions.data = res.data;
            $scope.showLoader = false;
            $scope.rowNumberGenerate();
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }
    $scope.onSelectDistType = function () {
        $scope.clearModel();
        if ($scope.model.CUSTOMER_TYPE == 'Manual') {
            $scope.isAutomatic = false;
            let manualInput = document.getElementsByClassName('typeaheadcontainer');
            let autoInput = document.getElementById('auto');
            let manualProduct = document.getElementById('manualProduct');
            manualInput[0].classList.remove("d-none");
            autoInput.classList.add("d-none");
            manualProduct.classList.remove("d-none");
            $scope.model.CUSTOMER_MANUAL = $scope.model.CUSTOMER_NAME;
        } else {
            $scope.isAutomatic = true;
            let manualInput = document.getElementsByClassName('typeaheadcontainer');
            let autoInput = document.getElementById('auto');
            let manualProduct = document.getElementById('manualProduct');
            manualInput[0].classList.add("d-none");
            autoInput.classList.remove("d-none");
            manualProduct.classList.add("d-none");

        }

    }
    $scope.onSelectDistTypeWhileEdit = function () {
        if ($scope.model.CUSTOMER_TYPE == 'Manual') {
            $scope.isAutomatic = false;
            let manualInput = document.getElementsByClassName('typeaheadcontainer');
            let autoInput = document.getElementById('auto');
            let manualProduct = document.getElementById('manualProduct');
            manualInput[0].classList.remove("d-none");
            autoInput.classList.add("d-none");
            manualProduct.classList.remove("d-none");
            $scope.model.CUSTOMER_MANUAL = $scope.model.CUSTOMER_NAME;
        } else {
            $scope.isAutomatic = true;
            let manualInput = document.getElementsByClassName('typeaheadcontainer');
            let autoInput = document.getElementById('auto');
            let manualProduct = document.getElementById('manualProduct');
            manualInput[0].classList.add("d-none");
            autoInput.classList.remove("d-none");
            manualProduct.classList.add("d-none");
        }
    }
    $scope.autoCompleteManualDistList = function (value) {
        if (value.length > 1) {
            let param = {
                searchKey: value
            }
            return refurbishmentServices.getManualDistList(param).then(function (res) {
                return res.data;
            }, function (error) {
                alert(error);
            });
        }
    }
    $scope.typeaheadSelectedDist = function (entity, selectedItem) {
        $scope.model.CUSTOMER_CODE = selectedItem.CUSTOMER_CODE;
        $scope.model.CUSTOMER_NAME = selectedItem.CUSTOMER_NAME;
        $scope.model.CUSTOMER_ADDRESS = selectedItem.CUSTOMER_ADDRESS;

    };
    $scope.autoCompleteManualProductList = function (value) {
        if (value.length > 1) {
            let param = {
                searchKey: value
            }
            return refurbishmentServices.getManualProductList(param).then(function (res) {
                return res.data;
            }, function (error) {
                alert(error);
            });
        }
    }
    $scope.typeaheadSelectedProduct = function (entity, selectedItem) {
        $scope.gridOptions.data.push({
            PRODUCT_CODE: selectedItem.SKU_CODE,
            PRODUCT_NAME: selectedItem.SKU_NAME,
            PACK_SIZE: selectedItem.PACK_SIZE,
            TRADE_PRICE: selectedItem.TRADE_PRICE,
            REVISED_PRICE: selectedItem.TRADE_PRICE,
            CLAIM_QTY: 0,
            RECEIVED_QTY: 0,
            DISPUTE_QTY: 0
        });
        $scope.PRODUCT_MANUAL = '';
        $scope.rowNumberGenerate();
    };
    $scope.SearchMstData = function () {
        let param = {
            fromDate: $scope.DATE_FROM,
            toDate: $scope.DATE_TO,
        }
        $scope.showLoader = true;
        refurbishmentServices.getMstList(param).then(function (res) {
            $scope.gridList.data = res.data;
            $scope.showLoader = false;
            $scope.rowNoGenOfGridList();
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });

        $('#SearchModal').modal('show');
    }
    $scope.editMst = function (rowEntity) {
        let param = {
            mstId: rowEntity.MST_SLNO
        }
        refurbishmentServices.getDistListWhileEdit(param).then(function (res) {
            $scope.distList = res.data;
            setTimeout(function () {
                $scope.$apply(function () {
                    $scope.model = rowEntity;
                    setTimeout(function () {
                        $scope.$apply(function () {
                            $scope.onSelectDistTypeWhileEdit();
                        });
                    }, 500);
                });
                setTimeout(function () {
                    $scope.$apply(function () {
                        $scope.loadCustomer();
                    });
                }, 500);
            }, 500);
            $scope.saveBtnName = "Update";
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
            $scope.saveBtnName = "Save";
        });
        $scope.showLoader = false;
        $scope.loadDtlByMstId(rowEntity.MST_SLNO);
        $('#SearchModal').modal('hide');

    }
    $scope.loadDtlByMstId = function (mstId) {
        let param = {
            mstId: mstId
        }
        refurbishmentServices.loadDtlByMstId(param).then(function (res) {
            $scope.gridOptions.data = [];
            $scope.gridOptions.data = res.data;
            for (let i = 0; i < $scope.gridOptions.data.length; i++) {
                if ($scope.gridOptions.data[i].REFURBISHMENT_PRODUCT_STATUS == 'Cancel') {
                    for (let j = 0; j < $scope.gridOptions.data.length; j++) {
                        if ($scope.gridOptions.data[i].PRODUCT_CODE === $scope.gridOptions.data[j].PRODUCT_CODE && $scope.gridOptions.data[i].BATCH_NO === $scope.gridOptions.data[j].BATCH_NO && $scope.gridOptions.data[i].CLAIM_QTY === $scope.gridOptions.data[j].CLAIM_QTY && $scope.gridOptions.data[j].REFURBISHMENT_PRODUCT_STATUS === 'Refurbishment')
                        {
                            $scope.gridOptions.data[i].PARENT = 'TEMP';
                            break;
                        }
                    }
                }
            }
            for (let i = 0; i < $scope.gridOptions.data.length; i++) {
                if ($scope.gridOptions.data[i].REFURBISHMENT_PRODUCT_STATUS == 'Cancel') {
                    $timeout(function () {
                        angular.forEach($scope.gridOptions.data, function (item) {
                            if ($scope.gridOptions.data[i].PRODUCT_CODE === item.PRODUCT_CODE && $scope.gridOptions.data[i].BATCH_NO === item.BATCH_NO && $scope.gridOptions.data[i].CLAIM_QTY === item.CLAIM_QTY && item.REFURBISHMENT_PRODUCT_STATUS === 'Refurbishment') {
                                var hashKey = item.$$hashKey;
                                $scope.gridOptions.data[i].PARENT = hashKey;
                                return false
                            }

                        });
                    });
                }
            }
            $scope.showLoader = false;
            $scope.rowNumberGenerate();
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }
    $scope.addCancelRow = function (rowEntity, colName) {
        if (['ROW_NO', 'PRODUCT_CODE','PRODUCT_NAME', 'PACK_SIZE', 'VALUE'].includes(colName)) {
            const exists = $scope.gridOptions.data.some(item => item.PARENT === rowEntity.$$hashKey);
            if (!exists && rowEntity.PARENT === undefined && rowEntity.REFURBISHMENT_PRODUCT_STATUS === 'Refurbishment' && $scope.model.CUSTOMER_TYPE == 'Automation') {
                let newRow = {
                    PRODUCT_CODE: rowEntity.PRODUCT_CODE,
                    PRODUCT_NAME: rowEntity.PRODUCT_NAME,
                    BATCH_NO: rowEntity.BATCH_NO,
                    PACK_SIZE: rowEntity.PACK_SIZE,
                    TRADE_PRICE: rowEntity.TRADE_PRICE,
                    REVISED_PRICE: rowEntity.TRADE_PRICE,
                    CLAIM_QTY: rowEntity.CLAIM_QTY,
                    RECEIVED_QTY: 0,
                    DISPUTE_QTY: 0,
                    VALUE: 0,
                    REFURBISHMENT_PRODUCT_STATUS: 'Cancel',
                    PARENT: rowEntity.$$hashKey
                };
                var rowIndex = $scope.gridOptions.data.indexOf(rowEntity);
                if (rowIndex !== -1) {
                    $scope.gridOptions.data.splice(rowIndex + 1, 0, newRow);
                }
            } else if (exists) {
                alert('Item already exist !');
            }
            $scope.rowNumberGenerate();
        }
    }
    //------------Save Operation--------------------
    $scope.SaveData = function (model) {
        if ($scope.formRefurbishment.$invalid) {
            let err = $scope.formRefurbishment.$error.required;
            for (var i = 0; i < err.length; i++) {
                let input = document.getElementById(err[i].$$attr.id);
                input.style.border = '1.5px solid red';
            }
            alert("Please Select " + err[0].$$attr.id);
            $scope.buttonClicked = false;
            return;
        }
        $scope.isSaveBtnVisible = false;
        $scope.showLoader = true;
        model.Details = $scope.gridOptions.data;
        refurbishmentServices.addOrUpdate(model).then(function (res) {
            notificationservice.Notification(res.data.Status, 'true', "Data Saved Successfully :)");
            if (res.data.Status == 'true') {
                $scope.showLoader = false;
                $scope.isSaveBtnVisible = true;
                $scope.model.MST_SLNO = parseInt(res.data.Parent);
                $scope.model.CLAIM_NO = res.data.Key;
                let input = document.getElementById('CLAIM_NO');
                input.style.border = '3px solid green';
                $scope.loadDtlByMstId(res.data.Parent);
                $scope.saveBtnName = "Update";
            }
            else {
                $scope.showLoader = false;
                $scope.isSaveBtnVisible = true;
            }
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
            $scope.isSaveBtnVisible = true;
        });
    }
    $scope.Confirm = function (model) {
        $scope.showLoader = true;
        model.Details = $scope.gridOptions.data;
        refurbishmentServices.approval(model).then(function (res) {
            notificationservice.Notification(res.data.Status, 'true', "Congratulations! The approval of your refurbishment has been received.");
            if (res.data.Status == 'true') {
                $scope.showLoader = false;
                $scope.isSaveBtnVisible = true;
                $scope.model.MST_SLNO = parseInt(res.data.Parent);
                $scope.model.CLAIM_NO = res.data.Key;
                $scope.model.APPROVED_STATUS = 'A';
                let input = document.getElementById('CLAIM_NO');
                input.style.border = '3px solid green';
                $scope.loadDtlByMstId(res.data.Parent);
                $scope.saveBtnName = "Update";
            }
            else {
                $scope.showLoader = false;
                $scope.isSaveBtnVisible = true;
            }
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
            $scope.isSaveBtnVisible = true;
        });
    }
    //----------------- Permission Date-------------
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'Refurbishment',
            Action_Name: 'Receiving'
        }
        permissionProvider.GetPermission($scope.permissionReqModel).then(function (data) {
            $scope.getPermissions = data.data;
            $scope.model.ADD_PERMISSION = $scope.getPermissions.adD_PERMISSION;
            $scope.model.EDIT_PERMISSION = $scope.getPermissions.ediT_PERMISSION;
            $scope.model.DELETE_PERMISSION = $scope.getPermissions.deletE_PERMISSION;
            $scope.model.CONFIRM_PERMISSION = $scope.getPermissions.confirM_PERMISSION;
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
    //---------- Function Call ---------------------
    $scope.getDistList();
    //----------Trigger-----------------------------
    $scope.loadCustomer = function () {
        $('.select2-single').trigger('change');
    }
    var inputs = document.querySelectorAll(".form-control");
    inputs.forEach(input => {
        input.addEventListener('focusin', () => {
            input.style.border = '1px solid #aaa';
        }, false);
    })
    //-----------Data manupulate Function-----------
    $scope.onChangeGridRow = function (rowEntity) {
        rowEntity.VALUE = rowEntity.RECEIVED_QTY * rowEntity.REVISED_PRICE;
        $scope.calculateTotal();
    }
    $scope.calculateTotal = function () {
        $scope.model.RECEIVE_TOTAL_AMOUNT = 0;
        for (var i = 0; i < $scope.gridOptions.data.length; i++) {
            if ($scope.gridOptions.data[i].VALUE != NaN && $scope.gridOptions.data[i].REFURBISHMENT_PRODUCT_STATUS != 'Cancel') {
                $scope.model.RECEIVE_TOTAL_AMOUNT += $scope.gridOptions.data[i].VALUE;
            }
        }
    }
    $scope.ClearForm = function () {
        if (confirm('Do you really want to Clear Form? It cannot be reversed !') == true) {
            window.location.href = "/Inventory/Refurbishment/Receiving";
        }

    }
    $scope.clearModel = function () {
        $scope.model.MST_SLNO = 0;
        $scope.model.RECEIVE_CARTON_QTY = 0;
        $scope.model.RECEIVE_BAG_QTY = 0;
        $scope.model.CHALLAN_NUMBER = "";
        $scope.model.CHALLAN_DATE = "";
        $scope.model.CUSTOMER_CODE = "";
        $scope.model.CUSTOMER_NAME = "";
        $scope.model.CUSTOMER_ADDRESS = "";
        $scope.model.SENDING_CARTON_QTY = 0;
        $scope.model.SENDING_BAG_QTY = 0;
        $scope.model.SENDING_TOTAL_AMOUNT = 0;
        $scope.model.DRIVER_CODE = "";
        $scope.model.DRIVER_NAME = "";
        $scope.model.DRIVER_CONTACT_NO = "";
        $scope.model.VEHICLE_NO = "";
        $scope.model.CUSTOMER_MANUAL = "";
        $scope.model.CUSTOMER = "";
        $scope.gridOptions.data = [];
    }


    //-----------Report Print Function -----------
    $scope.GetPdfView = function (entity) {
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Inventory/Report/GenerateReport?ReportId=" + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&MST_ID=" + entity.MST_SLNO + "&REPORT_ID=77&REPORT_EXTENSION=" + 'Pdf' + "&PREVIEW=" + 'NO';
        window.open(href, '_blank');
    }
}])