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
    public class StorageManager : IStorageManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public StorageManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.STORAGE_ID ASC) AS ROW_NO,
                                    M.STORAGE_ID, M.STORAGE_NAME, M.COMPANY_ID , M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM STORAGE_INFO  M  Where M.COMPANY_ID = :param1";
        string LoadSearchableData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.STORAGE_ID ASC) AS ROW_NO,
                                    M.STORAGE_ID, M.STORAGE_NAME, M.COMPANY_ID, M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM STORAGE_INFO  M  Where M.COMPANY_ID = :param1 AND upper(M.STORAGE_NAME) Like '%' || upper(:param2) || '%'";

        string AddOrUpdate_AddQuery() => @"INSERT INTO STORAGE_INFO 
                                         (STORAGE_ID, STORAGE_NAME, REMARKS,STATUS, COMPANY_ID,  Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6,TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), :param8 )";
        string AddOrUpdate_UpdateQuery() => @"UPDATE STORAGE_INFO SET 
                                            STORAGE_NAME = :param2,REMARKS = :param3,STATUS = :param4,
                                            UPDATED_BY = :param5, UPDATED_DATE = TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param7 WHERE STORAGE_ID = :param1";
        string GetNewStorage_Info_IdQuery() => "SELECT NVL(MAX(STORAGE_ID),0) + 1 STORAGE_ID  FROM STORAGE_INFO";
        string Get_LastStorage_Ino() => "SELECT  STORAGE_ID  FROM STORAGE_INFO Where  STORAGE_ID = (SELECT   NVL(MAX(STORAGE_ID),0) STORAGE_ID From STORAGE_INFO where COMPANY_ID = :param1 )";


        //---------- Method Execution Part ------------------------------------------------



        public async Task<string> AddOrUpdate(string db, Storage_Info model)
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

                    if (model.STORAGE_ID == 0)
                    {

                        model.STORAGE_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewStorage_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] { model.STORAGE_ID.ToString(), model.STORAGE_NAME, model.REMARKS, model.STATUS, model.COMPANY_ID.ToString(), model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.STORAGE_ID.ToString(), model.STORAGE_NAME, model.REMARKS, model.STATUS,
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
        public async Task<string> GetSearchableStorage(string db, int Company_Id, string Storage) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Storage })));


    }
}




