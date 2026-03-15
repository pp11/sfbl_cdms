
using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.TableModels.User;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public class DivisionManager : IDivisionManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        private readonly IUserLogManager _logManager;
        public DivisionManager(ICommonServices commonServices, IConfiguration configuration, IUserLogManager logManager)
        {
            _commonServices = commonServices;
            _configuration = configuration;
            _logManager = logManager;
        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.DIVISION_ID ASC) AS ROW_NO,
                                    M.DIVISION_ID, M.DIVISION_NAME, M.COMPANY_ID, M.DIVISION_CODE, M.DIVISION_ADDRESS, M.REMARKS, M.DIVISION_STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM DIVISION_INFO  M  Where M.COMPANY_ID = :param1";
        string LoadActiveDivisionData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.DIVISION_ID ASC) AS ROW_NO,
                                    M.DIVISION_ID, M.DIVISION_NAME, M.COMPANY_ID, M.DIVISION_CODE, M.DIVISION_ADDRESS, M.REMARKS, M.DIVISION_STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM DIVISION_INFO  M  Where M.DIVISION_STATUS = 'Active' AND M.COMPANY_ID = :param1";

        string LoadSearchableData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.DIVISION_ID ASC) AS ROW_NO,
                                    M.DIVISION_ID, M.DIVISION_NAME, M.COMPANY_ID, M.DIVISION_CODE, M.DIVISION_ADDRESS, M.REMARKS, M.DIVISION_STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM DIVISION_INFO  M  Where M.COMPANY_ID = :param1 AND upper(M.DIVISION_NAME) Like '%' || upper(:param2) || '%'";

        string AddOrUpdate_AddQuery() => @"INSERT INTO DIVISION_INFO 
                                         (DIVISION_ID, DIVISION_NAME, DIVISION_CODE,DIVISION_ADDRESS, REMARKS,DIVISION_STATUS, COMPANY_ID,  Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8,TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'), :param10 )";
        string AddOrUpdate_UpdateQuery() => @"UPDATE DIVISION_INFO SET 
                                            DIVISION_NAME = :param2,DIVISION_ADDRESS = :param3,REMARKS = :param4,DIVISION_STATUS = :param5,
                                            UPDATED_BY = :param6, UPDATED_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param8 WHERE DIVISION_ID = :param1";
        string GetNewDivision_Info_IdQuery() => "SELECT NVL(MAX(DIVISION_ID),0) + 1 DIVISION_ID  FROM DIVISION_INFO";
        string Get_LastDivision_Ino() => "SELECT  DIVISION_ID, DIVISION_CODE  FROM DIVISION_INFO Where  DIVISION_ID = (SELECT   NVL(MAX(DIVISION_ID),0) DIVISION_ID From DIVISION_INFO where COMPANY_ID = :param1 )";


        //---------- Method Execution Part ------------------------------------------------


        public async Task<string> LoadData(Division_Info division_Info)
        {
             await _logManager.AddOrUpdate(division_Info.db_security, "View", "DIVISION_INFO", division_Info.COMPANY_ID, division_Info.UNIT_ID,Convert.ToInt32(division_Info.ENTERED_BY) , division_Info.ENTERED_TERMINAL, "/SalesAndDistribution/Division/DivisionInfo",0,"");
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(division_Info.db_sales), LoadData_Query(), _commonServices.AddParameter(new string[] { division_Info.COMPANY_ID.ToString() })));

        }

        public async Task<string> LoadActiveDivisionData(Division_Info division_Info) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(division_Info.db_sales), LoadActiveDivisionData_Query(), _commonServices.AddParameter(new string[] { division_Info.COMPANY_ID.ToString() })));

        public async Task<string> AddOrUpdate( Division_Info model)
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
                    bool status = true;
                    if (model.DIVISION_ID == 0)
                    {
                        //model.DIVISION_CODE = await GenerateDivisionCode(db, model.COMPANY_ID.ToString());
                        model.DIVISION_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(model.db_sales), GetNewDivision_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] { model.DIVISION_ID.ToString(), model.DIVISION_NAME, model.DIVISION_CODE, model.DIVISION_ADDRESS, model.REMARKS, model.DIVISION_STATUS, model.COMPANY_ID.ToString(), model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));
                        status = false;
                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.DIVISION_ID.ToString(), model.DIVISION_NAME, model.DIVISION_ADDRESS, model.REMARKS, model.DIVISION_STATUS,
                                model.UPDATED_BY.ToString(), model.UPDATED_DATE,
                                model.UPDATED_TERMINAL })));

                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(model.db_sales), listOfQuery);


                    string st = status == false ? "Add" : "Edit";
                    await _logManager.AddOrUpdate(model.db_security, st, "DIVISION_INFO", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/SalesAndDistribution/Division/InsertOrEdit", model.DIVISION_ID, "");

                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<string> GetSearchableDivision(Division_Info division_Info) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(division_Info.db_sales), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { division_Info.COMPANY_ID.ToString(), division_Info.DIVISION_NAME })));

        public async Task<string> GenerateDivisionCode(Division_Info division_Info)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(division_Info.db_sales), Get_LastDivision_Ino(), _commonServices.AddParameter(new string[] { division_Info.COMPANY_ID.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["DIVISION_CODE"].ToString().Substring(1, (CodeConstants.DivisionInfo_CodeLength - 1))) + 1).ToString();
                    int serial_length = serial.Length;
                    code = CodeConstants.DivisionInfo_CodeConst;
                    for (int i = 0; i < (CodeConstants.DivisionInfo_CodeLength - (serial_length + 1)); i++)
                    {
                        code += "0";
                    }
                    code += serial;
                }
                else
                {
                    code = CodeConstants.DivisionInfo_CodeConst + "0001";
                }
                return code;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
