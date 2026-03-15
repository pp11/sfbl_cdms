using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.Manager
{
    public class DistributionRouteManager: IDistributionRouteManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public DistributionRouteManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;
        }

        string NewId() => "SELECT NVL(MAX(DIST_ROUTE_ID),0) + 1 DIST_ROUTE_ID FROM DISTRIBUTION_ROUTE_INFO";
        string LoadData_Query() => @"SELECT D.*, (SELECT C.COMPANY_NAME FROM COMPANY_INFO C WHERE COMPANY_ID = D.COMPANY_ID AND ROWNUM = 1) AS COMPANY_NAME FROM DISTRIBUTION_ROUTE_INFO D
            WHERE D.COMPANY_ID = :param1";
        string AddOrUpdate_AddQuery() => @"INSERT INTO DISTRIBUTION_ROUTE_INFO 
                (DIST_ROUTE_ID, DIST_ROUTE_NAME, STATUS, COMPANY_ID, REMARKS, ENTERED_BY, ENTERED_DATE, ENTERED_TERMINAL)
                VALUES (:param1, :param2, :param3, :param4, :param5, :param6, TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), :param8)";

        string AddOrUpdate_UpdateQuery() => @"UPDATE DISTRIBUTION_ROUTE_INFO 
                SET
                DIST_ROUTE_NAME = :param2,
                STATUS = :param3,
                COMPANY_ID = :param4,
                REMARKS = :param5,
                UPDATED_BY = :param6,
                UPDATED_DATE = TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'),
                UPDATED_TERMINAL = :param8
                WHERE DIST_ROUTE_ID = :param1";

        public async Task<string> AddOrUpdate(string db, Distribution_Route_Info model)
        {
            List<QueryPattern> listOfQuery = new List<QueryPattern>();
            var newId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), NewId(), _commonServices.AddParameter(new string[] { }));

            if (model.DIST_ROUTE_ID == 0)
            {
                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                    _commonServices.AddParameter(new string[] {
                                newId.ToString(),
                                model.DIST_ROUTE_NAME,
                                model.STATUS,
                                model.COMPANY_ID.ToString(),
                                model.REMARKS,
                                model.ENTERED_BY,
                                model.ENTERED_DATE,
                                model.ENTERED_TERMINAL,
                    })));
            }
            else
            {
                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQuery(),
                    _commonServices.AddParameter(new string[] {
                                model.DIST_ROUTE_ID.ToString(),
                                model.DIST_ROUTE_NAME,
                                model.STATUS,
                                model.COMPANY_ID.ToString(),
                                model.REMARKS,
                                model.UPDATED_BY,
                                model.UPDATED_DATE,
                                model.UPDATED_TERMINAL,
                    })));
            }

            await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
            return "1";
        }

        public async Task<string> LoadData(string db, int companyId)
        {
            var data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Query(),
                _commonServices.AddParameter(new string[] { companyId.ToString() }));

            DataSet ds = new DataSet();
            ds.Tables.Add(data);

            return _commonServices.DataSetToJSON(ds);
        }
    }
}
