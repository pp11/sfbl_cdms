ngApp.controller('ngGridCtrl', ['$scope', 'MasterReportServices', 'permissionProvider', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, MasterReportServices, permissionProvider, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = {
        COMPANY_ID: 0, PRODUCT_SEASON_ID: '', PRODUCT_TYPE_ID: '', PRIMARY_PRODUCT_ID: '', UNIT_ID: '', BASE_PRODUCT_ID: '', CATEGORY_ID: '', BRAND_ID: '', DIVISION_ID: ''
        , REGION_ID: '', AREA_ID: '', TERRITORY_ID: '', MARKET_ID: '', CUSTOMER_ID: '', PRODUCT_BONUS_MST_ID: 'ALL'
    }
    $scope.model.DATE_FROM = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.model.DATE_TO = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Brands = [];
    $scope.ReportData = [];
    $scope.Category = [];
    $scope.Companies = [];
    $scope.Unit = [];
    $scope.PrimaryProduct = [];
    $scope.ProductSeason = [];
    $scope.ProductType = [];
    $scope.ProductBonus = [];
    $scope.BaseProduct = [];
    $scope.Group = [];
    $scope.PackSize = [];
    $scope.ProducStorage = [];
    $scope.Status = [];
    $scope.CompanyUnit = [];

    /* --------------Customer Info--------------*/
    $scope.Divisions = [];
    $scope.Regions = [];
    $scope.Areas = [];
    $scope.Territories = [];
    $scope.Markets = [];
    $scope.GetDivitionToMarketRelations = [];
    /* --------------Customer Info--------------*/


    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;
        MasterReportServices.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.CompanyLoad = function () {
        $scope.showLoader = true;
        MasterReportServices.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);
            for (let i = 0; i < $scope.Companies.length; i++) {
                if (parseInt($scope.Companies[i].COMPANY_ID) == $scope.model.COMPANY_ID) {
                    $scope.model.COMPANY_NAME = $scope.Companies[i].COMPANY_NAME;
                }
            }
            $interval(function () {
                $scope.LoadCOMPANY_ID();
            }, 800, 2);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }

    $scope.CompanyUnitLoad = function () {
        $scope.showLoader = true;
        MasterReportServices.GetUnitId().then(function (data) {
            let depot = $scope.CompanyUnit.find(e => e.UNIT_ID == parseInt(data.data));
            if ($scope.model.USER_TYPE != 'SuperAdmin') {
                $scope.CompanyUnit = [];
                $scope.CompanyUnit.push(depot);
            }
            $scope.model.UNIT_ID = parseInt(data.data);
            $interval(function () {

                $scope.LoadUNIT_ID();
            }, 800, 2);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }

    $scope.typeaheadSelectedCustomer = function (entity, selectedItem) {
        $scope.model.CUSTOMER_ID = selectedItem.CUSTOMER_ID;
        $scope.model.CUSTOMER_CODE = selectedItem.CUSTOMER_CODE;

    };

    $scope.AutoCompleteDataLoadForCustomer = function (value) {
        return MasterReportServices.GetSearchableCustomer($scope.model.COMPANY_ID, value).then(function (data) {
            $scope.CustomersList = [];
            for (var i = 0; i < data.data.length; i++) {
                var _customer = {
                    CUSTOMER_CODE: data.data[i].CUSTOMER_CODE,
                    CUSTOMER_NAME: data.data[i].CUSTOMER_NAME,
                    CUSTOMER_ID: data.data[i].CUSTOMER_ID,
                    CUSTOMER_NAME_CODE: data.data[i].CUSTOMER_NAME + ' | CODE:' + data.data[i].CUSTOMER_CODE
                }
                $scope.CustomersList.push(_customer);

            }

            return $scope.CustomersList;
        }, function (error) {
            alert(error);



        });
    }

    //#region Customer Info Report
    $scope.GetDivitionToMarketRelation = function () {
        $scope.showLoader = true;
        MasterReportServices.GetDivitionToMarketRelation($scope.model.DIVISION_ID).then(function (data) {
            $scope.GetDivitionToMarketRelations = data.data;
            $scope.Divisions = [];
            $scope.Regions = [];
            $scope.Areas = [];
            $scope.Territories = [];
            $scope.Markets = [];

            const map = new Map();
            const mapREGION_ID = new Map();
            const mapAREA_ID = new Map();
            const mapTERRITORY_ID = new Map();
            const mapMARKET_ID = new Map();
            for (const item of data.data) {
                if (!map.has(item.DIVISION_ID)) {
                    map.set(item.DIVISION_ID, true);    // set any value to Map
                    $scope.Divisions.push({
                        DIVISION_ID: item.DIVISION_ID,
                        DIVISION_NAME: item.DIVISION_NAME
                    });
                }
                if (!mapREGION_ID.has(item.REGION_ID)) {
                    mapREGION_ID.set(item.REGION_ID, true);    // set any value to Map
                    $scope.Regions.push({
                        REGION_ID: item.REGION_ID,
                        REGION_NAME: item.REGION_NAME
                    });
                }
                if (!mapAREA_ID.has(item.AREA_ID)) {
                    mapAREA_ID.set(item.AREA_ID, true);    // set any value to Map
                    $scope.Areas.push({
                        AREA_ID: item.AREA_ID,
                        AREA_NAME: item.AREA_NAME
                    });
                }
                if (!mapTERRITORY_ID.has(item.TERRITORY_ID)) {
                    mapTERRITORY_ID.set(item.TERRITORY_ID, true);    // set any value to Map
                    $scope.Territories.push({
                        TERRITORY_ID: item.TERRITORY_ID,
                        TERRITORY_NAME: item.TERRITORY_NAME
                    });
                }
                if (!mapMARKET_ID.has(item.MARKET_ID)) {
                    mapMARKET_ID.set(item.MARKET_ID, true);    // set any value to Map
                    $scope.Markets.push({
                        MARKET_ID: item.MARKET_ID,
                        MARKET_NAME: item.MARKET_NAME
                    });
                }

            }

            $scope.Divisions.unshift({ DIVISION_ID: '', DIVISION_NAME: 'ALL' })
            $scope.Regions.unshift({ REGION_ID: '', REGION_NAME: 'ALL' })
            $scope.Areas.unshift({ AREA_ID: '', AREA_NAME: 'ALL' })
            $scope.Territories.unshift({ TERRITORY_ID: '', TERRITORY_NAME: 'ALL' })
            $scope.Markets.unshift({ MARKET_ID: '', MARKET_NAME: 'ALL' })


        }, function (error) {

            console.log(error);
            $scope.showLoader = false;

        });
    }
    $scope.DivisionChange = function () {
        $scope.showLoader = true;
        let GetDivitionToMarketRelations = [];
        $scope.Regions = [];
        $scope.Areas = [];
        $scope.Territories = [];
        $scope.Markets = [];
        if ($scope.model.DIVISION_ID != '') {
            let DIVISION_ID = $scope.model.DIVISION_ID;
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations.filter((x) => { return x.DIVISION_ID == DIVISION_ID; });
        } else {

            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations;
        }
        const mapREGION_ID = new Map();
        const mapAREA_ID = new Map();
        const mapTERRITORY_ID = new Map();
        const mapMARKET_ID = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!mapREGION_ID.has(item.REGION_ID)) {
                mapREGION_ID.set(item.REGION_ID, true);    // set any value to Map
                $scope.Regions.push({
                    REGION_ID: item.REGION_ID,
                    REGION_NAME: item.REGION_NAME
                });
            }
            if (!mapAREA_ID.has(item.AREA_ID)) {
                mapAREA_ID.set(item.AREA_ID, true);    // set any value to Map
                $scope.Areas.push({
                    AREA_ID: item.AREA_ID,
                    AREA_NAME: item.AREA_NAME
                });
            }
            if (!mapTERRITORY_ID.has(item.TERRITORY_ID)) {
                mapTERRITORY_ID.set(item.TERRITORY_ID, true);    // set any value to Map
                $scope.Territories.push({
                    TERRITORY_ID: item.TERRITORY_ID,
                    TERRITORY_NAME: item.TERRITORY_NAME
                });
            }
            if (!mapMARKET_ID.has(item.MARKET_ID)) {
                mapMARKET_ID.set(item.MARKET_ID, true);    // set any value to Map
                $scope.Markets.push({
                    MARKET_ID: item.MARKET_ID,
                    MARKET_NAME: item.MARKET_NAME
                });
            }
        }

        $scope.Regions.unshift({ REGION_ID: '', REGION_NAME: 'ALL' })
        $scope.Areas.unshift({ AREA_ID: '', AREA_NAME: 'ALL' })
        $scope.Territories.unshift({ TERRITORY_ID: '', TERRITORY_NAME: 'ALL' })
        $scope.Markets.unshift({ MARKET_ID: '', MARKET_NAME: 'ALL' })
        $scope.model.REGION_ID = '';
        $scope.model.AREA_ID = '';
        $scope.model.TERRITORY_ID = '';
        $scope.model.MARKET_ID = '';
    }
    $scope.RegionChange = function () {
        $scope.showLoader = true;
        let GetDivitionToMarketRelations = [];
        $scope.Areas = [];
        $scope.Territories = [];
        $scope.Markets = [];
        if ($scope.model.REGION_ID != '') {
            let REGION_ID = $scope.model.REGION_ID;
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations.filter((x) => { return x.REGION_ID == REGION_ID; });
        } else {
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations;
        }
        var map = new Map();

        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.AREA_ID)) {
                map.set(item.AREA_ID, true);    // set any value to Map
                $scope.Areas.push({
                    AREA_ID: item.AREA_ID,
                    AREA_NAME: item.AREA_NAME
                });
            }
        }
        map = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.TERRITORY_ID)) {
                map.set(item.TERRITORY_ID, true);    // set any value to Map
                $scope.Territories.push({
                    TERRITORY_ID: item.TERRITORY_ID,
                    TERRITORY_NAME: item.TERRITORY_NAME
                });
            }
        }
        map = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.MARKET_ID)) {
                map.set(item.MARKET_ID, true);    // set any value to Map
                $scope.Markets.push({
                    MARKET_ID: item.MARKET_ID,
                    MARKET_NAME: item.MARKET_NAME
                });
            }
        }
        $scope.Areas.unshift({ AREA_ID: '', AREA_NAME: 'ALL' })
        $scope.Territories.unshift({ TERRITORY_ID: '', TERRITORY_NAME: 'ALL' })
        $scope.Markets.unshift({ MARKET_ID: '', MARKET_NAME: 'ALL' })
        $scope.model.AREA_ID = '';
        $scope.model.TERRITORY_ID = '';
        $scope.model.MARKET_ID = '';

    }
    $scope.AreaChange = function () {
        $scope.showLoader = true;
        let GetDivitionToMarketRelations = [];
        $scope.Territories = [];
        $scope.Markets = [];
        if ($scope.model.AREA_ID != '') {
            let AREA_ID = $scope.model.AREA_ID;
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations.filter((x) => { return x.AREA_ID == AREA_ID; });
        } else {
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations;
        }
        var map = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.TERRITORY_ID)) {
                map.set(item.TERRITORY_ID, true);    // set any value to Map
                $scope.Territories.push({
                    TERRITORY_ID: item.TERRITORY_ID,
                    TERRITORY_NAME: item.TERRITORY_NAME
                });
            }
        }
        map = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.MARKET_ID)) {
                map.set(item.MARKET_ID, true);    // set any value to Map
                $scope.Markets.push({
                    MARKET_ID: item.MARKET_ID,
                    MARKET_NAME: item.MARKET_NAME
                });
            }
        }

        $scope.Territories.unshift({ TERRITORY_ID: '', TERRITORY_NAME: 'ALL' })
        $scope.Markets.unshift({ MARKET_ID: '', MARKET_NAME: 'ALL' })
        $scope.model.TERRITORY_ID = '';
        $scope.model.MARKET_ID = '';

    }
    $scope.TerritoryChange = function () {
        $scope.showLoader = true;
        let GetDivitionToMarketRelations = [];

        $scope.Markets = [];
        if ($scope.model.TERRITORY_ID != '') {
            let TERRITORY_ID = $scope.model.TERRITORY_ID;
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations.filter((x) => { return x.TERRITORY_ID == TERRITORY_ID; });
        } else {
            GetDivitionToMarketRelations = $scope.GetDivitionToMarketRelations;
        }
        var map = new Map();
        for (const item of GetDivitionToMarketRelations) {
            if (!map.has(item.MARKET_ID)) {
                map.set(item.MARKET_ID, true);    // set any value to Map
                $scope.Markets.push({
                    MARKET_ID: item.MARKET_ID,
                    MARKET_NAME: item.MARKET_NAME
                });
            }
        }

        $scope.Markets.unshift({ MARKET_ID: '', MARKET_NAME: 'ALL' })
        $scope.model.MARKET_ID = '';

    }
    //#endregion

    $scope.DataLoad = function (companyId) {
        $scope.showLoader = true;
        MasterReportServices.LoadData(companyId).then(function (data) {

            $scope.ReportData = data.data;
            $scope.showLoader = false;
            $scope.model.COMPANY_SEARCH_ID = companyId;
            $scope.model.ReportColor = "color";

            $interval(function () {
                $scope.LoadCOMPANY_ID();
            }, 800, 2);
        }, function (error) {
            $scope.showLoader = false;

        });
    }
    $scope.GetBrandData = function () {
        $scope.showLoader = true;
        var companyId = 0;
        MasterReportServices.LoadBrand(companyId).then(function (data) {
            $scope.Brands = data.data;
            $scope.Brands.unshift({ BRAND_ID: '', BRAND_NAME: 'All' })
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }
    $scope.GetCategoryData = function () {
        $scope.showLoader = true;
        var companyId = 0;
        MasterReportServices.LoadCategory(companyId).then(function (data) {


            $scope.Category = data.data;
            $scope.Category.unshift({ CATEGORY_ID: '', CATEGORY_NAME: 'All' })
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }
    $scope.LoadCompanyUnitData = function () {
        $scope.showLoader = true;
        MasterReportServices.LoadCompanyUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.CompanyUnit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });
            $scope.CompanyUnit.unshift({ UNIT_ID: '', UNIT_NAME: 'All' })
            $scope.CompanyUnitLoad();
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.LoadGroupData = function () {
        $scope.showLoader = true;
        MasterReportServices.LoadGroupData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Group = data.data;
            $scope.Group.unshift({ GROUP_ID: '', GROUP_NAME: 'All' })
            //$scope.CompanyUnitLoad();
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }
    $scope.LoadPrimaryProductData = function () {
        $scope.showLoader = true;

        MasterReportServices.LoadPrimaryProductData($scope.model.COMPANY_ID).then(function (data) {
            $scope.PrimaryProduct = data.data;
            $scope.PrimaryProduct.unshift({ PRIMARY_PRODUCT_ID: '', PRIMARY_PRODUCT_NAME: 'All' })
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }
    $scope.LoadBaseProductData = function () {
        $scope.showLoader = true;
        MasterReportServices.LoadBaseProductData($scope.model.COMPANY_ID).then(function (data) {
            $scope.BaseProduct = data.data;
            $scope.BaseProduct.unshift({ BASE_PRODUCT_ID: '', BASE_PRODUCT_NAME: 'All' })
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }
    $scope.LoadProductSeasonData = function () {
        $scope.showLoader = true;

        MasterReportServices.LoadProductSeasonData($scope.model.COMPANY_ID).then(function (data) {
            $scope.ProductSeason = data.data;
            $scope.ProductSeason.unshift({ PRODUCT_SEASON_ID: '', PRODUCT_SEASON_NAME: 'All' })
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }
    $scope.LoadProductData = function () {
        $scope.showLoader = true;
        MasterReportServices.LoadProductData().then(function (data) {
            $scope.Products = data.data;
            
            $scope.showLoader = false;
            var _Products = {

                SKU_ID: "ALL",
                SKU_NAME: "---Select SKU---",
                SKU_CODE:"N/A"

            }

            $scope.Products.push(_Products);
            $scope.Products.reverse();
            $scope.model.SKU_ID = 'ALL';
        }, function (error) {
            alert(error);


            $scope.showLoader = false;

        });
    }
    $scope.LoadProductTypeData = function () {
        $scope.showLoader = true;

        MasterReportServices.LoadProductTypeData($scope.model.COMPANY_ID).then(function (data) {
            $scope.ProductType = data.data;
            $scope.ProductType.unshift({ PRODUCT_TYPE_ID: '', PRODUCT_TYPE_NAME: 'All' })
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }
    $scope.LoadProductBonuData = function (reportId) {
        $scope.showLoader = true;
        $scope.ProductBonus = [];
        if (reportId == 21) {
            MasterReportServices.LoadProductBonuData($scope.model.COMPANY_ID).then(function (data) {
                $scope.ProductBonus = data.data;
                $scope.ProductBonus.unshift({ PRODUCT_BONUS_MST_ID: 'ALL', BONUS_NAME: 'All' })
                $scope.showLoader = false;
                $scope.model.PRODUCT_BONUS_MST_ID = 'ALL';
            }, function (error) {
                $scope.showLoader = false;

            });
        } else {
            MasterReportServices.LoadBonuData($scope.model.COMPANY_ID).then(function (data) {
                for (var i = 0; i < data.data.length; i++) {
                    $scope.ProductBonus.unshift({ PRODUCT_BONUS_MST_ID: data.data[i].BONUS_MST_ID, BONUS_NAME: data.data[i].BONUS_NAME });
                }
                $scope.ProductBonus.unshift({ PRODUCT_BONUS_MST_ID: 'ALL', BONUS_NAME: 'All' })
                $scope.model.PRODUCT_BONUS_MST_ID = 'ALL';
                $scope.showLoader = false;
            }, function (error) {
                $scope.showLoader = false;

            });
        }

    }
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'Report',
            Action_Name: 'MasterReport'
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
            $scope.LoadCompanyUnitData();
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            $scope.showLoader = false;

        });
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

    $scope.LoadReportParamters = function (reportId, id_serial) {
        document.getElementById("company_unit_param").style.display = "block";
        $scope.showLoader = true;
        $scope.model.REPORT_ID = reportId;
        MasterReportServices.IsReportPermitted(reportId).then(function (data) {
            $scope.model.CSV_PERMISSION = data.data.CSV_PERMISSION;
            $scope.model.PREVIEW_PERMISSION = data.data.PREVIEW_PERMISSION;
            $scope.model.PDF_PERMISSION = data.data.PDF_PERMISSION;
            var _id = 'selection[' + id_serial + ']';
            var _id_selected = 'ReportIdEncrypt[' + id_serial + ']';
            let checkboxes = document.getElementsByClassName('checkbox-inline');
            for (var i = 0; i < checkboxes.length; i++) {
                let checkbox = document.getElementById(checkboxes[i].id);
                checkbox.checked = false;
            }
            let checkbox = document.getElementById(_id);
            $scope.model.reportIdEncryptedSelected = document.getElementById(_id_selected).value;
            checkbox.checked = true;
            document.getElementById("date_from_param").style.display = "none";
            document.getElementById("date_to_param").style.display = "none";
            if (reportId == 34 || reportId == 35 || reportId == 36) {  //Customer Price | Credit Policy | Customer Information
                $scope.ToggleLocationParam('none');
                $scope.ToggleProductParam('none');
                document.getElementById("company_customer_name_param").style.display = "block";
                document.getElementById("product_bonus_mst_id_param").style.display = "none";
            } else if (reportId == 1 || reportId == 2) {               //Product Information | Product Price Information
                $scope.ToggleProductParam('block')
                document.getElementById("product_bonus_mst_id_param").style.display = "none";
                document.getElementById("sku_id_param").style.display = "block";
            } else if (reportId == 21 || reportId == 69 || reportId == 72 || reportId == 26) {
                document.getElementById("company_unit_param").style.display = "none";
                document.getElementById("sku_id_param").style.display = "block";
                document.getElementById("product_bonus_mst_id_param").style.display = "none";
                document.getElementById("company_customer_name_param").style.display = "none";
                document.getElementById("date_from_param").style.display = "block";
                document.getElementById("date_to_param").style.display = "block";
                $scope.ToggleLocationParam('none');
                $scope.ToggleProductParam('none')
            //    $scope.LoadProductBonuData(reportId);
            } else if (reportId == 29 || reportId == 32) {
                $scope.ToggleLocationParam('block');
                if (reportId == 29) {
                    document.getElementById("company_customer_name_param").style.display = "block";
                } else {
                    document.getElementById("company_customer_name_param").style.display = "none";
                }
                document.getElementById("product_bonus_mst_id_param").style.display = "none";
                document.getElementById("sku_id_param").style.display = "none";
                $scope.ToggleProductParam('none')
            }

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


            $interval(function () {
                $scope.LoadCOMPANY_ID();
            }, 800, 2);

            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });



    }
    $scope.ToggleLocationParam = function (value) {
        document.getElementById("company_divition_param").style.display = value;
        document.getElementById("company_region_param").style.display = value;
        document.getElementById("company_area_param").style.display = value;
        document.getElementById("company_Territory_param").style.display = value;
        document.getElementById("company_market_param").style.display = value;
    }
    $scope.ToggleProductParam = function (value) {
        document.getElementById("brand_param").style.display = value;
        document.getElementById("category_param").style.display = value;
        document.getElementById("primary_product_param").style.display = value;
        document.getElementById("base_product_param").style.display = value;
        document.getElementById("product_season_param").style.display = value;
        document.getElementById("group_param").style.display = value;
        document.getElementById("product_type_param").style.display = value;
    }

    $scope.LoadCOMPANY_ID = function () {
        $('#COMPANY_ID').trigger('change');
    }
    $scope.LoadProductData();
    $scope.DataLoad($scope.model.COMPANY_ID);
    $scope.GetPermissionData();
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
    //$scope.LoadCompanyUnitData();
    $scope.GetBrandData();
    $scope.GetCategoryData();
    $scope.LoadBaseProductData();
    $scope.LoadGroupData();
    $scope.LoadPrimaryProductData();
    $scope.LoadProductSeasonData();
    $scope.LoadProductTypeData();
    //$scope.LoadProductBonuData();
    $scope.GetDivitionToMarketRelation();
    //$scope.CompanyUnitLoad();


    $scope.GetPdfView = function () {
        if ($scope.model.CUSTOMER_NAME == null || $scope.model.CUSTOMER_NAME == '' || $scope.model.CUSTOMER_NAME == undefined) {
            $scope.model.CUSTOMER_ID = '';
        }
        if ($scope.model.REPORT_ID == 1) {
            var color = $scope.model.ReportColor;
            var IsLogoApplicable = $scope.model.IsLogoApplicable;
            var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
            var href = "/SalesAndDistribution/Report/CreatePDF?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&BRAND_ID=" + $scope.model.BRAND_ID + "&CATEGORY_ID=" + $scope.model.CATEGORY_ID + "&PRIMARY_PRODUCT_ID=" + $scope.model.PRIMARY_PRODUCT_ID + "&BASE_PRODUCT_ID=" + $scope.model.BASE_PRODUCT_ID + "&PRODUCT_TYPE_ID=" + $scope.model.PRODUCT_TYPE_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&PRODUCT_SEASON_ID=" + $scope.model.PRODUCT_SEASON_ID + "&GROUP_ID=" + $scope.model.GROUP_ID + "&SKU_ID=" + $scope.model.SKU_ID;
            window.open(href, '_blank');
        }
        else {

            var color = $scope.model.ReportColor;
            var IsLogoApplicable = $scope.model.IsLogoApplicable;
            var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
            var href = "/SalesAndDistribution/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&REPORT_ID=" + $scope.model.REPORT_ID + "&SKU_ID=" + $scope.model.SKU_ID + "&CUSTOMER_ID=" + $scope.model.CUSTOMER_ID + "&CUSTOMER_CODE=" + $scope.model.CUSTOMER_CODE + "&PRODUCT_BONUS_MST_ID=" + $scope.model.PRODUCT_BONUS_MST_ID + "&BRAND_ID=" + $scope.model.BRAND_ID + "&CATEGORY_ID=" + $scope.model.CATEGORY_ID + "&PRIMARY_PRODUCT_ID=" + $scope.model.PRIMARY_PRODUCT_ID + "&BASE_PRODUCT_ID=" + $scope.model.BASE_PRODUCT_ID + "&PRODUCT_TYPE_ID=" + $scope.model.PRODUCT_TYPE_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&PRODUCT_SEASON_ID=" + $scope.model.PRODUCT_SEASON_ID + "&GROUP_ID=" + $scope.model.GROUP_ID + "&DIVISION_ID=" + $scope.model.DIVISION_ID + "&REGION_ID=" + $scope.model.REGION_ID + "&AREA_ID=" + $scope.model.AREA_ID + "&TERRITORY_ID=" + $scope.model.TERRITORY_ID + "&MARKET_ID=" + $scope.model.MARKET_ID + "&CUSTOMER_ID=" + $scope.model.CUSTOMER_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&REPORT_EXTENSION=" + 'Pdf';
            window.open(href, '_blank');
        }

    }
    $scope.GetPreview = function () {

        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/SalesAndDistribution/Report/ReportPreview?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable;
        window.open(href, '_blank');
    }
    $scope.GetExcel = function () {

        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/SalesAndDistribution/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&REPORT_ID=" + $scope.model.REPORT_ID + "&SKU_ID=" + $scope.model.SKU_ID + "&CUSTOMER_ID=" + $scope.model.CUSTOMER_ID + "&CUSTOMER_CODE=" + $scope.model.CUSTOMER_CODE + "&PRODUCT_BONUS_MST_ID=" + $scope.model.PRODUCT_BONUS_MST_ID + "&BRAND_ID=" + $scope.model.BRAND_ID + "&CATEGORY_ID=" + $scope.model.CATEGORY_ID + "&PRIMARY_PRODUCT_ID=" + $scope.model.PRIMARY_PRODUCT_ID + "&BASE_PRODUCT_ID=" + $scope.model.BASE_PRODUCT_ID + "&PRODUCT_TYPE_ID=" + $scope.model.PRODUCT_TYPE_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&PRODUCT_SEASON_ID=" + $scope.model.PRODUCT_SEASON_ID + "&GROUP_ID=" + $scope.model.GROUP_ID + "&DIVISION_ID=" + $scope.model.DIVISION_ID + "&REGION_ID=" + $scope.model.REGION_ID + "&AREA_ID=" + $scope.model.AREA_ID + "&TERRITORY_ID=" + $scope.model.TERRITORY_ID + "&MARKET_ID=" + $scope.model.MARKET_ID + "&CUSTOMER_ID=" + $scope.model.CUSTOMER_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&REPORT_EXTENSION=" + 'Excel';
        window.open(href, '_blank');
    }
    $scope.triggerChange = function (element) {
        let changeEvent = new Event('change');
        element.dispatchEvent(changeEvent);
    }

}]);

