using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Security;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Security
{
    public class ReportConfigurationManager : IReportConfigurationManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public ReportConfigurationManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }
        string loadDataQuery() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.REPORT_ID ASC) AS ROW_NO,M.ORDER_BY_SLNO,
                               M.REPORT_ID, M.REPORT_NAME,M.REPORT_TITLE, M.STATUS,M.HAS_PREVIEW,M.HAS_CSV,HAS_PDF, M.MENU_ID,
                               (SELECT N.MENU_NAME FROM MENU_CONFIGURATION N WHERE N.MENU_ID = M.MENU_ID ) MENU_NAME
                               FROM REPORT_CONFIGURATION M WHERE M.COMPANY_ID = :param1";

        string AddOrUpdate_AddQuery() => @"INSERT INTO Report_Configuration 
                                      (REPORT_ID, REPORT_NAME ,MENU_ID, HAS_PDF,HAS_CSV,HAS_PREVIEW,STATUS ,ORDER_BY_SLNO, Company_Id, Entered_BY, Entered_Date, Entered_Terminal,REPORT_TITLE) 
                                      VALUES(:param1 ,:param2  ,:param3  ,:param4,:param5  ,:param6,:param7, :param8,:param9,:param10,TO_DATE(:param11, 'DD/MM/YYYY HH:MI:SS AM'),:param12, :param13)";
        string AddOrUpdate_UpdateQuery() => @"UPDATE Report_Configuration SET 
                                         Report_Name = :param2 ,MENU_ID = :param3 ,HAS_PDF = :param4 ,HAS_CSV = :param5 ,
                                         HAS_PREVIEW = :param6 ,STATUS = :param7 ,ORDER_BY_SLNO = :param8, 
                                         Updated_By= :param9, Updated_Date= TO_DATE(:param10, 'DD/MM/YYYY HH:MI:SS AM'), Updated_Terminal= :param11 , REPORT_TITLE = :param12
                                         WHERE Report_ID = :param1";
        string ActivateReportQuery() => "UPDATE Report_Configuration SET  Status = '"+Status.Active+"' WHERE Report_Id = :param1";
        string DeactivateReportQuery() => "UPDATE Report_Configuration SET  Status = '" + Status.InActive + "' WHERE Report_Id = :param1";
        string GetNewReport_ConfigurationIdQuery() => "SELECT NVL(MAX(REPORT_ID),0) + 1 REPORT_ID  FROM REPORT_CONFIGURATION";

        public async Task<string> LoadData(string db, int companyId) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), loadDataQuery(), _commonServices.AddParameter(new string[] { companyId.ToString() })));
        
        public async Task<string> GetReturnInvoiceNumbers(string db, ReportParams reportParams)
        {
            var query = @"SELECT MST_ID, INVOICE_NO FROM RETURN_MST
            WHERE COMPANY_ID = :param1
            AND TRUNC(RETURN_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY')";

            if (reportParams.UNIT_ID != "ALL")
            {
                query += " And RETURN_UNIT_ID = " + reportParams.UNIT_ID;
            }
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { reportParams.COMPANY_ID.ToString(), reportParams.DATE_FROM, reportParams.DATE_TO })));
        }

        public async Task<string> AddOrUpdate(string db, Report_Configuration model)
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

                    if (model.REPORT_ID == 0)
                    {

                        model.REPORT_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewReport_ConfigurationIdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] 
                        {model.REPORT_ID.ToString(), model.REPORT_NAME, model.MENU_ID.ToString(), model.HAS_PDF, model.HAS_CSV, model.HAS_PREVIEW, model.STATUS, model.ORDER_BY_SLNO.ToString(), model.COMPANY_ID.ToString(), model.ENTERED_BY, model.ENTERED_DATE, model.ENTERED_TERMINAL,model.REPORT_TITLE })));

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] { model.REPORT_ID.ToString(), model.REPORT_NAME, 
                                model.MENU_ID.ToString(), model.HAS_PDF, model.HAS_CSV, model.HAS_PREVIEW, 
                                model.STATUS.ToString(), model.ORDER_BY_SLNO.ToString(),
                                model.UPDATED_BY,model.UPDATED_DATE, model.UPDATED_TERMINAL ,model.REPORT_TITLE
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

        public async Task<string> ActivateReport(string db, int id)
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

                    listOfQuery.Add(_commonServices.AddQuery(ActivateReportQuery(), _commonServices.AddParameter(new string[] { id.ToString() })));

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<string> DeactivateReport(string db, int id)
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


                    listOfQuery.Add(_commonServices.AddQuery(DeactivateReportQuery(), _commonServices.AddParameter(new string[] { id.ToString() })));

                    await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }


        //Role Report Configuration ------------------------------------------------

        string GetSearchableRolesQuery() => @"Select ROW_NUMBER() OVER(ORDER BY ROLE_ID ASC) AS ROW_NO,ROLE_ID,ROLE_NAME,STATUS,
                                 TO_CHAR(ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                 from Role_Info Where COMPANY_ID  = :param1 AND upper(ROLE_NAME) like '%' || upper(:param2) || '%'";

        string RoleReportQuery() => @"SELECT M.REPORT_ID, M.REPORT_NAME, M.REPORT_TITLE , SM.MENU_ID, SM.MENU_NAME FROM REPORT_CONFIGURATION M
                                  LEFT OUTER JOIN MENU_CONFIGURATION SM ON SM.MENU_ID = M.MENU_ID
                                  WHERE M.STATUS = 'Active' AND M.COMPANY_ID = :param1 ";
        string RoleConfigQuery() => @"SELECT R.ID, R.REPORT_ID , R.ROLE_ID, R.PDF_PERMISSION, R.PREVIEW_PERMISSION, R.CSV_PERMISSION FROM REPORT_CONFIGURATION M
                                    LEFT OUTER JOIN ROLE_REPORT_CONFIGURATION R ON R.REPORT_ID = M.REPORT_ID 
                                    WHERE  M.COMPANY_ID = :param1 AND R.ROLE_ID = :param2 ";

        string AddRoleReportConfigQuery() => @"Insert Into ROlE_REPORT_CONFIGURATION
                                          (ID,COMPANY_ID,ROLE_ID,REPORT_ID, PDF_PERMISSION,PREVIEW_PERMISSION,
                                          CSV_PERMISSION, ENTERED_BY,ENTERED_DATE,ENTERED_TERMINAL)
                                          Values (:param1,:param2,:param3,:param4,:param5,:param6,:param7,:param8,TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'),:param10)";

        string AccRoleReportConfigUpdateQuery() => @"UPDATE ROlE_REPORT_CONFIGURATION SET PDF_PERMISSION = :param2,PREVIEW_PERMISSION =  :param3,
                                           CSV_PERMISSION = :param4, UPDATED_BY = :param5, UPDATED_DATE = TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'),UPDATED_TERMINAL = :param7 WHERE ID = :param1";
        string GetNewRoleReportConfigIdQuery() => "SELECT NVL(MAX(ID),0) + 1 ID  FROM ROlE_REPORT_CONFIGURATION";


        public async Task<string> GetSearchableRoles(string db, int companyId, string role_name) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetSearchableRolesQuery(), _commonServices.AddParameter(new string[] { companyId.ToString(), role_name })));

        public async Task<string> RoleReportConfigSelectionList(string db, int companyId, int roleId)
        {

            DataTable MenuLoad = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), RoleReportQuery(), _commonServices.AddParameter(new string[] { companyId.ToString() }));
            DataTable ConfigDataLoad = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), RoleConfigQuery(), _commonServices.AddParameter(new string[] { companyId.ToString(), roleId.ToString() }));
            List<RoleReportConfigView> roleReportConfigView = new List<RoleReportConfigView>();

            for (int i = 0; i < MenuLoad.Rows.Count; i++)
            {
                RoleReportConfigView model = new RoleReportConfigView
                {
                    ROW_NO = i + 1,

                    REPORT_ID = Convert.ToInt32(MenuLoad.Rows[i]["REPORT_ID"]),
                    REPORT_NAME = MenuLoad.Rows[i]["REPORT_NAME"].ToString(),
                    REPORT_TITLE = MenuLoad.Rows[i]["REPORT_TITLE"].ToString()

                };
                if (MenuLoad.Rows[i]["MENU_ID"] != null && MenuLoad.Rows[i]["MENU_NAME"].ToString() != "")
                {
                    model.MENU_ID = Convert.ToInt32(MenuLoad.Rows[i]["MENU_ID"]);
                    model.MENU_NAME = MenuLoad.Rows[i]["MENU_NAME"].ToString();
                }

                roleReportConfigView.Add(model);
            }

            for (int i = 0; i < ConfigDataLoad.Rows.Count; i++)
            {

                RoleReportConfigView configView = roleReportConfigView.Where(x => x.REPORT_ID == Convert.ToInt32(ConfigDataLoad.Rows[i]["REPORT_ID"])).FirstOrDefault();
                if(configView != null)
                {
                    configView.ID = Convert.ToInt32(ConfigDataLoad.Rows[i]["ID"]);
                    configView.ROLE_ID = Convert.ToInt32(ConfigDataLoad.Rows[i]["ROLE_ID"]);
                    configView.PREVIEW_PERMISSION = ConfigDataLoad.Rows[i]["PREVIEW_PERMISSION"].ToString();

                    configView.CSV_PERMISSION = ConfigDataLoad.Rows[i]["CSV_PERMISSION"].ToString();
                    configView.PDF_PERMISSION = ConfigDataLoad.Rows[i]["PDF_PERMISSION"].ToString();

                }

            }

            return JsonSerializer.Serialize(roleReportConfigView);

        }

        public async Task<string> AddRoleReportConfiguration(string db, List<Role_Report_Configuration> roleReportConfig)
        {

            List<QueryPattern> listOfQuery = new List<QueryPattern>();
            try
            {
                this.BindRoleReportConfig(roleReportConfig);
                int new_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewRoleReportConfigIdQuery(), _commonServices.AddParameter(new string[] { }));
                foreach (var model in roleReportConfig)
                {

                    if (model.ID == 0)
                    {


                        model.ID = new_ID;

                        listOfQuery.Add(_commonServices.AddQuery(AddRoleReportConfigQuery(),
                        _commonServices.AddParameter(new string[] { model.ID.ToString(),  model.COMPANY_ID.ToString(), model.ROLE_ID.ToString(), model.REPORT_ID.ToString()
                        ,  model.PDF_PERMISSION,model.PREVIEW_PERMISSION, model.CSV_PERMISSION
                        , model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL
                         })));
                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(AccRoleReportConfigUpdateQuery(),
                         _commonServices.AddParameter(new string[] { model.ID.ToString(), model.PDF_PERMISSION,model.PREVIEW_PERMISSION, model.CSV_PERMISSION
                        , model.UPDATED_BY.ToString(), model.UPDATED_DATE, model.UPDATED_TERMINAL
                          })));

                    }

                    new_ID++;
                }
                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);

                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }


        public List<Role_Report_Configuration> BindRoleReportConfig(List<Role_Report_Configuration> model)
        {
            foreach (var item in model)
            {
                item.CSV_PERMISSION = item.CSV_PERMISSION != Status.Active ? Status.InActive : Status.Active;
                item.PDF_PERMISSION = item.PDF_PERMISSION != Status.Active ? Status.InActive : Status.Active;
                item.PREVIEW_PERMISSION = item.PREVIEW_PERMISSION != Status.Active ? Status.InActive : Status.Active;
            }

            return model;
        }

        //Report User Configuration--------------------------------------------------------------------

        string GetSearchableUserQuery() => @"Select ROW_NUMBER() OVER(ORDER BY USER_ID ASC) AS ROW_NO, USER_ID, EMAIL, EMPLOYEE_ID,  ( USER_NAME || ' (' || EMPLOYEE_ID || ') ' )  USER_NAME from User_Info Where COMPANY_ID = :param1 AND upper(USER_NAME) like '%' || upper(:param2) || '%'";
        string GetSearchableCentralUserQuery() => @"Select ROW_NUMBER() OVER(ORDER BY USER_ID ASC) AS ROW_NO, USER_ID, EMAIL, EMPLOYEE_ID,  ( USER_NAME || ' (' || EMPLOYEE_ID || ') ' )  USER_NAME from User_Info Where upper(USER_NAME) like '%' || upper(:param1) || '%'";

        string UserReportQuery() => @"SELECT M.REPORT_ID, M.REPORT_NAME, M.REPORT_TITLE, SM.MENU_ID, SM.MENU_NAME FROM REPORT_CONFIGURATION M
                                  LEFT OUTER JOIN MENU_CONFIGURATION SM ON SM.MENU_ID = M.MENU_ID
                                  WHERE M.STATUS = 'Active' AND M.COMPANY_ID = :param1";
        string UserReportConfigQuery() => @"SELECT R.ID, R.USER_ID, R.REPORT_ID, R.PDF_PERMISSION, R.PREVIEW_PERMISSION, R.CSV_PERMISSION FROM REPORT_CONFIGURATION M
                                    LEFT OUTER JOIN REPORT_USER_CONFIGURATION R ON R.REPORT_ID = M.REPORT_ID 
                                    WHERE  M.COMPANY_ID = :param1 AND R.USER_ID = :param2";

        string AddUserReportConfigQuery() => @"Insert Into REPORT_USER_CONFIGURATION (ID,COMPANY_ID,USER_ID,REPORT_ID,PDF_PERMISSION,PREVIEW_PERMISSION,
                                          CSV_PERMISSION,ENTERED_BY,ENTERED_DATE,ENTERED_TERMINAL)
                                          Values (:param1,:param2,:param3,:param4,:param5,:param6,:param7,:param8,TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'),:param10)";

        string UpdateUserReportConfigUpdateQuery() => @"UPDATE REPORT_USER_CONFIGURATION SET PDF_PERMISSION =:param1 ,PREVIEW_PERMISSION = :param2,CSV_PERMISSION =  :param3,
                                            UPDATED_BY = :param4, UPDATED_DATE = TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),UPDATED_TERMINAL = :param6 WHERE ID = :param7";
        string GetNewUserReportConfigIdQuery() => "SELECT NVL(MAX(ID),0) + 1 ID  FROM REPORT_USER_CONFIGURATION";


        public async Task<string> GetSearchableUsers(string db, int companyId, string user_Name) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetSearchableUserQuery(), _commonServices.AddParameter(new string[] { companyId.ToString(), user_Name })));
        public async Task<string> GetSearchableCentralUsers(string db, string user_Name) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetSearchableCentralUserQuery(), _commonServices.AddParameter(new string[] { user_Name })));

        public async Task<string> UserReportConfigSelectionList(string db, int companyId, int userId)
        {

            DataTable MenuLoad = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), UserReportQuery(), _commonServices.AddParameter(new string[] { companyId.ToString() }));
            DataTable ConfigDataLoad = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), UserReportConfigQuery(), _commonServices.AddParameter(new string[] { companyId.ToString(), userId.ToString() }));
            List<UserReportConfigView> UserReportConfigView = new List<UserReportConfigView>();

            for (int i = 0; i < MenuLoad.Rows.Count; i++)
            {
                UserReportConfigView model = new UserReportConfigView
                {
                    ROW_NO = i + 1,

                    REPORT_ID = Convert.ToInt32(MenuLoad.Rows[i]["REPORT_ID"]),
                    REPORT_NAME = MenuLoad.Rows[i]["REPORT_NAME"].ToString(),
                    REPORT_TITLE = MenuLoad.Rows[i]["REPORT_TITLE"].ToString()
                };
                if (MenuLoad.Rows[i]["MENU_ID"] != null && MenuLoad.Rows[i]["MENU_ID"].ToString() != "")
                {
                    model.MENU_ID = Convert.ToInt32(MenuLoad.Rows[i]["MENU_ID"]);
                    model.MENU_NAME = MenuLoad.Rows[i]["MENU_NAME"].ToString();
                }
                UserReportConfigView.Add(model);
            }

            for (int i = 0; i < ConfigDataLoad.Rows.Count; i++)
            {

                UserReportConfigView configView = UserReportConfigView.Where(x => x.REPORT_ID == Convert.ToInt32(ConfigDataLoad.Rows[i]["REPORT_ID"])).FirstOrDefault();
                if (configView != null)
                {
                    configView.ID = Convert.ToInt32(ConfigDataLoad.Rows[i]["ID"]);
                    configView.USER_ID = Convert.ToInt32(ConfigDataLoad.Rows[i]["USER_ID"]);
                    configView.PDF_PERMISSION = ConfigDataLoad.Rows[i]["PDF_PERMISSION"].ToString();
                    configView.PREVIEW_PERMISSION = ConfigDataLoad.Rows[i]["PREVIEW_PERMISSION"].ToString();
                    configView.CSV_PERMISSION = ConfigDataLoad.Rows[i]["CSV_PERMISSION"].ToString();
                }


            }
            if (UserReportConfigView.Count == 0)
            {
                return "1";
            }
            return JsonSerializer.Serialize(UserReportConfigView);

        }

        public async Task<string> AddUserReportConfiguration(string db, List<Report_User_Configuration> UserReportConfig)
        {

            List<QueryPattern> listOfQuery = new List<QueryPattern>();
            try
            {
                this.BindUserReportConfig(UserReportConfig);
                int new_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewUserReportConfigIdQuery(), _commonServices.AddParameter(new string[] { }));
                foreach (var model in UserReportConfig)
                {

                    if (model.ID == 0)
                    {


                        model.ID = new_ID;

                        listOfQuery.Add(_commonServices.AddQuery(AddUserReportConfigQuery(),
                        _commonServices.AddParameter(new string[] { model.ID.ToString(),  model.COMPANY_ID.ToString(), model.USER_ID.ToString(), model.REPORT_ID.ToString()
                        , model.PDF_PERMISSION,model.PREVIEW_PERMISSION, model.CSV_PERMISSION
                        , model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL
                         })));
                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(UpdateUserReportConfigUpdateQuery(),
                         _commonServices.AddParameter(new string[] {  model.PDF_PERMISSION,
                             model.PREVIEW_PERMISSION, model.CSV_PERMISSION
                        , model.UPDATED_BY.ToString(), model.UPDATED_DATE, model.UPDATED_TERMINAL, model.ID.ToString()
                          })));

                    }

                    new_ID++;
                }
                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);

                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }


        public List<Report_User_Configuration> BindUserReportConfig(List<Report_User_Configuration> model)
        {
            foreach (var item in model)
            {
                item.PDF_PERMISSION = item.PDF_PERMISSION != Status.Active ? Status.InActive : Status.Active;
                item.PREVIEW_PERMISSION = item.PREVIEW_PERMISSION != Status.Active ? Status.InActive : Status.Active;
                item.CSV_PERMISSION = item.CSV_PERMISSION != Status.Active ? Status.InActive : Status.Active;
            }

            return model;
        }


        //Report Permission --------------------------------------------------------------
        string LoadProductReportData_Query() =>@"Select distinct  REPORT_ID, REPORT_NAME,ORDER_BY_SLNO, REPORT_TITLE,MENU_ID ,
 PDF_PERMISSION, PREVIEW_PERMISSION, CSV_PERMISSION ,MENU_NAME,MENU_ACTION from 
(Select distinct MC.REPORT_ID, MC.REPORT_NAME,MC.ORDER_BY_SLNO, MC.REPORT_TITLE, MC.MENU_ID , CONCAT(MCD.AREA, MCD.CONTROLLER) MENU_NAME,MCD.ACTION MENU_ACTION,
RM.PDF_PERMISSION, RM.PREVIEW_PERMISSION, RM.CSV_PERMISSION
from ROLE_INFO R 
INNER JOIN ROLE_REPORT_CONFIGURATION RM on RM.ROLE_ID = R.ROLE_ID
INNER JOIN ROLE_USER_CONFIGURATION RU on R.ROLE_ID = RU.ROLE_ID
INNER JOIN REPORT_Configuration MC on MC.REPORT_ID = RM.REPORT_ID 
INNER JOIN MENU_CONFIGURATION MCD on MCD.MENU_ID = MC.MENU_ID 

 Where MC.STATUS ='Active' AND (RM.PDF_PERMISSION = 'Active' OR RM.CSV_PERMISSION = 'Active' OR RM.PREVIEW_PERMISSION = 'Active' )  AND RU.COMPANY_ID = :param1 AND  RU.USER_ID = :param2 
 UNION ALL
 Select distinct MC.REPORT_ID, MC.REPORT_NAME,MC.ORDER_BY_SLNO, MC.REPORT_TITLE, MC.MENU_ID ,CONCAT(MCD.AREA, MCD.CONTROLLER)  MENU_NAME ,MCD.ACTION MENU_ACTION,
 RU.PDF_PERMISSION, RU.PREVIEW_PERMISSION, RU.CSV_PERMISSION
from  REPORT_USER_CONFIGURATION RU
INNER JOIN REPORT_Configuration MC on MC.REPORT_ID = RU.REPORT_ID 
 INNER JOIN MENU_CONFIGURATION MCD on MCD.MENU_ID = MC.MENU_ID 
 Where MC.STATUS ='Active' AND (RU.PDF_PERMISSION = 'Active' OR RU.CSV_PERMISSION = 'Active' OR RU.PREVIEW_PERMISSION = 'Active' )   AND RU.COMPANY_ID = :param1 AND  RU.USER_ID = :param2 AND RU.REPORT_ID  Not In (Select RRC.REPORT_ID From ROLE_REPORT_CONFIGURATION RRC 
 INNER JOIN ROLE_USER_CONFIGURATION RUC on RRC.ROLE_ID = RUC.ROLE_ID 


 where USER_ID = :param2 ) ) x";


        public async Task<DataTable> LoadReportPermissionDatatable(string db, int Company_Id, int User_Id) => await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadProductReportData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), User_Id.ToString() }));
        public async Task<List<ReportPermission>> LoadReportPermissionData(string db, int Company_Id, int user_Id)
        {
            DataTable dt = await LoadReportPermissionDatatable(db, Company_Id, user_Id);
            List<ReportPermission> reportPermissions = new List<ReportPermission>();


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ReportPermission menuMaster = new ReportPermission
                {
                    MENU_ID = Convert.ToInt32(dt.Rows[i]["MENU_ID"]),
                    ORDER_BY_SLNO = Convert.ToInt32(dt.Rows[i]["ORDER_BY_SLNO"].ToString()),
                    REPORT_NAME = (dt.Rows[i]["REPORT_NAME"].ToString()),
                    REPORT_ID = Convert.ToInt32(dt.Rows[i]["REPORT_ID"]),
                    REPORT_TITLE = dt.Rows[i]["REPORT_TITLE"].ToString(),
                    MENU_NAME = dt.Rows[i]["MENU_NAME"].ToString(),
                    MENU_ACTION = dt.Rows[i]["MENU_ACTION"].ToString(),
                    PDF_PERMISSION = (dt.Rows[i]["PDF_PERMISSION"].ToString()),
                    PREVIEW_PERMISSION = (dt.Rows[i]["PREVIEW_PERMISSION"].ToString()),
                    CSV_PERMISSION = (dt.Rows[i]["CSV_PERMISSION"].ToString()),
                };
                reportPermissions.Add(menuMaster);
            }
            return reportPermissions;
        }

        public ReportPermission IsReportPermitted(int reportId, List<ReportPermission> permissions)
        {
            ReportPermission reportPermission = permissions.Where(x => x.REPORT_ID == reportId).FirstOrDefault();
            if (reportPermission!= null)
            {
                return reportPermission;
            }
            return new ReportPermission();
        }

    }
}
