using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Interfaces;
using Econocom.Model.Models.Benchmark;
using Econocom.Data;
using Econocom.Model.ViewModel;
using Econocom.Model.Enum;

namespace Web.Controllers
{ 
    public class ClientDetailController : Controller
    {
        private EconocomContext db = new EconocomContext();

         private IEconocomService service;
         public ClientDetailController(IEconocomService econocomService)
        {
            this.service = econocomService;
        }

        //
        // GET: /ClientDetail/

        public ViewResult Index()
        {
        
            //return View(db.ClientDetails.ToList());
            return View();
        }

        //
        // GET: /ClientDetail/Details/5

        public ViewResult Details(int id)
        {
            //DETAILSCLIENT clientdetail = db.ClientDetails.Find(id);
            //return View(clientdetail);
            return View();
        }

        //
        // GET: /ClientDetail/Details/5

        public ViewResult Profile(int id)
        {
            var rTypes = from ReportType r in Enum.GetValues(typeof(ReportType))
                         select new { ID = r, Name = r.ToString() };

            ViewBag.ReportTypes = new SelectList(rTypes, "ID", "Name");
            DETAILSCLIENT clientdetail = service.GetDetailsClient(id);
            return View();
        }

        //
        // GET: /ClientDetail/Create
        
        public ActionResult Create(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(DETAILSCLIENT clientdetail, int id)
        {
            CLIENT client = service.GetClientById(id);
            clientdetail = service.SetDetailsClient(clientdetail);
            //client.ClientDetailId = clientdetail.Id;
            service.UpdateClient(client);
            var clientId = id;
            return RedirectToAction("Profile", new { id = clientId });
        }

        //
        // POST: /ClientDetail/Create

        public ActionResult CreateClientDetails(RegisterViewModel registerDetails)
        {

            DETAILSCLIENT clientdetail = service.GetDetailsClient(registerDetails.ClientId);

           if (clientdetail == null)
           {
               return RedirectToAction("Create", new { id = registerDetails.ClientId});
           }
           else
           {
               return RedirectToAction("Profile", new { id = registerDetails.ClientId });
           }
            
        }
        
        //
        // GET: /ClientDetail/Edit/5
 
        public ActionResult Edit(int id)
        {
           // DETAILSCLIENT clientdetail = db.ClientDetails.Find(id);
            //return View(clientdetail);
            return View();
        }

        //
        // POST: /ClientDetail/Edit/5

        [HttpPost]
        public ActionResult Edit(DETAILSCLIENT clientdetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientdetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clientdetail);
        }

        //
        // GET: /ClientDetail/Delete/5
 
        public ActionResult Delete(int id)
        {
            //DETAILSCLIENT clientdetail = db.ClientDetails.Find(id);
            //return View(clientdetail);
            return View();
        }

        //
        // POST: /ClientDetail/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            //DETAILSCLIENT clientdetail = db.ClientDetails.Find(id);
            //db.ClientDetails.Remove(clientdetail);
            //db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}