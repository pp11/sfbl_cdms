ngApp.controller('ngGridCtrl', ['$scope', 'SkuLoadingChargeServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, SkuLoadingChargeServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {
    
    'use strict'
    $scope.model = {
        LOADING_CHARGE_ID: 0
        , SKU_ID: 0
        , SKU_CODE: ''
        , ENTRY_DATE: ''
        , EFFECT_START_DATE: ''
        , EFFECT_END_DATE: ''        
        , SHIPPER_QTY: 0
        , LOADING_CHARGE: 0        
        , COMPANY_ID: 0
        , UNIT_ID: 0
        , STATUS: 'Active'
        , REMARKS: ''
    }

    $scope.Status = [];
    $scope.Products = [];
    
    $scope.model.ENTRY_DATE = CurrentDate;

    $scope.LoadStatus = function () {
        var Active = {
            STATUS: 'Active'
        }
        var Inactive = {
            STATUS: 'Inactive'
        }
        $scope.Status.push(Active);
        $scope.Status.push(Inactive);

    }

    $scope.CompanyIDLoad = function () {
        
        $scope.showLoader = true;

        SkuLoadingChargeServices.GetCompanyId().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }

    $scope.CompanyNameLoad = function () {
        $scope.showLoader = true;
        SkuLoadingChargeServices.GetCompanyName().then(function (data) {
            $scope.model.COMPANY_NAME = data.data;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }


    $scope.LoadProductData = function () {
        $scope.showLoader = true;

        SkuLoadingChargeServices.LoadProductData().then(function (data) {
            
            $scope.Products_data = data.data;
            var _Products = {
                SKU_ID: "0",
                SKU_NAME: "None",
                SKU_CODE: "None",
                PACK_SIZE: "None",
                SHIPPER_QTY:"0"

            }
            $scope.Products.push(_Products);
            for (var i in $scope.Products_data) {
                $scope.Products.push($scope.Products_data[i]);
            }

            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.changeProduct = function () {
        
        const searchIndex = $scope.Products.findIndex((x) => x.SKU_ID == $scope.model.SKU_ID);
        $scope.model.SKU_CODE = $scope.Products[searchIndex].SKU_CODE;
        $scope.model.PACK_SIZE = $scope.Products[searchIndex].PACK_SIZE;
        $scope.model.SHIPPER_QTY = $scope.Products[searchIndex].SHIPPER_QTY;
    };


    $scope.LoadSKU_ID = function () {
        $('#SKU_ID').trigger('change');

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


    $scope.ClearForm = function () {
        window.location.href = "/SalesAndDistribution/SkuLoadingCharge/SkuLoadingCharge";
    }

    $scope.SaveData = function (model) {
        
        $scope.showLoader = true;
        
        $scope.model.SKU_ID = parseInt($scope.model.SKU_ID);
        if ($scope.model.LOADING_CHARGE_ID == NaN) {
            $scope.model.LOADING_CHARGE_ID = parseInt($scope.model.LOADING_CHARGE_ID);
        }
        SkuLoadingChargeServices.AddOrUpdate(model).then(function (data) {
            
            if (data.data > 0) {
                notificationservice.Notification(1, 1, 'Data Save Successfully !!');

            } else {
                notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');

            }
            $scope.showLoader = false;
            $scope.model.LOADING_CHARGE_ID = parseInt(data.data);
            //$scope.GetPermissionData();
            //$scope.CompanyLoad();
            //$scope.LoadFormData();
            $interval(function () {
                $scope.LoadSKU_ID();
            }, 800, 2);
            $scope.SearchData();

        });
    }



    $scope.gridSearch = (gridregistrationservice.GridRegistration("Sku Loading Charge"));
    $scope.gridSearch.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridSearch.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50', visible: false
        }
        , { name: 'LOADING_CHARGE_ID', field: 'LOADING_CHARGE_ID', displayName: 'Charge ID', visible: true, width: '6%' }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , { name: 'UNIT_ID', field: 'UNIT_ID', visible: false }
        , { name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'SKU Code', enableFiltering: true, width: '10%' }
        , { name: 'SKU_ID', field: 'SKU_ID', displayName: 'SKU ID', enableFiltering: true, width: '8%' }
        , { name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'SKU Name', enableFiltering: true, width: '30%' }
        , { name: 'PACK_SIZE', field: 'PACK_SIZE', displayName: 'Pack Size', enableFiltering: true, width: '8%' }
        , { name: 'SHIPPER_QTY', field: 'SHIPPER_QTY', displayName: 'Shipper Qty', enableFiltering: true, width: '10%' }
        , { name: 'LOADING_CHARGE', field: 'LOADING_CHARGE', displayName: 'Loading Charge', enableFiltering: true, width: '10%' }
        , { name: 'EFFECT_START_DATE', field: 'EFFECT_START_DATE', displayName: 'Effect Start Date', enableFiltering: true, width: '10%', visible: true }
        , { name: 'EFFECT_END_DATE', field: 'EFFECT_END_DATE', displayName: 'Effect End Date', enableFiltering: true, width: '10%', visible: true }
        , { name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '15%', visible: false }
        , { name: 'ENTRY_DATE', field: 'ENTRY_DATE', displayName: 'ENTRY_DATE', enableFiltering: true, width: '15%', visible: false }
        , { name: 'STATUS', field: 'STATUS', displayName: 'Status', enableFiltering: true, width: '15%' }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;"  ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];
    $scope.gridSearch.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"


    $scope.SearchData = function (companyId) {
        
        $scope.showLoader = true;

        SkuLoadingChargeServices.SearchData(companyId).then(function (data) {
            
            
            $scope.gridSearch.data = data.data;
            $scope.showLoader = false;
            //$scope.model.COMPANY_SEARCH_ID = companyId;
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    $scope.EditData = function (entity) {
        
        $scope.model.LOADING_CHARGE_ID = entity.LOADING_CHARGE_ID;
        $scope.model.COMPANY_ID = entity.COMPANY_ID;
        $scope.model.UNIT_ID = entity.UNIT_ID;
        $scope.model.SKU_CODE = entity.SKU_CODE;
        $scope.model.SKU_ID = entity.SKU_ID;
        $scope.model.SKU_NAME = entity.SKU_NAME;
        $scope.model.PACK_SIZE = entity.PACK_SIZE;
        $scope.model.SHIPPER_QTY = entity.SHIPPER_QTY;
        $scope.model.LOADING_CHARGE = entity.LOADING_CHARGE;        
        $scope.model.EFFECT_START_DATE = entity.EFFECT_START_DATE;
        $scope.model.EFFECT_END_DATE = entity.EFFECT_END_DATE;
        $scope.model.ENTRY_DATE = entity.ENTRY_DATE;
        $scope.model.REMARKS = entity.REMARKS;
        if ($scope.model.REMARKS == null) {
            $scope.model.REMARKS = '';
        }
        // $scope.gridSearch.data = data.data;
        $scope.model.STATUS = entity.STATUS;
        $interval(function () {
            $scope.LoadSKU_ID();
        }, 800, 2);
    }






    $scope.CompanyIDLoad();
    $scope.CompanyNameLoad();
    $scope.LoadStatus();
    $scope.LoadProductData();
    $scope.SearchData();


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


    
}]);

