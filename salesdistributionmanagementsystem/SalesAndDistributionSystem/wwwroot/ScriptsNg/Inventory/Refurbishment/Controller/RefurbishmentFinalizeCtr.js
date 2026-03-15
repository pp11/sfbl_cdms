ngApp.controller('ngGridCtrl', ['$scope', 'refurbishmentServices', 'uiGridConstants', 'permissionProvider', 'gridregistrationservice', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, refurbishmentServices, uiGridConstants, permissionProvider, gridregistrationservice, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'
    //-------variaable Declare------------------
    $scope.showloader = false;
    $scope.saveBtnName = "Save";
    $scope.isSaveBtnVisible = true;
    $scope.isAutomatic = true;
    $scope.RCV_TOTAL_AMOUNT = 0;
    $scope.REST_AMOUNT = 0;
    $scope.DATE_FROM = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.DATE_TO = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.model = { MST_SLNO: 0, CUSTOMER_TYPE: 'Automation', APPROVED_STATUS:'P', Details: [] }
    $scope.model.RECEIVE_DATE = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.model.FINALIZE_DATE = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.claims = [];
    //-----------grid operation------------------
    $scope.gridOptions = (gridregistrationservice.GridRegistration("Approved Product For Refurbishment"));
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptions.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '2%'
        }
        , {
            name: 'DTL_SLNO', field: 'DTL_SLNO', displayName: 'DTL_SLNO', enableFiltering: false, width: '10%', enableColumnMenu: false, visible: false
        }
        , {
            name: 'MST_SLNO', field: 'MST_SLNO', displayName: 'MST_SLNO', enableFiltering: false, width: '10%', enableColumnMenu: false, visible: false
        }
        , {
            name: 'REFURBISHMENT_PRODUCT_STATUS', field: 'REFURBISHMENT_PRODUCT_STATUS', displayName: 'Product Status', enableFiltering: false, visible: false, width: '10%', cellTemplate:
                '<select name="REFURBISHMENT_PRODUCT_STATUS" class="pl-sm form-control" ng-change="grid.appScope.onChangeGridRow(row.entity)"  ng-model="row.entity.REFURBISHMENT_PRODUCT_STATUS" required> <option value="Refurbishment">Refurbishment</option><option value="Cancel">Cancel</option></select > '

        }
        , {
            name: 'PRODUCT_CODE', field: 'PRODUCT_CODE', displayName: 'Product Code', enableFiltering: false, width: '10%', enableColumnMenu: false
        }
        , {
            name: 'PRODUCT_NAME', field: 'PRODUCT_NAME', displayName: 'Product Name', enableFiltering: false, width: '10%', enableColumnMenu: false
        }
        , {
            name: 'PACK_SIZE', field: 'PACK_SIZE', displayName: 'Pack Size', enableFiltering: false, width: '8%', enableColumnMenu: false
        }
        , {
            name: 'CLAIM_QTY', field: 'CLAIM_QTY', displayName: 'Claim Qty', enableFiltering: false, width: '12%', visible: false, cellTemplate:
                '<input type="number"  ng-model="row.entity.CLAIM_QTY"  min="0" step="any"   class="pl-sm text-right"  ng-readonly="grid.appScope.isAutomatic"/>'

        }
        , {
            name: 'RECEIVED_QTY', field: 'RECEIVED_QTY', displayName: 'Received Qty', enableFiltering: false, width: '12%', cellTemplate:
                '<input type="number" ng-change="grid.appScope.onChangeGridRow(row.entity)"  ng-model="row.entity.RECEIVED_QTY"  min="0" step="any"   class="pl-sm text-right" readonly />'

        }
        , {
            name: 'DISPUTE_QTY', field: 'DISPUTE_QTY', displayName: 'Dispute Qty', enableFiltering: false, width: '12%', visible: false, cellTemplate:
                '<input type="number"  ng-model="row.entity.DISPUTE_QTY"  min="0" step="any"   class="pl-sm text-right" />'

        }
        , {
            name: 'TRADE_PRICE', field: 'TRADE_PRICE', displayName: 'Tread Price', enableFiltering: false, width: '12%', cellTemplate:
                '<input type="number"  ng-model="row.entity.TRADE_PRICE"  min="0" step="any"   class="pl-sm text-right"  ng-readonly="grid.appScope.isAutomatic"/>'

        }
        , {
            name: 'REVISED_PRICE', field: 'REVISED_PRICE', displayName: 'Revised Price', enableFiltering: false, width: '12%', cellTemplate:
                '<input type="number" ng-change="grid.appScope.onChangeGridRow(row.entity)" ng-model="row.entity.REVISED_PRICE"  min="0" step="any"   class="pl-sm text-right" readonly />', footerCellTemplate: '<div class="ui-grid-cell-contents" style="color: black; text-align: right;">Grand Total:</div>'
        }
        , {
            name: 'EXPIRY_DATE', field: 'EXPIRY_DATE', displayName: 'Expiry Date', enableFiltering: false, visible: false, width: '15%', cellTemplate:
                '<div class="simple-date2">'
                + '<div class="input-group date">'
                + '<div class="input-group-prepend">'
                + '</div>'
                + '<input  type="text" datepicker class="form-control"  readonly  ng-model="row.entity.EXPIRY_DATE" placeholder="dd/mm/yyyy" id="EXPIRY_DATE">'
                + '</div>'
                + '</div>'
        }
        , {
            name: 'VALUE', field: 'VALUE', displayName: 'Value', enableFiltering: false, enableColumnMenu: false, width: '10%', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: Red;color: White; text-align: right;">{{grid.appScope.RCV_TOTAL_AMOUNT}}</div>'
        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remarks', enableFiltering: false, enableColumnMenu: false, width: '10%', visible: true, cellTemplate:
                '<input type="text"  ng-model="row.entity.REMARKS"   class="pl-sm" />'
        }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-click="grid.appScope.addNewRowToInvoice(row.entity)" type="button" class="btn btn-outline-primary mb-1"><img src="/img/fast-forward.gif" height="15px" style="border:none" ></button>' +
                '<button style="margin-bottom: 5px;" ng-show="1>2" ng-click="grid.appScope.removeItem(row.entity, grid.appScope.invoiceGridList.data)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +
                '</div>'
        }
    ]
    $scope.gridOptions.showColumnFooter = true;

    $scope.gridDtls = (gridregistrationservice.GridRegistration("Product Analysis For Refurbishment"));
    $scope.gridDtls.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridDtls.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '40'
        }
        , {
            name: 'DTL_SLNO', field: 'DTL_SLNO', displayName: 'DTL_SLNO', enableFiltering: false, width: '10%', enableColumnMenu: false, visible: false
        }
        , {
            name: 'MST_SLNO', field: 'MST_SLNO', displayName: 'MST_SLNO', enableFiltering: false, width: '10%', enableColumnMenu: false, visible: false
        }
        , {
            name: 'PRODUCT_CODE', field: 'PRODUCT_CODE', displayName: 'Product Code', enableFiltering: false, width: '110', enableColumnMenu: false, footerCellTemplate: '<div class="ui-grid-cell-contents" style="color: black; text-align: right;">Rest Amount:</div>'
        }
        , {
            name: 'PRODUCT_NAME', field: 'PRODUCT_NAME', displayName: 'Product Name', enableFiltering: false, width: '120', enableColumnMenu: false, footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: Red;color: White; text-align: right;">{{grid.appScope.REST_AMOUNT}}</div>'
        }
        , {
            name: 'PACK_SIZE', field: 'PACK_SIZE', displayName: 'Pack Size', enableFiltering: false, width: '90', enableColumnMenu: false
        }
        , {
            name: 'PASSED_QTY', field: 'PASSED_QTY', displayName: 'Stock Qty', enableFiltering: false, width: '90', enableColumnMenu: false
        }
        , {
            name: 'PROD_QTY', field: 'PROD_QTY', displayName: 'Adjust Qty', enableFiltering: false, width: '110', visible: true, cellTemplate:
                '<input type="number" ng-change="grid.appScope.onChangeGridRow(row.entity)"  ng-model="row.entity.PROD_QTY"  min="0" step="any"   class="pl-sm text-right" />'

        }
        , {
            name: 'TRADE_PRICE', field: 'TRADE_PRICE', displayName: 'Tread Price', enableFiltering: false, width: '110', cellTemplate:
                '<input type="number"  ng-model="row.entity.TRADE_PRICE"  min="0" step="any"   class="pl-sm text-right"  ng-readonly="grid.appScope.isAutomatic"/>'

        }
        , {
            name: 'REVISED_PRICE', field: 'REVISED_PRICE', displayName: 'Revised Price', enableFiltering: false, width: '12%', cellTemplate:
                '<input type="number" ng-change="grid.appScope.onChangeGridRow(row.entity)" ng-model="row.entity.REVISED_PRICE"  min="0" step="any"   class="pl-sm text-right" />',footerCellTemplate: '<div class="ui-grid-cell-contents" style="color: black; text-align: right;">Grand Total:</div>'
        }
       
        , {
            name: 'AMOUNT', field: 'AMOUNT', displayName: 'Amount', enableFiltering: false, enableColumnMenu: false, width: '10%', footerCellTemplate: '<div class="ui-grid-cell-contents" style="background-color: Red;color: White; text-align: right;">{{grid.appScope.model.TOTAL_AMOUNT}}</div>'
        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remarks', enableFiltering: false, enableColumnMenu: false, width: '10%', visible: true, cellTemplate:
                '<input type="text"  ng-model="row.entity.REMARKS"   class="pl-sm" />'
        }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-click="grid.appScope.removeItem(row.entity, grid.appScope.invoiceGridList.data)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +
                '</div>'
        }
    ]
    $scope.gridDtls.showColumnFooter= true;
    $scope.rowNumberGenerate = function () {
        for (let i = 0; i < $scope.gridOptions.data.length; i++) {
            $scope.gridOptions.data[i].ROW_NO = i + 1;
        }
    }
    $scope.rowNoGenDtlGrid = function () {
        for (let i = 0; i < $scope.gridDtls.data.length; i++) {
            $scope.gridDtls.data[i].ROW_NO = i + 1;
        }
    }
    $scope.removeItem = function (rowEntity) {
        if (confirm('Do you really want to remove this row? It cannot be undone !') == true) {
            var index = $scope.gridDtls.data.indexOf(rowEntity);
            if (index !== -1) {
                $scope.gridDtls.data.splice(index, 1);
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
        { name: 'MST_SLNO', displayName: "Sl", visible: false, width: '50' },
        { name: 'APPROVED_MGS', displayName: "Approved Status", visible: true, width: '120' },
        { name: 'FINALIZE_DATE', displayName: "Finalize Date", visible: true, width: '110' },
        { name: 'FINALIZE_SHIFT', displayName: "Finalize Shift", visible: true, width: '110' },
        { name: 'CUSTOMER_CODE', displayName: "Dist. Code", visible: true, width: '110' },
        { name: 'CUSTOMER_NAME', displayName: "Dist. Name", visible: true, width: '110' },
        { name: 'CLAIM_NO', displayName: "Claim No", visible: true, width: '130' },
        { name: 'RECEIVE_CATEGORY', displayName: "Receive Category", visible: true, width: '130' },
        { name: 'TOTAL_AMOUNT', displayName: "Total Amount", visible: true, width: '110' },
        { name: 'REMARKS', displayName: "Remarks", visible: true, width: '110' },
        { name: 'ENTERED_BY', displayName: "Entered By", visible: true, width: '110' },
        { name: 'APPROVED_STATUS', displayName: "Approved Status", visible: false, width: '130' }
    ];
    $scope.gridList.rowTemplate = `<div ng-style='row.entity.is_selected == true && grid.appScope.rowStyle' ng-dblclick=\"grid.appScope.editMst(row.entity)\" title=\"Double click to get Edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>`;
    $scope.rowNoGenOfGridList = function () {
        for (let i = 0; i < $scope.gridList.data.length; i++) {
            $scope.gridList.data[i].ROW_NO = i + 1;
        }
    }
    //--------------Data Load----------------------
    $scope.getClaims = function () {
        $scope.showLoader = true;
        refurbishmentServices.getClaims().then(function (res) {
            $scope.claims = res.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }
    $scope.onSelectClaim = function () {
        let selectedOption = JSON.parse($scope.model.CLAIM);
        $scope.model.RECEIVE_DATE = selectedOption.RECEIVE_DATE;
        $scope.model.RECEIVE_CATEGORY = selectedOption.RECEIVE_CATEGORY;
        $scope.model.CUSTOMER_TYPE = selectedOption.CUSTOMER_TYPE;
        $scope.model.CUSTOMER_NAME = selectedOption.CUSTOMER_NAME;
        $scope.model.CUSTOMER_CODE = selectedOption.CUSTOMER_CODE;
        $scope.model.CUSTOMER_ADDRESS = selectedOption.CUSTOMER_ADDRESS;
        $scope.model.CHALLAN_NUMBER = selectedOption.CHALLAN_NUMBER;
        $scope.model.CHALLAN_DATE = selectedOption.CHALLAN_DATE;
        $scope.model.CLAIM_NO = selectedOption.CLAIM_NO;
        refurbishmentServices.getProductsByClaimNo({ claimNo: $scope.model.CLAIM_NO }).then(function (res) {
            $scope.gridOptions.data = res.data;
            $scope.showLoader = false;
            $scope.rowNumberGenerate();
            $scope.calculateRvcTotal();
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }
    $scope.autoCompleteManualProductList = function (value) {
        if (value.length > 1) {
            let param = {
                searchKey: value
            }
            return refurbishmentServices.getManualProductsWithStock(param).then(function (res) {
                return res.data;
            }, function (error) {
                alert(error);
            });
        }
    }
    $scope.typeaheadSelectedProduct = function (entity, selectedItem) {
        $scope.gridDtls.data.push({
            PRODUCT_CODE: selectedItem.SKU_CODE,
            PRODUCT_NAME: selectedItem.SKU_NAME,
            PACK_SIZE: selectedItem.PACK_SIZE,
            TRADE_PRICE: selectedItem.TRADE_PRICE,
            REVISED_PRICE: selectedItem.REVISED_PRICE,
            PASSED_QTY: selectedItem.PASSED_QTY,
            PROD_QTY: 0,
            AMOUNT:0
        });
        $scope.PRODUCT_MANUAL = '';
        $scope.rowNoGenDtlGrid();
    };
    $scope.addNewRowToInvoice = function (selectedItem) {
        $scope.gridDtls.data.push({
            PRODUCT_CODE: selectedItem.PRODUCT_CODE,
            PRODUCT_NAME: selectedItem.PRODUCT_NAME,
            PACK_SIZE: selectedItem.PACK_SIZE,
            TRADE_PRICE: selectedItem.TRADE_PRICE,
            REVISED_PRICE: selectedItem.REVISED_PRICE,
            PROD_QTY: selectedItem.RECEIVED_QTY,
            AMOUNT: selectedItem.VALUE,
            PASSED_QTY: selectedItem.PASSED_QTY
        });
        $scope.PRODUCT_MANUAL = '';
        $scope.rowNoGenDtlGrid();
        $scope.calculateTotal();
    };
    $scope.SearchMstData = function () {
        let param = {
            fromDate: $scope.DATE_FROM,
            toDate: $scope.DATE_TO,
        }
        $scope.showLoader = true;
        refurbishmentServices.getFinalizationMstList(param).then(function (res) {
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
        refurbishmentServices.getClaimsWhileEdit(param).then(function (res) {
            $scope.claims = res.data;
            setTimeout(function () {
                $scope.$apply(function () {
                    $scope.model = rowEntity;
                    setTimeout(function () {
                        $scope.$apply(function () {
                            refurbishmentServices.getProductsByClaimNo({ claimNo: $scope.model.CLAIM_NO }).then(function (res) {
                                $scope.gridOptions.data = res.data;
                                $scope.showLoader = false;
                                $scope.rowNumberGenerate();
                                setTimeout(function () {
                                    $scope.$apply(function () {
                                        $scope.calculateRvcTotal();
                                    });
                                }, 500);
                            }, function (error) {
                                alert(error);
                                $scope.showLoader = false;
                            });
                        });
                    }, 500);
                });
                setTimeout(function () {
                    $scope.$apply(function () {
                        $scope.loadClaim();
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
        refurbishmentServices.loadFinalizationDtlByMstId(param).then(function (res) {
            $scope.gridDtls.data = [];
            $scope.gridDtls.data = res.data;
            $scope.showLoader = false;
            $scope.rowNoGenDtlGrid();
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
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
        model.Details = $scope.gridDtls.data;
        model.RcvDetails = $scope.gridOptions.data;
        refurbishmentServices.addOrUpdateFinalization(model).then(function (res) {
            notificationservice.Notification(res.data.Status, 'true', "Data Saved Successfully :)");
            if (res.data.Status == 'true') {
                $scope.showLoader = false;
                $scope.isSaveBtnVisible = true;
                $scope.model.MST_SLNO = parseInt(res.data.Parent);
                $scope.model.CLAIM_NO = res.data.Key;
                let input = document.getElementById('CLAIM');
                input.style.border = '1px solid green';
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
        refurbishmentServices.finalizationApproval(model).then(function (res) {
            notificationservice.Notification(res.data.Status, 'true', "Congratulations! The approval of your refurbishment finalization has been received.");
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
            Action_Name: 'Finalization'
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
    $scope.getClaims();
    //----------Trigger-----------------------------
    $scope.loadClaim = function () {
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
        rowEntity.AMOUNT = rowEntity.PROD_QTY * rowEntity.REVISED_PRICE;
        $scope.calculateTotal();
    }
    $scope.calculateTotal = function () {
        $scope.model.TOTAL_AMOUNT = 0;
        for (var i = 0; i < $scope.gridDtls.data.length; i++) {
            if ($scope.gridDtls.data[i].AMOUNT != NaN) {
                $scope.model.TOTAL_AMOUNT += $scope.gridDtls.data[i].AMOUNT;
            }
        }
        $scope.REST_AMOUNT = $scope.model.TOTAL_AMOUNT - $scope.RCV_TOTAL_AMOUNT;
    }
    $scope.calculateRvcTotal = function () {
        $scope.RCV_TOTAL_AMOUNT = 0;
        for (var i = 0; i < $scope.gridOptions.data.length; i++) {
            if ($scope.gridOptions.data[i].VALUE != NaN) {
                $scope.RCV_TOTAL_AMOUNT += $scope.gridOptions.data[i].VALUE;
            }
        }
        $scope.REST_AMOUNT = $scope.model.TOTAL_AMOUNT - $scope.RCV_TOTAL_AMOUNT;
    }
    $scope.ClearForm = function () {
        if (confirm('Do you really want to Clear Form? It cannot be reversed !') == true) {
            window.location.href = "/Inventory/Refurbishment/Finalization";
        }
    }
    //-----------Report Print Function -----------
    $scope.GetPdfView = function (entity) {
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Inventory/Report/GenerateReport?ReportId=" + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&MST_ID=" + entity.MST_SLNO + "&REPORT_ID=88&REPORT_EXTENSION=" + 'Pdf' + "&PREVIEW=" + 'NO';
        window.open(href, '_blank');
    }
}])