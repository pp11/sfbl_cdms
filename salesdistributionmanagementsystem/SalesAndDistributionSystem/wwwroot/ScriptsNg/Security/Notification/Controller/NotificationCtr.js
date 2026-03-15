ngApp.controller('ngNotificationCtrl', ['$scope', 'NotificationServices', 'notificationservice', '$http', '$log', '$filter', '$timeout', '$interval', '$q', function ($scope, NotificationServices, notificationservice, $http, $log, $filter, $timeout, $interval, $q) {

    'use strict'
    $scope.model = { COMPANY_ID: 0, DIVISION_ID: 0, DIVISION_NAME: '', DIVISION_CODE: '', DIVISION_ADDRESS: '', REMARKS: '', DIVISION_STATUS: 'Active' }

    $scope.Notification_Data = [];
    //var connection = new signalR.HubConnectionBuilder().withUrl("/NotificationHub",
    //    {
    //        skipNegotiation: true,
    //        transport: signalR.HttpTransportType.WebSockets
    //    }).build();
    //connection.start();
    //connection.on("ReceiveNotificationHandler", function (message) {
    //    
    //    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    //    $scope.NotificationLoad();
    //    notificationservice.Notification(1, 1, msg);
    //});


    $scope.NotificationLoad = function (companyId) {
        
        $scope.showLoader = true;

        NotificationServices.NotificationLoad().then(function (data) {
            
            
            $scope.Notification_Data = data.data;
            $scope.Notification_Count = $scope.Notification_Data.length;
            $scope.showLoader = false;
        }, function (error) {
            
            alert(error);
            
            $scope.showLoader = false;

        });
    }
   
        


    

  

    $scope.SaveData = function (model) {
        
        $scope.showLoader = true;
        
        DivisionInfoServices.AddOrUpdate(model).then(function (data) {

            notificationservice.Notification(data.data, 1, 'Data Save Successfully !!');
            if (data.data == 1) {
                //connection.invoke("SendMessage", "From Divsion", "Hey New Data Load Called!!").catch(function (err) {
                //    return console.error(err.toString());
                //});
                $scope.showLoader = false;
                $scope.GetPermissionData();
                $scope.CompanyLoad();
                $scope.LoadFormData();
            }
            else {
                $scope.showLoader = false;
            }
        });
    }

}]);

