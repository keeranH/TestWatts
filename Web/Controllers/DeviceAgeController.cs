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
    public class DeviceAgeController : Controller
    {
        private EconocomContext db = new EconocomContext();

        //
        // GET: /DeviceAge/

        public ViewResult Index()
        {
            //var deviceages = db.DeviceAges.Include(d => d.Client).Include(d => d.ActivitySector);
            //return View(deviceages.ToList());
            return View();
        }

        //
        // GET: /DeviceAge/Details/5

        public ViewResult Details(int id)
        {
           // DeviceAge deviceage = db.DeviceAges.Find(id);
            return View();
        }

        //
        // GET: /DeviceAge/Create

        public ActionResult Create()
        {
            //ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name");
            //ViewBag.ActivitySectorId = new SelectList(db.Sectors, "Id", "ActivitySegment");
            return View();
        } 

        //
        // POST: /DeviceAge/Create

        [HttpPost]
        public ActionResult Create(AGEDEVICE deviceage)
        {
            if (ModelState.IsValid)
            {
                //db..Add(deviceage);
                //db.SaveChanges();
                //return RedirectToAction("Index");  
            }

            //ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name", deviceage.ClientId);
            //ViewBag.ActivitySectorId = new SelectList(db.Sectors, "Id", "ActivitySegment", deviceage.ActivitySectorId);
            //return View(deviceage);
            return View();
        }
        
        //
        // GET: /DeviceAge/Edit/5
 
        public ActionResult Edit(int id)
        {
            //AGEDEVICE deviceage = db..Find(id);
            //ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name", deviceage.ClientId);
            //ViewBag.ActivitySectorId = new SelectList(db.Sectors, "Id", "ActivitySegment", deviceage.ActivitySectorId);
            return View();
        }

        //
        // POST: /DeviceAge/Edit/5

        [HttpPost]
        public ActionResult Edit(AGEDEVICE deviceage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deviceage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name", deviceage.ClientId);
            //ViewBag.ActivitySectorId = new SelectList(db.Sectors, "Id", "ActivitySegment", deviceage.ActivitySectorId);
            //return View(deviceage);
            return View();
        }

        //
        // GET: /DeviceAge/Delete/5
 
        public ActionResult Delete(int id)
        {
            //AGEDEVICE deviceage = db.DeviceAges.Find(id);
            //return View(deviceage);
            return View();
        }

        //
        // POST: /DeviceAge/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            //AGEDEVICE deviceage = db.DeviceAges.Find(id);
            //db.DeviceAges.Remove(deviceage);
            //db.SaveChanges();
            //return RedirectToAction("Index");
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}