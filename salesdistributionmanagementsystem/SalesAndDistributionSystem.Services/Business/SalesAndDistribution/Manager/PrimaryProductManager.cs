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
    public class PrimaryProductManager : IPrimaryProductManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public PrimaryProductManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.PRIMARY_PRODUCT_ID ASC) AS ROW_NO,
                                    M.PRIMARY_PRODUCT_ID, M.PRIMARY_PRODUCT_NAME, M.COMPANY_ID, M.PRIMARY_PRODUCT_CODE , M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM PRIMARY_PRODUCT_INFO  M  Where M.COMPANY_ID = :param1";
        string LoadSearchableData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.PRIMARY_PRODUCT_ID ASC) AS ROW_NO,
                                    M.PRIMARY_PRODUCT_ID, M.PRIMARY_PRODUCT_NAME, M.COMPANY_ID, M.PRIMARY_PRODUCT_CODE, M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM PRIMARY_PRODUCT_INFO  M  Where M.COMPANY_ID = :param1 AND upper(M.PRIMARY_PRODUCT_NAME) Like '%' || upper(:param2) || '%'";

        string AddOrUpdate_AddQuery() => @"INSERT INTO PRIMARY_PRODUCT_INFO 
                                         (PRIMARY_PRODUCT_ID, PRIMARY_PRODUCT_NAME, PRIMARY_PRODUCT_CODE, REMARKS,STATUS, COMPANY_ID,  Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7,TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'), :param9 )";
        string AddOrUpdate_UpdateQuery() => @"UPDATE PRIMARY_PRODUCT_INFO SET 
                                            PRIMARY_PRODUCT_NAME = :param2,REMARKS = :param3,STATUS = :param4,
                                            UPDATED_BY = :param5, UPDATED_DATE = TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param7 WHERE PRIMARY_PRODUCT_ID = :param1";
        string GetNewPrimaryProduct_Info_IdQuery() => "SELECT NVL(MAX(PRIMARY_PRODUCT_ID),0) + 1 PRIMARY_PRODUCT_ID  FROM PRIMARY_PRODUCT_INFO";
        string Get_LastPrimaryProduct_Ino() => "SELECT  PRIMARY_PRODUCT_ID, PRIMARY_PRODUCT_CODE  FROM PRIMARY_PRODUCT_INFO Where  PRIMARY_PRODUCT_ID = (SELECT   NVL(MAX(PRIMARY_PRODUCT_ID),0) PRIMARY_PRODUCT_ID From PRIMARY_PRODUCT_INFO where COMPANY_ID = :param1 )";


        //---------- Method Execution Part ------------------------------------------------



        public async Task<string> AddOrUpdate(string db, Primary_Product_Info model)
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

                    if (model.PRIMARY_PRODUCT_ID == 0)
                    {
                        //model.PRIMARY_PRODUCT_CODE = await GeneratePrimaryProductCode(db, model.COMPANY_ID.ToString());

                        model.PRIMARY_PRODUCT_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewPrimaryProduct_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] { model.PRIMARY_PRODUCT_ID.ToString(), model.PRIMARY_PRODUCT_NAME, model.PRIMARY_PRODUCT_CODE, model.REMARKS, model.STATUS, model.COMPANY_ID.ToString(), model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.PRIMARY_PRODUCT_ID.ToString(), model.PRIMARY_PRODUCT_NAME, model.REMARKS, model.STATUS,
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
        public async Task<string> GetSearchablePrimaryProduct(string db, int Company_Id, string PrimaryProduct) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), PrimaryProduct })));

        public async Task<string> GeneratePrimaryProductCode(string db, string Company_Id)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastPrimaryProduct_Ino(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["PRIMARY_PRODUCT_CODE"].ToString().Substring(1, (CodeConstants.PrimaryProductInfo_CodeLength - 1))) + 1).ToString();
                    int serial_length = serial.Length;
                    code = CodeConstants.PrimaryProductInfo_CodeConst;
                    for (int i = 0; i < (CodeConstants.PrimaryProductInfo_CodeLength - (serial_length + 1)); i++)
                    {
                        code += "0";
                    }
                    code += serial;
                }
                else
                {
                    code = CodeConstants.PrimaryProductInfo_CodeConst + "0001";
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




