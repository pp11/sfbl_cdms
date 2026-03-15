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

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.Manager
{
    public class DriverManager : IDriverManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public DriverManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;
        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT ROW_NUMBER () OVER (ORDER BY M.DRIVER_ID ASC) AS ROW_NO,
                       DRIVER_ID,
                       DRIVER_NAME,
                       CONTACT_NO,
                       STATUS,
                       COMPANY_ID,
                       REMARKS,
                       FN_UNIT_NAME(COMPANY_ID, UNIT_ID) UNIT_NAME,
                       TO_CHAR (M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE,
                       UNIT_ID
                  FROM DRIVER_INFO M
                 WHERE M.COMPANY_ID = :param1";

        string LoadSearchableData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.DRIVER_ID ASC) AS ROW_NO,
                                    DRIVER_ID, DRIVER_NAME, CONTACT_NO, STATUS, COMPANY_ID, REMARKS, TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM DRIVER_INFO  M  Where M.COMPANY_ID = :param1 AND upper(M.DRIVER_NAME) Like '%' || upper(:param2) || '%'";

        string AddOrUpdate_AddQuery() => @"INSERT INTO DRIVER_INFO 
                                         (DRIVER_ID, DRIVER_NAME, CONTACT_NO, STATUS, COMPANY_ID, REMARKS,  Entered_By, Entered_Date,ENTERED_TERMINAL, UNIT_ID ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'), :param9, :param10 )";

        string AddOrUpdate_UpdateQuery() => @"UPDATE DRIVER_INFO
   SET DRIVER_NAME = :param2,
       CONTACT_NO = :param3,
       STATUS = :param4,
       COMPANY_ID = :param5,
       REMARKS = :param6,
       UPDATED_BY = :param7,
       UPDATED_DATE = TO_DATE ( :param8, 'DD/MM/YYYY HH:MI:SS AM'),
       UPDATED_TERMINAL = :param9,UNIT_ID = :param10 WHERE DRIVER_ID = :param1";
        string GetNewDriver_Info_IdQuery() => "SELECT NVL(MAX(DRIVER_ID),0) + 1 DRIVER_ID  FROM DRIVER_INFO";
        string Get_LastDriver_Info() => "SELECT  DRIVER_ID  FROM DRIVER_INFO Where  DRIVER_ID = (SELECT NVL(MAX(DRIVER_ID),0) DRIVER_ID From DRIVER_INFO where COMPANY_ID = :param1 )";


        //---------- Method Execution Part ------------------------------------------------

        public async Task<string> AddOrUpdate(string db, Driver_Info model)
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
                    if (model.DRIVER_ID == 0)
                    {
                        model.DRIVER_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewDriver_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] { model.DRIVER_ID.ToString(), model.DRIVER_NAME, model.CONTACT_NO, model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS, model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL, model.UNIT_ID.ToString() }))); ;
                    }
                    else
                    {
                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.DRIVER_ID.ToString(), model.DRIVER_NAME, model.CONTACT_NO, model.STATUS, model.COMPANY_ID.ToString(),model.REMARKS,
                                model.UPDATED_BY.ToString(), model.UPDATED_DATE,
                                model.UPDATED_TERMINAL, model.UNIT_ID.ToString() })));
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
        public async Task<string> GetSearchableDriver(string db, int Company_Id, string Driver) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Driver })));


    }
}
