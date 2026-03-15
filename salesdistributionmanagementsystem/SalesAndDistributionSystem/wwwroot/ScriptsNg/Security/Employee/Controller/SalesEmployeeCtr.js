ngApp.controller('ngGridCtrl', ['$scope', 'EmployeeService', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', 'uiGridConstants', function ($scope, EmployeeService, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q, uiGridConstants) {
    $scope.model = { STATUS: 'Active' }
    $scope.Sbus = [];
    $scope.Companies = [];
    $scope.CompanyUnit = [];
    $scope.employeeActionInfos = [];
    $scope.productGroups = [];
    $scope.EmployeeStatus = [];
    //+++++++++++++++++++++++Dropdown Data Load Fucnction Defination and Call++++++++++++++++++++++++++++//
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        $scope.permissionReqModel = {
            Controller_Name: 'SalesEmployee',
            Action_Name: 'Index'
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
            $scope.model.CONFIRM_PERMISSION = $scope.getPermissions.confirM_PERMISSION;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            $scope.showLoader = false;
        });
    }
    $scope.DataLoad = function () {
        $scope.showLoader = true;
        let allCodeAndListData = EmployeeService.getAllCodeAndDropdownListData();
        let userCompanyAndUnitInfo = EmployeeService.fetchLoggedInCompanyAndUnit();
        let companyAndItsUnit = EmployeeService.getCompanyAndItsUnit();
        $scope.combineResult = $q.all([
            allCodeAndListData,
            userCompanyAndUnitInfo,
            companyAndItsUnit
        ]).then(function (response) {
            //console.log(response[0])

            $scope.employeeActionInfos = response[0].data[0];
            $scope.EmployeeStatus = response[0].data[1];
            $scope.productGroups = response[0].data[2];
            $scope.Sbus = response[0].data[3];

            //Login Data
            $scope.model.COMPANY_ID = parseInt(response[1].data.companyId);
            $scope.model.COMPANY_NAME = response[1].data.companyName;
            $scope.model.UNIT_ID = parseInt(response[1].data.unitId);
            $scope.model.UNIT_NAME = response[1].data.unitName;

            //company list and unit/depot list
            $scope.CompanyUnit = response[2].data[1];
            $scope.Companies = response[2].data[0];

            $scope.gridApi.grid.refresh();
            $scope.showLoader = false;
        }, function (error) {
            $scope.showLoader = true;
            alert(error);
            console.log(error);
        });
    }
    $scope.GetEmployeeList = function () {
        //$scope.showLoader = true;
        EmployeeService.getEmployeeList().then(function (data) {
            $scope.gridOptionsList.data = data.data;
            $scope.showLoader = false;
        }, function (error) {
            alert(error);
            console.log(error);
            $scope.showLoader = false;
        });
    }


    $scope.GetPermissionData();
    $scope.DataLoad();
    $scope.GetEmployeeList();
    //+++++++++++++++++++++++Dropdown Data Load Fucnction Defination and Call++++++++++++++++++++++++++++//
    //++++++++++++++++gridOptions def and funtion ++++++++++++++++++++++//
    $scope.gridOptions = (gridregistrationservice.GridRegistration("Date Wise Product-SBU Relation"));
    $scope.gridOptions = {
        enableSorting: false,
        enableFiltering: false,
        enableGridMenu: false,
        enableColumnMenus: false,
        columnDefs: [
            {
                name: '#',
                field: 'ROW_NO',
                enableFiltering: false,
                cellTemplate: '<div class="ui-grid-cell-contents">{{grid.renderContainers.body.visibleRowCache.indexOf(row) + 1}}</div>',
                width: '5%',
                pinnedLeft: true
            },
            {
                name: 'MARKET_NAME',
                field: 'MARKET_NAME',
                displayName: 'Market (Mtype)',
                enableFiltering: false,
                width: '20%',
                pinnedLeft: true,
                cellTemplate:
                    '<div class="typeaheadcontainer">' +
                    '<input ng-disabled="(row.entity.MST_ID != undefined || row.entity.MST_ID > 0)" type="text" autocomplete="off" style="width:100%;" class="form-control" name="ROLE_NAME"' +
                    ' ng-model="row.entity.MARKET_NAME"' +
                    ' ng-blur="grid.appScope.typeaheadBlurProduct(row.entity)"' +
                    ' uib-typeahead="Product.MARKET_NAME as Product.MARKET_NAME for Product in grid.appScope.AutoCompleteMarketWithType($viewValue)"' +
                    ' typeahead-append-to-body="true"' +
                    ' placeholder="Market name or type min 2 characters"' +
                    ' typeahead-editable="false"' +
                    ' typeahead-on-select="grid.appScope.typeaheadSelectedMarketWithType(row.entity, $item)" />' +
                    '</div>'
            },
            { name: 'MARKET_ID', field: 'MARKET_ID', visible: false },
            { name: 'MARKET_CODE', field: 'MARKET_CODE', visible: false },
            { name: 'MTYPE_ID', field: 'MTYPE_ID', visible: false },
            { name: 'MTYPE_CODE', field: 'MTYPE_CODE', displayName: 'Market Type Code', visible: false },
            {
                name: 'GROUP_ID',
                field: 'GROUP_ID',
                displayName: 'Base Group(Code)',
                enableFiltering: false,
                width: '18%',
                cellTemplate:
                    '<select ng-disabled="(row.entity.MST_ID != undefined || row.entity.MST_ID > 0)" style="width:100%" class="select2-single form-control" id="GROUP_ID" name="GROUP_ID" ng-model="row.entity.GROUP_ID" ng-options="item.GROUP_ID as item.GROUP_NAME for item in grid.appScope.productGroups" ng-change="grid.appScope.onGroupChange(row)"></select>'
            },
            { name: 'GROUP_CODE', field: 'GROUP_CODE', displayName: 'GROUP_CODE', visible: false },
            {
                name: 'EFFECT_START_DATE',
                field: 'EFFECT_START_DATE',
                displayName: 'Start',
                enableFiltering: false,
                width: '12%',
                cellTemplate:
                    '<input ng-click="grid.appScope.triggerDatePicker(rowRenderIndex,row,row.entity.EFFECT_START_DATE,col.uid)"  type="text" datepicker  class="form-control" ng-model="row.entity.EFFECT_START_DATE" placeholder="dd/mm/yyyy" id="EFFECT_START_DATE">'

            //        '<input ng-click="grid.appScope.triggerDatePicker(rowRenderIndex,row,row.entity.EFFECT_START_DATE,col.uid)" ng-disabled="!grid.appScope.isLastRow(row.entity)" type="text" datepicker ng-change="grid.appScope.CheckStartDateValidationMain(row.entity)" class="form-control" ng-model="row.entity.EFFECT_START_DATE" placeholder="dd/mm/yyyy" id="EFFECT_START_DATE">'
            },
            {
                name: 'EFFECT_END_DATE',
                field: 'EFFECT_END_DATE',
                displayName: 'End',
                enableFiltering: false,
                width: '12%',
                cellTemplate:
                    '<input ng-click="grid.appScope.triggerDatePicker(rowRenderIndex,row,row.entity.EFFECT_END_DATE,col.uid)"  type="text" datepicker ng-change="grid.appScope.CheckEndDateValidationMain(row.entity)" class="form-control" ng-model="row.entity.EFFECT_END_DATE" placeholder="dd/mm/yyyy"  id="EFFECT_END_DATE">'

            //        '<input ng-click="grid.appScope.triggerDatePicker(rowRenderIndex,row,row.entity.EFFECT_END_DATE,col.uid)" ng-disabled="!grid.appScope.isLastRow(row.entity) || row.entity.EFFECT_START_DATE == \'\'" type="text" datepicker ng-change="grid.appScope.CheckEndDateValidationMain(row.entity)" class="form-control" ng-model="row.entity.EFFECT_END_DATE" placeholder="dd/mm/yyyy"  id="EFFECT_END_DATE">'
            },

            {
                name: 'ACTION_TYPE_CODE',
                field: 'ACTION_TYPE_CODE',
                displayName: 'Action Type',
                enableFiltering: false,
                width: '17%',
                cellTemplate:
                    '<select ng-disabled="(row.entity.MST_ID != undefined || row.entity.MST_ID > 0)" style="width:100%" class="select2-single form-control" id="ACTION_TYPE_CODE" name="ACTION_TYPE_CODE" ng-model="row.entity.ACTION_TYPE_CODE" ng-options="item.ACTION_TYPE_CODE as item.ACTION_TYPE_NAME for item in grid.appScope.employeeActionInfos"></select>'
            },
            {
                name: 'REFERENCE_KEY',
                field: 'REFERENCE_KEY',
                displayName: 'Reference Key',
                enableFiltering: false,
                visible: true,
                width: '10%',
                cellTemplate:
                    '<input type="text" ng-model="row.entity.REFERENCE_KEY" class="pl-sm" />'
            },
            {
                name: 'Action',
                headerCellTemplate:
                    '<div class="ui-grid-cell-contents" style="text-align: center;">' +
                    '<button ng-click="grid.appScope.addNewRow()" type="button" class="btn btn-outline-danger mb-1"><img src="/img/plus-icon.png" height="15px" ></button>' +
                    '</div>',
                width: '40',
                enableFiltering: false,
                enableColumnMenu: false,
                pinnedRight: true,
                cellTemplate:
                    '<div style="margin: 1px;" ng-show="grid.appScope.isLastRow(row.entity) && (row.entity.MST_ID === undefined || row.entity.MST_ID <= 0)">' +
                    '<button style="margin-bottom: 5px;" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px"></button>' +
                    '</div>'
            }
        ],
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;

        }
    };
    $scope.typeaheadBlurProduct = function (entity) {
        if (!entity.MARKET_NAME && entity.MARKET_ID > 0) {
            entity.MARKET_NAME = entity.NAME_WITH_CODE;
        }
    };
    $scope.onGroupChange = function (row) {
        let selectedGroup = $scope.productGroups.find(function (item) {
            return item.GROUP_ID === row.entity.GROUP_ID;
        });
        if (selectedGroup) {
            row.entity.GROUP_CODE = selectedGroup.GROUP_CODE;
        }
    };
    $scope.isLastRow = function (row) {
        let index = $scope.gridOptions.data.findIndex(item => item === row);
        return index === $scope.gridOptions.data.length - 1;
    };
    $scope.addNewRow = function () {

        if ($scope.gridOptions.data.some(row => !row.ACTION_TYPE_CODE || !row.GROUP_ID || row.GROUP_ID === 0 || !row.EFFECT_START_DATE || !row.EFFECT_END_DATE)) {
            alert('You cannot add a new row as there is an undefined row.');
            return;
        }

        if ($scope.gridOptions.data.length === 0) {
            $scope.gridOptions.data.push({
                EFFECT_START_DATE: $filter("date")(new Date(), 'dd/MM/yyyy'),
                EFFECT_END_DATE: '31/12/9999'
            });
        } else {
            $scope.gridOptions.data.push({
                EFFECT_START_DATE: '',
                EFFECT_END_DATE: ''
            });
        }
    };
    $scope.CheckStartDateValidationMain = function (entity) {
        let parsedStartDate = parseDateFromString(entity.EFFECT_START_DATE);
        let parsedEndDate = parseDateFromString(entity.EFFECT_END_DATE);
        let currentDate = new Date();  // Get the current date with current time
        // Set time components of currentDate to midnight (00:00:00)
        currentDate.setHours(0, 0, 0, 0);
        if (parsedStartDate >= currentDate) {
            if ($scope.gridOptions.data.length > 1) {
                let previousRowParsedStartDate = parseDateFromString($scope.gridOptions.data[$scope.gridOptions.data.length - 2].EFFECT_START_DATE);
                if (parsedStartDate <= previousRowParsedStartDate) {
                    notificationservice.Notification("oh! you cannot enter a start date earlier than or equal to the previous row.", 'Ok', 'Data Save Successfully !!');
                    entity.EFFECT_START_DATE = '';
                } else {
                    // Subtract one day from parsedStartDate
                    parsedStartDate.setDate(parsedStartDate.getDate() - 1);

                    // Assign the previous day's date to the EFFECT_START_DATE of the previous row
                    $scope.gridOptions.data[$scope.gridOptions.data.length - 2].EFFECT_END_DATE = formatDate(parsedStartDate);
                    entity.EFFECT_END_DATE = '31/12/9999'
                }
            }
        } else {
            notificationservice.Notification("oh! you cannot enter a start date earlier than current date.", 'Ok', 'Data Save Successfully !!');
            entity.EFFECT_START_DATE = '';
        }
        $scope.CheckEndDateValidationMain(entity);
    };
    $scope.CheckEndDateValidationMain = function (entity) {
        let parsedStartDate = parseDateFromString(entity.EFFECT_START_DATE);
        let parsedEndDate = parseDateFromString(entity.EFFECT_END_DATE);
        if (parsedStartDate > parsedEndDate) {
            notificationservice.Notification("oh! you cannot enter a end date earlier than start date.", 'Ok', 'Data Save Successfully !!');
            entity.EFFECT_END_DATE = '';

        }
    };
    $scope.removeItem = function (entity) {
        let index = $scope.gridOptions.data.findIndex(item => item === entity);
        if (index !== -1) {
            $scope.gridOptions.data.splice(index, 1);
        }
    };
    $scope.AutoCompleteMarketWithType = function (value) {
        if (value.length > 1) {
            return EmployeeService.GetSearchableMarketWithType($scope.model.COMPANY_ID, value).then(function (data) {
                return data.data;
            }, function (error) {
                alert(error);
            });
        }

    }
    $scope.typeaheadSelectedMarketWithType = function (entity, selectedItem) {
        entity.MARKET_ID = selectedItem.MARKET_ID;
        entity.MARKET_CODE = selectedItem.MARKET_CODE;
        entity.MTYPE_ID = selectedItem.MTYPE_ID;
        entity.MTYPE_CODE = selectedItem.MTYPE_CODE;
        entity.NAME_WITH_CODE = selectedItem.MARKET_NAME;

    };
    //+++++++++++++++end of gridOptions def and funtion ++++++++++++++++++++++//
    //++++++++++++++++gridSbuType def and funtion++++++++++++++++++++++//
    $scope.gridSbuType = (gridregistrationservice.GridRegistration("Date Wise Product-SBU Relation"));
    $scope.gridSbuType = {
        enableSorting: false,
        enableFiltering: false,
        enableGridMenu: false,
        enableColumnMenus: false,
        columnDefs: [
            {
                name: '#',
                field: 'ROW_NO',
                enableFiltering: false,
                cellTemplate: '<div class="ui-grid-cell-contents">{{grid.renderContainers.body.visibleRowCache.indexOf(row) + 1}}</div>',
                width: '10%',
                pinnedLeft: true
            },
            { name: 'SBU_ID', field: 'SBU_ID', visible: false },
            {
                name: 'SBU_CODE',
                field: 'SBU_CODE',
                displayName: 'SBU',
                enableFiltering: false,
                enableSorting: false,
                width: '25%',
                cellTemplate:
                    '<select ng-disabled="(row.entity.MST_ID != undefined || row.entity.MST_ID > 0)" style="width:100%" class="select2-single form-control" id="SBU_CODE" name="SBU_CODE" ng-model="row.entity.SBU_CODE" ng-options="item.SBU_CODE as item.SBU_CODE for item in grid.appScope.Sbus" ng-change="grid.appScope.onSBUCodeChange(row)"></select>'
            },
            {
                name: 'EFFECT_START_DATE',
                field: 'EFFECT_START_DATE',
                displayName: 'Start',
                enableFiltering: false,
                width: '25%',
                cellTemplate:
                    '<input  ng-click="grid.appScope.triggerDatePicker(rowRenderIndex,row,row.entity.EFFECT_START_DATE,col.uid)" ng-disabled="!grid.appScope.isLastSBURow(row.entity)" type="text" datepicker ng-change="grid.appScope.CheckStartDateValidation(row.entity)" class="form-control" ng-model="row.entity.EFFECT_START_DATE" placeholder="dd/mm/yyyy" id="EFFECT_START_DATE">'
            },
            {
                name: 'EFFECT_END_DATE',
                field: 'EFFECT_END_DATE',
                displayName: 'End',
                enableFiltering: false,
                width: '25%',
                cellTemplate:
                    '<input ng-click="grid.appScope.triggerDatePicker(rowRenderIndex,row,row.entity.EFFECT_END_DATE,col.uid)" ng-disabled="!grid.appScope.isLastSBURow(row.entity) || row.entity.EFFECT_START_DATE == \'\'" type="text" datepicker ng-change="grid.appScope.CheckEndDateValidation(row.entity)" class="form-control"  ng-model="row.entity.EFFECT_END_DATE" placeholder="dd/mm/yyyy"  id="EFFECT_END_DATE">'
            },
            {
                name: 'Action',
                headerCellTemplate:
                    '<div class="ui-grid-cell-contents" style="text-align: center;">' +
                    '<button ng-click="grid.appScope.addSBURow()" type="button" class="btn btn-outline-danger mb-1"><img src="/img/plus-icon.png" height="15px" ></button>' +
                    '</div>',
                width: '13%',
                enableFiltering: false,
                enableColumnMenu: false,
                pinnedRight: true,
                cellTemplate:
                    '<div style="margin: 1px;" ng-show="grid.appScope.isLastSBURow(row.entity) && (row.entity.MST_ID === undefined || row.entity.MST_ID <= 0)">' +
                    '<button style="margin-bottom: 5px;" ng-click="grid.appScope.removeSBUItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px"></button>' +
                    '</div>'
            }
        ],
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;

        }
    };
    $scope.addSBURow = function () {

        // Add your validation here
        if ($scope.gridSbuType.data.some(row => row.SBU_ID === undefined || row.SBU_ID == 0 || row.EFFECT_START_DATE == '' || row.EFFECT_END_DATE == '')) {
            alert('You can add a new row as there is an undefined row.');
            return;
        }

        if ($scope.gridSbuType.data.length === 0) {
            $scope.gridSbuType.data.push({
                EFFECT_START_DATE: $filter("date")(new Date(), 'dd/MM/yyyy'),
                EFFECT_END_DATE: '31/12/9999'
            });
        } else {
            $scope.gridSbuType.data.push({
                EFFECT_START_DATE: '',
                EFFECT_END_DATE: ''
            });
        }
    };
    $scope.removeSBUItem = function (entity) {
        let index = $scope.gridSbuType.data.findIndex(item => item === entity);
        if (index !== -1) {
            $scope.gridSbuType.data.splice(index, 1);
        }
    };
    $scope.isLastSBURow = function (row) {
        var index = $scope.gridSbuType.data.findIndex(item => item === row);
        return index === $scope.gridSbuType.data.length - 1;
    };
    $scope.onSBUCodeChange = function (row) {
        let selectedSbu = $scope.Sbus.find(function (item) {
            return item.SBU_CODE === row.entity.SBU_CODE;
        });
        if (selectedSbu) {
            row.entity.SBU_ID = selectedSbu.SBU_ID;
        }
    };
    $scope.CheckStartDateValidation = function (entity) {
        let parsedStartDate = parseDateFromString(entity.EFFECT_START_DATE);
        let parsedEndDate = parseDateFromString(entity.EFFECT_END_DATE);
        let currentDate = new Date();  // Get the current date with current time
        // Set time components of currentDate to midnight (00:00:00)
        currentDate.setHours(0, 0, 0, 0);
        if (parsedStartDate >= currentDate) {
            if ($scope.gridSbuType.data.length > 1) {
                let previousRowParsedStartDate = parseDateFromString($scope.gridSbuType.data[$scope.gridSbuType.data.length - 2].EFFECT_START_DATE);
                if (parsedStartDate <= previousRowParsedStartDate) {
                    notificationservice.Notification("oh! you cannot enter a start date earlier than or equal to the previous row.", 'Ok', 'Data Save Successfully !!');
                    entity.EFFECT_START_DATE = '';
                } else {
                    // Subtract one day from parsedStartDate
                    parsedStartDate.setDate(parsedStartDate.getDate() - 1);

                    // Assign the previous day's date to the EFFECT_START_DATE of the previous row
                    $scope.gridSbuType.data[$scope.gridSbuType.data.length - 2].EFFECT_END_DATE = formatDate(parsedStartDate);
                    entity.EFFECT_END_DATE = '31/12/9999'
                }
            }
        } else {
            notificationservice.Notification("oh! you cannot enter a start date earlier than current date.", 'Ok', 'Data Save Successfully !!');
            entity.EFFECT_START_DATE = '';
        }
        $scope.CheckEndDateValidation(entity);
    };
    $scope.CheckEndDateValidation = function (entity) {
        let parsedStartDate = parseDateFromString(entity.EFFECT_START_DATE);
        let parsedEndDate = parseDateFromString(entity.EFFECT_END_DATE);
        if (parsedStartDate > parsedEndDate) {
            notificationservice.Notification("oh! you cannot enter a end date earlier than start date.", 'Ok', 'Data Save Successfully !!');
            entity.EFFECT_END_DATE = '';
        }
    };
    //++++++++++++++++End gridSbuType def and funtion++++++++++++++++++++++//
    //++++++++++++++++gridOptionsList : Employee List++++++++++++++++++++++//
    $scope.gridOptionsList = (gridregistrationservice.GridRegistration("Employee List"));
    $scope.gridOptionsList = {
        enableSorting: true,
        enableFiltering: true,
        showGridFooter: true,
        enableGridMenu: true,
        enableColumnMenus: false,
        enableHorizontalScrollbar: uiGridConstants.scrollbars.WHEN_NEEDED,
        enableVerticalScrollbar: uiGridConstants.scrollbars.WHEN_NEEDED,
        columnDefs:
            [
                {
                    name: '#',
                    field: 'ROW_NO',
                    enableFiltering: false,
                    cellTemplate: '<div class="ui-grid-cell-contents">{{grid.renderContainers.body.visibleRowCache.indexOf(row) + 1}}</div>',
                    width: '5%'
                },
                {
                    name: 'ID',
                    field: 'ID',
                    visible: false
                },
                {
                    name: 'COMPANY_ID',
                    field: 'COMPANY_ID',
                    visible: false
                },
                {
                    name: 'COMPANY_NAME',
                    field: 'COMPANY_NAME',
                    visible: false
                },
                {
                    name: 'EMPLOYEE_ID',
                    field: 'EMPLOYEE_ID',
                    displayName: 'Employee Id',
                    visible: false,
                    enableFiltering: true,
                    width: '12%'
                },
                {
                    name: 'EMPLOYEE_CODE',
                    field: 'EMPLOYEE_CODE',
                    displayName: 'Emp. Code',
                    enableFiltering: true,
                    width: '8%'
                },
                {
                    name: 'EMPLOYEE_NAME',
                    field: 'EMPLOYEE_NAME',
                    displayName: 'Emp. Name',
                    enableFiltering: true,
                    width: '14%'
                },
                {
                    name: 'CONTACT_NO',
                    field: 'CONTACT_NO',
                    displayName: 'Contact No',
                    enableFiltering: true,
                    width: '11%'
                },
                {
                    name: 'EMAIL',
                    field: 'EMAIL',
                    displayName: 'Email',
                    enableFiltering: true,
                    width: '18%'
                },
                {
                    name: 'STATUS',
                    field: 'STATUS',
                    displayName: 'Status',
                    enableFiltering: true,
                    width: '6%'
                },
                {
                    name: 'UNIT_NAME',
                    field: 'UNIT_NAME',
                    displayName: 'Unit Name',
                    enableFiltering: true,
                    width: '8%'
                },
                {
                    name: 'MARKET_LIST',
                    field: 'MARKET_LIST',
                    displayName: 'Current Market',
                    enableFiltering: true
                },
                {
                    name: 'TOTAL_MARKET',
                    field: 'TOTAL_MARKET',
                    displayName: 'Total Market',
                    enableFiltering: false,
                    width: '5%'
                },
                {
                    name: 'Actions',
                    displayName: 'Actions',
                    enableFiltering: true,
                    enableColumnMenu: false,
                    width: '10%',
                    cellTemplate: '<div style="margin:1px;">' +
                        '<button style="margin-bottom: 5px;" ng-show="grid.appScope.model.EDIT_PERMISSION == \'Active\'" ng-click="grid.appScope.EditData(row.entity)" type="button" class="btn btn-outline-primary mb-1">Update</button>' +
                        '</div>'
                }

            ],
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
        }
    };
    $scope.EditData = function (entity) {
        $scope.showDiv = !$scope.showDiv;
        EmployeeService.getEmployeeDataById(entity.EMPLOYEE_ID, entity.EMPLOYEE_CODE).then(function (data) {
            $scope.model = { ...$scope.model, ...data.data[0][0] };
            $scope.gridOptions.data = data.data[1];
            $scope.gridSbuType.data = data.data[2];
            //$timeout(function () {
            //    $('.select2-single').trigger('change');
            //});
        }, function (error) {
            console.log(error);
        });
    }
    //++++++++++++++++end gridOptionsList : Employee List++++++++++++++++++++++//
    //+++++++++++++++++++++++++++utility+++++++++++++++++++++++++++++++++++
    $scope.SaveData = function (model) {
        model.EMPLOYEE_CODE = model.EMPLOYEE_CODE.trim();
        model.EMPLOYEE_NAME = model.EMPLOYEE_NAME.trim();
        //model.CONTACT_NO = model.CONTACT_NO.trim();
        //model.EMAIL = model.EMAIL.trim();

        $scope.showLoader = true;
        model.EMPLOYEE_MARKET_MAPPINGs = $scope.gridOptions.data;
        model.EMPLOYEE_SBU_MAPPINGs = $scope.gridSbuType.data;
        EmployeeService.AddOrUpdate(model).then(function (data) {
            $scope.showLoader = false;
            notificationservice.Notification(data.data.Status, 'Ok', 'Data Save Successfully !!');
            if (data.data.Status == 'Ok') {
                //console.log(data.data);
                $scope.model = { ...$scope.model, ...data.data.Model };
                $scope.model.EMPLOYEE_ID = parseInt(data.data.Parent, 10);
                $scope.model.EMPLOYEE_CODE = data.data.Key;
                $scope.gridOptions.data = data.data.Model.EMPLOYEE_MARKET_MAPPINGs;
                $scope.gridSbuType.data = data.data.Model.EMPLOYEE_SBU_MAPPINGs;
                $scope.showLoader = false;
                $scope.GetEmployeeList();
            }
            else {
                console.log('model', model);
                $scope.showLoader = false;
            }
        }, function (error) {
            console.log('model', model);
            console.log('error', error);
            $scope.showLoader = false;
        });
    }
    $scope.triggerDatePicker = function (rowRenderIndex, row, rowEntityCell, coluid) {
        //console.log(rowRenderIndex, row, col, coluid)

        // Find the element with the ID this formate"1703633092941-2-uiGrid-000M-cell"
        let containerElement = document.getElementById(row.grid.id + '-' + rowRenderIndex + '-' + coluid + '-cell');
        // Find the first input field within the container element
        let inputField = containerElement.querySelector("input[type='text']");

        // Initialize the datepicker if it's not initialized yet
        $(inputField).datepicker({
            format: 'dd/mm/yyyy',
        });

        // Update the datepicker with the new date
        $(inputField).datepicker("update", rowEntityCell);
    };
    function parseDateFromString(dateString) {
        let dateComponents = dateString.split("/");
        return new Date(
            parseInt(dateComponents[2], 10),  // year
            parseInt(dateComponents[1], 10) - 1,  // month (0-based)
            parseInt(dateComponents[0], 10)  // day
        );
    }
    function formatDate(date) {
        // Format date as DD/MM/YYYY
        return (
            ('0' + date.getDate()).slice(-2) + '/' +
            ('0' + (date.getMonth() + 1)).slice(-2) + '/' +
            date.getFullYear()
        );
    }
    $scope.toggleDiv = function () {
        $scope.showDiv = !$scope.showDiv;
        $scope.GetEmployeeList();
    };
    //+++++++++++++++++++++++++++utility+++++++++++++++++++++++++++++++++++
}]);

