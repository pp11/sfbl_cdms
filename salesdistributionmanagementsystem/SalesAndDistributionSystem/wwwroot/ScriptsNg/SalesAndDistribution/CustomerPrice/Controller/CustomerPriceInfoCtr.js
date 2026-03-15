ngApp.controller('ngGridCtrl', ['$scope', 'CustomerPriceInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, CustomerPriceInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {
        CUSTOMER_PRICE_MSTID: 0,
        COMPANY_ID: 0,
        CUSTOMER_ID: "",
        CUSTOMER_CODE: null,
        EFFECT_START_DATE: ""
        , EFFECT_END_DATE: ""
        , CUSTOMER_STATUS: ""
        , REMARKS: ""
        , ENTRY_DATE: ""
        , GROUP_ID: ""
        , BRAND_ID: ""
        , CATEGORY_ID: ""
        , BASE_PRODUCT_ID: ""
        , PRODUCT_ID: []
        , SKU_PRICE: 0
        , COMMISSION_FLAG: ""
        , PRICE_FLAG: ""
        , COMMISSION_VALUE: 0
        , COMMISSION_TYPE: ""
        , ADD_COMMISSION1: 0
        , ADD_COMMISSION2: 0
        , customerSkuPriceList: []
        , CUSTOMER_IDs: ""
    }
    $scope.model.ENTRY_DATE = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.model.CUSTOMER_PRICE_MSTID = 0;

    $scope.getPermissions = [];
    $scope.ProductList = [];
    $scope.Companies = [];
    $scope.Unit = [];
    $scope.CustomerData = [];
    $scope.CustomerType = [];
    $scope.ProductList = [];

    $scope.BaseProducts = [];
    $scope.Categories = [];
    $scope.Brands = [];
    $scope.Groups = [];
    $scope.Products = [];
    $scope.existingSKU = [];
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Product Price Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }

    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '10%'
        }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '10%'
        }
        , {
            name: 'PACK_SIZE', field: 'PACK_SIZE', displayName: 'Pack Size', enableFiltering: true, width: '85'
        }
        , {
            name: 'PRICE_FLAG', field: 'PRICE_FLAG', displayName: 'Price Flag', enableFiltering: true, width: '10%'
            , headerCellTemplate: '<div class="">Price<br> Flag<br></div><select id="PRICE_FLAG" ng-change="grid.appScope.SetAlPriceFlag()" class="form-control" ng-model="PRICE_FLAG" ><option  value="Yes">Yes</option><option value="No">No</option></select> '
            , cellTemplate:
                ` <select class="form-control"  ng-model="row.entity.PRICE_FLAG" >
                                        <option  value="Yes">Yes</option>
                                        <option  value="No">No</option>
                                    </select>`
        }
        , {
            name: 'SKU_PRICE', field: 'SKU_PRICE', displayName: 'SKU Price', enableFiltering: true, width: '10%', cellTemplate:
                ` <input type="number" class="form-control" ng-model="row.entity.SKU_PRICE" >`
        }
        , { name: 'COMMISSION_FLAG', field: 'COMMISSION_FLAG', displayName: 'Commission Flag', enableFiltering: true, width: '10%'
            , headerCellTemplate: '<div class="">Commission Flag<br></div><select id="COMMISSION_FLAG" ng-change="grid.appScope.SetAlCommissionFlag()" class="form-control" ng-model="COMMISSION_FLAG" ><option  value="Yes">Yes</option><option value="No">No</option></select> '
            , cellTemplate:
                ` <select class="form-control" ng-model="row.entity.COMMISSION_FLAG" >
                                        <option  value="Yes">Yes</option>
                                        <option  value="No">No</option>
                                    </select>`
        }
        , {
            name: 'COMMISSION_TYPE', field: 'COMMISSION_TYPE', displayName: 'Commission Type', enableFiltering: true, width: '10%'
            , headerCellTemplate: '<div class="">Commission Type<br></div><select id="COMMISSION_TYPE" ng-change="grid.appScope.SetAlCommissionType()" class="form-control" ng-model="COMMISSION_TYPE" ><option  value="PCT">PCT</option><option value="Value">Value</option></select> '
            , cellTemplate:
                ` <select class="form-control" ng-model="row.entity.COMMISSION_TYPE" >
                                        <option  value="PCT">PCT</option>
                                        <option  value="Value">Value</option>
                                    </select>`
        }
        , {
            name: 'COMMISSION_VALUE', field: 'COMMISSION_VALUE', enableFiltering: true, width: '80'
            , headerCellTemplate: '<div class="">Commission Value<br></div><input id="MODEL_COL_FIELD3" ng-model="MODEL_COL_FIELD3" ng-change="grid.appScope.SetAllAddComm3()"  type="text" class="form-control" >'
            , cellTemplate:
                ` <input type="number" step="any" class="form-control" ng-model="row.entity.COMMISSION_VALUE" >`
        }
        , {
            name: 'ADDITIONAL_COMMISSION', field: 'ADDITIONAL_COMMISSION',
            headerCellTemplate: '<div class="">Additional Commission<br></div><input id="MODEL_COL_FIELD" ng-model="MODEL_COL_FIELD" ng-change="grid.appScope.SetAllAddComm()"  type="text" class="form-control" >'
            , enableFiltering: true, width: '80', cellTemplate:
                ` <input type="number" step="any" class="form-control" ng-model="row.entity.ADD_COMMISSION1" >`
        }
        , {
            name: 'ADD_COMMISSION', field: 'ADD_COMMISSION', displayName: 'Add Comm. 2'
            , headerCellTemplate: '<div class="">Additional Commission2<br></div><input id="MODEL_COL_FIELD2" ng-model="MODEL_COL_FIELD2" ng-change="grid.appScope.SetAllAddComm2()"  type="text" class="form-control" >', enableFiltering: true
            , width: '80', cellTemplate:
                ` <input type="number" step="any" class="form-control" ng-model="row.entity.ADD_COMMISSION2" >`
        }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +
                '</div>'
        },

    ]; 
    $scope.LoadCustomerTypeData = function () {
        $scope.showLoader = true;
        CustomerPriceInfoServices.LoadCustomerTypeData($scope.model.COMPANY_ID).then(function (data) {

            $scope.CustomerType = data.data;
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }


 
    $scope.SetAllAddComm = function()
    {
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

    $scope.SetAlCommissionType = function () {
        let value = document.getElementById("COMMISSION_TYPE").value;
        for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
            $scope.gridOptionsList.data[i].COMMISSION_TYPE = value;
        }

    }

    $scope.SetAlCommissionFlag = function () {
        let value = document.getElementById("COMMISSION_FLAG").value;
        for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
            $scope.gridOptionsList.data[i].COMMISSION_FLAG = value;
        }

    }
    $scope.SetAlPriceFlag = function () {
        let value = document.getElementById("PRICE_FLAG").value;
        for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
            $scope.gridOptionsList.data[i].PRICE_FLAG = value;
        }

    }


  
    $scope.GetExistingSku = function () {
        $scope.showLoader = true;
        
        $scope.existingSKU = [];
        CustomerPriceInfoServices.LoadExistingSkuData($scope.model.COMPANY_ID, $scope.model.CUSTOMER_ID).then(function (data) {        
            $scope.existingSKU = data.data;
            $scope.showLoader = false;

        }, function (error) {
            alert(error);
            $scope.showLoader = false;

        });
    }

    $scope.CheckDateValidation = function (entity) {
        
        var today = new Date();
        var day = entity.EFFECT_START_DATE.substring(0, 2)
        var month = entity.EFFECT_START_DATE.substring(3, 5)
        var year = entity.EFFECT_START_DATE.substring(6, 10)
        var StartDate = new Date(year, (month - 1), day);
        if (entity.EFFECT_START_DATE.substring(2, 3) != "/" || entity.EFFECT_START_DATE.substring(5, 6) != "/") {

            alert("Please Right Format of Start Date. Ex: dd/mm/yyyy (day/month/year)");
            entity.EFFECT_START_DATE = '';
            return "false";
        }
        var todate = today.getDate();
        var tomonth = today.getMonth();
        if (StartDate < today) {
            if ((parseInt(month) - 1) == tomonth && todate == parseInt(day)) {

            } else {
                alert("Start Date Can not be any Previous Days");
                entity.EFFECT_START_DATE = '';

                return "false";
            }

        }
        else if (parseInt(month) == parseInt(tomonth) && parseInt(day) < parseInt(todate)) {
            alert("Start Date Can not be any Previous Days");
            entity.EFFECT_START_DATE = '';
        }
        if (entity.EFFECT_END_DATE != null && entity.EFFECT_END_DATE != '') {
            var day1 = entity.EFFECT_END_DATE.substring(0, 2)
            var month1 = entity.EFFECT_END_DATE.substring(3, 5)
            var year1 = entity.EFFECT_END_DATE.substring(6, 10)
            var EndDate = new Date(year1, (month1 - 1), day1);
            if (entity.EFFECT_END_DATE.substring(2, 3) != "/" || entity.EFFECT_END_DATE.substring(5, 6) != "/") {
                alert("Please Right Format of End Date. Ex: dd/mm/yyyy (day/month/year)");
                entity.EFFECT_END_DATE = '';

                return "false";
            }
            if (EndDate < today) {
                if ((parseInt(month1) - 1) == tomonth && todate == parseInt(day1)) {

                } else {
                    alert("End Date Can not be any Previous Days");
                    entity.EFFECT_END_DATE = '';
                    return "false";
                }


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
        
        if (entity.EFFECT_START_DATE == null || entity.EFFECT_START_DATE == '') {
            alert("End date can not be entered before entering Start Date");
            entity.EFFECT_END_DATE = '';
            return "false";
        }
    }
    $scope.ClearForm = function () {
        window.location.href = "/SalesAndDistribution/PriceInfo/CustomerPriceInfo";
    }
    $scope.ClearProductForm = function () {
        $scope.gridOptionsList.data = [];
    }

    
    $scope.DataLoad = function () {
        $scope.showLoader = true;
        CustomerPriceInfoServices.LoadFilteredProduct($scope.model).then(function (data) {
            
            for (var i = 0; i < data.data.length; i++) {
                var indexx = $scope.gridOptionsList.data.findIndex(x => x.SKU_ID == data.data[i].SKU_ID);
                if (indexx == -1) {
                    $scope.gridOptionsList.data.push(data.data[i]);
                }
            }
                
            }, function (error) {
                alert(error);
                $scope.showLoader = false;

            });
        
        $scope.showLoader = false;
    }

    //Generate New Row No
    $scope.rowNumberGenerate = function () {
        for (let i = 0; i < $scope.gridOptionsList.data.length; i++) {
            $scope.gridOptionsList.data[i].ROW_NO = i;
        }
    }
    $scope.removeItem = function (entity) {

        if ($scope.gridOptionsList.data.length > 1) {
            var index = $scope.gridOptionsList.data.indexOf(entity);
            if ($scope.gridOptionsList.data.length > 0) {
                $scope.gridOptionsList.data.splice(index, 1);
            }
            $scope.rowNumberGenerate();


        } else {
            notificationservice.Notification("Only Single Row Left!!", "", 'Only Single Row Left!!');
        }


    }

    $scope.CompanyLoad = function () {

        CustomerPriceInfoServices.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);

            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.CompanyNameLoad = function () {

        CustomerPriceInfoServices.GetCompanyName().then(function (data) {
            $scope.model.COMPANY_NAME = data.data;


            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.LoadBaseProductData = function () {
        $scope.showLoader = true;

        CustomerPriceInfoServices.LoadBaseProductData().then(function (data) {
            $scope.BaseProducts = data.data;
            var _BaseProducts = {
                BASE_PRODUCT_ID: "0",
                BASE_PRODUCT_NAME: "All",
                BASE_PRODUCT_CODE: "ALL",

            }
            $scope.BaseProducts.push(_BaseProducts);

            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }
    $scope.LoadBrandData = function () {
        $scope.showLoader = true;

        CustomerPriceInfoServices.LoadBrandData().then(function (data) {
            $scope.Brands = data.data;
            $scope.showLoader = false;
            var _Brands = {
                BRAND_ID: "0",
                BRAND_NAME: "All",
                BRAND_CODE: "ALL",

            }
            $scope.Brands.push(_Brands);
        }, function (error) {
            alert(error);


            $scope.showLoader = false;

        });
    }
    $scope.LoadCategoryData = function () {
        $scope.showLoader = true;

        CustomerPriceInfoServices.LoadCategoryData().then(function (data) {
            $scope.Categories = data.data;
            var _Categories = {
                CATEGORY_ID: "0",
                CATEGORY_NAME: "All",
                CATEGORY_CODE: "ALL",

            }
            $scope.Categories.push(_Categories);
            $scope.showLoader = false;
        }, function (error) {
            alert(error);


            $scope.showLoader = false;

        });
    }
    $scope.LoadGroupData = function () {
        $scope.showLoader = true;

        CustomerPriceInfoServices.LoadGroupData().then(function (data) {
            $scope.Groups = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            var _Groups = {
                GROUP_ID: "0",
                GROUP_NAME: "All",
                GROUP_CODE: "ALL",

            }
            $scope.Groups.push(_Groups);
            $scope.showLoader = false;

        });
    }

    $scope.LoadProductData = function () {
        $scope.showLoader = true;
        CustomerPriceInfoServices.LoadProductdropdownData().then(function (data) {
            $scope.Products = data.data;
            $scope.showLoader = false;
            var _Products = {
                PRODUCT_ID: "0",
                PRODUCT_NAME: "All",
                PRODUCT_CODE: "ALL",

            }
            $scope.Products.push(_Products);
        }, function (error) {
            alert(error);
            $scope.showLoader = false;

        });
    }
    $scope.LoadLocationTypes = function () {
        $scope.showLoader = true;

        CustomerPriceInfoServices.LoadLocationTypes().then(function (data) {
            $scope.LocationTypes = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            var _Locations = {
                LOCATION_ID: "0",
                LOCATION_NAME: "All",
                LOCATION_CODE: "ALL",

            }
            $scope.Locations.push(_Locations);

            $scope.showLoader = false;

        });
    }




    $scope.LoadUNIT_ID = function () {
        $('#UNIT_ID').trigger('change');
    }


    $scope.GetPermissionData = function () {
        $scope.showLoader = true;

        $scope.permissionReqModel = {
            Controller_Name: 'PriceInfo',
            Action_Name: 'CustomerPriceInfo'
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
    $scope.LoadUnitData = function () {
        $scope.showLoader = true;

        CustomerPriceInfoServices.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {


            $scope.Unit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });;
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.typeaheadSelectedCustomer = function () {

        const searchIndex = $scope.CustomerData.findIndex((x) => x.CUSTOMER_ID == $scope.model.CUSTOMER_ID);

        $scope.model.CUSTOMER_CODE = $scope.CustomerData[searchIndex].CUSTOMER_CODE;
        $scope.model.CUSTOMER_STATUS = $scope.CustomerData[searchIndex].CUSTOMER_STATUS;
    };
    $scope.LoadCustomerData = function (model) {
        $scope.showLoader = true;
        CustomerPriceInfoServices.LoadCustomerDataByType(model.COMPANY_ID, model.CUSTOMER_TYPE_ID).then(function (data) {
            $scope.CustomerData = data.data;
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.AutoCompleteDataLoadForPrpoduct = function (value) {
        if (value.length >= 3) {
            return CustomerPriceInfoServices.GetSearchableProduct($scope.model.COMPANY_ID, value).then(function (data) {
                $scope.ProductList = data.data;


                return $scope.ProductList;
            }, function (error) {
                alert(error);



            });
        }
    }


    $scope.typeaheadSelectedProduct = function (entity, selectedItem) {
        $scope.model.SKU_ID = selectedItem.SKU_ID;
        $scope.model.SKU_NAME = selectedItem.SKU_NAME;
        $scope.model.SKU_CODE = selectedItem.SKU_CODE;

    };


    $scope.GetEditDataById = function (value, mode) {
        debugger
        if (value != undefined && value.length > 0 && mode == "update") {
            CustomerPriceInfoServices.GetEditDataById(value).then(function (data) {
                if (data.data != null && data.data.customerSkuPriceList != null && data.data.customerSkuPriceList.length > 0) {
                    $scope.model.CUSTOMER_TYPE_ID = data.data.CUSTOMER_TYPE_ID;
                    $scope.model.CUSTOMER_ID = data.data.CUSTOMER_ID;
                    $scope.model.CUSTOMER_PRICE_MSTID = data.data.CUSTOMER_PRICE_MSTID;
                    $scope.model.CUSTOMER_CODE = data.data.CUSTOMER_CODE;
                    $scope.model.EFFECT_START_DATE = data.data.EFFECT_START_DATE;
                    $scope.model.EFFECT_END_DATE = data.data.EFFECT_END_DATE;
                    $scope.model.CustomerType = data.data.CustomerType;
                    $scope.model.REMARKS = data.data.REMARKS;
                    $scope.model.CUSTOMER_STATUS = data.data.CUSTOMER_STATUS;
                    $scope.model.ENTRY_DATE = data.data.ENTRY_DATE;
                    if (data.data.customerSkuPriceList != null) {
                        $scope.gridOptionsList.data = data.data.customerSkuPriceList;
                    }
                    $scope.LoadCustomerData($scope.model);
                }
                $scope.showLoader = false;
            }, function (error) {
                alert(error);
            });
        } else {
            CustomerPriceInfoServices.GetEditDataById(value).then(function (data) {
                if (data.data != null && data.data.customerSkuPriceList != null && data.data.customerSkuPriceList.length > 0) {
                    $scope.model.CUSTOMER_TYPE_ID = data.data.CUSTOMER_TYPE_ID;
                    $scope.model.CUSTOMER_ID = "";
                    $scope.model.CUSTOMER_PRICE_MSTID = 0;
                    $scope.model.CUSTOMER_CODE = data.data.CUSTOMER_CODE;
                    $scope.model.EFFECT_START_DATE = data.data.EFFECT_START_DATE;
                    $scope.model.EFFECT_END_DATE = data.data.EFFECT_END_DATE;
                    $scope.model.CustomerType = data.data.CustomerType;
                    $scope.model.REMARKS = data.data.REMARKS;
                    $scope.model.CUSTOMER_STATUS = data.data.CUSTOMER_STATUS;
                    $scope.model.ENTRY_DATE = data.data.ENTRY_DATE;
                    if (data.data.customerSkuPriceList != null) {
                        for (var i = 0; i < data.data.customerSkuPriceList.length; i++) {
                            data.data.customerSkuPriceList[i].CUSTOMER_PRICE_DTLID = 0;
                        }

                        $scope.gridOptionsList.data = data.data.customerSkuPriceList;
                    }
                    $scope.LoadCustomerData($scope.model);
                }
                $scope.showLoader = false;
            }, function (error) {
                alert(error);
            });
        }
    }
    /*    $scope.DataLoad(0);*/
    $scope.GetPermissionData();
    $scope.CompanyLoad();
    $scope.CompanyNameLoad();
    $scope.LoadUnitData();
    $scope.LoadCustomerTypeData();
    $scope.LoadProductData();
    $scope.LoadBrandData();
    $scope.LoadCategoryData();
    $scope.LoadGroupData();
    $scope.LoadBaseProductData();
    $scope.SaveData = function (model) {
        $scope.showLoader = true;
        if (model.COMPANY_ID == null || model.COMPANY_ID < 0) {
            alert("Please Select Company");
            $scope.showLoader = false;
            return;
        }
        //if ((model.CUSTOMER_CODE == null || model.CUSTOMER_CODE < 0 || model.CUSTOMER_ID == null || model.CUSTOMER_ID < 0) || (model.CUSTOMER_IDs.length ==0)) {
        //    alert("Please  Select Cutomer");
        //    $scope.showLoader = false;
        //    return;
        //}
        if (model.EFFECT_START_DATE == null || model.EFFECT_START_DATE == '' || model.EFFECT_END_DATE == null || model.EFFECT_END_DATE =='') {
            alert("Please enter effect start and end date");
            $scope.showLoader = false;
            return;
        }
        if ($scope.gridOptionsList.data.length<1) {
            alert("No Data Found In Product Information");
            $scope.showLoader = false;
            return;
        }
        for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
            if ($scope.gridOptionsList.data[i].PRICE_FLAG == "" || $scope.gridOptionsList.data[i].PRICE_FLAG == null) {
                alert("Please Select Price Flag, Line: " + (i + 1));
                $scope.showLoader = false;
                return;
            }
            if ($scope.gridOptionsList.data[i].COMMISSION_FLAG == "" || $scope.gridOptionsList.data[i].COMMISSION_FLAG == null) {
                alert("Please  Select Commission Flag, Line: " + (i + 1));
                $scope.showLoader = false;
                return;
            }
           if ($scope.gridOptionsList.data[i].COMMISSION_TYPE == "" || $scope.gridOptionsList.data[i].COMMISSION_TYPE == null) {
               alert("Please Specify Commision Type, Line: " + (i + 1));
               $scope.showLoader = false;
                return;
            }
           
        }
        model.customerSkuPriceList = $scope.gridOptionsList.data;

        
        var dataa = "";

        for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
            if (i == 0) {
                dataa = JSON.stringify($scope.gridOptionsList.data[i].SKU_ID);
            } else {
                dataa = dataa + "," + $scope.gridOptionsList.data[i].SKU_ID;

            }
        }

        CustomerPriceInfoServices.LoadSKUPriceDtlDataRestrict($scope.model.CUSTOMER_ID, dataa, $scope.model.EFFECT_START_DATE, $scope.model.CUSTOMER_IDs).then(function (data) {

            if (data.data != "" && data.data != undefined && $scope.model.CUSTOMER_PRICE_MSTID < 1) {
                $scope.showLoader = false;

                alert(data.data);

            } else {

                if (confirm("Are you sure (Previous configured item which are in this list will be updated and use this pricing policy!!!))?")) {
                    $scope.showLoader = true;
                    if (model.CUSTOMER_IDs == "") {
                        model.CUSTOMER_IDs = [];
                    }
                    CustomerPriceInfoServices.AddOrUpdate(model).then(function (data) {
                        notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
                        if (data.data == 1) {
                            $scope.showLoader = false;
                            window.setTimeout(function () {
                                window.location.href = "/SalesAndDistribution/PriceInfo/CustomerPriceInfo";
                            }, 2000)
                        }
                        else {

                            $scope.showLoader = false;
                        }
                        model.customerSkuPriceList = [];
                        $scope.ProductList = [];
                    });
                }
                else {
                    $scope.showLoader = false;

                }

            }
        }, function (error) {

            $scope.showLoader = false;

        });
       

       
    }



}]);

