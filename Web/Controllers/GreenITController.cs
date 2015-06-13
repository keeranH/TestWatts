using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class GreenITController : Controller
    {
        //
        // GET: /GreenIT/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /GreenIT/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /GreenIT/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /GreenIT/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /GreenIT/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /GreenIT/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /GreenIT/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /GreenIT/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
