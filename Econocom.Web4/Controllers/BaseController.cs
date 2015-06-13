using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Econocom.Helper;
using Econocom.Model.Models.CMS;
using Econocom.Model.ViewModel.CMS;
using Econocom.Web4.Controllers.ApiControllers;
using NLog;
using Omu.Awesome.Mvc;

namespace Econocom.Web4.Controllers
{
    [WhiteSpaceFilter]
    [HandleError]
    public class BaseController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private ServiceApiController service;
        protected BaseController()
        {
            service = new ServiceApiController();
            InitLanguageDropDown();
        }
      

        public void InitLanguageDropDown()
        {

            try
            {
                var liste = service.GetListeLangue();
                ViewBag.Langues = liste.ToList();
                ViewBag.nombres = liste.Count();
                var currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                foreach (var langue in liste)
                {
                    if (currentCulture.Contains(langue.Code))
                        ViewBag.Langue = langue.LienImage;
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);                
            }
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lien"></param>
        /// <param name="nomDePage"></param>
        /// <param name="cultureDeLangue"></param>
        /// <returns></returns>
        public string InitierContenu(string lien, string nomDePage, string cultureDeLangue)
        {
            try
            {
                string culture = cultureDeLangue;
                var currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                if (currentCulture.ToLowerInvariant().Contains("en"))
                {
                   culture ="en";
                }
                var contenuModere = service.GetContenuPublier(lien, nomDePage, culture);

                if (contenuModere != null)
                {
                    contenuModere.Contenu = RemplacerVariable(contenuModere.Contenu);
                    var decoded = System.Net.WebUtility.HtmlDecode(contenuModere.Contenu);
                    contenuModere.Contenu = decoded;
                    ViewBag.ContenuModere = contenuModere;
                    return contenuModere.Contenu;
                }
                
            }
            catch (Exception e)
            {
                LogguerErreur(e);            
            }
            return null;
        }

        public ContenuModereViewModel GetContenu(string lien, string nomDePage, string cultureDeLangue)
        {
            try
            {
                string culture = cultureDeLangue;
                var currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                if (currentCulture.ToLowerInvariant().Equals("en-us"))
                {
                    culture = "en-GB";
                }
                var contenuModere = service.GetContenuPublier(lien, nomDePage, culture);

                if (contenuModere != null)
                {
                    contenuModere.Contenu = RemplacerVariable(contenuModere.Contenu);
                    var decoded = System.Net.WebUtility.HtmlDecode(contenuModere.Contenu);
                    contenuModere.Contenu = decoded;
                    ViewBag.ContenuModere = contenuModere;
                    return contenuModere;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return null;
        }

        
        public string RemplacerVariable(string contenuModere)
        {
            try
            {
                // Define name/value pairs to be replaced.
                var remplacements = new Dictionary<string,string>();
                remplacements.Add(VariableHelper.ServerAddress, System.Configuration.ConfigurationManager.AppSettings["ServerAddress"]);

                // Replace
                foreach (var remplacement in remplacements)
                {
                    contenuModere = contenuModere.Replace(remplacement.Key, remplacement.Value);
                }
                
            }
            catch (Exception e)
            {
                LogguerErreur(e);            
            }
            return contenuModere;
        }


        public string RemplacerEmail(string contenuModere, string commercialEmail, string webMasterEmail)
        {
            try
            {
                // Define name/value pairs to be replaced.
                var remplacements = new Dictionary<string, string>();
                remplacements.Add(VariableHelper.CommercialEmail, commercialEmail);
                remplacements.Add(VariableHelper.WebMasterEmail, webMasterEmail);

                // Replace
                if (contenuModere != null)
                {
                    foreach (var remplacement in remplacements)
                    {
                        contenuModere = contenuModere.Replace(remplacement.Key, remplacement.Value);
                    }
                    ViewBag.ContenuModere = contenuModere;
                }              
            }
            catch (Exception e)
            {
                LogguerErreur(e);            
            }
            return contenuModere;
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

        public void LogguerErreur(Exception e)
        {
            try
            {
                if (e == null)
                {
                    Logger.Error("Exception null");
                }
                else
                {
                    if (e.InnerException != null)
                    {
                        if (e.InnerException.InnerException != null)
                        {
                            Logger.Error(e.InnerException.InnerException.Message);
                        }
                        else
                        {
                            Logger.Error(e.InnerException.Message);
                        }
                    }
                    else
                    {
                        Logger.Error(e.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(e);            
                throw;
            }
        }

        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    if (filterContext.ExceptionHandled)
        //    {
        //        return;
        //    }
        //    //var routeData = new RouteValueDictionary();
        //    var action = "Index";
        //    //routeData.Values["lang"] = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        //    //    routeData.Values["controller"] = "Error";
        //    //    routeData.Values["action"] = action;
        //        filterContext.Result = new RedirectToRouteResult(
        //             new RouteValueDictionary(
        //                 new { action = action, controller = "Error", lang = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName }));
        //    filterContext.ExceptionHandled = true;

        //    //var httpContext = filterContext.HttpContext.ApplicationInstance.Context ;
        //    //// Output a nice error page
        //    //if (filterContext.HttpContext.IsCustomErrorEnabled)
        //    //{
        //    //   // filterContext.ExceptionHandled = true;
        //    //    //this.View("Error").ExecuteResult(this.ControllerContext);

        //    //    var controller = new ErrorController();
        //    //    var routeData = new RouteData();
        //    //    var action = "Index";
        //    //    var ex = filterContext.Exception;
        //    //    if (ex is HttpException)
        //    //    {
        //    //        var httpEx = ex as HttpException;

        //    //        switch (httpEx.GetHttpCode())
        //    //        {
        //    //            case 404:
        //    //                action = "HttpError404";
        //    //                break;

        //    //            // others if any
        //    //        }
        //    //    }

        //    //    httpContext.ClearError();
        //    //    httpContext.Response.Clear();
        //    //    httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;


        //    //    routeData.Values["controller"] = "Error";
        //    //    routeData.Values["action"] = action;

        //    //    //controller.ViewData.Model = new HandleErrorInfo(ex, currentController, currentAction);
        //    //    ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));

        //    //}


        //}
    }
}