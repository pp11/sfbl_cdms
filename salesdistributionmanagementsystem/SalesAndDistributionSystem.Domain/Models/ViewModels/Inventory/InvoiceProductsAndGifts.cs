using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.Inventory
{
    public class InvoiceProductsAndGifts
    {
      public  DataSet Products { get; set; }
      public DataSet Gifts { get; set; }
      public List<string> Invoices { get; set; }
      public string ProductList { get; set; }
      public string GiftList { get; set; }

    }
}
