using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public class AreaTerritoryRelationManager : IAreaTerritoryRelationManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public AreaTerritoryRelationManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Master_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.AREA_TERRITORY_MST_ID ASC) AS ROW_NO,
                                   M.AREA_TERRITORY_MST_ID, M.AREA_ID, N.AREA_NAME, M.AREA_CODE, M.COMPANY_ID, M.REMARKS,
                                   M.AREA_TERRITORY_MST_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE,  'DD/MM/YYYY') EFFECT_START_DATE  
                                   FROM AREA_TERRITORY_MST  M  
                                   INNER JOIN AREA_INFO N ON N.AREA_ID = M.AREA_ID
                                   Where M.COMPANY_ID = :param1";
        string LoadData_MasterById_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.AREA_TERRITORY_MST_ID ASC) AS ROW_NO,
                                   M.AREA_TERRITORY_MST_ID,  N.AREA_NAME,  M.AREA_ID,M.AREA_CODE, M.COMPANY_ID, M.REMARKS,
                                   M.AREA_TERRITORY_MST_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE  
                                   FROM AREA_TERRITORY_MST  M 
                                   INNER JOIN AREA_INFO N ON N.AREA_ID = M.AREA_ID
                                   Where M.AREA_TERRITORY_MST_ID = :param1";

        string AddOrUpdate_Master_AddQuery() => @"INSERT INTO AREA_TERRITORY_MST 
                                         (AREA_TERRITORY_MST_ID,AREA_ID, AREA_CODE, REMARKS,AREA_TERRITORY_MST_STATUS,
                                         COMPANY_ID,EFFECT_START_DATE,EFFECT_END_DATE , Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6,
                                         TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'), :param9,
                                         TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), :param11 )";
        string AddOrUpdate_Master_UpdateQuery() => @"UPDATE AREA_TERRITORY_MST SET 
                                           AREA_ID =  :param2,AREA_CODE = :param3,REMARKS = :param4,AREA_TERRITORY_MST_STATUS = :param5,
                                           COMPANY_ID = :param6, EFFECT_START_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),
                                           EFFECT_END_DATE = TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'),
                                            UPDATED_BY = :param9, UPDATED_DATE = TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param11 WHERE AREA_TERRITORY_MST_ID = :param1";
        string GetNewArea_Terrritory_MST_IdQuery() => "SELECT NVL(MAX(AREA_TERRITORY_MST_ID),0) + 1 AREA_TERRITORY_MST_ID  FROM AREA_TERRITORY_MST";

        string LoadData_Detail_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.AREA_TERRITORY_DTL_ID ASC) AS ROW_NO,M.AREA_TERRITORY_DTL_ID,
                                          M.AREA_TERRITORY_MST_ID,   M.TERRITORY_ID,M.TERRITORY_CODE, M.COMPANY_ID, M.REMARKS,
                                          M.AREA_TERRITORY_DTL_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE  
                                          FROM AREA_TERRITORY_DTL  M  Where M.AREA_TERRITORY_DTL_ID = :param1";
        string LoadData_DetailByMasterId_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.AREA_TERRITORY_DTL_ID ASC) AS ROW_NO,M.AREA_TERRITORY_DTL_ID,
                                          M.AREA_TERRITORY_MST_ID, N.TERRITORY_NAME,  M.TERRITORY_ID,M.TERRITORY_CODE, M.COMPANY_ID, M.REMARKS,
                                          M.AREA_TERRITORY_DTL_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE  
                                          FROM AREA_TERRITORY_DTL  M
                                          INNER JOIN TERRITORY_INFO N ON N.TERRITORY_ID = M.TERRITORY_ID
                                          Where M.AREA_TERRITORY_MST_ID = :param1";

        string AddOrUpdate_Detail_AddQuery() => @"INSERT INTO AREA_TERRITORY_DTL 
                                         (AREA_TERRITORY_DTL_ID, AREA_TERRITORY_MST_ID,TERRITORY_ID, TERRITORY_CODE, REMARKS,AREA_TERRITORY_DTL_STATUS,
                                         COMPANY_ID,EFFECT_START_DATE,EFFECT_END_DATE , Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6,:param7,
                                         TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'),TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'), :param10,
                                         TO_DATE(:param11, 'DD/MM/YYYY HH:MI:SS AM'), :param12 )";
        string AddOrUpdate_Detail_UpdateQuery() => @"UPDATE AREA_TERRITORY_DTL SET 
                                           TERRITORY_ID =  :param2,TERRITORY_CODE = :param3,REMARKS = :param4,AREA_TERRITORY_DTL_STATUS = :param5,
                                           COMPANY_ID = :param6, EFFECT_START_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),
                                           EFFECT_END_DATE = TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'),
                                            UPDATED_BY = :param9, UPDATED_DATE = TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param11 WHERE AREA_TERRITORY_DTL_ID = :param1";

        string GetNewArea_Territory_Dtl_IdQuery() => "SELECT NVL(MAX(AREA_TERRITORY_DTL_ID),0) + 1 AREA_TERRITORY_DTL_ID  FROM AREA_TERRITORY_DTL";
        string DeleteArea_Territory_Dtl_IdQuery() => "DELETE  FROM AREA_TERRITORY_DTL WHere AREA_TERRITORY_DTL_ID = :param1";

        string Existing_Area_Load_Query() => @"Select AREA_ID,AREA_CODE, AREA_NAME from AREA_INFO where  COMPANY_ID = :param1 AND AREA_ID not in (SELECT distinct AREA_ID from AREA_TERRITORY_MST where COMPANY_ID =:param1)";
        string Existing_Territory_Load_Query() => @"Select TERRITORY_ID,TERRITORY_CODE, TERRITORY_NAME from TERRITORY_INFO where  COMPANY_ID = :param1 AND TERRITORY_ID not in (SELECT distinct TERRITORY_ID from AREA_TERRITORY_DTL where COMPANY_ID =:param1)";

        //---------- Method Execution Part ------------------------------------------------
        public async Task<string> Existing_Area_Load(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Existing_Area_Load_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<string> Existing_Territory_Load(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Existing_Territory_Load_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));



        public async Task<string> AddOrUpdate(string db, Area_Territory_Mst model)
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
                    int detail_id = 0;


                    if (model.AREA_TERRITORY_MST_ID == 0)
                    {
                        //-------------Add to master table--------------------

                        model.AREA_TERRITORY_MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewArea_Terrritory_MST_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Master_AddQuery(), _commonServices.AddParameter(new string[] { model.AREA_TERRITORY_MST_ID.ToString(), model.AREA_ID.ToString(), model.AREA_CODE, model.REMARKS, model.AREA_TERRITORY_MST_STATUS, model.COMPANY_ID.ToString(), model.EFFECT_START_DATE, model.EFFECT_END_DATE, model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));
                        
                        //-------------Add to detail table--------------------

                        foreach (var item in model.area_Territory_Dtls)
                        {
                            detail_id = detail_id == 0? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewArea_Territory_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (detail_id + 1);

                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(), _commonServices.AddParameter(new string[] { detail_id.ToString(), model.AREA_TERRITORY_MST_ID.ToString(), item.TERRITORY_ID.ToString(), item.TERRITORY_CODE, item.REMARKS, item.AREA_TERRITORY_DTL_STATUS, item.COMPANY_ID.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.ENTERED_BY.ToString(), item.ENTERED_DATE, item.ENTERED_TERMINAL })));

                        }
                    }
                    else
                    {
                        Area_Territory_Mst area_Territory_Mst =await  LoadDetailData_ByMasterId_List(db, model.AREA_TERRITORY_MST_ID);
                        //-------------Edit on Master table--------------------

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_Master_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.AREA_TERRITORY_MST_ID.ToString(),
                                model.AREA_ID.ToString(), model.AREA_CODE, model.REMARKS, model.AREA_TERRITORY_MST_STATUS, model.COMPANY_ID.ToString(), model.EFFECT_START_DATE, model.EFFECT_END_DATE,
                                model.UPDATED_BY.ToString(), model.UPDATED_DATE,
                                model.UPDATED_TERMINAL })));
                        foreach (var item in model.area_Territory_Dtls)
                        {
                            if(item.AREA_TERRITORY_DTL_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------

                                detail_id = detail_id == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewArea_Territory_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (detail_id + 1);

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(), _commonServices.AddParameter(new string[] { detail_id.ToString(), model.AREA_TERRITORY_MST_ID.ToString(), item.TERRITORY_ID.ToString(), item.TERRITORY_CODE, item.REMARKS, item.AREA_TERRITORY_DTL_STATUS, item.COMPANY_ID.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.ENTERED_BY.ToString(), item.ENTERED_DATE, item.ENTERED_TERMINAL })));

                            }
                            else
                            {
                                //-------------Edit on detail table--------------------

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_UpdateQuery(), _commonServices.AddParameter(new string[] { item.AREA_TERRITORY_DTL_ID.ToString(), item.TERRITORY_ID.ToString(), item.TERRITORY_CODE, item.REMARKS, item.AREA_TERRITORY_DTL_STATUS, item.COMPANY_ID.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.UPDATED_BY.ToString(), item.UPDATED_DATE, item.UPDATED_TERMINAL })));

                            }


                        }
                        foreach (var item in area_Territory_Mst.area_Territory_Dtls)
                        {
                            bool status = true;

                            foreach (var updateditem in model.area_Territory_Dtls)
                            {
                                if(item.AREA_TERRITORY_DTL_ID == updateditem.AREA_TERRITORY_DTL_ID)
                                {
                                    status = false;
                                }
                            }
                            if(status)
                            {
                                //-------------Delete row from detail table--------------------

                                listOfQuery.Add(_commonServices.AddQuery(DeleteArea_Territory_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { item.AREA_TERRITORY_DTL_ID.ToString() })));

                            }

                        }
                     

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
        public async Task<string> LoadData_Master(string db, int Company_Id)
        {
            List<Area_Territory_Mst> area_Territory_Mst_list = new List<Area_Territory_Mst>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Master_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                Area_Territory_Mst area_Territory_Mst = new Area_Territory_Mst();
                area_Territory_Mst.AREA_TERRITORY_MST_ID = Convert.ToInt32(data.Rows[i]["AREA_TERRITORY_MST_ID"]);
                area_Territory_Mst.AREA_TERRITORY_MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["AREA_TERRITORY_MST_ID"].ToString());
                area_Territory_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                area_Territory_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                area_Territory_Mst.AREA_CODE = data.Rows[i]["AREA_CODE"].ToString();
                area_Territory_Mst.AREA_NAME = data.Rows[i]["AREA_NAME"].ToString();

                area_Territory_Mst.AREA_ID = Convert.ToInt32(data.Rows[i]["AREA_ID"].ToString());
                area_Territory_Mst.AREA_TERRITORY_MST_STATUS = data.Rows[i]["AREA_TERRITORY_MST_STATUS"].ToString();
                area_Territory_Mst.EFFECT_END_DATE = data.Rows[i]["EFFECT_END_DATE"].ToString();
                area_Territory_Mst.EFFECT_START_DATE = data.Rows[i]["EFFECT_START_DATE"].ToString();
                area_Territory_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                area_Territory_Mst_list.Add(area_Territory_Mst);
            }
            return JsonSerializer.Serialize(area_Territory_Mst_list);
         }
    
      //public async Task<string> LoadData_Master(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Master_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> LoadData_MasterById(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<string> LoadData_DetailById(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Detail_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));
        public async Task<string> LoadData_DetailByMasterId(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<Area_Territory_Mst> LoadDetailData_ByMasterId_List(string db, int Id)
        {
            Area_Territory_Mst _Territory_Mst = new Area_Territory_Mst();
            
            DataTable dataTable =  await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));
            if(dataTable.Rows.Count>0)
            {
                _Territory_Mst.COMPANY_ID = Convert.ToInt32(dataTable.Rows[0]["COMPANY_ID"]);
                _Territory_Mst.AREA_TERRITORY_MST_ID = Id;
                _Territory_Mst.AREA_NAME = dataTable.Rows[0]["AREA_NAME"].ToString();

                _Territory_Mst.AREA_CODE = dataTable.Rows[0]["AREA_CODE"].ToString();
                _Territory_Mst.AREA_ID = Convert.ToInt32(dataTable.Rows[0]["AREA_ID"]);
                _Territory_Mst.AREA_TERRITORY_MST_STATUS = dataTable.Rows[0]["AREA_TERRITORY_MST_STATUS"].ToString();
                _Territory_Mst.EFFECT_START_DATE = dataTable.Rows[0]["EFFECT_START_DATE"].ToString();
                _Territory_Mst.EFFECT_END_DATE = dataTable.Rows[0]["EFFECT_END_DATE"].ToString();
                _Territory_Mst.REMARKS = dataTable.Rows[0]["REMARKS"].ToString();
               
                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));
               
                _Territory_Mst.area_Territory_Dtls = new List<Area_Territory_Dtl>();

                for (int i=0;i< dataTable_detail.Rows.Count;i++)
                {
                    Area_Territory_Dtl _Territory_Dtl = new Area_Territory_Dtl();

                    _Territory_Dtl.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                    _Territory_Dtl.TERRITORY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["TERRITORY_ID"]);
                    _Territory_Dtl.TERRITORY_NAME = dataTable_detail.Rows[i]["TERRITORY_NAME"].ToString();

                    _Territory_Dtl.TERRITORY_CODE = dataTable_detail.Rows[i]["TERRITORY_CODE"].ToString();
                    _Territory_Dtl.REMARKS = dataTable_detail.Rows[i]["REMARKS"].ToString();
                    _Territory_Dtl.AREA_TERRITORY_DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["AREA_TERRITORY_DTL_ID"]);
                    _Territory_Dtl.AREA_TERRITORY_DTL_STATUS = dataTable_detail.Rows[i]["AREA_TERRITORY_DTL_STATUS"].ToString();
                    _Territory_Dtl.EFFECT_START_DATE = dataTable_detail.Rows[i]["EFFECT_START_DATE"].ToString();
                    _Territory_Dtl.EFFECT_END_DATE = dataTable_detail.Rows[i]["EFFECT_END_DATE"].ToString();
                    _Territory_Mst.area_Territory_Dtls.Add(_Territory_Dtl);
                }
            }

            return _Territory_Mst;



        }



    }
}
