using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.ViewModel;
using Econocom.Web4.Controllers.ApiControllers;

namespace Econocom.Web4.Controllers
{
    public class ModerationController : Controller
    {
        //
        // GET: /Moderation/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Clients()
        {
            try
            {
                var apiModeration = new ModerationApiController();
                //var clients = apiModeration.GetClientNonModerer();
                return View();
            }
            catch (Exception)
            {

                return RedirectToAction("NotFound", "Error");
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                var apiModeration = new ModerationApiController();
                var client = apiModeration.GetClientParId(id);
                if (client == null)
                    return RedirectToAction("NotFound", "Error");

                var apiLogin = new LoginApiController();
                var pays = apiLogin.GetPays();
                ViewBag.PaysIds = new SelectList(pays, "Id", "LibellePays");
                var registrationViewmModel = new RegistrationClientModel();//client.ToRegistrationClientModel();
                return View(registrationViewmModel);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        public ActionResult Edit(String btnstatus, RegistrationClientModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var btnStatus = btnstatus.Equals("Accept") ? "Accepté" : "Rejeté";
                    var apiModeration = new ModerationApiController();
                    apiModeration.ActualiserClient(model.ToClient());
                    return RedirectToAction("Clients", "Moderation");
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
