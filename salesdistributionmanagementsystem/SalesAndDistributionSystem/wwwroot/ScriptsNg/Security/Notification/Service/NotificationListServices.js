ngApp.service("InsertOrEditServices", function ($http) {
    this.GetNotificationList = function (company_id, date_from, date_to) {
        
        return $http.get('/Security/Notification/LoadData', { params: { COMPANY_ID: company_id, DATE_FROM: date_from, DATE_TO: date_to } });
    }

})