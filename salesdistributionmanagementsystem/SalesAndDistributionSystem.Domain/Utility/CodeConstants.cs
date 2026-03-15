using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Utility
{
    public static class CodeConstants
    {
        public const string DivisionInfo_CodeConst = "D";
        public const int DivisionInfo_CodeLength = 5;

        public const string RegionInfo_CodeConst = "R";
        public const int RegionInfo_CodeLength = 5;

        public const int Requisition_No_CodeLength = 5;

        public const string AreaInfo_CodeConst = "A";
        public const int AreaInfo_CodeLength = 5;

        public const string MarketInfo_CodeConst = "M";
        public const int MarketInfo_CodeLength = 5;

        public const string TerritoryInfo_CodeConst = "T";
        public const int TerritoryInfo_CodeLength = 5;

        public const string BrandInfo_CodeConst = "B";
        public const int BrandInfo_CodeLength = 5;

        public const string BaseProductInfo_CodeConst = "B";
        public const int BaseProductInfo_CodeLength = 5;

        public const string CategoryInfo_CodeConst = "C";
        public const int CategoryInfo_CodeLength = 5;

        public const string CustomerTypeInfo_CodeConst = "C";
        public const int CustomerTypeInfo_CodeLength = 5;

        public const string GroupInfo_CodeConst = "G";
        public const int GroupInfo_CodeLength = 5;

        public const string InvoiceTypeInfo_CodeConst = "I";
        public const int InvoiceTypeInfo_CodeLength = 5;

        public const string PrimaryProductInfo_CodeConst = "I";
        public const int PrimaryProductInfo_CodeLength = 5;

        public const string ProductTypeInfo_CodeConst = "T";
        public const int ProductTypeInfo_CodeLength = 5;

        public const string ProductInfo_CodeConst = "SKU";
        public const int ProductInfo_CodeLength = 7;
        public const string CustomerInfo_CodeConst = "C";
        public const int CustomerInfo_CodeLength = 6;

        public const string Report_Secret_Key = "STLSDSREPORTING";
  
        public const string Report_URL = "https://localhost:44387/";
        //public const string Report_URL_RELEASE = "http://172.16.242.31:84/";
        public const string Report_URL_RELEASE = "http://103.147.56.105:84/";

        public const string Email_URL = "https://localhost:44305/";
        //public const string Email_URL_RELEASE = "http://172.16.242.31:89/";
        public const string Email_URL_RELEASE = "http://103.147.56.105:89/";

        public const string State_ReadOnly_Key = "dtlxqt";
        public const string State_Editable_Key = "edtxqt";

    }
}
