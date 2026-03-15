using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.Manager
{
    public class SupplierManager : ISupplierManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public SupplierManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;
        }


        //-------------------Query Part ---------------------------------------------------

        string LoadData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.SUPPLIER_ID ASC) AS ROW_NO,
                                    SUPPLIER_ID, SUPPLIER_NAME, ADDRESS, EMAIL, PHONE_NO, MOBILE_NO, STATUS, COMPANY_ID, REMARKS, TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE, SUPPLIER_TYPE
                                    FROM SUPPLIER_INFO  M  Where M.COMPANY_ID = :param1";
        string LoadSearchableData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.SUPPLIER_ID ASC) AS ROW_NO,
                                    SUPPLIER_ID, SUPPLIER_NAME, ADDRESS, EMAIL, PHONE_NO, MOBILE_NO, STATUS, COMPANY_ID, REMARKS, TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE, SUPPLIER_TYPE
                                    FROM SUPPLIER_INFO  M  Where M.COMPANY_ID = :param1 AND upper(M.SUPPLIER_NAME) Like '%' || upper(:param2) || '%'";

        string LoadSupplierByType_Query() => @"SELECT * FROM SUPPLIER_LIST
                                WHERE COMPANY_ID = :param1 AND STATUS = 'Active'";

        string AddOrUpdate_AddQuery() => @"INSERT INTO SUPPLIER_INFO 
                                         (SUPPLIER_ID, SUPPLIER_NAME, ADDRESS, EMAIL, PHONE_NO, MOBILE_NO, STATUS, COMPANY_ID, REMARKS,  Entered_By, Entered_Date,ENTERED_TERMINAL, SUPPLIER_TYPE ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8, :param9, :param10, TO_DATE(:param11, 'DD/MM/YYYY HH:MI:SS AM'), :param12, :param13 )";
        string AddOrUpdate_UpdateQuery() => @"UPDATE SUPPLIER_INFO SET 
                                            SUPPLIER_NAME = :param2,ADDRESS = :param3,EMAIL = :param4,PHONE_NO = :param5,
                                            MOBILE_NO = :param6,STATUS = :param7,COMPANY_ID = :param8,REMARKS = :param9,
                                            UPDATED_BY = :param10, UPDATED_DATE = TO_DATE(:param11, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param12,
                            SUPPLIER_TYPE = :param13
                            WHERE SUPPLIER_ID = :param1";
        string GetNewSupplier_Info_IdQuery() => "SELECT NVL(MAX(SUPPLIER_ID),0) + 1 SUPPLIER_ID  FROM SUPPLIER_INFO";
        string Get_LastSupplier_Info() => "SELECT  SUPPLIER_ID  FROM SUPPLIER_INFO Where  SUPPLIER_ID = (SELECT NVL(MAX(SUPPLIER_ID),0) SUPPLIER_ID From SUPPLIER_INFO where COMPANY_ID = :param1 )";


        //---------- Method Execution Part ------------------------------------------------

        public async Task<string> AddOrUpdate(string db, Supplier_Info model)
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

                    if (model.SUPPLIER_ID == 0)
                    {
                        model.SUPPLIER_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewSupplier_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));
                        //                                                                                                           (SUPPLIER_ID, SUPPLIER_NAME, ADDRESS, EMAIL, PHONE_NO, MOBILE_NO, STATUS, COMPANY_ID, REMARKS,  Entered_By, Entered_Date,ENTERED_TERMINAL ) 

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(), _commonServices.AddParameter(new string[] { model.SUPPLIER_ID.ToString(), model.SUPPLIER_NAME, model.ADDRESS, model.EMAIL, model.PHONE_NO, model.MOBILE_NO, model.STATUS, model.COMPANY_ID.ToString(), model.REMARKS, model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL, model.SUPPLIER_TYPE }))); ;
                    }
                    else
                    {
                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                                model.SUPPLIER_ID.ToString(), model.SUPPLIER_NAME, model.ADDRESS, model.EMAIL, model.PHONE_NO, model.MOBILE_NO, model.STATUS, model.COMPANY_ID.ToString(),model.REMARKS,
                                model.UPDATED_BY.ToString(), model.UPDATED_DATE,
                                model.UPDATED_TERMINAL, model.SUPPLIER_TYPE })));
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
        public async Task<string> LoadData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> GetSearchableSupplier(string db, int Company_Id, string Supplier) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Supplier })));

        public async Task<string> GetSupplierByType(string db, int Company_Id, string supplierType)
        {
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSupplierByType_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        }
    }
}
