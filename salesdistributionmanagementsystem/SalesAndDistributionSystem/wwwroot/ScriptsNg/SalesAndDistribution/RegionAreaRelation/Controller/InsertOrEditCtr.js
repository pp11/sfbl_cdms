ngApp.controller('ngGridCtrl', ['$scope', 'InsertOrEditServices', 'RegionInfoServices', 'AreaInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InsertOrEditServices, RegionInfoServices, AreaInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { COMPANY_ID: 0, REGION_ID: 0, REGION_CODE: '', REGION_AREA_MST_STATUS: 'Active', EFFECT_START_DATE: '', EFFECT_END_DATE: '', REMARKS: ''}

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];
    $scope.AreasList = [];
    $scope.Areas = [];
    $scope.AreasListAll = [];
    $scope.Regions = [];
    $scope.RegionsList = [];
    $scope.RegionsListAll = [];
    $scope.relationdata = [];
    $scope.Dtl_Status = [];

    $scope.DefalutData = function () {
        return { COMPANY_ID: 0, REGION_ID: 0, REGION_CODE: '', REGION_AREA_MST_STATUS: 'Active', EFFECT_START_DATE: '', EFFECT_END_DATE: '', REMARKS: '' }
    }

    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 1, COMPANY_ID: 0, AREA_ID: 0, AREA_CODE: '', REGION_AREA_DTL_STATUS: 'Active', EFFECT_START_DATE: '', EFFECT_END_DATE: '', REMARKS: ''
        }
    }

    $scope.gridOptions = (gridregistrationservice.GridRegistration("Region Area Relation"));
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
            if ($scope.gridOptions.data[i].AREA_CODE == '') {
                $scope.gridOptions.data[i].AREA_ID = 0;
                $scope.gridOptions.data[i].AREA_NAME = '';
                $scope.gridOptions.data[i].AREA_NAME_CODE = '';

            }
           
           
        }

    }
    $scope.CheckDateValidation = function (entity) {
        
        if (entity.REGION_AREA_DTL_ID > 0 && entity.ROW_NO == 0) {
            return "true";
        }
        var today = new Date();
        var day = entity.EFFECT_START_DATE.substring(0, 2)
        var month = entity.EFFECT_START_DATE.substring(3, 5)
        var year = entity.EFFECT_START_DATE.substring(6, 10)
        var StartDate = new Date(year, month, day);
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
        else if (parseInt(month) == parseInt(tomonth) && parseInt(day) < parseInt(todate)) {
            alert("Start Date Can not be any Previous Days");
            entity.EFFECT_START_DATE = '';
        }
        if (entity.EFFECT_END_DATE != null && entity.EFFECT_END_DATE != '') {
            var day1 = entity.EFFECT_END_DATE.substring(0, 2)
            var month1 = entity.EFFECT_END_DATE.substring(3, 5)
            var year1 = entity.EFFECT_END_DATE.substring(6, 10)
            var EndDate = new Date(year1, month1, day1);
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
        

        if (entity.EFFECT_START_DATE == null || entity.EFFECT_START_DATE == '') {
            alert("End date can not be entered before entering Start Date");
            entity.EFFECT_END_DATE = '';
            return "false";
        }
    }
    //Add New Row

    $scope.addNewRow = () => {
        
        if ($scope.gridOptions.data.length > 0 && $scope.gridOptions.data[0].AREA_CODE != null && $scope.gridOptions.data[0].AREA_CODE != '' && $scope.gridOptions.data[0].AREA_CODE != 'undefined') {
            var result = $scope.CheckDateValidation($scope.gridOptions.data[0]);
            if (result == "true") {
                var newRow = {
                    ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, REGION_AREA_MST_ID: $scope.gridOptions.data[0].REGION_AREA_MST_ID, REGION_AREA_DTL_ID: $scope.gridOptions.data[0].REGION_AREA_DTL_ID, AREA_ID: $scope.gridOptions.data[0].AREA_ID, AREA_CODE: $scope.gridOptions.data[0].AREA_CODE, REGION_AREA_DTL_STATUS: $scope.gridOptions.data[0].REGION_AREA_DTL_STATUS, EFFECT_START_DATE: $scope.gridOptions.data[0].EFFECT_START_DATE, EFFECT_END_DATE: $scope.gridOptions.data[0].EFFECT_END_DATE, REMARKS: $scope.gridOptions.data[0].REMARKS
                }
                var newRowSelected = {
                    AREA_CODE: $scope.gridOptions.data[0].AREA_CODE
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
    $scope.AreasLoad = function () {

        $scope.showLoader = true;

        InsertOrEditServices.GetExistingArea($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.Areas = data.data;
            $scope.AreasListAll = data.data;

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.EditItem = (entity) => {
        
        if ($scope.gridOptions.data.length > 0) {

            var newRow = {
                ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, REGION_AREA_MST_ID: entity.REGION_AREA_MST_ID, REGION_AREA_DTL_ID: entity.REGION_AREA_DTL_ID, AREA_ID: entity.AREA_ID,AREA_CODE: entity.AREA_CODE, REGION_AREA_DTL_STATUS: entity.REGION_AREA_DTL_STATUS, EFFECT_START_DATE: entity.EFFECT_START_DATE, EFFECT_END_DATE: entity.EFFECT_END_DATE, REMARKS: entity.REMARKS
            }
            $scope.gridOptions.data[0] = newRow;


        } else {
            notificationservice.Notification("Please Select Valid data", "", 'Only Single Row Left!!');

        }
        $scope.rowNumberGenerate();
        $scope.removeItem(entity);
    };
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
    $scope.LoadFormData = function () {
        
        $scope.showLoader = true;

        InsertOrEditServices.LoadData($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.relationdata = data.data;
            $scope.showLoader = false;


            for (var i in data.data) {
                
                if (data.data[i].REGION_CODE
                    == $scope.model.REGION_CODE) {
                    
                    $scope.model.REGION_AREA_MST_ID_ENCRYPTED = data.data[i].REGION_AREA_MST_ID_ENCRYPTED;
                    $scope.GetEditDataById($scope.model.REGION_AREA_MST_ID_ENCRYPTED);
                    $scope.addNewRow();
                }
            }
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
    $scope.gridOptions.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: false,  width: '50'
        }
        , {
            name: 'AREA_ID', field: 'AREA_ID', visible: false
        }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , {
            name: 'AREA_NAME', field: 'AREA_NAME', displayName: 'Area', enableFiltering: false, width: '28%', cellTemplate:
                '<select class="select2-single form-control" data-select2-id="{{row.entity.AREA_ID_GR}}" id="AREA_CODE" ng-disabled="row.entity.ROW_NO !=0"' +
                'name="AREA_CODE" ng-model="row.entity.AREA_CODE" style="width:100%" >' +
                '<option ng-repeat="item in grid.appScope.Areas" ng-selected="item.AREA_CODE == row.entity.AREA_CODE" value="{{item.AREA_CODE}}">{{ item.AREA_NAME }} | Code: {{ item.AREA_CODE }} </option>' +
                '</select>'
        }
        , {
            name: 'AREA_CODE', field: 'AREA_CODE', displayName: 'Code', enableFiltering: false, visible: false, width: '12%', cellTemplate:
                '<input type="text"  ng-model="row.entity.AREA_CODE" disabled="true"  class="pl-sm" />'
        }
        , {
            name: 'EFFECT_START_DATE', field: 'EFFECT_START_DATE', displayName: 'Start', enableFiltering: false, width: '15%', cellTemplate:
                '<div class="simple-date2">'
                + '<div class="input-group date">'
                + '<input   readonly datepicker  ng-disabled="row.entity.ROW_NO !=0" type="text" class="form-control" ng-model="row.entity.EFFECT_START_DATE" placeholder="dd/mm/yyyy" id="EFFECT_START_DATE">'
                + '</div>'
                + '</div>'
        }
        , {
            name: 'EFFECT_END_DATE', field: 'EFFECT_END_DATE', displayName: 'End', enableFiltering: false, width: '15%', cellTemplate:
                '<div class="simple-date2">'
                + '<div class="input-group date">'

                + '<input  readonly datepicker   ng-disabled="row.entity.ROW_NO !=0" type="text" class="form-control" ng-model="row.entity.EFFECT_END_DATE" placeholder="dd/mm/yyyy" ng-change="grid.appScope.CheckENDDateValidation(row.entity)"  id="EFFECT_END_DATE">'
                + '</div>'
                + '</div>'
        }
        
        , {
            name: 'REMARKS', field: 'REMARKS', displayName: 'Remark', enableFiltering: false, width: '15%', cellTemplate:
                '<input type="text"   ng-disabled="row.entity.ROW_NO !=0" ng-model="row.entity.REMARKS"  class="pl-sm" />'


        }
        , {
            name: 'REGION_AREA_DTL_STATUS', field: 'REGION_AREA_DTL_STATUS', displayName: 'Status', enableFiltering: false, width: '12%', cellTemplate:
                '<select class="form-control" id="REGION_AREA_DTL_STATUS"  ng-disabled="row.entity.ROW_NO !=0"'
                +'name="REGION_AREA_DTL_STATUS" ng-model="row.entity.REGION_AREA_DTL_STATUS" style="width:100%">'

                +'<option ng-repeat="item in grid.appScope.Status" value="{{item.STATUS}}" ng-selected="item.STATUS == row.entity.REGION_AREA_DTL_STATUS" >{{ item.STATUS }}</option>'
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
    $scope.AutoCompleteDataLoadForRegion = function (value) {
       
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

    $scope.SearchableAreaLoad = function (value) {
        
            

            return AreaInfoServices.GetSearchableArea($scope.model.COMPANY_ID, value).then(function (data) {
                
                $scope.AreasList = [];
                for (var i = 0; i < data.data.length; i++) {
                    var _area = {
                        AREA_CODE: data.data[i].AREA_CODE,
                        AREA_NAME: data.data[i].AREA_NAME,
                        AREA_ID: data.data[i].AREA_ID,
                        AREA_NAME_CODE: data.data[i].AREA_NAME + ' (' + data.data[i].AREA_CODE + ')',
                    }
                    $scope.AreasList.push(_area);

                }

                return $scope.AreasList;
            }, function (error) {
                alert(error);
                

                
            });
        
    }
    $scope.typeaheadSelectedRegion = function () {
        
        const searchIndex = $scope.Regions.findIndex((x) => x.REGION_CODE == $scope.model.REGION_CODE);

        $scope.model.REGION_ID = $scope.Regions[searchIndex].REGION_ID;
        $scope.model.REGION_CODE = $scope.Regions[searchIndex].REGION_CODE;
        $scope.model.REGION_NAME = $scope.Regions[searchIndex].REGION_NAME;

    };

    $scope.typeaheadSelectedArea = function (entity, selectedItem) {
        
        entity.AREA_ID = selectedItem.AREA_ID;
         entity.AREA_CODE = selectedItem.AREA_CODE;
          entity.AREA_NAME = selectedItem.AREA_NAME;
        entity.AREA_NAME_CODE = selectedItem.AREA_NAME_CODE;

    };

    
   

   
   
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
        window.location = "/SalesAndDistribution/RegionAreaRelation/InsertOrEdit";

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
            Controller_Name: 'RegionAreaRelation',
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

    $scope.formatRepoSelection =  function (repo) {
        return repo.text.split("--")[0];
    }

    $(".select2-single-Region").select2({
        placeholder: "Select",
        templateResult: $scope.formatRepo,
        templateSelection: $scope.formatRepoSelection
    });
   
    $scope.GetPermissionData();
    $scope.CompanyLoad();
    $scope.CompanyNameLoad();

    $scope.LoadStatus();
    $scope.RegionsLoad()
    $scope.AreasLoad()

    // This Method work is Edit Data Loading
    $scope.GetEditDataById = function (value) {
        
        if (value != undefined && value.length > 0) {
            InsertOrEditServices.GetEditDataById(value).then(function (data) {
                
                
                if (data.data != null && data.data.region_Area_Dtls != null && data.data.region_Area_Dtls.length > 0) {
                    $scope.model = data.data;
                    $scope.CompanyLoad();
                    $scope.CompanyNameLoad();
                    if ($scope.model.REGION_AREA_MST_STATUS == null) {
                        $scope.model.REGION_AREA_MST_STATUS = 'Active';
                    }
                    if (data.data.region_Area_Dtls != null) {
                        $scope.gridOptions.data = data.data.region_Area_Dtls;

                    }
                    var RegionData = {
                        REGION_CODE: $scope.model.REGION_CODE,
                        REGION_NAME: $scope.model.REGION_NAME,
                        REGION_ID: $scope.model.REGION_ID,
                    }
                    $scope.Regions.push(RegionData);
                    for (var i in $scope.gridOptions.data) {

                        var AreaData = {
                            AREA_CODE: $scope.gridOptions.data[i].AREA_CODE,
                            AREA_NAME: $scope.gridOptions.data[i].AREA_NAME,
                            AREA_ID: $scope.gridOptions.data[i].AREA_ID,
                        }
                        $scope.Areas.push(AreaData);
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

    $scope.SaveData = function (model) {
        
        $scope.showLoader = true;

        $scope.model.COMPANY_ID = parseInt($scope.model.COMPANY_ID);
        const searchRegIndex = $scope.Regions.findIndex((x) => x.REGION_CODE == $scope.model.REGION_CODE);

        $scope.model.REGION_ID = $scope.Regions[searchRegIndex].REGION_ID;
        $scope.gridOptions.data = $scope.gridOptions.data.filter((x) => x.ROW_NO !== 0);
        for (var i in $scope.gridOptions.data) {
            const searchIndex = $scope.Areas.findIndex((x) => x.AREA_CODE == $scope.gridOptions.data[i].AREA_CODE);
            if (searchIndex != -1) {
                $scope.gridOptions.data[i].AREA_ID = $scope.Areas[searchIndex].AREA_ID;
                $scope.gridOptions.data[i].AREA_CODE = $scope.Areas[searchIndex].AREA_CODE;
                $scope.gridOptions.data[i].AREA_NAME = $scope.Areas[searchIndex].AREA_NAME;

            }
        }

        $scope.model.region_Area_Dtls = $scope.gridOptions.data;

        
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

}]);

