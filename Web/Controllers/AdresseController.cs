using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Models.Benchmark;
using Econocom.Data;

namespace Web.Controllers
{ 
    public class AdresseController : Controller
    {
        private EconocomContext db = new EconocomContext();

        //
        // GET: /Adresse/

        public ViewResult Index()
        {
            var adresses = db.ADRESSEs.Include(a => a.PAY);
            return View(adresses.ToList());
        }

        //
        // GET: /Adresse/Details/5

        public ViewResult Details(int id)
        {
            ADRESSE adresse = db.ADRESSEs.Find(id);
            return View(adresse);
        }

        //
        // GET: /Adresse/Create

        public ActionResult Create()
        {
            ViewBag.IDPAYS = new SelectList(db.PAYS, "IDPAYS", "LIBELLEPAYS");
            return View();
        } 

        //
        // POST: /Adresse/Create

        [HttpPost]
        public ActionResult Create(ADRESSE adresse)
        {
            if (ModelState.IsValid)
            {
                db.ADRESSEs.Add(adresse);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.IDPAYS = new SelectList(db.PAYS, "IDPAYS", "LIBELLEPAYS", adresse.IDPAYS);
            return View(adresse);
        }
        
        //
        // GET: /Adresse/Edit/5
 
        public ActionResult Edit(int id)
        {
            ADRESSE adresse = db.ADRESSEs.Find(id);
            ViewBag.IDPAYS = new SelectList(db.PAYS, "IDPAYS", "LIBELLEPAYS", adresse.IDPAYS);
            return View(adresse);
        }

        //
        // POST: /Adresse/Edit/5

        [HttpPost]
        public ActionResult Edit(ADRESSE adresse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adresse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDPAYS = new SelectList(db.PAYS, "PAYSID", "LIBELLEPAYS", adresse.IDPAYS);
            return View(adresse);
        }

        //
        // GET: /Adresse/Delete/5
 
        public ActionResult Delete(int id)
        {
            ADRESSE adresse = db.ADRESSEs.Find(id);
            return View(adresse);
        }

        //
        // POST: /Adresse/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            ADRESSE adresse = db.ADRESSEs.Find(id);
            db.ADRESSEs.Remove(adresse);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}