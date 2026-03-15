using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Common;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.Manager
{
    public class CollectionReverseManager : ICollectionReverseManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;

        public CollectionReverseManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;
        }

        public async Task<string> GetTransactions(string db, string batch_no)
        {
            var query = @"SELECT A.COLLECTION_DTL_ID,
                   B.COLLECTION_MST_ID,
                   B.BATCH_NO,
                   TO_CHAR(B.BATCH_DATE, 'DD/MM/YYYY') BATCH_DATE,
                   B.BATCH_STATUS,
                   A.CUSTOMER_ID,
                   A.CUSTOMER_CODE,
                   FN_CUSTOMER_NAME(A.COMPANY_ID, CUSTOMER_CODE) CUSTOMER_NAME,
                   A.VOUCHER_NO,
                   TO_CHAR(A.VOUCHER_DATE, 'DD/MM/YYYY') VOUCHER_DATE,
                   A.INVOICE_NO,
                   A.COLLECTION_MODE,
                   A.COLLECTION_AMT,
                   A.TDS_AMT,
                   A.MEMO_COST,
                   A.NET_COLLECTION_AMT,
                   A.UNIT_ID,
                   FN_UNIT_NAME(A.COMPANY_ID, A.UNIT_ID) UNIT_NAME,
                   A.BANK_ID,
                   FN_BANK_NAME(A.BANK_ID) BANK_NAME,
                   A.BRANCH_ID,
                   FN_BRANCH_NAME(A.BANK_ID, A.BRANCH_ID) BRANCH_NAME
              FROM COLLECTION_DTL A, COLLECTION_MST B
             WHERE  B.BATCH_NO=:param1 and   A.COLLECTION_MST_ID = B.COLLECTION_MST_ID
                   AND B.BATCH_POSTING_STATUS = 'Approved'
                   AND A.COLLECTION_DTL_ID NOT IN (SELECT COLLECTION_DTL_ID FROM COLLECTION_REVERSE)
             ORDER BY A.COLLECTION_DTL_ID DESC";
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { batch_no })));
        }

        public async Task<string> LoadData(string db, string Company_Id, string Unit_Id)
        {
            var query = @"SELECT R.COLL_REVERSE_ID,
       R.BATCH_NO,
       TO_CHAR(R.BATCH_DATE, 'DD/MM/YYYY') BATCH_DATE ,
       R.CUSTOMER_ID,
       R.CUSTOMER_CODE,
       C.CUSTOMER_NAME,
       C.CUSTOMER_ADDRESS,
       R.VOUCHER_NO,
       TO_CHAR(R.VOUCHER_DATE, 'DD/MM/YYYY') VOUCHER_DATE ,
       R.BANK_ID,
       R.BRANCH_ID,
       R.COLLECTION_MODE,
       R.COLLECTION_AMT,
       R.TDS_AMT,
       R.MEMO_COST,
       R.NET_COLLECTION_AMT,
       R.ENTERED_BY,
       TO_CHAR(R.ENTERED_DATE, 'DD/MM/YYYY HH:MI:SS AM') ENTERED_DATE ,
       R.ENTERED_TERMINAL,
       R.REMARKS,
       TO_CHAR(R.REVERSE_DATE, 'DD/MM/YYYY') REVERSE_DATE ,
       R.COLLECTION_DTL_ID
  FROM COLLECTION_REVERSE R, CUSTOMER_INFO C 
  WHERE C.CUSTOMER_ID= R.CUSTOMER_ID ORDER BY COLL_REVERSE_ID DESC";
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] {  })));

        }

        public async Task<string> Save(string db, COLLECTION_REVERSE model)
        {
            DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT D.COLLECTION_DTL_ID, NVL(R.COLL_REVERSE_ID,0) COLL_REVERSE_ID
FROM COLLECTION_DTL D
LEFT OUTER JOIN COLLECTION_REVERSE R ON R.COLLECTION_DTL_ID=D.COLLECTION_DTL_ID
WHERE D.COLLECTION_DTL_ID= :param1", _commonServices.AddParameter(new string[] { model.COLLECTION_DTL_ID.ToString() }));
            if (dataTable.Rows[0]["COLL_REVERSE_ID"].ToString() != "0")
            {
                return "You are not able to Reverse this collection as it has already been Reversed.";
               
            }
            else
            {
                var maxIdQuery = @"SELECT NVL(MAX(COLL_REVERSE_ID),0) + 1 COLL_REVERSE_ID FROM COLLECTION_REVERSE";
                model.COLL_REVERSE_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), maxIdQuery, _commonServices.AddParameter(new string[] { }));
                var query = @"INSERT INTO COLLECTION_REVERSE (COLL_REVERSE_ID,
                                BATCH_NO,
                                BATCH_DATE,
                                CUSTOMER_ID,
                                CUSTOMER_CODE,
                                VOUCHER_NO,
                                VOUCHER_DATE,
                                BANK_ID,
                                BRANCH_ID,
                                COLLECTION_MODE,
                                COLLECTION_AMT,
                                TDS_AMT,
                                MEMO_COST,
                                NET_COLLECTION_AMT,
                                COMPANY_ID,
                                UNIT_ID,
                                REMARKS,
                                REVERSE_DATE,
                                ENTERED_BY,
                                ENTERED_DATE,
                                ENTERED_TERMINAL,
                                COLLECTION_DTL_ID)
                        VALUES (:param1,
                                :param2,
                                TO_DATE(:param3, 'DD/MM/YYYY'),
                                :param4,
                                :param5,
                                :param6,
                                TO_DATE(:param7, 'DD/MM/YYYY'),
                                :param8,
                                :param9,
                                :param10,
                                :param11,
                                :param12,
                                :param13,
                                :param14,
                                :param15,
                                :param16,
                                :param17,
                                TO_DATE(:param18, 'DD/MM/YYYY'),
                                :param19,
                                TO_DATE(:param20, 'DD/MM/YYYY HH:MI:SS AM'),
                                :param21,
                                :param22)";
                List<QueryPattern> listOfQuery = new List<QueryPattern>{_commonServices.AddQuery(query, _commonServices.AddParameter(new string[] {
                        model.COLL_REVERSE_ID.ToString(),
                        model.BATCH_NO?.ToString(),
                        model.BATCH_DATE?.ToString(),
                        model.CUSTOMER_ID.ToString(),
                        model.CUSTOMER_CODE?.ToString(),
                        model.VOUCHER_NO?.ToString(),
                        model.VOUCHER_DATE?.ToString(),
                        model.BANK_ID.ToString(),
                        model.BRANCH_ID.ToString(),
                        model.COLLECTION_MODE?.ToString(),
                        model.COLLECTION_AMT.ToString(),
                        model.TDS_AMT.ToString(),
                        model.MEMO_COST.ToString(),
                        model.NET_COLLECTION_AMT.ToString(),
                        model.COMPANY_ID.ToString(),
                        model.UNIT_ID.ToString(),
                        model.REMARKS?.ToString(),
                        model.REVERSE_DATE?.ToString(),
                        model.ENTERED_BY?.ToString(),
                        model.ENTERED_DATE?.ToString(),
                        model.ENTERED_TERMINAL?.ToString(),
                        model.COLLECTION_DTL_ID.ToString()
                }))};
                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                return "1";
            }


        }


    }
}
