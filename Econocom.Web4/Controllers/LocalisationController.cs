using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Econocom.Model.Models.CMS;
using Econocom.Web4.Controllers.ApiControllers;
using NLog;
using System.IO;


namespace Econocom.Web4.Controllers
{
    public class LocalisationController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        //
        // GET: /Localisation/

        
       
        public ActionResult ChangeCulture(string language)
        {
            Logger.Info(" language {0}", language);
            var service = new ServiceApiController();
            var langueChoisi = service.GetLangueByCode(language);

            if (langueChoisi != null)
            {
                Logger.Info(" langueChoisi.Code {0}, langueChoisi.Culture {1}", langueChoisi.Code, langueChoisi.Culture);
                try
                {
                   // Session.Add("LangueChoisi", langueChoisi);
                    var culture = CultureInfo.CreateSpecificCulture(langueChoisi.Culture);

                    Logger.Info(" CurrentCulture {0}", culture.DisplayName);
                    System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
                    if (Request.IsAuthenticated)
                    {                       
                        var email = User.Identity.Name;
                        var contact = service.GetContactParEmail(email);
                        var clientId = contact.Client.Id;

                        // if SecteurActiviteId is null, redirect to input SecteurActiviteId
                        if (contact.Client.SecteurActiviteId == null)
                        {
                            var translatedRoute = TranslatedUrl(langueChoisi);
                            return Redirect(translatedRoute);
                        }
                        else //redirect to consommation index
                        {
                            
                            var urlConso = "";
                            var routageConso = service.GetRoutageByLangueIdAndValeurActuelle(langueChoisi.Id,
                                                                                             "Consommation");
                            if (routageConso != null)
                            {
                                Logger.Info("routageConso.ValeurTraduite {0}, routageConso.ValeurActuelle {1}",
                                            routageConso.ValeurTraduite, routageConso.ValeurActuelle);
                                urlConso = "/" + langueChoisi.Code + "/" + routageConso.ValeurTraduite + "/Index/" +
                                           clientId;
                            }
                            else
                            {
                                Logger.Info(" routageHome null");
                                urlConso = "/" + langueChoisi.Code + "/Consommation/Index/" + clientId;
                            }

                            Logger.Info("url {0} ", urlConso);
                            return Redirect(urlConso);
                        }
                        //return RedirectToRoute(new { controller = "Consommation", action = "Index", id = clientId });
                    }
                    else
                    {
                        var routageHome = service.GetRoutageByLangueIdAndValeurActuelle(langueChoisi.Id, "Home");
                        if (routageHome != null)
                            Logger.Info("routageHome.ValeurTraduite {0}, routageHome.ValeurActuelle {1}",
                                        routageHome.ValeurTraduite, routageHome.ValeurActuelle);
                        else
                        {
                            Logger.Info(" routageHome null");
                        }
                        string url = "/" + langueChoisi.Code + "/" + routageHome.ValeurTraduite + "/";
                        Logger.Info("url {0} ", url);
                        return Redirect(url);
                    }
                    
                }
                catch (Exception e)
                {
                    Logger.Error("langage {0}", language);
                    Logger.Error(e.StackTrace);
                }
            }
            else
            {
                Logger.Info(" langueChoisi null");
            }
            
            return RedirectToAction("Index","Home");
        }

        public String TranslatedUrl(Langue langueChoisi)
        {
            var translatedUrl = "";
            var service = new ServiceApiController();
            if (HttpContext.ApplicationInstance.Request.UrlReferrer != null)
            {
                var fullUrl = HttpContext.ApplicationInstance.Request.UrlReferrer.ToString();
                var query = HttpContext.ApplicationInstance.Request.UrlReferrer.Query;
                var url = fullUrl;

                var request = new HttpRequest(null, url, null);
                var response = new HttpResponse(new StringWriter());
                var httpContext = new HttpContext(request, response);

                var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

                if(routeData != null)
                {
                    var values = routeData.Values;
                    string controllerName = values["controller"].ToString();
                    var controllerRoutage = service.GetRoutageByLangueIdAndValeurActuelle(langueChoisi.Id,
                                                                                          controllerName);
                    var controller = controllerRoutage != null ? controllerRoutage.ValeurTraduite : controllerName;

                    string actionName = values["action"].ToString();
                    var actionRoutage = service.GetRoutageByLangueIdAndValeurActuelle(langueChoisi.Id, actionName);
                    var action = actionRoutage != null ? actionRoutage.ValeurTraduite : actionName;

                    translatedUrl = "/" + langueChoisi.Code + "/" + controller + "/" + action + query;
                }
                else
                {
                    translatedUrl = "/" + langueChoisi.Code + "/Home";
                }
            }
            return translatedUrl;
        }
    }
}
