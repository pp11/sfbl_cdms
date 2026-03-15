using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution.Manager
{
    public class BonusManager : IBonusManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public BonusManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }
        //Query Part ------------------------------------------------------------------------------
        string LoadData_Query() => @"Select
                                ROW_NUMBER() OVER(ORDER BY BONUS_MST_ID ASC) AS ROW_NO
                               ,BONUS_MST_ID
                               ,BONUS_NAME
                               ,COMPANY_ID
                               ,EFFECT_END_DATE
                               ,EFFECT_START_DATE
                               ,ENTERED_BY
                               ,ENTERED_DATE
                               ,ENTERED_TERMINAL
                               ,ENTRY_DATE
                               ,LOCATION_TYPE
                               ,REMARKS
                               ,STATUS
                               ,UNIT_ID
                               from BONUS_MST 

                                Where COMPANY_ID = :param1";

        
  string LoadBonusMstById_Query() => @"Select
                                ROW_NUMBER() OVER(ORDER BY BONUS_MST_ID ASC) AS ROW_NO
                               ,BONUS_MST_ID
                               ,BONUS_NAME
                               ,COMPANY_ID
                               ,EFFECT_END_DATE
                               ,EFFECT_START_DATE
                               ,ENTERED_BY
                               ,ENTERED_DATE
                               ,ENTERED_TERMINAL
                               ,ENTRY_DATE
                               ,LOCATION_TYPE
                               ,REMARKS
                               ,STATUS
                               ,UNIT_ID
                               from BONUS_MST 

                                Where BONUS_MST_ID = :param1";
        string LoadBonusDeclareProductById_Query() => @"Select
                                ROW_NUMBER() OVER(ORDER BY BP.BONUS_DECLARE_ID ASC) AS ROW_NO
                                ,BP.BASE_PRODUCT_ID
                                ,BP.BONUS_DECLARE_ID
                                ,BP.BONUS_MST_ID
                                ,BP.BRAND_ID
                                ,BP.CATEGORY_ID
                                ,BP.ENTERED_BY
                                ,BP.ENTERED_DATE
                                ,BP.ENTERED_TERMINAL
                                ,BP.GROUP_ID
                                ,BP.SKU_CODE
                                ,BP.SKU_ID
                                ,BP.STATUS
                               
                              ,p.COMPANY_ID
                              ,p.FONT_COLOR
                              ,p.GROUP_ID
                              ,p.PACK_SIZE
                              ,p.PACK_UNIT
                              ,p.PACK_VALUE
                              ,p.PRIMARY_PRODUCT_ID
                              ,p.PRODUCT_TYPE_ID

                              ,p.SKU_NAME

                              ,p.UNIT_ID

                              ,bpi.BASE_PRODUCT_NAME
                              ,b.BRAND_NAME
                              ,g.GROUP_NAME
                              ,C.CATEGORY_NAME

                               from BONUS_DECLARE_PRODUCT  BP
                               left outer join Product_Info p on P.SKU_ID = BP.SKU_ID
                               left outer join Base_Product_Info bpi on bpi.BASE_PRODUCT_ID = P.BASE_PRODUCT_ID
                               left outer join Category_info c on C.CATEGORY_ID = P.CATEGORY_ID
                               left outer join Group_Info g on g.GROUP_ID = P.GROUP_ID
                               left outer join BRAND_INFO b on b.BRAND_ID = P.BRAND_ID
                                Where BONUS_MST_ID = :param1";
        string LoadBonusLocationById_Query() => @"Select
                                ROW_NUMBER() OVER(ORDER BY BONUS_LOCATION_ID ASC) AS ROW_NO
                                ,BONUS_LOCATION_ID
                                ,BONUS_MST_ID
                                ,ENTERED_BY
                                ,ENTERED_DATE
                                ,ENTERED_TERMINAL
                                ,LOCATION_CODE
                                ,LOCATION_ID
                                ,LOCATION_TYPE
                                
                                ,STATUS

                               from BONUS_LOCATION 
                               

                                Where BONUS_MST_ID = :param1";
        string LoadBonusDtlById_Query() => @"Select
                                ROW_NUMBER() OVER(ORDER BY BONUS_DTL_ID ASC) AS ROW_NO
                                ,BONUS_DTL_ID
                                ,BONUS_MST_ID
                                ,BONUS_QTY
                                ,BONUS_SKU_CODE
                                ,BONUS_SKU_ID
                                ,BONUS_SLAB_TYPE
                                ,BONUS_TYPE
                                ,CALCULATION_TYPE
                                ,DISCOUNT_PCT
                                ,DISCOUNT_VAL
                                ,ENTERED_BY
                                ,ENTERED_DATE
                                ,ENTERED_TERMINAL
                                ,GIFT_ITEM_ID
                                ,GIFT_QTY
                                ,SLAB_QTY
                                ,STATUS

                                from BONUS_DTL

                                Where BONUS_MST_ID = :param1 ";
        string LoadGroupData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.GROUP_ID ASC) AS ROW_NO,
                                    M.GROUP_ID, M.GROUP_NAME, M.COMPANY_ID, M.GROUP_CODE , M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM GROUP_INFO  M  Where M.COMPANY_ID = :param1";
        string LoadBrandData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.BRAND_ID ASC) AS ROW_NO,
                                    M.BRAND_ID, M.BRAND_NAME, M.COMPANY_ID, M.BRAND_CODE , M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM BRAND_INFO  M  Where M.COMPANY_ID = :param1";
        string LoadCategoryData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.CATEGORY_ID ASC) AS ROW_NO,
                                    M.CATEGORY_ID, M.CATEGORY_NAME, M.COMPANY_ID, M.CATEGORY_CODE , M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM CATEGORY_INFO  M  Where M.COMPANY_ID = :param1";

        string LoadBaseProductData_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.BASE_PRODUCT_ID ASC) AS ROW_NO,
                                    M.BASE_PRODUCT_ID, M.BASE_PRODUCT_NAME, M.COMPANY_ID, M.BASE_PRODUCT_CODE , M.REMARKS, M.STATUS,  TO_CHAR(M.ENTERED_DATE, 'YYYY-MM-DD') ENTERED_DATE
                                    FROM BASE_PRODUCT_INFO  M  Where M.COMPANY_ID = :param1";
        string LoadProductData_Query() => @"Select
                               ROW_NUMBER() OVER(ORDER BY p.SKU_ID ASC) AS ROW_NO
                              ,p.BASE_PRODUCT_ID
                              ,p.BRAND_ID
                              ,p.CATEGORY_ID
                              ,p.COMPANY_ID
                              ,p.FONT_COLOR
                              ,NVL(p.GROUP_ID,0) GROUP_ID
                              ,p.PACK_SIZE
                              ,p.PACK_UNIT
                              ,p.PACK_VALUE
                              ,p.PRIMARY_PRODUCT_ID
                              ,p.PRODUCT_SEASON_ID
                              ,p.PRODUCT_STATUS
                              ,p.PRODUCT_TYPE_ID
                              ,p.QTY_PER_PACK
                              ,p.REMARKS
                              ,p.SHIPPER_QTY
                              ,p.SHIPPER_VOLUME
                              ,p.SHIPPER_VOLUME_UNIT
                              ,p.SHIPPER_WEIGHT
                              ,p.SHIPPER_WEIGHT_UNIT
                              ,p.SKU_CODE
                              ,p.SKU_ID
                              ,p.SKU_NAME
                              ,p.SKU_NAME_BANGLA
                              ,p.STORAGE_ID
                              ,p.UNIT_ID
                              ,p.WEIGHT_PER_PACK
                              ,p.WEIGHT_UNIT
                              ,BP.BASE_PRODUCT_NAME
                              ,b.BRAND_NAME
                              ,g.GROUP_NAME
                              ,C.CATEGORY_NAME
                              ,0 UNIT_TP

                               from Product_Info p
                               left outer join Base_Product_Info bp on BP.BASE_PRODUCT_ID = P.BASE_PRODUCT_ID
                               left outer join Category_info c on C.CATEGORY_ID = P.CATEGORY_ID
                               left outer join Group_Info g on g.GROUP_ID = P.GROUP_ID
                               left outer join BRAND_INFO b on b.BRAND_ID = P.BRAND_ID

                                Where p.COMPANY_ID = :param1   and P.PRODUCT_STATUS = 'Active'";

        string GetNewBonus_Mst_IdQuery() => "SELECT NVL(MAX(BONUS_MST_ID),0) + 1 BONUS_MST_ID  FROM BONUS_MST";
        string GetNewBonus_location_IdQuery() => "SELECT NVL(MAX(BONUS_LOCATION_ID),0) + 1 BONUS_MST_ID  FROM BONUS_LOCATION";
        string GetNewBonus_declare_product_IdQuery() => "SELECT NVL(MAX(BONUS_DECLARE_ID),0) + 1 BONUS_MST_ID  FROM BONUS_DECLARE_PRODUCT";
        string GetNewBonus_Dtl_IdQuery() => "SELECT NVL(MAX(BONUS_DTL_ID),0) + 1 BONUS_MST_ID  FROM BONUS_DTL";

        string GetLocation_ByLocationTypeQuery() => "begin   :param1 := FN_FIND_BONUS_LOCATION(:param2,:param3) ;end;";


        string AddOrUpdate_AddBonus_MstQuery() => @"INSERT INTO BONUS_MST 
                                         (BONUS_MST_ID
                                         ,BONUS_NAME
                                         ,COMPANY_ID
                                         ,EFFECT_END_DATE
                                         ,EFFECT_START_DATE
                                         ,ENTERED_BY
                                         ,ENTERED_DATE
                                         ,ENTERED_TERMINAL
                                         ,ENTRY_DATE
                                         ,LOCATION_TYPE
                                         ,REMARKS
                                         ,STATUS
                                         ,UNIT_ID
                                          ) 
                                         VALUES ( :param1, :param2, :param3, TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM') ,TO_DATE(:param5, 'DD/MM/YYYY HH:MI:SS AM'),:param6 ,TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), :param8,TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'),:param10 ,:param11,:param12,:param13)";
        string AddOrUpdate_UpdateBonusMst_Query() => @"UPDATE BONUS_MST SET 
                                          BONUS_NAME = :param2
                                         ,EFFECT_END_DATE = TO_DATE( :param3, 'DD/MM/YYYY HH:MI:SS AM')
                                         ,EFFECT_START_DATE = TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM')
                                         ,LOCATION_TYPE = :param5
                                         ,REMARKS = :param6
                                         ,STATUS = :param7
                                         ,UPDATED_BY = :param8, UPDATED_DATE = TO_DATE(:param9, 'DD/MM/YYYY HH:MI:SS AM'), 
                                          UPDATED_TERMINAL = :param10 WHERE BONUS_MST_ID = :param1";
        string AddOrUpdate_AddBonus_LocationQuery() => @"INSERT INTO BONUS_LOCATION 
                                         (BONUS_LOCATION_ID
                                          ,BONUS_MST_ID
                                          ,ENTERED_BY
                                          ,ENTERED_DATE
                                          ,ENTERED_TERMINAL
                                          ,LOCATION_CODE
                                          ,LOCATION_ID
                                          ,LOCATION_TYPE
                                          ,STATUS
                                           ) 
                                         VALUES ( :param1, :param2, :param3, TO_DATE(:param4, 'DD/MM/YYYY HH:MI:SS AM') ,:param5,:param6 ,:param7, :param8,:param9)";
        string AddOrUpdate_Delete_LocationQuery() => @"DELETE FROM BONUS_LOCATION Where BONUS_LOCATION_ID = :param1";
        string AddOrUpdate_Delete_DeclareProductQuery() => @"DELETE FROM BONUS_DECLARE_PRODUCT Where BONUS_DECLARE_ID = :param1";
        string AddOrUpdate_AddBonus_DltQuery() => @"INSERT INTO BONUS_DTL 
                                         (BONUS_DTL_ID
                                          ,BONUS_MST_ID
                                          ,BONUS_QTY
                                          ,BONUS_SKU_CODE
                                          ,BONUS_SKU_ID
                                          ,BONUS_SLAB_TYPE
                                          ,BONUS_TYPE
                                          ,CALCULATION_TYPE
                                          ,DISCOUNT_PCT
                                          ,DISCOUNT_VAL
                                          ,ENTERED_BY
                                          ,ENTERED_DATE
                                          ,ENTERED_TERMINAL
                                          ,GIFT_ITEM_ID
                                          ,GIFT_QTY
                                          ,SLAB_QTY
                                          ,STATUS
                                           ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5,:param6 ,:param7, :param8,:param9,:param10,:param11 ,TO_DATE(:param12, 'DD/MM/YYYY HH:MI:SS AM'), :param13,:param14,:param15, :param16,:param17)";
        string AddOrUpdate_BonusDtl_Update_Query() => @"UPDATE BONUS_DTL SET 
                                           
                                           BONUS_QTY = :param2
                                          ,BONUS_SKU_CODE = :param3
                                          ,BONUS_SKU_ID = :param4
                                          ,BONUS_SLAB_TYPE = :param5
                                          ,BONUS_TYPE = :param6
                                          ,CALCULATION_TYPE = :param7
                                          ,DISCOUNT_PCT = :param8
                                          ,DISCOUNT_VAL = :param9
                                          ,GIFT_ITEM_ID = :param10
                                          ,GIFT_QTY = :param11
                                          ,SLAB_QTY = :param12
                                          ,STATUS = :param13
                                         ,UPDATED_BY = :param14, UPDATED_DATE = TO_DATE(:param15, 'DD/MM/YYYY HH:MI:SS AM'), 
                                          UPDATED_TERMINAL = :param16 WHERE BONUS_DTL_ID = :param1";
        string AddOrUpdate_Bonus_Declare_ProductQuery() => @"INSERT INTO BONUS_DECLARE_PRODUCT 
                                         (BASE_PRODUCT_ID
                                          ,BONUS_DECLARE_ID
                                          ,BONUS_MST_ID
                                          ,BRAND_ID
                                          ,CATEGORY_ID
                                          ,ENTERED_BY
                                          ,ENTERED_DATE
                                          ,ENTERED_TERMINAL
                                          ,GROUP_ID
                                          ,SKU_CODE
                                          ,SKU_ID
                                          ,STATUS
                                           ) 
                                         VALUES ( :param1, :param2, :param3, :param4,:param5,:param6 ,TO_DATE(:param7, 'DD/MM/YYYY HH:MI:SS AM'), :param8,:param9,:param10,:param11 ,:param12)";

        //Execution Part--------------------------------------------------------------------

        public async Task<string> LoadData(string db, int Company_Id)
        {
           DataTable dataTable =   await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            List<Bonus_Mst> bonus_Msts = new List<Bonus_Mst>();
            if (dataTable.Rows.Count > 0)
            {
                for(int i=0;i<dataTable.Rows.Count;i++)
                {
                    Bonus_Mst _bonus_Mst = new Bonus_Mst();

                    _bonus_Mst.COMPANY_ID = Convert.ToInt32(dataTable.Rows[i]["COMPANY_ID"]);
                    _bonus_Mst.BONUS_MST_ID = Convert.ToInt32(dataTable.Rows[i]["BONUS_MST_ID"]);
                    _bonus_Mst.BONUS_NAME = dataTable.Rows[i]["BONUS_NAME"].ToString();

                    _bonus_Mst.LOCATION_TYPE = dataTable.Rows[i]["LOCATION_TYPE"].ToString();
                    _bonus_Mst.STATUS = dataTable.Rows[i]["STATUS"].ToString();
                    _bonus_Mst.EFFECT_START_DATE = dataTable.Rows[i]["EFFECT_START_DATE"].ToString();
                    _bonus_Mst.EFFECT_END_DATE = dataTable.Rows[i]["EFFECT_END_DATE"].ToString();
                    _bonus_Mst.REMARKS = dataTable.Rows[i]["REMARKS"].ToString();
                    _bonus_Mst.BONUS_MST_ID_ENCRYPTED = _commonServices.Encrypt(_bonus_Mst.BONUS_MST_ID.ToString());
                    bonus_Msts.Add(_bonus_Mst);
                }
            }
            return JsonSerializer.Serialize(bonus_Msts);
        }
           

        public async Task<string> LoadBaseProductData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadBaseProductData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> LoadCategoryData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadCategoryData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> LoadBrandData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadBrandData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> LoadGroupData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadGroupData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> LoadProductData(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadProductData_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));

        public int LoadNewBonusNo(string db) => _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewBonus_Mst_IdQuery(), _commonServices.AddParameter(new string[] { }));
        public async Task<DataTable> GetLocation_ByLocationTypeDatatable(string db, int Company_Id, string LocationType) => await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetLocation_ByLocationTypeQuery(), _commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(), LocationType, Company_Id.ToString() }));

        public async Task<string> GetLocation_ByLocationType(string db, int Company_Id, string LocationType) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), GetLocation_ByLocationTypeQuery(), _commonServices.AddParameter(new string[] { OracleDbType.RefCursor.ToString(),  LocationType,Company_Id.ToString() })));
        public async Task<string> GetProductDataFiltered(string db, int Company_Id, ProductFilterParameters parameters )
        {
            string Query = LoadProductData_Query();
       
            if (parameters.BASE_PRODUCT_ID != null && parameters.BASE_PRODUCT_ID.Count > 0)
            {
                string _BASE_PRODUCT_ID = "";
                for (int i = 0; i < parameters.BASE_PRODUCT_ID.Count; i++)
                {
                    if(parameters.BASE_PRODUCT_ID[i]!="0")
                    {
                        if (i == 0)
                        {
                            _BASE_PRODUCT_ID = parameters.BASE_PRODUCT_ID[i];

                        }
                        else
                        {
                            _BASE_PRODUCT_ID = _BASE_PRODUCT_ID + "," + parameters.BASE_PRODUCT_ID[i];

                        }
                    }
                    
                }
                if(_BASE_PRODUCT_ID!="")
                {
                    Query = Query + " AND p.BASE_PRODUCT_ID in (" + _BASE_PRODUCT_ID + ")";

                }
            }
            if (parameters.GROUP_ID != null && parameters.GROUP_ID.Count > 0)
            {
                string _GROUP_ID = "";
                for (int i = 0; i < parameters.GROUP_ID.Count; i++)
                {
                    if (parameters.GROUP_ID[i] != "0")
                    {
                        if (i == 0)
                        {
                            _GROUP_ID = parameters.GROUP_ID[i];

                        }
                        else
                        {
                            _GROUP_ID = _GROUP_ID + "," + parameters.GROUP_ID[i];

                        }
                    }
                }
                if(_GROUP_ID!="")
                {
                    Query = Query + " AND  p.GROUP_ID in (" + _GROUP_ID + ")";

                }
            }

            if (parameters.BRAND_ID != null && parameters.BRAND_ID.Count > 0)
            {
                string _BRAND_ID = "";
                for (int i = 0; i < parameters.BRAND_ID.Count; i++)
                {
                    if (parameters.BRAND_ID[i] != "0")
                    {
                        if (i == 0)
                        {
                            _BRAND_ID = parameters.BRAND_ID[i];

                        }
                        else
                        {
                            _BRAND_ID = _BRAND_ID + "," + parameters.BRAND_ID[i];

                        }
                    }
                        
                }
                if (_BRAND_ID != "")
                {
                    Query = Query + " AND  p.BRAND_ID in (" + _BRAND_ID + ")";

                }
            }

            if (parameters.CATEGORY_ID != null && parameters.CATEGORY_ID.Count > 0)
            {
                string _CATEGORY_ID = "";
                for (int i = 0; i < parameters.CATEGORY_ID.Count; i++)
                {
                    if (parameters.CATEGORY_ID[i] != "0")
                    {
                        if (i == 0)
                        {
                            _CATEGORY_ID = parameters.CATEGORY_ID[i];

                        }
                        else
                        {
                            _CATEGORY_ID = _CATEGORY_ID + "," + parameters.CATEGORY_ID[i];

                        }
                    }
                        
                }
                if (_CATEGORY_ID != "")
                {
                    Query = Query + " AND  p.CATEGORY_ID in (" + _CATEGORY_ID + ")";

                }
            }


            if (parameters.SKU_ID != null && parameters.SKU_ID.Count > 0)
            {
                string _SKU_ID = "";
                for(int i=0;i<parameters.SKU_ID.Count;i++)
                {
                    if (parameters.SKU_ID[i] != "0")
                    {
                        if (i == 0)
                        {
                            _SKU_ID = parameters.SKU_ID[i];

                        }
                        else
                        {
                            _SKU_ID = _SKU_ID + "," + parameters.SKU_ID[i];

                        }
                    }
                        
                }
                if (_SKU_ID != "")
                {
                    Query = Query + " AND  p.SKU_ID in (" + _SKU_ID + ")";

                }
            }
            

            return _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), Query, _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        }


        public string LoadLocationTypes(string db, int Company_Id)
        {
            List<string> LocationTypes = new List<string>();
            LocationTypes.Add("Division");
            LocationTypes.Add("Region");
            LocationTypes.Add("Area");
            LocationTypes.Add("Teritory");
            LocationTypes.Add("Market");
            LocationTypes.Add("Customer");

            return JsonSerializer.Serialize(LocationTypes);

        }
        public async Task<Bonus_Mst> LoadEditDataByMasterId(string db, int Id)
        {
            Bonus_Mst _bonus_Mst = new Bonus_Mst();

            DataTable dataTable = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadBonusMstById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));
            if (dataTable.Rows.Count > 0)
            {
                _bonus_Mst.COMPANY_ID = Convert.ToInt32(dataTable.Rows[0]["COMPANY_ID"]);
                _bonus_Mst.BONUS_MST_ID = Id;
                _bonus_Mst.BONUS_NAME = dataTable.Rows[0]["BONUS_NAME"].ToString();

                _bonus_Mst.LOCATION_TYPE = dataTable.Rows[0]["LOCATION_TYPE"].ToString();
                _bonus_Mst.STATUS = dataTable.Rows[0]["STATUS"].ToString();
                _bonus_Mst.EFFECT_START_DATE = Convert.ToDateTime(dataTable.Rows[0]["EFFECT_START_DATE"]).ToString("dd/MM/yyyy");
                _bonus_Mst.EFFECT_END_DATE = Convert.ToDateTime(dataTable.Rows[0]["EFFECT_END_DATE"]).ToString("dd/MM/yyyy");
                _bonus_Mst.REMARKS = dataTable.Rows[0]["REMARKS"].ToString();

                DataTable dataTable_location = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadBonusLocationById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));

                _bonus_Mst.Bonus_Locations = new List<Bonus_Location>();
                List<Bonus_Location> _locations = new List<Bonus_Location>();
                DataTable locations = await GetLocation_ByLocationTypeDatatable(db, _bonus_Mst.COMPANY_ID, _bonus_Mst.LOCATION_TYPE);
               for(int i = 0; i < locations.Rows.Count; i++)
                {
                    Bonus_Location bonus_Location = new Bonus_Location();
                    bonus_Location.LOCATION_CODE = locations.Rows[i]["LOCATION_CODE"].ToString();
                    bonus_Location.LOCATION_ID = Convert.ToInt32(locations.Rows[i]["LOCATION_ID"]);
                    bonus_Location.LOCATION_NAME = locations.Rows[i]["LOCATION_NAME"].ToString();

                    _locations.Add(bonus_Location);
                }
                for (int i = 0; i < dataTable_location.Rows.Count; i++)
                {
                    Bonus_Location _bonus_Location = new Bonus_Location();
                    
                    _bonus_Location.BONUS_LOCATION_ID = Convert.ToInt32(dataTable_location.Rows[i]["BONUS_LOCATION_ID"]);
                    _bonus_Location.BONUS_MST_ID = Convert.ToInt32(dataTable_location.Rows[i]["BONUS_MST_ID"]);
                    _bonus_Location.LOCATION_CODE = dataTable_location.Rows[i]["LOCATION_CODE"].ToString();
                    _bonus_Location.LOCATION_ID = Convert.ToInt32(dataTable_location.Rows[i]["LOCATION_ID"]);

                    _bonus_Location.LOCATION_NAME = _locations.Find(x => x.LOCATION_ID == _bonus_Location.LOCATION_ID).LOCATION_NAME;

                    _bonus_Location.LOCATION_TYPE = dataTable_location.Rows[i]["LOCATION_TYPE"].ToString();
                    _bonus_Location.STATUS = dataTable_location.Rows[i]["STATUS"].ToString();
                    _bonus_Mst.Bonus_Locations.Add(_bonus_Location);
                }
                DataTable dataTable_Product = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadBonusDeclareProductById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));

                _bonus_Mst.BonusDeclareProduct = new List<Bonus_Declare_Product>();

                for (int i = 0; i < dataTable_Product.Rows.Count; i++)
                {
                    Bonus_Declare_Product _bonus_product = new Bonus_Declare_Product();

                    _bonus_product.BASE_PRODUCT_ID = Convert.ToInt32(dataTable_Product.Rows[i]["BASE_PRODUCT_ID"]);
                    _bonus_product.BONUS_DECLARE_ID = Convert.ToInt32(dataTable_Product.Rows[i]["BONUS_DECLARE_ID"]);
                    _bonus_product.BONUS_MST_ID = Convert.ToInt32(dataTable_Product.Rows[i]["BONUS_MST_ID"]);
                    _bonus_product.PACK_SIZE = dataTable_Product.Rows[i]["PACK_SIZE"].ToString();
                    _bonus_product.CATEGORY_NAME = dataTable_Product.Rows[i]["CATEGORY_NAME"].ToString();
                    _bonus_product.BASE_PRODUCT_NAME = dataTable_Product.Rows[i]["BASE_PRODUCT_NAME"].ToString();
                    _bonus_product.GROUP_NAME = dataTable_Product.Rows[i]["GROUP_NAME"].ToString();
                    _bonus_product.BRAND_NAME = dataTable_Product.Rows[i]["BRAND_NAME"].ToString();
                    _bonus_product.SKU_NAME = dataTable_Product.Rows[i]["SKU_NAME"].ToString();

                    _bonus_product.BRAND_ID = Convert.ToInt32(dataTable_Product.Rows[i]["BRAND_ID"]);
                    _bonus_product.CATEGORY_ID = Convert.ToInt32(dataTable_Product.Rows[i]["CATEGORY_ID"]);
                    _bonus_product.GROUP_ID = Convert.ToInt32(dataTable_Product.Rows[i]["GROUP_ID"]);
                    _bonus_product.SKU_CODE = dataTable_Product.Rows[i]["SKU_CODE"].ToString();
                    _bonus_product.SKU_ID = Convert.ToInt32(dataTable_Product.Rows[i]["SKU_ID"]);
                    _bonus_product.STATUS = dataTable_Product.Rows[i]["STATUS"].ToString();
           
                    _bonus_Mst.BonusDeclareProduct.Add(_bonus_product);
                }
                DataTable dataTable_dlts = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadBonusDtlById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));

                _bonus_Mst.Bonus_Dtls = new List<Bonus_Dtl>();

                for (int i = 0; i < dataTable_dlts.Rows.Count; i++)
                {
                    Bonus_Dtl _bonus_Dtl = new Bonus_Dtl();

                    _bonus_Dtl.BONUS_DTL_ID = Convert.ToInt32(dataTable_dlts.Rows[i]["BONUS_DTL_ID"]);
                    _bonus_Dtl.BONUS_MST_ID = Convert.ToInt32(dataTable_dlts.Rows[i]["BONUS_MST_ID"]);
                    _bonus_Dtl.BONUS_QTY = Convert.ToDecimal(dataTable_dlts.Rows[i]["BONUS_QTY"]);
                    _bonus_Dtl.BONUS_SKU_CODE = dataTable_dlts.Rows[i]["BONUS_SKU_CODE"].ToString();
                    _bonus_Dtl.BONUS_SKU_ID = Convert.ToInt32(dataTable_dlts.Rows[i]["BONUS_SKU_ID"]);
                    _bonus_Dtl.BONUS_SLAB_TYPE = dataTable_dlts.Rows[i]["BONUS_SLAB_TYPE"].ToString();
                    _bonus_Dtl.BONUS_TYPE = dataTable_dlts.Rows[i]["BONUS_TYPE"].ToString();
                    _bonus_Dtl.CALCULATION_TYPE = dataTable_dlts.Rows[i]["CALCULATION_TYPE"].ToString();
                    _bonus_Dtl.DISCOUNT_PCT = Convert.ToDecimal(dataTable_dlts.Rows[i]["DISCOUNT_PCT"]);
                    _bonus_Dtl.DISCOUNT_VAL = Convert.ToDecimal(dataTable_dlts.Rows[i]["DISCOUNT_VAL"]);
                    _bonus_Dtl.GIFT_ITEM_ID = Convert.ToInt32(dataTable_dlts.Rows[i]["GIFT_ITEM_ID"]);
                    _bonus_Dtl.GIFT_QTY = Convert.ToDecimal(dataTable_dlts.Rows[i]["GIFT_QTY"]);
                    _bonus_Dtl.SLAB_QTY = Convert.ToDecimal(dataTable_dlts.Rows[i]["SLAB_QTY"]);
                    _bonus_Dtl.STATUS = dataTable_dlts.Rows[i]["STATUS"].ToString();

                    _bonus_Mst.Bonus_Dtls.Add(_bonus_Dtl);
                }
            }

            return _bonus_Mst;



        }
        public async Task<string> AddOrUpdate(string db, int Company_Id, Bonus_Mst model)
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

                    if (model.BONUS_MST_ID == 0)
                    {

                        model.BONUS_MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewBonus_Mst_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddBonus_MstQuery(), _commonServices.AddParameter(new string[] { model.BONUS_MST_ID.ToString(), model.BONUS_NAME, model.COMPANY_ID.ToString(), model.EFFECT_END_DATE, model.EFFECT_START_DATE, model.ENTERED_BY, model.ENTERED_DATE, model.ENTERED_TERMINAL, model.ENTRY_DATE, model.LOCATION_TYPE, model.REMARKS,model.STATUS,model.UNIT_ID.ToString() })));
                        int location_id = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewBonus_location_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        foreach (var item in model.Bonus_Locations)
                        {
                            item.BONUS_LOCATION_ID = location_id;
                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddBonus_LocationQuery(), _commonServices.AddParameter(new string[] { item.BONUS_LOCATION_ID.ToString(), model.BONUS_MST_ID.ToString(), model.ENTERED_BY, model.ENTERED_DATE, model.ENTERED_TERMINAL, item.LOCATION_CODE, item.LOCATION_ID.ToString(), model.LOCATION_TYPE, item.STATUS })));
                            location_id++;
                        }
                        int declareId= _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewBonus_declare_product_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        foreach (var item in model.BonusDeclareProduct)
                        {
                            item.BONUS_DECLARE_ID = declareId;

                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Bonus_Declare_ProductQuery(), _commonServices.AddParameter(new string[] { item.BASE_PRODUCT_ID.ToString(), item.BONUS_DECLARE_ID.ToString(), model.BONUS_MST_ID.ToString(), item.BRAND_ID.ToString(), item.CATEGORY_ID.ToString(), model.ENTERED_BY, model.ENTERED_DATE, model.ENTERED_TERMINAL, item.GROUP_ID.ToString(),item.SKU_CODE, item.SKU_ID.ToString(), item.STATUS })));
                            declareId++;
                        }
                        int detail_id = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewBonus_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        foreach (var item in model.Bonus_Dtls)
                        {
                            item.BONUS_DTL_ID = detail_id;

                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddBonus_DltQuery(), _commonServices.AddParameter(new string[] { item.BONUS_DTL_ID.ToString(), model.BONUS_MST_ID.ToString(),item.BONUS_QTY.ToString(),item.BONUS_SKU_CODE,item.BONUS_SKU_ID.ToString(),item.BONUS_SLAB_TYPE,item.BONUS_TYPE,item.CALCULATION_TYPE,item.DISCOUNT_PCT.ToString(),item.DISCOUNT_VAL.ToString(), model.ENTERED_BY, model.ENTERED_DATE, model.ENTERED_TERMINAL, item.GIFT_ITEM_ID.ToString(),item.GIFT_QTY.ToString(),item.SLAB_QTY.ToString(),item.STATUS })));
                            detail_id++;
                        }
                    
                    }
                    else
                    {

                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_UpdateBonusMst_Query(), _commonServices.AddParameter(new string[] { model.BONUS_MST_ID.ToString(), model.BONUS_NAME, model.EFFECT_END_DATE, model.EFFECT_START_DATE, model.LOCATION_TYPE, model.REMARKS, model.STATUS, model.UPDATED_BY, model.UPDATED_DATE, model.UPDATED_TERMINAL })));
                        int location_id = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewBonus_location_IdQuery(), _commonServices.AddParameter(new string[] { }));

                        for (int i = 0; i < model.Bonus_Locations.Count; i++)
                        {
                            if(model.Bonus_Locations[i].BONUS_LOCATION_ID==0)
                            {

                                    model.Bonus_Locations[i].BONUS_LOCATION_ID = location_id;
                                    listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddBonus_LocationQuery(), _commonServices.AddParameter(new string[] { model.Bonus_Locations[i].BONUS_LOCATION_ID.ToString(), model.BONUS_MST_ID.ToString(), model.ENTERED_BY, model.ENTERED_DATE, model.ENTERED_TERMINAL, model.Bonus_Locations[i].LOCATION_CODE, model.Bonus_Locations[i].LOCATION_ID.ToString(), model.LOCATION_TYPE, model.Bonus_Locations[i].STATUS })));
                                    location_id++;

                            }
                        }

                        int declareId = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewBonus_declare_product_IdQuery(), _commonServices.AddParameter(new string[] { }));
                        for (int i = 0; i < model.BonusDeclareProduct.Count; i++)
                        {
                            if (model.BonusDeclareProduct[i].BONUS_DECLARE_ID == 0)
                            {

                                model.BonusDeclareProduct[i].BONUS_DECLARE_ID = declareId;

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Bonus_Declare_ProductQuery(), _commonServices.AddParameter(new string[] { model.BonusDeclareProduct[i].BASE_PRODUCT_ID.ToString(), model.BonusDeclareProduct[i].BONUS_DECLARE_ID.ToString(), model.BONUS_MST_ID.ToString(), model.BonusDeclareProduct[i].BRAND_ID.ToString(), model.BonusDeclareProduct[i].CATEGORY_ID.ToString(), model.ENTERED_BY, model.ENTERED_DATE, model.ENTERED_TERMINAL, model.BonusDeclareProduct[i].GROUP_ID.ToString(), model.BonusDeclareProduct[i].SKU_CODE, model.BonusDeclareProduct[i].SKU_ID.ToString(), model.BonusDeclareProduct[i].STATUS })));
                                declareId++;

                            }
                        }

                        int detail_id = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetNewBonus_Dtl_IdQuery(), _commonServices.AddParameter(new string[] { }));
                        for (int i = 0; i < model.Bonus_Dtls.Count; i++)
                        {
                            if (model.Bonus_Dtls[i].BONUS_DTL_ID == 0)
                            {

                                model.Bonus_Dtls[i].BONUS_DTL_ID = declareId;

                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_AddBonus_DltQuery(), _commonServices.AddParameter(new string[] { model.Bonus_Dtls[i].BONUS_DTL_ID.ToString(), model.BONUS_MST_ID.ToString(), model.Bonus_Dtls[i].BONUS_QTY.ToString(), model.Bonus_Dtls[i].BONUS_SKU_CODE, model.Bonus_Dtls[i].BONUS_SKU_ID.ToString(), model.Bonus_Dtls[i].BONUS_SLAB_TYPE, model.Bonus_Dtls[i].BONUS_TYPE, model.Bonus_Dtls[i].CALCULATION_TYPE, model.Bonus_Dtls[i].DISCOUNT_PCT.ToString(), model.Bonus_Dtls[i].DISCOUNT_VAL.ToString(), model.ENTERED_BY, model.ENTERED_DATE, model.ENTERED_TERMINAL, model.Bonus_Dtls[i].GIFT_ITEM_ID.ToString(), model.Bonus_Dtls[i].GIFT_QTY.ToString(), model.Bonus_Dtls[i].SLAB_QTY.ToString(), model.Bonus_Dtls[i].STATUS })));
                                declareId++;

                            }
                            else
                            {

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
    }
}
