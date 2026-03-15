ngApp.controller('ngGridCtrl', ['$scope', 'ReceiveReportService', 'permissionProvider', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, ReceiveReportService, permissionProvider, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {

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
        ReceiveReportService.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;
        ReceiveReportService.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseFloat(data.data);
            $interval(function () {
                $scope.LoadCOMPANY_ID();
            }, 800, 2);
            $scope.showLoader = false;
        });
    }
    $scope.DataLoad = function (companyId) {
        $scope.showLoader = true;
        ReceiveReportService.LoadData(companyId).then(function (data) {
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
        ReceiveReportService.LoadCompanyUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.CompanyUnit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });
            $scope.CompanyUnit.unshift({ UNIT_ID: 'ALL', UNIT_NAME: 'All' })
            /*$scope.model.UNIT_ID = 'ALL';*/
            $scope.showLoader = false;
            $scope.CompanyUnitLoad();
        }, function (error) {

            $scope.showLoader = false;

        });
    }
    $scope.CompanyUnitLoad = function () {
        $scope.showLoader = true;
        ReceiveReportService.GetUnitId().then(function (data) {
            let depot = $scope.CompanyUnit.find(e => e.UNIT_ID == parseInt(data.data));
            if ($scope.model.USER_TYPE != 'SuperAdmin') {
                $scope.CompanyUnit = [];
                $scope.CompanyUnit.push(depot);
            }
            $scope.model.UNIT_ID = data.data;
            $scope.model.UNIT_NAME = $scope.CompanyUnit.find(e => e.UNIT_ID == $scope.model.UNIT_ID)?.UNIT_NAME;

            $interval(function () {
                $scope.LoadUNIT_ID();
            }, 800, 2);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;

        });
    }
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'Report',
            Action_Name: 'ReceiveReport'
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
        ReceiveReportService.IsReportPermitted(reportId).then(function (data) {
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
            if ($scope.model.REPORT_ID == 17 || $scope.model.REPORT_ID == 19 || $scope.model.REPORT_ID == 82 || $scope.model.REPORT_ID == 81 || $scope.model.REPORT_ID == 87 || $scope.model.REPORT_ID == 104) {
                $scope.HideFields();
                $scope.ShowDates();
                $scope.LoadProductData();
                document.getElementById("multi_products").style.display = "block";
                document.getElementById("base_product_batch_param").style.display = "block";
            }
            $scope.LoadData();
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.LoadData = () => {
        document.getElementById("company_unit_param").style.display = "block";
        if ($scope.model.REPORT_ID == 17 || $scope.model.REPORT_ID == 19) {
            $scope.GetBatches();
        }
        else if ($scope.model.REPORT_ID == 20) {
            $scope.HideFields();
            $scope.ShowDates();
            document.getElementById("transfer_note_no").style.display = "block";
            $scope.LoadTransferNote();
        }
        else if ($scope.model.REPORT_ID == 22) {
            $scope.HideFields();
            $scope.ShowDates();
            document.getElementById("challan_no_param").style.display = "block";
            $scope.LoadChallans("other");
        }
        else if ($scope.model.REPORT_ID == 23) {
            $scope.HideFields();
            $scope.ShowDates();
            document.getElementById("challan_no_param").style.display = "block";
            $scope.LoadChallans("gift");
        }
        else if ($scope.model.REPORT_ID == 25) {
            //Gift Register Report
            $scope.HideFields();
            $scope.ShowDates();
            document.getElementById("item_id_param").style.display = "block";
            $scope.LoadGiftItems();
        }
        else if ($scope.model.REPORT_ID == 78) {
            //Daily Batch Wise Receiving Register
            $scope.HideFields();
            $scope.ShowDates();
        }
        else if ($scope.model.REPORT_ID == 81 || $scope.model.REPORT_ID == 82) {
            //Date Wise Consumption Register and  Date wise receiving register Report
            $scope.HideFields();
            $scope.ShowDates();
            document.getElementById("multi_products").style.display = "block";

        } else if ($scope.model.REPORT_ID == 79 || $scope.model.REPORT_ID == 80) {
            //Date Wise FG Received From Other Register and Date Wise Miscellaneous Register
            $scope.HideFields();
            $scope.ShowDates();
        } else if ($scope.model.REPORT_ID == 87 || $scope.model.REPORT_ID == 104) {
            //Batch Size vs Receiving Quantity
            $scope.HideFields();
            $scope.ShowDates();
            document.getElementById("company_unit_param").style.display = "Block";
            document.getElementById("multi_products").style.display = "none";
            document.getElementById("single_products").style.display = "block";
        }

    }
    $scope.HideFields = () => {
        document.getElementById("date_from_param").style.display = "none";
        document.getElementById("date_to_param").style.display = "none";
        document.getElementById("transfer_note_no").style.display = "none";
        document.getElementById("challan_no_param").style.display = "none";
        document.getElementById("multi_products").style.display = "none";
        document.getElementById("base_product_batch_param").style.display = "none";
        document.getElementById("item_id_param").style.display = "none";
        document.getElementById("single_products").style.display = "none";

    }
    $scope.ShowDates = () => {
        document.getElementById("date_from_param").style.display = "block";
        document.getElementById("date_to_param").style.display = "block";
    }
    $scope.LoadTransferNote = () => {
        //delete $scope.model.REPORT_ID;
        ReceiveReportService.GetTransferNotes($scope.model).then(response => {
            $scope.transferNotes = response.data;
        })
    }
    $scope.LoadTransferNo = () => {
        ReceiveReportService.GetTransferNo($scope.model).then(response => {
            $scope.transferNotes = response.data;
        })
    }
    $scope.LoadTransferRcvNo = () => {
        ReceiveReportService.GetTransferRcvNo($scope.model).then(response => {
            $scope.transferNotes = response.data;
        })
    }
    $scope.LoadChallans = (type) => {
        if (type == 'gift') {
            ReceiveReportService.GetGiftChallans($scope.model).then(response => {
                $scope.challans = response.data;
            })
        }
        else {
            ReceiveReportService.GetChallans($scope.model).then(response => {
                $scope.challans = response.data;
            })
        }
    }
    $scope.LoadGiftItems = () => {
        ReceiveReportService.GetGiftItems($scope.model).then(response => {
            $scope.items = response.data;
        })
    }
    $scope.LoadProductData = function () {
        $scope.showLoader = true;
        if ($scope.Products.length < 1) {

            ReceiveReportService.LoadProductData().then(function (data) {
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
    }
    $scope.GetBatches = () => {
        if ($scope.model.MULTI_SKU_ID != "") {
            $scope.model.BASE_PRODUCT_ID = $scope.model.MULTI_SKU_ID?.join(", ");
        } else {
            $scope.model.BASE_PRODUCT_ID = null;
        }
        if ($scope.model.REPORT_ID == 17 || $scope.model.REPORT_ID == 19) {
            if ($scope.model.MULTI_SKU_ID != ""
                && $scope.model.MULTI_SKU_ID != undefined
                && $scope.model.MULTI_SKU_ID != null) {
                ReceiveReportService.GetBatches($scope.model).then(response => {
                    $scope.Batches = response.data;
                    $scope.triggerChange(document.getElementById("multi_products"));
                })
            }
        }
    }
    $scope.DataLoad($scope.model.COMPANY_ID);
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
    $scope.GetPermissionData();
    $scope.LoadCompanyUnitData();
    $scope.GetPdfView = function () {
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Inventory/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&BRAND_ID=" + $scope.model.BRAND_ID + "&CATEGORY_ID=" + $scope.model.CATEGORY_ID + "&PRIMARY_PRODUCT_ID=" + $scope.model.PRIMARY_PRODUCT_ID + "&BASE_PRODUCT_ID=" + $scope.model.BASE_PRODUCT_ID + "&PRODUCT_TYPE_ID=" + $scope.model.PRODUCT_TYPE_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&PRODUCT_SEASON_ID=" + $scope.model.PRODUCT_SEASON_ID + "&GROUP_ID=" + $scope.model.GROUP_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&MST_ID=" + $scope.model.MST_ID + "&REPORT_ID=" + $scope.model.REPORT_ID + "&SKU_ID=" + $scope.model.SKU_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&BATCH_NO=" + $scope.model.MULTI_BATCH_NO + "&REPORT_EXTENSION=" + 'Pdf' + '&TRANSFER_NOTE_NO=' + $scope.model.TRANSFER_NOTE_NO + '&CHALLAN_NO=' + $scope.model.CHALLAN_NO + '&GIFT_ITEM_ID=' + $scope.model.GIFT_ITEM_ID;
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
        var href = "/Inventory/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&BRAND_ID=" + $scope.model.BRAND_ID + "&CATEGORY_ID=" + $scope.model.CATEGORY_ID + "&PRIMARY_PRODUCT_ID=" + $scope.model.PRIMARY_PRODUCT_ID + "&BASE_PRODUCT_ID=" + $scope.model.BASE_PRODUCT_ID + "&PRODUCT_TYPE_ID=" + $scope.model.PRODUCT_TYPE_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&PRODUCT_SEASON_ID=" + $scope.model.PRODUCT_SEASON_ID + "&GROUP_ID=" + $scope.model.GROUP_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&MST_ID=" + $scope.model.MST_ID + "&REPORT_ID=" + $scope.model.REPORT_ID + "&SKU_ID=" + $scope.model.SKU_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&BATCH_NO=" + $scope.model.MULTI_BATCH_NO + "&REPORT_EXTENSION=" + 'Excel';;
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

