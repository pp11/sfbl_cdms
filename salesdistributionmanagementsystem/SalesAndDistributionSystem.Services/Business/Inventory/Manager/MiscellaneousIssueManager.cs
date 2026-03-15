using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Business.User;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.Manager
{
    public class MiscellaneousIssueManager : IMiscellaneousIssueManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        private readonly IUserManager _userManager;

        public MiscellaneousIssueManager(ICommonServices commonServices, IConfiguration configuration, IUserManager userManager)
        {
            _commonServices = commonServices;
            _configuration = configuration;
            _userManager = userManager;
        }

        string LoadProductData_Query() => @"
                                         Select
                                        ROW_NUMBER() OVER(ORDER BY p.SKU_ID ASC) AS ROW_NO  
                                        ,p.SKU_CODE
                                        ,p.SKU_ID
                                        ,p.SKU_NAME
                                        ,p.SKU_NAME_BANGLA
                                        ,p.PACK_SIZE  
                                        ,p.UNIT_TP,
                                        nvl(PASSED_QTY,0) STOCK_QTY
                                        from VW_PRODUCT_PRICE p, 
                                        (
                                        SELECT SKU_ID, COMPANY_ID,UNIT_ID ,
                                        SUM(nvl(PASSED_QTY,0)) PASSED_QTY
                                        FROM VW_BATCH_WISE_STOCK V
                                        WHERE UNIT_ID = :param1
                                        GROUP BY UNIT_ID,SKU_ID,COMPANY_ID
                                         ) V
                                        where p.SKU_ID=v.SKU_ID
                                        and   p.COMPANY_ID=v.COMPANY_ID
                                        AND p.COMPANY_ID = :param2";

        string GetMiscIssue_Mst_IdQuery() => "SELECT NVL(MAX(MST_ID),0) + 1 MST_ID  FROM MISCELLANEOUS_ISSUE_MST";
        string GetMiscIssue_Dtl_IdQuery() => "SELECT NVL(MAX(DTL_ID),0) + 1 DTL_ID  FROM MISCELLANEOUS_ISSUE_DTL";
        string AddOrUpdate_AddQuery() => @"INSERT INTO MISCELLANEOUS_ISSUE_MST
                                        (
                                             MST_ID                                        
                                            ,UNIT_ID
                                            ,ISSUE_NO
                                            ,ISSUE_DATE
                                            ,SUBJECT
                                            ,RAISED_FROM
                                            ,ISSUE_BY                                            
                                            ,COMPANY_ID
                                            ,STATUS
                                            ,REMARKS
                                            ,ENTERED_BY
                                            ,ENTERED_DATE
                                            ,ENTERED_TERMINAL
                                         ) 
                                          VALUES ( :param1, :param2, :param3,TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'), :param5,:param6, :param7, :param8, :param9, :param10, :param11,TO_DATE(:param12, 'DD/MM/YYYY HH:MI:SS AM'), :param13)";
        string AddOrUpdate_AddQueryDTL() => @"INSERT INTO MISCELLANEOUS_ISSUE_DTL 
                    (MST_ID
                        ,DTL_ID
                        ,UNIT_ID
                        ,ISSUE_DATE 
                        ,SKU_ID
                        ,SKU_CODE
                        ,UNIT_TP                        
                        ,ISSUE_QTY
                        ,ISSUE_AMOUNT
                        ,STATUS
                        ,COMPANY_ID
                        ,REMARKS
                        ,ENTERED_BY
                        ,ENTERED_DATE
                        ,ENTERED_TERMINAL
              
                   ) 
                    VALUES ( :param1, :param2, :param3, TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'), :param5, :param6, :param7, :param8, :param9, :param10, :param11, :param12,:param13,TO_DATE(:param14, 'DD/MM/YYYY HH:MI:SS AM'),:param15)";

        string AddOrUpdate_UpdateQuery() => @"UPDATE MISCELLANEOUS_ISSUE_MST SET  
                                                 UNIT_ID= :param2
                                                ,ISSUE_NO= :param3                                                
                                                ,ISSUE_DATE= TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,SUBJECT= :param5
                                                ,RAISED_FROM= :param6
                                                ,ISSUE_BY= :param7
                                                ,STATUS= :param8
                                                ,COMPANY_ID= :param9
                                                ,REMARKS= :param10
                                                ,UPDATED_BY= :param11
                                                ,UPDATED_DATE= TO_DATE(:param12, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,UPDATED_TERMINAL= :param13
                                                WHERE MST_ID = :param1";

        string AddOrUpdate_UpdateQueryDTL() => @"UPDATE MISCELLANEOUS_ISSUE_DTL SET                                                   
                                                 ISSUE_QTY= :param2
                                                ,ISSUE_AMOUNT= :param3
                                                ,STATUS= :param4                                                
                                                ,REMARKS= :param5
                                                ,UPDATED_BY= :param6
                                                ,UPDATED_DATE=  TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,UPDATED_TERMINAL= :param8
                                                WHERE DTL_ID = :param1";
        string LoadData_MasterById_Query() => @"
         SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO,M.MST_ID                                       
                                            ,M.UNIT_ID
                                            ,M.ISSUE_NO                                            
                                            ,TO_CHAR(M.ISSUE_DATE, 'DD/MM/YYYY') ISSUE_DATE
                                            ,M.SUBJECT
                                            ,M.RAISED_FROM
                                            ,M.ISSUE_BY                                            
                                            ,M.STATUS
                                            ,M.COMPANY_ID
                                            ,M.REMARKS
                                            ,M.ENTERED_BY
                                            ,M.ENTERED_DATE
                                            ,M.ENTERED_TERMINAL                
            FROM MISCELLANEOUS_ISSUE_MST  M WHERE M.MST_ID= :param1
            ";
        string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO,M.MST_ID                                       
                                            ,M.UNIT_ID
                                            ,M.ISSUE_NO                                            
                                            ,TO_CHAR(M.ISSUE_DATE, 'DD/MM/YYYY') ISSUE_DATE
                                            ,M.SUBJECT
                                            ,M.RAISED_FROM
                                            ,M.ISSUE_BY                                            
                                            ,M.STATUS
                                            ,M.COMPANY_ID
                                            ,M.REMARKS
                                            ,M.ENTERED_BY
                                            ,M.ENTERED_DATE
                                            ,M.ENTERED_TERMINAL                
            FROM MISCELLANEOUS_ISSUE_MST  M WHERE M.COMPANY_ID=:param1 AND trunc(M.ENTERED_DATE ) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param2, 'DD/MM/YYYY')
            ";

        string LoadData_DetailByMasterId_Query() => @"
            SELECT ROW_NUMBER () OVER (ORDER BY B.DTL_ID ASC) AS ROW_NO,
                   B.*, 
    
                   C.SKU_NAME,
                   C.PACK_SIZE,        

                   NVL((SELECT STOCK_QTY FROM VW_SKU_WISE_STOCK WHERE COMPANY_ID=A.COMPANY_ID AND UNIT_ID=A.UNIT_ID AND SKU_ID=B.SKU_ID),0) STOCK_QTY,
                   B.ISSUE_QTY ISSUE_QTY, 
                   A.ISSUE_NO, A.SUBJECT, A.RAISED_FROM
            FROM MISCELLANEOUS_ISSUE_MST A, MISCELLANEOUS_ISSUE_DTL B,PRODUCT_INFO C
            WHERE A.MST_ID        = B.MST_ID
            AND   B.SKU_ID        = C.SKU_ID
            AND   A.MST_ID        = :param1";
        string DeleteMisc_Dtl_IdQuery() => "DELETE  FROM MISCELLANEOUS_ISSUE_DTL WHere DTL_ID = :param1";
        
        //-----------------TASK---------------//
        public async Task<string> LoadProductData(string db, int Company_Id,int Unit_id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadProductData_Query(), _commonServices.AddParameter(new string[] { Unit_id.ToString(),Company_Id.ToString()})));
        //public async Task<string> LoadSKUBatchData(string db, int Company_Id, int Unit_Id, int Sku_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSkuBatch_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Unit_Id.ToString(), Sku_Id.ToString() })));

        public async Task<string> AddOrUpdate(string db, Miscellaneous_Issue_Mst model)
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
                    int dtlId = 0;
                    if (model.MST_ID == 0)
                    {

                        model.MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetMiscIssue_Mst_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        model.ISSUE_NO = _commonServices.GenerateRequisitionCode(_configuration.GetConnectionString(db), "FN_GENERATE_MISC_ISSUE_NO", model.COMPANY_ID.ToString(), model.UNIT_ID.ToString());

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                         _commonServices.AddParameter(new string[] {model.MST_ID.ToString(),
                             model.UNIT_ID.ToString(),model.ISSUE_NO,
                             model.ISSUE_DATE, model.SUBJECT,model.RAISED_FROM,model.ISSUE_BY, model.COMPANY_ID.ToString(), model.STATUS, model.REMARKS,
                             model.ENTERED_BY, model.ENTERED_DATE,  model.ENTERED_TERMINAL
                            })));

                        if (model.MiscellaneousIssueDtlList != null && model.MiscellaneousIssueDtlList.Count > 0)
                        {
                            dtlId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetMiscIssue_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { }));
                            foreach (var item in model.MiscellaneousIssueDtlList)
                            {
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(),
                               _commonServices.AddParameter(new string[] { model.MST_ID.ToString(),dtlId.ToString(),
                                     item.UNIT_ID.ToString(), item.ISSUE_DATE,
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(),
                                    item.ISSUE_QTY.ToString(), item.ISSUE_AMOUNT.ToString(), item.STATUS,
                                     item.COMPANY_ID.ToString(), item.REMARKS,  item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL
                                  })));
                                dtlId++;
                            }
                        }

                    }
                    else
                    {
                        Miscellaneous_Issue_Mst miscellaneous_Issue_Mst = await LoadDetailData_ByMasterId_List(db, model.MST_ID);
                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.MST_ID.ToString(),
                             model.UNIT_ID.ToString(), model.ISSUE_NO,model.ISSUE_DATE,
                             model.SUBJECT, model.RAISED_FROM,model.ISSUE_BY,model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                             model.UPDATED_BY.ToString(), model.UPDATED_DATE.ToString(),
                             model.UPDATED_TERMINAL.ToString()})));
                        foreach (var item in model.MiscellaneousIssueDtlList)
                        {
                            if (item.DTL_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------

                                dtlId = dtlId == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetMiscIssue_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (dtlId + 1);

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(), _commonServices.AddParameter(new string[] {model.MST_ID.ToString(),dtlId.ToString(),
                                     item.UNIT_ID.ToString(), item.ISSUE_DATE,
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(),
                                    item.ISSUE_QTY.ToString(), item.ISSUE_AMOUNT.ToString(), item.STATUS,
                                     item.COMPANY_ID.ToString(), item.REMARKS,  item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL})));

                            }
                            else
                            {


                                //-------------Edit on detail table--------------------
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQueryDTL(), _commonServices.AddParameter(new string[] { item.DTL_ID.ToString(),

                                     item.ISSUE_QTY.ToString(), item.ISSUE_AMOUNT.ToString(), item.STATUS,
                                     item.REMARKS,item.UPDATED_BY,item.UPDATED_DATE,item.UPDATED_TERMINAL })));

                            }

                        }

                        foreach (var item in miscellaneous_Issue_Mst.MiscellaneousIssueDtlList)
                        {
                            bool status = true;

                            foreach (var updateditem in model.MiscellaneousIssueDtlList)
                            {
                                if (item.DTL_ID == updateditem.DTL_ID)
                                {
                                    status = false;
                                }
                            }
                            if (status)
                            {
                                //-------------Delete row from detail table--------------------

                                listOfQuery.Add(_commonServices.AddQuery(DeleteMisc_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { item.DTL_ID.ToString() })));

                            }

                        }


                    }
                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return model.MST_ID.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        //public async Task<string> GenerateIssueNo(string db, string Company_Id)
        // {
        //     try
        //     {
        //         string code;
        //         DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastArea_Ino(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
        //         if (dataTable.Rows.Count > 0)
        //         {
        //             string serial = (Convert.ToInt32(dataTable.Rows[0]["AREA_CODE"].ToString().Substring(1, CodeConstants.AreaInfo_CodeLength - 1)) + 1).ToString();
        //             int serial_length = serial.Length;
        //             code = CodeConstants.AreaInfo_CodeConst;
        //             for (int i = 0; i < (CodeConstants.AreaInfo_CodeLength - (serial_length + 1)); i++)
        //             {
        //                 code += "0";
        //             }
        //             code += serial;
        //         }
        //         else
        //         {
        //             code = CodeConstants.AreaInfo_CodeConst + "0001";
        //         }
        //         return code;
        //     }
        //     catch (Exception ex)
        //     {
        //         throw ex;
        //     }
        // }

        public async Task<string> LoadData(string db, int Company_Id, string date_from, string date_to)
        {
            List<Miscellaneous_Issue_Mst> miscellaneous_Issue_Mst = new List<Miscellaneous_Issue_Mst>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), date_from, date_to }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                Miscellaneous_Issue_Mst issue_Mst = new Miscellaneous_Issue_Mst();
                issue_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                issue_Mst.MST_ID = Convert.ToInt32(data.Rows[i]["MST_ID"]);
                issue_Mst.UNIT_ID = Convert.ToInt32(data.Rows[i]["UNIT_ID"]);
                issue_Mst.ISSUE_NO = data.Rows[i]["ISSUE_NO"].ToString();
                issue_Mst.ISSUE_DATE = data.Rows[i]["ISSUE_DATE"].ToString();
                issue_Mst.SUBJECT = data.Rows[i]["SUBJECT"].ToString();
                issue_Mst.RAISED_FROM = data.Rows[i]["RAISED_FROM"].ToString();
                issue_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
                issue_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                issue_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                
                issue_Mst.STATUS = data.Rows[i]["STATUS"].ToString();



                miscellaneous_Issue_Mst.Add(issue_Mst);
            }
            return JsonSerializer.Serialize(miscellaneous_Issue_Mst);
        }
        public Task<string> LoadData_Master(string db, int Company_Id)
        {
            throw new NotImplementedException();
        }

        public async Task<Miscellaneous_Issue_Mst> LoadDetailData_ByMasterId_List(string db, int _Id)
        {
            Miscellaneous_Issue_Mst _issue_Mst = new Miscellaneous_Issue_Mst();

            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
            if (data.Rows.Count > 0)
            {
                _issue_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);

                _issue_Mst.ROW_NO = Convert.ToInt32(data.Rows[0]["ROW_NO"]);
                _issue_Mst.MST_ID = Convert.ToInt32(data.Rows[0]["MST_ID"]);
                _issue_Mst.UNIT_ID = Convert.ToInt32(data.Rows[0]["UNIT_ID"]); 
                _issue_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[0]["MST_ID"].ToString());
                _issue_Mst.ISSUE_NO = data.Rows[0]["ISSUE_NO"].ToString();
                _issue_Mst.ISSUE_DATE = data.Rows[0]["ISSUE_DATE"].ToString();
                _issue_Mst.SUBJECT = data.Rows[0]["SUBJECT"].ToString();
                _issue_Mst.RAISED_FROM = data.Rows[0]["RAISED_FROM"].ToString();
                _issue_Mst.REMARKS = data.Rows[0]["REMARKS"].ToString();
                _issue_Mst.ISSUE_BY = data.Rows[0]["ISSUE_BY"].ToString();                
                _issue_Mst.STATUS = data.Rows[0]["STATUS"].ToString();                  

                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
                _issue_Mst.MiscellaneousIssueDtlList = new List<Miscellaneous_Issue_Dtl>();
                for (int i = 0; i < dataTable_detail.Rows.Count; i++)
                {
                    Miscellaneous_Issue_Dtl _issue_Dtl = new Miscellaneous_Issue_Dtl();

                    _issue_Dtl.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                    _issue_Dtl.MST_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MST_ID"]);
                    _issue_Dtl.DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DTL_ID"]);
                    _issue_Dtl.UNIT_ID = Convert.ToInt32(dataTable_detail.Rows[i]["UNIT_ID"]);
                    _issue_Dtl.ISSUE_DATE = data.Rows[0]["ISSUE_DATE"].ToString();
                    _issue_Dtl.SKU_ID = Convert.ToInt32(dataTable_detail.Rows[i]["SKU_ID"]);
                    _issue_Dtl.SKU_CODE = dataTable_detail.Rows[i]["SKU_CODE"].ToString();
                    _issue_Dtl.SKU_NAME = dataTable_detail.Rows[i]["SKU_NAME"].ToString();
                    _issue_Dtl.PACK_SIZE = dataTable_detail.Rows[i]["PACK_SIZE"].ToString();
                    _issue_Dtl.UNIT_TP = Convert.ToInt32(dataTable_detail.Rows[i]["UNIT_TP"]);
    

                    _issue_Dtl.ISSUE_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["ISSUE_QTY"]);
                    _issue_Dtl.STOCK_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["STOCK_QTY"]);
                    _issue_Dtl.ISSUE_AMOUNT = Convert.ToInt32(dataTable_detail.Rows[i]["ISSUE_AMOUNT"]);
                    _issue_Dtl.STATUS = dataTable_detail.Rows[i]["STATUS"].ToString();
                    _issue_Dtl.REMARKS = dataTable_detail.Rows[i]["REMARKS"].ToString();
                    _issue_Dtl.ENTERED_BY = dataTable_detail.Rows[i]["ENTERED_BY"].ToString();
                    _issue_Dtl.ENTERED_DATE = dataTable_detail.Rows[i]["ENTERED_DATE"].ToString();


                    _issue_Dtl.ROW_NO = Convert.ToInt32(dataTable_detail.Rows[i]["ROW_NO"]);
                    _issue_Mst.MiscellaneousIssueDtlList.Add(_issue_Dtl);
                }
            }

            return _issue_Mst;
        }

        //Task<Miscellaneous_Issue_Mst> LoadDetailData_ByMasterId_List(string db, int _Id)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
