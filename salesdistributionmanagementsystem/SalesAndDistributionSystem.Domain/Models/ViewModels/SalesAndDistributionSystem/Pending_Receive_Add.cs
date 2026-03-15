using SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem
{
    public class Pending_Receive_Add
    {
        public int REQUISITION_UNIT_ID { get; set; }

        public int SKU_ID { get; set; }
        public string ISSUE_NO { get; set; }
    }
}
