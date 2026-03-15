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
    public class DivisionRegionRelationManager : IDivisionRegionRelationManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public DivisionRegionRelationManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Master_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.DIVISION_REGION_MST_ID ASC) AS ROW_NO,
                                   M.DIVISION_REGION_MST_ID, M.DIVISION_ID, N.DIVISION_NAME, M.DIVISION_CODE, M.COMPANY_ID, M.REMARKS,
                                   M.DIVISION_REGION_MST_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE,  'DD/MM/YYYY') EFFECT_START_DATE  
                                   FROM DIVISION_REGION_MST  M  
                                   INNER JOIN DIVISION_INFO N ON N.DIVISION_ID = M.DIVISION_ID
                                   Where M.COMPANY_ID = :param1";
        string LoadData_MasterById_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.DIVISION_REGION_MST_ID ASC) AS ROW_NO,
                                   M.DIVISION_REGION_MST_ID,  N.DIVISION_NAME,  M.DIVISION_ID,M.DIVISION_CODE, M.COMPANY_ID, M.REMARKS,
                                   M.DIVISION_REGION_MST_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE  
                                   FROM DIVISION_REGION_MST  M 
                                   INNER JOIN DIVISION_INFO N ON N.DIVISION_ID = M.DIVISION_ID
                                   Where M.DIVISION_REGION_MST_ID = :param1";

        string AddOrUpdate_Master_AddQuery() => @"INSERT INTO DIVISION_REGION_MST 
                                         (DIVISION_REGION_MST_ID,DIVISION_ID, DIVISION_CODE, REMARKS,DIVISION_REGION_MST_STATUS,
                                         COMPANY_ID,EFFECT_START_DATE,EFFECT_END_DATE , Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6,
                                         TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'), :param9,
                                         TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), :param11 )";
        string AddOrUpdate_Master_UpdateQuery() => @"UPDATE DIVISION_REGION_MST SET 
                                           DIVISION_ID =  :param2,DIVISION_CODE = :param3,REMARKS = :param4,DIVISION_REGION_MST_STATUS = :param5,
                                           COMPANY_ID = :param6, EFFECT_START_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),
                                           EFFECT_END_DATE = TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'),
                                            UPDATED_BY = :param9, UPDATED_DATE = TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param11 WHERE DIVISION_REGION_MST_ID = :param1";
        string GetNewDivision_Region_MST_IdQuery() => "SELECT NVL(MAX(DIVISION_REGION_MST_ID),0) + 1 DIVISION_REGION_MST_ID  FROM DIVISION_REGION_MST";

        string LoadData_Detail_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.DIVISION_REGION_DTL_ID ASC) AS ROW_NO,M.DIVISION_REGION_DTL_ID,
                                          M.DIVISION_REGION_MST_ID,   M.REGION_ID,M.REGION_CODE, M.COMPANY_ID, M.REMARKS,
                                          M.DIVISION_REGION_DTL_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE  
                                          FROM DIVISION_REGION_DTL  M  Where M.DIVISION_REGION_DTL_ID = :param1";
        string LoadData_DetailByMasterId_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.DIVISION_REGION_DTL_ID ASC) AS ROW_NO,M.DIVISION_REGION_DTL_ID,
                                          M.DIVISION_REGION_MST_ID, N.REGION_NAME,  M.REGION_ID,M.REGION_CODE, M.COMPANY_ID, M.REMARKS,
                                          M.DIVISION_REGION_DTL_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE  
                                          FROM DIVISION_REGION_DTL  M
                                          INNER JOIN REGION_INFO N ON N.REGION_ID = M.REGION_ID
                                          Where M.DIVISION_REGION_MST_ID = :param1";

        string AddOrUpdate_Detail_AddQuery() => @"INSERT INTO DIVISION_REGION_DTL 
                                         (DIVISION_REGION_DTL_ID, DIVISION_REGION_MST_ID,REGION_ID, REGION_CODE, REMARKS,DIVISION_REGION_DTL_STATUS,
                                         COMPANY_ID,EFFECT_START_DATE,EFFECT_END_DATE , Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6,:param7,
                                         TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'),TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'), :param10,
                                         TO_DATE(:param11, 'DD/MM/YYYY HH:MI:SS AM'), :param12 )";
        string AddOrUpdate_Detail_UpdateQuery() => @"UPDATE DIVISION_REGION_DTL SET 
                                           REGION_ID =  :param2,REGION_CODE = :param3,REMARKS = :param4,DIVISION_REGION_DTL_STATUS = :param5,
                                           COMPANY_ID = :param6, EFFECT_START_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),
                                           EFFECT_END_DATE = TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'),
                                            UPDATED_BY = :param9, UPDATED_DATE = TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param11 WHERE DIVISION_REGION_DTL_ID = :param1";

        string GetNewDivision_Region_Dtl_IdQuery() => "SELECT NVL(MAX(DIVISION_REGION_DTL_ID),0) + 1 DIVISION_REGION_DTL_ID  FROM DIVISION_REGION_DTL";
        string DeleteDivision_Region_Dtl_IdQuery() => "DELETE  FROM DIVISION_REGION_DTL WHere DIVISION_REGION_DTL_ID = :param1";
        string Existing_Division_Load_Query() => @"Select DIVISION_ID,DIVISION_CODE, DIVISION_NAME from DIVISION_INFO where COMPANY_ID = :param1 AND  DIVISION_ID not in (SELECT distinct DIVISION_ID from DIVISION_REGION_MST where COMPANY_ID = :param1)";
        string Existing_Region_Load_Query() => @"Select REGION_ID,REGION_CODE, REGION_NAME from REGION_INFO where  COMPANY_ID = :param1 AND REGION_ID  not in (SELECT distinct REGION_ID from DIVISION_REGION_DTL where COMPANY_ID =:param1)";
        //---------- Method Execution Part ------------------------------------------------

        public async Task<string> Existing_Division_Load(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Existing_Division_Load_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<string> Existing_Region_Load(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Existing_Region_Load_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<string> AddOrUpdate(string db, Division_Region_Mst model)
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


                    if (model.DIVISION_REGION_MST_ID == 0)
                    {
                        //-------------Add to master table--------------------

                        model.DIVISION_REGION_MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewDivision_Region_MST_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Master_AddQuery(), _commonServices.AddParameter(new string[] { model.DIVISION_REGION_MST_ID.ToString(), model.DIVISION_ID.ToString(), model.DIVISION_CODE, model.REMARKS, model.DIVISION_REGION_MST_STATUS, model.COMPANY_ID.ToString(), model.EFFECT_START_DATE, model.EFFECT_END_DATE, model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));
                        
                        //-------------Add to detail table--------------------

                        foreach (var item in model.division_Region_Dtls)
                        {
                            detail_id = detail_id == 0? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewDivision_Region_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (detail_id + 1);

                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(), _commonServices.AddParameter(new string[] { detail_id.ToString(), model.DIVISION_REGION_MST_ID.ToString(), item.REGION_ID.ToString(), item.REGION_CODE, item.REMARKS, item.DIVISION_REGION_DTL_STATUS, item.COMPANY_ID.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.ENTERED_BY.ToString(), item.ENTERED_DATE, item.ENTERED_TERMINAL })));

                        }
                    }
                    else
                    {
                        Division_Region_Mst diivision_Region_Mst =await  LoadDetailData_ByMasterId_List(db, model.DIVISION_REGION_MST_ID);
                        //-------------Edit on Master table--------------------

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_Master_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.DIVISION_REGION_MST_ID.ToString(),
                                model.DIVISION_ID.ToString(), model.DIVISION_CODE, model.REMARKS, model.DIVISION_REGION_MST_STATUS, model.COMPANY_ID.ToString(), model.EFFECT_START_DATE, model.EFFECT_END_DATE,
                                model.UPDATED_BY.ToString(), model.UPDATED_DATE,
                                model.UPDATED_TERMINAL })));
                        foreach (var item in model.division_Region_Dtls)
                        {
                            if(item.DIVISION_REGION_DTL_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------

                                detail_id = detail_id == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewDivision_Region_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (detail_id + 1);

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(), _commonServices.AddParameter(new string[] { detail_id.ToString(), model.DIVISION_REGION_MST_ID.ToString(), item.REGION_ID.ToString(), item.REGION_CODE, item.REMARKS, item.DIVISION_REGION_DTL_STATUS, item.COMPANY_ID.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.ENTERED_BY.ToString(), item.ENTERED_DATE, item.ENTERED_TERMINAL })));

                            }
                            else
                            {
                                //-------------Edit on detail table--------------------

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_UpdateQuery(), _commonServices.AddParameter(new string[] { item.DIVISION_REGION_DTL_ID.ToString(), item.REGION_ID.ToString(), item.REGION_CODE, item.REMARKS, item.DIVISION_REGION_DTL_STATUS, item.COMPANY_ID.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.UPDATED_BY.ToString(), item.UPDATED_DATE, item.UPDATED_TERMINAL })));

                            }


                        }
                        foreach (var item in diivision_Region_Mst.division_Region_Dtls)
                        {
                            bool status = true;

                            foreach (var updateditem in model.division_Region_Dtls)
                            {
                                if(item.DIVISION_REGION_DTL_ID == updateditem.DIVISION_REGION_DTL_ID)
                                {
                                    status = false;
                                }
                            }
                            if(status)
                            {
                                //-------------Delete row from detail table--------------------

                                listOfQuery.Add(_commonServices.AddQuery(DeleteDivision_Region_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { item.DIVISION_REGION_DTL_ID.ToString() })));

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
            List<Division_Region_Mst> division_Region_Mst_list = new List<Division_Region_Mst>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Master_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                Division_Region_Mst division_Region_Mst = new Division_Region_Mst();
                division_Region_Mst.DIVISION_REGION_MST_ID = Convert.ToInt32(data.Rows[i]["DIVISION_REGION_MST_ID"]);
                division_Region_Mst.DIVISION_REGION_MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["DIVISION_REGION_MST_ID"].ToString());
                division_Region_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                division_Region_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                division_Region_Mst.DIVISION_CODE = data.Rows[i]["DIVISION_CODE"].ToString();
                division_Region_Mst.DIVISION_NAME = data.Rows[i]["DIVISION_NAME"].ToString();

                division_Region_Mst.DIVISION_ID = Convert.ToInt32(data.Rows[i]["DIVISION_ID"].ToString());
                division_Region_Mst.DIVISION_REGION_MST_STATUS = data.Rows[i]["DIVISION_REGION_MST_STATUS"].ToString();
                division_Region_Mst.EFFECT_END_DATE = data.Rows[i]["EFFECT_END_DATE"].ToString();
                division_Region_Mst.EFFECT_START_DATE = data.Rows[i]["EFFECT_START_DATE"].ToString();
                division_Region_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                division_Region_Mst_list.Add(division_Region_Mst);
            }
            return JsonSerializer.Serialize(division_Region_Mst_list);
         }
    
      //public async Task<string> LoadData_Master(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Master_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> LoadData_MasterById(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<string> LoadData_DetailById(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Detail_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));
        public async Task<string> LoadData_DetailByMasterId(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<Division_Region_Mst> LoadDetailData_ByMasterId_List(string db, int Id)
        {
            Division_Region_Mst _Region_Mst = new Division_Region_Mst();
            
            DataTable dataTable =  await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));
            if(dataTable.Rows.Count>0)
            {
                _Region_Mst.COMPANY_ID = Convert.ToInt32(dataTable.Rows[0]["COMPANY_ID"]);
                _Region_Mst.DIVISION_REGION_MST_ID = Id;
                _Region_Mst.DIVISION_NAME = dataTable.Rows[0]["DIVISION_NAME"].ToString();

                _Region_Mst.DIVISION_CODE = dataTable.Rows[0]["DIVISION_CODE"].ToString();
                _Region_Mst.DIVISION_ID = Convert.ToInt32(dataTable.Rows[0]["DIVISION_ID"]);
                _Region_Mst.DIVISION_REGION_MST_STATUS = dataTable.Rows[0]["DIVISION_REGION_MST_STATUS"].ToString();
                _Region_Mst.EFFECT_START_DATE = dataTable.Rows[0]["EFFECT_START_DATE"].ToString();
                _Region_Mst.EFFECT_END_DATE = dataTable.Rows[0]["EFFECT_END_DATE"].ToString();
                _Region_Mst.REMARKS = dataTable.Rows[0]["REMARKS"].ToString();
               
                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));
               
                _Region_Mst.division_Region_Dtls = new List<Division_Region_Dtl>();

                for (int i=0;i< dataTable_detail.Rows.Count;i++)
                {
                    Division_Region_Dtl _Region_Dtl = new Division_Region_Dtl();

                    _Region_Dtl.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                    _Region_Dtl.REGION_ID = Convert.ToInt32(dataTable_detail.Rows[i]["REGION_ID"]);
                    _Region_Dtl.REGION_NAME = dataTable_detail.Rows[i]["REGION_NAME"].ToString();

                    _Region_Dtl.REGION_CODE = dataTable_detail.Rows[i]["REGION_CODE"].ToString();
                    _Region_Dtl.REMARKS = dataTable_detail.Rows[i]["REMARKS"].ToString();
                    _Region_Dtl.DIVISION_REGION_DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DIVISION_REGION_DTL_ID"]);
                    _Region_Dtl.DIVISION_REGION_DTL_STATUS = dataTable_detail.Rows[i]["DIVISION_REGION_DTL_STATUS"].ToString();
                    _Region_Dtl.EFFECT_START_DATE = dataTable_detail.Rows[i]["EFFECT_START_DATE"].ToString();
                    _Region_Dtl.EFFECT_END_DATE = dataTable_detail.Rows[i]["EFFECT_END_DATE"].ToString();
                    _Region_Mst.division_Region_Dtls.Add(_Region_Dtl);
                }
            }

            return _Region_Mst;



        }



    }
}
