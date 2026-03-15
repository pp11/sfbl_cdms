using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class CUSTOMER_WISE_SKU_COMM_ADD_MST
    {
        public CUSTOMER_WISE_SKU_COMM_ADD_MST()
        {
            DETAILS = new HashSet<CUSTOMER_WISE_SKU_COMM_ADD_DTL>();
        }
        public int MST_ID { get; set; }
        public string UNIT_ID { get; set; }
        public string SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public int COMPANY_ID { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }

        public ICollection<CUSTOMER_WISE_SKU_COMM_ADD_DTL> DETAILS { get; set; }

        [NotMapped]
        public decimal SKU_PRICE { get; set; }
    }
}
