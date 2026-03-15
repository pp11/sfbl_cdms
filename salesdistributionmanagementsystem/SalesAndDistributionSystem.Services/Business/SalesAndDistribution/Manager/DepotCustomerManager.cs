using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.Manager
{
    public class DepotCustomerManager : IDepotCustomerManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public DepotCustomerManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        // Master
        string Load_UnitData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.ID ASC) AS ROW_NO,
                                        M.COMPANY_ID, M.COMPANY_NAME, M.COMPANY_SHORT_NAME, M.COMPANY_ADDRESS1, M.COMPANY_ADDRESS2, M.UNIT_ID, M.UNIT_NAME, M.UNIT_SHORT_NAME, M.UNIT_ADDRESS1, M.UNIT_ADDRESS2, M.UNIT_TYPE FROM COMPANY_INFO  M  
                                        Where M.COMPANY_ID = :param1";

        string LoadData_Master_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO, M.MST_ID,
                                   M.DEPOT_ID, U.UNIT_ID DEPOT_CODE,U.UNIT_NAME DEPOT_NAME, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE , TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, M.STATUS, M.COMPANY_ID, M.REMARKS
                                   FROM DEPOT_CUSTOMER_MST  M  
                                   LEFT OUTER JOIN STL_ERP_SCS.COMPANY_INFO U ON U.COMPANY_ID=M.COMPANY_ID AND U.UNIT_ID=M.DEPOT_ID
                                   Where M.COMPANY_ID = :param1";

        string LoadData_MasterById_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO, M.MST_ID,
                                   M.DEPOT_ID, M.DEPOT_CODE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE , TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, M.STATUS, M.COMPANY_ID, M.REMARKS                             
                                   FROM DEPOT_CUSTOMER_MST  M  
                                   Where M.MST_ID = :param1";

        string AddOrUpdate_Master_AddQuery() => @"INSERT INTO DEPOT_CUSTOMER_MST 
                                         (MST_ID,DEPOT_ID, DEPOT_CODE, EFFECT_START_DATE, EFFECT_END_DATE, STATUS, COMPANY_ID, REMARKS, Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3,TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'),TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'), :param6,:param7,:param8,:param9,
                                         TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), :param11 )";
        string AddOrUpdate_Master_UpdateQuery() => @"UPDATE DEPOT_CUSTOMER_MST SET 
                                           DEPOT_ID =  :param2,DEPOT_CODE = :param3, EFFECT_START_DATE = TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'),
                                           EFFECT_END_DATE = TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'), STATUS = :param6, COMPANY_ID = :param7, REMARKS = :param8,
                                            UPDATED_BY = :param9, UPDATED_DATE = TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param11 WHERE MST_ID = :param1";

        string GetNewDepotCustomer_IdQuery() => "SELECT NVL(MAX(MST_ID),0) + 1 MST_ID  FROM DEPOT_CUSTOMER_MST";

        // Detail
        string LoadData_DetailByMasterId_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.DTL_ID ASC) AS ROW_NO, 
                                   M.MST_ID,  DTL_ID, M.CUSTOMER_ID, M.CUSTOMER_CODE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE , TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, M.STATUS, M.COMPANY_ID, M.REMARKS                              
                                   FROM DEPOT_CUSTOMER_DTL  M  
                                   Where M.MST_ID = :param1";

        string LoadData_DetailById_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.DTL_ID ASC) AS ROW_NO,
                                   M.MST_ID, M.CUSTOMER_ID, M.CUSTOMER_CODE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE , TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, M.STATUS, M.COMPANY_ID, M.REMARKS,                               
                                   FROM DEPOT_CUSTOMER_DTL  M  
                                   Where M.DTL_ID = :param1";

        string AddOrUpdate_Detail_AddQuery() => @"INSERT INTO DEPOT_CUSTOMER_DTL 
                                         (DTL_ID, MST_ID, CUSTOMER_ID, CUSTOMER_CODE, EFFECT_START_DATE, EFFECT_END_DATE, STATUS, COMPANY_ID, REMARKS, Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4,TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), :param7,:param8,:param9,:param10,
                                         TO_DATE(:param11, 'DD/MM/YYYY HH:MI:SS AM'), :param12 )";

        string AddOrUpdate_Detail_UpdateQuery() => @"UPDATE DEPOT_CUSTOMER_DTL SET 
                                           MST_ID =:param2, CUSTOMER_ID =  :param3,CUSTOMER_CODE = :param4, EFFECT_START_DATE = TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),
                                           EFFECT_END_DATE = TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), STATUS = :param7, COMPANY_ID = :param8, REMARKS = :param9,
                                           UPDATED_BY = :param10, UPDATED_DATE = TO_DATE(:param11, 'DD/MM/YYYY HH:MI:SS AM'), 
                                           UPDATED_TERMINAL = :param12 WHERE DTL_ID = :param1";

        string GetNewDepotDtl_IdQuery() => "SELECT NVL(MAX(DTL_ID),0) + 1 DTL_ID  FROM DEPOT_CUSTOMER_DTL";

        string DeleteDepot_Dtl_IdQuery() => "DELETE  FROM DEPOT_CUSTOMER_DTL WHere DTL_ID = :param1";


        //---------- Method Execution Part ------------------------------------------------

        public async Task<string> LoadData_Master(string db, int Company_Id)
        {
            List<Depot_Customer_Mst> Depot_Customer_Mst_list = new List<Depot_Customer_Mst>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Master_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                Depot_Customer_Mst depot_Customer_Mst = new Depot_Customer_Mst();
                depot_Customer_Mst.MST_ID = Convert.ToInt32(data.Rows[i]["MST_ID"]);
                depot_Customer_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());

                depot_Customer_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                depot_Customer_Mst.DEPOT_NAME = (data.Rows[i]["DEPOT_NAME"]).ToString();
                depot_Customer_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                depot_Customer_Mst.DEPOT_ID = Convert.ToInt32(data.Rows[i]["DEPOT_ID"]);
                depot_Customer_Mst.DEPOT_CODE = data.Rows[i]["DEPOT_CODE"].ToString();
                depot_Customer_Mst.EFFECT_END_DATE = data.Rows[i]["EFFECT_END_DATE"].ToString();
                depot_Customer_Mst.EFFECT_START_DATE = data.Rows[i]["EFFECT_START_DATE"].ToString();
                depot_Customer_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                depot_Customer_Mst.STATUS = data.Rows[i]["STATUS"].ToString();
                Depot_Customer_Mst_list.Add(depot_Customer_Mst);
            }
            return JsonSerializer.Serialize(Depot_Customer_Mst_list);
        }


        public async Task<Depot_Customer_Mst> LoadDetailData_ByMasterId_List(string db, int Id)
        {
            Depot_Customer_Mst _Depot_Mst = new Depot_Customer_Mst();

            DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));
            if (dataTable.Rows.Count > 0)
            {
                _Depot_Mst.COMPANY_ID = Convert.ToInt32(dataTable.Rows[0]["COMPANY_ID"]);
                _Depot_Mst.MST_ID = Id;
                _Depot_Mst.DEPOT_ID = Convert.ToInt32(dataTable.Rows[0]["DEPOT_ID"]);
                _Depot_Mst.DEPOT_CODE = dataTable.Rows[0]["DEPOT_CODE"].ToString(); 
                _Depot_Mst.EFFECT_START_DATE = dataTable.Rows[0]["EFFECT_START_DATE"].ToString();
                _Depot_Mst.EFFECT_END_DATE = dataTable.Rows[0]["EFFECT_END_DATE"].ToString();
                _Depot_Mst.REMARKS = dataTable.Rows[0]["REMARKS"].ToString();

                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));

                _Depot_Mst.Depot_Customer_Dtls = new List<Depot_Customer_Dtl>();

                for (int i = 0; i < dataTable_detail.Rows.Count; i++)
                {
                    Depot_Customer_Dtl _Depot_Dtl = new Depot_Customer_Dtl();

                    _Depot_Dtl.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                    _Depot_Dtl.DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DTL_ID"]);
                    _Depot_Dtl.MST_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MST_ID"]);

                    _Depot_Dtl.CUSTOMER_ID = Convert.ToInt32(dataTable_detail.Rows[i]["CUSTOMER_ID"]);
                    _Depot_Dtl.CUSTOMER_CODE = dataTable_detail.Rows[i]["CUSTOMER_CODE"].ToString();
                    _Depot_Dtl.EFFECT_START_DATE = dataTable_detail.Rows[i]["EFFECT_START_DATE"].ToString();
                    _Depot_Dtl.EFFECT_END_DATE = dataTable_detail.Rows[i]["EFFECT_END_DATE"].ToString();
                    _Depot_Dtl.REMARKS = dataTable_detail.Rows[i]["REMARKS"].ToString();
                    _Depot_Mst.Depot_Customer_Dtls.Add(_Depot_Dtl);
                }
            }

            return _Depot_Mst;
        }

        public async Task<string> AddOrUpdate(string db, Depot_Customer_Mst model)
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

                    if (model.MST_ID == 0)
                    {
                        //-------------Add to master table--------------------

                        model.MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewDepotCustomer_IdQuery(), _commonServices.AddParameter(new string[] { }));
                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Master_AddQuery(), _commonServices.AddParameter(new string[] { model.MST_ID.ToString(), model.DEPOT_ID.ToString(), model.DEPOT_CODE.ToString(),  model.EFFECT_START_DATE, model.EFFECT_END_DATE, model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS, model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));

                        //-------------Add to detail table--------------------

                        foreach (var item in model.Depot_Customer_Dtls)
                        {
                            detail_id = detail_id == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewDepotDtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (detail_id + 1);
                            item.MST_ID = model.MST_ID;
                            item.CUSTOMER_ID = Convert.ToInt32(item.CUSTOMER_CODE);
                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(), _commonServices.AddParameter(new string[] { detail_id.ToString(), item.MST_ID.ToString(), item.CUSTOMER_ID.ToString(), item.CUSTOMER_CODE.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.STATUS, item.COMPANY_ID.ToString(), item.REMARKS, item.ENTERED_BY.ToString(), item.ENTERED_DATE, item.ENTERED_TERMINAL })));
                        }
                    }
                    else
                    {
                        Depot_Customer_Mst depot_Customer_Mst = await LoadDetailData_ByMasterId_List(db, model.MST_ID);
                        //-------------Edit on Master table--------------------

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_Master_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.MST_ID.ToString(), model.DEPOT_ID.ToString(), model.DEPOT_CODE.ToString(),  model.EFFECT_START_DATE, model.EFFECT_END_DATE, model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                                    model.UPDATED_BY.ToString(), model.UPDATED_DATE,
                                    model.UPDATED_TERMINAL })));

                        foreach (var item in model.Depot_Customer_Dtls)
                        {
                            if (item.DTL_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------

                                detail_id = detail_id == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewDepotDtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (detail_id + 1);
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(), _commonServices.AddParameter(new string[] { detail_id.ToString(), model.MST_ID.ToString(), item.CUSTOMER_ID.ToString(), item.CUSTOMER_CODE.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.STATUS, item.COMPANY_ID.ToString(), item.REMARKS, item.ENTERED_BY.ToString(), item.ENTERED_DATE, item.ENTERED_TERMINAL })));
                            }
                            else
                            {
                                //-------------Edit on detail table--------------------
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_UpdateQuery(), _commonServices.AddParameter(new string[] {  item.DTL_ID.ToString(), item.MST_ID.ToString(), item.CUSTOMER_ID.ToString(), item.CUSTOMER_CODE.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.STATUS, item.COMPANY_ID.ToString(), item.REMARKS, item.ENTERED_BY.ToString(), item.ENTERED_DATE, item.ENTERED_TERMINAL })));

                            }
                        }
                        foreach (var item in depot_Customer_Mst.Depot_Customer_Dtls)
                        {
                            bool status = true;

                            foreach (var updateditem in model.Depot_Customer_Dtls)
                            {
                                if (item.DTL_ID == updateditem.DTL_ID)
                                {
                                    status = false;
                                }
                            }
                            if (status)
                            {
                                //-------------Delete row from detail table--------------------

                                listOfQuery.Add(_commonServices.AddQuery(DeleteDepot_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { item.DTL_ID.ToString() })));
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

        public async Task<string> LoadData_DetailByMasterId(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<string> LoadData_DetailById(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));
    }
}
