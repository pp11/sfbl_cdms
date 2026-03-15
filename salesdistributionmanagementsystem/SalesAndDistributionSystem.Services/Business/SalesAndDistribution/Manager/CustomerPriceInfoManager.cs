using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Business.User;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.Manager
{
    public class CustomerPriceInfoManager : ICustomerPriceInfoManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        private readonly IUserLogManager _logManager;

        public CustomerPriceInfoManager(ICommonServices commonServices, IConfiguration configuration,IUserLogManager userLogManager)
        {
            _commonServices = commonServices;
            _configuration = configuration;
            _logManager = userLogManager;

        }

        string AddOrUpdate_AddQuery() => @"INSERT INTO CUSTOMER_SKU_PRICE_MST 
                                         (CUSTOMER_PRICE_MSTID
                                            ,CUSTOMER_CODE
                                            ,CUSTOMER_ID
                                            ,ENTRY_DATE
                                            ,EFFECT_START_DATE
                                            ,EFFECT_END_DATE
                                            ,COMPANY_ID
                                            ,UNIT_ID
                                            ,STATUS
                                            ,REMARKS
                                            ,ENTERED_BY
                                            ,ENTERED_DATE
                                            ,ENTERED_TERMINAL
                                            ) 
                                          VALUES ( :param1, :param2, :param3, TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'),TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),TO_DATE( :param6, 'DD/MM/YYYY HH:MI:SS AM'), :param7, :param8, :param9, :param10, :param11,TO_DATE(:param12, 'DD/MM/YYYY HH:MI:SS AM'), :param13)";
        string AddOrUpdate_AddQueryDTL() => @"INSERT INTO CUSTOMER_SKU_PRICE_DTL 
                    (CUSTOMER_PRICE_DTLID,
                    CUSTOMER_PRICE_MSTID,
                    SKU_ID,
                    SKU_CODE,
                    GROUP_ID,
                    CATEGORY_ID,
                    BRAND_ID,
                    BASE_PRODUCT_ID,
                    PRICE_FLAG,
                    SKU_PRICE,
                    COMMISSION_FLAG,
                    COMMISSION_TYPE,
                    COMMISSION_VALUE,
                    ADD_COMMISSION1,
                    ADD_COMMISSION2,
                    STATUS,
                    COMPANY_ID,
                    UNIT_ID,
                    ENTERED_BY,
                    ENTERED_DATE,
                    ENTERED_TERMINAL
                   ) 
                    VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8, :param9, :param10, :param11, :param12, :param13, :param14, :param15, :param16,
                    :param17, :param18, :param19,TO_DATE(:param20, 'DD/MM/YYYY HH:MI:SS AM'), :param21)";

        string LoadSKUPriceDtlData_Query() => @"Select
                                                CUSTOMER_PRICE_DTLID,
                                                CUSTOMER_PRICE_MSTID,
                                                SKU_ID,
                                                SKU_CODE,
                                                GROUP_ID,
                                                CATEGORY_ID,
                                                BRAND_ID,
                                                BASE_PRODUCT_ID,
                                                PRICE_FLAG,
                                                SKU_PRICE,
                                                COMMISSION_FLAG,
                                                COMMISSION_TYPE,
                                                COMMISSION_VALUE,
                                                ADD_COMMISSION1,
                                                ADD_COMMISSION2,
                                                STATUS,
                                                COMPANY_ID,
                                                UNIT_ID,
                                                ENTERED_BY,
                                                ENTERED_DATE,
                                                ENTERED_TERMINAL,
                                                UPDATED_BY,
                                                UPDATED_DATE,
                                                UPDATED_TERMINAL

                                               from Customer_Sku_Price_Dtl 
                                                Where COMPANY_ID = :param1";
        string LoadSKUPriceDtlDataBySKUId_Query() => @"Select
                                                CUSTOMER_PRICE_DTLID
                                                from Customer_Sku_Price_Dtl 
                                                Where COMPANY_ID = :param1 AND SKU_ID = :param2";

        string LoadSKUPriceDtl_RestrictSKU_Query() => @"Select 
                                                
                                                D.CUSTOMER_PRICE_DTLID,
                                                D.CUSTOMER_PRICE_MSTID,
                                                D.SKU_ID,
                                                D.SKU_CODE, 
                                                M.EFFECT_START_DATE,
                                                M.EFFECT_END_DATE

                                                from Customer_Sku_Price_Dtl D
                                                Left outer join Customer_Sku_Price_MST M on M.CUSTOMER_PRICE_MSTID = D.CUSTOMER_PRICE_MSTID
                                                Where D.COMPANY_ID = :param1 AND D.SKU_ID   = :param2 AND M.CUSTOMER_ID = :param3 AND
                                                (TRUNC(M.EFFECT_END_DATE) > TO_DATE(:param4,'dd/mm/yyyy')  OR TRUNC(TO_DATE(:param4,'dd/mm/yyyy')) > TRUNC(SYSDATE))";


        string LoadSKUPriceMstData_Query() => @"CUSTOMER_PRICE_MSTID
                                                CUSTOMER_CODE,
                                                CUSTOMER_ID,
                                                ENTRY_DATE,
                                                EFFECT_START_DATE,
                                                EFFECT_END_DATE,
                                                COMPANY_ID,
                                                UNIT_ID,
                                                STATUS,
                                                REMARKS,
                                                ENTERED_BY,
                                                ENTERED_DATE,
                                                ENTERED_TERMINAL,
                                                UPDATED_BY,
                                                UPDATED_DATE,
                                                UPDATED_TERMINAL

                                               from CUSTOMER 
                                                Where COMPANY_ID = :param1";
        string LoadCustomerExistingSku_Query_New() => @"select COUNT(D.SKU_ID ) from Customer_Sku_Price_Dtl D
                                                Left outer join Customer_Sku_Price_MST M on M.CUSTOMER_PRICE_MSTID = D.CUSTOMER_PRICE_MSTID
                                                Where D.COMPANY_ID = :param1 AND D.SKU_ID   = :param2 AND M.CUSTOMER_ID = :param3 AND
                                                (TRUNC(M.EFFECT_END_DATE) > TO_DATE(:param4,'dd/mm/yyyy')  OR TRUNC(TO_DATE(:param4,'dd/mm/yyyy')) > TRUNC(SYSDATE))";

        string LoadCustomerExistingSku_Query() => @"select D.SKU_ID  from Customer_Sku_Price_Dtl D
                                                Left outer join Customer_Sku_Price_MST M on M.CUSTOMER_PRICE_MSTID = D.CUSTOMER_PRICE_MSTID
                                                Where D.COMPANY_ID = :param1 AND D.SKU_ID   = :param2 AND M.CUSTOMER_ID = :param3 AND
                                                (TRUNC(M.EFFECT_END_DATE) > TO_DATE(:param4,'dd/mm/yyyy')  OR TRUNC(TO_DATE(:param4,'dd/mm/yyyy')) > TRUNC(SYSDATE))";
        string LoadMasterData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.CUSTOMER_PRICE_MSTID ASC) AS ROW_NO,M.CUSTOMER_PRICE_MSTID
                                            ,M.CUSTOMER_CODE
                                            ,M.CUSTOMER_ID
                                            ,TO_CHAR(M.ENTRY_DATE, 'DD/MM/YYYY') ENTRY_DATE
                                            ,TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE
                                            ,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE
                                            ,M.COMPANY_ID
                                            ,M.UNIT_ID
                                            ,M.STATUS
                                            ,M.REMARKS
                                            ,M.ENTERED_BY
                                            ,M.ENTERED_DATE
                                            ,M.ENTERED_TERMINAL  
                                            ,cusinfo.CUSTOMER_NAME
            FROM customer_sku_price_mst  M 
            left join CUSTOMER_INFO cusinfo on cusinfo.CUSTOMER_ID = M.CUSTOMER_ID
            where  M.COMPANY_ID = :param1";
        string LoadData_DetailByMasterId_Query() => @"  SELECT ROW_NUMBER() OVER(ORDER BY M.CUSTOMER_PRICE_DTLID ASC) AS ROW_NO,M.*,PI.SKU_NAME,PI.PACK_SIZE
                                          FROM Customer_Sku_Price_Dtl  M
                                          left join Product_Info pi on PI.SKU_CODE = M.SKU_CODE
                                          Where M.CUSTOMER_PRICE_MSTID = :param1";
        private string DeleteCustomer_Sku_Price_DtlByIdQuery() => "DELETE  FROM Customer_Sku_Price_Dtl WHere CUSTOMER_PRICE_DTLID = :param1 AND CUSTOMER_PRICE_MSTID=:param2";

        string LoadData_MasterById_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.CUSTOMER_PRICE_MSTID ASC) AS ROW_NO,M.CUSTOMER_PRICE_MSTID
                                            ,M.CUSTOMER_CODE
                                            ,M.CUSTOMER_ID
                                            ,TO_CHAR(M.ENTRY_DATE, 'DD/MM/YYYY') ENTRY_DATE
                                            ,TO_CHAR(M.EFFECT_START_DATE, 'DD/MM/YYYY') EFFECT_START_DATE
                                            ,TO_CHAR(M.EFFECT_END_DATE, 'DD/MM/YYYY') EFFECT_END_DATE
                                            ,M.COMPANY_ID
                                            ,M.UNIT_ID
                                            ,M.STATUS
                                            ,M.REMARKS
                                            ,M.ENTERED_BY
                                            ,M.ENTERED_DATE
                                            ,M.ENTERED_TERMINAL  
                                            ,I.CUSTOMER_TYPE_ID
                                            ,I.CUSTOMER_NAME                 
            FROM customer_sku_price_mst  M , CUSTOMER_INFO I
            where Customer_Price_MstId = :param1 AND I.CUSTOMER_ID= M.CUSTOMER_ID AND I.CUSTOMER_CODE= M.CUSTOMER_CODE";

        string AddOrUpdate_UpdateQuery() => @"UPDATE CUSTOMER_SKU_PRICE_MST SET  
                                                    CUSTOMER_CODE= :param2,
                                                    CUSTOMER_ID= :param3,
                                                    ENTRY_DATE= TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM'),
                                                    EFFECT_START_DATE= TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),
                                                    EFFECT_END_DATE= TO_DATE(:param6, 'DD/MM/YYYY HH:MI:SS AM'),
                                                    COMPANY_ID= :param7,
                                                    UNIT_ID= :param8,
                                                    STATUS= :param9,
                                                    REMARKS= :param10,
                                                    UPDATED_BY= :param11,
                                                    UPDATED_DATE= TO_DATE(:param12, 'DD/MM/YYYY HH:MI:SS AM'),
                                                    UPDATED_TERMINAL= :param13
                                                WHERE CUSTOMER_PRICE_MSTID = :param1";

        string AddOrUpdate_UpdateQueryDTL() => @"UPDATE CUSTOMER_SKU_PRICE_DTL SET  
                                                CUSTOMER_PRICE_MSTID = :param2,
                                                SKU_ID = :param3,
                                                SKU_CODE = :param4,
                                                GROUP_ID = :param5,
                                                CATEGORY_ID = :param6,
                                                BRAND_ID = :param7,
                                                BASE_PRODUCT_ID = :param8,
                                                PRICE_FLAG = :param9,
                                                SKU_PRICE = :param10,
                                                COMMISSION_FLAG = :param11,
                                                COMMISSION_TYPE = :param12,
                                                COMMISSION_VALUE = :param13,
                                                ADD_COMMISSION1 = :param14,
                                                ADD_COMMISSION2 = :param15,
                                                STATUS = :param16,
                                                COMPANY_ID = :param17,
                                                UNIT_ID = :param18,
                                                UPDATED_BY = :param19,
                                                UPDATED_DATE = TO_DATE(:param20, 'DD/MM/YYYY HH:MI:SS AM'),
                                                UPDATED_TERMINAL  = :param21
                                                WHERE CUSTOMER_PRICE_DTLID = :param1";

        string GetCustomerPrice_Info_IdQuery() => "SELECT NVL(MAX(CUSTOMER_PRICE_MSTID),0) + 1 SKU_ID  FROM CUSTOMER_SKU_PRICE_MST";
        string GetCustomerPrice_Info_IdQuery_Dtl() => "SELECT NVL(MAX(CUSTOMER_PRICE_DTLID),0) + 1 SKU_ID  FROM CUSTOMER_SKU_PRICE_DTL";
        public async Task<string> AddOrUpdate(string db, Customer_SKU_Price_Mst model)
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
                    if(model.CUSTOMER_PRICE_MSTID==0)
                    {
                        int mst_id = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetCustomerPrice_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));
                        int cycle = 0;
                        int dtlId = 0;

                        foreach (var itemz in model.CUSTOMER_IDs)
                        {
                            if (cycle != 0)
                            {
                                mst_id++;
                            }
                            model.CUSTOMER_PRICE_MSTID = mst_id;
                            model.CUSTOMER_CODE = _commonServices.GetMaximumNumber<string>(_configuration.GetConnectionString(db), "SELECT  CUSTOMER_CODE FROM CUSTOMER_INFO WHERE CUSTOMER_ID = :param1", _commonServices.AddParameter(new string[] { itemz.ToString() }));

                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                                 _commonServices.AddParameter(new string[] {model.CUSTOMER_PRICE_MSTID.ToString(),
                             model.CUSTOMER_CODE,itemz.ToString(),model.ENTRY_DATE,
                             model.EFFECT_START_DATE.ToString(), model.EFFECT_END_DATE, model.COMPANY_ID.ToString(), model.UNIT_ID.ToString(),
                             model.CUSTOMER_STATUS, model.REMARKS, model.ENTERED_BY,
                             model.ENTERED_DATE,  model.ENTERED_TERMINAL
                                    })));

                                if (model.customerSkuPriceList != null && model.customerSkuPriceList.Count > 0)
                                {
                                    foreach (var item in model.customerSkuPriceList)
                                    {
                                        int check_ = await _commonServices.GetMaximumNumberAsyn<int>(_configuration.GetConnectionString(db), LoadCustomerExistingSku_Query_New(), _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), item.SKU_ID.ToString(), model.CUSTOMER_ID, model.EFFECT_START_DATE }));
                                        if (check_ > 0)
                                        {
                                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQueryDTL(), _commonServices.AddParameter(new string[] { check_.ToString(),model.CUSTOMER_PRICE_MSTID.ToString(),
                                         item.SKU_ID.ToString(),item.SKU_CODE,
                                         item.GROUP_ID.ToString(), item.CATEGORY_ID.ToString(), item.BRAND_ID.ToString(), item.BASE_PRODUCT_ID.ToString(),
                                         item.PRICE_FLAG, item.SKU_PRICE.ToString(), item.COMMISSION_FLAG,
                                         item.COMMISSION_TYPE,  item.COMMISSION_VALUE.ToString(),  item.ADD_COMMISSION1.ToString(),  item.ADD_COMMISSION2.ToString(),  item.STATUS
                                         ,item.COMPANY_ID.ToString(),  item.UNIT_ID.ToString(),item.UPDATED_BY,item.UPDATED_DATE,item.UPDATED_TERMINAL })));


                                        }
                                        else
                                        {
                                            dtlId = dtlId == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetCustomerPrice_Info_IdQuery_Dtl(), _commonServices.AddParameter(new string[] { })) : (dtlId + 1);

                                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(),
                                             _commonServices.AddParameter(new string[] { dtlId.ToString(),model.CUSTOMER_PRICE_MSTID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.GROUP_ID.ToString(), item.CATEGORY_ID.ToString(), item.BRAND_ID.ToString(), item.BASE_PRODUCT_ID.ToString(),
                                     item.PRICE_FLAG, item.SKU_PRICE.ToString(), item.COMMISSION_FLAG,
                                     item.COMMISSION_TYPE,  item.COMMISSION_VALUE.ToString(),  item.ADD_COMMISSION1.ToString(),  item.ADD_COMMISSION2.ToString(),  item.STATUS
                                     ,item.COMPANY_ID.ToString(),  item.UNIT_ID.ToString(),  item.ENTERED_BY,  item.ENTERED_DATE.ToString(),  item.ENTERED_TERMINAL
                                          })));
                                            dtlId++;

                                        }

                                    }
                                }



                            cycle++;

                        }
                    }
                    else
                    {
                        int dtlId = 0;


                        if (model.CUSTOMER_PRICE_MSTID == 0)
                        {
                            model.CUSTOMER_PRICE_MSTID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetCustomerPrice_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));
                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                             _commonServices.AddParameter(new string[] {model.CUSTOMER_PRICE_MSTID.ToString(),
                             model.CUSTOMER_CODE, model.CUSTOMER_ID.ToString(),model.ENTRY_DATE,
                             model.EFFECT_START_DATE.ToString(), model.EFFECT_END_DATE, model.COMPANY_ID.ToString(), model.UNIT_ID.ToString(),
                             model.CUSTOMER_STATUS, model.REMARKS, model.ENTERED_BY,
                             model.ENTERED_DATE,  model.ENTERED_TERMINAL
                                })));

                            if (model.customerSkuPriceList != null && model.customerSkuPriceList.Count > 0)
                            {
                                foreach (var item in model.customerSkuPriceList)
                                {
                                    int check_ = await _commonServices.GetMaximumNumberAsyn<int>(_configuration.GetConnectionString(db), LoadCustomerExistingSku_Query_New(), _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), item.SKU_ID.ToString(), model.CUSTOMER_ID, model.EFFECT_START_DATE }));
                                    if (check_ > 0)
                                    {
                                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQueryDTL(), _commonServices.AddParameter(new string[] { check_.ToString(),model.CUSTOMER_PRICE_MSTID.ToString(),
                                         item.SKU_ID.ToString(),item.SKU_CODE,
                                         item.GROUP_ID.ToString(), item.CATEGORY_ID.ToString(), item.BRAND_ID.ToString(), item.BASE_PRODUCT_ID.ToString(),
                                         item.PRICE_FLAG, item.SKU_PRICE.ToString(), item.COMMISSION_FLAG,
                                         item.COMMISSION_TYPE,  item.COMMISSION_VALUE.ToString(),  item.ADD_COMMISSION1.ToString(),  item.ADD_COMMISSION2.ToString(),  item.STATUS
                                         ,item.COMPANY_ID.ToString(),  item.UNIT_ID.ToString(),item.UPDATED_BY,item.UPDATED_DATE,item.UPDATED_TERMINAL })));


                                    }
                                    else
                                    {
                                        dtlId = dtlId == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetCustomerPrice_Info_IdQuery_Dtl(), _commonServices.AddParameter(new string[] { })) : (dtlId + 1);

                                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(),
                                         _commonServices.AddParameter(new string[] { dtlId.ToString(),model.CUSTOMER_PRICE_MSTID.ToString(),
                                     item.SKU_ID.ToString(),item.SKU_CODE,
                                     item.GROUP_ID.ToString(), item.CATEGORY_ID.ToString(), item.BRAND_ID.ToString(), item.BASE_PRODUCT_ID.ToString(),
                                     item.PRICE_FLAG, item.SKU_PRICE.ToString(), item.COMMISSION_FLAG,
                                     item.COMMISSION_TYPE,  item.COMMISSION_VALUE.ToString(),  item.ADD_COMMISSION1.ToString(),  item.ADD_COMMISSION2.ToString(),  item.STATUS
                                     ,item.COMPANY_ID.ToString(),  item.UNIT_ID.ToString(),  item.ENTERED_BY,  item.ENTERED_DATE.ToString(),  item.ENTERED_TERMINAL
                                      })));
                                        dtlId++;

                                    }

                                }
                            }

                        }
                        else
                        {
                            Customer_SKU_Price_Mst _Customer_Mst = new Customer_SKU_Price_Mst();
                            _Customer_Mst = await LoadDetailData_ByMasterId_List(db, model.CUSTOMER_PRICE_MSTID);
                            listOfQuery.Add(_commonServices.AddQuery(
                                AddOrUpdate_UpdateQuery(),
                                _commonServices.AddParameter(new string[] { model.CUSTOMER_PRICE_MSTID.ToString(),
                             model.CUSTOMER_CODE, model.CUSTOMER_ID.ToString(),model.ENTRY_DATE.ToString(),
                             model.EFFECT_START_DATE.ToString(), model.EFFECT_END_DATE.ToString(), model.COMPANY_ID.ToString(), model.UNIT_ID.ToString(),
                             model.CUSTOMER_STATUS.ToString(), model.REMARKS.ToString(),
                             model.UPDATED_BY.ToString(), model.UPDATED_DATE.ToString(),
                             model.UPDATED_TERMINAL.ToString()})));
                            foreach (var item in model.customerSkuPriceList)
                            {
                                if (item.CUSTOMER_PRICE_DTLID == 0)
                                {
                                    //-------------Add new row on detail table--------------------

                                    dtlId = dtlId == 0 ? _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetCustomerPrice_Info_IdQuery_Dtl(), _commonServices.AddParameter(new string[] { })) : (dtlId + 1);

                                    listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQueryDTL(), _commonServices.AddParameter(new string[] {dtlId.ToString(),model.CUSTOMER_PRICE_MSTID.ToString(),
                                         item.SKU_ID.ToString(),item.SKU_CODE,
                                         item.GROUP_ID.ToString(), item.CATEGORY_ID.ToString(), item.BRAND_ID.ToString(), item.BASE_PRODUCT_ID.ToString(),
                                         item.PRICE_FLAG, item.SKU_PRICE.ToString(), item.COMMISSION_FLAG,
                                         item.COMMISSION_TYPE,  item.COMMISSION_VALUE.ToString(),  item.ADD_COMMISSION1.ToString(),  item.ADD_COMMISSION2.ToString(),  item.STATUS
                                         ,  item.COMPANY_ID.ToString(),  item.UNIT_ID.ToString(),  item.ENTERED_BY,  item.ENTERED_DATE.ToString(),  item.ENTERED_TERMINAL })));

                                }
                                else
                                {
                                    //-------------Edit on detail table--------------------

                                    listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateQueryDTL(), _commonServices.AddParameter(new string[] { item.CUSTOMER_PRICE_DTLID.ToString(),model.CUSTOMER_PRICE_MSTID.ToString(),
                                         item.SKU_ID.ToString(),item.SKU_CODE,
                                         item.GROUP_ID.ToString(), item.CATEGORY_ID.ToString(), item.BRAND_ID.ToString(), item.BASE_PRODUCT_ID.ToString(),
                                         item.PRICE_FLAG, item.SKU_PRICE.ToString(), item.COMMISSION_FLAG,
                                         item.COMMISSION_TYPE,  item.COMMISSION_VALUE.ToString(),  item.ADD_COMMISSION1.ToString(),  item.ADD_COMMISSION2.ToString(),  item.STATUS
                                         ,item.COMPANY_ID.ToString(),  item.UNIT_ID.ToString(),item.UPDATED_BY,item.UPDATED_DATE,item.UPDATED_TERMINAL })));

                                }

                            }
                            foreach (var detail in _Customer_Mst.customerSkuPriceList)
                            {
                                bool status = true;

                                foreach (var updateditem in model.customerSkuPriceList)
                                {
                                    if (detail.CUSTOMER_PRICE_DTLID == updateditem.CUSTOMER_PRICE_DTLID)
                                    {
                                        status = false;
                                    }
                                }
                                if (status)
                                {
                                    //-------------Delete row from detail table--------------------

                                    listOfQuery.Add(_commonServices.AddQuery(DeleteCustomer_Sku_Price_DtlByIdQuery(), _commonServices.AddParameter(new string[] { detail.CUSTOMER_PRICE_DTLID.ToString(), _Customer_Mst.CUSTOMER_PRICE_MSTID.ToString() })));

                                }
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

        public async Task<string> LoadSKUPriceDtlDataRestrict(string db, string Company_Id,int cust_id,List<int> sku_id,string start_date, List<string> cust_ids)
        {
            string result = "";
            if(cust_id>0)
            {
                foreach (var item in sku_id)
                {
                    DataTable dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSKUPriceDtl_RestrictSKU_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), item.ToString(), cust_id.ToString(), start_date }));
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        result = result + " | " + dt.Rows[0]["SKU_CODE"].ToString().Trim() + " - Start: " + dt.Rows[0]["EFFECT_START_DATE"].ToString() + " - End: " + dt.Rows[0]["EFFECT_END_DATE"].ToString();
                    }

                }

            }
            else
            {
                foreach (var itemx in cust_ids)
                {
                    foreach (var item in sku_id)
                    {
                        DataTable dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSKUPriceDtl_RestrictSKU_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), item.ToString(), cust_id.ToString(), start_date }));
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            string name_code = _commonServices.GetMaximumNumber<string>(_configuration.GetConnectionString(db), "SELECT CUSTOMER_CODE FROM CUSTOMER_INFO WHERE CUSTOMER_ID = :param1 ", _commonServices.AddParameter(new string[] { itemx.ToString() }));
                            result = result + " | Cust Code : " + name_code + " -> " + dt.Rows[0]["SKU_CODE"].ToString().Trim() + " - Start: " + dt.Rows[0]["EFFECT_START_DATE"].ToString() + " - End: " + dt.Rows[0]["EFFECT_END_DATE"].ToString();
                        }

                    }

                }

               
            }
            
            return result;

        }


        public async Task<string> LoadSKUPriceDtlData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSKUPriceDtlData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> LoadSKUPriceMstData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSKUPriceMstData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> GetCustomerExistingSKUData(string db, int Company_Id, int Customer_Id)
        {
            var dt = _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadCustomerExistingSku_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), Customer_Id.ToString() })));
            return dt;
        }


        public async Task<string> LoadData_Master(string db, int Company_Id)
        {
            List<Customer_SKU_Price_Mst> Customer_SKU_Price_Mstt_list = new List<Customer_SKU_Price_Mst>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadMasterData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                Customer_SKU_Price_Mst customer_SKU_Price_Mst = new Customer_SKU_Price_Mst();
                customer_SKU_Price_Mst.CUSTOMER_PRICE_MSTID = Convert.ToInt32(data.Rows[i]["CUSTOMER_PRICE_MSTID"]);
                customer_SKU_Price_Mst.CUSTOMER_PRICE_MSTID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["CUSTOMER_PRICE_MSTID"].ToString());
                customer_SKU_Price_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                customer_SKU_Price_Mst.COMPANY_ID = Convert.ToInt32(data.Rows[i]["COMPANY_ID"]);
                customer_SKU_Price_Mst.CUSTOMER_ID = data.Rows[i]["CUSTOMER_ID"].ToString();
                customer_SKU_Price_Mst.CUSTOMER_NAME = data.Rows[i]["CUSTOMER_NAME"].ToString();
                customer_SKU_Price_Mst.CUSTOMER_CODE = data.Rows[i]["CUSTOMER_CODE"].ToString();

                customer_SKU_Price_Mst.EFFECT_END_DATE = data.Rows[i]["EFFECT_END_DATE"].ToString();
                customer_SKU_Price_Mst.EFFECT_START_DATE = data.Rows[i]["EFFECT_START_DATE"].ToString();
                customer_SKU_Price_Mst.REMARKS = data.Rows[i]["REMARKS"].ToString();
                Customer_SKU_Price_Mstt_list.Add(customer_SKU_Price_Mst);
            }
            return JsonSerializer.Serialize(Customer_SKU_Price_Mstt_list);
        }

        public async Task<Customer_SKU_Price_Mst> LoadDetailData_ByMasterId_List(string db, int Id)
        {
            try
            {
                Customer_SKU_Price_Mst _Customer_Mst = new Customer_SKU_Price_Mst();
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    _Customer_Mst.COMPANY_ID = Convert.ToInt32(dataTable.Rows[0]["COMPANY_ID"]);
                    _Customer_Mst.CUSTOMER_PRICE_MSTID = Id;
                    _Customer_Mst.CUSTOMER_ID = dataTable.Rows[0]["CUSTOMER_ID"].ToString();
                    _Customer_Mst.CUSTOMER_CODE = dataTable.Rows[0]["CUSTOMER_CODE"].ToString();
                    _Customer_Mst.EFFECT_START_DATE = dataTable.Rows[0]["EFFECT_START_DATE"].ToString();
                    _Customer_Mst.EFFECT_END_DATE = dataTable.Rows[0]["EFFECT_END_DATE"].ToString();
                    _Customer_Mst.REMARKS = dataTable.Rows[0]["REMARKS"].ToString();
                    _Customer_Mst.UNIT_ID = Convert.ToInt32(dataTable.Rows[0]["UNIT_ID"]);
                    _Customer_Mst.CUSTOMER_STATUS = dataTable.Rows[0]["STATUS"] != null ? dataTable.Rows[0]["STATUS"].ToString() : "Active";
                    _Customer_Mst.ROW_NO = Convert.ToInt32(dataTable.Rows[0]["ROW_NO"].ToString());
                    _Customer_Mst.ENTRY_DATE = dataTable.Rows[0]["ENTRY_DATE"].ToString();
                    _Customer_Mst.CUSTOMER_NAME = dataTable.Rows[0]["CUSTOMER_NAME"].ToString();
                    _Customer_Mst.CUSTOMER_TYPE_ID = dataTable.Rows[0]["CUSTOMER_TYPE_ID"].ToString();
                    DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));
                    _Customer_Mst.customerSkuPriceList = new List<Customer_SKU_Price_Dtl>();
                    for (int i = 0; i < dataTable_detail.Rows.Count; i++)
                    {
                        Customer_SKU_Price_Dtl _Cus_Dtl = new Customer_SKU_Price_Dtl();
                        _Cus_Dtl.COMPANY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["COMPANY_ID"]);
                        _Cus_Dtl.ADD_COMMISSION1 = dataTable_detail.Rows[i]["ADD_COMMISSION1"].ToString() != ""? Convert.ToDecimal(dataTable_detail.Rows[i]["ADD_COMMISSION1"]) : 0;
                        _Cus_Dtl.ADD_COMMISSION2 = dataTable_detail.Rows[i]["ADD_COMMISSION2"].ToString() != ""?  Convert.ToDecimal(dataTable_detail.Rows[i]["ADD_COMMISSION2"]) : 0;
                        _Cus_Dtl.BASE_PRODUCT_ID = Convert.ToInt32(dataTable_detail.Rows[i]["BASE_PRODUCT_ID"]);
                        _Cus_Dtl.BRAND_ID = Convert.ToInt32(dataTable_detail.Rows[i]["BRAND_ID"]);
                        _Cus_Dtl.CATEGORY_ID = Convert.ToInt32(dataTable_detail.Rows[i]["CATEGORY_ID"]);
                        _Cus_Dtl.COMMISSION_FLAG = dataTable_detail.Rows[i]["COMMISSION_FLAG"].ToString();
                        _Cus_Dtl.COMMISSION_TYPE = dataTable_detail.Rows[i]["COMMISSION_TYPE"].ToString();
                        _Cus_Dtl.COMMISSION_VALUE = Convert.ToDecimal(dataTable_detail.Rows[i]["COMMISSION_VALUE"]);
                        _Cus_Dtl.CUSTOMER_PRICE_DTLID = Convert.ToInt32(dataTable_detail.Rows[i]["CUSTOMER_PRICE_DTLID"]);
                        _Cus_Dtl.CUSTOMER_PRICE_MSTID = Convert.ToInt32(dataTable_detail.Rows[i]["CUSTOMER_PRICE_MSTID"]);
                        _Cus_Dtl.ENTERED_BY = dataTable_detail.Rows[i]["ENTERED_BY"].ToString();
                        _Cus_Dtl.ENTERED_DATE = dataTable_detail.Rows[i]["ENTERED_DATE"].ToString();
                        _Cus_Dtl.GROUP_ID = Convert.ToInt32(dataTable_detail.Rows[i]["GROUP_ID"]);
                        _Cus_Dtl.PRICE_FLAG = dataTable_detail.Rows[i]["PRICE_FLAG"].ToString();
                        _Cus_Dtl.SKU_NAME = dataTable_detail.Rows[i]["SKU_NAME"].ToString();
                        _Cus_Dtl.SKU_CODE = dataTable_detail.Rows[i]["SKU_CODE"].ToString();
                        _Cus_Dtl.SKU_ID = Convert.ToInt32(dataTable_detail.Rows[i]["SKU_ID"]);
                        _Cus_Dtl.PACK_SIZE = dataTable_detail.Rows[i]["PACK_SIZE"].ToString();
                        _Cus_Dtl.SKU_PRICE = Convert.ToDecimal(dataTable_detail.Rows[i]["SKU_PRICE"]);
                        _Cus_Dtl.STATUS = dataTable_detail.Rows[i]["STATUS"].ToString();
                        _Cus_Dtl.UNIT_ID = Convert.ToInt32(dataTable_detail.Rows[i]["UNIT_ID"]);
                        _Cus_Dtl.ROW_NO = Convert.ToInt32(dataTable_detail.Rows[i]["ROW_NO"]);
                        _Customer_Mst.customerSkuPriceList.Add(_Cus_Dtl);
                    }
                }

                return _Customer_Mst;



            }
            catch (Exception ex)
            {
                throw ex;
            }
            

        }

        public Task<string> LoadData(string db, int Company_Id)
        {
            throw new NotImplementedException();
        }


    }
}
