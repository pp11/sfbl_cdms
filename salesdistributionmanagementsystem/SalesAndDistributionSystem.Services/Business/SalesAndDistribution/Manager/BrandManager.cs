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
    public class BrandManager : IBrandManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public BrandManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.BRAND_ID ASC) AS ROW_NO,
                                    M.BRAND_ID, M.BRAND_NAME, M.COMPANY_ID, M.BRAND_CODE , M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM BRAND_INFO  M  Where M.COMPANY_ID = :param1";
        string LoadSearchableData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.BRAND_ID ASC) AS ROW_NO,
                                    M.BRAND_ID, M.BRAND_NAME, M.COMPANY_ID, M.BRAND_CODE, M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM BRAND_INFO  M  Where M.COMPANY_ID = :param1 AND upper(M.BRAND_NAME) Like '%' || upper(:param2) || '%'";

        string AddOrUpdate_AddQuery() => @"INSERT INTO BRAND_INFO 
                                         (BRAND_ID, BRAND_NAME, BRAND_CODE, REMARKS,STATUS, COMPANY_ID,  Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7,TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'), :param9 )";
        string AddOrUpdate_UpdateQuery() => @"UPDATE BRAND_INFO SET 
                                            BRAND_NAME = :param2,REMARKS = :param3,STATUS = :param4,
                                            UPDATED_BY = :param5, UPDATED_DATE = TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param7 WHERE BRAND_ID = :param1";
        string GetNewBrand_Info_IdQuery() => "SELECT NVL(MAX(BRAND_ID),0) + 1 BRAND_ID  FROM BRAND_INFO";
        string Get_LastBrand_Ino() => "SELECT  BRAND_ID, BRAND_CODE  FROM BRAND_INFO Where  BRAND_ID = (SELECT   NVL(MAX(BRAND_ID),0) BRAND_ID From BRAND_INFO where COMPANY_ID = :param1 )";


        //---------- Method Execution Part ------------------------------------------------



        public async Task<string> AddOrUpdate(string db, Brand_Info model)
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

                    if (model.BRAND_ID == 0)
                    {
                        //model.BRAND_CODE = await GenerateBrandCode(db, model.COMPANY_ID.ToString());

                        model.BRAND_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewBrand_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] { model.BRAND_ID.ToString(), model.BRAND_NAME, model.BRAND_CODE, model.REMARKS, model.STATUS, model.COMPANY_ID.ToString(), model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.BRAND_ID.ToString(), model.BRAND_NAME, model.REMARKS, model.STATUS,
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
        public async Task<string> GetSearchableBrand(string db, int Company_Id, string brand) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), brand })));

        public async Task<string> GenerateBrandCode(string db, string Company_Id)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastBrand_Ino(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["BRAND_CODE"].ToString().Substring(1, (CodeConstants.BrandInfo_CodeLength - 1))) + 1).ToString();
                    int serial_length = serial.Length;
                    code = CodeConstants.BrandInfo_CodeConst;
                    for (int i = 0; i < (CodeConstants.BrandInfo_CodeLength - (serial_length + 1)); i++)
                    {
                        code += "0";
                    }
                    code += serial;
                }
                else
                {
                    code = CodeConstants.BrandInfo_CodeConst + "0001";
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




