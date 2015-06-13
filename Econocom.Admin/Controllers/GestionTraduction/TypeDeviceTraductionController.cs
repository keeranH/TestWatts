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
    public class TypeDeviceTraductionController : Controller
    {
         private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public TypeDeviceTraductionController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /AgeDeviceTraduction/
        public ActionResult Index()
        {
            try
            {
                var listeTypeDevice = _service.GetListeTypeDevice();
                return View(listeTypeDevice);
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
        public ActionResult ActualiserContenu(TypeDeviceTraductionViewModel typeDeviceTraductionViewModel)
        {
            try
            {
                var typeDeviceTraduction = new TypeDeviceTraduction();
                typeDeviceTraduction.InjectFrom(typeDeviceTraductionViewModel);
                typeDeviceTraduction.Id = typeDeviceTraductionViewModel.Id.Value;
                _service.SetTypeDeviceTraduction(typeDeviceTraduction);
                return Json("<span class=\"succes\">Votre modification a ete prise en compte</span>", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return Json("<span class=\"erreur\">Votre modification a echoue</span>", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifierContenu(int idTypeDevice, int idLangue)
        {
            var typeDeviceTraductionViewModel = new TypeDeviceTraductionViewModel();
            try
            {
                var langues = _service.GetListeLangue();

                var ageDeviceTraduction = _service.GetTypeDeviceTraduction(idTypeDevice, idLangue);
                if (ageDeviceTraduction == null)
                {
                    ageDeviceTraduction = new TypeDeviceTraduction();
                    ageDeviceTraduction.TypeDeviceId = idTypeDevice;
                }
                typeDeviceTraductionViewModel.InjectFrom(ageDeviceTraduction);
                typeDeviceTraductionViewModel.Id = ageDeviceTraduction.Id;
                typeDeviceTraductionViewModel.LangueId = idLangue;
                typeDeviceTraductionViewModel.ListeLangues = langues;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return PartialView("ContenuPartial", typeDeviceTraductionViewModel);
        }
    }
}
