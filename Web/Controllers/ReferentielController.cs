using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Interfaces;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Web.Models.Client;
using Web.ServiceReference1;
using AdresseViewModel = Web.Models.AdresseViewModel;

namespace Web.Controllers
{
    public class ReferentielController : Controller
    {

        private Web.ServiceReference1.IEconocomService service;
        public ReferentielController()
        {
            service = new EconocomServiceClient();
        }

        //
        // GET: /Generic/

        public ActionResult Test()
        {
            object dynamicObject = null;//service.ListeOrigines();

            return View(dynamicObject);
        }


        //
        // GET: /Generic/

        public ActionResult Index()
        {
            var liste = service.GetPeople();
            //var liste1 = service.ListePays();
            var contact = new CONTACT();
            contact.EMAIL = "rk@gg.cmo";
            contact.MOTPASSE = "asd";
            try
            {
                var listec = service.GetContact(contact);
            }
            catch (Exception e)
            {
                var exc = e;
            }
            //try
            //{
            //    liste = service.GetPeopleWithOutCicularReferencing();
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}

            // var liste = service.ListePays();
           // dynamic dynamicObject1 = service.ListeAdresse();
            //dynamic d3 = service.GetTests();
            return View(liste);
        }
       
       
        //
        // GET: /Generic/Edit/5

        public ActionResult Edit(int id)
        {
            //dynamic dynamicObject = service.ListeTypeObjet().First();
            //object x = ViewData.Model;
            //return View(dynamicObject);

            //var pays = new PAYS();
            //pays.IDPAYS = 1;
            //pays.LIBELLEPAYS = "pays mu";

            //var adresse = new ADRESSE();
            //adresse.IDADRESSE = 1;
            //adresse.CODEPOSTAL = "230";
            //adresse.IDPAYS = 1;
            //adresse.PAY = pays;

            //dynamic dynamicObject = adresse;      

            var add = new AdresseViewModel();
            var listePays = new List<PAYS> {
                    new PAYS(){ IDPAYS = 1, LIBELLEPAYS = "Furrytail" },
                    new PAYS(){ IDPAYS = 2, LIBELLEPAYS = "Peaches" }
            };

            var listeStates = new List<PAYS> {
                    new PAYS(){ IDPAYS = 1, LIBELLEPAYS = "Furrytail" },
                    new PAYS(){ IDPAYS = 2, LIBELLEPAYS = "Peaches" }
            };

            add.CountryCollection = new SelectList(listePays, "IDPAYS", "LIBELLEPAYS");
            add.StateCollection = new SelectList(listeStates, "IDPAYS", "LIBELLEPAYS");

            var client = new ClientViewModel();
            client.ClientStateCollection = new SelectList(listeStates, "IDPAYS", "LIBELLEPAYS");
            client.AdresseViewModel = add;

            var ib = new IdentiteBancaireViewModel();
            ib.Nom = "new";
            client.IdentiteBancaireViewModel = ib;

            var contacts = new List<ContactViewModel>();
            client.ContactViewModels = contacts;
            //dynamic dynamicObject = add; 
            dynamic dynamicObject = client; 

            return View(dynamicObject);
        }

        //
        // POST: /Generic/Edit/5
        static IList CreateGenericList(Type typeInList)
        {
            var genericListType = typeof(List<>).MakeGenericType(new[] { typeInList });
            return (IList)Activator.CreateInstance(genericListType);
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                var objectType = collection.GetValue("objectType");            
                var c = objectType.AttemptedValue.Split(',').Take(2);
                var gen3 = CreateGenericList(Type.GetType(c.ElementAt(0)+","+c.ElementAt(1)));

                Type type = gen3.GetType().GetGenericArguments()[0];
                var model = Activator.CreateInstance(type);

                var binder = Binders.GetBinder(type);

                var bindingContext = new ModelBindingContext()
                {
                    ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, type),
                    ModelState = ModelState,
                    ValueProvider = collection
                };

                binder.BindModel(ControllerContext, bindingContext);
                TryUpdateModel(model);
                
                dynamic model1 = model;
                
                //service.SetObject(model1);
              
                return RedirectToAction("Index");               
            }
            catch
            {
                return View();
            }
        }        
    }
}
