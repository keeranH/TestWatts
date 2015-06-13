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
    public class UnpublishedContentController : Controller
    {
        private EconocomEntities db = new EconocomEntities();

        //
        // GET: /Unpublished/

        public ViewResult Index()
        {
            //var unpublishedcontents = db.UnpublishedContents.Include(u => u.Language).Include(u => u.Page).Include(u => u.TemplateKey);
           //return View(unpublishedcontents.ToList());
            return View();
        }

        //
        // GET: /Unpublished/Details/5

        public ViewResult Details(int id)
        {
            UnpublishedContent unpublishedcontent = db.UnpublishedContents.Find(id);
            return View(unpublishedcontent);
        }

        //
        // GET: /Unpublished/Create

        public ActionResult Create()
        {
            ViewBag.language_id = new SelectList(db.Languages, "id", "name");
            ViewBag.page_id = new SelectList(db.Pages, "id", "relative_path");
            ViewBag.key_id = new SelectList(db.TemplateKeys, "id", "name");
            return View();
        }

        //
        // POST: /Unpublished/Create

        [HttpPost]
        public ActionResult Create(UnpublishedContent unpublishedcontent)
        {
            if (ModelState.IsValid)
            {
                db.UnpublishedContents.Add(unpublishedcontent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.language_id = new SelectList(db.Languages, "id", "name", unpublishedcontent.language_id);
            ViewBag.page_id = new SelectList(db.Pages, "id", "relative_path", unpublishedcontent.page_id);
            ViewBag.key_id = new SelectList(db.TemplateKeys, "id", "name", unpublishedcontent.key_id);
            return View(unpublishedcontent);
        }

        //
        // GET: /Unpublished/Edit/5

        public ActionResult Edit(int id)
        {
            UnpublishedContent unpublishedcontent = db.UnpublishedContents.Find(id);
            ViewBag.language_id = new SelectList(db.Languages, "id", "name", unpublishedcontent.language_id);
            ViewBag.page_id = new SelectList(db.Pages, "id", "relative_path", unpublishedcontent.page_id);
            ViewBag.key_id = new SelectList(db.TemplateKeys, "id", "name", unpublishedcontent.key_id);
            return View(unpublishedcontent);
        }

        //
        // POST: /Unpublished/Edit/5

        [HttpPost]
        public ActionResult Edit(UnpublishedContent unpublishedcontent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(unpublishedcontent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.language_id = new SelectList(db.Languages, "id", "name", unpublishedcontent.language_id);
            ViewBag.page_id = new SelectList(db.Pages, "id", "relative_path", unpublishedcontent.page_id);
            ViewBag.key_id = new SelectList(db.TemplateKeys, "id", "name", unpublishedcontent.key_id);
            return View(unpublishedcontent);
        }

        //
        // GET: /Unpublished/Delete/5

        public ActionResult Delete(int id)
        {
            UnpublishedContent unpublishedcontent = db.UnpublishedContents.Find(id);
            return View(unpublishedcontent);
        }

        //
        // POST: /Unpublished/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            UnpublishedContent unpublishedcontent = db.UnpublishedContents.Find(id);
            db.UnpublishedContents.Remove(unpublishedcontent);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



    }
}
