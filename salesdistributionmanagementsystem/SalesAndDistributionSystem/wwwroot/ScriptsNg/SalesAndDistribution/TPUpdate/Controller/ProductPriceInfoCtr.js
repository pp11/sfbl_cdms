ngApp.controller('ngGridCtrl', ['$scope', 'ProductPriceInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, ProductPriceInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {
        COMPANY_ID : 0
        , EMPLOYEE_PRICE:  0
        , GROSS_PROFIT : 0
        , MRP : 0
        , SKU_CODE : ''
        , SKU_ID : 0
        , SPECIAL_PRICE : 0
        , SUPPLIMENTARY_TAX : 0
        , UNIT_ID : 0
        , UNIT_TP : 0
        , UNIT_VAT : 0
        , PRICE_ID : 0
    }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Unit = [];
    $scope.ProductList = [];
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Product Price Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }

    $scope.model.UNIT_ID = 9;

    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false,  width: '50'
        } 
       
        , { name: 'PRICE_ID', field: 'PRODUCT_TYPE_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , { name: 'SKU_ID', field: 'SKU_ID', visible: false }
        , { name: 'UNIT_ID', field: 'UNIT_ID', visible: false }
        , { name: 'PRICE_ID', field: 'PRICE_ID', visible: false }
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'Product', enableFiltering: true, width: '15%'
        }
        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'Code', enableFiltering: true, width: '10%'
        }
        , {
            name: 'MRP', field: 'MRP', displayName: 'MRP Price', enableFiltering: true, width: '8%'
        }

        , {
            name: 'EMPLOYEE_PRICE', field: 'EMPLOYEE_PRICE', displayName: 'Employee Price', enableFiltering: true, width: '8%'
        }
        , {
            name: 'SPECIAL_PRICE', field: 'SPECIAL_PRICE', displayName: 'Special Price', enableFiltering: true, width: '8%'
        }
       
        , {
            name: 'GROSS_PROFIT', field: 'GROSS_PROFIT', displayName: 'Gross Profit', enableFiltering: true, width: '8%'
        }
        , { name: 'SUPPLIMENTARY_TAX', field: 'SUPPLIMENTARY_TAX', displayName: 'Supplimentary Tax', enableFiltering: true, width: '8%' }
        , { name: 'UNIT_TP', field: 'UNIT_TP', displayName: 'Unit TP', enableFiltering: true, width: '8%' }
        , { name: 'UNIT_VAT', field: 'UNIT_VAT', displayName: 'Unit VAT', enableFiltering: true, width: '8%' }
        , { name: 'PRICE_EFFECT_DATE', field: 'PRICE_EFFECT_DATE', displayName: 'Price Effect Start', enableFiltering: true, width: '12%' }
        , { name: 'PRICE_ENTRY_DATE', field: 'PRICE_ENTRY_DATE', displayName: 'Price Entry Start', enableFiltering: true, width: '12%' }

        ,{
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];
    $scope.gridOptionsList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.DataLoad = function (companyId) {
        
        $scope.showLoader = true;

        ProductPriceInfoServices.LoadData(companyId).then(function (data) {
            debugger
            $scope.gridOptionsList.data = data.data;
            
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = companyId;
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;

        ProductPriceInfoServices.GetCompany().then(function (data) {
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

        ProductPriceInfoServices.GetCompanyList().then(function (data) {
            
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    
  
    $scope.ClearForm = function () {
        window.location.href = "/SalesAndDistribution/ProductPrice/ProductPriceInfo";
    }
    $scope.LoadFormData = function () {
        
        $scope.showLoader = true;

        ProductPriceInfoServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = $scope.model.COMPANY_ID;
            for (var i in $scope.gridOptionsList.data) {
                
                if ($scope.gridOptionsList.data[i].SKU_CODE == $scope.model.SKU_CODE) {
                    $scope.EditData($scope.gridOptionsList.data[i]);
                }
            }
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.EditData = function (entity) {
        

        $scope.model.COMPANY_ID = entity.COMPANY_ID;
        $scope.model.EMPLOYEE_PRICE = entity.EMPLOYEE_PRICE;
        $scope.model.GROSS_PROFIT = entity.GROSS_PROFIT;
        $scope.model.MRP = entity.MRP;
        $scope.model.SKU_CODE = entity.SKU_CODE;
        $scope.model.SKU_NAME = entity.SKU_NAME;
        $scope.model.PRICE_EFFECT_DATE = entity.PRICE_EFFECT_DATE;
        $scope.model.PRICE_ENTRY_DATE = entity.PRICE_ENTRY_DATE;

        $scope.model.SKU_ID = entity.SKU_ID;
        $scope.model.SPECIAL_PRICE = entity.SPECIAL_PRICE;
        $scope.model.SUPPLIMENTARY_TAX = entity.SUPPLIMENTARY_TAX;
        $scope.model.UNIT_ID = entity.UNIT_ID;
        $scope.model.UNIT_TP = entity.UNIT_TP;
        $scope.model.UNIT_VAT = entity.UNIT_VAT;
        $scope.model.PRICE_ID = entity.PRICE_ID;
        $interval(function () {
            $('#SKU_ID').trigger('change');
        }, 800, 4);
        $interval(function () {
            $scope.LoadUNIT_ID();
        }, 800, 2);
        
       
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

        ProductPriceInfoServices.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            
            
            $scope.Unit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
  
    $scope.AutoCompleteDataLoadForPrpoduct = function (value) {
        if (value.length >= 3) {
            

            return ProductPriceInfoServices.GetSearchableProduct( $scope.model.COMPANY_ID,value).then(function (data) {
                $scope.ProductList = data.data;
                

                return $scope.ProductList;
            }, function (error) {
                alert(error);
                

                
            });
        }
    }
    $scope.LoadProductData = function () {
        $scope.showLoader = true;

        ProductPriceInfoServices.LoadProductData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.ProductList = data.data;
           
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            
            $scope.showLoader = false;

        });
    }

    $scope.typeaheadSelectedProduct = function (entity, selectedItem) {
        $scope.model.SKU_ID = selectedItem.SKU_ID;
        $scope.model.SKU_NAME = selectedItem.SKU_NAME;
        $scope.model.SKU_CODE = selectedItem.SKU_CODE;

    };
    $scope.OnProductSelect = function () {
        
        var selectedItem = $scope.ProductList.filter((x) => x.SKU_ID == parseInt($scope.model.SKU_ID));
        $scope.model.SKU_ID = selectedItem[0].SKU_ID;
        $scope.model.SKU_NAME = selectedItem[0].SKU_NAME;
        $scope.model.SKU_CODE = selectedItem[0].SKU_CODE;
        $interval(function () {
            $('#SKU_ID').trigger('change');
        }, 800, 4);
    };
    $scope.LoadProductData();
    $scope.GetPermissionData();
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
    $scope.LoadUnitData();
    $scope.DataLoad(0);
   
    $scope.SaveData = function (model) {
        
        $scope.showLoader = true;
        
        model.UNIT_ID = parseInt(model.UNIT_ID);
        if (isNaN(model.GROSS_PROFIT) || model.GROSS_PROFIT == null) {
            model.GROSS_PROFIT = 0;
        }
        if (isNaN(model.SUPPLIMENTARY_TAX) || model.SUPPLIMENTARY_TAX == null) {
            model.SUPPLIMENTARY_TAX = 0;
        }
        if (isNaN(model.UNIT_VAT) || model.UNIT_VAT == null) {
            model.UNIT_VAT = 0;
        }
        ProductPriceInfoServices.AddOrUpdate(model).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;

                setTimeout($scope.ClearForm,3000);
                

            }
            else {
                $scope.showLoader = false;
            }
        });
    }

}]);

