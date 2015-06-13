using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Business
{
    public class SendEmailListener
    {
        public void sendMailRegistration(object o, SendEmailEventArgs e)
        {

            SmtpClient smtpClient = new SmtpClient("smtp.astek.mu", 25);
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("aarrah", "astek");
            smtpClient.Credentials = credentials;

            try
            {
                smtpClient.Send(e.mailObj);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }



        }    

    }
}
