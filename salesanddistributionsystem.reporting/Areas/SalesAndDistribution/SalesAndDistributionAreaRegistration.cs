using System.Web.Mvc;

namespace SalesAndDistributionSystem.Reporting.Areas.SalesAndDistribution
{
    public class SalesAndDistributionAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SalesAndDistribution";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SalesAndDistribution_default",
                "SalesAndDistribution/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}