using Microsoft.AspNetCore.Mvc;
using SalesAndDistributionSystem.Common;

namespace SalesAndDistributionSystem.Areas.Sales.Controllers
{

    [Area("SalesAndDistribution")]
    public class SalesController : Controller
    {
        public IActionResult List()
        {
            return View();
        }
        public IActionResult Add()
        {
            return View();
        }
    }
}
