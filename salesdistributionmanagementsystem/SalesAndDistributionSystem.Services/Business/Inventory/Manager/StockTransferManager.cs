using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Utility;
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
    public class StockTransferManager : IStockTransferManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public StockTransferManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        string AddOrUpdate_AddQuery() => @"INSERT INTO DEPOT_STOCK_TRANSFER_MST 
                                        (
                                            MST_ID
                                            ,REF_NO
                                            ,REF_DATE
                                            ,TRANSFER_TYPE
                                            ,TRANSFER_NO
                                            ,TRANSFER_DATE
                                            ,TRANSFER_UNIT_ID
                                            ,TRANS_RCV_UNIT_ID
                                            ,TRANSFER_AMOUNT
                                            ,TRANSFER_BY
                                            ,STATUS
                                            ,COMPANY_ID
                                            ,REMARKS
                                            ,ENTERED_BY
                                            ,ENTERED_DATE
                                            ,ENTERED_TERMINAL
                                            ,TOTAL_WEIGHT
                                            ,TOTAL_VOLUME
                                           
                                         ) 
                                          VALUES ( :param1, :param2,TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM'), :param4,:param5 ,TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), :param7, :param8, :param9, :param10, :param11,:param12,:param13,:param14,TO_DATE(:param15, 'DD/MM/YYYY HH:MI:SS AM'), :param16, :param17, :param18)";

        string AddOrUpdate_AddQueryDTL() => @"INSERT INTO DEPOT_STOCK_TRANSFER_DTL 
                    (MST_ID
                        ,DTL_ID
                        ,SKU_ID
                        ,SKU_CODE
                        ,UNIT_TP
                        ,TRANSFER_QTY
                        ,TRANSFER_AMOUNT
                        ,STATUS
                        ,COMPANY_ID
                        ,ENTERED_BY
                        ,ENTERED_DATE
                        ,ENTERED_TERMINAL
                        ,SHIPPER_QTY
                        ,NO_OF_SHIPPER
                        ,LOOSE_QTY
                        ,SHIPPER_WEIGHT
                        ,TOTAL_SHIPPER_WEIGHT
                        ,LOOSE_WEIGHT
                        ,TOTAL_WEIGHT

                        ,SHIPPER_VOLUME
                        ,TOTAL_SHIPPER_VOLUME
                        ,LOOSE_VOLUME
                        ,TOTAL_VOLUME
                        ,PER_PACK_VOLUME
                        ,PER_PACK_WEIGHT
              
                   ) 
                    VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8, :param9, :param10, TO_DATE(:param11, 'DD/MM/YYYY HH:MI:SS AM'),:param12
, :param13, :param14, :param15, :param16, :param17, :param18, :param19, :param20, :param21, :param22, :param23, :param24, :param25)";

        string AddOrUpdate_UpdateQuery() => @"UPDATE DEPOT_STOCK_TRANSFER_MST SET  
                                                REF_NO= :param2
                                                ,REF_DATE= TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,TRANSFER_TYPE= :param4
                                                ,TRANSFER_NO= :param5
                                                ,TRANSFER_DATE=TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,TRANSFER_UNIT_ID= :param7
                                                ,TRANS_RCV_UNIT_ID= :param8
                                                ,TRANSFER_AMOUNT= :param9
                                                ,TRANSFER_BY= :param10
                                                ,STATUS= :param11
                                                ,COMPANY_ID= :param12
                                                ,REMARKS= :param13
                                                ,UPDATED_BY= :param14
                                                ,UPDATED_DATE= TO_DATE(:param15, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,UPDATED_TERMINAL= :param16
                                                ,TOTAL_WEIGHT= :param17
                                                ,TOTAL_VOLUME= :param18
                                                WHERE MST_ID = :param1";


        string AddOrUpdate_UpdateQueryDTL() => @"UPDATE DEPOT_STOCK_TRANSFER_DTL SET  
                                                 SKU_ID= :param2
                                                ,SKU_CODE= :param3
                                                ,UNIT_TP= :param4
                                                ,TRANSFER_QTY= :param5
                                                ,TRANSFER_AMOUNT= :param6
                                                ,STATUS= :param7
                                                ,COMPANY_ID= :param8
                                                ,UPDATED_BY= :param9
                                                ,UPDATED_DATE=  TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,UPDATED_TERMINAL= :param11
                                                ,SHIPPER_QTY= :param12
                                                ,NO_OF_SHIPPER= :param13
                                                ,LOOSE_QTY= :param14
                                                ,SHIPPER_WEIGHT= :param15
                                                ,TOTAL_SHIPPER_WEIGHT= :param16
                                                ,LOOSE_WEIGHT= :param17
                                                ,TOTAL_WEIGHT= :param18

                                                ,SHIPPER_VOLUME= :param19
                                                ,TOTAL_SHIPPER_VOLUME= :param20
                                                ,LOOSE_VOLUME= :param21
                                                ,TOTAL_VOLUME= :param22
                                                ,PER_PACK_VOLUME= :param23
                                                ,PER_PACK_WEIGHT= :param24
                                                WHERE DTL_ID = :param1";

        string LoadData_MasterById_Query() => @"
            SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO,M.MST_ID
                                                ,M.REF_NO
                                                ,TO_CHAR(M.REF_DATE, 'DD/MM/YYYY') REF_DATE
                                                ,M.TRANSFER_TYPE
                                                ,M.TRANSFER_NO
                                                ,TO_CHAR(M.TRANSFER_DATE, 'DD/MM/YYYY HH:MI:SS AM') TRANSFER_DATE
                                                ,M.TRANSFER_UNIT_ID
                                                ,M.TRANS_RCV_UNIT_ID
                                                ,M.TRANSFER_AMOUNT
                                                ,M.TRANSFER_BY
                                                ,M.STATUS
                                                ,M.COMPANY_ID
                                                ,M.REMARKS
                                                ,M.ENTERED_BY
                                                ,M.ENTERED_DATE
                                                ,M.ENTERED_TERMINAL
                                                ,M.UPDATED_BY
                                                ,M.UPDATED_DATE
                                                ,M.UPDATED_TERMINAL
                                                ,M.TOTAL_WEIGHT
                                                ,M.TOTAL_VOLUME
            FROM DEPOT_STOCK_TRANSFER_MST  M 
            where M.MST_ID = :param1";

        string LoadData_DetailByMasterId_Query() => @"  SELECT ROW_NUMBER () OVER (ORDER BY B.DTL_ID ASC) AS ROW_NO,
                                                       B.DTL_ID, B.MST_ID, B.SKU_ID, B.SKU_CODE,  B.TRANSFER_QTY, B.TRANSFER_AMOUNT, B.STATUS, B.COMPANY_ID, B.ENTERED_BY, B.ENTERED_DATE, B.ENTERED_TERMINAL, B.UPDATED_BY, B.UPDATED_DATE, B.UPDATED_TERMINAL, B.SHIPPER_QTY, B.NO_OF_SHIPPER, B.LOOSE_QTY, B.SHIPPER_WEIGHT, B.TOTAL_SHIPPER_WEIGHT, B.LOOSE_WEIGHT, B.TOTAL_WEIGHT, B.SHIPPER_VOLUME, B.TOTAL_SHIPPER_VOLUME, B.LOOSE_VOLUME, B.TOTAL_VOLUME, B.PER_PACK_VOLUME, B.PER_PACK_WEIGHT,  FN_SKU_PRICE (B.SKU_ID, B.SKU_CODE,  B.COMPANY_ID,A.TRANSFER_UNIT_ID) UNIT_TP,

                                                       C.PACK_SIZE, 
                                                       NVL((SELECT STOCK_QTY FROM VW_SKU_WISE_STOCK WHERE COMPANY_ID=A.COMPANY_ID AND UNIT_ID=A.TRANSFER_UNIT_ID AND SKU_ID=B.SKU_ID),0) STOCK_QTY
                                        
                                                FROM DEPOT_STOCK_TRANSFER_MST A, DEPOT_STOCK_TRANSFER_DTL B,PRODUCT_INFO C
                                                WHERE A.MST_ID        = B.MST_ID
                                                AND   B.SKU_ID        = C.SKU_ID
                                                AND   A.MST_ID        = :param1";

        string GetStock_Mst_IdQuery() => "SELECT NVL(MAX(MST_ID),0) + 1 MST_ID  FROM DEPOT_STOCK_TRANSFER_MST";
        string GetRequisition_Dtl_IdQuery() => "SELECT NVL(MAX(DTL_ID),0) + 1 DTL_ID  FROM DEPOT_STOCK_TRANSFER_DTL";
        string Get_LastStock_no() => "SELECT  TRANSFER_NO  FROM DEPOT_STOCK_TRANSFER_MST Where  MST_ID = (SELECT   NVL(MAX(MST_ID),0) MST_ID From DEPOT_STOCK_TRANSFER_MST where COMPANY_ID = :param1 )";
        string DeleteArea_strock_Dtl_IdQuery() => "DELETE  FROM DEPOT_STOCK_TRANSFER_DTL WHere DTL_ID = :param1";

        string LoadProductData_Query(string UnitId) => string.Format(@"Select
                               ROW_NUMBER() OVER(ORDER BY p.SKU_ID ASC) AS ROW_NO
                              ,p.BASE_PRODUCT_ID
                              ,p.BRAND_ID
                              ,p.CATEGORY_ID
                              ,p.COMPANY_ID
                              ,p.FONT_COLOR
                              ,p.GROUP_ID
                              ,p.PACK_SIZE
                              ,p.PACK_UNIT
                              ,p.PACK_VALUE
                              ,p.PRIMARY_PRODUCT_ID
                              ,p.PRODUCT_SEASON_ID
                              ,p.PRODUCT_STATUS
                              ,p.PRODUCT_TYPE_ID
                              ,p.QTY_PER_PACK
                              ,p.REMARKS
                              ,p.SHIPPER_QTY
                              ,p.SHIPPER_VOLUME
                              ,p.SHIPPER_VOLUME_UNIT
                              ,p.SHIPPER_WEIGHT
                              ,p.SHIPPER_WEIGHT_UNIT
                              ,p.SKU_CODE
                              ,p.SKU_ID
                              ,p.SKU_NAME
                              ,p.SKU_NAME_BANGLA
                              ,p.STORAGE_ID
                              ,p.UNIT_ID
                              ,p.WEIGHT_PER_PACK
                              ,p.WEIGHT_UNIT
                              ,BP.BASE_PRODUCT_NAME
                              ,b.BRAND_NAME
                              ,g.GROUP_NAME
                              ,C.CATEGORY_NAME
                              ,0 UNIT_TP
                              ,NVL((SELECT STOCK_QTY FROM VW_SKU_WISE_STOCK WHERE COMPANY_ID=P.COMPANY_ID AND UNIT_ID= {0} AND SKU_ID=P.SKU_ID),0) STOCK_QTY
                               from Product_Info p
 
                               left outer join Base_Product_Info bp on BP.BASE_PRODUCT_ID = P.BASE_PRODUCT_ID
                               left outer join Category_info c on C.CATEGORY_ID = P.CATEGORY_ID
                               left outer join Group_Info g on g.GROUP_ID = P.GROUP_ID
                               left outer join BRAND_INFO b on b.BRAND_ID = P.BRAND_ID
 
                                Where p.COMPANY_ID = :param1", UnitId);

        string LoadMasterData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO,
                                                M.MST_ID
                                                ,M.REF_NO
                                                ,M.REF_DATE
                                                ,M.TRANSFER_TYPE
                                                ,M.TRANSFER_NO
                                                ,TO_CHAR(M.TRANSFER_DATE, 'DD/MM/YYYY') TRANSFER_DATE
                                                ,M.TRANSFER_UNIT_ID
                                                ,M.TRANS_RCV_UNIT_ID
                                                ,(Select SUM(FN_SKU_PRICE (SKU_ID, SKU_CODE,  M.COMPANY_ID, M.TRANSFER_UNIT_ID)*TRANSFER_QTY) from DEPOT_STOCK_TRANSFER_DTL where MST_ID = M.MST_ID) TRANSFER_AMOUNT
                                                ,M.TRANSFER_BY
                                                ,M.STATUS
                                                ,M.COMPANY_ID
                                                ,M.REMARKS
                                                ,M.ENTERED_BY
                                                ,M.ENTERED_DATE
                                                ,M.ENTERED_TERMINAL
                                             ,M.TOTAL_WEIGHT
                                                ,M.TOTAL_VOLUME
                                            ,(SELECT  UNIT_NAME FROM COMPANY_INFO CI WHERE M.TRANSFER_UNIT_ID = CI.UNIT_ID AND ROWNUM = 1) TRANSFER_UNIT_NAME
            FROM DEPOT_STOCK_TRANSFER_MST  M 
            where  M.COMPANY_ID = :param1 AND M.TRANSFER_UNIT_ID= :param2 AND trunc(M.ENTERED_DATE ) BETWEEN TO_DATE(:param3, 'DD/MM/YYYY') AND TO_DATE(:param4, 'DD/MM/YYYY') and M.TRANSFER_NO NOT IN (SELECT TRANSFER_NO FROM DEPOT_STOCK_TRANS_RCV_MST) ORDER BY M.MST_ID DESC";
        string LoadReceivableTransfer_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO,
                                                M.MST_ID
                                                ,M.REF_NO
                                                ,M.REF_DATE
                                                ,M.TRANSFER_TYPE
                                                ,M.TRANSFER_NO
                                                ,TO_CHAR(M.TRANSFER_DATE, 'DD/MM/YYYY') TRANSFER_DATE
                                                ,M.TRANSFER_UNIT_ID
                                                ,M.TRANS_RCV_UNIT_ID
                                                ,M.TRANSFER_AMOUNT
                                                ,M.TRANSFER_BY
                                                ,M.STATUS
                                                ,M.COMPANY_ID
                                                ,M.REMARKS
                                                ,M.ENTERED_BY
                                                ,M.ENTERED_DATE
                                                ,M.ENTERED_TERMINAL
                                           
                                            ,(SELECT  UNIT_NAME FROM COMPANY_INFO CI WHERE M.TRANSFER_UNIT_ID = CI.UNIT_ID AND ROWNUM = 1) TRANSFER_UNIT_NAME
            FROM DEPOT_STOCK_TRANSFER_MST  M 
            where  M.COMPANY_ID = :param1 AND M.TRANS_RCV_UNIT_ID= :param2 AND M.TRANSFER_NO  IN (SELECT REQUISITION_NO FROM DEPOT_DISPATCH_REQUISITION) AND M.TRANSFER_NO NOT IN (SELECT TRANSFER_NO FROM DEPOT_STOCK_TRANS_RCV_MST)  ORDER BY M.MST_ID DESC";
        
        public async Task<string> LoadProductData(string db, int Company_Id,string UnitId) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadProductData_Query(UnitId), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> AddOrUpdate(string db, DEPOT_STOCK_TRANSFER_MST model)
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

                        model.MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetStock_Mst_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        model.TRANSFER_NO = _commonServices.GenerateRequisitionCode(_configuration.GetConnectionString(db), "FN_GENERATE_STOCK_TRANSFER_NO", model.COMPANY_ID.ToString(), model.TRANSFER_UNIT_ID.ToString());

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                         _commonServices.AddParameter(new string[] {model.MST_ID.ToString(),
                             model.REF_NO, model.REF_DATE,model.TRANSFER_TYPE,
                             model.TRANSFER_NO, model.TRANSFER_DATE,model.TRANSFER_UNIT_ID.ToString(),model.TRANS_RCV_UNIT_ID.ToString(),
                             model.TRANSFER_AMOUNT.ToString(),model.TRANSFER_BY.ToString(),
                             model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                             model.ENTERED_BY, model.ENTERED_DATE,  model.ENTERED_TERMINAL,model.TOTAL_WEIGHT.ToString(),model.TOTAL_VOLUME.ToString()
                            })));

                        if (model.stockTransferDtlList != null && model.stockTransferDtlList.Count > 0)
                        {
                            dtlId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { }));
                            foreach (var item in model.stockTransferDtlList)
                            {


                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(),
                               _commonServices.AddParameter(new string[] { model.MST_ID.ToString(),dtlId.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(), item.TRANSFER_QTY.ToString(), item.TRANSFER_AMOUNT.ToString(), item.STATUS,
                                     model.COMPANY_ID.ToString(),  item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL,item.SHIPPER_QTY.ToString(),
                                            item.NO_OF_SHIPPER.ToString(),item.LOOSE_QTY.ToString(),item.SHIPPER_WEIGHT.ToString(),item.TOTAL_SHIPPER_WEIGHT.ToString(),item.LOOSE_WEIGHT.ToString(),item.TOTAL_WEIGHT.ToString(),
                                            item.SHIPPER_VOLUME.ToString(),item.TOTAL_SHIPPER_VOLUME.ToString(),item.LOOSE_VOLUME.ToString(),item.TOTAL_VOLUME.ToString(),item.PER_PACK_VOLUME.ToString(),item.PER_PACK_WEIGHT.ToString()
                                  })));
                                dtlId++;
                            }
                        }

                    }
                    else
                    {
                        DEPOT_STOCK_TRANSFER_MST depot_strock_transfer_Mst = await LoadDetailData_ByMasterId_List(db, model.MST_ID);
                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.MST_ID.ToString(),
                             model.REF_NO, model.REF_DATE,model.TRANSFER_TYPE,
                             model.TRANSFER_NO, model.TRANSFER_DATE,model.TRANSFER_UNIT_ID.ToString(),model.TRANS_RCV_UNIT_ID.ToString(),
                             model.TRANSFER_AMOUNT.ToString(),model.TRANSFER_BY.ToString(),model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                             model.UPDATED_BY.ToString(), model.UPDATED_DATE.ToString(),
                             model.UPDATED_TERMINAL.ToString(),model.TOTAL_WEIGHT.ToString(),model.TOTAL_VOLUME.ToString()})));
                        foreach (var item in model.stockTransferDtlList)
                        {
                            if (item.DTL_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------

                                dtlId = dtlId == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (dtlId + 1);

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(), _commonServices.AddParameter(new string[] {model.MST_ID.ToString(),dtlId.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(), item.TRANSFER_QTY.ToString(), item.TRANSFER_AMOUNT.ToString(), item.STATUS,
                                     model.COMPANY_ID.ToString(),  item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL,item.SHIPPER_QTY.ToString(),
                                            item.NO_OF_SHIPPER.ToString(),item.LOOSE_QTY.ToString(),item.SHIPPER_WEIGHT.ToString(),item.TOTAL_SHIPPER_WEIGHT.ToString(),item.LOOSE_WEIGHT.ToString(),item.TOTAL_WEIGHT.ToString(),
                                            item.SHIPPER_VOLUME.ToString(),item.TOTAL_SHIPPER_VOLUME.ToString(),item.LOOSE_VOLUME.ToString(),item.TOTAL_VOLUME.ToString(),item.PER_PACK_VOLUME.ToString(),item.PER_PACK_WEIGHT.ToString()})));

                            }
                            else
                            {
                                //-------------Edit on detail table--------------------
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQueryDTL(), _commonServices.AddParameter(new string[] { item.DTL_ID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(), item.TRANSFER_QTY.ToString(), item.TRANSFER_AMOUNT.ToString(), item.STATUS,
                                     item.COMPANY_ID.ToString(),item.UPDATED_BY,item.UPDATED_DATE,item.UPDATED_TERMINAL,item.SHIPPER_QTY.ToString(),
                                            item.NO_OF_SHIPPER.ToString(),item.LOOSE_QTY.ToString(),item.SHIPPER_WEIGHT.ToString(),item.TOTAL_SHIPPER_WEIGHT.ToString(),item.LOOSE_WEIGHT.ToString(),item.TOTAL_WEIGHT.ToString(),
                                            item.SHIPPER_VOLUME.ToString(),item.TOTAL_SHIPPER_VOLUME.ToString(),item.LOOSE_VOLUME.ToString(),item.TOTAL_VOLUME.ToString(),item.PER_PACK_VOLUME.ToString(),item.PER_PACK_WEIGHT.ToString() })));

                            }

                        }

                        foreach (var item in depot_strock_transfer_Mst.stockTransferDtlList)
                        {
                            bool status = true;

                            foreach (var updateditem in model.stockTransferDtlList)
                            {
                                if (item.DTL_ID == updateditem.DTL_ID)
                                {
                                    status = false;
                                }
                            }
                            if (status)
                            {
                                //-------------Delete row from detail table--------------------

                                listOfQuery.Add(_commonServices.AddQuery(DeleteArea_strock_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { item.DTL_ID.ToString() })));

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

        public Task<string> GetCustomerExistingSKUData(string db, int Company_Id, int Customer_Id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> LoadData(string db, int Company_Id, string unitId, string date_from, string date_to)
        {
            List<DEPOT_STOCK_TRANSFER_MST> requisition_Mst_list = new List<DEPOT_STOCK_TRANSFER_MST>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadMasterData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), unitId,date_from, date_to }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                DEPOT_STOCK_TRANSFER_MST requisition_Mst = new DEPOT_STOCK_TRANSFER_MST();
                requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[i]["MST_ID"]);
                requisition_Mst.TRANSFER_UNIT_ID = data.Rows[i]["TRANSFER_UNIT_ID"].ToString();
                requisition_Mst.TRANSFER_UNIT_NAME = data.Rows[i]["TRANSFER_UNIT_NAME"].ToString();
                requisition_Mst.TRANS_RCV_UNIT_ID = data.Rows[0]["TRANS_RCV_UNIT_ID"].ToString();
                requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
                requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                requisition_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                requisition_Mst.TRANSFER_AMOUNT = Convert.ToDouble(data.Rows[i]["TRANSFER_AMOUNT"]);

                requisition_Mst.TOTAL_WEIGHT = Convert.ToDouble(data.Rows[i]["TOTAL_WEIGHT"]);
                requisition_Mst.TOTAL_VOLUME = Convert.ToDouble(data.Rows[i]["TOTAL_VOLUME"]);
                requisition_Mst.STATUS = data.Rows[i]["STATUS"].ToString();

                requisition_Mst.TRANSFER_DATE = data.Rows[i]["TRANSFER_DATE"].ToString();
                requisition_Mst.REF_DATE = data.Rows[i]["REF_DATE"].ToString();
                requisition_Mst.TRANSFER_NO = data.Rows[i]["TRANSFER_NO"].ToString();
                requisition_Mst.REF_NO = data.Rows[i]["REF_NO"].ToString();
                requisition_Mst.REF_DATE = data.Rows[i]["REF_DATE"].ToString();

                requisition_Mst_list.Add(requisition_Mst);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }

        public Task<string> LoadData_Master(string db, int Company_Id)
        {
            throw new NotImplementedException();
        }

        public async Task<DEPOT_STOCK_TRANSFER_MST> LoadDetailData_ByMasterId_List(string db, int _Id)
        {
            DEPOT_STOCK_TRANSFER_MST _requisition_Mst = new DEPOT_STOCK_TRANSFER_MST();

            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
            if (data.Rows.Count > 0)
            {
                _requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);

                _requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[0]["ROW_NO"]);
                _requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[0]["MST_ID"]);
                _requisition_Mst.REF_NO = data.Rows[0]["REF_NO"].ToString();
                _requisition_Mst.REF_DATE = data.Rows[0]["REF_DATE"].ToString();
       
                _requisition_Mst.TRANSFER_UNIT_ID = data.Rows[0]["TRANSFER_UNIT_ID"].ToString();
                _requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[0]["MST_ID"].ToString());
                _requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);
                _requisition_Mst.REMARKS = data.Rows[0]["REMARKS"].ToString();
                _requisition_Mst.TRANSFER_AMOUNT = Convert.ToDouble(data.Rows[0]["TRANSFER_AMOUNT"]);
     
                _requisition_Mst.TRANSFER_BY = data.Rows[0]["TRANSFER_BY"].ToString();
                _requisition_Mst.TRANSFER_DATE = data.Rows[0]["TRANSFER_DATE"].ToString();
                _requisition_Mst.TRANSFER_NO = data.Rows[0]["TRANSFER_NO"].ToString();
                _requisition_Mst.TOTAL_WEIGHT = Convert.ToDouble(data.Rows[0]["TOTAL_WEIGHT"]);
                _requisition_Mst.TOTAL_VOLUME = Convert.ToDouble(data.Rows[0]["TOTAL_VOLUME"]);

                _requisition_Mst.STATUS = data.Rows[0]["STATUS"].ToString();
                _requisition_Mst.TRANS_RCV_UNIT_ID = data.Rows[0]["TRANS_RCV_UNIT_ID"].ToString();

                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
                _requisition_Mst.stockTransferDtlList = new List<DEPOT_STOCK_TRANSFER_DTL>();
                for (int i = 0; i < dataTable_detail.Rows.Count; i++)
                {
                    DEPOT_STOCK_TRANSFER_DTL _requisition_Dtl = new DEPOT_STOCK_TRANSFER_DTL();

                    _requisition_Dtl.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                    _requisition_Dtl.MST_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MST_ID"]);
                    _requisition_Dtl.DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DTL_ID"]);

                    _requisition_Dtl.SKU_ID = dataTable_detail.Rows[i]["SKU_ID"].ToString();
                    _requisition_Dtl.SKU_CODE = dataTable_detail.Rows[i]["SKU_CODE"].ToString();
                    _requisition_Dtl.UNIT_TP = Convert.ToDouble(dataTable_detail.Rows[i]["UNIT_TP"]);
                    _requisition_Dtl.TRANSFER_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["TRANSFER_QTY"]);
                    _requisition_Dtl.TRANSFER_AMOUNT = Convert.ToDecimal(dataTable_detail.Rows[i]["TRANSFER_AMOUNT"]);
       
                    _requisition_Dtl.STATUS = dataTable_detail.Rows[i]["STATUS"].ToString();
                    _requisition_Dtl.STOCK_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["STOCK_QTY"]);
                    _requisition_Dtl.ENTERED_BY = dataTable_detail.Rows[i]["ENTERED_BY"].ToString();
                    _requisition_Dtl.ENTERED_DATE = dataTable_detail.Rows[i]["ENTERED_DATE"].ToString();
                    _requisition_Dtl.LOOSE_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["LOOSE_QTY"]);
                    _requisition_Dtl.LOOSE_VOLUME = Convert.ToDouble(dataTable_detail.Rows[i]["LOOSE_VOLUME"]);
                    _requisition_Dtl.LOOSE_WEIGHT = Convert.ToDouble(dataTable_detail.Rows[i]["LOOSE_WEIGHT"]);
                    _requisition_Dtl.SHIPPER_WEIGHT = Convert.ToDouble(dataTable_detail.Rows[i]["SHIPPER_WEIGHT"]);
                    _requisition_Dtl.TOTAL_VOLUME = Convert.ToDouble(dataTable_detail.Rows[i]["TOTAL_VOLUME"]);
                    _requisition_Dtl.TOTAL_WEIGHT = Convert.ToDouble(dataTable_detail.Rows[i]["TOTAL_WEIGHT"]);


                    _requisition_Dtl.SHIPPER_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["SHIPPER_QTY"]);
                    _requisition_Dtl.SHIPPER_WEIGHT = Convert.ToDouble(dataTable_detail.Rows[i]["SHIPPER_WEIGHT"]);
                    _requisition_Dtl.SHIPPER_VOLUME = Convert.ToDouble(dataTable_detail.Rows[i]["SHIPPER_VOLUME"]);


                    _requisition_Dtl.ROW_NO = Convert.ToInt32(dataTable_detail.Rows[i]["ROW_NO"]);
                    _requisition_Mst.stockTransferDtlList.Add(_requisition_Dtl);
                }
            }

            return _requisition_Mst;
        }

        public Task<string> LoadSKUPriceDtlData(string db, int Company_Id)
        {
            throw new NotImplementedException();
        }

        public Task<string> LoadSKUPriceMstData(string db, int Company_Id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GenerateStockTrnsferCode(string db, string Company_Id)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastStock_no(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["TRANSFER_NO"].ToString().Substring(5, 4)) + 1).ToString();
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
                return "S"+code;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> LoadReceivableTransfer(string db, int Company_Id, string unitId)
        {
            List<DEPOT_STOCK_TRANSFER_MST> requisition_Mst_list = new List<DEPOT_STOCK_TRANSFER_MST>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadReceivableTransfer_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), unitId }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                DEPOT_STOCK_TRANSFER_MST requisition_Mst = new DEPOT_STOCK_TRANSFER_MST();
                requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[i]["MST_ID"]);
                requisition_Mst.TRANSFER_UNIT_ID = data.Rows[i]["TRANSFER_UNIT_ID"].ToString();
                requisition_Mst.TRANSFER_UNIT_NAME = data.Rows[i]["TRANSFER_UNIT_NAME"].ToString();
                requisition_Mst.TRANS_RCV_UNIT_ID = data.Rows[0]["TRANS_RCV_UNIT_ID"].ToString();
                requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
                requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                requisition_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                requisition_Mst.TRANSFER_AMOUNT = Convert.ToDouble(data.Rows[i]["TRANSFER_AMOUNT"]);


                requisition_Mst.STATUS = data.Rows[i]["STATUS"].ToString();

                requisition_Mst.TRANSFER_DATE = data.Rows[i]["TRANSFER_DATE"].ToString();
                requisition_Mst.REF_DATE = data.Rows[i]["REF_DATE"].ToString();
                requisition_Mst.TRANSFER_NO = data.Rows[i]["TRANSFER_NO"].ToString();
                requisition_Mst.REF_NO = data.Rows[i]["REF_NO"].ToString();
                requisition_Mst.REF_DATE = data.Rows[i]["REF_DATE"].ToString();

                requisition_Mst_list.Add(requisition_Mst);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }

       

        public async Task<string> LoadRcvUnitStock(string db, int unitId, int skuId) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT STOCK_QTY FROM VW_SKU_WISE_STOCK WHERE  UNIT_ID= :param1 AND SKU_ID=:param2", _commonServices.AddParameter(new string[] { unitId.ToString(),skuId.ToString() })));

       
    }
}
