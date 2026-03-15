using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Product_Price_Info
    {
        public int COMPANY_ID { get; set; }
        public int UNIT_ID { get; set; }
        public int SKU_ID { get; set; }
        public int PRICE_ID { get; set; }
        public string SKU_CODE { get; set; }
        public string PRICE_EFFECT_DATE { get; set; }
        public string PRICE_ENTRY_DATE { get; set; }
        public decimal EMPLOYEE_PRICE { get; set; }
        public decimal GROSS_PROFIT { get; set; }
        public decimal MRP { get; set; }
        public decimal UNIT_TP { get; set; }
        public decimal UNIT_VAT { get; set; }
        public decimal SPECIAL_PRICE { get; set; }
        public decimal SUPPLIMENTARY_TAX { get; set; }

        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        //Helper Attributes
        public int ROW_NO { get; set; }
    }
}
