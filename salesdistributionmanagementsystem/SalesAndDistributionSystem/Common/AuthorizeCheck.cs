using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using SalesAndDistributionSystem.Domain.Models.ViewModels.Security;
using System.Text.Json;

namespace SalesAndDistributionSystem.Common
{
    public class AuthorizeCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ServiceProvider provider = new ServiceProvider();
            var discriptor = filterContext.ActionDescriptor.RouteValues.Values;
            string ControllerName = string.Empty;
            string ActionName = string.Empty;
            string Area = string.Empty;

            if (discriptor != null)
            {
                int i = 0;
                foreach (var item in discriptor)
                {
                    if(i == 1)
                    {
                        ActionName = item;

                    }
                    else if(i==2)
                    {
                        ControllerName = item;

                    }
                    else
                    {
                        Area = item;
                    }
                    i++;
                }
                bool Is_Permitted = false;
                if (filterContext.HttpContext.Session.GetString("RolePermission") != null)
                {
                    Is_Permitted = provider.HasPermission(JsonSerializer.Deserialize<MenuDistribution>(filterContext.HttpContext.Session.GetString("RolePermission")), ControllerName, ActionName);
                }
                if (!Is_Permitted)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {    {"Area", ""},
                         { "controller", "Home" },
                         { "action", "PermissionRestricted" }
                     });
                }

            }
        }
    }
}
