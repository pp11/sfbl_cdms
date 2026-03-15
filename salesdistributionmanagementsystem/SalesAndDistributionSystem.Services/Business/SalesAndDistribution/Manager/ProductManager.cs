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

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public class ProductManager : IProductManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public ProductManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------
        string FilterQry= "";
        string LoadData_Query() => @"Select
                               ROW_NUMBER() OVER(ORDER BY SKU_ID ASC) AS ROW_NO
                              ,BASE_PRODUCT_ID
                              ,BRAND_ID
                              ,CATEGORY_ID
                              ,COMPANY_ID
                              ,FONT_COLOR
                              ,GROUP_ID
                              ,PACK_SIZE
                              ,PACK_UNIT
                              ,PACK_VALUE
                              ,PRIMARY_PRODUCT_ID
                              ,PRODUCT_SEASON_ID
                              ,PRODUCT_STATUS
                              ,PRODUCT_TYPE_ID
                              ,QTY_PER_PACK
                              ,REMARKS
                              ,SHIPPER_QTY
                              ,SHIPPER_VOLUME
                              ,SHIPPER_VOLUME_UNIT
                              ,SHIPPER_WEIGHT
                              ,SHIPPER_WEIGHT_UNIT
                              ,SKU_CODE
                              ,SKU_ID
                              ,SKU_NAME
                              ,SKU_NAME_BANGLA
                              ,STORAGE_ID
                              ,UNIT_ID
                              ,WEIGHT_PER_PACK
                              ,WEIGHT_UNIT
                              ,DISTRIBUTOR_PRODUCT_TYPE,PRODUCT_SEGMENT_NAME
                               from Product_Info Where COMPANY_ID = :param1";

        string LoadSkuCodeData_Query() => @"select SKU_CODE, SKU_NAME, PACK_SIZE, SHIPPER_QTY, PRODUCT_STATUS from VW_ALL_PRODUCT_INFO WHERE SKU_CODE  not in ( select sku_code from product_INFO)";
        string LoadPackSizeData_Query() => @"      
                                     Select DISTINCT PACK_SIZE from 
                                      (SELECT PACK_SIZE_NAME PACK_SIZE from PACK_SIZE_INFO 
                                       UNION ALL
                                       SELECT p.PACK_SIZE FROM PRODUCT_INFO p
                                       UNION ALL
                                       SELECT  f.SKU_NAME_BANGLA PACK_SIZE from PRODUCT_INFO f
                                       UNION ALL
                                       select  PACK_SIZE from VW_ALL_PRODUCT_INFO WHERE SKU_CODE  not in ( select sku_code from product_INFO)
                                       )";

        string LoadProductPrimaryData_Query() => @"Select
                               ROW_NUMBER() OVER(ORDER BY SKU_ID ASC) AS ROW_NO
                              ,BASE_PRODUCT_ID
                              ,BRAND_ID
                              ,CATEGORY_ID
                              ,COMPANY_ID
                              ,FONT_COLOR
                              ,GROUP_ID
                              ,PACK_SIZE
                              ,PACK_UNIT
                              ,PACK_VALUE
                              ,PRIMARY_PRODUCT_ID
                              ,PRODUCT_SEASON_ID
                              ,PRODUCT_STATUS
                              ,PRODUCT_TYPE_ID
                              ,QTY_PER_PACK
                              ,REMARKS
                              ,SHIPPER_QTY
                              ,SHIPPER_VOLUME
                              ,SHIPPER_VOLUME_UNIT
                              ,SHIPPER_WEIGHT
                              ,SHIPPER_WEIGHT_UNIT
                              ,SKU_CODE
                              ,SKU_ID
                              ,SKU_NAME
                              ,SKU_NAME_BANGLA
                              ,STORAGE_ID
                              ,UNIT_ID
                              ,WEIGHT_PER_PACK
                              ,WEIGHT_UNIT
                               from Product_Info Where PRODUCT_STATUS = 'Incomplete' and COMPANY_ID = :param1";
        string LoadProductdropdownData_Query() => @"Select
                               SKU_CODE
                              ,SKU_ID
                              ,SKU_NAME
                              ,PACK_SIZE
                               from Product_Info Where COMPANY_ID = :param1 and PRODUCT_STATUS = 'Active'";
        string LoadDataFromView_Query() => @"SELECT * FROM VW_PRODUCT_PRICE";
        string LoadDropDownDataFromView_Query() => @"SELECT M.SKU_ID, M.SKU_NAME, M.SKU_CODE, M.PACK_SIZE, M.UNIT_TP, M.MRP  FROM VW_PRODUCT_PRICE M ";
        string LoadFilteredData_Query() => string.Format(@"Select
                               ROW_NUMBER() OVER(ORDER BY SKU_ID ASC) AS ROW_NO
                              ,NVL(BASE_PRODUCT_ID,0) BASE_PRODUCT_ID
                              ,NVL(BRAND_ID,0) BRAND_ID
                              ,NVL(CATEGORY_ID,0) CATEGORY_ID
                              ,COMPANY_ID
                              ,NVL(GROUP_ID,0) GROUP_ID
                              ,PACK_SIZE
                              ,'' PRICE_FLAG
                              ,0 SKU_PRICE
                              ,'' COMMISSION_FLAG
                              ,'' COMMISSION_TYPE
                              ,0 COMMISSION_VALUE
                              ,0 ADD_COMMISSION1
                              ,0 ADD_COMMISSION2
                              ,'' REMARKS
                            
                              ,SKU_CODE
                              ,SKU_ID
                              ,SKU_NAME
                              ,'' SKU_NAME_BANGLA
                             
                               from Product_Info Where PRODUCT_STATUS = 'Active' AND COMPANY_ID = :param1 {0}", FilterQry);
        string LoadSearchableData_Query() => @"Select
                                ROW_NUMBER() OVER(ORDER BY SKU_ID ASC) AS ROW_NO
                               ,BASE_PRODUCT_ID
                               ,BRAND_ID
                               ,CATEGORY_ID
                               ,COMPANY_ID
                               ,FONT_COLOR
                               ,GROUP_ID
                               ,PACK_SIZE
                               ,PACK_UNIT
                               ,PACK_VALUE
                               ,PRIMARY_PRODUCT_ID
                               ,PRODUCT_SEASON_ID
                               ,PRODUCT_STATUS
                               ,PRODUCT_TYPE_ID
                               ,QTY_PER_PACK
                               ,REMARKS
                               ,SHIPPER_QTY
                               ,SHIPPER_VOLUME
                               ,SHIPPER_VOLUME_UNIT
                               ,SHIPPER_WEIGHT
                               ,SHIPPER_WEIGHT_UNIT
                               ,SKU_CODE
                               ,SKU_ID
                               ,SKU_NAME
                               ,SKU_NAME_BANGLA
                               ,STORAGE_ID
                               ,UNIT_ID
                               ,WEIGHT_PER_PACK
                               ,WEIGHT_UNIT
                               from Product_Info Where COMPANY_ID = :param1 AND upper(SKU_NAME) Like '%' || upper(:param2) || '%'";
        string AddOrUpdate_AddQuery() => @"INSERT INTO PRODUCT_INFO 
                                         (BASE_PRODUCT_ID
                                          ,BRAND_ID
                                          ,CATEGORY_ID
                                          ,COMPANY_ID
                                          ,FONT_COLOR
                                          ,GROUP_ID
                                          ,PACK_SIZE
                                          ,PACK_UNIT
                                          ,PACK_VALUE
                                          ,PRIMARY_PRODUCT_ID
                                          ,PRODUCT_SEASON_ID
                                          ,PRODUCT_STATUS
                                          ,PRODUCT_TYPE_ID
                                          ,QTY_PER_PACK
                                          ,REMARKS
                                          ,SHIPPER_QTY
                                          ,SHIPPER_VOLUME
                                          ,SHIPPER_VOLUME_UNIT
                                          ,SHIPPER_WEIGHT
                                          ,SHIPPER_WEIGHT_UNIT
                                          ,SKU_CODE
                                          ,SKU_ID
                                          ,SKU_NAME
                                          ,SKU_NAME_BANGLA
                                          ,STORAGE_ID
                                          ,UNIT_ID
                                          ,WEIGHT_PER_PACK
                                          ,WEIGHT_UNIT
                                          ,Entered_By
                                          ,Entered_Date,
                                        ENTERED_TERMINAL,
                                        DISTRIBUTOR_PRODUCT_TYPE, PRODUCT_SEGMENT_NAME) 
                                          VALUES ( :param1, :param2, :param3, :param4, :param5, :param6, :param7, :param8, :param9, :param10, :param11, :param12, :param13, :param14,:param15, :param16, :param17, :param18, :param19,:param20,:param21, :param22, :param23, :param24, :param25, :param26, :param27, :param28, :param29,TO_DATE(:param30, 'DD/MM/YYYY HH:MI:SS AM'), :param31, :param32,:param33 )";
        string AddOrUpdate_UpdateQuery() => @"UPDATE Product_Info SET  
                                             BASE_PRODUCT_ID = :param1
                                            ,BRAND_ID =  :param2
                                            ,CATEGORY_ID =  :param3
                                            ,COMPANY_ID =  :param4
                                            ,FONT_COLOR =  :param5
                                            ,GROUP_ID =  :param6
                                            ,PACK_SIZE =  :param7
                                            ,PACK_UNIT =  :param8
                                            ,PACK_VALUE =  :param9
                                            ,PRIMARY_PRODUCT_ID =  :param10
                                            ,PRODUCT_SEASON_ID =  :param11
                                            ,PRODUCT_STATUS =  :param12
                                            ,PRODUCT_TYPE_ID =  :param13
                                            ,QTY_PER_PACK =  :param14
                                            ,REMARKS =  :param15
                                            ,SHIPPER_QTY =  :param16
                                            ,SHIPPER_VOLUME =  :param17
                                            ,SHIPPER_VOLUME_UNIT =  :param18
                                            ,SHIPPER_WEIGHT =  :param19
                                            ,SHIPPER_WEIGHT_UNIT =  :param20
                                            ,SKU_CODE =  :param21
                                            ,SKU_NAME =  :param22
                                            ,SKU_NAME_BANGLA =  :param23
                                            ,STORAGE_ID =  :param24
                                            ,UNIT_ID =  :param25
                                            ,WEIGHT_PER_PACK =  :param26
                                            ,WEIGHT_UNIT  =  :param27,
                                            UPDATED_BY = :param28, UPDATED_DATE = TO_DATE(:param29, 'DD/MM/YYYY HH:MI:SS AM'), 
                                            UPDATED_TERMINAL = :param30,
                                            DISTRIBUTOR_PRODUCT_TYPE= :param31,
                                            PRODUCT_SEGMENT_NAME=:param32
                                            WHERE SKU_ID = :param33";
        string GetNewProduct_Info_IdQuery() => "SELECT NVL(MAX(SKU_ID),0) + 1 SKU_ID  FROM PRODUCT_INFO";
        string Get_LastProduct_Ino() => "SELECT  SKU_ID, SKU_CODE  FROM PRODUCT_INFO Where  SKU_ID = (SELECT   NVL(MAX(SKU_ID),0) SKU_ID From PRODUCT_INFO where COMPANY_ID = :param1 )";
        string LoadProductByProductCode_Query() => @"    
                                        SELECT 
   SKU_ID, SKU_CODE, SKU_NAME, 
   SKU_NAME_BANGLA, PACK_SIZE, PRODUCT_TYPE_ID, 
   PRIMARY_PRODUCT_ID, BRAND_ID, CATEGORY_ID, 
   BASE_PRODUCT_ID, GROUP_ID, PRODUCT_SEASON_ID, 
   QTY_PER_PACK, WEIGHT_PER_PACK, WEIGHT_UNIT, 
   SHIPPER_QTY, SHIPPER_WEIGHT, SHIPPER_WEIGHT_UNIT, 
   SHIPPER_VOLUME, SHIPPER_VOLUME_UNIT, PACK_UNIT, 
   PACK_VALUE, STORAGE_ID, FONT_COLOR, 
   COMPANY_ID, UNIT_ID, PRODUCT_STATUS, 
   REMARKS, ENTERED_BY, ENTERED_DATE, 
   ENTERED_TERMINAL, UPDATED_BY, UPDATED_DATE, 
   UPDATED_TERMINAL
FROM PRODUCT_INFO Where SKU_CODE = :param1 and COMPANY_ID = :param2";



        string Get_SKU_Depot_Relation() => @"Select ROWNUM ROW_NO, SD.SKU_DEPO_ID, 
SD.SKU_ID, 
SD.SKU_CODE, 
SD.DEPOT_ID, 
P.SKU_NAME,
P.SKU_CODE,
C.UNIT_NAME,
C.COMPANY_NAME
from SKU_DEPO_RELATION SD,PRODUCT_INFO P, COMPANY_INFO C 
WHERE SD.SKU_ID = P.SKU_ID AND P.COMPANY_ID = C.COMPANY_ID AND SD.DEPOT_ID = C.UNIT_ID AND P.COMPANY_ID = :param1";
        string SKU_Depot_Add() => @"INSERT INTO SKU_DEPO_RELATION (
   SKU_DEPO_ID, 
   SKU_ID, 
   SKU_CODE, 
   DEPOT_ID,
   ENTERED_DATE, 
   ENTERED_BY, 
   ENTERED_TERMINAL) 
VALUES ( :param1,
 :param2,
 :param3,
 :param4,
  TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),
 :param6,
 :param7 )";
        string SKU_Depot_Delete() => @"DELETE FROM SKU_DEPO_RELATION WHERE SKU_DEPO_ID = :param1";
        string Get_SKU_Depot_Id() => @"SELECT NVL(MAX(SKU_DEPO_ID),0)+1  FROM SKU_DEPO_RELATION";


        //---------- Method Execution Part ------------------------------------------------

        public async Task<string> AddOSkuDepoRelation(string db, SKU_DEPOT_RELATION model)
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


                    if (model.SKU_DEPO_ID == 0)
                    {

                        model.SKU_DEPO_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), Get_SKU_Depot_Id(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(SKU_Depot_Add(),
                         _commonServices.AddParameter(new string[] { model.SKU_DEPO_ID.ToString(),
                                       model.SKU_ID.ToString(),
                                       model.SKU_CODE,
                                       model.DEPOT_ID.ToString(),
                                       model.ENTERED_DATE,
                                       model.ENTERED_BY,
                                       model.ENTERED_TERMINAL })));

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
        public async Task<string> DeleteSkuDepoRelation(string db, string sku_depot_id)
        {
            List<QueryPattern> listOfQuery = new List<QueryPattern>();

            try
            {

                listOfQuery.Add(_commonServices.AddQuery(SKU_Depot_Delete(),
                      _commonServices.AddParameter(new string[] { sku_depot_id })));

                await _commonServices.SaveChangesAsyn(_configuration.GetConnectionString(db), listOfQuery);
                return "1";
            }

            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }
        

        public async Task<string> AddOrUpdate(string db, Product_Info model)
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


                    if (model.SKU_ID == 0)
                    {
                        model.SKU_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewProduct_Info_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddQuery(),
                         _commonServices.AddParameter(new string[] { model.BASE_PRODUCT_ID.ToString(),
                             model.BRAND_ID.ToString(), model.CATEGORY_ID.ToString(),model.COMPANY_ID.ToString(),
                             model.FONT_COLOR, model.GROUP_ID.ToString(), model.PACK_SIZE, model.PACK_UNIT,
                             model.PACK_VALUE.ToString(), model.PRIMARY_PRODUCT_ID.ToString(), model.PRODUCT_SEASON_ID.ToString(),
                             model.PRODUCT_STATUS.ToString(),  model.PRODUCT_TYPE_ID.ToString(),
                             model.QTY_PER_PACK.ToString(), model.REMARKS, model.SHIPPER_QTY.ToString(),
                             model.SHIPPER_VOLUME.ToString(), model.SHIPPER_VOLUME_UNIT, model.SHIPPER_WEIGHT.ToString(),
                             model.SHIPPER_WEIGHT_UNIT.ToString(),model.SKU_CODE, model.SKU_ID.ToString(),
                             model.SKU_NAME, model.SKU_NAME_BANGLA, model.STORAGE_ID.ToString(), model.UNIT_ID.ToString(),
                             model.WEIGHT_PER_PACK.ToString(),model.WEIGHT_UNIT.ToString(),
                             model.ENTERED_BY.ToString(), model.ENTERED_DATE, model.ENTERED_TERMINAL, model.DISTRIBUTOR_PRODUCT_TYPE, model.PRODUCT_SEGMENT_NAME })));

                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(
                            AddOrUpdate_UpdateQuery(),
                            _commonServices.AddParameter(new string[] {
                             model.BASE_PRODUCT_ID.ToString(),
                             model.BRAND_ID.ToString(), model.CATEGORY_ID.ToString(),model.COMPANY_ID.ToString(),
                             model.FONT_COLOR, model.GROUP_ID.ToString(), model.PACK_SIZE, model.PACK_UNIT,
                             model.PACK_VALUE.ToString(), model.PRIMARY_PRODUCT_ID.ToString(), model.PRODUCT_SEASON_ID.ToString(),
                             model.PRODUCT_STATUS.ToString(),  model.PRODUCT_TYPE_ID.ToString(),
                             model.QTY_PER_PACK.ToString(), model.REMARKS, model.SHIPPER_QTY.ToString(),
                             model.SHIPPER_VOLUME.ToString(), model.SHIPPER_VOLUME_UNIT, model.SHIPPER_WEIGHT.ToString(),
                             model.SHIPPER_WEIGHT_UNIT.ToString(),model.SKU_CODE,
                             model.SKU_NAME, model.SKU_NAME_BANGLA, model.STORAGE_ID.ToString(), model.UNIT_ID.ToString(),
                             model.WEIGHT_PER_PACK.ToString(),model.WEIGHT_UNIT.ToString(),
                                model.UPDATED_BY.ToString(), model.UPDATED_DATE,
                                model.UPDATED_TERMINAL, model.DISTRIBUTOR_PRODUCT_TYPE,model.PRODUCT_SEGMENT_NAME,model.SKU_ID.ToString() })));

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
        public async Task<DataTable> LoadProductPrimaryDataTable(string db, int Company_Id) => await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadProductPrimaryData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));

        public async Task<DataTable> LoadDataTable(string db, int Company_Id) => await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString()}));
        public async Task<DataTable> LoadSkuCodeDataTable(string db, int Company_Id) => await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSkuCodeData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString()}));
        public async Task<DataTable> LoadPackSizeDataTable(string db, int Company_Id) => await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadPackSizeData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString()}));
        public async Task<DataTable> LoadProductdropdownDataTable(string db, int Company_Id) => await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadProductdropdownData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString()}));
        public async Task<DataTable> LoadFilteredDataTable(string db, int Company_Id) => await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadFilteredData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString()}));
        public async Task<string> LoadProductPrimaryData(string db, int Company_Id) => _commonServices.DataTableToJSON(await LoadProductPrimaryDataTable(db, Company_Id));
        public async Task<string> LoadSKU_DEPOTData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_SKU_Depot_Relation(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));

        public async Task<string> LoadData(string db, int Company_Id) => _commonServices.DataTableToJSON(await LoadDataTable(db,Company_Id));
        public async Task<string> LoadSkuCodeData(string db, int Company_Id) => _commonServices.DataTableToJSON(await LoadSkuCodeDataTable(db,Company_Id));

        public async Task<string> LoadPackSizeData(string db, int Company_Id) => _commonServices.DataTableToJSON(await LoadPackSizeDataTable(db, Company_Id));
        public async Task<string> LoadProductdropdownData(string db, int Company_Id) => _commonServices.DataTableToJSON(await LoadProductdropdownDataTable(db, Company_Id));

        public async Task<string> GetSearchableProduct(string db, int Company_Id, string product) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadSearchableData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString(), product })));

        public async Task<string> LoadFilteredData(Price_Dtl_Param priceInfo)
        {
            if (!string.IsNullOrEmpty(priceInfo.GROUP_ID) && priceInfo.GROUP_ID != "0")
            {
              
                FilterQry += string.Format("  and Group_ID = {0}", Convert.ToInt32(priceInfo.GROUP_ID));
            }
            if (!string.IsNullOrEmpty(priceInfo.BRAND_ID) && priceInfo.BRAND_ID != "0")
            {
                FilterQry += string.Format("  and Brand_ID = {0}", Convert.ToInt32(priceInfo.BRAND_ID));
            }
           
            if (!string.IsNullOrEmpty(priceInfo.BASE_PRODUCT_ID) && priceInfo.BASE_PRODUCT_ID !="0")
            {
                FilterQry += string.Format("  and Base_Product_Id = {0}", Convert.ToInt32(priceInfo.BASE_PRODUCT_ID));
            }
            if (!string.IsNullOrEmpty(priceInfo.CATEGORY_ID) && priceInfo.CATEGORY_ID != "0")
            {
                FilterQry += string.Format("  and Category_Id = {0}", Convert.ToInt32(priceInfo.CATEGORY_ID));
            }
            if (priceInfo.PRODUCT_ID != null && priceInfo.PRODUCT_ID.Count > 0)
            {
                string _SKU_ID = "";
                for (int i = 0; i < priceInfo.PRODUCT_ID.Count; i++)
                {
                    if (priceInfo.PRODUCT_ID[i] != "0")
                    {
                        if (i == 0)
                        {
                            _SKU_ID = priceInfo.PRODUCT_ID[i];

                        }
                        else
                        {
                            _SKU_ID = _SKU_ID + "," + priceInfo.PRODUCT_ID[i];

                        }
                    }

                }
                if (_SKU_ID != "")
                {
                    FilterQry = FilterQry + " AND  SKU_ID in (" + _SKU_ID + ")";

                }
            }
            //if (!string.IsNullOrEmpty(priceInfo.PRODUCT_ID) && priceInfo.PRODUCT_ID != "0")
            //{
            //    FilterQry += string.Format("  and SKU_ID = {0}", Convert.ToInt32(priceInfo.PRODUCT_ID));
            //}
             var dd =_commonServices.DataTableToJSON(await LoadFilteredDataTable(priceInfo.db, priceInfo.COMPANY_ID));
            return dd;
        }
        public async Task<string> GenerateProductCode(string db, string Company_Id)
        {
            try
            {
                string code;
                DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Get_LastProduct_Ino(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
                if (dataTable.Rows.Count > 0)
                {
                    string serial = (Convert.ToInt32(dataTable.Rows[0]["SKU_CODE"].ToString().Substring(3, (CodeConstants.ProductInfo_CodeLength - 3))) + 1).ToString();
                    int serial_length = serial.Length;
                    code = CodeConstants.ProductInfo_CodeConst;
                    for (int i = 0; i < (CodeConstants.ProductInfo_CodeLength - (serial_length + 3)); i++)
                    {
                        code += "0";
                    }
                    code += serial;
                }
                else
                {
                    code = CodeConstants.ProductInfo_CodeConst + "0001";
                }
                return code;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> LoadProductByProductCode(string db, int Company_Id, string product_code)
         => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadProductByProductCode_Query(), _commonServices.AddParameter(new string[] { product_code,  Company_Id.ToString() })));

        public async Task<string> LoadProductSegmentInfo(string db)
         => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), "SELECT *FROM SEGMENT_INFO", _commonServices.AddParameter(new string[] {  })));

        public async Task<string> LoadDataFromView(string db, int Company_Id)
        {
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadDataFromView_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        }        
        public async Task<string> LoadDropDownDataFromView(string db, int Company_Id)
        {
            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadDropDownDataFromView_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        }
    }
}




