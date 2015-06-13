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
    public class ClasseDeviceTraductionController : Controller
    {
         private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public ClasseDeviceTraductionController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /AgeDeviceTraduction/
        public ActionResult Index()
        {
            try
            {
                var listeAgeDevice = _service.GetListeClasseDevice();
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
        public ActionResult ActualiserContenu(ClasseDeviceTraductionViewModel classeDeviceTraductionViewModel)
        {
            try
            {
                var classeDeviceTraduction = new ClasseDeviceTraduction();
                classeDeviceTraduction.InjectFrom(classeDeviceTraductionViewModel);
                classeDeviceTraduction.Id = classeDeviceTraductionViewModel.Id.Value;
                _service.SetClasseDeviceTraduction(classeDeviceTraduction);
                return Json("<span class=\"succes\">Votre modification a ete prise en compte</span>", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return Json("<span class=\"erreur\">Votre modification a echoue</span>", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifierContenu(int idClasseDevice, int idLangue)
        {
            var classeDeviceTraductionViewModel = new ClasseDeviceTraductionViewModel();
            try
            {
                var langues = _service.GetListeLangue();

                var ageDeviceTraduction = _service.GetClasseDeviceTraduction(idClasseDevice, idLangue);
                if (ageDeviceTraduction == null)
                {
                    ageDeviceTraduction = new ClasseDeviceTraduction();
                    ageDeviceTraduction.ClasseDeviceId = idClasseDevice;
                }
                classeDeviceTraductionViewModel.InjectFrom(ageDeviceTraduction);
                classeDeviceTraductionViewModel.Id = ageDeviceTraduction.Id;
                classeDeviceTraductionViewModel.LangueId = idLangue;
                classeDeviceTraductionViewModel.ListeLangues = langues;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return PartialView("ContenuPartial", classeDeviceTraductionViewModel);
        }
    }
}
