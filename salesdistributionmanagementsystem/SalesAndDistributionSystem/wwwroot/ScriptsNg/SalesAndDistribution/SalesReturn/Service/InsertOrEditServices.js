ngApp.service("InsertOrEditServices", function ($http) {

    
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/SalesOrder/GetCompany');
    }
    this.GetUnit = function () {
        return $http.get('/SalesAndDistribution/SalesOrder/GetUnit');
    }
    this.LoadInvoiceTypes = function (companyId) {

        return $http.post('/SalesAndDistribution/InvoiceType/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadSearchableInvoice = function (inv_no) {
        return $http.post('/SalesAndDistribution/SalesInvoice/LoadSearchableInvoice', { INVOICE_NO: inv_no });
    }
  
    this.LoadPostableData = function (companyId, order_date, order_type, market_id) {
        return $http.post('/SalesAndDistribution/SalesInvoice/LoadPostableData', { COMPANY_ID: parseInt(companyId), ORDER_DATE: order_date, ORDER_TYPE: order_type, DIVISION_ID: parseInt(market_id) });
    }

    this.Load_Sales_Invoice_Mst_data = function () {
        return $http.get('/SalesAndDistribution/SalesInvoice/Load_Sales_Invoice_Mst_data');
    }

    this.LoadPreInvoiceData = function (model) {
        
        return $http.post('/SalesAndDistribution/SalesInvoice/LoadPreInvoiceData', {
            ORDER_MST_ID: parseInt(model.ORDER_MST_ID), ORDER_DATE: model.ORDER_DATE, CUSTOMER_ID: parseInt(model.CUSTOMER_ID), CUSTOMER_CODE: model.CUSTOMER_CODE, ORDER_UNIT_ID: parseInt(model.ORDER_UNIT_ID), INVOICE_UNIT_ID: parseInt(model.INVOICE_UNIT_ID), COMPANY_ID: parseInt(model.COMPANY_ID)
        });
    }
   
    this.CancelOrder = function (companyId, order_mst_id) {
        return $http.post('/SalesAndDistribution/SalesInvoice/CancelOrder', { COMPANY_ID: parseInt(companyId), ORDER_MST_ID: parseInt(order_mst_id) });
    }
    this.DeleteReturn = function ( invoice) {
        return $http.post('/SalesAndDistribution/SalesReturn/DeleteReturn', { INVOICE_NO: invoice });
    }

    this.ProcessPartialReturn = function (Req_data) {
        
        return $http.post('/SalesAndDistribution/SalesReturn/ProcessPartialReturnList', JSON.stringify(Req_data));
    }
    this.SalesReturnPartial = function (model) {
        return $http.post('/SalesAndDistribution/SalesReturn/SalesReturnPartial', JSON.stringify(model));
    }
    this.LoadInvoiceDetailsData = function (mst_id) {
        return $http.post('/SalesAndDistribution/SalesInvoice/LoadInvoiceDetailsData', { q: mst_id });
    }
   
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetUnitList = function () {
        return $http.get('/Security/Company/LoadUnitData');
    }

    this.LoadCustomerData = function (companyId) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadCustomerData', { COMPANY_ID: parseInt(companyId) });
    }


    this.FullReturn = function (model) {
        return $http.post('/SalesAndDistribution/SalesReturn/FullReturn', { INVOICE_NO: model.INVOICE_NO, RETURN_UNIT_ID: model.RETURN_UNIT_ID, COMPANY_ID: model.COMPANY_ID, INVOICE_DATE: model.INVOICE_DATE, CUSTOMER_ID: model.CUSTOMER_ID, CUSTOMER_CODE: model.CUSTOMER_CODE });
    }
    this.LoadSalesReturnMst = function (model) {
        
        return $http.post('/SalesAndDistribution/SalesReturn/LoadSalesReturnMst', {
           
            DATE_FROM: model.DATE_FROM,
            DATE_TO: model.DATE_TO,
            CUSTOMER_ID: model.CUSTOMER_ID,
            ORDER_TYPE: model.RETURN_TYPE

        });
    }
    this.LoadInvoiceDetailsData = function (mst_id) {
        return $http.post('/SalesAndDistribution/SalesInvoice/LoadInvoiceDetailsData', { q: mst_id });
    }
    this.LoadReturnDetailsData = function (mst_id) {
        return $http.post('/SalesAndDistribution/SalesReturn/LoadInvoiceDetailsData', { q: mst_id });
    }

    this.InvoicesLoad = function (model) {
        return $http.post('/SalesAndDistribution/SalesReturn/InvoicesLoad', { DATE_FROM: model.DATE_FROM, DATE_TO: model.DATE_TO });
    }

});