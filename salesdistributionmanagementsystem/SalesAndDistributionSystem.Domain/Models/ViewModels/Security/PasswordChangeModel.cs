using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Models.ViewModels.Security
{
    public class PasswordChangeModel
    {
        public string Password { get; set; }
        public string PasswordCopy { get; set; }
        public string User_Name { get; set; }
        public string Path { get; set; }
        public int Company_Id { get; set; }

        public bool RememberMe { get; set; }
        public bool MailCredential { get; set; }
        public string Email { get; set; }

        public string UniqueKey { get; set; }
        public int USER_ID { get; set; }
        public string BaseUrl { get; set; }


    }
}
