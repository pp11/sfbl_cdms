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
    public class PackSizeManager : IPackSizeManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public PackSizeManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.PACK_SIZE_ID ASC) AS ROW_NO,
                                    M.PACK_SIZE_ID, M.PACK_SIZE_NAME, M.COMPANY_ID , M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM PACK_SIZE_INFO  M  Where M.COMPANY_ID = :param1";
        string LoadSearchableData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.PACK_SIZE_ID ASC) AS ROW_NO,
                                    M.PACK_SIZE_ID, M.PACK_SIZE_NAME, M.COMPANY_ID, M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM PACK_SIZE_INFO  M  Where M.COMPANY_ID = :param1 AND upper(M.PACK_SIZE_NAME) Like '%' || upper(:param2) || '%'";

        string AddOrUpdate_AddQuery() => @"INSERT INTO PACK_SIZE_INFO 
                                         (PACK_SIZE_ID, PACK_SIZE_NAME, REMARKS,STATUS, COMPANY_ID,  Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6,TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), :param8 )";
        string AddOrUpdate_UpdateQuery() => @"UPDATE PACK_SIZE_INFO SET 
                                            PACK_SIZE_NAME = :param2,REMARKS = :param3,STATUS = :param4,
                                            UPDATED_BY = :param5, UPDATED_DATE = TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param7 WHERE PACK_SIZE_ID = :param1";
        string GetNewPackSize_Info_IdQuery() => "SELECT NVL(MAX(PACK_SIZE_ID),0) + 1 PACK_SIZE_ID  FROM PACK_SIZE_INFO";
        string Get_LastPackSize_Ino() => "SELECT  PACK_SIZE_ID  FROM PACK_SIZE_INFO Where  PACK_SIZE_ID = (SELECT   NVL(MAX(PACK_SIZE_ID),0) PACK_SIZE_ID From PACK_SIZE_INFO where COMPANY_ID = :param1 )";


        //---------- Method Execution Part ------------------------------------------------



        public async Task<string> AddOrUpdate(string db, Pack_Size_Info model)
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

                    if (model.PACK_SIZE_ID == 0)
                    {

                        model.PACK_SIZE_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewPackSize_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] { model.PACK_SIZE_ID.ToString(), model.PACK_SIZE_NAME, model.REMARKS, model.STATUS, model.COMPANY_ID.ToString(), model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.PACK_SIZE_ID.ToString(), model.PACK_SIZE_NAME, model.REMARKS, model.STATUS,
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
        public async Task<string> GetSearchablePackSize(string db, int Company_Id, string PackSize) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), PackSize })));


    }
}




