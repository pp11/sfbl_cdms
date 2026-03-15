using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.Manager
{
    public class SkuLoadingChargeManager : ISkuLoadingChargeManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;

        public SkuLoadingChargeManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;
        }


        string AddOrUpdate_AddQuery() => @"INSERT INTO SKU_WISE_LOADING_CHARGE 
                                         (LOADING_CHARGE_ID, SKU_ID,SKU_CODE,
                                         ENTRY_DATE,EFFECT_START_DATE,EFFECT_END_DATE,
                                         SHIPPER_QTY ,LOADING_CHARGE,
                                         COMPANY_ID ,UNIT_ID ,STATUS,REMARKS ,
                                         ENTERED_BY ,ENTERED_DATE,ENTERED_TERMINAL) 
                                         VALUES 
                                        ( :param1, :param2, :param3, 
                                          TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'), TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'), TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'),
                                          :param7, :param8,
                                          :param9, :param10,:param11,:param12,
                                          :param13, TO_DATE(:param14, 'DD/MM/YYYY HH:MI:SS AM'),:param15                                          
                                        )";

        string IsSKULoadingChargeExist() => @"SELECT COUNT(S.LOADING_CHARGE_ID ) FROM SKU_WISE_LOADING_CHARGE S, PRODUCT_INFO P WHERE S.SKU_ID=P.SKU_ID AND S.COMPANY_ID = :param1 AND S.SKU_ID = :param2 AND TRUNC(S.EFFECT_END_DATE) >= TO_DATE(:param3,'DD/MM/YYYY')";
        string AddOrUpdate_UpdateQuery() => @"UPDATE SKU_WISE_LOADING_CHARGE SET          EFFECT_START_DATE= TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),
                                            EFFECT_END_DATE = TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'),
                                            LOADING_CHARGE = :param8,                                            
                                            STATUS = :param11,REMARKS = :param12,
                                            UPDATED_BY = :param13, UPDATED_DATE = TO_DATE(:param14, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param15 WHERE LOADING_CHARGE_ID = :param1";



        string GetCommissionIDGenerateQuery() => "SELECT NVL(MAX(LOADING_CHARGE_ID),0) + 1 LOADING_CHARGE_ID  FROM SKU_WISE_LOADING_CHARGE";

        string Search_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY LOADING_CHARGE_ID) AS ROW_NO,
                                S.LOADING_CHARGE_ID ,S.SKU_CODE, S.SKU_ID, p.SKU_NAME ,P.PACK_SIZE,
                                TO_CHAR(S.ENTRY_DATE,'DD/MM/YYYY HH:MI:SS AM')ENTRY_DATE ,TO_CHAR(S.EFFECT_START_DATE,'DD/MM/YYYY') EFFECT_START_DATE,TO_CHAR(S.EFFECT_END_DATE ,'DD/MM/YYYY')EFFECT_END_DATE,
                                S.SHIPPER_QTY,S.LOADING_CHARGE ,S.COMPANY_ID  ,S.UNIT_ID,S.STATUS ,S.REMARKS ,
                                S.ENTERED_BY,S.ENTERED_DATE,S.ENTERED_TERMINAL,S.UPDATED_BY,S.UPDATED_DATE,S.UPDATED_TERMINAL    
                                FROM SKU_WISE_LOADING_CHARGE S, PRODUCT_INFO P WHERE S.SKU_ID=P.SKU_ID AND S.COMPANY_ID = :param1";

        //---------- Method Execution Part ------------------------------------------------


        public async Task<string> AddOrUpdate(string db, SkuLoadingCharge model)
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

                    if (model.LOADING_CHARGE_ID == 0)
                    {
                        //model.AREA_CODE = await GenerateAreaCode(db, model.COMPANY_ID.ToString());
                      int Previous_Count =      _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), IsSKULoadingChargeExist(), _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.SKU_ID.ToString(),model.EFFECT_START_DATE  }));
                        if (Previous_Count > 0)
                        {
                            return "The loading charge of this SKU is already exist in this due date!!!";
                        }
                        else
                        {
                            model.LOADING_CHARGE_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetCommissionIDGenerateQuery(), _commonServices.AddParameter(new string[] { }));

                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                                _commonServices.AddParameter(new string[] { model.LOADING_CHARGE_ID.ToString(), model.SKU_ID.ToString(),model.SKU_CODE,
                                                                       model.ENTRY_DATE, model.EFFECT_START_DATE, model.EFFECT_END_DATE,
                                                                       model.SHIPPER_QTY.ToString(), model.LOADING_CHARGE.ToString(),
                                                                     model.COMPANY_ID.ToString(), model.UNIT_ID.ToString(), model.STATUS, model.REMARKS,
                                                                     model.ENTERED_BY,  model.ENTERED_DATE,  model.ENTERED_TERMINAL
                                                                            })));

                        }
                       
                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                               model.LOADING_CHARGE_ID.ToString(),  model.SKU_ID.ToString(),model.SKU_CODE,
                                model.ENTRY_DATE, model.EFFECT_START_DATE, model.EFFECT_END_DATE,
                                model.SHIPPER_QTY.ToString(), model.LOADING_CHARGE.ToString(), 
                                model.COMPANY_ID.ToString(), model.UNIT_ID.ToString(), model.STATUS, model.REMARKS,
                                model.UPDATED_BY,  model.UPDATED_DATE,  model.UPDATED_TERMINAL

                            })));

                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return model.LOADING_CHARGE_ID.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<string> SearchData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Search_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));

    }
}
