ngApp.controller('ngGridCtrl', ['$scope', 'ProductInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, ProductInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'
    $scope.model = {
        BASE_PRODUCT_ID: 0
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
        , REMARKS: ''
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
    $scope.Unit_RELATION = [];
    $scope.PrimaryProduct = [];
    $scope.ProductSeason = [];
    $scope.Segments = [];
    $scope.ProductType = [];
    $scope.BaseProduct = [];
    $scope.Group = [];
    $scope.PackSize = [];
    $scope.SkuCode = [];
    $scope.ProducStorage = [];
    $scope.Status = [];
    $scope.CompanyUnit = [];
    $scope.product_list = [];
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Product Info"));
    $scope.gridOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
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
            name: 'PACK_SIZE', field: 'PACK_SIZE', displayName: 'Pack Size', enableFiltering: true, width: '15%'
        }

        //, {
        //    name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '25%'
        //}
        , { name: 'PRODUCT_STATUS', field: 'PRODUCT_STATUS', displayName: 'Status', enableFiltering: true, width: '18%' }
        , {
            name: 'Action', displayName: 'Action', width: '18%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];
    $scope.gridOptionsList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"


    $scope.gridSKUDepoOptionsList = (gridregistrationservice.GridRegistration("Product Relation  Info"));
    $scope.gridSKUDepoOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridSKUDepoOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
        }

        , { name: 'SKU_ID', field: 'SKU_ID', visible: false }
        , { name: 'DEPOT_ID', field: 'DEPOT_ID', visible: false }


        , {
            name: 'SKU_NAME', field: 'SKU_NAME', displayName: 'Name', enableFiltering: true, width: '22%'
        }
        , {
            name: 'SKU_CODE', field: 'SKU_CODE', displayName: 'Code', enableFiltering: true, width: '30%'
        }
        , {
            name: 'UNIT_NAME', field: 'UNIT_NAME', displayName: 'Depo Name', enableFiltering: true, width: '35%'
        }

        //, {
        //    name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: true, width: '25%'
        //}
        , {
            name: 'Action', displayName: 'Action', width: '18%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.DeleteSkuDepotRelation(row.entity)" type="button" class="btn btn-outline-primary mb-1">Delete</button>' +
                '</div>'
        },

    ];
    $scope.gridSKUDepoOptionsList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"


    


    $scope.gridPrimaryOptionsList = (gridregistrationservice.GridRegistration("Incomplete Product"));
    $scope.gridPrimaryOptionsList.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridPrimaryOptionsList.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false, width: '50'
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
        , {
            name: 'Action', displayName: 'Action', width: '18%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" data-dismiss="modal" aria-label="Close" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                '</div>'
        },

    ];
    $scope.gridPrimaryOptionsList.rowTemplate = "<div ng-dblclick=\"grid.appScope.EditData(row.entity)\" title=\"Please double click to edit \" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>"

    $scope.DataLoad = function (companyId) {
        $scope.showLoader = true;

        ProductInfoServices.LoadData(companyId).then(function (data) {
            $scope.gridOptionsList.data = data.data;
            $scope.product_list = data.data;
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
    $scope.LoadProductPrimaryData = function (companyId) {
        $scope.showLoader = true;

        ProductInfoServices.LoadProductPrimaryData(companyId).then(function (data) {
            $scope.gridPrimaryOptionsList.data = data.data;
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

    $scope.LoadProductPrimaryData();

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
    $scope.DeleteSkuDepotRelation = function (entity) {
        ProductInfoServices.DeleteSkuDepotRelation(entity.SKU_DEPO_ID).then(function (data) {
            notificationservice.Notification("The Specific relation is deleted", 1, '');
            $scope.LoadSKU_DEPOTData();
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.LoadSKU_DEPOTData = function () {
        ProductInfoServices.LoadSKU_DEPOTData().then(function (data) {
            $scope.gridSKUDepoOptionsList.data = [];
            $scope.gridSKUDepoOptionsList.data = data.data;

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

    $scope.LoadGroupData = function () {
        $scope.showLoader = true;

        ProductInfoServices.LoadGroupData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Group_dt = [];
            $scope.Group_dt[0] = {
                GRROUP_ID: '0',
                GROUP_NAME: 'None'
            };
            $scope.Group = $scope.Group_dt.concat(data.data);
            $scope.LoadPackSizeData();
            $scope.LoadSkuCodeData();
           
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.LoadPackSizeData = function () {
        $scope.showLoader = true;

        ProductInfoServices.LoadPackSizeData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Group_dt = [];
            $scope.Group_dt[0] = {
                PACK_SIZE: 'None',
            };
            $scope.PackSize = $scope.Group_dt.concat(data.data);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.LoadProductSegmentInfo = function () {
        $scope.showLoader = true;
        ProductInfoServices.LoadProductSegmentInfo().then(function (data) {
            $scope.Segments = data.data
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.LoadProductSegmentInfo();
    $scope.LoadSkuCodeData = function () {
        $scope.showLoader = true;
        ProductInfoServices.LoadSkuCodeData().then(function (data) {
            $scope.SkuCode = data.data;
            $scope.LoadPrimaryProductData();
        
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.LoadSkuInfo = function () {
        for (var i = 0; i < $scope.SkuCode.length; i++) {
            if ($scope.SkuCode[i].SKU_CODE == $scope.model.SKU_CODE) {
                $scope.model.SKU_NAME = $scope.SkuCode[i].SKU_NAME;
                $scope.model.PACK_SIZE = $scope.SkuCode[i].PACK_SIZE;
                $scope.model.SHIPPER_QTY = $scope.SkuCode[i].SHIPPER_QTY;
                $scope.model.SHOW_SKU_CODE = $scope.SkuCode[i].SKU_CODE;
            }
        }
    }

    $scope.LoadPrimaryProductData = function () {
        $scope.showLoader = true;

        ProductInfoServices.LoadPrimaryProductData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Group_dt = [];
            $scope.Group_dt[0] = {
                PRIMARY_PRODUCT_ID: '0',
                PRIMARY_PRODUCT_NAME: 'None'
            };
            $scope.PrimaryProduct = $scope.Group_dt.concat(data.data);
            $scope.LoadProductSeasonData();
            $scope.LoadProductTypeData();
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.LoadBaseProductData = function () {
        $scope.showLoader = true;

        ProductInfoServices.LoadBaseProductData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Group_dt = [];
            $scope.Group_dt[0] = {
                BASE_PRODUCT_ID: '0',
                BASE_PRODUCT_NAME: 'None'
            };
            $scope.BaseProduct = $scope.Group_dt.concat(data.data);
            $scope.LoadBrandData();
           
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.LoadBrandData = function () {
        $scope.showLoader = true;

        ProductInfoServices.LoadBrandData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Group_dt[0] = {
                BRAND_ID: '0',
                BRAND_NAME: 'None'
            };
            $scope.Brand = $scope.Group_dt.concat(data.data);
            $scope.LoadCategoryData();
            
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.LoadCategoryData = function () {
        $scope.showLoader = true;

        ProductInfoServices.LoadCategoryData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Group_dt[0] = {
                CATEGORY_ID: '0',
                CATEGORY_NAME: 'None'
            };
            $scope.Categories = $scope.Group_dt.concat(data.data);
            $scope.LoadGroupData();
           
            $scope.LoadStorageData();
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.LoadProductSeasonData = function () {
        $scope.showLoader = true;

        ProductInfoServices.LoadProductSeasonData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Group_dt[0] = {
                PRODUCT_SEASON_ID: '0',
                PRODUCT_SEASON_NAME: 'None'
            };
            $scope.ProductSeason = $scope.Group_dt.concat(data.data);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.LoadProductTypeData = function () {
        $scope.showLoader = true;

        ProductInfoServices.LoadProductTypeData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Group_dt[0] = {
                PRODUCT_TYPE_ID: '0',
                PRODUCT_TYPE_NAME: 'None'
            };
            $scope.ProductType = $scope.Group_dt.concat(data.data);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.LoadStorageData = function () {
        $scope.showLoader = true;

        ProductInfoServices.LoadStorageData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Group_dt[0] = {
                STORAGE_ID: '0',
                STORAGE_NAME: 'None'
            };
            $scope.ProducStorage = $scope.Group_dt.concat(data.data);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.LoadUnitData = function () {
        $scope.showLoader = true;

        ProductInfoServices.LoadUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Unit = data.data;
            $scope.Unit_RELATION = data.data;

            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.LoadStatusFilteredDataAll = function () {
        if ($scope.AllCustomer == true) {
            $scope.DataLoad($scope.model.COMPANY_ID);
            $scope.ActiveCustomer = false;
            $scope.InActiveCustomer = false;
        }
    }
    $scope.LoadStatusFilteredDataActive = function () {
        if ($scope.ActiveCustomer == true) {
            $scope.showLoader = true;

            ProductInfoServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
                $scope.gridOptionsList.data = data.data.filter(function (element) { return element.PRODUCT_STATUS == 'Active' });;
                $scope.AllCustomer = false;
                $scope.InActiveCustomer = false;
                $scope.showLoader = false;
            }, function (error) {
                alert(error);

                $scope.showLoader = false;
            });
        }
    }
    $scope.LoadStatusFilteredDataInActive = function () {
        if ($scope.InActiveCustomer == true) {
            $scope.showLoader = true;

            ProductInfoServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
                $scope.gridOptionsList.data = data.data.filter(function (element) { return element.PRODUCT_STATUS == 'InActive' });;
                $scope.AllCustomer = false;
                $scope.ActiveCustomer = false;
                $scope.showLoader = false;
            }, function (error) {
                alert(error);

                $scope.showLoader = false;
            });
        }
    }
    //$scope.LoadGeneratedProductCode = function () {
    //    $scope.showLoader = true;
    //
    //    ProductInfoServices.GenerateProductCode($scope.model.COMPANY_ID).then(function (data) {
    //
    //
    //        $scope.model.SKU_CODE = data.data;
    //        $scope.showLoader = false;
    //    }, function (error) {
    //        alert(error);

    //        $scope.showLoader = false;

    //    });
    //}
    $scope.LoadStatus = function () {
        var Active = {
            STATUS: 'Active'
        }
        var InComplete = {
            STATUS: 'Incomplete'
        }
        var InActive = {
            STATUS: 'InActive'
        }
        $scope.Status.push(Active);
        $scope.Status.push(InComplete);

        $scope.Status.push(InActive);
    }
    $scope.ClearForm = function () {
        window.location.href = "/SalesAndDistribution/Product/ProductInfo";
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
        $scope.model.SHOW_SKU_CODE = entity.SKU_CODE; //ridwan Work For sku code showing
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
        $scope.model.DISTRIBUTOR_PRODUCT_TYPE = entity.DISTRIBUTOR_PRODUCT_TYPE;
        $scope.model.PRODUCT_SEGMENT_NAME = entity.PRODUCT_SEGMENT_NAME??'';


        $interval(function () {
            $scope.LoadSkuCode
        }, 800, 2);

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

    $scope.LoadSkuCode = function () {
        $('#SKU_CODE').trigger('change');
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
    $scope.LoadBaseProductData();
  
    $scope.LoadUnitData();
    $scope.LoadCompanyUnitData();

    $scope.DataLoad($scope.model.COMPANY_ID);

    $scope.SaveData = function (model) {
        if ($scope.formMenuCategoryAdd.$invalid) {
            let err = $scope.formMenuCategoryAdd.$error.required;
            for (var i = 0; i < err.length; i++) {
                let input = document.getElementById(err[i].$$attr.id);
                input.style.border = '3px solid red';
            }
            notificationservice.Notification("Please Select " + err[0].$$attr.id, 1, '');

            //alert("Please Select " + err[0].$$attr.id);
            return;
        }

        $scope.submit_flag = true;

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
        if (isNaN(model.STORAGE_ID)) {
            model.STORAGE_ID = 0;
        } else { model.STORAGE_ID = parseInt(model.STORAGE_ID); }

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
        if (isNaN(model.STORAGE_ID)) {
            model.STORAGE_ID = 0;
        }
        model.WEIGHT_PER_PACK = model.WEIGHT_PER_PACK == null ? 0 : model.WEIGHT_PER_PACK;

        model.WEIGHT_UNIT = model.WEIGHT_UNIT == null ? '' : model.WEIGHT_UNIT;
        model.UNIT_ID = parseInt(model.UNIT_ID ?? '0');

        if (model.PRODUCT_STATUS == 'Active'
        ) {
            if (model.QTY_PER_PACK < 1) {
                notificationservice.Notification('Qty Per pack can not be 0', 1, 'Qty Per pack can not be 0');
                $scope.submit_flag = false;
            } else if (model.SHIPPER_VOLUME < 0) {
                notificationservice.Notification('Shipper volume can not be 0', 1, 'Shipper volume can not be 0');
                $scope.submit_flag = false;
            } else if (model.SHIPPER_WEIGHT < 0) {
                notificationservice.Notification('Shipper weight can not be 0', 1, 'Shipper weight can not be 0');
                $scope.submit_flag = false;
            }
            $scope.showLoader = false;
        }
        if ($scope.submit_flag == true) {
            ProductInfoServices.AddOrUpdate(model).then(function (data) {
                notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
                if (data.data == 1) {
                    $scope.LoadProductPrimaryData();

                    $scope.LoadFormData();
                    $scope.showLoader = false;
                }
                else {
                    $scope.showLoader = false;
                }
            });
        }
    }


    $scope.SaveSKUDepotRelation = function () {
        $scope.submit_flag_relation = true;
        if ($scope.UNIT_CUSTOMER == undefined || isNaN($scope.UNIT_CUSTOMER) || $scope.UNIT_CUSTOMER == 0) {
            $scope.submit_flag_relation = false;
            notificationservice.Notification("Please select Depot!", 1, 'Please select Depot!');

        }
        if ($scope.SKU_CUSTOMER == undefined || isNaN($scope.SKU_CUSTOMER) || $scope.SKU_CUSTOMER == 0) {
            $scope.submit_flag_relation = false;
            notificationservice.Notification("Please select SKU!", 1, 'Please select SKU!');

        }

        var ind = $scope.gridSKUDepoOptionsList.data.findIndex(x => x.SKU_ID == $scope.SKU_CUSTOMER && x.DEPOT_ID == $scope.UNIT_CUSTOMER);

        if (ind != -1) {
            $scope.submit_flag_relation = false;
            notificationservice.Notification("The Relation Already exist! Please check " + ind + " No Row", 1, "The Relation Already exist! Please check " + ind + " No Row");

        }

        if ($scope.submit_flag_relation == true) {
            $scope.depo_model = {
                SKU_ID: parseInt($scope.SKU_CUSTOMER),
                SKU_CODE: $scope.product_list.filter(x => x.SKU_ID == $scope.SKU_CUSTOMER)[0].SKU_CODE,
                DEPOT_ID: parseInt($scope.UNIT_CUSTOMER),
                SKU_DEPO_ID : 0,
                ENTERED_DATE : '',
                ENTERED_BY : '',
                ENTERED_TERMINAL : '',
                COMPANY_ID : 1,
                ROW_NO: 0
             }
          

        //Helper Attributes
            ProductInfoServices.AddSkuDepotRelation($scope.depo_model).then(function (data) {
                notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
                if (data.data == 1) {
                    $scope.LoadSKU_DEPOTData();
                    $scope.SKU_CUSTOMER = "0";
                    $scope.UNIT_CUSTOMER = "0";

                    $scope.showLoader = false;
                }
                else {
                    $scope.showLoader = false;
                }
            });
        }
    }
}]);