using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SalesAndDistributionSystem.Domain.Common;
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
    public class CompanyManager : ICompanyManager
    {
        private readonly IConfiguration connString;
        private readonly ICommonServices _commonService;
        private readonly IConfiguration _configuration;

        public CompanyManager(IConfiguration connstring, ICommonServices commonServices, IConfiguration configuration)
        {
            connString = connstring;
            _commonService = commonServices;
            _configuration = configuration;
        }

        string GetCompanyListQuery() => "Select distinct  COMPANY_ADDRESS1,COMPANY_ADDRESS2,COMPANY_ID,COMPANY_NAME,COMPANY_SHORT_NAME from Company_Info";
        string GetCompanyByIdQuery() => "Select distinct  COMPANY_ADDRESS1,COMPANY_ADDRESS2,COMPANY_ID,COMPANY_NAME,COMPANY_SHORT_NAME from Company_Info Where Company_ID = :param1";
        string AddOrUpdateCompanyInsertQuery() => @"INSERT INTO Company_Info (
                                         ID
                                        ,COMPANY_ID
                                        ,COMPANY_NAME
                                        ,COMPANY_SHORT_NAME
                                        ,COMPANY_ADDRESS1
                                        ,COMPANY_ADDRESS2
                                        ,UNIT_ID
                                       ) 
                                       VALUES ( :param1, :param2, :param3, :param4, :param5 , :param6, :param7 )";
        string AddOrUpdateCompanyUpdateQuery() => @"Update Company_Info Set COMPANY_NAME = :param1,COMPANY_SHORT_NAME  =:param2, COMPANY_ADDRESS1 =  :param3, COMPANY_ADDRESS2 = :param4 Where COMPANY_ID = :param5";
        string GetNewCompanyIdQuery() => "SELECT NVL(MAX(COMPANY_ID),0) + 1 COMPANY_ID  FROM COMPANY_INFO";
        string GetNewIdQuery() => "SELECT NVL(MAX(ID),0) + 1 ID  FROM COMPANY_INFO";

        public async Task<DataTable> GetCompanyListDataTable(string db) => await _commonService.GetDataTableAsyn(connString.GetConnectionString(db), GetCompanyListQuery(), _commonService.AddParameter(new string[] { }));
        public async Task<DataTable> GetCompanyByIdDataTable(string db, int id) => await _commonService.GetDataTableAsyn(connString.GetConnectionString(db), GetCompanyByIdQuery(), _commonService.AddParameter(new string[] { id.ToString() }));

        public async Task<string> GetCompanyJsonList(string db) => _commonService.DataTableToJSON(await GetCompanyListDataTable(db));
       
        public async Task<List<Company_Info>> GetCompanyList(string db)
        {
            DataTable companyData = await GetCompanyListDataTable(db);

            if (companyData != null && companyData.Rows.Count > 0)
            {
                List<Company_Info> company_Infos = new List<Company_Info>();
                foreach (DataRow row in companyData.Rows)
                {
                    Company_Info company = new Company_Info
                    {
                        COMPANY_ID = Convert.ToInt32(row["COMPANY_ID"]),
                        COMPANY_NAME = row["COMPANY_NAME"].ToString(),
                        COMPANY_SHORT_NAME = row["COMPANY_SHORT_NAME"].ToString(),
                        COMPANY_ADDRESS1 = row["COMPANY_ADDRESS1"].ToString(),
                        COMPANY_ADDRESS2 = row["COMPANY_ADDRESS2"].ToString()
                    };

                    company_Infos.Add(company);
                }
              

                return company_Infos;

            }
            return null;
        }

        public async Task<Company_Info> GetCompanyById(string db, int id)
        {
            DataTable companyData = await GetCompanyByIdDataTable(db, id);

            if (companyData != null && companyData.Rows.Count > 0)
            {
                
                    Company_Info company = new Company_Info
                    {
                        COMPANY_ID = Convert.ToInt32(companyData.Rows[0]["COMPANY_ID"]),
                        COMPANY_NAME = companyData.Rows[0]["COMPANY_NAME"].ToString(),
                        COMPANY_SHORT_NAME = companyData.Rows[0]["COMPANY_SHORT_NAME"].ToString(),
                        COMPANY_ADDRESS1 = companyData.Rows[0]["COMPANY_ADDRESS1"].ToString(),
                        COMPANY_ADDRESS2 = companyData.Rows[0]["COMPANY_ADDRESS2"].ToString()
                    };

                


                return company;

            }
            return null;
        }

        public async Task<string> AddOrUpdate(string db, Company_Info model)
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

                    if (model.COMPANY_ID == 0)
                    {
                        model.STATUS = Status.InActive;
                        model.ID = _commonService.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewIdQuery(), _commonService.AddParameter(new string[] { }));
                        model.COMPANY_ID = _commonService.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewCompanyIdQuery(), _commonService.AddParameter(new string[] { }));

                        model.UNIT_ID = 0;

                        listOfQuery.Add(_commonService.AddQuery(AddOrUpdateCompanyInsertQuery(), _commonService.AddParameter(new string[]
                        {model.ID.ToString(), model.COMPANY_ID.ToString(), model.COMPANY_NAME.ToString(), model.COMPANY_SHORT_NAME, 
                            model.COMPANY_ADDRESS1, model.COMPANY_ADDRESS2, model.UNIT_ID.ToString() })));

                    }
                    else
                    {
                        listOfQuery.Add(_commonService.AddQuery(AddOrUpdateCompanyUpdateQuery(),
                            _commonService.AddParameter(new string[] { model.COMPANY_NAME, model.COMPANY_SHORT_NAME,
                                model.COMPANY_ADDRESS1.ToString(), model.COMPANY_ADDRESS2, model.COMPANY_ID.ToString()
                            })));

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
        //-------------------------- Unit ---------------------------------------

        string GetUnitListQuery() => "Select distinct ID, UNIT_ID, UNIT_ADDRESS1, UNIT_ADDRESS2, COMPANY_ID,COMPANY_NAME,UNIT_TYPE, UNIT_SHORT_NAME, UNIT_NAME,STATUS from Company_Info Where UNIT_ID!=0";
        string GetUnitListByCompanyIdQuery() => "Select distinct ID, UNIT_ID, UNIT_ADDRESS1, UNIT_ADDRESS2, COMPANY_ID,COMPANY_NAME,UNIT_TYPE, UNIT_SHORT_NAME, UNIT_NAME,STATUS from Company_Info Where UNIT_ID!=0 AND COMPANY_ID = :param1";
        string GetCompanyInfoByUnitIdQuery() => "SELECT * FROM COMPANY_INFO WHERE COMPANY_ID=:param1 and UNIT_ID=:param2";
        string AddOrUpdateUnitInsertQuery() => @"INSERT INTO Company_Info (
                                         ID
                                        ,COMPANY_ID
                                        ,COMPANY_NAME
                                        ,COMPANY_SHORT_NAME
                                        ,COMPANY_ADDRESS1
                                        ,COMPANY_ADDRESS2
                                        ,UNIT_ID
                                        ,UNIT_NAME
                                        ,UNIT_SHORT_NAME
                                        ,UNIT_TYPE
                                        ,UNIT_ADDRESS1
                                        ,UNIT_ADDRESS2
                                        ,STATUS
                                       ) 
                                       VALUES ( :param1, :param2, :param3, :param4, :param5 , :param6, :param7 ,:param8, :param9, :param10, :param11, :param12 , :param13 )";
        string AddOrUpdateUnitUpdateLikeQuery() => @"Update Company_Info Set UNIT_NAME = :param1,UNIT_SHORT_NAME  =:param2, UNIT_TYPE =  :param3, UNIT_ADDRESS1 = :param4, UNIT_ADDRESS2 = :param5, UNIT_ID = :param6, STATUS = 'Active' Where COMPANY_ID = :param7 AND ID = :param8";

        string AddOrUpdateUnitUpdateQuery() => @"Update Company_Info Set UNIT_NAME = :param1,UNIT_SHORT_NAME  =:param2, UNIT_TYPE =  :param3, UNIT_ADDRESS1 = :param4, UNIT_ADDRESS2 = :param5 Where  ID = :param6";
        string GetUpdatableCompanyQuery() => "Select distinct ID,  COMPANY_ADDRESS1,COMPANY_ADDRESS2,COMPANY_ID,COMPANY_NAME,COMPANY_SHORT_NAME from Company_Info Where Unit_ID = 0 AND COMPANY_ID = :param1";
        string GetNewUnitIdQuery() => "SELECT NVL(MAX(UNIT_ID),0) + 1 UNIT_ID  FROM COMPANY_INFO WHERE COMPANY_ID = :param1";
        string ActivateUnitQuery() => "Update Company_Info Set STATUS = 'Active' WHERE ID =  :param1";
        string DeActivateUnitQuery() => "Update Company_Info Set STATUS = 'InActive' WHERE ID =  :param1";
       
        public async Task<DataTable> GetUnitListDataTable(string db) => await _commonService.GetDataTableAsyn(connString.GetConnectionString(db), GetUnitListQuery(), _commonService.AddParameter(new string[] { }));
        public async Task<string> GetUnitJsonList(string db) => _commonService.DataTableToJSON(await GetUnitListDataTable(db));
        public async Task<string> GetUnitByCompanyId(string db,int Company_Id)
        {
          DataTable dataTable = await  _commonService.GetDataTableAsyn(connString.GetConnectionString(db), GetUnitListByCompanyIdQuery(), _commonService.AddParameter(new string[] { Company_Id.ToString() }));

            return _commonService.DataTableToJSON(dataTable);

        }
        public async Task<string> GetUnitInfo(string db, int companyId, int unitId)
        {
            var dt = await _commonService.GetDataTableAsyn(connString.GetConnectionString(db), GetCompanyInfoByUnitIdQuery(), _commonService.AddParameter(new string[] { companyId.ToString(), unitId.ToString() }));
            return JsonConvert.SerializeObject(dt);
        }

        public async Task<DataTable> GetCompUpdatableDataTable(string db, int companyId) => await _commonService.GetDataTableAsyn(connString.GetConnectionString(db), GetUpdatableCompanyQuery(), _commonService.AddParameter(new string[] { companyId.ToString() }));

        public async Task<string> AddOrUpdateUnit(string db, Company_Info model)
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

                    if (model.UNIT_ID == 0)
                    {
                        DataTable existingCompany = await GetCompUpdatableDataTable(db, model.COMPANY_ID);
                      
                        if(existingCompany.Rows.Count>0)
                        {
                            model.UNIT_ID = _commonService.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewUnitIdQuery(), _commonService.AddParameter(new string[] { model.COMPANY_ID.ToString() }));
                            model.ID = Convert.ToInt32(existingCompany.Rows[0]["ID"]);
                            listOfQuery.Add((_commonService.AddQuery(AddOrUpdateUnitUpdateLikeQuery(), _commonService.AddParameter(new string[]
                            { 
                            model.UNIT_NAME.ToString(), model.UNIT_SHORT_NAME.ToString(), model.UNIT_TYPE,
                            model.UNIT_ADDRESS1, model.UNIT_ADDRESS2, model.UNIT_ID.ToString(), model.COMPANY_ID.ToString(), model.ID.ToString()  }))));
                        }
                        else
                        {
                             Company_Info company = await   GetCompanyById(db, model.COMPANY_ID);
                            model.STATUS = Status.Active;
                            model.COMPANY_ADDRESS1 = company.COMPANY_ADDRESS1;
                            model.COMPANY_ADDRESS2 = company.COMPANY_ADDRESS2;
                            model.COMPANY_NAME = company.COMPANY_NAME;
                            model.COMPANY_SHORT_NAME = company.COMPANY_SHORT_NAME;
                            model.UNIT_ID = _commonService.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewUnitIdQuery(), _commonService.AddParameter(new string[] { model.COMPANY_ID.ToString() }));
                            model.ID = _commonService.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewIdQuery(), _commonService.AddParameter(new string[] { }));

                            listOfQuery.Add((_commonService.AddQuery(AddOrUpdateUnitInsertQuery(), _commonService.AddParameter(new string[]
                   {model.ID.ToString(), model.COMPANY_ID.ToString(),model.COMPANY_NAME, model.COMPANY_SHORT_NAME.ToString(), model.COMPANY_ADDRESS1,
                            model.COMPANY_ADDRESS2, model.UNIT_ID.ToString(), model.UNIT_NAME, model.UNIT_SHORT_NAME.ToString(), model.UNIT_TYPE.ToString(),model.UNIT_ADDRESS1, model.UNIT_ADDRESS2, model.STATUS  }))));

                        }
                       
                    }
                    else
                    {
                        listOfQuery.Add(_commonService.AddQuery(AddOrUpdateUnitUpdateQuery(),
                            _commonService.AddParameter(new string[] { model.UNIT_NAME, model.UNIT_SHORT_NAME,
                                model.UNIT_TYPE.ToString(), model.UNIT_ADDRESS1, model.UNIT_ADDRESS2, model.ID.ToString()
                            })));

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
        public async Task<string> ActivateUnit(string db, int id)
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

                    listOfQuery.Add(_commonService.AddQuery(ActivateUnitQuery(), _commonService.AddParameter(new string[] { id.ToString() })));

                    await _commonService.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<string> DeactivateUnit(string db, int id)
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


                    listOfQuery.Add(_commonService.AddQuery(DeActivateUnitQuery(), _commonService.AddParameter(new string[] { id.ToString() })));

                    await _commonService.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                    return "1";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        //im//
        public async Task<string> GetCompanyAndItsUnit(string db, string companyId)
        {
            List<string> queries = new List<string>
            {
                "SELECT DISTINCT COMPANY_ID, COMPANY_NAME, COMPANY_SHORT_NAME, COMPANY_ADDRESS1, COMPANY_ADDRESS2,CURRENCY FROM COMPANY_INFO WHERE COMPANY_ID=:param1",
                "SELECT UNIT_ID, UNIT_NAME, UNIT_SHORT_NAME, UNIT_ADDRESS1, UNIT_ADDRESS2, UNIT_TYPE FROM COMPANY_INFO WHERE COMPANY_ID=:param1",
            };
            List<Dictionary<string, string>> parametersList = new List<Dictionary<string, string>>
            {
                _commonService.AddParameter(new string[] {companyId }),
                _commonService.AddParameter(new string[] {companyId })
               };
            var dataSet = await _commonService.GetDataSetForMultiQueryWithParamAsync(
                _configuration.GetConnectionString(db), queries, parametersList
            );
            return _commonService.DataSetToJSON(dataSet);
        }
        
    }
}
