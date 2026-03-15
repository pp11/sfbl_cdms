using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Order_Dtl
    {
        public int COMPANY_ID { get; set; }
       
        public int ORDER_MST_ID { get; set; }
        public int ORDER_DTL_ID { get; set; }
        public int SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public decimal ORDER_AMOUNT { get; set; }
        public int ORDER_QTY { get; set; }
        public decimal? REVISED_ORDER_QTY { get; set; }
        public string REMARKS { get; set; }
        public decimal SPA_UNIT_TP { get; set; }
        public decimal SPA_TOTAL_AMOUNT { get; set; }
        public decimal SPA_REQ_TIME_STOCK { get; set; }
        public decimal SPA_DISCOUNT_VAL_PCT { get; set; }
        public string SPA_DISCOUNT_TYPE { get; set; }
        public decimal SPA_DISCOUNT_AMOUNT { get; set; }
        public decimal SPA_CUST_COM { get; set; }
        public decimal SPA_AMOUNT { get; set; }
        public string STATUS { get; set; }
        public int UNIT_ID { get; set; }
        public decimal UNIT_TP { get; set; }

        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public decimal CUSTOMER_DISC_AMOUNT { get; set; }
        public decimal CUSTOMER_ADD1_DISC_AMOUNT { get; set; }
        public decimal CUSTOMER_ADD2_DISC_AMOUNT { get; set; }
        public decimal CUSTOMER_PRODUCT_DISC_AMOUNT { get; set; }
        public decimal PROD_BONUS_PRICE_DISC_AMOUNT { get; set; }
        public decimal LOADING_CHARGE_AMOUNT { get; set; }
        public decimal INVOICE_ADJUSTMENT_AMOUNT { get; set; }
        public decimal BONUS_PRICE_DISC_AMOUNT { get; set; }
        public decimal NET_ORDER_AMOUNT { get; set; }
        //Helper Data Attributes
        
        public decimal SUGGESTED_QTY { get; set; }
        public decimal TARGET_QTY { get; set; }
        public decimal MAXIMUM_QTY { get; set; }
        public decimal MINIMUM_QTY { get; set; }
        public decimal REMAINING_QTY { get; set; }


        public int ROW_NO { get; set; }


    }
}
