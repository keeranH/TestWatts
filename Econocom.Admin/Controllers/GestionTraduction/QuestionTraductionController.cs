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
    public class QuestionTraductionController : Controller
    {
         private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public QuestionTraductionController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /AgeDeviceTraduction/
        public ActionResult Index()
        {
            try
            {
                var listeQuestion = _service.GetListeQuestion();
                return View(listeQuestion);
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
        public ActionResult ActualiserContenu(QuestionTraductionViewModel questionTraductionViewModel)
        {
            try
            {
                var questionTraduction = new QuestionTraduction();
                questionTraduction.InjectFrom(questionTraductionViewModel);
                questionTraduction.Id = questionTraductionViewModel.Id.Value;
                _service.SetQuestionTraduction(questionTraduction);
                return Json("<span class=\"succes\">Votre modification a ete prise en compte</span>", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return Json("<span class=\"erreur\">Votre modification a echoue</span>", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifierContenu(int idQuestion, int idLangue)
        {
            var questionTraductionViewModel = new QuestionTraductionViewModel();
            try
            {
                var langues = _service.GetListeLangue();

                var questionTraduction = _service.GetQuestionTraduction(idQuestion, idLangue);
                if (questionTraduction == null)
                {
                    questionTraduction = new QuestionTraduction();
                    questionTraduction.QuestionId = idQuestion;
                }
                questionTraductionViewModel.InjectFrom(questionTraduction);
                questionTraductionViewModel.Id = questionTraduction.Id;
                questionTraductionViewModel.LangueId = idLangue;
                questionTraductionViewModel.ListeLangues = langues;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return PartialView("ContenuPartial", questionTraductionViewModel);
        }
    }
}
