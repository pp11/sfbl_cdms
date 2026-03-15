ngApp.controller('ngGridCtrl', ['$scope', 'InsertOrEditServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InsertOrEditServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'

    $scope.model = { COMPANY_ID: 0, UNIT_ID: 0, COLLECTION_MST_ID: 0, BATCH_NO: '', BATCH_STATUS: '', BATCH_POSTING_STATUS: 'Active', BATCH_DATE: '' }
    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Batch_Status = [{ BATCH_STATUS: 'Active' }, { BATCH_STATUS: 'InActive' }];
    $scope.Posting_Status = [{ BATCH_POSTING_STATUS: 'Pending' }, { BATCH_POSTING_STATUS: 'Approved' }];
    $scope.Collection_Modes = [];
    $scope.Customers = [];
    $scope.Branchs = [];
    $scope.Modes = [];
    $scope.Banks = [];
    $scope.Branchs_Copy;

    $scope.model.BATCH_POSTING_STATUS = 'Pending';

    $scope.DefalutData = function () {
        return { COMPANY_ID: 0, UNIT_ID: 0, COLLECTION_MST_ID: 0, BATCH_NO: '', BATCH_STATUS: 'Active', BATCH_POSTING_STATUS: 'Debit', BATCH_DATE: '' }
    }

    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 0, COMPANY_ID: 0, UNIT_ID: 0, COLLECTION_DTL_ID: 0, INVOICE_NO: '', BALANCE: 0,
            COLLECTION_MST_ID: 0, COLLECTION_AMT: 0, BRANCH_ID: '', BANK_ID: '', CUSTOMER_ID: 0, MEMO_COST: 0, TDS_AMT: 0, NET_COLLECTION_AMT: 0,
            COLLECTION_MODE: '', CUSTOMER_CODE: '', VOUCHER_NO: '', VOUCHER_DATE: '', REMARKS: '', BALANCE_TDS_AMT:0
        }
    }

    $scope.gridOptions = (gridregistrationservice.GridRegistration("Region Area Relation"));
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;

    }
    $scope.gridOptions.data = [];
    //Grid Register
    $scope.gridOptions = {
        data: [$scope.GridDefalutData()],
        showGridFooter: true
    }


    $scope.formateDate = function (date) {
        const today = new Date(date);
        const yyyy = today.getFullYear();
        let mm = today.getMonth() + 1; // Months start at 0!
        let dd = today.getDate();

        if (dd < 10) dd = '0' + dd;
        if (mm < 10) mm = '0' + mm;

        const formattedToday = dd + '/' + mm + '/' + yyyy;

        return formattedToday;
    }

    $scope.Batch_date_Load = function () {
        const today = new Date();
        const yyyy = today.getFullYear();
        let mm = today.getMonth() + 1; // Months start at 0!
        let dd = today.getDate();

        if (dd < 10) dd = '0' + dd;
        if (mm < 10) mm = '0' + mm;

        $scope.model.BATCH_DATE = dd + '/' + mm + '/' + yyyy;
    }

    $scope.Batch_date_Load();

    //Generate New Row No
    $scope.rowNumberGenerate = function () {
        for (let i = 0; i < $scope.gridOptions.data.length; i++) {
            $scope.gridOptions.data[i].ROW_NO = i;
        }
    }
    $scope.OnCustomerSelected = function (entity) {
        entity.COLLECTION_AMT = 0;
        entity.NET_COLLECTION_AMT = 0;
        entity.TDS_AMT = 0;
        entity.BANK_ID = "1";
        entity.BRANCH_ID = "1";
        if ($scope.Customers.length <1) {
            $scope.LoadCustomerDaywiseBalance(entity);
        } else {
            const index = $scope.Customers.findIndex(x => x.CUSTOMER_CODE == entity.CUSTOMER_CODE);
            if (index != -1) {
                entity.CUSTOMER_ID = $scope.Customers[index].CUSTOMER_ID;
                entity.CUSTOMER_NAME = $scope.Customers[index].CUSTOMER_NAME;
                $scope.LoadCustomerDaywiseBalance(entity);
            }
        }
    }
    //Add New Row

    $scope.addNewRow = () => {
        //for (var i = 0; i < $scope.gridOptions.data.length; i++) {
        //    $scope.gridOptions.data[i].NET_COLLECTION_AMT = $scope.gridOptions.data[i].TDS_AMT + $scope.gridOptions.data[i].COLLECTION_AMT + $scope.gridOptions.data[i].MEMO_COST;
        //}

        if ($scope.gridOptions.data.length > 0 && $scope.gridOptions.data[0].CUSTOMER_CODE != null && $scope.gridOptions.data[0].CUSTOMER_CODE != '' && $scope.gridOptions.data[0].CUSTOMER_CODE != 'undefined' && $scope.gridOptions.data[0].COLLECTION_AMT >0) {
           
            var newRow = {
                ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, UNIT_ID: $scope.model.UNIT_ID,
                COLLECTION_DTL_ID: $scope.gridOptions.data[0].COLLECTION_DTL_ID,
                COLLECTION_MST_ID: $scope.model.COLLECTION_MST_ID,
                COLLECTION_AMT: $scope.gridOptions.data[0].COLLECTION_AMT,
                BRANCH_ID: $scope.gridOptions.data[0].BRANCH_ID,
                BANK_ID: $scope.gridOptions.data[0].BANK_ID,
                CUSTOMER_ID: $scope.gridOptions.data[0].CUSTOMER_ID,
                MEMO_COST: $scope.gridOptions.data[0].MEMO_COST,
                TDS_AMT: $scope.gridOptions.data[0].TDS_AMT,
                BALANCE_TDS_AMT: $scope.gridOptions.data[0].BALANCE_TDS_AMT,
                NET_COLLECTION_AMT: $scope.gridOptions.data[0].NET_COLLECTION_AMT,
                COLLECTION_MODE: $scope.gridOptions.data[0].COLLECTION_MODE,
                CUSTOMER_CODE: $scope.gridOptions.data[0].CUSTOMER_CODE,
                VOUCHER_NO: $scope.gridOptions.data[0].VOUCHER_NO,
                VOUCHER_DATE: $scope.gridOptions.data[0].VOUCHER_DATE,
                INVOICE_NO: $scope.gridOptions.data[0].INVOICE_NO,
                REMARKS: $scope.gridOptions.data[0].REMARKS,
                BALANCE: $scope.gridOptions.data[0].BALANCE,
                CUSTOMER_NAME: $scope.gridOptions.data[0].CUSTOMER_NAME
            }

            //$scope.gridOptions.data.push(newRow);

            var chekFlag = $scope.gridOptions.data[0].OLD_ROW_NO ?? 0;
            if (chekFlag == 0) {
                $scope.gridOptions.data.splice(1, 0, newRow);
            } else {
                $scope.gridOptions.data.splice($scope.gridOptions.data[0].OLD_ROW_NO, 0, newRow);
            }
            $scope.gridOptions.data[0] = $scope.GridDefalutData();
        } else {
            notificationservice.Notification("Please select a valid row where a customer is chosen and the collection amount is greater than zero", "", 'Only Single Row Left!!');
        }
        $scope.rowNumberGenerate();
    };

    $scope.EditItem = (entity) => {
        if ($scope.gridOptions.data.length > 0) {
            var newRow = {
                ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, UNIT_ID: $scope.model.UNIT_ID,
                COLLECTION_DTL_ID: entity.COLLECTION_DTL_ID,
                COLLECTION_MST_ID: entity.COLLECTION_MST_ID,
                COLLECTION_AMT: entity.COLLECTION_AMT,
                BRANCH_ID: entity.BRANCH_ID, BANK_ID: entity.BANK_ID,
                CUSTOMER_ID: entity.CUSTOMER_ID, MEMO_COST: entity.MEMO_COST,
                TDS_AMT: entity.TDS_AMT, NET_COLLECTION_AMT: entity.NET_COLLECTION_AMT,
                COLLECTION_MODE: entity.COLLECTION_MODE, CUSTOMER_CODE: entity.CUSTOMER_CODE,
                VOUCHER_NO: entity.VOUCHER_NO, VOUCHER_DATE: entity.VOUCHER_DATE,
                INVOICE_NO: entity.INVOICE_NO,
                REMARKS: entity.REMARKS,
                BALANCE: entity.BALANCE, CUSTOMER_NAME: entity.CUSTOMER_NAME,
                OLD_ROW_NO: entity.ROW_NO,
                BALANCE_TDS_AMT: entity.BALANCE_TDS_AMT
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
    $scope.LoadFormData = function () {
        $scope.showLoader = true;

        InsertOrEditServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
            $scope.relationdata = data.data;
            $scope.showLoader = false;

            for (var i in data.data) {
                if (data.data[i].REGION_CODE == $scope.model.REGION_CODE) {
                    $scope.model.REGION_AREA_MST_ID_ENCRYPTED = data.data[i].REGION_AREA_MST_ID_ENCRYPTED;
                    $scope.GetEditDataById($scope.model.REGION_AREA_MST_ID_ENCRYPTED);
                    $scope.addNewRow();
                }
            }
        }, function (error) {
            alert(error);

            $scope.showLoader = false;
        });
    }
    $scope.gridOptions.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '40',pinnedLeft: true
        }
        , {
            name: 'COLLECTION_DTL_ID', field: 'COLLECTION_DTL_ID', visible: false
        }
        , {
            name: 'COLLECTION_MST_ID', field: 'COLLECTION_MST_ID', visible: false
        }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , { name: 'UNIT_ID', field: 'UNIT_ID', visible: false }

        , { name: 'CUSTOMER_ID', field: 'CUSTOMER_ID', visible: false }

        , {
            name: 'CUSTOMER_CODE', field: 'CUSTOMER_CODE', pinnedLeft: true, displayName: 'Customer', enableFiltering: true, width: '22%', cellTemplate:
                //'<div ng-show="row.entity.ROW_NO==0"><select class="select2-single form-control" id="CUSTOMER_CODE" ng-disabled="row.entity.ROW_NO !=0"   ng-change="grid.appScope.OnCustomerSelected(row.entity)"  ' +
                //'name="CUSTOMER_CODE" ng-model="row.entity.CUSTOMER_CODE" style="width:100%" >' +
                //'<option ng-repeat="item in grid.appScope.Customers" ng-selected="item.CUSTOMER_CODE == row.entity.CUSTOMER_CODE" value="{{item.CUSTOMER_CODE}}">{{ item.CUSTOMER_NAME }} | Code: {{ item.CUSTOMER_CODE }} </option>' +
                //'</select></div>' +
                '<div class="typeaheadcontainer"  ng-show="row.entity.ROW_NO==0">' +
                '<input type="text" autocomplete="off" style="width:100%;" class="form-control" name="ROLE_NAME"' +
                ' ng-model="row.entity.CUSTOMER_NAME"' +
                ' uib-typeahead="Customer.CUSTOMER_NAME as Customer.CUSTOMER_NAME_CODE for Customer in grid.appScope.AutoCompleteDataLoadForCustomer($viewValue)"' +
                ' typeahead-append-to-body="true"' +
                ' placeholder="Customer Name Min. Two Character"' +
                ' typeahead-editable="false"' +
                ' typeahead-on-select="grid.appScope.typeaheadSelectedCustomer(row.entity, $item)" />' +
                '</div>'+
                '<div ng-show="row.entity.ROW_NO>0" > {{ row.entity.CUSTOMER_NAME }} | Code: {{ row.entity.CUSTOMER_CODE }} </div>'
        }
        , {
            name: 'BANK_ID', field: 'BANK_ID', displayName: 'Bank', enableFiltering: false, width: '7%', cellTemplate:
                '<select class="form-control" id="BANK_ID" ng-disabled="row.entity.ROW_NO !=0"  ' +
                'name="BANK_ID" ng-model="row.entity.BANK_ID" style="width:100%" >' +
                '<option ng-repeat="item in grid.appScope.Banks" ng-selected="item.BANK_ID == row.entity.BANK_ID" value="{{item.BANK_ID}}">{{ item.BANK_NAME }} </option>' +
                '</select>'
        }

        , {
            name: 'BRANCH_ID', field: 'BRANCH_ID', headerCellTemplate: '<div class="">Branch<br>Code</div>',  enableFiltering: false, width: '8%', cellTemplate:
                '<select class="form-control" id="CUSTOMER_CODE" ng-disabled="row.entity.ROW_NO !=0 || row.entity.BANK_ID<1"   ' +
                'name="BRANCH_ID" ng-model="row.entity.BRANCH_ID" style="width:100%" >' +
                '<option ng-repeat="item in grid.appScope.Branchs | filter:{ BANK_ID: row.entity.BANK_ID }" ng-selected="item.BRANCH_ID == row.entity.BRANCH_ID" value="{{item.BRANCH_ID}}">{{ item.BRANCH_NAME }} </option>' +
                '</select>'
        }
        , {
            name: 'INVOICE_NO', field: 'INVOICE_NO', headerCellTemplate: '<div class="">Invoice<br>No</div>', enableFiltering: false, width: '8%', visible:false, cellTemplate:
                '<div class= "typeaheadcontainer" >' +
                '<input type="text" autocomplete="off" style="width:100%;" class="form-control" name="INVOICE_NO"' +
                'ng-model="row.entity.INVOICE_NO" ' +
                'uib-typeahead="Invoice as Invoice.INVOICE_NO for Invoice in grid.appScope.AutoCompleteDataLoadForInvoice($viewValue, row.entity.CUSTOMER_ID)"' +
                'typeahead-append-to-body="true"' +
                'placeholder="Enter Invoice No minimum 1 character"' +
                'typeahead-editable="false"' +
                'typeahead-on-select="grid.appScope.typeaheadSelectedInvoice(row.entity, $item)" />'
        }
        , {
            name: 'BALANCE', field: 'BALANCE', headerCellTemplate: '<div class="">Balance<br>Amount</div>', enableFiltering: false, width: '8%', cellTemplate:
                '<input ng-disabled="true" ng-model="row.entity.BALANCE"  class="pl-sm" />'
        }, {
            name: 'BALANCE_TDS_AMT', field: 'BALANCE_TDS_AMT', headerCellTemplate: '<div class="">Balance<br>TDS</div>',  enableFiltering: false, width: '8%'
        }
        , {
            name: 'COLLECTION_MODE', field: 'COLLECTION_MODE', headerCellTemplate: '<div class="">Collection<br>Mode</div>',enableFiltering: false, width: '8%', cellTemplate:
                '<select class="form-control" id="COLLECTION_MODE" ng-disabled="row.entity.ROW_NO !=0"' +
                'name="CUSTOMER_CODE" ng-model="row.entity.COLLECTION_MODE" style="width:100%" >' +
                '<option ng-repeat="item in grid.appScope.Modes" ng-selected="item.MODE_NAME == row.entity.COLLECTION_MODE" value="{{item.MODE_NAME}}">{{ item.MODE_NAME }} </option>' +
                '</select>'
        }
        , {
            name: 'COLLECTION_AMT', field: 'COLLECTION_AMT', headerCellTemplate: '<div class="">Collection<br>Amount</div>',  enableFiltering: false, width: '8%', cellTemplate:
                '<input type="number" min="0" ng-disabled="row.entity.ROW_NO !=0" ng-model="row.entity.COLLECTION_AMT" ng-change="grid.appScope.onChangeCollectionAmt(row.entity)" class="pl-sm" required />'
        }
        
        , {
            name: 'MEMO_COST', field: 'MEMO_COST', headerCellTemplate: '<div class="">Memo<br>Cost</div>', enableFiltering: false, width: '8%', cellTemplate:
                '<input type="number"  min="0"  ng-disabled="row.entity.ROW_NO !=0" ng-model="row.entity.MEMO_COST" ng-change="grid.appScope.onChangeCollectionAmt(row.entity)" class="pl-sm" />'
        }, {
            name: 'NET_COLLECTION_AMT', field: 'NET_COLLECTION_AMT', displayName: 'Net Collection Amt', enableFiltering: false, width: '8%', cellTemplate:
                '<input type="number"  min="0"  ng-disabled="true" ng-model="row.entity.NET_COLLECTION_AMT"  class="pl-sm" />'
        },
        , {
            name: 'TDS_AMT', field: 'TDS_AMT', displayName: 'TDS Amt', enableFiltering: false, width: '8%'
        }
        , {
            name: 'VOUCHER_NO', field: 'VOUCHER_NO', headerCellTemplate: '<div class="">Voucher<br>No</div>', enableFiltering: false, width: '8%', cellTemplate:
                '<input type="text"   ng-disabled="row.entity.ROW_NO !=0" ng-model="row.entity.VOUCHER_NO"  class="pl-sm" />'
        }
        , {
            name: 'VOUCHER_DATE', field: 'VOUCHER_DATE', headerCellTemplate: '<div class="">Voucher<br>Date</div>', enableFiltering: false, width: '8%', cellTemplate:
                '<div class="simple-date2">'
                + '<div class="input-group date">'
                + '<div class="input-group-prepend">'
                + '</div>'
                + '<input  type="text" readonly  ng-disabled="row.entity.ROW_NO !=0" placeholder="dd/mm/yyyy" ng-model="row.entity.VOUCHER_DATE" value="row.entity.VOUCHER_DATE"  class="pl-sm" datepicker >'
                + '</div>'
                + '</div>'
        }
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remarks', enableFiltering: false, width: '15%', cellTemplate:
                '<input type="text"   ng-disabled="row.entity.ROW_NO !=0" ng-model="row.entity.REMARKS"  class="pl-sm" />'
        }

        , {
            name: 'Action', displayName: 'Action', width: '10%', pinnedRight: true, enableFiltering: false, enableColumnMenu: false, pinnedRight: true, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO == 0" ng-click="grid.appScope.addNewRow()" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                '</div>'
        },

    ];
    $scope.LoadCustomerData = function () {
        InsertOrEditServices.LoadCustomerData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Customers = data.data;

            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

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
    $scope.UnitLoad = function () {
        InsertOrEditServices.GetUnit().then(function (data) {
            $scope.model.UNIT_ID = parseInt(data.data);

            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.UnitNameLoad = function () {
        InsertOrEditServices.GetUnitName().then(function (data) {
            $scope.model.UNIT_NAME = data.data;

            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.AutoCompleteDataLoadForInvoice = function (value, customer_id) {
        if (value.length >= 1) {
            return InsertOrEditServices.LoadSearchableInvoice(value, customer_id).then(function (data) {
                return data.data;
            }, function (error) {
                alert(error);
            });
        }
    }

    $scope.typeaheadSelectedInvoice = function (entity, selectedItem) {
        entity.INVOICE_NO = selectedItem.INVOICE_NO;
    };
    //$scope.AutoCompleteDataLoadForCustomer = function (value) {
    //    return InsertOrEditServices.GetSearchableCustomer($scope.model.COMPANY_ID, value).then(function (data) {
    //        let filteredObj = data.data.filter(function (item1) {
    //            return !$scope.gridOptions.data.some(function (item2) {
    //                return item1.CUSTOMER_ID == item2.CUSTOMER_ID;
    //            });
    //        });

    //        $scope.CustomersList = [];
    //        for (var i = 0; i < filteredObj.length; i++) {
    //            var _customer = {
    //                CUSTOMER_CODE: filteredObj[i].CUSTOMER_CODE,
    //                CUSTOMER_NAME: filteredObj[i].CUSTOMER_NAME,
    //                CUSTOMER_ID: filteredObj[i].CUSTOMER_ID,
    //                CUSTOMER_NAME_CODE: filteredObj[i].CUSTOMER_NAME + ' | CODE:' + filteredObj[i].CUSTOMER_CODE
    //            }
    //            $scope.CustomersList.push(_customer);
    //        }

    //        return $scope.CustomersList;
    //    }, function (error) {
    //        alert(error);
    //    });
    //}

    $scope.AutoCompleteDataLoadForCustomer = function (value) {
        return InsertOrEditServices.GetSearchableCustomer($scope.model.COMPANY_ID, value).then(function (data) {
            $scope.CustomersList = [];
            for (var i = 0; i < data.data.length; i++) {
                var _customer = {
                    CUSTOMER_CODE: data.data[i].CUSTOMER_CODE,
                    CUSTOMER_NAME: data.data[i].CUSTOMER_NAME,
                    CUSTOMER_ID: data.data[i].CUSTOMER_ID,
                    CUSTOMER_NAME_CODE: data.data[i].CUSTOMER_NAME + ' | CODE:' + data.data[i].CUSTOMER_CODE
                }
                $scope.CustomersList.push(_customer);
            }

            return $scope.CustomersList;
        }, function (error) {
            alert(error);
        });
    }
    $scope.typeaheadSelectedCustomer = function (entity, selectedItem) {
        entity.CUSTOMER_ID = selectedItem.CUSTOMER_ID;
        entity.CUSTOMER_CODE = selectedItem.CUSTOMER_CODE;
        $scope.OnCustomerSelected(entity);
    };
    $scope.LoadBranchs = function () {
        InsertOrEditServices.LoadBranchData().then(function (data) {
            $scope.Branchs = data.data;
            $scope.Branchs_Copy = data.data;
            $scope.Banks = [];

            var flags = [], l = $scope.Branchs.length, i;
            for (i = 0; i < l; i++) {
                if (flags[$scope.Branchs[i].BANK_ID]) continue;
                flags[$scope.Branchs[i].BANK_ID] = true;
                $scope.Banks.push($scope.Branchs[i]);
            }
          

            $scope.showLoader = false;
        });
    }

    $scope.OnBankSelected = function (entity) {
        $scope.Branchs = $scope.Branchs_Copy.filter(x => x.BANK_ID == entity.BANK_ID);
        $scope.showLoader = false;
    }
    $scope.LoadCollectionModeData = function () {
        InsertOrEditServices.LoadCollectionModeData().then(function (data) {
            $scope.Modes = data.data;
            $scope.showLoader = false;
        });
    }
    $scope.LoadCustomerDaywiseBalance = function (entity) {
        InsertOrEditServices.LoadCustomerDaywiseBalance(parseInt(entity.CUSTOMER_ID)).then(function (data) {
            if (data.data.length > 0) {
                entity.BALANCE = data.data[0].CLOSING_INV_BALANCE;
                entity.BALANCE_TDS_AMT = data.data[0].CLOSING_TDS_BALANCE;
                $scope.onChangeCollectionAmt(entity);
            } else {
                entity.BALANCE = 0;
                entity.BALANCE_TDS_AMT = 0;
                $scope.onChangeCollectionAmt(entity);
            }
            $scope.showLoader = false;
        });
    }
    $scope.LoadCustomerDaywiseBalanceReturn = function (entity) {
        InsertOrEditServices.LoadCustomerDaywiseBalance(parseInt(entity.CUSTOMER_ID)).then(function (data) {
            entity.BALANCE = data.data[0].CLOSING_INV_BALANCE;
            entity.BALANCE_TDS_AMT = data.data[0].CLOSING_TDS_BALANCE;
        });
    }

    $scope.onMemoCostAmt = function (entity) {
        if (entity.MEMO_COST != undefined && isNaN(entity.MEMO_COST) == false && entity.MEMO_COST > 0) {
            entity.NET_COLLECTION_AMT = entity.NET_COLLECTION_AMT + entity.MEMO_COST;
        }
    }

    $scope.onChangeCollectionAmt = function (entity) {
        //if (entity.COLLECTION_AMT > entity.BALANCE) {
        //    alert("The collection amount cannot exceed the customer's balance.");
        //    notificationservice.Notification("The collection amount cannot exceed the customer's balance.", 1, 'Data Save Successfully !!');
        //    entity.COLLECTION_AMT = entity.BALANCE;
            
        //}


        let dt = $scope.gridOptions.data.filter(x => x.CUSTOMER_ID == entity.CUSTOMER_ID && x.ROW_NO != 0);
        if (dt.length > 0) {
            let tds_previous = 0;

            for (var i = 0; i < dt.length; i++) {
                tds_previous += dt[i].TDS_AMT;
            }
            entity.TDS_AMT = parseFloat((entity.BALANCE_TDS_AMT - tds_previous).toFixed(2));
            entity.NET_COLLECTION_AMT = parseFloat((entity.COLLECTION_AMT - entity.TDS_AMT).toFixed(2));
            if (entity.NET_COLLECTION_AMT < 0) {
                entity.NET_COLLECTION_AMT = 0;
                entity.TDS_AMT = entity.COLLECTION_AMT;
            }

        } else {
            entity.TDS_AMT = entity.BALANCE_TDS_AMT;
            entity.NET_COLLECTION_AMT = parseFloat((entity.COLLECTION_AMT - entity.TDS_AMT).toFixed(2));
            if (entity.NET_COLLECTION_AMT < 0) {
                entity.NET_COLLECTION_AMT = 0;
                entity.TDS_AMT = entity.COLLECTION_AMT;
            }
        }
        if (entity.TDS_AMT < 0) {
            entity.NET_COLLECTION_AMT = entity.COLLECTION_AMT;
            entity.TDS_AMT = 0;

        }
        if (entity.MEMO_COST != undefined && isNaN(entity.MEMO_COST) == false && entity.MEMO_COST > 0) {
            entity.NET_COLLECTION_AMT = entity.NET_COLLECTION_AMT + entity.MEMO_COST;
        }
          
    }

    


    $scope.LoadCollectionMode = function () {
        var Active = {
            COLLECTION_MODE: 'Debit'
        }
        var InActive = {
            COLLECTION_MODE: 'Credit'
        }
        $scope.Collection_Modes.push(Active);
        $scope.Collection_Modes.push(InActive);
    }

    $scope.ClearForm = function () {
        window.location = "/SalesAndDistribution/CollectionV2/InsertOrEdit";
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
            Controller_Name: 'CollectionV2',
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
    $scope.CompanyLoad();
    $scope.CompanyNameLoad();
    $scope.UnitLoad();
    $scope.UnitNameLoad();
    $scope.LoadCollectionMode();
    $scope.LoadCustomerData();
    $scope.model.BATCH_STATUS = 'Active';
    $scope.model.BATCH_POSTING_STATUS = 'Debit';
    $scope.LoadCollectionModeData()
    $scope.LoadBranchs();
    // This Method work is Edit Data Loading
    $scope.GetEditDataById = function (value) {
        if (value != undefined && value.length > 0) {
            $scope.showLoader = true;
            InsertOrEditServices.GetEditDataById(value, $scope.model.UNIT_ID).then(function (data) {
                if (data.data != null && data.data != "" && data.data[1].length > 0) {
                    $scope.CompanyLoad();
                    $scope.CompanyNameLoad();
                    $scope.UnitLoad();
                    $scope.UnitNameLoad();
                    $scope.model = data.data[0][0];
                    $scope.gridOptions.data = data.data[1];
                    for (var i = 0; i < $scope.gridOptions.data.length; i++) {
                        $scope.gridOptions.data[i].BANK_ID = $scope.gridOptions.data[i].BANK_ID.toString();
                        $scope.gridOptions.data[i].BRANCH_ID = $scope.gridOptions.data[i].BRANCH_ID.toString();;

                    }
                    $scope.addNewRow();

                    //for (var i = 0; i < $scope.gridOptions.data.length; i++) {
                    //    if (i > 0) {
                    //        $scope.OnCustomerSelected($scope.gridOptions.data[i]);
                    //    }
                    //}
                  
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
        if ($scope.gridOptions.data.find((x) => x.ROW_NO == 0).CUSTOMER_ID > 0)
        {
            if (confirm("You selected a customer for collection at row zero but did not add them to the list. Do you want to save without including this customer?")) {
                // Code to execute if user clicks "OK" in the confirmation dialog
            } else {
                $scope.showLoader = false;
                return;
            }
        }

        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);
        $scope.model.VERSION_NO = "2";
        $scope.gridOptions.data = $scope.gridOptions.data.filter((x) => x.ROW_NO !== 0);
        $scope.model.Collection_Dtls = $scope.gridOptions.data;
        if ($scope.gridOptions.data.length > 0) {
            for (var i = 0; i < $scope.model.Collection_Dtls.length; i++) {
                $scope.model.Collection_Dtls[i].INVOICE_NO = $scope.model.Collection_Dtls[i].INVOICE_NO == null || $scope.model.Collection_Dtls[i].INVOICE_NO == undefined || $scope.model.Collection_Dtls[i].INVOICE_NO == NaN ? '' : $scope.model.Collection_Dtls[i].INVOICE_NO;
                $scope.model.Collection_Dtls[i].BRANCH_ID = $scope.model.Collection_Dtls[i].BRANCH_ID;
                const findendex = $scope.Branchs.findIndex(x => x.BRANCH_ID == $scope.model.Collection_Dtls[i].BRANCH_ID);
                if (findendex != -1) {
                    $scope.model.Collection_Dtls[i].BANK_ID = $scope.Branchs[findendex].BANK_ID;
                }

                var data = $scope.Customers.findIndex(x => x.CUSTOMER_CODE == $scope.model.Collection_Dtls[i].CUSTOMER_CODE)
                if (data != -1) {
                    $scope.model.Collection_Dtls[i].CUSTOMER_ID = parseInt($scope.Customers[data].CUSTOMER_ID);
                }

            }
            InsertOrEditServices.AddOrUpdate(model).then(function (data) {
                if (data.data.Status == 1) {
                    $scope.showLoader = false;
                    $scope.GetEditDataById(data.data.Key);
                    notificationservice.Notification(data.data.Status, 1, 'Data Save Successfully !!');
                }
                else {
                    notificationservice.Notification(data.data.Status, 1, 'Data Save Successfully !!');
                    $scope.showLoader = false;
                    $scope.addNewRow();
                    console.log(model)
                }
            });
        } else {
            $scope.showLoader = false;
            notificationservice.Notification("No Data Found In Details Section", 1, 'Data Save Successfully !!');
            $scope.gridOptions.data[0] = $scope.GridDefalutData();
        }
    }
}]);