using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Models.Benchmark;
using Econocom.Data;
using Econocom.Model.ViewModel;
using Security;
using System.Web.Security;
using Web.ServiceReference1;

namespace Web.Controllers
{
    public class ContactController : Controller
    {

        private EconocomContext db = new EconocomContext();
        private Web.ServiceReference1.IEconocomService service;

        public ContactController()
        {
            service = new EconocomServiceClient();
        }

        //
        // GET: /Contact/

        //public ViewResult Index()
        //{           
        //    return View();
        //}

        ////
        //// GET: /Contact/Details/5

        //public ViewResult Details(int id)
        //{
        //    var contact = service.GetContactById(id);
        //    return View(contact);
        //}

        ////
        //// GET: /Contact/Create

        //public ActionResult Create()
        //{
        //    return View();
        //} 

        ////
        //// POST: /Contact/Create

        //[HttpPost]
        //public ActionResult Create(Contact contact)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        return RedirectToAction("Index");  
        //    }

        //    return View(contact);
        //}

        ////
        //// GET: /Contact/Edit/5

        //public ActionResult Edit(int id)
        //{
        //    var contact = service.GetContactById(id);
        //    return View(contact);
        //}

        ////
        //// POST: /Contact/Edit/5

        //[HttpPost]
        //public ActionResult Edit(Contact contact)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        return RedirectToAction("Index");
        //    }
        //    return View(contact);
        //}

        ////
        //// GET: /Contact/Delete/5

        //public ActionResult Delete(int id)
        //{

        //    return View();
        //}

        ////
        //// POST: /Contact/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(int id)
        //{            

        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{           
        //    base.Dispose(disposing);
        //}

        ////
        //// GET: /Contact/AccountVerification/5

        public ActionResult AccountVerification(Guid id)
        {
            Guid newId = Guid.NewGuid();
            if (id != null)
                newId = (Guid) id;
            string verificationCode = newId.ToString();
            var contact = service.GetContactParVerificationCode(verificationCode);
            if (contact != null)
            {
                var newContact = new RegisterViewModel {Id = newId, ClientId = contact.CLIENT.IDCLIENT};

                var questions = service.GetQuestions();

                ViewBag.Questions = questions.AsQueryable();
                ViewBag.QuestionId = new SelectList(questions.AsEnumerable(), "IDQUESTION", "Label");

                return View(newContact);
            }
            return RedirectToAction("NotFound", "Error"); //change to error page
        }

        ////
        //// POST: /Contact/AccountVerification/5

        [HttpPost]
        public ActionResult AccountVerification(RegisterViewModel enregistrementModel)
        {
            ViewBag.QuestionId = new SelectList(service.GetQuestions().AsEnumerable(), "IDQUESTION", "Label");
            if (ModelState.IsValid)
            {
                try
                {
                   
                    var contact = service.VerificationCompte(enregistrementModel);
                    if (contact != null)
                    {
                        FormsAuthentication.SetAuthCookie(contact.PRENOMCONTACT, false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {

                        return View(enregistrementModel);
                    }

                }
                catch (Exception e)
                {
                    return View(enregistrementModel);
                }


            }
            return View(enregistrementModel);
        }
    }
}