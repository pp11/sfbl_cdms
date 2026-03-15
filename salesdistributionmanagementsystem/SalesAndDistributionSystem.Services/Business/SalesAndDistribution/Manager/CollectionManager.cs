using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public class CollectionManager : ICollectionManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;

        public CollectionManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;
        }

        //-------------------Query Part ---------------------------------------------------

        private string LoadData_MStQuery() => @"SELECT  COLLECTION_MST_ID, BATCH_NO,TO_CHAR( BATCH_DATE,'dd/MM/yyyy') BATCH_DATE, BATCH_STATUS, BATCH_POSTING_STATUS, COMPANY_ID, UNIT_ID, TO_CHAR( APPROVED_DATE, 'DD/MM/YYYY HH:MI:SS AM') APPROVED_DATE
                                    FROM Collection_Mst   Where COMPANY_ID = :param1  ORDER BY COLLECTION_MST_ID DESC";

        private string LoadDataById_MStQuery() => @"SELECT ROWNUM ROW_NO, COLLECTION_MST_ID, BATCH_NO, TO_CHAR(BATCH_DATE,'dd/MM/yyyy') BATCH_DATE, BATCH_STATUS, BATCH_POSTING_STATUS, COMPANY_ID, UNIT_ID
                                    FROM Collection_Mst   Where COLLECTION_MST_ID = :param1";

        private string AddOrUpdate_MStAddQuery() => @"INSERT INTO Collection_Mst
                                         (COLLECTION_MST_ID, BATCH_NO, BATCH_DATE, BATCH_STATUS, BATCH_POSTING_STATUS, COMPANY_ID, UNIT_ID,  Entered_By, Entered_Date,ENTERED_TERMINAL,VERSION_NO )
                                         VALUES ( :param1, :param2, TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM'), :param4, :param5, :param6, :param7, :param8,TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'), :param10,:param11 )";

        private string AddOrUpdate_MStUpdateQuery() => @"UPDATE Collection_Mst SET
                                            BATCH_STATUS = :param2,
                                            BATCH_POSTING_STATUS = :param3,UPDATED_BY = :param4, UPDATED_DATE = TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),
                                            UPDATED_TERMINAL = :param6 WHERE COLLECTION_MST_ID = :param1";

        private string GetNewBrand_MSt_IdQuery() => "SELECT NVL(MAX(COLLECTION_MST_ID),0) + 1 COLLECTION_MST_ID  FROM Collection_Mst";

        private string LoadData_DtlQuery(int unit_id) => @"
   SELECT DISTINCT D.COLLECTION_DTL_ID,
                  D.COLLECTION_MST_ID,
                  D.CUSTOMER_ID,
                  D.CUSTOMER_CODE,
                  C.CUSTOMER_NAME,
                  D.VOUCHER_NO,
                  TO_CHAR (D.VOUCHER_DATE, 'dd/MM/yyyy') VOUCHER_DATE,
                  D.BRANCH_ID,
                  D.BANK_ID,
                  D.REMARKS,
                  D.INVOICE_NO,
                  D.COLLECTION_MODE,
                  D.COLLECTION_AMT,
                  D.TDS_AMT,
                  D.MEMO_COST,
                  D.NET_COLLECTION_AMT,
                  D.COMPANY_ID,
                  D.UNIT_ID,
                  D.ENTERED_DATE,
                  P.CLOSING_INV_BALANCE AS BALANCE,
                  P.CLOSING_TDS_BALANCE AS BALANCE_TDS_AMT
    FROM Collection_Dtl D,
         Customer_Info C,
         (SELECT A.CUSTOMER_ID, A.CLOSING_INV_BALANCE, A.CLOSING_TDS_BALANCE
            FROM DATE_WISE_BALANCE_CENTRAL A
           WHERE A.BALANCE_DATE = (SELECT MAX (BALANCE_DATE)
                                     FROM DATE_WISE_BALANCE_CENTRAL
                                    WHERE CUSTOMER_ID = A.CUSTOMER_ID)) P
   WHERE     C.CUSTOMER_ID = D.CUSTOMER_ID
         AND D.CUSTOMER_ID = P.CUSTOMER_ID
         AND COLLECTION_MST_ID = :param1
ORDER BY D.ENTERED_DATE DESC, D.COLLECTION_DTL_ID ASC ";
        string LoadDetailsByMasterId_Query() => "SELECT D.COLLECTION_DTL_ID, D.COLLECTION_MST_ID FROM COLLECTION_DTL D WHERE D.COLLECTION_MST_ID=:param1 AND D.UNIT_ID=:param2";
        string DeleteCollectionDetailByIdQuery() => "DELETE FROM COLLECTION_DTL D WHERE D.COLLECTION_DTL_ID= :param1 AND D.COLLECTION_MST_ID=:param2 AND D.UNIT_ID=:param3 ";

        private string LoadBranch_Query() => @"Select B.BRANCH_NAME  || ' - ' ||  M.BANK_NAME as BRANCH_NAME, B.BRANCH_ID, M.BANK_ID, M.BANK_NAME from Branch_info B
left outer join Bank_info M on M.BANK_ID = B.BANK_ID";

        private string Load_CustomerDaywise_Balance_Query() => @"SELECT
    COMPANY_ID, UNIT_ID, BALANCE_DATE,
    CUSTOMER_ID, CUSTOMER_CODE, OPENING_INV_BALANCE,
    CLOSING_INV_BALANCE, OPENING_TDS_BALANCE, CLOSING_TDS_BALANCE
    FROM DATE_WISE_BALANCE WHERE CUSTOMER_ID = :param1 and unit_id = :param2 and BALANCE_DATE = (select MAX(BALANCE_DATE) from DATE_WISE_BALANCE where CUSTOMER_ID = :param1 and unit_Id = :param2 )";
        private string Load_CustomerDaywise_Balance_QueryV2() => @"SELECT COMPANY_ID,
       BALANCE_DATE,
       CUSTOMER_ID,
       CUSTOMER_CODE,
       OPENING_INV_BALANCE,
       CLOSING_INV_BALANCE,
       OPENING_TDS_BALANCE,
       CLOSING_TDS_BALANCE
FROM DATE_WISE_BALANCE_CENTRAL
WHERE     CUSTOMER_ID = :param1
       AND BALANCE_DATE = (SELECT MAX (BALANCE_DATE)
                             FROM DATE_WISE_BALANCE_CENTRAL
                            WHERE CUSTOMER_ID = :param1)";

        private string LoadCollectionMode_Query() => @"Select ID, MODE_NAME from Collection_Mode";

        private string AddOrUpdate_DtlAddQuery() => @"INSERT INTO Collection_Dtl
                                         (COLLECTION_DTL_ID, COLLECTION_MST_ID, CUSTOMER_ID,
                                         CUSTOMER_CODE, VOUCHER_NO, VOUCHER_DATE, BRANCH_ID, INVOICE_NO,
                                          COLLECTION_MODE, COLLECTION_AMT, TDS_AMT, MEMO_COST, NET_COLLECTION_AMT,
                                          COMPANY_ID, UNIT_ID , Entered_By, Entered_Date,ENTERED_TERMINAL,BANK_ID,REMARKS, VERSION_NO )
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), :param7,
                                          :param8, :param9, :param10,:param11, :param12, :param13,
                                          :param14, :param15, :param16,TO_DATE(:param17, 'DD/MM/YYYY HH:MI:SS AM'), :param18, :param19, :param20,:param21 )";

        private string AddOrUpdate_DtlUpdateQuery() => @"UPDATE Collection_Dtl SET
                                            CUSTOMER_CODE = :param2,
                                            CUSTOMER_ID = :param3,
                                            VOUCHER_NO = :param4,
                                            VOUCHER_DATE =TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),
                                            BRANCH_ID = :param6,
                                            INVOICE_NO = :param7,
                                            COLLECTION_MODE = :param8,
                                            COLLECTION_AMT = :param9,
                                            TDS_AMT = :param10,
                                            MEMO_COST = :param11,
                                            NET_COLLECTION_AMT = :param12,
                                            UPDATED_BY = :param13,
                                            UPDATED_DATE = TO_DATE(:param14, 'DD/MM/YYYY HH:MI:SS AM'),
                                            UPDATED_TERMINAL = :param15,
                                            BANK_ID = :param16,
                                            REMARKS = :param17
                                            WHERE COLLECTION_DTL_ID = :param1";

        private string GetNewBrand_Dtl_IdQuery() => "SELECT NVL(MAX(COLLECTION_DTL_ID),0) + 1 COLLECTION_DTL_ID  FROM Collection_Dtl";

        private string Update_Approval_Query() => @"update COLLECTION_MST SET BATCH_POSTING_STATUS = 'Approved',  APPROVED_BY = :param2,
                                            APPROVED_DATE = TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM'),
                                           APPROVED_TERMINAL = :param4 WHERE COLLECTION_MST_ID = :param1";

        //---------- Method Execution Part ------------------------------------------------
        public async Task<string> Update_Approval(string db, Collection_Mst model)
        {
            Result _result = new Result();
            if (model == null)
            {
                _result.Status = "No data provided to insert!!!!";
            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {
                    DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT COLLECTION_MST_ID, BATCH_POSTING_STATUS, APPROVED_BY, APPROVED_DATE, APPROVED_TERMINAL FROM COLLECTION_MST WHERE COLLECTION_MST_ID= :param1", _commonServices.AddParameter(new string[] { model.COLLECTION_MST_ID.ToString() }));
                    if (dataTable.Rows[0]["BATCH_POSTING_STATUS"].ToString() == "Approved")
                    {
                        _result.Status = "You are not able to post this collection as it has already been posted.";
                    }
                    else
                    {
                        listOfQuery.Add(_commonServices.AddQuery(Update_Approval_Query(), _commonServices.AddParameter(new string[] { model.COLLECTION_MST_ID.ToString(), model.ENTERED_BY, model.ENTERED_DATE, model.ENTERED_TERMINAL })));
                        listOfQuery.Add(_commonServices.AddQuery("update COLLECTION_DTL SET BATCH_POSTING_STATUS = 'Approved' WHERE COLLECTION_MST_ID = :param1", _commonServices.AddParameter(new string[] { model.COLLECTION_MST_ID.ToString() })));

                        await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                        _result.Key = _commonServices.Encrypt(model.COLLECTION_MST_ID.ToString());
                        _result.Status = "1";
                    }
                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }
            }
            return JsonSerializer.Serialize(_result);
        }

        public async Task<string> AddOrUpdate(string db, Collection_Mst model)
        {
            Result _result = new Result();
            if (model == null)
            {
                _result.Status = "No data provided to insert!!!!";
            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {
                    int dtlId = 0;
                    if (model.COLLECTION_MST_ID == 0)
                    {
                        model.COLLECTION_MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewBrand_MSt_IdQuery(), _commonServices.AddParameter(new string[] { }));
                        model.BATCH_NO = model.COLLECTION_MST_ID.ToString();
                        model.BATCH_POSTING_STATUS = "Pending";

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_MStAddQuery(), _commonServices.AddParameter(new string[] {
                                model.COLLECTION_MST_ID.ToString(), model.BATCH_NO, model.BATCH_DATE,
                                model.BATCH_STATUS, model.BATCH_POSTING_STATUS,
                                model.COMPANY_ID.ToString(),model.UNIT_ID.ToString(),
                                model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL,model.VERSION_NO })));

                        dtlId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewBrand_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { }));
                        foreach (var item in model.Collection_Dtls)
                        {
                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_DtlAddQuery(), _commonServices.AddParameter(new string[] {
                                     dtlId.ToString(),model.COLLECTION_MST_ID.ToString(),
                                     item.CUSTOMER_ID.ToString(),item.CUSTOMER_CODE,
                                     item.VOUCHER_NO.ToString(), item.VOUCHER_DATE.ToString(), item.BRANCH_ID.ToString(), item.INVOICE_NO.ToString(),
                                     item.COLLECTION_MODE, item.COLLECTION_AMT.ToString(), item.TDS_AMT.ToString(),
                                     item.MEMO_COST.ToString(),  item.NET_COLLECTION_AMT.ToString(),  model.COMPANY_ID.ToString(),  model.UNIT_ID.ToString(),
                                     model.ENTERED_BY,  model.ENTERED_DATE.ToString(),  model.ENTERED_TERMINAL,item.BANK_ID.ToString(),item.REMARKS, model.VERSION_NO
                              })));
                            dtlId++;
                        }
                    }
                    else
                    {
                        DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT COLLECTION_MST_ID, BATCH_POSTING_STATUS, APPROVED_BY, APPROVED_DATE, APPROVED_TERMINAL FROM COLLECTION_MST WHERE COLLECTION_MST_ID= :param1", _commonServices.AddParameter(new string[] { model.COLLECTION_MST_ID.ToString() }));
                        if (dataTable.Rows[0]["BATCH_POSTING_STATUS"].ToString() == "Approved")
                        {
                            _result.Status = "You are not able to edit this collection as it has already been posted.";
                            return JsonSerializer.Serialize(_result);
                        }
                        else
                        {
                            DataTable _dtl = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadDetailsByMasterId_Query(), _commonServices.AddParameter(new string[] { model.COLLECTION_MST_ID.ToString(), model.UNIT_ID.ToString() }));
                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_MStUpdateQuery(), _commonServices.AddParameter(new string[] {
                             model.COLLECTION_MST_ID.ToString(),
                             model.BATCH_STATUS, model.BATCH_POSTING_STATUS.ToString(),
                             model.UPDATED_BY.ToString(), model.UPDATED_DATE.ToString(),
                             model.UPDATED_TERMINAL.ToString()})));
                            

                            dtlId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewBrand_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { }));
                            foreach (var item in model.Collection_Dtls)
                            {
                                if (item.COLLECTION_DTL_ID == 0)
                                {
                                    listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_DtlAddQuery(), _commonServices.AddParameter(new string[] {
                                     dtlId.ToString(),model.COLLECTION_MST_ID.ToString(),
                                     item.CUSTOMER_ID.ToString(),item.CUSTOMER_CODE,
                                     item.VOUCHER_NO.ToString(), item.VOUCHER_DATE.ToString(), item.BRANCH_ID.ToString(), item.INVOICE_NO.ToString(),
                                     item.COLLECTION_MODE, item.COLLECTION_AMT.ToString(), item.TDS_AMT.ToString(),
                                     item.MEMO_COST.ToString(),  item.NET_COLLECTION_AMT.ToString(),  model.COMPANY_ID.ToString(),  model.UNIT_ID.ToString(),
                                     model.UPDATED_BY,  model.UPDATED_DATE.ToString(),  model.UPDATED_TERMINAL,item.BANK_ID.ToString(),item.REMARKS, model.VERSION_NO})));
                                    dtlId++;
                                }
                                else
                                {

                                    listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_DtlUpdateQuery(), _commonServices.AddParameter(new string[] {
                                     item.COLLECTION_DTL_ID.ToString(),item.CUSTOMER_CODE,
                                     item.CUSTOMER_ID.ToString(),item.VOUCHER_NO,
                                     item.VOUCHER_DATE, item.BRANCH_ID.ToString(), item.INVOICE_NO.ToString(),
                                     item.COLLECTION_MODE, item.COLLECTION_AMT.ToString(), item.TDS_AMT.ToString(),
                                     item.MEMO_COST.ToString(),  item.NET_COLLECTION_AMT.ToString(), 
                                     model.UPDATED_BY,  model.UPDATED_DATE.ToString(),  model.UPDATED_TERMINAL,item.BANK_ID.ToString(),item.REMARKS, model.VERSION_NO})));
                                                                    
                                }

                            }


                            foreach (DataRow row in _dtl.Rows)
                            {
                                bool status = true;

                                foreach (var updatedItem in model.Collection_Dtls)
                                {
                                    if (Convert.ToInt32(row["COLLECTION_DTL_ID"]) == updatedItem.COLLECTION_DTL_ID)
                                    {
                                        status = false;
                                    }
                                }
                                if (status)
                                {
                                    //-------------Delete row from detail table--------------------

                                    listOfQuery.Add(_commonServices.AddQuery(DeleteCollectionDetailByIdQuery(), _commonServices.AddParameter(new string[] { row["COLLECTION_DTL_ID"].ToString(), model.COLLECTION_MST_ID.ToString(), model.UNIT_ID.ToString() })));

                                }
                            }

                        }


                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    _result.Key = _commonServices.Encrypt(model.COLLECTION_MST_ID.ToString());
                    _result.Status = "1";
                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }
            }
            return JsonSerializer.Serialize(_result);
        }

        public async Task<string> LoadBranch(string db) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadBranch_Query(), _commonServices.AddParameter(new string[] { })));

        public async Task<string> LoadCollectionMode(string db) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadCollectionMode_Query(), _commonServices.AddParameter(new string[] { })));

        public async Task<string> LoadCustomerDaywiseBalance(string db, string customerId, string unit_id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Load_CustomerDaywise_Balance_Query(), _commonServices.AddParameter(new string[] { customerId, unit_id })));
        public async Task<string> LoadCustomerDaywiseBalanceV2(string db, string customerId, string unit_id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Load_CustomerDaywise_Balance_QueryV2(), _commonServices.AddParameter(new string[] { customerId })));

        public async Task<string> LoadData(string db, string Company_Id, string Unit_Id)
        {
            List<Collection_Mst> collection_Msts = new List<Collection_Mst>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MStQuery(), _commonServices.AddParameter(new string[] { Company_Id }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                Collection_Mst customer_Market_Mst = new Collection_Mst();
                customer_Market_Mst.COLLECTION_MST_ID = Convert.ToInt32(data.Rows[i]["COLLECTION_MST_ID"]);
                customer_Market_Mst.COLLECTION_MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["COLLECTION_MST_ID"].ToString());
                customer_Market_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                customer_Market_Mst.BATCH_DATE = data.Rows[i]["BATCH_DATE"].ToString();
                customer_Market_Mst.APPROVED_DATE = data.Rows[i]["APPROVED_DATE"].ToString();
                customer_Market_Mst.BATCH_NO = data.Rows[i]["BATCH_NO"].ToString();
                customer_Market_Mst.BATCH_POSTING_STATUS = data.Rows[i]["BATCH_POSTING_STATUS"].ToString();
                customer_Market_Mst.BATCH_STATUS = data.Rows[i]["BATCH_STATUS"].ToString();
                collection_Msts.Add(customer_Market_Mst);
            }
            return JsonSerializer.Serialize(collection_Msts);
        }

        public async Task<string> GetEditDataById(string db, int Id, int unit_id)
        {
            DataSet ds = new DataSet();
            DataTable _mst = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadDataById_MStQuery(), _commonServices.AddParameter(new string[] { Id.ToString() }));
            DataTable _dtl = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DtlQuery(unit_id), _commonServices.AddParameter(new string[] { Id.ToString() }));
            ds.Tables.Add(_mst);
            ds.Tables.Add(_dtl);

            return _commonServices.DataSetToJSON(ds);
        }
    }
}