using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Invoice_Dtl
    {
        public int DTL_ID { get; set; }

        public int MST_ID { get; set; }

        public decimal INVOICE_QTY { get; set; }
        public decimal INVOICE_ADJUSTMENT_AMOUNT { get; set; }
        public decimal INVOICE_DISCOUNT_AMOUNT { get; set; }
        public decimal CUSTOMER_PRODUCT_DISC_AMOUNT { get; set; }
        public decimal CUSTOMER_DISC_AMOUNT { get; set; }
        public decimal CUSTOMER_ADD2_DISC_AMOUNT { get; set; }
        public decimal CUSTOMER_ADD1_DISC_AMOUNT { get; set; }
        public decimal CURRENT_STOCK { get; set; }
        public int COMPANY_ID { get; set; }
        public decimal BONUS_PRICE_DISC_AMOUNT { get; set; }


        public decimal LOADING_CHARGE_AMOUNT { get; set; }
        public decimal NET_PRODUCT_AMOUNT { get; set; }
        public decimal PREVIOUS_IMS_QTY { get; set; }

        public decimal TOTAL_AMOUNT { get; set; }
        public decimal SUPPLIMENTARY_TAX { get; set; }
        public decimal SUGGESTED_LIFTING_QTY { get; set; }
        public int SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public decimal VAT_AMOUNT { get; set; }
        public decimal UNIT_VAT { get; set; }
        public decimal UNIT_TP { get; set; }
        public decimal TP_AMOUNT { get; set; }

        public string ENTERED_BY { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string ENTERED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }


        //-----helper Atributes------------
        public int ROW_NO { get; set; }

    }
}
