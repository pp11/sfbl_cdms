ngApp.controller('ngGridCtrl', ['$scope', 'CreditInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, CreditInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {
        CREDIT_ID: 0,
        COMPANY_ID: 0,
        CUSTOMER_ID: "",
        CUSTOMER_CODE: null,
        EFFECT_START_DATE: ""
        , EFFECT_END_DATE: ""
        , CUSTOMER_STATUS: ""
        , REMARKS: ""
        , ENTRY_DATE: ""
        , CREDIT_LIMIT: 0
        , CREDIT_DAYS:0
      
    }


    $scope.getPermissions = [];
    $scope.ProductList = [];
    $scope.Companies = [];
    $scope.Unit = [];
    $scope.CustomerData = [];
    $scope.CustomerType = [];


    $scope.Batch_date_Load = function () {

        const today = new Date();
        const yyyy = today.getFullYear();
        let mm = today.getMonth() + 1; // Months start at 0!
        let dd = today.getDate();

        if (dd < 10) dd = '0' + dd;
        if (mm < 10) mm = '0' + mm;

        $scope.model.ENTRY_DATE = dd + '/' + mm + '/' + yyyy;

    }

    $scope.Batch_date_Load();


    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Product Price Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }





    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }
        , {
            name: 'CUSTOMER_NAME', field: 'CUSTOMER_NAME', displayName: 'NAME', enableFiltering: true, width: '20%'
        }
        , {
            name: 'CUSTOMER_CODE', field: 'CUSTOMER_CODE', displayName: 'CODE', enableFiltering: true, width: '10%'
        }
        , {
            name: 'EFFECT_START_DATE', field: 'EFFECT_START_DATE', displayName: 'EFFECT START DATE', enableFiltering: true, width: '15%'
        }
        , {
            name: 'EFFECT_END_DATE', field: 'EFFECT_END_DATE', displayName: 'EFFECT END DATE', enableFiltering: true, width: '15%'
        }
        , {
            name: 'CREDIT_LIMIT', field: 'CREDIT_LIMIT', displayName: 'CREDIT LIMIT', enableFiltering: true, width: '10%'
        }
        , {
            name: 'CREDIT_DAYS', field: 'CREDIT_DAYS', displayName: 'CREDIT_DAYS', enableFiltering: true, width: '10%'
        }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];


    $scope.LoadCustomerTypeData = function () {
        $scope.showLoader = true;

        CreditInfoServices.LoadCustomerTypeData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.CustomerType = data.data;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }




    $scope.ClearForm = function () {
        window.location.href = "/SalesAndDistribution/CreditInfo/CreditPriceInfo";
    }
    $scope.DataLoad = function (companyId) {
        $scope.showLoader = true;
    
        setTimeout(function () {
            CreditInfoServices.LoadData($scope.model.companyId).then(function (data) {
                ;

                var dataList = data.data;
                $scope.gridOptionsList.data = dataList;
              
                $scope.showLoader = false;

            }, function (error) {
                alert(error);
                $scope.showLoader = false;

            });
        }, 2000)

    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        CreditInfoServices.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);
            for (let i = 0; i < $scope.Companies.length; i++) {
                if (parseInt($scope.Companies[i].COMPANY_ID) == $scope.model.COMPANY_ID) {
                    $scope.model.COMPANY_NAME = $scope.Companies[i].COMPANY_NAME;
                }
            }
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;

        CreditInfoServices.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }




    $scope.LoadUNIT_ID = function () {
        $('#UNIT_ID').trigger('change');
    }


    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'ProductPrice',
            Action_Name: 'ProductPriceInfo'
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

        CreditInfoServices.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            
            
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
    $scope.LoadCustomerData = function () {
        $scope.showLoader = true;

        CreditInfoServices.LoadCustomerData($scope.model.COMPANY_ID).then(function (data) {
            
            
            $scope.CustomerData = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }


    $scope.EditData = function (entity) {
       
        $scope.model.CREDIT_ID = entity.CREDIT_ID;
        $scope.model.COMPANY_ID = entity.COMPANY_ID;
        $scope.model.CUSTOMER_ID = entity.CUSTOMER_ID;
        $scope.model.CUSTOMER_CODE = entity.CUSTOMER_CODE;
        $scope.model.EFFECT_START_DATE = entity.EFFECT_START_DATE;
        $scope.model.EFFECT_END_DATE = entity.EFFECT_END_DATE;
        $scope.model.CUSTOMER_STATUS = entity.CUSTOMER_STATUS;
        $scope.model.REMARKS = entity.REMARKS;
        $scope.model.ENTRY_DATE = entity.ENTRY_DATE;
        $scope.model.CREDIT_LIMIT = entity.CREDIT_LIMIT;
        $scope.model.CREDIT_DAYS = entity.CREDIT_DAYS;


      
    }

    $scope.DataLoad(0);
    $scope.GetPermissionData();
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
    $scope.LoadUnitData();
    $scope.LoadCustomerData();
    $scope.LoadCustomerTypeData();

    var inputs = document.querySelectorAll(".form-control");
    inputs.forEach(input => {
        input.addEventListener('focusin', () => {
            input.style.border = '1px solid green';
        }, false);
    })

    $scope.SaveData = function (model) {
        if ($scope.formMenuCategoryAdd.$invalid) {
            let err = $scope.formMenuCategoryAdd.$error.required;
            for (var i = 0; i < err.length; i++) {
                let input = document.getElementById(err[i].$$attr.id);
                input.style.border = '3px solid red';
            }
            alert("Please Select " + err[0].$$attr.id);
            return;
        }
        if (model.CREDIT_DAYS <= 0 || model.CREDIT_LIMIT <= 0 ) {
            alert('CREDIT DAYS AND CREDIT LIMIT CAN NOT BE ZERO OR NEGATIVE');
            return;
        }

        var flag = false;
        $scope.showLoader = true;
        CreditInfoServices.AddOrUpdate(model).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;

                window.setTimeout(function () {
                    window.location.href = "/SalesAndDistribution/CreditInfo/CreditInfo";
                }, 2000)
            }
            else {

                $scope.showLoader = false;
            }
      
        });
    }

}]);


