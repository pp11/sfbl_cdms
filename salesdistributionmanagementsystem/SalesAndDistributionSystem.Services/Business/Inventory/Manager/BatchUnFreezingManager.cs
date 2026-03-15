using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Services.Business.Inventory.IManager;
using SalesAndDistributionSystem.Services.Business.Inventory.Manager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Inventory.Manager
{
    public class BatchUnFreezingManager : IBatchUnFreezingManager
    {

        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public BatchUnFreezingManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        string GetBatchNo_Query() => @"SELECT DISTINCT 
                                              A.BATCH_NO,
                                              A.BATCH_ID,
                                              A.SKU_ID,
                                              A.SKU_CODE,
                                              FN_SKU_NAME(A.COMPANY_ID,A.SKU_ID) SKU_NAME,
                                              FN_PACK_SIZE(A.COMPANY_ID,A.SKU_ID) PACK_SIZE,
                                              A.UNIT_TP,
                                              A.BATCH_QTY BATCH_QTY,
                                              A.FREEZE_QTY BATCH_FREEZE_QTY,
                                             A.FREEZE_QTY BATCH_UN_FREEZE_QTY,
                                             A.MST_ID FREEZE_MST_ID,
                                             A.DTL_ID FREEZE_DTL_ID


                                             
                                       FROM   BATCH_FREEZING_DTL A
                                       WHERE  A.COMPANY_ID =:param1
                                       AND    A.UNIT_ID    =:param2
                                       AND    A.SKU_ID     =:param3";


        string GetSkuList_Query() => @"SELECT  DISTINCT
                                               SKU_ID,
                                               SKU_CODE,
                                               FN_SKU_NAME(COMPANY_ID,SKU_ID) SKU_NAME,
                                               FN_PACK_SIZE(COMPANY_ID,SKU_ID) PACK_SIZE       
                                        FROM  BATCH_FREEZING_DTL
                                        WHERE COMPANY_ID = :param1
                                        AND   UNIT_ID = :param2
                                        AND   NVL(FREEZE_QTY,0)>0                                        
                                        ORDER BY FN_SKU_NAME(COMPANY_ID,SKU_ID) ASC";



        string GetDtlData_Query() => @"SELECT  ROW_NUMBER() OVER(ORDER BY DTL_ID ASC) AS ROW_NO,
                                               DTL_ID, 
                                               MST_ID, 
                                               COMPANY_ID, 
                                               UNIT_ID, 
                                               SKU_ID, 
                                               SKU_CODE, 
                                               FN_SKU_NAME(COMPANY_ID,SKU_ID) SKU_NAME,
                                               FN_PACK_SIZE(COMPANY_ID,SKU_ID) PACK_SIZE,
                                               UNIT_TP, 
                                               BATCH_ID, 
                                               BATCH_NO,
                                               (SELECT PASSED_QTY FROM BATCH_WISE_STOCK WHERE COMPANY_ID=A.COMPANY_ID AND UNIT_ID=A.UNIT_ID AND SKU_ID=A.SKU_ID AND BATCH_ID=A.BATCH_ID)STOCK_QTY,
                                               BATCH_QTY, 
                                               FREEZE_QTY BATCH_FREEZE_QTY,
                                               UN_FREEZE_QTY BATCH_UN_FREEZE_QTY
                                        FROM   BATCH_UN_FREEZING_DTL A
                                        WHERE  MST_ID=:param1
                                        ORDER BY DTL_ID ASC";



        string GetMstData_Query() => @"SELECT MST_ID, 
                                               TO_CHAR(ENTRY_DATE,'DD/MM/RRRR')ENTRY_DATE, 
                                               COMPANY_ID,
                                               FN_COMPANY_NAME(COMPANY_ID)COMPANY_NAME, 
                                               UNIT_ID, 
                                               FN_UNIT_NAME(COMPANY_ID,UNIT_ID) UNIT_NAME,
                                               SKU_ID, 
                                               SKU_CODE,
                                               FN_SKU_NAME(COMPANY_ID,SKU_ID) SKU_NAME,
                                               FN_PACK_SIZE(COMPANY_ID,SKU_ID) PACK_SIZE, 
                                               REMARKS
                                        FROM BATCH_UN_FREEZING_MST
                                        WHERE COMPANY_ID = :param1
                                        AND   UNIT_ID = :param2
                                        ORDER BY MST_ID DESC";


        string InsertQueryMst() => @"INSERT INTO BATCH_UN_FREEZING_MST(
                                                                    MST_ID, 
                                                                    ENTRY_DATE, 
                                                                    COMPANY_ID, 
                                                                    UNIT_ID, 
                                                                    SKU_ID, 
                                                                    SKU_CODE, 
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
                                                                    TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'),  
                                                                    :param10                                                                    
                                                                    )";


        string InsertQueryDtl() => @"INSERT INTO BATCH_UN_FREEZING_DTL 
                                                                   (
                                                                    DTL_ID, 
                                                                    MST_ID,
                                                                    UN_FREEZE_DATE,
                                                                    COMPANY_ID, 
                                                                    UNIT_ID, 
                                                                    SKU_ID, 
                                                                    SKU_CODE, 
                                                                    UNIT_TP,
                                                                    BATCH_ID, 
                                                                    BATCH_NO,                                                                     
                                                                    BATCH_QTY, 
                                                                     FREEZE_QTY, 
                                                                    UN_FREEZE_QTY,
                                                                    ENTERED_BY, 
                                                                    ENTERED_DATE, 
                                                                    ENTERED_TERMINAL
                                                                    
                                                                   ) 
                                                            VALUES (:param1, 
                                                                    :param2,                                                                    
                                                                    TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM'),
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
                                                                    TO_DATE(:param15, 'DD/MM/YYYY HH:MI:SS AM'), 
                                                                    :param16
                                                                     
                                                                    )";

        string UpdateQueryMst() => @"UPDATE BATCH_UN_FREEZING_MST SET  
                                                                  REMARKS=:param1,
                                                                  UPDATED_BY= :param2,
                                                                  UPDATED_DATE=  TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM'),
                                                                  UPDATED_TERMINAL= :param4
                                                                  WHERE MST_ID = :param5";

        string UpdateQueryDtl() => @"UPDATE BATCH_UN_FREEZING_DTL SET FREEZE_QTY = :param1, 
                                                                    UN_FREEZE_QTY = :param2, 
                                                                   UPDATED_BY= :param3,
                                                                   UPDATED_DATE=  TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'),
                                                                   UPDATED_TERMINAL= :param5
                                                                   WHERE DTL_ID = :param6";



        public async Task<string> GetBatchList(string db, int company_id, int unit_id, int sku_id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetBatchNo_Query(), _commonServices.AddParameter(new string[] { company_id.ToString(), unit_id.ToString(), sku_id.ToString() })));

        public async Task<string> GetSkuList(string db, int company_id, int unit_id)
            => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetSkuList_Query(), _commonServices.AddParameter(new string[] { company_id.ToString(), unit_id.ToString() })));

        public async Task<string> GetDtlData(string db, int mst_id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetDtlData_Query(), _commonServices.AddParameter(new string[] { mst_id.ToString() })));

        public async Task<string> GetMstData(string db, int company_id, int unit_id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetMstData_Query(), _commonServices.AddParameter(new string[] { company_id.ToString(), unit_id.ToString() })));

        string GetMaxMstIdQuery() => "SELECT NVL(MAX(MST_ID),0)+1 MST_ID FROM BATCH_UN_FREEZING_MST";
        string GetMaxDtlIdQuery() => "SELECT NVL(MAX(DTL_ID),0)+1 DTL_ID FROM BATCH_UN_FREEZING_DTL";

        public async Task<string> InsertOrUpdate(string db, BatchUnFreezingMst model)
        {
            if (model == null)
            {
                return "No data provided to insert!!!!";

            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {

                    int dtlId = 0;

                    if (model.MST_ID == 0)
                    {

                        model.MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetMaxMstIdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(InsertQueryMst(), _commonServices.AddParameter(new string[] { model.MST_ID.ToString(),
                                                                                                                               model.ENTERED_DATE.ToString(),
                                                                                                                               model.COMPANY_ID.ToString(),
                                                                                                                               model.UNIT_ID.ToString(),
                                                                                                                               model.SKU_ID.ToString(),
                                                                                                                               model.SKU_CODE.ToString(),
                                                                                                                               model.REMARKS,
                                                                                                                               model.ENTERED_BY.ToString(),
                                                                                                                               model.ENTERED_DATE,
                                                                                                                               model.ENTERED_TERMINAL
                                                                                                                               })));

                        if (model.batchFreezingDtlList != null && model.batchFreezingDtlList.Count > 0)
                        {
                            dtlId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetMaxDtlIdQuery(), _commonServices.AddParameter(new string[] { }));

                            foreach (var item in model.batchFreezingDtlList)
                            {


                                listOfQuery.Add(_commonServices.AddQuery(InsertQueryDtl(), _commonServices.AddParameter(new string[] { dtlId.ToString(),
                                                                                                                                      model.MST_ID.ToString(),
                                                                                                                                      model.ENTERED_DATE,
                                                                                                                                      item.COMPANY_ID.ToString(),
                                                                                                                                      item.UNIT_ID.ToString(),
                                                                                                                                      item.SKU_ID.ToString(),
                                                                                                                                      item.SKU_CODE,
                                                                                                                                      item.UNIT_TP.ToString(),
                                                                                                                                      item.BATCH_ID.ToString(),
                                                                                                                                      item.BATCH_NO,
                                                                                                                                      item.BATCH_QTY.ToString(),
                                                                                                                                      item.BATCH_FREEZE_QTY.ToString(),
                                                                                                                                      item.BATCH_UN_FREEZE_QTY.ToString(),
                                                                                                                                      model.ENTERED_BY.ToString(),
                                                                                                                                      model.ENTERED_DATE,
                                                                                                                                      model.ENTERED_TERMINAL

                                                                                                                                      })));

                                dtlId++;
                            }
                        }

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(UpdateQueryMst(), _commonServices.AddParameter(new string[] {  model.REMARKS,
                                                                                                                                model.UPDATED_BY.ToString(),
                                                                                                                                model.UPDATED_DATE.ToString(),
                                                                                                                                model.UPDATED_TERMINAL.ToString(),
                                                                                                                                model.MST_ID.ToString()
                                                                                                                                })));

                        foreach (var item in model.batchFreezingDtlList)
                        {
                            if (item.DTL_ID == 0)
                            {
                                dtlId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetMaxDtlIdQuery(), _commonServices.AddParameter(new string[] { }));

                                listOfQuery.Add(_commonServices.AddQuery(InsertQueryDtl(), _commonServices.AddParameter(new string[] { dtlId.ToString(),
                                                                                                                                      model.MST_ID.ToString(),
                                                                                                                                      model.ENTERED_DATE,
                                                                                                                                      item.COMPANY_ID.ToString(),
                                                                                                                                      item.UNIT_ID.ToString(),
                                                                                                                                      item.SKU_ID.ToString(),
                                                                                                                                      item.SKU_CODE,
                                                                                                                                      item.UNIT_TP.ToString(),
                                                                                                                                      item.BATCH_ID.ToString(),
                                                                                                                                      item.BATCH_NO,
                                                                                                                                      item.BATCH_QTY.ToString(),
                                                                                                                                      item.BATCH_FREEZE_QTY.ToString(),
                                                                                                                                      item.BATCH_UN_FREEZE_QTY.ToString(),
                                                                                                                                      model.ENTERED_BY.ToString(),
                                                                                                                                      model.ENTERED_DATE,
                                                                                                                                      model.ENTERED_TERMINAL
                                                                                                                                      })));

                            }
                            else
                            {
                                listOfQuery.Add(_commonServices.AddQuery(UpdateQueryDtl(), _commonServices.AddParameter(new string[] {  item.BATCH_FREEZE_QTY.ToString(),
                                                                                                                                        item.BATCH_UN_FREEZE_QTY.ToString(),
                                                                                                                                        model.UPDATED_BY.ToString(),
                                                                                                                                        model.UPDATED_DATE,
                                                                                                                                        model.UPDATED_TERMINAL,
                                                                                                                                        item.DTL_ID.ToString()
                                                                                                                                      })));

                            }

                        }

                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return model.MST_ID.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }



    }
}
