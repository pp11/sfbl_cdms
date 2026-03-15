using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.Inventory
{
    public class DistributionFilterParameter
    {
        public string DATE_FROM { get; set; } = DateTime.Now.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string DATE_TO { get; set; } = DateTime.Now.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public int DIST_ROUTE_ID { get; set; }
        public int COMPANY_ID { get; set; }
        public string VEHICLE_NO { get; set; }
        public int CUSTOMER_ID { get; set; }
        public int INVOICE_UNIT_ID { get; set; }
    }
}
