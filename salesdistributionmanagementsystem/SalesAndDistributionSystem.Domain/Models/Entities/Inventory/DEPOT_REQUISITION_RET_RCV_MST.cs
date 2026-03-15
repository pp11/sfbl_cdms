using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
 
    public class DEPOT_REQUISITION_RET_RCV_MST
    {
        public int MST_ID { get; set; }
        public string MST_ID_ENCRYPTED { get; set; }
        public string RETURN_UNIT_ID { get; set; }
        public int RETURN_RCV_UNIT_ID { get; set; }
        public string RET_RCV_UNIT_NAME { get; set; }
        public string RET_RCV_NO { get; set; }
        public string RET_RCV_DATE { get; set; }
        public string RETURN_DATE { get; set; }
        public string RETURN_NO { get; set; }
        public string REQUISITION_NO { get; set; }
        public double RETURN_AMOUNT { get; set; }
        public double RET_RCV_AMOUNT { get; set; }

        public string RET_RCV_BY { get; set; }
        public string STATUS { get; set; }
        public int COMPANY_ID { get; set; }

        public string REMARKS { get; set; }
        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string q { get; set; }
        public int ROW_NO { get; set; }
        public List<DEPOT_REQUISITION_RET_RCV_DTL> requisitionRetRcvDtlList { get; set; }
    }
}
