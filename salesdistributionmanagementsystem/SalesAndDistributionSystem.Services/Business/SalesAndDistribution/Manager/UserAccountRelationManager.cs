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
    public class UserAccountRelationManager : IUserAccountRelationManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public UserAccountRelationManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Master_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.USER_ACCOUNT_MST_ID ASC) AS ROW_NO,
                                   M.USER_ACCOUNT_MST_ID, M.USER_ID, N.USER_NAME, M.USER_CODE, M.COMPANY_ID, M.REMARKS,
                                   M.USER_TYPE,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE,  'DD/MM/YYYY') EFFECT_START_DATE  
                                   FROM USER_ACCOUNT_RELATION_MST  M  
                                   INNER JOIN USER_INFO N ON N.USER_ID = M.USER_ID
                                   Where M.COMPANY_ID = :param1";
        string LoadData_MasterById_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.USER_ACCOUNT_MST_ID ASC) AS ROW_NO,
                                   M.USER_ACCOUNT_MST_ID,  N.USER_NAME,  M.USER_ID,M.USER_CODE, M.COMPANY_ID, M.REMARKS,
                                   M.USER_TYPE,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE  
                                   FROM USER_ACCOUNT_RELATION_MST  M 
                                   INNER JOIN USER_INFO N ON N.USER_ID = M.USER_ID
                                   Where M.USER_ACCOUNT_MST_ID = :param1";

        string AddOrUpdate_Master_AddQuery() => @"INSERT INTO USER_ACCOUNT_RELATION_MST 
                                         (USER_ACCOUNT_MST_ID,USER_ID, USER_CODE, REMARKS,USER_TYPE,
                                         COMPANY_ID,EFFECT_START_DATE,EFFECT_END_DATE , Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6,
                                         TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'), :param9,
                                         TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), :param11 )";
        string AddOrUpdate_Master_UpdateQuery() => @"UPDATE USER_ACCOUNT_RELATION_MST SET 
                                           USER_ID =  :param2,USER_CODE = :param3,REMARKS = :param4,USER_TYPE = :param5,
                                           COMPANY_ID = :param6, EFFECT_START_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),
                                            EFFECT_END_DATE = TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'),
                                            UPDATED_BY = :param9, UPDATED_DATE = TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param11 WHERE USER_ACCOUNT_MST_ID = :param1";
        string GetNewUser_Account_MST_IdQuery() => "SELECT NVL(MAX(USER_ACCOUNT_MST_ID),0) + 1 USER_ACCOUNT_MST_ID  FROM USER_ACCOUNT_RELATION_MST";

        string LoadData_Detail_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.USER_ACCOUNT_DTL_ID ASC) AS ROW_NO,M.USER_ACCOUNT_DTL_ID,
                                          M.USER_ACCOUNT_MST_ID,   M.ACCOUNT_ID,M.ACCOUNT_CODE, M.COMPANY_ID, M.REMARKS,
                                          M.USER_TYPE,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE  
                                          FROM USER_ACCOUNT_RELATION_DTL  M  Where M.USER_ACCOUNT_DTL_ID = :param1";
        string LoadData_DetailByMasterId_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.USER_ACCOUNT_DTL_ID ASC) AS ROW_NO,M.USER_ACCOUNT_DTL_ID,
                                          M.USER_ACCOUNT_MST_ID, N.CUSTOMER_NAME ACCOUNT_NAME,  M.ACCOUNT_ID,M.ACCOUNT_CODE, M.COMPANY_ID, M.REMARKS,
                                          M.USER_TYPE,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE  
                                          FROM USER_ACCOUNT_RELATION_DTL  M
                                          INNER JOIN CUSTOMER_INFO N ON N.CUSTOMER_ID = M.ACCOUNT_ID
                                          Where M.USER_ACCOUNT_MST_ID = :param1";

        string AddOrUpdate_Detail_AddQuery() => @"INSERT INTO USER_ACCOUNT_RELATION_DTL 
                                         (USER_ACCOUNT_DTL_ID, USER_ACCOUNT_MST_ID,ACCOUNT_ID, ACCOUNT_CODE, REMARKS,USER_TYPE,
                                         COMPANY_ID,EFFECT_START_DATE,EFFECT_END_DATE , Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6,:param7,
                                         TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'),TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'), :param10,
                                         TO_DATE(:param11, 'DD/MM/YYYY HH:MI:SS AM'), :param12 )";
        string AddOrUpdate_Detail_UpdateQuery() => @"UPDATE USER_ACCOUNT_RELATION_DTL SET 
                                           ACCOUNT_ID =  :param2,ACCOUNT_CODE = :param3,REMARKS = :param4,USER_TYPE = :param5,
                                           COMPANY_ID = :param6, EFFECT_START_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),
                                           EFFECT_END_DATE = TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'),
                                            UPDATED_BY = :param9, UPDATED_DATE = TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param11 WHERE USER_ACCOUNT_DTL_ID = :param1";

        string GetNewUser_Account_Dtl_IdQuery() => "SELECT NVL(MAX(USER_ACCOUNT_DTL_ID),0) + 1 USER_ACCOUNT_DTL_ID  FROM USER_ACCOUNT_RELATION_DTL";
        string DeleteUser_Account_Dtl_IdQuery() => "DELETE  FROM USER_ACCOUNT_RELATION_DTL WHere USER_ACCOUNT_DTL_ID = :param1";
        string Existing_User_Load_Query() => @"Select USER_ID,TO_CHAR(USER_ID) USER_CODE, USER_NAME from USER_INFO where COMPANY_ID = :param1 AND  USER_ID not in (SELECT distinct USER_ID from USER_ACCOUNT_RELATION_MST where COMPANY_ID = :param1)";
        string Existing_Account_Load_Query() => @"SELECT CUSTOMER_ID ACCOUNT_ID,
       CUSTOMER_CODE ACCOUNT_CODE,
       CUSTOMER_NAME ACCOUNT_NAME
       FROM CUSTOMER_INFO
       WHERE     COMPANY_ID = :param1
       AND CUSTOMER_TYPE_ID = 1
       AND CUSTOMER_STATUS = 'Active'
       AND CUSTOMER_ID NOT IN (SELECT DISTINCT D.ACCOUNT_ID
                                 FROM USER_ACCOUNT_RELATION_DTL D, USER_ACCOUNT_RELATION_MST M , USER_INFO U
                                WHERE  M.USER_ACCOUNT_MST_ID = D.USER_ACCOUNT_MST_ID AND  U.USER_ID = M.USER_ID AND U.USER_TYPE != 'OSM' ) AND COMPANY_ID = :param1";
        //---------- Method Execution Part ------------------------------------------------

        public async Task<string> Existing_User_Load(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Existing_User_Load_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<string> Existing_Account_Load(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Existing_Account_Load_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<string> AddOrUpdate(string db, User_Account_Relation_Mst model)
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


                    if (model.USER_ACCOUNT_MST_ID == 0)
                    {
                        //-------------Add to master table--------------------

                        model.USER_ACCOUNT_MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), 
                            GetNewUser_Account_MST_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Master_AddQuery(), _commonServices.AddParameter(new string[] { model.USER_ACCOUNT_MST_ID.ToString(), model.USER_CODE.ToString(), model.USER_CODE, model.REMARKS, model.USER_TYPE, model.COMPANY_ID.ToString(), model.EFFECT_START_DATE, model.EFFECT_END_DATE, model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));
                        
                        //-------------Add to detail table--------------------

                        foreach (var item in model.user_account_relation_Dtls)
                        {
                            detail_id = detail_id == 0? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewUser_Account_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (detail_id + 1);

                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(), _commonServices.AddParameter(new string[] { detail_id.ToString(), model.USER_ACCOUNT_MST_ID.ToString(), item.ACCOUNT_ID.ToString(), item.ACCOUNT_CODE, item.REMARKS, item.USER_TYPE, item.COMPANY_ID.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.ENTERED_BY.ToString(), item.ENTERED_DATE, item.ENTERED_TERMINAL })));

                        }
                    }
                    else
                    {
                        User_Account_Relation_Mst diivision_Region_Mst =await  LoadDetailData_ByMasterId_List(db, model.USER_ACCOUNT_MST_ID);
                        //-------------Edit on Master table--------------------

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_Master_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.USER_ACCOUNT_MST_ID.ToString(),
                                model.USER_ID.ToString(), model.USER_CODE, model.REMARKS, model.USER_TYPE, model.COMPANY_ID.ToString(), model.EFFECT_START_DATE, model.EFFECT_END_DATE,
                                model.UPDATED_BY.ToString(), model.UPDATED_DATE,
                                model.UPDATED_TERMINAL })));
                        foreach (var item in model.user_account_relation_Dtls)
                        {
                            if(item.USER_ACCOUNT_DTL_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------

                                detail_id = detail_id == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewUser_Account_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (detail_id + 1);

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(), _commonServices.AddParameter(new string[] { detail_id.ToString(), model.USER_ACCOUNT_MST_ID.ToString(), item.ACCOUNT_ID.ToString(), item.ACCOUNT_CODE, item.REMARKS, item.USER_TYPE, item.COMPANY_ID.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.ENTERED_BY.ToString(), item.ENTERED_DATE, item.ENTERED_TERMINAL })));

                            }
                            else
                            {
                                //-------------Edit on detail table--------------------

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_UpdateQuery(), _commonServices.AddParameter(new string[] { item.USER_ACCOUNT_DTL_ID.ToString(), item.ACCOUNT_ID.ToString(), item.ACCOUNT_CODE, item.REMARKS, item.USER_TYPE, item.COMPANY_ID.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.UPDATED_BY.ToString(), item.UPDATED_DATE, item.UPDATED_TERMINAL })));

                            }


                        }
                        foreach (var item in diivision_Region_Mst.user_account_relation_Dtls)
                        {
                            bool status = true;

                            foreach (var updateditem in model.user_account_relation_Dtls)
                            {
                                if(item.USER_ACCOUNT_DTL_ID == updateditem.USER_ACCOUNT_DTL_ID)
                                {
                                    status = false;
                                }
                            }
                            if(status)
                            {
                                //-------------Delete row from detail table--------------------

                                listOfQuery.Add(_commonServices.AddQuery(DeleteUser_Account_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { item.USER_ACCOUNT_DTL_ID.ToString() })));

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
            List<User_Account_Relation_Mst> division_Region_Mst_list = new List<User_Account_Relation_Mst>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Master_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                User_Account_Relation_Mst division_Region_Mst = new User_Account_Relation_Mst();
                division_Region_Mst.USER_ACCOUNT_MST_ID = Convert.ToInt32(data.Rows[i]["USER_ACCOUNT_MST_ID"]);
                division_Region_Mst.USER_ACCOUNT_MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["USER_ACCOUNT_MST_ID"].ToString());
                division_Region_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                division_Region_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                division_Region_Mst.USER_CODE = data.Rows[i]["USER_CODE"].ToString();
                division_Region_Mst.USER_NAME = data.Rows[i]["USER_NAME"].ToString();

                division_Region_Mst.USER_ID = Convert.ToInt32(data.Rows[i]["USER_ID"].ToString());
                division_Region_Mst.USER_TYPE = data.Rows[i]["USER_TYPE"].ToString();
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

        public async Task<User_Account_Relation_Mst> LoadDetailData_ByMasterId_List(string db, int Id)
        {
            User_Account_Relation_Mst _Region_Mst = new User_Account_Relation_Mst();
            
            DataTable dataTable =  await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));
            if(dataTable.Rows.Count>0)
            {
                _Region_Mst.COMPANY_ID = Convert.ToInt32(dataTable.Rows[0]["COMPANY_ID"]);
                _Region_Mst.USER_ACCOUNT_MST_ID = Id;
                _Region_Mst.USER_NAME = dataTable.Rows[0]["USER_NAME"].ToString();

                _Region_Mst.USER_CODE = dataTable.Rows[0]["USER_CODE"].ToString();
                _Region_Mst.USER_ID = Convert.ToInt32(dataTable.Rows[0]["USER_ID"]);
                _Region_Mst.USER_TYPE = dataTable.Rows[0]["USER_TYPE"].ToString();
                _Region_Mst.EFFECT_START_DATE = dataTable.Rows[0]["EFFECT_START_DATE"].ToString();
                _Region_Mst.EFFECT_END_DATE = dataTable.Rows[0]["EFFECT_END_DATE"].ToString();
                _Region_Mst.REMARKS = dataTable.Rows[0]["REMARKS"].ToString();
               
                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));
               
                _Region_Mst.user_account_relation_Dtls = new List<User_Account_Relation_Dtl>();

                for (int i=0;i< dataTable_detail.Rows.Count;i++)
                {
                    User_Account_Relation_Dtl _Region_Dtl = new User_Account_Relation_Dtl();

                    _Region_Dtl.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                    _Region_Dtl.ACCOUNT_ID = Convert.ToInt32(dataTable_detail.Rows[i]["ACCOUNT_ID"]);
                    _Region_Dtl.ACCOUNT_NAME = dataTable_detail.Rows[i]["ACCOUNT_NAME"].ToString();

                    _Region_Dtl.ACCOUNT_CODE = dataTable_detail.Rows[i]["ACCOUNT_CODE"].ToString();
                    _Region_Dtl.REMARKS = dataTable_detail.Rows[i]["REMARKS"].ToString();
                    _Region_Dtl.USER_ACCOUNT_DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["USER_ACCOUNT_DTL_ID"]);
                    _Region_Dtl.USER_TYPE = dataTable_detail.Rows[i]["USER_TYPE"].ToString();
                    _Region_Dtl.EFFECT_START_DATE = dataTable_detail.Rows[i]["EFFECT_START_DATE"].ToString();
                    _Region_Dtl.EFFECT_END_DATE = dataTable_detail.Rows[i]["EFFECT_END_DATE"].ToString();
                    _Region_Mst.user_account_relation_Dtls.Add(_Region_Dtl);
                }
            }

            return _Region_Mst;



        }



    }
}
