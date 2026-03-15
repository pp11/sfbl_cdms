using Microsoft.Extensions.Configuration;
using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Business.Security
{
    public class MenuPermissionManager : IMenuPermissionManager
    {
        private readonly ICommonServices _commonServices;
        private readonly IConfiguration _configuration;
        public MenuPermissionManager(ICommonServices commonServices, IConfiguration configuration)
        {
            _commonServices = commonServices;
            _configuration = configuration;

        }

        string MenuCategoryQuery() => "SELECT  MODULE_ID, MODULE_NAME, ORDER_BY_NO FROM Module_Info WHERE UPPER(STATUS)='ACTIVE' ORDER BY ORDER_BY_NO";
        string MenuQuery() => @"SELECT DISTINCT MC.MENU_ID,
                MC.MENU_NAME,
                MC.ORDER_BY_SLNO,
                MC.HREF,
                MC.AREA,
                MC.CONTROLLER,
                MC.ACTION,
                MC.PARENT_MENU_ID,
                MC.MODULE_ID,
                RU.ADD_PERMISSION,
                RU.LIST_VIEW,
                RU.EDIT_PERMISSION,
                RU.DELETE_PERMISSION,
                RU.DETAIL_VIEW,
                RU.DOWNLOAD_PERMISSION,
                RU.CONFIRM_PERMISSION,
                MC.MENU_SHOW,
                COALESCE(RU.UPDATED_DATE, RU.ENTERED_DATE)  AS DATE_ACTION
  FROM MENU_USER_CONFIGURATION RU
       INNER JOIN MENU_CONFIGURATION MC ON MC.MENU_ID = RU.MENU_ID
 WHERE UPPER (MC.STATUS) = UPPER ('Active')
       AND UPPER (RU.LIST_VIEW)= 'ACTIVE' 
       --OR  RU.ADD_PERMISSION= 'Active' OR RU.EDIT_PERMISSION= 'Active' OR RU.DELETE_PERMISSION= 'Active' OR RU.DETAIL_VIEW= 'Active' OR RU.DOWNLOAD_PERMISSION= 'Active' OR RU.CONFIRM_PERMISSION= 'Active')
       AND MC.COMPANY_ID = :param1
       AND RU.USER_ID = :param2

UNION ALL

SELECT DISTINCT MC.MENU_ID,
                MC.MENU_NAME,
                MC.ORDER_BY_SLNO,
                MC.HREF,
                MC.AREA,
                MC.CONTROLLER,
                MC.ACTION,
                MC.PARENT_MENU_ID,
                MC.MODULE_ID,
                RM.ADD_PERMISSION,
                RM.LIST_VIEW,
                RM.EDIT_PERMISSION,
                RM.DELETE_PERMISSION,
                RM.DETAIL_VIEW,
                RM.DOWNLOAD_PERMISSION,
                RM.CONFIRM_PERMISSION,
                MC.MENU_SHOW,
                COALESCE(RM.UPDATED_DATE, RM.ENTERED_DATE)  AS DATE_ACTION
  FROM ROLE_MENU_CONFIGURATION RM 
       INNER JOIN ROLE_USER_CONFIGURATION RU ON RM.ROLE_ID = RU.ROLE_ID
       INNER JOIN MENU_CONFIGURATION MC ON MC.MENU_ID = RM.MENU_ID
 WHERE UPPER (MC.STATUS) = UPPER ('Active')
       AND UPPER(RM.LIST_VIEW)= 'ACTIVE' 
       --(OR  RM.ADD_PERMISSION= 'Active' OR RM.EDIT_PERMISSION= 'Active' OR RM.DELETE_PERMISSION= 'Active' OR RM.DETAIL_VIEW= 'Active' OR RM.DOWNLOAD_PERMISSION= 'Active' OR RM.CONFIRM_PERMISSION= 'Active')
       AND MC.COMPANY_ID = :param1 
       AND RU.USER_ID = :param2
       AND RM.MENU_ID NOT IN (SELECT MENU_ID FROM MENU_USER_CONFIGURATION WHERE USER_ID = :param2)
       ORDER BY DATE_ACTION DESC";
        string MenuShortQuery() => @"SELECT DISTINCT MC.MENU_ID
                
  FROM MENU_USER_CONFIGURATION RU
       INNER JOIN MENU_CONFIGURATION MC ON MC.MENU_ID = RU.MENU_ID
 WHERE UPPER (MC.STATUS) = UPPER ('Active')
       AND MC.COMPANY_ID = :param1
       AND RU.USER_ID = :param2

UNION ALL

SELECT DISTINCT MC.MENU_ID
  FROM ROLE_MENU_CONFIGURATION RM 
       INNER JOIN ROLE_USER_CONFIGURATION RU ON RM.ROLE_ID = RU.ROLE_ID
       INNER JOIN MENU_CONFIGURATION MC ON MC.MENU_ID = RM.MENU_ID
 WHERE UPPER (MC.STATUS) = UPPER ('Active')
       AND RM.LIST_VIEW = 'Active' 
       AND MC.COMPANY_ID = :param1 
       AND RU.USER_ID = :param2
       AND RM.MENU_ID NOT IN (SELECT MENU_ID FROM MENU_USER_CONFIGURATION WHERE USER_ID = :param2)";
        string defaultPageQuery() => "Select M.HREF  from USER_DEFAULT_PAGE d Left outer Join Menu_Configuration m on m.Menu_Id = D.MENU_ID Where D.USER_ID = :param1";

        string LoadPermittedMenuQuery() => "select  MENU_ID, MENU_NAME,ORDER_BY_SLNO, HREF,AREA, CONTROLLER, ACTION, PARENT_MENU_ID , MODULE_ID  from Menu_Configuration Where COMPANY_ID = :param1";

        string SearchableMenuLoadQuery() => @"Select distinct  MENU_ID, MENU_NAME,ORDER_BY_SLNO, HREF,AREA, CONTROLLER, ACTION, PARENT_MENU_ID ,
 MODULE_ID , ADD_PERMISSION, LIST_VIEW, EDIT_PERMISSION, DELETE_PERMISSION, DETAIL_VIEW, DOWNLOAD_PERMISSION from 
(Select distinct MC.MENU_ID, MC.MENU_NAME,MC.ORDER_BY_SLNO, MC.HREF,MC.AREA, MC.CONTROLLER, MC.ACTION, MC.PARENT_MENU_ID ,
 MC.MODULE_ID , RM.ADD_PERMISSION, RM.LIST_VIEW, RM.EDIT_PERMISSION, RM.DELETE_PERMISSION, RM.DETAIL_VIEW, RM.DOWNLOAD_PERMISSION
from ROLE_INFO R 
INNER JOIN ROLE_MENU_CONFIGURATION RM on RM.ROLE_ID = R.ROLE_ID
INNER JOIN ROLE_USER_CONFIGURATION RU on R.ROLE_ID = RU.ROLE_ID
INNER JOIN Menu_Configuration MC on MC.MENU_ID = RM.MENU_ID 
 Where MC.STATUS ='Active' AND RM.LIST_VIEW = 'Active'  AND RU.COMPANY_ID = :param1 AND  upper(MC.MENU_NAME) Like '%' || upper(:param2) || '%' AND MC.MENU_ID NOT IN (Select T.PARENT_MENU_ID From MENU_CONFIGURATION T) AND RU.USER_ID = :param3 
 UNION ALL
 Select distinct MC.MENU_ID, MC.MENU_NAME,MC.ORDER_BY_SLNO, MC.HREF,MC.AREA, MC.CONTROLLER, MC.ACTION, MC.PARENT_MENU_ID ,
 MC.MODULE_ID , RU.ADD_PERMISSION, RU.LIST_VIEW, RU.EDIT_PERMISSION, RU.DELETE_PERMISSION, RU.DETAIL_VIEW, RU.DOWNLOAD_PERMISSION
from  MENU_USER_CONFIGURATION RU
INNER JOIN Menu_Configuration MC on MC.MENU_ID = RU.MENU_ID 
 Where MC.STATUS ='Active' AND RU.LIST_VIEW = 'Active' AND RU.COMPANY_ID = :param1 AND  upper(MC.MENU_NAME) Like '%' || upper(:param2) || '%' AND MC.MENU_ID NOT IN (Select T.PARENT_MENU_ID From MENU_CONFIGURATION T) AND RU.USER_ID = :param3 ) x";

        public string SearchableMenuLoad(string db,string comp_id,string User_Id,string menu_name)
        {
            DataTable table = _commonServices.GetDataTable(_configuration.GetConnectionString(db), SearchableMenuLoadQuery(), _commonServices.AddParameter(new string[] { comp_id, menu_name, User_Id }));

            return _commonServices.DataTableToJSON(table);
        }

        public async Task<MenuDistribution> LoadPermittedMenuByUserId(string db,int id, int companyId)
        {
            try
            {
                List<PermittedMenu> allMenes = await LoadLoadPermittedMenus(db, companyId);
                
                DataTable MenuCategoryData = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), MenuCategoryQuery(), new Dictionary<string, string>());
                DataTable MenuData = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db), MenuQuery(), _commonServices.AddParameter(new string[] { companyId.ToString() , id.ToString()}));
                List<PermittedMenu> permittedMenus =_commonServices.ConvertToList<PermittedMenu>(MenuData);
                List<PermittedModule> MenuCategories = new List<PermittedModule>();
                
                for (int i = 0; i < MenuCategoryData.Rows.Count; i++)
                {
                    PermittedModule menuCategory = new PermittedModule
                    {
                        MODULE_ID = Convert.ToInt32(MenuCategoryData.Rows[i]["MODULE_ID"]),
                        ORDER_BY_NO = Convert.ToInt32(MenuCategoryData.Rows[i]["ORDER_BY_NO"].ToString()),
                        MODULE_NAME = (MenuCategoryData.Rows[i]["MODULE_NAME"].ToString())
                    };
                    MenuCategories.Add(menuCategory);
                }
                List<PermittedMenu> MenuMasters = GetPermittedMenus(permittedMenus, allMenes);
                MenuDistribution menuDistribution = new MenuDistribution
                {
                    PermittedModules = MenuCategories,
                    PermittedMenus = MenuMasters
                };
                return menuDistribution;

            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
        public List<PermittedMenu> GetPermittedMenus(List<PermittedMenu> selectedMenuIds, List<PermittedMenu> menuConfigurations)
        {
            var permittedMenus = new List<PermittedMenu>();

            foreach (var item in selectedMenuIds)
            {
                var menu = selectedMenuIds.FirstOrDefault(m => m.MENU_ID == item.MENU_ID);
                if (menu != null && !permittedMenus.Any(m => m.MENU_ID == menu.MENU_ID))
                {
                    permittedMenus.Add(menu);
                    AddParentMenus(menu.PARENT_MENU_ID, permittedMenus, menuConfigurations);
                }
            }

            return permittedMenus;
        }

        private void AddParentMenus(int parentId, List<PermittedMenu> permittedMenus, List<PermittedMenu> menuConfigurations)
        {
            if (parentId == 0)
                return;

            var parentMenu = menuConfigurations.FirstOrDefault(m => m.MENU_ID == parentId);
            if (parentMenu != null && !permittedMenus.Any(m => m.MENU_ID == parentMenu.MENU_ID))
            {
                permittedMenus.Add(parentMenu);
                AddParentMenus(parentMenu.PARENT_MENU_ID, permittedMenus, menuConfigurations);
            }
        }
        //im//
        public async Task<List<PermittedMenu>> LoadLoadPermittedMenus(string db, int CompanyId)
        {
            DataTable MenuData = await _commonServices.GetDataTableAsyn(_configuration.GetConnectionString(db),
                @"SELECT  MENU_ID, MENU_NAME,ORDER_BY_SLNO, HREF,AREA, CONTROLLER, ACTION, PARENT_MENU_ID , MODULE_ID,MENU_SHOW  FROM MENU_CONFIGURATION WHERE COMPANY_ID = :param1 AND UPPER (STATUS) = 'ACTIVE'",
                _commonServices.AddParameter(new string[] { CompanyId.ToString() }));
            List<PermittedMenu> MenuMasters = new List<PermittedMenu>();
            for (int i = 0; i < MenuData.Rows.Count; i++)
            {
                PermittedMenu menuMaster = new PermittedMenu
                {
                    MENU_ID = Convert.ToInt32(MenuData.Rows[i]["MENU_ID"]),
                    ORDER_BY_SLNO = Convert.ToInt32(MenuData.Rows[i]["ORDER_BY_SLNO"].ToString()),
                    MENU_NAME = (MenuData.Rows[i]["MENU_NAME"].ToString()),
                    HREF = (MenuData.Rows[i]["HREF"].ToString()),
                    AREA = (MenuData.Rows[i]["AREA"].ToString()),
                    CONTROLLER = (MenuData.Rows[i]["CONTROLLER"].ToString()),
                    ACTION = (MenuData.Rows[i]["ACTION"].ToString()),
                    MODULE_ID = Convert.ToInt32(MenuData.Rows[i]["MODULE_ID"].ToString()),
                    PARENT_MENU_ID = Convert.ToInt32(MenuData.Rows[i]["PARENT_MENU_ID"]),
                    MENU_SHOW = (MenuData.Rows[i]["MENU_SHOW"].ToString())

                };
               
                MenuMasters.Add(menuMaster);
            }
            return MenuMasters;
        }
        public string LoadUserDefaultPageById(string db, int User_Id)
        {
            DataTable MenuCategoryData =  _commonServices.GetDataTable(_configuration.GetConnectionString(db), defaultPageQuery(), _commonServices.AddParameter(new string[] { User_Id.ToString() }));
           if(MenuCategoryData!=null && MenuCategoryData.Rows.Count>0)
            {
                return MenuCategoryData.Rows[0]["HREF"].ToString();
            }
            else
            {
                return null;
            }
             
        }
      

    }
}
