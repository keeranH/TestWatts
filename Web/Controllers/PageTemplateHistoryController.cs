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
    public class PageTemplateHistoryController : Controller
    {
        private EconocomEntities db = new EconocomEntities();

        //
        // GET: /PageTemplateHistory/

        public ViewResult Index()
        {
            //var pagetemplatehistories = db.PageTemplateHistories.Include(p => p.Page).Include(p => p.Template);
            //return View(pagetemplatehistories.ToList());
            return View();
        }

        //
        // GET: /PageTemplateHistory/Details/5

        public ViewResult Details(int id)
        {
            PageTemplateHistory pagetemplatehistory = db.PageTemplateHistories.Find(id);
            return View(pagetemplatehistory);
        }

        //
        // GET: /PageTemplateHistory/Create

        public ActionResult Create()
        {
            ViewBag.page_id = new SelectList(db.Pages, "id", "relative_path");
            ViewBag.template_id = new SelectList(db.Templates, "id", "name");
            return View();
        }

        //
        // POST: /PageTemplateHistory/Create

        [HttpPost]
        public ActionResult Create(PageTemplateHistory pagetemplatehistory)
        {
            if (ModelState.IsValid)
            {
                db.PageTemplateHistories.Add(pagetemplatehistory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.page_id = new SelectList(db.Pages, "id", "relative_path", pagetemplatehistory.page_id);
            ViewBag.template_id = new SelectList(db.Templates, "id", "name", pagetemplatehistory.template_id);
            return View(pagetemplatehistory);
        }

        //
        // GET: /PageTemplateHistory/Edit/5

        public ActionResult Edit(int id)
        {
            PageTemplateHistory pagetemplatehistory = db.PageTemplateHistories.Find(id);
            ViewBag.page_id = new SelectList(db.Pages, "id", "relative_path", pagetemplatehistory.page_id);
            ViewBag.template_id = new SelectList(db.Templates, "id", "name", pagetemplatehistory.template_id);
            return View(pagetemplatehistory);
        }

        //
        // POST: /PageTemplateHistory/Edit/5

        [HttpPost]
        public ActionResult Edit(PageTemplateHistory pagetemplatehistory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pagetemplatehistory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.page_id = new SelectList(db.Pages, "id", "relative_path", pagetemplatehistory.page_id);
            ViewBag.template_id = new SelectList(db.Templates, "id", "name", pagetemplatehistory.template_id);
            return View(pagetemplatehistory);
        }

        //
        // GET: /PageTemplateHistory/Delete/5

        public ActionResult Delete(int id)
        {
            PageTemplateHistory pagetemplatehistory = db.PageTemplateHistories.Find(id);
            return View(pagetemplatehistory);
        }

        //
        // POST: /PageTemplateHistory/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            PageTemplateHistory pagetemplatehistory = db.PageTemplateHistories.Find(id);
            db.PageTemplateHistories.Remove(pagetemplatehistory);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
