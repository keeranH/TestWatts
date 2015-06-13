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
    public class PolitiqueTraductionController : Controller
    {
         private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public PolitiqueTraductionController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /AgeDeviceTraduction/
        public ActionResult Index()
        {
            try
            {
                var listePolitique = _service.GetListePolitique();
                return View(listePolitique);
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
        public ActionResult ActualiserContenu(PolitiqueTraductionViewModel politiqueTraductionViewModel)
        {
            try
            {
                var politiqueTraduction = new PolitiqueTraduction();
                politiqueTraduction.InjectFrom(politiqueTraductionViewModel);
                politiqueTraduction.Id = politiqueTraductionViewModel.Id.Value;
                _service.SetPolitiqueTraduction(politiqueTraduction);
                return Json("<span class=\"succes\">Votre modification a ete prise en compte</span>", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return Json("<span class=\"erreur\">Votre modification a echoue</span>", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifierContenu(int idPolitique, int idLangue)
        {
            var politiqueTraductionViewModel = new PolitiqueTraductionViewModel();
            try
            {
                var langues = _service.GetListeLangue();

                var politiqueTraduction = _service.GetPolitiqueTraduction(idPolitique, idLangue);
                if (politiqueTraduction == null)
                {
                    politiqueTraduction = new PolitiqueTraduction();
                    politiqueTraduction.PolitiqueId = idPolitique;
                }
                politiqueTraductionViewModel.InjectFrom(politiqueTraduction);
                politiqueTraductionViewModel.Id = politiqueTraduction.Id;
                politiqueTraductionViewModel.LangueId = idLangue;
                politiqueTraductionViewModel.ListeLangues = langues;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return PartialView("ContenuPartial", politiqueTraductionViewModel);
        }
    }
}
