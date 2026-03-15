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
    public class FgReceiveFromProductionManager : IFgReceiveFromProductionManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        private readonly IUserManager _userManager;

        public FgReceiveFromProductionManager(ICommonServices commonServices, IConfiguration configuration, IUserManager userManager)
        {
            _commonServices = commonServices;
            _configuration = configuration;
            _userManager = userManager;
        }

        //-------------------Query Part ---------------------------------------------------
        string LoadFGReceiveData_Query(int unitId)
        {
            var sql = @"SELECT 
                ROW_NUMBER() OVER(ORDER BY M.TRANSFER_NOTE_NO ASC) AS ROW_NO,
                M.COMPANY_ID,
                M.UNIT_ID,
                M.UNIT_NAME,
                M.TRANSFER_NOTE_NO,
                M.TRANSFER_DATE,
                M.BATCH_NO,
                M.SKU_ID,
                P.SKU_CODE,
                P.SKU_NAME,
                P.PACK_SIZE,
                TRANSFER_QTY,
                MRP,
                TP UNIT_TP,
                MFG_DATE,
                EXP_DATE EXPIRE_DATE
            FROM";
            
            if(unitId == 1) 
            {
                sql += " VW_OLP_TRANSFER_NOTE_PSTL M";
            }
            else if(unitId == 2)
            {
                sql += " VW_OLP_TRANSFER_NOTE_RSTL_SND M";
            }
            //else if(unitId == 4)
            //{
            //    sql += " VW_OLP_TRANSFER_NOTE_SHHPL M";
            //}
            //else
            //{
            //    sql += " VW_OLP_TRANSFER_NOTE M";
            //}

            sql += @" LEFT outer join PRODUCT_INFO P on P.SKU_CODE = M.SKU_CODE
            where M.COMPANY_ID = :param1 and M.UNIT_ID = :param2
            and M.TRANSFER_NOTE_NO  not in (Select TRANSFER_NOTE_NO 
            from FG_RECEIVING_FROM_PRODUCTION 
            where M.COMPANY_ID = :param1 and M.UNIT_ID = :param2)";

            return sql;
        }


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
                                      ,TO_CHAR(M.TRANSFER_DATE, 'YYYY-MM-DD') TRANSFER_DATE
                                      ,M.TRANSFER_NOTE_NO
                                      ,M.TRANSFER_TYPE
                                      ,M.UNIT_ID
                                      ,M.UNIT_TP     
                                      ,M.RECEIVE_STATUS        
                                      FROM FG_RECEIVING_FROM_PRODUCTION  M
LEFT OUTER JOIN PRODUCT_INFO P on P.SKU_ID = M.SKU_ID
Where M.COMPANY_ID = :param1 AND M.UNIT_ID = :param2 AND trunc(M.ENTERED_DATE ) BETWEEN TO_DATE(:param3, 'DD/MM/YYYY') AND TO_DATE(:param4, 'DD/MM/YYYY')";
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
                                      ,TO_CHAR(M.TRANSFER_DATE, 'YYYY-MM-DD') TRANSFER_DATE
                                      ,M.TRANSFER_NOTE_NO
                                      ,M.TRANSFER_TYPE
                                      ,M.UNIT_ID
                                      ,M.UNIT_TP                                 
                                      FROM FG_RECEIVING_FROM_PRODUCTION  M
LEFT OUTER JOIN PRODUCT_INFO P on P.SKU_ID = M.SKU_ID
Where M.COMPANY_ID = :param1 AND M.UNIT_ID = :param2 AND  ( M.CHECKED_BY_ID is  null or M.CHECKED_BY_ID  =0 )";
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
									  ,TO_CHAR(M.TRANSFER_DATE, 'YYYY-MM-DD') TRANSFER_DATE
                                      ,M.TRANSFER_NOTE_NO
                                      ,M.TRANSFER_TYPE
                                      ,M.UNIT_ID
                                      ,M.UNIT_TP
                                      ,M.MRP
									  FROM FG_RECEIVING_FROM_PRODUCTION  M  Where M.RECEIVE_ID = :param1";

        string AddOrUpdate_AddQuery() => @"INSERT INTO FG_RECEIVING_FROM_PRODUCTION 
                                         (RECEIVE_ID, BATCH_ID, BATCH_NO, REMARKS,RECEIVE_STATUS, COMPANY_ID,EXPIRY_DATE, MFG_DATE,PACK_SIZE,RECEIVED_BY_ID,RECEIVE_AMOUNT,RECEIVE_DATE,RECEIVE_QTY,RECEIVE_STOCK_TYPE,SHIPPER_QTY,SKU_CODE,SKU_ID,TRANSFER_DATE,TRANSFER_NOTE_NO,TRANSFER_TYPE,UNIT_ID,UNIT_TP , Entered_By,Entered_Date,ENTERED_TERMINAL, TRANSFER_QTY, MRP, BATCH_PRICE_REVIEW_STATUS ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5,  :param6,TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), TO_DATE(:param8, 'DD/MM/YYYY HH:MI:SS AM'), :param9, :param10, :param11,TO_DATE(:param12, 'DD/MM/YYYY HH:MI:SS AM'), :param13, :param14, :param15, :param16, :param17,TO_DATE(:param18, 'DD/MM/YYYY HH:MI:SS AM'),:param19,:param20,:param21,:param22,:param23,TO_DATE(:param24, 'DD/MM/YYYY HH:MI:SS AM'),:param25, :param26, :param27, :param28 )";
        string AddOrUpdate_UpdateQuery() => @"UPDATE FG_RECEIVING_FROM_PRODUCTION SET 
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
											,TRANSFER_DATE = TO_DATE(:param17, 'DD/MM/YYYY HH:MI:SS AM')
											,TRANSFER_NOTE_NO = :param18
											,TRANSFER_TYPE = :param19
											,UNIT_ID = :param20
											,UNIT_TP = :param21
											,REMARKS = :param22
                                            ,UPDATED_BY = :param23
											,UPDATED_DATE = TO_DATE(:param24, 'DD/MM/YYYY HH:MI:SS AM')
                                            ,UPDATED_TERMINAL = :param25, CHECKED_BY_DATE =  TO_DATE(:param26, 'DD/MM/YYYY HH:MI:SS AM') 
                                            , TRANSFER_QTY = :param27
                                            , MRP = :param28
                                            WHERE RECEIVE_ID = :param1";
        string GetNewDivision_Info_IdQuery() => "SELECT NVL(MAX(RECEIVE_ID),0) + 1 RECEIVE_ID  FROM FG_RECEIVING_FROM_PRODUCTION";
        string Get_LastDivision_Ino() => "SELECT  RECEIVE_ID  FROM FG_RECEIVING_FROM_PRODUCTION Where  RECEIVE_ID = (SELECT   NVL(MAX(RECEIVE_ID),0) RECEIVE_ID From FG_RECEIVING_FROM_PRODUCTION)";


        //---------- Method Execution Part ------------------------------------------------
        public async Task<string> LoadUnchekedData(string db, string db_security, int Company_Id, int Unit_Id)
        {
            List<Fg_Reciving_From_Production> _From_Productions = new List<Fg_Reciving_From_Production>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadUncheckData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Unit_Id.ToString() }));
            DataTable user_dataTable = _userManager.GetUsersByCompanyDataTable(db_security, Company_Id);
            List<User_Info> users = new List<User_Info>();
            for (int i = 0; i < user_dataTable.Rows.Count; i++)
            {
                User_Info user_ = new User_Info();
                user_.USER_ID = Convert.ToInt32(user_dataTable.Rows[i]["USER_ID"]);
                user_.USER_NAME = Convert.ToString(user_dataTable.Rows[i]["USER_NAME"]);
                users.Add(user_);
            }
            for (int i = 0; i < data.Rows.Count; i++)
            {

                Fg_Reciving_From_Production fg_Reciving_From_Production = new Fg_Reciving_From_Production();
                fg_Reciving_From_Production.RECEIVE_ID = Convert.ToInt32(data.Rows[i]["RECEIVE_ID"]);
                fg_Reciving_From_Production.RECEIVE_ID_ENCRYPTED = _commonServices.Encrypt(fg_Reciving_From_Production.RECEIVE_ID.ToString());
                fg_Reciving_From_Production.TRANSFER_NOTE_NO = Convert.ToInt32(data.Rows[i]["TRANSFER_NOTE_NO"]).ToString(); ;
                fg_Reciving_From_Production.TRANSFER_DATE = Convert.ToString(data.Rows[i]["TRANSFER_DATE"]);

                fg_Reciving_From_Production.BATCH_ID = Convert.ToInt32(data.Rows[i]["BATCH_ID"]);
                fg_Reciving_From_Production.BATCH_NO = Convert.ToString(data.Rows[i]["BATCH_NO"]);
                fg_Reciving_From_Production.CHECKED_BY_DATE = Convert.ToString(data.Rows[i]["CHECKED_BY_DATE"]);
                fg_Reciving_From_Production.CHECKED_BY_ID = data.Rows[i]["CHECKED_BY_ID"] != null && data.Rows[i]["CHECKED_BY_ID"].ToString() != "" ? Convert.ToInt32(data.Rows[i]["CHECKED_BY_ID"]) : 0;
                fg_Reciving_From_Production.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                fg_Reciving_From_Production.EXPIRY_DATE = data.Rows[i]["EXPIRY_DATE"] != null && data.Rows[i]["EXPIRY_DATE"].ToString() != "" ? Convert.ToString(data.Rows[i]["EXPIRY_DATE"]) : ""; ;
                fg_Reciving_From_Production.MFG_DATE = data.Rows[i]["MFG_DATE"] != null && data.Rows[i]["MFG_DATE"].ToString() != "" ? Convert.ToString(data.Rows[i]["MFG_DATE"]) : "";
                fg_Reciving_From_Production.PACK_SIZE = Convert.ToString(data.Rows[i]["PACK_SIZE"]);
                fg_Reciving_From_Production.RECEIVED_BY_ID = data.Rows[i]["RECEIVED_BY_ID"] != null && data.Rows[i]["RECEIVED_BY_ID"].ToString() != "" && Convert.ToInt32(data.Rows[i]["RECEIVED_BY_ID"]) > 0 ? Convert.ToInt32(data.Rows[i]["RECEIVED_BY_ID"]) : 0;
                fg_Reciving_From_Production.RECEIVE_AMOUNT = Convert.ToInt32(data.Rows[i]["RECEIVE_AMOUNT"]);
                fg_Reciving_From_Production.RECEIVE_DATE = Convert.ToString(data.Rows[i]["RECEIVE_DATE"]);
                fg_Reciving_From_Production.RECEIVE_QTY = Convert.ToInt32(data.Rows[i]["RECEIVE_QTY"]);
                fg_Reciving_From_Production.RECEIVE_STATUS = Convert.ToString(data.Rows[i]["RECEIVE_STATUS"]);
                fg_Reciving_From_Production.RECEIVE_STOCK_TYPE = Convert.ToString(data.Rows[i]["RECEIVE_STOCK_TYPE"]);
                fg_Reciving_From_Production.REMARKS = Convert.ToString(data.Rows[i]["REMARKS"]);
                fg_Reciving_From_Production.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                fg_Reciving_From_Production.SHIPPER_QTY = Convert.ToInt32(data.Rows[i]["SHIPPER_QTY"]);
                fg_Reciving_From_Production.SKU_CODE = Convert.ToString(data.Rows[i]["SKU_CODE"]);
                fg_Reciving_From_Production.SKU_NAME = Convert.ToString(data.Rows[i]["SKU_NAME"]);

                fg_Reciving_From_Production.SKU_ID = Convert.ToInt32(data.Rows[i]["SKU_ID"]);
                fg_Reciving_From_Production.RECEIVED_BY_NAME = fg_Reciving_From_Production.CHECKED_BY_ID > 0 ? users.Find(x => x.USER_ID == fg_Reciving_From_Production.RECEIVED_BY_ID).USER_NAME : "";
                fg_Reciving_From_Production.CHECKED_BY_NAME = fg_Reciving_From_Production.CHECKED_BY_ID > 0 ? users.Find(x => x.USER_ID == fg_Reciving_From_Production.CHECKED_BY_ID).USER_NAME : "";
                _From_Productions.Add(fg_Reciving_From_Production);
            }
            return JsonSerializer.Serialize(_From_Productions);
        }

        public async Task<string> LoadData(string db, string db_security, int Company_Id, int Unit_Id, string date_from, string date_to)
        {
            List<Fg_Reciving_From_Production> _From_Productions = new List<Fg_Reciving_From_Production>();
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
            for (int i = 0; i < data.Rows.Count; i++)
            {

                Fg_Reciving_From_Production fg_Reciving_From_Production = new Fg_Reciving_From_Production();
                fg_Reciving_From_Production.RECEIVE_ID = Convert.ToInt32(data.Rows[i]["RECEIVE_ID"]);
                fg_Reciving_From_Production.RECEIVE_ID_ENCRYPTED = _commonServices.Encrypt(fg_Reciving_From_Production.RECEIVE_ID.ToString());
                fg_Reciving_From_Production.TRANSFER_NOTE_NO = data.Rows[i]["TRANSFER_NOTE_NO"].ToString();
                fg_Reciving_From_Production.TRANSFER_DATE = Convert.ToString(data.Rows[i]["TRANSFER_DATE"]);

                fg_Reciving_From_Production.BATCH_ID = Convert.ToInt32(data.Rows[i]["BATCH_ID"]);
                fg_Reciving_From_Production.BATCH_NO = Convert.ToString(data.Rows[i]["BATCH_NO"]);
                fg_Reciving_From_Production.CHECKED_BY_DATE = Convert.ToString(data.Rows[i]["CHECKED_BY_DATE"]);
                fg_Reciving_From_Production.CHECKED_BY_ID = data.Rows[i]["CHECKED_BY_ID"] != null && data.Rows[i]["CHECKED_BY_ID"].ToString() != "" ? Convert.ToInt32(data.Rows[i]["CHECKED_BY_ID"]) : 0;
                fg_Reciving_From_Production.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                fg_Reciving_From_Production.EXPIRY_DATE = data.Rows[i]["EXPIRY_DATE"] != null && data.Rows[i]["EXPIRY_DATE"].ToString() != "" ? Convert.ToString(data.Rows[i]["EXPIRY_DATE"]) : ""; ;
                fg_Reciving_From_Production.MFG_DATE = data.Rows[i]["MFG_DATE"] != null && data.Rows[i]["MFG_DATE"].ToString() != "" ? Convert.ToString(data.Rows[i]["MFG_DATE"]) : "";
                fg_Reciving_From_Production.PACK_SIZE = Convert.ToString(data.Rows[i]["PACK_SIZE"]);
                fg_Reciving_From_Production.RECEIVED_BY_ID = data.Rows[i]["RECEIVED_BY_ID"] != null && data.Rows[i]["RECEIVED_BY_ID"].ToString() != "" && Convert.ToInt32(data.Rows[i]["RECEIVED_BY_ID"]) > 0 ? Convert.ToInt32(data.Rows[i]["RECEIVED_BY_ID"]) : 0;
                fg_Reciving_From_Production.RECEIVE_AMOUNT = Convert.ToInt32(data.Rows[i]["RECEIVE_AMOUNT"]);
                fg_Reciving_From_Production.RECEIVE_DATE = Convert.ToString(data.Rows[i]["RECEIVE_DATE"]);
                fg_Reciving_From_Production.RECEIVE_QTY = Convert.ToInt32(data.Rows[i]["RECEIVE_QTY"]);
                fg_Reciving_From_Production.RECEIVE_STATUS = Convert.ToString(data.Rows[i]["RECEIVE_STATUS"]);
                fg_Reciving_From_Production.RECEIVE_STOCK_TYPE = Convert.ToString(data.Rows[i]["RECEIVE_STOCK_TYPE"]);
                fg_Reciving_From_Production.REMARKS = Convert.ToString(data.Rows[i]["REMARKS"]);
                fg_Reciving_From_Production.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                fg_Reciving_From_Production.SHIPPER_QTY = Convert.ToInt32(data.Rows[i]["SHIPPER_QTY"]);
                fg_Reciving_From_Production.SKU_CODE = Convert.ToString(data.Rows[i]["SKU_CODE"]);
                fg_Reciving_From_Production.SKU_NAME = Convert.ToString(data.Rows[i]["SKU_NAME"]);

                fg_Reciving_From_Production.SKU_ID = Convert.ToInt32(data.Rows[i]["SKU_ID"]);
                fg_Reciving_From_Production.RECEIVED_BY_NAME = fg_Reciving_From_Production.CHECKED_BY_ID > 0 ? users.Find(x => x.USER_ID == fg_Reciving_From_Production.RECEIVED_BY_ID).USER_NAME : "";
                fg_Reciving_From_Production.CHECKED_BY_NAME = fg_Reciving_From_Production.CHECKED_BY_ID > 0 ? users.Find(x => x.USER_ID == fg_Reciving_From_Production.CHECKED_BY_ID).USER_NAME : "";
                _From_Productions.Add(fg_Reciving_From_Production);
            }
            return JsonSerializer.Serialize(_From_Productions);

        }
        public async Task<string> LoadFGTransferData(string db, int Company_Id, int UnitId) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadFGReceiveData_Query(UnitId), _commonServices.AddParameter(new string[] { Company_Id.ToString(), UnitId.ToString() })));



        public async Task<string> AddOrUpdate(string db, Fg_Reciving_From_Production model)
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

                    if (model.RECEIVE_ID == 0)
                    {
                        //model.DIVISION_CODE = await GenerateDivisionCode(db, model.COMPANY_ID.ToString());
                        model.RECEIVE_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewDivision_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                            _commonServices.AddParameter(new string[] { model.RECEIVE_ID.ToString(),
                                model.BATCH_ID.ToString(), model.BATCH_NO,
                                model.REMARKS, model.RECEIVE_STATUS, model.COMPANY_ID.ToString(),
                                model.EXPIRY_DATE.ToString(),
                                model.MFG_DATE.ToString(), model.PACK_SIZE.ToString(),
                                model.RECEIVED_BY_ID.ToString(), model.RECEIVE_AMOUNT.ToString(),
                                model.RECEIVE_DATE.ToString(), model.RECEIVE_QTY.ToString(),
                                model.RECEIVE_STOCK_TYPE.ToString(), model.SHIPPER_QTY.ToString(),
                                model.SKU_CODE.ToString(), model.SKU_ID.ToString(),
                                model.TRANSFER_DATE.ToString(), model.TRANSFER_NOTE_NO.ToString(),
                                model.TRANSFER_TYPE.ToString(), model.UNIT_ID.ToString(),
                                model.UNIT_TP.ToString(),model.ENTERED_BY.ToString(),
                                model.ENTERED_DATE, model.ENTERED_TERMINAL, model.TRANSFER_QTY.ToString(), 
                                model.MRP.ToString(), model.BATCH_PRICE_REVIEW_STATUS })));
                    }
                    else
                    {
                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.RECEIVE_ID.ToString(),model.BATCH_ID.ToString(), model.BATCH_NO,
                                model.RECEIVE_STATUS ,model.CHECKED_BY_ID.ToString(), model.EXPIRY_DATE,
                                model.MFG_DATE,  model.PACK_SIZE,  model.RECEIVED_BY_ID.ToString(),
                                model.RECEIVE_AMOUNT.ToString(),  model.RECEIVE_DATE,  model.RECEIVE_QTY.ToString(),
                                model.RECEIVE_STOCK_TYPE,model.SHIPPER_QTY.ToString(),  model.SKU_CODE,
                                model.SKU_ID.ToString(),model.TRANSFER_DATE,    model.TRANSFER_NOTE_NO.ToString(),  model.TRANSFER_TYPE.ToString(),
                                model.UNIT_ID.ToString(),  model.UNIT_TP.ToString(),  model.REMARKS.ToString(),
                                model.UPDATED_BY.ToString(), model.UPDATED_DATE,
                                model.UPDATED_TERMINAL,model.CHECKED_BY_DATE, model.TRANSFER_QTY.ToString(), model.MRP.ToString() })));

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

        public async Task<string> LoadDetailDataById(string db, int Id)
        {
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadEditData_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));
        }

        public async Task<string> GetLastMrp(string db, string sku_code, int company_id, int unit_id)
        {
            var query = @"SELECT MRP, UNIT_TP FROM VW_FG_RECEIVING
                WHERE SKU_CODE = :param1
                AND COMPANY_ID = :param2
                AND UNIT_ID = :param3
                AND RECEIVE_ID = (SELECT MAX(RECEIVE_ID) FROM VW_FG_RECEIVING 
                    WHERE SKU_CODE = :param1
                    AND COMPANY_ID = :param2
                    AND UNIT_ID = :param3)";
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { sku_code, company_id.ToString(), unit_id.ToString() })));
        }
    }
}
