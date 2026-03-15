ngApp.controller('ngGridCtrl', ['$scope', 'SalesAndCollectionReportServices', 'permissionProvider', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, SalesAndCollectionReportServices, permissionProvider, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {
    $scope.showLoader = true;
    'use strict'
    $scope.model = {
        COMPANY_ID: 0, PRODUCT_SEASON_ID: '', PRODUCT_TYPE_ID: '', PRIMARY_PRODUCT_ID: '', UNIT_ID: '', BASE_PRODUCT_ID: '', CATEGORY_ID: '', BRAND_ID: '', REPORT_ID: '', RequzitionName: 'Raise', UNIT_NAME: "ALL", CUSTOMER_ID: 0
    }
    $scope.InvoiceDate = 'From';
    $scope.model.DATE_FROM = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.model.DATE_TO = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.CompanyUnit = [];
    $scope.DistributionNumbers = []
    $scope.InvoiceNumbers = [];
    $scope.Divisions = [];
    $scope.Regions = [];
    $scope.Areas = [];
    $scope.Territories = [];
    $scope.Customers = [];
    $scope.Locations = [];
    $scope.MarketCustomers = [];
    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;
        SalesAndCollectionReportServices.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;
        SalesAndCollectionReportServices.GetCompany().then(function (data) {
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
        SalesAndCollectionReportServices.GetUnitId().then(function (data) {
            let depot = $scope.CompanyUnit.find(e => e.UNIT_ID == parseInt(data.data));
            if ($scope.model.USER_TYPE != 'SuperAdmin') {
                $scope.CompanyUnit = [];
                $scope.CompanyUnit.push(depot);
            }
            $scope.model.UNIT_ID = parseInt(data.data);
            var obj = $scope.CompanyUnit.find(x => x.UNIT_ID == parseInt(data.data));
            $scope.model.UNIT_NAME = obj.UNIT_NAME;
            $interval(function () {
                $scope.LoadUNIT_ID();
            }, 800, 2);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.GetDevisionCustomer = function () {
        SalesAndCollectionReportServices.GetDivitionToMarketRelation().then(response => {
            $scope.Locations = response.data;
            $scope.Divisions = [...new Map($scope.Locations.map(item =>
                [item["DIVISION_CODE"], item])).values()];
        })
    }
    $scope.onChangeDivision = function () {
        $scope.Regions = [...new Map($scope.Locations.filter(e =>
            $scope.model.DIVISION_CODE.some(id => id == e.DIVISION_CODE)).map(item =>
                [item["REGION_CODE"], item])).values()];
    }
    $scope.onChangeRegion = function () {
        $scope.Areas = [...new Map($scope.Locations.filter(e =>
            $scope.model.REGION_CODE.some(id => id == e.REGION_CODE)).map(item =>
                [item["AREA_CODE"], item])).values()];
    }
    $scope.onChangeArea = function () {
        $scope.Territories = [...new Map($scope.Locations.filter(e =>
            $scope.model.AREA_CODE.some(id => id == e.AREA_CODE)).map(item =>
                [item["TERRITORY_CODE"], item])).values()];
    }
    $scope.onChangeTerritory = function () {
        $scope.MarketCustomers = [...new Map($scope.Locations.filter(e =>
            $scope.model.TERRITORY_CODE.some(id => id == e.TERRITORY_CODE)).map(item =>
                [item["CUSTOMER_ID"], item])).values()];
        console.log($scope.MarketCustomers);
    }
    $scope.DataLoad = function (companyId) {
        $scope.showLoader = true;
        SalesAndCollectionReportServices.LoadData(companyId).then(function (data) {
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
    $scope.LoadCompanyUnitData = function () {
        $scope.showLoader = true;
        SalesAndCollectionReportServices.LoadCompanyUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.CompanyUnit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });
            $scope.CompanyUnit.unshift({ UNIT_ID: 'ALL', UNIT_NAME: 'ALL' })
            $scope.CompanyUnitLoad();
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }


    $scope.LoadCustomers = function () {
        $scope.showLoader = true;
        if ($scope.Customers.length == 0) {
            SalesAndCollectionReportServices.LoadCustomerDropdownData($scope.model.COMPANY_ID).then(function (data) {
                $scope.Customers = data.data;
              $scope.Customers.unshift({ CUSTOMER_CODE: '', CUSTOMER_ID: 0, CUSTOMER_NAME: '---ALL---' });
            }, function (error) {
                $scope.showLoader = false;
            });
        }
        $scope.showLoader = false;

    }

    $scope.GetInvoiceNumbers = function () {
        SalesAndCollectionReportServices.GetInvoiceNumbers($scope.model).then(response => {
            $scope.InvoiceNumbers = response.data;
        })
    }
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'Report',
            Action_Name: 'SalesAndCollectionReport'
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
    $scope.LoadUNIT_ID = function () {
        $('#UNIT_ID').trigger('change');
    }
    $scope.LoadCOMPANY_ID = function () {
        $('#COMPANY_ID').trigger('change');
    }
    $scope.LoadReportParamters = function (reportId, id_serial, report_name) {
        $scope.InvoiceDate = "From";
        $scope.model.REPORT_ID = reportId;
        $scope.showLoader = true;
        SalesAndCollectionReportServices.IsReportPermitted(reportId).then(function (data) {
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

            if (reportId == 40) {
                $scope.InvoiceDate = "Invoice";
                $scope.HideFields();
                $scope.ShowUnit();
                document.getElementById("date_from_param").style.display = "block";
            }
            if (reportId == 41) {
                $scope.HideFields();
                $scope.ShowDates();
                $scope.ShowUnit();
                //$scope.LoadCustomers();
                document.getElementById("customer_id_param").style.display = "block";
            }
            if (reportId == 42) {
                $scope.HideFields();
                $scope.ShowDates();
                $scope.ShowUnit();
                $scope.GetInvoiceNumbers();
                document.getElementById("mst_id_param").style.display = "block";
            }
            if (reportId == 43) {
                $scope.HideFields();
                $scope.ShowDates();
                $scope.ShowUnit();
                document.getElementById("mst_id_param").style.display = "none";
            }
            if (reportId == 61) {
                $scope.HideFields();
                $scope.ShowDates();
                $scope.ShowUnit();
                document.getElementById("company_division_param").style.display = "block";
                document.getElementById("company_region_param").style.display = "block";
                document.getElementById("company_area_param").style.display = "block";
                document.getElementById("company_territory_param").style.display = "block";
                document.getElementById("company_customer_param").style.display = "block";
            }
            if (reportId == 59 || reportId == 62 || reportId == 63 || reportId == 64 || reportId == 65) {
                $scope.HideFields();
                $scope.ShowDates();
                $scope.ShowUnit();
                document.getElementById("customer_id_param").style.display = "block";
            } if (reportId == 86) {
                $scope.HideFields();
                $scope.ShowDates();
                document.getElementById("mst_id_param").style.display = "none";
                $scope.ShowUnit();
            }


            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.HideFields = () => {
        document.getElementById("date_from_param").style.display = "none";
        document.getElementById("date_to_param").style.display = "none";
        document.getElementById("customer_id_param").style.display = "none";
        document.getElementById("mst_id_param").style.display = "none";
        document.getElementById("company_unit_param").style.display = "none";

        document.getElementById("company_division_param").style.display = "none";
        document.getElementById("company_region_param").style.display = "none";
        document.getElementById("company_area_param").style.display = "none";
        document.getElementById("company_territory_param").style.display = "none";
        document.getElementById("company_customer_param").style.display = "none";

    }
    $scope.ShowDates = () => {
        document.getElementById("date_from_param").style.display = "block";
        document.getElementById("date_to_param").style.display = "block";
    }
    $scope.ShowUnit = () => {
        document.getElementById("company_unit_param").style.display = "block";
    }
    $scope.parameterChanged = function () {
        if ($scope.model.REPORT_ID == 42) {
            $scope.GetInvoiceNumbers();
        }
    }
    $scope.DataLoad($scope.model.COMPANY_ID);
    $scope.GetPermissionData();
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
    //$scope.LoadCompanyUnitData();
    $scope.GetDevisionCustomer();
    //$scope.CompanyUnitLoad();
    $scope.LoadCustomers();
    $scope.GetPdfView = function () {
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        if ($scope.model.REPORT_ID == 42) {
            if ($scope.model.MST_ID == null || $scope.model.MST_ID == 0 || $scope.model.MST_ID == '') {
                notificationservice.Notification("Select Invoice!", "", '');
                return;
            }
        }
        var href = "/SalesAndDistribution/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&BRAND_ID=" + $scope.model.BRAND_ID + "&CATEGORY_ID=" + $scope.model.CATEGORY_ID + "&PRIMARY_PRODUCT_ID=" + $scope.model.PRIMARY_PRODUCT_ID + "&BASE_PRODUCT_ID=" + $scope.model.BASE_PRODUCT_ID + "&PRODUCT_TYPE_ID=" + $scope.model.PRODUCT_TYPE_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&PRODUCT_SEASON_ID=" + $scope.model.PRODUCT_SEASON_ID + "&GROUP_ID=" + $scope.model.GROUP_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&MST_ID=" + $scope.model.MST_ID + "&REPORT_ID=" + $scope.model.REPORT_ID + "&SKU_ID=" + $scope.model.SKU_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&BATCH_NO=" + $scope.model.MULTI_BATCH_NO + "&REPORT_EXTENSION=" + 'Pdf' + '&CUSTOMER_ID=' + $scope.model.CUSTOMER_ID + '&MST_ID=' + $scope.model.MST_ID;

        window.open(href, '_blank');
    }
    $scope.GetPreview = function () {
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/SalesAndDistribution/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&BRAND_ID=" + $scope.model.BRAND_ID + "&CATEGORY_ID=" + $scope.model.CATEGORY_ID + "&PRIMARY_PRODUCT_ID=" + $scope.model.PRIMARY_PRODUCT_ID + "&BASE_PRODUCT_ID=" + $scope.model.BASE_PRODUCT_ID + "&PRODUCT_TYPE_ID=" + $scope.model.PRODUCT_TYPE_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&PRODUCT_SEASON_ID=" + $scope.model.PRODUCT_SEASON_ID + "&GROUP_ID=" + $scope.model.GROUP_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&MST_ID=" + $scope.model.MST_ID + "&REPORT_ID=" + $scope.model.REPORT_ID + "&SKU_ID=" + $scope.model.SKU_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&BATCH_NO=" + $scope.model.MULTI_BATCH_NO + "&REPORT_EXTENSION=" + 'Pdf' + '&CUSTOMER_ID=' + $scope.model.CUSTOMER_ID + '&MST_ID=' + $scope.model.MST_ID;
        window.open(href, '_blank');
    }
    $scope.GetExcel = function () {
        if ($scope.model.REPORT_ID == 61 || $scope.model.REPORT_ID == 66 || $scope.model.REPORT_ID == 67) {
            var href = "/Inventory/Report/ExportXL?ReportId=" + $scope.model.reportIdEncryptedSelected
                + '&UNIT_ID=' + $scope.model.UNIT_ID
                + '&DATE_FROM=' + $scope.model.DATE_FROM
                + '&DATE_TO=' + $scope.model.DATE_TO
                + '&DIVISION_CODE=' + $scope.model.DIVISION_CODE
                + '&REGION_CODE=' + $scope.model.REGION_CODE
                + '&AREA_CODE=' + $scope.model.AREA_CODE
                + '&TERRITORY_CODE=' + $scope.model.TERRITORY_CODE
                + '&CUSTOMER_ID=' + $scope.model.MARKET_CUSTOMER_ID
                + '&COMPANY_ID=' + $scope.model.COMPANY_ID;

            window.open(href, '_blank');
        }
        else {
            var color = $scope.model.ReportColor;
            var IsLogoApplicable = $scope.model.IsLogoApplicable;
            var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
            var href = "/SalesAndDistribution/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&BRAND_ID=" + $scope.model.BRAND_ID + "&CATEGORY_ID=" + $scope.model.CATEGORY_ID + "&PRIMARY_PRODUCT_ID=" + $scope.model.PRIMARY_PRODUCT_ID + "&BASE_PRODUCT_ID=" + $scope.model.BASE_PRODUCT_ID + "&PRODUCT_TYPE_ID=" + $scope.model.PRODUCT_TYPE_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&PRODUCT_SEASON_ID=" + $scope.model.PRODUCT_SEASON_ID + "&GROUP_ID=" + $scope.model.GROUP_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&MST_ID=" + $scope.model.MST_ID + "&REPORT_ID=" + $scope.model.REPORT_ID + "&SKU_ID=" + $scope.model.SKU_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&BATCH_NO=" + $scope.model.MULTI_BATCH_NO + "&REPORT_EXTENSION=" + 'Excel' + '&CUSTOMER_ID=' + $scope.model.CUSTOMER_ID + '&MST_ID=' + $scope.model.MST_ID;
            window.open(href, '_blank');
        }

    }
    $scope.triggerChange = function (element) {
        let changeEvent = new Event('change');
        element.dispatchEvent(changeEvent);
    }
    $scope.selectUnitName = () => {
        $scope.model.UNIT_NAME = $scope.CompanyUnit.find(e => e.UNIT_ID == $scope.model.UNIT_ID)?.UNIT_NAME;
        $scope.parameterChanged();
    }
}]);

