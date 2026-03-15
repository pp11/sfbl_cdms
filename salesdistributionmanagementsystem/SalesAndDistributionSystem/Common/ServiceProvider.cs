using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using SalesAndDistributionSystem.Domain.Utility;
using System.Collections.Generic;

namespace SalesAndDistributionSystem.Common
{
    public class ServiceProvider
    {
        public string GetConnectionString(string company_Id = "1",string AreaName = "Security",string OpType= "Operation")
        {
            if(AreaName == "SalesAndDistribution")
            {
                if(OpType == "Report")
                {
                    return "Connection";
                }
                return "myconn_SDS";
            }
            return "myconn";
        }
        public bool HasPermission(MenuDistribution menuDist, string Controller, string Action)
        {
            foreach(var item in menuDist.PermittedMenus)
            {
                if(item.ACTION == Action && item.CONTROLLER == Controller && item.LIST_VIEW == Status.Active)
                {
                    return true;
                }

            }
            return false;
        }
       
    }
}
