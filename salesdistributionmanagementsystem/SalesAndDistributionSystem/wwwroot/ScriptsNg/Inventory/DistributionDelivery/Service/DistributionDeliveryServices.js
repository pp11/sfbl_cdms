ngApp.service("DistributionDeliveryServices", function($http) {
    this.GetUnit = function () {
        return $http.get('/Security/Company/GetUserUnit');
    }
    this.GetVehicles = function (companyId) {
        return $http.post('/SalesAndDistribution/Vehicle/LoadData', { COMPANY_ID: companyId });
    }
    this.GetDistributionRoutes = function () {
        return $http.get('/Inventory/DistributionDelivery/GetDistributionRoutes');
    }
    this.GetInvoices = function () {
        return $http.get('/Inventory/DistributionDelivery/GetPendingInvoices');
    }
    this.GetCustomer = function (routeId) {
        return $http.get('/Inventory/DistributionDelivery/CustomerByRoute/' + routeId);
    }
    this.GetProductsByInvoice = function (id) {
        return $http.get('/Inventory/DistributionDelivery/GetProductsByInvoice/' + id);
    }
    this.GetProductsByInvoices = function (Invoices) {
        return $http.post('/Inventory/DistributionDelivery/GetProductsByInvoices/', JSON.stringify(Invoices));
    }
    this.GetProductsByGifts = function (Invoices) {
        return $http.post('/Inventory/DistributionDelivery/GetProductsByGifts/', JSON.stringify(Invoices));
    }
    this.GetGiftByInvoice = function (id) {
        return $http.get('/Inventory/DistributionDelivery/GetGiftByInvoice/' + id);
    }
    this.AddOrUpdate = function (model) {
        return $http.post('/Inventory/DistributionDelivery/AddOrUpdate', JSON.stringify(model));
    }
    this.GetDeliveryList = function (model) {
        return $http.post('/Inventory/DistributionDelivery/LoadData', model)
    }
    this.GetNonConfirmDeliveryList = function (model) {
        return $http.post('/Inventory/DistributionDelivery/GetNonConfirmDeliveryList', model)
    }
    this.GetEditDataById = function (id) {
        return $http.post('/Inventory/DistributionDelivery/GetEditDataById', { MST_ID_ENCRYPTED: id })
    }
    this.GetProductBatches = function (id) {
        return $http.post('/Inventory/DistributionDelivery/GetProductBatches', { MST_ID_ENCRYPTED: id });
    }
})