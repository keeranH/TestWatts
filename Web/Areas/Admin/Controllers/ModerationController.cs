using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Interfaces;
using Econocom.Model.ViewModel;
using Model.Interfaces;
using Econocom.Model.Models.Benchmark;
using Econocom.Data;
using Web.Controllers;
using Web.ServiceReference1;


namespace Web.Areas.Admin.Controllers
{
    public class ModerationController : BaseController
    {
        private EconocomContext db = new EconocomContext();
        private Web.ServiceReference1.IEconocomService service;
        public ModerationController()
        {
            service = new EconocomServiceClient();
        }

      
        // GET: /Admin/Moderation/Clients
        public ActionResult Clients()
        {
            try
            {
                var clients = service.GetClientNonModerer();
                return View(clients);
            }
            catch (Exception)
            {
                
                   return RedirectToAction("NotFound", "Error");
            }
        }

        ////
        //// GET: /Client/Edit/5
        public ActionResult Edit(int id)
        {
           try
           {
                ViewBag.IDPAYS = service.GetPays();
                var client = service.GetClientParId(id);
                if (client == null)
                    return RedirectToAction("NotFound", "Error");

       

                return View(client);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Error");
            }
        }

          [HttpPost]
          public ActionResult Edit(String btnstatus, Client model)
        {
           
            if (ModelState.IsValid)
            {
              try
                {
                    var btnStatus = btnstatus.Equals("Accept") ? "Accepté" : "Rejeté";
                    service.ActualiserClient(model, btnStatus);
                    return RedirectToAction("Clients","Moderation");
                }
                catch (Exception e)
                {
                    return RedirectToAction("Index", "Error");
                }                
            }

            if (model == null)
               return RedirectToAction("NotFound", "Error");

            return View(model);
        }                     
    }
}
