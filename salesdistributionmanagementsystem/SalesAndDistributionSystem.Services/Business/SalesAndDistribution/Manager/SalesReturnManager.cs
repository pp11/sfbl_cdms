using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Company;
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
    public class SalesReturnManager : ISalesReturnManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        private readonly IUserLogManager _logManager;
        private readonly INotificationManager _NotificationManager;
        public SalesReturnManager(ICommonServices commonServices, IConfiguration configuration, IUserLogManager logManager, INotificationManager NotificationManager)
        {
            _commonServices = commonServices;
            _configuration = configuration;
            _logManager = logManager;
            _NotificationManager = NotificationManager;

        }

        //-------------------Query Part ---------------------------------------------------


        string  LoadSalesReturnMst_Query() => @"Select ROWNUM ROW_NO, MST_ID, RETURN_NO,TO_CHAR(RETURN_DATE,'dd/MM/yyyy HH:MI:SS AM') RETURN_DATE,
               RETURN_UNIT_ID, RETURN_TYPE, INVOICE_NO,TO_CHAR(INVOICE_DATE,'dd/MM/yyyy HH:MI:SS AM') INVOICE_DATE,C.CUSTOMER_NAME,
               NET_INVOICE_AMOUNT, R.CUSTOMER_ID, R.CUSTOMER_CODE, MARKET_ID, TP_AMOUNT, VAT_AMOUNT, TOTAL_AMOUNT, CUSTOMER_DISC_AMOUNT, 
               CUSTOMER_ADD1_DISC_AMOUNT, CUSTOMER_ADD2_DISC_AMOUNT,
               CUSTOMER_PRODUCT_DISC_AMOUNT, BONUS_PRICE_DISC_AMOUNT,
               PROD_BONUS_PRICE_DISC_AMOUNT, LOADING_CHARGE_AMOUNT, 
               RETURN_ADJUSTMENT_AMOUNT, RETURN_DISCOUNT_AMOUNT, 
               NET_RETURN_AMOUNT, TDS_AMOUNT, RETURN_VERSION, 
               REMARKS, RETURN_STATUS, R.COMPANY_ID from Return_MST R
               Left outer join CUSTOMER_INFO C on C.CUSTOMER_ID = R.CUSTOMER_ID
               WHERE R.COMPANY_ID =  :param1 and RETURN_UNIT_ID = :param2";

        string LoadSalesReturnMst_Query_ByCustomerCode() => @"Select ROWNUM ROW_NO, MST_ID, RETURN_NO,TO_CHAR(RETURN_DATE,'dd/MM/yyyy HH:MI:SS AM') RETURN_DATE,
               RETURN_UNIT_ID, RETURN_TYPE, INVOICE_NO,TO_CHAR(INVOICE_DATE,'dd/MM/yyyy HH:MI:SS AM') INVOICE_DATE,C.CUSTOMER_NAME,
               NET_INVOICE_AMOUNT, R.CUSTOMER_ID, R.CUSTOMER_CODE, MARKET_ID, TP_AMOUNT, VAT_AMOUNT, TOTAL_AMOUNT, CUSTOMER_DISC_AMOUNT, 
               CUSTOMER_ADD1_DISC_AMOUNT, CUSTOMER_ADD2_DISC_AMOUNT,
               CUSTOMER_PRODUCT_DISC_AMOUNT, BONUS_PRICE_DISC_AMOUNT,
               PROD_BONUS_PRICE_DISC_AMOUNT, LOADING_CHARGE_AMOUNT, 
               RETURN_ADJUSTMENT_AMOUNT, RETURN_DISCOUNT_AMOUNT, 
               NET_RETURN_AMOUNT, TDS_AMOUNT, RETURN_VERSION, 
               REMARKS, RETURN_STATUS, R.COMPANY_ID from Return_MST R
               Left outer join CUSTOMER_INFO C on C.CUSTOMER_ID = R.CUSTOMER_ID
               WHERE R.COMPANY_ID =  :param1 and R.CUSTOMER_CODE = :param2";

        string LoadSalesReturnMstById_Query() => @"Select ROWNUM ROW_NO, MST_ID, RETURN_NO,TO_CHAR(RETURN_DATE,'dd/MM/yyyy HH:MI:SS AM') RETURN_DATE,
               RETURN_UNIT_ID, RETURN_TYPE, INVOICE_NO,TO_CHAR(INVOICE_DATE,'dd/MM/yyyy HH:MI:SS AM') INVOICE_DATE,
               NET_INVOICE_AMOUNT, R.CUSTOMER_ID, R.CUSTOMER_CODE, MARKET_ID, TP_AMOUNT, VAT_AMOUNT, TOTAL_AMOUNT, CUSTOMER_DISC_AMOUNT,C.CUSTOMER_NAME, 
               CUSTOMER_ADD1_DISC_AMOUNT, CUSTOMER_ADD2_DISC_AMOUNT,
               CUSTOMER_PRODUCT_DISC_AMOUNT, BONUS_PRICE_DISC_AMOUNT,
               PROD_BONUS_PRICE_DISC_AMOUNT, LOADING_CHARGE_AMOUNT, 
               RETURN_ADJUSTMENT_AMOUNT, RETURN_DISCOUNT_AMOUNT, 
               NET_RETURN_AMOUNT, TDS_AMOUNT, RETURN_VERSION, 
               REMARKS, RETURN_STATUS, R.COMPANY_ID from Return_MST R
               Left outer join CUSTOMER_INFO C on C.CUSTOMER_ID = R.CUSTOMER_ID
               WHERE MST_ID =  :param1";
        string SalesReturnPartialProcess_Query() => @"begin PRC_PARTIAL_RETURN_PRODUCT(:param1, :param2, :param3, :param4); end;";
        string SalesReturnPartial_Query() => @"select FN_PROCESS_RETURN(:param1, :param2,:param3 , :param4 ) From Dual";

        string FullReturn_Query() => @"begin PRC_FULL_RETURN(:param1, TO_DATE(:param2, 'DD/MM/YYYY HH:MI:SS PM'), :param3, :param4, :param5, :param6); end;";


        string SalesReturnDetails_Detail() => @"Select ROWNUM ROW_NO,  DTL_ID, MST_ID, D.SKU_ID, D.SKU_CODE, UNIT_TP, UNIT_VAT,
                                                SUPPLIMENTARY_TAX, INVOICE_QTY, RETURN_QTY, TP_AMOUNT, VAT_AMOUNT,
                                               TOTAL_AMOUNT, CUSTOMER_DISC_AMOUNT, CUSTOMER_ADD1_DISC_AMOUNT, 
                                               CUSTOMER_ADD2_DISC_AMOUNT, CUSTOMER_PRODUCT_DISC_AMOUNT, BONUS_PRICE_DISC_AMOUNT, 
                                                PROD_BONUS_PRICE_DISC_AMOUNT, LOADING_CHARGE_AMOUNT, RETURN_ADJUSTMENT_AMOUNT, 
                                                RETURN_DISCOUNT_AMOUNT, NET_PRODUCT_AMOUNT, D.COMPANY_ID, P.SKU_NAME
                                                from RETURN_DTL D
                                                LEFT OUTER JOIN PRODUCT_INFO P on P.SKU_ID = D.SKU_ID
                                                Where MST_ID = :param1";

        string SalesReturnDetails_Bonus() => @"Select ROWNUM ROW_NO,  BONUS_ID, DTL_ID, RETURN_DATE, RETURN_UNIT_ID, SKU_ID, SKU_CODE, UNIT_TP, BATCH_ID, BATCH_NO, RETURN_BONUS_QTY, RETURN_BONUS_AMOUNT, COMPANY_ID
                                              from RETURN_Bonus";
        string SalesReturnDetails_Issue() => @"Select ROWNUM ROW_NO,  ISSUE_ID, DTL_ID, RETURN_DATE, RETURN_UNIT_ID, D.SKU_ID,  D.SKU_CODE, UNIT_TP,
                                              BATCH_ID, BATCH_NO, RETURN_QTY, RETURN_AMOUNT, D.COMPANY_ID, P.SKU_NAME
                                              from RETURN_ISSUE D
                                              LEFT OUTER JOIN PRODUCT_INFO P on P.SKU_ID = D.SKU_ID";
        string SalesReturnDetails_Gift() => @"Select ROWNUM ROW_NO,  D.GIFT_ID, DTL_ID, RETURN_DATE, RETURN_UNIT_ID, D.GIFT_ITEM_ID, D.UNIT_TP,
                                              BATCH_ID, BATCH_NO, GIFT_RETURN_QTY, GIFT_RETURN_AMOUNT, D.COMPANY_ID
                                              from RETURN_GIFT D
                                              LEFT OUTER JOIN GIFT_ITEM_INFO P on P.GIFT_ITEM_ID = D.GIFT_ID";
        string SalesReturnDetails_ComboBonus() => @"Select ROWNUM ROW_NO, COMBO_BONUS_ID, MST_ID, RETURN_DATE, RETURN_UNIT_ID, SKU_ID, SKU_CODE, UNIT_TP, BATCH_ID, BATCH_NO, RETURN_BONUS_QTY, RETURN_BONUS_AMOUNT, COMPANY_ID
                                                from RETURN_COMBO_BONUS Where MST_ID = :param1";
        string SalesReturnDetails_ComboGift() => @"Select ROWNUM ROW_NO, COMBO_GIFT_ID, MST_ID, RETURN_DATE, RETURN_UNIT_ID, GIFT_ITEM_ID, UNIT_TP, BATCH_ID, BATCH_NO, GIFT_RETURN_QTY, GIFT_RETURN_AMOUNT, COMPANY_ID
                                               from RETURN_COMBO_GIFT Where MST_ID = :param1";
        string ReturnDelete_Query() => @" begin
                           PRC_RETURN_DELETE(:param1);
                      end;";


        string ReturnDelete_Temp() => @"Delete
                                        from TT_RETURN_DTL Where INVOICE_NO = :param1 AND SKU_ID = :param2";
        string SalesReturnInvoiceLoad() => @"Select  I.MST_ID,  I.INVOICE_NO,  I.INVOICE_DATE,  I.INVOICE_STATUS from INVOICE_MST I 
 WHERE I.COMPANY_ID = :param1 and I.INVOICE_UNIT_ID = :param2 and I.INVOICE_NO NOT IN (Select INVOICE_NO from RETURN_MST) ";
        string SalesReturnInvoicesLoadByCustomerCode() => @"Select  I.MST_ID,  I.INVOICE_NO,  I.INVOICE_DATE,  I.INVOICE_STATUS from INVOICE_MST I 
 WHERE I.COMPANY_ID = :param1 and I.CUSTOMER_CODE = :param2 and I.INVOICE_NO NOT IN (Select INVOICE_NO from RETURN_MST)";

        //---------- Method Execution Part ------------------------------------------------





        public async Task<string> DeleteReturn(string db, Return_Mst model)
        {
            string result = await _commonServices.ProcedureCallAsyn<string>(_configuration.GetConnectionString(db), ReturnDelete_Query(), _commonServices.AddParameter(new string[] { model.INVOICE_NO }));
            await _logManager.AddOrUpdate(model.db_security, "Delete Return", "RETURN_MST", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/SalesAndDistribution/Return/InsertOrUpdate", 0, "Invoice No:"+ model.INVOICE_NO);
            await _NotificationManager.AddOrderNotification(model.db_security, model.db_sales, 2, "A Return is Deleted of  Invoice No: " + model.INVOICE_NO + ". ", model.COMPANY_ID, model.UNIT_ID);

            result = "1";
            return result;
        }
        public async Task<string> FullReturn(string db,SalesReturnParameters model)
        {
            DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT *FROM DEPOT_CUSTOMER_DIST_INVOICE WHERE INVOICE_NO= :param1", _commonServices.AddParameter(new string[] { model.INVOICE_NO }));
            if (dataTable.Rows.Count > 0)
            {
                return "You cannot return this invoice because it has been dispatched";
            }
            await _logManager.AddOrUpdate(model.db_security, "Full Return", "RETURN_MST", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/SalesAndDistribution/Return/InsertOrUpdate", 0, "Retrun No:" + model.INVOICE_NO);
            await _NotificationManager.AddOrderNotification(model.db_security, model.db_sales, 2, "A Full Return For Invoice " + model.INVOICE_NO + " Has been Generated. ", model.COMPANY_ID, model.UNIT_ID);
            return await _commonServices.ProcedureCallAsyn<string>(_configuration.GetConnectionString(db), FullReturn_Query(),_commonServices.AddParameter(new string[] { model.INVOICE_NO, model.INVOICE_DATE, model.COMPANY_ID.ToString(), model.RETURN_UNIT_ID.ToString(), model.CUSTOMER_ID.ToString(), model.CUSTOMER_CODE }));
        }

        public async Task<string> LoadSalesReturns(OrderSKUFilterParameters model)
        {
            string Query = "";

           if (model.IsDistributor == "True")
            {
                Query = LoadSalesReturnMst_Query_ByCustomerCode();
            }
            else
            {
                 Query = LoadSalesReturnMst_Query();

            }

            if (model.CUSTOMER_ID != null && model.CUSTOMER_ID != "")
            {
                Query += string.Format("  AND R.CUSTOMER_ID = {0}", model.CUSTOMER_ID);

            }
            if (model.ORDER_TYPE != null && model.ORDER_TYPE != "")
            {
                Query += string.Format("  AND RETURN_TYPE = '{0}'", model.ORDER_TYPE);

            }


            if ((model.DATE_FROM != null && model.DATE_FROM != "") && (model.DATE_TO != null || model.DATE_TO != ""))
            {
                Query += string.Format("  AND  RETURN_DATE between TO_DATE('{0}','DD/MM/YYYY HH:MI:SS AM') AND TO_DATE('{1}','DD/MM/YYYY HH:MI:SS AM')+1", model.DATE_FROM, model.DATE_TO);
            }
            DataTable dataTable = new DataTable();
            if (model.IsDistributor == "True")
            {
                Query = LoadSalesReturnMst_Query_ByCustomerCode();
                dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), Query,
               _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.CUSTOMER_CODE.ToString() }));
            }
            else
            {
                Query = LoadSalesReturnMst_Query();
                dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), Query,
               _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.UNIT_ID.ToString() }));

            }

             

            List<Return_Mst> bonus_Msts = new List<Return_Mst>();

            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Return_Mst _bonus_Mst = new Return_Mst
                    {
                        ROW_NO = Convert.ToInt32(dataTable.Rows[i]["ROW_NO"]),
                        CUSTOMER_NAME = dataTable.Rows[i]["CUSTOMER_NAME"].ToString(),
                        INVOICE_NO = dataTable.Rows[i]["INVOICE_NO"].ToString(),
                        INVOICE_DATE = dataTable.Rows[i]["INVOICE_DATE"].ToString(),
                        MST_ID = Convert.ToInt32(dataTable.Rows[i]["MST_ID"]),
                        CUSTOMER_CODE = dataTable.Rows[i]["CUSTOMER_CODE"].ToString(),
                        RETURN_TYPE = dataTable.Rows[i]["RETURN_TYPE"].ToString(),
                        RETURN_STATUS = dataTable.Rows[i]["RETURN_STATUS"].ToString(),
                        RETURN_DATE = dataTable.Rows[i]["RETURN_DATE"].ToString(),
                        RETURN_NO = dataTable.Rows[i]["RETURN_NO"].ToString(),
                        REMARKS = dataTable.Rows[i]["REMARKS"] != null && dataTable.Rows[i]["REMARKS"].ToString() != "" ? dataTable.Rows[i]["REMARKS"].ToString() : "",
                        NET_RETURN_AMOUNT = dataTable.Rows[i]["NET_RETURN_AMOUNT"] != null && dataTable.Rows[i]["NET_RETURN_AMOUNT"].ToString() != "" ? Convert.ToDecimal(dataTable.Rows[i]["NET_RETURN_AMOUNT"]):0,
                        NET_INVOICE_AMOUNT = dataTable.Rows[i]["NET_INVOICE_AMOUNT"] != null && dataTable.Rows[i]["NET_INVOICE_AMOUNT"].ToString() != "" ? Convert.ToDecimal(dataTable.Rows[i]["NET_INVOICE_AMOUNT"]) :0 ,
                        TOTAL_AMOUNT = dataTable.Rows[i]["TOTAL_AMOUNT"] != null && dataTable.Rows[i]["TOTAL_AMOUNT"].ToString() != "" ? Convert.ToDecimal(dataTable.Rows[i]["TOTAL_AMOUNT"]) : 0

                    };

                    _bonus_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(_bonus_Mst.MST_ID.ToString());
                    bonus_Msts.Add(_bonus_Mst);
                }
            }
            return JsonSerializer.Serialize(bonus_Msts);



        }

        public async Task<string> LoadSalesReturnMst(string db,  string Company_Id, string Invoice_Unit_id)
        {
            DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSalesReturnMst_Query(),
                _commonServices.AddParameter(new string[] { Company_Id, Invoice_Unit_id }));
            List<Return_Mst> bonus_Msts = new List<Return_Mst>();

            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Return_Mst _bonus_Mst = new Return_Mst
                    {
                        ROW_NO = Convert.ToInt32(dataTable.Rows[i]["ROW_NO"]),
                        CUSTOMER_NAME = dataTable.Rows[i]["CUSTOMER_NAME"].ToString(),
                        INVOICE_NO = dataTable.Rows[i]["INVOICE_NO"].ToString(),
                        INVOICE_DATE = dataTable.Rows[i]["INVOICE_DATE"].ToString(),
                        MST_ID = Convert.ToInt32(dataTable.Rows[i]["MST_ID"]),
                        CUSTOMER_CODE = dataTable.Rows[i]["CUSTOMER_CODE"].ToString(),
                        RETURN_TYPE = dataTable.Rows[i]["RETURN_TYPE"].ToString(),
                        RETURN_STATUS = dataTable.Rows[i]["RETURN_STATUS"].ToString(),
                        RETURN_DATE = dataTable.Rows[i]["RETURN_DATE"].ToString(),
                        RETURN_NO = dataTable.Rows[i]["RETURN_NO"].ToString(),
                        REMARKS = dataTable.Rows[i]["REMARKS"].ToString(),
                        NET_RETURN_AMOUNT = Convert.ToDecimal(dataTable.Rows[i]["NET_RETURN_AMOUNT"]),
                        NET_INVOICE_AMOUNT = Convert.ToDecimal(dataTable.Rows[i]["NET_INVOICE_AMOUNT"]),
                        TOTAL_AMOUNT = dataTable.Rows[i]["TOTAL_AMOUNT"] != null ? Convert.ToDecimal(dataTable.Rows[i]["TOTAL_AMOUNT"]) : Convert.ToDecimal(dataTable.Rows[i]["NET_RETURN_AMOUNT"]) 

                    };

                    _bonus_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(_bonus_Mst.MST_ID.ToString());
                    bonus_Msts.Add(_bonus_Mst);
                }
            }
            return JsonSerializer.Serialize(bonus_Msts);



        }
        public async Task<string> LoadReturnDetailsData(string db, string Mst_Id)
        {

            DataSet ds = new DataSet();
            DataTable _ReturnMst = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSalesReturnMstById_Query(), _commonServices.AddParameter(new string[] { Mst_Id }));
            DataTable _ReturnDtl = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), SalesReturnDetails_Detail(), _commonServices.AddParameter(new string[] { Mst_Id }));
            DataTable _ReturnComboGifts = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), SalesReturnDetails_ComboGift(), _commonServices.AddParameter(new string[] { Mst_Id }));
            DataTable _ReturnComboBonus = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), SalesReturnDetails_ComboBonus(), _commonServices.AddParameter(new string[] { Mst_Id }));

            List<int> InvDtl_Ids = new List<int>();
            string Dtl_Ids = "";
            foreach (DataRow row in _ReturnDtl.Rows)
            {

                int dtl_id = Convert.ToInt32(row["DTL_ID"]);
                int st = InvDtl_Ids.Where(x => x == dtl_id).FirstOrDefault();
                if (st < 1)
                {
                    InvDtl_Ids.Add(dtl_id);
                }

            }

            for (int i = 0; i < InvDtl_Ids.Count; i++)
            {
                if (i == 0)
                {
                    Dtl_Ids = InvDtl_Ids[i].ToString();
                }
                else
                {
                    Dtl_Ids = Dtl_Ids + "," + InvDtl_Ids[i].ToString();

                }


            }

            string inv_gift = SalesReturnDetails_Gift() + " WHERE DTL_ID in (" + Dtl_Ids + ")";
            string inv_bonus = SalesReturnDetails_Bonus() + " WHERE DTL_ID in (" + Dtl_Ids + ")";
            string inv_issue = SalesReturnDetails_Issue() + " WHERE DTL_ID in (" + Dtl_Ids + ")";

            DataTable _returnGifts = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), inv_gift, _commonServices.AddParameter(new string[] { }));

            DataTable _returnBonus = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), inv_bonus, _commonServices.AddParameter(new string[] { }));
            DataTable _returnIssue = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), inv_issue, _commonServices.AddParameter(new string[] { }));

            ds.Tables.Add(_ReturnMst);
            ds.Tables.Add(_ReturnDtl);
            ds.Tables.Add(_ReturnComboGifts);
            ds.Tables.Add(_ReturnComboBonus);
            ds.Tables.Add(_returnGifts);
            ds.Tables.Add(_returnBonus);
            ds.Tables.Add(_returnIssue);

            return _commonServices.DataSetToJSON(ds);

        }
        public async Task<string> ProcessPartialReturn(string db, Process_data model)
        {
              List<QueryPattern> listOfQuery = new List<QueryPattern>();

               listOfQuery.Add(_commonServices.AddQuery(ReturnDelete_Temp(), _commonServices.AddParameter(new string[] { model.INVOICE_NO, model.SKU_ID.ToString(),}))); ;

                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);

                await _logManager.AddOrUpdate(model.db_security, "Process Partial Return", "RETURN_MST", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/SalesAndDistribution/Return/InsertOrUpdate", 0, "Retrun No:" + model.INVOICE_NO + "Sku ID:"+model.SKU_ID);

            return await _commonServices.PreExecuteProcedureCallAsyn<string>(_configuration.GetConnectionString(db), SalesReturnPartialProcess_Query(), _commonServices.AddParameter(new string[] { model.INVOICE_NO, model.SKU_ID.ToString(), model.SKU_CODE,model.RETURN_QTY.ToString() }));
        }

        public async Task<string> SalesReturnPartial(string db, List<Process_data> model)
        {
            DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT *FROM DEPOT_CUSTOMER_DIST_INVOICE WHERE INVOICE_NO= :param1", _commonServices.AddParameter(new string[] { model[0].INVOICE_NO }));
            if (dataTable.Rows.Count > 0)
            {
                return "You cannot return this invoice because it has been dispatched";
            }


            string result = model[0].INVOICE_NO + " Can not be returned!!";
            await _logManager.AddOrUpdate(model[0].db_security, "Partial Return", "RETURN_MST", model[0].COMPANY_ID, model[0].UNIT_ID, Convert.ToInt32(model[0].ENTERED_BY), model[0].ENTERED_TERMINAL, "/SalesAndDistribution/Return/InsertOrUpdate", 0, "Retrun No:" + model[0].INVOICE_NO);
            await _NotificationManager.AddOrderNotification(model[0].db_security, model[0].db_sales, 2, "A Partial Return For Invoice " + model[0].INVOICE_NO + " Has been Generated. ", model[0].COMPANY_ID, model[0].UNIT_ID);
            string process = "";
            for(int i=0;i<model.Count;i++)
            {
                process = process + model[i].SKU_CODE+ "," + model[i].RETURN_QTY.ToString() + ";";
            }


            DataTable ckhDt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT   ROUND(M.NET_INVOICE_AMOUNT - SUM((D.NET_PRODUCT_AMOUNT / D.INVOICE_QTY) * B.QTY), 2) REMAINING
FROM INVOICE_MST M
INNER JOIN   INVOICE_DTL D ON M.MST_ID= D.MST_ID
INNER JOIN (SELECT SUBSTR(REGEXP_SUBSTR (:param1, '[^;]+', 1, ROWNUM),1,INSTR(REGEXP_SUBSTR (:param1, '[^;]+', 1, ROWNUM),',')-1) PRODUCT,
                SUBSTR(REGEXP_SUBSTR (:param1, '[^;]+', 1, ROWNUM),INSTR(REGEXP_SUBSTR (:param1, '[^;]+', 1, ROWNUM),',')+1) QTY     
         FROM  DUAL
         WHERE SUBSTR(REGEXP_SUBSTR (:param1, '[^;]+', 1, ROWNUM),1,INSTR(REGEXP_SUBSTR (:param1, '[^;]+', 1, ROWNUM),',')-1) IS NOT NULL
         CONNECT BY LEVEL <= LENGTH (REGEXP_REPLACE (:param1, '[^;]+'))) B ON D.SKU_CODE=B.PRODUCT
WHERE    M.INVOICE_NO=:param2 AND M.INVOICE_UNIT_ID=:param3 AND M.COMPANY_ID=:param4
GROUP BY M.NET_INVOICE_AMOUNT", _commonServices.AddParameter(new string[] { process, process, process, process, process, process, process, model[0].INVOICE_NO, model[0].UNIT_ID.ToString(), model[0].COMPANY_ID.ToString() }));
            if (ckhDt.Rows.Count > 0)
            {
                // Assuming REMAINING is in the first column of the first row
                decimal remaining = Convert.ToDecimal(ckhDt.Rows[0]["REMAINING"]);

                if (remaining >= 0)
                {
                    
                }
                else
                {
                    return "So you can not return more then Net invoice, Please Chek Adjuastment Or Qty";
                }
            }
            else
            {
                return "error data";
            }



            DataTable Dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), SalesReturnPartial_Query(), _commonServices.AddParameter(new string[] { model[0].INVOICE_NO, process ,model[0].UNIT_ID.ToString(),model[0].COMPANY_ID.ToString() }));
            if(Dt.Rows.Count>0)
            {
                result  = Dt.Rows[0][0].ToString();
            }
            return result;
        }

        public async Task<string> InvoicesLoad(string db, string Date_From, string Date_To, string CompanyId,string UnitId,string IsDistributor,string CUSTOMER_CODE)
        {
            string Query = "";
            DataTable _invoices = new DataTable();
            if (IsDistributor == "True")
            {
                Query = SalesReturnInvoicesLoadByCustomerCode();
                Query += string.Format("  AND  I.INVOICE_DATE between TO_DATE('{0}','DD/MM/YYYY') AND TO_DATE('{1}','DD/MM/YYYY')+1", Date_From, Date_To);

                _invoices = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Query, _commonServices.AddParameter(new string[] { CompanyId, CUSTOMER_CODE }));

            }
            else
            {
                Query = SalesReturnInvoiceLoad();
                Query += string.Format("  AND  I.INVOICE_DATE between TO_DATE('{0}','DD/MM/YYYY') AND TO_DATE('{1}','DD/MM/YYYY')+1", Date_From, Date_To);

                _invoices = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Query, _commonServices.AddParameter(new string[] { CompanyId, UnitId }));

            }
            List<Invoice_Mst> bonus_Msts = new List<Invoice_Mst>();
            if (_invoices.Rows.Count > 0)
            {
                for (int i = 0; i < _invoices.Rows.Count; i++)
                {
                    Invoice_Mst _bonus_Mst = new Invoice_Mst
                    {

                        INVOICE_NO = _invoices.Rows[i]["INVOICE_NO"].ToString(),
                        MST_ID = Convert.ToInt32(_invoices.Rows[i]["MST_ID"]),
                        MST_ID_ENCRYPTED = _commonServices.Encrypt(_invoices.Rows[i]["MST_ID"].ToString())

                    };
                    bonus_Msts.Add(_bonus_Mst);
                }
            }
            return JsonSerializer.Serialize(bonus_Msts);

        }

    }
}
