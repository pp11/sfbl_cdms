using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAndDistributionSystem.Domain.Common
{
    public class EmailConfiguration
    {
        public string From { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Title { get; set; }
        public string EmailBody { get; set; }
        public string EmailBody_UserName { get; set; }
        public string EmailBody_Password { get; set; }
        public string EmailBody_PageLink { get; set; }


        public List<IFormFile> Attachments { get; set; }
    }
}
