using Microsoft.Extensions.Configuration;
using NUnit.Framework.Internal;
using Org.BouncyCastle.Utilities.Collections;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.Entities.Target;
using SalesAndDistributionSystem.Services.Business.Company;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Business.Target.InMarketSales.Interface;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Target.InMarketSales
{
    public class InMarketSales : IInMarketSales
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        private readonly INotificationManager _NotificationManager;
        private readonly IUserLogManager _logManager;
        public InMarketSales(ICommonServices commonServices, IConfiguration configuration, INotificationManager notificationManager, IUserLogManager userLogManager)
        {
            _commonServices = commonServices;
            _configuration = configuration;
            _NotificationManager = notificationManager;
            _logManager = userLogManager;
        }
        string GET_IN_MARKET_SALES_MST_ID_QUERY() => "SELECT NVL(MAX(MST_ID),0) + 1 MST_ID  FROM IN_MARKET_SALES_MST";
        string GET_IN_MARKET_SALES_DTL_ID_QUERY() => "SELECT NVL(MAX(DTL_ID),0) + 1 DTL_ID  FROM IN_MARKET_SALES_DTL";
        string LOAD_MASTER_DATA_QUERY() => @"SELECT ROWNUM ROW_NO,
         M.MST_ID,
         TO_CHAR (M.ENTRY_DATE, 'DD/MM/YYY') ENTRY_DATE,
         M.YEAR,
         M.MONTH_CODE,
         CASE
            WHEN M.MONTH_CODE = '01' THEN 'January'
            WHEN M.MONTH_CODE = '02' THEN 'February'
            WHEN M.MONTH_CODE = '03' THEN 'March'
            WHEN M.MONTH_CODE = '04' THEN 'April'
            WHEN M.MONTH_CODE = '05' THEN 'May'
            WHEN M.MONTH_CODE = '06' THEN 'June'
            WHEN M.MONTH_CODE = '07' THEN 'July'
            WHEN M.MONTH_CODE = '08' THEN 'August'
            WHEN M.MONTH_CODE = '09' THEN 'September'
            WHEN M.MONTH_CODE = '10' THEN 'October'
            WHEN M.MONTH_CODE = '11' THEN 'November'
            WHEN M.MONTH_CODE = '12' THEN 'December'
            ELSE 'No Match'
         END
            AS MONTH_NAME,
         M.MARKET_CODE,
         I.MARKET_NAME,
         M.ENTERED_DATE
    FROM IN_MARKET_SALES_MST M
         INNER JOIN MARKET_INFO I ON I.MARKET_ID = M.MARKET_ID
ORDER BY M.YEAR, M.MONTH_CODE, M.MARKET_ID";
        string LOAD_MST_DATA_BY_ID_QUERY() => @"SELECT MST_ID, TO_CHAR(ENTRY_DATE,'DD/MM/YYYY') ENTRY_DATE, YEAR, MONTH_CODE, MARKET_ID, MARKET_CODE, COMPANY_ID, UNIT_ID, ENTERED_BY, ENTERED_DATE, ENTERED_TERMINAL, UPDATED_BY, UPDATED_DATE, UPDATED_TERMINAL FROM IN_MARKET_SALES_MST M WHERE M.MST_ID= :param1 ";
        string LOAD_DTL_DATA_BY_ID_QUERY() => @"SELECT ROW_NUMBER () OVER (ORDER BY D.DTL_ID ASC) ROW_NO,
       D.DTL_ID,
       D.MST_ID,
       D.SKU_ID,
       D.SKU_CODE,
       D.UNIT_TP,
       D.MRP,
       D.SALES_QTY,
       D.SALES_VALUE,
       D.COMPANY_ID,
       D.UNIT_ID,
       P.PACK_SIZE
  FROM IN_MARKET_SALES_DTL D INNER JOIN PRODUCT_INFO P ON P.SKU_ID = D.SKU_ID
 WHERE D.MST_ID = :param1";

        string MST_ADD_QUERY() => @"INSERT INTO IN_MARKET_SALES_MST (MST_ID, ENTRY_DATE, YEAR, MONTH_CODE, MARKET_ID, MARKET_CODE, COMPANY_ID, UNIT_ID, ENTERED_BY, ENTERED_DATE, ENTERED_TERMINAL) VALUES ( :param1, TO_DATE ( :param2, 'DD/MM/YYYY HH:MI:SS AM'), :param3, :param4, :param5, :param6, :param7, :param8, :param9, TO_DATE ( :param10, 'DD/MM/YYYY HH:MI:SS AM'), :param11)";
        string MST_UPDATE_QUERY() => @"UPDATE IN_MARKET_SALES_MST SET ENTRY_DATE = TO_DATE (:param1, 'DD/MM/YYYY HH:MI:SS AM'), YEAR = :param2, MONTH_CODE = :param3, MARKET_ID = :param4, MARKET_CODE = :param5, UNIT_ID = :param6, UPDATED_BY = :param7, UPDATED_DATE = TO_DATE (:param8, 'DD/MM/YYYY HH:MI:SS AM'), UPDATED_TERMINAL = :param9 WHERE MST_ID= :param10";
        string DTL_ADD_QUERY() => @"INSERT INTO IN_MARKET_SALES_DTL (DTL_ID, MST_ID, SKU_ID, SKU_CODE, UNIT_TP, MRP, SALES_QTY, SALES_VALUE, COMPANY_ID, UNIT_ID, ENTERED_BY, ENTERED_DATE, ENTERED_TERMINAL) VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8, :param9,:param10, :param11, TO_DATE ( :param12, 'DD/MM/YYYY HH:MI:SS AM'), :param13)";
        string DTL_UPDATE_QUERY() => @"UPDATE IN_MARKET_SALES_DTL SET SKU_ID = :param1, SKU_CODE = :param2, UNIT_TP = :param3, MRP = :param4, SALES_QTY = :param5, SALES_VALUE = :param6, UPDATED_BY = :param7, UPDATED_DATE = TO_DATE (:param8, 'DD/MM/YYYY HH:MI:SS AM'), UPDATED_TERMINAL = :param9 WHERE DTL_ID=:param10";
        public async Task<string> AddOrUpdate(string db, IN_MARKET_SALES_MST model)
        {
            Result result = new Result();
            if (model == null)
            {
                result.Status = "No data provided to insert!!!!";
            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {
                    bool status_action = true;
                    string dtl_list = "";
                    int dtlId = 0;
                    if (model.MST_ID == 0)
                    {
                        model.MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GET_IN_MARKET_SALES_MST_ID_QUERY(), _commonServices.AddParameter(new string[] { }));
                        listOfQuery.Add(_commonServices.AddQuery(MST_ADD_QUERY(),
                         _commonServices.AddParameter(new string[] {model.MST_ID.ToString(),
                             model.ENTRY_DATE.ToString(), model.YEAR.ToString(),model.MONTH_CODE,
                             model.MARKET_ID, model.MARKET_CODE.ToString(),model.COMPANY_ID.ToString(),model.UNIT_ID.ToString(), model.ENTERED_BY.ToString(), model.ENTERED_DATE,
                             model.ENTERED_TERMINAL})));

                        if (model.IN_MARKET_SALES_DTL != null && model.IN_MARKET_SALES_DTL.Count > 0)
                        {
                            dtlId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GET_IN_MARKET_SALES_DTL_ID_QUERY(), _commonServices.AddParameter(new string[] { }));
                            foreach (var item in model.IN_MARKET_SALES_DTL)
                            {
                                listOfQuery.Add(_commonServices.AddQuery(DTL_ADD_QUERY(),
                               _commonServices.AddParameter(new string[] {dtlId.ToString(), model.MST_ID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(), item.MRP.ToString(), item.SALES_QTY.ToString(), item.SALES_VALUE.ToString(),
                                     model.COMPANY_ID.ToString(), model.UNIT_ID.ToString(),  item.ENTERED_BY,  item.ENTERED_DATE,  model.ENTERED_TERMINAL})));
                                dtl_list += "," + dtlId;

                                dtlId++;

                            }
                        }
                        status_action = false;


                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            MST_UPDATE_QUERY(),
                            _commonServices.AddParameter(new string[] {
                             model.ENTRY_DATE, model.YEAR,model.MONTH_CODE,
                             model.MARKET_ID, model.MARKET_CODE,model.UNIT_ID.ToString(),model.UPDATED_BY, model.UPDATED_DATE, model.UPDATED_TERMINAL,model.MST_ID.ToString()})));
                        DataTable preLine = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), "SELECT M.DTL_ID FROM IN_MARKET_SALES_DTL M WHERE  M.MST_ID=:param1", _commonServices.AddParameter(new string[] { model.MST_ID.ToString() }));
                        var prevLines = preLine.AsEnumerable().Select(item => new { DTL_ID = Convert.ToString(item["DTL_ID"]) }).ToList();
                        var deletedLines = prevLines.Where(p => model.IN_MARKET_SALES_DTL.All(p2 => p2.DTL_ID != p.DTL_ID));
                        foreach (var item in model.IN_MARKET_SALES_DTL)
                        {
                            if (item.DTL_ID == "0" || String.IsNullOrWhiteSpace(item.DTL_ID))
                            {
                                //-------------Add new row on detail table--------------------
                                dtlId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GET_IN_MARKET_SALES_DTL_ID_QUERY(), _commonServices.AddParameter(new string[] { }));
                                listOfQuery.Add(_commonServices.AddQuery(DTL_ADD_QUERY(),
                               _commonServices.AddParameter(new string[] {dtlId.ToString(), model.MST_ID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(), item.MRP.ToString(), item.SALES_QTY.ToString(), item.SALES_VALUE.ToString(),
                                     model.COMPANY_ID.ToString(), model.UNIT_ID.ToString(),  item.ENTERED_BY,  item.ENTERED_DATE,  model.ENTERED_TERMINAL})));
                                dtl_list += "," + dtlId;

                                dtlId++;

                            }
                            else
                            {
                                //-------------Edit on detail table--------------------
                                listOfQuery.Add(_commonServices.AddQuery(DTL_UPDATE_QUERY(), _commonServices.AddParameter(new string[] {
                                     item.SKU_ID,item.SKU_CODE,
                                     item.UNIT_TP.ToString(), item.MRP.ToString(), item.SALES_QTY.ToString(), item.SALES_VALUE.ToString(),
                                     item.UPDATED_BY.ToString(), item.UPDATED_DATE,item.UPDATED_TERMINAL, item.DTL_ID})));
                                dtl_list += "," + item.DTL_ID;

                            }

                        }
                        foreach (var item in deletedLines)
                        {
                            //-------------Delete on detail table--------------------
                            listOfQuery.Add(_commonServices.AddQuery("DELETE IN_MARKET_SALES_DTL WHERE DTL_ID=:param1", _commonServices.AddParameter(new string[] { item.DTL_ID })));

                        }


                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    result.Key = _commonServices.Encrypt(model.MST_ID.ToString());
                    string st = status_action == false ? "Add" : "Edit";

                    //await _logManager.AddOrUpdate(model.db_security, st, "DEPOT_REQUISITION_RAISE_MST", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/Inventory/Requisition/Requisition", model.MST_ID, dtl_list);

                    result.Status = "1";
                }
                catch (Exception ex)
                {
                    result.Status = ex.Message;
                }
            }
            return JsonSerializer.Serialize(result);

        }

        public async Task<string> AddOrUpdateXlsx(string db, IN_MARKET_SALES_MST model)
        {
            Result result = new Result();
            if (model == null)
            {
                result.Status = "No data provided to insert!!!!";
            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {
                    bool status_action = true;
                    string dtl_list = "";
                    int dtlId = 0;
                    if (model.MST_ID == 0)
                    {
                        model.MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GET_IN_MARKET_SALES_MST_ID_QUERY(), _commonServices.AddParameter(new string[] { }));
                        dtlId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GET_IN_MARKET_SALES_DTL_ID_QUERY(), _commonServices.AddParameter(new string[] { }));
                        var distinctMstData = model.IN_MARKET_SALES_DTL.Select(p => new { p.MARKET_ID, p.MARKET_CODE, p.MONTH_CODE, p.YEAR }).Distinct();
                        foreach (var distinctMarket in distinctMstData.Select(p => new { p.MARKET_ID, p.MARKET_CODE }).Distinct())
                        {
                            var distinctYears = distinctMstData.Where(x => x.MARKET_ID == distinctMarket.MARKET_ID).Select(x => x.YEAR).Distinct();
                            foreach (var distinctYear in distinctYears)
                            {
                                foreach (var distinctMonth in distinctMstData.Where(x => x.MARKET_ID == distinctMarket.MARKET_ID && x.YEAR == distinctYear).Select(x => x.MONTH_CODE).Distinct())
                                {
                                    var obj = model.IN_MARKET_SALES_DTL.Where(x => x.MARKET_ID == distinctMarket.MARKET_ID && x.YEAR == distinctYear && x.MONTH_CODE == distinctMonth);
                                    listOfQuery.Add(_commonServices.AddQuery(MST_ADD_QUERY(), _commonServices.AddParameter(new string[] { model.MST_ID.ToString(), obj.Select(x => x.ENTRY_DATE).First(), distinctYear, distinctMonth, distinctMarket.MARKET_ID, distinctMarket.MARKET_CODE, model.COMPANY_ID.ToString(), model.UNIT_ID.ToString(), model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));
                                    foreach (var item in obj)
                                    {
                                        listOfQuery.Add(_commonServices.AddQuery(DTL_ADD_QUERY(),
                                       _commonServices.AddParameter(new string[] {dtlId.ToString(), model.MST_ID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(), item.MRP.ToString(), item.SALES_QTY.ToString(), item.SALES_VALUE.ToString(),
                                     model.COMPANY_ID.ToString(), model.UNIT_ID.ToString(),  item.ENTERED_BY,  item.ENTERED_DATE,  model.ENTERED_TERMINAL})));
                                        dtl_list += "," + dtlId;

                                        dtlId++;

                                    }
                                    model.MST_ID++;
                                }
                            }

                        }

                        status_action = false;
                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    result.Key = _commonServices.Encrypt(model.MST_ID.ToString());
                    string st = status_action == false ? "Add" : "Edit";

                    //await _logManager.AddOrUpdate(model.db_security, st, "DEPOT_REQUISITION_RAISE_MST", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/Inventory/Requisition/Requisition", model.MST_ID, dtl_list);

                    result.Status = "1";
                }
                catch (Exception ex)
                {
                    result.Status = ex.Message;
                }
            }
            return JsonSerializer.Serialize(result);

        }
        public async Task<string> LoadData(string db, int Company_Id)
        {
            List<IN_MARKET_SALES_MST> in_market_sales_mst_list = new List<IN_MARKET_SALES_MST>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LOAD_MASTER_DATA_QUERY(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                IN_MARKET_SALES_MST in_market_sales_mst = new IN_MARKET_SALES_MST();
                in_market_sales_mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                in_market_sales_mst.MST_ID = Convert.ToInt32(data.Rows[i]["MST_ID"]);
                in_market_sales_mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
                in_market_sales_mst.YEAR = data.Rows[i]["YEAR"].ToString();
                in_market_sales_mst.MONTH_CODE = data.Rows[i]["MONTH_CODE"].ToString();
                in_market_sales_mst.ENTRY_DATE = data.Rows[i]["ENTRY_DATE"].ToString();
                in_market_sales_mst.MARKET_CODE = data.Rows[i]["MARKET_CODE"].ToString();
                in_market_sales_mst.ENTERED_DATE = data.Rows[i]["ENTERED_DATE"].ToString();
                in_market_sales_mst.MARKET_NAME = data.Rows[i]["MARKET_NAME"].ToString();
                in_market_sales_mst.MONTH_NAME = data.Rows[i]["MONTH_NAME"].ToString();

                in_market_sales_mst_list.Add(in_market_sales_mst);
            }
            return JsonSerializer.Serialize(in_market_sales_mst_list);
        }
        public async Task<IN_MARKET_SALES_MST> GetEditDataById(string db, int mst_id)
        {
            IN_MARKET_SALES_MST in_market_sales_mst = new IN_MARKET_SALES_MST();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LOAD_MST_DATA_BY_ID_QUERY(), _commonServices.AddParameter(new string[] { mst_id.ToString() }));
            if (data.Rows.Count > 0)
            {
                in_market_sales_mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);
                in_market_sales_mst.MST_ID = Convert.ToInt32(data.Rows[0]["MST_ID"]);
                in_market_sales_mst.ENTRY_DATE = (data.Rows[0]["ENTRY_DATE"]).ToString();
                in_market_sales_mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[0]["MST_ID"].ToString());
                in_market_sales_mst.YEAR = (data.Rows[0]["YEAR"]).ToString();
                in_market_sales_mst.MONTH_CODE = data.Rows[0]["MONTH_CODE"].ToString();
                in_market_sales_mst.MARKET_ID = data.Rows[0]["MARKET_ID"].ToString();
                in_market_sales_mst.MARKET_CODE = (data.Rows[0]["MARKET_CODE"]).ToString();
                in_market_sales_mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);
                in_market_sales_mst.UNIT_ID = Convert.ToInt32(data.Rows[0]["UNIT_ID"]);

                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LOAD_DTL_DATA_BY_ID_QUERY(), _commonServices.AddParameter(new string[] { mst_id.ToString() }));
                in_market_sales_mst.IN_MARKET_SALES_DTL = new List<IN_MARKET_SALES_DTL>();
                for (int i = 0; i < dataTable_detail.Rows.Count; i++)
                {
                    IN_MARKET_SALES_DTL in_market_sales_dtl = new IN_MARKET_SALES_DTL();
                    in_market_sales_dtl.DTL_ID = dataTable_detail.Rows[i]["DTL_ID"].ToString();
                    in_market_sales_dtl.MST_ID = dataTable_detail.Rows[i]["MST_ID"].ToString();
                    in_market_sales_dtl.SKU_ID = (dataTable_detail.Rows[i]["SKU_ID"]).ToString();
                    in_market_sales_dtl.SKU_CODE = dataTable_detail.Rows[i]["SKU_CODE"].ToString();
                    in_market_sales_dtl.PACK_SIZE = dataTable_detail.Rows[i]["PACK_SIZE"].ToString();
                    in_market_sales_dtl.UNIT_TP = Convert.ToDecimal(dataTable_detail.Rows[i]["UNIT_TP"]);
                    in_market_sales_dtl.MRP = Convert.ToDecimal(dataTable_detail.Rows[i]["MRP"]);
                    in_market_sales_dtl.SALES_QTY = Convert.ToDecimal(dataTable_detail.Rows[i]["SALES_QTY"]);
                    in_market_sales_dtl.SALES_VALUE = Convert.ToDecimal(dataTable_detail.Rows[i]["SALES_VALUE"]);

                    in_market_sales_mst.IN_MARKET_SALES_DTL.Add(in_market_sales_dtl);
                }
            }
            return in_market_sales_mst;
        }

        public DataTable Exportxlsx(string db, IN_MARKET_SALES_MST model)
        {
            DataTable data;
            if (model.MONTH_CODE == "undefined" || model.MONTH_CODE == "00")
            {
                var query = @"SELECT DISTINCT  :param1 ENTRY_DATE, :param2 YEAR, '01' MONTH_CODE, I.MARKET_CODE MARKET_CODE, I.MARKET_NAME, M.SKU_CODE SKU_CODE, M.SKU_NAME,  0 SALES_QTY 
FROM  VW_PRODUCT_PRICE M 
CROSS JOIN MARKET_INFO I 
WHERE I.MARKET_CODE=:param3

UNION 
SELECT DISTINCT :param1 ENTRY_DATE, :param2 YEAR, '02' MONTH_CODE, I.MARKET_CODE MARKET_CODE, I.MARKET_NAME, M.SKU_CODE SKU_CODE,  M.SKU_NAME, 0 SALES_QTY 
FROM VW_PRODUCT_PRICE M 
CROSS JOIN MARKET_INFO I 
WHERE I.MARKET_CODE=:param3

UNION 
SELECT DISTINCT :param1 ENTRY_DATE, :param2 YEAR, '03' MONTH_CODE, I.MARKET_CODE MARKET_CODE, I.MARKET_NAME, M.SKU_CODE SKU_CODE, M.SKU_NAME,  0 SALES_QTY 
FROM VW_PRODUCT_PRICE M 
CROSS JOIN MARKET_INFO I 
WHERE I.MARKET_CODE=:param3

UNION 
SELECT :param1 ENTRY_DATE, :param2 YEAR, '04' MONTH_CODE, I.MARKET_CODE MARKET_CODE, I.MARKET_NAME, M.SKU_CODE SKU_CODE, M.SKU_NAME,  0 SALES_QTY 
FROM VW_PRODUCT_PRICE M 
CROSS JOIN MARKET_INFO I 
WHERE I.MARKET_CODE=:param3

UNION 
SELECT DISTINCT :param1 ENTRY_DATE, :param2 YEAR, '05' MONTH_CODE, I.MARKET_CODE MARKET_CODE, I.MARKET_NAME, M.SKU_CODE SKU_CODE, M.SKU_NAME,  0 SALES_QTY 
FROM VW_PRODUCT_PRICE M 
CROSS JOIN MARKET_INFO I 
WHERE I.MARKET_CODE=:param3

UNION 
SELECT DISTINCT :param1 ENTRY_DATE, :param2 YEAR, '06' MONTH_CODE, I.MARKET_CODE MARKET_CODE, I.MARKET_NAME, M.SKU_CODE SKU_CODE, M.SKU_NAME,  0 SALES_QTY 
FROM VW_PRODUCT_PRICE M 
CROSS JOIN MARKET_INFO I 
WHERE I.MARKET_CODE=:param3

UNION 
SELECT DISTINCT :param1 ENTRY_DATE, :param2 YEAR, '07' MONTH_CODE, I.MARKET_CODE MARKET_CODE, I.MARKET_NAME, M.SKU_CODE SKU_CODE, M.SKU_NAME,  0 SALES_QTY 
FROM VW_PRODUCT_PRICE M 
CROSS JOIN MARKET_INFO I 
WHERE I.MARKET_CODE=:param3

UNION 
SELECT DISTINCT :param1 ENTRY_DATE, :param2 YEAR, '08' MONTH_CODE, I.MARKET_CODE MARKET_CODE, I.MARKET_NAME, M.SKU_CODE SKU_CODE, M.SKU_NAME,  0 SALES_QTY 
FROM VW_PRODUCT_PRICE M 
CROSS JOIN MARKET_INFO I 
WHERE I.MARKET_CODE=:param3

UNION 
SELECT DISTINCT :param1 ENTRY_DATE, :param2 YEAR, '09' MONTH_CODE, I.MARKET_CODE MARKET_CODE, I.MARKET_NAME, M.SKU_CODE SKU_CODE, M.SKU_NAME,  0 SALES_QTY 
FROM VW_PRODUCT_PRICE M 
CROSS JOIN MARKET_INFO I 
WHERE I.MARKET_CODE=:param3

UNION 
SELECT DISTINCT :param1 ENTRY_DATE, :param2 YEAR, '10' MONTH_CODE, I.MARKET_CODE MARKET_CODE, I.MARKET_NAME, M.SKU_CODE SKU_CODE, M.SKU_NAME,  0 SALES_QTY 
FROM VW_PRODUCT_PRICE M 
CROSS JOIN MARKET_INFO I 
WHERE I.MARKET_CODE=:param3

UNION 
SELECT DISTINCT :param1 ENTRY_DATE, :param2 YEAR, '11' MONTH_CODE, I.MARKET_CODE MARKET_CODE, I.MARKET_NAME, M.SKU_CODE SKU_CODE, M.SKU_NAME,  0 SALES_QTY 
FROM VW_PRODUCT_PRICE M 
CROSS JOIN MARKET_INFO I 
WHERE I.MARKET_CODE=:param3

UNION 
SELECT DISTINCT :param1 ENTRY_DATE, :param2 YEAR, '12' MONTH_CODE, I.MARKET_CODE MARKET_CODE, I.MARKET_NAME, M.SKU_CODE SKU_CODE, M.SKU_NAME,  0 SALES_QTY 
FROM VW_PRODUCT_PRICE M 
CROSS JOIN MARKET_INFO I 
WHERE I.MARKET_CODE=:param3
ORDER BY  MONTH_CODE, SKU_CODE";

                data = _commonServices.GetDataTable(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { model.ENTRY_DATE, model.YEAR, model.MARKET_CODE }));

            }
            else
            {
                var query = @"SELECT DISTINCT :param1 ENTRY_DATE, :param2 YEAR, :param3 MONTH_CODE, I.MARKET_CODE MARKET_CODE, I.MARKET_NAME, M.SKU_CODE SKU_CODE, M.SKU_NAME,  0 SALES_QTY 
 FROM VW_PRODUCT_PRICE M 
 CROSS JOIN MARKET_INFO I 
 WHERE I.MARKET_CODE=:param4
 ORDER BY I.MARKET_CODE, M.SKU_CODE";

                data = _commonServices.GetDataTable(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { model.ENTRY_DATE, model.YEAR, model.MONTH_CODE, model.MARKET_CODE }));

            }
            return data;
        }


    }
}
