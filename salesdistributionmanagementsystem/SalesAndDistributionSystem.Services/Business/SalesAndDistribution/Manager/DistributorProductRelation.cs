using Glimpse.Core.ClientScript;
using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Common;
using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.SalesAndDistribution
{
    public class DistributorProductRelation : IDistributorProductRelation
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public DistributorProductRelation(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        //-------------------Query Part ---------------------------------------------------

        string LoadData_Master_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO, M.MST_ID,
                                   M.DISTRIBUTOR_PRODUCT_TYPE
                                   FROM DISTRIBUTOR_PRODUCT_MST  M";
        string LoadData_MasterById_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.MST_ID ASC) AS ROW_NO, M.MST_ID,
                                   M.DISTRIBUTOR_PRODUCT_TYPE
                                   FROM DISTRIBUTOR_PRODUCT_MST  M
                                   Where M.MST_ID = :param1";

        string AddOrUpdate_Master_AddQuery() => @"INSERT INTO DISTRIBUTOR_PRODUCT_MST 
                                         (MST_ID, DISTRIBUTOR_PRODUCT_TYPE ) 
                                         VALUES ( :param1, :param2)";
        string AddOrUpdate_Master_UpdateQuery() => @"UPDATE DISTRIBUTOR_PRODUCT_MST SET 
                                           DISTRIBUTOR_PRODUCT_TYPE =  :param2 WHERE MST_ID = :param1";
        string GetDISTRIBUTOR_PRODUCT_MST_IdQuery() => "SELECT NVL(MAX(MST_ID),0) + 1 MST_ID  FROM DISTRIBUTOR_PRODUCT_MST";
        string GetDISTRIBUTOR_PRODUCT_DTL_IdQuery() => "SELECT NVL(MAX(DTL_ID),0) + 1 MST_ID  FROM DISTRIBUTOR_PRODUCT_DTL";


        string LoadData_Detail_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.DTL_ID ASC) AS ROW_NO,M.DTL_ID,
                                          M.MST_ID,   M.SKU_CODE, M.SKU_ID FROM DISTRIBUTOR_PRODUCT_DTL  M  Where M.DTL_ID = :param1";
        string LoadData_DetailByMasterId_Query() => @"SELECT ROW_NUMBER() OVER(ORDER BY M.DTL_ID ASC) AS ROW_NO,M.DTL_ID,
                                          M.MST_ID,   M.SKU_CODE, M.SKU_ID ,UNIT_ID FROM DISTRIBUTOR_PRODUCT_DTL  M  Where M.MST_ID = :param1";

        string AddOrUpdate_Detail_AddQuery() => @"INSERT INTO DISTRIBUTOR_PRODUCT_DTL 
                                         (DTL_ID, MST_ID,SKU_ID, SKU_CODE, UNIT_ID ) 
                                         VALUES ( :param1, :param2, :param3, :param4, :param5 )";
        string AddOrUpdate_Detail_UpdateQuery() => @"UPDATE DISTRIBUTOR_PRODUCT_DTL SET  SKU_ID =  :param1, SKU_CODE = :param2, UNIT_ID = :param3 WHERE DTL_ID = :param4 AND MST_ID=:param5";

        string DeleteDISTRIBUTOR_PRODUCT_DTL_IdQuery() => "DELETE FROM DISTRIBUTOR_PRODUCT_DTL WHERE DTL_ID = :param1 AND MST_ID=:param2";
        //---------- Method Execution Part ------------------------------------------------

        public async Task<string> AddOrUpdate(string db, Distributor_Product_Mst model)
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
                    int detail_id = 0;
                    detail_id = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetDISTRIBUTOR_PRODUCT_DTL_IdQuery(), _commonServices.AddParameter(new string[] { }));

                    if (model.MST_ID == 0)
                    {
                        //-------------Add to master table--------------------
                        model.MST_ID = _commonServices.GetMaximumNumber<int>(_configuration.GetConnectionString(db), GetDISTRIBUTOR_PRODUCT_MST_IdQuery(), _commonServices.AddParameter(new string[] { }));
                        listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Master_AddQuery(), _commonServices.AddParameter(new string[] { model.MST_ID.ToString(), model.DISTRIBUTOR_PRODUCT_TYPE })));

                        //-------------Add to detail table--------------------
                        foreach (var item in model.distributor_Product_Dtls)
                        {
                            string units = "";
                            for(int i=0;i< item.UNIT_ID.Count;i++)
                            {
                                if(i==0)
                                {
                                    units = item.UNIT_ID[i];
                                }
                                else
                                {
                                    units = units + "," +item.UNIT_ID[i];
                                }
                            }
                            listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(), _commonServices.AddParameter(new string[] { detail_id.ToString(), model.MST_ID.ToString(), item.SKU_ID.ToString(), item.SKU_CODE,units })));
                            detail_id++;
                        }
                    }
                    else
                    {
                        Distributor_Product_Mst diivision_Region_Mst =await  LoadDetailData_ByMasterId_List(db, model.MST_ID);
                        //-------------Edit on Master table--------------------
                        listOfQuery.Add(_commonServices.AddQuery( AddOrUpdate_Master_UpdateQuery(), _commonServices.AddParameter(new string[] { model.MST_ID.ToString(), model.DISTRIBUTOR_PRODUCT_TYPE })));

                        foreach (var item in model.distributor_Product_Dtls)
                        {
                            string units = "";
                            if(item.UNIT_ID!=null)
                            {
                                for (int i = 0; i < item.UNIT_ID.Count; i++)
                                {
                                    if (i == 0)
                                    {
                                        units = item.UNIT_ID[i];
                                    }
                                    else
                                    {
                                        units = units + "," + item.UNIT_ID[i];
                                    }
                                }

                            }
                            
                            if (item.DTL_ID == 0)
                            {
                                //-------------Add new row on detail table--------------------
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_AddQuery(), _commonServices.AddParameter(new string[] { detail_id.ToString(), model.MST_ID.ToString(), item.SKU_ID.ToString(), item.SKU_CODE , units })));
                                detail_id++;
                            }
                            else
                            {
                                //-------------Edit on detail table--------------------
                                listOfQuery.Add(_commonServices.AddQuery(AddOrUpdate_Detail_UpdateQuery(), _commonServices.AddParameter(new string[] { item.SKU_ID.ToString(), item.SKU_CODE,units ,item.DTL_ID.ToString(), model.MST_ID.ToString() })));
                            }
                        }
                        foreach (var item in diivision_Region_Mst.distributor_Product_Dtls)
                        {
                            bool status = true;
                            foreach (var updateditem in model.distributor_Product_Dtls)
                            {
                                if (item.DTL_ID == updateditem.DTL_ID)
                                {
                                    status = false;
                                }
                            }
                            if (status)
                            {
                                //-------------Delete row from detail table--------------------

                                listOfQuery.Add(_commonServices.AddQuery(DeleteDISTRIBUTOR_PRODUCT_DTL_IdQuery(), _commonServices.AddParameter(new string[] { item.DTL_ID.ToString(), model.MST_ID.ToString() })));

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
        public async Task<string> LoadData_Master(string db, int Company_Id)
        {
            List<Distributor_Product_Mst> division_Region_Mst_list = new List<Distributor_Product_Mst>();
            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Master_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() }));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                Distributor_Product_Mst division_Region_Mst = new Distributor_Product_Mst();
                division_Region_Mst.MST_ID = Convert.ToInt32(data.Rows[i]["MST_ID"]);
                division_Region_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[i]["MST_ID"].ToString());
                division_Region_Mst.ROW_NO = Convert.ToInt32(data.Rows[i]["ROW_NO"]);
                division_Region_Mst.DISTRIBUTOR_PRODUCT_TYPE = data.Rows[i]["DISTRIBUTOR_PRODUCT_TYPE"].ToString();

                division_Region_Mst_list.Add(division_Region_Mst);
            }
            return JsonSerializer.Serialize(division_Region_Mst_list);
        }

        //public async Task<string> LoadData_Master(string db, int Company_Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Master_Query(), _commonServices.AddParameter(new string[] { Company_Id.ToString() })));
        public async Task<string> LoadData_MasterById(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<string> LoadData_DetailById(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_Detail_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));
        public async Task<string> LoadData_DetailByMasterId(string db, int Id) => _commonServices.DataTableToJSON(await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { Id.ToString() })));

        public async Task<Distributor_Product_Mst> LoadDetailData_ByMasterId_List(string db, int Id)
        {
            Distributor_Product_Mst division_Region_Mst = new Distributor_Product_Mst();

            DataTable data = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_MasterById_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));
            if (data.Rows.Count > 0)
            {
                division_Region_Mst.MST_ID = Convert.ToInt32(data.Rows[0]["MST_ID"]);
                division_Region_Mst.MST_ID_ENCRYPTED = _commonServices.Encrypt(data.Rows[0]["MST_ID"].ToString());
                division_Region_Mst.ROW_NO = Convert.ToInt32(data.Rows[0]["ROW_NO"]);
                division_Region_Mst.DISTRIBUTOR_PRODUCT_TYPE = data.Rows[0]["DISTRIBUTOR_PRODUCT_TYPE"].ToString();

                DataTable dataTable_detail = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), LoadData_DetailByMasterId_Query(), _commonServices.AddParameter(new string[] { Id.ToString() }));

                division_Region_Mst.distributor_Product_Dtls = new List<Distributor_Product_Dtl>();

                for (int i = 0; i < dataTable_detail.Rows.Count; i++)
                {
                    Distributor_Product_Dtl _Region_Dtl = new Distributor_Product_Dtl();

                    _Region_Dtl.DTL_ID = Convert.ToInt32(dataTable_detail.Rows[i]["DTL_ID"]);
                    _Region_Dtl.MST_ID = Convert.ToInt32(dataTable_detail.Rows[i]["MST_ID"]);
                    _Region_Dtl.SKU_ID = Convert.ToString(dataTable_detail.Rows[i]["SKU_ID"]);
                    _Region_Dtl.SKU_CODE = dataTable_detail.Rows[i]["SKU_CODE"].ToString();
                    _Region_Dtl.ROW_NO = Convert.ToInt32(dataTable_detail.Rows[i]["ROW_NO"]);
                    string unit_name_id = Convert.ToString(dataTable_detail.Rows[i]["UNIT_ID"]);
                    if(unit_name_id.Length>0)
                    {
                        _Region_Dtl.UNIT_ID = new List<string>();
                          string[] units = unit_name_id.Split(',');
                          for(int j=0;j<units.Length;j++)
                        {
                            _Region_Dtl.UNIT_ID.Add(units[j]);
                            if (j == 0)
                            {
                                _Region_Dtl.UNIT_NAME = _commonServices.GetMaximumNumber<string>(_configuration.GetConnectionString(db), "Select unit_name FROM COMPANY_INFO where UNIT_ID = :param1", _commonServices.AddParameter(new string[] { units[j] }));
                            }
                            else
                            {
                                _Region_Dtl.UNIT_NAME = _Region_Dtl.UNIT_NAME + " | " +  _commonServices.GetMaximumNumber<string>(_configuration.GetConnectionString(db), "Select unit_name FROM COMPANY_INFO where UNIT_ID = :param1", _commonServices.AddParameter(new string[] { units[j] }));

                            }
                        }
                    }
                    division_Region_Mst.distributor_Product_Dtls.Add(_Region_Dtl);
                }
            }

            return division_Region_Mst;



        }



    }
}
