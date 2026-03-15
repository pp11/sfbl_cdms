using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public class CustomerManager : ICustomerManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public CustomerManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"Select ROW_NUMBER() OVER(ORDER BY C.CUSTOMER_ID ASC) AS ROW_NO
                               ,C.BIN_NO
                               ,C.CLOSING_DATE
                               ,C.COMPANY_ID
                               ,C.CONTACT_PERSON_NAME
                               ,C.CONTACT_PERSON_NO
                               ,C.CUSTOMER_ADDRESS
                               ,C.CUSTOMER_ADDRESS_BANGLA
                               ,C.CUSTOMER_CODE
                               ,C.CUSTOMER_CONTACT
                               ,C.CUSTOMER_EMAIL
                               ,C.CUSTOMER_ID
                               ,C.CUSTOMER_NAME
                               ,C.CUSTOMER_NAME_BANGLA
                               ,C.CUSTOMER_REMARKS
                               ,C.CUSTOMER_STATUS
                               ,C.CUSTOMER_TYPE_ID
                               ,C.DB_LOCATION_ID
                               ,C.DB_LOCATION_NAME
                               ,C.DELIVERY_ADDRESS
                               ,C.DELIVERY_ADDRESS_BANGLA
                               ,C.ENTERED_BY
                               ,C.ENTERED_DATE
                               ,C.ENTERED_TERMINAL
                               ,C.OPENING_DATE
                               ,C.PRICE_TYPE_ID
                               ,C.PROPRIETOR_NAME
                               ,C.SECURITY_MONEY
                               ,C.TDS_FLAG
                               ,C.TIN_NO
                               ,C.TRADE_LICENSE_NO
                               ,C.UNIT_ID
                               ,C.UPDATED_BY
                               ,C.UPDATED_DATE
                               ,C.UPDATED_TERMINAL
                               ,C.VAT_REG_NO
                               ,C.DISTRIBUTOR_PRODUCT_TYPE 
                               ,NVL(C.SUGGEST_PERCENT,0) SUGGEST_PERCENT
                               ,NVL(C.MAXIMUM_PERCENT,0) MAXIMUM_PERCENT
                               ,NVL(C.MINIMUM_PERCENT,0) MINIMUM_PERCENT

                               from Customer_Info C
                               Where C.COMPANY_ID = :param1";
        string LoadActiveCustomerData_Query() => @"Select ROW_NUMBER() OVER(ORDER BY CUSTOMER_ID ASC) AS ROW_NO
                               ,BIN_NO
                               ,CLOSING_DATE
                               ,COMPANY_ID
                               ,CONTACT_PERSON_NAME
                               ,CONTACT_PERSON_NO
                               ,CUSTOMER_ADDRESS
                               ,CUSTOMER_ADDRESS_BANGLA
                               ,CUSTOMER_CODE
                               ,CUSTOMER_CONTACT
                               ,CUSTOMER_EMAIL
                               ,CUSTOMER_ID
                               ,CUSTOMER_NAME
                               ,CUSTOMER_NAME_BANGLA
                               ,CUSTOMER_REMARKS
                               ,CUSTOMER_STATUS
                               ,CUSTOMER_TYPE_ID
                               ,DB_LOCATION_ID
                               ,DB_LOCATION_NAME
                               ,DELIVERY_ADDRESS
                               ,DELIVERY_ADDRESS_BANGLA
                               ,ENTERED_BY
                               ,ENTERED_DATE
                               ,ENTERED_TERMINAL
                               ,OPENING_DATE
                               ,PRICE_TYPE_ID
                               ,PROPRIETOR_NAME
                               ,SECURITY_MONEY
                               ,TDS_FLAG
                               ,TIN_NO
                               ,TRADE_LICENSE_NO
                               ,UNIT_ID
                               ,ROUTE_ID
                               ,UPDATED_BY
                               ,UPDATED_DATE
                               ,UPDATED_TERMINAL
                               ,VAT_REG_NO
                               ,NVL(SUGGEST_PERCENT,0)
                               ,NVL(MAXIMUM_PERCENT,0)
                               ,NVL(MINIMUM_PERCENT,0)
                               from Customer_Info Where COMPANY_ID = :param1 and Customer_Status = 'Active'";
        string LoadCustomerDropdownData_Query() => @"SELECT CUSTOMER_ID,
       CUSTOMER_NAME,
       CUSTOMER_CODE,
       CUSTOMER_STATUS  
 FROM CUSTOMER_INFO
 WHERE COMPANY_ID = :param1";
        string LoadCustomerDataByType_Query() => @"Select I.CUSTOMER_ID, I.CUSTOMER_CODE, I.CUSTOMER_NAME  from CUSTOMER_INFO I Where COMPANY_ID = :param1 AND I.CUSTOMER_TYPE_ID= :param2 AND Customer_Status = 'Active'";
        string LoadSearchableData_Query() => @"SELECT CUSTOMER_CODE,
CUSTOMER_ID,
CUSTOMER_NAME
FROM Customer_Info
WHERE     ROWNUM <= 50
AND COMPANY_ID = :param1
AND CONCAT (UPPER (CUSTOMER_NAME), CUSTOMER_CODE) LIKE '%' || UPPER ( :param2) || '%'
ORDER BY CUSTOMER_NAME ASC";
        string AddOrUpdate_AddQuery() => @"INSERT INTO Customer_Info 
                                         (BIN_NO
                               ,CLOSING_DATE
                               ,COMPANY_ID
                               ,CONTACT_PERSON_NAME
                               ,CONTACT_PERSON_NO
                               ,CUSTOMER_ADDRESS
                               ,CUSTOMER_ADDRESS_BANGLA
                               ,CUSTOMER_CODE
                               ,CUSTOMER_CONTACT
                               ,CUSTOMER_EMAIL
                               ,CUSTOMER_ID
                               ,CUSTOMER_NAME
                               ,CUSTOMER_NAME_BANGLA
                               ,CUSTOMER_REMARKS
                               ,CUSTOMER_STATUS
                               ,CUSTOMER_TYPE_ID
                               ,DB_LOCATION_ID
                               ,DB_LOCATION_NAME
                               ,DELIVERY_ADDRESS
                               ,DELIVERY_ADDRESS_BANGLA
                               ,ENTERED_BY
                               ,ENTERED_DATE
                               ,ENTERED_TERMINAL
                               ,OPENING_DATE
                               ,PRICE_TYPE_ID
                               ,PROPRIETOR_NAME
                               ,SECURITY_MONEY
                               ,TDS_FLAG
                               ,TIN_NO
                               ,TRADE_LICENSE_NO
                               ,UNIT_ID
                               ,VAT_REG_NO
                               ,SUGGEST_PERCENT
                               ,MAXIMUM_PERCENT
                               ,MINIMUM_PERCENT
                                ) 
                                VALUES ( :param1, TO_DATE(:param2, 'DD/MM/YYYY HH:MI:SS AM'), :param3, :param4, :param5, :param6, :param7, :param8, :param9, :param10, :param11, :param12, :param13, :param14,:param15, :param16, :param17, :param18, :param19,:param20,:param21,TO_DATE(:param22, 'DD/MM/YYYY HH:MI:SS AM') , :param23,TO_DATE(:param24, 'DD/MM/YYYY HH:MI:SS AM'), :param25, :param26, :param27, :param28, :param29, :param30, :param31, :param32,:param33,:param34, :param35)";
        string AddOrUpdate_UpdateQuery() => @"UPDATE Customer_Info SET  
                                BIN_NO = :param1
                               ,CONTACT_PERSON_NAME = :param2
                               ,CONTACT_PERSON_NO = :param3
                               ,CUSTOMER_ADDRESS = :param4
                               ,CUSTOMER_ADDRESS_BANGLA = :param5
                               ,CUSTOMER_CONTACT = :param6
                               ,CUSTOMER_EMAIL = :param7
                               ,CUSTOMER_NAME = :param8
                               ,CUSTOMER_NAME_BANGLA = :param9
                               ,CUSTOMER_REMARKS = :param10
                               ,CUSTOMER_STATUS = :param11
                               ,CUSTOMER_TYPE_ID = :param12
                               ,DB_LOCATION_ID = :param13
                               ,DB_LOCATION_NAME = :param14
                               ,DELIVERY_ADDRESS = :param15
                               ,DELIVERY_ADDRESS_BANGLA = :param16
                               
                               ,PRICE_TYPE_ID = :param17
                               ,PROPRIETOR_NAME = :param18
                               ,SECURITY_MONEY = :param19
                               ,TDS_FLAG = :param20
                               ,TIN_NO = :param21
                               ,TRADE_LICENSE_NO = :param22
                               ,UNIT_ID = :param23
                               ,VAT_REG_NO = :param24
                               ,UPDATED_BY = :param25
                               ,UPDATED_DATE = TO_DATE(:param26, 'DD/MM/YYYY HH:MI:SS AM')
                                ,UPDATED_TERMINAL = :param27
                                ,SUGGEST_PERCENT= :param28
                                ,MAXIMUM_PERCENT= :param29
                                ,MINIMUM_PERCENT= :param30

                                WHERE CUSTOMER_ID = :param31";
        string AddOrUpdate_UpdateDist_ProductQuery() => @"UPDATE Customer_Info SET  
                                DISTRIBUTOR_PRODUCT_TYPE= :param1
                                WHERE CUSTOMER_ID = :param2";
        string GetNewCustomer_Info_IdQuery() => "SELECT NVL(MAX(CUSTOMER_ID),0) + 1 CUSTOMER_ID  FROM CUSTOMER_INFO";
        string Get_LastCustomer_Ino() => "SELECT  CUSTOMER_ID, CUSTOMER_CODE  FROM CUSTOMER_INFO Where  CUSTOMER_ID = (SELECT   NVL(MAX(CUSTOMER_ID),0) CUSTOMER_ID From CUSTOMER_INFO where COMPANY_ID = :param1 )";
        //---------- Method Execution Part ------------------------------------------------

        string Get_LastCrrId() => @"SELECT NVL(MAX(ID),0) ID  FROM CUSTOMER_ROUTE_RELATION";
        string Route_relation_Add_query() => @"INSERT INTO CUSTOMER_ROUTE_RELATION (ID, ROUTE_ID, CUSTOMER_ID, SL_NO) VALUES
            (:param1, :param2, :param3, :param4)";

        string Route_relation_Update_query() => @"UPDATE CUSTOMER_ROUTE_RELATION
            SET ROUTE_ID=:param2,
            SL_NO = :param3
            WHERE ID = :param1";
        string Get_route_relation_query() => @"SELECT * FROM CUSTOMER_ROUTE_RELATION WHERE CUSTOMER_ID = :param1";
        string DeleteRouteRelation_Query() => @"DELETE FROM CUSTOMER_ROUTE_RELATION WHERE ID = :param1";

        public async Task<string> AddOrUpdate(string db, Customer_Info model)
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
                    if (model.CUSTOMER_ID == 0)
                    {
                        model.CUSTOMER_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewCustomer_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));
                        model.DB_LOCATION_ID = model.CUSTOMER_ID;
                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                         _commonServices.AddParameter(new string[] { model.BIN_NO,
                             model.CLOSING_DATE.ToString(), model.COMPANY_ID.ToString(),model.CONTACT_PERSON_NAME,
                             model.CONTACT_PERSON_NO, model.CUSTOMER_ADDRESS, model.CUSTOMER_ADDRESS_BANGLA, model.CUSTOMER_CODE,
                             model.CUSTOMER_CONTACT, model.CUSTOMER_EMAIL, model.CUSTOMER_ID.ToString(),
                             model.CUSTOMER_NAME,  model.CUSTOMER_NAME_BANGLA,
                             model.CUSTOMER_REMARKS, model.CUSTOMER_STATUS, model.CUSTOMER_TYPE_ID.ToString(),
                             model.DB_LOCATION_ID.ToString(), model.DB_LOCATION_NAME, model.DELIVERY_ADDRESS,
                             model.DELIVERY_ADDRESS_BANGLA,model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL,  model.OPENING_DATE,
                             model.PRICE_TYPE_ID.ToString(), model.PROPRIETOR_NAME, model.SECURITY_MONEY.ToString(), model.TDS_FLAG == "Yes"? "1": "0",
                             model.TIN_NO,model.TRADE_LICENSE_NO,model.UNIT_ID.ToString(),model.VAT_REG_NO, /*model.DISTRIBUTOR_PRODUCT_TYPE,*/
                             model.SUGGEST_PERCENT.ToString(),
                             model.MAXIMUM_PERCENT.ToString(),
                             model.MINIMUM_PERCENT.ToString() 
                             })));


                        if (model.Customer_Route_Relations.Count > 0)
                        {
                            var id = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), Get_LastCrrId(), _commonServices.AddParameter(new string[] { }));

                            foreach (var dtl in model.Customer_Route_Relations)
                            {
                                listOfQuery.Add(_commonServices.AddQuery(Route_relation_Add_query(), _commonServices.AddParameter(new string[] {
                                    (++id).ToString(),
                                    dtl.ROUTE_ID.ToString(),
                                    model.CUSTOMER_ID.ToString(),
                                    dtl.SL_NO.ToString()
                                })));
                            }
                        }
                    }
                    else
                    {
                        listOfQuery.Add(_commonServices.AddQuery(

                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                             model.BIN_NO,
                             model.CONTACT_PERSON_NAME,
                             model.CONTACT_PERSON_NO, model.CUSTOMER_ADDRESS, model.CUSTOMER_ADDRESS_BANGLA,
                             model.CUSTOMER_CONTACT, model.CUSTOMER_EMAIL,
                             model.CUSTOMER_NAME,  model.CUSTOMER_NAME_BANGLA,
                             model.CUSTOMER_REMARKS, model.CUSTOMER_STATUS, model.CUSTOMER_TYPE_ID.ToString(),
                             model.DB_LOCATION_ID.ToString(), model.DB_LOCATION_NAME, model.DELIVERY_ADDRESS,
                             model.DELIVERY_ADDRESS_BANGLA, model.PRICE_TYPE_ID.ToString(), model.PROPRIETOR_NAME, model.SECURITY_MONEY.ToString(), model.TDS_FLAG.ToString(),
                             model.TIN_NO,model.TRADE_LICENSE_NO,model.UNIT_ID.ToString(),model.VAT_REG_NO, model.UPDATED_BY.ToString(), model.UPDATED_DATE,
                             model.UPDATED_TERMINAL,/*model.DISTRIBUTOR_PRODUCT_TYPE,*/
                             model.SUGGEST_PERCENT.ToString(),
                             model.MAXIMUM_PERCENT.ToString(),
                             model.MINIMUM_PERCENT.ToString() , model.CUSTOMER_ID.ToString()
                            })));

                        var id = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), Get_LastCrrId(), _commonServices.AddParameter(new string[] { }));

                        foreach (var dtl in model.Customer_Route_Relations)
                        {
                            if (dtl.ID == 0)
                            {
                                listOfQuery.Add(_commonServices.AddQuery(Route_relation_Add_query(), _commonServices.AddParameter(new string[] {
                                    (++id).ToString(),
                                    dtl.ROUTE_ID.ToString(),
                                    model.CUSTOMER_ID.ToString(),
                                    dtl.SL_NO.ToString()
                                })));
                            }
                            else
                            {
                                listOfQuery.Add(_commonServices.AddQuery(Route_relation_Update_query(), _commonServices.AddParameter(new string[] {
                                    (++id).ToString(),
                                    dtl.ROUTE_ID.ToString(),
                                    dtl.SL_NO.ToString()
                                })));
                            }
                        }

                        var dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_route_relation_query(),
                         _commonServices.AddParameter(new string[] { model.CUSTOMER_ID.ToString() }));
                        var oldData = dataTable.ToList<Customer_Route_Relation>();

                        foreach(var data in oldData)
                        {
                            if(!model.Customer_Route_Relations.Any(e => e.ID == data.ID))
                            {
                                //delete
                                listOfQuery.Add(_commonServices.AddQuery(DeleteRouteRelation_Query(), _commonServices.AddParameter(new string[] { data.ID.ToString() })));
                            }
                        }
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
        public async Task<string> AddOrUpdate_Dist_Product_Type (string db, Customer_Info model)
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
                   
                  
                        listOfQuery.Add(_commonServices.AddQuery(

                            AddOrUpdate_UpdateDist_ProductQuery(),
                            _commonServices.AddParameter(new string[] {
                            model.DISTRIBUTOR_PRODUCT_TYPE, model.CUSTOMER_ID.ToString()
                            })));


                       

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        public async Task<string> LoadData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> LoadActiveCustomerData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadActiveCustomerData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> LoadCustomerDropdownData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadCustomerDropdownData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> LoadCustomerDataByType(string db, int Company_Id, int customer_type_id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadCustomerDataByType_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), customer_type_id.ToString() })));
        public async Task<string> GetSearchableCustomer(string db, int Company_Id, string customer) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), customer })));
        public async Task<string> GenerateCustomerCode(string db, string Company_Id, string Company_Name)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastCustomer_Ino(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["CUSTOMER_CODE"].ToString().Substring(1, (CodeConstants.CustomerInfo_CodeLength - 1))) + 1).ToString();
                    int serial_length = serial.Length;
                    code = CodeConstants.CustomerInfo_CodeConst;
                    for (int i = 0; i < (CodeConstants.CustomerInfo_CodeLength - (serial_length + 1)); i++)
                    {
                        code += "0";
                    }
                    code += serial;
                }
                else
                {
                    code = CodeConstants.CustomerInfo_CodeConst + "00001";
                }
                return code;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetCustomerRoutes(string db, int customerId)
        {
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_route_relation_query(),
                         _commonServices.AddParameter(new string[] { customerId.ToString() })));
        }
    }
}




