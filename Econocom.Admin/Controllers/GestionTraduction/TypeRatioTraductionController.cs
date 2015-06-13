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
    public class TypeRatioTraductionController : Controller
    {
         private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public TypeRatioTraductionController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /AgeDeviceTraduction/
        public ActionResult Index()
        {
            try
            {
                var listeTypeRatio = _service.GetListeTypeRatio();
                return View(listeTypeRatio);
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
        public ActionResult ActualiserContenu(TypeRatioTraductionViewModel typeRatioTraductionViewModel)
        {
            try
            {
                var typeRatioTraduction = new TypeRatioTraduction();
                typeRatioTraduction.InjectFrom(typeRatioTraductionViewModel);
                typeRatioTraduction.Id = typeRatioTraductionViewModel.Id.Value;
                _service.SetTypeRatioTraduction(typeRatioTraduction);
                return Json("<span class=\"succes\">Votre modification a ete prise en compte</span>", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return Json("<span class=\"erreur\">Votre modification a echoue</span>", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifierContenu(int idTypeRatio, int idLangue)
        {
            var typeRatioTraductionViewModel = new TypeRatioTraductionViewModel();
            try
            {
                var langues = _service.GetListeLangue();

                var typeRatioTraduction = _service.GetTypeRatioTraduction(idTypeRatio, idLangue);
                if (typeRatioTraduction == null)
                {
                    typeRatioTraduction = new TypeRatioTraduction();
                    typeRatioTraduction.TypeRatioId = idTypeRatio;
                }
                typeRatioTraductionViewModel.InjectFrom(typeRatioTraduction);
                typeRatioTraductionViewModel.Id = typeRatioTraduction.Id;
                typeRatioTraductionViewModel.LangueId = idLangue;
                typeRatioTraductionViewModel.ListeLangues = langues;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return PartialView("ContenuPartial", typeRatioTraductionViewModel);
        }
    }
}
