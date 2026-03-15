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
    public class MeasuringUnitManager : IMeasuringUnitManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public MeasuringUnitManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MEASURING_UNIT_ID ASC) AS ROW_NO,
                                    M.MEASURING_UNIT_ID, M.MEASURING_UNIT_NAME, M.COMPANY_ID , M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM MEASURING_UNIT_INFO  M  Where M.COMPANY_ID = :param1";
        string LoadSearchableData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MEASURING_UNIT_ID ASC) AS ROW_NO,
                                    M.MEASURING_UNIT_ID, M.MEASURING_UNIT_NAME, M.COMPANY_ID, M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM MEASURING_UNIT_INFO  M  Where M.COMPANY_ID = :param1 AND upper(M.MEASURING_UNIT_NAME) Like '%' || upper(:param2) || '%'";

        string AddOrUpdate_AddQuery() => @"INSERT INTO MEASURING_UNIT_INFO 
                                         (MEASURING_UNIT_ID, MEASURING_UNIT_NAME, REMARKS,STATUS, COMPANY_ID,  Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6,TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), :param8 )";
        string AddOrUpdate_UpdateQuery() => @"UPDATE MEASURING_UNIT_INFO SET 
                                            MEASURING_UNIT_NAME = :param2,REMARKS = :param3,STATUS = :param4,
                                            UPDATED_BY = :param5, UPDATED_DATE = TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param7 WHERE MEASURING_UNIT_ID = :param1";
        string GetNewMeasuringUnit_Info_IdQuery() => "SELECT NVL(MAX(MEASURING_UNIT_ID),0) + 1 MEASURING_UNIT_ID  FROM MEASURING_UNIT_INFO";
        string Get_LastMeasuringUnit_Ino() => "SELECT  MEASURING_UNIT_ID  FROM MEASURING_UNIT_INFO Where  MEASURING_UNIT_ID = (SELECT   NVL(MAX(MEASURING_UNIT_ID),0) MEASURING_UNIT_ID From MEASURING_UNIT_INFO where COMPANY_ID = :param1 )";


        //---------- Method Execution Part ------------------------------------------------



        public async Task<string> AddOrUpdate(string db, Measuring_Unit_Info model)
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

                    if (model.MEASURING_UNIT_ID == 0)
                    {

                        model.MEASURING_UNIT_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewMeasuringUnit_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] { model.MEASURING_UNIT_ID.ToString(), model.MEASURING_UNIT_NAME, model.REMARKS, model.STATUS, model.COMPANY_ID.ToString(), model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.MEASURING_UNIT_ID.ToString(), model.MEASURING_UNIT_NAME, model.REMARKS, model.STATUS,
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
        public async Task<string> GetSearchableMeasuringUnit(string db, int Company_Id, string MeasuringUnit) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), MeasuringUnit })));


    }
}




