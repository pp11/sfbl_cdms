using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Utilities.Collections;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using SalesAndDistributionSystem.Domain.Models.TableModels.User;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.User
{
    public class UserManager : IUserManager
    {
        private readonly IConfiguration connString;
        private readonly ICommonServices _commonService;
        private readonly IEmailService _EmailService;


        public UserManager(IConfiguration connstring, ICommonServices commonServices, IEmailService EmailService)
        {
            connString = connstring;
            _commonService = commonServices;
            _EmailService = EmailService;
        }

        string UserQuery() => "SELECT  U.USER_ID ID, U.USER_NAME NAME, U.UNIT_ID UNITID, C.UNIT_TYPE UNITTYPE, U.USER_TYPE  USERTYPE, C.COMPANY_ID COMPANYID, U.EMAIL, C.COMPANY_NAME,U.EMPLOYEE_ID FROM USER_INFO U LEFT OUTER JOIN COMPANY_INFO C ON C.COMPANY_ID = U.COMPANY_ID AND C.UNIT_ID= U.UNIT_ID WHERE U.EMAIL = :param1";
        string UserQuery4() => @"SELECT 
    U.USER_ID AS ID,
    U.USER_NAME AS NAME,
    U.USER_PASSWORD,
    U.UNIT_ID AS UNITID,
    C.UNIT_TYPE AS UNITTYPE,
    U.USER_TYPE AS USERTYPE,
    C.COMPANY_ID AS COMPANYID,
    U.EMAIL,
    U.EMPLOYEE_ID,
    C.COMPANY_NAME,
    C.UNIT_NAME,
    R.ROLE_NAMES
FROM 
    USER_INFO U
    LEFT OUTER JOIN COMPANY_INFO C 
        ON C.COMPANY_ID = U.COMPANY_ID 
        AND C.UNIT_ID = U.UNIT_ID
    LEFT OUTER JOIN (
        SELECT 
            RU.USER_ID,
            LISTAGG(RI.ROLE_NAME, ', ') WITHIN GROUP (ORDER BY RI.ROLE_NAME) AS ROLE_NAMES
        FROM 
            ROLE_USER_CONFIGURATION RU
            JOIN ROLE_INFO RI ON RI.ROLE_ID = RU.ROLE_ID
        GROUP BY RU.USER_ID
    ) R ON R.USER_ID = U.USER_ID
WHERE 
    U.EMAIL = :param1 
    AND U.STATUS = 'Active'
";

        string UserQuery2() => "Select  User_Id from User_Info Where Email = :param1 AND  USER_PASSWORD = :param2";
        string UserQuery3() => "Select  COMPANY_ID FROM USER_INFO Where USER_ID= :param1";



        public DataTable GetUserByEmailDataTable(string db, string Email) => _commonService.GetDataTable(connString.GetConnectionString(db), UserQuery(), _commonService.AddParameter(new string[] { Email }));
                public DataTable GetUserByUserIdDataTable(string db, int userId) => _commonService.GetDataTable(connString.GetConnectionString(db), @"SELECT U.USER_NAME, U.USER_TYPE, E.EMPLOYEE_NAME EMAIL
FROM USER_INFO U, EMPLOYEE_INFO E
WHERE U.EMPLOYEE_ID = E.EMPLOYEE_ID AND U.USER_ID = :param1 ", _commonService.AddParameter(new string[] { userId.ToString() }));

		public DataTable GetUserByEmailAndCompanyDataTable(string db, string Email, int CompanyId) => _commonService.GetDataTable(connString.GetConnectionString(db), UserQuery4(), _commonService.AddParameter(new string[] { Email.Trim() }));

        public DataTable CheckValidUserDataTable(string db, string Email, string Password) => _commonService.GetDataTable(connString.GetConnectionString(db), UserQuery2(), _commonService.AddParameter(new string[] { Email, Password }));
        public DataTable GetUserByUseridDataTable(string db, int UserId) => _commonService.GetDataTable(connString.GetConnectionString(db), UserQuery3(), _commonService.AddParameter(new string[] { UserId.ToString() }));

        public Auth GetUserByEmail(string db, string Email)
        {
            Auth auth = new Auth();
            DataTable userData = GetUserByEmailDataTable(db, Email);

            if (userData != null && userData.Rows.Count > 0)
            {

                auth.Email = userData.Rows[0]["Email"].ToString();
                auth.UserName = userData.Rows[0]["Name"].ToString();
                auth.UserId = Convert.ToInt32(userData.Rows[0]["Id"]);
                auth.CompanyId = Convert.ToInt32(userData.Rows[0]["CompanyId"]);
                auth.CompanyName = (userData.Rows[0]["Company_Name"].ToString());

                auth.UnitId = Convert.ToInt32(userData.Rows[0]["UnitId"]);
                auth.UnitType = userData.Rows[0]["UnitType"].ToString();
                auth.UserType = userData.Rows[0]["UserType"].ToString();
                auth.DistributorId = Convert.ToInt32(userData.Rows[0]["EMPLOYEE_ID"].ToString());
                return auth;

            }
            return null;



        }
        public Auth GetUserByUserId(string db, int userId)
        {
            Auth auth = new Auth();
            DataTable userData = GetUserByUserIdDataTable(db, userId);

            if (userData != null && userData.Rows.Count > 0)
            {
                auth.Email = userData.Rows[0]["EMAIL"].ToString();
                auth.UserName = userData.Rows[0]["USER_NAME"].ToString();
                auth.UserType = userData.Rows[0]["USER_TYPE"].ToString();
                return auth;
            }
            return null;



        }       
        public Auth GetUserByEmailAndCompany(string db, string Email, int companyId)
        {
            Auth auth = new Auth();
            DataTable userData = GetUserByEmailAndCompanyDataTable(db, Email, companyId);
            if (userData != null && userData.Rows.Count > 0)
            {
                auth.Email = userData.Rows[0]["Email"].ToString();
                auth.UserName = userData.Rows[0]["Name"].ToString();
                auth.UserId = Convert.ToInt32(userData.Rows[0]["Id"]);
                auth.CompanyId = Convert.ToInt32(userData.Rows[0]["CompanyId"]);
                auth.CompanyName = (userData.Rows[0]["Company_Name"].ToString());
                auth.UnitName = (userData.Rows[0]["UNIT_NAME"].ToString());
                auth.Password = (userData.Rows[0]["USER_PASSWORD"].ToString());
                auth.UnitId = Convert.ToInt32(userData.Rows[0]["UnitId"]);
                auth.UnitType = userData.Rows[0]["UnitType"].ToString();
                auth.UserType = userData.Rows[0]["UserType"].ToString();
                auth.RoleNames = userData.Rows[0]["ROLE_NAMES"].ToString();
                auth.DistributorId = Convert.ToInt32(userData.Rows[0]["EMPLOYEE_ID"].ToString());
                return auth;

            }
            return null;
        }

        public bool IsValidUser(string db, string Email, string Password, int CompanyId, string HashPass)
        {
            //DataTable IsValidUser = CheckValidUserDataTable( db, Email, _commonService.Decrypt(Password));
            DataTable IsValidUser = CheckValidUserDataTable(db, Email, _commonService.Encrypt(Password));
            string decryptValue = _commonService.Decrypt(HashPass);
            if (Password == decryptValue)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        //----------------------------- User Function -----------------------------------------

        string UserAccordingToCompany() => @"Select  distinct  ROW_NUMBER() OVER(ORDER BY  u.USER_ID ASC) AS ROW_NO,
                                         u.USER_TYPE
                                        ,u.USER_NAME
                                        ,u.USER_ID
                                        ,u.UNIT_ID
                                        ,u.ENTERED_DATE
                                        ,u.EMPLOYEE_ID
                                        ,u.EMAIL
                                        ,u.COMPANY_ID
                                        ,c.COMPANY_NAME
                                        from User_Info u 
                                        inner join Company_Info c on c.Company_Id = u.Company_id AND C.UNIT_ID = U.UNIT_ID 
                                        Where u.Company_ID = :param1";
        string GetUsers() => @"SELECT U.USER_ID,
       U.USER_NAME,
       U.USER_TYPE,
       U.USER_PASSWORD,
       U.EMAIL,
       U.EMPLOYEE_ID,
       U.COMPANY_ID,
       U.UNIT_ID,    
       U.UNIQUEACCESSKEY,
       U.STATUS , 
       C.COMPANY_NAME,
       C.UNIT_NAME
  FROM USER_INFO U
       INNER JOIN COMPANY_INFO C ON C.COMPANY_ID = U.COMPANY_ID AND C.UNIT_ID = U.UNIT_ID ORDER BY U.USER_ID DESC";
        string GetEmployeesWithoutAccount() => @"Select ID, EMPLOYEE_ID, EMPLOYEE_CODE, EMPLOYEE_NAME, EMPLOYEE_STATUS, COMPANY_ID, UNIT_ID from Employee_Info where COMPANY_ID = :param1 AND  EMPLOYEE_ID NOT IN (Select EMPLOYEE_ID from User_info) ";
        string GetEmployeeByEmployeeId() => @"Select ID, EMPLOYEE_ID, EMPLOYEE_CODE, EMPLOYEE_NAME, EMPLOYEE_STATUS, COMPANY_ID, UNIT_ID from Employee_Info where Employee_Id = :param1 ";

        string GetUsersByCompany() => @"SELECT U.USER_ID,
       U.USER_NAME,
       U.USER_TYPE,
       U.USER_PASSWORD,
       U.EMAIL,
       U.EMPLOYEE_ID,
       U.COMPANY_ID,
       U.UNIT_ID,    
       U.UNIQUEACCESSKEY,
       U.STATUS , 
       C.COMPANY_NAME,
       C.UNIT_NAME
  FROM USER_INFO U
       INNER JOIN COMPANY_INFO C ON C.COMPANY_ID = U.COMPANY_ID AND C.UNIT_ID = U.UNIT_ID AND U.COMPANY_ID = :param1 ORDER BY U.USER_ID DESC";

        string GetNewUSER_IDQuery() => "SELECT NVL(MAX(USER_ID),0) + 1 USER_ID  FROM USER_INFO";
        string AddOrUpdatyeInsertQuery() => @"INSERT INTO USER_INFO (
                           USER_ID
                          ,USER_TYPE
                          ,USER_PASSWORD
                          ,USER_NAME
                          ,UNIT_ID
                          ,ENTERED_TERMINAL
                          ,ENTERED_DATE
                          ,ENTERED_BY
                          ,EMPLOYEE_ID
                          ,EMAIL
                          ,COMPANY_ID
                          ,STATUS) 
                          VALUES(:param1 ,:param2  ,:param3  ,:param4,:param5  ,:param6,TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),:param8,:param9,:param10,:param11,:param12 )";
        string AddOrUpdateUpdateQuery() => @"UPDATE USER_INFO SET 
                                         USER_TYPE = :param2,
                                         Updated_By= :param3, 
                                         Updated_Date= TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'), 
                                         Updated_Terminal= :param5 , 
                                         USER_NAME = :param6,
                                         EMAIL = :param7 ,
                                         STATUS = :param8 
                                         WHERE USER_ID = :param1";
        string UpdateUniqueKeyByUser() => @"UPDATE USER_INFO SET UNIQUEACCESSKEY = :param1 WHERE USER_ID = :param2";
        string UpdatePassWordAndKeyByUser() => @"UPDATE USER_INFO SET UNIQUEACCESSKEY = :param1, USER_PASSWORD = :param3,STATUS= 'InActive'  WHERE USER_ID = :param2";


        public DataTable GetUserByCompanyDataTable(string db, string Company) => _commonService.GetDataTable(connString.GetConnectionString(db), UserAccordingToCompany(), _commonService.AddParameter(new string[] { Company }));
        public string GetUsers(string db) => _commonService.DataTableToJSON(_commonService.GetDataTable(connString.GetConnectionString(db), GetUsers(), _commonService.AddParameter(new string[] { })));
        public string GetEmployeesWithoutAccount(string db, int CompanyId) => _commonService.DataTableToJSON(_commonService.GetDataTable(connString.GetConnectionString(db), GetEmployeesWithoutAccount(), _commonService.AddParameter(new string[] { CompanyId.ToString() })));
        public DataTable GetEmployeeByEmployeeId(string db, string EmployeeId) => _commonService.GetDataTable(connString.GetConnectionString(db), GetEmployeeByEmployeeId(), _commonService.AddParameter(new string[] { EmployeeId }));
        public DataTable GetUsersByCompanyDataTable(string db, int CompanyId) => _commonService.GetDataTable(connString.GetConnectionString(db), GetUsersByCompany(), _commonService.AddParameter(new string[] { CompanyId.ToString() }));

        public string GetUsersByCompany(string db, int CompanyId) => _commonService.DataTableToJSON(_commonService.GetDataTable(connString.GetConnectionString(db), GetUsersByCompany(), _commonService.AddParameter(new string[] { CompanyId.ToString() })));

        public string GetUserByCompanyJsonList(string db, string Company) => _commonService.DataTableToJSON(GetUserByCompanyDataTable(db, Company));

        public async Task<string> AddOrUpdate(string db, User_Info model, string Path, string url)
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
                    DataTable dataTable = GetEmployeeByEmployeeId(db, model.EMPLOYEE_ID);

                    if (dataTable.Rows.Count > 0)
                    {
                        if (model.USER_ID == 0)
                        {
                            //EmailConfiguration emailConfiguration = new EmailConfiguration();
                            if (model.USER_TYPE == "")
                            {
                                model.USER_TYPE = UserType.General;
                            }
                            RandomStringGenerator generator = new RandomStringGenerator();
                            model.USER_ID = _commonService.GetMaximumNumber<int>(connString.GetConnectionString(db), GetNewUSER_IDQuery(), _commonService.AddParameter(new string[] { }));

                            model.USER_NAME = dataTable.Rows[0]["EMPLOYEE_NAME"].ToString();
                            model.COMPANY_ID = model.COMPANY_ID == 0 ? Convert.ToInt32(dataTable.Rows[0]["COMPANY_ID"].ToString()) : model.COMPANY_ID;
                            //model.UNIT_ID = Convert.ToInt32(dataTable.Rows[0]["UNIT_ID"].ToString());
                            //emailConfiguration.EmailBody_Password = generator.RandomPassword(8);

                            model.USER_PASSWORD = _commonService.Encrypt("123");

                            listOfQuery.Add(_commonService.AddQuery(AddOrUpdatyeInsertQuery(), _commonService.AddParameter(new string[] { model.USER_ID.ToString(), model.USER_TYPE, model.USER_PASSWORD.ToString(), model.USER_NAME, model.UNIT_ID.ToString(), model.ENTERED_TERMINAL, model.ENTERED_DATE.ToString(), model.ENTERED_BY.ToString(), model.EMPLOYEE_ID.ToString(), model.EMAIL, model.COMPANY_ID.ToString(), model.STATUS })));
                            model.UNIQUEACCESSKEY = generator.RandomPassword(12);
                            listOfQuery.Add(_commonService.AddQuery(UpdateUniqueKeyByUser(), _commonService.AddParameter(new string[] { model.UNIQUEACCESSKEY, model.USER_ID.ToString() })));
                            int defaultPageIdentity = _commonService.GetMaximumNumber<int>(connString.GetConnectionString(db), GetNewUSER_DEFAULT_PAGEIDQuery(), _commonService.AddParameter(new string[] { }));
                            listOfQuery.Add(_commonService.AddQuery(DeletePreviousDefaultQuery(), _commonService.AddParameter(new string[] { model.USER_ID.ToString() })));
                            listOfQuery.Add(_commonService.AddQuery(AddOrUpdateDefaultInsertPage(), _commonService.AddParameter(new string[] { defaultPageIdentity.ToString(), model.USER_ID.ToString(), model.COMPANY_ID.ToString(), "255", model.ENTERED_DATE.ToString(), model.ENTERED_BY.ToString(), model.ENTERED_TERMINAL })));


                            //emailConfiguration.Subject = "Email and User Account Verification";
                            //emailConfiguration.ToEmail = model.EMAIL;
                            //emailConfiguration.Title = "Email Verification";
                            //emailConfiguration.EmailBody_UserName = model.USER_NAME;
                            //emailConfiguration.EmailBody = "A unique link to reset your password has been generated for you. Please Login By Using Following Link ( User Name: " + emailConfiguration.EmailBody_UserName + " and Password: " + emailConfiguration.EmailBody_Password + " (Auto Generated)) and Change your password.";

                            //emailConfiguration.EmailBody_PageLink = url + "Security/User/AccountVerification?UniqueId=" + model.UNIQUEACCESSKEY;
                            //emailConfiguration.Body = _EmailService.BodyReader(emailConfiguration, Path);

                            //await _EmailService.SendEmailAsync(emailConfiguration);


                        }
                        else
                        {
                            model.USER_NAME = dataTable.Rows[0]["EMPLOYEE_NAME"].ToString();
                            listOfQuery.Add(_commonService.AddQuery(AddOrUpdateUpdateQuery(),
                                _commonService.AddParameter(new string[] { model.USER_ID.ToString(), model.USER_TYPE.ToString(), model.UPDATED_BY,model.UPDATED_DATE, model.UPDATED_TERMINAL, model.USER_NAME, model.EMAIL, model.STATUS
                                })));

                        }
                        await _commonService.SaveChangesAsyn(connString.GetConnectionString(db), listOfQuery);



                    }
                    else
                    {
                        return "Employee ID : " + model.EMPLOYEE_ID + " not found! Please Enter Valid Employee No.!";
                    }


                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

        }

        public int GetCompanyIdByUserId(string db, int userId)
        {
            int result = 0;
            DataTable userData = GetUserByUseridDataTable(db, userId);

            if (userData != null && userData.Rows.Count > 0)
            {

                return Convert.ToInt32(userData.Rows[0]["COMPANY_ID"].ToString());
            }
            return result;
        }


        //-------------------------Default Page--------------------------------------
        string LoadSearchableDefaultPagesQuery() => @"SELECT  M.MENU_ID, M.MENU_NAME || ' (' || M.CONTROLLER || '/' || M.ACTION || ')' DEFAULTPAGE
                              FROM MENU_CONFIGURATION M WHERE COMPANY_ID = :param1 AND M.STATUS = 'Active' 
                              AND (NVL(M.CONTROLLER, ' ') != ' ' OR NVL(M.ACTION,'') != ' ') AND
                              (M.MENU_NAME Like '%' || :param2 || '%' OR upper(M.CONTROLLER) Like '%' || upper(:param2) || '%' 
                              OR M.ACTION Like '%' || :param2 || '%')";
        string LoadDefaultPagesQuery() => @"Select distinct U.EMPLOYEE_ID,   D.ID,D.MENU_ID,D.COMPANY_ID,D.USER_ID, U.USER_NAME USER_NAME,
                               M.MENU_NAME || ' (' || M.CONTROLLER || '/' || M.ACTION || ')' MENU_NAME, TO_CHAR(D.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                               From USER_DEFAULT_PAGE D, Menu_Configuration M, Company_Info c,  User_info u   
                               Where D.COMPANY_ID = :param1 and M.MENU_ID = D.MENU_ID And c.COMPANY_ID = D.COMPANY_ID And U.USER_ID = D.USER_ID";
        string AddOrUpdateDefaultInsertPage() => @"INSERT INTO USER_DEFAULT_PAGE (
                           ID,
                           USER_ID,
                           COMPANY_ID,
                           MENU_ID,
                           ENTERED_DATE,
                           ENTERED_BY,
                           ENTERED_TERMINAL
                          ) 
                          VALUES(:param1 ,:param2  ,:param3  ,:param4, TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),  :param6,:param7 )";
        string AddOrUpdateDefaultUpdatePage() => @"UPDATE  USER_DEFAULT_PAGE SET MENU_ID = :param1,
                           UPDAETD_DATE = TO_DATE(:param2, 'DD/MM/YYYY HH:MI:SS AM'),
                           UPDATED_BY = :param3,
                           UPDATED_TERMINAL, = :param4 
                           Where ID = :param5";

        string UpdateUserStatusQuery() => @"UPDATE  USER_INFO SET STATUS = 'Active'
                           
                           Where USER_ID = :param1";
        string DeletePreviousDefaultQuery() => @"DELETE from USER_DEFAULT_PAGE Where User_ID = :param1";
        string GetNewUSER_DEFAULT_PAGEIDQuery() => "SELECT NVL(MAX(ID),0) + 1 USER_ID  FROM USER_DEFAULT_PAGE";
        public async Task<string> LoadSearchableDefaultPages(string db, int companyId, string defaultpage) => _commonService.DataTableToJSON(await _commonService.GetDataTableAsyn(connString.GetConnectionString(db), LoadSearchableDefaultPagesQuery(), _commonService.AddParameter(new string[] { companyId.ToString(), defaultpage })));
        public async Task<string> LoadDefaultPages(string db, int companyId) => _commonService.DataTableToJSON(await _commonService.GetDataTableAsyn(connString.GetConnectionString(db), LoadDefaultPagesQuery(), _commonService.AddParameter(new string[] { companyId.ToString() })));
        public async Task<string> AddOrUpdateDefaultPage(string db, Default_Page model)
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

                        model.ID = _commonService.GetMaximumNumber<int>(connString.GetConnectionString(db), GetNewUSER_DEFAULT_PAGEIDQuery(), _commonService.AddParameter(new string[] { }));
                        listOfQuery.Add(_commonService.AddQuery(DeletePreviousDefaultQuery(), _commonService.AddParameter(new string[]
                        {model.USER_ID.ToString()})));
                        listOfQuery.Add(_commonService.AddQuery(AddOrUpdateDefaultInsertPage(), _commonService.AddParameter(new string[]
                        {
                            model.ID.ToString(), model.USER_ID.ToString(), model.COMPANY_ID.ToString(), model.MENU_ID.ToString(),model.ENTERED_DATE.ToString(), model.ENTERED_BY.ToString(),model.ENTERED_TERMINAL
                        })));


                    }
                    else
                    {
                        listOfQuery.Add(_commonService.AddQuery(AddOrUpdateDefaultUpdatePage(),
                            _commonService.AddParameter(new string[] { model.MENU_ID.ToString(), model.UPDATED_DATE.ToString(), model.UPDATED_BY, model.UPDATED_TERMINAL, model.ID.ToString()
                            })));

                    }

                    await _commonService.SaveChangesAsyn(connString.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

        }

        public User_Info IsVerified(string db, string UniquKey)
        {
            User_Info auth = new User_Info();
            DataTable dataTable = _commonService.GetDataTable(connString.GetConnectionString(db), "SELECT USER_ID, USER_NAME, EMPLOYEE_ID FROM USER_INFO Where UNIQUEACCESSKEY = :param1", _commonService.AddParameter(new string[] { UniquKey }));
            if (dataTable.Rows.Count > 0)
            {
                auth.USER_ID = Convert.ToInt32(dataTable.Rows[0]["USER_ID"]);
                auth.USER_NAME = dataTable.Rows[0]["USER_NAME"].ToString();


                auth.EMPLOYEE_ID = dataTable.Rows[0]["EMPLOYEE_ID"].ToString();
                List<QueryPattern> listOfQuery = new List<QueryPattern>();

                listOfQuery.Add(_commonService.AddQuery(UpdateUserStatusQuery(),
                            _commonService.AddParameter(new string[] { auth.USER_ID.ToString()
                            })));
                _commonService.SaveChanges(connString.GetConnectionString(db), listOfQuery);
                return auth;


            }

            return new User_Info();


        }

        //-----------------User password Update--------------------------

        string UpdatePassword() => @"UPDATE USER_INFO SET USER_PASSWORD =:param1  WHERE USER_ID = :param2";

        public async Task<string> UpdateUserPassword(string db, PasswordChangeModel changeModel)
        {

            if (changeModel.Password == changeModel.PasswordCopy && changeModel.USER_ID != 0)
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();

                listOfQuery.Add(_commonService.AddQuery(UpdatePassword(),
                            _commonService.AddParameter(new string[] { _commonService.Encrypt(changeModel.Password), changeModel.USER_ID.ToString()
                            })));
                if (changeModel.MailCredential == true)
                {
                    EmailConfiguration emailConfiguration = new EmailConfiguration();
                    emailConfiguration.Subject = "Password Update Credential";
                    emailConfiguration.ToEmail = changeModel.Email;
                    emailConfiguration.Title = "Password Update Credential";

                    emailConfiguration.EmailBody_UserName = changeModel.User_Name;
                    emailConfiguration.EmailBody_PageLink = changeModel.BaseUrl + "Security/Login/Index";
                    emailConfiguration.EmailBody_Password = changeModel.Password;

                    emailConfiguration.EmailBody = "You have successfully updated your password. Please Login By Using Following Credential ( User Name: " + emailConfiguration.EmailBody_UserName + " and Password: " + emailConfiguration.EmailBody_Password + ")";

                    emailConfiguration.Body = _EmailService.BodyReader(emailConfiguration, changeModel.Path);
                    await _EmailService.SendEmailAsync(emailConfiguration);

                }
                await _commonService.SaveChangesAsyn(connString.GetConnectionString(db), listOfQuery);

                return "Password Changed successfully!!";
            }


            return "Password does not match. Please try again";


        }

        public async Task<string> ForgetPasswordVerify(string db, PasswordChangeModel model)
        {
            if (model.USER_ID > 0)
            {
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                EmailConfiguration emailConfiguration = new EmailConfiguration();
                RandomStringGenerator generator = new RandomStringGenerator();
                emailConfiguration.EmailBody_Password = generator.RandomPassword(8);
                string USER_PASSWORD = _commonService.Encrypt(emailConfiguration.EmailBody_Password);
                string UniqueKey = generator.RandomPassword(12);
                listOfQuery.Add(_commonService.AddQuery(UpdatePassWordAndKeyByUser(), _commonService.AddParameter(new string[] { UniqueKey, model.USER_ID.ToString(), USER_PASSWORD })));

                emailConfiguration.Subject = "Email and User Account Verification";
                emailConfiguration.ToEmail = model.Email;
                emailConfiguration.Title = "Email Verification";
                emailConfiguration.EmailBody_UserName = model.Email;
                emailConfiguration.EmailBody_PageLink = model.BaseUrl + "Security/User/AccountVerification?UniqueId=" + UniqueKey;

                emailConfiguration.EmailBody = "A unique link to verify your account has been generated for you. Please verify your account by using the following link. After verification, Please Log In ( User Name: " + emailConfiguration.EmailBody_UserName + " and Password: " + emailConfiguration.EmailBody_Password + " (Auto Generated))  and change your password if needed.";

                emailConfiguration.Body = _EmailService.BodyReader(emailConfiguration, model.Path);

                await _EmailService.SendEmailAsync(emailConfiguration);

                await _commonService.SaveChangesAsyn(connString.GetConnectionString(db), listOfQuery);


                return "Please check your email and follow the steps as instructed. Thank you.";

            }
            else
            {
                return "Please Enter Valid Email Address!";
            }

        }

        public List<User_Info> GetPasswords(string db)
        {
            var query = @"SELECT * FROM USER_INFO";
            var data = _commonService.GetDataTable(connString.GetConnectionString(db), query, _commonService.AddParameter(new string[] { })).ToList<User_Info>();
            foreach (var item in data)
            {
                item.USER_PASSWORD = _commonService.Decrypt(item.USER_PASSWORD);
            }

            return data;
        }

        //im//
        public async Task<string> GetAllCodeAndDropdownListData(string db, string companyId, string unitId)
        {
            List<string> queries = new List<string>
            {
                "SELECT VALUE, NAME, SHORT_NAME, TYPE, STATUS FROM CODE WHERE UPPER(STATUS)='ACTIVE' AND UPPER(USE_IN_FORM) ='EMPLOYEE' ORDER BY NAME",
                "SELECT UNIT_ID, UNIT_NAME FROM COMPANY_INFO WHERE UPPER(STATUS)='ACTIVE'",
                "SELECT E.ID, E.EMPLOYEE_ID, E.EMPLOYEE_CODE, E.EMPLOYEE_NAME, E.EMPLOYEE_STATUS, E.COMPANY_ID, E.UNIT_ID FROM EMPLOYEE_INFO E WHERE     E.EMPLOYEE_ID NOT IN (SELECT EMPLOYEE_ID FROM USER_INFO) AND E.COMPANY_ID = :param1"

            };
            List<Dictionary<string, string>> parametersList = new List<Dictionary<string, string>>
            {
                _commonService.AddParameter(new string[] {}),
                _commonService.AddParameter(new string[] {}),
                _commonService.AddParameter(new string[] {companyId})

            };
            var dataSet = await _commonService.GetDataSetForMultiQueryWithParamAsync(
                connString.GetConnectionString(db), queries, parametersList
            );
            return _commonService.DataSetToJSON(dataSet);
        }

        //im//
        public async Task<string> ActivateUser(string db, int id)
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

                    listOfQuery.Add(_commonService.AddQuery("UPDATE USER_INFO SET  STATUS = '" + Status.Active + "' WHERE USER_ID = :param1", _commonService.AddParameter(new string[] { id.ToString() })));

                    await _commonService.SaveChangesAsyn(connString.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<string> DeactivateUser(string db, int id)
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
                    listOfQuery.Add(_commonService.AddQuery("UPDATE USER_INFO SET  STATUS = '" + Status.InActive + "' WHERE USER_ID = :param1", _commonService.AddParameter(new string[] { id.ToString() })));
                    await _commonService.SaveChangesAsyn(connString.GetConnectionString(db), listOfQuery);
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
