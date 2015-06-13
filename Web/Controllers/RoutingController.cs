using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data;
using Model;
using System.Data;

namespace Web.Controllers
{
    public class RoutingController : Controller
    {
        private EconocomEntities db = new EconocomEntities();

        //
        // GET: /Routing/

        public ViewResult Index()
        {
            //var routings = db.Routings.Include(r => r.Language).Include(r => r.Page);
            //return View(routings.ToList());
            return View();
        }

        //
        // GET: /Routing/Details/5

        public ViewResult Details(int id)
        {
            Routing routing = db.Routings.Find(id);
            return View(routing);
        }

        //
        // GET: /Routing/Create

        public ActionResult Create()
        {
            ViewBag.language_id = new SelectList(db.Languages, "id", "name");
            ViewBag.page_id = new SelectList(db.Pages, "id", "relative_path");
            return View();
        }

        //
        // POST: /Routing/Create

        [HttpPost]
        public ActionResult Create(Routing routing)
        {
            if (ModelState.IsValid)
            {
                db.Routings.Add(routing);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.language_id = new SelectList(db.Languages, "id", "name", routing.language_id);
            ViewBag.page_id = new SelectList(db.Pages, "id", "relative_path", routing.page_id);
            return View(routing);
        }

        //
        // GET: /Routing/Edit/5

        public ActionResult Edit(int id)
        {
            Routing routing = db.Routings.Find(id);
            ViewBag.language_id = new SelectList(db.Languages, "id", "name", routing.language_id);
            ViewBag.page_id = new SelectList(db.Pages, "id", "relative_path", routing.page_id);
            return View(routing);
        }

        //
        // POST: /Routing/Edit/5

        [HttpPost]
        public ActionResult Edit(Routing routing)
        {
            if (ModelState.IsValid)
            {
                db.Entry(routing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.language_id = new SelectList(db.Languages, "id", "name", routing.language_id);
            ViewBag.page_id = new SelectList(db.Pages, "id", "relative_path", routing.page_id);
            return View(routing);
        }

        //
        // GET: /Routing/Delete/5

        public ActionResult Delete(int id)
        {
            Routing routing = db.Routings.Find(id);
            return View(routing);
        }

        //
        // POST: /Routing/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Routing routing = db.Routings.Find(id);
            db.Routings.Remove(routing);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
