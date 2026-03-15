using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Common
{
    public class Auth
    {
        public int UserId { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Password { get; set; }
        public string UniqueId { get; set; }
        public string UnitName { get; set; }

        public int UnitId { get; set; }
        public string UnitType { get; set; }
        public string UserType { get; set; }

        public int DistributorId { get; set; }
        public string DefaultPage { get; set; }

        public string RoleNames { get; set; }
    }
}
