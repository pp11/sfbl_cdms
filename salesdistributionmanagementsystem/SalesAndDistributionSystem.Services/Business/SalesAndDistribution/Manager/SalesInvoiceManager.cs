using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public class SalesInvoiceManager : ISalesInvoiceManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        private readonly IUserLogManager _logManager;

        public SalesInvoiceManager(ICommonServices commonServices, IConfiguration configuration, IUserLogManager logManager)
        {
            _commonServices = commonServices;
            _configuration = configuration;
            _logManager = logManager;
        }

        //-------------------Query Part ---------------------------------------------------

        string LoadPostableData_Query() => @"Select  ROWNUM ROW_NO, O.ORDER_MST_ID, O.ORDER_STATUS, O.ORDER_NO,I.INVOICE_TYPE_NAME,C.CUSTOMER_CODE,C.CUSTOMER_ID, O.ORDER_UNIT_ID, O.INVOICE_UNIT_ID, C.CUSTOMER_NAME, TO_CHAR(O.ORDER_DATE , 'dd/MM/yyyy')  ORDER_DATE , M.MARKET_NAME, O.ORDER_AMOUNT, O.NET_ORDER_AMOUNT from ORDER_MST O
                LEFT OUTER JOIN CUSTOMER_INFO C on C.CUSTOMER_ID = O.CUSTOMER_ID
                LEFT OUTER JOIN MARKET_INFO M on M.MARKET_ID = O.MARKET_ID
                LEFT OUTER JOIN INVOICE_TYPE_INFO I on I.INVOICE_TYPE_CODE= O.ORDER_TYPE
                WHERE O.ORDER_STATUS = 'Active' AND O.ORDER_PROCESS_STATUS = 'Complete' AND O.FINAL_SUBMIT_CONFIRM_STATUS = 'Complete' AND O.COMPANY_ID = :param1 AND O.ORDER_UNIT_ID = :param2 ";


        string SinglePostOrder_Query() => @"
                      begin
                            PRC_SINGLE_ORDER_POST(:param1 , :param2 , :param3  ,:param4, :param5, :param6 );
                      end;";
        string InvoiceDelete_Query() => @" begin
                           PRC_INVOICE_DELETE(:param1);
                      end;";
        string UpdateOrderStatus_Query() => @"UPDATE ORDER_MST SET ORDER_STATUS = 'Complete', INVOICE_STATUS = 'Complete' WHERE ORDER_MST_ID = :param1";
        string CancelOrder_Query() => @"begin PRC_CANCEL_ORDER(:param1, :param2, :param3); end;";

        string Load_PreInvocie_Data_Query() => @"select ROWNUM ROW_NO, ORDER_MST_ID, SKU_ID, SKU_CODE, SKU_NAME, PACK_SIZE, CURRENT_STOCK, ORDER_QTY, REVISED_ORDER_QTY, BONUS_QTY, TOTAL_QTY, STOCK_AFTER_INVOICE, SKU_PRICE, CUSTOMER_DISC_AMOUNT, CUSTOMER_PROD_DISC_AMOUNT, BONUS_DISC_AMOUNT, TOTAL_AMOUNT, NET_AMOUNT, PREV_IMS_QTY, SUGG_LIFTING_QTY, DIST_CURRENT_STOCK, LOADING_DISCOUNT, ADJUSTMENT_DISCOUNT,COMBO_LOADING_DISCOUNT,MST_COMBO_DISCOUNT   from table(FN_PRE_INVOICE(:param1 , TO_DATE( :param2  , 'dd/MM/yyyy')  , :param3 , :param4 , :param5 ,:param6  ,  :param7 ))";

        string Update_Qrder_Revised_Qty_Query() => @"UPDATE ORDER_DTL SET 
                       REVISED_ORDER_QTY = :param1
                       Where ORDER_MST_ID = :param2 AND SKU_ID = :param3";

        string Invoice_MST_Load_Query() => @"select  ROWNUM ROW_NO,  I.MST_ID, INVOICE_NO, TO_CHAR(I.INVOICE_DATE,'dd/MM/yyyy') INVOICE_DATE, 
I.INVOICE_TYPE_CODE,T.INVOICE_TYPE_NAME, I.DELIVERY_DATE, I.ORDER_MST_ID,I.CUSTOMER_CODE,C.CUSTOMER_NAME, I.ORDER_MST_ID,
 I.ORDER_NO, I.ORDER_DATE, I.ORDER_AMOUNT, I.NET_INVOICE_AMOUNT,I.INVOICE_UNIT_ID
 from INVOICE_MST  I
 LEFT OUTER JOIN CUSTOMER_INFO C on C.CUSTOMER_ID = I.CUSTOMER_ID
 LEFT OUTER JOIN Invoice_type_info T on T.INVOICE_TYPE_CODE = I.INVOICE_TYPE_CODE Where I.COMPANY_ID = :param1 and I.INVOICE_UNIT_ID = :param2";


        //-invoice Detail queries-------------

        string Invoice_Details_InvoiceMst_Query() => @"select  I.MST_ID, INVOICE_NO, TO_CHAR(I.INVOICE_DATE,'dd/MM/yyyy hh:mi:ss AM') INVOICE_DATE, 
I.INVOICE_TYPE_CODE,T.INVOICE_TYPE_NAME, I.ORDER_MST_ID,I.CUSTOMER_CODE,C.CUSTOMER_NAME,
 I.ORDER_NO,TO_CHAR(I.ORDER_DATE,'dd/MM/yyyy') ORDER_DATE, I.ORDER_AMOUNT, I.NET_INVOICE_AMOUNT,I.ORDER_NO,TO_CHAR(I.DELIVERY_DATE,'dd/MM/yyyy') DELIVERY_DATE, I.REPLACE_CLAIM_NO
  , I.BONUS_PROCESS_NO, I.BONUS_CLAIM_NO, I.CUSTOMER_ID,I.MARKET_ID, I.TERRITORY_ID,
   I.AREA_ID, I.REGION_ID, I.DIVISION_ID,I.PAYMENT_MODE, I.TP_AMOUNT,I.INVOICE_UNIT_ID
    ,I.VAT_AMOUNT, I.TOTAL_AMOUNT, I.CUSTOMER_DISC_AMOUNT, I.CUSTOMER_ADD1_DISC_AMOUNT,
     I.CUSTOMER_ADD2_DISC_AMOUNT, I.CUSTOMER_PRODUCT_DISC_AMOUNT, I.BONUS_PRICE_DISC_AMOUNT,
      I.PROD_BONUS_PRICE_DISC_AMOUNT, I.LOADING_CHARGE_AMOUNT, I.INVOICE_ADJUSTMENT_AMOUNT, 
      I.INVOICE_DISCOUNT_AMOUNT, I.NET_INVOICE_AMOUNT, I.TDS_AMOUNT, I.INVOICE_VERSION, I.REMARKS, 
      I.INVOICE_STATUS,I.COMPANY_ID,  I.MARKET_CODE
 from INVOICE_MST  I
 LEFT OUTER JOIN CUSTOMER_INFO C on C.CUSTOMER_ID = I.CUSTOMER_ID
 LEFT OUTER JOIN Invoice_type_info T on T.INVOICE_TYPE_CODE = I.INVOICE_TYPE_CODE
 WHERE I.MST_ID = :param1";

        string Invoice_Details_InvoiceDTL_Query() => @"
           SELECT 
                 ROWNUM ROW_NO,  D.DTL_ID,  D.MST_ID,  D.INVOICE_UNIT_ID, 
    D.INVOICE_DATE,  D.SKU_ID,  D.SKU_CODE, P.SKU_NAME,
    D.UNIT_TP,  D.INVOICE_QTY,  D.TP_AMOUNT,  D.TOTAL_AMOUNT
   ,  D.NET_PRODUCT_AMOUNT,  D.COMPANY_ID
FROM INVOICE_DTL D
LEFT OUTER JOIN PRODUCT_INFO P on P.SKU_ID = D.SKU_ID  WHERE D.MST_ID = :param1";
        string Invoice_Details_InvoiceComboGift_Query() => @"
          SELECT 
    ROWNUM ROW_NO, B.COMBO_GIFT_ID, B.MST_ID, B.INVOICE_DATE, 
   B.INVOICE_UNIT_ID, B.GIFT_ITEM_ID, B.UNIT_TP, P.GIFT_ITEM_NAME,
   B.BATCH_ID, B.BATCH_NO, B.GIFT_QTY, 
   B.GIFT_AMOUNT, B.COMPANY_ID
FROM INVOICE_COMBO_GIFT B
Left OUTER JOIN GIFT_ITEM_INFO P on P.GIFT_ITEM_ID = B.GIFT_ITEM_ID
WHERE B.MST_ID = :param1";

        string Invoice_Details_InvoiceComboBonus_Query() => @"
          SELECT 
   ROWNUM ROW_NO, B.COMBO_BONUS_ID, B.MST_ID, B.INVOICE_DATE, 
   B.INVOICE_UNIT_ID, B.SKU_ID, B.SKU_CODE, P.SKU_NAME,
   B.UNIT_TP, B.BATCH_ID, B.BATCH_NO, 
   B.BONUS_QTY, B.BONUS_AMOUNT, B.COMPANY_ID
  
FROM INVOICE_COMBO_BONUS B
LEFT OUTER JOIN PRODUCT_INFO P on P.SKU_ID = B.SKU_ID WHERE B.MST_ID = :param1";
        string Invoice_Details_InvoiceBonus_Query() => @"
          SELECT ROWNUM ROW_NO, B.BONUS_ID, B.DTL_ID, B.INVOICE_DATE, 
    B.INVOICE_UNIT_ID,  B.SKU_ID,  B.SKU_CODE, P.SKU_NAME,
    B.UNIT_TP,  B.BATCH_ID,  B.BATCH_NO, 
    B.BONUS_QTY,  B.BONUS_AMOUNT,  B.COMPANY_ID
  
FROM STL_ERP_SDS.INVOICE_BONUS B
LEFT OUTER JOIN PRODUCT_INFO P on P.SKU_ID = B.SKU_ID";
        string Invoice_Details_InvoiceIssue_Query() => @"
          SELECT 
   ROWNUM ROW_NO, B.ISSUE_ID, B.DTL_ID, B.INVOICE_DATE, 
   B.INVOICE_UNIT_ID, B.SKU_ID, B.SKU_CODE, P.SKU_NAME,
  B.UNIT_TP, B.BATCH_ID, B.BATCH_NO, 
   B.ISSUE_QTY, B.ISSUE_AMOUNT, B.COMPANY_ID
FROM INVOICE_ISSUE B
LEFT OUTER JOIN PRODUCT_INFO P on P.SKU_ID = B.SKU_ID";
   


        string Invoice_Details_InvoiceGift_Query() => @"
         SELECT 
   ROWNUM ROW_NO, B.GIFT_ID, B.DTL_ID, B.INVOICE_DATE, P.GIFT_ITEM_NAME,
   B.INVOICE_UNIT_ID, B.GIFT_ITEM_ID, B.UNIT_TP, 
   B.BATCH_ID, B.BATCH_NO, B.GIFT_QTY, 
  B.GIFT_AMOUNT, B.COMPANY_ID

FROM INVOICE_GIFT B
Left OUTER JOIN GIFT_ITEM_INFO P on P.GIFT_ITEM_ID = B.GIFT_ITEM_ID";



        string LoadSearchableInvoice_Query() => @"Select  INVOICE_NO , MST_ID FROM INVOICE_MST O
                WHERE  O.COMPANY_ID = :param1 AND O.INVOICE_UNIT_ID = :param2 AND upper(INVOICE_NO) like '%' || upper(:param3) || '%'   AND ROWNUM <21";
        string LoadSearchableInvoiceByCustomer_Query() => @"Select  INVOICE_NO , MST_ID FROM INVOICE_MST O
                WHERE  O.COMPANY_ID = :param1 AND O.INVOICE_UNIT_ID = :param2 AND upper(INVOICE_NO) like '%' || upper(:param3) || '%' AND O.CUSTOMER_ID = :param4  AND ROWNUM <21";

        string LoadSearchableInvoiceIndateAsc_Query() => @"SELECT INVOICE_NO, MST_ID
    FROM INVOICE_MST O
   WHERE   O.COMPANY_ID = :param1
         AND O.INVOICE_UNIT_ID = :param2
         AND UPPER (INVOICE_NO) LIKE '%' || UPPER ( :param3) || '%'
AND To_CHAR(O.invoice_date,'dd/MM/yyyy') = :param4 
         
         AND ROWNUM < 30
ORDER BY O.MST_ID ASC";
        string LoadSearchableOrderIndateAsc_Query() => @"SELECT ORDER_NO INVOICE_NO,ORDER_MST_ID MST_ID
    FROM ORDER_MST O
   WHERE   O.COMPANY_ID = :param1
         AND O.ORDER_UNIT_ID = :param2
         AND UPPER (ORDER_NO) LIKE '%' || UPPER ( :param3) || '%'
AND To_CHAR(O.ORDER_DATE,'dd/MM/yyyy') = :param4 
         
         AND ROWNUM < 30
ORDER BY O.ORDER_MST_ID ASC";
        string LoadSearchableInvoiceIndateDesc_Query() => @"SELECT INVOICE_NO, MST_ID
    FROM INVOICE_MST O
   WHERE O.COMPANY_ID = :param1
        AND O.INVOICE_UNIT_ID = :param2
         AND UPPER (INVOICE_NO) LIKE '%' || UPPER ( :param3) || '%'
AND To_CHAR(O.invoice_date,'dd/MM/yyyy') = :param4

         AND ROWNUM< 30
ORDER BY O.MST_ID  DESC";
        string LoadSearchableOrderIndateDesc_Query() => @"SELECT ORDER_NO INVOICE_NO,ORDER_MST_ID MST_ID
    FROM ORDER_MST O
   WHERE   O.COMPANY_ID = :param1
         AND O.ORDER_UNIT_ID = :param2
         AND UPPER (ORDER_NO) LIKE '%' || UPPER ( :param3) || '%'
AND To_CHAR(O.ORDER_DATE,'dd/MM/yyyy') = :param4 
         
         AND ROWNUM < 30
ORDER BY O.ORDER_MST_ID DESC";
        string LoadMaxMinInvoiceInDate() => @"SELECT MAX (INVOICE_NO) MAX_INVOICE, MIN (INVOICE_NO) MIN_INVOICE
  FROM INVOICE_MST O
 WHERE     O.COMPANY_ID = :param1
       AND O.INVOICE_UNIT_ID = :param2
       AND TO_CHAR (O.invoice_date, 'dd/MM/yyyy') = :param3
       AND ROWNUM < 30";
        string LoadMaxMinOrderInDate() => @"SELECT MAX (ORDER_NO) MAX_INVOICE, MIN (ORDER_NO) MIN_INVOICE
  FROM ORDER_MST O
 WHERE     O.COMPANY_ID = :param1
       AND O.INVOICE_UNIT_ID = :param2
       AND TO_CHAR (O.ORDER_DATE, 'dd/MM/yyyy') = :param3
       AND ROWNUM < 30";

        //---------- Method Execution Part ------------------------------------------------




        public async Task<string> LoadPostableData(OrderSKUFilterParameters model)
        {
            string Query = LoadPostableData_Query();
            if (model.DIVISION_ID != null && model.DIVISION_ID != "")
            {
                Query += string.Format("  AND O.DIVISION_ID = {0}", model.DIVISION_ID);
            }
            if (model.REGION_ID != null && model.REGION_ID != "")
            {
                Query += string.Format("  AND O.REGION_ID = {0}", model.REGION_ID);

            }
            if (model.AREA_ID != null && model.AREA_ID != "")
            {
                Query += string.Format("  AND O.AREA_ID = {0}", model.AREA_ID);

            }
            if (model.TERRITORY_ID != null && model.TERRITORY_ID != "")
            {
                Query += string.Format("  AND O.TERRITORY_ID = {0}", model.TERRITORY_ID);

            }
            if (model.CUSTOMER_ID != null && model.CUSTOMER_ID != "")
            {
                Query += string.Format("  AND O.CUSTOMER_ID = {0}", model.CUSTOMER_ID);

            }
            if (model.ORDER_TYPE != null && model.ORDER_TYPE != "")
            {
                Query += string.Format("  AND O.ORDER_TYPE = '{0}'", model.ORDER_TYPE);

            }
            if (model.ORDER_STATUS != null && model.ORDER_STATUS != "")
            {
                Query += string.Format("  AND O.ORDER_STATUS = '{0}'", model.ORDER_STATUS);

            }
            if (model.ORDER_ENTRY_TYPE != null && model.ORDER_ENTRY_TYPE != "")
            {
                Query += string.Format("  AND O.ORDER_ENTRY_TYPE = '{0}'", model.ORDER_ENTRY_TYPE);

            }
            if ((model.DATE_FROM != null && model.DATE_FROM != "") && (model.DATE_TO != null || model.DATE_TO != ""))
            {
                Query += string.Format("  AND  O.ORDER_DATE between TO_DATE('{0}','DD/MM/YYYY') AND TO_DATE('{1}','DD/MM/YYYY')+1", model.DATE_FROM, model.DATE_TO);

            } 
            DataTable dataTable =  await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), Query , _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.UNIT_ID.ToString() }));
            List<Order_Mst> bonus_Msts = new List<Order_Mst>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Order_Mst _bonus_Mst = new Order_Mst
                    {
                        ROW_NO = Convert.ToInt32(dataTable.Rows[i]["ROW_NO"]),
                        CUSTOMER_NAME = dataTable.Rows[i]["CUSTOMER_NAME"].ToString(),
                        MARKET_NAME = dataTable.Rows[i]["MARKET_NAME"].ToString(),
                        ORDER_DATE = dataTable.Rows[i]["ORDER_DATE"].ToString(),
                        ORDER_UNIT_ID = Convert.ToInt32(dataTable.Rows[i]["ORDER_UNIT_ID"]),
                        INVOICE_UNIT_ID = Convert.ToInt32(dataTable.Rows[i]["INVOICE_UNIT_ID"]),
                        CUSTOMER_CODE = dataTable.Rows[i]["CUSTOMER_CODE"].ToString(),
                        CUSTOMER_ID = Convert.ToInt32(dataTable.Rows[i]["CUSTOMER_ID"]),
                        ORDER_STATUS = dataTable.Rows[i]["ORDER_STATUS"].ToString(),

                        ORDER_MST_ID = Convert.ToInt32(dataTable.Rows[i]["ORDER_MST_ID"]),
                        ORDER_NO = dataTable.Rows[i]["ORDER_NO"].ToString(),
                        INVOICE_TYPE_NAME = dataTable.Rows[i]["INVOICE_TYPE_NAME"].ToString(),

                        ORDER_AMOUNT = Convert.ToInt32(dataTable.Rows[i]["ORDER_AMOUNT"]),
                        NET_ORDER_AMOUNT = dataTable.Rows[i]["NET_ORDER_AMOUNT"] != null && dataTable.Rows[i]["NET_ORDER_AMOUNT"].ToString() != "" ? Convert.ToInt32(dataTable.Rows[i]["NET_ORDER_AMOUNT"]) : 0

                    };

                    _bonus_Mst.ORDER_MST_ID_ENCRYPTED = _commonServices.Encrypt(_bonus_Mst.ORDER_MST_ID.ToString());
                    bonus_Msts.Add(_bonus_Mst);
                }
            }
            await _logManager.AddOrUpdate(model.db_security, "View", "INVOICE_MST", Convert.ToInt32(model.COMPANY_ID), Convert.ToInt32(model.UNIT_ID), Convert.ToInt32(model.ENTERED_BY),model.ENTERED_TERMINAL, "/SalesAndDistribution/Invoice/List", 0, "");

            return JsonSerializer.Serialize(bonus_Msts);
        }
        public async Task<string> AllPostOrder(string db, int Company_Id, int Unit_id, List<string> Order_id, string db_security,int enter_by,string terminal)
        {
            List<QueryPattern> listOfQuery = new List<QueryPattern>();

            List<string> _result;
            Result result_final = new Result();
            string result;
            string log_dtl = "Order_id: ";
            result_final.Key = "1";
            foreach (var item in Order_id)
            {
                 result = await _commonServices.ProcedureCallAsyn<string>(_configuration.GetConnectionString(db), SinglePostOrder_Query(), _commonServices.AddParameter(new string[] { item, Company_Id.ToString(), Unit_id.ToString(), OracleDbType.Varchar2.ToString(), OracleDbType.Varchar2.ToString(),enter_by.ToString() }));
                if (result != "")
                {
                    _result = JsonSerializer.Deserialize<List<string>>(result);
                    if (_result[0].ToString() == "1")
                    {
                        listOfQuery.Add(_commonServices.AddQuery(UpdateOrderStatus_Query(), _commonServices.AddParameter(new string[] { item })));
                        result_final.Status = result_final.Status + " Order Id: "+ item +", "+ _result[1];
                        log_dtl +=  item + ",";
                    }
                    else
                    {
                        result_final.Key = _result[0];
                        result_final.Status = result_final.Status + " Order Id: " + item + ", "+ _result[1];

                    }
                }
            }
          
           
            await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
            await _logManager.AddOrUpdate(db_security, "All Post Order", "INVOICE_MST", Company_Id, Unit_id, enter_by,terminal, "/SalesAndDistribution/Invoice/InsertOrUpdate", 0, log_dtl);

            result = JsonSerializer.Serialize(result_final);
            return result;


        }
        public async Task<string> SinglePostOrder(string db, int Company_Id,int Unit_id,string Order_id, string db_security, int enter_by, string terminal)
        {
            List<QueryPattern> listOfQuery = new List<QueryPattern>();

            List<string> _result = new List<string>();
            Result result_final = new Result();
            string result =await  _commonServices.ProcedureCallAsyn<string>(_configuration.GetConnectionString(db), SinglePostOrder_Query(), _commonServices.AddParameter(new string[] { Order_id, Company_Id.ToString(), Unit_id.ToString(), OracleDbType.Varchar2.ToString(),OracleDbType.Varchar2.ToString(),enter_by.ToString() }));
            if(result!="")
            {
                _result =  JsonSerializer.Deserialize<List<string>>(result);
            }
            if(_result[0].ToString() == "1")
            {
                listOfQuery.Add(_commonServices.AddQuery(UpdateOrderStatus_Query(), _commonServices.AddParameter(new string[] { Order_id.ToString() })));
                
            }
            await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
            result_final.Key = _result[0];
            result_final.Status = _result[1];
            result = JsonSerializer.Serialize(result_final);
            await _logManager.AddOrUpdate(db_security, "Single Post Order", "INVOICE_MST", Company_Id, Unit_id, enter_by, terminal, "/SalesAndDistribution/Invoice/InsertOrUpdate", 0, "Order_id" + Order_id);

            return result;


        }
        
        public async Task<string> CancelOrder(string db, string Company_Id, string Unit_id, string Order_id)
        {
            string result = await _commonServices.ProcedureCallAsyn<string>(_configuration.GetConnectionString(db), CancelOrder_Query(), _commonServices.AddParameter(new string[] { Order_id, Company_Id, Unit_id }));
            result = "1";
            return result;
        }
        public async Task<string> DeleteInvoice(string db, string InvoiceNo,int Company_Id,int Unit_Id, string db_security, int enter_by, string terminal)
        {
            string result = await _commonServices.ProcedureCallAsyn<string>(_configuration.GetConnectionString(db), InvoiceDelete_Query(), _commonServices.AddParameter(new string[] { InvoiceNo }));
            await _logManager.AddOrUpdate(db_security, "Delete Invoice", "INVOICE_MST", Company_Id, Unit_Id, enter_by, terminal, "/SalesAndDistribution/Invoice/InsertOrUpdate", 0, "Invoice No" + InvoiceNo);

            result = "1";
            return result;
        }

        public async Task<string> Load_Sales_Invoice_Mst_data(OrderSKUFilterParameters model)
        {
            string Query = Invoice_MST_Load_Query();
            if (model.DIVISION_ID != null && model.DIVISION_ID != "")
            {
                Query += string.Format("  AND I.DIVISION_ID = {0}", model.DIVISION_ID);
            }
            if (model.REGION_ID != null && model.REGION_ID != "")
            {
                Query += string.Format("  AND I.REGION_ID = {0}", model.REGION_ID);

            }
            if (model.AREA_ID != null && model.AREA_ID != "")
            {
                Query += string.Format("  AND I.AREA_ID = {0}", model.AREA_ID);

            }
            if (model.TERRITORY_ID != null && model.TERRITORY_ID != "")
            {
                Query += string.Format("  AND I.TERRITORY_ID = {0}", model.TERRITORY_ID);

            }
            if (model.CUSTOMER_ID != null && model.CUSTOMER_ID != "")
            {
                Query += string.Format("  AND I.CUSTOMER_ID = {0}", model.CUSTOMER_ID);

            }
            if (model.ORDER_TYPE != null && model.ORDER_TYPE != "")
            {
                Query += string.Format("  AND I.INVOICE_TYPE_CODE = '{0}'", model.ORDER_TYPE);

            }
            if (model.ORDER_STATUS != null && model.ORDER_STATUS != "")
            {
                Query += string.Format("  AND I.INVOICE_STATUS = '{0}'", model.ORDER_STATUS);

            }
        
            if ((model.DATE_FROM != null && model.DATE_FROM != "") && (model.DATE_TO != null || model.DATE_TO != ""))
            {
                Query += string.Format("  AND  I.INVOICE_DATE between TO_DATE('{0}','DD/MM/YYYY') AND TO_DATE('{1}','DD/MM/YYYY')+1", model.DATE_FROM, model.DATE_TO);

            }
            DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), Query, _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.UNIT_ID.ToString() }));
            List<Invoice_Mst> bonus_Msts = new List<Invoice_Mst>();
          //   

            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Invoice_Mst _bonus_Mst = new Invoice_Mst
                    {
                        ROW_NO = Convert.ToInt32(dataTable.Rows[i]["ROW_NO"]),
                        CUSTOMER_NAME = dataTable.Rows[i]["CUSTOMER_NAME"].ToString(),
                        INVOICE_NO = dataTable.Rows[i]["INVOICE_NO"].ToString(),
                        INVOICE_DATE = dataTable.Rows[i]["INVOICE_DATE"].ToString(),
                        MST_ID = Convert.ToInt32(dataTable.Rows[i]["MST_ID"]),
                        CUSTOMER_CODE = dataTable.Rows[i]["CUSTOMER_CODE"].ToString(),
                        INVOICE_TYPE_NAME = dataTable.Rows[i]["INVOICE_TYPE_NAME"].ToString(),
                        INVOICE_UNIT_ID = Convert.ToInt32(dataTable.Rows[i]["INVOICE_UNIT_ID"]),

                        ORDER_MST_ID = Convert.ToInt32(dataTable.Rows[i]["ORDER_MST_ID"]),
                        ORDER_NO = dataTable.Rows[i]["ORDER_NO"].ToString(),
                        ORDER_DATE = dataTable.Rows[i]["ORDER_DATE"].ToString(),
                        ORDER_AMOUNT = Convert.ToInt32(dataTable.Rows[i]["ORDER_AMOUNT"]),

                        NET_INVOICE_AMOUNT = Convert.ToInt32(dataTable.Rows[i]["NET_INVOICE_AMOUNT"])
                    };

                    _bonus_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(_bonus_Mst.MST_ID.ToString());
                    bonus_Msts.Add(_bonus_Mst);
                }
            }
            await _logManager.AddOrUpdate(model.db_security, "View", "INVOICE_MST", model.COMPANY_ID,model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/SalesAndDistribution/Invoice/List", 0,  "");

            return JsonSerializer.Serialize(bonus_Msts);

        }

        public async Task<string> Load_PreInvoice_Data(string db, string OrderId, string OrderDate, string CustomerId, string CustomerCode, string OrderUnitId, string InvoiceUnitId, string Company_Id) 
            => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Load_PreInvocie_Data_Query(), _commonServices.AddParameter(new string[] { OrderId, OrderDate, CustomerId, CustomerCode, OrderUnitId, InvoiceUnitId, Company_Id.ToString() })));
      
        public async Task<string> UpdateOrderRevisedQty(string db, Order_Mst _Mst)
        {
            List<QueryPattern> listOfQuery = new List<QueryPattern>();
            foreach(var item in _Mst.Order_Dtls)
            {
                    listOfQuery.Add(_commonServices.AddQuery(Update_Qrder_Revised_Qty_Query(), _commonServices.AddParameter(new string[] { item.REVISED_ORDER_QTY.ToString(), item.ORDER_MST_ID.ToString(), item.SKU_ID.ToString(), })));

            }
            

            await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
           
            return "1";


        }

        public async Task<string> LoadInvoiceDetailsData(string db, string Mst_Id)
        {

            DataSet ds = new DataSet();
            DataTable _InvoiceMst = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Invoice_Details_InvoiceMst_Query(), _commonServices.AddParameter(new string[] { Mst_Id }));
            DataTable _InvoiceDtl = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Invoice_Details_InvoiceDTL_Query(), _commonServices.AddParameter(new string[] { Mst_Id }));
            DataTable _InvoiceComboGifts = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Invoice_Details_InvoiceComboGift_Query(), _commonServices.AddParameter(new string[] { Mst_Id }));
            DataTable _InvoiceComboBonus = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Invoice_Details_InvoiceComboBonus_Query(), _commonServices.AddParameter(new string[] { Mst_Id }));

            List<int> InvDtl_Ids = new List<int>();
            string Dtl_Ids = "";
            foreach (DataRow row in _InvoiceDtl.Rows)
            {

                int dtl_id = Convert.ToInt32(row["DTL_ID"]);
                int st = InvDtl_Ids.Where(x => x == dtl_id).FirstOrDefault();
                if(st <1)
                {
                    InvDtl_Ids.Add(dtl_id);
                }

            }

            for(int i = 0;i<InvDtl_Ids.Count;i++)
            {
                if (i == 0)
                {
                    Dtl_Ids = InvDtl_Ids[i].ToString();
                }
                else
                {
                    Dtl_Ids = Dtl_Ids + ","+ InvDtl_Ids[i].ToString();

                }


            }

            string inv_gift = Invoice_Details_InvoiceGift_Query() + " WHERE B.DTL_ID in (" + Dtl_Ids + ")";
            string inv_bonus = Invoice_Details_InvoiceBonus_Query() + " WHERE B.DTL_ID in (" + Dtl_Ids + ")";
            string inv_issue = Invoice_Details_InvoiceIssue_Query() + " WHERE B.DTL_ID in (" + Dtl_Ids + ")";

            DataTable _invoiceGifts = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), inv_gift, _commonServices.AddParameter(new string[] { }));

            DataTable _InvoiceBonus = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), inv_bonus, _commonServices.AddParameter(new string[] { }));
            DataTable _InvoiceIssue = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), inv_issue, _commonServices.AddParameter(new string[] { }));

            ds.Tables.Add(_InvoiceMst);
            ds.Tables.Add(_InvoiceDtl);
            ds.Tables.Add(_InvoiceComboGifts);
            ds.Tables.Add(_InvoiceComboBonus);
            ds.Tables.Add(_invoiceGifts);
            ds.Tables.Add(_InvoiceBonus);
            ds.Tables.Add(_InvoiceIssue);
          
            return _commonServices.DataSetToJSON(ds);

        }
        public async Task<string> LoadSearchableInvoice(string db, string Company_Id, string Unit_Id, string Invoice_No)
        {
            DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableInvoice_Query(), _commonServices.AddParameter(new string[] { Company_Id, Unit_Id, Invoice_No }));

            List<Invoice_Mst> bonus_Msts = new List<Invoice_Mst>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Invoice_Mst _bonus_Mst = new Invoice_Mst
                    {

                        INVOICE_NO = dataTable.Rows[i]["INVOICE_NO"].ToString(),
                        MST_ID = Convert.ToInt32(dataTable.Rows[i]["MST_ID"]),
                        MST_ID_ENCRYPTED = _commonServices.Encrypt(dataTable.Rows[i]["MST_ID"].ToString())

                    };
                    bonus_Msts.Add(_bonus_Mst);
                }
            }
            return JsonSerializer.Serialize(bonus_Msts);
        }

        public async Task<string> LoadSearchableInvoiceByCustomer(string db, string Company_Id, string Unit_Id, string Invoice_No, string CUSTOMER_ID)
        {
            DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableInvoiceByCustomer_Query(), _commonServices.AddParameter(new string[] { Company_Id, Unit_Id, Invoice_No, CUSTOMER_ID }));

            List<Invoice_Mst> bonus_Msts = new List<Invoice_Mst>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Invoice_Mst _bonus_Mst = new Invoice_Mst
                    {

                        INVOICE_NO = dataTable.Rows[i]["INVOICE_NO"].ToString(),
                        MST_ID = Convert.ToInt32(dataTable.Rows[i]["MST_ID"]),
                        MST_ID_ENCRYPTED = _commonServices.Encrypt(dataTable.Rows[i]["MST_ID"].ToString())

                    };
                    bonus_Msts.Add(_bonus_Mst);
                }
            }
            return JsonSerializer.Serialize(bonus_Msts);
        }

        public async Task<string> LoadSearchableInvoiceInDate(string db, string Unit_Id, string Company_Id, string Invoice_No, string invoice_date, string q)
        {

            DataTable dataTable;
            if (q == "ASC")
            {

                dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableInvoiceIndateAsc_Query(), _commonServices.AddParameter(new string[] { Company_Id, Unit_Id, Invoice_No, invoice_date }));
            }
            else
            {
                dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableInvoiceIndateDesc_Query(), _commonServices.AddParameter(new string[] { Company_Id, Unit_Id, Invoice_No, invoice_date }));
            }


            List<Invoice_Mst> bonus_Msts = new List<Invoice_Mst>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Invoice_Mst _bonus_Mst = new Invoice_Mst
                    {

                        INVOICE_NO = dataTable.Rows[i]["INVOICE_NO"].ToString(),
                        MST_ID = Convert.ToInt32(dataTable.Rows[i]["MST_ID"]),
                        MST_ID_ENCRYPTED = _commonServices.Encrypt(dataTable.Rows[i]["MST_ID"].ToString())

                    };
                    bonus_Msts.Add(_bonus_Mst);
                }
            }
            return JsonSerializer.Serialize(bonus_Msts);
        }
        public async Task<string> LoadSearchableOrderInDate(string db, string Unit_Id, string Company_Id, string Invoice_No, string invoice_date, string q)
        {

            DataTable dataTable;
            if (q == "ASC")
            {

                dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableOrderIndateAsc_Query(), _commonServices.AddParameter(new string[] { Company_Id, Unit_Id, Invoice_No, invoice_date }));
            }
            else
            {
                dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableOrderIndateDesc_Query(), _commonServices.AddParameter(new string[] { Company_Id, Unit_Id, Invoice_No, invoice_date }));
            }


            List<Invoice_Mst> bonus_Msts = new List<Invoice_Mst>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Invoice_Mst _bonus_Mst = new Invoice_Mst
                    {

                        INVOICE_NO = dataTable.Rows[i]["INVOICE_NO"].ToString(),
                        MST_ID = Convert.ToInt32(dataTable.Rows[i]["MST_ID"]),
                        MST_ID_ENCRYPTED = _commonServices.Encrypt(dataTable.Rows[i]["MST_ID"].ToString())

                    };
                    bonus_Msts.Add(_bonus_Mst);
                }
            }
            return JsonSerializer.Serialize(bonus_Msts);
        }



        public async Task<string> LoadMaxMinInvoiceInDate(string db, string Unit_Id, string Company_Id,  string invoice_date)
        {

            DataTable dataTable= await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadMaxMinInvoiceInDate(), _commonServices.AddParameter(new string[] { Company_Id, Unit_Id,  invoice_date }));

            
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in dataTable.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in dataTable.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            var data= JsonSerializer.Serialize(parentRow);

            //List<Invoice_Mst> bonus_Msts = new List<Invoice_Mst>();
            //if (dataTable.Rows.Count > 0)
            //{
            //    for (int i = 0; i < dataTable.Rows.Count; i++)
            //    {
            //        Invoice_Mst _bonus_Mst = new Invoice_Mst
            //        {

            //            INVOICE_NO = dataTable.Rows[i]["INVOICE_NO"].ToString(),
            //            MST_ID = Convert.ToInt32(dataTable.Rows[i]["MST_ID"]),
            //            MST_ID_ENCRYPTED = _commonServices.Encrypt(dataTable.Rows[i]["MST_ID"].ToString())

            //        };
            //        bonus_Msts.Add(_bonus_Mst);
            //    }
            //}
            return data;
        }
        public async Task<string> LoadMaxMinOrderInDate(string db, string Unit_Id, string Company_Id, string invoice_date)
        {

            DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadMaxMinOrderInDate(), _commonServices.AddParameter(new string[] { Company_Id, Unit_Id, invoice_date }));


            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in dataTable.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in dataTable.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            var data = JsonSerializer.Serialize(parentRow);

            //List<Invoice_Mst> bonus_Msts = new List<Invoice_Mst>();
            //if (dataTable.Rows.Count > 0)
            //{
            //    for (int i = 0; i < dataTable.Rows.Count; i++)
            //    {
            //        Invoice_Mst _bonus_Mst = new Invoice_Mst
            //        {

            //            INVOICE_NO = dataTable.Rows[i]["INVOICE_NO"].ToString(),
            //            MST_ID = Convert.ToInt32(dataTable.Rows[i]["MST_ID"]),
            //            MST_ID_ENCRYPTED = _commonServices.Encrypt(dataTable.Rows[i]["MST_ID"].ToString())

            //        };
            //        bonus_Msts.Add(_bonus_Mst);
            //    }
            //}
            return data;
        }
    }
}
