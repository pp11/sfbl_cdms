ngApp.controller('ngGridCtrl', ['$scope', 'GiftItemReceiveServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, GiftItemReceiveServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

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

    $scope.model = {
        GIFT_ITEM_ID: 0,
        GIFT_ITEM_NAME: '',
        REMARKS: '',
        STATUS: 'Active',
        RECEIVE_ID : 0,
        RECEIVE_DATE: $scope.formatDate(new Date()),
        SUPPLIER_ID: 0,
        CHALLAN_NO: '',
        CHALLAN_DATE: '',
        BATCH_NO: '',
        BATCH_ID: 0,
        GIFT_ITEM_ID: 0,
        GIFT_ITEM_PRICE: 0,
        RECEIVE_QTY: 0,
        RECEIVE_AMOUNT: 0,
        RECEIVE_STATUS: 'Active',
        COMPANY_ID: 0,
        UNIT_ID: 0,
        RECEIVED_BY_ID: 0,
        REMARKS: ''
    }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];
    $scope.Gifts = [];
    $scope.Suppliers = [];

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("GiftItem Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false,  width: '50'
        }
        , { name: 'RECEIVE_ID', field: 'RECEIVE_ID', visible: false }

        , { name: 'GIFT_ITEM__ID', field: 'GIFT_ITEM_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , { name: 'SUPPLIER_ID', field: 'SUPPLIER_ID', visible: false }
        , { name: 'UNIT_ID', field: 'UNIT_ID', visible: false }

     
        , {
            name: 'GIFT_ITEM_NAME', field: 'GIFT_ITEM_NAME', displayName: 'Name', enableFiltering: true, width: '12%'    
        }
        , {
            name: 'GIFT_ITEM_PRICE', field: 'GIFT_ITEM_PRICE', displayName: 'Price', enableFiltering: true, width: '8%'
        }
        , {
            name: 'RECEIVE_QTY', field: 'RECEIVE_QTY', displayName: 'Qty', enableFiltering: true, width: '8%'
        }
        , {
            name: 'RECEIVE_AMOUNT', field: 'RECEIVE_AMOUNT', displayName: 'Amount', enableFiltering: true, width: '8%'
        }
        , {
            name: 'RECEIVE_STATUS', field: 'RECEIVE_STATUS', displayName: 'Status', enableFiltering: true, width: '8%'
        }
        , {
            name: 'CHALLAN_NO', field: 'CHALLAN_NO', displayName: 'Challan No', enableFiltering: true, width: '8%'
        }
        , {
            name: 'CHALLAN_DATE', field: 'CHALLAN_DATE', displayName: 'Challan Date', enableFiltering: true, width: '8%'
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch', enableFiltering: true, width: '8%'
        }
        , {
            name: 'SUPPLIER_NAME', field: 'SUPPLIER_NAME', displayName: 'Supplier', enableFiltering: true, width: '12%'
        }
        , {
            name: 'RECEIVED_BY_NAME', field: 'RECEIVED_BY_NAME', displayName: 'Received By', enableFiltering: true, width: '8%'
        }
       
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '8%'
        }
        ,{
            name: 'Action', displayName: 'Action', width: '18%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];
    $scope.gridOptionsList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.DataLoad = function (companyId) {
        $scope.showLoader = true;

        GiftItemReceiveServices.LoadData(companyId).then(function (data) {
            
            
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    $scope.LoadSupplierData = function () {

        GiftItemReceiveServices.LoadSupplierData(0).then(function (data) {
            $scope.Suppliers = data.data;
            

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.LoadGiftItemData = function () {

        GiftItemReceiveServices.LoadGiftItemData(0).then(function (data) {
            
            $scope.Gifts = data.data;

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }

    $scope.GiftCalculation = function () {
        
        $scope.model.RECEIVE_AMOUNT =    $scope.model.RECEIVE_QTY * $scope.model.GIFT_ITEM_PRICE

    }


    $scope.CompanyLoad = function () {

        GiftItemReceiveServices.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.CompanyNameLoad = function () {

        GiftItemReceiveServices.GetCompanyName().then(function (data) {
            $scope.model.COMPANY_NAME = data.data;


            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }

    $scope.LoadStatus = function () {
        var Active = {
            STATUS: 'Active'
        }
        var InActive = {
            STATUS: 'InActive'
        }
        $scope.Status.push(Active);
        $scope.Status.push(InActive);

    }

    $scope.ClearForm = function () {
        $scope.model.GIFT_ITEM_ID = 0;
        $scope.model.GIFT_ITEM_NAME = '';
        $scope.model.STATUS = 'Active';
        $scope.model.REMARKS = '';
    }

    $scope.EditData = function (entity) {
        $scope.model.GIFT_ITEM_ID = entity.GIFT_ITEM_ID;
        $scope.model.GIFT_ITEM_NAME = entity.GIFT_ITEM_NAME;
        $scope.model.REMARKS = entity.REMARKS;
        $scope.model.RECEIVE_ID = entity.RECEIVE_ID;
        $scope.model.RECEIVE_DATE = entity.RECEIVE_DATE;
        $scope.model.SUPPLIER_ID = entity.SUPPLIER_ID;
        $scope.model.CHALLAN_NO = JSON.stringify(entity.CHALLAN_NO);
        $scope.model.CHALLAN_DATE = entity.CHALLAN_DATE;
        $scope.model.BATCH_NO = entity.BATCH_NO;
        $scope.model.BATCH_ID = entity.BATCH_ID;
        $scope.model.GIFT_ITEM_ID = entity.GIFT_ITEM_ID;
        $scope.model.GIFT_ITEM_PRICE = entity.GIFT_ITEM_PRICE;
        $scope.model.RECEIVE_QTY = entity.RECEIVE_QTY;
        $scope.model.RECEIVE_AMOUNT = entity.RECEIVE_AMOUNT;
        $scope.model.RECEIVE_STATUS = entity.RECEIVE_STATUS;
        $scope.model.UNIT_ID = entity.UNIT_ID;
        $scope.model.RECEIVED_BY_ID = parseInt(entity.RECEIVED_BY_ID);
        $scope.model.REMARKS = entity.REMARKS == null ? '' : entity.REMARKS;
        $interval(function () {
            $scope.LoadGIFT_ITEM_ID();
        }, 800, 2);
        $interval(function () {
            $scope.LoadSUPPLIER_ID();
        }, 800, 2);
    }
    $scope.LoadGIFT_ITEM_ID = function () {
        $('#GIFT_ITEM_ID').trigger('change');
    }
    $scope.LoadSUPPLIER_ID = function () {
        $('#SUPPLIER_ID').trigger('change');
    }
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'GiftItemReceiving',
            Action_Name: 'GiftItemReceive'
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
   
    $scope.DataLoad(0);
    $scope.GetPermissionData();
    $scope.CompanyLoad();
    $scope.CompanyNameLoad();
    $scope.LoadSupplierData();
    $scope.LoadGiftItemData();

    $scope.LoadStatus();


    $scope.SaveData = function (model) {
        $scope.showLoader = true;

        model.SUPPLIER_ID = parseInt(model.SUPPLIER_ID);
        model.GIFT_ITEM_ID = parseInt(model.GIFT_ITEM_ID);

        GiftItemReceiveServices.AddOrUpdate(model).then(function (data) {
            notificationservice.Notification(data.data.Status, 1, 'Data Save Successfully !!');
            
            if (data.data.Status == 1) {
                $scope.DataLoad(0);
                var item = JSON.parse(data.data.Key);
                $scope.model.RECEIVE_ID = parseInt(item[0].RECEIVE_ID);
                $scope.model.BATCH_NO = item[0].BATCH_NO;
                $scope.showLoader = false;
            }
            else {
                $scope.showLoader = false;
            }
            $interval(function () {
                $scope.LoadGIFT_ITEM_ID();
            }, 800, 2);
            $interval(function () {
                $scope.LoadSUPPLIER_ID();
            }, 800, 2);
        });
    }

}]);

