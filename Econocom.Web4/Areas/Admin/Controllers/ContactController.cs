using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel.CMS;
using Econocom.Web4.Controllers;
using Econocom.Web4.Controllers.ApiControllers;

namespace Econocom.Web4.Areas.Admin.Controllers
{
    public class ContactController : BaseController
    {
        private ServiceApiController service;

        public ContactController()
        {
            service = new ServiceApiController();
        }

        //
        // GET: /Admin/Contact/

        public ActionResult Index()
        {
            var client = service.GetClientParNom("Econocom");

            var contacts = service.GetContactParNomClient("Econocom");

            var contactViewModel = new ContactViewModel();

            if (contacts.Count > 0)
            {
                contactViewModel.Contacts = contacts.ToList();
            }
            else
            {
                var contact1 = new Contact();
                var typeContact1 = service.GetTypeContactParLibelle("Commercial");
                contact1.ClientId = client.Id;
                contact1.TypeContactId = typeContact1.Id;
                Contact c1 = service.SetContact(contact1);

                var contact2 = new Contact();
                var typeContact2 = service.GetTypeContactParLibelle("Web Master");
                contact2.ClientId = client.Id;
                contact2.TypeContactId = typeContact2.Id;
                Contact c2 = service.SetContact(contact2);

                var contactListe = new List<Contact> { c1, c2 };
                contactViewModel.Contacts = contactListe;
            }

            return View(contactViewModel);
        }

        [HttpPost]
        public ActionResult Index(ContactViewModel contactViewModel)
        {
           try
           {
               service.MAJContact(contactViewModel);
           }
           catch (Exception e)
           {
               throw e;
           }
            return null;
        }
        

    }
}
