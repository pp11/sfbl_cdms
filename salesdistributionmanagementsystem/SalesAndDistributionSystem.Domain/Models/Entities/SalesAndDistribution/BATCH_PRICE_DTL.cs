using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class BATCH_PRICE_DTL
    {
        public int DTL_ID { get; set; }
        public string BATCH_NO { get; set; }
        public int BATCH_ID { get; set; }
        public decimal MRP { get; set; }
        public decimal UNIT_TP { get; set; }
        public int UNIT_ID { get; set; }

        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }

        //helper property
        //public decimal PASSED_QTY { get; set; }
        //public int SKU_ID { get; set; }
        //public string SKU_CODE { get; set; }
        //public decimal SPECIAL_PRICE { get; set; }
        //public decimal SUPPLIMENTARY_TAX { get; set; }
        //public decimal UNIT_VAT { get; set; }
        //public decimal EMPLOYEE_PRICE { get; set; }
        //public decimal GROSS_PROFIT { get; set; }
        //public int COMPANY_ID { get; set; }
        //public int MST_ID { get; set; }
        public string UNIT_NAME { get; set; }

    }

}
