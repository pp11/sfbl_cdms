ngApp.controller('ngGridCtrl', ['$scope', 'TargetReportService', 'permissionProvider', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, TargetReportService, permissionProvider, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {
    $scope.showLoader = true;

    'use strict'

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.DistributionNumbers = []
    $scope.Divisions = [];
    $scope.Regions = [];
    $scope.Areas = [];
    $scope.Territories = [];
    $scope.Markets = [];

    var year = new Date().getFullYear();
    var month = new Date().getMonth();
    $scope.years = [-5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5].map(e => e + year)

    $scope.months = [{
        Id: '01',
        Name: 'JANUARY'
    }, {
        Id: '02',
        Name: 'FEBRUARY'
    }, {
        Id: '03',
        Name: 'MARCH'
    }, {
        Id: '04',
        Name: 'APRIL'
    }, {
        Id: '05',
        Name: 'MAY'
    }, {
        Id: '06',
        Name: 'JUNE'
    }, {
        Id: '07',
        Name: 'JULY'
    }, {
        Id: '08',
        Name: 'AUGUST'
    }, {
        Id: '09',
        Name: 'SEPTEMBER'
    }, {
        Id: '10',
        Name: 'OCTOBER'
    }, {
        Id: '11',
        Name: 'NOVEMBER'
    }, {
        Id: '12',
        Name: 'DECEMBER'
    }];

    $scope.model = {
        COMPANY_ID: 0, MONTH_CODE: month.toString().padStart(2, '0'), YEAR: year.toString(), MARKET_ID:''
    }

    $scope.LoadCompanies = function () {
        $scope.showLoader = true;
        TargetReportService.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.LoadCompany = function () {
        $scope.showLoader = true;

        TargetReportService.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseFloat(data.data);
            $interval(function () {
                $scope.LoadCOMPANY_ID();
            }, 800, 2);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.DataLoad = function (companyId) {
        $scope.showLoader = true;

        TargetReportService.LoadData(companyId).then(function (data) {
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

    $interval(function () {
        $scope.LoadYear();
    }, 800, 2);

    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'Report',
            Action_Name: 'TargetReport'
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

    $scope.LoadCOMPANY_ID = function () {
        $('#COMPANY_ID').trigger('change');
    }

    $scope.LoadYear = function () {
        $('#YEAR').trigger('change');
    }

    $scope.LoadReportParamters = function (reportId, id_serial, report_name) {
        $scope.model.REPORT_ID = reportId;
        $scope.showLoader = true;
        TargetReportService.IsReportPermitted(reportId).then(function (data) {
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
            if (reportId == 49) {  //In Market Sales Report
                document.getElementById("company_divition_param").style.display = "none";
                document.getElementById("company_region_param").style.display = "none";
                document.getElementById("company_area_param").style.display = "none";
                document.getElementById("company_Territory_param").style.display = "none";
            } else if (reportId == 70 || reportId == 76) {
                document.getElementById("company_divition_param").style.display = "block";
                document.getElementById("company_region_param").style.display = "block";
                document.getElementById("company_area_param").style.display = "block";
                document.getElementById("company_Territory_param").style.display = "block";
                document.getElementById("company_market_param").style.display = "block";
                document.getElementById("company_customer_name_param").style.display = "block";
            }
            else {
                document.getElementById("company_divition_param").style.display = "block";
                document.getElementById("company_region_param").style.display = "block";
                document.getElementById("company_area_param").style.display = "block";
                document.getElementById("company_Territory_param").style.display = "block";
                document.getElementById("company_market_param").style.display = "block";
                document.getElementById("company_customer_name_param").style.display = "block";
            }
            //$scope.LoadData();

            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.LoadData = () => {
        if ($scope.model.REPORT_ID == 39) {
            //$scope.HideFields();
            $scope.LoadDistributionNumbers();
            document.getElementById("dispatch_no_param").style.display = "block";
        }
        if ($scope.model.REPORT_ID == 46) {
            //$scope.HideFields();
            $scope.LoadDistributionDeliveryNumbers();
            document.getElementById("dispatch_no_param").style.display = "block";
        }
    }

    $scope.DataLoad($scope.model.COMPANY_ID);
    $scope.LoadCompanies();
    $scope.LoadCompany();
    $scope.GetPermissionData();

    $scope.GetDivitionToMarketRelation = function () {
        $scope.showLoader = true;
        TargetReportService.GetDivitionToMarketRelation($scope.model.DIVISION_ID).then(function (data) {
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

    $scope.GetDivitionToMarketRelation();

    $scope.typeaheadSelectedCustomer = function (entity, selectedItem) {
        $scope.model.CUSTOMER_ID = selectedItem.CUSTOMER_ID;
        $scope.model.CUSTOMER_CODE = selectedItem.CUSTOMER_CODE;
    };
    $scope.AutoCompleteDataLoadForCustomer = function (value) {
        return TargetReportService.GetSearchableCustomer($scope.model.COMPANY_ID, value).then(function (data) {
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

    $scope.GetPdfView = function () {
        if ($scope.model.REPORT_ID == 0) {
            alert("Select report!")
            return;
        }
        if ($scope.model.CUSTOMER_NAME == null || $scope.model.CUSTOMER_NAME == '' || $scope.model.CUSTOMER_NAME == undefined) {
            $scope.model.CUSTOMER_ID = '';
            $scope.model.CUSTOMER_CODE = '';
        }
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Target/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&UNIT_ID=" + $scope.model.UNIT_ID + "&YEAR=" + $scope.model.YEAR + "&MONTH_CODE=" + $scope.model.MONTH_CODE + "&REPORT_ID=" + $scope.model.REPORT_ID + "&REPORT_EXTENSION=" + 'Pdf' + "&DIVISION_ID=" + $scope.model.DIVISION_ID + "&CUSTOMER_ID=" + $scope.model.CUSTOMER_ID + "&CUSTOMER_CODE=" + $scope.model.CUSTOMER_CODE + "&REGION_ID=" + $scope.model.REGION_ID + "&AREA_ID=" + $scope.model.AREA_ID + "&TERRITORY_ID=" + $scope.model.TERRITORY_ID + "&MARKET_ID=" + $scope.model.MARKET_ID;
        window.open(href, '_blank');
    }

    $scope.GetPreview = function () {
        if ($scope.model.REPORT_ID == 0) {
            alert("Select report!")
            return;
        }
        if ($scope.model.CUSTOMER_NAME == null || $scope.model.CUSTOMER_NAME == '' || $scope.model.CUSTOMER_NAME == undefined) {
            $scope.model.CUSTOMER_ID = '';
            $scope.model.CUSTOMER_CODE = '';
        }
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Target/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&UNIT_ID=" + $scope.model.UNIT_ID + "&YEAR=" + $scope.model.YEAR + "&MONTH_CODE=" + $scope.model.MONTH_CODE + "&REPORT_ID=" + $scope.model.REPORT_ID + "&REPORT_EXTENSION=" + 'pdf' + "&DIVISION_ID=" + $scope.model.DIVISION_ID + "&CUSTOMER_ID=" + $scope.model.CUSTOMER_ID + "&CUSTOMER_CODE=" + $scope.model.CUSTOMER_CODE + "&REGION_ID=" + $scope.model.REGION_ID + "&AREA_ID=" + $scope.model.AREA_ID + "&TERRITORY_ID=" + $scope.model.TERRITORY_ID + "&MARKET_ID=" + $scope.model.MARKET_ID;
        window.open(href, '_blank');
    }

    $scope.GetExcel = function () {
        if ($scope.model.REPORT_ID == 0) {
            alert("Select report!")
            return;
        }
        if ($scope.model.CUSTOMER_NAME == null || $scope.model.CUSTOMER_NAME == '' || $scope.model.CUSTOMER_NAME == undefined) {
            $scope.model.CUSTOMER_ID = '';
            $scope.model.CUSTOMER_CODE = '';
        }
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/Target/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&UNIT_ID=" + $scope.model.UNIT_ID + "&YEAR=" + $scope.model.YEAR + "&MONTH_CODE=" + $scope.model.MONTH_CODE + "&REPORT_ID=" + $scope.model.REPORT_ID + "&REPORT_EXTENSION=" + 'Excel' + "&DIVISION_ID=" + $scope.model.DIVISION_ID + "&CUSTOMER_ID=" + $scope.model.CUSTOMER_ID + "&CUSTOMER_CODE=" + $scope.model.CUSTOMER_CODE + "&REGION_ID=" + $scope.model.REGION_ID + "&AREA_ID=" + $scope.model.AREA_ID + "&TERRITORY_ID=" + $scope.model.TERRITORY_ID + "&MARKET_ID=" + $scope.model.MARKET_ID;
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