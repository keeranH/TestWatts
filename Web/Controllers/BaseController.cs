using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Business.Service;
using Econocom.Data;
using Econocom.Model.Interfaces;
using Econocom.Model.Models.CMS;
using Econocom.Service;

namespace Web.Controllers
{
    public abstract class BaseController : Controller
    {
        private Web.ServiceReference1.IEconocomService _service;
        public Web.ServiceReference1.IEconocomService Service
        {
            get { return _service;  }
            set { _service = value; }
        }

        protected BaseController()
        {
            
        }

        protected BaseController(Web.ServiceReference1.IEconocomService econocomService)
        {
            this._service = econocomService;
        }

        public void InitLanguageDropDown()
        {

            try
            {
                var liste = _service.ListeLangues();
                ViewBag.Langues = liste.ToList();
                ViewBag.nombres = liste.Count();

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void InitierContentuPublier(string nomDePage, string cultureDeLangue)
        {
            try
            {
                var listeContenuPublier = _service.ListeContenuPublier(nomDePage, cultureDeLangue);

                var listeContenuPourChaqueCle = listeContenuPublier.Select(item => new KeyValuePair<int, string>(item.CleModele.IdCleModele, item.Contenu)).ToList();

                ViewBag.PublishedContents = listeContenuPourChaqueCle;
                InitLanguageDropDown();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ContenuModere InitierContentu(string nomDePage)
        {
            try
            {
                var contenuPublier = _service.GetContenuPublier(nomDePage);
                return contenuPublier;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ActionResult Erreur()
        {
            return View("Error");
        }

        public PartialViewResult ErreurPartielle()
        {
            return PartialView();
        }

        protected string RenderPartialViewToString()
        {
            return RenderPartialViewToString(null, null);
        }

        protected string RenderPartialViewToString(string viewName)
        {
            return RenderPartialViewToString(viewName, null);
        }

        protected string RenderPartialViewToString(object model)
        {
            return RenderPartialViewToString(null, model);
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
