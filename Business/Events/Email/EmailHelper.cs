using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Business.Events.Email
{
    public class EmailHelper
    {
        static string mailServer = ConfigurationManager.AppSettings["MailServer"];
        static string mailPort = ConfigurationManager.AppSettings["MailPort"];  
        static string mailUserName = ConfigurationManager.AppSettings["MailUserName"];
        static string mailUserPassword = ConfigurationManager.AppSettings["MailUserPassword"];
        static string AccountVerificationUrl = ConfigurationManager.AppSettings["AccountVerificationUrl"];

        public static void SendRegistrationMailToContacts(ICollection<Contact> contacts)
        {
            var status = false;
            if (contacts == null || contacts.Count == 0)
                return;

            foreach (var contact in contacts)
            {
                try
                {
                    status = SendRegistrationMail(contact);
                    if (status)
                    {
                        //add to nlog
                    }                     
                    else
                    {
                    }
                    //add to nlog
                }
                catch (Exception)
                {
                   //add to nlog
                }
            }
            
        }

        public static bool SendRegistrationMail(Contact contact)
        {
            var mailMessage = BuildMessage(mailUserName, contact.Email, AccountVerificationUrl , contact.CodeVerification,"subject");
            try
            {
                return SendMail(mailMessage);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SendResetPasswordMail(Contact contact)
        {
            var mailMessage = BuildMessage(mailUserName, contact.Email, AccountVerificationUrl, contact.CodeVerification, "Reset Password Link");
            try
            {
                return SendMail(mailMessage);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SendMail(MailMessage message)
        {            
            var smtpPort = 25;
            Int32.TryParse(mailPort, out smtpPort);
            var smtpClient = new SmtpClient(mailServer, smtpPort);
            var credentials = new System.Net.NetworkCredential(mailUserName, mailUserPassword);
            smtpClient.Credentials = credentials;

            try
            {
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                //add to nlog dump
                return false;
            }
            return true;
        }

        public static MailMessage BuildMessage(string from, string to, string body,string verificationCode, string subject)
        {
            body = "<a href=\"" + body +verificationCode +"\">MailLink</a>";
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            var mailMessage = new MailMessage
                {
                    From = new MailAddress(@from),
                    IsBodyHtml = true,
                    Subject = subject,
                    Body = body
                };
            mailMessage.To.Add(new MailAddress(to));
                    
            return mailMessage;
        }
    }
}
