using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Invoice_Mst
    {
        public int MST_ID { get; set; }
       
        public string COMPANY_ID { get; set; }
        

        public int DIVISION_ID { get; set; }
        public int DIVISION_EID { get; set; }
        public decimal REGION_ID { get; set; }
        public decimal REGION_EID { get; set; }
        public int AREA_EID { get; set; }
        public int AREA_ID { get; set; }
        public int TERRITORY_ID { get; set; }
        public int MARKET_ID { get; set; }

        public string DELIVERY_DATE { get; set; }
        public int CUSTOMER_ID { get; set; }


       
        public string INVOICE_VERSION { get; set; }
        public int INVOICE_UNIT_ID { get; set; }
        public int INVOICE_TYPE_ID { get; set; }
        public string  INVOICE_NO { get; set; }
        public string INVOICE_STATUS { get; set; }
        public decimal INVOICE_DISCOUNT_AMOUNT { get; set; }
        public string INVOICE_DATE { get; set; }
        public decimal INVOICE_ADJUSTMENT_AMOUNT { get; set; }
        public decimal CUSTOMER_ADD1_DISC_AMOUNT { get; set; }
        public decimal CUSTOMER_ADD2_DISC_AMOUNT { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public decimal CUSTOMER_DISC_AMOUNT { get; set; }
        public string BONUS_CLAIM_NO { get; set; }
        public decimal BONUS_PRICE_DISC_AMOUNT { get; set; }
        public string BONUS_PROCESS_NO { get; set; }
        public decimal CUSTOMER_PRODUCT_DISC_AMOUNT { get; set; }
        public decimal LOADING_CHARGE_AMOUNT { get; set; }
        public decimal PROD_BONUS_PRICE_DISC_AMOUNT { get; set; }
        public string PAYMENT_MODE { get; set; }
        public string ORDER_NO { get; set; }
        public int ORDER_MST_ID { get; set; }
        public string ORDER_DATE { get; set; }
        public decimal ORDER_AMOUNT { get; set; }
        public decimal NET_INVOICE_AMOUNT { get; set; }
        public int TERRITORY_EID { get; set; }
        public decimal TDS_AMOUNT { get; set; }
        public int SR_ID { get; set; }
        public string REPLACE_CLAIM_NO { get; set; }
        public string REMARKS { get; set; }

        public decimal VAT_AMOUNT { get; set; }
        public decimal TP_AMOUNT { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }

        public string ENTERED_BY { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string ENTERED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public List<Invoice_Dtl> invoice_Dtls { get; set; }


        //-----helper Atributes------------
        public int ROW_NO { get; set; }
        public string MST_ID_ENCRYPTED { get; set; }
        public string q { get; set; }

        public string CUSTOMER_NAME { get; set; }
        public string INVOICE_TYPE_NAME { get; set; }

        

    }
}
