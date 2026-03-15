using SalesAndDistributionSystem.Domain.Models.ViewModels.SalesAndDistributionSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.SalesAndDistribution
{
    public class Return_Mst : Base
    {

        public int MST_ID { get; set; }     
        public int MARKET_ID { get; set; }
        public int CUSTOMER_ID { get; set; }

        public string  INVOICE_NO { get; set; }
        public string INVOICE_DATE { get; set; }
        public decimal CUSTOMER_ADD1_DISC_AMOUNT { get; set; }
        public decimal CUSTOMER_ADD2_DISC_AMOUNT { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public decimal CUSTOMER_DISC_AMOUNT { get; set; }
        public decimal BONUS_PRICE_DISC_AMOUNT { get; set; }
        public decimal CUSTOMER_PRODUCT_DISC_AMOUNT { get; set; }
        public decimal LOADING_CHARGE_AMOUNT { get; set; }
        public decimal PROD_BONUS_PRICE_DISC_AMOUNT { get; set; }
        public decimal NET_INVOICE_AMOUNT { get; set; }
        public decimal TDS_AMOUNT { get; set; }
        public string REMARKS { get; set; }

        public decimal VAT_AMOUNT { get; set; }
        public decimal TP_AMOUNT { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
        public string RETURN_STATUS { get; set; }
        public string RETURN_NO { get; set; }
        public decimal RETURN_DISCOUNT_AMOUNT { get; set; }
        public string RETURN_DATE { get; set; }
        public decimal RETURN_ADJUSTMENT_AMOUNT { get; set; }
        public decimal NET_RETURN_AMOUNT { get; set; }
        public string RETURN_VERSION { get; set; }
        public int RETURN_UNIT_ID { get; set; }
        public string RETURN_TYPE { get; set; }


        public List<Return_Dtl> return_Dtls { get; set; }


        //-----helper Atributes------------
        public string MST_ID_ENCRYPTED { get; set; }
        public string q { get; set; }
        public int SKU_ID { get; set; }
        public string SKU_CODE { get; set; }
        public int RETURN_QTY { get; set; }

        public string CUSTOMER_NAME { get; set; }

        public List<Process_data> Process_data { get; set; }




    }
}
