using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Business
{
    public delegate void SendEmailHandler(object o, SendEmailEventArgs e);

    public class SendEmailEventArgs :EventArgs
    {
        public MailMessage mailObj;

        public SendEmailEventArgs(MailMessage mailObj)
        {
           this.mailObj = mailObj;
        }    
    
    }
}
