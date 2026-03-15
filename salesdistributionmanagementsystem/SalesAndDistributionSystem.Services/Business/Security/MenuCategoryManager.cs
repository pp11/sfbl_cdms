using Microsoft.Extensions.Configuration;
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
    public class MenuCategoryManager: IMenuCategoryManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public MenuCategoryManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;
           
        }
        string loadDataQuery() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MODULE_ID ASC) AS ROW_NO,
                                  M.MODULE_ID, M.MODULE_NAME, M.STATUS, U.USER_NAME CREATENAME, TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE,
                                  ORDER_BY_NO FROM MODULE_INFO  M, USER_INFO U WHERE U.USER_ID = M.ENTERED_BY AND M.COMPANY_ID = :param1";
        string AddOrUpdate_AddQuery() => @"INSERT INTO Module_Info 
                                         (Module_Id, Module_Name, Status, Entered_By, Entered_Date,ENTERED_TERMINAL, Order_By_No, Company_Id ) 
                                         VALUES ( :param1, :param2, :param3, :param4, TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM') , :param6, :param7, :param8 )";
        string AddOrUpdate_UpdateQuery() => @"UPDATE Module_Info SET 
                                            Module_Name = :param2, UPDATED_BY = :param3, UPDATED_DATE = TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param5, Order_By_No = :param6 WHERE Module_Id = :param1";
        string ActivateModuleQuery() => "UPDATE Module_Info SET  Status = '" +Status.Active+ "' WHERE Module_Id = :param1";
        string DeactivateeModuleQuery() => "UPDATE Module_Info SET  Status = '" + Status.InActive + "' WHERE Module_Id = :param1";
        string HasMenuUndereModule() => "SELECT A.MODULE_ID FROM MODULE_INFO A INNER JOIN  MENU_CONFIGURATION B ON B.MODULE_ID = A.MODULE_ID WHERE A.MODULE_ID = :param1";
        string DeleteeModuleQuery() => "DELETE FROM MODULE_INFO WHERE MODULE_ID = :param1";
        string GetNewModule_InfoIdQuery() => "SELECT NVL(MAX(MODULE_ID),0) + 1 MODULE_ID  FROM MODULE_INFO";
       

        public string LoadData(string db, int companyId) => _commonServices.DataTableToJSON(_commonServices.GetDataTable(_configuration.GetConnectionString(db) ,loadDataQuery(), _commonServices.AddParameter(new string[] { companyId.ToString() })));
        public async Task<string> AddOrUpdate(string db, Module_Info model)
        {
            if (model == null)
            {
                return  "No data provided to insert!!!!";

            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {
                   
                    if (model.MODULE_ID == 0)
                    {
               
                        model.MODULE_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewModule_InfoIdQuery(), _commonServices.AddParameter(new string[] { }));
                        model.STATUS = Status.Active;

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] {model.MODULE_ID.ToString(),  model.MODULE_NAME, model.STATUS, model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL, model.ORDER_BY_NO.ToString(),model.COMPANY_ID.ToString() })));

                    }
                    else
                    {
                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQuery(), _commonServices.AddParameter(new string[] { model.MODULE_ID.ToString(), model.MODULE_NAME, model.UPDATED_BY.ToString(),model.UPDATED_DATE , model.UPDATED_TERMINAL, model.ORDER_BY_NO.ToString() })));
                      
                    }
                   
                  await  _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                  return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

        }

        public async Task<string> ActivateMenuCategory(string db,int id)
        {
            if (id <1)
            {
                return "No data provided !!!!";

            }
            else
            {
                
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {

                    listOfQuery.Add(_commonServices.AddQuery(ActivateModuleQuery(), _commonServices.AddParameter(new string[] { id.ToString() })));

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<string> DeactivateMenuCategory(string db, int id)
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


                    listOfQuery.Add(_commonServices.AddQuery(DeactivateeModuleQuery(), _commonServices.AddParameter(new string[] { id.ToString() })));

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<string> DeleteMenuCategory(string db, int id)
        {
            DataTable data = _commonServices.GetDataTable(_configuration.GetConnectionString(db), HasMenuUndereModule(), _commonServices.AddParameter(new string[] { id.ToString() }));
            if (data != null && data.Rows.Count > 0)
            {
                return " Sorry!! You can't Delete this menu category. Already Some Menu's are assigned under this Menu Category.";
            }
            else
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                try
                {


                    listOfQuery.Add(_commonServices.AddQuery(DeleteeModuleQuery(), _commonServices.AddParameter(new string[] { id.ToString() })));

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
    }
}
