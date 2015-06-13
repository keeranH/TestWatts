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
    public class TypeUsageTraductionController : Controller
    {
         private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public TypeUsageTraductionController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /TypeUsageTraduction/
        public ActionResult Index()
        {
            try
            {
                var listeAgeDevice = _service.GetListeTypeUsage();
                return View(listeAgeDevice);
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
        public ActionResult ActualiserContenu(TypeUsageTraductionViewModel typeUsageTraductionViewModel)
        {
            try
            {
                var typeUsageTraduction = new TypeUsageTraduction();
                typeUsageTraduction.InjectFrom(typeUsageTraductionViewModel);
                typeUsageTraduction.Id = typeUsageTraductionViewModel.Id.Value;
                _service.SetTypeUsageTraduction(typeUsageTraduction);
                return Json("<span class=\"succes\">Votre modification a ete prise en compte</span>", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return Json("<span class=\"erreur\">Votre modification a echoue</span>", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifierContenu(int idTypeUsage, int idLangue)
        {
            var typeUsageTraductionViewModel = new TypeUsageTraductionViewModel();
            try
            {
                var langues = _service.GetListeLangue();

                var typeUsageTraduction = _service.GetTypeUsageTraduction(idTypeUsage, idLangue);
                if (typeUsageTraduction == null)
                {
                    typeUsageTraduction = new TypeUsageTraduction();
                    typeUsageTraduction.TypeUsageId = idTypeUsage;
                }
                typeUsageTraductionViewModel.InjectFrom(typeUsageTraduction);
                typeUsageTraductionViewModel.Id = typeUsageTraduction.Id;
                typeUsageTraductionViewModel.LangueId = idLangue;
                typeUsageTraductionViewModel.ListeLangues = langues;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return PartialView("ContenuPartial", typeUsageTraductionViewModel);
        }
    }
}
