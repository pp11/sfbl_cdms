using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public class SalesOrderRationingManager : ISalesOrderRationingManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        private readonly IUserLogManager _logManager;
        private readonly IOrderAdjustmentManager _AdjustmentManager;

        public SalesOrderRationingManager(ICommonServices commonServices, IConfiguration configuration, IUserLogManager logManager, IOrderAdjustmentManager adjustmentManager)
        {
            _commonServices = commonServices;
            _configuration = configuration;
            _logManager = logManager;
            _AdjustmentManager = adjustmentManager;
        }

        //-------------------Query Part ---------------------------------------------------

        string LoadRationingData_Query() => @"
begin  :param1 := FN_ORDER_DETAILS_DATA( TO_DATE(:param2,'dd/MM/yyyy'),TO_DATE(:param3,'dd/MM/yyyy'), 
:param4,:param5  ,:param6, :param7,
:param8,
:param9 ,
:param10
);end;                            ";

        //PRC_UPDATE_ORDER(vORDER_MST_ID, vSKU_ID NUMBER, vORDER_QTY NUMBER, vCOMPANY_ID NUMBER, vUNIT_ID NUMBER );

        string SaveRationing_Query() => @" begin
                           PRC_UPDATE_ORDER(:param1, :param2, :param3, :param4, :param5 );
                      end;";


        string Select_OrderQuery() => @"Select 
                                                          NVL(ADJUSTMENT_AMOUNT,0) ADJUSTMENT_AMOUNT , NET_ORDER_AMOUNT                                                
                                                          FROM ORDER_MST
                                                          WHERE ORDER_MST_ID = :param1";

        string AddOrUpdate_AddQuery() => @"INSERT INTO ORDER_ADJUSTMENT 
                                         (ID, ORDER_MST_ID, ORDER_NO, ADJUSTMENT_ID, ADJUSTMENT_AMOUNT, ORDER_UNIT_ID, REMARKS, COMPANY_ID,  Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8,:param9, TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), :param11 )";
        string UpdateOrderAdjQuery() => @"UPDATE ORDER_MST  SET 
                                                          ADJUSTMENT_AMOUNT = :param2,                                                         
                                                          NET_ORDER_AMOUNT = :param3
                                                          WHERE ORDER_MST_ID = :param1";
        string Get_NewOrderAdjustmentId_Query() => "SELECT NVL(MAX(ID),0) + 1 AREA_ID  FROM ORDER_ADJUSTMENT";

        //---------- Method Execution Part ------------------------------------------------


        public async Task<string> AddOrUpdate(string db, List<SalesOrderRationing> model)
        {
            if (model == null)
            {
                return "No data provided to insert!!!!";
            }
            else
            {


                try
                {
                    bool status_action = true;
                    string dtl_list = "";
                    List<QueryPattern> listOfQuery = new List<QueryPattern>();
                    List<SalesOrderRationing> adjAbleOrders = model.GroupBy(d => new { d.ORDER_NO, d.ADJ_AMT, d.ADJ_TYPE }).Select(grp => grp.First()).ToList();



                    for (int i = 0; i < model.Count; i++)
                    {
                        listOfQuery.Add(_commonServices.AddQuery(SaveRationing_Query(),
                          _commonServices.AddParameter(new string[] {
                                model[i].ORDER_MST_ID.ToString(), model[i].SKU_ID.ToString(), model[i].REVISED_QTY.ToString(), model[i].COMPANY_ID.ToString(),
                                model[i].UNIT_ID.ToString()
                          })));
                        dtl_list += ",MST:" + model[0].ORDER_MST_ID + "SKU:" + model[0].SKU_ID;


                    }
                    int id = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), Get_NewOrderAdjustmentId_Query(), _commonServices.AddParameter(new string[] { }));
                    decimal adjustment_amnt = 0, adjustment_Oamnt = 0;

                    foreach (var item in adjAbleOrders.Where(x=>x.ADJ_TYPE!=0 && x.ADJ_AMT>0))
                    {
                        DataTable dtable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Select_OrderQuery(),_commonServices.AddParameter(new string[] { item.ORDER_MST_ID.ToString() }));
                        if (dtable.Rows.Count > 0)
                        {
                            adjustment_Oamnt = Convert.ToDecimal(dtable.Rows[0]["NET_ORDER_AMOUNT"].ToString()) - item.ADJ_AMT;
                            adjustment_amnt = dtable.Rows[0]["ADJUSTMENT_AMOUNT"].ToString() == "" ? 0 + item.ADJ_AMT : Convert.ToDecimal(dtable.Rows[0]["ADJUSTMENT_AMOUNT"].ToString()) + item.ADJ_AMT;

                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                             _commonServices.AddParameter(new string[] {
                               id.ToString(), item.ORDER_MST_ID.ToString(), item.ORDER_NO, item.ADJ_TYPE.ToString() ,
                                item.ADJ_AMT.ToString() , item.UNIT_ID.ToString(),"",
                                item.COMPANY_ID.ToString(), item.ENTERED_BY.ToString(), item.ENTERED_DATE, item.ENTERED_TERMINAL })));

                            listOfQuery.Add(_commonServices.AddQuery(UpdateOrderAdjQuery(),
                                                     _commonServices.AddParameter(new string[] {
                                 item.ORDER_MST_ID.ToString(), adjustment_amnt.ToString() , adjustment_Oamnt.ToString() })));
                            id++;

                        }
                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);



                    string st = status_action == false ? "Add" : "Edit";
                    await _logManager.AddOrUpdate(model[0].db_security, st, "ORDER_MST", model[0].COMPANY_ID, model[0].UNIT_ID, Convert.ToInt32(model[0].ENTERED_BY), model[0].ENTERED_TERMINAL, "/SalesAndDistribution/SalesOrderRationing/InsertOrEdit", 0, dtl_list);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }


        public async Task<string> LoadRationingData(OrderSKUFilterParameters model)
        {
            await _logManager.AddOrUpdate(model.db_security, "Retioning View", "ORDER_MST/DTL", model.COMPANY_ID, model.UNIT_ID, Convert.ToInt32(model.ENTERED_BY), model.ENTERED_TERMINAL, "/SalesAndDistribution/Division/DivisionInfo", 0, "");
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(model.db_sales), LoadRationingData_Query(), _commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), model.DATE_FROM, model.DATE_TO, model.DIVISION_ID, model.REGION_ID, model.AREA_ID, model.TERRITORY_ID, model.CUSTOMER_ID, model.UNIT_ID.ToString(), model.COMPANY_ID.ToString() })));
        }
    }
}
