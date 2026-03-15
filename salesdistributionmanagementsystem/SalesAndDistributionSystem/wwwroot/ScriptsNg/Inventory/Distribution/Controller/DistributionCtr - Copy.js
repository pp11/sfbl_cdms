
ngApp.controller('ngGridCtrl', ['$scope', 'DistributionService', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, distributionService, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {
        COMPANY_ID: 0,
        MST_ID: 0,
        MST_ID_ENCRYPTED: "",
        DISTRIBUTION_NO: "",
        DISTRIBUTION_DATE: "",
        VECHILE_NO: ""
        , VEHICLE_DESCRIPTION: ""
        , VEHICLE_TOTAL_VOLUME: 0
        , VEHICLE_TOTAL_WEIGHT: 0
        , DRIVER_ID: 0
        , DISTRIBUTION_BY: ""
        , STATUS: "Active"
        , REMAEKS: ""
        , requisitionIssueDtlList: []
    
    }

    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 0,
            DEPOT_REQ_ID: 0,
            MST_ID: 0,
            REQUISITION_NO: "",
            REQUISITION_DATE: "",
            REQUISITION_UNIT_ID: '',
            REQUISITION_UNIT_NAME: '',
            ISSUE_NO: "",
            ISSUE_DATE: "",
            ISSUE_UNIT_ID: "",
            ISSUE_UNIT_NAME: "",
      
            STATUS: 'Active',
            COMPANY_ID: 0,
            REMARKS: '',

        }
    }

    $scope.GridDefalutProductData = function () {
        return {
            ROW_NO: 0,
            DEPOT_REQ_PRODUCT_ID: 0,
            DEPOT_REQ_ID: 0,
            MST_ID: 0,
            ISSUE_NO: "",
            ISSUE_DATE: "",
            ISSUE_UNIT_ID: '',
            DISTRIBUTION_DATE: "",
            SKU_ID: "",
            SKU_CODE: "",
            UNIT_TP: "",
            BATCH_ID: "",
            BATCH_NO: "",
            ISSUE_QTY: "",
            ISSUE_AMOUNT: "",
            DISTRIBUTION_QTY: "",
      
            DISTRIBUTION_AMOUNT: "",
            UNIT_TP: 'Active',
            COMPANY_ID: 0,
            REMARKS: '',
        }
    }


    $scope.ClearEntity = function (entity) {

        entity.ROW_NO = 0,
            entity.DEPOT_REQ_ID = 0,
            entity.MST_ID = 0,
            entity.COMPANY_ID = 0,
            entity.REQUISITION_NO = "",
            entity.REQUISITION_DATE = "",
            entity.REQUISITION_UNIT_ID = '',
            entity.ISSUE_NO = "",
            entity.ISSUE_DATE = "",
            entity.ISSUE_UNIT_ID = ''
    };

    $scope.getPermissions = [];
    $scope.ProductList = [];
    $scope.Companies = [];
    $scope.Vehicles = [];
    $scope.Unit = [];
    $scope.CustomerData = [];
    $scope.CustomerType = [];
    $scope.ProductList = [];
    $scope.RequisitionList = [];

    $scope.BaseProducts = [];
    $scope.Categories = [];
    $scope.Brands = [];
    $scope.Groups = [];
    $scope.Products = [];
    $scope.existingSKU = [];
    $scope.Products = [];
    $scope.Status = [];

    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Distribution Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.data = [];

    $scope.gridOptionsList = {
        data: [$scope.GridDefalutData()]
    }

    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'MST_ID', field: 'MST_ID', visible: false }
        , { name: 'Is_Selected', field: 'Is_Selected', visible: false }
        , { name: 'DEPOT_REQ_ID', field: 'DEPOT_REQ_ID', visible: false }
        , {
            name: 'REQUISITION_NO', field: 'REQUISITION_NO', displayName: 'Requisition No', enableFiltering: true, width: '20%', cellTemplate:
                '<select ng-disabled="row.entity.ROW_NO != 0" class="select2-single form-control"   data-select2-id="{{row.entity.REQUISITION_NO}}" class="REQUISITION_NO"' +
                'name="REQUISITION_NO" ng-model="row.entity.REQUISITION_NO" style="width:100%" ng-change="grid.appScope.typeaheadSelectedRequisitionNo(row.entity)">' +
                '<option ng-repeat="item in grid.appScope.RequisitionList" ng-selected="item.REQUISITION_NO == row.entity.REQUISITION_NO" value="{{item.REQUISITION_NO}}">{{ item.REQUISITION_NO }}</option>' +
                '</select>'
        }
        ,
        {
            name: 'REQUISITION_DATE', field: 'REQUISITION_DATE', displayName: 'Requisition Date', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="text" disabled  style="text-align: right;"  ng-model="row.entity.REQUISITION_DATE"  class="pl-sm" />'
        }
        , {
            name: 'REQUISITION_UNIT_NAME', field: 'REQUISITION_UNIT_NAME', displayName: 'Requisition Unit Id', enableFiltering: true, width: '15%', cellTemplate:
                '<input type="text" style="text-align:right;" disabled  ng-model="row.entity.REQUISITION_UNIT_NAME"  class="pl-sm" />'
        }
        , {
            name: 'ISSUE_NO', field: 'ISSUE_NO', displayName: 'Issue No', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="text"  style="text-align:right;" disabled  ng-model="row.entity.ISSUE_NO"  class="pl-sm" />'
        },
        {
            name: 'ISSUE_DATE', field: 'ISSUE_DATE', displayName: 'Issue Date', enableFiltering: true, width: '15%', cellTemplate:
                '<input type="text" disabled  style="text-align:right;"    ng-model="row.entity.ISSUE_DATE"  class="pl-sm" />'
        },

        {
            name: 'ISSUE_UNIT_NAME', field: 'ISSUE_UNIT_NAME', displayName: 'Issue Unit Id', enableFiltering: true, width: '15%', cellTemplate:
                '<input type="text"  style="text-align:right;" disabled  ng-model="row.entity.ISSUE_UNIT_NAME"  class="pl-sm" />'
        },
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +

                '<button style="margin-bottom: 5px;"   ng-click="grid.appScope.addNewRow(row.entity)" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                '</div>'
        }



    ];

    $scope.gridOptionsList.rowTemplate = "<div ng-style='row.entity.Is_Selected == true && grid.appScope.myObj' ng-dblclick=\"grid.appScope.SelectInvoice(row.entity)\" title=\"Please double click to enter products \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"
    $scope.isTrue = false;
    $scope.myObj = {
        "background": "#999",
      

    }
    //--------------Distribution Product Grid-------------------------------------------------

    $scope.SelectInvoice = (entity) => {
        for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
            $scope.gridOptionsList.data[i].Is_Selected = false;
        }
        
        if (entity.ROW_NO != 0) {
            $scope.selectedInvoice = entity;
            $scope.selectedInvoice.Is_Selected = true;
            let invoice = $scope.gridOptionsList.data.find(e => e.REQUISITION_NO == entity.REQUISITION_NO);
            $scope.gridOptionsProductList.data = invoice.requisitionProductDtlList ?? [];
        }
    }

    $scope.gridOptionsProductList = (gridregistrationservice.GridRegistration("Distribution Info"));
    $scope.gridOptionsProductList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsProductList.data = [];

    $scope.gridOptionsProductList = {
        data: [$scope.GridDefalutProductData()]
    }

    $scope.gridOptionsProductList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , { name: 'BATCH_ID', field: 'BATCH_ID', visible: false }
        , { name: 'REQUISITION_UNIT_ID', field: 'REQUISITION_UNIT_ID', visible: false }
        , { name: 'ISSUE_UNIT_ID', field: 'ISSUE_UNIT_ID', visible: false }
        , { name: 'ISSUE_DATE', field: 'ISSUE_DATE', visible: false }
        , { name: 'ISSUE_NO', field: 'ISSUE_NO', visible: false }
        , { name: 'ISSUE_DATE', field: 'ISSUE_DATE', visible: false }
        , { name: 'ENTERED_BY', field: 'ENTERED_BY', visible: false }
        , { name: 'UPDATED_BY', field: 'UPDATED_BY', visible: false }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '15%', cellTemplate:
                '<input type="text" disabled  style="text-align: right;"  ng-model="row.entity.SKU_NAME"  class="pl-sm" />'
        }
        ,
        {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="text" disabled  style="text-align: right;"  ng-model="row.entity.SKU_CODE"  class="pl-sm" />'
        }
        , {
            name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number" disabled style="text-align:right;" disabled  ng-model="row.entity.UNIT_TP"  class="pl-sm" />'
        }
        , {
            name: 'BATCH_NO', field: 'BATCH_NO', displayName: 'Batch No', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="text"  style="text-align:right;" disabled  ng-model="row.entity.BATCH_NO"  class="pl-sm" />'
        },
        {
            name: 'ISSUE_QTY', field: 'ISSUE_QTY', displayName: 'Issue Qty', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number" disabled style="text-align:right;"    ng-model="row.entity.ISSUE_QTY"  class="pl-sm" />'
        },

        {
            name: 'ISSUE_AMOUNT', field: 'ISSUE_AMOUNT', displayName: 'Issue Amount', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-model="row.entity.ISSUE_AMOUNT"  class="pl-sm" />'
        },
        {
            name: 'DISTRIBUTION_QTY', field: 'DISTRIBUTION_QTY', displayName: 'Dis. Qty', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;"  ng-change="grid.appScope.typeaheadSelectedQty(row.entity)"   ng-model="row.entity.DISTRIBUTION_QTY"  class="pl-sm" />'
        },
        {
            name: 'DISTRIBUTION_AMOUNT', field: 'DISTRIBUTION_AMOUNT', displayName: 'Distribution Amount', enableFiltering: true, width: '10%', cellTemplate:
                '<input type="number"  style="text-align:right;" disabled  ng-model="row.entity.DISTRIBUTION_AMOUNT"  class="pl-sm" />'
        },
        , {
            name: 'Action', displayName: 'Action', width: '10%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeProductItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                '</div>'
        }



    ];



    $scope.rowNumberGenerate = function () {

        $scope.model.DISTRIBUTION_AMOUNT = 0;
        for (let i = 0; i < $scope.gridOptionsList.data.length; i++) {
          
         
            $scope.gridOptionsList.data[i].ROW_NO = i;
          
        }
    }


    $scope.typeaheadSelectedQty = function (entity) {
        
        //const searchIndex = $scope.Products.findIndex((x) => x.SKU_ID == entity.SKU_ID);
        //entity.SKU_CODE = $scope.Products[searchIndex].SKU_CODE;
        //entity.UNIT_TP = $scope.Products[searchIndex].UNIT_TP;
        if (entity.DISTRIBUTION_QTY == '' || entity.DISTRIBUTION_QTY == null || entity.DISTRIBUTION_QTY < 0) {
            entity.DISTRIBUTION_QTY = 0;
        }
        entity.DISTRIBUTION_AMOUNT = parseInt(entity.UNIT_TP) * parseInt(entity.DISTRIBUTION_QTY)
        $scope.rowNumberGenerate();

    };

    $scope.GetEditDataById = function (value) {
        
        if (value != undefined && value.length > 0) {
            distributionService.GetEditDataById(value).then(function (data) {
                

                if (data.data != null && data.data.requisitionIssueDtlList != null && data.data.requisitionIssueDtlList.length > 0) {
                    $scope.RequisitionList = [...$scope.RequisitionList, ...data.data.requisitionIssueDtlList];
                    $scope.model.COMPANY_ID = data.data.COMPANY_ID;
                    $scope.model.MST_ID = data.data.MST_ID;
                    $scope.model.DISTRIBUTION_NO = data.data.DISTRIBUTION_NO;
                    $scope.model.DISTRIBUTION_DATE = data.data.DISTRIBUTION_DATE;
                    $scope.model.VECHILE_NO = data.data.VEHICLE_NO;
                    $scope.model.VEHICLE_DESCRIPTION = data.data.VEHICLE_DESCRIPTION;
                    $scope.model.VEHICLE_TOTAL_VOLUME = data.data.VEHICLE_TOTAL_VOLUME;
                    $scope.model.VEHICLE_TOTAL_WEIGHT = data.data.VEHICLE_TOTAL_WEIGHT;
                    $scope.model.DRIVER_ID = data.data.DRIVER_ID;

                    $scope.model.STATUS = data.data.STATUS;
                    $scope.model.DISTRIBUTION_BY = data.data.DISTRIBUTION_BY;
        
                    $scope.model.requisitionIssueDtlList = data.data.requisitionIssueDtlList;

                    $scope.gridOptionsList.data = [$scope.GridDefalutData(),...data.data.requisitionIssueDtlList];
         
                    $scope.gridOptionsProductList.data = data.data.requisitionIssueDtlList[0].requisitionProductDtlList;
                   // $scope.addDefaultRow($scope.GridDefalutData());

                    $("#VECHILE_NO").trigger("change");
                  

             
                }
          
                $scope.LoadSKUId();
                $scope.rowNumberGenerate();
                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                console.log(error);
            });
        }
    }


    $scope.addNewRow = (entity) => {
        var count = 0;
        
        if ($scope.gridOptionsList.data.length > 0 && $scope.gridOptionsList.data[0].REQUISITION_NO != null && $scope.gridOptionsList.data[0].REQUISITION_NO != '' && $scope.gridOptionsList.data[0].REQUISITION_NO != 'undefined') {
            for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
                if ($scope.gridOptionsList.data[i].REQUISITION_NO == entity.REQUISITION_NO) {
                    count++;
                }

            }
            if (count == 1 || count == 0 || entity.SKU_CODE == "") {
                if (entity.REQUISITION_QTY <= 0) {
                    notificationservice.Notification("Requistion quantity must be greater then zero!", "", 'Requistion quantity must be greater then zero!!');
                    $scope.ClearEntity(entity)
                    return;
                }
           
                $scope.showLoader = true;
                var value = "";
             

                distributionService.GetDistributionDetailDataById(entity.REQUISITION_NO).then(function (data) {
                    for (var i = 0; i < $scope.gridOptionsList.data.length; i++) {
                        $scope.gridOptionsList.data[i].Is_Selected = false;
                    }
                    var newRow = {
                        ROW_NO: 1, Is_Selected: true, COMPANY_ID: $scope.model.COMPANY_ID, MST_ID: $scope.gridOptionsList.data[0].MST_ID, REQUISITION_NO: $scope.gridOptionsList.data[0].REQUISITION_NO, REQUISITION_DATE: $scope.gridOptionsList.data[0].REQUISITION_DATE, REQUISITION_UNIT_NAME: $scope.gridOptionsList.data[0].REQUISITION_UNIT_NAME, REQUISITION_UNIT_ID: $scope.gridOptionsList.data[0].REQUISITION_UNIT_ID, ISSUE_NO: $scope.gridOptionsList.data[0].ISSUE_NO, ISSUE_DATE: $scope.gridOptionsList.data[0].ISSUE_DATE, ISSUE_UNIT_NAME: $scope.gridOptionsList.data[0].ISSUE_UNIT_NAME, ISSUE_UNIT_ID: $scope.gridOptionsList.data[0].ISSUE_UNIT_ID, STATUS: $scope.gridOptionsList.data[0].STATUS, requisitionProductDtlList : data.data[0]
                    }
                   
        
                 
               
                    for (var i = 0; i < newRow.requisitionProductDtlList.length; i++) {
                        newRow.requisitionProductDtlList[i].DISTRIBUTION_AMOUNT = newRow.requisitionProductDtlList[i].ISSUE_AMOUNT;
                        newRow.requisitionProductDtlList[i].DISTRIBUTION_QTY = newRow.requisitionProductDtlList[i].DISTRIBUTION_QTY;

                    }
                    $scope.gridOptionsList.data.push(newRow);
                    $scope.gridOptionsProductList.data = newRow.requisitionProductDtlList;
           
                    $scope.gridOptionsList.data[0] = $scope.GridDefalutData();
                    $scope.rowNumberGenerate();
                 
                    $scope.showLoader = false;
                    $scope.LoadSKUId();
                }, function (error) {
                    console.log(error);
                    $scope.showLoader = false;

                });


            }
            else {
                notificationservice.Notification("Requisition already exist!", "", 'Requisition already exist!');
                $scope.ClearEntity(entity)
            }
           

        }
        //} else {
        //    notificationservice.Notification("Please Enter Valid Region First", "", 'Only Single Row Left!!');

        //}
        $scope.rowNumberGenerate();
    };

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
    $scope.removeProductItem = function (entity) {
        
        if ($scope.gridOptionsProductList.data.length > 1) {
            var index = $scope.gridOptionsProductList.data.indexOf(entity);
            if ($scope.gridOptionsProductList.data.length > 0) {
                $scope.gridOptionsProductList.data.splice(index, 1);
            }
            $scope.rowNumberGenerate();


        } else {
            notificationservice.Notification("Only Single Row Left!!", "", 'Only Single Row Left!!');
        }


    }

    $scope.EditItem = (entity) => {
        
        if ($scope.gridOptionsList.data.length > 0) {

            var newRow = {
                ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, MST_ID: entity.MST_ID, DTL_ID: entity.DTL_ID, REGION_ID: entity.REGION_ID, REGION_CODE: entity.REGION_CODE, SKU_ID: entity.SKU_ID, SKU_CODE: entity.SKU_CODE, STATUS: entity.STATUS, REQUISITION_AMOUNT: entity.REQUISITION_AMOUNT, REQUISITION_QTY: entity.REQUISITION_QTY, UNIT_TP: entity.UNIT_TP, STATUS: entity.STATUS

            }
            $scope.gridOptionsList.data[0] = newRow;


        } else {
            notificationservice.Notification("Please Select Valid data", "", 'Only Single Row Left!!');

        }
        $scope.rowNumberGenerate();
        $scope.removeItem(entity);
    };

    $scope.LoadFormData = function () {
        
        $scope.showLoader = true;
        distributionService.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.relationdata = data.data;
            for (var i = 0; i < $scope.relationdata.length; i++) {
                
                if ($scope.relationdata[i].MST_ID == parseInt($scope.model.MST_ID)) {
                    $scope.model.MST_ID_ENCRYPTED = $scope.relationdata[i].MST_ID_ENCRYPTED
                }
            }
            $scope.EditData($scope.model.MST_ID_ENCRYPTED)


            $scope.showLoader = false;

        }, function (error) {
            
            alert(error);
            console.log(error);
            $scope.showLoader = false;

        });
    }
    $scope.EditData = function (MST_ID_ENCRYPTED) {
        
        window.location = "/Inventory/Distribution/Distribution?Id=" + MST_ID_ENCRYPTED;
    }
    $scope.typeaheadSelectedRequisitionNo = function (entity) {
        
        const searchIndex = $scope.RequisitionList.findIndex((x) => x.REQUISITION_NO == entity.REQUISITION_NO);
       /* entity.SKU_ID = $scope.Products[searchIndex].SKU_ID;*/
        entity.REQUISITION_DATE = $scope.RequisitionList[searchIndex].REQUISITION_DATE;
        entity.MST_ID = $scope.RequisitionList[searchIndex].MST_ID;
        entity.REQUISITION_UNIT_ID = $scope.RequisitionList[searchIndex].REQUISITION_UNIT_ID;
        entity.REQUISITION_UNIT_NAME = $scope.RequisitionList[searchIndex].REQUISITION_UNIT_NAME;
        entity.ISSUE_NO = $scope.RequisitionList[searchIndex].ISSUE_NO;
        entity.ISSUE_DATE = $scope.RequisitionList[searchIndex].ISSUE_DATE;
        entity.ISSUE_UNIT_ID = $scope.RequisitionList[searchIndex].ISSUE_UNIT_ID;
        entity.ISSUE_UNIT_NAME = $scope.RequisitionList[searchIndex].ISSUE_UNIT_NAME;
   
        $scope.LoadRequisition();

    };

    $scope.typeaheadSelectedRequisition = function (entity) {
        
        const searchIndex = $scope.RequisitionList.findIndex((x) => x.REQUISITION_NO == entity.REQUISITION_NO);
        entity.REQUISITION_NO = $scope.RequisitionList[searchIndex].REQUISITION_NO;
     
        $scope.LoadRequisition();

    };
 
    $scope.LoadSKUId = function () {
        setTimeout(function () {
            $('#SKU_CODE').trigger('change');
            $('#SKU_ID').trigger('change');
            $("#REQUISITION_UNIT_ID").trigger('change');

        }, 1000)

    }
    $scope.LoadRequisition = function () {
        setTimeout(function () {
            $('#REQUISITION_NO').trigger('change');
       

        }, 1000)

    }

  

    $scope.ClearForm = function () {
        window.location.href = "/Inventory/Distribution/Distribution";
    }
    //$scope.DataLoad = function (companyId) {
    //    $scope.showLoader = true;
    //    $scope.SkuList = "";
    //    $scope.GetExistingSku();
    //    console.log($scope.existingSKU);
    //    setTimeout(function () {
    //        requisitionServices.LoadFilteredProduct($scope.model).then(function (data) {
    //            var dataList = [];
    //            var flag = 0
    //            for (var i = 0; i < data.data.length; i++) {
    //                for (var j = 0; j < $scope.existingSKU.length; j++) {
    //                    if (data.data[i].SKU_ID == $scope.existingSKU[j].SKU_ID) {
    //                        flag = 1;
    //                    }
    //                }
    //                if (flag == 0) {
    //                    dataList.push(data.data[i]);

    //                }
    //                else {
    //                    flag = 0;
    //                }
    //            }
    //            $scope.gridOptionsList.data = dataList;
    //            //if ($scope.existingSKU.length > 0) {
    //            //    for (var j = 0; j < $scope.existingSKU.length; j++) {
    //            //        $scope.SkuList += $scope.existingSKU[j].SKU_ID + ",";
    //            //    }
    //            //    notificationservice.Notification(1, 1, $scope.SkuList + " already exist!");
    //            //}
    //            $scope.showLoader = false;

    //        }, function (error) {
    //            alert(error);
    //            $scope.showLoader = false;

    //        });
    //    }, 2000)

    //}
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        distributionService.GetCompany().then(function (data) {
            
            $scope.model.COMPANY_ID = parseFloat(data.data);
            /*            $scope.DataLoad($scope.model.COMPANY_ID);*/
            $interval(function () {
                $('#COMPANY_ID').trigger('change');
            }, 800, 4);
            $scope.showLoader = false;
        }, function (error) {
            console.log(error);
            $scope.showLoader = false;

        });
    }
    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;

        distributionService.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            console.log(error);
            $scope.showLoader = false;

        });
    }
        $scope.VehicleLoad = function () {
            $scope.showLoader = true;

            distributionService.GetVehicleList().then(function (data) {
                $scope.Vehicles = data.data;
                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                console.log(error);
                $scope.showLoader = false;

            });
    }
    $scope.LoadVehicleData = function () {
        
        for (var i = 0; i < $scope.Vehicles.length; i++) {
            if ($scope.Vehicles[i].VEHICLE_NO == $scope.model.VECHILE_NO) {
     
                $scope.model.VEHICLE_DESCRIPTION = $scope.Vehicles[i].VEHICLE_DESCRIPTION;
                $scope.model.VEHICLE_TOTAL_VOLUME = $scope.Vehicles[i].VEHICLE_TOTAL_VOLUME;
                $scope.model.VEHICLE_TOTAL_WEIGHT = $scope.Vehicles[i].VEHICLE_TOTAL_WEIGHT;
                $scope.model.DRIVER_ID = $scope.Vehicles[i].DRIVER_ID;

            }
        }
    }

    $scope.LoadProductData = function () {
        $scope.showLoader = true;

        distributionService.LoadProductData().then(function (data) {
            
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
            console.log(error);

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


    $scope.LoadUNIT_ID = function () {
        $('#UNIT_ID').trigger('change');
    }

    $scope.LoadCOMPANY_ID = function () {
        $('#COMPANY_ID').trigger('change');
    }
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'RequisitionIssue',
            Action_Name: 'RequisitionIssue'
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
            console.log(error);
            $scope.showLoader = false;

        });
    }
    $scope.LoadUnitData = function () {
        $scope.showLoader = true;

        distributionService.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            
            
            $scope.Unit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });;
            $scope.showLoader = false;
        }, function (error) {
            console.log(error);
            $scope.showLoader = false;

        });
    }
    $scope.LoadRequisitionData = function () {
        $scope.showLoader = true;

        distributionService.LoadRequisitionData($scope.model.COMPANY_ID).then(function (data) {
            
 
            
            $scope.RequisitionList = data.data[0];
            $scope.showLoader = false;
        }, function (error) {
            console.log(error);
            $scope.showLoader = false;

        });
    }

    $scope.LoadRequisitionDtlData = function (entity) {
        
        
        $scope.showLoader = true;
        var value = "";
     

        distributionService.GetProductsByRequisitionNo(entity.REQUISITION_NO).then(function (data) {
            
            
            $scope.gridOptionsProductList.data = data.data;
            for (var i = 0; i < $scope.gridOptionsProductList.data.length; i++) {
                $scope.gridOptionsProductList.data[i].DISTRIBUTION_AMOUNT = $scope.gridOptionsProductList.data[i].ISSUE_AMOUNT;
                $scope.gridOptionsProductList.data[i].DISTRIBUTION_QTY = $scope.gridOptionsProductList.data[i].ISSUE_QTY ;

            }
            $scope.gridOptionsList.data[0].requisitionProductDtlList = data.data;
            $scope.gridOptionsList.data[0] = $scope.GridDefalutData();
            $scope.showLoader = false;
            $scope.LoadSKUId();
        }, function (error) {
            console.log(error);
            $scope.showLoader = false;

        });
    }

    $scope.SaveData = function (model) {

        if ($scope.model.REQUISITION_NO == "") {
            notificationservice.Notification('Requisition no not selected!', "", 'Requisition no not selected!');
            return;
        }
        
        for (var i = 0; i < $scope.model.requisitionIssueDtlList.length; i++) {
            for (var j = 0; j < $scope.model.requisitionIssueDtlList[i].requisitionProductDtlList.length; j++) {
                if ($scope.model.requisitionIssueDtlList[i].requisitionProductDtlList[j].DISTRIBUTION_QTY > $scope.model.requisitionIssueDtlList[i].requisitionProductDtlList[j].ISSUE_QTY) {
                    notificationservice.Notification('Distribution Qty is greater then Issue Qty!', "", 'Distribution Qty is greater then Issue Qty!');

                    return;
                }
            }
        }
        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);
        $scope.model.requisitionIssueDtlList = $scope.gridOptionsList.data;
        if ($scope.model.requisitionIssueDtlList != null) {
            if ($scope.model.requisitionIssueDtlList.length > 1 && $scope.model.requisitionIssueDtlList[0].REQUISITION_NO == "") {
                $scope.model.requisitionIssueDtlList.splice(0, 1);
            }
        }
         
     
        
        $scope.showLoader = true;
        distributionService.AddOrUpdate(model).then(function (data) {

            
            if (parseInt(data.data) > 0) {
                $scope.showLoader = false;
                $scope.model.MST_ID = data.data;
                notificationservice.Notification(1, 1, 'data save successfully!');
                setTimeout(function () {
                    $scope.LoadFormData();
                }, 1000)



                //window.location.reload();

            }
            else {
                $scope.showLoader = false;
                notificationservice.Notification('data not save successfully!', "", 'data not save successfully!');

            }
        });
    }




    //$scope.typeaheadSelectedProduct = function (entity, selectedItem) {
    //    $scope.model.SKU_ID = selectedItem.SKU_ID;
    //    $scope.model.SKU_NAME = selectedItem.SKU_NAME;
    //    $scope.model.SKU_CODE = selectedItem.SKU_CODE;

    //};


    ///*    $scope.DataLoad(0);*/
    $scope.GetPermissionData();
    $scope.CompaniesLoad();
    $scope.VehicleLoad()
    $scope.CompanyLoad();
    $scope.LoadUnitData();
    $scope.LoadRequisitionData();
    $scope.LoadProductData();
    $scope.LoadStatus();




}]);

