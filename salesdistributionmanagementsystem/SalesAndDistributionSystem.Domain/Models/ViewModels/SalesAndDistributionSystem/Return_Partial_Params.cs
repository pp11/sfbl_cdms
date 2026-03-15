using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem
{
    public class Return_Partial_Params : Base
    {
        public string INVOICE_NO { get; set; }
        public List<Process_data> Process_data { get; set; }

    }
}
