using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Division_Info : Base
    {
        public int DIVISION_ID { get;set;}
        public string DIVISION_NAME { get;set;}
        public string DIVISION_CODE { get; set; }
        public string DIVISION_STATUS { get; set; }
        public string DIVISION_ADDRESS { get; set; }
        public string REMARKS { get; set; }
       

    }
}
