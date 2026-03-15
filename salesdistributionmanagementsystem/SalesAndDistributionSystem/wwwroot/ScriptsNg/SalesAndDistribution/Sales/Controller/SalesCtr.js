ngApp.controller('ngGridCtrl', ['$scope', 'SalesService', 'permissionProvider', 'notificationservice', 'gridregistrationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, SalesService, permissionProvider, notificationservice, gridregistrationservice, $http, $log, $filter, $timeout, $interval, $q) {

    $scope.model = { ID: 0, COMPANY_ID: 0, COMPANY_NAME: '', COMPANY_SHORT_NAME: '', COMPANY_ADDRESS1: '', COMPANY_ADDRESS1: '' }
    $scope.gridOptions = (gridregistrationservice.GridRegistration("Sales List"));
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    }
    $scope.rowNumberGenerate = function () {
        for (let i = 0; i < $scope.gridOptions.data.length; i++) {
            $scope.gridOptions.data[i].ROW_NO = i + 1;
        }
        
    }
    $scope.GridDefalutData = function () {
        return {
            ROW_NO: 1,

            PRODUCT_NAME: '',
            QUANTITY: 0,
            UNIT: '',
            PRICE_MRP: 0,
            PRICE_TOTAL: 0
        }
    }
   
    $scope.gridOptions = {
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
            $interval(function () {
                $scope.gridApi.core.handleWindowResize();
            }, 800, 20);
        },
        data: [$scope.GridDefalutData()]
    }
    $scope.RoleLst = [];
    $scope.Companies = []
    $scope.addNewRow = () => {
        $scope.gridOptions.data.push($scope.GridDefalutData());
        $scope.rowNumberGenerate();
    };
   

    // Grid one row remove if this mehtod is call
    $scope.removeItem = function (entity) {

        var index = $scope.gridOptions.data.indexOf(entity);
        if ($scope.gridOptions.data.length > 0) {
            $scope.gridOptions.data.splice(index, 1);
        }
        $scope.rowNumberGenerate();
    }

    $scope.AutoCompleteDataLoadForRole = function (value) {
        if (value.length >= 3) {
            

            return SalesService.GetSearchableRoles(value, $scope.model.COMPANY_ID).then(function (data) {
                $scope.RoleLst = data.data;
                

                return $scope.RoleLst;
            }, function (error) {
                alert(error);
                

                
            });
        }
    }


    $scope.typeaheadSelectedRole = function (entity, selectedItem) {
        $scope.model.ROLE_ID = selectedItem.ROLE_ID;
        $scope.model.ROLE_NAME = selectedItem.ROLE_NAME;

    };


    $scope.gridOptions.columnDefs = [
        { name: 'SL', field: 'ROW_NO', enableFiltering: false, width: '50' }

        , { name: 'COMPANY_ID', field: 'COMPANY_ID', visible: false }
        , { name: 'ID', field: 'ID', visible: false }

        , {
            name: 'PRODUCT_NAME', field: 'PRODUCT_NAME', displayName: 'Product', enableFiltering: false, width: '20%', cellTemplate:
                '<div class="typeaheadcontainer">'+
                '<input type="text" autocomplete="off" style="width:100%;" class= "form-control" name="ROLE_NAME"'+
                                                       'ng-model="model.ROLE_NAME"' + 
                                                       'uib-typeahead="Role as Role.ROLE_NAME for Role in grid.appScope.AutoCompleteDataLoadForRole($viewValue)| limitTo:5"' +
                                                       'typeahead-append-to-body="true"' +
                                                       'placeholder = "Enter (Customer Name) minimum 3 character"'+
                                                       'typeahead-editable="false"' +
                                                       'typeahead-on-select="grid.appScope.typeaheadSelectedRole(row.entity, $item)" />' +
                    '</div>'
        }
        , {
            name: 'QUANTITY', field: 'QUANTITY', displayName: 'QTY', enableFiltering: false, width: '10%', cellTemplate:
                '<input required="required"  type="number"  ng-model="row.entity.QUANTITY"  class="pl-sm" />'
        }
        , {
            name: 'UNIT', field: 'UNIT', displayName: 'Unit ', enableFiltering: false, width: ' 24%', cellTemplate:
                '<input required="required"  type="text"  ng-model="row.entity.COMPANY_ADDRESS1"  class="pl-sm" />'
        }
        , {
            name: 'PRICE_MRP', field: 'PRICE_MRP', displayName: 'Price (MRP) ', enableFiltering: false, width: '12%', cellTemplate:
                '<input required="required"  type="number"  ng-model="row.entity.PRICE_MRP"  class="pl-sm" />'
        }, {
            name: 'PRICE_TOTAL', field: 'PRICE_TOTAL', displayName: 'Price', enableFiltering: false, width: '16%', cellTemplate:
                '<input required="required"  type="number"  ng-model="row.entity.PRICE_MRP"  class="pl-sm" />'
        }

        , {
            name: 'Actions', displayName: 'Actions', enableFiltering: false, enableColumnMenu: false, width: '30%', cellTemplate:
                '<div style="margin:1px;">' +
                '<button style="margin-bottom: 5px;" ng-click="grid.appScope.addNewRow()" type="button" class="btn btn-outline-primary mb-1"><img src="/img/plus-icon.png" height="15px" style="border:none" ></button>' +
                '<button style="margin-bottom: 5px;" ng-click="grid.appScope.removeItem(row.entity)" type="button" class="btn btn-outline-danger mb-1"><img src="/img/minus-icon.png" height="15px" ></button>' +
             '</div>'
        },

    ];


}]);

