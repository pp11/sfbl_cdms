ngApp.controller('ngGridCtrl', ['$scope', 'StockTransferServices', 'permissionProvider', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, StockTransferServices, permissionProvider, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {

    $scope.showLoader = true;

    'use strict'
    $scope.model = {
        COMPANY_ID: 0, PRODUCT_SEASON_ID: '', PRODUCT_TYPE_ID: '', PRIMARY_PRODUCT_ID: '', UNIT_ID: '', BASE_PRODUCT_ID: '', CATEGORY_ID: '', BRAND_ID: '', REPORT_ID: '', RequzitionName: 'Raise', UNIT_NAME: "ALL"
    }
    $scope.model.DATE_FROM = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.model.DATE_TO = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.CompanyUnit = [];
    $scope.ReportData = [];
    $scope.transferNotes = [];
    $scope.Products = [];
    $scope.Batches = [];
    $scope.challans = [];
    $scope.items = [];


    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;
        StockTransferServices.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        });
    }

    $scope.CompanyLoad = function () {
        $scope.showLoader = true;
        StockTransferServices.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseFloat(data.data);
            $interval(function () {
                $scope.LoadCOMPANY_ID();
            }, 800, 2);
            $scope.showLoader = false;
        });
    }

    $scope.DataLoad = function (companyId) {
        $scope.showLoader = true;
        StockTransferServices.LoadData(companyId).then(function (data) {
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
        StockTransferServices.LoadCompanyUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.CompanyUnit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });
            $scope.CompanyUnit.unshift({ UNIT_ID: 'ALL', UNIT_NAME: 'All' })
            /*$scope.model.UNIT_ID = 'ALL';*/
            $scope.CompanyUnitLoad();
            $scope.showLoader = false;

        }, function (error) {

            $scope.showLoader = false;

        });
    }

    $scope.CompanyUnitLoad = function () {
        $scope.showLoader = true;
        StockTransferServices.GetUnitId().then(function (data) {
            let depot = $scope.CompanyUnit.find(e => e.UNIT_ID == parseInt(data.data));
            if ($scope.model.USER_TYPE != 'SuperAdmin') {
                $scope.CompanyUnit = [];
                $scope.CompanyUnit.push(depot);
            }
            $scope.model.UNIT_ID = data.data;
            $interval(function () {
                $scope.LoadUNIT_ID();
            }, 800, 2);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }

    $scope.LoadReportParamters = function (reportId, id_serial, report_name) {
        $scope.model.REPORT_ID = reportId;
        $scope.showLoader = true;
        StockTransferServices.IsReportPermitted(reportId).then(function (data) {
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
            $scope.model.DATE_FROM = $filter("date")(Date.now(), 'dd/MM/yyyy');
            $scope.model.DATE_TO = $filter("date")(Date.now(), 'dd/MM/yyyy');

            $scope.LoadData();

            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.LoadData = () => {
        if ($scope.model.REPORT_ID == 15) {
            $scope.LoadTransferNo();
        }
        else if ($scope.model.REPORT_ID == 16) {
            $scope.LoadTransferRcvNo();
        }
    }

    $scope.LoadTransferNo = () => {
        StockTransferServices.GetTransferNo($scope.model).then(response => {
            $scope.transferNotes = response.data;
        })
    }

    $scope.LoadTransferRcvNo = () => {
        StockTransferServices.GetTransferRcvNo($scope.model).then(response => {
            $scope.transferNotes = response.data;
        })
    }

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'Report',
            Action_Name: 'StockTransferReport'
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

    $scope.DataLoad($scope.model.COMPANY_ID);
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
    $scope.GetPermissionData();
    $scope.LoadCompanyUnitData();


    $scope.LoadCOMPANY_ID = function () {
        $('#COMPANY_ID').trigger('change');
    }

    $scope.LoadUNIT_ID = function () {
        $('#UNIT_ID').trigger('change');
    }

    $scope.GetPdfView = function () {
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Inventory/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&BRAND_ID=" + $scope.model.BRAND_ID + "&CATEGORY_ID=" + $scope.model.CATEGORY_ID + "&PRIMARY_PRODUCT_ID=" + $scope.model.PRIMARY_PRODUCT_ID + "&BASE_PRODUCT_ID=" + $scope.model.BASE_PRODUCT_ID + "&PRODUCT_TYPE_ID=" + $scope.model.PRODUCT_TYPE_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&PRODUCT_SEASON_ID=" + $scope.model.PRODUCT_SEASON_ID + "&GROUP_ID=" + $scope.model.GROUP_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&MST_ID=" + $scope.model.MST_ID + "&REPORT_ID=" + $scope.model.REPORT_ID + "&SKU_ID=" + $scope.model.SKU_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&BATCH_NO=" + $scope.model.MULTI_BATCH_NO + "&REPORT_EXTENSION=" + 'Pdf' + '&TRANSFER_NOTE_NO=' + $scope.model.TRANSFER_NOTE_NO + '&CHALLAN_NO=' + $scope.model.CHALLAN_NO + '&GIFT_ITEM_ID=' + $scope.model.GIFT_ITEM_ID + "&PREVIEW=" + 'NO';
        window.open(href, '_blank');
    }
    $scope.GetPreview = function () {
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Inventory/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&BRAND_ID=" + $scope.model.BRAND_ID + "&CATEGORY_ID=" + $scope.model.CATEGORY_ID + "&PRIMARY_PRODUCT_ID=" + $scope.model.PRIMARY_PRODUCT_ID + "&BASE_PRODUCT_ID=" + $scope.model.BASE_PRODUCT_ID + "&PRODUCT_TYPE_ID=" + $scope.model.PRODUCT_TYPE_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&PRODUCT_SEASON_ID=" + $scope.model.PRODUCT_SEASON_ID + "&GROUP_ID=" + $scope.model.GROUP_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&MST_ID=" + $scope.model.MST_ID + "&REPORT_ID=" + $scope.model.REPORT_ID + "&SKU_ID=" + $scope.model.SKU_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&BATCH_NO=" + $scope.model.MULTI_BATCH_NO + "&REPORT_EXTENSION=" + 'Pdf' + '&TRANSFER_NOTE_NO=' + $scope.model.TRANSFER_NOTE_NO + '&CHALLAN_NO=' + $scope.model.CHALLAN_NO + '&GIFT_ITEM_ID=' + $scope.model.GIFT_ITEM_ID + "&PREVIEW=" + 'YES';
        window.open(href, '_blank');
    }
    
    $scope.GetExcel = function () {
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Inventory/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&BRAND_ID=" + $scope.model.BRAND_ID + "&CATEGORY_ID=" + $scope.model.CATEGORY_ID + "&PRIMARY_PRODUCT_ID=" + $scope.model.PRIMARY_PRODUCT_ID + "&BASE_PRODUCT_ID=" + $scope.model.BASE_PRODUCT_ID + "&PRODUCT_TYPE_ID=" + $scope.model.PRODUCT_TYPE_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&PRODUCT_SEASON_ID=" + $scope.model.PRODUCT_SEASON_ID + "&GROUP_ID=" + $scope.model.GROUP_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&MST_ID=" + $scope.model.MST_ID + "&REPORT_ID=" + $scope.model.REPORT_ID + "&SKU_ID=" + $scope.model.SKU_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&BATCH_NO=" + $scope.model.MULTI_BATCH_NO + "&REPORT_EXTENSION=" + 'Excel' + '&TRANSFER_NOTE_NO=' + $scope.model.TRANSFER_NOTE_NO + '&CHALLAN_NO=' + $scope.model.CHALLAN_NO + '&GIFT_ITEM_ID=' + $scope.model.GIFT_ITEM_ID + "&PREVIEW=" + 'NO';
        window.open(href, '_blank');
    }
    $scope.triggerChange = function (element) {
        let changeEvent = new Event('change');
        element.dispatchEvent(changeEvent);
    }

    $scope.selectUnitName = () => {
        $scope.model.UNIT_NAME = $scope.CompanyUnit.find(e => e.UNIT_ID == $scope.model.UNIT_ID)?.UNIT_NAME;
        $scope.LoadData();
    }
}]);