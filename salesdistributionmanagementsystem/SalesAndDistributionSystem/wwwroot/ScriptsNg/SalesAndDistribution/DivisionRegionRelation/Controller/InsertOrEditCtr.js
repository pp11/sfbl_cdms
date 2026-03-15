ngApp.controller('ngGridCtrl', ['$scope', 'InsertOrEditServices', 'DivisionInfoServices', 'RegionInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InsertOrEditServices, DivisionInfoServices, RegionInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { COMPANY_ID: 0, DIVISION_ID: 0, DIVISION_CODE: '', DIVISION_REGION_MST_STATUS: 'Active', EFFECT_START_DATE: '', EFFECT_END_DATE: '', REMARKS: ''}

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];
    $scope.Divisions = [];
    $scope.DivisionsList = [];
    $scope.Regions = [];
    $scope.RegionsList = [];
    $scope.RegionsListAll = [];
    $scope.relationdata = [];
    $scope.Dtl_Status = [];
    
    $scope.DefalutData = function () {
        return { COMPANY_ID: 0, DIVISION_ID: 0, DIVISION_CODE: '', DIVISION_REGION_MST_STATUS: 'Active', EFFECT_START_DATE: '', EFFECT_END_DATE: '', REMARKS: '' }
    }

    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 1, COMPANY_ID: 0, REGION_ID: 0, REGION_CODE: '', DIVISION_REGION_DTL_STATUS: 'Active', EFFECT_START_DATE: '', EFFECT_END_DATE: '', REMARKS: ''
        }
    }

    $scope.gridOptions = (gridregistrationservice.GridRegistration("Division Info"));
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptions.data = [];
    //Grid Register
    $scope.gridOptions = {
        data: [$scope.GridDefalutData()]
    }
    
    
    //Generate New Row No
    $scope.rowNumberGenerate = function () {
        
        for (let i = 0; i < $scope.gridOptions.data.length; i++) {
            
            $scope.gridOptions.data[i].ROW_NO = i;
            if ($scope.gridOptions.data[i].REGION_CODE == '') {
                $scope.gridOptions.data[i].REGION_ID = 0;
                $scope.gridOptions.data[i].REGION_NAME = '';
                $scope.gridOptions.data[i].REGION_NAME_CODE = '';

            }
           
           
        }

    }
    
    //Add New Row

    $scope.addNewRow = () => {
        
        if ($scope.gridOptions.data.length > 0 && $scope.gridOptions.data[0].REGION_CODE != null && $scope.gridOptions.data[0].REGION_CODE != '' && $scope.gridOptions.data[0].REGION_CODE != 'undefined') {
            var result =  $scope.CheckDateValidation($scope.gridOptions.data[0]);
            if (result == "true") {
                var newRow = {
                    ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, DIVISION_REGION_MST_ID: $scope.gridOptions.data[0].DIVISION_REGION_MST_ID, DIVISION_REGION_DTL_ID: $scope.gridOptions.data[0].DIVISION_REGION_DTL_ID, REGION_ID: $scope.gridOptions.data[0].REGION_ID, REGION_CODE: $scope.gridOptions.data[0].REGION_CODE, DIVISION_REGION_DTL_STATUS: $scope.gridOptions.data[0].DIVISION_REGION_DTL_STATUS, EFFECT_START_DATE: $scope.gridOptions.data[0].EFFECT_START_DATE, EFFECT_END_DATE: $scope.gridOptions.data[0].EFFECT_END_DATE, REMARKS: $scope.gridOptions.data[0].REMARKS
                }
                var newRowSelected = {
                    REGION_CODE: $scope.gridOptions.data[0].REGION_CODE
                }
                $scope.RegionsList.push(newRowSelected);
                $scope.gridOptions.data.push(newRow);
                $scope.gridOptions.data[0] = $scope.GridDefalutData();

            }
            
        } else {
            notificationservice.Notification("Please Enter Valid Region First", "", 'Only Single Row Left!!');

        }
        $scope.rowNumberGenerate();

    };
    $scope.EditItem = (entity) => {
        
        if ($scope.gridOptions.data.length > 0) {
           
            var newRow = {
                ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, DIVISION_REGION_MST_ID: entity.DIVISION_REGION_MST_ID,   DIVISION_REGION_DTL_ID: entity.DIVISION_REGION_DTL_ID,   REGION_ID: entity.REGION_ID, REGION_CODE: entity.REGION_CODE, DIVISION_REGION_DTL_STATUS: entity.DIVISION_REGION_DTL_STATUS, EFFECT_START_DATE: entity.EFFECT_START_DATE, EFFECT_END_DATE: entity.EFFECT_END_DATE, REMARKS: entity.REMARKS
            }
            $scope.gridOptions.data[0] = newRow;


        } else {
            notificationservice.Notification("Please Select Valid data", "", 'Only Single Row Left!!');

        }
        $scope.rowNumberGenerate();
        $scope.removeItem(entity);
    };
    $scope.CheckDateValidation = function (entity) {
        
        if (entity.DIVISION_REGION_DTL_ID > 0) {
            return "true";
        }
        var today = new Date();
        var day = entity.EFFECT_START_DATE.substring(0, 2)
        var month = entity.EFFECT_START_DATE.substring(3, 5)
        var year = entity.EFFECT_START_DATE.substring(6, 10)
        var StartDate = new Date(parseInt(year), parseInt(month), parseInt(day));
        if (entity.EFFECT_START_DATE.substring(2, 3) != "/" || entity.EFFECT_START_DATE.substring(5, 6) != "/") {
            alert("Please Right Format of Start Date. Ex: dd/mm/yyyy (day/month/year)");
            entity.EFFECT_START_DATE = '';
            return "false";
        }
        var todate = today.getDate();
        var tomonth = today.getMonth();

        if (StartDate < today) {
            alert("Start Date Can not be any Previous Days");
            entity.EFFECT_START_DATE = '';

            return "false";
        }
        else if (parseInt(month) == parseInt(tomonth) &&  parseInt(day) < parseInt(todate)) {
            alert("Start Date Can not be any Previous Days");
            entity.EFFECT_START_DATE = '';
        }
        if (entity.EFFECT_END_DATE != null && entity.EFFECT_END_DATE != '') {
            var day1 = entity.EFFECT_END_DATE.substring(0, 2)
            var month1 = entity.EFFECT_END_DATE.substring(3, 5)
            var year1 = entity.EFFECT_END_DATE.substring(6, 10)
            var EndDate = new Date(parseInt(year1), parseInt(month1), parseInt(day1));
            if (entity.EFFECT_END_DATE.substring(2, 3) != "/" || entity.EFFECT_END_DATE.substring(5, 6) != "/") {
                alert("Please Right Format of End Date. Ex: dd/mm/yyyy (day/month/year)");
                entity.EFFECT_END_DATE = '';

                return "false";
            }
            if (EndDate < today) {
                alert("End Date Can not be any Previous Days");
                entity.EFFECT_END_DATE = '';
                return "false";
            }
            if (EndDate < StartDate) {
                alert("End Date Can not be less than Start Day");
                entity.EFFECT_END_DATE = '';
                return "false";
            }
        }



        return "true"
    }
    $scope.CheckENDDateValidation = function (entity) {
        
        if (entity.EFFECT_START_DATE == null || entity.EFFECT_START_DATE == '' ) {
            alert("End date can not be entered before entering Start Date");
            entity.EFFECT_END_DATE = '';
            return "false";
        }
    }
    // Grid one row remove if this mehtod is call
    $scope.removeItem = function (entity) {
        
        if ($scope.gridOptions.data.length > 1) {
            var index = $scope.gridOptions.data.indexOf(entity);
            if ($scope.gridOptions.data.length > 0) {
                $scope.gridOptions.data.splice(index, 1);
            }
            $scope.rowNumberGenerate();
            

        } else {
            notificationservice.Notification("Only Single Row Left!!", "", 'Only Single Row Left!!');
        }

        
    }

    $scope.gridOptions.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false,  width: '50'
        }
        , {
            name: 'DIVISION_REGION_MST_ID', field: 'DIVISION_REGION_MST_ID', visible: false
        }
        , {
            name: 'DIVISION_REGION_DTL_ID', field: 'DIVISION_REGION_DTL_ID', visible: false
        }
        , {
            name: 'REGION_ID', field: 'REGION_ID', visible: false
        }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , {
            name: 'REGION_NAME', field: 'REGION_NAME', displayName: 'Region', enableFiltering: false, width: '35%', cellTemplate:
                '<select class="select2-single form-control" data-select2-id="{{row.entity.REGION_ID_GR}}" id="REGION_CODE" ng-disabled="row.entity.ROW_NO !=0 || row.entity.DIVISION_REGION_DTL_ID>0"'+
                    'name="REGION_CODE" ng-model="row.entity.REGION_CODE" style="width:100%" >' +
                    '<option ng-repeat="item in grid.appScope.Regions" ng-selected="item.REGION_CODE == row.entity.REGION_CODE" value="{{item.REGION_CODE}}">{{ item.REGION_NAME }} | Code: {{ item.REGION_CODE }}</option>' +
                '</select>'
        }
        , {
            name: 'REGION_CODE', field: 'REGION_CODE', displayName: 'Code', enableFiltering: false, width: '15%', cellTemplate:
                '<input type="text"  ng-model="row.entity.REGION_CODE" disabled="true"  class="pl-sm" />'
        }
        , {
            name: 'EFFECT_START_DATE', field: 'EFFECT_START_DATE', displayName: 'Start', enableFiltering: false, width: '15%', cellTemplate:
                '<div class="simple-date2">'
                + '<div class="input-group date">'
                + '<div class="input-group-prepend">'
                + '</div>'
                + '<input  type="text" datepicker class="form-control"  readonly ng-disabled="row.entity.ROW_NO !=0" ng-model="row.entity.EFFECT_START_DATE" placeholder="dd/mm/yyyy" id="EFFECT_START_DATE">'
                + '</div>'
                + '</div>'
        }
        , {
            name: 'EFFECT_END_DATE', field: 'EFFECT_END_DATE', displayName: 'End', enableFiltering: false, width: '15%', cellTemplate:
                '<div class="simple-date2">'
                + '<div class="input-group date">'
                + '<div class="input-group-prepend">'
                + '</div>'
                + '<input  type="text" readonly datepicker ng-change="grid.appScope.CheckENDDateValidation(row.entity)" class="form-control"  ng-disabled="row.entity.ROW_NO !=0" ng-model="row.entity.EFFECT_END_DATE" placeholder="dd/mm/yyyy"  id="EFFECT_END_DATE">'
                + '</div>'
                + '</div>'
        }
        
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: false,visible:false, width: '15%', cellTemplate:
                '<input type="text"  ng-disabled="row.entity.ROW_NO !=0" ng-model="row.entity.REMARKS"  class="pl-sm" />'


        }
        , {
            name: 'DIVISION_REGION_DTL_STATUS', field: 'DIVISION_REGION_DTL_STATUS', displayName: 'Status', enableFiltering: false, width: '15%', cellTemplate:
                '<select class="form-control"  ng-disabled="row.entity.ROW_NO !=0" id="DIVISION_REGION_DTL_STATUS"'
                +'name="DIVISION_REGION_DTL_STATUS" ng-model="row.entity.DIVISION_REGION_DTL_STATUS" style="width:100%">'
                +'<option ng-repeat="item in grid.appScope.Status" value="{{item.STATUS}}" ng-selected="item.STATUS == row.entity.DIVISION_REGION_DTL_STATUS" >{{ item.STATUS }}</option>'
                +'</select>'
        }
        ,{
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.EditItem(row.entity)" type="button" class="btn btn-outline-success mb-1"><img src="/img/edit-icon.png" height="15px" ></button>' +

                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO == 0" ng-click="grid.appScope.addNewRow()" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                '</div>'
        },

    ];

    $scope.CompanyLoad = function () {

        InsertOrEditServices.GetCompany().then(function (data) {
            $scope.model.COMPANY_ID = parseInt(data.data);
           
            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.CompanyNameLoad = function () {

        InsertOrEditServices.GetCompanyName().then(function (data) {
            $scope.model.COMPANY_NAME = data.data;
            

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.AutoCompleteDataLoadForDivision = function (value) {
            

            return DivisionInfoServices.GetSearchableDivision($scope.model.COMPANY_ID, value).then(function (data) {
                
                $scope.DivisionsList = [];
                for (var i = 0; i < data.data.length; i++) {
                    var _division = {
                        DIVISION_CODE: data.data[i].DIVISION_CODE,
                        DIVISION_NAME: data.data[i].DIVISION_NAME,
                        DIVISION_ID: data.data[i].DIVISION_ID,
                        DIVISION_NAME_CODE: data.data[i].DIVISION_NAME + ' (' + data.data[i].DIVISION_CODE + ')',
                    }
                    $scope.DivisionsList.push(_division);

                }

                return $scope.DivisionsList;
            }, function (error) {
                alert(error);
                

                
            });
    }

    $scope.SearchableRegionLoad = function (value) {
            

            return RegionInfoServices.GetSearchableRegion($scope.model.COMPANY_ID, value).then(function (data) {
                
                $scope.RegionsList = [];
                for (var i = 0; i < data.data.length; i++) {
                    var _region = {
                        REGION_CODE: data.data[i].REGION_CODE,
                        REGION_NAME: data.data[i].REGION_NAME,
                        REGION_ID: data.data[i].REGION_ID,
                        REGION_NAME_CODE: data.data[i].REGION_NAME + ' (' + data.data[i].REGION_CODE + ')',
                    }
                    $scope.RegionsList.push(_region);

                }

                return $scope.RegionsList;
            }, function (error) {
                alert(error);
                

                
            });
        
    }

    $scope.typeaheadSelectedDivision = function () {
        
        const searchIndex = $scope.Divisions.findIndex((x) => x.DIVISION_CODE == $scope.model.DIVISION_CODE);

        $scope.model.DIVISION_ID = $scope.Divisions[searchIndex].DIVISION_ID;
        $scope.model.DIVISION_CODE = $scope.Divisions[searchIndex].DIVISION_CODE;
        $scope.model.DIVISION_NAME = $scope.Divisions[searchIndex].DIVISION_NAME;

    };

    //$scope.typeaheadSelectedRegion = function (entity) {
    //    
    //    const searchIndex = $scope.Regions.findIndex((x) => x.REGION_CODE == entity.REGION_CODE);
    //    if (searchIndex != -1) {
    //        entity.REGION_ID = $scope.Regions[searchIndex].REGION_ID;
    //        entity.REGION_CODE = $scope.Regions[searchIndex].REGION_CODE;
    //        entity.REGION_NAME = $scope.Regions[searchIndex].REGION_NAME;
    //        $scope.Regions = $scope.Regions.filter((x) => x.REGION_CODE !== entity.REGION_CODE);

    //    }
    //   };
    $scope.typeaheadSelectedRegion = function (entity, selectedItem) {
        
        entity.REGION_ID = selectedItem.REGION_ID;
        entity.REGION_CODE = selectedItem.REGION_CODE;
        entity.REGION_NAME = selectedItem.REGION_NAME;
        entity.REGION_NAME_CODE = selectedItem.REGION_NAME_CODE;

    };

    $scope.DivisionsLoad = function () {
        
        $scope.showLoader = true;

        InsertOrEditServices.GetExistingDivision($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.Divisions = data.data;

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.RegionsLoad = function () {

        $scope.showLoader = true;

        InsertOrEditServices.GetExistingRegion($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.Regions = data.data;
            $scope.RegionsListAll = data.data;

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
   

   
    $scope.LoadStatus = function () {
       
        var Active = {
            STATUS: 'Active'
        }
        var InActive = {
            STATUS: 'InActive'
        }
        $scope.Status.push(Active);
        $scope.Status.push(InActive);

    }

   
    

    $scope.ClearForm = function () {
        window.location = "/SalesAndDistribution/DivisionRegionRelation/InsertOrEdit";

    }
    $scope.GetCompanyDisableStatus = function (entity) {
        
        if ($scope.model.USER_TYPE == "SuperAdmin") {
            return false;
        }
        else {
            return true;
        }
    }
    
    $scope.GetPermissionData = function () {
        $scope.showLoader = true;
        
        $scope.permissionReqModel = {
            Controller_Name: 'DivisionRegionRelation',
            Action_Name: 'InsertOrEdit'
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
    $scope.formatRepo = function (repo) {
        
        if (repo.loading) {
            return repo.text;
        }
        if (repo.text != "") {
            const textArray = repo.text.split("--");
            let text_title = textArray[0];
            let text_title_2 = textArray[1];
            var $container = $(
                "<div class='select2-result-repository clearfix'>" +
                "<div class='select2-result-repository__meta'>" +
                "<div class='select2-result-repository__title' style='font-size:14px;font-weight:700'></div>" +
                "<div class='select2-result-repository__watchers' style='font-size:12px;font-weight:700'> <span>Code: </span>  </div>" +
                "</div>" +
                "</div>"
            );

            $container.find(".select2-result-repository__title").text(text_title);
            $container.find(".select2-result-repository__watchers").append(text_title_2);


        }

        return $container;
    }

    $scope.formatRepoSelection = function (repo) {
        return repo.text.split("--")[0];
    }

    $(".select2-single-Division").select2({
        placeholder: "Select",
        templateResult: $scope.formatRepo,
        templateSelection: $scope.formatRepoSelection
    });
    $scope.GetPermissionData();
    $scope.CompanyLoad();
    $scope.CompanyNameLoad();

    $scope.DivisionsLoad();
    $scope.LoadStatus();
    $scope.RegionsLoad();
   
    // This Method work is Edit Data Loading
    $scope.GetEditDataById = function (value) {
        
        if (value != undefined && value.length > 0) {
            InsertOrEditServices.GetEditDataById(value).then(function (data) {
                
    

                if (data.data != null && data.data.division_Region_Dtls !=null && data.data.division_Region_Dtls.length>0) {
                    $scope.model = data.data;
                    $scope.CompanyLoad();
                    $scope.CompanyNameLoad();
                    if ($scope.model.DIVISION_REGION_MST_STATUS == null) {
                        $scope.model.DIVISION_REGION_MST_STATUS = 'Active';
                    }
                    if (data.data.division_Region_Dtls != null) {
                        $scope.gridOptions.data = data.data.division_Region_Dtls;

                    }
                    var DivisionData = {
                        DIVISION_CODE: $scope.model.DIVISION_CODE,
                        DIVISION_NAME: $scope.model.DIVISION_NAME,
                        DIVISION_ID: $scope.model.DIVISION_ID,
                    }
                    $scope.Divisions.push(DivisionData);
                    for (var i in $scope.gridOptions.data) {

                        var RegionData = {
                            REGION_CODE: $scope.gridOptions.data[i].REGION_CODE,
                            REGION_NAME: $scope.gridOptions.data[i].REGION_NAME,
                            REGION_ID: $scope.gridOptions.data[i].REGION_ID,
                        }
                        $scope.Regions.push(RegionData);
                    }
                    $scope.addNewRow();

                    
                }
                
                $scope.rowNumberGenerate();
                $scope.showLoader = false;
            }, function (error) {
                alert(error);
                
            });
        }
    }
    $scope.LoadFormData = function () {
        
        $scope.showLoader = true;

        InsertOrEditServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.relationdata = data.data;
            $scope.showLoader = false;


            for (var i in data.data) {
                
                if (data.data[i].DIVISION_NAME == $scope.model.DIVISION_NAME) {
                    
                    $scope.model.DIVISION_REGION_MST_ID_ENCRYPTED = data.data[i].DIVISION_REGION_MST_ID_ENCRYPTED;
                    $scope.GetEditDataById($scope.model.DIVISION_REGION_MST_ID_ENCRYPTED);
                    $scope.addNewRow();
                }
            }
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.SaveData = function (model) {
        
        $scope.showLoader = true;
        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);
        if ($scope.gridOptions.data.length > 1) {
            $scope.gridOptions.data = $scope.gridOptions.data.filter((x) => x.ROW_NO !== 0);
            for (var i in $scope.gridOptions.data) {
                const searchIndex = $scope.Regions.findIndex((x) => x.REGION_CODE == $scope.gridOptions.data[i].REGION_CODE);
                if (searchIndex != -1) {
                    $scope.gridOptions.data[i].REGION_ID = $scope.Regions[searchIndex].REGION_ID;
                    $scope.gridOptions.data[i].REGION_CODE = $scope.Regions[searchIndex].REGION_CODE;
                    $scope.gridOptions.data[i].REGION_NAME = $scope.Regions[searchIndex].REGION_NAME;

                }
            }
            $scope.model.division_Region_Dtls = $scope.gridOptions.data;

            InsertOrEditServices.AddOrUpdate(model).then(function (data) {
                notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
                if (data.data == 1) {
                    $scope.showLoader = false;
                    $scope.LoadFormData();


                }
                else {
                    $scope.showLoader = false;
                    $scope.addNewRow();

                }
            });
        }
        else {
            notificationservice.Notification('Please Insert Region and add to list!', 1, 'Data Save Successfully !!');

        }
    }

}]);

