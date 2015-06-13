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
    public class CategorieDeviceTraductionController : Controller
    {
         private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public CategorieDeviceTraductionController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /AgeDeviceTraduction/
        public ActionResult Index()
        {
            try
            {
                var listeCategorieDevice = _service.GetListeCategorieDevice();
                return View(listeCategorieDevice);
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
        public ActionResult ActualiserContenu(CategorieDeviceTraductionViewModel categorieDeviceTraductionViewModel)
        {
            try
            {
                var categorieDeviceTraduction = new CategorieDeviceTraduction();
                categorieDeviceTraduction.InjectFrom(categorieDeviceTraductionViewModel);
                categorieDeviceTraduction.Id = categorieDeviceTraductionViewModel.Id.Value;
                _service.SetCategorieDeviceTraduction(categorieDeviceTraduction);
                return Json("<span class=\"succes\">Votre modification a ete prise en compte</span>", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return Json("<span class=\"erreur\">Votre modification a echoue</span>", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifierContenu(int idCategorieDevice, int idLangue)
        {
            var categorieDeviceTraductionViewModel = new CategorieDeviceTraductionViewModel();
            try
            {
                var langues = _service.GetListeLangue();

                var categorieDeviceTraduction = _service.GetCategorieDeviceTraduction(idCategorieDevice, idLangue);
                if (categorieDeviceTraduction == null)
                {
                    categorieDeviceTraduction = new CategorieDeviceTraduction();
                    categorieDeviceTraduction.CategorieDeviceId = idCategorieDevice;
                }
                categorieDeviceTraductionViewModel.InjectFrom(categorieDeviceTraduction);
                categorieDeviceTraductionViewModel.Id = categorieDeviceTraduction.Id;
                categorieDeviceTraductionViewModel.LangueId = idLangue;
                categorieDeviceTraductionViewModel.ListeLangues = langues;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return PartialView("ContenuPartial", categorieDeviceTraductionViewModel);
        }
    }
}
