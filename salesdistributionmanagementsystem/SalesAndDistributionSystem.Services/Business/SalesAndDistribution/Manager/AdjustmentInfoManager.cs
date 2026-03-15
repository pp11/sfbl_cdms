using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public class AdjustmentInfoManager : IAdjustmentInfoManager
    {

        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;


        public AdjustmentInfoManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        string GetMaxAdjustmentIdQuery() => "SELECT NVL(MAX(ADJUSTMENT_ID),0) + 1 ADJUSTMENT_ID  FROM ADJUSTMENT_INFO";

        string InertQuery() => @"INSERT INTO ADJUSTMENT_INFO(
                                                            ADJUSTMENT_ID,
                                                            ADJUSTMENT_NAME, 
                                                            ADJUSTMENT_STATUS,
                                                            COMPANY_ID,
                                                            REMARKS,  
                                                            ENTERED_BY, 
                                                            ENTERED_DATE,
                                                            ENTERED_TERMINAL 
                                                            ) 
                                                     VALUES ( 
                                                            :param1, 
                                                            :param2, 
                                                            :param3, 
                                                            :param4, 
                                                            :param5, 
                                                            :param6, 
                                                            TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), 
                                                            :param8 
                                                            )";

        string UpdateQuery() => @"UPDATE ADJUSTMENT_INFO  SET 
                                                          ADJUSTMENT_NAME = :param2,                                                         
                                                          ADJUSTMENT_STATUS = :param3,
                                                          REMARKS = :param4,
                                                          UPDATED_BY = :param5, 
                                                          UPDATED_DATE = TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), 
                                                          UPDATED_TERMINAL = :param7 
                                                          WHERE ADJUSTMENT_ID = :param1";

        string SearchDataQuery() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.ADJUSTMENT_ID ASC) AS ROW_NO,
                                    M.ADJUSTMENT_ID, 
                                    M.ADJUSTMENT_NAME,                                     
                                    M.ADJUSTMENT_STATUS, 
                                    M.COMPANY_ID,
                                    FN_COMPANY_NAME(M.COMPANY_ID) COMPANY_NAME,
                                    M.REMARKS, 
                                    TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM ADJUSTMENT_INFO M  
                                    WHERE M.COMPANY_ID = :param1";

        public async Task<string> InsertOrUpdate(string db, Adjustment_Info model)
        {
            Result result = new Result();
            if (model == null)
            {
                result.Status =   "No data provided to insert !!!";
            }
            else
            {


                List<QueryPattern> listOfQuery = new List<QueryPattern>();

                try
                {

                    if (model.ADJUSTMENT_ID == 0)
                    {

                        model.ADJUSTMENT_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetMaxAdjustmentIdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(InertQuery(), _commonServices.AddParameter(new string[] {
                                                                                                                           model.ADJUSTMENT_ID.ToString(),
                                                                                                                           model.ADJUSTMENT_NAME,
                                                                                                                           model.ADJUSTMENT_STATUS,
                                                                                                                           model.COMPANY_ID.ToString(),
                                                                                                                           model.REMARKS,
                                                                                                                           model.ENTERED_BY.ToString(),
                                                                                                                           model.ENTERED_DATE,
                                                                                                                           model.ENTERED_TERMINAL
                                                                                                                           })));

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(UpdateQuery(), _commonServices.AddParameter(new string[] {
                                                                                                                          model.ADJUSTMENT_ID.ToString(),
                                                                                                                          model.ADJUSTMENT_NAME,
                                                                                                                          model.ADJUSTMENT_STATUS,
                                                                                                                          model.REMARKS,
                                                                                                                          model.UPDATED_BY.ToString(),
                                                                                                                          model.UPDATED_DATE,
                                                                                                                          model.UPDATED_TERMINAL })));

                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    result .Key =  model.ADJUSTMENT_ID.ToString();
                    result.Status = "1";

                }
                catch (Exception ex)
                {
                    result.Status =  ex.Message;
                }
            }
            return JsonSerializer.Serialize(result);

        }

        public async Task<string> SearchData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), SearchDataQuery(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));





    }
}
