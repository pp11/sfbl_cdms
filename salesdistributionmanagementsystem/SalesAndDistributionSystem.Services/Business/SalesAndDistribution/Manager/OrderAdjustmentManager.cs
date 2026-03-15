using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public class OrderAdjustmentManager : IOrderAdjustmentManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public OrderAdjustmentManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.ID ASC) AS ROW_NO,
                                    M.ID, A.ADJUSTMENT_NAME, M.COMPANY_ID, ID, M.ORDER_MST_ID, M.ORDER_NO, M.ADJUSTMENT_ID, M.ADJUSTMENT_AMOUNT,
                                    M.ORDER_UNIT_ID, M.REMARKS
                                    FROM ORDER_ADJUSTMENT  M
                                    LEFT OUTER JOIN ADJUSTMENT_INFO A on A.ADJUSTMENT_ID = M.ADJUSTMENT_ID
                                    Where M.COMPANY_ID = :param1";
        string LoadDataByOrderId_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.ID ASC) AS ROW_NO,
                                    M.ID, A.ADJUSTMENT_NAME, M.COMPANY_ID, ID, M.ORDER_MST_ID, M.ORDER_NO, M.ADJUSTMENT_ID, M.ADJUSTMENT_AMOUNT,
                                    M.ORDER_UNIT_ID, M.REMARKS, M.MONTH_NUMBER,M.YEAR_NUMBER
                                    FROM ORDER_ADJUSTMENT  M
                                    LEFT OUTER JOIN ADJUSTMENT_INFO A on A.ADJUSTMENT_ID = M.ADJUSTMENT_ID
                                    Where M.COMPANY_ID = :param1 And M.ORDER_MST_ID = :param2";
        string AddOrUpdate_AddQuery() => @"INSERT INTO ORDER_ADJUSTMENT 
                                         (ID, ORDER_MST_ID, ORDER_NO, ADJUSTMENT_ID, ADJUSTMENT_AMOUNT, ORDER_UNIT_ID, REMARKS, COMPANY_ID,  Entered_By, Entered_Date,ENTERED_TERMINAL ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8,:param9, TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), :param11 )";
        string AddOrUpdate_UpdateQuery() => @" UPDATE ORDER_ADJUSTMENT SET 
                                            ADJUSTMENT_ID = :param2,ADJUSTMENT_AMOUNT = :param3,REMARKS = :param4,
                                            UPDATED_BY = :param5, UPDATED_DATE = TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param7 WHERE ID = :param1";
        string Get_NewOrderAdjustmentId_Query() => "SELECT NVL(MAX(ID),0) + 1 AREA_ID  FROM ORDER_ADJUSTMENT";
        string GetAdjustQuery() => "SELECT ADJUSTMENT_AMOUNT  FROM ORDER_ADJUSTMENT WHERE ID = :param1";


        string UpdateOrderAdjQuery() => @"UPDATE ORDER_MST  SET 
                                                          ADJUSTMENT_AMOUNT = :param2,                                                         
                                                          NET_ORDER_AMOUNT = :param3
                                                          WHERE ORDER_MST_ID = :param1";
        string Select_OrderQuery() => @"Select 
                                                          NVL(ADJUSTMENT_AMOUNT,0) ADJUSTMENT_AMOUNT , NET_ORDER_AMOUNT                                                
                                                          FROM ORDER_MST
                                                          WHERE ORDER_MST_ID = :param1";


        //---------- Method Execution Part ------------------------------------------------



        public async Task<string> AddOrUpdate(string db, Order_Adjustment model)
        {
            Result result = new Result();
            if (model == null)
            {
                result.Status = "No data provided to insert!!!!";

            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {
                    DataTable dtable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db),
                               Select_OrderQuery(),
                                _commonServices.AddParameter(new string[] { model.ORDER_MST_ID.ToString() }));

                    decimal adjustment_amnt = 0, adjustment_Oamnt = 0;


                    if (dtable.Rows.Count > 0)
                    {
                        adjustment_Oamnt = Convert.ToDecimal(dtable.Rows[0]["NET_ORDER_AMOUNT"].ToString()) - model.ADJUSTMENT_AMOUNT;
                        adjustment_amnt = dtable.Rows[0]["ADJUSTMENT_AMOUNT"].ToString() == "" ? 0 + model.ADJUSTMENT_AMOUNT : Convert.ToDecimal(dtable.Rows[0]["ADJUSTMENT_AMOUNT"].ToString()) + model.ADJUSTMENT_AMOUNT;
                    }
                    if (model.ID == 0)
                    {
                        //model.AREA_CODE = await GenerateAreaCode(db, model.COMPANY_ID.ToString());

                        model.ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), Get_NewOrderAdjustmentId_Query(), _commonServices.AddParameter(new string[] { }));
                        if(model.ADJUSTMENT_ID != 4) //rem bonus
                        {
                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
    _commonServices.AddParameter(new string[] {
                                model.ID.ToString(), model.ORDER_MST_ID.ToString(), model.ORDER_NO, model.ADJUSTMENT_ID.ToString() ,
                                model.ADJUSTMENT_AMOUNT.ToString() , model.ORDER_UNIT_ID.ToString(), model.REMARKS,
                                model.COMPANY_ID.ToString(), model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));
                        }
                        else {
                            listOfQuery.Add(_commonServices.AddQuery(@"INSERT INTO ORDER_ADJUSTMENT 
                                         (ID, ORDER_MST_ID, ORDER_NO, ADJUSTMENT_ID, ADJUSTMENT_AMOUNT, ORDER_UNIT_ID, REMARKS, COMPANY_ID,  ENTERED_BY, ENTERED_DATE,ENTERED_TERMINAL,MONTH_NUMBER,YEAR_NUMBER ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8,:param9, TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), :param11, :param12, :param13 )", _commonServices.AddParameter(new string[] {
                                model.ID.ToString(), model.ORDER_MST_ID.ToString(), model.ORDER_NO, model.ADJUSTMENT_ID.ToString() ,
                                model.ADJUSTMENT_AMOUNT.ToString() , model.ORDER_UNIT_ID.ToString(), model.REMARKS,
                                model.COMPANY_ID.ToString(), model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL, model.MONTH_NUMBER, model.YEAR_NUMBER })));

                        }
                        listOfQuery.Add(_commonServices.AddQuery(UpdateOrderAdjQuery(),
                         _commonServices.AddParameter(new string[] {
                                 model.ORDER_MST_ID.ToString(), adjustment_amnt.ToString() , adjustment_Oamnt.ToString() })));

                    }
                    else
                    {
                        decimal dd = await _commonServices.GetMaximumNumberAsyn<decimal>(_configuration.GetConnectionString(db), GetAdjustQuery(), _commonServices.AddParameter(new string[] { model.ID.ToString() }));
                        listOfQuery.Add(_commonServices.AddQuery(UpdateOrderAdjQuery(),
                                          _commonServices.AddParameter(new string[] {
                                 model.ORDER_MST_ID.ToString(), (adjustment_amnt-dd).ToString() , (adjustment_Oamnt+dd).ToString() })));

                        if (model.ADJUSTMENT_ID !=4)//rem bonus
                        {
                            listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.ID.ToString(), model.ADJUSTMENT_ID.ToString(), model.ADJUSTMENT_AMOUNT.ToString(), model.REMARKS,
                                model.UPDATED_BY.ToString(), model.UPDATED_DATE,
                                model.UPDATED_TERMINAL })));
                        }
                        else
                        {
                            listOfQuery.Add(_commonServices.AddQuery(
                            @" UPDATE ORDER_ADJUSTMENT SET 
                                            ADJUSTMENT_ID = :param2,ADJUSTMENT_AMOUNT = :param3,REMARKS = :param4,
                                            UPDATED_BY = :param5, UPDATED_DATE = TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param7,MONTH_NUMBER=:param8,YEAR_NUMBER=:param9  WHERE ID = :param1",
                            _commonServices.AddParameter(new string[] {
                                model.ID.ToString(), model.ADJUSTMENT_ID.ToString(), model.ADJUSTMENT_AMOUNT.ToString(), model.REMARKS,
                                model.UPDATED_BY.ToString(), model.UPDATED_DATE,
                                model.UPDATED_TERMINAL, model.MONTH_NUMBER, model.YEAR_NUMBER })));
                        }
                        


                        
                    }

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    result.Key = model.ID.ToString();
                    result.Status = "1";
                    result.Parent = _commonServices.Encrypt(model.ORDER_MST_ID.ToString());
                }
                catch (Exception ex)
                {
                    result.Status = ex.Message;
                }
            }
            return JsonSerializer.Serialize(result);

        }
        public async Task<string> AddOrUpdateDebitCreditAdj(string db, DEBIT_CREDIT_ADJ model)
        {
            Result result = new Result();
            if (model == null)
            {
                result.Status = "No data provided to insert!!!!";

            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {
                    if (model.ID == 0)
                    {
                        model.ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), "SELECT NVL(MAX(ID),0) + 1 AREA_ID  FROM DEBIT_CREDIT_ADJ", _commonServices.AddParameter(new string[] { }));
                        model.ADJUSTMENT_NO = _commonServices.GetMaximumNumber<string>(_configuration.GetConnectionString(db), "SELECT NVL(MAX(ADJUSTMENT_NO),10000000) + 1 ADJUSTMENT_NO  FROM DEBIT_CREDIT_ADJ", _commonServices.AddParameter(new string[] { }));
                        listOfQuery.Add(_commonServices.AddQuery(@"INSERT INTO DEBIT_CREDIT_ADJ 
(ID, ADJUSTMENT_NO, CUSTOMER_ID, CUSTOMER_CODE, ADJUSTMENT_AMOUNT, COMPANY_ID, REMARKS, POSTING_STATUS, ENTERED_BY, ENTERED_DATE, ENTERED_TERMINAL) 
VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8,:param9, TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), :param11 )", _commonServices.AddParameter(new string[] {model.ID.ToString(), model.ADJUSTMENT_NO, model.CUSTOMER_ID.ToString(), model.CUSTOMER_CODE,model.ADJUSTMENT_AMOUNT.ToString() , model.COMPANY_ID.ToString(), model.REMARKS.ToString(),"NotApproved", model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL })));
                    }
                    else
                    {
                        listOfQuery.Add(_commonServices.AddQuery( @"UPDATE DEBIT_CREDIT_ADJ
                                                                    SET 
                                                                    ADJUSTMENT_NO = :param2,
                                                                    CUSTOMER_ID = :param3,
                                                                    CUSTOMER_CODE = :param4,
                                                                    ADJUSTMENT_AMOUNT = :param5,
                                                                    COMPANY_ID = :param6,
                                                                    REMARKS = :param7
                                                                    WHERE
                                                                    ID = :param1",_commonServices.AddParameter(new string[] {model.ID.ToString(), model.ADJUSTMENT_NO, model.CUSTOMER_ID.ToString(), model.CUSTOMER_CODE,model.ADJUSTMENT_AMOUNT.ToString() , model.COMPANY_ID.ToString(), model.REMARKS})));
                    }


                    DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT POSTING_STATUS FROM DEBIT_CREDIT_ADJ WHERE ID = :param1", _commonServices.AddParameter(new string[] { model.ID.ToString() }));
                    if (dataTable.Rows.Count > 0)
                    {
                        if (dataTable.Rows[0]["POSTING_STATUS"].ToString() == "Approved")
                        {
                            result.Status = "You cannot modify this adjustment because it has been posted, please reload";
                        }
                        else
                        {
                            await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                            result.Status = "1";
                        }
                    }
                    else
                    {
                        await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                        result.Status = "1";
                    }
                }
                catch (Exception ex)
                {
                    result.Status = ex.Message;
                }
            }
            return JsonSerializer.Serialize(result);

        }

        public async Task<string> PostDebitCreditAdj(string db, DEBIT_CREDIT_ADJ model)
        {
            Result result = new Result();
            if (model == null)
            {
                result.Status = "No data provided to insert!!!!";

            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {
                    DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT POSTING_STATUS FROM DEBIT_CREDIT_ADJ WHERE ID = :param1", _commonServices.AddParameter(new string[] { model.ID.ToString() }));
                    if (dataTable.Rows.Count > 0)
                    {
                        if (dataTable.Rows[0]["POSTING_STATUS"].ToString() == "Approved")
                        {
                            result.Status = "You cannot modify this adjustment because it has been posted, please reload";
                        }
                        else
                        {
                            listOfQuery.Add(_commonServices.AddQuery(@"     UPDATE DEBIT_CREDIT_ADJ
                                                                    SET 
                                                                    POSTING_STATUS = :param2,
                                                                    POSTED_BY = :param3,
                                                                    POSTED_DATE = TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'),
                                                                    POSTED_TERMINAL = :param5
                                                                    WHERE
                                                                    ID = :param1", _commonServices.AddParameter(new string[] { model.ID.ToString(), "Approved", model.POSTED_BY, model.POSTED_DATE.ToString(), model.POSTED_TERMINAL })));
                            await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                            result.Status = "1";
                        }
                    }
                    else
                    {
                        listOfQuery.Add(_commonServices.AddQuery(@"     UPDATE DEBIT_CREDIT_ADJ
                                                                    SET 
                                                                    POSTING_STATUS = :param2,
                                                                    POSTED_BY = :param3,
                                                                    POSTED_DATE = TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'),
                                                                    POSTED_TERMINAL = :param5
                                                                    WHERE
                                                                    ID = :param1", _commonServices.AddParameter(new string[] { model.ID.ToString(), "Approved", model.POSTED_BY, model.POSTED_DATE.ToString(), model.POSTED_TERMINAL })));
                        await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                        result.Status = "1";
                    }
                }
                catch (Exception ex)
                {
                    result.Status = ex.Message;
                }
            }
            return JsonSerializer.Serialize(result);

        }


        public async Task<string> LoadData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> LoadDebitCreditAdjData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT A.ID,
       A.ADJUSTMENT_NO,
       A.CUSTOMER_ID,
       A.CUSTOMER_CODE,
       A.ADJUSTMENT_AMOUNT,
       A.COMPANY_ID,
       A.REMARKS,
       A.POSTING_STATUS,
       A.ENTERED_BY,
       TO_CHAR(A.ENTERED_DATE,'DD/MM/YYYY') ENTERED_DATE,
       A.ENTERED_TERMINAL,
       A.POSTED_BY,
       TO_CHAR(A.POSTED_DATE,'DD/MM/YYYY') POSTED_DATE,
       A.POSTED_TERMINAL,
       C.CUSTOMER_NAME
 FROM DEBIT_CREDIT_ADJ A, CUSTOMER_INFO C
 WHERE A.CUSTOMER_ID=C.CUSTOMER_ID AND A.COMPANY_ID = :param1 ORDER BY A.ID DESC ", _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> LoadDataByOrderId(string db, int Company_Id, int Order_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadDataByOrderId_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Order_Id.ToString() })));

    }
}
