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
    public class CustomerMarketRelationManager : ICustomerMarketRelationManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public CustomerMarketRelationManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Master_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.CUSTOMER_MARKET_MST_ID ASC) AS ROW_NO,
                                   M.CUSTOMER_MARKET_MST_ID, M.CUSTOMER_ID, N.CUSTOMER_NAME, M.CUSTOMER_CODE, M.COMPANY_ID, M.REMARKS,
                                   M.CUSTOMER_MARKET_MST_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE,  'DD/MM/YYYY') EFFECT_START_DATE  
                                   FROM CUSTOMER_MARKET_MST  M  
                                   INNER JOIN CUSTOMER_INFO N ON N.CUSTOMER_ID = M.CUSTOMER_ID
                                   Where M.COMPANY_ID = :param1";
        string LoadData_MasterById_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.CUSTOMER_MARKET_MST_ID ASC) AS ROW_NO,
                                   M.CUSTOMER_MARKET_MST_ID,  N.CUSTOMER_NAME,  M.CUSTOMER_ID,M.CUSTOMER_CODE, M.COMPANY_ID, M.REMARKS,
                                   M.CUSTOMER_MARKET_MST_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE  
                                   FROM CUSTOMER_MARKET_MST  M 
                                   INNER JOIN CUSTOMER_INFO N ON N.CUSTOMER_ID = M.CUSTOMER_ID
                                   Where M.CUSTOMER_MARKET_MST_ID = :param1";

        string AddOrUpdate_Master_AddQuery() => @"INSERT INTO CUSTOMER_MARKET_MST 
                                         (CUSTOMER_MARKET_MST_ID,CUSTOMER_ID, CUSTOMER_CODE, REMARKS,CUSTOMER_MARKET_MST_STATUS,
                                         COMPANY_ID,EFFECT_START_DATE,EFFECT_END_DATE , Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6,
                                         TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'), :param9,
                                         TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), :param11 )";
        string AddOrUpdate_Master_UpdateQuery() => @"UPDATE CUSTOMER_MARKET_MST SET 
                                           CUSTOMER_ID =  :param2,CUSTOMER_CODE = :param3,REMARKS = :param4,CUSTOMER_MARKET_MST_STATUS = :param5,
                                           COMPANY_ID = :param6, EFFECT_START_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),
                                           EFFECT_END_DATE = TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'),
                                            UPDATED_BY = :param9, UPDATED_DATE = TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param11 WHERE CUSTOMER_MARKET_MST_ID = :param1";
        string GetNewCustomer_Market_MST_IdQuery() => "SELECT NVL(MAX(CUSTOMER_MARKET_MST_ID),0) + 1 CUSTOMER_MARKET_MST_ID  FROM CUSTOMER_MARKET_MST";

        string LoadData_Detail_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.CUSTOMER_MARKET_DTL_ID ASC) AS ROW_NO,M.CUSTOMER_MARKET_DTL_ID,
                                          M.CUSTOMER_MARKET_MST_ID,   M.MARKET_ID,M.MARKET_CODE, M.COMPANY_ID, M.REMARKS, M.INVOICE_FLAG,
                                          M.CUSTOMER_MARKET_DTL_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE  
                                          FROM CUSTOMER_MARKET_DTL  M  Where M.CUSTOMER_MARKET_DTL_ID = :param1";
        string LoadData_DetailByMasterId_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.CUSTOMER_MARKET_DTL_ID ASC) AS ROW_NO,M.CUSTOMER_MARKET_DTL_ID,
                                          M.CUSTOMER_MARKET_MST_ID, N.MARKET_NAME,  M.MARKET_ID,M.MARKET_CODE, M.COMPANY_ID, M.REMARKS,M.INVOICE_FLAG,
                                          M.CUSTOMER_MARKET_DTL_STATUS,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE, TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE  
                                          FROM CUSTOMER_MARKET_DTL  M
                                          INNER JOIN MARKET_INFO N ON N.MARKET_ID = M.MARKET_ID
                                          Where M.CUSTOMER_MARKET_MST_ID = :param1";

        string AddOrUpdate_Detail_AddQuery() => @"INSERT INTO CUSTOMER_MARKET_DTL 
                                         (CUSTOMER_MARKET_DTL_ID, CUSTOMER_MARKET_MST_ID,MARKET_ID, MARKET_CODE, REMARKS,CUSTOMER_MARKET_DTL_STATUS,
                                         COMPANY_ID,EFFECT_START_DATE,EFFECT_END_DATE , Entered_By, Entered_Date,ENTERED_TERMINAL,INVOICE_FLAG ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6,:param7,
                                         TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'),TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'), :param10,
                                         TO_DATE(:param11, 'DD/MM/YYYY HH:MI:SS AM'), :param12,:param13 )";
        string AddOrUpdate_Detail_UpdateQuery() => @"UPDATE CUSTOMER_MARKET_DTL SET 
                                           MARKET_ID =  :param2,MARKET_CODE = :param3,REMARKS = :param4,CUSTOMER_MARKET_DTL_STATUS = :param5,
                                           COMPANY_ID = :param6, EFFECT_START_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),
                                           EFFECT_END_DATE = TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'),
                                            UPDATED_BY = :param9, UPDATED_DATE = TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param11,INVOICE_FLAG = :param12 WHERE CUSTOMER_MARKET_DTL_ID = :param1";

        string GetNewCustomer_Market_Dtl_IdQuery() => "SELECT NVL(MAX(CUSTOMER_MARKET_DTL_ID),0) + 1 CUSTOMER_MARKET_DTL_ID  FROM CUSTOMER_MARKET_DTL";
        string DeleteCustomer_Market_Dtl_IdQuery() => "DELETE  FROM CUSTOMER_MARKET_DTL WHere CUSTOMER_MARKET_DTL_ID = :param1 AND CUSTOMER_MARKET_MST_ID=:param2";
        string Existing_Customer_Load_Query() => @"Select CUSTOMER_ID,CUSTOMER_CODE, CUSTOMER_NAME from CUSTOMER_INFO where COMPANY_ID = :param1 AND  CUSTOMER_ID not in (SELECT distinct CUSTOMER_ID from CUSTOMER_MARKET_MST where COMPANY_ID = :param1)";
        string Existing_Market_Load_Query() => @"Select MARKET_ID,MARKET_CODE, MARKET_NAME from MARKET_INFO where  COMPANY_ID = :param1";
        //---------- Method Execution Part ------------------------------------------------

        public async Task<string> Existing_Customer_Load(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Existing_Customer_Load_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<string> Existing_Market_Load(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Existing_Market_Load_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<string> AddOrUpdate(string db, Customer_Market_Mst model)
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


                    if (model.CUSTOMER_MARKET_MST_ID == 0)
                    {
                        //-------------Add to master table--------------------

                        model.CUSTOMER_MARKET_MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewCustomer_Market_MST_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Master_AddQuery(), _commonServices.AddParameter(new string[] { model.CUSTOMER_MARKET_MST_ID.ToString(), model.CUSTOMER_ID.ToString(), model.CUSTOMER_CODE, model.REMARKS, model.CUSTOMER_MARKET_MST_STATUS, model.COMPANY_ID.ToString(), model.EFFECT_START_DATE, model.EFFECT_END_DATE, model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));
                        
                        //-------------Add to detail table--------------------

                        foreach (var item in model.customer_Market_Dtls)
                        {
                            detail_id = detail_id == 0? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewCustomer_Market_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (detail_id + 1);

                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(), _commonServices.AddParameter(new string[] { detail_id.ToString(), model.CUSTOMER_MARKET_MST_ID.ToString(), item.MARKET_ID.ToString(), item.MARKET_CODE, item.REMARKS, item.CUSTOMER_MARKET_DTL_STATUS, item.COMPANY_ID.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.ENTERED_BY.ToString(), item.ENTERED_DATE, item.ENTERED_TERMINAL,item.INVOICE_FLAG })));

                        }
                    }
                    else
                    {
                        Customer_Market_Mst diivision_Market_Mst =await  LoadDetailData_ByMasterId_List(db, model.CUSTOMER_MARKET_MST_ID);
                        //-------------Edit on Master table--------------------

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_Master_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.CUSTOMER_MARKET_MST_ID.ToString(),
                                model.CUSTOMER_ID.ToString(), model.CUSTOMER_CODE, model.REMARKS, model.CUSTOMER_MARKET_MST_STATUS, model.COMPANY_ID.ToString(), model.EFFECT_START_DATE, model.EFFECT_END_DATE,
                                model.UPDATED_BY.ToString(), model.UPDATED_DATE,
                                model.UPDATED_TERMINAL })));
                        foreach (var item in model.customer_Market_Dtls)
                        {
                            if(item.CUSTOMER_MARKET_DTL_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------

                                detail_id = detail_id == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewCustomer_Market_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (detail_id + 1);

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(), _commonServices.AddParameter(new string[] { detail_id.ToString(), model.CUSTOMER_MARKET_MST_ID.ToString(), item.MARKET_ID.ToString(), item.MARKET_CODE, item.REMARKS, item.CUSTOMER_MARKET_DTL_STATUS, item.COMPANY_ID.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.ENTERED_BY.ToString(), item.ENTERED_DATE, item.ENTERED_TERMINAL,item.INVOICE_FLAG })));

                            }
                            else
                            {
                                //-------------Edit on detail table--------------------

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_UpdateQuery(), _commonServices.AddParameter(new string[] { item.CUSTOMER_MARKET_DTL_ID.ToString(), item.MARKET_ID.ToString(), item.MARKET_CODE, item.REMARKS, item.CUSTOMER_MARKET_DTL_STATUS, item.COMPANY_ID.ToString(), item.EFFECT_START_DATE, item.EFFECT_END_DATE, item.UPDATED_BY.ToString(), item.UPDATED_DATE, item.UPDATED_TERMINAL, item.INVOICE_FLAG })));

                            }


                        }
                        foreach (var item in diivision_Market_Mst.customer_Market_Dtls)
                        {
                            bool status = true;

                            foreach (var updateditem in model.customer_Market_Dtls)
                            {
                                if(item.CUSTOMER_MARKET_DTL_ID == updateditem.CUSTOMER_MARKET_DTL_ID)
                                {
                                    status = false;
                                }
                            }
                            if(status)
                            {
                                //-------------Delete row from detail table--------------------

                                listOfQuery.Add(_commonServices.AddQuery(DeleteCustomer_Market_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { item.CUSTOMER_MARKET_DTL_ID.ToString(), model.CUSTOMER_MARKET_MST_ID.ToString() })));

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
            List<Customer_Market_Mst> customer_Market_Mst_list = new List<Customer_Market_Mst>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Master_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                Customer_Market_Mst customer_Market_Mst = new Customer_Market_Mst();
                customer_Market_Mst.CUSTOMER_MARKET_MST_ID = Convert.ToInt32(data.Rows[i]["CUSTOMER_MARKET_MST_ID"]);
                customer_Market_Mst.CUSTOMER_MARKET_MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["CUSTOMER_MARKET_MST_ID"].ToString());
                customer_Market_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                customer_Market_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                customer_Market_Mst.CUSTOMER_CODE = data.Rows[i]["CUSTOMER_CODE"].ToString();
                customer_Market_Mst.CUSTOMER_NAME = data.Rows[i]["CUSTOMER_NAME"].ToString();

                customer_Market_Mst.CUSTOMER_ID = Convert.ToInt32(data.Rows[i]["CUSTOMER_ID"].ToString());
                customer_Market_Mst.CUSTOMER_MARKET_MST_STATUS = data.Rows[i]["CUSTOMER_MARKET_MST_STATUS"].ToString();
                customer_Market_Mst.EFFECT_END_DATE = data.Rows[i]["EFFECT_END_DATE"].ToString();
                customer_Market_Mst.EFFECT_START_DATE = data.Rows[i]["EFFECT_START_DATE"].ToString();
                customer_Market_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                customer_Market_Mst_list.Add(customer_Market_Mst);
            }
            return JsonSerializer.Serialize(customer_Market_Mst_list);
         }
    
      //public async Task<string> LoadData_Master(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Master_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> LoadData_MasterById(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<string> LoadData_DetailById(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Detail_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));
        public async Task<string> LoadData_DetailByMasterId(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<Customer_Market_Mst> LoadDetailData_ByMasterId_List(string db, int Id)
        {
            Customer_Market_Mst _Market_Mst = new Customer_Market_Mst();
            
            DataTable dataTable =  await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));
            if(dataTable.Rows.Count>0)
            {
                _Market_Mst.COMPANY_ID = Convert.ToInt32(dataTable.Rows[0]["COMPANY_ID"]);
                _Market_Mst.CUSTOMER_MARKET_MST_ID = Id;
                _Market_Mst.CUSTOMER_NAME = dataTable.Rows[0]["CUSTOMER_NAME"].ToString();

                _Market_Mst.CUSTOMER_CODE = dataTable.Rows[0]["CUSTOMER_CODE"].ToString();
                _Market_Mst.CUSTOMER_ID = Convert.ToInt32(dataTable.Rows[0]["CUSTOMER_ID"]);
                _Market_Mst.CUSTOMER_MARKET_MST_STATUS = dataTable.Rows[0]["CUSTOMER_MARKET_MST_STATUS"].ToString();
                _Market_Mst.EFFECT_START_DATE = dataTable.Rows[0]["EFFECT_START_DATE"].ToString();
                _Market_Mst.EFFECT_END_DATE = dataTable.Rows[0]["EFFECT_END_DATE"].ToString();
                _Market_Mst.REMARKS = dataTable.Rows[0]["REMARKS"].ToString();
               
                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));
               
                _Market_Mst.customer_Market_Dtls = new List<Customer_Market_Dtl>();

                for (int i=0;i< dataTable_detail.Rows.Count;i++)
                {
                    Customer_Market_Dtl _Market_Dtl = new Customer_Market_Dtl();

                    _Market_Dtl.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                    _Market_Dtl.MARKET_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MARKET_ID"]);
                    _Market_Dtl.MARKET_NAME = dataTable_detail.Rows[i]["MARKET_NAME"].ToString();
                    _Market_Dtl.INVOICE_FLAG = dataTable_detail.Rows[i]["INVOICE_FLAG"].ToString();

                    _Market_Dtl.MARKET_CODE = dataTable_detail.Rows[i]["MARKET_CODE"].ToString();
                    _Market_Dtl.REMARKS = dataTable_detail.Rows[i]["REMARKS"].ToString();
                    _Market_Dtl.CUSTOMER_MARKET_DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["CUSTOMER_MARKET_DTL_ID"]);
                    _Market_Dtl.CUSTOMER_MARKET_DTL_STATUS = dataTable_detail.Rows[i]["CUSTOMER_MARKET_DTL_STATUS"].ToString();
                    _Market_Dtl.EFFECT_START_DATE = dataTable_detail.Rows[i]["EFFECT_START_DATE"].ToString();
                    _Market_Dtl.EFFECT_END_DATE = dataTable_detail.Rows[i]["EFFECT_END_DATE"].ToString();
                    _Market_Mst.customer_Market_Dtls.Add(_Market_Dtl);
                }
            }

            return _Market_Mst;



        }



    }
}
