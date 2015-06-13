using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contact = Econocom.Model.Models.Benchmark.Contact;

namespace Econocom.Model.ViewModel.CMS
{
    public class ContactsViewModel
    {
        public virtual List<Contact> Contacts { get; set; }
    }
}
