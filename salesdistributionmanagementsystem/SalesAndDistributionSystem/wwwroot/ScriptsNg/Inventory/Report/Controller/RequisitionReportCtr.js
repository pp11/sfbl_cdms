ngApp.controller('ngGridCtrl', ['$scope', 'InventoryReportServices', 'permissionProvider', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InventoryReportServices, permissionProvider, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {

    $scope.showLoader = true;

    'use strict'
    $scope.model = {
        COMPANY_ID: 0, PRODUCT_SEASON_ID: '', PRODUCT_TYPE_ID: '', PRIMARY_PRODUCT_ID: '', UNIT_ID:'', BASE_PRODUCT_ID: '', CATEGORY_ID: '', BRAND_ID: '', REPORT_ID: '', RequzitionName: 'Raise'
    }
    $scope.model.DATE_FROM = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.model.DATE_TO = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.ReportData = [];
    $scope.CompanyUnit = [];
    $scope.RequisitionList = [];
    $scope.Companies = [];

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


    $scope.LoadCompanyUnitData = function () {
        $scope.showLoader = true;
        InventoryReportServices.LoadCompanyUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.CompanyUnit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });
            $scope.CompanyUnit.unshift({ UNIT_ID: '', UNIT_NAME: 'All' })
            $scope.model.UNIT_ID = '';
            $scope.CompanyUnitLoad();
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }

    $scope.LoadRequisitionDataBetweenDate = function (Type, reportId) {
        $scope.showLoader = true;
        $scope.RequisitionList = [];
        InventoryReportServices.LoadRequisitionDataBetweenDate($scope.model.COMPANY_ID, $scope.model.UNIT_ID.toString(), $scope.model.DATE_FROM, $scope.model.DATE_TO, Type, reportId).then(function (data) {
            $scope.RequisitionList = data.data;
            var _RequisitionList = {
                MST_ID: "",
                REQUISITION_NO: "All",
            }

            $scope.RequisitionList.push(_RequisitionList);
            $scope.RequisitionList.reverse();
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }


    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'Report',
            Action_Name: 'RequisitionReport'
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

    $scope.LoadUNIT_ID = function () {
        $('#UNIT_ID').trigger('change');
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
            $scope.model.RequzitionName = report_name;

            $scope.GetRequisitionNo();

            $interval(function () {
                $scope.LoadCOMPANY_ID();
            }, 800, 2);

            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.GetRequisitionNo = () => {
        var reportId = $scope.model.REPORT_ID;
        var report_name = $scope.model.RequzitionName;
        if (reportId == 6 || reportId == 7 || reportId == 8 || reportId == 9
            || reportId == 10 || reportId == 37 || reportId == 38 || reportId == 24 || reportId == 28 || reportId == 33) {
            document.getElementById("requzition_no_param").style.display = "block";
            $scope.LoadRequisitionDataBetweenDate(report_name, reportId);
        }
        else {
            document.getElementById("requzition_no_param").style.display = "none";
        }
    }

    $scope.SetRequisitionNo = (id) => {
        $scope.model.REQUISITION_NO = RequisitionList.find(e => e.MST_ID = id);
    }

    $scope.DataLoad($scope.model.COMPANY_ID);
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
    $scope.LoadCompanyUnitData();
    $scope.GetPermissionData();
    //$scope.CompanyUnitLoad();

    $scope.GetPdfView = function () {
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Inventory/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&BRAND_ID=" + $scope.model.BRAND_ID + "&CATEGORY_ID=" + $scope.model.CATEGORY_ID + "&PRIMARY_PRODUCT_ID=" + $scope.model.PRIMARY_PRODUCT_ID + "&BASE_PRODUCT_ID=" + $scope.model.BASE_PRODUCT_ID + "&PRODUCT_TYPE_ID=" + $scope.model.PRODUCT_TYPE_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&PRODUCT_SEASON_ID=" + $scope.model.PRODUCT_SEASON_ID + "&GROUP_ID=" + $scope.model.GROUP_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&MST_ID=" + $scope.model.MST_ID + "&REQUISITION_NO=" + $scope.model.REQUISITION_NO + "&REPORT_ID=" + $scope.model.REPORT_ID + "&SKU_ID=" + $scope.model.SKU_ID + "&REPORT_EXTENSION=" + 'Pdf';
        window.open(href, '_blank');
    }
    $scope.GetPreview = function () {
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Inventory/Report/ReportPreview?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO;
        window.open(href, '_blank');
    }

    $scope.GetExcel = function () {
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Inventory/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&BRAND_ID=" + $scope.model.BRAND_ID + "&CATEGORY_ID=" + $scope.model.CATEGORY_ID + "&PRIMARY_PRODUCT_ID=" + $scope.model.PRIMARY_PRODUCT_ID + "&BASE_PRODUCT_ID=" + $scope.model.BASE_PRODUCT_ID + "&PRODUCT_TYPE_ID=" + $scope.model.PRODUCT_TYPE_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&PRODUCT_SEASON_ID=" + $scope.model.PRODUCT_SEASON_ID + "&GROUP_ID=" + $scope.model.GROUP_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&MST_ID=" + $scope.model.MST_ID + "&REPORT_ID=" + $scope.model.REPORT_ID + "&SKU_ID=" + $scope.model.SKU_ID + "&REPORT_EXTENSION=" + 'Excel';
        window.open(href, '_blank');
    }
    $scope.triggerChange = function (element) {
        let changeEvent = new Event('change');
        element.dispatchEvent(changeEvent);
    }

}]);

