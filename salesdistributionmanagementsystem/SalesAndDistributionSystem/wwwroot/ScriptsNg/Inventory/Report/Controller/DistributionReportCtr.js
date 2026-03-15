ngApp.controller('ngGridCtrl', ['$scope', 'DistributionReportServices', 'permissionProvider', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, DistributionReportServices, permissionProvider, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {
    $scope.showLoader = true;
    'use strict'
    $scope.model = {
        COMPANY_ID: 0, PRODUCT_SEASON_ID: '', PRODUCT_TYPE_ID: '', PRIMARY_PRODUCT_ID: '', UNIT_ID: '', BASE_PRODUCT_ID: '', CATEGORY_ID: '', BRAND_ID: '', REPORT_ID: '', RequzitionName: 'Raise', UNIT_NAME: "ALL"
    }
    $scope.dispatch = 'Dispatch';
    $scope.model.DATE_FROM = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.model.DATE_TO = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.CompanyUnit = [];
    $scope.DistributionNumbers = []
    $scope.Customers = [];
    $scope.pendingInvoiceList = [];
    $scope.PendingInvoices = [];

    //function properties
    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;
        DistributionReportServices.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.CompanyLoad = function () {
        $scope.showLoader = true;
        DistributionReportServices.GetCompany().then(function (data) {
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
        DistributionReportServices.GetUnitId().then(function (data) {
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
    $scope.DataLoad = function (companyId) {
        $scope.showLoader = true;
        DistributionReportServices.LoadData(companyId).then(function (data) {
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
        DistributionReportServices.LoadCompanyUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.CompanyUnit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });
            $scope.model.UNIT_ID = 'ALL';
            $scope.CompanyUnitLoad();
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'Report',
            Action_Name: 'DistributionReport'
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

    //trigger
    $scope.LoadUNIT_ID = function () {
        $('#UNIT_ID').trigger('change');
    }
    $scope.LoadCOMPANY_ID = function () {
        $('#COMPANY_ID').trigger('change');
    }

    $scope.LoadReportParamters = function (reportId, id_serial, report_name) {
        $scope.model.REPORT_ID = reportId;
        $scope.showLoader = true;
        DistributionReportServices.IsReportPermitted(reportId).then(function (data) {
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
            $scope.LoadData();
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.HideFields = () => {
        document.getElementById("date_from_param").style.display = "none";
        document.getElementById("date_to_param").style.display = "none";
        document.getElementById("dispatch_no_param").style.display = "none";
        document.getElementById("dispatch_no_param_v2").style.display = "none";
        document.getElementById("customer_id_param").style.display = "none";
        document.getElementById("invoice_no_param").style.display = "none";
        document.getElementById("invoice_status").style.display = "none";
    }

    $scope.ShowDates = () => {
        document.getElementById("date_from_param").style.display = "block";
        document.getElementById("date_to_param").style.display = "block";
    }

    $scope.LoadDistributionNumbers = () => {
        DistributionReportServices.GetDistributionNumbers($scope.model).then((response => {
            $scope.DistributionNumbers = response.data;
        }))
    }

    $scope.LoadData = () => {
        if ($scope.model.REPORT_ID == 39) {
            $scope.dispatch = 'Dispatch';
            $scope.HideFields();
            $scope.LoadDistributionNumbers();
            $scope.ShowDates();
            document.getElementById("dispatch_no_param").style.display = "block";
        }
        if ($scope.model.REPORT_ID == 71) {
            $scope.HideFields();
            $scope.LoadDistributionNumbers();
            $scope.ShowDates();
            document.getElementById("dispatch_no_param_v2").style.display = "block";
        }
        if ($scope.model.REPORT_ID == 46 || $scope.model.REPORT_ID == 91 || $scope.model.REPORT_ID == 90 || $scope.model.REPORT_ID == 89) {
            $scope.dispatch = 'Distribution';
            $scope.HideFields();
            $scope.LoadDistributionDeliveryNumbers();
            $scope.ShowDates();
            document.getElementById("dispatch_no_param").style.display = "block";
        }
        if ($scope.model.REPORT_ID == 50) {
            $scope.HideFields();
            $scope.GetPendingInvoices();
            document.getElementById("customer_id_param").style.display = "block";
            document.getElementById("invoice_no_param").style.display = "block";
        }
        if ($scope.model.REPORT_ID == 51) {
            $scope.HideFields();
            $scope.GetPendingInvoices();
            document.getElementById("customer_id_param").style.display = "block";
            document.getElementById("invoice_no_param").style.display = "block";
        }
        if ($scope.model.REPORT_ID == 74) {
            $scope.HideFields();
            $scope.GetPendingInvoices();
            document.getElementById("customer_id_param").style.display = "block";
            document.getElementById("invoice_status").style.display = "block";
        }
        if ($scope.model.REPORT_ID == 53 || $scope.model.REPORT_ID == 52) {
            $scope.HideFields();
            $scope.ShowDates();
        }
        if ($scope.model.REPORT_ID == 54) {
            $scope.HideFields();
            $scope.GetPendingInvoices();
            document.getElementById("customer_id_param").style.display = "block";
            document.getElementById("invoice_no_param").style.display = "block";
        }
    }

    $scope.LoadDistributionDeliveryNumbers = () => {
        DistributionReportServices.GetDistributionDeliveryNumbers($scope.model).then((response => {
            $scope.DistributionNumbers = response.data;
        }))
    }

    $scope.LoadCustomers = function () {
        if ($scope.Customers.length == 0) {
            DistributionReportServices.LoadCustomerDropdownData($scope.model.COMPANY_ID).then(response => {
                $scope.Customers = response.data;
            })
        }
    }

    $scope.GetPendingInvoices = function () {
        /*if ($scope.PendingInvoices?.length == 0) {*/
        DistributionReportServices.GetUnitWiseInvoices($scope.model.UNIT_ID).then(invData => {
                $scope.PendingInvoices = [...invData.data[0]];
                $scope.pendingInvoiceList = invData.data[0];
            })
      /*  }*/
    }

    $scope.GetPendingInvoicesByCustomer = function () {
        if ($scope.model.CUSTOMER_NAME == null || $scope.model.CUSTOMER_NAME == '' || $scope.model.CUSTOMER_NAME == undefined) {
            $scope.model.CUSTOMER_ID = '';
            $scope.model.CUSTOMER_CODE = '';
            $scope.pendingInvoiceList = $scope.PendingInvoices;
        } else {
            $scope.pendingInvoiceList = $scope.PendingInvoices.filter(e => e.CUSTOMER_ID == $scope.model.CUSTOMER_ID);
        }

    }
    $scope.ClearCustomer = function () {
        $scope.model.CUSTOMER_ID = '';
        $scope.model.CUSTOMER_CODE = '';
        $scope.model.CUSTOMER_NAME = '';
        $scope.pendingInvoiceList = $scope.PendingInvoices;

    }

    $scope.typeaheadSelectedCustomer = function (entity, selectedItem) {
        $scope.GetPendingInvoicesByCustomer();
        $scope.model.CUSTOMER_ID = selectedItem.CUSTOMER_ID;
        $scope.model.CUSTOMER_CODE = selectedItem.CUSTOMER_CODE;
    };

    $scope.AutoCompleteDataLoadForCustomer = function (value) {
        return DistributionReportServices.GetSearchableCustomer($scope.model.COMPANY_ID, value).then(function (data) {
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

    $scope.DataLoad($scope.model.COMPANY_ID);
    $scope.GetPermissionData();
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
    //$scope.LoadCompanyUnitData();
    //$scope.CompanyUnitLoad();
    $scope.LoadCustomers();
    $scope.GetPdfView = function () {
        if ($scope.model.CUSTOMER_NAME == null || $scope.model.CUSTOMER_NAME == '' || $scope.model.CUSTOMER_NAME == undefined) {
            $scope.model.CUSTOMER_ID = '';
            $scope.model.CUSTOMER_CODE = '';
        }
        if ($scope.model.UNIT_ID == "" || $scope.model.UNIT_ID == null || $scope.model.UNIT_ID == "ALL") {
            alert("Select Unit!");
            return;
        }
        if ($scope.model.REPORT_ID == 46 && ($scope.model.DISPATCH_NO == '' || $scope.model.DISPATCH_NO == null || $scope.model.DISPATCH_NO == undefined)) {
            alert('Please Select Distribution No');
            return;
        }
        if ($scope.model.REPORT_ID == 39 && ($scope.model.DISPATCH_NO == '' || $scope.model.DISPATCH_NO == null || $scope.model.DISPATCH_NO == undefined)) {
            alert('Please Select Dispatch  No');
            return;
        }
        if ($scope.model.REPORT_ID == 71 && ($scope.model.MST_ID == '' || $scope.model.MST_ID == null || $scope.model.MST_ID == undefined)) {
            alert('Please Select Dispatch  No');
            return;
        }
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Inventory/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&UNIT_ID=" + $scope.model.UNIT_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&REPORT_ID=" + $scope.model.REPORT_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&REPORT_EXTENSION=" + 'Pdf' + '&DISPATCH_NO=' + $scope.model.DISPATCH_NO + '&CUSTOMER_ID=' + $scope.model.CUSTOMER_ID + '&CUSTOMER_CODE=' + $scope.model.CUSTOMER_CODE + '&INVOICE_NO=' + $scope.model.INVOICE_NO + '&MST_ID=' + $scope.model.MST_ID + '&INVOICE_STATUS=' + $scope.model.INVOICE_STATUS + "&PREVIEW=" + 'NO';
        window.open(href, '_blank');
    }
    $scope.GetPreview = function () {
        if ($scope.model.CUSTOMER_NAME == null || $scope.model.CUSTOMER_NAME == '' || $scope.model.CUSTOMER_NAME == undefined) {
            $scope.model.CUSTOMER_ID = '';
            $scope.model.CUSTOMER_CODE = '';
        }
        if ($scope.model.UNIT_ID == "" || $scope.model.UNIT_ID == null || $scope.model.UNIT_ID == "ALL") {
            alert("Select Unit!");
            return;
        }
        if ($scope.model.REPORT_ID == 46 && ($scope.model.DISPATCH_NO == '' || $scope.model.DISPATCH_NO == null || $scope.model.DISPATCH_NO == undefined)) {
            alert('Please Select Distribution No');
            return;
        }
        if ($scope.model.REPORT_ID == 39 && ($scope.model.DISPATCH_NO == '' || $scope.model.DISPATCH_NO == null || $scope.model.DISPATCH_NO == undefined)) {
            alert('Please Select Dispatch  No');
            return;
        }
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Inventory/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&UNIT_ID=" + $scope.model.UNIT_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&REPORT_ID=" + $scope.model.REPORT_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&REPORT_EXTENSION=" + 'Pdf' + '&DISPATCH_NO=' + $scope.model.DISPATCH_NO + '&CUSTOMER_ID=' + $scope.model.CUSTOMER_ID + '&CUSTOMER_CODE=' + $scope.model.CUSTOMER_CODE + '&INVOICE_NO=' + $scope.model.INVOICE_NO + '&MST_ID=' + $scope.model.MST_ID + '&INVOICE_STATUS=' + $scope.model.INVOICE_STATUS + "&PREVIEW=" + 'YES';
        window.open(href, '_blank');
    }
    $scope.GetExcel = function () {
        if ($scope.model.CUSTOMER_NAME == null || $scope.model.CUSTOMER_NAME == '' || $scope.model.CUSTOMER_NAME == undefined) {
            $scope.model.CUSTOMER_ID = '';
            $scope.model.CUSTOMER_CODE = '';
        }
        if ($scope.model.UNIT_ID == "" || $scope.model.UNIT_ID == null || $scope.model.UNIT_ID == "ALL") {
            alert("Select Unit!");
            return;
        }
        if ($scope.model.REPORT_ID == 46 && ($scope.model.DISPATCH_NO == '' || $scope.model.DISPATCH_NO == null || $scope.model.DISPATCH_NO == undefined)) {
            alert('Please Select Dispatch  No');
            return;
        }
        if ($scope.model.REPORT_ID == 39 && ($scope.model.DISPATCH_NO == '' || $scope.model.DISPATCH_NO == null || $scope.model.DISPATCH_NO == undefined)) {
            alert('Please Select Dispatch  No');
            return;
        }
        if ($scope.model.REPORT_ID == 71 && ($scope.model.MST_ID == '' || $scope.model.MST_ID == null || $scope.model.MST_ID == undefined)) {
            alert('Please Select Dispatch  No');
            return;
        }
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Inventory/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&UNIT_ID=" + $scope.model.UNIT_ID + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&REPORT_ID=" + $scope.model.REPORT_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&REPORT_EXTENSION=" + 'Excel' + '&DISPATCH_NO=' + $scope.model.DISPATCH_NO + '&CUSTOMER_ID=' + $scope.model.CUSTOMER_ID + '&CUSTOMER_CODE=' + $scope.model.CUSTOMER_CODE + '&INVOICE_NO=' + $scope.model.INVOICE_NO + '&MST_ID=' + $scope.model.MST_ID + '&INVOICE_STATUS=' + $scope.model.INVOICE_STATUS + "&PREVIEW=" + 'NO';
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

