using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Inventory.IManager;
using SalesAndDistributionSystem.Services.Business.Inventory.Manager.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Inventory.Manager
{
    public class DistributionManager : IDistributionManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public DistributionManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        string AddOrUpdate_AddQuery() => @"INSERT INTO DEPOT_REQ_DISTRIBUTION_MST 
                                        (
                                            MST_ID
                                            ,DISTRIBUTION_NO
                                            ,DISTRIBUTION_DATE
                                            ,VECHILE_NO
                                            ,VECHILE_DESCRIPTION
                                            ,VECHILE_TOTAL_VOLUME
                                            ,VECHILE_TOTAL_WEIGHT
                                            ,DRIVER_ID
                                            ,DISTRIBUTION_BY
                                            ,STATUS
                                            ,COMPANY_ID
                                            ,REMARKS
                                            ,ENTERED_BY
                                            ,ENTERED_DATE
                                            ,ENTERED_TERMINAL
                                         ) 
                                          VALUES ( :param1, :param2,TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS'), :param4,:param5 ,:param6, :param7, :param8, :param9, :param10, :param11,:param12,:param13,TO_DATE(:param14, 'DD/MM/YYYY HH:MI:SS'), :param15)";


        string AddOrUpdate_UpdateQuery() => @"UPDATE DEPOT_REQ_DISTRIBUTION_MST SET  
                                              
                                                DISTRIBUTION_NO = :param2
                                                ,DISTRIBUTION_DATE = TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS')
                                                ,VECHILE_NO = :param4
                                                ,VECHILE_DESCRIPTION = :param5
                                                ,VECHILE_TOTAL_VOLUME = :param6
                                                ,VECHILE_TOTAL_WEIGHT = :param7
                                                ,DRIVER_ID = :param8
                                                ,DISTRIBUTION_BY = :param9
                                                ,STATUS= :param10
                                                ,COMPANY_ID= :param11
                                                ,REMARKS= :param12
                                                ,UPDATED_BY= :param13
                                                ,UPDATED_DATE= TO_DATE(:param14, 'DD/MM/YYYY HH:MI:SS')
                                                ,UPDATED_TERMINAL= :param15
                                                WHERE MST_ID = :param1";

        string AddOrUpdate_AddQueryIssue() => @"INSERT INTO DEPOT_REQ_DISTRIBUTION_REQ 
                    (DEPOT_REQ_ID
                        ,MST_ID
                        ,REQUISITION_NO
                        ,REQUISITION_DATE
                        ,REQUISITION_UNIT_ID
                        ,ISSUE_NO
                        ,ISSUE_DATE
                        ,ISSUE_UNIT_ID
                        ,STATUS
                        ,COMPANY_ID
                        ,ENTERED_BY
                        ,ENTERED_DATE
                        ,ENTERED_TERMINAL

                   ) 
                    VALUES ( :param1, :param2, :param3,TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS'), :param5, :param6,TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS'), :param8, :param9,:param10,:param11,TO_DATE(:param12, 'DD/MM/YYYY HH:MI:SS'), :param13)";

        string AddOrUpdate_UpdateQueryIssue() => @"UPDATE DEPOT_REQ_DISTRIBUTION_REQ SET 
                                                    REQUISITION_NO= :param2
                                                    ,REQUISITION_DATE=  TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS')
                                                    ,REQUISITION_UNIT_ID= :param4
                                                    ,ISSUE_NO= :param5
                                                    ,ISSUE_DATE=   TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS')
                                                    ,ISSUE_UNIT_ID= :param7
                                                    ,STATUS= :param8
                                                    ,COMPANY_ID= :param9
                                                    ,UPDATED_BY= :param10
                                                    ,UPDATED_DATE=  TO_DATE(:param11, 'DD/MM/YYYY HH:MI:SS')
                                                    ,UPDATED_TERMINAL= :param12
                                                    WHERE DEPOT_REQ_ID = :param1";

        string AddOrUpdate_AddQueryProduct() => @"INSERT INTO DEPOT_REQ_DISTRIBUTION_PRODUCT 
                    (   DEPOT_REQ_PRODUCT_ID
                        ,DEPOT_REQ_ID
                        ,MST_ID
                        ,ISSUE_NO
                        ,ISSUE_DATE
                        ,ISSUE_UNIT_ID
                        ,DISTRIBUTION_DATE
                        ,SKU_ID
                        ,SKU_CODE
                        ,UNIT_TP
                        ,BATCH_ID
                        ,BATCH_NO
                        ,ISSUE_QTY
                        ,ISSUE_AMOUNT
                        ,DISTRIBUTION_QTY
                        ,DISTRIBUTION_AMOUNT
                        ,STATUS
                        ,COMPANY_ID
                        ,ENTERED_BY
                        ,ENTERED_DATE
                        ,ENTERED_TERMINAL

                   ) 
                    VALUES ( :param1, :param2, :param3,:param4,TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS'), :param6,TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS'), :param8, :param9,:param10,:param11,:param12,:param13,:param14,:param15,:param16,:param17,:param18,:param19,TO_DATE(:param20, 'DD/MM/YYYY HH:MI:SS'), :param21)";

        string AddOrUpdate_UpdateQueryProduct() => @"UPDATE DEPOT_REQ_DISTRIBUTION_PRODUCT SET 

                                                    ISSUE_NO= :param2
                                                    ,ISSUE_DATE =  TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS')
                                                    ,ISSUE_UNIT_ID= :param4
                                                    ,DISTRIBUTION_DATE =  TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS')
                                                    ,SKU_ID= :param6
                                                    ,SKU_CODE= :param7
                                                    ,UNIT_TP= :param8
                                                    ,BATCH_ID= :param9
                                                    ,BATCH_NO= :param10
                                                    ,ISSUE_QTY= :param11
                                                    ,ISSUE_AMOUNT= :param12
                                                    ,DISTRIBUTION_QTY= :param13
                                                    ,DISTRIBUTION_AMOUNT= :param14
                                                    ,STATUS= :param15
                                                    ,COMPANY_ID= :param16
                                                    ,UPDATED_BY= :param17
                                                    ,UPDATED_DATE=  TO_DATE(:param18, 'DD/MM/YYYY HH:MI:SS')
                                                    ,UPDATED_TERMINAL= :param19
                                                    WHERE DEPOT_REQ_PRODUCT_ID = :param1";

        string LoadBatchData_MasterById_Query() => @"
            SELECT ROW_NUMBER() OVER(ORDER BY MST_ID ASC) AS ROW_NO,ISSUE_BATCH_ID
                                            ,DTL_ID
                                            ,MST_ID
                                            ,REQUISITION_NO
                                            ,ISSUE_NO
                                            ,ISSUE_DATE
                                            ,COMPANY_ID
                                            ,REQUISITION_UNIT_ID
                                            ,ISSUE_UNIT_ID
                                            ,SKU_ID
                                            ,FN_SKU_NAME(COMPANY_ID,SKU_ID) SKU_NAME
                                            ,SKU_CODE
                                            ,UNIT_TP
                                            ,BATCH_ID
                                            ,BATCH_NO
                                            ,ISSUE_QTY
                                            ,ISSUE_AMOUNT                
            FROM DEPOT_REQUISITION_ISSUE_BATCH  
            where MST_ID = :param1";

        string LoadMasterData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO,
                                               M.MST_ID
                                                ,M.DISTRIBUTION_NO
                                                ,TO_CHAR(M.DISTRIBUTION_DATE, 'DD/MM/YYYY') DISTRIBUTION_DATE
                                                ,M.VECHILE_NO
                                                ,M.VECHILE_DESCRIPTION
                                                ,M.VECHILE_TOTAL_VOLUME
                                                ,M.VECHILE_TOTAL_WEIGHT
                                                ,M.DRIVER_ID
                                                ,M.DISTRIBUTION_BY
                                                ,M.STATUS
                                                ,M.COMPANY_ID
                                                ,M.REMARKS
                                                ,M.ENTERED_BY
                                                ,M.ENTERED_DATE
                                                ,M.ENTERED_TERMINAL
                                          
                                   
            FROM DEPOT_REQ_DISTRIBUTION_MST  M 
            where  M.COMPANY_ID = :param1  ORDER BY M.MST_ID DESC";

        string LoadData_MasterById_Query() => @"
            SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO,M.MST_ID
                                                 ,M.MST_ID
                                                ,M.DISTRIBUTION_NO
                                                ,TO_CHAR(M.DISTRIBUTION_DATE, 'DD/MM/YYYY') DISTRIBUTION_DATE
                                                ,M.VECHILE_NO
                                                ,M.VECHILE_DESCRIPTION
                                                ,M.VECHILE_TOTAL_VOLUME
                                                ,M.VECHILE_TOTAL_WEIGHT
                                                ,M.DRIVER_ID
                                                ,M.DISTRIBUTION_BY
                                                ,M.STATUS
                                                ,M.COMPANY_ID
                                                ,M.REMARKS
                                                ,M.ENTERED_BY
                                                ,M.ENTERED_DATE
                                                ,M.ENTERED_TERMINAL            
            FROM DEPOT_REQ_DISTRIBUTION_MST  M 
            where M.MST_ID = :param1";

        string LoadData_DistributionReq_Query() => @" SELECT ROW_NUMBER () OVER (ORDER BY A.DEPOT_REQ_ID ASC) AS ROW_NO,
                                                       A.* 
                                                      ,TO_CHAR(A.REQUISITION_DATE, 'DD/MM/YYYY') REQUISITION_DATE_FORMATED
                                                        ,TO_CHAR(A.ISSUE_DATE, 'DD/MM/YYYY') ISSUE_DATE_FORMATED
                                                        ,FN_UNIT_NAME(A.COMPANY_ID,A.ISSUE_UNIT_ID) ISSUE_UNIT_NAME
                                                        ,FN_UNIT_NAME(A.COMPANY_ID,A.REQUISITION_UNIT_ID) REQUISITION_UNIT_NAME

                                                FROM DEPOT_REQ_DISTRIBUTION_REQ A
                                                WHERE A.MST_ID = :param1";

        string LoadData_All_DistributionReq_Query() => @" SELECT ROW_NUMBER () OVER (ORDER BY A.DEPOT_REQ_ID ASC) AS ROW_NO,
                                                       A.* 
                                                      ,TO_CHAR(A.REQUISITION_DATE, 'DD/MM/YYYY') REQUISITION_DATE_FORMATED
                                                        ,TO_CHAR(A.ISSUE_DATE, 'DD/MM/YYYY') ISSUE_DATE_FORMATED
                                                        ,FN_UNIT_NAME(A.COMPANY_ID,A.ISSUE_UNIT_ID) ISSUE_UNIT_NAME
                                                        ,FN_UNIT_NAME(A.COMPANY_ID,A.REQUISITION_UNIT_ID) REQUISITION_UNIT_NAME
                                                FROM DEPOT_REQ_DISTRIBUTION_REQ A where A.REQUISITION_NO not in(select REQUISITION_NO from DEPOT_REQUISITION_RCV_MST)
                                                ";


        string LoadData_All_DistributionProduct_Query() => @" SELECT ROW_NUMBER () OVER (ORDER BY A.DEPOT_REQ_PRODUCT_ID ASC) AS ROW_NO,
                                                       A.* 
                                                        ,TO_CHAR(A.DISTRIBUTION_DATE, 'DD/MM/YYYY') DISTRIBUTION_DATE_FORMATED
                                                        ,TO_CHAR(A.ISSUE_DATE, 'DD/MM/YYYY') ISSUE_DATE_FORMATED
                                                       ,FN_SKU_NAME(A.COMPANY_ID,A.SKU_ID) SKU_NAME
                                                      
                                                FROM DEPOT_REQ_DISTRIBUTION_PRODUCT A
                                                WHERE A.DEPOT_REQ_ID = :param1";

        string LoadData_DistributionProduct_Query() => @" SELECT ROW_NUMBER () OVER (ORDER BY A.DEPOT_REQ_PRODUCT_ID ASC) AS ROW_NO,
                                                       A.* 
                                                        ,TO_CHAR(A.DISTRIBUTION_DATE, 'DD/MM/YYYY') DISTRIBUTION_DATE_FORMATED
                                                        ,TO_CHAR(A.ISSUE_DATE, 'DD/MM/YYYY') ISSUE_DATE_FORMATED
                                                       ,FN_SKU_NAME(A.COMPANY_ID,A.SKU_ID) SKU_NAME
                                                      
                                                FROM DEPOT_REQ_DISTRIBUTION_PRODUCT A
                                                WHERE A.DEPOT_REQ_ID = :param1";
        string Delete_Request_Dtl_IdQuery() => "DELETE  FROM DEPOT_REQ_DISTRIBUTION_REQ WHere DEPOT_REQ_ID = :param1";
        string Delete_Product_Dtl_IdQuery() => "DELETE  FROM DEPOT_REQ_DISTRIBUTION_PRODUCT WHere DEPOT_REQ_PRODUCT_ID = :param1";
        string DeleteArea_Requisition_Dtl_IdQuery() => "DELETE  FROM DEPOT_REQ_DISTRIBUTION_REQ WHere DEPOT_REQ_ID = :param1";
        string GetDistribution_Mst_IdQuery() => "SELECT NVL(MAX(MST_ID),0) + 1 MST_ID  FROM DEPOT_REQ_DISTRIBUTION_MST";
        string Get_LastDistribution_no() => "SELECT  DISTRIBUTION_NO  FROM DEPOT_REQ_DISTRIBUTION_MST Where  MST_ID = (SELECT   NVL(MAX(MST_ID),0) MST_ID From DEPOT_REQ_DISTRIBUTION_MST where COMPANY_ID = :param1 )";
        string GetRequisition_Issue_IdQuery() => "SELECT NVL(MAX(DEPOT_REQ_ID),0) + 1 DEPOT_REQ_ID  FROM DEPOT_REQ_DISTRIBUTION_REQ";
        string GetRequisition_Product_IdQuery() => "SELECT NVL(MAX(DEPOT_REQ_PRODUCT_ID),0) + 1 DEPOT_REQ_PRODUCT_ID  FROM DEPOT_REQ_DISTRIBUTION_PRODUCT";
        public async Task<string> AddOrUpdate(string db, DEPOT_REQ_DISTRIBUTION_MST model)
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

                    int DEPOT_REQ_ID = 0;
                    int DEPOT_REQ_PRODUCT_ID = 0;
                    if (model.MST_ID == 0)
                     {

                        model.MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetDistribution_Mst_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        model.DISTRIBUTION_NO = await GenerateDistributionCode(db, model.COMPANY_ID.ToString());
                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                         _commonServices.AddParameter(new string[] {model.MST_ID.ToString(),
                             model.DISTRIBUTION_NO.ToString(), model.DISTRIBUTION_DATE.ToString(),model.VEHICLE_NO,model.VEHICLE_DESCRIPTION,model.VEHICLE_TOTAL_VOLUME.ToString(),
                             model.VEHICLE_TOTAL_WEIGHT.ToString(), model.DRIVER_ID.ToString(),model.DISTRIBUTION_BY,model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                             model.ENTERED_BY, model.ENTERED_DATE,  model.ENTERED_TERMINAL
                            })));
                        #region Distribution Request Add
                        if (model.requisitionIssueDtlList != null && model.requisitionIssueDtlList.Count > 0)
                        {
                            DEPOT_REQ_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Issue_IdQuery(), _commonServices.AddParameter(new string[] { }));
                            DEPOT_REQ_PRODUCT_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Product_IdQuery(), _commonServices.AddParameter(new string[] { }));
                            foreach (var item in model.requisitionIssueDtlList)
                            {
                                //item.REQUISITION_DATE = Convert.ToDateTime(item.REQUISITION_DATE).ToString("dd/MM/yyyy hh:mm:ss");
                                item.ISSUE_DATE = Convert.ToDateTime(item.ISSUE_DATE).ToString("dd/MM/yyyy hh:mm:ss");
                        

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryIssue(),
                               _commonServices.AddParameter(new string[] { DEPOT_REQ_ID.ToString(),model.MST_ID.ToString(),
                                     item.REQUISITION_NO,item.REQUISITION_DATE,
                                     item.REQUISITION_UNIT_ID.ToString(), item.ISSUE_NO.ToString(), item.ISSUE_DATE.ToString(),
                                     item.ISSUE_UNIT_ID.ToString(),item.STATUS,
                                     item.COMPANY_ID.ToString(),  item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL
                                  })));

                                #region Distribution Product Add
                                if (item.requisitionProductDtlList != null && item.requisitionProductDtlList.Count > 0)
                                {

                                    
                                    foreach (var product in item.requisitionProductDtlList)
                                    {
                                      
                                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryProduct(),
                                            _commonServices.AddParameter(new string[] { DEPOT_REQ_PRODUCT_ID.ToString(),DEPOT_REQ_ID.ToString(),model.MST_ID.ToString(),
                                            product.ISSUE_NO,item.ISSUE_DATE,
                                            product.ISSUE_UNIT_ID.ToString(), product.DISTRIBUTION_DATE.ToString(), product.SKU_ID.ToString(),
                                            product.SKU_CODE.ToString(),product.UNIT_TP.ToString(),product.BATCH_ID.ToString(),product.BATCH_NO.ToString(),
                                            product.ISSUE_QTY.ToString(),product.ISSUE_AMOUNT.ToString(),product.DISTRIBUTION_QTY.ToString(),product.DISTRIBUTION_AMOUNT.ToString(),product.STATUS,
                                            product.COMPANY_ID.ToString(),  product.ENTERED_BY.ToString(),  product.ENTERED_DATE,  product.ENTERED_TERMINAL
                                            })));
                                       

                                        DEPOT_REQ_PRODUCT_ID++;
                                    }
                                }
                                #endregion
                                DEPOT_REQ_ID++;
                            }
                        }
                        #endregion




                    }
                    else
                    {
                        //DEPOT_REQUISITION_ISSUE_MST depot_Requisition_issue_Mst = await LoadDetailData_ByMasterId_List(db, model.MST_ID);
                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.MST_ID.ToString(),
                             model.DISTRIBUTION_NO.ToString(), model.DISTRIBUTION_DATE.ToString(),model.VEHICLE_NO,model.VEHICLE_DESCRIPTION,model.VEHICLE_TOTAL_VOLUME.ToString(),
                             model.VEHICLE_TOTAL_WEIGHT.ToString(), model.DRIVER_ID.ToString(),model.DISTRIBUTION_BY,model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                             model.UPDATED_BY.ToString(), model.UPDATED_DATE.ToString(),
                             model.UPDATED_TERMINAL.ToString()})));
                        foreach (var item in model.requisitionIssueDtlList)
                        {
                           // item.REQUISITION_DATE = Convert.ToDateTime(item.REQUISITION_DATE).ToString("dd/MM/yyyy hh:mm:ss");
                            //item.ISSUE_DATE = Convert.ToDateTime(item.ISSUE_DATE).ToString("dd/MM/yyyy hh:mm:ss");
                      
                       
                            if (item.DEPOT_REQ_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------

                                DEPOT_REQ_ID = DEPOT_REQ_ID == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Issue_IdQuery(), _commonServices.AddParameter(new string[] { })) : (DEPOT_REQ_ID + 1);
                                DEPOT_REQ_PRODUCT_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Product_IdQuery(), _commonServices.AddParameter(new string[] { }));
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryIssue(),
                                  _commonServices.AddParameter(new string[] { DEPOT_REQ_ID.ToString(),model.MST_ID.ToString(),
                                     item.REQUISITION_NO,item.REQUISITION_DATE,
                                     item.REQUISITION_UNIT_ID.ToString(), item.ISSUE_NO.ToString(), item.ISSUE_DATE.ToString(),
                                     item.ISSUE_UNIT_ID.ToString(),item.STATUS,
                                     item.COMPANY_ID.ToString(),  item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL
                                })));

                                #region Distribution Product Add
                                if (item.requisitionProductDtlList != null && item.requisitionProductDtlList.Count > 0)
                                {


                                    foreach (var product in item.requisitionProductDtlList)
                                    {

                                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryProduct(),
                                        _commonServices.AddParameter(new string[] { DEPOT_REQ_PRODUCT_ID.ToString(),DEPOT_REQ_ID.ToString(),model.MST_ID.ToString(),
                                            product.ISSUE_NO,item.ISSUE_DATE,
                                            product.ISSUE_UNIT_ID.ToString(), product.DISTRIBUTION_DATE.ToString(), product.SKU_ID.ToString(),
                                            product.SKU_CODE.ToString(),product.UNIT_TP.ToString(),product.BATCH_ID.ToString(),product.BATCH_NO.ToString(),
                                            product.ISSUE_QTY.ToString(),product.ISSUE_AMOUNT.ToString(),product.DISTRIBUTION_QTY.ToString(),product.DISTRIBUTION_AMOUNT.ToString(),product.STATUS,
                                            product.COMPANY_ID.ToString(),  product.ENTERED_BY.ToString(),  product.ENTERED_DATE,  product.ENTERED_TERMINAL
                                        })));


                                        DEPOT_REQ_PRODUCT_ID++;
                                    }
                                }
                                #endregion
                                DEPOT_REQ_ID++;
                            }
                            else
                            {
                                DEPOT_REQ_DISTRIBUTION_MST depot_Distribution_issue_Mst = await LoadDistributionDetailData_ByMasterId(db, model.MST_ID);

                                //-------------Edit on detail table--------------------
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQueryIssue(), _commonServices.AddParameter(new string[] { item.DEPOT_REQ_ID.ToString(),
                                     item.REQUISITION_NO,item.REQUISITION_DATE,
                                     item.REQUISITION_UNIT_ID.ToString(), item.ISSUE_NO.ToString(), item.ISSUE_DATE.ToString(),
                                     item.ISSUE_UNIT_ID.ToString(),item.STATUS,
                                     item.COMPANY_ID.ToString(),item.UPDATED_BY,item.UPDATED_DATE,item.UPDATED_TERMINAL })));

                                #region Distribution Product Update
                                if (item.requisitionProductDtlList != null && item.requisitionProductDtlList.Count > 0)
                                {


                                    foreach (var product in item.requisitionProductDtlList)
                                    {
                                        //product.DISTRIBUTION_DATE = Convert.ToDateTime(product.DISTRIBUTION_DATE).ToString("dd/MM/yyyy hh:mm:ss");

                                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQueryProduct(),
                                        _commonServices.AddParameter(new string[] { product.DEPOT_REQ_PRODUCT_ID.ToString(),
                                            product.ISSUE_NO,item.ISSUE_DATE,
                                            product.ISSUE_UNIT_ID.ToString(), product.DISTRIBUTION_DATE.ToString(), product.SKU_ID.ToString(),
                                            product.SKU_CODE.ToString(),product.UNIT_TP.ToString(),product.BATCH_ID.ToString(),product.BATCH_NO.ToString(),
                                            product.ISSUE_QTY.ToString(),product.ISSUE_AMOUNT.ToString(),product.DISTRIBUTION_QTY.ToString(),product.DISTRIBUTION_AMOUNT.ToString(),product.STATUS,
                                            product.COMPANY_ID.ToString(),  product.UPDATED_BY.ToString(),  product.UPDATED_DATE,  product.UPDATED_TERMINAL
                                        })));

                                    }
                                }
                                #endregion

                                #region delete Requisition
                                foreach (var distribution in depot_Distribution_issue_Mst.requisitionIssueDtlList)
                                {
                                    bool status = true;

                                    foreach (var updateditem in model.requisitionIssueDtlList)
                                    {
                                        if (distribution.DEPOT_REQ_ID == updateditem.DEPOT_REQ_ID)
                                        {
                                            #region Delete Product
                                            foreach (var productList in distribution.requisitionProductDtlList)
                                            {
                                                bool productStatus = true;
                                                foreach(var updatedProduct in updateditem.requisitionProductDtlList)
                                                {
                                                    if(productList.DEPOT_REQ_PRODUCT_ID == updatedProduct.DEPOT_REQ_PRODUCT_ID)
                                                    {
                                                        productStatus = false;
                                                        break;
                                                    }
                                                }
                                                if (productStatus)
                                                {
                                                    listOfQuery.Add(_commonServices.AddQuery(Delete_Product_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { productList.DEPOT_REQ_PRODUCT_ID.ToString() })));
                                                }
                                            }
                                            #endregion

                                            status = false;
                                          
                                        }
                                    }
                                    if (status)
                                    {
                                        //-------------Delete row from detail table--------------------
                                        foreach(var product in distribution.requisitionProductDtlList)
                                        {
                                            listOfQuery.Add(_commonServices.AddQuery(Delete_Product_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { product.DEPOT_REQ_PRODUCT_ID.ToString() })));
                                        }
                                        listOfQuery.Add(_commonServices.AddQuery(Delete_Request_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { distribution.DEPOT_REQ_ID.ToString() })));

                                    }
                                    
                                }
                                #endregion
                              

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
           // throw new NotImplementedException();
        }

        public async Task<string> GetPendingRequisition(string db, int companyId, int unitId)
        {
            var query = String.Format(@"begin :param1 := fn_issue_pending_for_dist({0},'{1}'); 
                          end;", companyId, unitId);

            var dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { "RefCursor" }));
            //var dt = await _commonServices.GetDataSetAsyn(_configuration.GetConnectionString(db), PendingInvoice_Query(),
            //    _commonServices.AddParameter(new string[] { companyId.ToString(), unitId.ToString() }));
            var ds = new DataSet();
            ds.Tables.Add(dt);

            return _commonServices.DataSetToJSON(ds);
        }

        public async Task<string> GetProductsByRequisition(string db, int companyId, int unitId, string RequisitionNo)
        {
            var query = String.Format(@"begin :param1 := fn_issue_prod_pending_for_dist({0},'{1}', {2}); 
                          end;", companyId, unitId, RequisitionNo);


            var dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { "RefCursor" }));
            //var dt = await _commonServices.GetDataSetAsyn(_configuration.GetConnectionString(db), GetProductsByInvoice_Query(),
            //    _commonServices.AddParameter(new string[] { companyId.ToString(), invoiceNo }));
            var ds = new DataSet();
            ds.Tables.Add(dt);

            return _commonServices.DataSetToJSON(ds);
        }
        public async Task<string> LoadData(string db, int Company_Id)
        {
            List<DEPOT_REQ_DISTRIBUTION_MST> requisition_Mst_list = new List<DEPOT_REQ_DISTRIBUTION_MST>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadMasterData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                DEPOT_REQ_DISTRIBUTION_MST requisition_Mst = new DEPOT_REQ_DISTRIBUTION_MST();
                requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[i]["MST_ID"]);
                requisition_Mst.DISTRIBUTION_NO = data.Rows[i]["DISTRIBUTION_NO"].ToString();
                requisition_Mst.DISTRIBUTION_DATE = data.Rows[i]["DISTRIBUTION_DATE"].ToString();
                requisition_Mst.VEHICLE_NO = data.Rows[0]["VECHILE_NO"].ToString();
                requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
                requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                requisition_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                requisition_Mst.VEHICLE_TOTAL_VOLUME = Convert.ToInt32(data.Rows[i]["VECHILE_TOTAL_VOLUME"]);
                requisition_Mst.VEHICLE_TOTAL_WEIGHT = Convert.ToInt32(data.Rows[i]["VECHILE_TOTAL_WEIGHT"]);
                requisition_Mst.DRIVER_ID = data.Rows[i]["DRIVER_ID"].ToString();
 

                requisition_Mst.STATUS = data.Rows[i]["STATUS"].ToString();

                requisition_Mst.VEHICLE_DESCRIPTION = data.Rows[i]["VECHILE_DESCRIPTION"].ToString();
                requisition_Mst.DISTRIBUTION_BY = data.Rows[i]["DISTRIBUTION_BY"].ToString();
                requisition_Mst.STATUS = data.Rows[i]["STATUS"].ToString();


                requisition_Mst_list.Add(requisition_Mst);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }



        public async Task<string> LoadDistributionReqData(string db, int Company_Id)
        {
            List<DEPOT_REQ_DISTRIBUTION_REQ> requisition_Mst_list = new List<DEPOT_REQ_DISTRIBUTION_REQ>();
            DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_All_DistributionReq_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            for (int i = 0; i < dataTable_detail.Rows.Count; i++)
            {
                DEPOT_REQ_DISTRIBUTION_REQ _distribution_Req = new DEPOT_REQ_DISTRIBUTION_REQ();

                _distribution_Req.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                _distribution_Req.MST_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MST_ID"]);
                _distribution_Req.DEPOT_REQ_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DEPOT_REQ_ID"]);

                _distribution_Req.REQUISITION_NO = dataTable_detail.Rows[i]["REQUISITION_NO"].ToString();
                _distribution_Req.REQUISITION_DATE = dataTable_detail.Rows[i]["REQUISITION_DATE_FORMATED"].ToString();
                _distribution_Req.REQUISITION_UNIT_ID = Convert.ToInt32(dataTable_detail.Rows[i]["REQUISITION_UNIT_ID"]);
                _distribution_Req.REQUISITION_UNIT_NAME = dataTable_detail.Rows[i]["REQUISITION_UNIT_NAME"].ToString();
                _distribution_Req.ISSUE_NO = dataTable_detail.Rows[i]["ISSUE_NO"].ToString();
                _distribution_Req.ISSUE_DATE = dataTable_detail.Rows[i]["ISSUE_DATE_FORMATED"].ToString();

                _distribution_Req.ISSUE_UNIT_ID = Convert.ToInt32(dataTable_detail.Rows[i]["ISSUE_UNIT_ID"]);
                _distribution_Req.ISSUE_UNIT_NAME = dataTable_detail.Rows[i]["ISSUE_UNIT_NAME"].ToString();
                _distribution_Req.STATUS = dataTable_detail.Rows[i]["STATUS"].ToString();
                _distribution_Req.ENTERED_BY = dataTable_detail.Rows[i]["ENTERED_BY"].ToString();
                _distribution_Req.ENTERED_DATE = dataTable_detail.Rows[i]["ENTERED_DATE"].ToString();
                _distribution_Req.ROW_NO = Convert.ToInt32(dataTable_detail.Rows[i]["ROW_NO"]);


                requisition_Mst_list.Add(_distribution_Req);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }

        public async Task<string> LoadDistributionProductDataByReqId(string db, int Company_Id,int ReqId)
        {
            List<DEPOT_REQ_DISTRIBUTION_PRODUCT> requisition_Mst_list = new List<DEPOT_REQ_DISTRIBUTION_PRODUCT>();
            DataTable dataTable_product_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_All_DistributionProduct_Query(), _commonServices.AddParameter(new string[] { ReqId.ToString() }));
            for (int j = 0; j < dataTable_product_detail.Rows.Count; j++)
            {
                DEPOT_REQ_DISTRIBUTION_PRODUCT _distribution_product = new DEPOT_REQ_DISTRIBUTION_PRODUCT();
                _distribution_product.MST_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["MST_ID"]);
                _distribution_product.DEPOT_REQ_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["DEPOT_REQ_ID"]);
                _distribution_product.DEPOT_REQ_PRODUCT_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["DEPOT_REQ_PRODUCT_ID"]);

                _distribution_product.ISSUE_NO = dataTable_product_detail.Rows[j]["ISSUE_NO"].ToString();
                _distribution_product.ISSUE_DATE = dataTable_product_detail.Rows[j]["ISSUE_DATE_FORMATED"].ToString();
                _distribution_product.DISTRIBUTION_DATE = dataTable_product_detail.Rows[j]["DISTRIBUTION_DATE_FORMATED"].ToString();
                _distribution_product.ISSUE_UNIT_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["ISSUE_UNIT_ID"]);
                _distribution_product.ISSUE_QTY = Convert.ToInt32(dataTable_product_detail.Rows[j]["ISSUE_QTY"]);
                _distribution_product.DISTRIBUTION_QTY = Convert.ToInt32(dataTable_product_detail.Rows[j]["DISTRIBUTION_QTY"]);
                _distribution_product.ISSUE_AMOUNT = Convert.ToDouble(dataTable_product_detail.Rows[j]["ISSUE_AMOUNT"]);
                _distribution_product.DISTRIBUTION_AMOUNT = Convert.ToDouble(dataTable_product_detail.Rows[j]["DISTRIBUTION_AMOUNT"]);


                _distribution_product.DISTRIBUTION_DATE = dataTable_product_detail.Rows[j]["DISTRIBUTION_DATE"].ToString();
                _distribution_product.SKU_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["SKU_ID"]);
                _distribution_product.UNIT_TP = Convert.ToDouble(dataTable_product_detail.Rows[j]["UNIT_TP"]);
                _distribution_product.SKU_CODE = dataTable_product_detail.Rows[j]["SKU_CODE"].ToString();
                _distribution_product.BATCH_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["BATCH_ID"]);
                _distribution_product.BATCH_NO = dataTable_product_detail.Rows[j]["BATCH_NO"].ToString();
                _distribution_product.SKU_NAME = dataTable_product_detail.Rows[j]["SKU_NAME"].ToString();
                _distribution_product.ENTERED_DATE = dataTable_product_detail.Rows[j]["ENTERED_DATE"].ToString();
                _distribution_product.COMPANY_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["COMPANY_ID"]);
                _distribution_product.ROW_NO = Convert.ToInt32(dataTable_product_detail.Rows[j]["ROW_NO"]);


                requisition_Mst_list.Add(_distribution_product);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }

        public async Task<string> GenerateDistributionCode(string db, string Company_Id)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastDistribution_no(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["DISTRIBUTION_NO"].ToString().Substring(4, 4)) + 1).ToString();
                    int serial_length = serial.Length;
                    code = DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM");
                    for (int i = 0; i < (CodeConstants.Requisition_No_CodeLength - (serial_length + 1)); i++)
                    {
                        code += "0";
                    }
                    code += serial;
                }
                else
                {
                    code = DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + "0001";
                }
                return code;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DEPOT_REQ_DISTRIBUTION_MST> LoadDistributionDetailData_ByMasterId(string db, int _Id)
        {
            DEPOT_REQ_DISTRIBUTION_MST _distribution_Mst = new DEPOT_REQ_DISTRIBUTION_MST();

            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
            if (data.Rows.Count > 0)
            {
                _distribution_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);

                _distribution_Mst.ROW_NO = Convert.ToInt32(data.Rows[0]["ROW_NO"]);
                _distribution_Mst.MST_ID = Convert.ToInt32(data.Rows[0]["MST_ID"]);
                _distribution_Mst.DISTRIBUTION_NO = data.Rows[0]["DISTRIBUTION_NO"].ToString();
                _distribution_Mst.DISTRIBUTION_DATE = data.Rows[0]["DISTRIBUTION_DATE"].ToString();
                _distribution_Mst.VEHICLE_NO = data.Rows[0]["VECHILE_NO"].ToString();
                _distribution_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[0]["MST_ID"].ToString());
                _distribution_Mst.VEHICLE_DESCRIPTION = data.Rows[0]["VECHILE_DESCRIPTION"].ToString();
                _distribution_Mst.REMARKS = data.Rows[0]["REMARKS"].ToString();
                _distribution_Mst.VEHICLE_TOTAL_VOLUME = Convert.ToInt32(data.Rows[0]["VECHILE_TOTAL_VOLUME"]);
                _distribution_Mst.VEHICLE_TOTAL_WEIGHT = Convert.ToInt32(data.Rows[0]["VECHILE_TOTAL_WEIGHT"]);
                _distribution_Mst.DRIVER_ID = data.Rows[0]["DRIVER_ID"].ToString();


                _distribution_Mst.STATUS = data.Rows[0]["STATUS"].ToString();

                _distribution_Mst.VEHICLE_DESCRIPTION = data.Rows[0]["VECHILE_DESCRIPTION"].ToString();
                _distribution_Mst.DISTRIBUTION_BY = data.Rows[0]["DISTRIBUTION_BY"].ToString();
                _distribution_Mst.STATUS = data.Rows[0]["STATUS"].ToString();

                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DistributionReq_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
                _distribution_Mst.requisitionIssueDtlList = new List<DEPOT_REQ_DISTRIBUTION_REQ>();
                for (int i = 0; i < dataTable_detail.Rows.Count; i++)
                {
                    DEPOT_REQ_DISTRIBUTION_REQ _distribution_Req = new DEPOT_REQ_DISTRIBUTION_REQ();

                    _distribution_Req.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                    _distribution_Req.MST_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MST_ID"]);
                    _distribution_Req.DEPOT_REQ_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DEPOT_REQ_ID"]);

                    _distribution_Req.REQUISITION_NO = dataTable_detail.Rows[i]["REQUISITION_NO"].ToString();
                    _distribution_Req.REQUISITION_DATE = dataTable_detail.Rows[i]["REQUISITION_DATE_FORMATED"].ToString();
                    _distribution_Req.REQUISITION_UNIT_ID = Convert.ToInt32(dataTable_detail.Rows[i]["REQUISITION_UNIT_ID"]);
                    _distribution_Req.REQUISITION_UNIT_NAME = dataTable_detail.Rows[i]["REQUISITION_UNIT_NAME"].ToString();
                    _distribution_Req.ISSUE_NO = dataTable_detail.Rows[i]["ISSUE_NO"].ToString();
                    _distribution_Req.ISSUE_DATE = dataTable_detail.Rows[i]["ISSUE_DATE_FORMATED"].ToString();
                    _distribution_Req.ISSUE_UNIT_NAME = dataTable_detail.Rows[i]["ISSUE_UNIT_NAME"].ToString();
                
                    _distribution_Req.ISSUE_UNIT_ID = Convert.ToInt32(dataTable_detail.Rows[i]["ISSUE_UNIT_ID"]);
                    _distribution_Req.STATUS = dataTable_detail.Rows[i]["STATUS"].ToString();
                    _distribution_Req.ENTERED_BY = dataTable_detail.Rows[i]["ENTERED_BY"].ToString();
                    _distribution_Req.ENTERED_DATE = dataTable_detail.Rows[i]["ENTERED_DATE"].ToString();
                    _distribution_Req.ROW_NO = Convert.ToInt32(dataTable_detail.Rows[i]["ROW_NO"]);
                    _distribution_Mst.requisitionIssueDtlList.Add(_distribution_Req);

                    DataTable dataTable_product_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DistributionProduct_Query(), _commonServices.AddParameter(new string[] { _distribution_Req.DEPOT_REQ_ID.ToString() }));

                    _distribution_Req.requisitionProductDtlList = new List<DEPOT_REQ_DISTRIBUTION_PRODUCT>();
                    for (int j = 0; j < dataTable_product_detail.Rows.Count; j++)
                    {
                        DEPOT_REQ_DISTRIBUTION_PRODUCT _distribution_product = new DEPOT_REQ_DISTRIBUTION_PRODUCT();
                        _distribution_product.MST_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["MST_ID"]);
                        _distribution_product.DEPOT_REQ_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["DEPOT_REQ_ID"]);
                        _distribution_product.DEPOT_REQ_PRODUCT_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["DEPOT_REQ_PRODUCT_ID"]);

                        _distribution_product.ISSUE_NO = dataTable_product_detail.Rows[j]["ISSUE_NO"].ToString();
                        _distribution_product.ISSUE_DATE = dataTable_product_detail.Rows[j]["ISSUE_DATE_FORMATED"].ToString();
                        _distribution_product.DISTRIBUTION_DATE = dataTable_product_detail.Rows[j]["DISTRIBUTION_DATE_FORMATED"].ToString();
                        _distribution_product.ISSUE_UNIT_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["ISSUE_UNIT_ID"]);
                        _distribution_product.ISSUE_QTY = Convert.ToInt32(dataTable_product_detail.Rows[j]["ISSUE_QTY"]);
                        _distribution_product.DISTRIBUTION_QTY = Convert.ToInt32(dataTable_product_detail.Rows[j]["DISTRIBUTION_QTY"]);
                        _distribution_product.ISSUE_AMOUNT = Convert.ToDouble(dataTable_product_detail.Rows[j]["ISSUE_AMOUNT"]);
                        _distribution_product.DISTRIBUTION_AMOUNT = Convert.ToDouble(dataTable_product_detail.Rows[j]["DISTRIBUTION_AMOUNT"]);


                        _distribution_product.DISTRIBUTION_DATE = dataTable_product_detail.Rows[j]["DISTRIBUTION_DATE"].ToString();
                        _distribution_product.SKU_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["SKU_ID"]);
                        _distribution_product.UNIT_TP = Convert.ToDouble(dataTable_product_detail.Rows[j]["UNIT_TP"]);
                        _distribution_product.SKU_CODE = dataTable_product_detail.Rows[j]["SKU_CODE"].ToString();
                        _distribution_product.BATCH_ID = Convert.ToInt32(dataTable_product_detail.Rows[j]["BATCH_ID"]);
                        _distribution_product.BATCH_NO = dataTable_product_detail.Rows[j]["BATCH_NO"].ToString();
                        _distribution_product.SKU_NAME = dataTable_product_detail.Rows[j]["SKU_NAME"].ToString();
                        _distribution_product.ENTERED_DATE = dataTable_product_detail.Rows[j]["ENTERED_DATE"].ToString();
                        _distribution_product.ROW_NO = Convert.ToInt32(dataTable_product_detail .Rows[j]["ROW_NO"]);
                        _distribution_Req.requisitionProductDtlList.Add(_distribution_product);
                    }
                }
            }

            return _distribution_Mst;
        }

     
    }
}
