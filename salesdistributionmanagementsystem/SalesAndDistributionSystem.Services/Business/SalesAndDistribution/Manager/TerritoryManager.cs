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
    public class TerritoryManager : ITerritoryManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public TerritoryManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.TERRITORY_ID ASC) AS ROW_NO,
                                    M.TERRITORY_ID, M.TERRITORY_NAME, M.COMPANY_ID, M.TERRITORY_CODE, M.TERRITORY_ADDRESS, M.REMARKS, M.TERRITORY_STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM Territory_Info  M  Where M.COMPANY_ID = :param1";
        string LoadSearchableData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.TERRITORY_ID ASC) AS ROW_NO,
                                    M.TERRITORY_ID, M.TERRITORY_NAME, M.COMPANY_ID, M.TERRITORY_CODE, M.TERRITORY_ADDRESS, M.REMARKS, M.TERRITORY_STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM Territory_Info  M  Where M.COMPANY_ID = :param1 AND upper(M.TERRITORY_NAME) Like '%' || upper(:param2) || '%'";

        string AddOrUpdate_AddQuery() => @"INSERT INTO Territory_Info 
                                         (TERRITORY_ID, TERRITORY_NAME, TERRITORY_CODE,TERRITORY_ADDRESS, REMARKS,TERRITORY_STATUS, COMPANY_ID,  Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8,TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'), :param10 )";
        string AddOrUpdate_UpdateQuery() => @"UPDATE Territory_Info SET 
                                            TERRITORY_NAME = :param2,TERRITORY_ADDRESS = :param3,REMARKS = :param4,TERRITORY_STATUS = :param5,
                                            UPDATED_BY = :param6, UPDATED_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param8 WHERE TERRITORY_ID = :param1";
        string GetNewTerritory_Info_IdQuery() => "SELECT NVL(MAX(TERRITORY_ID),0) + 1 TERRITORY_ID  FROM Territory_Info";
        string Get_LastTerritory_Ino() => "SELECT  TERRITORY_ID, TERRITORY_CODE  FROM Territory_Info Where  TERRITORY_ID = (SELECT   NVL(MAX(TERRITORY_ID),0) TERRITORY_ID From Territory_Info where COMPANY_ID = :param1 )";


        //---------- Method Execution Part ------------------------------------------------



        public async Task<string> AddOrUpdate(string db, Territory_Info model)
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

                    if (model.TERRITORY_ID == 0)
                    {
                        //model.TERRITORY_CODE = await GenerateTerritoryCode(db, model.COMPANY_ID.ToString());

                        model.TERRITORY_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewTerritory_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] { model.TERRITORY_ID.ToString(), model.TERRITORY_NAME, model.TERRITORY_CODE, model.TERRITORY_ADDRESS, model.REMARKS,model.TERRITORY_STATUS, model.COMPANY_ID.ToString() , model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL})));

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { 
                                model.TERRITORY_ID.ToString(), model.TERRITORY_NAME, model.TERRITORY_ADDRESS, model.REMARKS, model.TERRITORY_STATUS,
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
        public async Task<string> GetSearchableTerritory(string db, int Company_Id, string territory) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), territory })));

        public async Task<string> GenerateTerritoryCode(string db,string Company_Id)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastTerritory_Ino(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["TERRITORY_CODE"].ToString().Substring(1, 4)) + 1).ToString();
                    int serial_length = serial.Length;
                    code = CodeConstants.TerritoryInfo_CodeConst;
                    for (int i = 0; i < (CodeConstants.TerritoryInfo_CodeLength - (serial_length + 1) ); i++)
                    {
                        code += "0";
                    }
                    code += serial;
                }
                else
                {
                    code = CodeConstants.TerritoryInfo_CodeConst + "0001";
                }
                return code;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
