using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Admin.Controllers.ServiceApi;
using Econocom.Model.Models.CMS;
using Econocom.Resource;
using Omu.Awesome.Mvc;

namespace Econocom.Admin.Controllers
{
    [WhiteSpaceFilter]
    public class BaseController : Controller
    {
        private readonly ServiceApiController _service;
        protected BaseController()
        {
            _service = new ServiceApiController();
        }
      

        public void InitLanguageDropDown()
        {

            try
            {
                //var liste = service.GetListeLangue();
                //ViewBag.Langues = liste.ToList();
                //ViewBag.nombres = liste.Count();

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /*
        public void InitierContentuPublier(string nomDePage, string cultureDeLangue)
        {
            try
            {
                var listeContenuPublier = service.GetListeContenuPublier(nomDePage, cultureDeLangue);

                //var listeContenuPourChaqueCle = listeContenuPublier.Select(item => new KeyValuePair<int, string>(item.CleModele.Id, item.Contenu)).ToList();

                ViewBag.PublishedContents = listeContenuPublier;
                InitLanguageDropDown();
            }
            catch (Exception e)
            {
                throw e;
            }
        }*/

        public void InitierContentu(string lien, string nomDePage, string cultureDeLangue)
        {
            try
            {
                var contenuModere = _service.GetContenuPublier(lien, nomDePage, cultureDeLangue);
                ViewBag.ContenuModere = contenuModere;
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

        public string HandleException(Exception e)
        {
            string message = null;

            if (e.GetType() == typeof (System.Data.ConstraintException))
            {
                message = Traduction.Err_ConstraintException;
            }
            else if (e.GetType() == typeof (System.Data.DeletedRowInaccessibleException))
            {
                message = Traduction.Err_DeletedRowInaccessibleException;
            }
            else if (e.GetType() == typeof (System.Data.DuplicateNameException))
            {
                message = Traduction.Err_DuplicateNameException;
            }
            else if (e.GetType() == typeof (System.Data.EntityException))
            {
                message = Traduction.Err_EntityException;
            }
            else if (e.GetType() == typeof (System.Data.InvalidConstraintException))
            {
                message = Traduction.Err_InvalidConstraintException;
            }
            else if (e.GetType() == typeof (System.Data.InvalidExpressionException))
            {
                message = Traduction.Err_InvalidExpressionException;
            }
            else if (e.GetType() == typeof (System.Data.MissingPrimaryKeyException))
            {
                message = Traduction.Err_MissingPrimaryKeyException;
            }
            else if (e.GetType() == typeof (System.Data.NoNullAllowedException))
            {
                message = Traduction.Err_NoNullAllowedException;
            }
            else if (e is ObjectNotFoundException)
            {
                message = Traduction.Err_ObjectNotFoundException;
            }
            else if (e.GetType() == typeof (System.Data.ReadOnlyException))
            {
                message = Traduction.Err_ReadOnlyException;
            }
            else if (e.GetType() == typeof (System.Data.RowNotInTableException))
            {
                message = Traduction.Err_RowNotInTableException;
            }
            else if (e.GetType() == typeof (System.Data.StrongTypingException))
            {
                message = Traduction.Err_StrongTypingException;
            }
            else if (e.GetType() == typeof (System.Data.UpdateException))
            {
                message = Traduction.Err_UpdateException;
            }
            
            if (message == null && e.InnerException != null)
            {
                message = HandleException(e.InnerException);
            }
              
            return message;
        }
    }
}