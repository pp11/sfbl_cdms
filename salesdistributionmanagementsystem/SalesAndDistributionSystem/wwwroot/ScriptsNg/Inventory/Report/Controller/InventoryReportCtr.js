ngApp.controller('ngGridCtrl', ['$scope', 'InventoryReportServices', 'permissionProvider', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InventoryReportServices, permissionProvider, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {

    $scope.showLoader = true;

    'use strict'
    $scope.model = {
        COMPANY_ID: 0, PRODUCT_SEASON_ID: '', PRODUCT_TYPE_ID: '', PRIMARY_PRODUCT_ID: '', UNIT_ID: '', BASE_PRODUCT_ID: '', CATEGORY_ID: '', BRAND_ID: '', REPORT_ID: '', RequzitionName: 'Raise', UNIT_NAME: "ALL", QUERY: 'HASVALUES', PRODUCT_STATUS: 'ALL'
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
    $scope.Products = [];
    $scope.ProductType = [];
    $scope.BaseProduct = [];
    $scope.Group = [];
    $scope.PackSize = [];
    $scope.ProducStorage = [];
    $scope.Status = [];
    $scope.CompanyUnit = [];
    $scope.Batches = [];
    $scope.challans = [];
    $scope.items = [];

    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;
        InventoryReportServices.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;
        InventoryReportServices.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseFloat(data.data);
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
        InventoryReportServices.GetUnitId().then(function (data) {
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

    $scope.DataLoad = function (companyId) {

        $scope.showLoader = true;

        InventoryReportServices.LoadData(companyId).then(function (data) {
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
        InventoryReportServices.LoadBrand(companyId).then(function (data) {
            $scope.Brands = data.data;
            //var _brand = {
            //    BRAND_ID: "",
            //    BRAND_NAME: "All",
            //}
            //$scope.Brands.push(_brand);
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }

    $scope.GetCategoryData = function () {
        $scope.showLoader = true;
        var companyId = 0;
        InventoryReportServices.LoadCategory(companyId).then(function (data) {
            $scope.Category = data.data;
            //var _category = {
            //    CATEGORY_ID: "",
            //    CATEGORY_NAME: "All",
            //}
            //$scope.Category.push(_category);
            $scope.showLoader = false;
        }, function (error) {
            alert(error);

            $scope.showLoader = false;

        });
    }

    $scope.LoadCompanyUnitData = function () {
        $scope.showLoader = true;
        InventoryReportServices.LoadCompanyUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.CompanyUnit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });
            $scope.CompanyUnit.unshift({ UNIT_ID: 'ALL', UNIT_NAME: 'All' })
            $scope.model.UNIT_ID = 'ALL';
            $scope.CompanyUnitLoad();
            $scope.showLoader = false;
        }, function (error) {

            $scope.showLoader = false;

        });
    }

    $scope.LoadGroupData = function () {
        $scope.showLoader = true;

        InventoryReportServices.LoadGroupData($scope.model.COMPANY_ID).then(function (data) {
            $scope.Group = data.data;
            //var _Group = {
            //    GROUP_ID: "",
            //    GROUP_NAME: "All",
            //}
            //$scope.Group.push(_Group);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }

    $scope.LoadPrimaryProductData = function () {
        $scope.showLoader = true;

        InventoryReportServices.LoadPrimaryProductData($scope.model.COMPANY_ID).then(function (data) {
            $scope.PrimaryProduct = data.data;
            //var _PrimaryProduct = {
            //    PRIMARY_PRODUCT_ID: "",
            //    PRIMARY_PRODUCT_NAME: "All",
            //}
            //$scope.PrimaryProduct.push(_PrimaryProduct);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.LoadProductData = function () {
        $scope.showLoader = true;
        InventoryReportServices.LoadProductData().then(function (data) {
            $scope.Products = data.data;

            $scope.showLoader = false;
            var _Products = {
                SKU_ID: "ALL",
                SKU_NAME: "---Select SKU---",
            }

            $scope.Products.push(_Products);
            $scope.Products.reverse();
            $scope.model.SKU_ID = 'ALL';
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }

    $scope.LoadBaseProductData = function () {
        $scope.showLoader = true;

        InventoryReportServices.LoadBaseProductData($scope.model.COMPANY_ID).then(function (data) {
            $scope.BaseProduct = data.data;
            //var _BaseProduct = {
            //    BASE_PRODUCT_ID: "",
            //    BASE_PRODUCT_NAME: "All",
            //}
            //$scope.BaseProduct.push(_BaseProduct);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }

    $scope.LoadProductSeasonData = function () {
        $scope.showLoader = true;

        InventoryReportServices.LoadProductSeasonData($scope.model.COMPANY_ID).then(function (data) {
            $scope.ProductSeason = data.data;
            //var _ProductSeason = {
            //    PRODUCT_SEASON_ID: "",
            //    PRODUCT_SEASON_NAME: "All",
            //}
            //$scope.ProductSeason.push(_ProductSeason);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }
    $scope.LoadProductTypeData = function () {
        $scope.showLoader = true;

        InventoryReportServices.LoadProductTypeData($scope.model.COMPANY_ID).then(function (data) {
            $scope.ProductType = data.data;
            //var _ProductType = {
            //    PRODUCT_TYPE_ID: "",
            //    PRODUCT_TYPE_NAME: "All",
            //}
            //$scope.ProductType.push(_ProductType);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }



    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'Report',
            Action_Name: 'InventoryReport'
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

    $scope.LoadCOMPANY_ID = function () {
        $('#COMPANY_ID').trigger('change');
    }

    $scope.LoadReportParamters = function (reportId, id_serial, report_name) {
        $scope.model.REPORT_ID = reportId;
        $scope.showLoader = true;
        InventoryReportServices.IsReportPermitted(reportId).then(function (data) {
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
            if (reportId == 3 || reportId == 5) {
                $scope.HideFields();
                if (reportId == 5) {
                    document.getElementById("date_to_param_label").innerHTML = "As on Date";
                    document.getElementById("date_to_param").style.display = "block";
                }
                document.getElementById("sku_id_param").style.display = "block";
                document.getElementById("query_param").style.display = "block";
                document.getElementById("brand_param").style.display = "block";
                document.getElementById("category_param").style.display = "block";
                document.getElementById("base_product_param").style.display = "block";
                document.getElementById("primary_product_param").style.display = "block";
                document.getElementById("group_param").style.display = "block";
                document.getElementById("product_season_param").style.display = "block";
                document.getElementById("product_type_param").style.display = "block";
            }
            else if (reportId == 14) { //Stock Consumption Report
                $scope.HideFields();
                $scope.ShowDates();
                document.getElementById("sku_id_param").style.display = "block";
                document.getElementById("product_status_param").style.display = "block";
                document.getElementById("query_param").style.display = "block";
            }
            else if (reportId == 18) {
                $scope.HideFields();
                document.getElementById("sku_id_param").style.display = "block";
                //document.getElementById("brand_param").style.display = "block";
                //document.getElementById("category_param").style.display = "block";
                //document.getElementById("multi_products").style.display = "block";
                //document.getElementById("base_product_batch_param").style.display = "block";
                //document.getElementById("date_to_param_label").innerHTML = "As on Date";
                //document.getElementById("date_to_param").style.display = "block";
                //$scope.GetBatches();
            }
            else if (reportId == 27) { //
                //Gift Register Report
                $scope.HideFields();
                document.getElementById("date_to_param_label").innerHTML = "As on Date";
                document.getElementById("date_to_param").style.display = "block";
                //$scope.ShowDates();
                document.getElementById("item_id_param").style.display = "block";
                $scope.LoadGiftItems();
            }
            else if ( reportId == 75) {
                $scope.HideFields();
                document.getElementById("sku_id_param").style.display = "block";
                $scope.ShowDates();
                //    document.getElementById("multi_products").style.display = "block";
            } else if (reportId == 30)  {
                $scope.HideFields();
                document.getElementById("sku_id_param").style.display = "block";
                //    document.getElementById("multi_products").style.display = "block";
            }
            else if (reportId == 31) {
                $scope.HideFields();
                //document.getElementById("multi_products").style.display = "block";
                document.getElementById("sku_id_param").style.display = "block";
            } else if (reportId == 83 || reportId == 84) {
                $scope.HideFields();
                //document.getElementById("multi_products").style.display = "block";
                document.getElementById("sku_id_param").style.display = "block";
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

    $scope.GetBatches = () => {
        if ($scope.model.MULTI_SKU_ID != "") {
            $scope.model.SKU_ID = $scope.model.MULTI_SKU_ID?.join(", ");
        }
        if ($scope.model.REPORT_ID == 18) {
            if ($scope.model.MULTI_SKU_ID != "") {
                InventoryReportServices.GetBatches($scope.model).then(response => {
                    $scope.Batches = response.data;
                    //    $scope.triggerChange(document.getElementById("multi_products"));
                })
            }
        }
    }

    $scope.HideFields = () => {
        document.getElementById("sku_id_param").style.display = "none";
        document.getElementById("brand_param").style.display = "none";
        document.getElementById("category_param").style.display = "none";
        document.getElementById("base_product_param").style.display = "none";
        document.getElementById("primary_product_param").style.display = "none";
        document.getElementById("group_param").style.display = "none";
        document.getElementById("product_season_param").style.display = "none";
        document.getElementById("product_type_param").style.display = "none";
        document.getElementById("base_product_batch_param").style.display = "none";
        document.getElementById("challan_no_param").style.display = "none";
        document.getElementById("item_id_param").style.display = "none";
        document.getElementById("date_from_param").style.display = "none";
        document.getElementById("date_to_param").style.display = "none";
        document.getElementById("date_to_param_label").innerHTML = "To Date";
        document.getElementById("query_param").style.display = "none";
        document.getElementById("product_status_param").style.display = "none";
    }

    $scope.ShowDates = () => {
        document.getElementById("date_from_param").style.display = "block";
        document.getElementById("date_to_param").style.display = "block";
    }

    $scope.LoadGiftItems = () => {
        InventoryReportServices.GetGiftItems($scope.model).then(response => {
            $scope.items = response.data;
        })
    }

    $scope.LoadProductData();
    $scope.DataLoad($scope.model.COMPANY_ID);
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
    $scope.LoadCompanyUnitData();
    $scope.GetPermissionData();
    $scope.LoadCompanyUnitData();
    $scope.GetBrandData();
    $scope.GetCategoryData();
    $scope.LoadBaseProductData();
    $scope.LoadGroupData();
    $scope.LoadPrimaryProductData();
    $scope.LoadProductSeasonData();
    $scope.LoadProductTypeData();
    //$scope.CompanyUnitLoad();
    $scope.GetPdfView = function () {
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Inventory/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&BRAND_ID=" + $scope.model.BRAND_ID + "&CATEGORY_ID=" + $scope.model.CATEGORY_ID + "&PRIMARY_PRODUCT_ID=" + $scope.model.PRIMARY_PRODUCT_ID + "&BASE_PRODUCT_ID=" + $scope.model.BASE_PRODUCT_ID + "&PRODUCT_TYPE_ID=" + $scope.model.PRODUCT_TYPE_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&PRODUCT_SEASON_ID=" + $scope.model.PRODUCT_SEASON_ID + "&GROUP_ID=" + $scope.model.GROUP_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&MST_ID=" + $scope.model.MST_ID + "&REPORT_ID=" + $scope.model.REPORT_ID + "&SKU_ID=" + $scope.model.SKU_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&BATCH_NO=" + $scope.model.MULTI_BATCH_NO + "&REPORT_EXTENSION=" + 'Pdf' + '&CHALLAN_NO=' + $scope.model.CHALLAN_NO + '&GIFT_ITEM_ID=' + $scope.model.GIFT_ITEM_ID + '&QUERY=' + $scope.model.QUERY + '&PRODUCT_STATUS=' + $scope.model.PRODUCT_STATUS;;
        window.open(href, '_blank');
    }
    $scope.GetPreview = function () {
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Inventory/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&BRAND_ID=" + $scope.model.BRAND_ID + "&CATEGORY_ID=" + $scope.model.CATEGORY_ID + "&PRIMARY_PRODUCT_ID=" + $scope.model.PRIMARY_PRODUCT_ID + "&BASE_PRODUCT_ID=" + $scope.model.BASE_PRODUCT_ID + "&PRODUCT_TYPE_ID=" + $scope.model.PRODUCT_TYPE_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&PRODUCT_SEASON_ID=" + $scope.model.PRODUCT_SEASON_ID + "&GROUP_ID=" + $scope.model.GROUP_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&MST_ID=" + $scope.model.MST_ID + "&REPORT_ID=" + $scope.model.REPORT_ID + "&SKU_ID=" + $scope.model.SKU_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&BATCH_NO=" + $scope.model.MULTI_BATCH_NO + "&REPORT_EXTENSION=" + 'Pdf' + '&CHALLAN_NO=' + $scope.model.CHALLAN_NO + '&GIFT_ITEM_ID=' + $scope.model.GIFT_ITEM_ID + '&QUERY=' + $scope.model.QUERY + '&PRODUCT_STATUS=' + $scope.model.PRODUCT_STATUS;;
        window.open(href, '_blank');
    }
    $scope.GetExcel = function () {
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Inventory/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&BRAND_ID=" + $scope.model.BRAND_ID + "&CATEGORY_ID=" + $scope.model.CATEGORY_ID + "&PRIMARY_PRODUCT_ID=" + $scope.model.PRIMARY_PRODUCT_ID + "&BASE_PRODUCT_ID=" + $scope.model.BASE_PRODUCT_ID + "&PRODUCT_TYPE_ID=" + $scope.model.PRODUCT_TYPE_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&PRODUCT_SEASON_ID=" + $scope.model.PRODUCT_SEASON_ID + "&GROUP_ID=" + $scope.model.GROUP_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&MST_ID=" + $scope.model.MST_ID + "&REPORT_ID=" + $scope.model.REPORT_ID + "&SKU_ID=" + $scope.model.SKU_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&BATCH_NO=" + $scope.model.MULTI_BATCH_NO + "&REPORT_EXTENSION=" + 'Excel' + '&CHALLAN_NO=' + $scope.model.CHALLAN_NO + '&GIFT_ITEM_ID=' + $scope.model.GIFT_ITEM_ID + '&QUERY=' + $scope.model.QUERY + '&PRODUCT_STATUS=' + $scope.model.PRODUCT_STATUS;
        window.open(href, '_blank');
    }
    $scope.triggerChange = function (element) {
        let changeEvent = new Event('change');
        element.dispatchEvent(changeEvent);
    }
    $scope.selectUnitName = () => {
        $scope.model.UNIT_NAME = $scope.CompanyUnit.find(e => e.UNIT_ID == $scope.model.UNIT_ID)?.UNIT_NAME;
    }
}]);

