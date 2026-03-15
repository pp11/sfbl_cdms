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
    public class BaseProductManager : IBaseProductManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public BaseProductManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.BASE_PRODUCT_ID ASC) AS ROW_NO,
                                    M.BASE_PRODUCT_ID, M.BASE_PRODUCT_NAME, M.COMPANY_ID, M.BASE_PRODUCT_CODE , M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM BASE_PRODUCT_INFO  M  Where M.COMPANY_ID = :param1";
        string LoadSearchableData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.BASE_PRODUCT_ID ASC) AS ROW_NO,
                                    M.BASE_PRODUCT_ID, M.BASE_PRODUCT_NAME, M.COMPANY_ID, M.BASE_PRODUCT_CODE, M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM BASE_PRODUCT_INFO  M  Where M.COMPANY_ID = :param1 AND upper(M.BASE_PRODUCT_NAME) Like '%' || upper(:param2) || '%'";

        string AddOrUpdate_AddQuery() => @"INSERT INTO BASE_PRODUCT_INFO 
                                         (BASE_PRODUCT_ID, BASE_PRODUCT_NAME, BASE_PRODUCT_CODE, REMARKS,STATUS, COMPANY_ID,  Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7,TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'), :param9 )";
        string AddOrUpdate_UpdateQuery() => @"UPDATE BASE_PRODUCT_INFO SET 
                                            BASE_PRODUCT_NAME = :param2,REMARKS = :param3,STATUS = :param4,
                                            UPDATED_BY = :param5, UPDATED_DATE = TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param7 WHERE BASE_PRODUCT_ID = :param1";
        string GetNewBase_Product_Info_IdQuery() => "SELECT NVL(MAX(BASE_PRODUCT_ID),0) + 1 BASE_PRODUCT_ID  FROM BASE_PRODUCT_INFO";
        string Get_LastBase_Product_Info() => "SELECT  BASE_PRODUCT_ID, BASE_PRODUCT_CODE  FROM BASE_PRODUCT_INFO Where  BASE_PRODUCT_ID = (SELECT   NVL(MAX(BASE_PRODUCT_ID),0) BASE_PRODUCT_ID From BASE_PRODUCT_INFO where COMPANY_ID = :param1 )";


        //---------- Method Execution Part ------------------------------------------------



        public async Task<string> AddOrUpdate(string db, Base_Product_Info model)
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

                    if (model.BASE_PRODUCT_ID == 0)
                    {
                        model.BASE_PRODUCT_CODE = await GenerateBase_ProductCode(db, model.COMPANY_ID.ToString());

                        model.BASE_PRODUCT_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewBase_Product_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] { model.BASE_PRODUCT_ID.ToString(), model.BASE_PRODUCT_NAME, model.BASE_PRODUCT_CODE, model.REMARKS, model.STATUS, model.COMPANY_ID.ToString(), model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.BASE_PRODUCT_ID.ToString(), model.BASE_PRODUCT_NAME, model.REMARKS, model.STATUS,
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
        public async Task<string> GetSearchableBase_Product(string db, int Company_Id, string base_Product) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), base_Product })));

        public async Task<string> GenerateBase_ProductCode(string db, string Company_Id)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastBase_Product_Info(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["BASE_PRODUCT_CODE"].ToString().Substring(1, (CodeConstants.BaseProductInfo_CodeLength-1))) + 1).ToString();
                    int serial_length = serial.Length;
                    code = CodeConstants.BaseProductInfo_CodeConst;
                    for (int i = 0; i < (CodeConstants.BaseProductInfo_CodeLength - (serial_length + 1)); i++)
                    {
                        code += "0";
                    }
                    code += serial;
                }
                else
                {
                    code = CodeConstants.BaseProductInfo_CodeConst + "0001";
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




