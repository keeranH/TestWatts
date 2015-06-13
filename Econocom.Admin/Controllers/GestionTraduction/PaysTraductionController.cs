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
    public class PaysTraductionController : Controller
    {
         private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public PaysTraductionController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /AgeDeviceTraduction/
        public ActionResult Index()
        {
            try
            {
                var listePays = _service.GetListePays();
                return View(listePays);
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
        public ActionResult ActualiserContenu(PaysTraductionViewModel paysTraductionViewModel)
        {
            try
            {
                var paysTraduction = new PaysTraduction();
                paysTraduction.InjectFrom(paysTraductionViewModel);
                paysTraduction.Id = paysTraductionViewModel.Id.Value;
                _service.SetPaysTraduction(paysTraduction);
                return Json("<span class=\"succes\">Votre modification a ete prise en compte</span>", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return Json("<span class=\"erreur\">Votre modification a echoue</span>", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifierContenu(int idPays, int idLangue)
        {
            var paysTraductionViewModel = new PaysTraductionViewModel();
            try
            {
                var langues = _service.GetListeLangue();

                var paysTraduction = _service.GetPaysTraduction(idPays, idLangue);
                if (paysTraduction == null)
                {
                    paysTraduction = new PaysTraduction();
                    paysTraduction.PaysId = idPays;
                }
                paysTraductionViewModel.InjectFrom(paysTraduction);
                paysTraductionViewModel.Id = paysTraduction.Id;
                paysTraductionViewModel.LangueId = idLangue;
                paysTraductionViewModel.ListeLangues = langues;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return PartialView("ContenuPartial", paysTraductionViewModel);
        }
    }
}
