using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Models.Benchmark;
using Econocom.Data;
using Econocom.Model.Interfaces;

namespace Web.Controllers
{ 
    public class SupportController : Controller
    {
        private IEconocomService service;
        public SupportController(IEconocomService econocomService)
        {
            this.service = econocomService;
        }

        //
        // GET: /Support/

        public ViewResult Index()
        {
            var supports = service.GetSupports();
            return View(supports.ToList());
        }

        //
        // GET: /Support/Details/5

        public ViewResult Details(int id)
        {
            var support = service.GetSupport(id);
            return View(support);
        }

        //
        // GET: /Support/Create

        public ActionResult Create()
        {
            var countries = service.GetCountries();
            ViewBag.CountryId = new SelectList(countries, "Id", "Name");
            return View();
        } 

        //
        // POST: /Support/Create

        [HttpPost]
        public ActionResult Create(Support support)
        {
            if (ModelState.IsValid)
            {
                support.CreatedDate = DateTime.Now;
                service.SetSupport(support);
                return RedirectToAction("Index");  
            }
            var countries = service.GetCountries();
            ViewBag.CountryId = new SelectList(countries, "Id", "Name", support.CountryId);
            return View(support);
        }        
                
        protected override void Dispose(bool disposing)
        {            
            base.Dispose(disposing);
        }
    }
}