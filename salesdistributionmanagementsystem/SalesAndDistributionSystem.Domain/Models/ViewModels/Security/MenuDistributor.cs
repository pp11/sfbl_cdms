using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.Security
{
    public class MenuDistributor
    {
        public List<PermittedModule> PermittedModules { get; set; }
        public List<PermittedMenu> PermittedMenusPart1 { get; set; }
        public List<PermittedMenu> PermittedMenusPart2 { get; set; }
        public List<PermittedMenu> PermittedMenusPart3 { get; set; }
        public List<PermittedMenu> PermittedMenusPart4 { get; set; }

    }
}
