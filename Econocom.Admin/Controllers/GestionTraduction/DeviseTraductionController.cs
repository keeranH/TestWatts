using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Admin.Controllers.ServiceApi;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.Traduction;
using Econocom.Model.ViewModel.Traduction;
using NLog;
using Omu.ValueInjecter;

namespace Econocom.Admin.Controllers.GestionTraduction
{
    public class DeviseTraductionController : Controller
    {
         private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public DeviseTraductionController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /DeviseTraduction/
        public ActionResult Index()
        {
            try
            {
                //var listeDevise = _service.GetListeDevise();
                var listeDevise = _service.GetDevises();
                return View(listeDevise);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ActualiserContenu(DeviseTraductionViewModel deviseTraductionViewModel)
        {
            try
            {
                var devise = new DeviseTraduction();
                devise.InjectFrom(deviseTraductionViewModel);
                devise.Id = deviseTraductionViewModel.Id.Value;
                _service.SetDeviseTraduction(devise);
                return Json("<span class=\"succes\">Votre modification a ete prise en compte</span>", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return Json("<span class=\"erreur\">Votre modification a echoue</span>", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifierContenu(int idDevise, int idLangue)
        {
            var deviseTraductionViewModel = new DeviseTraductionViewModel();
            try
            {
                var langues = _service.GetListeLangue();

                var deviseTraduction = _service.GetDeviseTraduction(idDevise, idLangue);
                if (deviseTraduction == null)
                {
                    deviseTraduction = new DeviseTraduction();
                    deviseTraduction.DeviseId = idDevise;
                }
                deviseTraductionViewModel.InjectFrom(deviseTraduction);
                deviseTraductionViewModel.Id = deviseTraduction.Id;
                deviseTraductionViewModel.LangueId = idLangue;
                deviseTraductionViewModel.ListeLangues = langues;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return PartialView("ContenuPartial", deviseTraductionViewModel);
        }
    }
}
