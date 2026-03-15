ngApp.controller('ngGridCtrl', ['$scope', 'ProductReportServices', 'permissionProvider', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, ProductReportServices, permissionProvider, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {
    'use strict'
    $scope.model = {
        COMPANY_ID: 0, PRODUCT_SEASON_ID: '', DIVISION_ID: '', REGION_ID: '', AREA_ID: '', TERRITORY_ID: '', MARKET_ID: '', UNIT_ID: '', CUSTOMER_ID: '', CUSTOMER_CODE: 'ALL'
    }
    $scope.dotMatrixPrinterButton = false;
    $scope.model.INVOICE_DATE = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.model.DATE_FROM = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.model.DATE_TO = $filter("date")(Date.now(), 'dd/MM/yyyy');
    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Divisions = [];
    $scope.Regions = [];
    $scope.Areas = [];
    $scope.Territories = [];
    $scope.Markets = [];
    $scope.Unit = [];
    $scope.ReportData = [];
    $scope.CompanyUnit = [];
    $scope.GetDivitionToMarketRelations = [];
    $scope.Invoice = 'Invoice';
    $scope.CompaniesLoad = function () {
        $scope.showLoader = true;
        ProductReportServices.GetCompanyList().then(function (data) {
            $scope.Companies = data.data;
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.CompanyLoad = function () {
        $scope.showLoader = true;
        ProductReportServices.GetCompany().then(function (data) {
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
        ProductReportServices.GetUnitId().then(function (data) {
            let depot = $scope.CompanyUnit.find(e => e.UNIT_ID == parseInt(data.data));
            if ($scope.model.USER_TYPE != 'SuperAdmin') {
                $scope.CompanyUnit = [];
                $scope.CompanyUnit.push(depot);
            }
            $scope.model.UNIT_ID = parseInt(data.data);
            //var obj = $scope.CompanyUnit.find(x => x.UNIT_ID == parseInt(data.data));
            //$scope.model.UNIT_NAME = obj.UNIT_NAME;
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
        return ProductReportServices.GetSearchableCustomer($scope.model.COMPANY_ID, value).then(function (data) {
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

    $scope.AutoCompleteDataLoadForInvoice = function (value) {
        let inv_date = $scope.model.INVOICE_DATE;
        let invoice_unit_id = parseInt($scope.model.UNIT_ID);
        let company_id = $scope.model.COMPANY_ID.toString();
        if (value.length >= 1) {
            if ($scope.model.REPORT_ID != 55) {
                return ProductReportServices.LoadSearchableInvoice(value, inv_date, 'ASC', invoice_unit_id, company_id).then(function (data) {
                    return data.data;
                }, function (error) {
                    alert(error);
                });
            } else {
                return ProductReportServices.LoadSearchableOrder(value, inv_date, 'ASC', invoice_unit_id, company_id).then(function (data) {
                    return data.data;
                }, function (error) {
                    alert(error);
                });
            }
            
        }
    }

    $scope.AutoCompleteInvoiceDesc = function (value) {
        let inv_date = $scope.model.INVOICE_DATE;
        let invoice_unit_id = parseInt($scope.model.UNIT_ID);
        let company_id = $scope.model.COMPANY_ID.toString();
        if (value.length >= 1) {
            if ($scope.model.REPORT_ID != 55) {
                return ProductReportServices.LoadSearchableInvoice(value, inv_date, 'DESC', invoice_unit_id, company_id).then(function (data) {
                    return data.data;
                }, function (error) {
                    alert(error);
                });
            } else {
                return ProductReportServices.LoadSearchableOrder(value, inv_date, 'DESC', invoice_unit_id, company_id).then(function (data) {
                    return data.data;
                }, function (error) {
                    alert(error);
                }); }
            
        }
    }

    $scope.DataLoad = function (companyId) {
        $scope.showLoader = true;
        ProductReportServices.LoadData(companyId).then(function (data) {
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

    $scope.GetDivitionToMarketRelation = function () {
        $scope.showLoader = true;
        ProductReportServices.GetDivitionToMarketRelation($scope.model.DIVISION_ID).then(function (data) {
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

    //Not Needed
    $scope.GetDivisionData = function () {
        $scope.showLoader = true;
        ProductReportServices.GetDivisions($scope.model.COMPANY_ID).then(function (data) {
            $scope.Divisions = data.data;
            var _divition = {
                DIVISION_ID: "",
                DIVISION_NAME: "ALL"
            }
            $scope.Divisions.push(_divition);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.GetRegionData = function () {
        $scope.showLoader = true;
        ProductReportServices.GetRegions($scope.model.COMPANY_ID).then(function (data) {
            $scope.Regions = data.data;
            var _region = {
                REGION_ID: "",
                REGION_NAME: "ALL"
            }
            $scope.Regions.push(_region);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.GetAreaData = function () {
        $scope.showLoader = true;
        ProductReportServices.GetAreas($scope.model.COMPANY_ID).then(function (data) {
            $scope.Areas = data.data;
            var _area = {
                AREA_ID: "",
                AREA_NAME: "ALL"
            }
            $scope.Areas.push(_area);
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.GetTerritoryData = function () {
        $scope.showLoader = true;
        ProductReportServices.GetTerritories($scope.model.COMPANY_ID).then(function (data) {
            $scope.Territories = data.data;
            $scope.Territories.sort((a, b) => {
                let fa = a.TERRITORY_NAME.toLowerCase(),
                    fb = b.TERRITORY_NAME.toLowerCase();

                if (fa < fb) {
                    return -1;
                }
                if (fa > fb) {
                    return 1;
                }
                return 0;
            });
            $scope.Territories.unshift({ TERRITORY_ID: '', TERRITORY_NAME: 'ALL' })
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    $scope.GetMarketData = function () {
        $scope.showLoader = true;
        ProductReportServices.GetMarkets($scope.model.COMPANY_ID).then(function (data) {
            $scope.Markets = data.data;

            $scope.Markets.sort((a, b) => {
                let fa = a.MARKET_NAME.toLowerCase(),
                    fb = b.MARKET_NAME.toLowerCase();

                if (fa < fb) {
                    return -1;
                }
                if (fa > fb) {
                    return 1;
                }
                return 0;
            });
            $scope.Markets.unshift({ MARKET_ID: '', MARKET_NAME: 'ALL' })
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }
    //$scope.LoadPRODUCT_SEASON_ID = function () {
    //    $('#PRODUCT_SEASON_ID').trigger('change');
    //}
    //$scope.LoadPRIMARY_PRODUCT_ID = function () {
    //    $('#PRIMARY_PRODUCT_ID').trigger('change');
    //}
    //$scope.LoadPRODUCT_TYPE_ID = function () {
    //    $('#PRODUCT_TYPE_ID').trigger('change');

    //}
    //$scope.LoadBASE_PRODUCT_ID = function () {
    //    $('#BASE_PRODUCT_ID').trigger('change');

    //}
    //$scope.LoadBRAND_ID = function () {
    //    $('#BRAND_ID').trigger('change');

    //}
    //$scope.LoadCATEGORY_ID = function () {
    //    $('#CATEGORY_ID').trigger('change');
    //}
    //$scope.LoadGROUP_ID = function () {
    //    $('#GROUP_ID').trigger('change');
    //}
    //Not Needed
    $scope.LoadCompanyUnitData = function () {
        $scope.showLoader = true;
        ProductReportServices.LoadCompanyUnitData($scope.model.COMPANY_ID).then(function (data) {
            $scope.CompanyUnit = data.data.filter(function (element) { return element.COMPANY_ID == $scope.model.COMPANY_ID });
            /* $scope.CompanyUnit.unshift({ UNIT_ID: '', UNIT_NAME: '---Select Unit---' })*/
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
            Action_Name: 'InvoiceReport'
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

    $scope.InvoiceDateChange = function () {
        let obj = $scope.CompanyUnit.find(x => x.UNIT_ID == parseInt($scope.model.UNIT_ID));
        $scope.model.UNIT_NAME = obj.UNIT_NAME;

        if ($scope.model.REPORT_ID == 48 || $scope.model.REPORT_ID == 11 || $scope.model.REPORT_ID == 57 || $scope.model.REPORT_ID == 55 || $scope.model.REPORT_ID == 12) {
            let inv_date = $scope.model.INVOICE_DATE;
            let invoice_unit_id = parseInt($scope.model.UNIT_ID);
            let company_id = $scope.model.COMPANY_ID.toString();
            if ($scope.model.REPORT_ID != 55) {
                $scope.Invoice = 'Invoice';
                return ProductReportServices.LoadMaxMinInvoiceInDate(inv_date, invoice_unit_id, company_id).then(function (data) {
                    if (!angular.equals({}, data.data[0].MAX_INVOICE)) {
                        $scope.model.INVOICE_NO_TO = data.data[0].MAX_INVOICE;
                        $scope.model.INVOICE_NO_FROM = data.data[0].MIN_INVOICE;
                    } else {
                        $scope.model.INVOICE_NO_TO = null;
                        $scope.model.INVOICE_NO_FROM = null;
                    }
                }, function (error) {
                    alert(error);
                });
            } else {
                $scope.Invoice = 'Order';
                return ProductReportServices.LoadMaxMinOrderInDate(inv_date, invoice_unit_id, company_id).then(function (data) {
                    if (!angular.equals({}, data.data[0].MAX_INVOICE)) {
                        $scope.model.INVOICE_NO_TO = data.data[0].MAX_INVOICE;
                        $scope.model.INVOICE_NO_FROM = data.data[0].MIN_INVOICE;
                    } else {
                        $scope.model.INVOICE_NO_TO = null;
                        $scope.model.INVOICE_NO_FROM = null;
                    }
                }, function (error) {
                    alert(error);
                });
            }
        }
    }

    $scope.LoadReportParamters = function (reportId, id_serial) {
        $scope.showLoader = true;
        $scope.model.REPORT_ID = reportId;
        let obj = $scope.CompanyUnit.find(x => x.UNIT_ID == parseInt($scope.model.UNIT_ID));
        $scope.model.UNIT_NAME = obj.UNIT_NAME;
        ProductReportServices.IsReportPermitted(reportId).then(function (data) {
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
            if (reportId == 11 || reportId == 57) {
                $scope.dotMatrixPrinterButton = true;

            } else {
                $scope.dotMatrixPrinterButton = false;
            }
            if (reportId == 11 || reportId == 57 || reportId == 55 || reportId == 12 || reportId == 44 || reportId == 45 || reportId == 48 || reportId == 60 || reportId == 94 || reportId == 96 || reportId == 93 || reportId == 92) {
                let inv_date = $scope.model.INVOICE_DATE;
                let invoice_unit_id = parseInt($scope.model.UNIT_ID);
                let company_id = $scope.model.COMPANY_ID.toString();
                if ($scope.model.REPORT_ID != 55) {
                    $scope.Invoice = 'Invoice';
                    ProductReportServices.LoadMaxMinInvoiceInDate(inv_date, invoice_unit_id, company_id).then(function (data) {
                        if (!angular.equals({}, data.data[0].MAX_INVOICE)) {
                            $scope.model.INVOICE_NO_TO = data.data[0].MAX_INVOICE;
                            $scope.model.INVOICE_NO_FROM = data.data[0].MIN_INVOICE;
                        } else {
                            $scope.model.INVOICE_NO_TO = null;
                            $scope.model.INVOICE_NO_FROM = null;
                        }
                    }, function (error) {
                        alert(error);
                    });
                } else {
                    $scope.Invoice = 'Order';
                    ProductReportServices.LoadMaxMinOrderInDate(inv_date, invoice_unit_id, company_id).then(function (data) {
                        if (!angular.equals({}, data.data[0].MAX_INVOICE)) {
                            $scope.model.INVOICE_NO_TO = data.data[0].MAX_INVOICE;
                            $scope.model.INVOICE_NO_FROM = data.data[0].MIN_INVOICE;
                        } else {
                            $scope.model.INVOICE_NO_TO = null;
                            $scope.model.INVOICE_NO_FROM = null;
                        }
                    }, function (error) {
                        alert(error);
                    });
                }

                document.getElementById("invoice_param").style.display = "block";
                document.getElementById("date_from_param").style.display = "none";
                document.getElementById("date_to_param").style.display = "none";
                document.getElementById("company_invoice_no_from_param").style.display = "block";
                document.getElementById("company_divition_param").style.display = "block";
                document.getElementById("company_region_param").style.display = "block";
                document.getElementById("company_area_param").style.display = "block";
                document.getElementById("company_Territory_param").style.display = "block";
                document.getElementById("company_market_param").style.display = "block";
                document.getElementById("company_customer_name_param").style.display = "block";
                if (reportId == 44 || reportId == 45 || reportId == 60 || reportId == 94 || reportId == 96 || reportId == 93 || reportId == 92) {
                    document.getElementById("company_invoice_no_from_param").style.display = "none";
                }
                if (reportId == 96) {
                    document.getElementById("date_from_param").style.display = "block";
                    document.getElementById("date_to_param").style.display = "block";
                    document.getElementById("invoice_param").style.display = "none";

                }
            } else {
                $scope.model.INVOICE_NO_TO = null;
                $scope.model.INVOICE_NO_FROM = null;
                document.getElementById("invoice_param").style.display = "none";
                document.getElementById("date_from_param").style.display = "block";
                document.getElementById("date_to_param").style.display = "block";
                document.getElementById("company_invoice_no_from_param").style.display = "none";
                document.getElementById("company_divition_param").style.display = "none";
                document.getElementById("company_region_param").style.display = "none";
                document.getElementById("company_area_param").style.display = "none";
                document.getElementById("company_Territory_param").style.display = "none";
                document.getElementById("company_market_param").style.display = "none";
                document.getElementById("company_customer_name_param").style.display = "block";

                if (reportId == 13 || reportId == 56 || reportId == 95 || reportId == 105) {
                    document.getElementById("company_divition_param").style.display = "block";
                    document.getElementById("company_region_param").style.display = "block";
                    document.getElementById("company_area_param").style.display = "block";
                    document.getElementById("company_Territory_param").style.display = "block";
                    document.getElementById("company_market_param").style.display = "block";
                }
            }
            if ($scope.model.USER_TYPE == 'Distributor') {
                ProductReportServices.GetDistributorId().then(function (res) {
                    $scope.model.CUSTOMER_ID = res.data.DistributorId;
                    $scope.model.CUSTOMER_CODE = res.data.DistributorId;
                    $scope.model.CUSTOMER_NAME = res.data.DistributorId;
                    document.getElementById("company_customer_name_param").style.display = "none";

                });


            }
            //$interval(function () {
            //    $scope.LoadCOMPANY_ID();
            //}, 800, 2);

            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = false;
        });
    }

    $scope.DataLoad($scope.model.COMPANY_ID);
    $scope.GetPermissionData();
    $scope.CompaniesLoad();
    $scope.CompanyLoad();
    //$scope.GetDivisionData();
    //$scope.GetRegionData();
    //$scope.GetAreaData();
    //$scope.GetTerritoryData();
    //$scope.GetMarketData();
    //$scope.LoadCompanyUnitData();
    $scope.GetDivitionToMarketRelation();
    //$scope.CompanyUnitLoad();

    $scope.GetExcel = function () {
        if ($scope.model.CUSTOMER_NAME == null || $scope.model.CUSTOMER_NAME == '' || $scope.model.CUSTOMER_NAME == undefined) {
            $scope.model.CUSTOMER_ID = '';
            $scope.model.CUSTOMER_CODE = '';
        }
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/SalesAndDistribution/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&REPORT_ID=" + $scope.model.REPORT_ID + "&INVOICE_DATE=" + $scope.model.INVOICE_DATE + "&INVOICE_NO_FROM=" + $scope.model.INVOICE_NO_FROM + "&INVOICE_NO_TO=" + $scope.model.INVOICE_NO_TO + "&DIVISION_ID=" + $scope.model.DIVISION_ID + "&REGION_ID=" + $scope.model.REGION_ID + "&AREA_ID=" + $scope.model.AREA_ID + "&TERRITORY_ID=" + $scope.model.TERRITORY_ID + "&MARKET_ID=" + $scope.model.MARKET_ID + "&CUSTOMER_ID=" + $scope.model.CUSTOMER_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&CUSTOMER_CODE=" + $scope.model.CUSTOMER_CODE + "&REPORT_EXTENSION=" + 'Excel';
        window.open(href, '_blank');
    }
    $scope.GetPdfView = function () {
        if ($scope.model.CUSTOMER_NAME == null || $scope.model.CUSTOMER_NAME == '' || $scope.model.CUSTOMER_NAME == undefined) {
            $scope.model.CUSTOMER_ID = '';
            $scope.model.CUSTOMER_CODE = '';
        }
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/SalesAndDistribution/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&REPORT_ID=" + $scope.model.REPORT_ID + "&INVOICE_DATE=" + $scope.model.INVOICE_DATE + "&INVOICE_NO_FROM=" + $scope.model.INVOICE_NO_FROM + "&INVOICE_NO_TO=" + $scope.model.INVOICE_NO_TO + "&DIVISION_ID=" + $scope.model.DIVISION_ID + "&REGION_ID=" + $scope.model.REGION_ID + "&AREA_ID=" + $scope.model.AREA_ID + "&TERRITORY_ID=" + $scope.model.TERRITORY_ID + "&MARKET_ID=" + $scope.model.MARKET_ID + "&CUSTOMER_ID=" + $scope.model.CUSTOMER_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&CUSTOMER_CODE=" + $scope.model.CUSTOMER_CODE + "&REPORT_EXTENSION=" + 'Pdf' + "&PREVIEW=" + 'NO' + "&DOT_PRINTER=" + 'NO';
        window.open(href, '_blank');
    }
    $scope.GetDotPdfView = function () {
        if ($scope.model.CUSTOMER_NAME == null || $scope.model.CUSTOMER_NAME == '' || $scope.model.CUSTOMER_NAME == undefined) {
            $scope.model.CUSTOMER_ID = '';
            $scope.model.CUSTOMER_CODE = '';
        }
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/SalesAndDistribution/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&REPORT_ID=" + $scope.model.REPORT_ID + "&INVOICE_DATE=" + $scope.model.INVOICE_DATE + "&INVOICE_NO_FROM=" + $scope.model.INVOICE_NO_FROM + "&INVOICE_NO_TO=" + $scope.model.INVOICE_NO_TO + "&DIVISION_ID=" + $scope.model.DIVISION_ID + "&REGION_ID=" + $scope.model.REGION_ID + "&AREA_ID=" + $scope.model.AREA_ID + "&TERRITORY_ID=" + $scope.model.TERRITORY_ID + "&MARKET_ID=" + $scope.model.MARKET_ID + "&CUSTOMER_ID=" + $scope.model.CUSTOMER_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&CUSTOMER_CODE=" + $scope.model.CUSTOMER_CODE + "&REPORT_EXTENSION=" + 'Pdf' + "&PREVIEW=" + 'NO' + "&DOT_PRINTER=" + 'YES';
        window.open(href, '_blank');
    }
    $scope.GetPreview = function () {
        //var color = $scope.model.ReportColor;
        //var IsLogoApplicable = $scope.model.IsLogoApplicable;
        //var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        //var href = "/SalesAndDistribution/Report/ReportPreview?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable;
        //window.open(href, '_blank');
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/SalesAndDistribution/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&REPORT_ID=" + $scope.model.REPORT_ID + "&INVOICE_DATE=" + $scope.model.INVOICE_DATE + "&INVOICE_NO_FROM=" + $scope.model.INVOICE_NO_FROM + "&INVOICE_NO_TO=" + $scope.model.INVOICE_NO_TO + "&DIVISION_ID=" + $scope.model.DIVISION_ID + "&REGION_ID=" + $scope.model.REGION_ID + "&AREA_ID=" + $scope.model.AREA_ID + "&TERRITORY_ID=" + $scope.model.TERRITORY_ID + "&MARKET_ID=" + $scope.model.MARKET_ID + "&CUSTOMER_ID=" + $scope.model.CUSTOMER_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&CUSTOMER_CODE=" + $scope.model.CUSTOMER_CODE + "&REPORT_EXTENSION=" + 'Pdf' + "&PREVIEW=" + 'YES' + "&DOT_PRINTER=" + 'NO';
        window.open(href, '_blank');
    }
    $scope.GetDotPreview = function () {
        //var color = $scope.model.ReportColor;
        //var IsLogoApplicable = $scope.model.IsLogoApplicable;
        //var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        //var href = "/SalesAndDistribution/Report/ReportPreview?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable;
        //window.open(href, '_blank');
        var color = $scope.model.ReportColor;
        var IsLogoApplicable = $scope.model.IsLogoApplicable;
        var IsCompanyApplicable = $scope.model.IsCompanyApplicable;
        var href = "/SalesAndDistribution/Report/GenerateReport?ReportId=" + $scope.model.reportIdEncryptedSelected + "&Color=" + color + "&IsLogoApplicable=" + IsLogoApplicable + "&IsCompanyApplicable=" + IsCompanyApplicable + "&REPORT_ID=" + $scope.model.REPORT_ID + "&INVOICE_DATE=" + $scope.model.INVOICE_DATE + "&INVOICE_NO_FROM=" + $scope.model.INVOICE_NO_FROM + "&INVOICE_NO_TO=" + $scope.model.INVOICE_NO_TO + "&DIVISION_ID=" + $scope.model.DIVISION_ID + "&REGION_ID=" + $scope.model.REGION_ID + "&AREA_ID=" + $scope.model.AREA_ID + "&TERRITORY_ID=" + $scope.model.TERRITORY_ID + "&MARKET_ID=" + $scope.model.MARKET_ID + "&CUSTOMER_ID=" + $scope.model.CUSTOMER_ID + "&UNIT_ID=" + $scope.model.UNIT_ID + "&UNIT_NAME=" + $scope.model.UNIT_NAME + "&DATE_FROM=" + $scope.model.DATE_FROM + "&DATE_TO=" + $scope.model.DATE_TO + "&CUSTOMER_CODE=" + $scope.model.CUSTOMER_CODE + "&REPORT_EXTENSION=" + 'Pdf' + "&PREVIEW=" + 'YES' + "&DOT_PRINTER=" + 'YES';
        window.open(href, '_blank');
    }

    $scope.triggerChange = function (element) {
        let changeEvent = new Event('change');
        element.dispatchEvent(changeEvent);
    }
}]);