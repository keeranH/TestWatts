using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using NLog;

namespace Econocom.Helper.Url
{
    public class UrlHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static bool ReferrerController(Uri referrer, RouteData currentRouteData)
        {
            if (referrer == null)
                return false;

            try
            {
                // Split the url to url + query string
                var fullUrl = referrer.ToString();
                var questionMarkIndex = fullUrl.IndexOf('?');
                string queryString = null;
                string url = fullUrl;
                if (questionMarkIndex != -1) // There is a QueryString
                {
                    url = fullUrl.Substring(0, questionMarkIndex);
                    queryString = fullUrl.Substring(questionMarkIndex + 1);
                }

                // Arranges
                var request = new HttpRequest(null, url, queryString);
                var response = new HttpResponse(new StringWriter());
                var httpContext = new HttpContext(request, response);

                var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

                // Extract the data    
                var values = routeData.Values;
                var referrerController = values["controller"];

                var currentController = currentRouteData.Values["controller"];

                if (referrerController.Equals(currentController))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                Logger.Error("url referrer {0}", referrer.ToString());
                Logger.Error(e.StackTrace);
                throw;
            }
            return false;
        }

        public static String GetLanguageFromUrl(string requestPath)
        {
            string[] values = requestPath.Split('/');
            var existing = new string[]{"en","fr","es", "de", "it"};

            if (values.Any())
            {
                return values.FirstOrDefault(x => (x.Length == 2 && existing.Any(o=>o==x)));
            }
            return null;
        }

        public static CultureInfo GetCultureFromUrl(string requestPath)
        {
            var language = GetLanguageFromUrl(requestPath);
            if (language != null)
            {
                var culture = CultureInfo.CreateSpecificCulture(language);
                    System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
                return culture;
            }
            return new CultureInfo("fr");
        }

    }
}
