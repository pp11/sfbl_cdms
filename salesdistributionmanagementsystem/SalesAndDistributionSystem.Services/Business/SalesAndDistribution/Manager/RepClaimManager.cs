using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.TableModels.Company;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Company;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public class RepClaimManager : IRepClaimManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        private readonly IUserLogManager _logManager;
        private readonly INotificationManager _NotificationManager;

        public RepClaimManager(ICommonServices commonServices, IConfiguration configuration, IUserLogManager logManager, INotificationManager NotificationManager)
        {
            _commonServices = commonServices;
            _configuration = configuration;
            _logManager = logManager;
            _NotificationManager = NotificationManager;
        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT 
   ROWNUM ROW_NO, O.ORDER_MST_ID, O.ORDER_NO,
   TO_CHAR(O.ORDER_DATE, 'DD/MM/YYYY') ORDER_DATE, 
   O.ORDER_TYPE, TO_CHAR(O.DELIVERY_DATE, 'DD/MM/YYYY') DELIVERY_DATE, O.CUSTOMER_ID, 
   O.CUSTOMER_CODE, O.MARKET_ID, M.MARKET_NAME, C.CUSTOMER_NAME ,
   O.ORDER_ENTRY_TYPE, O.ORDER_AMOUNT, O.ORDER_STATUS
   FROM ORDER_MST O
   LEFT OUTER JOIN CUSTOMER_INFO C on C.CUSTOMER_ID = O.CUSTOMER_ID
   LEFT OUTER JOIN MARKET_INFO M on M.MARKET_ID = O.MARKET_ID
   Where O.COMPANY_ID = :param1 and O.ORDER_UNIT_ID = :param2 AND TO_DATE(SYSDATE,'DD/MM/YYYY') = TO_DATE(O.ENTERED_DATE,'DD/MM/YYYY') ";

        string LoadFilteredData_Query() => @"SELECT 
   ROWNUM ROW_NO, O.ORDER_MST_ID, O.ORDER_NO,
   TO_CHAR(O.ORDER_DATE, 'DD/MM/YYYY') ORDER_DATE, 
   O.ORDER_TYPE, TO_CHAR(O.DELIVERY_DATE, 'DD/MM/YYYY') DELIVERY_DATE, O.CUSTOMER_ID, 
   O.CUSTOMER_CODE, O.MARKET_ID, M.MARKET_NAME, C.CUSTOMER_NAME ,
   O.ORDER_ENTRY_TYPE, O.ORDER_AMOUNT, O.ORDER_STATUS
   FROM ORDER_MST O
   LEFT OUTER JOIN CUSTOMER_INFO C on C.CUSTOMER_ID = O.CUSTOMER_ID
   LEFT OUTER JOIN MARKET_INFO M on M.MARKET_ID = O.MARKET_ID
   Where O.COMPANY_ID = :param1 and O.ORDER_UNIT_ID = :param2 ";
        string LoadProducts_Query() => @"Select p.SKU_ID, p.SKU_CODE, p.SKU_NAME, p.PACK_SIZE,  p.COMPANY_ID, p.UNIT_ID , I.UNIT_TP
                       from Product_Info p
                       inner  join PRODUCT_PRICE_INFO I on I.SKU_ID = p.SKU_ID And PRICE_EFFECT_DATE = ( Select MAX(PRICE_EFFECT_DATE) From PRODUCT_PRICE_INFO where SKU_ID =  p.SKU_ID)
                       Where  p.PRODUCT_STATUS = 'Active' and    p.company_ID = :param1";
        string LoadCustomerData_Query() => @"Select COMPANY_ID, DIVISION_ID, DIVISION_CODE, DIVISION_NAME, REGION_ID, REGION_CODE,
 REGION_NAME, AREA_ID, AREA_CODE, AREA_NAME, TERRITORY_ID, TERRITORY_CODE, TERRITORY_NAME,
  MARKET_ID, MARKET_CODE, MARKET_NAME, INVOICE_FLAG, CUSTOMER_ID, CUSTOMER_CODE, CUSTOMER_NAME, CUSTOMER_ADDRESS
   from VW_LOCATION_FOR_ORDER Where COMPANY_ID = :param1";
        string LoadProductPrice_Query() => @"select  FN_ORDER_SKU_PRICE(:param1, :param2, :param3)  from DUAL";
        string AddOrUpdate_Master_AddQuery() => @"INSERT INTO ORDER_MST 
                                         (ORDER_MST_ID,ORDER_NO, ORDER_DATE, ORDER_TYPE,DELIVERY_DATE,
                                         CUSTOMER_ID,CUSTOMER_CODE,MARKET_ID , TERRITORY_ID, AREA_ID,REGION_ID,DIVISION_ID,PAYMENT_MODE,REPLACE_CLAIM_NO,
                                         BONUS_PROCESS_NO,BONUS_CLAIM_NO,INVOICE_STATUS,ORDER_AMOUNT,ORDER_STATUS,COMPANY_ID,ORDER_UNIT_ID,INVOICE_UNIT_ID,REMARKS
                                         ,SPA_TOTAL_AMOUNT,SPA_COMMISSION_PCT,SPA_COMMISSION_AMOUNT,SPA_NET_AMOUNT,ENTERED_BY,ENTERED_DATE,ENTERED_TERMINAL,ORDER_ENTRY_TYPE) 
                                         VALUES ( :param1, :param2,  TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM'), :param4,TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'), :param6,
                                        :param7,:param8, :param9, :param10, :param11, :param12, :param13, :param14
                                         , :param15, :param16, :param17, :param18, :param19 , :param20
                                          , :param21, :param22, :param23, :param24, :param25 , :param26
                                           , :param27, :param28, TO_DATE(:param29, 'DD/MM/YYYY HH:MI:SS AM'), :param30, :param31
                                          )";
        string AddOrUpdate_Detail_AddQuery() => @"INSERT INTO ORDER_DTL
                                         (ORDER_DTL_ID, ORDER_MST_ID, SKU_ID, SKU_CODE, ORDER_QTY, UNIT_TP, ORDER_AMOUNT, STATUS, 
                                         COMPANY_ID, UNIT_ID, REMARKS, SPA_UNIT_TP, SPA_AMOUNT, SPA_REQ_TIME_STOCK, SPA_DISCOUNT_TYPE, SPA_DISCOUNT_VAL_PCT,
                                         SPA_CUST_COM, SPA_DISCOUNT_AMOUNT, SPA_TOTAL_AMOUNT,
                                         ENTERED_BY,ENTERED_DATE,ENTERED_TERMINAL) 
                                         VALUES ( :param1, :param2,  :param3, :param4,:param5, :param6,
                                        :param7,:param8, :param9, :param10, :param11, :param12, :param13, :param14
                                         , :param15, :param16, :param17, :param18, :param19 , :param20
                                          , TO_DATE(:param21, 'DD/MM/YYYY HH:MI:SS AM'), :param22
                                          )";
        string AddOrUpdate_Master_UpdateQuery() => @"UPDATE ORDER_MST SET  ORDER_TYPE = :param2,
                      DELIVERY_DATE = TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM'),
                      CUSTOMER_ID = :param4, CUSTOMER_CODE = :param5,
                      MARKET_ID = :param6 , TERRITORY_ID = :param7 , AREA_ID= :param8 ,REGION_ID = :param9 ,DIVISION_ID = :param10, 
                      PAYMENT_MODE = :param11,
                      REPLACE_CLAIM_NO =  :param12,                
                      BONUS_PROCESS_NO =  :param13,                   
                      BONUS_CLAIM_NO =   :param14,                   
                      INVOICE_STATUS =  :param15,                   
                      ORDER_AMOUNT =  :param16,
                      ORDER_STATUS =  :param17,
                      ORDER_UNIT_ID   = :param18,                  
                      INVOICE_UNIT_ID =   :param19,               
                      REMARKS =    :param20,
                      UPDATED_BY = :param21, UPDATED_DATE = TO_DATE(:param22, 'DD/MM/YYYY HH:MI:SS AM'), 
                      UPDATED_TERMINAL = :param23 WHERE ORDER_MST_ID = :param1";

        string AddOrUpdate_Detail_UpdateQuery() => @"UPDATE ORDER_DTL SET   SKU_ID = :param2
                       ,SKU_CODE =:param3
                       ,ORDER_QTY = :param4
                       ,UNIT_TP = :param5
                       ,ORDER_AMOUNT = :param6
                       ,STATUS =  :param7
                       ,REMARKS = :param8          
                      
                       
                       ,UPDATED_BY = :param9, UPDATED_DATE = TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), 
                       UPDATED_TERMINAL = :param11 WHERE ORDER_DTL_ID = :param1";

        string GetNewOrder_MST_IdQuery() => "SELECT NVL(MAX(ORDER_MST_ID),0) + 1 ORDER_MST_ID  FROM ORDER_MST";

        string LoadData_Detail_Query() => @"Select  ROWNUM ROW_NO, ORDER_DTL_ID, ORDER_MST_ID, SKU_ID, SKU_CODE, ORDER_QTY, REVISED_ORDER_QTY, UNIT_TP,
 ORDER_AMOUNT, STATUS, COMPANY_ID, UNIT_ID, REMARKS, SPA_UNIT_TP, SPA_AMOUNT, SPA_REQ_TIME_STOCK,
  SPA_DISCOUNT_TYPE, SPA_DISCOUNT_VAL_PCT, SPA_CUST_COM, SPA_DISCOUNT_AMOUNT, SPA_TOTAL_AMOUNT from ORDER_DTL WHERE ORDER_MST_ID = :param1";
        string LoadData_DetailByMasterId_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.REGION_AREA_DTL_ID ASC) AS ROW_NO,M.REGION_AREA_DTL_ID,
                                          M.REGION_AREA_MST_ID, N.AREA_NAME,  M.AREA_ID,M.AREA_CODE, M.COMPANY_ID, M.REMARKS,
                                          M.REGION_AREA_DTL_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE  
                                          FROM REGION_AREA_DTL  M
                                          INNER JOIN AREA_INFO N ON N.AREA_ID = M.AREA_ID
                                          Where M.REGION_AREA_MST_ID = :param1";

             
        string GetNewOrder_Dtl_IdQuery() => "SELECT NVL(MAX(ORDER_DTL_ID),0) + 1 ORDER_DTL_ID  FROM ORDER_DTL";
        string GetNewOrder_MST_NoQuery() => "SELECT ORDER_NO +1  ORDER_NO FROM ORDER_MST where ORDER_MST_ID = (SELECT NVL(MAX(ORDER_MST_ID),0)  ORDER_MST_ID  FROM ORDER_MST WHERE COMPANY_ID = :param1)";

        string GetOrder_MST_BY_Id_Query() => @"SELECT ROWNUM ROW_NO, ORDER_MST_ID, ORDER_NO,TO_CHAR(ORDER_DATE, 'DD/MM/YYYY')  ORDER_DATE, ORDER_TYPE,TO_CHAR(DELIVERY_DATE, 'DD/MM/YYYY') DELIVERY_DATE, CUSTOMER_ID, CUSTOMER_CODE, MARKET_ID, TERRITORY_ID, AREA_ID, REGION_ID, DIVISION_ID, PAYMENT_MODE, REPLACE_CLAIM_NO, BONUS_PROCESS_NO, BONUS_CLAIM_NO, INVOICE_STATUS, ORDER_AMOUNT, ORDER_STATUS, COMPANY_ID, ORDER_UNIT_ID, INVOICE_UNIT_ID, REMARKS, SPA_TOTAL_AMOUNT, SPA_COMMISSION_PCT, SPA_COMMISSION_AMOUNT, SPA_NET_AMOUNT, ORDER_ENTRY_TYPE FROM ORDER_MST where ORDER_MST_ID = :param1";
        string GetOrder_DTL_BY_MstId_Query() => @"Select ROWNUM ROW_NO, ORDER_DTL_ID, ORDER_MST_ID, SKU_ID, SKU_CODE, ORDER_QTY, REVISED_ORDER_QTY, UNIT_TP, ORDER_AMOUNT, STATUS, COMPANY_ID, UNIT_ID, REMARKS, SPA_UNIT_TP, SPA_AMOUNT, SPA_REQ_TIME_STOCK, SPA_DISCOUNT_TYPE, SPA_DISCOUNT_VAL_PCT, SPA_CUST_COM, SPA_DISCOUNT_AMOUNT, SPA_TOTAL_AMOUNT from ORDER_DTL where ORDER_MST_ID= :param1";

        string Delete_Order_Dtl_by_Id() => @"Delete From ORDER_DTL where ORDER_DTL_ID = :param1";
        string Order_Ready_For_Invoice_Count_Query() => @"Select Count(ORDER_MST_ID) Order_Count from ORDER_MST Where ORDER_STATUS = 'Active' and COMPANY_ID = :param1 and ORDER_UNIT_ID  = :param2";

        string  Get_Customer_Balance_Query() => @"Select  CUSTOMER_ID, CUSTOMER_CODE, INVOICE_BALANCE, TDS_BALANCE from CUSTOMER_BALANCE WHERE CUSTOMER_ID = :param1 and UNIT_ID = :param2 and COMPANY_ID = :param3";

        string Get_Customer_Credit_Limit_Query() => @"Select * from Credit_INFO Where STATUS = 'Active' AND CUSTOMER_ID = :param1 and 
COMPANY_ID = :param2 and UNIT_ID = :param3 and 
TO_DATE(SYSDATE,'DD/MM/YYYY') between TO_DATE(EFFECT_START_DATE,'DD/MM/YYYY') and TO_DATE(EFFECT_END_DATE,'DD/MM/YYYY')";
        
        //---------- Method Execution Part ------------------------------------------------


        public async Task<string> AddOrUpdate(string db, Order_Mst model)
        {
            if (model == null)
            {
                return "No data provided to insert!!!!";
            }
            else
            {
                bool status_action = true;
                string dtl_list = "";

                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {
                    int detail_id = 0;
                    int detail_id_log = 0;


                    if (model.ORDER_MST_ID == 0)
                    {
                        //-------------Add to master table--------------------

                        model.ORDER_MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewOrder_MST_IdQuery(), _commonServices.AddParameter(new string[] { }));
                        model.ORDER_NO = _commonServices.GetMaximumNumber<string>(_configuration.GetConnectionString(db), GetNewOrder_MST_NoQuery(), _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString() })).ToString();
                        detail_id_log = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewOrder_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        
                        if (model.ORDER_NO == "")
                        {
                            model.ORDER_NO = "1";

                        }

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Master_AddQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.ORDER_MST_ID.ToString(), model.ORDER_NO.ToString(), model.ORDER_DATE, model.ORDER_TYPE,
                                model.DELIVERY_DATE, model.CUSTOMER_ID.ToString(), model.CUSTOMER_CODE, model.MARKET_ID.ToString(), 
                                model.TERRITORY_ID.ToString(), model.AREA_ID.ToString(), model.REGION_ID.ToString(),
                                model.DIVISION_ID.ToString(),model.PAYMENT_MODE,model.REPLACE_CLAIM_NO,model.BONUS_PROCESS_NO,
                                model.BONUS_CLAIM_NO,model.INVOICE_STATUS, model.ORDER_AMOUNT.ToString(), model.ORDER_STATUS
                            , model.COMPANY_ID.ToString(), model.ORDER_UNIT_ID.ToString(), model.ORDER_UNIT_ID.ToString(), model.REMARKS, model.SPA_TOTAL_AMOUNT.ToString()
                            , model.SPA_COMMISSION_PCT.ToString(), model.SPA_COMMISSION_AMOUNT.ToString(), model.SPA_NET_AMOUNT.ToString(), model.ENTERED_BY,
                             model.ENTERED_DATE, model.ENTERED_TERMINAL, model.ORDER_ENTRY_TYPE
                            })));
                        
                        //-------------Add to detail table--------------------

                        foreach (var item in model.Order_Dtls)
                        {
                            detail_id = 0;
                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(),
                                _commonServices.AddParameter(new string[] {
                                    detail_id.ToString(), model.ORDER_MST_ID.ToString(), item.SKU_ID.ToString(), item.SKU_CODE, item.ORDER_QTY.ToString(),
                                     item.UNIT_TP.ToString(), item.ORDER_AMOUNT.ToString(), item.STATUS, model.COMPANY_ID.ToString(),
                                    item.UNIT_ID.ToString(), item.REMARKS,item.SPA_UNIT_TP.ToString(),item.SPA_AMOUNT.ToString() ,
                                    item.SPA_REQ_TIME_STOCK.ToString(),item.SPA_DISCOUNT_TYPE.ToString(),item.SPA_DISCOUNT_VAL_PCT.ToString()
                                    ,item.SPA_CUST_COM.ToString(),item.SPA_DISCOUNT_AMOUNT.ToString(), item.SPA_TOTAL_AMOUNT.ToString(),
                                    model.ENTERED_BY,model.ENTERED_DATE,model.ENTERED_TERMINAL
                                })));
                            dtl_list += "," + ++detail_id_log;

                        }
                        status_action = false;

                    }
                    else
                    {
                        Order_Mst _Mst = JsonSerializer.Deserialize<Order_Mst>(await this.GetEditDatabyId(db, model.ORDER_MST_ID));
                       
                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_Master_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.ORDER_MST_ID.ToString(),
                                model.ORDER_TYPE.ToString(), model.DELIVERY_DATE, model.CUSTOMER_ID.ToString(), model.CUSTOMER_CODE
                               , model.MARKET_ID.ToString(), model.TERRITORY_ID.ToString(), model.AREA_ID.ToString(),
                                model.REGION_ID.ToString(), model.DIVISION_ID.ToString(),
                               model.PAYMENT_MODE, model.REPLACE_CLAIM_NO,
                               model.BONUS_PROCESS_NO, model.BONUS_CLAIM_NO,
                               model.INVOICE_STATUS, model.ORDER_AMOUNT.ToString(),
                               model.ORDER_STATUS, model.ORDER_UNIT_ID.ToString(),
                               model.INVOICE_UNIT_ID.ToString(), model.REMARKS,
                               model.UPDATED_BY, model.UPDATED_DATE,
                               model.UPDATED_TERMINAL.ToString(),

                               })));


                        foreach (var item in model.Order_Dtls)
                        {
                            if (item.ORDER_DTL_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------

                                detail_id = 0;
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(),
                                                               _commonServices.AddParameter(new string[] {
                                    detail_id.ToString(), model.ORDER_MST_ID.ToString(), item.SKU_ID.ToString(), item.SKU_CODE, item.ORDER_QTY.ToString(),
                                    item.UNIT_TP.ToString(), item.ORDER_AMOUNT.ToString(), item.STATUS, model.COMPANY_ID.ToString(),
                                    item.UNIT_ID.ToString(), item.REMARKS,item.SPA_UNIT_TP.ToString(),item.SPA_AMOUNT.ToString() ,
                                    item.SPA_REQ_TIME_STOCK.ToString(),item.SPA_DISCOUNT_TYPE.ToString(),item.SPA_DISCOUNT_VAL_PCT.ToString()
                                    ,item.SPA_CUST_COM.ToString(),item.SPA_DISCOUNT_AMOUNT.ToString(), item.SPA_TOTAL_AMOUNT.ToString(),
                                    model.ENTERED_BY,model.ENTERED_DATE,model.ENTERED_TERMINAL
                                  })));

                                dtl_list += "," + ++detail_id_log;

                            }
                            else
                            {
                                //-------------Edit on detail table--------------------
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_UpdateQuery(),
                                    _commonServices.AddParameter(new string[] { 
                                        item.ORDER_DTL_ID.ToString(), item.SKU_ID.ToString(),
                                        item.SKU_CODE, item.ORDER_QTY.ToString(),
                                        item.UNIT_TP.ToString(), item.ORDER_AMOUNT.ToString(), 
                                        item.STATUS,item.REMARKS,
                                        model.UPDATED_BY.ToString(), model.UPDATED_DATE, model.UPDATED_TERMINAL })));
                                dtl_list += "," + item.ORDER_DTL_ID;

                            }

                        }
                        foreach (var item in _Mst.Order_Dtls)
                        {
                            bool status = true;

                            foreach (var updateditem in model.Order_Dtls)
                            {
                                if (item.ORDER_DTL_ID == updateditem.ORDER_DTL_ID)
                                {
                                    status = false;
                                }
                            }
                            if (status)
                            {
                                //-------------Delete row from detail table--------------------

                                listOfQuery.Add(_commonServices.AddQuery(Delete_Order_Dtl_by_Id(), _commonServices.AddParameter(new string[] { item.ORDER_DTL_ID.ToString() })));
                                dtl_list += ",Deleted: " + item.ORDER_DTL_ID;

                            }

                        }


                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);

                    string st = status_action == false ? "Add" : "Edit";
                    await _logManager.AddOrUpdate(model.db_security, st, "ORDER_MST", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/SalesAndDistribution/SalesOrder/InsertOrEdit", model.ORDER_MST_ID, dtl_list);

                    //Notification Add---------------
                    var Pending_Invoice_Count = await _commonServices.GetMaximumNumberAsyn<int>(_configuration.GetConnectionString(model.db_sales), Order_Ready_For_Invoice_Count_Query(), _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.UNIT_ID.ToString() }));

                    await _NotificationManager.AddOrderNotification(model.db_security,model.db_sales, 1, "New Order " + model.ORDER_NO + " Has been added/Edited and ready for Invoice. Order Ready for Invoice Count: " + Pending_Invoice_Count, model.COMPANY_ID, model.UNIT_ID);

                    return _commonServices.Encrypt(model.ORDER_MST_ID.ToString());
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        public async Task<string> LoadData( Order_Mst model)
        {
            DataTable dataTable =  await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), LoadData_Query(), _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.UNIT_ID.ToString() }));
            List<Order_Mst> bonus_Msts = new List<Order_Mst>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Order_Mst _bonus_Mst = new Order_Mst()   ; 
                    _bonus_Mst.ROW_NO = Convert.ToInt32(dataTable.Rows[i]["ROW_NO"]);
                    _bonus_Mst.CUSTOMER_ID = Convert.ToInt32(dataTable.Rows[i]["CUSTOMER_ID"]);
                    _bonus_Mst.MARKET_ID = Convert.ToInt32(dataTable.Rows[i]["MARKET_ID"]);

                    _bonus_Mst.ORDER_MST_ID = Convert.ToInt32(dataTable.Rows[i]["ORDER_MST_ID"]);
                    _bonus_Mst.ORDER_NO = dataTable.Rows[i]["ORDER_NO"].ToString();

                    _bonus_Mst.ORDER_DATE = dataTable.Rows[i]["ORDER_DATE"].ToString();
                    _bonus_Mst.ORDER_TYPE = dataTable.Rows[i]["ORDER_TYPE"].ToString();
                    _bonus_Mst.DELIVERY_DATE = dataTable.Rows[i]["DELIVERY_DATE"].ToString();
                    _bonus_Mst.CUSTOMER_CODE = dataTable.Rows[i]["CUSTOMER_CODE"].ToString();
                    _bonus_Mst.MARKET_NAME = dataTable.Rows[i]["MARKET_NAME"].ToString();
                    _bonus_Mst.ORDER_ENTRY_TYPE = dataTable.Rows[i]["ORDER_ENTRY_TYPE"].ToString();
                    _bonus_Mst.CUSTOMER_NAME = dataTable.Rows[i]["CUSTOMER_NAME"].ToString();
                    _bonus_Mst.ORDER_AMOUNT = Convert.ToInt32(dataTable.Rows[i]["ORDER_AMOUNT"]);

                    _bonus_Mst.ORDER_MST_ID_ENCRYPTED = _commonServices.Encrypt(_bonus_Mst.ORDER_MST_ID.ToString());
                    bonus_Msts.Add(_bonus_Mst);
                }
            }
           
            await _logManager.AddOrUpdate(model.db_security, "View", "ORDER_MST", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/SalesAndDistribution/Division/DivisionInfo", 0, "");
          
            
         
            return JsonSerializer.Serialize(bonus_Msts);
        }
        public async Task<string> LoadFilteredData(OrderSKUFilterParameters model)
        {
            string Query = LoadFilteredData_Query();
            if(model.DIVISION_ID != null  && model.DIVISION_ID!="")
            {
                Query += string.Format("  AND O.DIVISION_ID = {0}", model.DIVISION_ID);
            } 
            if(model.REGION_ID != null && model.REGION_ID!="")
            {
                Query += string.Format("  AND O.REGION_ID = {0}", model.REGION_ID);

            }
            if (model.AREA_ID != null &&  model.AREA_ID != "")
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
                Query += string.Format("  AND  O.ORDER_DATE between TO_DATE('{0}','DD/MM/YYYY') AND TO_DATE('{1}','DD/MM/YYYY')", model.DATE_FROM,model.DATE_TO);

            }
            DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), Query, _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.UNIT_ID.ToString() }));
            List<Order_Mst> bonus_Msts = new List<Order_Mst>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Order_Mst _bonus_Mst = new Order_Mst();
                    _bonus_Mst.ROW_NO = Convert.ToInt32(dataTable.Rows[i]["ROW_NO"]);
                    _bonus_Mst.CUSTOMER_ID = Convert.ToInt32(dataTable.Rows[i]["CUSTOMER_ID"]);
                    _bonus_Mst.MARKET_ID = Convert.ToInt32(dataTable.Rows[i]["MARKET_ID"]);

                    _bonus_Mst.ORDER_MST_ID = Convert.ToInt32(dataTable.Rows[i]["ORDER_MST_ID"]);
                    _bonus_Mst.ORDER_NO = dataTable.Rows[i]["ORDER_NO"].ToString();
                    _bonus_Mst.ORDER_STATUS = dataTable.Rows[i]["ORDER_STATUS"].ToString();

                    _bonus_Mst.ORDER_DATE = dataTable.Rows[i]["ORDER_DATE"].ToString();
                    _bonus_Mst.ORDER_TYPE = dataTable.Rows[i]["ORDER_TYPE"].ToString();
                    _bonus_Mst.DELIVERY_DATE = dataTable.Rows[i]["DELIVERY_DATE"].ToString();
                    _bonus_Mst.CUSTOMER_CODE = dataTable.Rows[i]["CUSTOMER_CODE"].ToString();
                    _bonus_Mst.MARKET_NAME = dataTable.Rows[i]["MARKET_NAME"].ToString();
                    _bonus_Mst.ORDER_ENTRY_TYPE = dataTable.Rows[i]["ORDER_ENTRY_TYPE"].ToString();
                    _bonus_Mst.CUSTOMER_NAME = dataTable.Rows[i]["CUSTOMER_NAME"].ToString();
                    _bonus_Mst.ORDER_AMOUNT = Convert.ToInt32(dataTable.Rows[i]["ORDER_AMOUNT"]);

                    _bonus_Mst.ORDER_MST_ID_ENCRYPTED = _commonServices.Encrypt(_bonus_Mst.ORDER_MST_ID.ToString());
                    bonus_Msts.Add(_bonus_Mst);
                }
            }

            await _logManager.AddOrUpdate(model.db_security, "View", "ORDER_MST", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/SalesAndDistribution/Division/DivisionInfo", 0, "");



            return JsonSerializer.Serialize(bonus_Msts);
        }

        public async Task<decimal> LoadProductPrice(string db, string Company_Id, int SKU_ID, string Order_Type,int Customer_Id) 
            =>await _commonServices.GetMaximumNumberAsyn<decimal>(_configuration.GetConnectionString(db), LoadProductPrice_Query(), _commonServices.AddParameter(new string[] { Order_Type, Customer_Id.ToString(), SKU_ID.ToString() })); 

        public async Task<string> LoadCustomer(string db, string Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadCustomerData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> LoadProducts(string db, string Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadProducts_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> GetEditDatabyId(string db, int id)
        {
            DataTable data_master = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetOrder_MST_BY_Id_Query(), _commonServices.AddParameter(new string[] { id.ToString() }));
            DataTable data_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetOrder_DTL_BY_MstId_Query(), _commonServices.AddParameter(new string[] { id.ToString() }));

            Order_Mst order_Mst = new Order_Mst();
            if(data_master.Rows.Count>0)
            {
                order_Mst.COMPANY_ID = Convert.ToInt32(data_master.Rows[0]["COMPANY_ID"]);
                order_Mst.ROW_NO = Convert.ToInt32(data_master.Rows[0]["ROW_NO"]);
                order_Mst.ORDER_MST_ID = Convert.ToInt32(data_master.Rows[0]["ORDER_MST_ID"]);
                order_Mst.ORDER_NO = data_master.Rows[0]["ORDER_NO"].ToString();
                order_Mst.ORDER_DATE = data_master.Rows[0]["ORDER_DATE"].ToString();
                order_Mst.ORDER_TYPE = data_master.Rows[0]["ORDER_TYPE"].ToString();
                order_Mst.DELIVERY_DATE = data_master.Rows[0]["DELIVERY_DATE"].ToString();
                order_Mst.CUSTOMER_ID = Convert.ToInt32(data_master.Rows[0]["CUSTOMER_ID"]);
                order_Mst.CUSTOMER_CODE = data_master.Rows[0]["CUSTOMER_CODE"].ToString();
               order_Mst.PAYMENT_MODE = data_master.Rows[0]["PAYMENT_MODE"].ToString();
               order_Mst.ORDER_ENTRY_TYPE = data_master.Rows[0]["ORDER_ENTRY_TYPE"]!=null? data_master.Rows[0]["ORDER_ENTRY_TYPE"].ToString(): "Menual";
               order_Mst.REMARKS = data_master.Rows[0]["REMARKS"]!=null? data_master.Rows[0]["REMARKS"].ToString() : "";
               order_Mst.ORDER_STATUS = data_master.Rows[0]["ORDER_STATUS"].ToString();
               order_Mst.AREA_ID = Convert.ToInt32(data_master.Rows[0]["AREA_ID"]);
               order_Mst.MARKET_ID = Convert.ToInt32(data_master.Rows[0]["MARKET_ID"]);
               order_Mst.DIVISION_ID = Convert.ToInt32(data_master.Rows[0]["DIVISION_ID"]);
               order_Mst.TERRITORY_ID = Convert.ToInt32(data_master.Rows[0]["TERRITORY_ID"]);
               order_Mst.REGION_ID = Convert.ToInt32(data_master.Rows[0]["REGION_ID"]);
               order_Mst.ORDER_UNIT_ID = Convert.ToInt32(data_master.Rows[0]["ORDER_UNIT_ID"]);
               order_Mst.INVOICE_UNIT_ID = Convert.ToInt32(data_master.Rows[0]["INVOICE_UNIT_ID"]);
               order_Mst.REPLACE_CLAIM_NO =data_master.Rows[0]["REPLACE_CLAIM_NO"]!=null?  data_master.Rows[0]["REPLACE_CLAIM_NO"].ToString() : "";
               order_Mst.BONUS_PROCESS_NO = data_master.Rows[0]["BONUS_PROCESS_NO"]!=null? data_master.Rows[0]["BONUS_PROCESS_NO"].ToString() : "";
               order_Mst.BONUS_CLAIM_NO = data_master.Rows[0]["BONUS_CLAIM_NO"]!=null? data_master.Rows[0]["BONUS_CLAIM_NO"].ToString(): "";
               order_Mst.INVOICE_STATUS = data_master.Rows[0]["INVOICE_STATUS"].ToString();
            order_Mst.ORDER_AMOUNT = Convert.ToDecimal(data_master.Rows[0]["ORDER_AMOUNT"].ToString());
            order_Mst.SPA_TOTAL_AMOUNT = Convert.ToDecimal(data_master.Rows[0]["SPA_TOTAL_AMOUNT"].ToString());
            order_Mst.SPA_COMMISSION_PCT = Convert.ToDecimal(data_master.Rows[0]["SPA_COMMISSION_PCT"].ToString());
            order_Mst.SPA_COMMISSION_AMOUNT = Convert.ToDecimal(data_master.Rows[0]["SPA_COMMISSION_AMOUNT"].ToString());
            order_Mst.SPA_NET_AMOUNT = Convert.ToDecimal(data_master.Rows[0]["SPA_NET_AMOUNT"].ToString());

                order_Mst.ORDER_MST_ID = Convert.ToInt32(data_master.Rows[0]["ORDER_MST_ID"]);
                if (data_detail.Rows.Count > 0)
                {
                    order_Mst.Order_Dtls = new List<Order_Dtl>();
                    for (int i= 0;i<data_detail.Rows.Count;i++)
                    {
                           Order_Dtl order_Dtl = new Order_Dtl();
                        order_Dtl.COMPANY_ID = Convert.ToInt32(data_detail.Rows[i]["COMPANY_ID"]);
                        order_Dtl.ROW_NO = Convert.ToInt32(data_detail.Rows[i]["ROW_NO"]);
                        order_Dtl.ORDER_MST_ID = Convert.ToInt32(data_detail.Rows[i]["ORDER_MST_ID"]);
                        order_Dtl.ORDER_DTL_ID = Convert.ToInt32(data_detail.Rows[i]["ORDER_DTL_ID"]);

                        order_Dtl.SKU_ID = Convert.ToInt32(data_detail.Rows[i]["SKU_ID"]);
                        order_Dtl.SKU_CODE = data_detail.Rows[i]["SKU_CODE"] != null ? data_detail.Rows[i]["SKU_CODE"].ToString() : "";
                        order_Dtl.ORDER_QTY = data_detail.Rows[i]["ORDER_QTY"] != null ? Convert.ToInt32(data_detail.Rows[i]["ORDER_QTY"]) : 0;
                        //order_Dtl.REVISED_ORDER_QTY = data_detail.Rows[i]["REVISED_ORDER_QTY"] != null ? Convert.ToInt32(data_detail.Rows[i]["REVISED_ORDER_QTY"]) : 0;
                        order_Dtl.UNIT_TP = data_detail.Rows[i]["UNIT_TP"] != null ? Convert.ToDecimal(data_detail.Rows[i]["UNIT_TP"]) : 0;
                        order_Dtl.ORDER_AMOUNT = data_detail.Rows[i]["ORDER_AMOUNT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["ORDER_AMOUNT"]) : 0;
                        order_Dtl.STATUS = data_detail.Rows[i]["STATUS"] != null ? data_detail.Rows[i]["STATUS"].ToString() : "";
                        order_Dtl.REMARKS = data_detail.Rows[i]["REMARKS"] != null ? data_detail.Rows[i]["REMARKS"].ToString() : "";
                        order_Dtl.SPA_UNIT_TP = data_detail.Rows[i]["SPA_UNIT_TP"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_UNIT_TP"]) : 0;
                        order_Dtl.SPA_AMOUNT = data_detail.Rows[i]["SPA_AMOUNT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_AMOUNT"]) : 0;
                        order_Dtl.SPA_DISCOUNT_AMOUNT = data_detail.Rows[i]["SPA_DISCOUNT_AMOUNT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_DISCOUNT_AMOUNT"]) : 0;
                        order_Dtl.SPA_REQ_TIME_STOCK = data_detail.Rows[i]["SPA_REQ_TIME_STOCK"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_REQ_TIME_STOCK"]) : 0;
                        order_Dtl.SPA_CUST_COM = data_detail.Rows[i]["SPA_CUST_COM"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_CUST_COM"]) : 0;
                        order_Dtl.SPA_DISCOUNT_VAL_PCT = data_detail.Rows[i]["SPA_DISCOUNT_VAL_PCT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_DISCOUNT_VAL_PCT"]) : 0;
                        order_Dtl.SPA_DISCOUNT_TYPE = data_detail.Rows[i]["SPA_DISCOUNT_TYPE"] != null ? data_detail.Rows[i]["SPA_DISCOUNT_TYPE"].ToString() : "";
                        order_Dtl.SPA_TOTAL_AMOUNT = data_detail.Rows[i]["SPA_TOTAL_AMOUNT"] != null ? Convert.ToDecimal(data_detail.Rows[i]["SPA_TOTAL_AMOUNT"]) : 0;

                        
                        order_Dtl.UNIT_ID = Convert.ToInt32(data_detail.Rows[i]["UNIT_ID"]);
                        order_Mst.Order_Dtls.Add(order_Dtl);
                    }
                }
            }
            
            return JsonSerializer.Serialize(order_Mst);


        }
        public async Task<string> Get_Customer_Balance(string db, string Company_Id,string Unit_Id,string Customer_Id)
        {
            DataTable Balance = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_Customer_Balance_Query(), _commonServices.AddParameter(new string[] { Customer_Id, Unit_Id, Company_Id }));
            DataTable Credit_limit = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_Customer_Credit_Limit_Query(), _commonServices.AddParameter(new string[] { Customer_Id, Company_Id, Unit_Id,  }));
            DataSet Balance_Credit_Limit = new DataSet();
            Balance_Credit_Limit.Tables.Add(Balance);
            Balance_Credit_Limit.Tables.Add(Credit_limit);
            return _commonServices.DataSetToJSON(Balance_Credit_Limit);

        }

    }
}
