using SalesAndDistributionSystem.Domain.Models.TableModels.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.Security
{
    public class MenuDistribution
    {
        public List<PermittedModule> PermittedModules { get; set; }
        public List<PermittedMenu> PermittedMenus { get; set; }

      

    }
}
