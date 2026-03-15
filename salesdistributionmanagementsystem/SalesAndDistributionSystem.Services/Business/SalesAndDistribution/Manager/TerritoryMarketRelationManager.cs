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
    public class TerritoryMarketRelationManager : ITerritoryMarketRelationManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public TerritoryMarketRelationManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Master_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.TERRITORY_MARKET_MST_ID ASC) AS ROW_NO,
                                   M.TERRITORY_MARKET_MST_ID, M.TERRITORY_ID, N.TERRITORY_NAME, M.TERRITORY_CODE, M.COMPANY_ID, M.REMARKS,
                                   M.TERRITORY_MARKET_MST_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE,  'DD/MM/YYYY') EFFECT_START_DATE  
                                   FROM TERRITORY_MARKET_MST  M  
                                   INNER JOIN TERRITORY_INFO N ON N.TERRITORY_ID = M.TERRITORY_ID
                                   Where M.COMPANY_ID = :param1";
        string LoadData_MasterById_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.TERRITORY_MARKET_MST_ID ASC) AS ROW_NO,
                                   M.TERRITORY_MARKET_MST_ID,  N.TERRITORY_NAME,  M.TERRITORY_ID,M.TERRITORY_CODE, M.COMPANY_ID, M.REMARKS,
                                   M.TERRITORY_MARKET_MST_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE  
                                   FROM TERRITORY_MARKET_MST  M 
                                   INNER JOIN TERRITORY_INFO N ON N.TERRITORY_ID = M.TERRITORY_ID
                                   Where M.TERRITORY_MARKET_MST_ID = :param1";

        string AddOrUpdate_Master_AddQuery() => @"INSERT INTO TERRITORY_MARKET_MST 
                                         (TERRITORY_MARKET_MST_ID,TERRITORY_ID, TERRITORY_CODE, REMARKS,TERRITORY_MARKET_MST_STATUS,
                                         COMPANY_ID,EFFECT_START_DATE,EFFECT_END_DATE , Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6,
                                         TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'), :param9,
                                         TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), :param11 )";
        string AddOrUpdate_Master_UpdateQuery() => @"UPDATE TERRITORY_MARKET_MST SET 
                                           TERRITORY_ID =  :param2,TERRITORY_CODE = :param3,REMARKS = :param4,TERRITORY_MARKET_MST_STATUS = :param5,
                                           COMPANY_ID = :param6, EFFECT_START_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),
                                           EFFECT_END_DATE = TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'),
                                            UPDATED_BY = :param9, UPDATED_DATE = TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param11 WHERE TERRITORY_MARKET_MST_ID = :param1";
        string GetNewTerritory_Market_MST_IdQuery() => "SELECT NVL(MAX(TERRITORY_MARKET_MST_ID),0) + 1 TERRITORY_MARKET_MST_ID  FROM TERRITORY_MARKET_MST";

        string LoadData_Detail_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.TERRITORY_MARKET_DTL_ID ASC) AS ROW_NO,M.TERRITORY_MARKET_DTL_ID,
                                          M.TERRITORY_MARKET_MST_ID,   M.MARKET_ID,M.MARKET_CODE, M.COMPANY_ID, M.REMARKS,
                                          M.TERRITORY_MARKET_DTL_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE  
                                          FROM TERRITORY_MARKET_DTL  M  Where M.TERRITORY_MARKET_DTL_ID = :param1";
        string LoadData_DetailByMasterId_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.TERRITORY_MARKET_DTL_ID ASC) AS ROW_NO,M.TERRITORY_MARKET_DTL_ID,
                                          M.TERRITORY_MARKET_MST_ID, N.MARKET_NAME,  M.MARKET_ID,M.MARKET_CODE, M.COMPANY_ID, M.REMARKS,
                                          M.TERRITORY_MARKET_DTL_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE  
                                          FROM TERRITORY_MARKET_DTL  M
                                          INNER JOIN MARKET_INFO N ON N.MARKET_ID = M.MARKET_ID
                                          Where M.TERRITORY_MARKET_MST_ID = :param1";

        string AddOrUpdate_Detail_AddQuery() => @"INSERT INTO TERRITORY_MARKET_DTL 
                                         (TERRITORY_MARKET_DTL_ID, TERRITORY_MARKET_MST_ID,MARKET_ID, MARKET_CODE, REMARKS,TERRITORY_MARKET_DTL_STATUS,
                                         COMPANY_ID,EFFECT_START_DATE,EFFECT_END_DATE , Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6,:param7,
                                         TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'),TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'), :param10,
                                         TO_DATE(:param11, 'DD/MM/YYYY HH:MI:SS AM'), :param12 )";
        string AddOrUpdate_Detail_UpdateQuery() => @"UPDATE TERRITORY_MARKET_DTL SET 
                                           MARKET_ID =  :param2,MARKET_CODE = :param3,REMARKS = :param4,TERRITORY_MARKET_DTL_STATUS = :param5,
                                           COMPANY_ID = :param6, EFFECT_START_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),
                                           EFFECT_END_DATE = TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'),
                                            UPDATED_BY = :param9, UPDATED_DATE = TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param11 WHERE TERRITORY_MARKET_DTL_ID = :param1";

        string GetNewTerritory_Market_Dtl_IdQuery() => "SELECT NVL(MAX(TERRITORY_MARKET_DTL_ID),0) + 1 TERRITORY_MARKET_DTL_ID  FROM TERRITORY_MARKET_DTL";
        string DeleteTerritory_Market_Dtl_IdQuery() => "DELETE  FROM TERRITORY_MARKET_DTL WHere TERRITORY_MARKET_DTL_ID = :param1";

        string Existing_Territory_Load_Query() => @"Select TERRITORY_ID,TERRITORY_CODE, TERRITORY_NAME from TERRITORY_INFO where  COMPANY_ID = :param1 AND TERRITORY_ID not in (SELECT distinct TERRITORY_ID from TERRITORY_MARKET_MST where COMPANY_ID =:param1)";
        string Existing_Market_Load_Query() => @"Select MARKET_ID,MARKET_CODE,MARKET_NAME from MARKET_INFO where  COMPANY_ID = :param1 AND MARKET_ID not in (SELECT distinct MARKET_ID from TERRITORY_MARKET_DTL where COMPANY_ID =:param1)";

        //---------- Method Execution Part ------------------------------------------------
        public async Task<string> Existing_Territory_Load(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Existing_Territory_Load_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<string> Existing_Market_Load(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Existing_Market_Load_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));



        public async Task<string> AddOrUpdate(string db, Territory_Market_Mst model)
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


                    if (model.TERRITORY_MARKET_MST_ID == 0)
                    {
                        //-------------Add to master table--------------------

                        model.TERRITORY_MARKET_MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewTerritory_Market_MST_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Master_AddQuery(), _commonServices.AddParameter(new string[] { model.TERRITORY_MARKET_MST_ID.ToString(), model.TERRITORY_ID.ToString(), model.TERRITORY_CODE, model.REMARKS, model.TERRITORY_MARKET_MST_STATUS, model.COMPANY_ID.ToString(), model.EFFECT_START_DATE, model.EFFECT_END_DATE, model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));
                        
                        //-------------Add to detail table--------------------

                        foreach (var item in model.territory_Market_Dtls)
                        {
                            detail_id = detail_id == 0? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewTerritory_Market_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (detail_id + 1);

                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(), _commonServices.AddParameter(new string[] { detail_id.ToString(), model.TERRITORY_MARKET_MST_ID.ToString(), item.MARKET_ID.ToString(), item.MARKET_CODE, item.REMARKS, item.TERRITORY_MARKET_DTL_STATUS, item.COMPANY_ID.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.ENTERED_BY.ToString(), item.ENTERED_DATE, item.ENTERED_TERMINAL })));

                        }
                    }
                    else
                    {
                        Territory_Market_Mst territory_Market_Mst = await  LoadDetailData_ByMasterId_List(db, model.TERRITORY_MARKET_MST_ID);
                        //-------------Edit on Master table--------------------

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_Master_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.TERRITORY_MARKET_MST_ID.ToString(),
                                model.TERRITORY_ID.ToString(), model.TERRITORY_CODE, model.REMARKS, model.TERRITORY_MARKET_MST_STATUS, model.COMPANY_ID.ToString(), model.EFFECT_START_DATE, model.EFFECT_END_DATE,
                                model.UPDATED_BY.ToString(), model.UPDATED_DATE,
                                model.UPDATED_TERMINAL })));
                        foreach (var item in model.territory_Market_Dtls)
                        {
                            if(item.TERRITORY_MARKET_DTL_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------

                                detail_id = detail_id == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewTerritory_Market_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (detail_id + 1);

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(), _commonServices.AddParameter(new string[] { detail_id.ToString(), model.TERRITORY_MARKET_MST_ID.ToString(), item.MARKET_ID.ToString(), item.MARKET_CODE, item.REMARKS, item.TERRITORY_MARKET_DTL_STATUS, item.COMPANY_ID.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.ENTERED_BY.ToString(), item.ENTERED_DATE, item.ENTERED_TERMINAL })));

                            }
                            else
                            {
                                //-------------Edit on detail table--------------------

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_UpdateQuery(), _commonServices.AddParameter(new string[] { item.TERRITORY_MARKET_DTL_ID.ToString(), item.MARKET_ID.ToString(), item.MARKET_CODE, item.REMARKS, item.TERRITORY_MARKET_DTL_STATUS, item.COMPANY_ID.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.UPDATED_BY.ToString(), item.UPDATED_DATE, item.UPDATED_TERMINAL })));

                            }


                        }
                        foreach (var item in territory_Market_Mst.territory_Market_Dtls)
                        {
                            bool status = true;

                            foreach (var updateditem in model.territory_Market_Dtls)
                            {
                                if(item.TERRITORY_MARKET_DTL_ID == updateditem.TERRITORY_MARKET_DTL_ID)
                                {
                                    status = false;
                                }
                            }
                            if(status)
                            {
                                //-------------Delete row from detail table--------------------

                                listOfQuery.Add(_commonServices.AddQuery(DeleteTerritory_Market_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { item.TERRITORY_MARKET_DTL_ID.ToString() })));

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
            List<Territory_Market_Mst> territory_Market_Mst_list = new List<Territory_Market_Mst>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Master_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
               Territory_Market_Mst territory_Market_Mst = new Territory_Market_Mst();
                territory_Market_Mst.TERRITORY_MARKET_MST_ID = Convert.ToInt32(data.Rows[i]["TERRITORY_MARKET_MST_ID"]);
                territory_Market_Mst.TERRITORY_MARKET_MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["TERRITORY_MARKET_MST_ID"].ToString());
                territory_Market_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                territory_Market_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                territory_Market_Mst.TERRITORY_CODE = data.Rows[i]["TERRITORY_CODE"].ToString();
                territory_Market_Mst.TERRITORY_NAME = data.Rows[i]["TERRITORY_NAME"].ToString();

                territory_Market_Mst.TERRITORY_ID = Convert.ToInt32(data.Rows[i]["TERRITORY_ID"].ToString());
                territory_Market_Mst.TERRITORY_MARKET_MST_STATUS = data.Rows[i]["TERRITORY_MARKET_MST_STATUS"].ToString();
                territory_Market_Mst.EFFECT_END_DATE = data.Rows[i]["EFFECT_END_DATE"].ToString();
                territory_Market_Mst.EFFECT_START_DATE = data.Rows[i]["EFFECT_START_DATE"].ToString();
                territory_Market_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                territory_Market_Mst_list.Add(territory_Market_Mst);
            }
            return JsonSerializer.Serialize(territory_Market_Mst_list);
         }
    
      //public async Task<string> LoadData_Master(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Master_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> LoadData_MasterById(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<string> LoadData_DetailById(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Detail_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));
        public async Task<string> LoadData_DetailByMasterId(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<Territory_Market_Mst> LoadDetailData_ByMasterId_List(string db, int Id)
        {
            Territory_Market_Mst _Market_Mst = new Territory_Market_Mst();
            
            DataTable dataTable =  await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));
            if(dataTable.Rows.Count>0)
            {
                _Market_Mst.COMPANY_ID = Convert.ToInt32(dataTable.Rows[0]["COMPANY_ID"]);
                _Market_Mst.TERRITORY_MARKET_MST_ID = Id;
                _Market_Mst.TERRITORY_NAME = dataTable.Rows[0]["TERRITORY_NAME"].ToString();

                _Market_Mst.TERRITORY_CODE = dataTable.Rows[0]["TERRITORY_CODE"].ToString();
                _Market_Mst.TERRITORY_ID = Convert.ToInt32(dataTable.Rows[0]["TERRITORY_ID"]);
                _Market_Mst.TERRITORY_MARKET_MST_STATUS = dataTable.Rows[0]["TERRITORY_Market_MST_STATUS"].ToString();
                _Market_Mst.EFFECT_START_DATE = dataTable.Rows[0]["EFFECT_START_DATE"].ToString();
                _Market_Mst.EFFECT_END_DATE = dataTable.Rows[0]["EFFECT_END_DATE"].ToString();
                _Market_Mst.REMARKS = dataTable.Rows[0]["REMARKS"].ToString();
               
                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));
               
                _Market_Mst.territory_Market_Dtls = new List<Territory_Market_Dtl>();

                for (int i=0;i< dataTable_detail.Rows.Count;i++)
                {
                    Territory_Market_Dtl _Market_Dtl = new Territory_Market_Dtl();

                    _Market_Dtl.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                    _Market_Dtl.MARKET_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MARKET_ID"]);
                    _Market_Dtl.MARKET_NAME = dataTable_detail.Rows[i]["MARKET_NAME"].ToString();

                    _Market_Dtl.MARKET_CODE = dataTable_detail.Rows[i]["MARKET_CODE"].ToString();
                    _Market_Dtl.REMARKS = dataTable_detail.Rows[i]["REMARKS"].ToString();
                    _Market_Dtl.TERRITORY_MARKET_DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["TERRITORY_MARKET_DTL_ID"]);
                    _Market_Dtl.TERRITORY_MARKET_DTL_STATUS = dataTable_detail.Rows[i]["TERRITORY_MARKET_DTL_STATUS"].ToString();
                    _Market_Dtl.EFFECT_START_DATE = dataTable_detail.Rows[i]["EFFECT_START_DATE"].ToString();
                    _Market_Dtl.EFFECT_END_DATE = dataTable_detail.Rows[i]["EFFECT_END_DATE"].ToString();
                    _Market_Mst.territory_Market_Dtls.Add(_Market_Dtl);
                }
            }

            return _Market_Mst;



        }



    }
}
