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
    public class MarketManager : IMarketManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public MarketManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;
        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MARKET_ID ASC) AS ROW_NO,
                                    M.MARKET_ID, M.MARKET_NAME, M.COMPANY_ID, M.MARKET_CODE, M.MARKET_ADDRESS, M.REMARKS, M.MARKET_STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM MARKET_INFO  M  Where M.COMPANY_ID = :param1";
        string MarketDropDownData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MARKET_ID ASC) AS ROW_NO,
                                    M.MARKET_ID, M.MARKET_NAME, M.COMPANY_ID, M.MARKET_CODE, M.MARKET_ADDRESS, M.REMARKS, M.MARKET_STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM MARKET_INFO  M  Where M.COMPANY_ID = :param1";
        string GetDivitionToMarketRelation_Query() => @"SELECT DISTINCT M.COMPANY_ID, M.DIVISION_ID, M.DIVISION_NAME ||  ' | '  ||  DIVISION_CODE   DIVISION_NAME, 
M.REGION_ID, M.REGION_NAME ||  ' | '  ||  REGION_CODE REGION_NAME, M.AREA_ID , M.AREA_NAME  ||  ' | '  ||  AREA_CODE AREA_NAME, 
M.TERRITORY_ID, M.TERRITORY_NAME  ||  ' | '  ||  TERRITORY_CODE TERRITORY_NAME, M.MARKET_ID , M.MARKET_NAME ||  ' | '  ||  MARKET_CODE MARKET_NAME ,M.CUSTOMER_ID, M.CUSTOMER_NAME ||  ' | '  ||  CUSTOMER_CODE CUSTOMER_NAME
From VW_DIVISION_TO_CUST_RELATION M  
order by M.DIVISION_ID, M.REGION_ID,  M.AREA_ID, M.TERRITORY_ID,  M.MARKET_ID";
        string LoadActiveMarkets_Query() => @"SELECT ROWNUM AS ROW_NO,
                                    M.MARKET_ID, M.MARKET_NAME, M.COMPANY_ID, M.MARKET_CODE, M.MARKET_ADDRESS, M.REMARKS, M.MARKET_STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM MARKET_INFO  M 
                                
                                    
                                     Where M.MARKET_STATUS = 'Active' AND M.COMPANY_ID = :param1 Order By M.MARKET_NAME ASC";
        string LoadSearchableData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MARKET_ID ASC) AS ROW_NO,
                                    M.MARKET_ID, M.MARKET_NAME, M.COMPANY_ID, M.MARKET_CODE, M.MARKET_ADDRESS, M.REMARKS, M.MARKET_STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM MARKET_INFO  M  Where M.COMPANY_ID = :param1 AND upper(M.MARKET_NAME) Like '%' || upper(:param2) || '%'";

        string AddOrUpdate_AddQuery() => @"INSERT INTO MARKET_INFO 
                                         (MARKET_ID, MARKET_NAME, MARKET_CODE,MARKET_ADDRESS, REMARKS,MARKET_STATUS, COMPANY_ID,  Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8,TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'), :param10 )";
        string AddOrUpdate_UpdateQuery() => @"UPDATE MARKET_INFO SET 
                                            MARKET_NAME = :param2,MARKET_ADDRESS = :param3,REMARKS = :param4,MARKET_STATUS = :param5,
                                            UPDATED_BY = :param6, UPDATED_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param8 WHERE MARKET_ID = :param1";
        string GetNewMarket_Info_IdQuery() => "SELECT NVL(MAX(MARKET_ID),0) + 1 MARKET_ID  FROM MARKET_INFO";
        string Get_LastMarket_Ino() => "SELECT  MARKET_ID, MARKET_CODE  FROM MARKET_INFO Where  MARKET_ID = (SELECT   NVL(MAX(MARKET_ID),0) MARKET_ID From MARKET_INFO where COMPANY_ID = :param1 )";


        //---------- Method Execution Part ------------------------------------------------



        public async Task<string> AddOrUpdate(string db, Market_Info model)
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

                    if (model.MARKET_ID == 0)
                    {
                        //model.MARKET_CODE = await GenerateMarketCode(db, model.COMPANY_ID.ToString());

                        model.MARKET_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewMarket_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] { model.MARKET_ID.ToString(), model.MARKET_NAME, model.MARKET_CODE, model.MARKET_ADDRESS, model.REMARKS,model.MARKET_STATUS, model.COMPANY_ID.ToString() , model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL})));

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { 
                                model.MARKET_ID.ToString(), model.MARKET_NAME, model.MARKET_ADDRESS, model.REMARKS, model.MARKET_STATUS,
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
        
        public async Task<string> LoadActiveMarkets(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadActiveMarkets_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));

        public async Task<string> LoadData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> MarketDropDownDataData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), MarketDropDownData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> GetSearchableMarket(string db, int Company_Id, string market) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), market })));

        public async Task<string> GenerateMarketCode(string db,string Company_Id)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastMarket_Ino(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["MARKET_CODE"].ToString().Substring(1, (CodeConstants.MarketInfo_CodeLength - 1))) + 1).ToString();
                    int serial_length = serial.Length;
                    code = CodeConstants.MarketInfo_CodeConst;
                    for (int i = 0; i < (CodeConstants.MarketInfo_CodeLength - (serial_length + 1) ); i++)
                    {
                        code += "0";
                    }
                    code += serial;
                }
                else
                {
                    code = CodeConstants.MarketInfo_CodeConst + "0001";
                }
                return code;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        
        public async Task<string> GetDivitionToMarketRelation(string db/*, int ROW_NO*/) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetDivitionToMarketRelation_Query(), _commonServices.AddParameter(new string[] {/* ROW_NO.ToString()*/ })));

    }
}
