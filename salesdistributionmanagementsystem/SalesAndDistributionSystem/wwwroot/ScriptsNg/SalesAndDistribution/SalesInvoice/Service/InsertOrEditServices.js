ngApp.service("InsertOrEditServices", function ($http) {

    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetCompany = function () {
        return $http.get('/SalesAndDistribution/SalesOrder/GetCompany');
    }
    this.GetUnit = function () {
        return $http.get('/SalesAndDistribution/SalesOrder/GetUnit');
    }
    this.LoadInvoiceTypes = function (companyId) {

        return $http.post('/SalesAndDistribution/InvoiceType/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetEditDataById = function (id) {

        return $http.post('/SalesAndDistribution/SalesOrder/GetEditDataById', { q: id });
    }
    this.LoadPostableData = function (model) {
        return $http.post('/SalesAndDistribution/SalesInvoice/LoadPostableData', {

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

    this.Load_Sales_Invoice_Mst_data = function (model) {
        return $http.post('/SalesAndDistribution/SalesInvoice/Load_Sales_Invoice_Mst_data', {
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

    this.LoadPreInvoiceData = function (model) {
        
        return $http.post('/SalesAndDistribution/SalesInvoice/LoadPreInvoiceData', {
            ORDER_MST_ID: parseInt(model.ORDER_MST_ID), ORDER_DATE: model.ORDER_DATE, CUSTOMER_ID: parseInt(model.CUSTOMER_ID), CUSTOMER_CODE: model.CUSTOMER_CODE, ORDER_UNIT_ID: parseInt(model.ORDER_UNIT_ID), INVOICE_UNIT_ID: parseInt(model.INVOICE_UNIT_ID), COMPANY_ID: parseInt(model.COMPANY_ID)
        });
    }
    this.DeleteInvoice = function ( invoice) {
        return $http.post('/SalesAndDistribution/SalesInvoice/DeleteInvoice', { INVOICE_NO: invoice });
    }
    this.CancelOrder = function (companyId, order_mst_id) {
        return $http.post('/SalesAndDistribution/SalesInvoice/CancelOrder', { COMPANY_ID: parseInt(companyId), ORDER_MST_ID: parseInt(order_mst_id) });
    }

    this.LoadInvoiceDetailsData = function (mst_id) {
        return $http.post('/SalesAndDistribution/SalesInvoice/LoadInvoiceDetailsData', { q: mst_id });
    }

    this.UpdateOrderRevisedQty = function (model) {
        
        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/SalesInvoice/UpdateOrderRevisedQty",
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
    this.SinglePostOrder = function (companyId, order_id) {
        return $http.post('/SalesAndDistribution/SalesInvoice/SinglePostOrder', { COMPANY_ID: parseInt(companyId), ORDER_MST_ID: parseInt(order_id) });
    }
    this.AllPostOrder = function (companyId, order_no) {
        return $http.post('/SalesAndDistribution/SalesInvoice/AllPostOrder', { COMPANY_ID: parseInt(companyId), ORDER_NO_LIST: order_no });
    }
    this.LoadProductPrice = function (companyId, sku_id, model) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadProductPrice', {
            COMPANY_ID: parseInt(companyId), SKU_ID: parseInt(sku_id), ORDER_TYPE: model.ORDER_TYPE, CUSTOMER_ID: parseInt(model.CUSTOMER_ID)
           
        });
    }
    this.LoadActiveDivisions = function (companyId) {
        return $http.post('/SalesAndDistribution/Division/LoadActiveDivisionData', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetDivitionToMarketRelation = function (row_no) { //row_num : DIVISION_ID 
        return $http.post('/SalesAndDistribution/Market/GetDivitionToMarketRelation', { ROW_NO: parseInt(row_no) });
    }
});