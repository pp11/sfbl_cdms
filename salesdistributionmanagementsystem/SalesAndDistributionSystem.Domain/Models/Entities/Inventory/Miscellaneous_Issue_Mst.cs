using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.Entities.Inventory
{
    public class Miscellaneous_Issue_Mst
    {
        public int MST_ID { get; set; }
        public string MST_ID_ENCRYPTED { get; set; }
        public int UNIT_ID { get; set; }
        public string UNIT_NAME { get; set; }
        public string ISSUE_NO { get; set; }
        public string ISSUE_DATE { get; set; }
        public string SUBJECT { get; set; }
        public string RAISED_FROM { get; set; }
        public string ISSUE_BY { get; set; }
        public int COMPANY_ID { get; set; }
        public string STATUS { get; set; }
        public string REMARKS { get; set; }

        public string ENTERED_BY { get; set; }
        public string ENTERED_DATE { get; set; }
        public string ENTERED_TERMINAL { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_TERMINAL { get; set; }
        public string UPDATED_DATE { get; set; }
        public string q { get; set; }
        public int ROW_NO { get; set; }
        public List<Miscellaneous_Issue_Dtl> MiscellaneousIssueDtlList { get; set; }


        //Helper Attribute.....
        public int SKU_ID { get; set; }
        public double ISSUE_AMOUNT { get; set; }

        public static implicit operator Miscellaneous_Issue_Mst(Miscellaneous_Issue_Dtl v)
        {
            throw new NotImplementedException();
        }
    }
}
