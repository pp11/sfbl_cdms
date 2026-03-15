ngApp.controller('ngGridCtrl', ['$scope', 'InsertOrEditServices', 'CustomerInfoServices', 'MarketInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InsertOrEditServices, CustomerInfoServices, MarketInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { COMPANY_ID: 0, CUSTOMER_ID: 0, CUSTOMER_CODE: '', CUSTOMER_MARKET_MST_STATUS: 'Active', EFFECT_START_DATE: '', EFFECT_END_DATE: '', REMARKS: ''}

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];
    $scope.Customers = [];
    $scope.CustomersList = [];
    $scope.Markets = [];
    $scope.MarketsList = [];
    $scope.MarketsListAll = [];
    $scope.model.INVOICE_FLAG = 'NO';
    $scope.invoiceFlag = [];
    $scope.Dtl_Status = [];
    
    $scope.DefalutData = function () {
        return { COMPANY_ID: 0, CUSTOMER_ID: 0, CUSTOMER_CODE: '', CUSTOMER_MARKET_MST_STATUS: 'Active', EFFECT_START_DATE: '', EFFECT_END_DATE: '', REMARKS: '' }
    }

    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 1, COMPANY_ID: 0, MARKET_ID: 0, MARKET_CODE: '', CUSTOMER_MARKET_DTL_STATUS: 'Active', EFFECT_START_DATE: '', EFFECT_END_DATE: '', REMARKS: '', INVOICE_FLAG: 'NO'
        }
    }

    $scope.gridOptions = (gridregistrationservice.GridRegistration("Customer Info"));
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptions.data = [];
    //Grid Register
    $scope.gridOptions = {
        data: [$scope.GridDefalutData()]
    }
    
    
    //Generate New Row No
    $scope.rowNumberGenerate = function () {
        
        for (let i = 0; i < $scope.gridOptions.data.length; i++) {
            
            $scope.gridOptions.data[i].ROW_NO = i;
            if ($scope.gridOptions.data[i].MARKET_CODE == '') {
                $scope.gridOptions.data[i].MARKET_ID = 0;
                $scope.gridOptions.data[i].MARKET_NAME = '';
                $scope.gridOptions.data[i].MARKET_NAME_CODE = '';

            }
           
           
        }

    }




    //Add New Row

    $scope.addNewRow = () => {
        
        if ($scope.gridOptions.data.length > 0 && $scope.gridOptions.data[0].MARKET_CODE != null && $scope.gridOptions.data[0].MARKET_CODE != '' && $scope.gridOptions.data[0].MARKET_CODE != 'undefined') {
            var result =  $scope.CheckDateValidation($scope.gridOptions.data[0]);
            if (result == "true") {
                var newRow = {
                    ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, CUSTOMER_MARKET_MST_ID: $scope.gridOptions.data[0].CUSTOMER_MARKET_MST_ID, CUSTOMER_MARKET_DTL_ID: $scope.gridOptions.data[0].CUSTOMER_MARKET_DTL_ID, MARKET_ID: $scope.gridOptions.data[0].MARKET_ID, MARKET_CODE: $scope.gridOptions.data[0].MARKET_CODE, CUSTOMER_MARKET_DTL_STATUS: $scope.gridOptions.data[0].CUSTOMER_MARKET_DTL_STATUS, EFFECT_START_DATE: $scope.gridOptions.data[0].EFFECT_START_DATE, EFFECT_END_DATE: $scope.gridOptions.data[0].EFFECT_END_DATE, REMARKS: $scope.gridOptions.data[0].REMARKS, INVOICE_FLAG: $scope.gridOptions.data[0].INVOICE_FLAG
                }
                var newRowSelected = {
                    MARKET_CODE: $scope.gridOptions.data[0].MARKET_CODE
                }
                $scope.MarketsList.push(newRowSelected);
                $scope.gridOptions.data.push(newRow);
                $scope.gridOptions.data[0] = $scope.GridDefalutData();

            }
            
        } else {
            notificationservice.Notification("Please Enter Valid Market First", "", 'Only Single Row Left!!');

        }
        $scope.rowNumberGenerate();

    };
    $scope.EditItem = (entity) => {
        
        if ($scope.gridOptions.data.length > 0) {
           
            var newRow = {
                ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, CUSTOMER_MARKET_MST_ID: entity.CUSTOMER_MARKET_MST_ID, CUSTOMER_MARKET_DTL_ID: entity.CUSTOMER_MARKET_DTL_ID, MARKET_ID: entity.MARKET_ID, MARKET_CODE: entity.MARKET_CODE, CUSTOMER_MARKET_DTL_STATUS: entity.CUSTOMER_MARKET_DTL_STATUS, EFFECT_START_DATE: entity.EFFECT_START_DATE, EFFECT_END_DATE: entity.EFFECT_END_DATE, REMARKS: entity.REMARKS, INVOICE_FLAG: entity.INVOICE_FLAG
            }
            $scope.gridOptions.data[0] = newRow;


        } else {
            notificationservice.Notification("Please Select Valid data", "", 'Only Single Row Left!!');

        }
        $scope.rowNumberGenerate();
        $scope.removeItem(entity);
    };
    $scope.CheckDateValidation = function (entity) {
        
        if (entity.CUSTOMER_MARKET_DTL_ID > 0 && entity.ROW_NO == 0) {
            return "true";
        }
        var today = new Date();
        var day = entity.EFFECT_START_DATE.substring(0, 2)
        var month = entity.EFFECT_START_DATE.substring(3, 5)
        var year = entity.EFFECT_START_DATE.substring(6, 10)
        var StartDate = new Date(year, month, day);
        if (entity.EFFECT_START_DATE.substring(2, 3) != "/" || entity.EFFECT_START_DATE.substring(5, 6) != "/") {
            alert("Please Right Format of Start Date. Ex: dd/mm/yyyy (day/month/year)");
            entity.EFFECT_START_DATE = '';
            return "false";
        }
        var todate = today.getDate();
        var tomonth = today.getMonth();
        if (StartDate < today) {
            alert("Start Date Can not be any Previous Days");
            entity.EFFECT_START_DATE = '';

            return "false";
        }
        else if (parseInt(month) == parseInt(tomonth) && parseInt(day) < parseInt(todate)) {
            alert("Start Date Can not be any Previous Days");
            entity.EFFECT_START_DATE = '';
        }
        if (entity.EFFECT_END_DATE != null && entity.EFFECT_END_DATE != '') {
            var day1 = entity.EFFECT_END_DATE.substring(0, 2)
            var month1 = entity.EFFECT_END_DATE.substring(3, 5)
            var year1 = entity.EFFECT_END_DATE.substring(6, 10)
            var EndDate = new Date(year1, month1, day1);
            if (entity.EFFECT_END_DATE.substring(2, 3) != "/" || entity.EFFECT_END_DATE.substring(5, 6) != "/") {
                alert("Please Right Format of End Date. Ex: dd/mm/yyyy (day/month/year)");
                entity.EFFECT_END_DATE = '';

                return "false";
            }
            if (EndDate < today) {
                alert("End Date Can not be any Previous Days");
                entity.EFFECT_END_DATE = '';
                return "false";
            }
            if (EndDate < StartDate) {
                alert("End Date Can not be less than Start Day");
                entity.EFFECT_END_DATE = '';
                return "false";
            }
        }






        return "true"
    }
    $scope.CheckENDDateValidation = function (entity) {
        
        var Startdate = new Date();


        var StartDate = new Date(entity.EFFECT_END_DATE);
        if (entity.EFFECT_START_DATE == null || entity.EFFECT_START_DATE == '' ) {
            alert("End date can not be entered before entering Start Date");
            entity.EFFECT_END_DATE = '';
            return "false";
        }
    }
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

    $scope.gridOptions.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false,  width: '50'
        }
        , {
            name: 'CUSTOMER_MARKET_MST_ID', field: 'CUSTOMER_MARKET_MST_ID', visible: false
        }
        , {
            name: 'CUSTOMER_MARKET_DTL_ID', field: 'CUSTOMER_MARKET_DTL_ID', visible: false
        }
        , {
            name: 'MARKET_ID', field: 'MARKET_ID', visible: false
        }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , {
            name: 'MARKET_NAME', field: 'MARKET_NAME', displayName: 'Market', enableFiltering: false, width: '35%', cellTemplate:
                '<select class="select2-single form-control" data-select2-id="{{row.entity.MARKET_ID_GR}}" id="MARKET_CODE" ng-disabled="row.entity.ROW_NO !=0 || row.entity.CUSTOMER_MARKET_DTL_ID>0"'+
                    'name="MARKET_CODE" ng-model="row.entity.MARKET_CODE" style="width:100%" >' +
                    '<option ng-repeat="item in grid.appScope.Markets" ng-selected="item.MARKET_CODE == row.entity.MARKET_CODE" value="{{item.MARKET_CODE}}">{{ item.MARKET_NAME }} | Code: {{ item.MARKET_CODE }}</option>' +
                '</select>'
        }
        , {
            name: 'MARKET_CODE', field: 'MARKET_CODE', displayName: 'Code', enableFiltering: false, visible:false, width: '15%', cellTemplate:
                '<input type="text"  ng-model="row.entity.MARKET_CODE" disabled="true"  class="pl-sm" />'
        }
        , {
            name: 'EFFECT_START_DATE', field: 'EFFECT_START_DATE', displayName: 'Start', enableFiltering: false, width: '15%', cellTemplate:
                '<div class="simple-date2">'
                + '<div class="input-group date">'
                + '<div class="input-group-prepend">'
                + '</div>'
                + '<input  type="text" readonly datepicker class="form-control"  ng-disabled="row.entity.ROW_NO !=0" ng-model="row.entity.EFFECT_START_DATE" placeholder="dd/mm/yyyy" id="EFFECT_START_DATE">'
                + '</div>'
                + '</div>'
        }
        , {
            name: 'EFFECT_END_DATE', field: 'EFFECT_END_DATE', displayName: 'End', enableFiltering: false, width: '15%', cellTemplate:
                '<div class="simple-date2">'
                + '<div class="input-group date">'
                + '<div class="input-group-prepend">'
                + '</div>'
                + '<input  type="text" readonly datepicker ng-change="grid.appScope.CheckENDDateValidation(row.entity)" class="form-control"  ng-disabled="row.entity.ROW_NO !=0" ng-model="row.entity.EFFECT_END_DATE" placeholder="dd/mm/yyyy"  id="EFFECT_END_DATE">'
                + '</div>'
                + '</div>'
        }
        
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: false,visible:false, width: '15%', cellTemplate:
                '<input type="text"  ng-disabled="row.entity.ROW_NO !=0" ng-model="row.entity.REMARKS"  class="pl-sm" />'


        }
        , {
            name: 'CUSTOMER_MARKET_DTL_STATUS', field: 'CUSTOMER_MARKET_DTL_STATUS', displayName: 'Status', enableFiltering: false, width: '15%', cellTemplate:
                '<select class="form-control"  ng-disabled="row.entity.ROW_NO !=0" id="CUSTOMER_MARKET_DTL_STATUS"'
                +'name="CUSTOMER_MARKET_DTL_STATUS" ng-model="row.entity.CUSTOMER_MARKET_DTL_STATUS" style="width:100%">'
                +'<option ng-repeat="item in grid.appScope.Status" value="{{item.STATUS}}" ng-selected="item.STATUS == row.entity.CUSTOMER_MARKET_DTL_STATUS" >{{ item.STATUS }}</option>'
                +'</select>'
        }
        , {
            name: 'INVOICE_FLAG', field: 'INVOICE_FLAG', displayName: 'Invoice Flag', enableFiltering: false, width: '15%', cellTemplate:
                '<select  class= "form-control" id = "INVOICE_FLAG" '+
                  'name ="INVOICE_FLAG" ng-model="row.entity.INVOICE_FLAG" style = "width:100%" ng - change="typeaheadSelectedCustomer()" >'+
                     ' <option ng-repeat="item in grid.appScope.invoiceFlag" ng-selected="item.INVOICE_FLAG == row.entity.INVOICE_FLAG" value="{{item.INVOICE_FLAG}}">{{ item.INVOICE_FLAG }}</option>'+
                '</select>'
        }
        
        ,{
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.EditItem(row.entity)" type="button" class="btn btn-outline-success mb-1"><img src="/img/edit-icon.png" height="15px" ></button>' +

                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO == 0" ng-click="grid.appScope.addNewRow()" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                '</div>'
        },

    ];


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
    $scope.AutoCompleteDataLoadForCustomer = function (value) {
            

            return CustomerInfoServices.GetSearchableCustomer($scope.model.COMPANY_ID, value).then(function (data) {
                
                $scope.CustomersList = [];
                for (var i = 0; i < data.data.length; i++) {
                    var _customer = {
                        CUSTOMER_CODE: data.data[i].CUSTOMER_CODE,
                        CUSTOMER_NAME: data.data[i].CUSTOMER_NAME,
                        CUSTOMER_ID: data.data[i].CUSTOMER_ID,
                        CUSTOMER_NAME_CODE: data.data[i].CUSTOMER_NAME + ' (' + data.data[i].CUSTOMER_CODE + ')',
                    }
                    $scope.CustomersList.push(_customer);

                }

                return $scope.CustomersList;
            }, function (error) {
                alert(error);
                

                
            });
    }

    $scope.SearchableMarketLoad = function (value) {
            

            return MarketInfoServices.GetSearchableMarket($scope.model.COMPANY_ID, value).then(function (data) {
                
                $scope.MarketsList = [];
                for (var i = 0; i < data.data.length; i++) {
                    var _market = {
                        MARKET_CODE: data.data[i].MARKET_CODE,
                        MARKET_NAME: data.data[i].MARKET_NAME,
                        MARKET_ID: data.data[i].MARKET_ID,
                        MARKET_NAME_CODE: data.data[i].MARKET_NAME + ' (' + data.data[i].MARKET_CODE + ')',
                    }
                    $scope.MarketsList.push(_market);

                }

                return $scope.MarketsList;
            }, function (error) {
                alert(error);
                

                
            });
        
    }

    $scope.typeaheadSelectedCustomer = function () {
        
        const searchIndex = $scope.Customers.findIndex((x) => x.CUSTOMER_CODE == $scope.model.CUSTOMER_CODE);

        $scope.model.CUSTOMER_ID = $scope.Customers[searchIndex].CUSTOMER_ID;
        $scope.model.CUSTOMER_CODE = $scope.Customers[searchIndex].CUSTOMER_CODE;
        $scope.model.CUSTOMER_NAME = $scope.Customers[searchIndex].CUSTOMER_NAME;

    };

    //$scope.typeaheadSelectedMarket = function (entity) {
    //    
    //    const searchIndex = $scope.Markets.findIndex((x) => x.MARKET_CODE == entity.MARKET_CODE);
    //    if (searchIndex != -1) {
    //        entity.MARKET_ID = $scope.Markets[searchIndex].MARKET_ID;
    //        entity.MARKET_CODE = $scope.Markets[searchIndex].MARKET_CODE;
    //        entity.MARKET_NAME = $scope.Markets[searchIndex].MARKET_NAME;
    //        $scope.Markets = $scope.Markets.filter((x) => x.MARKET_CODE !== entity.MARKET_CODE);

    //    }
    //   };
    $scope.typeaheadSelectedMarket = function (entity, selectedItem) {
        
        entity.MARKET_ID = selectedItem.MARKET_ID;
        entity.MARKET_CODE = selectedItem.MARKET_CODE;
        entity.MARKET_NAME = selectedItem.MARKET_NAME;
        entity.MARKET_NAME_CODE = selectedItem.MARKET_NAME_CODE;

    };

    $scope.CustomersLoad = function () {
        
        $scope.showLoader = true;

        InsertOrEditServices.GetExistingCustomer($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.Customers = data.data;

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.MarketsLoad = function () {

        $scope.showLoader = true;

        InsertOrEditServices.GetExistingMarket($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.Markets = data.data;
            $scope.MarketsListAll = data.data;

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
    $scope.LoadInvoiceFlag = function () {

        var Active = {
            INVOICE_FLAG: 'NO'
        }
        var InActive = {
            INVOICE_FLAG: 'YES'
        }
        $scope.invoiceFlag.push(Active);
        $scope.invoiceFlag.push(InActive);

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
        window.location = "/SalesAndDistribution/CustomerMarketRelation/InsertOrEdit";

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
            Controller_Name: 'CustomerMarketRelation',
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

    $scope.formatRepo = function (repo) {
        
        if (repo.loading) {
            return repo.text;
        }
        if (repo.text != "") {
            const textArray = repo.text.split("--");
            let text_title = textArray[0];
            let text_title_2 = textArray[1];
            var $container = $(
                "<div class='select2-result-repository clearfix'>" +
                "<div class='select2-result-repository__meta'>" +
                "<div class='select2-result-repository__title' style='font-size:14px;font-weight:700'></div>" +
                "<div class='select2-result-repository__watchers' style='font-size:12px;font-weight:700'> <span>Code: </span>  </div>" +
                "</div>" +
                "</div>"
            );

            $container.find(".select2-result-repository__title").text(text_title);
            $container.find(".select2-result-repository__watchers").append(text_title_2);


        }

        return $container;
    }

    $scope.formatRepoSelection = function (repo) {
        return repo.text.split("--")[0];
    }

    $(".select2-single-Customer").select2({
        placeholder: "Select",
        templateResult: $scope.formatRepo,
        templateSelection: $scope.formatRepoSelection
    });

    $scope.GetPermissionData();
    $scope.CompanyLoad();
    $scope.CompanyNameLoad();

    $scope.CustomersLoad();
    $scope.LoadStatus();
    $scope.MarketsLoad();
    $scope.LoadInvoiceFlag();
   
    // This Method work is Edit Data Loading
    $scope.GetEditDataById = function (value) {
        
        if (value != undefined && value.length > 0) {
            InsertOrEditServices.GetEditDataById(value).then(function (data) {
                
                if (data.data != null && data.data.customer_Market_Dtls !=null && data.data.customer_Market_Dtls.length>0) {
                    $scope.model = data.data;
                    $scope.CompanyLoad();
                    $scope.CompanyNameLoad();

                    if ($scope.model.CUSTOMER_MARKET_MST_STATUS == null) {
                        $scope.model.CUSTOMER_MARKET_MST_STATUS = 'Active';
                    }
                    if (data.data.customer_Market_Dtls != null) {
                        $scope.gridOptions.data = data.data.customer_Market_Dtls;

                    }
                    var CustomerData = {
                        CUSTOMER_CODE: $scope.model.CUSTOMER_CODE,
                        CUSTOMER_NAME: $scope.model.CUSTOMER_NAME,
                        CUSTOMER_ID: $scope.model.CUSTOMER_ID,
                    }
                    $scope.Customers.push(CustomerData);
                    for (var i in $scope.gridOptions.data) {

                        var MarketData = {
                            MARKET_CODE: $scope.gridOptions.data[i].MARKET_CODE,
                            MARKET_NAME: $scope.gridOptions.data[i].MARKET_NAME,
                            MARKET_ID: $scope.gridOptions.data[i].MARKET_ID,
                        }
                        $scope.Markets.push(MarketData);
                    }
                    $scope.addNewRow();

                    
                }
                
                $scope.rowNumberGenerate();
                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                
            });
        }
    }
    $scope.LoadFormData = function () {
        
        $scope.showLoader = true;

        InsertOrEditServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.showLoader = false;


            for (var i in data.data) {
                
                if (data.data[i].CUSTOMER_CODE
                    == $scope.model.CUSTOMER_CODE) {
                    
                    $scope.model.CUSTOMER_MARKET_MST_ID_ENCRYPTED = data.data[i].CUSTOMER_MARKET_MST_ID_ENCRYPTED;
                    $scope.GetEditDataById($scope.model.CUSTOMER_MARKET_MST_ID_ENCRYPTED);
                    $scope.addNewRow();
                }
            }
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.SaveData = function (model) {
        let hasDupArray = $scope.gridOptions.data.filter(function (object) {
            return object.INVOICE_FLAG == 'YES' && object.ROW_NO !== 0;
        });

        if (hasDupArray.length > 1) {
            alert("Data has duplicate market invoice flag values set to 'YES'.");
            $scope.showLoader = false;
            return;
        } 
        $scope.showLoader = true;
        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);
        $scope.gridOptions.data = $scope.gridOptions.data.filter((x) => x.ROW_NO !== 0);
        for (var i in $scope.gridOptions.data) {
            const searchIndex = $scope.Markets.findIndex((x) => x.MARKET_CODE == $scope.gridOptions.data[i].MARKET_CODE);
            if (searchIndex != -1) {
                $scope.gridOptions.data[i].MARKET_ID = $scope.Markets[searchIndex].MARKET_ID;
                $scope.gridOptions.data[i].MARKET_CODE = $scope.Markets[searchIndex].MARKET_CODE;
                $scope.gridOptions.data[i].MARKET_NAME = $scope.Markets[searchIndex].MARKET_NAME;

            }
        }
        
        $scope.model.customer_Market_Dtls = $scope.gridOptions.data;

        InsertOrEditServices.AddOrUpdate(model).then(function (data) {
            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;
                $scope.LoadFormData();
            }
            else {
                $scope.showLoader = false;
                $scope.addNewRow();

            }
        });
    }

}]);

