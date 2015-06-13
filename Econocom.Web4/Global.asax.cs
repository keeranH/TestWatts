using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Econocom.Web4.Controllers;
using NLog;
using Omu.Awesome.Mvc;
using Econocom.Web4.Controllers.ApiControllers;
using Econocom.Model.Models.CMS;

namespace Econocom.Web4
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            var service= new ServiceApiController();
            
            List<Routage> listeDesRoutages=  service.ListeDesRoutages();

            var listedeRouteValueTranslation= new List<RouteValueTranslation>();
            var translationProvider = new DictionaryRouteValueTranslationProvider(listedeRouteValueTranslation);

            if (listeDesRoutages != null)
            {
                for (int i = 0; i < listeDesRoutages.Count; i++)
                {
                    var langueId = listeDesRoutages[i].LangueId;
                    if (langueId != null)
                    {
                        Langue e = service.GetLangue((int) langueId);
                        
                        CultureInfo cinfo = CultureInfo.CreateSpecificCulture(e.Culture);

                        var rvt = new RouteValueTranslation(cinfo, listeDesRoutages[i].ValeurActuelle,
                                                            listeDesRoutages[i].ValeurTraduite);
                        listedeRouteValueTranslation.Add(rvt);
                    }
                }
            }

            translationProvider=new DictionaryRouteValueTranslationProvider(listedeRouteValueTranslation);

            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("Content/{*pathInfo}");
            routes.IgnoreRoute("uploads/{*pathInfo}");
            routes.IgnoreRoute("{*staticfiles}", new {staticfiles = @".*\.(css|js|gif|jpg)(/.*)?"});

            routes.MapTranslatedRoute(
                "TranslatedRoute",
                "{lang}/{controller}/{action}/{id}",
                new { controller = "Accueil", action = "Index", id = "", lang = "fr" },
                new { controller = translationProvider, action = translationProvider },
                true
                );

            routes.MapRoute(
                "Default", // Route name
                "{lang}/{controller}/{action}/{id}", // URL with parameters
                new { controller = "Accueil", action = "Index", lang = "fr", id = UrlParameter.Optional } // Parameter defaults
                );

            routes.MapRoute(
                "DefaultNoLanguage", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Accueil", action = "Index", id = UrlParameter.Optional } // Parameter defaults
                );

            routes.MapRoute(
                "DefaultCultureOnly", // Route name
                "{lang}", // URL with parameters
                new { controller = "Accueil", action = "Index", lang = "fr", id = UrlParameter.Optional } // Parameter defaults
                );
        }
        
        
        
        
        protected void Application_Start()
        {
            ModelMetadataProviders.Current = new AwesomeModelMetadataProvider();
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
           
            Bootstrapper.Bootstrap();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
           HttpContext.Current.Request.Cookies.Remove(FormsAuthentication.FormsCookieName);
           Session.RemoveAll();

           var culture= Thread.CurrentThread.CurrentCulture;
           var ui =   Thread.CurrentThread.CurrentUICulture;
        }

        protected void Session_End(object sender, EventArgs e)
        {
            HttpContext.Current.Request.Cookies.Remove(FormsAuthentication.FormsCookieName);
            Session.RemoveAll();
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            var httpException = exception as HttpException;
            Response.Clear();
            Server.ClearError();
            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = "Index";
            routeData.Values["exception"] = exception;
            Response.StatusCode = 500;

            routeData.Values["lang"] = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            if (httpException != null)
            {
                Response.StatusCode = httpException.GetHttpCode();
                switch (Response.StatusCode)
                {
                    case 403:
                        routeData.Values["action"] = "HttpError403";
                        break;
                    case 404:
                        routeData.Values["action"] = "HttpError404";
                        if (exception.Message == "File does not exist.")
                        {
                            Logger.Error(string.Format("{0} {1}", exception.Message, HttpContext.Current.Request.Url.ToString()), exception);
                        }
                        break;
                    case 505:
                        routeData.Values["action"] = "HttpError505";
                        break;
                    default:
                        routeData.Values["action"] = "Index";
                        break;
                }
            }
           
            if (exception != null)
            {               
                Logger.Error(exception.StackTrace);
                if (exception.InnerException != null)
                {
                    Logger.Error(exception.InnerException.StackTrace);
                }
            }

            IController errorsController = new ErrorController();
            var rc = new RequestContext(new HttpContextWrapper(Context), routeData);
            errorsController.Execute(rc);
        }
    }
}