ngApp.controller('ngGridCtrl', ['$scope', 'ProductInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, ProductInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {
        
          BASE_PRODUCT_ID : 0
        , PRIMARY_PRODUCT_ID: 0
        , PRODUCT_SEASON_ID: 0
        , PRODUCT_TYPE_ID: 0
        , BRAND_ID: 0
        , STORAGE_ID: 0
        , SKU_ID: 0
        , UNIT_ID: 0
        , CATEGORY_ID: 0
        , COMPANY_ID: 0
        , GROUP_ID: 0
        , SKU_CODE: ''
        , SKU_NAME: ''
        , SKU_NAME_BANGLA: ''
        , FONT_COLOR: ''
        , PACK_SIZE: ''
        , PACK_UNIT: ''
        , PACK_VALUE: 0
        , PRODUCT_STATUS: 'Active'
        , QTY_PER_PACK: 0
        , SHIPPER_QTY: 0
        , REMARKS: 0
        , SHIPPER_VOLUME: 0
        , SHIPPER_VOLUME_UNIT: ''
        , SHIPPER_WEIGHT: 0
        , SHIPPER_WEIGHT_UNIT: ''
        , WEIGHT_UNIT: ''
        , WEIGHT_PER_PACK: 0
    }

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Categories = [];
    $scope.Brand = [];
    $scope.Unit = [];
    $scope.PrimaryProduct = [];
    $scope.ProductSeason = [];
    $scope.ProductType = [];
    $scope.BaseProduct = [];
    $scope.Group = [];
    $scope.PackSize = [];
    $scope.ProducStorage = [];
    $scope.Status = [];
    $scope.CompanyUnit = [];
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Product Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false,  width: '50'
        }

        , { name: 'SKU_ID', field: 'SKU_ID', visible: false }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
     
        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'Name', enableFiltering: true, width: '22%'
        }
        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'Code', enableFiltering: true, width: '15%'
        }
       
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '25%'
        }
        , { name: 'PRODUCT_STATUS', field: 'PRODUCT_STATUS', displayName: 'Status', enableFiltering: true, width: '18%' }
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

        ProductInfoServices.LoadProductPrimaryData(companyId).then(function (data) {
            
            
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = companyId;
            $interval(function () {
                $scope.LoadCOMPANY_ID();
            }, 800, 2);
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {

        ProductInfoServices.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.CompanyNameLoad = function () {

        ProductInfoServices.GetCompanyName().then(function (data) {
            $scope.model.COMPANY_NAME = data.data;


            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }

    $scope.LoadCompanyUnitData = function () {
        $scope.showLoader = true;

        ProductInfoServices.LoadCompanyUnitData($scope.model.COMPANY_ID).then(function (data) {
            
            
            $scope.CompanyUnit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });;
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
   

    $scope.LoadStatus = function () {

        var Active = {
            STATUS: 'Active'
        }
        var Active = {
            STATUS: 'Primary'
        }
        var InActive = {
            STATUS: 'InActive'
        }
        $scope.Status.push(Active);
        $scope.Status.push(InActive);

    }
    $scope.ClearForm = function () {
        window.location.href = "/SalesAndDistribution/Product/ProductPrimaryInfo";

    }
    $scope.LoadFormData = function () {
        
        $scope.showLoader = true;

        ProductInfoServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
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
        
        $scope.model.SKU_ID = entity.SKU_ID;
        $scope.model.SKU_NAME = entity.SKU_NAME;
        $scope.model.SKU_CODE = entity.SKU_CODE;
        $scope.model.SKU_NAME_BANGLA = entity.SKU_NAME_BANGLA;
        $scope.model.COMPANY_ID = entity.COMPANY_ID;
        $scope.model.PRODUCT_STATUS = entity.PRODUCT_STATUS;
        $scope.model.FONT_COLOR = entity.FONT_COLOR;
        $scope.model.PACK_UNIT = entity.PACK_UNIT;
        $scope.model.QTY_PER_PACK = entity.QTY_PER_PACK;
        $scope.model.REMARKS = entity.REMARKS;
        $scope.model.SHIPPER_QTY = entity.SHIPPER_QTY;
        $scope.model.SHIPPER_VOLUME = entity.SHIPPER_VOLUME;
        $scope.model.SHIPPER_WEIGHT = entity.SHIPPER_WEIGHT;
        $scope.model.UNIT_ID = entity.UNIT_ID;
        $scope.model.PACK_SIZE = entity.PACK_SIZE;
        $scope.model.WEIGHT_PER_PACK = entity.WEIGHT_PER_PACK;
        $scope.model.WEIGHT_UNIT = entity.WEIGHT_UNIT;

        $scope.model.BASE_PRODUCT_ID = entity.BASE_PRODUCT_ID;
        $scope.model.BRAND_ID = entity.BRAND_ID;
        $scope.model.CATEGORY_ID = entity.CATEGORY_ID;
        $scope.model.GROUP_ID = entity.GROUP_ID;
        $scope.model.PRIMARY_PRODUCT_ID = entity.PRIMARY_PRODUCT_ID;
        $scope.model.PRODUCT_SEASON_ID = entity.PRODUCT_SEASON_ID;
        $scope.model.PRODUCT_TYPE_ID = entity.PRODUCT_TYPE_ID;
        $scope.model.STORAGE_ID = entity.STORAGE_ID;
        $scope.model.SHIPPER_VOLUME_UNIT = entity.SHIPPER_VOLUME_UNIT;
        $scope.model.SHIPPER_WEIGHT_UNIT = entity.SHIPPER_WEIGHT_UNIT;
        $scope.model.PACK_VALUE = entity.PACK_VALUE;

        $interval(function () {
            $scope.LoadPRODUCT_SEASON_ID();
        }, 800, 2);
        $interval(function () {
            $scope.LoadPRIMARY_PRODUCT_ID();
        }, 800, 2);
        $interval(function () {
            $scope.LoadPRODUCT_TYPE_ID();
        }, 800, 2);
        $interval(function () {
            $scope.LoadBASE_PRODUCT_ID();
        }, 800, 2);
        $interval(function () {
            $scope.LoadSTORAGE_ID();
        }, 800, 2);
      
        $interval(function () {
            $scope.LoadBRAND_ID();
        }, 800, 2);
        $interval(function () {
            $scope.LoadUNIT_ID();
        }, 800, 2);
        $interval(function () {
            $scope.LoadCATEGORY_ID();
        }, 800, 2);
        $interval(function () {
            $scope.LoadGROUP_ID();
        }, 800, 2);
       
      
      
    }
    $scope.LoadPRODUCT_SEASON_ID = function () {
        $('#PRODUCT_SEASON_ID').trigger('change');

    }
    $scope.LoadPRIMARY_PRODUCT_ID = function () {
        $('#PRIMARY_PRODUCT_ID').trigger('change');

    }
    $scope.LoadPRODUCT_TYPE_ID = function () {
        $('#PRODUCT_TYPE_ID').trigger('change');

    }
    $scope.LoadBASE_PRODUCT_ID = function () {
        $('#BASE_PRODUCT_ID').trigger('change');

    }
    $scope.LoadSTORAGE_ID = function () {
        $('#STORAGE_ID').trigger('change');

    }
    $scope.LoaPACK_SIZE = function () {
        $('#PACK_SIZE').trigger('change');
    }
    $scope.LoadUNIT_ID = function () {
        $('#UNIT_ID').trigger('change');
    }
    $scope.LoadBRAND_ID = function () {
        $('#BRAND_ID').trigger('change');
    }
    $scope.LoadCATEGORY_ID = function () {
        $('#CATEGORY_ID').trigger('change');
    }
    $scope.LoadGROUP_ID = function () {
        $('#GROUP_ID').trigger('change');
    }
    $scope.LoadWEIGHT_UNIT = function () {
        $('#WEIGHT_UNIT').trigger('change');
    }
    $scope.LoadCOMPANY_ID = function () {
        $('#COMPANY_ID').trigger('change');
    }
    $scope.LoadSHIPPER_VOLUME_UNIT = function () {
        $('#SHIPPER_VOLUME_UNIT').trigger('change');
    }
    $scope.LoadSHIPPER_WEIGHT_UNIT = function () {
        $('#SHIPPER_WEIGHT_UNIT').trigger('change');
    }
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'Product',
            Action_Name: 'ProductInfo'
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

    $scope.LoadStatus();
    //$scope.LoadGeneratedProductCode();  commented date: 6/27/2022
    //$scope.LoadBaseProductData();
    //$scope.LoadBrandData();
    //$scope.LoadCategoryData();
    //$scope.LoadGroupData();
    //$scope.LoadPackSizeData();
    $scope.LoadPrimaryProductData();
    $scope.LoadProductSeasonData();
    $scope.LoadProductTypeData();
    $scope.LoadStorageData();
    $scope.LoadUnitData();
    $scope.LoadCompanyUnitData();

    $scope.DataLoad($scope.model.COMPANY_ID);



    $scope.SaveData = function (model) {
        
        $scope.showLoader = true;
        if (isNaN(model.BASE_PRODUCT_ID)) {
            model.BASE_PRODUCT_ID = 0;
        } else { model.BASE_PRODUCT_ID = parseInt(model.BASE_PRODUCT_ID); }
        if (isNaN(model.PRODUCT_TYPE_ID)) {
            model.PRODUCT_TYPE_ID = 0;
        } else { model.PRODUCT_TYPE_ID = parseInt(model.PRODUCT_TYPE_ID); }
        if (isNaN(model.SKU_ID)) {
            model.SKU_ID = 0;
        } else { model.SKU_ID = parseInt(model.SKU_ID); }
        if (isNaN(model.PRODUCT_SEASON_ID)) {
            model.PRODUCT_SEASON_ID = 0;
        } else { model.PRODUCT_SEASON_ID = parseInt(model.PRODUCT_SEASON_ID); }
        if (isNaN(model.PRIMARY_PRODUCT_ID)) {
            model.PRIMARY_PRODUCT_ID = 0;
        } else { model.PRIMARY_PRODUCT_ID = parseInt(model.PRIMARY_PRODUCT_ID); }
        if (isNaN(model.GROUP_ID)) {
            model.GROUP_ID = 0;
        } else { model.GROUP_ID = parseInt(model.GROUP_ID); }
        if (isNaN(model.COMPANY_ID)) {
            model.COMPANY_ID = 0;
        } else { model.COMPANY_ID = parseInt(model.COMPANY_ID); }
        if (isNaN(model.CATEGORY_ID)) {
            model.CATEGORY_ID = 0;
        } else { model.CATEGORY_ID = parseInt(model.CATEGORY_ID); }
        if (isNaN(model.BRAND_ID)) {
            model.BRAND_ID = 0;
        } else { model.BRAND_ID = parseInt(model.BRAND_ID); }
        if (isNaN(model.BASE_PRODUCT_ID)) {
            model.BASE_PRODUCT_ID = 0;
        } else { model.BASE_PRODUCT_ID = parseInt(model.BASE_PRODUCT_ID); }
        model.FONT_COLOR = model.FONT_COLOR == null ? '' : model.FONT_COLOR;
        model.PACK_UNIT = model.PACK_UNIT == null ? '' : model.PACK_UNIT;
        model.PACK_VALUE = model.PACK_VALUE == null ? 0 : model.PACK_VALUE;
        model.QTY_PER_PACK = model.QTY_PER_PACK == null ? 0 : model.QTY_PER_PACK;
        model.REMARKS = model.REMARKS == null ? '' : model.REMARKS;
        model.SHIPPER_VOLUME = model.SHIPPER_VOLUME == null ? 0 : model.SHIPPER_VOLUME;
        model.SHIPPER_VOLUME_UNIT = model.SHIPPER_VOLUME_UNIT == null ? '' : model.SHIPPER_VOLUME_UNIT;
        model.SHIPPER_WEIGHT = model.SHIPPER_WEIGHT == null ? 0 : model.SHIPPER_WEIGHT;
        model.SHIPPER_WEIGHT_UNIT = model.SHIPPER_WEIGHT_UNIT == null ? '' : model.SHIPPER_WEIGHT_UNIT;
        model.SKU_NAME_BANGLA = model.SKU_NAME_BANGLA == null ? '' : model.SKU_NAME_BANGLA;
        if (isNaN(model.STORAGE_ID))
         {
              model.STORAGE_ID = 0;
         }
        model.WEIGHT_PER_PACK = model.WEIGHT_PER_PACK == null ? 0 : model.WEIGHT_PER_PACK;

        model.WEIGHT_UNIT = model.WEIGHT_UNIT == null ? '' : model.WEIGHT_UNIT;
        model.UNIT_ID = model.UNIT_ID == null ? 0 : model.UNIT_ID;

        
        ProductInfoServices.AddOrUpdate(model).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                $scope.showLoader = false;

                $scope.LoadFormData();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

}]);

