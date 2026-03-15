using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework.Internal;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.Inventory;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Inventory;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.Inventory.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Inventory.Manager
{
    public class DistributionDeliveryManager : IDistributionDeliveryManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public DistributionDeliveryManager(ICommonServices commonServices,
            IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;
        }

        #region Master_Query
        //********************** Master *********************
        string GetNewID_IdQuery() => "SELECT NVL(MAX(MST_ID),0) + 1 MST_ID  FROM DEPOT_CUSTOMER_DIST_MST";

        string Get_LastDistribution_no() => "SELECT  DISTRIBUTION_NO  FROM DEPOT_CUSTOMER_DIST_MST Where MST_ID = (SELECT NVL(MAX(MST_ID),0) MST_ID From DEPOT_CUSTOMER_DIST_MST where COMPANY_ID = :param1 )";

        string LoadData_Master_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO, MST_ID, null as MST_ID_ENCRYPTED, DISTRIBUTION_NO, DISTRIBUTION_DATE, 
            M.DIST_ROUTE_ID, R.DIST_ROUTE_NAME, VEHICLE_NO, VEHICLE_DESCRIPTION, VEHICLE_TOTAL_VOLUME, VEHICLE_TOTAL_WEIGHT, DRIVER_ID, DISTRIBUTION_BY, M.STATUS, M.COMPANY_ID, 
            INVOICE_UNIT_ID, M.REMARKS, CASE WHEN CONFIRMED = 0 THEN 'NO' WHEN CONFIRMED = 1 THEN 'YES' END CONFIRMED 
            FROM DEPOT_CUSTOMER_DIST_MST M, DISTRIBUTION_ROUTE_INFO R
            WHERE M.DIST_ROUTE_ID = R.DIST_ROUTE_ID
            AND M.COMPANY_ID = :param1
            AND trunc(M.DISTRIBUTION_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY')
            AND INVOICE_UNIT_ID = :param4
            ORDER BY DISTRIBUTION_NO";

        string LoadNonConfirmedList_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO, MST_ID, null as MST_ID_ENCRYPTED, DISTRIBUTION_NO, DISTRIBUTION_DATE, 
            M.DIST_ROUTE_ID, R.DIST_ROUTE_NAME, VEHICLE_NO, VEHICLE_DESCRIPTION, VEHICLE_TOTAL_VOLUME, VEHICLE_TOTAL_WEIGHT, DRIVER_ID, DISTRIBUTION_BY, M.STATUS, M.COMPANY_ID, 
            INVOICE_UNIT_ID, M.REMARKS
            FROM DEPOT_CUSTOMER_DIST_MST M, DISTRIBUTION_ROUTE_INFO R
            WHERE M.DIST_ROUTE_ID = R.DIST_ROUTE_ID
            AND M.CONFIRMED = 0
            AND M.COMPANY_ID = :param1
            AND trunc(M.DISTRIBUTION_DATE) BETWEEN TO_DATE(:param2, 'DD/MM/YYYY') AND TO_DATE(:param3, 'DD/MM/YYYY')
            AND INVOICE_UNIT_ID = :param4
            ORDER BY DISTRIBUTION_NO";

        string LoadDataById_Master_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO, 
                                           MST_ID, null as MST_ID_ENCRYPTED, 
                                           DISTRIBUTION_NO, DISTRIBUTION_DATE, 
                                           DIST_ROUTE_ID, M.VEHICLE_NO, M.VEHICLE_DESCRIPTION, 
                                           M.VEHICLE_TOTAL_VOLUME, M.VEHICLE_TOTAL_WEIGHT,
                                           M.DRIVER_ID, 
                                           DISTRIBUTION_BY, M.STATUS, 
                                           M.COMPANY_ID, 
                                           INVOICE_UNIT_ID, M.REMARKS, null AS Invoices,
                                           V.DRIVER_NAME,
                                           WU.MEASURING_UNIT_NAME AS WEIGHT_UNIT,
                                           VU.MEASURING_UNIT_NAME AS VOLUME_UNIT,
                                           M.CONFIRMED,
                                           M.DRIVER_PHONE DRIVER_PHONE
                                           FROM DEPOT_CUSTOMER_DIST_MST M
                                           LEFT JOIN VEHICLE_INFO V ON M.VEHICLE_NO = V.VEHICLE_NO
                                           LEFT JOIN MEASURING_UNIT_INFO WU ON V.WEIGHT_UNIT = WU.MEASURING_UNIT_NAME
                                           LEFT JOIN MEASURING_UNIT_INFO VU ON V.VOLUME_UNIT = VU.MEASURING_UNIT_NAME
                                           LEFT JOIN DRIVER_INFO D on D.DRIVER_ID = M.DRIVER_ID
                                           WHERE M.COMPANY_ID = :param1 and M.MST_ID = :param2";

        string AddOrUpdate_Master_AddQuery() => @"INSERT INTO DEPOT_CUSTOMER_DIST_MST 
                                         (MST_ID, DISTRIBUTION_NO, DISTRIBUTION_DATE, DIST_ROUTE_ID, VEHICLE_NO, VEHICLE_DESCRIPTION, VEHICLE_TOTAL_VOLUME, VEHICLE_TOTAL_WEIGHT, DRIVER_ID, DISTRIBUTION_BY, STATUS, COMPANY_ID, INVOICE_UNIT_ID, REMARKS, ENTERED_BY, ENTERED_DATE, ENTERED_TERMINAL, TOTAL_VOLUMN, TOTAL_WEIGHT,DRIVER_PHONE) 
                                         VALUES ( :param1, :param2, TO_DATE(:param3, 'DD/MM/YYYY'), :param4, :param5, :param6, :param7, :param8, :param9, :param10, :param11, :param12, :param13, :param14, :param15, TO_DATE(:param16, 'DD/MM/YYYY HH:MI:SS AM'), :param17, :param18, :param19,:param20 )";

        string AddOrUpdate_Master_UpdateQuery() => @"UPDATE DEPOT_CUSTOMER_DIST_MST SET 
                                            DISTRIBUTION_NO =  :param2, DISTRIBUTION_DATE = TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM'),
                                            DIST_ROUTE_ID = :param4, VEHICLE_NO = :param5 , VEHICLE_DESCRIPTION = :param6, VEHICLE_TOTAL_VOLUME = :param7, VEHICLE_TOTAL_WEIGHT = :param8,
                                            DRIVER_ID = :param9, DISTRIBUTION_BY = :param10 , STATUS = :param11, COMPANY_ID = :param12, INVOICE_UNIT_ID = :param13, REMARKS = :param14,
                                            UPDATED_BY = :param15, UPDATED_DATE = TO_DATE(:param16, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param17,
                                            TOTAL_VOLUMN = :param18,
                                            TOTAL_WEIGHT = :param19,
                                            CONFIRMED = :param20,
                                            DRIVER_PHONE = :param21
                                            WHERE MST_ID = :param1";
        #endregion
        #region Invoice_query
        //********************** Distribution Invoice *********************
        string GetLastInvoice_IdQuery() => "SELECT NVL(MAX(DEPOT_INV_ID),0) DEPOT_INV_ID FROM DEPOT_CUSTOMER_DIST_INVOICE";

        string LoadData_DistInvoiceByMstId_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.DEPOT_INV_ID ASC) AS ROW_NO, DEPOT_INV_ID, MST_ID, INVOICE_NO, TO_CHAR(INVOICE_DATE, 'DD/MM/YYYY') AS INVOICE_DATE, 
        INVOICE_UNIT_ID, M.CUSTOMER_ID, 
         M.CUSTOMER_CODE, DIST_ROUTE_ID,DIST_ROUTE_ID ROUTE_ID, M.STATUS, M.COMPANY_ID,
         C.CUSTOMER_NAME,
         M.CUSTOMER_CHALLAN_NO
         FROM DEPOT_CUSTOMER_DIST_INVOICE M
         LEFT JOIN CUSTOMER_INFO C ON M.CUSTOMER_ID = C.CUSTOMER_ID
        WHERE M.COMPANY_ID = :param1 AND M.MST_ID = :param2";

        string PendingInvoice_Query() => "SELECT * FROM VW_INVOICE_FOR_DISTRIBUTION WHERE COMPANY_ID = :param1 AND INVOICE_UNIT_ID = :param2";

        string GetInvoice_Query() => "SELECT INVOICE_TYPE_CODE, ORDER_NO, TO_CHAR(ORDER_DATE, 'DD/MM/YYYY hh:mi:ss AM') ORDER_DATE, ORDER_MST_ID, INVOICE_UNIT_ID FROM INVOICE_MST WHERE INVOICE_NO = :param1";
        string GetOrder_Query() => "SELECT * FROM ORDER_MST WHERE ORDER_NO = :param1";
        string AddOrUpdate_DistInvoice_AddQuery() => @"INSERT INTO DEPOT_CUSTOMER_DIST_INVOICE 
                                         (DEPOT_INV_ID, MST_ID, INVOICE_NO, INVOICE_DATE, INVOICE_UNIT_ID, CUSTOMER_ID, CUSTOMER_CODE, DIST_ROUTE_ID, STATUS, COMPANY_ID, ENTERED_BY, ENTERED_DATE, ENTERED_TERMINAL,ORDER_MST_ID,ORDER_NO,INVOICE_TYPE_CODE,ORDER_UNIT_ID,CUSTOMER_CHALLAN_NO, ORDER_DATE) 
                                         VALUES ( :param1, :param2, :param3, TO_DATE(:param4, 'DD/MM/YYYY'), :param5, :param6, :param7, :param8, :param9, :param10, :param11, TO_DATE(:param12, 'DD/MM/YYYY HH:MI:SS AM'), :param13, :param14, :param15, :param16, :param17,:param18, TO_DATE(:param19, 'DD/MM/YYYY HH:MI:SS AM'))";

        string AddOrUpdate_DistInvoice_UpdateQuery() => @"UPDATE DEPOT_CUSTOMER_DIST_INVOICE SET 
                                            INVOICE_NO = :param2, INVOICE_DATE = TO_DATE(:param3, 'DD/MM/YYYY HH:MI:SS AM'),
                                            INVOICE_UNIT_ID = :param4 , CUSTOMER_ID = :param5, CUSTOMER_CODE = :param6, DIST_ROUTE_ID = :param7,
                                            STATUS = :param8, COMPANY_ID = :param9, UPDATED_BY = :param10, UPDATED_DATE = TO_DATE(:param11, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param12 WHERE DEPOT_INV_ID = :param1";

        #endregion
        #region Product_Query
        //********************** Distribution Products *********************
        string GetLastProdInvoice_IdQuery() => "SELECT NVL(MAX(DEPOT_PRODUCT_ID),0) DEPOT_PRODUCT_ID FROM DEPOT_CUSTOMER_DIST_PRODUCT";

        string LoadData_DistProductByMstId_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.DEPOT_PRODUCT_ID ASC) AS ROW_NO, DEPOT_PRODUCT_ID, DEPOT_INV_ID, MST_ID, SKU_ID, SKU_CODE, UNIT_TP,
                                                        INVOICE_QTY, BONUS_QTY, DISTRIBUTION_QTY, DISTRIBUTION_BONUS_QTY, TOTAL_DISTRIBUTION_QTY, SHIPPER_QTY, NO_OF_SHIPPER, LOOSE_QTY, SHIPPER_WEIGHT, SHIPPER_VOLUMN, 
                                                        LOOSE_WEIGHT, LOOSE_VOLUMN, TOTAL_WEIGHT, TOTAL_VOLUMN, COMPANY_ID, INVOICE_UNIT_ID FROM DEPOT_CUSTOMER_DIST_PRODUCT M WHERE M.COMPANY_ID = :param1 AND M.MST_ID = :param2";
        string GetProductsByInvoice_Query() => "SELECT * FROM VW_INVOICE_PRODUCT_FOR_DIST WHERE COMPANY_ID = :param1 and MST_ID = :param2";
        string LoadData_DistProductByInvoiceId_Query() => @"SELECT 
            ROW_NUMBER() OVER(ORDER BY M.DEPOT_PRODUCT_ID ASC) AS ROW_NO
            , TO_CHAR(M.ENTERED_DATE, 'DD/MM/YYYY') AS ENTERED_DATE
            , TO_CHAR(M.UPDATED_DATE, 'DD/MM/YYYY') AS UPDATED_DATE
            , M.DEPOT_PRODUCT_ID,
M.DEPOT_INV_ID, 
M.MST_ID, 
M.SKU_ID, 
M.SKU_CODE, 
M.UNIT_TP, 
M.INVOICE_QTY, 
M.BONUS_QTY, 
M.DISTRIBUTION_QTY, 
M.DISTRIBUTION_BONUS_QTY, 
M.TOTAL_DISTRIBUTION_QTY, 
M.SHIPPER_QTY, M.NO_OF_SHIPPER, 
M.LOOSE_QTY, M.SHIPPER_WEIGHT, 
M.SHIPPER_VOLUMN, 
M.LOOSE_WEIGHT, 
M.LOOSE_VOLUMN, 
M.TOTAL_WEIGHT, 
M.TOTAL_VOLUMN, 
M.COMPANY_ID, 
M.INVOICE_UNIT_ID, 
M.UPDATED_BY, 
M.UPDATED_DATE, 
M.UPDATED_TERMINAL, 
M.DISTRIBUTION_DATE, 
M.INVOICE_NO
            
            , (M.INVOICE_QTY ) PENDING_INVOICE_DIST_QTY
            , (M.BONUS_QTY ) PENDING_BONUS_DIST_QTY

            , FN_SKU_NAME(M.COMPANY_ID, M.SKU_ID) AS SKU_NAME
            , FN_PACK_SIZE(M.COMPANY_ID, M.SKU_ID) AS PACK_SIZE
            , P.WEIGHT_PER_PACK
            , P.SHIPPER_VOLUME_UNIT
            , P.SHIPPER_WEIGHT_UNIT
            , P.SHIPPER_WEIGHT AS PER_SHIPPER_WEIGHT
            , P.SHIPPER_VOLUME AS PER_SHIPPER_VOLUME
            , P.PACK_VALUE
            , CONCAT(M.TOTAL_VOLUMN, P.SHIPPER_VOLUME_UNIT) AS SKU_TOTAL_VOLUME
            , CONCAT(M.TOTAL_WEIGHT, P.SHIPPER_WEIGHT_UNIT) AS SKU_TOTAL_WEIGHT
            FROM DEPOT_CUSTOMER_DIST_PRODUCT M
            LEFT JOIN PRODUCT_INFO P ON M.SKU_ID = P.SKU_ID
            WHERE M.COMPANY_ID = :param1 AND M.MST_ID = :param2 AND DEPOT_INV_ID = :param3";

        string AddOrUpdate_DistProduct_AddQuery() => @"INSERT INTO DEPOT_CUSTOMER_DIST_PRODUCT 
                                         (DEPOT_PRODUCT_ID, DEPOT_INV_ID, MST_ID, SKU_ID, SKU_CODE, UNIT_TP, INVOICE_QTY, BONUS_QTY, DISTRIBUTION_QTY, DISTRIBUTION_BONUS_QTY, TOTAL_DISTRIBUTION_QTY, SHIPPER_QTY, NO_OF_SHIPPER, LOOSE_QTY, SHIPPER_WEIGHT, SHIPPER_VOLUMN, LOOSE_WEIGHT, LOOSE_VOLUMN, TOTAL_WEIGHT, TOTAL_VOLUMN, COMPANY_ID, INVOICE_UNIT_ID, ENTERED_BY, ENTERED_DATE, ENTERED_TERMINAL, DISTRIBUTION_DATE, INVOICE_NO ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8, :param9, :param10, :param11, :param12, :param13, :param14, :param15, :param16, :param17, :param18, :param19, :param20, :param21, :param22, :param23, TO_DATE(:param24, 'DD/MM/YYYY HH:MI:SS AM'), :param25, TO_DATE(:param26, 'DD/MM/YYYY'), :param27)";

        string AddOrUpdate_DistProduct_UpdateQuery() => @"UPDATE DEPOT_CUSTOMER_DIST_PRODUCT SET 
                                            SKU_ID =  :param2, SKU_CODE = :param3, UNIT_TP = :param4, INVOICE_QTY = :param5 , BONUS_QTY = :param6, DISTRIBUTION_QTY = :param7, DISTRIBUTION_BONUS_QTY = :param8, TOTAL_DISTRIBUTION_QTY = :param9,
                                            SHIPPER_QTY = :param10, NO_OF_SHIPPER = :param11, LOOSE_QTY = :param12, SHIPPER_WEIGHT = :param13 , SHIPPER_VOLUMN = :param14, LOOSE_WEIGHT = :param15, LOOSE_VOLUMN = :param16, TOTAL_WEIGHT = :param17, TOTAL_VOLUMN = :param18, COMPANY_ID = :param19, INVOICE_UNIT_ID = :param20,
                                            UPDATED_BY = :param21, UPDATED_DATE = TO_DATE(:param22, 'DD/MM/YYYY HH:MI:SS AM'), UPDATED_TERMINAL = :param23, DISTRIBUTION_DATE = TO_DATE(:param24, 'DD/MM/YYYY HH:MI:SS AM')
                               WHERE DEPOT_PRODUCT_ID = :param1";

        string DeleteProduct_Query() => @"DELETE FROM DEPOT_CUSTOMER_DIST_PRODUCT WHERE DEPOT_PRODUCT_ID = :param1";
        #endregion

        #region product_batches
        string LoadBatchesByProductId_Query() => @"SELECT 
            ROW_NUMBER() OVER(ORDER BY M.DEPOT_PRODUCT_ID ASC) AS ROW_NO
            , M.*
            FROM DEPOT_CUSTOMER_DIST_PROD_BATCH M
            WHERE M.COMPANY_ID = :param1 AND M.MST_ID = :param2 AND DEPOT_PRODUCT_ID = :param3";

        string LoadBatchesByMstId_Query() => @"SELECT 
            ROW_NUMBER() OVER(ORDER BY M.DEPOT_PRODUCT_ID ASC) AS ROW_NO
            , M.*
            , FN_SKU_NAME(M.COMPANY_ID, M.SKU_ID) AS SKU_NAME
            , D.INVOICE_NO
            FROM DEPOT_CUSTOMER_DIST_PROD_BATCH M
            LEFT JOIN DEPOT_CUSTOMER_DIST_PRODUCT D ON M.DEPOT_PRODUCT_ID = D.DEPOT_PRODUCT_ID
            WHERE M.MST_ID = :param1";

        string DeleteProductBatch_Query() => @"DELETE FROM DEPOT_CUSTOMER_DIST_PROD_BATCH WHERE DEPOT_BATCH_ID = :param1";
        #endregion
        #region Gift Query
        //********************** Distribution Gift Batches *********************
        string GetLastGiftInvoice_IdQuery() => "SELECT NVL(MAX(DEPOT_BATCH_ID),0) DEPOT_BATCH_ID FROM DEPOT_CUSTOMER_DIST_GIFT_BATCH";

        string LoadData_DistGiftsByInvoiceId_Query() => @"SELECT 
            ROW_NUMBER() OVER(ORDER BY M.DEPOT_BATCH_ID ASC) AS ROW_NO
            ,TO_CHAR(ENTERED_DATE, 'DD/MM/YYYY') AS ENTERED_DATE
            ,TO_CHAR(UPDATED_DATE, 'DD/MM/YYYY') AS UPDATED_DATE
            , M.*
            FROM DEPOT_CUSTOMER_DIST_GIFT_BATCH M 
            WHERE M.COMPANY_ID = :param1 AND M.MST_ID = :param2 AND DEPOT_INVOICE_ID = :param3";

        string AddOrUpdate_DistGift_AddQuery() => @"INSERT INTO DEPOT_CUSTOMER_DIST_GIFT_BATCH (
                                            DEPOT_BATCH_ID,
                                            DEPOT_INVOICE_ID,
                                            MST_ID,
                                            GIFT_ITEM_ID,
                                            UNIT_TP,
                                            BATCH_ID,
                                            BATCH_NO,
                                            GIFT_QTY,
                                            COMPANY_ID,
                                            INVOICE_UNIT_ID,
                                            ENTERED_BY,
                                            ENTERED_DATE,
                                            ENTERED_TERMINAL) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8, :param9, :param10, :param11, TO_DATE(:param12, 'DD/MM/YYYY HH:MI:SS AM'), :param13)";

        string AddOrUpdate_DistGift_UpdateQuery() => @"UPDATE DEPOT_CUSTOMER_DIST_GIFT_BATCH SET 
                                            DEPOT_BATCH_ID = :param2,
                                            DEPOT_INVOICE_ID = :param3,
                                            MST_ID = :param4,
                                            GIFT_ITEM_ID = :param5,
                                            UNIT_TP = :param6,
                                            BATCH_ID = :param7,
                                            BATCH_NO = :param8,
                                            GIFT_QTY = :param9,
                                            COMPANY_ID = :param10,
                                            INVOICE_UNIT_ID = :param11,
                                            UPDATED_BY = :param12,
                                            UPDATED_DATE = :param13,
                                            UPDATED_TERMINAL = :param14
                                            WHERE DEPOT_PRODUCT_ID = :param1";
        string GetGiftByInvoice_Query() => "SELECT * FROM VW_INVOICE_GIFT_FOR_DIST WHERE COMPANY_ID = :param1 and MST_ID = :param2";
        string DeleteGift_Query() => @"DELETE FROM DEPOT_CUSTOMER_DIST_GIFT_BATCH WHERE DEPOT_BATCH_ID = :param1";


        #endregion

        //string DistributionRoute_Query() => @"SELECT DISTINCT A.* 
        //    FROM DISTRIBUTION_ROUTE_INFO A
        //    LEFT OUTER JOIN CUSTOMER_ROUTE_RELATION C on A.DIST_ROUTE_ID = C.ROUTE_ID
        //    LEFT JOIN CUSTOMER_INFO B ON B.CUSTOMER_ID = C.CUSTOMER_ID
        //    WHERE A.COMPANY_ID=:param1 
        //    AND A.STATUS = 'Active'
        //    AND B.UNIT_ID = :param2";
        string DistributionRoute_Query() => @"SELECT DISTINCT A.* 
            FROM DISTRIBUTION_ROUTE_INFO A
            LEFT OUTER JOIN CUSTOMER_ROUTE_RELATION C on A.DIST_ROUTE_ID = C.ROUTE_ID
            LEFT JOIN CUSTOMER_INFO B ON B.CUSTOMER_ID = C.CUSTOMER_ID
            WHERE A.COMPANY_ID=:param1 
            AND A.STATUS = 'Active'
            AND :param2 = :param2";

        string Distable_Qty_By_InvoiceNo_Query() => @"
  SELECT DISTINCT M.INVOICE_NO,
         D.SKU_CODE,
         SUM (NVL (P.INVOICE_QTY, 0)) DISTRIBUTION_QTY,
         SUM (D.INVOICE_QTY) INVOICE_QTY,
         SUM (D.INVOICE_QTY) - SUM (NVL (P.INVOICE_QTY, 0))
            DISTRIBUTIONABLE_QTY
    FROM INVOICE_MST M
         LEFT OUTER JOIN INVOICE_DTL D ON D.MST_ID = M.MST_ID
         LEFT OUTER JOIN DEPOT_CUSTOMER_DIST_INVOICE I
            ON I.INVOICE_NO = M.INVOICE_NO
         LEFT OUTER JOIN DEPOT_CUSTOMER_DIST_PRODUCT P
            ON P.DEPOT_INV_ID = I.DEPOT_INV_ID AND P.SKU_CODE = D.SKU_CODE
   WHERE M.INVOICE_NO = :param1
GROUP BY M.INVOICE_NO, D.SKU_CODE";


        public async Task<string> GetDistributionRoutes(string db, int companyId, int unitId)
        {
            var dt = await _commonServices.GetDataSetAsyn(_configuration.GetConnectionString(db), DistributionRoute_Query(),
                _commonServices.AddParameter(new string[] { companyId.ToString(), unitId.ToString() }));
            return _commonServices.DataSetToJSON(dt);
        }

        public async Task<string> GetPendingInvoices(string db, int companyId, int unitId)
        {
            var query = String.Format(@"begin :param1 := fn_invoice_pending_for_dist({0},'{1}'); 
                          end;", companyId, unitId);

            var dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { "RefCursor" }));
            //var dt = await _commonServices.GetDataSetAsyn(_configuration.GetConnectionString(db), PendingInvoice_Query(),
            //    _commonServices.AddParameter(new string[] { companyId.ToString(), unitId.ToString() }));
            var ds = new DataSet();
            ds.Tables.Add(dt);

            return _commonServices.DataSetToJSON(ds);
        }

        public async Task<string> GetProductsByInvoice(string db, int companyId, int unitId, string invoiceNo)
        {
            var query = String.Format(@"begin :param1 := fn_product_pending_for_dist({0},'{1}', {2}); 
                          end;", companyId, unitId, invoiceNo);
            var dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { "RefCursor" }));
            //var dt = await _commonServices.GetDataSetAsyn(_configuration.GetConnectionString(db), GetProductsByInvoice_Query(),
            //    _commonServices.AddParameter(new string[] { companyId.ToString(), invoiceNo }));
            var ds = new DataSet();
            ds.Tables.Add(dt);

            return _commonServices.DataSetToJSON(ds);
        }

        public async Task<string> GetGiftByInvoice(string db, int companyId, int unitId, string invoiceNo)
        {
            var query = String.Format(@"begin :param1 := fn_gift_pending_for_dist({0},'{1}', {2}); 
                          end;", companyId, unitId, invoiceNo);
            var dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { "RefCursor" }));
            return _commonServices.DataTableToJSON(dt);
        }

        public async Task<string> GetInvoiceProducts(string db, int companyId, int unitId, List<string> invoiceNos)
        {
            try
            {
                InvoiceProductsAndGifts Invoice = new InvoiceProductsAndGifts();
                Invoice.Products = new DataSet();

                for (int i = 0; i < invoiceNos.Count; i++)
                {
                    DataTable dt = new DataTable();

                    var query = String.Format(@"begin :param1 := fn_product_pending_for_dist({0},'{1}', '{2}'); 
                          end;", companyId, unitId, invoiceNos[i]);
                    dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { "RefCursor" }));
                    Invoice.Products.Tables.Add(dt);


                }
                return _commonServices.DataSetToJSON(Invoice.Products);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<string> GetInvoiceGifts(string db, int companyId, int unitId, List<string> invoiceNos)
        {
            try
            {
                InvoiceProductsAndGifts Invoice = new InvoiceProductsAndGifts();
                Invoice.Gifts = new DataSet();

                for (int i = 0; i < invoiceNos.Count; i++)
                {
                    DataTable dt1 = new DataTable();

                    var query1 = String.Format(@"begin :param1 := fn_gift_pending_for_dist({0},'{1}', '{2}'); 
                          end;", companyId, unitId, invoiceNos[i]);
                    dt1 = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query1, _commonServices.AddParameter(new string[] { "RefCursor" }));
                    Invoice.Gifts.Tables.Add(dt1);

                }
                return _commonServices.DataSetToJSON(Invoice.Gifts);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //public async Task<string> CustomerByRoute(string db, int companyId, int unitId, string routeId)
        //{
        //    var query = @"SELECT * FROM CUSTOMER_INFO
        //        WHERE COMPANY_ID=:param1
        //        AND UNIT_ID=:param2
        //        AND ROUTE_ID in (" + routeId + ")";

        //    var dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { companyId.ToString(), unitId.ToString() }));
        //    return _commonServices.DataTableToJSON(dt);
        //}

        //private async Task<List<PendingDeliverytCustomerList>> LoadPendingInvoiceWithCustomer(string db, int companyId, int unitId,string routeId)
        //{
        //    string[] Route_Ids = routeId.Split(',');
        //    List<int> Route_Id_list = new List<int>();
        //    foreach(var item in Route_Ids)
        //    {
        //        Route_Id_list.Add(Convert.ToInt32(item));
        //    }
        //    var invoice_pending = String.Format(@"begin :param1 := fn_invoice_pending_for_dist({0},'{1}'); 
        //                  end;", companyId, unitId);
        //    var dt0 = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), invoice_pending, _commonServices.AddParameter(new string[] { "RefCursor" }));
        //    List<PendingDeliverytCustomerList> _pending_inv = new List<PendingDeliverytCustomerList>();

        //    foreach(var item in Route_Id_list)
        //    {
        //        for (int i = 0; i < dt0.Rows.Count; i++)
        //        {
        //            PendingDeliverytCustomerList _list = new PendingDeliverytCustomerList();

        //            _list.ROUTE_ID = dt0.Rows[0]["ROUTE_ID"] != null && dt0.Rows[0]["ROUTE_ID"].ToString() != "" ? Convert.ToInt32(dt0.Rows[0]["ROUTE_ID"].ToString()) : 0;
        //            if (_list.ROUTE_ID != 0 && _list.ROUTE_ID==item)
        //            {
        //                _list.COMPANY_ID = Convert.ToInt32(dt0.Rows[0]["COMPANY_ID"].ToString());
        //                _list.INVOICE_DATE = dt0.Rows[0]["INVOICE_DATE"].ToString();
        //                _list.INVOICE_UNIT_ID = Convert.ToInt32(dt0.Rows[0]["INVOICE_UNIT_ID"].ToString());
        //                _list.CUSTOMER_ID = Convert.ToInt32(dt0.Rows[0]["CUSTOMER_ID"].ToString());
        //                _list.CUSTOMER_CODE = dt0.Rows[0]["CUSTOMER_CODE"].ToString();
        //                _list.CUSTOMER_NAME = dt0.Rows[0]["CUSTOMER_NAME"].ToString();
        //                _list.MARKET_CODE = dt0.Rows[0]["MARKET_CODE"].ToString();
        //                _list.INVOICE_NO = dt0.Rows[0]["INVOICE_NO"].ToString();
        //                _pending_inv.Add(_list);
        //            }


        //        }
        //    }

        //    if (_pending_inv == null)
        //    {
        //        _pending_inv = new List<PendingDeliverytCustomerList>();

        //    }
        //    return _pending_inv;
        //}
//        private async Task<List<PendingDeliveryInvoiceWithSKU>> LoadPendingSKUWithInvoice(string db, int companyId, int unitId, string Invoice_Nos)
//        {
//            try
//            {

//                var sku_pending = @"Select I.ORDER_NO, I.ORDER_MST_ID, I.MST_ID, I.INVOICE_NO, I.SKU_ID, I.SKU_CODE, I.SKU_NAME,
//I.PACK_SIZE, I.SHIPPER_QTY, I.INVOICE_QTY, I.BONUS_QTY, I.TOTAL_QTY, I.COMPANY_ID, I.INVOICE_UNIT_ID, 
//I.PER_SHIPPER_WEIGHT, I.SHIPPER_WEIGHT_UNIT, I.PER_SHIPPER_VOLUME, I.SHIPPER_VOLUME_UNIT, I.PACK_VALUE,
//I.PACK_UNIT, I.WEIGHT_PER_PACK, I.WEIGHT_UNIT,C.CUSTOMER_ID,C.CUSTOMER_NAME,C.CUSTOMER_CODE
//from VW_INVOICE_PRODUCT_FOR_DIST I
//Left outer join INVOICE_MST M on M.MST_ID = I.MST_ID
//LEFT OUTER join CUSTOMER_INFO C on C.CUSTOMER_ID = M.CUSTOMER_ID
//Where I.COMPANY_ID =:param1 and I.INVOICE_UNIT_ID = :param2";
//                List<PendingDeliveryInvoiceWithSKU> _pending_sku = new List<PendingDeliveryInvoiceWithSKU>();

//                if (Invoice_Nos != "")
//                {
//                    sku_pending += " and I.INVOICE_NO in (" + Invoice_Nos + ")";


//                    var dt0 = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), sku_pending, _commonServices.AddParameter(new string[] { companyId.ToString(), unitId.ToString() }));



//                    for (int i = 0; i < dt0.Rows.Count; i++)
//                    {
//                        PendingDeliveryInvoiceWithSKU _list = new PendingDeliveryInvoiceWithSKU();
//                        _list.ORDER_MST_ID = Convert.ToInt32(dt0.Rows[i]["ORDER_MST_ID"].ToString());
//                        _list.ORDER_NO = dt0.Rows[i]["ORDER_NO"].ToString();
//                        _list.INVOICE_UNIT_ID = Convert.ToInt32(dt0.Rows[i]["INVOICE_UNIT_ID"].ToString());
//                        _list.MST_ID = Convert.ToInt32(dt0.Rows[i]["MST_ID"].ToString());
//                        _list.INVOICE_NO = dt0.Rows[i]["INVOICE_NO"].ToString();
//                        _list.SKU_ID = Convert.ToInt32(dt0.Rows[i]["SKU_ID"].ToString());
//                        _list.SKU_CODE = dt0.Rows[i]["SKU_CODE"].ToString();
//                        _list.SKU_NAME = dt0.Rows[i]["SKU_NAME"].ToString();
//                        _list.PACK_SIZE = dt0.Rows[i]["PACK_SIZE"].ToString();
//                        _list.SHIPPER_QTY = dt0.Rows[i]["SHIPPER_QTY"] != null && dt0.Rows[i]["SHIPPER_QTY"].ToString() != "" ? Convert.ToDecimal(dt0.Rows[i]["SHIPPER_QTY"].ToString()) : 0;
//                        _list.INVOICE_QTY = dt0.Rows[i]["INVOICE_QTY"] != null && dt0.Rows[i]["INVOICE_QTY"].ToString() != "" ? Convert.ToDecimal(dt0.Rows[i]["INVOICE_QTY"].ToString()) : 0;
//                        _list.BONUS_QTY = dt0.Rows[i]["BONUS_QTY"] != null && dt0.Rows[i]["BONUS_QTY"].ToString() != "" ? Convert.ToDecimal(dt0.Rows[i]["BONUS_QTY"].ToString()) : 0;
//                        _list.TOTAL_QTY = dt0.Rows[i]["TOTAL_QTY"] != null && dt0.Rows[i]["TOTAL_QTY"].ToString() != "" ? Convert.ToDecimal(dt0.Rows[i]["TOTAL_QTY"].ToString()) : 0;
//                        _list.COMPANY_ID = dt0.Rows[i]["COMPANY_ID"] != null && dt0.Rows[i]["COMPANY_ID"].ToString() != "" ? Convert.ToInt32(dt0.Rows[i]["COMPANY_ID"].ToString()) : 0;
//                        _list.PER_SHIPPER_WEIGHT = dt0.Rows[i]["PER_SHIPPER_WEIGHT"] != null && dt0.Rows[i]["PER_SHIPPER_WEIGHT"].ToString() != "" ? Convert.ToDecimal(dt0.Rows[i]["PER_SHIPPER_WEIGHT"].ToString()) : 0;
//                        _list.SHIPPER_WEIGHT_UNIT = dt0.Rows[i]["SHIPPER_WEIGHT_UNIT"] != null && dt0.Rows[i]["SHIPPER_WEIGHT_UNIT"].ToString() != "" ? "" : dt0.Rows[i]["SHIPPER_WEIGHT_UNIT"].ToString();
//                        _list.PER_SHIPPER_VOLUME = dt0.Rows[i]["PER_SHIPPER_VOLUME"] != null && dt0.Rows[i]["PER_SHIPPER_VOLUME"].ToString() != "" ? Convert.ToDecimal(dt0.Rows[i]["PER_SHIPPER_VOLUME"].ToString()) : 0;
//                        _list.SHIPPER_VOLUME_UNIT = dt0.Rows[i]["SHIPPER_VOLUME_UNIT"] != null && dt0.Rows[i]["SHIPPER_VOLUME_UNIT"].ToString() != "" ? "" : dt0.Rows[i]["SHIPPER_VOLUME_UNIT"].ToString();
//                        _list.PACK_VALUE = dt0.Rows[i]["PACK_VALUE"] != null && dt0.Rows[i]["PACK_VALUE"].ToString() != "" ? Convert.ToDecimal(dt0.Rows[i]["PACK_VALUE"].ToString()) : 0;
//                        _list.PACK_UNIT = dt0.Rows[i]["PACK_UNIT"] != null && dt0.Rows[i]["PACK_UNIT"].ToString() != "" ? dt0.Rows[i]["PACK_UNIT"].ToString() : "";
//                        _list.WEIGHT_PER_PACK = dt0.Rows[i]["WEIGHT_PER_PACK"] != null && dt0.Rows[i]["WEIGHT_PER_PACK"].ToString() != "" ? Convert.ToDecimal(dt0.Rows[i]["WEIGHT_PER_PACK"].ToString()) : 0;
//                        _list.WEIGHT_UNIT = dt0.Rows[i]["WEIGHT_UNIT"] != null && dt0.Rows[i]["WEIGHT_UNIT"].ToString() != "" ? dt0.Rows[i]["WEIGHT_UNIT"].ToString() : "";
//                        _list.CUSTOMER_ID = Convert.ToInt32(dt0.Rows[i]["CUSTOMER_ID"].ToString());
//                        _list.CUSTOMER_CODE = dt0.Rows[i]["CUSTOMER_CODE"].ToString();
//                        _list.CUSTOMER_NAME = dt0.Rows[i]["CUSTOMER_NAME"].ToString();

//                        _pending_sku.Add(_list);
//                    }

//                }



//                if (_pending_sku == null)
//                {
//                    _pending_sku = new List<PendingDeliveryInvoiceWithSKU>();

//                }
//                return _pending_sku;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }


//        }

        private async Task<List<PendingDeliveryInvoiceWithSKU>> LoadPendingSKUWithInvoice(string db, int companyId, int unitId, string Invoice_Nos)
        {
            try
            {
                var sku_pending = @"Select I.ORDER_NO, I.ORDER_MST_ID, I.MST_ID, I.INVOICE_NO, I.SKU_ID, I.SKU_CODE, I.SKU_NAME,
        I.PACK_SIZE, I.SHIPPER_QTY, I.INVOICE_QTY, I.BONUS_QTY, I.TOTAL_QTY, I.COMPANY_ID, I.INVOICE_UNIT_ID, 
        I.PER_SHIPPER_WEIGHT, I.SHIPPER_WEIGHT_UNIT, I.PER_SHIPPER_VOLUME, I.SHIPPER_VOLUME_UNIT, I.PACK_VALUE,
        I.PACK_UNIT, I.WEIGHT_PER_PACK, I.WEIGHT_UNIT,C.CUSTOMER_ID,C.CUSTOMER_NAME,C.CUSTOMER_CODE
        from VW_INVOICE_PRODUCT_FOR_DIST I
        Left outer join INVOICE_MST M on M.MST_ID = I.MST_ID
        LEFT OUTER join CUSTOMER_INFO C on C.CUSTOMER_ID = M.CUSTOMER_ID
        Where I.COMPANY_ID = :param1 and I.INVOICE_UNIT_ID = :param2";

                List<PendingDeliveryInvoiceWithSKU> _pending_sku = new List<PendingDeliveryInvoiceWithSKU>();

                if (!string.IsNullOrEmpty(Invoice_Nos))
                {
                    // Split Invoice_Nos into chunks of 1000 or less
                    var invoiceNosList = Invoice_Nos.Split(',').ToList();
                    var chunkSize = 1000;
                    var chunks = new List<List<string>>();

                    for (int i = 0; i < invoiceNosList.Count; i += chunkSize)
                    {
                        chunks.Add(invoiceNosList.Skip(i).Take(chunkSize).ToList());
                    }

                    // Loop through chunks and query the database for each chunk
                    foreach (var chunk in chunks)
                    {
                        var chunkInvoiceNos = string.Join(",", chunk);
                        var queryWithChunk = sku_pending + " and I.INVOICE_NO in (" + chunkInvoiceNos + ")";

                        var dt0 = await _commonServices.GetDataTableAsyn(
                            _configuration.GetConnectionString(db),
                            queryWithChunk,
                            _commonServices.AddParameter(new string[] { companyId.ToString(), unitId.ToString() })
                        );

                        for (int i = 0; i < dt0.Rows.Count; i++)
                        {
                            PendingDeliveryInvoiceWithSKU _list = new PendingDeliveryInvoiceWithSKU();
                            _list.ORDER_MST_ID = Convert.ToInt32(dt0.Rows[i]["ORDER_MST_ID"].ToString());
                            _list.ORDER_NO = dt0.Rows[i]["ORDER_NO"].ToString();
                            _list.INVOICE_UNIT_ID = Convert.ToInt32(dt0.Rows[i]["INVOICE_UNIT_ID"].ToString());
                            _list.MST_ID = Convert.ToInt32(dt0.Rows[i]["MST_ID"].ToString());
                            _list.INVOICE_NO = dt0.Rows[i]["INVOICE_NO"].ToString();
                            _list.SKU_ID = Convert.ToInt32(dt0.Rows[i]["SKU_ID"].ToString());
                            _list.SKU_CODE = dt0.Rows[i]["SKU_CODE"].ToString();
                            _list.SKU_NAME = dt0.Rows[i]["SKU_NAME"].ToString();
                            _list.PACK_SIZE = dt0.Rows[i]["PACK_SIZE"].ToString();
                            _list.SHIPPER_QTY = dt0.Rows[i]["SHIPPER_QTY"] != null && dt0.Rows[i]["SHIPPER_QTY"].ToString() != "" ? Convert.ToDecimal(dt0.Rows[i]["SHIPPER_QTY"].ToString()) : 0;
                            _list.INVOICE_QTY = dt0.Rows[i]["INVOICE_QTY"] != null && dt0.Rows[i]["INVOICE_QTY"].ToString() != "" ? Convert.ToDecimal(dt0.Rows[i]["INVOICE_QTY"].ToString()) : 0;
                            _list.BONUS_QTY = dt0.Rows[i]["BONUS_QTY"] != null && dt0.Rows[i]["BONUS_QTY"].ToString() != "" ? Convert.ToDecimal(dt0.Rows[i]["BONUS_QTY"].ToString()) : 0;
                            _list.TOTAL_QTY = dt0.Rows[i]["TOTAL_QTY"] != null && dt0.Rows[i]["TOTAL_QTY"].ToString() != "" ? Convert.ToDecimal(dt0.Rows[i]["TOTAL_QTY"].ToString()) : 0;
                            _list.COMPANY_ID = dt0.Rows[i]["COMPANY_ID"] != null && dt0.Rows[i]["COMPANY_ID"].ToString() != "" ? Convert.ToInt32(dt0.Rows[i]["COMPANY_ID"].ToString()) : 0;
                            _list.PER_SHIPPER_WEIGHT = dt0.Rows[i]["PER_SHIPPER_WEIGHT"] != null && dt0.Rows[i]["PER_SHIPPER_WEIGHT"].ToString() != "" ? Convert.ToDecimal(dt0.Rows[i]["PER_SHIPPER_WEIGHT"].ToString()) : 0;
                            _list.SHIPPER_WEIGHT_UNIT = dt0.Rows[i]["SHIPPER_WEIGHT_UNIT"] != null && dt0.Rows[i]["SHIPPER_WEIGHT_UNIT"].ToString() != "" ? "" : dt0.Rows[i]["SHIPPER_WEIGHT_UNIT"].ToString();
                            _list.PER_SHIPPER_VOLUME = dt0.Rows[i]["PER_SHIPPER_VOLUME"] != null && dt0.Rows[i]["PER_SHIPPER_VOLUME"].ToString() != "" ? Convert.ToDecimal(dt0.Rows[i]["PER_SHIPPER_VOLUME"].ToString()) : 0;
                            _list.SHIPPER_VOLUME_UNIT = dt0.Rows[i]["SHIPPER_VOLUME_UNIT"] != null && dt0.Rows[i]["SHIPPER_VOLUME_UNIT"].ToString() != "" ? "" : dt0.Rows[i]["SHIPPER_VOLUME_UNIT"].ToString();
                            _list.PACK_VALUE = dt0.Rows[i]["PACK_VALUE"] != null && dt0.Rows[i]["PACK_VALUE"].ToString() != "" ? Convert.ToDecimal(dt0.Rows[i]["PACK_VALUE"].ToString()) : 0;
                            _list.PACK_UNIT = dt0.Rows[i]["PACK_UNIT"] != null && dt0.Rows[i]["PACK_UNIT"].ToString() != "" ? dt0.Rows[i]["PACK_UNIT"].ToString() : "";
                            _list.WEIGHT_PER_PACK = dt0.Rows[i]["WEIGHT_PER_PACK"] != null && dt0.Rows[i]["WEIGHT_PER_PACK"].ToString() != "" ? Convert.ToDecimal(dt0.Rows[i]["WEIGHT_PER_PACK"].ToString()) : 0;
                            _list.WEIGHT_UNIT = dt0.Rows[i]["WEIGHT_UNIT"] != null && dt0.Rows[i]["WEIGHT_UNIT"].ToString() != "" ? dt0.Rows[i]["WEIGHT_UNIT"].ToString() : "";
                            _list.CUSTOMER_ID = Convert.ToInt32(dt0.Rows[i]["CUSTOMER_ID"].ToString());
                            _list.CUSTOMER_CODE = dt0.Rows[i]["CUSTOMER_CODE"].ToString();
                            _list.CUSTOMER_NAME = dt0.Rows[i]["CUSTOMER_NAME"].ToString();

                            _pending_sku.Add(_list);
                        }
                    }
                }

                return _pending_sku ?? new List<PendingDeliveryInvoiceWithSKU>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //im//
        public async Task<string> CustomerByRoute(string db, int companyId, int unitId, string routeId)
        {
            try
            {
                string Invoice_Nos = "";
                List<int> Route_Id_list = routeId.Split(',').Select(int.Parse).ToList();
                string invoice_pending = String.Format(@"begin :param1 := fn_invoice_pending_for_dist({0},'{1}');end;", companyId, unitId);
                var dt0 = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), invoice_pending, _commonServices.AddParameter(new string[] { "RefCursor" }));
                List<PendingDeliverytCustomerList> _pending_inv = new List<PendingDeliverytCustomerList>();
                if (dt0.Rows.Count>0)
                {
                    foreach (DataRow row in dt0.Rows)
                    {
                        int ROUTE_ID = Convert.ToInt32(row["ROUTE_ID"]);
                        if (Route_Id_list.Contains(ROUTE_ID))
                        {
                            PendingDeliverytCustomerList _list = new PendingDeliverytCustomerList
                            {
                                COMPANY_ID = Convert.ToInt32(row["COMPANY_ID"].ToString()),
                                INVOICE_DATE = row["INVOICE_DATE"].ToString(),
                                INVOICE_UNIT_ID = Convert.ToInt32(row["INVOICE_UNIT_ID"].ToString()),
                                CUSTOMER_ID = Convert.ToInt32(row["CUSTOMER_ID"].ToString()),
                                CUSTOMER_CODE = row["CUSTOMER_CODE"].ToString(),
                                CUSTOMER_NAME = row["CUSTOMER_NAME"].ToString(),
                                MARKET_CODE = row["MARKET_CODE"].ToString(),
                                INVOICE_NO = row["INVOICE_NO"].ToString(),
                                SERIAL_NO = Convert.ToInt32(row["SERIAL_NO"].ToString()),
                                NET_INVOICE_AMOUNT = row["NET_INVOICE_AMOUNT"].ToString(),
                                NET_INVOICE_QTY = row["NET_INVOICE_QTY"].ToString(),
                                ROUTE_ID= ROUTE_ID
                            };
                            _pending_inv.Add(_list);
                            if (!Invoice_Nos.Contains(_list.INVOICE_NO))
                            {
                                if (Invoice_Nos == "")
                                {
                                    Invoice_Nos = "'" + _list.INVOICE_NO + "'";
                                }
                                else
                                {
                                    Invoice_Nos += ",'" + _list.INVOICE_NO + "'";
                                }
                            }
                        }
                    }

                    if (_pending_inv.Count <=0 )
                    {
                        return "No invoice Found!";
                    }

                    List<PendingDeliveryInvoiceWithSKU> _pending_sku = await LoadPendingSKUWithInvoice(db, companyId, unitId, Invoice_Nos);

                    DataTable D01 = _commonServices.ListToDataTable(_pending_inv.OrderBy(x => x.SERIAL_NO).ToList());
                    DataTable D02 = _commonServices.ListToDataTable(_pending_sku);
                    DataSet DSet = new DataSet();
                    DSet.Tables.Add(D01);
                    DSet.Tables.Add(D02);
                    return _commonServices.DataSetToJSON(DSet);
                }
                return "No Route Found!";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> AddOrUpdate(string db, Depot_Customer_Dist_Mst model)
        {

            if (model == null)
            {
                throw new Exception("No data provided to insert!!!!");
            }
            else
            {
                
                List<QueryPattern> listOfQuery = new List<QueryPattern>();
                var lastInvId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetLastInvoice_IdQuery(), _commonServices.AddParameter(new string[] { }));
                var lastInvProductId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetLastProdInvoice_IdQuery(), _commonServices.AddParameter(new string[] { }));
                var lastGiftBatchId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetLastGiftInvoice_IdQuery(), _commonServices.AddParameter(new string[] { }));

                if (model.MST_ID == 0)
                {
                    var (flagInsert, mgsInsert) = await CheckDelivaryQty(db, model);
                    if (flagInsert)
                    {
                        //model.DIVISION_CODE = await GenerateDivisionCode(db, model.COMPANY_ID.ToString());
                        model.MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewID_IdQuery(), _commonServices.AddParameter(new string[] { }));
                        model.DISTRIBUTION_NO = _commonServices.GenerateRequisitionCode(_configuration.GetConnectionString(db), "FN_GENERATE_DISTRIBUTION_NO", model.COMPANY_ID.ToString(), model.INVOICE_UNIT_ID.ToString());
                        string check_validity = "1";  /*await Check_Validity_Distribution(db, model);*/
                        if (check_validity == "1")
                        {
                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Master_AddQuery(), _commonServices.AddParameter(new string[] {
                                model.MST_ID.ToString(),
                                model.DISTRIBUTION_NO,
                                model.DISTRIBUTION_DATE,
                                model.DIST_ROUTE_ID.ToString(),
                                model.VEHICLE_NO,
                                model.VEHICLE_DESCRIPTION,
                                model.VEHICLE_TOTAL_VOLUME.ToString(),
                                model.VEHICLE_TOTAL_WEIGHT.ToString(),
                                model.DRIVER_ID,
                                model.DISTRIBUTION_BY,
                                model.STATUS,
                                model.COMPANY_ID.ToString(),
                                model.INVOICE_UNIT_ID.ToString(),
                                model.REMARKS,
                                model.ENTERED_BY,
                                model.ENTERED_DATE,
                                model.ENTERED_TERMINAL,
                                model.TOTAL_VOLUMN.ToString(),
                                model.TOTAL_WEIGHT.ToString(),
                                model.DRIVER_PHONE
                     })));

                            var uniqueCustomer = model.Invoices.Select(e => e.CUSTOMER_ID).Distinct();
                            Dictionary<int, string> challanNo = new Dictionary<int, string>();
                            foreach (var customer in uniqueCustomer)
                            {
                                challanNo.Add(customer, _commonServices.GenerateRequisitionCode(_configuration.GetConnectionString(db), "FN_GENERATE_CUST_CHALLAN_NO", model.COMPANY_ID.ToString(), model.INVOICE_UNIT_ID.ToString(), customer.ToString()));
                            }

                            foreach (var invoice in model.Invoices)
                            {
                                var InvTypeCode = "";
                                var OrderNo = "";
                                int orderMstId = 0;
                                int orderUnitId = 0;
                                var OrderDate = "";
                                DataTable invTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetInvoice_Query(), _commonServices.AddParameter(new string[] { invoice.INVOICE_NO.ToString() }));

                                if (invTable.Rows.Count > 0)
                                {
                                    InvTypeCode = invTable.Rows[0]["INVOICE_TYPE_CODE"].ToString();
                                    OrderNo = invTable.Rows[0]["ORDER_NO"].ToString();
                                    OrderDate = invTable.Rows[0]["ORDER_DATE"].ToString();
                                    DataTable orderTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetOrder_Query(), _commonServices.AddParameter(new string[] { OrderNo.ToString() }));
                                    if (orderTable.Rows.Count > 0)
                                    {
                                        orderMstId = Convert.ToInt32(invTable.Rows[0]["ORDER_MST_ID"]);
                                        orderUnitId = Convert.ToInt32(invTable.Rows[0]["INVOICE_UNIT_ID"]);
                                        OrderNo = invTable.Rows[0]["ORDER_NO"].ToString();
                                        OrderDate = invTable.Rows[0]["ORDER_DATE"].ToString();
                                    }
                                }

                                invoice.DEPOT_INV_ID = ++lastInvId;
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_DistInvoice_AddQuery(), _commonServices.AddParameter(new string[] {
                                invoice.DEPOT_INV_ID.ToString(),
                                model.MST_ID.ToString(),
                                invoice.INVOICE_NO,
                                invoice.INVOICE_DATE,
                                invoice.INVOICE_UNIT_ID.ToString(),
                                invoice.CUSTOMER_ID.ToString(),
                                invoice.CUSTOMER_CODE,
                                invoice.DIST_ROUTE_ID.ToString(),
                                //model.DIST_ROUTE_ID.ToString(),
                                invoice.STATUS,
                                invoice.COMPANY_ID.ToString(),
                                model.ENTERED_BY,
                                model.ENTERED_DATE,
                                model.ENTERED_TERMINAL,
                                orderMstId.ToString(),
                                OrderNo,
                                InvTypeCode,
                                orderUnitId.ToString(),
                                challanNo[invoice.CUSTOMER_ID],
                                OrderDate
                            })));

                                foreach (var product in invoice.Products)
                                {
                                    listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_DistProduct_AddQuery(), _commonServices.AddParameter(new string[] {
                                        (++lastInvProductId).ToString(),
                                        invoice.DEPOT_INV_ID.ToString(),
                                        model.MST_ID.ToString(),
                                        product.SKU_ID.ToString(),
                                        product.SKU_CODE,
                                        product.UNIT_TP.ToString(),
                                        product.INVOICE_QTY.ToString(),
                                        product.BONUS_QTY.ToString(),
                                        product.DISTRIBUTION_QTY.ToString(),
                                        product.DISTRIBUTION_BONUS_QTY.ToString(),
                                        product.TOTAL_DISTRIBUTION_QTY.ToString(),
                                        product.SHIPPER_QTY.ToString(),
                                        product.NO_OF_SHIPPER.ToString(),
                                        product.LOOSE_QTY.ToString(),
                                        product.SHIPPER_WEIGHT.ToString(),
                                        product.SHIPPER_VOLUMN.ToString(),
                                        product.LOOSE_WEIGHT.ToString(),
                                        product.LOOSE_VOLUMN.ToString(),
                                        product.TOTAL_WEIGHT.ToString(),
                                        product.TOTAL_VOLUMN.ToString(),
                                        product.COMPANY_ID.ToString(),
                                        model.INVOICE_UNIT_ID.ToString(),
                                        model.ENTERED_BY.ToString(),
                                        model.ENTERED_DATE,
                                        model.ENTERED_TERMINAL,
                                        model.DISTRIBUTION_DATE,
                                        invoice.INVOICE_NO
                                    })));
                                }
                                if (invoice.Gift_Batches != null)
                                {
                                    foreach (var gift in invoice.Gift_Batches)
                                    {
                                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_DistGift_AddQuery(), _commonServices.AddParameter(new string[] {
                                    (++lastGiftBatchId).ToString(),
                                    invoice.DEPOT_INV_ID.ToString(),
                                    model.MST_ID.ToString(),
                                    gift.GIFT_ITEM_ID.ToString(),
                                    gift.UNIT_TP.ToString(),
                                    gift.BATCH_ID.ToString(),
                                    gift.BATCH_NO.ToString(),
                                    gift.GIFT_QTY.ToString(),
                                    model.COMPANY_ID.ToString(),
                                    model.INVOICE_UNIT_ID.ToString(),
                                    model.ENTERED_BY,
                                    model.ENTERED_DATE,
                                    model.ENTERED_TERMINAL
                                })));
                                    }

                                }
                            }
                        }
                        else
                        {
                            return check_validity;
                        }
                    }
                    else
                    {
                        throw new Exception(mgsInsert);
                    }
                }
                else
                {
                    var (flagUpdate, mgsUpdate) = await CheckDelivaryQty(db, model);
                    if (flagUpdate)
                    {
                        DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"SELECT CONFIRMED FROM DEPOT_CUSTOMER_DIST_MST WHERE DISTRIBUTION_NO = :param1", _commonServices.AddParameter(new string[] { model.DISTRIBUTION_NO }));
                        if (dataTable.Rows.Count > 0)
                        {
                            if (dataTable.Rows[0]["CONFIRMED"].ToString() == "1")
                            {
                                throw new Exception("You cannot modify this dispatch because it has been Confirmed");
                            }
                        }

                        var mst = await GetDataById(db, model.COMPANY_ID, model.MST_ID.ToString());
                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Master_UpdateQuery(), _commonServices.AddParameter(new string[] {
                                model.MST_ID.ToString(),
                                model.DISTRIBUTION_NO,
                                model.DISTRIBUTION_DATE,
                                model.DIST_ROUTE_ID.ToString(),
                                model.VEHICLE_NO,
                                model.VEHICLE_DESCRIPTION,
                                model.VEHICLE_TOTAL_VOLUME.ToString(),
                                model.VEHICLE_TOTAL_WEIGHT.ToString(),
                                model.DRIVER_ID,
                                model.DISTRIBUTION_BY,
                                model.STATUS,
                                model.COMPANY_ID.ToString(),
                                model.INVOICE_UNIT_ID.ToString(),
                                model.REMARKS,
                                model.UPDATED_BY,
                                model.UPDATED_DATE,
                                model.UPDATED_TERMINAL,
                                model.TOTAL_VOLUMN.ToString(),
                                model.TOTAL_WEIGHT.ToString(),
                                model.CONFIRMED.ToString(),
                                model.DRIVER_PHONE
                            })));

                        var uniqueCustomer = model.Invoices.Select(e => e.CUSTOMER_ID).Distinct();
                        Dictionary<int, string> challanNo = new Dictionary<int, string>();
                        foreach (var customer in uniqueCustomer)
                        {
                            if (mst.Invoices.Any(e => e.CUSTOMER_ID == customer))
                            {
                                challanNo.Add(customer, mst.Invoices
                                    .Where(e => e.CUSTOMER_ID == customer)
                                    .Select(e => e.CUSTOMER_CHALLAN_NO)
                                    .FirstOrDefault());
                            }
                            else
                            {
                                challanNo.Add(customer, _commonServices.GenerateRequisitionCode(_configuration.GetConnectionString(db), "FN_GENERATE_CUST_CHALLAN_NO", model.COMPANY_ID.ToString(), model.INVOICE_UNIT_ID.ToString(), customer.ToString()));
                            }
                        }

                        foreach (var invoice in model.Invoices)
                        {
                            var InvTypeCode = "";
                            var OrderNo = "";
                            int orderMstId = 0;
                            int orderUnitId = 0;
                            var orderDate = "";
                            DataTable invTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetInvoice_Query(), _commonServices.AddParameter(new string[] { invoice.INVOICE_NO.ToString() }));
                            if (invTable.Rows.Count > 0)
                            {

                                InvTypeCode = invTable.Rows[0]["INVOICE_TYPE_CODE"].ToString();
                                OrderNo = invTable.Rows[0]["ORDER_NO"].ToString();
                                orderDate = invTable.Rows[0]["ORDER_DATE"].ToString();
                                DataTable orderTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetOrder_Query(), _commonServices.AddParameter(new string[] { OrderNo.ToString() }));
                                if (orderTable.Rows.Count > 0)
                                {
                                    orderMstId = Convert.ToInt32(invTable.Rows[0]["ORDER_MST_ID"]);
                                    orderUnitId = Convert.ToInt32(invTable.Rows[0]["INVOICE_UNIT_ID"]);
                                    OrderNo = invTable.Rows[0]["ORDER_NO"].ToString();
                                    orderDate = invTable.Rows[0]["ORDER_DATE"].ToString();
                                }
                            }
                            if (invoice.DEPOT_INV_ID == 0)
                            {
                                //new Invoice: add everything under this invoice
                                invoice.DEPOT_INV_ID = ++lastInvId;
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_DistInvoice_AddQuery(),
                                    _commonServices.AddParameter(new string[] {
                                        invoice.DEPOT_INV_ID.ToString(),
                                        model.MST_ID.ToString(),
                                        invoice.INVOICE_NO,
                                        invoice.INVOICE_DATE,
                                        invoice.INVOICE_UNIT_ID.ToString(),
                                        invoice.CUSTOMER_ID.ToString(),
                                        invoice.CUSTOMER_CODE,
                                        model.DIST_ROUTE_ID.ToString(),
                                        invoice.STATUS,
                                        invoice.COMPANY_ID.ToString(),
                                        model.UPDATED_BY,
                                        model.UPDATED_DATE, //entered date will be model updated date
                                        model.UPDATED_TERMINAL,
                                        orderMstId.ToString(),
                                        OrderNo,
                                        InvTypeCode,
                                        orderUnitId.ToString(),
                                        challanNo[invoice.CUSTOMER_ID],
                                        orderDate
                                    })));

                                foreach (var product in invoice.Products)
                                {
                                    listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_DistProduct_AddQuery(), _commonServices.AddParameter(new string[] {
                                        (++lastInvProductId).ToString(),
                                        invoice.DEPOT_INV_ID.ToString(),
                                        model.MST_ID.ToString(),
                                        product.SKU_ID.ToString(),
                                        product.SKU_CODE,
                                        product.UNIT_TP.ToString(),
                                        product.INVOICE_QTY.ToString(),
                                        product.BONUS_QTY.ToString(),
                                        product.DISTRIBUTION_QTY.ToString(),
                                        product.DISTRIBUTION_BONUS_QTY.ToString(),
                                        product.TOTAL_DISTRIBUTION_QTY.ToString(),
                                        product.SHIPPER_QTY.ToString(),
                                        product.NO_OF_SHIPPER.ToString(),
                                        product.LOOSE_QTY.ToString(),
                                        product.SHIPPER_WEIGHT.ToString(),
                                        product.SHIPPER_VOLUMN.ToString(),
                                        product.LOOSE_WEIGHT.ToString(),
                                        product.LOOSE_VOLUMN.ToString(),
                                        product.TOTAL_WEIGHT.ToString(),
                                        product.TOTAL_VOLUMN.ToString(),
                                        product.COMPANY_ID.ToString(),
                                        model.INVOICE_UNIT_ID.ToString(),
                                        model.UPDATED_BY,
                                        model.UPDATED_DATE, //entered date will be model updated date
                                        model.UPDATED_TERMINAL
                                    })));
                                }

                                if (invoice.Gift_Batches != null)
                                {
                                    foreach (var gift in invoice.Gift_Batches)
                                    {
                                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_DistGift_AddQuery(), _commonServices.AddParameter(new string[] {
                                       (++lastGiftBatchId).ToString(),
                                        invoice.DEPOT_INV_ID.ToString(),
                                        model.MST_ID.ToString(),
                                        gift.GIFT_ITEM_ID.ToString(),
                                        gift.UNIT_TP.ToString(),
                                        gift.BATCH_ID.ToString(),
                                        gift.BATCH_NO.ToString(),
                                        gift.GIFT_QTY.ToString(),
                                        model.COMPANY_ID.ToString(),
                                        model.INVOICE_UNIT_ID.ToString(),
                                        model.UPDATED_BY,
                                        model.UPDATED_DATE, //entered date for this will be model updated date
                                        model.UPDATED_TERMINAL
                                    })));
                                    }
                                }
                            }
                            else
                            {
                                //old Invoice
                                //update invoice
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_DistInvoice_UpdateQuery(), _commonServices.AddParameter(new string[] {
                                    invoice.DEPOT_INV_ID.ToString(),
                                    invoice.INVOICE_NO,
                                    invoice.INVOICE_DATE,
                                    invoice.INVOICE_UNIT_ID.ToString(),
                                    invoice.CUSTOMER_ID.ToString(),
                                    invoice.CUSTOMER_CODE,
                                    model.DIST_ROUTE_ID.ToString(),
                                    invoice.STATUS,
                                    invoice.COMPANY_ID.ToString(),
                                    model.UPDATED_BY,
                                    model.UPDATED_DATE,
                                    model.UPDATED_TERMINAL
                                })));
                                //foreach invoice update product
                                foreach (var product in invoice.Products)
                                {
                                    //add product
                                    if (product.DEPOT_PRODUCT_ID == 0)
                                    {
                                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_DistProduct_AddQuery(),
                                        _commonServices.AddParameter(new string[] {
                                            (++lastInvProductId).ToString(),
                                            invoice.DEPOT_INV_ID.ToString(),
                                            model.MST_ID.ToString(),
                                            product.SKU_ID.ToString(),
                                            product.SKU_CODE,
                                            product.UNIT_TP.ToString(),
                                            product.INVOICE_QTY.ToString(),
                                            product.BONUS_QTY.ToString(),
                                            product.DISTRIBUTION_QTY.ToString(),
                                            product.DISTRIBUTION_BONUS_QTY.ToString(),
                                            product.TOTAL_DISTRIBUTION_QTY.ToString(),
                                            product.SHIPPER_QTY.ToString(),
                                            product.NO_OF_SHIPPER.ToString(),
                                            product.LOOSE_QTY.ToString(),
                                            product.SHIPPER_WEIGHT.ToString(),
                                            product.SHIPPER_VOLUMN.ToString(),
                                            product.LOOSE_WEIGHT.ToString(),
                                            product.LOOSE_VOLUMN.ToString(),
                                            product.TOTAL_WEIGHT.ToString(),
                                            product.TOTAL_VOLUMN.ToString(),
                                            product.COMPANY_ID.ToString(),
                                            model.INVOICE_UNIT_ID.ToString(),
                                            model.UPDATED_BY,
                                            model.UPDATED_DATE, //entered date for this will be model updated date
                                            model.UPDATED_TERMINAL
                                        })));
                                    }
                                    //update product
                                    listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_DistProduct_UpdateQuery(),
                                        _commonServices.AddParameter(new string[] {
                                            product.DEPOT_PRODUCT_ID.ToString(),
                                            product.SKU_ID.ToString(),
                                            product.SKU_CODE,
                                            product.UNIT_TP.ToString(),
                                            product.INVOICE_QTY.ToString(),
                                            product.BONUS_QTY.ToString(),
                                            product.DISTRIBUTION_QTY.ToString(),
                                            product.DISTRIBUTION_BONUS_QTY.ToString(),
                                            product.TOTAL_DISTRIBUTION_QTY.ToString(),
                                            product.SHIPPER_QTY.ToString(),
                                            product.NO_OF_SHIPPER.ToString(),
                                            product.LOOSE_QTY.ToString(),
                                            product.SHIPPER_WEIGHT.ToString(),
                                            product.SHIPPER_VOLUMN.ToString(),
                                            product.LOOSE_WEIGHT.ToString(),
                                            product.LOOSE_VOLUMN.ToString(),
                                            product.TOTAL_WEIGHT.ToString(),
                                            product.TOTAL_VOLUMN.ToString(),
                                            product.COMPANY_ID.ToString(),
                                            product.INVOICE_UNIT_ID.ToString(),
                                            model.UPDATED_BY.ToString(),
                                            model.UPDATED_DATE,
                                            model.UPDATED_TERMINAL,
                                            model.DISTRIBUTION_DATE
                                        })));
                                }
                            }
                        }
                        //delete invoice
                        foreach (var invoice in mst.Invoices)
                        {
                            var inv = model.Invoices.Where(e => e.DEPOT_INV_ID == invoice.DEPOT_INV_ID).FirstOrDefault();
                            if (inv != null)
                            {
                                if (invoice.Products != null)
                                {
                                    foreach (var product in invoice.Products)
                                    {
                                        if (!inv.Products.Any(e => e.DEPOT_PRODUCT_ID == product.DEPOT_PRODUCT_ID))
                                        {
                                            //delete product batches
                                            if (product.Batches != null)
                                            {
                                                foreach (var batch in product.Batches)
                                                {
                                                    listOfQuery.Add(_commonServices.AddQuery(DeleteProductBatch_Query(),
                                                       _commonServices.AddParameter(new string[]
                                                           { batch.DEPOT_BATCH_ID.ToString() })));
                                                }
                                            }

                                            //delete product
                                            listOfQuery.Add(_commonServices.AddQuery(DeleteProduct_Query(), _commonServices.AddParameter(new string[] { product.DEPOT_PRODUCT_ID.ToString() })));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //delete invoice
                                foreach (var product in invoice.Products)
                                {
                                    //delete product
                                    listOfQuery.Add(_commonServices.AddQuery(DeleteProduct_Query(), _commonServices.AddParameter(new string[] { product.DEPOT_PRODUCT_ID.ToString() })));
                                }

                                if (invoice.Gift_Batches != null)
                                {
                                    foreach (var gift in invoice.Gift_Batches)
                                    {
                                        //delete Gift
                                        listOfQuery.Add(_commonServices.AddQuery(DeleteGift_Query(),
                                            _commonServices.AddParameter(new string[] { gift.DEPOT_BATCH_ID.ToString() })));
                                    }
                                }

                                listOfQuery.Add(_commonServices.AddQuery("DELETE FROM DEPOT_CUSTOMER_DIST_INVOICE WHERE   DEPOT_INV_ID= :param1 AND  MST_ID= :param2",
                                           _commonServices.AddParameter(new string[] { invoice.DEPOT_INV_ID.ToString(), invoice.MST_ID.ToString() })));

                            }
                        }
                    }
                    else
                    {
                        throw new Exception(mgsUpdate);
                    }
                }
                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                model.MST_ID_ENCRYPTED = _commonServices.Encrypt(model.MST_ID.ToString());
                return JsonConvert.SerializeObject(model);
               
            }
        }

        public async Task<string> LoadData(string db, DistributionFilterParameter receive_info)
        {
            var sql = LoadData_Master_Query();

            if (!string.IsNullOrEmpty(receive_info.VEHICLE_NO))
            {
                sql += " AND M.VEHICLE_NO = " + receive_info.VEHICLE_NO;
            }
            if (receive_info.DIST_ROUTE_ID != 0)
            {
                sql += " AND M.DIST_ROUTE_ID = " + receive_info.DIST_ROUTE_ID;
            }
            var data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), sql,
                _commonServices.AddParameter(new string[] {
                    receive_info.COMPANY_ID.ToString(),
                    receive_info.DATE_FROM,
                    receive_info.DATE_TO,
                    receive_info.INVOICE_UNIT_ID.ToString()
                }));

            for (var i = 0; i < data.Rows.Count; i++)
            {
                data.Rows[i]["MST_ID_ENCRYPTED"] = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(data);

            return _commonServices.DataSetToJSON(ds);
        }

        public async Task<string> GetNonConfirmDeliveryList(string db, DistributionFilterParameter receive_info)
        {
            var sql = LoadNonConfirmedList_Query();

            if (!string.IsNullOrEmpty(receive_info.VEHICLE_NO))
            {
                sql += " AND M.VEHICLE_NO = " + receive_info.VEHICLE_NO;
            }
            if (receive_info.DIST_ROUTE_ID != 0)
            {
                sql += " AND M.DIST_ROUTE_ID = " + receive_info.DIST_ROUTE_ID;
            }
            var data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), sql,
                _commonServices.AddParameter(new string[] {
                    receive_info.COMPANY_ID.ToString(),
                    receive_info.DATE_FROM,
                    receive_info.DATE_TO,
                    receive_info.INVOICE_UNIT_ID.ToString()
                }));

            for (var i = 0; i < data.Rows.Count; i++)
            {
                data.Rows[i]["MST_ID_ENCRYPTED"] = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(data);

            return _commonServices.DataSetToJSON(ds);
        }

        private async Task<Depot_Customer_Dist_Mst> GetDataById(string db, int companyId, string id)
        {
            var dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadDataById_Master_Query(),
                _commonServices.AddParameter(new string[] { companyId.ToString(), id }));

            var mst = dataTable.Rows[0].ToObject<Depot_Customer_Dist_Mst>();

            var invoices = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DistInvoiceByMstId_Query(),
                _commonServices.AddParameter(new string[] { companyId.ToString(), id }));
            mst.Invoices = invoices.ToList<Depot_Customer_Dist_Invoice>();

            foreach (var invoice in mst.Invoices)
            {
                var gifts = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DistGiftsByInvoiceId_Query(),
                _commonServices.AddParameter(new string[] {
                    companyId.ToString(), id, invoice.DEPOT_INV_ID.ToString()
                }));

                invoice.Gift_Batches = gifts.ToList<Depot_Customer_Dist_Gift_Batch>();

                var products = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DistProductByInvoiceId_Query(),
                _commonServices.AddParameter(new string[] {
                    companyId.ToString(), id, invoice.DEPOT_INV_ID.ToString()
                }));

                invoice.Products = products.ToList<Depot_Customer_Dist_Product>();

                foreach (var product in invoice.Products)
                {
                    var batches = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadBatchesByProductId_Query(),
                        _commonServices.AddParameter(new string[] {
                        companyId.ToString(), id, product.DEPOT_PRODUCT_ID.ToString()
                    }));
                    product.Batches = batches.ToList<Depot_Customer_Dist_Prod_Batch>();
                }
            }

            return mst;
        }

        public async Task<string> GetProductBatches(string db, string mst_id)
        {
            var id = _commonServices.Decrypt(mst_id);
            var data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadBatchesByMstId_Query(),
                _commonServices.AddParameter(new string[] { id }));

            DataSet ds = new DataSet();
            ds.Tables.Add(data);

            return _commonServices.DataSetToJSON(ds);
        }

        public async Task<string> GetEditDataById(string db, int companyId, string id)
        {
            var mst = await GetDataById(db, companyId, id);
            return JsonConvert.SerializeObject(mst);
        }

        private async Task<string> GenerateDistributionCode(string db, int Company_Id)
        {
            string code;
            DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastDistribution_no(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            if (dataTable.Rows.Count > 0)
            {
                var prev = dataTable.Rows[0]["DISTRIBUTION_NO"].ToString().Substring(4);
                code = DateTime.Now.ToString("yyMM")
                    + (Convert.ToInt32(prev) + 1)
                       .ToString().PadLeft(CodeConstants.Requisition_No_CodeLength, '0');
            }
            else
            {
                code = DateTime.Now.ToString("yyMM") + 1.ToString().PadLeft(CodeConstants.Requisition_No_CodeLength, '0');
            }
            return code;
        }
        private async Task<string> Check_Validity_Distribution(string db, Depot_Customer_Dist_Mst model)
        {
            string msg = "";
            bool flag = true;
            if (model.CONFIRMED == 1)
            {

                for (int i = 0; i < model.Invoices.Count; i++)
                {
                    DataTable dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), @"Select TO_CHAR(ENTERED_DATE, 'MM/DD/YYYY HH:mm:ss AM') ENTERED_DATE from DEPOT_CUSTOMER_DIST_INVOICE  I
WHERE INVOICE_NO = :param1 AND I.MST_ID != :param2 AND ROWNUM = 1 ORDER BY DEPOT_INV_ID DESC", _commonServices.AddParameter(new string[] { model.Invoices[i].INVOICE_NO, model.MST_ID.ToString() }));
                    string interted_date = "";
                    if (dt.Rows.Count > 0)
                    {
                        interted_date = dt.Rows[0]["ENTERED_DATE"].ToString();
                    }
                    if (interted_date != "")
                    {
                        DateTime DT = DateTime.Parse(interted_date);
                        DateTime enter_date = DateTime.Now;
                        if (DT.Date == enter_date.Date)
                        {
                            if ((enter_date - DT).TotalMinutes < 15)
                            {
                                string query = string.Format(@"
SELECT DISTINCT M.INVOICE_NO,
         D.SKU_CODE,
         SUM (NVL (P.INVOICE_QTY, 0)) DISTRIBUTION_QTY,
         D.INVOICE_QTY INVOICE_QTY,
         D.INVOICE_QTY - SUM (NVL (P.INVOICE_QTY, 0))
            DISTRIBUTIONABLE_QTY
    FROM INVOICE_MST M
         LEFT OUTER JOIN INVOICE_DTL D ON D.MST_ID = M.MST_ID
         LEFT OUTER JOIN DEPOT_CUSTOMER_DIST_INVOICE I
            ON I.INVOICE_NO = M.INVOICE_NO-- AND I.MST_ID !={0}
         LEFT OUTER JOIN DEPOT_CUSTOMER_DIST_PRODUCT P
            ON P.DEPOT_INV_ID = I.DEPOT_INV_ID AND P.SKU_CODE = D.SKU_CODE
   WHERE M.INVOICE_NO = :param1 
GROUP BY M.INVOICE_NO, D.SKU_CODE, D.INVOICE_QTY", model.MST_ID);
                                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), query, _commonServices.AddParameter(new string[] { model.Invoices[i].INVOICE_NO }));
                                if (dataTable.Rows.Count > 0)
                                {
                                    for (int j = 0; j < dataTable.Rows.Count; j++)
                                    {
                                        Depot_Customer_Dist_Product product = model.Invoices[i].Products.Where(x => x.SKU_CODE == dataTable.Rows[j]["SKU_CODE"].ToString()).FirstOrDefault();
                                        if (product == null)
                                        {
                                            msg = "1";

                                            flag = true; break;
                                        }
                                        else
                                        {
                                            if (product.DISTRIBUTION_QTY == Convert.ToInt32(dataTable.Rows[j]["DISTRIBUTION_QTY"]) || product.DISTRIBUTION_QTY > Convert.ToInt32(dataTable.Rows[j]["DISTRIBUTIONABLE_QTY"]))
                                            {
                                                msg = "Copy Distribution Detected!!!!Distribution Already Exist!";

                                                flag = false; break;
                                            }
                                            else
                                            {
                                                msg = "1";

                                                flag = true; break;
                                            }
                                        }
                                    }


                                }
                                else
                                {
                                    msg = "1";

                                    flag = true; break;
                                }
                            }
                            else
                            {
                                msg = "1";

                                flag = true; break;
                            }
                        }
                        else
                        {
                            msg = "1";

                            flag = true; break;
                        }

                    }
                    else
                    {
                        msg = "1";
                        flag = true; break;
                    }


                }

                if (flag == false)
                {
                    return msg;
                }
                else
                {
                    return "1";
                }

            }
            else
            {


                for (int i = 0; i < model.Invoices.Count; i++)
                {
                    DataTable dt = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), "Select TO_CHAR(ENTERED_DATE, 'MM/DD/YYYY HH:mm:ss AM') ENTERED_DATE from DEPOT_CUSTOMER_DIST_INVOICE WHERE INVOICE_NO = :param1 AND ROWNUM = 1 ORDER BY DEPOT_INV_ID DESC", _commonServices.AddParameter(new string[] { model.Invoices[i].INVOICE_NO }));
                    string interted_date = "";
                    if (dt.Rows.Count > 0)
                    {
                        interted_date = dt.Rows[0]["ENTERED_DATE"].ToString();
                    }
                    if (interted_date != "")
                    {
                        DateTime DT = DateTime.Parse(interted_date);
                        DateTime enter_date = DateTime.Now;
                        if (DT.Date == enter_date.Date)
                        {
                            if ((enter_date - DT).TotalMinutes < 15)
                            {
                                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Distable_Qty_By_InvoiceNo_Query(), _commonServices.AddParameter(new string[] { model.Invoices[i].INVOICE_NO }));
                                if (dataTable.Rows.Count > 0)
                                {
                                    for (int j = 0; j < dataTable.Rows.Count; j++)
                                    {
                                        Depot_Customer_Dist_Product product = model.Invoices[i].Products.Where(x => x.SKU_CODE == dataTable.Rows[j]["SKU_CODE"].ToString()).FirstOrDefault();
                                        if (product == null)
                                        {
                                            msg = "1";

                                            flag = true; break;
                                        }
                                        else
                                        {
                                            if (product.DISTRIBUTION_QTY == Convert.ToInt32(dataTable.Rows[j]["DISTRIBUTION_QTY"]) || product.DISTRIBUTION_QTY > Convert.ToInt32(dataTable.Rows[j]["DISTRIBUTIONABLE_QTY"]))
                                            {
                                                msg = "Copy Distribution Detected!!!!Distribution Already Exist!";

                                                flag = false; break;
                                            }
                                            else
                                            {
                                                msg = "1";

                                                flag = true; break;
                                            }
                                        }
                                    }


                                }
                                else
                                {
                                }
                            }
                            else
                            {
                                msg = "1";

                                flag = true; break;
                            }
                        }
                        else
                        {
                            msg = "1";

                            flag = true; break;
                        }

                    }
                    else
                    {
                        msg = "1";
                        flag = true; break;
                    }


                }

                if (flag == false)
                {
                    return msg;
                }
                else
                {
                    return "1";
                }

            }

        }




        private async Task<(bool flag, string message)> CheckDelivaryQty(string db, Depot_Customer_Dist_Mst model)
        {
            try
            {
                string invoiceNos = string.Join(",", model.Invoices.Select(i => $"'{i.INVOICE_NO}'"));
                string sku_pending;
                if(model.MST_ID== 0)
                {
                    sku_pending = @$"SELECT A.COMPANY_ID,
                               A.UNIT_ID,
                               A.CUSTOMER_ID,
                               A.CUSTOMER_CODE,
                               A.MARKET_CODE,
                               A.INVOICE_NO,
                               A.INVOICE_DATE,
                               A.SKU_ID,
                               A.SKU_CODE,
                               B.SKU_NAME,
                               B.PACK_SIZE,
                               B.SHIPPER_QTY,
                               A.INVOICE_QTY,
                               A.BONUS_QTY,
                               A.PENDING_INVOICE_DIST_QTY,
                               A.PENDING_BONUS_DIST_QTY,
                               NVL(A.PENDING_INVOICE_DIST_QTY,0)+NVL(A.PENDING_BONUS_DIST_QTY,0) TOTAL_PENDING_QTY,
                               B.SHIPPER_WEIGHT AS PER_SHIPPER_WEIGHT,
                               B.SHIPPER_WEIGHT_UNIT,
                               B.SHIPPER_VOLUME AS PER_SHIPPER_VOLUME,
                               B.SHIPPER_VOLUME_UNIT,
                               B.PACK_VALUE,
                               B.PACK_UNIT,
                               B.WEIGHT_PER_PACK,
                               B.WEIGHT_UNIT
                        FROM
                           (
                                SELECT COMPANY_ID,
                                       INVOICE_UNIT_ID UNIT_ID,
                                       CUSTOMER_ID,
                                       CUSTOMER_CODE,
                                       MARKET_CODE,
                                       INVOICE_NO,
                                       INVOICE_DATE,
                                       SKU_ID,
                                       SKU_CODE,
                                       SUM(NVL(PENDING_INVOICE_DIST_QTY,0)) INVOICE_QTY,
                                       SUM(NVL(PENDING_BONUS_DIST_QTY,0)) BONUS_QTY,
                                       SUM(NVL(PENDING_INVOICE_DIST_QTY,0)) PENDING_INVOICE_DIST_QTY,
                                       SUM(NVL(PENDING_BONUS_DIST_QTY,0))PENDING_BONUS_DIST_QTY
                                FROM
                                   (
                                    SELECT   A.COMPANY_ID,
                                             A.INVOICE_UNIT_ID,
                                             D.CUSTOMER_ID,
                                             D.CUSTOMER_CODE,
                                             D.MARKET_CODE,
                                             A.INVOICE_NO,
                                             D.INVOICE_DATE,
                                             A.SKU_ID,
                                             A.SKU_CODE,
                                             A.BATCH_ID,
                                             A.BATCH_NO,
                                             A.INVOICE_QTY,
                                             B.RETURN_INVOICE_QTY,
                                             NVL(A.INVOICE_QTY,0)-NVL(B.RETURN_INVOICE_QTY,0) NET_INVOICE_QTY,
                                             NVL(C.DISTRIBUTION_INVOICE_QTY,0)DISTRIBUTION_INVOICE_QTY,
                                             NVL(A.INVOICE_QTY,0)-NVL(B.RETURN_INVOICE_QTY,0)-NVL(C.DISTRIBUTION_INVOICE_QTY,0) PENDING_INVOICE_DIST_QTY,
                                             NVL(A.BONUS_QTY,0) BONUS_QTY,
                                             NVL(B.RETURN_BONUS_QTY,0) RETURN_BONUS_QTY,
                                             NVL(A.BONUS_QTY,0)-NVL(B.RETURN_BONUS_QTY,0) NET_BONUS_QTY,
                                             NVL(C.DISTRIBUTION_BONUS_QTY,0)DISTRIBUTION_BONUS_QTY,
                                             NVL(A.BONUS_QTY,0)-NVL(B.RETURN_BONUS_QTY,0)-NVL(C.DISTRIBUTION_BONUS_QTY,0) PENDING_BONUS_DIST_QTY

                                    FROM
                                         (

                                              SELECT COMPANY_ID,
                                                     INVOICE_UNIT_ID,
                                                     INVOICE_NO,
                                                     SKU_ID,
                                                     SKU_CODE,
                                                     BATCH_ID,
                                                     BATCH_NO,
                                                     SUM(NVL(INVOICE_QTY,0))INVOICE_QTY,
                                                     SUM(NVL(BONUS_QTY,0)) BONUS_QTY
                                                FROM
                                                     (
                                                      SELECT A.COMPANY_ID,
                                                             A.INVOICE_UNIT_ID,
                                                             A.INVOICE_NO,
                                                             C.SKU_ID,
                                                             C.SKU_CODE,
                                                             C.BATCH_ID,
                                                             C.BATCH_NO,
                                                             C.ISSUE_QTY INVOICE_QTY,
                                                             0 BONUS_QTY
                                                     FROM INVOICE_MST A, INVOICE_DTL B,INVOICE_ISSUE C
                                                     WHERE A.MST_ID = B.MST_ID
                                                     AND   B.DTL_ID=C.DTL_ID
                                                     AND   A.COMPANY_ID=:param1
                                                     AND   A.INVOICE_UNIT_ID=:param2
                                                     AND   A.INVOICE_NO IN ( {invoiceNos} )

                                                     UNION ALL

                                                     SELECT  A.COMPANY_ID,
                                                             A.INVOICE_UNIT_ID,
                                                             A.INVOICE_NO,
                                                             C.SKU_ID,
                                                             C.SKU_CODE,
                                                             C.BATCH_ID,
                                                             C.BATCH_NO,
                                                             0 INVOICE_QTY,
                                                             C.BONUS_QTY
                                                     FROM INVOICE_MST A, INVOICE_DTL B, INVOICE_BONUS C
                                                     WHERE A.MST_ID = B.MST_ID
                                                     AND   B.DTL_ID = C.DTL_ID
                                                     AND   A.COMPANY_ID=:param1
                                                     AND   A.INVOICE_UNIT_ID=:param2
                                                     AND   A.INVOICE_NO IN ( {invoiceNos} )



                                                     UNION ALL

                                                     SELECT  A.COMPANY_ID,
                                                             A.INVOICE_UNIT_ID,
                                                             A.INVOICE_NO,
                                                             B.SKU_ID,
                                                             B.SKU_CODE,
                                                             B.BATCH_ID,
                                                             B.BATCH_NO,
                                                             0 INVOICE_QTY,
                                                             B.BONUS_QTY
                                                     FROM INVOICE_MST A,INVOICE_COMBO_BONUS B
                                                     WHERE A.MST_ID = B.MST_ID
                                                     AND   A.COMPANY_ID=:param1
                                                     AND   A.INVOICE_UNIT_ID=:param2
                                                     AND   A.INVOICE_NO IN ( {invoiceNos} )



                                                      )
                                              GROUP BY COMPANY_ID,INVOICE_UNIT_ID,INVOICE_NO,SKU_ID,SKU_CODE,BATCH_ID,BATCH_NO
                                         )A,
                                         (


                                              SELECT COMPANY_ID,
                                                     RETURN_UNIT_ID,
                                                     INVOICE_NO,
                                                     SKU_ID,
                                                     SKU_CODE,
                                                     BATCH_ID,
                                                     BATCH_NO,
                                                     SUM(NVL(RETURN_INVOICE_QTY,0))RETURN_INVOICE_QTY,
                                                     SUM(NVL(RETURN_BONUS_QTY,0)) RETURN_BONUS_QTY
                                                FROM
                                                     (
                                                     SELECT  A.COMPANY_ID,
                                                             A.RETURN_UNIT_ID,
                                                             A.INVOICE_NO,
                                                             C.SKU_ID,
                                                             C.SKU_CODE,
                                                             C.BATCH_ID,
                                                             C.BATCH_NO,
                                                             C.RETURN_QTY RETURN_INVOICE_QTY,
                                                             0 RETURN_BONUS_QTY
                                                     FROM RETURN_MST A, RETURN_DTL B,RETURN_ISSUE C
                                                     WHERE A.MST_ID = B.MST_ID
                                                     AND   B.DTL_ID=C.DTL_ID
                                                     AND   A.COMPANY_ID=:param1
                                                     AND   A.RETURN_UNIT_ID=:param2
                                                     AND   A.INVOICE_NO IN ( {invoiceNos} )

                                                     UNION ALL

                                                     SELECT  A.COMPANY_ID,
                                                             A.RETURN_UNIT_ID,
                                                             A.INVOICE_NO,
                                                             C.SKU_ID,
                                                             C.SKU_CODE,
                                                             C.BATCH_ID,
                                                             C.BATCH_NO,
                                                             0 INVOICE_QTY,
                                                             C.RETURN_BONUS_QTY
                                                     FROM RETURN_MST A, RETURN_DTL B,RETURN_BONUS C
                                                     WHERE A.MST_ID = B.MST_ID
                                                     AND   B.DTL_ID = C.DTL_ID
                                                     AND   A.COMPANY_ID=:param1
                                                     AND   A.RETURN_UNIT_ID=:param2
                                                     AND   A.INVOICE_NO IN ( {invoiceNos} )

                                                     UNION ALL

                                                     SELECT  A.COMPANY_ID,
                                                             A.RETURN_UNIT_ID,
                                                             A.INVOICE_NO,
                                                             B.SKU_ID,
                                                             B.SKU_CODE,
                                                             B.BATCH_ID,
                                                             B.BATCH_NO,
                                                             0 INVOICE_QTY,
                                                             B.RETURN_BONUS_QTY
                                                     FROM RETURN_MST A, RETURN_COMBO_BONUS B
                                                     WHERE A.MST_ID = B.MST_ID
                                                     AND   A.COMPANY_ID=:param1
                                                     AND   A.RETURN_UNIT_ID=:param2
                                                     AND   A.INVOICE_NO IN ( {invoiceNos} )

                                                      )
                                              GROUP BY COMPANY_ID,RETURN_UNIT_ID,INVOICE_NO,SKU_ID,SKU_CODE,BATCH_ID,BATCH_NO


                                         ) B,
                                         (
                                            SELECT COMPANY_ID,
                                                   INVOICE_UNIT_ID,
                                                   INVOICE_NO,
                                                   SKU_ID,
                                                   SKU_CODE,
                                                   BATCH_ID,
                                                   BATCH_NO,
                                                   SUM(NVL(DISTRIBUTION_INVOICE_QTY,0))DISTRIBUTION_INVOICE_QTY,
                                                   SUM(NVL(DISTRIBUTION_BONUS_QTY,0))DISTRIBUTION_BONUS_QTY
                                            FROM
                                                (
                                                SELECT A.COMPANY_ID,
                                                       A.INVOICE_UNIT_ID,
                                                       B.INVOICE_NO,
                                                       D.SKU_ID,
                                                       D.SKU_CODE,
                                                       D.BATCH_ID,
                                                       D.BATCH_NO,
                                                       D.DISTRIBUTION_INVOICE_QTY,
                                                       0 DISTRIBUTION_BONUS_QTY
                                                FROM DEPOT_CUSTOMER_DIST_MST A,
                                                     DEPOT_CUSTOMER_DIST_INVOICE B,
                                                     DEPOT_CUSTOMER_DIST_PRODUCT C,
                                                     DEPOT_CUSTOMER_DIST_PROD_BATCH D
                                                WHERE A.MST_ID=B.MST_ID
                                                AND   B.DEPOT_INV_ID=C.DEPOT_INV_ID
                                                AND   C.DEPOT_PRODUCT_ID=D.DEPOT_PRODUCT_ID
                                                AND   NVL(D.DISTRIBUTION_INVOICE_QTY,0)>0
                                                AND   A.COMPANY_ID=:param1
                                                AND   A.INVOICE_UNIT_ID=:param2
                                                AND   B.INVOICE_NO IN ( {invoiceNos} )


                                                UNION ALL

                                                SELECT A.COMPANY_ID,
                                                       A.INVOICE_UNIT_ID,
                                                       B.INVOICE_NO,
                                                       D.SKU_ID,
                                                       D.SKU_CODE,
                                                       D.BATCH_ID,
                                                       D.BATCH_NO,
                                                       0 DISTRIBUTION_INVOICE_QTY,
                                                       D.DISTRIBUTION_BONUS_QTY
                                                FROM DEPOT_CUSTOMER_DIST_MST A,
                                                     DEPOT_CUSTOMER_DIST_INVOICE B,
                                                     DEPOT_CUSTOMER_DIST_PRODUCT C,
                                                     DEPOT_CUSTOMER_DIST_PROD_BATCH D
                                                WHERE A.MST_ID=B.MST_ID
                                                AND   B.DEPOT_INV_ID=C.DEPOT_INV_ID
                                                AND   C.DEPOT_PRODUCT_ID=D.DEPOT_PRODUCT_ID
                                                AND   NVL(D.DISTRIBUTION_BONUS_QTY,0)>0
                                                AND   A.COMPANY_ID=:param1
                                                AND   A.INVOICE_UNIT_ID=:param2
                                                AND   B.INVOICE_NO IN ( {invoiceNos} )
                                               )
                                            GROUP BY COMPANY_ID,INVOICE_UNIT_ID,INVOICE_NO,SKU_ID,SKU_CODE,BATCH_ID,BATCH_NO
                                         )C,INVOICE_MST D
                                    WHERE A.COMPANY_ID=B.COMPANY_ID(+)
                                    AND   A.INVOICE_UNIT_ID=B.RETURN_UNIT_ID(+)
                                    AND   A.INVOICE_NO=B.INVOICE_NO(+)
                                    AND   A.SKU_ID=B.SKU_ID(+)
                                    AND   A.BATCH_ID=B.BATCH_ID(+)

                                    AND   A.COMPANY_ID=C.COMPANY_ID(+)
                                    AND   A.INVOICE_UNIT_ID=C.INVOICE_UNIT_ID(+)
                                    AND   A.INVOICE_NO=C.INVOICE_NO(+)
                                    AND   A.SKU_ID=C.SKU_ID(+)
                                    AND   A.BATCH_ID=C.BATCH_ID(+)
                                    AND   A.INVOICE_NO=D.INVOICE_NO
                                   )
                                   GROUP BY COMPANY_ID,INVOICE_UNIT_ID,CUSTOMER_ID,CUSTOMER_CODE,MARKET_CODE,INVOICE_NO,INVOICE_DATE,SKU_ID,SKU_CODE
                            )A,PRODUCT_INFO B
                        WHERE A.COMPANY_ID=B.COMPANY_ID
                        AND   A.SKU_ID=B.SKU_ID
                        AND   NVL(A.PENDING_INVOICE_DIST_QTY,0)+NVL(A.PENDING_BONUS_DIST_QTY,0)>0";

                }
                else
                {
                    sku_pending = @$"SELECT A.COMPANY_ID,
                               A.UNIT_ID,
                               A.CUSTOMER_ID,
                               A.CUSTOMER_CODE,
                               A.MARKET_CODE,
                               A.INVOICE_NO,
                               A.INVOICE_DATE,
                               A.SKU_ID,
                               A.SKU_CODE,
                               B.SKU_NAME,
                               B.PACK_SIZE,
                               B.SHIPPER_QTY,
                               A.INVOICE_QTY,
                               A.BONUS_QTY,
                               A.PENDING_INVOICE_DIST_QTY,
                               A.PENDING_BONUS_DIST_QTY,
                               NVL(A.PENDING_INVOICE_DIST_QTY,0)+NVL(A.PENDING_BONUS_DIST_QTY,0) TOTAL_PENDING_QTY,
                               B.SHIPPER_WEIGHT AS PER_SHIPPER_WEIGHT,
                               B.SHIPPER_WEIGHT_UNIT,
                               B.SHIPPER_VOLUME AS PER_SHIPPER_VOLUME,
                               B.SHIPPER_VOLUME_UNIT,
                               B.PACK_VALUE,
                               B.PACK_UNIT,
                               B.WEIGHT_PER_PACK,
                               B.WEIGHT_UNIT
                        FROM
                           (
                                SELECT COMPANY_ID,
                                       INVOICE_UNIT_ID UNIT_ID,
                                       CUSTOMER_ID,
                                       CUSTOMER_CODE,
                                       MARKET_CODE,
                                       INVOICE_NO,
                                       INVOICE_DATE,
                                       SKU_ID,
                                       SKU_CODE,
                                       SUM(NVL(PENDING_INVOICE_DIST_QTY,0)) INVOICE_QTY,
                                       SUM(NVL(PENDING_BONUS_DIST_QTY,0)) BONUS_QTY,
                                       SUM(NVL(PENDING_INVOICE_DIST_QTY,0)) PENDING_INVOICE_DIST_QTY,
                                       SUM(NVL(PENDING_BONUS_DIST_QTY,0))PENDING_BONUS_DIST_QTY
                                FROM
                                   (
                                    SELECT   A.COMPANY_ID,
                                             A.INVOICE_UNIT_ID,
                                             D.CUSTOMER_ID,
                                             D.CUSTOMER_CODE,
                                             D.MARKET_CODE,
                                             A.INVOICE_NO,
                                             D.INVOICE_DATE,
                                             A.SKU_ID,
                                             A.SKU_CODE,
                                             A.BATCH_ID,
                                             A.BATCH_NO,
                                             A.INVOICE_QTY,
                                             B.RETURN_INVOICE_QTY,
                                             NVL(A.INVOICE_QTY,0)-NVL(B.RETURN_INVOICE_QTY,0) NET_INVOICE_QTY,
                                             NVL(C.DISTRIBUTION_INVOICE_QTY,0)DISTRIBUTION_INVOICE_QTY,
                                             NVL(A.INVOICE_QTY,0)-NVL(B.RETURN_INVOICE_QTY,0)-NVL(C.DISTRIBUTION_INVOICE_QTY,0) PENDING_INVOICE_DIST_QTY,
                                             NVL(A.BONUS_QTY,0) BONUS_QTY,
                                             NVL(B.RETURN_BONUS_QTY,0) RETURN_BONUS_QTY,
                                             NVL(A.BONUS_QTY,0)-NVL(B.RETURN_BONUS_QTY,0) NET_BONUS_QTY,
                                             NVL(C.DISTRIBUTION_BONUS_QTY,0)DISTRIBUTION_BONUS_QTY,
                                             NVL(A.BONUS_QTY,0)-NVL(B.RETURN_BONUS_QTY,0)-NVL(C.DISTRIBUTION_BONUS_QTY,0) PENDING_BONUS_DIST_QTY

                                    FROM
                                         (

                                              SELECT COMPANY_ID,
                                                     INVOICE_UNIT_ID,
                                                     INVOICE_NO,
                                                     SKU_ID,
                                                     SKU_CODE,
                                                     BATCH_ID,
                                                     BATCH_NO,
                                                     SUM(NVL(INVOICE_QTY,0))INVOICE_QTY,
                                                     SUM(NVL(BONUS_QTY,0)) BONUS_QTY
                                                FROM
                                                     (
                                                      SELECT A.COMPANY_ID,
                                                             A.INVOICE_UNIT_ID,
                                                             A.INVOICE_NO,
                                                             C.SKU_ID,
                                                             C.SKU_CODE,
                                                             C.BATCH_ID,
                                                             C.BATCH_NO,
                                                             C.ISSUE_QTY INVOICE_QTY,
                                                             0 BONUS_QTY
                                                     FROM INVOICE_MST A, INVOICE_DTL B,INVOICE_ISSUE C
                                                     WHERE A.MST_ID = B.MST_ID
                                                     AND   B.DTL_ID=C.DTL_ID
                                                     AND   A.COMPANY_ID=:param1
                                                     AND   A.INVOICE_UNIT_ID=:param2
                                                     AND   A.INVOICE_NO IN ( {invoiceNos} )

                                                     UNION ALL

                                                     SELECT  A.COMPANY_ID,
                                                             A.INVOICE_UNIT_ID,
                                                             A.INVOICE_NO,
                                                             C.SKU_ID,
                                                             C.SKU_CODE,
                                                             C.BATCH_ID,
                                                             C.BATCH_NO,
                                                             0 INVOICE_QTY,
                                                             C.BONUS_QTY
                                                     FROM INVOICE_MST A, INVOICE_DTL B, INVOICE_BONUS C
                                                     WHERE A.MST_ID = B.MST_ID
                                                     AND   B.DTL_ID = C.DTL_ID
                                                     AND   A.COMPANY_ID=:param1
                                                     AND   A.INVOICE_UNIT_ID=:param2
                                                     AND   A.INVOICE_NO IN ( {invoiceNos} )



                                                     UNION ALL

                                                     SELECT  A.COMPANY_ID,
                                                             A.INVOICE_UNIT_ID,
                                                             A.INVOICE_NO,
                                                             B.SKU_ID,
                                                             B.SKU_CODE,
                                                             B.BATCH_ID,
                                                             B.BATCH_NO,
                                                             0 INVOICE_QTY,
                                                             B.BONUS_QTY
                                                     FROM INVOICE_MST A,INVOICE_COMBO_BONUS B
                                                     WHERE A.MST_ID = B.MST_ID
                                                     AND   A.COMPANY_ID=:param1
                                                     AND   A.INVOICE_UNIT_ID=:param2
                                                     AND   A.INVOICE_NO IN ( {invoiceNos} )



                                                      )
                                              GROUP BY COMPANY_ID,INVOICE_UNIT_ID,INVOICE_NO,SKU_ID,SKU_CODE,BATCH_ID,BATCH_NO
                                         )A,
                                         (


                                              SELECT COMPANY_ID,
                                                     RETURN_UNIT_ID,
                                                     INVOICE_NO,
                                                     SKU_ID,
                                                     SKU_CODE,
                                                     BATCH_ID,
                                                     BATCH_NO,
                                                     SUM(NVL(RETURN_INVOICE_QTY,0))RETURN_INVOICE_QTY,
                                                     SUM(NVL(RETURN_BONUS_QTY,0)) RETURN_BONUS_QTY
                                                FROM
                                                     (
                                                     SELECT  A.COMPANY_ID,
                                                             A.RETURN_UNIT_ID,
                                                             A.INVOICE_NO,
                                                             C.SKU_ID,
                                                             C.SKU_CODE,
                                                             C.BATCH_ID,
                                                             C.BATCH_NO,
                                                             C.RETURN_QTY RETURN_INVOICE_QTY,
                                                             0 RETURN_BONUS_QTY
                                                     FROM RETURN_MST A, RETURN_DTL B,RETURN_ISSUE C
                                                     WHERE A.MST_ID = B.MST_ID
                                                     AND   B.DTL_ID=C.DTL_ID
                                                     AND   A.COMPANY_ID=:param1
                                                     AND   A.RETURN_UNIT_ID=:param2
                                                     AND   A.INVOICE_NO IN ( {invoiceNos} )

                                                     UNION ALL

                                                     SELECT  A.COMPANY_ID,
                                                             A.RETURN_UNIT_ID,
                                                             A.INVOICE_NO,
                                                             C.SKU_ID,
                                                             C.SKU_CODE,
                                                             C.BATCH_ID,
                                                             C.BATCH_NO,
                                                             0 INVOICE_QTY,
                                                             C.RETURN_BONUS_QTY
                                                     FROM RETURN_MST A, RETURN_DTL B,RETURN_BONUS C
                                                     WHERE A.MST_ID = B.MST_ID
                                                     AND   B.DTL_ID = C.DTL_ID
                                                     AND   A.COMPANY_ID=:param1
                                                     AND   A.RETURN_UNIT_ID=:param2
                                                     AND   A.INVOICE_NO IN ( {invoiceNos} )

                                                     UNION ALL

                                                     SELECT  A.COMPANY_ID,
                                                             A.RETURN_UNIT_ID,
                                                             A.INVOICE_NO,
                                                             B.SKU_ID,
                                                             B.SKU_CODE,
                                                             B.BATCH_ID,
                                                             B.BATCH_NO,
                                                             0 INVOICE_QTY,
                                                             B.RETURN_BONUS_QTY
                                                     FROM RETURN_MST A, RETURN_COMBO_BONUS B
                                                     WHERE A.MST_ID = B.MST_ID
                                                     AND   A.COMPANY_ID=:param1
                                                     AND   A.RETURN_UNIT_ID=:param2
                                                     AND   A.INVOICE_NO IN ( {invoiceNos} )

                                                      )
                                              GROUP BY COMPANY_ID,RETURN_UNIT_ID,INVOICE_NO,SKU_ID,SKU_CODE,BATCH_ID,BATCH_NO


                                         ) B,
                                         (
                                            SELECT COMPANY_ID,
                                                   INVOICE_UNIT_ID,
                                                   INVOICE_NO,
                                                   SKU_ID,
                                                   SKU_CODE,
                                                   BATCH_ID,
                                                   BATCH_NO,
                                                   SUM(NVL(DISTRIBUTION_INVOICE_QTY,0))DISTRIBUTION_INVOICE_QTY,
                                                   SUM(NVL(DISTRIBUTION_BONUS_QTY,0))DISTRIBUTION_BONUS_QTY
                                            FROM
                                                (
                                                SELECT A.COMPANY_ID,
                                                       A.INVOICE_UNIT_ID,
                                                       B.INVOICE_NO,
                                                       D.SKU_ID,
                                                       D.SKU_CODE,
                                                       D.BATCH_ID,
                                                       D.BATCH_NO,
                                                       D.DISTRIBUTION_INVOICE_QTY,
                                                       0 DISTRIBUTION_BONUS_QTY
                                                FROM DEPOT_CUSTOMER_DIST_MST A,
                                                     DEPOT_CUSTOMER_DIST_INVOICE B,
                                                     DEPOT_CUSTOMER_DIST_PRODUCT C,
                                                     DEPOT_CUSTOMER_DIST_PROD_BATCH D
                                                WHERE A.MST_ID=B.MST_ID
                                                AND   B.DEPOT_INV_ID=C.DEPOT_INV_ID
                                                AND   C.DEPOT_PRODUCT_ID=D.DEPOT_PRODUCT_ID
                                                AND   NVL(D.DISTRIBUTION_INVOICE_QTY,0)>0
                                                AND   A.COMPANY_ID=:param1
                                                AND   A.INVOICE_UNIT_ID=:param2
                                                AND   B.INVOICE_NO IN ( {invoiceNos} )
                                                AND   A.MST_ID != {model.MST_ID}


                                                UNION ALL

                                                SELECT A.COMPANY_ID,
                                                       A.INVOICE_UNIT_ID,
                                                       B.INVOICE_NO,
                                                       D.SKU_ID,
                                                       D.SKU_CODE,
                                                       D.BATCH_ID,
                                                       D.BATCH_NO,
                                                       0 DISTRIBUTION_INVOICE_QTY,
                                                       D.DISTRIBUTION_BONUS_QTY
                                                FROM DEPOT_CUSTOMER_DIST_MST A,
                                                     DEPOT_CUSTOMER_DIST_INVOICE B,
                                                     DEPOT_CUSTOMER_DIST_PRODUCT C,
                                                     DEPOT_CUSTOMER_DIST_PROD_BATCH D
                                                WHERE A.MST_ID=B.MST_ID
                                                AND   B.DEPOT_INV_ID=C.DEPOT_INV_ID
                                                AND   C.DEPOT_PRODUCT_ID=D.DEPOT_PRODUCT_ID
                                                AND   NVL(D.DISTRIBUTION_BONUS_QTY,0)>0
                                                AND   A.COMPANY_ID=:param1
                                                AND   A.INVOICE_UNIT_ID=:param2
                                                AND   B.INVOICE_NO IN ( {invoiceNos} )
                                                AND   A.MST_ID != {model.MST_ID}
                                               )
                                            GROUP BY COMPANY_ID,INVOICE_UNIT_ID,INVOICE_NO,SKU_ID,SKU_CODE,BATCH_ID,BATCH_NO
                                         )C,INVOICE_MST D
                                    WHERE A.COMPANY_ID=B.COMPANY_ID(+)
                                    AND   A.INVOICE_UNIT_ID=B.RETURN_UNIT_ID(+)
                                    AND   A.INVOICE_NO=B.INVOICE_NO(+)
                                    AND   A.SKU_ID=B.SKU_ID(+)
                                    AND   A.BATCH_ID=B.BATCH_ID(+)

                                    AND   A.COMPANY_ID=C.COMPANY_ID(+)
                                    AND   A.INVOICE_UNIT_ID=C.INVOICE_UNIT_ID(+)
                                    AND   A.INVOICE_NO=C.INVOICE_NO(+)
                                    AND   A.SKU_ID=C.SKU_ID(+)
                                    AND   A.BATCH_ID=C.BATCH_ID(+)
                                    AND   A.INVOICE_NO=D.INVOICE_NO
                                   )
                                   GROUP BY COMPANY_ID,INVOICE_UNIT_ID,CUSTOMER_ID,CUSTOMER_CODE,MARKET_CODE,INVOICE_NO,INVOICE_DATE,SKU_ID,SKU_CODE
                            )A,PRODUCT_INFO B
                        WHERE A.COMPANY_ID=B.COMPANY_ID
                        AND   A.SKU_ID=B.SKU_ID
                        AND   NVL(A.PENDING_INVOICE_DIST_QTY,0)+NVL(A.PENDING_BONUS_DIST_QTY,0)>0";

                }
                var parameters = _commonServices.AddParameter(new string[] { model.COMPANY_ID.ToString(), model.INVOICE_UNIT_ID.ToString() });
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), sku_pending, parameters);
                foreach (var invoice in model.Invoices)
                {
                    foreach (var product in invoice.Products)
                    {
                        var matchingRows = dataTable.AsEnumerable().Where(row =>
                        row.Field<string>("INVOICE_NO") == Convert.ToString(invoice.INVOICE_NO) &&
                        row.Field<decimal>("SKU_ID") == Convert.ToDecimal(product.SKU_ID)).ToList();

                        if (matchingRows.Count > 1)
                        {
                            return (false, $"Multiple matching rows found for the product {product.SKU_NAME} of invoice {invoice.INVOICE_NO}");
                        }

                        if (matchingRows.Count == 1)
                        {
                            DataRow row = matchingRows.First();
                            decimal invoiceQty = row.Field<decimal>("INVOICE_QTY");
                            decimal BONUS_QTY = row.Field<decimal>("BONUS_QTY");

                            if (invoiceQty < product.DISTRIBUTION_QTY)
                            {
                                return (false, $"Qty: {product.DISTRIBUTION_QTY} is greater than Pending Qty: {invoiceQty} for the Product {product.SKU_NAME} of invoice {invoice.INVOICE_NO}");
                            }

                            if (BONUS_QTY < product.DISTRIBUTION_BONUS_QTY)
                            {
                                return (false, $"Bonus Qty: {product.DISTRIBUTION_BONUS_QTY} is greater than Pending Qty: {BONUS_QTY} for the Product {product.SKU_NAME} of invoice {invoice.INVOICE_NO}");
                            }
                        }
                        else
                        {
                            return (false, $"No pending quantity found for the product {product.SKU_NAME} of invoice {invoice.INVOICE_NO}");
                        }
                    }
                }
                return (true, "Everything is good!");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

    }
}
