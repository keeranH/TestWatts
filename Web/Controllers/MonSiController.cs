using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{




    public class MonSiController : Controller
    {
        //
        // GET: /MonSi/

        public ActionResult Index()
        {

            return View();
        }

        //
        // GET: /MonSi/Details/5

        public ActionResult Details()
        {

            dynamic dynamicContext = new ExpandoObject();

            dynamicContext.familyLogo = "TopImage.jpg";
            dynamicContext.imageUrl = "MainImage.jpg";
            dynamicContext.kwhValue = "68781";
            dynamicContext.percentageValue = "42";

            //IDictionary<string, object> dict = (IDictionary<string, object>)dynamicContext;
            //dict.Add("Test", dynamicContext);

            ViewBag.test = dynamicContext;
            return View();
        }

        //
        // GET: /MonSi/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /MonSi/Create

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
        // GET: /MonSi/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /MonSi/Edit/5

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
        // GET: /MonSi/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /MonSi/Delete/5

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
