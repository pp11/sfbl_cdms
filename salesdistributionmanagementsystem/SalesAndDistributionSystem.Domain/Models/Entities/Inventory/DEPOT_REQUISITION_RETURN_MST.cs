using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class DEPOT_REQUISITION_RETURN_MST
    {
        public int MST_ID { get; set; }
        public string MST_ID_ENCRYPTED { get; set; }
   
        public int RETURN_RCV_UNIT_ID { get; set; }
        public string RETURN_UNIT_ID { get; set; }
        public string RETURN_UNIT_NAME { get; set; }
        public string RETURN_TYPE { get; set; }
        public string RETURN_NO { get; set; }
        public string RETURN_DATE { get; set; }
        public string RECEIVE_NO { get; set; }
        public string RECEIVE_DATE { get; set; }
        public string REQUISITION_NO { get; set; }
        public double RECEIVE_AMOUNT { get; set; }
        public double RETURN_AMOUNT { get; set; }

        public string RETURN_BY { get; set; }
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
        public List<DEPOT_REQUISITION_RETURN_DTL> requisitionReturnDtlList { get; set; }
    }
}
