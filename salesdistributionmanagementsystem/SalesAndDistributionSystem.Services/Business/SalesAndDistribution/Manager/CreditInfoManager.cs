using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.Manager
{
    public class CreditInfoManager : ICreditInfoManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public CreditInfoManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        string GetCredit_Info_IdQuery() => "SELECT NVL(MAX(CREDIT_ID),0) + 1 CREDIT_INFO_ID  FROM CREDIT_INFO";
        string AddOrUpdate_AddQuery() => @"INSERT INTO CREDIT_INFO 
                                         (CREDIT_ID,
                                            CUSTOMER_CODE,
                                            CUSTOMER_ID,
                                            ENTRY_DATE,
                                            EFFECT_START_DATE,
                                            EFFECT_END_DATE,
                                            CREDIT_LIMIT,
                                            CREDIT_DAYS,
                                            COMPANY_ID,
                                            UNIT_ID,
                                            STATUS,
                                            REMARKS,
                                            ENTERED_BY,
                                            ENTERED_DATE,
                                            ENTERED_TERMINAL
                                            ) 
                                          VALUES ( :param1, :param2, :param3, TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'),TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),TO_DATE( :param6, 'DD/MM/YYYY HH:MI:SS AM'), :param7, :param8, :param9, :param10, :param11,:param12,:param13,TO_DATE(:param14, 'DD/MM/YYYY HH:MI:SS AM'), :param15)";
        string AddOrUpdate_UpdateQuery() => @"UPDATE CREDIT_INFO SET  
                                                    CUSTOMER_CODE= :param2,
                                                    CUSTOMER_ID= :param3,
                                                    ENTRY_DATE= TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'),
                                                    EFFECT_START_DATE= TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),
                                                    EFFECT_END_DATE= TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'),
                                                    CREDIT_LIMIT = :param7,
                                                    CREDIT_DAYS = :param8,
                                                    COMPANY_ID= :param9,
                                                    UNIT_ID= :param10,
                                                    STATUS= :param11,
                                                    REMARKS= :param12,
                                                    UPDATED_BY= :param13,
                                                    UPDATED_DATE= TO_DATE(:param14, 'DD/MM/YYYY HH:MI:SS AM'),
                                                    UPDATED_TERMINAL= :param15
                                                WHERE CREDIT_ID = :param1";
        string LoadCreditData_Query() => @"select  ROW_NUMBER() OVER(ORDER BY ci.Credit_Id ASC) AS ROW_NO,
                                            ci.CREDIT_ID,
                                            ci.CUSTOMER_CODE,
                                            ci.CUSTOMER_ID,
                                            TO_CHAR(ci.ENTRY_DATE, 'DD/MM/YYYY') ENTRY_DATE,
                                            TO_CHAR(ci.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE,
                                            TO_CHAR(ci.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE,
                                            ci.CREDIT_LIMIT,
                                            ci.CREDIT_DAYS,
                                            ci.COMPANY_ID,
                                            ci.UNIT_ID,
                                            ci.STATUS,
                                            ci.REMARKS,
                                            ci.ENTERED_BY,
                                            ci.ENTERED_DATE,
                                            ci.ENTERED_TERMINAL,
                                            ci.UPDATED_BY,
                                            ci.UPDATED_DATE,
                                            ci.UPDATED_TERMINAL,
                                            cusinfo.CUSTOMER_NAME
                                            from credit_info ci
                            left join CUSTOMER_INFO cusinfo on cusinfo.CUSTOMER_ID = ci.CUSTOMER_ID
                            where  ci.COMPANY_ID = :param1";


        string Get_Existing_Entry() => @"select CREDIT_ID, EFFECT_START_DATE,EFFECT_END_DATE from CREDIT_INFO WHERE CUSTOMER_ID = :param1 and TRUNC(TO_DATE(:param2,'DD/MM/YYYY')) between EFFECT_START_DATE and EFFECT_END_DATE
";
        public async Task<string> AddOrUpdate(string db, Credit_Info model)
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

                    if (model.CREDIT_ID == 0)
                    {
                        DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_Existing_Entry(), _commonServices.AddParameter(new string[] { model.CUSTOMER_ID.ToString(), model.EFFECT_START_DATE }));
                        
                        if(data.Rows.Count > 0)
                        {
                            return "Credit Setup exist between this date period";
                        }

                        model.CREDIT_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetCredit_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                         _commonServices.AddParameter(new string[] {model.CREDIT_ID.ToString(),
                             model.CUSTOMER_CODE, model.CUSTOMER_ID.ToString(),model.ENTRY_DATE,
                             model.EFFECT_START_DATE.ToString(), model.EFFECT_END_DATE, model.CREDIT_LIMIT.ToString(), model.CREDIT_DAYS.ToString(),
                             model.COMPANY_ID.ToString(), model.UNIT_ID.ToString(),model.CUSTOMER_STATUS,
                             model.REMARKS,model.ENTERED_BY,
                             model.ENTERED_DATE,  model.ENTERED_TERMINAL
                            })));

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.CREDIT_ID.ToString(),
                             model.CUSTOMER_CODE, model.CUSTOMER_ID.ToString(),model.ENTRY_DATE,
                             model.EFFECT_START_DATE.ToString(), model.EFFECT_END_DATE, model.CREDIT_LIMIT.ToString(), model.CREDIT_DAYS.ToString(),
                             model.COMPANY_ID.ToString(), model.UNIT_ID.ToString(),model.CUSTOMER_STATUS,
                             model.REMARKS,model.UPDATED_BY,
                             model.UPDATED_DATE,  model.UPDATED_TERMINAL})));
                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public Task<string> GetCustomerExistingCreditInfoData(string db, int Company_Id, int Customer_Id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> LoadCreditInfoData(string db, int Company_Id)
        {
            List<Credit_Info> credit_Info_List = new List<Credit_Info>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadCreditData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                Credit_Info credit_Info = new Credit_Info();
                credit_Info.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                credit_Info.CREDIT_ID = Convert.ToInt32(data.Rows[i]["CREDIT_ID"]);
                credit_Info.CREDIT_ID_Encrypted = _commonServices.Encrypt(data.Rows[i]["CREDIT_ID"].ToString());
                credit_Info.CUSTOMER_CODE = data.Rows[i]["CUSTOMER_CODE"].ToString();
                credit_Info.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                credit_Info.CUSTOMER_ID = data.Rows[i]["CUSTOMER_ID"].ToString();
                credit_Info.CUSTOMER_NAME = data.Rows[i]["CUSTOMER_NAME"].ToString();
                credit_Info.CREDIT_LIMIT = Convert.ToInt32(data.Rows[i]["CREDIT_LIMIT"]);
                credit_Info.CREDIT_DAYS = Convert.ToInt32(data.Rows[i]["CREDIT_DAYS"]);
                credit_Info.CUSTOMER_STATUS = data.Rows[i]["STATUS"].ToString();
                credit_Info.ENTRY_DATE = data.Rows[i]["ENTRY_DATE"].ToString();
                credit_Info.EFFECT_END_DATE = data.Rows[i]["EFFECT_END_DATE"].ToString();
                credit_Info.EFFECT_START_DATE = data.Rows[i]["EFFECT_START_DATE"].ToString();
                credit_Info.REMARKS = data.Rows[i]["REMARKS"].ToString();
                credit_Info_List.Add(credit_Info);
            }
            return JsonSerializer.Serialize(credit_Info_List);
        }

        public Task<Credit_Info> LoadCreditInfoDataList(string db, int _Id)
        {
            throw new NotImplementedException();
        }

        public Task<string> LoadData(string db, int Company_Id)
        {
            throw new NotImplementedException();
        }

        public Task<string> LoadData_Master(string db, int Company_Id)
        {
            throw new NotImplementedException();
        }
    }
}
