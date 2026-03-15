using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.ReportModels.Common.ProductReports;
using SalesAndDistributionSystem.Domain.Models.TableModels.Company;
using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Company
{
    public class NotificationManager : INotificationManager
    {
        private readonly IConfiguration connString;
        private readonly ICommonServices _commonService;
        private readonly IConfiguration _configuration;

        public NotificationManager(IConfiguration connstring, ICommonServices commonServices, IConfiguration configuration)
        {
            connString = connstring;
            _commonService = commonServices;
            _configuration = configuration;
        }

        string NotificationLoad_Query() => @"SELECT 
   ROWNUM, N.NOTIFICATION_ID, N.NOTIFICATION_POLICY_ID, N.NOTIFICATION_TITLE, 
   N.NOTIFICATION_BODY,TO_CHAR( N.NOTIFICATION_DATE, 'DD/MM/YYYY HH:MI:SS AM') NOTIFICATION_DATE, N.STATUS, 
   N.COMPANY_ID, N.UNIT_ID , V.ID
FROM NOTIFICATION N
INNER JOIN NOTIFICATION_VIEW V on V.NOTIFICATION_ID = N.NOTIFICATION_ID and V.STATUS != 'Active'
WHERE V.USER_ID = :param1 ";
        string DataLoad_Query() => @"SELECT 
   ROWNUM, N.NOTIFICATION_ID, N.NOTIFICATION_POLICY_ID, N.NOTIFICATION_TITLE, 
   N.NOTIFICATION_BODY,TO_CHAR( N.NOTIFICATION_DATE, 'DD/MM/YYYY HH:MI:SS AM') NOTIFICATION_DATE, V.STATUS, 
   N.COMPANY_ID, N.UNIT_ID , V.ID
FROM NOTIFICATION N
INNER JOIN NOTIFICATION_VIEW V on V.NOTIFICATION_ID = N.NOTIFICATION_ID
WHERE V.USER_ID = :param1 AND  trunc(N.NOTIFICATION_DATE ) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY') Order By N.NOTIFICATION_ID DESC";
        string Notification_Policy_Permitted_User() => @"Select V.ID,  V.NOTIFICATION_POLICY_ID,  V.STATUS,  V.COMPANY_ID,  V.UNIT_ID,  V.USER_ID,  V.VIEW_PERMISSION , P.NOTIFICATION_TITLE
from NOTIFICATION_VIEW_POLICY V
LEFT OUTER JOIN NOTIFICATION_POLICY P on P.NOTIFICATION_POLICY_ID = V.NOTIFICATION_POLICY_ID Where V.STATUS = 'Active' and P.STATUS = 'Active'
And V.NOTIFICATION_POLICY_ID = :param1 AND V.UNIT_ID = :param2 AND V.COMPANY_ID=:param3";
        string AddOrUpdateNotificationInsertQuery() => @"INSERT INTO NOTIFICATION (
 
                                         NOTIFICATION_ID
                                        ,NOTIFICATION_POLICY_ID
                                        ,NOTIFICATION_TITLE
                                        ,NOTIFICATION_BODY
                                        ,NOTIFICATION_DATE
                                        ,COMPANY_ID
                                        ,UNIT_ID
                                        ,STATUS
                                       ) 
                                       VALUES ( :param1, :param2, :param3, :param4, TO_DATE( :param5, 'DD/MM/YYYY HH:MI:SS AM') , :param6, :param7, :param8 )";
        string AddOrUpdateNotificationViewInsertQuery() => @"INSERT INTO NOTIFICATION_VIEW (
                                         ID
                                        ,NOTIFICATION_ID
                                        ,USER_ID
                                        ,STATUS
                                        ,COMPANY_ID
                                        ,UNIT_ID
                                        ,VIEW_DATE
                                       ) 
                                       VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, TO_DATE( :param7, 'DD/MM/YYYY HH:MI:SS AM') )";
        string UpdateNotificationViewStatus_Query() => @"Update NOTIFICATION_VIEW Set STATUS = 'Active' Where ID = :param1";
        string UpdateNotificationViewStatusByUser_Query() => @"Update NOTIFICATION_VIEW Set STATUS = 'Active' Where USER_ID = :param1";

        string GetNewNotificationIdQuery() => "SELECT NVL(MAX(NOTIFICATION_ID),0) + 1 NOTIFICATION_ID  FROM NOTIFICATION";
        string GetNewNotificationViewIdQuery() => "SELECT NVL(MAX(ID),0) + 1 ID  FROM NOTIFICATION_VIEW";

        public async Task<DataTable> NotificationLoad_Datatable(string db, int User_Id) => await _commonService.GetDataTableAsyn(connString.GetConnectionString(db), NotificationLoad_Query(), _commonService.AddParameter(new string[] { User_Id.ToString() }));
        public async Task<DataTable> DataLoad_Datatable(string db, string User_Id, string date_from , string date_to) => await _commonService.GetDataTableAsyn(connString.GetConnectionString(db), DataLoad_Query(), _commonService.AddParameter(new string[] { User_Id, date_from, date_to }));
        public async Task<DataTable> NotificationPermitted_Datatable(string db, int Policy_Id,int Unit_Id,int Company_Id) => await _commonService.GetDataTableAsyn(connString.GetConnectionString(db), Notification_Policy_Permitted_User(), _commonService.AddParameter(new string[] { Policy_Id.ToString(), Unit_Id.ToString(),Company_Id.ToString() }));
        public async Task<string> Notification_Permitted_Users(string db, int Policy_Id, int Unit_Id, int Company_Id) => _commonService.DataTableToJSON(await this.NotificationPermitted_Datatable(db,Policy_Id,Unit_Id,Company_Id));

        public async Task<List<Notification>> Notification_Permitted_User(string db, int Policy_Id,int Unit_Id,int Company_Id)
        {
            DataTable dataTable =await this.NotificationPermitted_Datatable(db, Policy_Id, Unit_Id, Company_Id);
            List<Notification> User_Ids = new List<Notification>();
            foreach (DataRow row in dataTable.Rows)
            {
                Notification notification = new Notification();
                notification.USER_ID = Convert.ToInt32(row["USER_ID"]);
                User_Ids.Add(notification);
                  
            }
            User_Ids[0].NOTIFICATION_TITLE = dataTable.Rows[0]["NOTIFICATION_TITLE"].ToString();
            return User_Ids;
        }



        public async Task<string> NotificationLoad(string db, Notification model) => _commonService.DataTableToJSON(await NotificationLoad_Datatable(db, model.USER_ID));
        public async Task<string> LoadData(string db, ReportParams model) => _commonService.DataTableToJSON(await DataLoad_Datatable(db, model.USER_NAME,model.DATE_FROM, model.DATE_TO ));
        
        public async Task<string> UpdateNotificationViewStatus(string db, Notification model)
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

                    if (model.ID>0)
                    {


                        listOfQuery.Add(_commonService.AddQuery(UpdateNotificationViewStatus_Query(), _commonService.AddParameter(new string[]
                        {  model.ID.ToString() })));

                    }

                    await _commonService.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        public async Task<string> UpdateNotificationViewStatusByUser(string db, Notification model)
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

                    if (model.USER_ID >0)
                    {


                        listOfQuery.Add(_commonService.AddQuery(UpdateNotificationViewStatusByUser_Query(), _commonService.AddParameter(new string[]
                        {  model.USER_ID.ToString() })));

                    }

                    await _commonService.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<string> AddOrUpdateNotification(string db, Notification model)
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

                    if (model.NOTIFICATION_ID == 0)
                    {
                        model.STATUS = Status.Active;
                        model.NOTIFICATION_ID = _commonService.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewNotificationIdQuery(), _commonService.AddParameter(new string[] { }));
                        model.ID = _commonService.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewNotificationViewIdQuery(), _commonService.AddParameter(new string[] { }));
                        List<Notification> permitted_Users = new List<Notification>();
                        permitted_Users = await Notification_Permitted_User(db, model.NOTIFICATION_POLICY_ID,model.UNIT_ID,model.COMPANY_ID);
                        model.NOTIFICATION_TITLE = permitted_Users[0].NOTIFICATION_TITLE;



                        listOfQuery.Add(_commonService.AddQuery(AddOrUpdateNotificationInsertQuery(), _commonService.AddParameter(new string[]
                                            {model.NOTIFICATION_ID.ToString(), model.NOTIFICATION_POLICY_ID.ToString(), model.NOTIFICATION_TITLE.ToString(), model.NOTIFICATION_BODY,
                            model.NOTIFICATION_DATE, model.COMPANY_ID.ToString(), model.UNIT_ID.ToString(), model.STATUS.ToString() })));
                      
                        foreach(var item in permitted_Users)
                        {
                            listOfQuery.Add(_commonService.AddQuery(AddOrUpdateNotificationViewInsertQuery(),
                                _commonService.AddParameter(new string[]
                                {
                                             model.ID.ToString(), model.NOTIFICATION_ID.ToString(), item.USER_ID.ToString(), Status.InActive, model.COMPANY_ID.ToString(),
                                            model.UNIT_ID.ToString(), model.NOTIFICATION_DATE
                                 })));
                            model.ID++;
                        }
                       
                        await _commonService.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);


                    }
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }


        //-- Notification Part-------------
      
        public async Task<string> AddOrderNotification(string db,string db_sales, int NotificationPolicyId, string NotificationBody, int CompanyId,int UnitId)
        {
            Notification notification = new Notification();
            notification.COMPANY_ID = CompanyId;
            notification.UNIT_ID = UnitId;
            notification.NOTIFICATION_DATE = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");

            notification.NOTIFICATION_POLICY_ID = NotificationPolicyId;
            notification.NOTIFICATION_BODY = NotificationBody;
            return  await  this.AddOrUpdateNotification(db, notification);
            
        }

       
    }
}
