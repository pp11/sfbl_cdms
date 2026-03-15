using Glimpse.AspNet.Tab;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using SalesAndDistributionSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SalesAndDistributionSystem.Services.Common
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
             _configuration = configuration;
        }

        public string BodyReader(EmailConfiguration emailConfiguration, string Path)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{Title}", emailConfiguration.Title);

            body = body.Replace("{UserName}", emailConfiguration.ToEmail);
            body = body.Replace("{Password}", emailConfiguration.EmailBody_Password);
            body = body.Replace("{PageLink}", emailConfiguration.EmailBody_PageLink);
            body = body.Replace("{PageBody}", emailConfiguration.EmailBody);
            return body;
        }
        static void Disable_CertificateValidation()
        {
            // Disabling certificate validation can expose you to a man-in-the-middle attack
            // which may allow your encrypted message to be read by an attacker
            // https://stackoverflow.com/a/14907718/740639
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (
                    object s,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors
                ) {
                    return true;
                };
        }



        public async Task SendEmailAsync(EmailConfiguration mailRequest)
        {
            //mailRequest.From = "verify.squareinformatix@gmail.com";
            //mailRequest.Host = "smtp.gmail.com";
            //mailRequest.Port = 587;
            //mailRequest.Password = "nmdyzfgpbhslagmm";
            //mailRequest.UserName = "verify.squareinformatix@gmail.com";

            mailRequest.From = "cdms.admin@squaregroup.com";
            mailRequest.Host = "172.16.128.41";
            mailRequest.Port = 587;
            //mailRequest.Port = 25;

            mailRequest.Password = "CDMSauto123";
            mailRequest.UserName = "cdms.admin@squaregroup.com";
           
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(mailRequest.From);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using SmtpClient smtp = new SmtpClient();
            smtp.Connect(mailRequest.Host, mailRequest.Port, SecureSocketOptions.None);
            Disable_CertificateValidation();

            await smtp.AuthenticateAsync(mailRequest.UserName, mailRequest.Password);

            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
