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
    public class FamilleDeviceTraductionController : Controller
    {
         private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public FamilleDeviceTraductionController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /AgeDeviceTraduction/
        public ActionResult Index()
        {
            try
            {
                var listeFamilleDevice = _service.GetListeFamilleDevice();
                return View(listeFamilleDevice);
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
        public ActionResult ActualiserContenu(FamilleDeviceTraductionViewModel familleDeviceTraductionViewModel)
        {
            try
            {
                var familleDeviceTraduction = new FamilleDeviceTraduction();
                familleDeviceTraduction.InjectFrom(familleDeviceTraductionViewModel);
                familleDeviceTraduction.Id = familleDeviceTraductionViewModel.Id.Value;
                _service.SetFamilleDeviceTraduction(familleDeviceTraduction);
                return Json("<span class=\"succes\">Votre modification a ete prise en compte</span>", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return Json("<span class=\"erreur\">Votre modification a echoue</span>", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifierContenu(int idFamilleDevice, int idLangue)
        {
            var familleDeviceTraductionViewModel = new FamilleDeviceTraductionViewModel();
            try
            {
                var langues = _service.GetListeLangue();

                var familleDeviceTraduction = _service.GetFamilleDeviceTraduction(idFamilleDevice, idLangue);
                if (familleDeviceTraduction == null)
                {
                    familleDeviceTraduction = new FamilleDeviceTraduction();
                    familleDeviceTraduction.FamilleDeviceId = idFamilleDevice;
                }
                familleDeviceTraductionViewModel.InjectFrom(familleDeviceTraduction);
                familleDeviceTraductionViewModel.Id = familleDeviceTraduction.Id;
                familleDeviceTraductionViewModel.LangueId = idLangue;
                familleDeviceTraductionViewModel.ListeLangues = langues;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return PartialView("ContenuPartial", familleDeviceTraductionViewModel);
        }
    }
}
