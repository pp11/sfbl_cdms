using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem
{
    public class ExistingSKURestrictParams 
    {
        public int cust_id { get; set; }
        public string sku_id { get; set; } 
        public string start_date { get; set; }
        public List<string> cust_ids { get; set; }

    }
}
