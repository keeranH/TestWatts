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
    public class AgeDeviceTraductionController : Controller
    {
         private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public AgeDeviceTraductionController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /AgeDeviceTraduction/
        public ActionResult Index()
        {
            try
            {
                var listeAgeDevice = _service.GetListeAgeDevice();
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
        public ActionResult ActualiserContenu(AgeDeviceTraductionViewModel ageDeviceTraductionViewModel)
        {
            try
            {
                var ageDevice = new AgeDeviceTraduction();
                ageDevice.InjectFrom(ageDeviceTraductionViewModel);
                ageDevice.Id = ageDeviceTraductionViewModel.Id.Value;
                _service.SetAgeDeviceTraduction(ageDevice);
                return Json("<span class=\"succes\">Votre modification a ete prise en compte</span>", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return Json("<span class=\"erreur\">Votre modification a echoue</span>", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifierContenu(int idAgeDevice, int idLangue)
        {
            var ageDeviceTraductionViewModel = new AgeDeviceTraductionViewModel();
            try
            {
                var langues = _service.GetListeLangue();

                var ageDeviceTraduction = _service.GetAgeDeviceTraduction(idAgeDevice, idLangue);
                if (ageDeviceTraduction == null)
                {
                    ageDeviceTraduction = new AgeDeviceTraduction();
                    ageDeviceTraduction.AgeDeviceId = idAgeDevice;
                }
                ageDeviceTraductionViewModel.InjectFrom(ageDeviceTraduction);
                ageDeviceTraductionViewModel.Id = ageDeviceTraduction.Id;
                ageDeviceTraductionViewModel.LangueId = idLangue;
                ageDeviceTraductionViewModel.ListeLangues = langues;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return PartialView("ContenuPartial", ageDeviceTraductionViewModel);
        }
    }
}
