using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.Manager
{
    public class VehicleManager : IVehicleManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;

        public VehicleManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;
        }

        //-------------------Query Part ---------------------------------------------------

        private string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.VEHICLE_ID ASC) AS ROW_NO,
                                    VEHICLE_ID, VEHICLE_NO, VEHICLE_DESCRIPTION, VEHICLE_TOTAL_VOLUME,    VOLUME_UNIT, VEHICLE_TOTAL_WEIGHT, WEIGHT_UNIT, M.DRIVER_ID ,M.DRIVER_NAME,D.CONTACT_NO DRIVER_PHONE, M.STATUS, M.COMPANY_ID, M.REMARKS, TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE,
                            U.MEASURING_UNIT_NAME AS WEIGHT_UNIT_NAME,
                            V.MEASURING_UNIT_NAME AS VOLUME_UNIT_NAME,
                            FN_UNIT_NAME(M.COMPANY_ID, M.UNIT_ID) UNIT_NAME,
                            M.UNIT_ID
                            FROM VEHICLE_INFO  M
                            LEFT JOIN MEASURING_UNIT_INFO U ON M.WEIGHT_UNIT = U.MEASURING_UNIT_NAME
                            LEFT JOIN MEASURING_UNIT_INFO V ON M.VOLUME_UNIT = V.MEASURING_UNIT_NAME
                            LEFT OUTER JOIN DRIVER_INFO D ON D.DRIVER_ID = M.DRIVER_ID
                            Where M.COMPANY_ID = :param1";

        private string LoadAllData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.VEHICLE_ID ASC) AS ROW_NO,
                                    VEHICLE_ID, VEHICLE_NO, VEHICLE_DESCRIPTION, VEHICLE_TOTAL_VOLUME, VOLUME_UNIT, VEHICLE_TOTAL_WEIGHT, WEIGHT_UNIT, DRIVER_ID, DRIVER_NAME, STATUS, COMPANY_ID, REMARKS, TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE, UNIT_ID
                                    FROM VEHICLE_INFO  M   Where M.Company_Id = :param1";

        private string LoadSearchableData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.VEHICLE_ID ASC) AS ROW_NO,
                            VEHICLE_ID, VEHICLE_NO, VEHICLE_DESCRIPTION, VEHICLE_TOTAL_VOLUME, VOLUME_UNIT, VEHICLE_TOTAL_WEIGHT, WEIGHT_UNIT, DRIVER_ID, DRIVER_NAME, STATUS, COMPANY_ID, REMARKS, TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                            FROM VEHICLE_INFO  M  Where M.COMPANY_ID = :param1 AND upper(M.VEHICLE_NO) Like '%' || upper(:param2) || '%'";

        private string AddOrUpdate_AddQuery() => @"INSERT INTO VEHICLE_INFO
                                         (VEHICLE_ID, VEHICLE_NO, VEHICLE_DESCRIPTION, VEHICLE_TOTAL_VOLUME, VOLUME_UNIT, VEHICLE_TOTAL_WEIGHT, WEIGHT_UNIT, DRIVER_ID, DRIVER_NAME, STATUS, COMPANY_ID, REMARKS,  Entered_By, ENTERED_DATE ,ENTERED_TERMINAL, UNIT_ID )
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8, :param9, :param10, :param11, :param12, :param13, TO_DATE(:param14, 'DD/MM/YYYY HH:MI:SS AM'), :param15, :param16 )";

        private string AddOrUpdate_UpdateQuery() => @"UPDATE VEHICLE_INFO SET
                                            VEHICLE_NO = :param2,VEHICLE_DESCRIPTION = :param3, VEHICLE_TOTAL_VOLUME = :param4,
                                            VOLUME_UNIT = :param5,VEHICLE_TOTAL_WEIGHT = :param6, WEIGHT_UNIT = :param7,
                                            DRIVER_ID = :param8, DRIVER_NAME = :param9, STATUS = :param10,COMPANY_ID = :param11,
                                            REMARKS = :param12, UPDATED_BY = :param13, UPDATED_DATE = TO_DATE(:param14, 'DD/MM/YYYY HH:MI:SS AM'),
                                            UPDATED_TERMINAL = :param15, UNIT_ID = :param16 WHERE VEHICLE_ID = :param1";

        private string GetNewVehicle_Info_IdQuery() => "SELECT NVL(MAX(VEHICLE_ID),0) + 1 VEHICLE_ID  FROM VEHICLE_INFO";

        private string Get_LastVehicle_Info() => "SELECT VEHICLE_ID FROM VEHICLE_INFO Where  VEHICLE_ID = (SELECT NVL(MAX(VEHICLE_ID),0) VEHICLE_ID From VEHICLE_INFO where COMPANY_ID = :param1 )";

        private string MeasuringUnitLoad_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MEASURING_UNIT_ID ASC) AS ROW_NO,
                                    M.MEASURING_UNIT_ID, M.MEASURING_UNIT_NAME, M.COMPANY_ID , M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM MEASURING_UNIT_INFO  M  Where M.COMPANY_ID = :param1 AND M.MEASURING_UNIT_TYPE= :param2";

        //---------- Method Execution Part ------------------------------------------------
        public async Task<DataTable> GetCompanyListDataTable(string db, int comp_id) => await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadAllData_Query(), _commonServices.AddParameter(new string[] { comp_id.ToString() }));

        public async Task<string> GetVehicleJsonList(string db, int comp_id) => _commonServices.DataTableToJSON(await GetCompanyListDataTable(db, comp_id));

        public async Task<string> AddOrUpdate(string db, Vehicle_Info model)
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
                    if (model.VEHICLE_ID == 0)
                    {
                        model.VEHICLE_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewVehicle_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));
                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] { model.VEHICLE_ID.ToString(), model.VEHICLE_NO, model.VEHICLE_DESCRIPTION, model.VEHICLE_TOTAL_VOLUME.ToString(), model.VOLUME_UNIT, model.VEHICLE_TOTAL_WEIGHT.ToString(), model.WEIGHT_UNIT, model.DRIVER_ID, model.DRIVER_NAME, model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS, model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL, model.UNIT_ID }))); ;
                    }
                    else
                    {
                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.VEHICLE_ID.ToString(), model.VEHICLE_NO, model.VEHICLE_DESCRIPTION, model.VEHICLE_TOTAL_VOLUME.ToString(), model.VOLUME_UNIT, model.VEHICLE_TOTAL_WEIGHT.ToString(), model.WEIGHT_UNIT, model.DRIVER_ID, model.DRIVER_NAME,
                                model.STATUS, model.COMPANY_ID.ToString(),model.REMARKS,
                                model.UPDATED_BY.ToString(), model.UPDATED_DATE,
                                model.UPDATED_TERMINAL, model.UNIT_ID })));
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

        public async Task<string> GetSearchableVehicle(string db, int Company_Id, string vehicle) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), vehicle })));

        public async Task<string> GetMeasuingUnit(string db, int Company_Id, string Measuring_Unit_Type) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), MeasuringUnitLoad_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Measuring_Unit_Type })));
    }
}