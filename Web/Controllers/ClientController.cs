using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Interfaces;
using Econocom.Model.Models.Benchmark;
using Econocom.Data;
using Econocom.Model.ViewModel;
using Econocom.Resource.Contact;
using Web.ServiceReference1;


namespace Web.Controllers
{ 
    public class ClientController : BaseController
    {
        private EconocomContext db = new EconocomContext();
        private Web.ServiceReference1.IEconocomService service;
        public ClientController()
        {
            service = new EconocomServiceClient();
        }
        
        public ActionResult Registration()
        {
            try
            {
                ViewBag.IDPAYS = service.GetPays();
                var model = new RegistrationClientModel {Type = "Full", Contacts = new List<ContactViewModel>()};
                var contact = new ContactViewModel();
                contact.id = Guid.NewGuid().ToString();
                model.Contacts.Add(contact);

                return View(model);
            }
            catch (Exception)
            {
                 return RedirectToAction("NotFound", "Error"); //change to error page
             
            }
        }

       private RegistrationClientModel ModeBouchon()
        {
            try
            {
                var adresse = new AdresseViewModel();
                adresse.ADRESSE1 = "ADRESSE1";
                adresse.ADRESSE2 = "ADRESSE2";
                adresse.ADRESSE3 = "ADRESSE3";
                adresse.CODEPOSTAL = "CODEPOSTAL";
                adresse.REGION = "REGION";
                adresse.VILLE = "VILLE";
                adresse.IDPAYS = 1;
              
                var identiteBancaire = new IDENTITEBANCAIRE();
                identiteBancaire.BIC = "BIC";
                identiteBancaire.CLERIB = "CLERIB";
                identiteBancaire.CODEBANQUE = "CODEBANQUE";
                identiteBancaire.GUICHET = "GUICHET";
                identiteBancaire.IBAN = "odel.IdentiteBancaire.IBAN";
                identiteBancaire.IDIDENTITEBANCAIRE = 123458999;
                identiteBancaire.NUMCOMPTE = "NUMCOMPTE";


                var registrationClientModel = new RegistrationClientModel();
                registrationClientModel.Nom = "Nom All";
                registrationClientModel.IdentificationNational = "12435efwg";
                registrationClientModel.Group = "test";
                registrationClientModel.TVAIntraComm = "test";
                registrationClientModel.CODENAF = "CODENAF";
                registrationClientModel.RaisonSociale = "RaisonSociale";
                registrationClientModel.Adresse = adresse;
                registrationClientModel.IdentiteBancaire = identiteBancaire;

                var contact = new ContactViewModel();
                contact.NOMCONTACT = "test";
                contact.NUMEROGSM = "1233555";
                contact.PRENOMCONTACT = "test";
                contact.NUMEROPHONE = "7894565";
                contact.FONCTION = "CEO";
                
                contact.EMAIL = "aarrah@astek.mu";
                
                var contacts = new List<ContactViewModel> {contact};

                registrationClientModel.Contacts = contacts;


                return registrationClientModel;
            }
            catch (Exception e)
            {
                
                throw e;
            }

            
        }
        
        
        [HttpPost]
        public ActionResult Registration(RegistrationClientModel model)
        {
            try
            {
                 ViewBag.IDPAYS = service.GetPays();
                /* var modelBouchon = ModeBouchon();
                 var client = modelBouchon.ToClient();*/
                // var isSave = service.SauveGardeClient(client);
                var client = model.ToClient();
                var isSave = service.SauveGardeClient(client);
                if (!isSave)
                {
                    return View(model);
                }


            }
            catch (FaultException fe)
            {
               ModelState.AddModelError(fe.Message,fe.Code.Name);
               return View(model);
            }
            catch (Exception e)
            {
                return View(model);
            }
           
            return RedirectToAction("ApresRegistration", "Client");
        } 

      

        public ActionResult ApresRegistration()
        {
            return View();
        }

        public ActionResult ContactRow()
        {
            return PartialView("ContactEditor");
        }

      private bool ValiderModel(CLIENT client)
        {
           return ModelState.IsValid;
        }



        
    /*   private bool ValidateCompleteModel(ClientRegistrationModel model)
       {
           var validModel = ValidateModel(model);
           if (string.IsNullOrEmpty(model.RIB))
           {
               ModelState.AddModelError("RIB", "Please enter a vali RIB");
           }

           return ModelState.IsValid && validModel;
       }*/
      /* public ActionResult Demo()
       {
           var countries = service.GetCountries();
           ViewBag.CountryId = new SelectList(countries, "Id", "Name");
           var model = new ClientRegistrationModel();
           model.Type = "Demo";

           model.Contacts = new List<Contact>();
           var contact = new Contact();
           model.Contacts.Add(contact);
           return View(model);
       }

        
        POST: /Client/Create

       [HttpPost]
       public ActionResult Demo(ClientRegistrationModel model)
       {
           var client = new Client();
           var validModel = ValidateModel(model);
           if (validModel)
           {
               client.Name = model.CompanyName;
               client.Address.PostalCode = model.PostalCode;
               client.ClientDetail.ClientCode = model.RaisonSociale;
               client.Address.Address1 = model.Address1;
               client.Address.Address2 = model.Address2;
               client.Address.CountryId = model.CountryId;

               if (!string.IsNullOrEmpty(model.Groupe))
               {                  
                   client.Group = model.Groupe;
               }
               client.Contacts = new List<Contact>();
               client.Contacts = model.Contacts;
               service.SetClient(client);
               return RedirectToAction("RegistrationSuccess", new { id = client.Id });
           }

           var countries = service.GetCountries();
           ViewBag.CountryId = new SelectList(countries, "Id", "Name", client.Address.CountryId);
           return View(model);
       }
        
        //POST: /Client/Create

       [HttpPost]
       public ActionResult Create(Client client)
       {
           if (ModelState.IsValid)
           {
               service.SetClient(client);
               return RedirectToAction("RegistrationSuccess", new { id = client.Id });  
           }

           var countries = service.GetCountries();
           ViewBag.CountryId = new SelectList(countries, "Id", "Name", client.Address.CountryId);
           return View(client);
       }
        
        
        //GET: /Client/Edit/5
 
       public ActionResult Edit(int id)
       {
           var client = service.GetClient(id);
           var countries = service.GetCountries();
           ViewBag.CountryId = new SelectList(countries, "Id", "Name", client.Address.CountryId);
           return View(client);
       }

        
       // GET: /Client/RegistrationComplete/5
       public ActionResult RegistrationComplete(int id)
       {
           var client = service.GetClient(id);
           return View(client);
       }

        
      //  GET: /Client/RegistrationSuccess/5
       public ActionResult RegistrationSuccess(int id)
       {
           var client = service.GetClient(id);
           return View(client);
       }

        
      //  POST: /Client/Edit/5

       [HttpPost]
       public ActionResult Edit(Client client)
       {
           if (ModelState.IsValid)
           {
               service.UpdateClient(client);
               return RedirectToAction("Profile", "ClientDetail");
           }
           var countries = service.GetCountries();
           ViewBag.CountryId = new SelectList(countries, "Id", "Name", client.Address.CountryId);
           return View(client);
       }

        
       // GET: /Client/Delete/5
 
       public ActionResult Delete(int id)
       {
           Client client = db.Clients.Find(id);
           return View(client);
       }

        
      //  POST: /Client/Delete/5

       [HttpPost, ActionName("Delete")]
       public ActionResult DeleteConfirmed(int id)
       {            
           Client client = db.Clients.Find(id);
           db.Clients.Remove(client);
           db.SaveChanges();
           return RedirectToAction("Index");
       }

       protected override void Dispose(bool disposing)
       {
           db.Dispose();
           base.Dispose(disposing);
       }*/
    }
}