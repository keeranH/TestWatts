using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Globalization;

namespace Econocom.Admin
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            customCulture.NumberFormat.NumberDecimalDigits = 4;
            customCulture.NumberFormat.NumberGroupSeparator = " ";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            //NumberFormatInfo nfiConsommation = (NumberFormatInfo)customCulture.NumberFormat.Clone();
            //nfiConsommation.NumberDecimalSeparator = ".";
            //nfiConsommation.NumberDecimalDigits = 4;
            //nfiConsommation.NumberGroupSeparator = " ";

            
            
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            //if (Request.IsAuthenticated)
            //{
            //    //get the username which we previously set in
            //    //forms authentication ticket in our login1_authenticate event
            //    string loggedUser = HttpContext.Current.User.Identity.Name;

            //    //build a custom identity and custom principal object based on this username
            //    CustomIdentitiy identity = new CustomIdentitiy(loggedUser);
            //    CustomPrincipal principal = new CustomPrincipal(identity);

            //    //set the principal to the current context
            //    HttpContext.Current.User = principal;
            //}
        }
    }
}