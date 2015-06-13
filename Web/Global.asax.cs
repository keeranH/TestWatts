using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Econocom.Data;
using Econocom.Helper.Session;

namespace Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {


        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        
         {
            CultureInfo cultureEN = CultureInfo.GetCultureInfo("en-US");
            CultureInfo cultureNL = CultureInfo.GetCultureInfo("nl-NL");
            CultureInfo cultureFR = CultureInfo.GetCultureInfo("fr-FR");

            DictionaryRouteValueTranslationProvider translationProvider = new DictionaryRouteValueTranslationProvider(
                new List<RouteValueTranslation>
                    {
                        new RouteValueTranslation(cultureEN, "Home", "Home"),
                        new RouteValueTranslation(cultureEN, "Index", "Welcome"),
                        new RouteValueTranslation(cultureNL, "Home", "Thuis"),
                        new RouteValueTranslation(cultureNL, "Index", "Welkom"),
                        new RouteValueTranslation(cultureFR, "Home", "Demarrer"),
                        new RouteValueTranslation(cultureFR, "Index", "Accueil")
                    }
                );

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapTranslatedRoute(
                "TranslatedRoute",
                "{lang}/{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = ""},
                new {controller = translationProvider, action = translationProvider},
                true
                );

            routes.MapRoute(
                "Default", // Route name
                "{culture}/{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index",culture= "fr", id = UrlParameter.Optional } // Parameter defaults
                );
        }

        protected void Application_Start()
        {
            //Database.SetInitializer<EconocomContext>(new DropCreateDatabaseIfModelChanges<EconocomContext>());            

            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            Bootstrapper.Initialise();
        }

    }
}