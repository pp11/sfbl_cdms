ngApp.controller('ngGridCtrl', ['$scope', 'InsertOrEditServices', 'CustomerInfoServices', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, InsertOrEditServices, CustomerInfoServices, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { COMPANY_ID: 0, USER_ID: 0, USER_CODE: '', USER_TYPE: 'Active', EFFECT_START_DATE: '01/01/1991', EFFECT_END_DATE: '01/01/2099', REMARKS: ''}

    $scope.getPermissions = [];
    $scope.Companies = [];
    $scope.Status = [];
    $scope.Users = [];
    $scope.UsersList = [];
    $scope.Accounts = [];
    $scope.AccountsList = [];
    $scope.AccountsListAll = [];
    $scope.relationdata = [];
    $scope.Dtl_Status = [];
    
    $scope.DefalutData = function () {
        return { COMPANY_ID: 0, USER_ID: 0, USER_CODE: '', USER_TYPE: 'DSM', EFFECT_START_DATE: '01/01/1991', EFFECT_END_DATE: '01/01/2099', REMARKS: '' }
    }

    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 1, COMPANY_ID: 0, ACCOUNT_ID: 0, ACCOUNT_CODE: '', USER_TYPE: 'DSM', EFFECT_START_DATE: '01/01/1991', EFFECT_END_DATE: '01/01/2099', REMARKS: ''
        }
    }

    $scope.gridOptions = (gridregistrationservice.GridRegistration("User Account Relation Info"));
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.gridOptions.data = [];
    //Grid Register
    $scope.gridOptions = {
        data: [$scope.GridDefalutData()]
    }
    
    $scope.gridOptions.columnDefs = [
        {
            name: '#', field: 'ROW_NO', enableFiltering: true, width: '50'
        }
        , {
            name: 'USER_ACCOUNT_MST_ID', field: 'USER_ACCOUNT_MST_ID', visible: false
        }
        , {
            name: 'USER_ACCOUNT_DTL_ID', field: 'USER_ACCOUNT_DTL_ID', visible: false
        }
        , {
            name: 'ACCOUNT_ID', field: 'ACCOUNT_ID', visible: false
        }
        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , {
            name: 'ACCOUNT_NAME', field: 'ACCOUNT_NAME', displayName: 'Account', enableFiltering: true, width: '40%', cellTemplate:
                '<select class="select2-single form-control" data-select2-id="{{row.entity.ACCOUNT_ID_GR}}" id="ACCOUNT_CODE" ng-disabled="row.entity.ROW_NO !=0 || row.entity.USER_ACCOUNT_DTL_ID>0"' +
                'name="ACCOUNT_CODE" ng-model="row.entity.ACCOUNT_CODE" style="width:100%" >' +
                '<option ng-repeat="item in grid.appScope.Accounts" ng-selected="item.ACCOUNT_CODE == row.entity.ACCOUNT_CODE" value="{{item.ACCOUNT_CODE}}">{{ item.ACCOUNT_NAME }} | Code: {{ item.ACCOUNT_CODE }}</option>' +
                '</select>'
        }
        , {
            name: 'ACCOUNT_CODE', field: 'ACCOUNT_CODE', displayName: 'Code', enableFiltering: true, width: '15%', cellTemplate:
                '<input type="text"  ng-model="row.entity.ACCOUNT_CODE" disabled="true"  class="pl-sm" />'
        }

        , {
            name: 'USER_TYPE', field: 'USER_TYPE', displayName: 'Status', enableFiltering: false, width: '25%', cellTemplate:
                '<select class="form-control"  ng-disabled="row.entity.ROW_NO !=0" id="USER_TYPE"'
                + 'name="USER_TYPE" ng-model="row.entity.USER_TYPE" style="width:100%">'
                + '<option ng-repeat="item in grid.appScope.Status" value="{{item.STATUS}}" ng-selected="item.STATUS == row.entity.USER_TYPE" >{{ item.STATUS }}</option>'
                + '</select>'
        }
        , {
            name: 'Action', displayName: 'Action', width: '15%', enableFiltering: false, enableColumnMenu: false, cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.EditItem(row.entity)" type="button" class="btn btn-outline-success mb-1"><img src="/img/edit-icon.png" height="15px" ></button>' +

                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO == 0" ng-click="grid.appScope.addNewRow()" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                '<button style="margin-bottom: 5px;"  ng-show="row.entity.ROW_NO != 0" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +

                '</div>'
        },

    ];
    //Generate New Row No
    $scope.rowNumberGenerate = function () {
        
        for (let i = 0; i < $scope.gridOptions.data.length; i++) {
            
            $scope.gridOptions.data[i].ROW_NO = i;
            if ($scope.gridOptions.data[i].ACCOUNT_CODE == '') {
                $scope.gridOptions.data[i].ACCOUNT_ID = 0;
                $scope.gridOptions.data[i].ACCOUNT_NAME = '';
                $scope.gridOptions.data[i].ACCOUNT_NAME_CODE = '';

            }
           
           
        }

    }
    
    //Add New Row

    $scope.addNewRow = () => {
        
        if ($scope.gridOptions.data.length > 0 && $scope.gridOptions.data[0].ACCOUNT_CODE != null && $scope.gridOptions.data[0].ACCOUNT_CODE != '' && $scope.gridOptions.data[0].ACCOUNT_CODE != 'undefined') {
            var result = "true";
            if (result == "true") {
                var newRow = {
                    ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, USER_ACCOUNT_MST_ID: $scope.gridOptions.data[0].USER_ACCOUNT_MST_ID, USER_ACCOUNT_DTL_ID: $scope.gridOptions.data[0].USER_ACCOUNT_DTL_ID, ACCOUNT_ID: $scope.gridOptions.data[0].ACCOUNT_ID, ACCOUNT_CODE: $scope.gridOptions.data[0].ACCOUNT_CODE, USER_TYPE: $scope.gridOptions.data[0].USER_TYPE, EFFECT_START_DATE: $scope.gridOptions.data[0].EFFECT_START_DATE, EFFECT_END_DATE: $scope.gridOptions.data[0].EFFECT_END_DATE, REMARKS: $scope.gridOptions.data[0].REMARKS
                }
                var newRowSelected = {
                    ACCOUNT_CODE: $scope.gridOptions.data[0].ACCOUNT_CODE
                }
                $scope.AccountsList.push(newRowSelected);
                $scope.gridOptions.data.push(newRow);
                $scope.gridOptions.data[0] = $scope.GridDefalutData();

            }
            
        } else {
            notificationservice.Notification("Please Enter Valid Account First", "", 'Only Single Row Left!!');

        }
        $scope.rowNumberGenerate();

    };
    $scope.EditItem = (entity) => {
        
        if ($scope.gridOptions.data.length > 0) {
           
            var newRow = {
                ROW_NO: 1, COMPANY_ID: $scope.model.COMPANY_ID, USER_ACCOUNT_MST_ID: entity.USER_ACCOUNT_MST_ID,   USER_ACCOUNT_DTL_ID: entity.USER_ACCOUNT_DTL_ID,   ACCOUNT_ID: entity.ACCOUNT_ID, ACCOUNT_CODE: entity.ACCOUNT_CODE, USER_TYPE: entity.USER_TYPE, EFFECT_START_DATE: entity.EFFECT_START_DATE, EFFECT_END_DATE: entity.EFFECT_END_DATE, REMARKS: entity.REMARKS
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
    $scope.AutoCompleteDataLoadForUser = function (value) {
            

            return CustomerInfoServices.GetSearchableCustomer($scope.model.COMPANY_ID, value).then(function (data) {
                
                $scope.UsersList = [];
                for (var i = 0; i < data.data.length; i++) {
                    var _division = {
                        USER_CODE: data.data[i].USER_CODE,
                        USER_NAME: data.data[i].USER_NAME,
                        USER_ID: data.data[i].USER_ID,
                        USER_NAME_CODE: data.data[i].USER_NAME + ' (' + data.data[i].USER_CODE + ')',
                    }
                    $scope.UsersList.push(_division);

                }

                return $scope.UsersList;
            }, function (error) {
                alert(error);
                

                
            });
    }

    $scope.SearchableAccountLoad = function (value) {
            

            return AccountInfoServices.GetSearchableAccount($scope.model.COMPANY_ID, value).then(function (data) {
                
                $scope.AccountsList = [];
                for (var i = 0; i < data.data.length; i++) {
                    var _region = {
                        ACCOUNT_CODE: data.data[i].ACCOUNT_CODE,
                        ACCOUNT_NAME: data.data[i].ACCOUNT_NAME,
                        ACCOUNT_ID: data.data[i].ACCOUNT_ID,
                        ACCOUNT_NAME_CODE: data.data[i].ACCOUNT_NAME + ' (' + data.data[i].ACCOUNT_CODE + ')',
                    }
                    $scope.AccountsList.push(_region);

                }

                return $scope.AccountsList;
            }, function (error) {
                alert(error);
                

                
            });
        
    }

    $scope.typeaheadSelectedUser = function () {
        
        const searchIndex = $scope.Users.findIndex((x) => x.USER_CODE == $scope.model.USER_CODE);

        $scope.model.USER_ID = $scope.Users[searchIndex].USER_ID;
        $scope.model.USER_CODE = $scope.Users[searchIndex].USER_CODE;
        $scope.model.USER_NAME = $scope.Users[searchIndex].USER_NAME;

    };

    //$scope.typeaheadSelectedAccount = function (entity) {
    //    
    //    const searchIndex = $scope.Accounts.findIndex((x) => x.ACCOUNT_CODE == entity.ACCOUNT_CODE);
    //    if (searchIndex != -1) {
    //        entity.ACCOUNT_ID = $scope.Accounts[searchIndex].ACCOUNT_ID;
    //        entity.ACCOUNT_CODE = $scope.Accounts[searchIndex].ACCOUNT_CODE;
    //        entity.ACCOUNT_NAME = $scope.Accounts[searchIndex].ACCOUNT_NAME;
    //        $scope.Accounts = $scope.Accounts.filter((x) => x.ACCOUNT_CODE !== entity.ACCOUNT_CODE);

    //    }
    //   };
    $scope.typeaheadSelectedAccount = function (entity, selectedItem) {
        
        entity.ACCOUNT_ID = selectedItem.ACCOUNT_ID;
        entity.ACCOUNT_CODE = selectedItem.ACCOUNT_CODE;
        entity.ACCOUNT_NAME = selectedItem.ACCOUNT_NAME;
        entity.ACCOUNT_NAME_CODE = selectedItem.ACCOUNT_NAME_CODE;

    };

    $scope.UsersLoad = function () {
        
        $scope.showLoader = true;

        InsertOrEditServices.GetExistingUser($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.Users = data.data;

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
    $scope.AccountsLoad = function () {

        $scope.showLoader = true;

        InsertOrEditServices.GetExistingAccount($scope.model.COMPANY_ID).then(function (data) {
            
            $scope.Accounts = data.data;
            $scope.AccountsListAll = data.data;

            $scope.showLoader = false;
        }, function (error) {
            
            $scope.showLoader = false;

        });
    }
   

   
    $scope.LoadStatus = function () {
       
        var Active = {
            STATUS: 'DSM'
        }
        var InActive = {
            STATUS: 'OSM'
        }
        $scope.Status.push(Active);
        $scope.Status.push(InActive);

    }

   
    

    $scope.ClearForm = function () {
        window.location = "/SalesAndDistribution/UserAccountRelation/InsertOrEdit";

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
            Controller_Name: 'UserAccountRelation',
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

    $(".select2-single-User").select2({
        placeholder: "Select",
        templateResult: $scope.formatRepo,
        templateSelection: $scope.formatRepoSelection
    });
    $scope.GetPermissionData();
    $scope.CompanyLoad();
    $scope.CompanyNameLoad();

    $scope.UsersLoad();
    $scope.LoadStatus();
    $scope.AccountsLoad();
   
    // This Method work is Edit Data Loading
    $scope.GetEditDataById = function (value) {
        
        if (value != undefined && value.length > 0) {
            InsertOrEditServices.GetEditDataById(value).then(function (data) {
                
    

                if (data.data != null && data.data.user_account_relation_Dtls !=null && data.data.user_account_relation_Dtls.length>0) {
                    $scope.model = data.data;
                    $scope.CompanyLoad();
                    $scope.CompanyNameLoad();
                    if ($scope.model.USER_TYPE == null) {
                        $scope.model.USER_TYPE = 'Active';
                    }
                    if (data.data.user_account_relation_Dtls != null) {
                        $scope.gridOptions.data = data.data.user_account_relation_Dtls;

                    }
                    var UserData = {
                        USER_CODE: $scope.model.USER_CODE,
                        USER_NAME: $scope.model.USER_NAME,
                        USER_ID: $scope.model.USER_ID,
                    }
                    $scope.Users.push(UserData);
                    for (var i in $scope.gridOptions.data) {

                        var AccountData = {
                            ACCOUNT_CODE: $scope.gridOptions.data[i].ACCOUNT_CODE,
                            ACCOUNT_NAME: $scope.gridOptions.data[i].ACCOUNT_NAME,
                            ACCOUNT_ID: $scope.gridOptions.data[i].ACCOUNT_ID,
                        }
                        $scope.Accounts.push(AccountData);
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
                
                if (data.data[i].USER_NAME == $scope.model.USER_NAME) {
                    
                    $scope.model.USER_ACCOUNT_MST_ID_ENCRYPTED = data.data[i].USER_ACCOUNT_MST_ID_ENCRYPTED;
                    $scope.GetEditDataById($scope.model.USER_ACCOUNT_MST_ID_ENCRYPTED);
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
                const searchIndex = $scope.Accounts.findIndex((x) => x.ACCOUNT_CODE == $scope.gridOptions.data[i].ACCOUNT_CODE);
                if (searchIndex != -1) {
                    $scope.gridOptions.data[i].ACCOUNT_ID = $scope.Accounts[searchIndex].ACCOUNT_ID;
                    $scope.gridOptions.data[i].ACCOUNT_CODE = $scope.Accounts[searchIndex].ACCOUNT_CODE;
                    $scope.gridOptions.data[i].ACCOUNT_NAME = $scope.Accounts[searchIndex].ACCOUNT_NAME;

                }
            }
            $scope.model.user_account_relation_Dtls = $scope.gridOptions.data;

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
            notificationservice.Notification('Please Insert Account and add to list!', 1, 'Data Save Successfully !!');

        }
    }

}]);

