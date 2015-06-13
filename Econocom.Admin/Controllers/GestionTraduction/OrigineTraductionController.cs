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
    public class OrigineTraductionController : Controller
    {
         private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public OrigineTraductionController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /AgeDeviceTraduction/
        public ActionResult Index()
        {
            try
            {
                var listeOrigine = _service.GetListeOrigine();
                return View(listeOrigine);
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
        public ActionResult ActualiserContenu(OrigineTraductionViewModel origineTraductionViewModel)
        {
            try
            {
                var origineTraduction = new OrigineTraduction();
                origineTraduction.InjectFrom(origineTraductionViewModel);
                origineTraduction.Id = origineTraductionViewModel.Id.Value;
                _service.SetOrigineTraduction(origineTraduction);
                return Json("<span class=\"succes\">Votre modification a ete prise en compte</span>", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return Json("<span class=\"erreur\">Votre modification a echoue</span>", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifierContenu(int idOrigine, int idLangue)
        {
            var origineTraductionViewModel = new OrigineTraductionViewModel();
            try
            {
                var langues = _service.GetListeLangue();

                var origineTraduction = _service.GetOrigineTraduction(idOrigine, idLangue);
                if (origineTraduction == null)
                {
                    origineTraduction = new OrigineTraduction();
                    origineTraduction.OrigineId = idOrigine;
                }
                origineTraductionViewModel.InjectFrom(origineTraduction);
                origineTraductionViewModel.Id = origineTraduction.Id;
                origineTraductionViewModel.LangueId = idLangue;
                origineTraductionViewModel.ListeLangues = langues;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return PartialView("ContenuPartial", origineTraductionViewModel);
        }
    }
}
