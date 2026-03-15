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
    public class RegionManager : IRegionManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public RegionManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.REGION_ID ASC) AS ROW_NO,
                                    M.REGION_ID, M.REGION_NAME, M.COMPANY_ID, M.REGION_CODE, M.REGION_ADDRESS, M.REMARKS, M.REGION_STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM Region_Info  M  Where M.COMPANY_ID = :param1";
        string LoadSearchableData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.REGION_ID ASC) AS ROW_NO,
                                    M.REGION_ID, M.REGION_NAME, M.COMPANY_ID, M.REGION_CODE, M.REGION_ADDRESS, M.REMARKS, M.REGION_STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM Region_Info  M  Where M.COMPANY_ID = :param1 AND upper(M.REGION_NAME) like '%' || upper(:param2) || '%'";
        string AddOrUpdate_AddQuery() => @"INSERT INTO Region_Info 
                                         (REGION_ID, REGION_NAME, REGION_CODE,REGION_ADDRESS, REMARKS,REGION_STATUS, COMPANY_ID,  Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8,TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'), :param10 )";
        string AddOrUpdate_UpdateQuery() => @"UPDATE Region_Info SET 
                                            REGION_NAME = :param2,REGION_ADDRESS = :param3,REMARKS = :param4,REGION_STATUS = :param5,
                                            UPDATED_BY = :param6, UPDATED_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param8 WHERE REGION_ID = :param1";
        string GetNewRegion_Info_IdQuery() => "SELECT NVL(MAX(REGION_ID),0) + 1 REGION_ID  FROM Region_Info";
        string Get_LastRegion_Ino() => "SELECT  REGION_ID, REGION_CODE  FROM Region_Info Where  REGION_ID = (SELECT   NVL(MAX(REGION_ID),0) REGION_ID From Region_Info where COMPANY_ID = :param1 )";


        //---------- Method Execution Part ------------------------------------------------



        public async Task<string> AddOrUpdate(string db, Region_Info model)
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

                    if (model.REGION_ID == 0)
                    {
                        //model.REGION_CODE = await GenerateRegionCode(db, model.COMPANY_ID.ToString());

                        model.REGION_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewRegion_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] { model.REGION_ID.ToString(), model.REGION_NAME, model.REGION_CODE, model.REGION_ADDRESS, model.REMARKS,model.REGION_STATUS, model.COMPANY_ID.ToString() , model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL})));

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { 
                                model.REGION_ID.ToString(), model.REGION_NAME, model.REGION_ADDRESS, model.REMARKS, model.REGION_STATUS,
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
        public async Task<string> GetSearchableRegion(string db, int Company_Id,string region) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), region })));

        public async Task<string> GenerateRegionCode(string db,string Company_Id)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastRegion_Ino(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["REGION_CODE"].ToString().Substring(1, 4)) + 1).ToString();
                    int serial_length = serial.Length;
                    code = CodeConstants.RegionInfo_CodeConst;
                    for (int i = 0; i < (CodeConstants.RegionInfo_CodeLength - (serial_length + 1) ); i++)
                    {
                        code += "0";
                    }
                    code += serial;
                }
                else
                {
                    code = CodeConstants.RegionInfo_CodeConst + "0001";
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
