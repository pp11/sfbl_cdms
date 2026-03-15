using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.TableModels.Company
{
    public class Company_Info
    {
        public int ID { get; set; }
        public int COMPANY_ID { get; set; }
        public string COMPANY_NAME { get; set; }
        public string COMPANY_SHORT_NAME { get; set; }

        public string COMPANY_ADDRESS2 { get; set; }
        public string COMPANY_ADDRESS1 { get; set; }
        public int UNIT_ID { get; set; }
        public string UNIT_NAME { get; set; }
        public string UNIT_SHORT_NAME { get; set; }

        public string UNIT_TYPE { get; set; }
        public string UNIT_ADDRESS1 { get; set; }
        public string UNIT_ADDRESS2 { get; set; }
        public string STATUS { get; set; }
       



    }
}
