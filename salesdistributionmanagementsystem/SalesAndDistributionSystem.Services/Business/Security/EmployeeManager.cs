using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Utilities.Collections;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Security
{
    public class EmployeeManager: IEmployeeManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public EmployeeManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }
        string loadDataQuery() => @"SELECT I.ID, I.EMPLOYEE_ID, I.EMPLOYEE_CODE, I.EMPLOYEE_NAME, I.EMPLOYEE_STATUS, I.COMPANY_ID, I.UNIT_ID, C.COMPANY_NAME, C.UNIT_NAME
FROM EMPLOYEE_INFO I , COMPANY_INFO C
WHERE C.UNIT_ID=I.UNIT_ID AND C.COMPANY_ID=I.COMPANY_ID ORDER BY I.ID DESC";
        string AddOrUpdate_AddQuery() => @"INSERT INTO EMPLOYEE_INFO (ID, EMPLOYEE_ID, EMPLOYEE_CODE, EMPLOYEE_NAME, EMPLOYEE_STATUS, COMPANY_ID, UNIT_ID)
VALUES(:param1 ,:param2  ,:param3  ,:param4,:param5  ,:param6,:param7)";
        string AddOrUpdate_UpdateQuery() => @"UPDATE EMPLOYEE_INFO
   SET EMPLOYEE_ID = :param1,
       EMPLOYEE_CODE = :param2,
       EMPLOYEE_NAME = :param3,
       EMPLOYEE_STATUS = :param4,
       COMPANY_ID = :param5,
       UNIT_ID = :param6
       WHERE ID=:param7";
        string LoadSearchableData_Query() => @"SELECT CUSTOMER_CODE,
CUSTOMER_ID,
CUSTOMER_NAME
FROM Customer_Info
WHERE     ROWNUM <= 50
AND COMPANY_ID = :param1 AND CUSTOMER_TYPE_ID=1
AND CONCAT (UPPER (CUSTOMER_NAME), CUSTOMER_CODE) LIKE '%' || UPPER ( :param2) || '%'
ORDER BY CUSTOMER_NAME ASC";
        string ActivateMenuQuery() => "UPDATE Menu_Configuration SET  Status = '"+Status.Active+"' WHERE Menu_Id = :param1";
        string DeactivateMenuQuery() => "UPDATE Menu_Configuration SET  Status = '" + Status.InActive + "' WHERE Menu_Id = :param1";
        string IsParentMenuQuery() => "SELECT MENU_ID FROM MENU_CONFIGURATION WHERE PARENT_MENU_ID = :param1";
        string DeleteMenuQuery() => "DELETE FROM MENU_CONFIGURATION WHERE MENU_ID = :param1";
        string EMPLOYEE_INFO_IDENTITY_QUERY() => "SELECT NVL(MAX(ID),0) + 1 ID  FROM EMPLOYEE_INFO";
        public string LoadData(string db, int companyId) => _commonServices.DataTableToJSON(_commonServices.GetDataTable(_configuration.GetConnectionString(db), loadDataQuery(), _commonServices.AddParameter(new string[] { companyId.ToString() })));
        public async Task<string> AddOrUpdate(string db, EMPLOYEE_INFO model)
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

                    if (model.ID == 0)
                    {
                        model.ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), EMPLOYEE_INFO_IDENTITY_QUERY(), _commonServices.AddParameter(new string[] { }));
                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] 
                        {model.ID.ToString(), model.EMPLOYEE_ID.ToString() , model.EMPLOYEE_CODE , model.EMPLOYEE_NAME, model.EMPLOYEE_STATUS, model.COMPANY_ID.ToString() , model.UNIT_ID.ToString() })));

                    }
                    else
                    {
                        
                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.EMPLOYEE_ID.ToString(), model.EMPLOYEE_CODE,
                                model.EMPLOYEE_NAME, model.EMPLOYEE_STATUS, model.COMPANY_ID.ToString(), model.UNIT_ID.ToString(),
                                model.ID.ToString(), 
                            })));

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

        public async Task<string> ActivateMenu(string db, int id)
        {
            if (id < 1)
            {
                return "No data provided !!!!";

            }
            else
            {

                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {

                    listOfQuery.Add(_commonServices.AddQuery(ActivateMenuQuery(), _commonServices.AddParameter(new string[] { id.ToString() })));

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<string> DeactivateMenu(string db, int id)
        {
            if (id < 1)
            {
                return "No data provided !!!!";

            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {


                    listOfQuery.Add(_commonServices.AddQuery(DeactivateMenuQuery(), _commonServices.AddParameter(new string[] { id.ToString() })));

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<string> DeleteMenu(string db, int id)
        {
            DataTable data = _commonServices.GetDataTable(_configuration.GetConnectionString(db), IsParentMenuQuery(), _commonServices.AddParameter(new string[] { id.ToString() }));
            if (data != null && data.Rows.Count > 0)
            {
                return " Sorry!! You can't Delete this menu. Already Some Menu's are assigned under this Menu.";
            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {


                    listOfQuery.Add(_commonServices.AddQuery(DeleteMenuQuery(), _commonServices.AddParameter(new string[] { id.ToString() })));

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<string> GetSearchableDistributor(string db, int Company_Id, string customer) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), customer })));
        //im//
        public async Task<string> GetAllCodeAndDropdownListData(string db, string companyId, string unitId)
        {
            List<string> queries = new List<string>
            {
                "SELECT VALUE, NAME, SHORT_NAME, TYPE, STATUS FROM CODE WHERE UPPER(STATUS)='ACTIVE' AND UPPER(USE_IN_FORM) ='EMPLOYEE' ORDER BY NAME",
                "SELECT UNIT_ID, UNIT_NAME FROM COMPANY_INFO WHERE UPPER(STATUS)='ACTIVE'",

            };
            List<Dictionary<string, string>> parametersList = new List<Dictionary<string, string>>
            {
                _commonServices.AddParameter(new string[] {}),
                _commonServices.AddParameter(new string[] {})

            };
            var dataSet = await _commonServices.GetDataSetForMultiQueryWithParamAsync(
                _configuration.GetConnectionString(db), queries, parametersList
            );
            return _commonServices.DataSetToJSON(dataSet);
        }

    }
}
