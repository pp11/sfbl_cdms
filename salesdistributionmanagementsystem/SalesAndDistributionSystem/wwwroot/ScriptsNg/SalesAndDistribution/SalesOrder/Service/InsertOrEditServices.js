ngApp.service("InsertOrEditServices", function ($http) {
    
    this.LoadData = function (companyId) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadDataProcessed = function (companyId) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadDataProcessed', { COMPANY_ID: parseInt(companyId) });
    }
    this.GetDistributorCode = function (companyId) {
        return $http.get('/Security/User/GetDistributorCode', { });
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
    this.LoadFilteredProcessedData = function (model) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadFilteredProcessedData', {
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
 
    this.Process_Order = function (model) {

        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/SalesOrder/Process_Order",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }
    this.Delete_Processed_Order = function (model) {

        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/SalesOrder/Delete_Processed_Order__Dtl",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }
    this.Delete_Order_full_Query = function (model) {

        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/SalesOrder/Delete_Order_full_Query",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(model),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
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
    this.Save_Final_Order = function (model) {
        return $http.post('/SalesAndDistribution/SalesOrder/Save_Final_Order', { ORDER_MST_ID: parseInt(model.ORDER_MST_ID), ORDER_MST_ID_ENCRYPTED: model.ORDER_MST_ID_ENCRYPTED, FINAL_SUBMISSION_STATUS: model.FINAL_SUBMISSION_STATUS, FINAL_SUBMIT_CONFIRM_STATUS: model.FINAL_SUBMIT_CONFIRM_STATUS, NOTIFY_TEXT: model.NOTIFY_TEXT });
    }

    this.Save_Final_Post_Order = function (model) {
        return $http.post('/SalesAndDistribution/SalesOrder/Save_Final_Post_Order', { ORDER_MST_ID: parseInt(model.ORDER_MST_ID), ORDER_MST_ID_ENCRYPTED: model.ORDER_MST_ID_ENCRYPTED, FINAL_SUBMISSION_STATUS: model.FINAL_SUBMISSION_STATUS, FINAL_SUBMIT_CONFIRM_STATUS: model.FINAL_SUBMIT_CONFIRM_STATUS, NOTIFY_TEXT: model.NOTIFY_TEXT });
    }

    //this.Save_Final_Order = function (model) {

    //    var dataType = 'application/json; charset=utf-8';
    //    return $http({
    //        type: 'POST',
    //        method: 'POST',
    //        url: "/SalesAndDistribution/SalesOrder/Save_Final_Order",
    //        dataType: 'json',
    //        contentType: dataType,
    //        data: JSON.stringify(model),
    //        headers: { 'Content-Type': 'application/json; charset=utf-8' },
    //    });

    //}
    this.GetCompanyList = function () {
        return $http.get('/Security/Company/LoadData');
    }
    this.GetUnitList = function () {
        return $http.get('/Security/Company/LoadUnitData');
    }
    this.LoadProductData = function (companyId, customer_id) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadProductsData', { COMPANY_ID: parseInt(companyId), CUSTOMER_ID: parseInt(customer_id) });
    }
    this.LoadProductsSpecific = function (companyId, customer_id) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadProductsSpecific', { COMPANY_ID: parseInt(companyId), CUSTOMER_ID: parseInt(customer_id) });
    }

    this.LoadProductType = function (companyId, customer_id) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadProductType', { COMPANY_ID: parseInt(companyId), CUSTOMER_ID: parseInt(customer_id) });
    }

    
    this.LoadProductsSpecificType = function (companyId, customer_id, type ) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadProductsSpecificType', { COMPANY_ID: parseInt(companyId), CUSTOMER_ID: parseInt(customer_id), distributor_product_type: type });
    }
    this.Get_Customer_Balance = function (customer_id) {
        return $http.post('/SalesAndDistribution/SalesOrder/Get_Customer_Balance', { CUSTOMER_ID: parseInt(customer_id)  });
    }
    
    this.LoadProductPrice = function (companyId, sku_id, model) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadProductPrice', {
            COMPANY_ID: parseInt(companyId), SKU_ID: parseInt(sku_id), ORDER_TYPE: model.ORDER_TYPE, CUSTOMER_ID: parseInt(model.CUSTOMER_ID)
           
        });
    }
    this.Replacement_Master = function (customer_id, mst_id) {
        return $http.post('/SalesAndDistribution/SalesOrder/Replacement_Master', { CUSTOMER_ID: parseInt(customer_id), ORDER_MST_ID: parseInt(mst_id) });
    }

    this.Replacement_Dtl = function (rep_claim_no, order_unit_id) {
        return $http.post('/SalesAndDistribution/SalesOrder/Replacement_Dtl', { REPLACE_CLAIM_NO: rep_claim_no, ORDER_UNIT_ID: order_unit_id });
    }

    this.Refurbishment_Master = function (customer_id) {
        return $http.post('/SalesAndDistribution/SalesOrder/Refurbishment_Master', { CUSTOMER_ID: parseInt(customer_id) });
    }

    this.Refurbishment_Dtl = function (rep_claim_no) {
        return $http.post('/SalesAndDistribution/SalesOrder/Refurbishment_Dtl', { REPLACE_CLAIM_NO: rep_claim_no });
    }

    this.GetDivisions = function (companyId) {
        return $http.post('/SalesAndDistribution/Division/LoadData', { COMPANY_ID: parseInt(companyId) });
    }

    this.GetDivitionToMarketRelation = function (row_no) { 
        return $http.post('/SalesAndDistribution/Market/GetDivitionToMarketRelation', { ROW_NO: parseInt(row_no) });
    }
    

    this.LoadBaseProductData = function (companyId) {
        return $http.post('/SalesAndDistribution/Bonus/LoadBaseProductData', { COMPANY_ID: parseInt(companyId) });
    }
    this.LoadBrandData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadBrandData', { COMPANY_ID: parseInt(comp_id) });
    }
    this.LoadCategoryData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadCategoryData', { COMPANY_ID: parseInt(comp_id) });
    }
    this.LoadGroupData = function (comp_id) {
        return $http.post('/SalesAndDistribution/Bonus/LoadGroupData', { COMPANY_ID: parseInt(comp_id) });
    }
    this.LoadProductsDataFiltered = function (comp_id, model) {
        return $http.post('/SalesAndDistribution/SalesOrder/GetProductDataFiltered', { COMPANY_ID: parseInt(comp_id), BASE_PRODUCT_ID: model.BASE_PRODUCT_ID, BASE_PRODUCT_ID: model.BASE_PRODUCT_ID, GROUP_ID: model.GROUP_ID, BRAND_ID: model.BRAND_ID, CATEGORY_ID: model.CATEGORY_ID, SKU_ID: model.SKU_ID, UNIT_ID: JSON.stringify(parseInt((model.UNIT_ID))), CUSTOMER_ID: parseInt(model.CUSTOMER_ID), CUSTOMER_CODE: model.CUSTOMER_CODE, ORDER_TYPE: model.ORDER_TYPE });
    }
    this.Save_Post_Confirm_Order = function (model) {
        return $http.post('/SalesAndDistribution/SalesOrder/Update_Order_Confirmation_Status', { ORDER_MST_ID: parseInt(model.ORDER_MST_ID), NOTIFY_TEXT: model.NOTIFY_TEXT });
    }

    this.Cancel_Post_Confirm_Order = function ( model) {
        return $http.post('/SalesAndDistribution/SalesOrder/Cancel_Order_Confirmation_Status', { ORDER_MST_ID: parseInt(model.ORDER_MST_ID) });
    }

    this.LoadOrderMonitorData = function (companyId, unitId) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadMonitoringData', { COMPANY_ID: parseInt(companyId), UNIT_ID: parseInt(unitId) });
    }
    this.LoadFilteredMonitorData = function (model) {
        return $http.post('/SalesAndDistribution/SalesOrder/LoadFilteredMonitoringData', {
            DATE_FROM: model.DATE_FROM,
            DATE_TO: model.DATE_TO,
            DIVISION_ID: model.DIVISION_ID,
            REGION_ID: model.REGION_ID,
            AREA_ID: model.AREA_ID,
            TERRITORY_ID: model.TERRITORY_ID,
            CUSTOMER_ID: model.CUSTOMER_ID,
            ORDER_ENTRY_TYPE: model.ORDER_ENTRY_TYPE,
            ORDER_STATUS: model.ORDER_STATUS,
            ORDER_TYPE: model.ORDER_TYPE,
            UNIT_ID: parseInt(model.UNIT_ID)
        });
    }

   

    this.LoadProductPerfactOrderQty = function (customer_code, order_type, order_unit_id, sku_code, sku_price, order_qty) {
        let obj = {
            CUSTOMER_CODE: customer_code,
            ORDER_TYPE: order_type,
            ORDER_UNIT_ID: order_unit_id,
            SKU_CODE: sku_code,
            ORDER_AMOUNT: sku_price,
            REGION_ID: order_qty
        };

        var dataType = 'application/json; charset=utf-8';
        return $http({
            type: 'POST',
            method: 'POST',
            url: "/SalesAndDistribution/SalesOrder/LoadProductPerfactOrderQty",
            dataType: 'json',
            contentType: dataType,
            data: JSON.stringify(obj),
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
        });

    }
});