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
    public class BancaireController : Controller
    {
        private EconocomContext db = new EconocomContext();

        //
        // GET: /Bancaire/

        public ViewResult Index()
        {
            return View(db.IDENTITEBANCAIREs.ToList());
        }

        //
        // GET: /Bancaire/Details/5

        public ViewResult Details(int id)
        {
            IDENTITEBANCAIRE identitebancaire = db.IDENTITEBANCAIREs.Find(id);
            return View(identitebancaire);
        }

        //
        // GET: /Bancaire/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Bancaire/Create

        [HttpPost]
        public ActionResult Create(IDENTITEBANCAIRE identitebancaire)
        {
            if (ModelState.IsValid)
            {
                db.IDENTITEBANCAIREs.Add(identitebancaire);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(identitebancaire);
        }
        
        //
        // GET: /Bancaire/Edit/5
 
        public ActionResult Edit(int id)
        {
            IDENTITEBANCAIRE identitebancaire = db.IDENTITEBANCAIREs.Find(id);
            return View(identitebancaire);
        }

        //
        // POST: /Bancaire/Edit/5

        [HttpPost]
        public ActionResult Edit(IDENTITEBANCAIRE identitebancaire)
        {
            if (ModelState.IsValid)
            {
                db.Entry(identitebancaire).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(identitebancaire);
        }

        //
        // GET: /Bancaire/Delete/5
 
        public ActionResult Delete(int id)
        {
            IDENTITEBANCAIRE identitebancaire = db.IDENTITEBANCAIREs.Find(id);
            return View(identitebancaire);
        }

        //
        // POST: /Bancaire/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            IDENTITEBANCAIRE identitebancaire = db.IDENTITEBANCAIREs.Find(id);
            db.IDENTITEBANCAIREs.Remove(identitebancaire);
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