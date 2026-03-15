using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Utilities.Collections;
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
    public class GiftItemReceivingManager : IGiftItemReceivingManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public GiftItemReceivingManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT ROWNUM ROW_NO, M.RECEIVE_ID, TO_CHAR(RECEIVE_DATE,'dd/MM/yyyy') RECEIVE_DATE, 
M.SUPPLIER_ID, CHALLAN_NO, TO_CHAR(CHALLAN_DATE,'dd/MM/yyyy') CHALLAN_DATE, M.BATCH_ID, BATCH_NO, M.GIFT_ITEM_ID,
M.GIFT_ITEM_PRICE, RECEIVE_QTY, RECEIVE_AMOUNT, RECEIVE_STATUS, M.COMPANY_ID, 
M.UNIT_ID, M.RECEIVED_BY_ID, M.REMARKS,G.GIFT_ITEM_NAME,S.SUPPLIER_NAME,U.USER_NAME RECEIVED_BY_NAME
FROM  GIFT_ITEM_RECEIVING  M
LEFT OUTER JOIN GIFT_ITEM_INFO G on G.GIFT_ITEM_ID = M.GIFT_ITEM_ID
LEFT OUTER JOIN SUPPLIER_INFO S on S.SUPPLIER_ID = M.SUPPLIER_ID
LEFT OUTER JOIN USER_INFO U on U.USER_ID = M.RECEIVED_BY_ID
Where M.COMPANY_ID = :param1";

        string AddOrUpdate_AddQuery() => @"INSERT INTO GIFT_ITEM_RECEIVING 
            (RECEIVE_ID, RECEIVE_DATE, SUPPLIER_ID,CHALLAN_NO, CHALLAN_DATE,BATCH_ID, BATCH_NO,
                GIFT_ITEM_ID ,GIFT_ITEM_PRICE ,RECEIVE_QTY, RECEIVE_AMOUNT, RECEIVE_STATUS, COMPANY_ID,
                UNIT_ID,RECEIVED_BY_ID, REMARKS, Entered_By, Entered_Date,ENTERED_TERMINAL ) 
            VALUES ( :param1, TO_DATE(:param2, 'DD/MM/YYYY HH:MI:SS AM'), :param3, :param4, 
                TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'), :param6,:param7, :param8, :param9, :param10, :param11,
                :param12, :param13, :param14, :param15, :param16, :param17, 
                TO_DATE(:param18, 'DD/MM/YYYY HH:MI:SS AM'), :param19)";

        string AddOrUpdate_UpdateQuery() => @"UPDATE GIFT_ITEM_RECEIVING SET 
                                            RECEIVE_DATE = TO_DATE(:param2, 'DD/MM/YYYY HH:MI:SS AM'),
                                            SUPPLIER_ID = :param3,
                                            CHALLAN_NO = :param4,
                                            CHALLAN_DATE =TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),
                                            BATCH_ID =:param6,
                                            BATCH_NO =:param7,
                                            GIFT_ITEM_ID  =:param8,
                                            GIFT_ITEM_PRICE  =:param9,
                                            RECEIVE_QTY  =:param10,
                                            RECEIVE_AMOUNT  =:param11,
                                            RECEIVE_STATUS =:param12,
                                            RECEIVED_BY_ID =:param13,
                                            REMARKS = :param14,
                                            UPDATED_BY = :param15, UPDATED_DATE = TO_DATE(:param16, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param17 WHERE RECEIVE_ID = :param1";
        string GetNewGift_Item_Info_IdQuery() => "SELECT NVL(MAX(RECEIVE_ID),0) + 1 RECEIVE_ID  FROM GIFT_ITEM_RECEIVING";


        //---------- Method Execution Part ------------------------------------------------



        public async Task<string> AddOrUpdate(string db, Gift_Item_Receiving model)
        { 
            Result  result = new Result();
            if (model == null)
            {
                result.Status =  "No data provided to insert!!!!";
            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {
                    if (model.RECEIVE_ID == 0)
                    {
                        model.RECEIVE_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewGift_Item_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));
                        model.BATCH_ID = model.RECEIVE_ID;
                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.RECEIVE_ID.ToString(), model.RECEIVE_DATE, model.SUPPLIER_ID.ToString()
                                , model.CHALLAN_NO, model.CHALLAN_DATE.ToString(),model.BATCH_ID.ToString() ,
                                model.BATCH_NO.ToString(), model.GIFT_ITEM_ID.ToString(), model.GIFT_ITEM_PRICE.ToString()
                               , model.RECEIVE_QTY.ToString(), model.RECEIVE_AMOUNT.ToString(), model.RECEIVE_STATUS,
                                model.COMPANY_ID.ToString(), model.UNIT_ID.ToString(), model.RECEIVED_BY_ID.ToString()
                                ,model.REMARKS.ToString(),
                                model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));
                    }
                    else
                    {

                       

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.RECEIVE_ID.ToString(), model.RECEIVE_DATE, model.SUPPLIER_ID.ToString(),
                                model.CHALLAN_NO, model.CHALLAN_DATE,model.BATCH_ID.ToString(), model.BATCH_NO,
                                model.GIFT_ITEM_ID.ToString() ,model.GIFT_ITEM_PRICE.ToString() ,
                                model.RECEIVE_QTY.ToString(), model.RECEIVE_AMOUNT.ToString(),model.RECEIVE_STATUS,
                                model.RECEIVED_BY_ID.ToString(),model.REMARKS.ToString(),
                                           model.UPDATED_BY.ToString(), model.UPDATED_DATE,
                                           model.UPDATED_TERMINAL
                                 })));
                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);

                    var sql = LoadData_Query();
                    sql += " AND RECEIVE_ID = " + model.RECEIVE_ID.ToString();

                    var item = _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), sql, _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString() })));

                    result.Key = item;
                    result.Status = "1";
                }
                catch (Exception ex)
                {
                    result.Status =  ex.Message;
                }
            }
            return JsonSerializer.Serialize(result);
        }

        public async Task<string> LoadData(string db, int Company_Id, int unit_id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Query() + " AND M.UNIT_ID = :param2 ", _commonServices.AddParameter(new string[] { Company_Id.ToString(),unit_id.ToString() })));


    }
}




