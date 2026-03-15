using Microsoft.Extensions.Configuration;
using NUnit.Framework.Internal;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Inventory;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Inventory.IManager;
using SalesAndDistributionSystem.Services.Business.Inventory.Manager.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Inventory.Manager
{
    public class StockTransferRcvManager : IStockTransferRcvManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public StockTransferRcvManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }
        string LoadDispatchedTransferData_Query() => @"SELECT
    ROW_NUMBER() OVER (ORDER BY M.MST_ID ASC) AS ROW_NO,
    M.MST_ID,
    M.REF_NO,
    M.REF_DATE,
    M.TRANSFER_TYPE,
    A.DISPATCH_NO,
    M.TRANSFER_NO,
    TO_CHAR(M.TRANSFER_DATE, 'DD/MM/YYYY') TRANSFER_DATE,
    M.TRANSFER_UNIT_ID,
    M.TRANS_RCV_UNIT_ID,
    M.TRANSFER_AMOUNT,
    M.TRANSFER_BY,
    M.STATUS,
    M.COMPANY_ID,
    M.REMARKS,
    M.ENTERED_BY,
    M.ENTERED_DATE,
    M.ENTERED_TERMINAL,
    FN_UNIT_NAME(M.COMPANY_ID, M.TRANSFER_UNIT_ID) TRANSFER_UNIT_NAME
FROM DEPOT_STOCK_TRANSFER_MST M
JOIN DEPOT_DISPATCH_REQUISITION B ON B.REQUISITION_NO = M.TRANSFER_NO
JOIN DEPOT_DISPATCH_MST A ON A.MST_ID = B.MST_ID
LEFT JOIN DEPOT_STOCK_TRANS_RCV_MST C ON A.DISPATCH_NO || B.REQUISITION_NO = C.DISPATCH_NO || C.TRANSFER_NO
WHERE M.COMPANY_ID = :param1
  AND M.TRANS_RCV_UNIT_ID = :param2
  AND A.DISPATCH_TYPE = 'Stock'
  AND C.DISPATCH_NO IS NULL -- Use a LEFT JOIN to exclude existing records
ORDER BY M.MST_ID DESC";
        string LoadDispatchedTransferDtl_Query() => @"SELECT ROW_NUMBER () OVER (ORDER BY C.DISPATCH_PRODUCT_ID ASC) AS ROW_NO,
       C.SKU_CODE,
      TO_CHAR(C.SKU_ID) SKU_ID,
       FN_SKU_NAME (A.COMPANY_ID, C.SKU_ID) SKU_NAME,
       STL_ERP_SDS.FN_SKU_PRICE(C.SKU_ID, C.SKU_CODE,C.COMPANY_ID, B.REQUISITION_UNIT_ID) UNIT_TP,
       FN_PACK_SIZE(C.COMPANY_ID,C.SKU_ID) PACK_SIZE,
       C.DISPATCH_QTY TRANSFER_QTY,
       C.DISPATCH_AMOUNT TRANSFER_AMOUNT,
       '' STATUS,
       C.COMPANY_ID,
       C.SHIPPER_QTY,
       C.NO_OF_SHIPPER,
       C.LOOSE_QTY,
       C.SHIPPER_WEIGHT,
       C.TOTAL_SHIPPER_WEIGHT,
       C.LOOSE_WEIGHT,
       C.TOTAL_WEIGHT,
       C.SHIPPER_VOLUME,
       C.TOTAL_SHIPPER_VOLUME,
       C.LOOSE_VOLUME,
       C.TOTAL_VOLUME,
       C.PER_PACK_VOLUME,
       C.PER_PACK_WEIGHT
  FROM DEPOT_DISPATCH_MST A,
       DEPOT_DISPATCH_REQUISITION B,
       DEPOT_DISPATCH_PRODUCT C,
       PRODUCT_INFO D
 WHERE     A.MST_ID = B.MST_ID
       AND B.DISPATCH_REQ_ID = C.DISPATCH_REQ_ID
       AND C.SKU_ID = D.SKU_ID
       AND A.COMPANY_ID = :param1
       AND B.REQUISITION_UNIT_ID = :param2
       AND A.DISPATCH_NO = :param3
       AND B.REQUISITION_NO = :param4";

        string AddOrUpdate_AddQuery() => @"INSERT INTO DEPOT_STOCK_TRANS_RCV_MST 
                                        (
                                           MST_ID
                                            ,TRANS_RCV_NO
                                            ,TRANS_RCV_DATE
                                            ,TRANSFER_NO
                                            ,TRANSFER_DATE
                                            ,TRANSFER_UNIT_ID
                                            ,TRANS_RCV_UNIT_ID
                                            ,TRANSFER_AMOUNT
                                            ,TRANS_RCV_AMOUNT
                                            ,TRANS_RCV_BY
                                            ,STATUS
                                            ,COMPANY_ID
                                            ,REMARKS
                                            ,ENTERED_BY
                                            ,ENTERED_DATE
                                            ,ENTERED_TERMINAL
                                            ,DISPATCH_NO

                                         ) 
                                          VALUES ( :param1, :param2,TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM'), :param4,TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM') ,:param6, :param7, :param8, :param9, :param10, :param11,:param12,:param13,:param14,TO_DATE(:param15, 'DD/MM/YYYY HH:MI:SS AM'), :param16,:param17)";

        string AddOrUpdate_AddQueryDTL() => @"INSERT INTO DEPOT_STOCK_TRANS_RCV_DTL 
                    (MST_ID
                        ,DTL_ID
                        ,SKU_ID
                        ,SKU_CODE
                        ,UNIT_TP
                        ,TRANSFER_QTY
                        ,TRANSFER_AMOUNT
                        ,TRANS_RCV_QTY
                        ,TRANS_RCV_AMOUNT
                        ,STATUS
                        ,COMPANY_ID
                        ,ENTERED_BY
                        ,ENTERED_DATE
                        ,ENTERED_TERMINAL
              
                   ) 
                    VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8, :param9, :param10, :param11,:param12,TO_DATE(:param13, 'DD/MM/YYYY HH:MI:SS AM'),:param14)";

        string AddOrUpdate_UpdateQuery() => @"UPDATE DEPOT_STOCK_TRANS_RCV_MST SET  
                                                TRANS_RCV_NO = :param2
                                                ,TRANS_RCV_DATE= TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,TRANSFER_NO= :param4
                                                ,TRANSFER_DATE= TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,TRANSFER_UNIT_ID= :param6
                                                ,TRANS_RCV_UNIT_ID= :param7
                                                ,TRANSFER_AMOUNT= :param8
                                                ,TRANS_RCV_AMOUNT= :param9
                                                ,TRANS_RCV_BY= :param10
                                                ,STATUS= :param11
                                                ,COMPANY_ID= :param12
                                                ,REMARKS= :param13
                                                ,UPDATED_BY= :param14
                                                ,UPDATED_DATE= TO_DATE(:param15, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,UPDATED_TERMINAL= :param16
                                                ,DISPATCH_NO=:param17
                                                WHERE MST_ID = :param1";


        string AddOrUpdate_UpdateQueryDTL() => @"UPDATE DEPOT_STOCK_TRANS_RCV_DTL SET  
                                                 SKU_ID= :param2
                                                ,SKU_CODE= :param3
                                                ,UNIT_TP= :param4
                                                ,TRANSFER_QTY= :param5
                                                ,TRANSFER_AMOUNT= :param6
                                                ,TRANS_RCV_QTY= :param7
                                                ,TRANS_RCV_AMOUNT= :param8
                                                ,STATUS= :param9
                                                ,COMPANY_ID= :param10
                                                ,UPDATED_BY= :param11
                                                ,UPDATED_DATE=  TO_DATE(:param12, 'DD/MM/YYYY HH:MI:SS AM')
                                                ,UPDATED_TERMINAL= :param13
                                                WHERE DTL_ID = :param1";
        string LoadData_MasterById_Query() => @"
            SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO,MST_ID
            ,TRANS_RCV_NO
            , TO_CHAR(TRANS_RCV_DATE, 'DD/MM/YYYY') TRANS_RCV_DATE
            ,TRANSFER_NO
            ,TO_CHAR(TRANSFER_DATE, 'DD/MM/YYYY') TRANSFER_DATE
            ,TRANSFER_UNIT_ID
            ,TRANS_RCV_UNIT_ID
            ,TRANSFER_AMOUNT
            ,TRANS_RCV_AMOUNT
            ,TRANS_RCV_BY
            ,STATUS
            ,COMPANY_ID
            ,REMARKS
            ,ENTERED_BY
            ,ENTERED_DATE
            ,ENTERED_TERMINAL
            ,UPDATED_BY
            ,UPDATED_DATE
            ,UPDATED_TERMINAL 
            ,DISPATCH_NO       
            FROM DEPOT_STOCK_TRANS_RCV_MST  M
            where M.MST_ID = :param1";

        string LoadData_DetailByMasterId_Query() => @" SELECT ROW_NUMBER () OVER (ORDER BY B.DTL_ID ASC) AS ROW_NO,
                                                       B.* ,
                                                       FN_SKU_NAME(B.COMPANY_ID,B.SKU_ID) SKU_NAME,
                                                       C.PACK_SIZE
                                        
                                                FROM DEPOT_STOCK_TRANS_RCV_MST A, DEPOT_STOCK_TRANS_RCV_DTL B,PRODUCT_INFO C
                                                WHERE A.MST_ID        = B.MST_ID
                                                AND   B.SKU_ID        = C.SKU_ID
                                                AND   A.MST_ID        = :param1";

        string LoadMasterData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO,M.MST_ID
                                            ,M.TRANS_RCV_NO
                                            ,TO_CHAR(M.TRANS_RCV_DATE, 'DD/MM/YYYY') TRANS_RCV_DATE
                                            ,M.TRANSFER_NO
                                            , TO_CHAR(M.TRANSFER_DATE, 'DD/MM/YYYY') TRANSFER_DATE
                                            ,M.TRANSFER_UNIT_ID
                                            ,M.TRANS_RCV_UNIT_ID
                                            ,M.TRANSFER_AMOUNT
                                            ,M.TRANS_RCV_AMOUNT
                                            ,M.TRANS_RCV_BY
                                            ,M.STATUS
                                            ,M.COMPANY_ID
                                            ,M.REMARKS
                                            ,M.ENTERED_BY
                                            ,M.ENTERED_DATE
                                            ,M.ENTERED_TERMINAL
                                            ,M.UPDATED_BY
                                            ,M.UPDATED_DATE
                                            ,M.UPDATED_TERMINAL
                                           
                                            ,(SELECT  UNIT_NAME FROM COMPANY_INFO CI WHERE M.TRANS_RCV_UNIT_ID = CI.UNIT_ID AND ROWNUM = 1) TRANS_RCV_UNIT_NAME
            FROM DEPOT_STOCK_TRANS_RCV_MST  M 
            where  M.COMPANY_ID = :param1 AND trunc(M.ENTERED_DATE ) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY')
            ORDER BY M.MST_ID DESC";

        string GetStock_Mst_IdQuery() => "SELECT NVL(MAX(MST_ID),0) + 1 MST_ID  FROM DEPOT_STOCK_TRANS_RCV_MST";
        string GetRequisition_Dtl_IdQuery() => "SELECT NVL(MAX(DTL_ID),0) + 1 DTL_ID  FROM DEPOT_STOCK_TRANS_RCV_DTL";
        string Get_LastStock_no() => "SELECT  TRANSFER_NO  FROM DEPOT_STOCK_TRANS_RCV_MST Where  MST_ID = (SELECT   NVL(MAX(MST_ID),0) MST_ID From DEPOT_STOCK_TRANS_RCV_MST where COMPANY_ID = :param1 )";
        public async Task<string> AddOrUpdate(string db, DEPOT_STOCK_TRANS_RCV_MST model)
        {
            Result _result = new Result();
            if (model == null)
            {
                _result.Status = "No Data Found! Contact with software team";
                return JsonSerializer.Serialize(_result);


            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {
                    if (!IsDataInsertedForTransferAndDispatch(db, model.TRANSFER_NO, model.DISPATCH_NO))
                    {
                        int dtlId = 0;
                        if (model.MST_ID == 0)
                        {

                            model.MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetStock_Mst_IdQuery(), _commonServices.AddParameter(new string[] { }));

                            model.TRANS_RCV_NO = _commonServices.GenerateRequisitionCode(_configuration.GetConnectionString(db), "FN_GENERATE_TRANSFER_RCV_NO", model.COMPANY_ID.ToString(), model.TRANSFER_UNIT_ID.ToString());
                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                             _commonServices.AddParameter(new string[] {model.MST_ID.ToString(),
                             model.TRANS_RCV_NO, model.TRANS_RCV_DATE,model.TRANSFER_NO,
                             model.TRANSFER_DATE, model.TRANSFER_UNIT_ID,model.TRANS_RCV_UNIT_ID.ToString(),
                             model.TRANSFER_AMOUNT.ToString(),model.TRANS_RCV_AMOUNT.ToString(),model.TRANS_RCV_BY.ToString(),
                             model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                             model.ENTERED_BY, model.ENTERED_DATE,  model.ENTERED_TERMINAL, model.DISPATCH_NO
                                })));

                            List<CheckTransferQtyVsRcv> previousRcvQtyByTransferNo = await LoadPreviousRcvQtyByTransferNo(db, model.TRANSFER_NO.Trim());
                            if (model.stockTransferDtlRcvList != null && model.stockTransferDtlRcvList.Count > 0)
                            {
                                dtlId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { }));
                                foreach (var item in model.stockTransferDtlRcvList)
                                {
                                    bool result = true;
                                    var filterObj = previousRcvQtyByTransferNo.Where(x => x.SKU_ID == item.SKU_ID && x.SKU_CODE == item.SKU_CODE).ToList();
                                    if (filterObj.Count > 0)
                                    {
                                        result = filterObj.Any(x => x.TRANSFER_QTY >= (x.TRANS_RCV_QTY + item.TRANS_RCV_QTY));
                                    }

                                    if (result)
                                    {
                                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(), _commonServices.AddParameter(new string[] {
                                     model.MST_ID.ToString(),
                                     dtlId.ToString(),
                                     item.SKU_ID.ToString(),
                                     item.SKU_CODE,
                                     item.UNIT_TP.ToString(),
                                     item.TRANSFER_QTY.ToString(),
                                     item.TRANSFER_AMOUNT.ToString(),
                                     item.TRANS_RCV_QTY.ToString(),
                                     item.TRANS_RCV_AMOUNT.ToString(),
                                     item.STATUS,
                                     item.COMPANY_ID.ToString(),
                                     item.ENTERED_BY,
                                     item.ENTERED_DATE,
                                     item.ENTERED_TERMINAL
                                                                         })));
                                    }
                                    else
                                    {
                                        _result.Status = "TRANS_RCV_QTY  Can not be greater than TRANSFER_QTY. Please Contact with your software team";
                                        return JsonSerializer.Serialize(_result);
                                    }

                                    dtlId++;
                                }
                            }

                        }
                        else
                        {
                            DEPOT_STOCK_TRANS_RCV_MST depot_strock_transfer_Mst = await LoadDetailData_ByMasterId_List(db, model.MST_ID);
                            listOfQuery.Add(_commonServices.AddQuery(
                                AddOrUpdate_UpdateQuery(),
                                _commonServices.AddParameter(new string[] { model.MST_ID.ToString(),
                             model.TRANS_RCV_NO, model.TRANS_RCV_DATE,model.TRANSFER_NO,
                             model.TRANSFER_DATE, model.TRANSFER_UNIT_ID,model.TRANS_RCV_UNIT_ID.ToString(),
                             model.TRANSFER_AMOUNT.ToString(),model.TRANS_RCV_AMOUNT.ToString(),model.TRANS_RCV_BY.ToString(),model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS,
                             model.UPDATED_BY.ToString(), model.UPDATED_DATE.ToString(),
                             model.UPDATED_TERMINAL.ToString(), model.DISPATCH_NO})));
                            foreach (var item in model.stockTransferDtlRcvList)
                            {
                                if (item.DTL_ID == 0)
                                {
                                    //-------------Add new row on detail table--------------------

                                    dtlId = dtlId == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetRequisition_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { })) : (dtlId + 1);

                                    listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(), _commonServices.AddParameter(new string[] {model.MST_ID.ToString(),dtlId.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(), item.TRANSFER_QTY.ToString(), item.TRANSFER_AMOUNT.ToString(),item.TRANS_RCV_QTY.ToString(), item.TRANS_RCV_AMOUNT.ToString(), item.STATUS,
                                     item.COMPANY_ID.ToString(),  item.ENTERED_BY,  item.ENTERED_DATE,  item.ENTERED_TERMINAL})));

                                }
                                else
                                {
                                    //-------------Edit on detail table--------------------
                                    listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQueryDTL(), _commonServices.AddParameter(new string[] { item.DTL_ID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.UNIT_TP.ToString(), item.TRANSFER_QTY.ToString(), item.TRANSFER_AMOUNT.ToString(), item.TRANS_RCV_QTY.ToString(), item.TRANS_RCV_AMOUNT.ToString(), item.STATUS,
                                     item.COMPANY_ID.ToString(),item.UPDATED_BY,item.UPDATED_DATE,item.UPDATED_TERMINAL })));

                                }

                            }
                        }
                    }
                    else
                    {
                        _result.Status = "The dispatch associated with this transfer number has already been marked as received.";
                        return JsonSerializer.Serialize(_result);
                    }
                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    _result.Key = model.MST_ID.ToString();
                    _result.Status = "1";
                }
                catch (Exception ex)
                {
                    _result.Status = ex.Message;
                }
            }
            return JsonSerializer.Serialize(_result);
        }



        public async Task<string> LoadData(string db, int Company_Id, string unitId, string date_from, string date_to)
        {
            List<DEPOT_STOCK_TRANS_RCV_MST> requisition_Mst_list = new List<DEPOT_STOCK_TRANS_RCV_MST>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadMasterData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), date_from, date_to }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                DEPOT_STOCK_TRANS_RCV_MST requisition_Mst = new DEPOT_STOCK_TRANS_RCV_MST();
                requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[i]["MST_ID"]);
                requisition_Mst.TRANSFER_UNIT_ID = data.Rows[i]["TRANSFER_UNIT_ID"].ToString();
                requisition_Mst.TRANS_RCV_UNIT_NAME = data.Rows[i]["TRANS_RCV_UNIT_NAME"].ToString();
                requisition_Mst.TRANS_RCV_UNIT_ID = data.Rows[0]["TRANS_RCV_UNIT_ID"].ToString();
                requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
                requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                requisition_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                requisition_Mst.TRANSFER_AMOUNT = Convert.ToDouble(data.Rows[i]["TRANSFER_AMOUNT"]);

                requisition_Mst.STATUS = data.Rows[i]["STATUS"].ToString();

                requisition_Mst.TRANSFER_DATE = data.Rows[i]["TRANSFER_DATE"].ToString();
                requisition_Mst.TRANS_RCV_DATE = data.Rows[i]["TRANS_RCV_DATE"].ToString();
                requisition_Mst.TRANSFER_NO = data.Rows[i]["TRANSFER_NO"].ToString();
                requisition_Mst.TRANS_RCV_NO = data.Rows[i]["TRANS_RCV_NO"].ToString();
                requisition_Mst.TRANS_RCV_BY = data.Rows[i]["TRANS_RCV_BY"].ToString();

                requisition_Mst_list.Add(requisition_Mst);
            }
            return JsonSerializer.Serialize(requisition_Mst_list);
        }



        public async Task<DEPOT_STOCK_TRANS_RCV_MST> LoadDetailData_ByMasterId_List(string db, int _Id)
        {
            DEPOT_STOCK_TRANS_RCV_MST _requisition_Mst = new DEPOT_STOCK_TRANS_RCV_MST();

            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
            if (data.Rows.Count > 0)
            {
                _requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);

                _requisition_Mst.ROW_NO = Convert.ToInt32(data.Rows[0]["ROW_NO"]);
                _requisition_Mst.MST_ID = Convert.ToInt32(data.Rows[0]["MST_ID"]);


                _requisition_Mst.TRANSFER_UNIT_ID = data.Rows[0]["TRANSFER_UNIT_ID"].ToString();
                _requisition_Mst.TRANS_RCV_UNIT_ID = data.Rows[0]["TRANS_RCV_UNIT_ID"].ToString();
                _requisition_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[0]["MST_ID"].ToString());
                _requisition_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[0]["COMPANY_ID"]);
                _requisition_Mst.REMARKS = data.Rows[0]["REMARKS"].ToString();
                _requisition_Mst.TRANSFER_AMOUNT = Convert.ToDouble(data.Rows[0]["TRANSFER_AMOUNT"]);
                _requisition_Mst.TRANS_RCV_AMOUNT = Convert.ToDouble(data.Rows[0]["TRANS_RCV_AMOUNT"]);

                _requisition_Mst.TRANS_RCV_BY = data.Rows[0]["TRANS_RCV_BY"].ToString();
                _requisition_Mst.TRANSFER_DATE = data.Rows[0]["TRANSFER_DATE"].ToString();
                _requisition_Mst.TRANS_RCV_DATE = data.Rows[0]["TRANS_RCV_DATE"].ToString();
                _requisition_Mst.TRANSFER_NO = data.Rows[0]["TRANSFER_NO"].ToString();
                _requisition_Mst.TRANS_RCV_NO = data.Rows[0]["TRANS_RCV_NO"].ToString();

                _requisition_Mst.STATUS = data.Rows[0]["STATUS"].ToString();
                _requisition_Mst.TRANS_RCV_UNIT_ID = data.Rows[0]["TRANS_RCV_UNIT_ID"].ToString();
                _requisition_Mst.DISPATCH_NO = data.Rows[0]["DISPATCH_NO"].ToString();
                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { _Id.ToString() }));
                _requisition_Mst.stockTransferDtlRcvList = new List<DEPOT_STOCK_TRANS_RCV_DTL>();
                for (int i = 0; i < dataTable_detail.Rows.Count; i++)
                {
                    DEPOT_STOCK_TRANS_RCV_DTL _requisition_Dtl = new DEPOT_STOCK_TRANS_RCV_DTL();

                    _requisition_Dtl.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                    _requisition_Dtl.MST_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MST_ID"]);
                    _requisition_Dtl.DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DTL_ID"]);

                    _requisition_Dtl.SKU_ID = dataTable_detail.Rows[i]["SKU_ID"].ToString();
                    _requisition_Dtl.SKU_CODE = dataTable_detail.Rows[i]["SKU_CODE"].ToString();
                    _requisition_Dtl.UNIT_TP = Convert.ToDouble(dataTable_detail.Rows[i]["UNIT_TP"]);
                    _requisition_Dtl.TRANSFER_QTY = Convert.ToInt32(dataTable_detail.Rows[i]["TRANSFER_QTY"]);
                    _requisition_Dtl.TRANSFER_AMOUNT = Convert.ToDouble(dataTable_detail.Rows[i]["TRANSFER_AMOUNT"]);
                    _requisition_Dtl.TRANS_RCV_AMOUNT = Convert.ToDouble(dataTable_detail.Rows[i]["TRANS_RCV_AMOUNT"]);
                    _requisition_Dtl.TRANS_RCV_QTY = Convert.ToDouble(dataTable_detail.Rows[i]["TRANS_RCV_QTY"]);

                    _requisition_Dtl.STATUS = dataTable_detail.Rows[i]["STATUS"].ToString();
                    _requisition_Dtl.SKU_NAME = dataTable_detail.Rows[i]["SKU_NAME"].ToString();

                    _requisition_Dtl.ENTERED_BY = dataTable_detail.Rows[i]["ENTERED_BY"].ToString();
                    _requisition_Dtl.ENTERED_DATE = dataTable_detail.Rows[i]["ENTERED_DATE"].ToString();


                    _requisition_Dtl.ROW_NO = Convert.ToInt32(dataTable_detail.Rows[i]["ROW_NO"]);
                    _requisition_Mst.stockTransferDtlRcvList.Add(_requisition_Dtl);
                }
            }

            return _requisition_Mst;
        }




        public async Task<string> GenerateStockTrnsRcvferCode(string db, string Company_Id)
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
                return "S" + code;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> LoadDispatchedTransferData(string db, int Company_Id, string unitId) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadDispatchedTransferData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), unitId })));

        public async Task<string> LoadDispatchedTransferDtl(string db, DEPOT_STOCK_TRANSFER_MST model) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadDispatchedTransferDtl_Query(), _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.TRANS_RCV_UNIT_ID, model.DISPATCH_NO, model.TRANSFER_NO })));


        public async Task<List<CheckTransferQtyVsRcv>> LoadPreviousRcvQtyByTransferNo(string db, string tranferNo)
        {
            List<CheckTransferQtyVsRcv> previousRcvQtyByTransferNo = new List<CheckTransferQtyVsRcv>();
            string query = @"SELECT X.TRANSFER_NO,
       X.SKU_CODE,
       X.SKU_ID,
       X.TRANS_RCV_QTY,
       Y.TRANSFER_QTY
  FROM (  SELECT M.TRANSFER_NO,
                 D.SKU_CODE,
                 D.SKU_ID,
                 SUM (D.TRANS_RCV_QTY) TRANS_RCV_QTY
            FROM DEPOT_STOCK_TRANS_RCV_MST M
                 LEFT OUTER JOIN DEPOT_STOCK_TRANS_RCV_DTL D
                    ON M.MST_ID = D.MST_ID
           WHERE M.TRANSFER_NO = :param1
        GROUP BY M.TRANSFER_NO, D.SKU_CODE, D.SKU_ID
        ORDER BY M.TRANSFER_NO, D.SKU_CODE, D.SKU_ID) X,
       (SELECT M.TRANSFER_NO,
               D.SKU_ID,
               D.SKU_CODE,
               D.TRANSFER_QTY
          FROM DEPOT_STOCK_TRANSFER_MST M
               LEFT OUTER JOIN DEPOT_STOCK_TRANSFER_DTL D
                  ON M.MST_ID = D.MST_ID
         WHERE M.TRANSFER_NO = :param1) Y
 WHERE     X.TRANSFER_NO = Y.TRANSFER_NO
       AND X.SKU_CODE = Y.SKU_CODE
       AND X.SKU_ID = Y.SKU_ID";
            DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { tranferNo }));

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                CheckTransferQtyVsRcv _requisition_Dtl = new CheckTransferQtyVsRcv();
                _requisition_Dtl.SKU_CODE = dataTable.Rows[i]["SKU_CODE"].ToString();
                _requisition_Dtl.SKU_ID = dataTable.Rows[i]["SKU_ID"].ToString();
                _requisition_Dtl.TRANS_RCV_QTY = Convert.ToDouble(dataTable.Rows[i]["TRANS_RCV_QTY"]);
                _requisition_Dtl.TRANSFER_QTY = Convert.ToDouble(dataTable.Rows[i]["TRANSFER_QTY"]);
                previousRcvQtyByTransferNo.Add(_requisition_Dtl);
            }
            return previousRcvQtyByTransferNo;
        }

        public bool IsDataInsertedForTransferAndDispatch(string db, string tranferNo, string dispatchNo)
        {
            List<CheckTransferQtyVsRcv> previousRcvQtyByTransferNo = new List<CheckTransferQtyVsRcv>();
            string query = @" SELECT M.TRANSFER_NO, M.DISPATCH_NO
                             FROM DEPOT_STOCK_TRANS_RCV_MST M
                             WHERE M.TRANSFER_NO = :param1 AND M.DISPATCH_NO = :param2";
            DataTable dataTable = _commonServices.GetDataTable(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { tranferNo, dispatchNo }));

            if (dataTable.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
