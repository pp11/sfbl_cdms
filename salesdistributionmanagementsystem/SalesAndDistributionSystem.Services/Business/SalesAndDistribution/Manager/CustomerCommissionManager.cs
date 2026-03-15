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
    public class CustomerCommissionManager : ICustomerCommissionManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public CustomerCommissionManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        string AddOrUpdate_AddQuery() => @"INSERT INTO CUSTOMER_COMMISSION 
                                         (COMMISSION_ID, CUSTOMER_CODE,CUSTOMER_ID,
                                         ENTRY_DATE,EFFECT_START_DATE,EFFECT_END_DATE,
                                         COMMISSION_TYPE ,COMMISSION_VALUE,ADD_COMMISSION1,ADD_COMMISSION2,
                                         COMPANY_ID ,UNIT_ID ,STATUS,REMARKS ,
                                         ENTERED_BY ,ENTERED_DATE,ENTERED_TERMINAL) 
                                         VALUES 
                                        ( :param1, :param2, :param3, 
                                          TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'), TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'), TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'),
                                          :param7, :param8,:param9,:param10,
                                          :param11, :param12,:param13,:param14,
                                          :param15, TO_DATE(:param16, 'DD/MM/YYYY HH:MI:SS AM'),:param17)";


        string AddOrUpdate_UpdateQuery() => @"UPDATE CUSTOMER_COMMISSION SET 
                                            EFFECT_END_DATE = TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'),
                                            COMMISSION_VALUE = :param8,
                                            ADD_COMMISSION1 = :param9,ADD_COMMISSION2=:param10,
                                            REMARKS = :param14,STATUS = :param13,
                                            UPDATED_BY = :param15, UPDATED_DATE = TO_DATE(:param16, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param17 WHERE COMMISSION_ID = :param1";



        string GetCommissionIDGenerateQuery() => "SELECT NVL(MAX(COMMISSION_ID),0) + 1 COMMISSION_ID  FROM CUSTOMER_COMMISSION";
        string Search_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY COMMISSION_ID) AS ROW_NO,
                                    CU.COMMISSION_ID ,CU.CUSTOMER_CODE,to_char(CU.CUSTOMER_ID)CUSTOMER_ID,C.CUSTOMER_NAME , TO_CHAR(CU.ENTRY_DATE,'DD/MM/YYYY HH:MI:SS AM')ENTRY_DATE ,TO_CHAR(CU.EFFECT_START_DATE,'DD/MM/YYYY') EFFECT_START_DATE,TO_CHAR(CU.EFFECT_END_DATE ,'DD/MM/YYYY')EFFECT_END_DATE,CU.COMMISSION_TYPE ,
                                    CU.COMMISSION_VALUE,CU.ADD_COMMISSION1,CU.ADD_COMMISSION2 ,CU.COMPANY_ID  ,CU.UNIT_ID,CU.STATUS ,CU.REMARKS ,
                                    CU.ENTERED_BY,CU.ENTERED_DATE,CU.ENTERED_TERMINAL,CU.UPDATED_BY,CU.UPDATED_DATE,CU.UPDATED_TERMINAL    
                                      FROM CUSTOMER_COMMISSION CU, CUSTOMER_INFO C WHERE CU.CUSTOMER_ID=C.CUSTOMER_ID AND CU.COMPANY_ID = :param1";

        //---------- Method Execution Part ------------------------------------------------


        public async Task<string> AddOrUpdate(string db, CustomerCommission model)
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

                    if (model.COMMISSION_ID == 0)
                    {
                        //model.AREA_CODE = await GenerateAreaCode(db, model.COMPANY_ID.ToString());

                        model.COMMISSION_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetCommissionIDGenerateQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), 
                            _commonServices.AddParameter(new string[] { model.COMMISSION_ID.ToString(), model.CUSTOMER_CODE, model.CUSTOMER_ID, 
                                                                       model.ENTRY_DATE, model.EFFECT_START_DATE, model.EFFECT_END_DATE, 
                                                                        model.COMMISSION_TYPE, model.COMMISSION_VALUE.ToString(), model.ADD_COMMISSION1.ToString(), model.ADD_COMMISSION2.ToString(),
                                                                     model.COMPANY_ID.ToString(), model.UNIT_ID.ToString(), model.STATUS, model.REMARKS,
                                                                     model.ENTERED_BY,  model.ENTERED_DATE,  model.ENTERED_TERMINAL
                                                                        })));                        
                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                               model.COMMISSION_ID.ToString(), model.CUSTOMER_CODE, model.CUSTOMER_ID.ToString(),
                                model.ENTRY_DATE, model.EFFECT_START_DATE, model.EFFECT_END_DATE,
                                model.COMMISSION_TYPE, model.COMMISSION_VALUE.ToString(), model.ADD_COMMISSION1.ToString(), model.ADD_COMMISSION2.ToString(),
                                model.COMPANY_ID.ToString(), model.UNIT_ID.ToString(), model.STATUS, model.REMARKS,
                                model.UPDATED_BY,  model.UPDATED_DATE,  model.UPDATED_TERMINAL

                            })));

                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return model.COMMISSION_ID.ToString();
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
