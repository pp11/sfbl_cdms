using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Services.Business.Inventory.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Inventory.Manager
{
    public class StockAdjustmentManager : IStockAdjustmentManager
    {


        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public StockAdjustmentManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }


        string GetBatchNo_Query() => @"SELECT DISTINCT 
                                              SKU_ID,
                                              SKU_CODE,
                                              FN_SKU_NAME(COMPANY_ID,SKU_ID) SKU_NAME,
                                              FN_PACK_SIZE(COMPANY_ID,SKU_ID) PACK_SIZE,
                                              UNIT_TP,
                                              BATCH_ID,
                                              BATCH_NO,
                                              PASSED_QTY STOCK_QTY
                                       FROM   BATCH_WISE_STOCK
                                       WHERE  COMPANY_ID =:param1
                                       AND    UNIT_ID    =:param2
                                       AND    SKU_ID     =:param3";

        string GetSearchData_Query() => @"SELECT   A.ADJUSTMENT_ID,
                                                   TO_CHAR(A.ADJUSTMENT_DATE,'DD/MM/RRRR')ADJUSTMENT_DATE,       
                                                   A.SKU_ID,
                                                   A.SKU_CODE,
                                                   FN_SKU_NAME(A.COMPANY_ID,A.SKU_ID) SKU_NAME,
                                                   FN_PACK_SIZE(A.COMPANY_ID,A.SKU_ID) PACK_SIZE,       
                                                   A.UNIT_TP,
                                                   A.BATCH_ID,
                                                   A.BATCH_NO,
                                                   A.STOCK_TYPE,
                                                   DECODE(A.STOCK_TYPE,'P','Pass Stock','Q','Quarantine stock')STOCK_TYPE_NAME,
                                                   A.ADJUSTMENT_TYPE,
                                                   DECODE(A.ADJUSTMENT_TYPE,'G','Gain','L','Lose') ADJUSTMENT_TYPE_NAME,
                                                   A.ADJUSTMENT_QTY,
                                                   A.ADJUSTMENT_AMOUNT,
                                                   B.PASSED_QTY STOCK_QTY,
                                                   A.REMARKS
                                            FROM SKU_STOCK_ADJUSTMENT A, BATCH_WISE_STOCK B
                                            WHERE A.COMPANY_ID=B.COMPANY_ID
                                            AND   A.UNIT_ID=B.UNIT_ID
                                            AND   A.SKU_ID=B.SKU_ID
                                            AND   A.BATCH_ID=B.BATCH_ID
                                            AND   A.COMPANY_ID =:param1
                                            AND   A.UNIT_ID    =:param2
                                            ORDER BY A.ADJUSTMENT_ID DESC";


        public async Task<string> GetBatchList(string db, int company_id, int unit_id, int sku_id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetBatchNo_Query(), _commonServices.AddParameter(new string[] { company_id.ToString(), unit_id.ToString(), sku_id.ToString() })));

        public async Task<string> GetSearchData(string db, int company_id, int unit_id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetSearchData_Query(), _commonServices.AddParameter(new string[] { company_id.ToString(), unit_id.ToString() })));

        string GetMaxIdQuery() => "SELECT NVL(MAX(ADJUSTMENT_ID),0) + 1 ADJUSTMENT_ID  FROM SKU_STOCK_ADJUSTMENT";

        string InertQuery() => @"INSERT INTO SKU_STOCK_ADJUSTMENT(
                                                                    ADJUSTMENT_ID,
                                                                    ADJUSTMENT_DATE,
                                                                    COMPANY_ID,
                                                                    UNIT_ID,
                                                                    STOCK_TYPE,
                                                                    ADJUSTMENT_TYPE,
                                                                    SKU_ID,
                                                                    SKU_CODE,
                                                                    UNIT_TP,
                                                                    BATCH_ID,
                                                                    BATCH_NO,
                                                                    ADJUSTMENT_QTY,
                                                                    ADJUSTMENT_AMOUNT,
                                                                    REMARKS,
                                                                    ENTERED_BY, 
                                                                    ENTERED_DATE,
                                                                    ENTERED_TERMINAL 
                                                                   ) 
                                                            VALUES ( 
                                                                :param1, 
                                                                TO_DATE(:param2, 'DD/MM/YYYY HH:MI:SS AM'), 
                                                                :param3, 
                                                                :param4, 
                                                                :param5, 
                                                                :param6, 
                                                                :param7, 
                                                                :param8, 
                                                                :param9, 
                                                                :param10, 
                                                                :param11, 
                                                                :param12, 
                                                                :param13, 
                                                                :param14, 
                                                                :param15, 
                                                                TO_DATE(:param16, 'DD/MM/YYYY HH:MI:SS AM'),
                                                                :param17 
                                                                )";


        string UpdateQuery() => @"UPDATE SKU_STOCK_ADJUSTMENT SET 
                                                              ADJUSTMENT_QTY = :param1,  
                                                              REMARKS = :param2, 
                                                              UPDATED_BY=:param3,
                                                              UPDATED_DATE = TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'), 
                                                              UPDATED_TERMINAL = :param5
                                                              WHERE ADJUSTMENT_ID = :param6";



        public async Task<string> InsertOrUpdate(string db, Stock_Adjustment model)
        {
            if (model == null)
            {
                return "No data provided to insert !!!";
            }
            else
            {


                List<QueryPattern> listOfQuery = new List<QueryPattern>();

                try
                {

                    if (model.ADJUSTMENT_ID == 0)
                    {

                        model.ADJUSTMENT_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetMaxIdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(InertQuery(), _commonServices.AddParameter(new string[] {
                                                                                                                           model.ADJUSTMENT_ID.ToString(),
                                                                                                                           model.ADJUSTMENT_DATE,
                                                                                                                           model.COMPANY_ID.ToString(),
                                                                                                                           model.UNIT_ID.ToString(),
                                                                                                                           model.STOCK_TYPE,
                                                                                                                           model.ADJUSTMENT_TYPE,
                                                                                                                           model.SKU_ID.ToString(),
                                                                                                                           model.SKU_CODE,
                                                                                                                           model.UNIT_TP.ToString(),
                                                                                                                           model.BATCH_ID.ToString(),
                                                                                                                           model.BATCH_NO,
                                                                                                                           model.ADJUSTMENT_QTY.ToString(),
                                                                                                                           model.ADJUSTMENT_AMOUNT.ToString(),
                                                                                                                           model.REMARKS,
                                                                                                                           model.ENTERED_BY.ToString(),
                                                                                                                           model.ENTERED_DATE,
                                                                                                                           model.ENTERED_TERMINAL
                                                                                                                           })));

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(UpdateQuery(), _commonServices.AddParameter(new string[] {
                                                                                                                          model.ADJUSTMENT_QTY.ToString(),
                                                                                                                          model.REMARKS,
                                                                                                                          model.UPDATED_BY.ToString(),
                                                                                                                          model.UPDATED_DATE,
                                                                                                                          model.UPDATED_TERMINAL,
                                                                                                                          model.ADJUSTMENT_ID.ToString()
                                                                                                                          })));

                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return model.ADJUSTMENT_ID.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }


    }
}
