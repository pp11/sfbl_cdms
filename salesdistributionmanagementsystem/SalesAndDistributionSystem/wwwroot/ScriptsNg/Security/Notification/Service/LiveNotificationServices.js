ngApp.service("LiveNotificationServices", function ($http) {
    
    this.NotificationLoad = function () {
        return $http.post('/Security/Notification/NotificationLoad', {  });
    }

    this.UpdateNotificationViewStatus = function (NotificationId) {
        return $http.get('/Security/Notification/UpdateNotificationViewStatus', { NOTIFICATION_ID: parseInt(NotificationId) });
    }
    this.Notification_Permitted_Users = function (policy_id, company_Id, unit_Id) {
        company_Id = company_Id == null || company_Id == NaN || company_Id == undefined || company_Id == 0 ? 0 : company_Id;
        unit_Id = unit_Id == null || unit_Id == NaN || unit_Id == undefined || unit_Id == 0 ? 0 : unit_Id;

        return $http.post('/Security/Notification/Notification_Permitted_Users', { NOTIFICATION_POLICY_ID: parseInt(policy_id), COMPANY_ID: parseInt(company_Id), UNIT_ID: parseInt(unit_Id) });
    }
    
   
   
});