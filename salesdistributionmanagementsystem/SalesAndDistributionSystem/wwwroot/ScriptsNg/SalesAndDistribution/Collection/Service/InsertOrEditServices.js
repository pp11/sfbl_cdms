ngApp.service("InsertOrEditServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/Collection/LoadData');
    }
    this.GetCompany = function () {
        return $http.get('/Security/Company/GetCompany');
    }
    this.GetCompanyName = function () {
        return $http.get('/Security/Company/GetCompanyName');
    }
    this.GetUnit = function () {
        return $http.get('/Security/Company/GetUnit');
    }
    this.LoadBranchData = function () {
        return $http.post('/SalesAndDistribution/Collection/LoadBranchData');
    }
    this.LoadCollectionModeData = function () {
        return $http.post('/SalesAndDistribution/Collection/LoadCollectionMode');
    }
    this.GetSearchableCustomer = function (comp_id, Customer) {
        return $http.post('/SalesAndDistribution/Customer/GetSearchableCustomer', { COMPANY_ID: parseInt(comp_id), CUSTOMER_NAME: Customer });
    }

    
    this.LoadCustomerDaywiseBalance = function (cust_id) {
        
        return $http.post('/SalesAndDistribution/Collection/LoadCustomerDaywiseBalance', { CUSTOMER_ID: cust_id });
    }
    
    this.GetUnitName = function () {
        return $http.get('/Security/Company/GetUnitName');
    }
    this.GetEditDataById = function (id, UNIT_ID) {

        return $http.post('/SalesAndDistribution/Collection/GetEditDataById', { q: id, UNIT_ID: UNIT_ID });
    }
    this.LoadSearchableInvoice = function (inv_no, customer_id) {
        return $http.post('/SalesAndDistribution/SalesInvoice/LoadInvoicesByCustomer', { INVOICE_NO: inv_no, CUSTOMER_ID: customer_id });
    }

    this.LoadCustomerData = function (companyId) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadCustomerData', { COMPANY_ID: parseInt(companyId) });
    }

    //this.LoadCustomerData = function (companyId) {
    //    return $http.get('/SalesAndDistribution/Customer/LoadCustomerDropdownData', { COMPANY_ID: parseInt(companyId) });
    //}

    this.AddOrUpdate = function (model) {
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/Collection/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
    this.Update_Approval = function (collection_mst_id) {
        return $http.post('/SalesAndDistribution/Collection/Update_Approval', { COLLECTION_MST_ID: parseInt(collection_mst_id) });
    }
});