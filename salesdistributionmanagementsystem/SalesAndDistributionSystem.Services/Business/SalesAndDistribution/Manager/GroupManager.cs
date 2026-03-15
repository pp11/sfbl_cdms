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

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public class GroupManager : IGroupManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public GroupManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.GROUP_ID ASC) AS ROW_NO,
                                    M.GROUP_ID, M.GROUP_NAME, M.COMPANY_ID, M.GROUP_CODE , M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM GROUP_INFO  M  Where M.COMPANY_ID = :param1";
        string LoadSearchableData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.GROUP_ID ASC) AS ROW_NO,
                                    M.GROUP_ID, M.GROUP_NAME, M.COMPANY_ID, M.GROUP_CODE, M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM GROUP_INFO  M  Where M.COMPANY_ID = :param1 AND upper(M.GROUP_NAME) Like '%' || upper(:param2) || '%'";

        string AddOrUpdate_AddQuery() => @"INSERT INTO GROUP_INFO 
                                         (GROUP_ID, GROUP_NAME, GROUP_CODE, REMARKS,STATUS, COMPANY_ID,  Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7,TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'), :param9 )";
        string AddOrUpdate_UpdateQuery() => @"UPDATE GROUP_INFO SET 
                                            GROUP_NAME = :param2,REMARKS = :param3,STATUS = :param4,
                                            UPDATED_BY = :param5, UPDATED_DATE = TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param7 WHERE GROUP_ID = :param1";
        string GetNewGroup_Info_IdQuery() => "SELECT NVL(MAX(GROUP_ID),0) + 1 GROUP_ID  FROM GROUP_INFO";
        string Get_LastGroup_Ino() => "SELECT  GROUP_ID, GROUP_CODE  FROM GROUP_INFO Where  GROUP_ID = (SELECT   NVL(MAX(GROUP_ID),0) GROUP_ID From GROUP_INFO where COMPANY_ID = :param1 )";


        //---------- Method Execution Part ------------------------------------------------



        public async Task<string> AddOrUpdate(string db, Group_Info model)
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

                    if (model.GROUP_ID == 0)
                    {
                        //model.GROUP_CODE = await GenerateGroupCode(db, model.COMPANY_ID.ToString());

                        model.GROUP_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewGroup_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] { model.GROUP_ID.ToString(), model.GROUP_NAME, model.GROUP_CODE, model.REMARKS, model.STATUS, model.COMPANY_ID.ToString(), model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.GROUP_ID.ToString(), model.GROUP_NAME, model.REMARKS, model.STATUS,
                                model.UPDATED_BY.ToString(), model.UPDATED_DATE,
                                model.UPDATED_TERMINAL })));

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

        public async Task<string> LoadData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> GetSearchableGroup(string db, int Company_Id, string Group) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Group })));

        public async Task<string> GenerateGroupCode(string db, string Company_Id)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastGroup_Ino(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["GROUP_CODE"].ToString().Substring(1, (CodeConstants.GroupInfo_CodeLength - 1))) + 1).ToString();
                    int serial_length = serial.Length;
                    code = CodeConstants.GroupInfo_CodeConst;
                    for (int i = 0; i < (CodeConstants.GroupInfo_CodeLength - (serial_length + 1)); i++)
                    {
                        code += "0";
                    }
                    code += serial;
                }
                else
                {
                    code = CodeConstants.GroupInfo_CodeConst + "0001";
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




