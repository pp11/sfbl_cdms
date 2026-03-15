using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.TableModels.User;
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
    public class FgReceiveFromOthersManager : IFgReceiveFromOthersManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        private readonly IUserManager _userManager;

        public FgReceiveFromOthersManager(ICommonServices commonServices, IConfiguration configuration, IUserManager userManager)
        {
            _commonServices = commonServices;
            _configuration = configuration;
            _userManager = userManager;
        }

        //-------------------Query Part ---------------------------------------------------
        string LoadFGReceiveData_Query() => @"SELECT 
                                        ROW_NUMBER() OVER(ORDER BY M.TRANSFER_NOTE_NO ASC) AS ROW_NO,
                                        M.COMPANY_ID, M.UNIT_ID, M.TRANSFER_NOTE_NO, 
                                        M.TRANSFER_DATE, M.BATCH_NO, M.PRD_PRC_MST_SLNO, 
                                        M.P_PRODUCT_CODE, M.S_PRODUCT_CODE SKU_CODE,P.SKU_NAME ,M.MANUFACTURING_DATE MFG_DATE, 
                                        M.EXPIRE_DATE, M.PACK_SIZE, M.TRANSFER_QTY, 
                                        M.BATCH_STATUS, M.TRANSFER_STATUS, M.EQUIVALENT_QTY, 
                                        M.ISSUED_BY, M.NO_SHIPPER_CTN, M.QTY_PER_SHIPPER, 
                                        M.LOOSE_QTY, M.REMARKS, M.REPACK_FLAG, 
                                        M.UNIT_CODE, M.TEST_REQUEST_NO
                                        FROM VW_PRODUCTION_FG M
                                        LEFT outer join PRODUCT_INFO P on P.SKU_CODE = M.S_PRODUCT_CODE
                                        where M.COMPANY_ID = :param1 and M.UNIT_ID = :param2
                                        and M.TRANSFER_NOTE_NO  not in (Select TRANSFER_NOTE_NO from FG_RECEIVING_FROM_PRODUCTION where M.COMPANY_ID = :param1 and M.UNIT_ID = :param2)";


        string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.RECEIVE_ID ASC) AS ROW_NO,
                                      M.RECEIVE_ID
                                      ,M.BATCH_ID
                                      ,M.BATCH_NO
                                      ,TO_CHAR(M.CHECKED_BY_DATE, 'YYYY-MM-DD') CHECKED_BY_DATE
                                      ,M.CHECKED_BY_ID
                                      ,M.COMPANY_ID
                                      ,TO_CHAR(M.EXPIRY_DATE, 'YYYY-MM-DD') EXPIRY_DATE
                                      ,TO_CHAR(M.MFG_DATE, 'YYYY-MM-DD') MFG_DATE
                                      ,M.PACK_SIZE
                                      ,M.RECEIVED_BY_ID
                                      ,M.RECEIVE_AMOUNT
                                      ,TO_CHAR(M.RECEIVE_DATE, 'YYYY-MM-DD') RECEIVE_DATE
                                      ,M.RECEIVE_QTY
                                      ,M.RECEIVE_STATUS
                                      ,M.RECEIVE_STOCK_TYPE
                                      ,M.REMARKS
                                      ,M.SHIPPER_QTY
                                      ,M.SKU_CODE
                                      ,M.SKU_ID
                                      ,P.SKU_NAME
                                      ,TO_CHAR(M.EXPIRY_DATE , 'YYYY-MM-DD') EXPIRY_DATE
                                   
                                      ,TO_CHAR(M.MFG_DATE, 'YYYY-MM-DD') MFG_DATE
                                      ,TO_CHAR(M.CHALLAN_DATE, 'YYYY-MM-DD') TRANSFER_DATE
                                      ,M.CHALLAN_NO
                                      ,M.RECEIVE_TYPE
                                      ,M.UNIT_ID
                                      ,M.UNIT_TP
                                      ,M.RECEIVE_STATUS
                                      ,S.SUPPLIER_NAME
                                      ,M.SUPPLIER_ID
                                      FROM TT_FG_RECEIVING_FROM_Others  M
LEFT OUTER JOIN PRODUCT_INFO P on P.SKU_ID = M.SKU_ID
LEFT OUTER JOIN SUPPLIER_INFO S ON S.SUPPLIER_ID = M.SUPPLIER_ID
Where M.COMPANY_ID = :param1 AND M.UNIT_ID = :param2 AND trunc(M.RECEIVE_DATE ) BETWEEN TO_DATE(:param3, 'DD/MM/YYYY') AND TO_DATE(:param4, 'DD/MM/YYYY')";
        string GetApprovedList() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.RECEIVE_ID ASC) AS ROW_NO,
                                      M.RECEIVE_ID
                                      ,M.BATCH_ID
                                      ,M.BATCH_NO
                                      ,TO_CHAR(M.CHECKED_BY_DATE, 'YYYY-MM-DD') CHECKED_BY_DATE
                                      ,M.CHECKED_BY_ID
                                      ,M.COMPANY_ID
                                      ,TO_CHAR(M.EXPIRY_DATE, 'YYYY-MM-DD') EXPIRY_DATE
                                      ,TO_CHAR(M.MFG_DATE, 'YYYY-MM-DD') MFG_DATE
                                      ,M.PACK_SIZE
                                      ,M.RECEIVED_BY_ID
                                      ,M.RECEIVE_AMOUNT
                                      ,TO_CHAR(M.RECEIVE_DATE, 'YYYY-MM-DD') RECEIVE_DATE
                                      ,M.RECEIVE_QTY
                                      ,M.RECEIVE_STATUS
                                      ,M.RECEIVE_STOCK_TYPE
                                      ,M.REMARKS
                                      ,M.SHIPPER_QTY
                                      ,M.SKU_CODE
                                      ,M.SKU_ID
                                      ,P.SKU_NAME
                                      ,TO_CHAR(M.EXPIRY_DATE , 'YYYY-MM-DD') EXPIRY_DATE
                                      ,TO_CHAR(M.MFG_DATE, 'YYYY-MM-DD') MFG_DATE
                                      ,TO_CHAR(M.CHALLAN_DATE, 'YYYY-MM-DD') CHALLAN_DATE
                                      ,M.CHALLAN_NO
                                      ,M.RECEIVE_TYPE
                                      ,M.UNIT_ID
                                      ,M.UNIT_TP
                                      FROM TT_FG_RECEIVING_FROM_OTHERS  M
                LEFT OUTER JOIN PRODUCT_INFO P on P.SKU_ID = M.SKU_ID
                Where M.COMPANY_ID = :param1 AND M.UNIT_ID = :param2 and ( M.CHECKED_BY_ID is NOT null and M.CHECKED_BY_ID <> 0 )";
        string LoadUncheckData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.RECEIVE_ID ASC) AS ROW_NO,
                                      M.RECEIVE_ID
                                      ,M.BATCH_ID
                                      ,M.BATCH_NO
                                      ,TO_CHAR(M.CHECKED_BY_DATE, 'YYYY-MM-DD') CHECKED_BY_DATE
                                      ,M.CHECKED_BY_ID
                                      ,M.COMPANY_ID
                                      ,TO_CHAR(M.EXPIRY_DATE, 'YYYY-MM-DD') EXPIRY_DATE
                                      ,TO_CHAR(M.MFG_DATE, 'YYYY-MM-DD') MFG_DATE
                                      ,M.PACK_SIZE
                                      ,M.RECEIVED_BY_ID
                                      ,M.RECEIVE_AMOUNT
                                      ,TO_CHAR(M.RECEIVE_DATE, 'YYYY-MM-DD') RECEIVE_DATE
                                      ,M.RECEIVE_QTY
                                      ,M.RECEIVE_STATUS
                                      ,M.RECEIVE_STOCK_TYPE
                                      ,M.REMARKS
                                      ,M.SHIPPER_QTY
                                      ,M.SKU_CODE
                                      ,M.SKU_ID
                                      ,P.SKU_NAME
                                      ,TO_CHAR(M.EXPIRY_DATE , 'YYYY-MM-DD') EXPIRY_DATE
                                      ,TO_CHAR(M.MFG_DATE, 'YYYY-MM-DD') MFG_DATE
                                      ,TO_CHAR(M.CHALLAN_DATE, 'YYYY-MM-DD') CHALLAN_DATE
                                      ,M.CHALLAN_NO
                                      ,M.RECEIVE_TYPE
                                      ,M.UNIT_ID
                                      ,M.UNIT_TP
                                      ,S.SUPPLIER_NAME
                                      ,M.SUPPLIER_ID
                                      FROM TT_FG_RECEIVING_FROM_OTHERS  M
                LEFT OUTER JOIN PRODUCT_INFO P on P.SKU_ID = M.SKU_ID
                LEFT OUTER JOIN SUPPLIER_INFO S ON S.SUPPLIER_ID = M.SUPPLIER_ID
                Where M.COMPANY_ID = :param1 AND M.UNIT_ID = :param2 and ( M.CHECKED_BY_ID is null or M.CHECKED_BY_ID = 0 )
                AND TRUNC(RECEIVE_DATE) BETWEEN TO_DATE(:param3, 'DD/MM/YYYY') AND TO_DATE(:param4, 'DD/MM/YYYY')  AND NVL(M.STATUS,'DD') != 'Cancelled' ";

        string LoadEditData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.RECEIVE_ID ASC) AS ROW_NO,
                                      M.RECEIVE_ID
                                      ,M.BATCH_ID
                                      ,M.BATCH_NO
									  ,TO_CHAR(M.CHECKED_BY_DATE, 'YYYY-MM-DD') CHECKED_BY_DATE
                                      ,M.CHECKED_BY_ID
                                      ,M.COMPANY_ID
									  ,TO_CHAR(M.EXPIRY_DATE, 'YYYY-MM-DD') EXPIRY_DATE
									  ,TO_CHAR(M.MFG_DATE, 'YYYY-MM-DD') MFG_DATE
                                      ,M.PACK_SIZE
                                      ,M.RECEIVED_BY_ID
                                      ,M.RECEIVE_AMOUNT
									  ,TO_CHAR(M.RECEIVE_DATE, 'YYYY-MM-DD') RECEIVE_DATE
                                      ,M.RECEIVE_QTY
                                      ,M.RECEIVE_STATUS
                                      ,M.RECEIVE_STOCK_TYPE
                                      ,M.REMARKS
                                      ,M.SHIPPER_QTY
                                      ,M.SKU_CODE
                                      ,M.SKU_ID
									  ,TO_CHAR(M.CHALLAN_DATE, 'YYYY-MM-DD') CHALLAN_DATE
                                      ,M.CHALLAN_NO
                                      ,M.RECEIVE_TYPE
                                      ,M.UNIT_ID
                                      ,M.UNIT_TP
                                      ,M.SUPPLIER_ID
                                      ,FN_SKU_NAME(M.COMPANY_ID,M.SKU_ID) SKU_NAME
                                      ,M.MRP
									  FROM TT_FG_RECEIVING_FROM_OTHERS M  Where M.RECEIVE_ID = :param1";

        string AddOrUpdate_AddQuery() => @"INSERT INTO TT_FG_RECEIVING_FROM_OTHERS 
                                         (RECEIVE_ID, REMARKS, RECEIVE_STATUS, COMPANY_ID, EXPIRY_DATE, MFG_DATE, PACK_SIZE, RECEIVED_BY_ID, RECEIVE_AMOUNT, RECEIVE_DATE, RECEIVE_QTY, RECEIVE_STOCK_TYPE, SHIPPER_QTY, SKU_CODE, SKU_ID, SUPPLIER_ID, RECEIVE_TYPE, UNIT_ID, UNIT_TP, Entered_By, Entered_Date, ENTERED_TERMINAL, CHALLAN_NO, CHALLAN_DATE, BATCH_NO, MRP, BATCH_PRICE_REVIEW_STATUS) 
                                         VALUES (
                                            :param1,
                                            :param2,
                                            :param3,
                                            :param4,
                                            TO_DATE(:param5, 'DD/MM/YYYY'),
                                            TO_DATE(:param6, 'DD/MM/YYYY'),
                                            :param7,
                                            :param8,
                                            :param9,
                                            TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'),
                                            :param11,
                                            :param12,
                                            :param13,
                                            :param14,
                                            :param15,
                                            :param16,
                                            :param17,
                                            :param18,
                                            :param19,
                                            :param20,
                                            TO_DATE(:param21, 'DD/MM/YYYY HH:MI:SS AM'),
                                            :param22,
                                            :param23,
                                            TO_DATE(:param24, 'DD/MM/YYYY HH:MI:SS AM'),
                                            :param25,
                                            :param26,
                                            :param27
                                            )";
        string AddOrUpdate_UpdateQuery() => @"UPDATE TT_FG_RECEIVING_FROM_OTHERS SET 
                                             BATCH_ID = :param2
											,BATCH_NO = :param3
											,RECEIVE_STATUS = :param4
											,CHECKED_BY_ID = :param5
											,EXPIRY_DATE = TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM')
											,MFG_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM')
											,PACK_SIZE = :param8
											,RECEIVED_BY_ID = :param9
											,RECEIVE_AMOUNT = :param10
											,RECEIVE_DATE = TO_DATE(:param11, 'DD/MM/YYYY HH:MI:SS AM')
											,RECEIVE_QTY = :param12
											,RECEIVE_STOCK_TYPE = :param13
											,SHIPPER_QTY = :param14
											,SKU_CODE = :param15
											,SKU_ID = :param16
											,UNIT_ID = :param17
											,UNIT_TP = :param18
											,REMARKS = :param19
                                            ,UPDATED_BY = :param20
											,UPDATED_DATE = TO_DATE(:param21, 'DD/MM/YYYY HH:MI:SS AM')
                                            ,UPDATED_TERMINAL = :param22 
                                            ,CHECKED_BY_DATE =  TO_DATE(:param23, 'DD/MM/YYYY HH:MI:SS AM') 
                                            ,CHALLAN_NO = :param24
                                            ,CHALLAN_DATE = TO_DATE(:param25, 'DD/MM/YYYY HH:MI:SS AM')
                                            ,SUPPLIER_ID = :param26
                                            ,RECEIVE_TYPE = :param27
                                            ,MRP = :param28
                                            WHERE RECEIVE_ID = :param1";
        string GetNewDivision_Info_IdQuery() => "SELECT NVL(MAX(RECEIVE_ID), 0) + 1 RECEIVE_ID  FROM TT_FG_RECEIVING_FROM_Others";
        string Get_LastDivision_Ino() => "SELECT  RECEIVE_ID  FROM FG_RECEIVING_FROM_PRODUCTION Where  RECEIVE_ID = (SELECT   NVL(MAX(RECEIVE_ID),0) RECEIVE_ID From FG_RECEIVING_FROM_PRODUCTION where COMPANY_ID = :param1 )";
        string Update_Status_To_Cancel() => "UPDATE TT_FG_RECEIVING_FROM_OTHERS SET STATUS = 'Cancelled' WHERE RECEIVE_ID = :param1";
        string Get_Refurbishment_SKU_ALL_Query() => @"select DISTINCT ROWNUM ROW_NO, M.CLAIM_NO CHALLAN_NO, P.SKU_ID, D.PRODUCT_CODE SKU_CODE, P.SKU_NAME, D.PROD_QTY,D.AMOUNT
from REFURBISHMENT_FINALIZE_MST M, REFURBISHMENT_FINALIZE_RECV D, PRODUCT_INFO P
WHERE M.MST_SLNO = D.MST_SLNO AND  P.SKU_CODE = D.PRODUCT_CODE AND
D.PRODUCT_CODE NOT IN (SELECT K.SKU_CODE  FROM FG_RECEIVING_FROM_OTHERS K WHERE K.CHALLAN_NO = M.CLAIM_NO AND K.SKU_CODE = D.PRODUCT_CODE ) 
AND M.APPROVED_STATUS = 'A' ";
        string Get_Refurbishment_SKU_Query() => @"select DISTINCT ROWNUM ROW_NO, M.CLAIM_NO CHALLAN_NO, P.SKU_ID, D.PRODUCT_CODE SKU_CODE, P.SKU_NAME, D.PROD_QTY,D.AMOUNT
from REFURBISHMENT_FINALIZE_MST M, REFURBISHMENT_FINALIZE_RECV D, PRODUCT_INFO P
WHERE M.MST_SLNO = D.MST_SLNO AND  P.SKU_CODE = D.PRODUCT_CODE AND
D.PRODUCT_CODE NOT IN (SELECT K.SKU_CODE  FROM FG_RECEIVING_FROM_OTHERS K WHERE K.CHALLAN_NO = M.CLAIM_NO AND K.SKU_CODE = D.PRODUCT_CODE ) 
AND M.APPROVED_STATUS = 'A'  AND  M.CLAIM_NO = :param1";
        //---------- Method Execution Part ------------------------------------------------
        public async Task<string> GetApprovedList(string db, string db_security, int Company_Id, int Unit_Id)
        {
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetApprovedList(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Unit_Id.ToString() }));

            DataTable user_dataTable = _userManager.GetUsersByCompanyDataTable(db_security, Company_Id);
            List<User_Info> users = new List<User_Info>();
            for (int i = 0; i < user_dataTable.Rows.Count; i++)
            {
                User_Info user_ = new User_Info();
                user_.USER_ID = Convert.ToInt32(user_dataTable.Rows[i]["USER_ID"]);
                user_.USER_NAME = Convert.ToString(user_dataTable.Rows[i]["USER_NAME"]);
                users.Add(user_);
            }
            var fg_Reciving_From_Others = data.ToList<Fg_Reciving_From_Others>();

            foreach (var item in fg_Reciving_From_Others)
            {
                item.RECEIVE_ID_ENCRYPTED = _commonServices.Encrypt(item.RECEIVE_ID.ToString());
                item.RECEIVED_BY_NAME = item.RECEIVED_BY_ID > 0 ? users.Find(x => x.USER_ID == item.RECEIVED_BY_ID).USER_NAME : "";
                item.CHECKED_BY_NAME = item.CHECKED_BY_ID > 0 ? users.Find(x => x.USER_ID == item.CHECKED_BY_ID).USER_NAME : "";
            }

            return JsonSerializer.Serialize(fg_Reciving_From_Others);
        }
         public async Task<string> LoadUnchekedData(string db, string db_security, int Company_Id, int Unit_Id, string DATE_FROM="01/01/0001", string DATE_TO= "01/01/4023")
        {
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadUncheckData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Unit_Id.ToString(), DATE_FROM, DATE_TO }));

            DataTable user_dataTable = _userManager.GetUsersByCompanyDataTable(db_security, Company_Id);
            List<User_Info> users = new List<User_Info>();
            for (int i = 0; i < user_dataTable.Rows.Count; i++)
            {
                User_Info user_ = new User_Info();
                user_.USER_ID = Convert.ToInt32(user_dataTable.Rows[i]["USER_ID"]);
                user_.USER_NAME = Convert.ToString(user_dataTable.Rows[i]["USER_NAME"]);
                users.Add(user_);
            }
            var fg_Reciving_From_Others = data.ToList<Fg_Reciving_From_Others>();

            foreach (var item in fg_Reciving_From_Others)
            {
                item.RECEIVE_ID_ENCRYPTED = _commonServices.Encrypt(item.RECEIVE_ID.ToString());
                item.RECEIVED_BY_NAME = item.RECEIVED_BY_ID > 0 ? users.Find(x => x.USER_ID == item.RECEIVED_BY_ID).USER_NAME : "";
                item.CHECKED_BY_NAME = item.CHECKED_BY_ID > 0 ? users.Find(x => x.USER_ID == item.CHECKED_BY_ID).USER_NAME : "";
            }

            return JsonSerializer.Serialize(fg_Reciving_From_Others);
        }

        public async Task<string> LoadData(string db, string db_security, int Company_Id, int Unit_Id, string date_from, string date_to)
        {
            List<Fg_Reciving_From_Others> _From_Productions = new List<Fg_Reciving_From_Others>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Unit_Id.ToString(), date_from, date_to }));
            DataTable user_dataTable = _userManager.GetUsersByCompanyDataTable(db_security, Company_Id);
            List<User_Info> users = new List<User_Info>();
            for (int i = 0; i < user_dataTable.Rows.Count; i++)
            {
                User_Info user_ = new User_Info();
                user_.USER_ID = Convert.ToInt32(user_dataTable.Rows[i]["USER_ID"]);
                user_.USER_NAME = Convert.ToString(user_dataTable.Rows[i]["USER_NAME"]);
                users.Add(user_);
            }

            List<Fg_Reciving_From_Others> fg_Reciving_From_Other = new List<Fg_Reciving_From_Others>();
            fg_Reciving_From_Other = data.ToList<Fg_Reciving_From_Others>();
            foreach (var item in fg_Reciving_From_Other)
            {

                item.RECEIVED_BY_NAME = item.RECEIVED_BY_ID > 0 ? users.Find(x => x.USER_ID == item.RECEIVED_BY_ID).USER_NAME : "";
                item.CHECKED_BY_NAME = item.CHECKED_BY_ID > 0 ? users.Find(x => x.USER_ID == item.CHECKED_BY_ID).USER_NAME : "";
                item.RECEIVE_ID_ENCRYPTED = _commonServices.Encrypt(item.RECEIVE_ID.ToString());

            }
            return JsonSerializer.Serialize(fg_Reciving_From_Other);

        }
        public async Task<string> LoadFGTransferData(string db, int Company_Id, int UnitId) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadFGReceiveData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), UnitId.ToString() })));
        public async Task<string> Get_Refurbishment_SKU(string db, string Ref_Code) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_Refurbishment_SKU_Query(), _commonServices.AddParameter(new string[] {  Ref_Code })));
        public async Task<string> Get_Refurbishment_SKU_ALL(string db) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_Refurbishment_SKU_ALL_Query(), _commonServices.AddParameter(new string[] { })));

        
        public async Task<string> GetBatches(string db, int Company_Id, int sku_id, string RECEIVE_TYPE)
        {
            var query = @"SELECT BATCH_NO, TP, MRP FROM VW_OLP_TRANSFER_NOTE
                where COMPANY_ID = :param1 and SKU_ID = :param2 and PLANT_NAME = :param3";
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { Company_Id.ToString(), sku_id.ToString(), RECEIVE_TYPE })));
        }

        public async Task<string> AddOrUpdate(string db, Fg_Reciving_From_Others model)
        {
            if (model == null)
            {
                return "No data provided to insert!!!!";
            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();

                bool isDiffPrice = false;
                var dt = _commonServices.GetDataTable(_configuration.GetConnectionString(db), @"SELECT O.BATCH_NO, O.SKU_CODE, O.PACK_SIZE,O.MRP, O.UNIT_TP  FROM TT_FG_RECEIVING_FROM_OTHERS O 
                    WHERE O.BATCH_NO=:param1 AND O.SKU_ID = :param2 AND CHECKED_BY_ID IS NOT NULL", _commonServices.AddParameter(new string[] { model.BATCH_NO, model.SKU_ID.ToString() }));

               // bool isDiffPrice = false;
                //var dt = _commonServices.GetDataTable(_configuration.GetConnectionString(db), @"SELECT O.BATCH_NO, O.SKU_CODE, O.PACK_SIZE,O.MRP, O.UNIT_TP  FROM FG_RECEIVING_FROM_OTHERS O WHERE O.BATCH_NO=:param1 AND O.SKU_CODE=:param2", _commonServices.AddParameter(new string[] { model.BATCH_NO, model.SKU_CODE }));
                //foreach (DataRow row in dt.Rows)
                foreach (DataRow row in dt.Rows)
                {

                    if (Convert.ToDecimal(row["MRP"]) != model.MRP || Convert.ToDecimal(row["UNIT_TP"]) != model.UNIT_TP)

                    {
                        isDiffPrice = true;
                    }
                }
                if (isDiffPrice)
                {
                    return "0";
                }

                if (model.RECEIVE_ID == 0)
                {
                    //model.DIVISION_CODE = await GenerateDivisionCode(db, model.COMPANY_ID.ToString());
                    model.RECEIVE_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewDivision_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));

                    listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                        _commonServices.AddParameter(new string[] {
                                model.RECEIVE_ID.ToString(),
                                model.REMARKS.ToString(),
                                model.RECEIVE_STATUS,
                                model.COMPANY_ID.ToString(),
                                model.EXPIRY_DATE,
                                model.MFG_DATE,
                                model.PACK_SIZE,
                                model.RECEIVED_BY_ID.ToString(),
                                model.RECEIVE_AMOUNT.ToString(),
                                model.RECEIVE_DATE,
                                model.RECEIVE_QTY.ToString(),
                                model.RECEIVE_STOCK_TYPE,
                                model.SHIPPER_QTY.ToString(),
                                model.SKU_CODE,
                                model.SKU_ID.ToString(),
                                model.SUPPLIER_ID.ToString(),
                                model.RECEIVE_TYPE,
                                model.UNIT_ID.ToString(),
                                model.UNIT_TP.ToString(),
                                model.ENTERED_BY,
                                model.ENTERED_DATE,
                                model.ENTERED_TERMINAL,
                                model.CHALLAN_NO.ToString(),
                                model.CHALLAN_DATE,
                                model.BATCH_NO,
                                model.MRP.ToString(),
                                model.BATCH_PRICE_REVIEW_STATUS
                        })));
                }
                else
                {
                    var param = _commonServices.AddParameter(new string[]
                            {
                                model.RECEIVE_ID.ToString(),
                                model.BATCH_ID.ToString(),
                                model.BATCH_NO,
                                model.RECEIVE_STATUS ,
                                model.CHECKED_BY_ID.ToString(),
                                model.EXPIRY_DATE,
                                model.MFG_DATE,
                                model.PACK_SIZE,
                                model.RECEIVED_BY_ID.ToString(),
                                model.RECEIVE_AMOUNT.ToString(),
                                model.RECEIVE_DATE,
                                model.RECEIVE_QTY.ToString(),
                                model.RECEIVE_STOCK_TYPE,
                                model.SHIPPER_QTY.ToString(),
                                model.SKU_CODE,
                                model.SKU_ID.ToString(),
                                model.UNIT_ID.ToString(),
                                model.UNIT_TP.ToString(),
                                model.REMARKS,
                                model.UPDATED_BY?.ToString(),
                                model.UPDATED_DATE,
                                model.UPDATED_TERMINAL,
                                model.CHECKED_BY_DATE,
                                model.CHALLAN_NO,
                                model.CHALLAN_DATE,
                                model.SUPPLIER_ID.ToString(),
                                model.RECEIVE_TYPE,
                                model.MRP.ToString()
                            });
                    listOfQuery.Add(_commonServices.AddQuery(
                        AddOrUpdate_UpdateQuery(),
                        param));

                }
                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                return _commonServices.Encrypt(model.RECEIVE_ID.ToString());
            }
        }

        public async Task<string> LoadDetailDataById(string db, int Id)
        {
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadEditData_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));
        }

        


         public async Task<string> UpdateStatusToCancel(string db, string id)
        {
          
                List<QueryPattern> listOfQuery = new List<QueryPattern>();

              
                    listOfQuery.Add(_commonServices.AddQuery(Update_Status_To_Cancel(),
                        _commonServices.AddParameter(new string[] {
                               id
                        })));
                
               
                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                return "1";
           
        }
    }
}
