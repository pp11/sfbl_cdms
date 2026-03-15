using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using SalesAndDistributionSystem.Domain.Models.TableModels.User;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.User
{
    public class UserLogManager : IUserLogManager
    {
        private readonly IConfiguration _configuration;
        private readonly ICommonServices _commonServices;


        public UserLogManager(IConfiguration connstring, ICommonServices commonServices)
        {
            _configuration = connstring;
            _commonServices = commonServices;
        }


        string LoadData_Query() => @"SELECT ROWNUM ROW_NO, L.LOG_ID, L.LOG_DATE, L.USER_ID, L.USER_TERMINAL, L.ACTIVITY_TYPE, 
            L.ACTIVITY_TABLE, L.TRANSACTION_ID, L.PAGE_REF, L.LOCATION, L.DTL, L.COMPANY_ID,
            U.USER_NAME, U.USER_TYPE, U.EMPLOYEE_ID
            FROM USER_LOG L
            LEFT JOIN USER_INFO U ON U.USER_ID = L.USER_ID
             where L.COMPANY_ID = :param1 ";

         string GetNewUSER_Log_IDQuery() => "SELECT NVL(MAX(LOG_ID),0) + 1 LOG_ID  FROM USER_LOG";
        string AddOrUpdatyeInsertQuery() => @"INSERT INTO USER_LOG (
                             ACTIVITY_TABLE
                            ,ACTIVITY_TYPE
                            ,COMPANY_ID
                            ,DTL
                            ,LOCATION
                            ,LOG_DATE
                            ,LOG_ID
                            ,PAGE_REF
                            ,TRANSACTION_ID
                            ,USER_ID
                            ,USER_TERMINAL
                            ,ENTERED_BY
                            ,ENTERED_DATE
                            ,ENTERED_TERMINAL
                            ,UNIT_ID

                            ) 
                       VALUES(:param1 ,:param2  ,:param3  ,:param4,:param5  ,TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'),:param7,:param8,:param9,:param10,:param11,:param12,TO_DATE(:param13, 'DD/MM/YYYY HH:MI:SS AM'),:param14, :param15 )";
         
        public async Task<string> AddOrUpdate(string db,string activity_type, string activity_table,int CompanyId,int UnitId, int UserId,string terminal,string page_link,int tran_id, string dtl)
        {
            
                User_Log model = new User_Log();
                model.ACTIVITY_TABLE = activity_table;
                model.ACTIVITY_TYPE = activity_type;
                model.COMPANY_ID = CompanyId;
                model.DTL = dtl;
                model.ENTERED_BY = UserId.ToString();
                model.ENTERED_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                model.ENTERED_TERMINAL = terminal;
                model.LOCATION = terminal;
                model.LOG_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                model.LOG_ID = 0;
                model.PAGE_REF = page_link;
                model.TRANSACTION_ID = tran_id;
                model.UNIT_ID = UnitId;
                model.USER_ID = UserId;
                model.USER_TERMINAL = terminal;




                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {

                    if (model.LOG_ID == 0)
                    {

                        model.LOG_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewUSER_Log_IDQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdatyeInsertQuery(), 
                            _commonServices.AddParameter(new string[] {
                                model.ACTIVITY_TABLE
                               ,model.ACTIVITY_TYPE
                               ,model.COMPANY_ID.ToString()
                               ,model.DTL
                               ,model.LOCATION
                               ,model.LOG_DATE
                               ,model.LOG_ID.ToString()
                               ,model.PAGE_REF
                               ,model.TRANSACTION_ID.ToString()
                               ,model.USER_ID.ToString()
                               ,model.USER_TERMINAL
                               ,model.ENTERED_BY
                               ,model.ENTERED_DATE
                               ,model.ENTERED_TERMINAL,
                                model.UNIT_ID.ToString()
                            })));
                    }
                    

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            

        }
        public async Task<string> LoadData(string db, string companyId) => _commonServices.DataTableToJSON( await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Query(), _commonServices.AddParameter(new string[] { companyId })));

        public async Task<string> Search(string db, int companyId, SearchModel model)
        {
            List<string> parameters = new List<string>();
            parameters.Add(companyId.ToString());
            var sql = @"SELECT ROWNUM ROW_NO, L.LOG_ID, L.LOG_DATE, L.USER_ID, L.USER_TERMINAL, L.ACTIVITY_TYPE, 
                L.ACTIVITY_TABLE, L.TRANSACTION_ID, L.PAGE_REF, L.LOCATION, L.DTL, L.COMPANY_ID,
                U.USER_NAME, U.USER_TYPE, U.EMPLOYEE_ID
                FROM USER_LOG L
                LEFT JOIN USER_INFO U ON U.USER_ID = L.USER_ID
            where L.COMPANY_ID = :param1 ";

            if (!string.IsNullOrWhiteSpace(model.USER_ID) && model.USER_ID != "0")
            {
                sql += " AND L.USER_ID = :param2";
                parameters.Add(model.USER_ID);
            }
            if (!string.IsNullOrWhiteSpace(model.FROM_DATE))
            {
                sql += " AND trunc(L.LOG_DATE) >= TO_DATE(:param3, 'DD/MM/YYYY')";
                parameters.Add(model.FROM_DATE);
            }
            if (!string.IsNullOrWhiteSpace(model.TO_DATE))
            {
                sql += " AND trunc(L.LOG_DATE) <= TO_DATE(:param4, 'DD/MM/YYYY')";
                parameters.Add(model.TO_DATE);
            }

            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), sql, _commonServices.AddParameter(parameters.ToArray())));
        }
    }
}
