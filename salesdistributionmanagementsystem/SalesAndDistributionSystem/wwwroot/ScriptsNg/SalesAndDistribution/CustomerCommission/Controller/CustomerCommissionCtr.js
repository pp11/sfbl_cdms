ngApp.controller('ngGridCtrl', ['$scope', 'CustomerCommissionServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, CustomerCommissionServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {
          COMMISSION_ID: 0   
        , CUSTOMER_ID: ''
        , CUSTOMER_CODE: ''       
        , ENTRY_DATE: ''
        , EFFECT_START_DATE: ''
        , EFFECT_END_DATE: ''
        , COMMISSION_TYPE: 'PCT'
        , COMMISSION_VALUE: 0
        , ADD_COMMISSION1: 0
        , ADD_COMMISSION2:0
        , COMPANY_ID: 0
        , UNIT_ID: 0
        , STATUS:'Active'
        , REMARKS: ''    
    }
    //$scope.model = {}
    $scope.Status = [];
    $scope.CommissionType = [];
    $scope.CustomerData = [];
    //$scope.STATUS = '';
    //$scope.COMMISSION_TYPE = '';

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

    $scope.LoadCommissionType = function () {
        var PCT = {
            COMMISSION_TYPE: 'PCT'
        }
        var Value = {
            COMMISSION_TYPE: 'Value'
        }
        $scope.CommissionType.push(PCT);
        $scope.CommissionType.push(Value);

    }

    $scope.CompanyIDLoad = function () {
        $scope.showLoader = true;
        CustomerCommissionServices.GetCompanyId().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }

    $scope.CompanyNameLoad = function () {
        $scope.showLoader = true;
        CustomerCommissionServices.GetCompanyName().then(function (data) {
            $scope.model.COMPANY_NAME = data.data;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

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
            /*let text_title_3 = textArray[2];*/

            var $container = $(
                "<div class='select2-result-repository clearfix'>" +
                "<div class='select2-result-repository__meta'>" +
                "<div class='select2-result-repository__title' style='font-size:14px;font-weight:700'></div>" +
                "<div class='select2-result-repository__watchers' style='font-size:12px;font-weight:700'> <span>Code: </span>  </div>" +
               /* "<div class='select2-result-repository__watchers_2' style='font-size:12px;font-weight:700'> <span>Pack Size: </span>  </div>" +*/
                "</div>" +
                "</div>"
            );

            $container.find(".select2-result-repository__title").text(text_title);
            $container.find(".select2-result-repository__watchers").append(text_title_2);
        }

        return $container;
    }

    $scope.formatRepoSelectionProduct = function (repo) {
        return repo.text.split("--")[0];
    }


    $(".select2-single-Customer").select2({
        placeholder: "Select",
        templateResult: $scope.formatRepoProduct,
        templateSelection: $scope.formatRepoSelectionProduct
    });

    $scope.LoadCustomerData = function () {
        $scope.showLoader = true;
        CustomerCommissionServices.LoadCustomerDropdownData($scope.model.COMPANY_ID).then(function (data) {
            $scope.CustomerData = data.data;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.LoadCUSTOMER_ID = function () {
        $('#CUSTOMER_ID').trigger('change');

    }


    $scope.changeCustomer = function () {
        
        const searchIndex = $scope.CustomerData.findIndex((x) => x.CUSTOMER_ID == $scope.model.CUSTOMER_ID);
        $scope.model.CUSTOMER_CODE = $scope.CustomerData[searchIndex].CUSTOMER_CODE;        
    };

    $scope.SaveData = function (model) {
        
        $scope.showLoader = true;
        if (!isNaN($scope.model.COMMISSION_ID)) {
            $scope.model.COMMISSION_ID = parseInt($scope.model.COMMISSION_ID);
        }
        if ($scope.formMenuCategoryAdd.$invalid) {
            let err = $scope.formMenuCategoryAdd.$error.required;
            for (var i = 0; i < err.length; i++) {
                let input = document.getElementById(err[i].$$attr.id);
                input.style.border = '3px solid red';
            }
            alert("Please Select " + err[0].$$attr.id);
            $scope.showLoader = false;
            return;
        }
        if (model.COMMISSION_VALUE <= 0 ) {
            alert('Commition value cannot be zero or negative');
            $scope.showLoader = false;
            return;
        }

        CustomerCommissionServices.AddOrUpdate(model).then(function (data) {
            notificationservice.Notification(1, 1, 'Data Save Successfully !!');            
                $scope.showLoader = false;
                $scope.model.COMMISSION_ID = parseInt(data.data);
                //$scope.GetPermissionData();
                //$scope.CompanyLoad();
                 //$scope.LoadFormData();
              $scope.SearchData();
            
        });
    }

    $scope.gridSearch = (gridregistrationservice.GridRegistration("Customer Commission"));
    $scope.gridSearch.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridSearch.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }

        , { name: 'COMMISSION_ID', field: 'COMMISSION_ID', displayName: 'Comm ID', visible: true, width: '8%' }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , { name: 'UNIT_ID', field: 'UNIT_ID', visible: false }
        , { name: 'CUSTOMER_CODE', field: 'CUSTOMER_CODE', displayName: 'Customer Code', enableFiltering: true, width: '12%', visible: false }
        , { name: 'CUSTOMER_ID', field: 'CUSTOMER_ID', displayName: 'Customer ID', enableFiltering: true, width: '10%', visible: false }
        , { name: 'CUSTOMER_NAME', field: 'CUSTOMER_NAME', displayName: 'Customer Name', enableFiltering: true, width: '20%' }
        , { name: 'COMMISSION_TYPE', field: 'COMMISSION_TYPE', displayName: 'Type', enableFiltering: true, width: '5%' }
        , { name: 'COMMISSION_VALUE', field: 'COMMISSION_VALUE', displayName: 'Customer Value', enableFiltering: true, width: '12%' }
        , { name: 'ADD_COMMISSION1', field: 'ADD_COMMISSION1', displayName: 'Add Commission1', enableFiltering: true, width: '12%' }
        , { name: 'ADD_COMMISSION2', field: 'ADD_COMMISSION2', displayName: 'Add Commission2', enableFiltering: true, width: '18%', visible: false }
        , { name: 'EFFECT_START_DATE', field: 'EFFECT_START_DATE', displayName: 'Effect Start Date', enableFiltering: true, width: '10%' }
        , { name: 'EFFECT_END_DATE', field: 'EFFECT_END_DATE', displayName: 'Effect End Date', enableFiltering: true, width: '10%' }
        , { name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '15%', visible: false }
        , { name: 'STATUS', field: 'STATUS', displayName: 'Status', enableFiltering: true, width: '10%' }
        , {name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;"  ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'},

    ];
    $scope.gridSearch.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"


    $scope.SearchData = function (companyId) {
        $scope.showLoader = true;
        CustomerCommissionServices.SearchData(companyId).then(function (data) {
            $scope.gridSearch.data = data.data;
            $scope.showLoader = false;
        }, function (error) {
            
            alert(error);
            $scope.showLoader = false;

        });
    }

    $scope.EditData = function (entity) {
        
        $scope.model.COMMISSION_ID = entity.COMMISSION_ID;
        $scope.model.COMPANY_ID = entity.COMPANY_ID;
        $scope.model.UNIT_ID = entity.UNIT_ID;
        $scope.model.CUSTOMER_CODE = entity.CUSTOMER_CODE;
        $scope.model.CUSTOMER_ID = entity.CUSTOMER_ID;
        $scope.model.CUSTOMER_NAME = entity.CUSTOMER_NAME;
        $scope.model.COMMISSION_TYPE = entity.COMMISSION_TYPE;
        $scope.model.COMMISSION_VALUE = entity.COMMISSION_VALUE;
        $scope.model.ADD_COMMISSION1 = entity.ADD_COMMISSION1;
        $scope.model.ADD_COMMISSION2 = entity.ADD_COMMISSION1;
        $scope.model.EFFECT_START_DATE = entity.EFFECT_START_DATE;
        $scope.model.EFFECT_END_DATE = entity.EFFECT_END_DATE;
        $scope.model.REMARKS = entity.REMARKS;
        if ($scope.model.REMARKS == null) {
            $scope.model.REMARKS = '';
        }
       // $scope.gridSearch.data = data.data;
        $scope.model.STATUS = entity.STATUS;
        $interval(function () {
            $scope.LoadCUSTOMER_ID();
        }, 800, 2);
    }

    $scope.ClearForm = function () {
        $scope.model.COMMISSION_ID = 0;
        $scope.model.ENTRY_DATE = CurrentDate;;
        $scope.model.COMMISSION_TYPE = "PCT";
        $scope.model.STATUS = "Active";
        $scope.model.COMMISSION_VALUE = '';
        $scope.model.ADD_COMMISSION1 = '';
        $scope.model.ADD_COMMISSION2 = '';
        $scope.model.EFFECT_START_DATE = '';   
        $scope.model.EFFECT_END_DATE = '';   
        $scope.model.REMARKS = '';
        $scope.model.CUSTOMER_CODE = '';
        $scope.model.CUSTOMER_ID = '';
        $scope.LoadCustomerData();
    }

    $scope.CompanyIDLoad();
    $scope.CompanyNameLoad();
    $scope.SearchData();
    $scope.LoadStatus();
    $scope.LoadCommissionType();
    $scope.LoadCustomerData();


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

