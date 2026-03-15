ngApp.service("InsertOrEditServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadFilteredData = function (model) {
        
        return $http.post('/SalesAndDistribution/SalesOrder/LoadFilteredData', {
            DATE_FROM: model.DATE_FROM,
            DATE_TO: model.DATE_TO,
            DIVISION_ID: model.DIVISION_ID,
            REGION_ID: model.REGION_ID,
            AREA_ID: model.AREA_ID,
            TERRITORY_ID: model.TERRITORY_ID,
            CUSTOMER_ID: model.CUSTOMER_ID,
            ORDER_ENTRY_TYPE: model.ORDER_ENTRY_TYPE,
            ORDER_STATUS: model.ORDER_STATUS,
            ORDER_TYPE: model.ORDER_TYPE
        });
    }

    
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/SalesOrder/GetCompany');
    }
    this.GetUnit = function () {
        return $http.get('/SalesAndDistribution/SalesOrder/GetUnit');
    }
   
    this.GetCompanyName = function () {
        return $http.get('/Security/Company/GetCompanyName');
    }
 
    this.LoadInvoiceTypes = function (companyId) {

        return $http.post('/SalesAndDistribution/InvoiceType/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetEditDataById = function (id) {

        return $http.post('/SalesAndDistribution/SalesOrder/GetEditDataById', { q: id });
    }
    this.LoadCustomerData = function (companyId) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadCustomerData', { COMPANY_ID: parseInt(companyId) });
    }

    this.AddOrUpdate = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/SalesOrder/AddOrUpdate",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });
       
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetUnitList = function () {
        return $http.get('/Security/Company/LoadUnitData');
    }
    this.LoadProductData = function (companyId) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadProductsData', { COMPANY_ID: parseInt(companyId) });
    }

    this.Get_Customer_Balance = function (customer_id) {
        return $http.post('/SalesAndDistribution/SalesOrder/Get_Customer_Balance', { CUSTOMER_ID: parseInt(customer_id)  });
    }
    
    this.LoadProductPrice = function (companyId, sku_id, model) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadProductPrice', {
            COMPANY_ID: parseInt(companyId), SKU_ID: parseInt(sku_id), ORDER_TYPE: model.ORDER_TYPE, CUSTOMER_ID: parseInt(model.CUSTOMER_ID)
           
        });
    }


    this.GetDivisions = function (companyId) {
        return $http.post('/SalesAndDistribution/Division/LoadData', { COMPANY_ID: parseInt(companyId) });
    }

    this.GetDivitionToMarketRelation = function (row_no) { //row_num : DIVISION_ID 
        return $http.post('/SalesAndDistribution/Market/GetDivitionToMarketRelation', { ROW_NO: parseInt(row_no) });
    }
    
});