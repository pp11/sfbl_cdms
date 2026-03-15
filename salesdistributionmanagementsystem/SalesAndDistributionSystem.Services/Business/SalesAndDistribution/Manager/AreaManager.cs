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
    public class AreaManager : IAreaManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public AreaManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.AREA_ID ASC) AS ROW_NO,
                                    M.AREA_ID, M.AREA_NAME, M.COMPANY_ID, M.AREA_CODE, M.AREA_ADDRESS, M.REMARKS, M.AREA_STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM AREA_INFO  M  Where M.COMPANY_ID = :param1";
        string LoadSearchableData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.AREA_ID ASC) AS ROW_NO,
                                    M.AREA_ID, M.AREA_NAME, M.COMPANY_ID, M.AREA_CODE, M.AREA_ADDRESS, M.REMARKS, M.AREA_STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM AREA_INFO  M  Where M.COMPANY_ID = :param1 AND upper(M.AREA_NAME) Like '%' || upper(:param2) || '%'";

        string AddOrUpdate_AddQuery() => @"INSERT INTO AREA_INFO 
                                         (AREA_ID, AREA_NAME, AREA_CODE,AREA_ADDRESS, REMARKS,AREA_STATUS, COMPANY_ID,  Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8,TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'), :param10 )";
        string AddOrUpdate_UpdateQuery() => @"UPDATE AREA_INFO SET 
                                            AREA_NAME = :param2,AREA_ADDRESS = :param3,REMARKS = :param4,AREA_STATUS = :param5,
                                            UPDATED_BY = :param6, UPDATED_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param8 WHERE AREA_ID = :param1";
        string GetNewArea_Info_IdQuery() => "SELECT NVL(MAX(AREA_ID),0) + 1 AREA_ID  FROM AREA_INFO";
        string Get_LastArea_Ino() => "SELECT  AREA_ID, AREA_CODE  FROM AREA_INFO Where  AREA_ID = (SELECT   NVL(MAX(AREA_ID),0) AREA_ID From AREA_INFO where COMPANY_ID = :param1 )";


        //---------- Method Execution Part ------------------------------------------------



        public async Task<string> AddOrUpdate(string db, Area_Info model)
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

                    if (model.AREA_ID == 0)
                    {
                        //model.AREA_CODE = await GenerateAreaCode(db, model.COMPANY_ID.ToString());

                        model.AREA_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewArea_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] { model.AREA_ID.ToString(), model.AREA_NAME, model.AREA_CODE, model.AREA_ADDRESS, model.REMARKS, model.AREA_STATUS, model.COMPANY_ID.ToString(), model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.AREA_ID.ToString(), model.AREA_NAME, model.AREA_ADDRESS, model.REMARKS, model.AREA_STATUS,
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
        public async Task<string> GetSearchableArea(string db, int Company_Id, string Area) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Area })));

        public async Task<string> GenerateAreaCode(string db, string Company_Id)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastArea_Ino(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["AREA_CODE"].ToString().Substring( 1 ,CodeConstants.AreaInfo_CodeLength-1)) + 1).ToString();
                    int serial_length = serial.Length;
                    code = CodeConstants.AreaInfo_CodeConst;
                    for (int i = 0; i < (CodeConstants.AreaInfo_CodeLength - (serial_length + 1)); i++)
                    {
                        code += "0";
                    }
                    code += serial;
                }
                else
                {
                    code = CodeConstants.AreaInfo_CodeConst + "0001";
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
