using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Order_Mst : Base
    {

        public int AREA_ID { get; set; }
        public string BONUS_CLAIM_NO { get; set; }
        public string BONUS_PROCESS_NO { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public int CUSTOMER_ID { get; set; }
        public string DELIVERY_DATE { get; set; }
        public int DIVISION_ID { get; set; }
     
        public string INVOICE_STATUS { get; set; }
        public int INVOICE_UNIT_ID { get; set; }
        public int MARKET_ID { get; set; }
        public decimal ORDER_AMOUNT { get; set; }
        public string ORDER_DATE { get; set; }
        public int ORDER_UNIT_ID { get; set; }
        public string ORDER_TYPE { get; set; }
        public string ORDER_STATUS { get; set; }
        public string ORDER_NO { get; set; }
        public int ORDER_MST_ID { get; set; }
        public string ORDER_ENTRY_TYPE { get; set; }
        public string FINAL_SUBMISSION_STATUS { get; set; }
        public string FINAL_SUBMIT_CONFIRM_STATUS { get; set; }
        public decimal SPA_TOTAL_AMOUNT { get; set; }
        public decimal SPA_NET_AMOUNT { get; set; }
        public decimal SPA_COMMISSION_PCT { get; set; }
        public decimal SPA_COMMISSION_AMOUNT { get; set; }
        public string REPLACE_CLAIM_NO { get; set; }
        public string REFURBISHMENT_CLAIM_NO { get; set; }

        public string REMARKS { get; set; }
        public int REGION_ID { get; set; }
        public string PAYMENT_MODE { get; set; }
        public int TERRITORY_ID { get; set; }
  

        public decimal DISCOUNT_AMOUNT { get; set; }
        public decimal ADJUSTMENT_AMOUNT { get; set; }
        public decimal NET_ORDER_AMOUNT { get; set; }
        public string ORDER_PROCESS_STATUS { get; set; }

        public List<Order_Dtl> Order_Dtls { get; set; }


        //------Helper Attributes--------------
        public string ORDER_MST_ID_ENCRYPTED { get; set; }
        public string NOTIFY_TEXT { get; set; }

        public string CUSTOMER_NAME { get; set; }

        public string MARKET_NAME { get; set; }
        public int SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public List<string> ORDER_NO_LIST { get; set; }
        public string IsOSM { get; set; }

        public string IsDistributor { get; set; }
        public string distributor_product_type { get; set; }
        public string q { get; set; }
        public string INVOICE_TYPE_NAME { get; set; }
        [NotMapped]
        public string INVOICE_NO { get; set; }
        [NotMapped]
        public string INVOICE_DATE { get; set; }

    }
}
