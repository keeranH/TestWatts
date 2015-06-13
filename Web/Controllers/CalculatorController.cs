using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Interfaces;

namespace Web.Controllers
{
    public class CalculatorController : Controller
    {
        private IEconocomService service;
        public CalculatorController(IEconocomService econocomService)
        {
            this.service = econocomService;
        }

        //
        // GET: /Calculator/Index/1

        public ActionResult Index(int id)
        {
            var client = service.GetClientById(id);
            var families = service.GetFamilies(client);
            return PartialView("Hierarchy", families);
        }

    }
}
